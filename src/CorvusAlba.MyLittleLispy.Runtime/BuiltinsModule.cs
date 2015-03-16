using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    [Module("builtins")]
    public class BuiltinsModule : IModule
    {
        private readonly IEnumerable<string> _builtins = new[]
	    {
		    "(define (<= x y) (or (< x y) (= x y)))",
		    "(define (>= x y) (or (> x y) (= x y)))",
		    "(define (xor x y) (and (or x y) (not (and x y))))",

                    // TODO revise for special cases
                    "(define (= x y) (eqv? x y))",
                    "(define (eq? x y) (eqv? x y))"
	    };

        public enum Platform
        {
            Windows,
            Linux,
            Mac
        }

        public static Platform GetRunningPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                    {
                        return Platform.Mac;
                    }
                    else
                    {
                        return Platform.Linux;
                    }

                case PlatformID.MacOSX:
                    return Platform.Mac;

                default:
                    return Platform.Windows;
            }
        }
        
        public void Import(Parser parser, Context context)
        {
            context.CurrentFrame.Bind("halt",
                 new Closure(new[] { "code" },
                        new ClrLambdaBody(c =>
                            {
                                throw new HaltException(c.Lookup("code").To<int>());
                            })));

            context.CurrentFrame.Bind("+",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Add(c.Lookup("b")))));

            context.CurrentFrame.Bind("-",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                            {
                                var b = c.Lookup("b");
                                if (b != Null.Value)
                                {
                                    return c.Lookup("a").Substract(b);
                                }
                                return c.Lookup("a").Negate();
                            })));

            context.CurrentFrame.Bind("*",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Multiple(c.Lookup("b")))));

            context.CurrentFrame.Bind("/",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Divide(c.Lookup("b")))));

            context.CurrentFrame.Bind("eqv?",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").EqualWithNull(c.Lookup("b")))));

            context.CurrentFrame.Bind("equal?",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                {
                                    var a = c.Lookup("a");
                                    var b = c.Lookup("b");

                                    var eqvResult = a.EqualWithNull(b);
                                    if (eqvResult.To<bool>())
                                    {
                                        return eqvResult;
                                    }

                                    var aCons = a as Cons;
                                    var bCons = b as Cons;
                                    if (aCons != null && bCons != null)
                                    {
                                        return aCons.EqualRecursive(bCons);
                                    }

                                    return eqvResult;
                                })));
            
            context.CurrentFrame.Bind("<",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Lesser(c.Lookup("b")))));

            context.CurrentFrame.Bind(">",
                 new Closure(new[] { "a", "b" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Greater(c.Lookup("b")))));

            context.CurrentFrame.Bind("not",
                 new Closure(new[] { "a" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Not())));

            context.CurrentFrame.Bind("car",
                 new Closure(new[] { "a" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Car())));

            context.CurrentFrame.Bind("cdr",
                 new Closure(new[] { "a" },
                        new ClrLambdaBody(c =>
                                  c.Lookup("a").Cdr())));

            context.CurrentFrame.Bind("display",
                 new Closure(new[] { "a" },
                        new ClrLambdaBody(c =>
                            {
                                System.Console.WriteLine(c.Lookup("a").ToString());
                                return Null.Value;
                            })));

            context.CurrentFrame.Bind("map",
                          new Closure(new[] { "a", "b" },
                              new ClrLambdaBody(c =>
                                  {
                                      var lambda = (Closure)c.Lookup("a");
                                      var list = c.Lookup("b").To<IEnumerable<Value>>();
                                      return new Cons(list.Select(value =>
                                                   c.Trampoline(c.InvokeClosure(lambda, new[] { value.ToExpression() }))).ToArray());
                                  })));

            context.CurrentFrame.Bind("apply",
                          new Closure(new[] { "a", "b" },
                              new ClrLambdaBody(c =>
                                  {
                                      var lambda = (Closure)c.Lookup("a");
                                      var list = c.Lookup("b").To<IEnumerable<Value>>();
                                      return c.Trampoline(
                                              c.InvokeClosure(lambda,
                                                      list.Select(value => value.ToExpression()).ToArray()));
                                  })));

            context.CurrentFrame.Bind("system-is-windows?", new Bool(GetRunningPlatform() == Platform.Windows));
            context.CurrentFrame.Bind("system-is-linux?", new Bool(GetRunningPlatform() == Platform.Linux));
            context.CurrentFrame.Bind("system-is-macos?", new Bool(GetRunningPlatform() == Platform.Mac));
            context.CurrentFrame.Bind("my-little-lispy-runtime-version", new String(Assembly.GetAssembly().GetName().Version.ToString()));
            
            foreach (var define in _builtins)
            {
                parser.Parse(define).Eval(context);
            }
        }
    }
}