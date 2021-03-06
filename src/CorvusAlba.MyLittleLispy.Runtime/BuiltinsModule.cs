﻿using System;
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
            context.CurrentFrame.Bind("eval",
                                      new Closure(new[] { "expression" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var expression = c.Lookup("expression").ToExpression();
                                                              var frame = context.CurrentFrame;
                                                              context.Pop();
                                                              var result = expression.Eval(context);
                                                              context.Push(frame);
                                                              return result;
                                                          })));
            
            context.CurrentFrame.Bind("halt",
                                      new Closure(new[] { "code" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              throw new HaltException(c.Lookup("code").To<int>());
                                                          })));

            context.CurrentFrame.Bind("cons",
                                      new Closure(new[] { "a", "b" },
                                                  new ClrLambdaBody(c =>
                                                                    new Cons(c.Lookup("a"), c.Lookup("b")))));
                            
            context.CurrentFrame.Bind("list",
                                      new Closure(new[] { ".", "args" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var args = c.Lookup("args").To<IEnumerable<Value>>().ToArray();
                                                              return new Cons(args);
                                                          })));
                            
            context.CurrentFrame.Bind("+",
                                      new Closure(new[] { ".", "args" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var args = c.Lookup("args").To<IEnumerable<Value>>().ToArray();
                                                              if (args.Length == 0)
                                                              {
                                                                  return new Integer(0);
                                                              }
                                                              var result = args.First();
                                                              foreach (var value in args.Skip(1))
                                                              {
                                                                  result = result.Add(value);
                                                              }
                                                              return result;
                                                          })));

            context.CurrentFrame.Bind("-",
                                      new Closure(new[] { "a", "b" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var b = c.Lookup("b");
                                                              if (!b.IsNull())
                                                              {
                                                                  return c.Lookup("a").Substract(b);
                                                              }
                                                              return c.Lookup("a").Negate();
                                                          })));
            
            context.CurrentFrame.Bind("*",
                                      new Closure(new[] { ".", "args" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var args = c.Lookup("args").To<IEnumerable<Value>>().ToArray();
                                                              if (args.Length == 0)
                                                              {
                                                                  return new Integer(0);
                                                              }
                                                              var result = args.First();
                                                              foreach (var value in args.Skip(1))
                                                              {
                                                                  result = result.Multiple(value);
                                                              }
                                                              return result;
                                                          })));
                                      
            context.CurrentFrame.Bind("/",
                                      new Closure(new[] { "a", "b" },
                                                  new ClrLambdaBody(c =>
                                                                    c.Lookup("a").Divide(c.Lookup("b")))));
            
            context.CurrentFrame.Bind("%",
                                      new Closure(new[] { "a", "b" },
                                                  new ClrLambdaBody(c =>
                                                                    c.Lookup("a").Remainder(c.Lookup("b")))));
            
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
                                                              System.Console.Write(c.Lookup("a").ToString());
                                                              return Cons.Empty;
                                                          })));

            context.CurrentFrame.Bind("display-line",
                                      new Closure(new[] { "a" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              System.Console.WriteLine(c.Lookup("a").ToString());
                                                              return Cons.Empty;
                                                          })));
            
            context.CurrentFrame.Bind("map",
                                      new Closure(new[] { "a", "b" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var lambda = (Closure)c.Lookup("a");
                                                              var list = c.Lookup("b").To<IEnumerable<Value>>();
                                                              return new Cons(list.Select(value =>
                                                                                          c.Trampoline(c.InvokeClosure(lambda, new[] { new Constant(value) }))).ToArray());
                                                          })));

            context.CurrentFrame.Bind("filter",
                          new Closure(new[] { "a", "b" },
                                      new ClrLambdaBody(c =>
                                      {
                                          var lambda = (Closure)c.Lookup("a");
                                          var list = c.Lookup("b").To<IEnumerable<Value>>();
                                          var result = list.Where(value => c.Trampoline(c.InvokeClosure(lambda, new[] { new Constant(value) })).To<bool>()).ToArray();
                                          if (result.Any())
                                          {
                                              return new Cons(result);
                                          }

                                          return Cons.Empty;
                                      })));

            context.CurrentFrame.Bind("build-list",
                                      new Closure(new[] { "n", "proc" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var n = c.Lookup("n").To<int>();
                                                              var list = new Value[n];
                                                              var lambda = (Closure)c.Lookup("proc");
                                                              for (int i = 0; i < n; i++)
                                                              {
                                                                  list[i] = c.Trampoline(c.InvokeClosure(lambda, new Node[] { (new Integer(i)).ToExpression() }));
                                                              }
                                                              return new Cons(list);
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

            context.CurrentFrame.Bind("length",
                          new Closure(new[] { "a" },
                                      new ClrLambdaBody(c => c.Lookup("a").Length())));

            context.CurrentFrame.Bind("list-ref",
                                      new Closure(new[] { "a", "b" },
                                                  new ClrLambdaBody(c => c.Lookup("a").ListRef(c.Lookup("b")))));
            
            context.CurrentFrame.Bind("append",
                                      new Closure(new[] { ".", "args" },
                                                  new ClrLambdaBody(c =>
                                                          {
                                                              var args = c.Lookup("args").To<IEnumerable<Value>>().ToArray();
                                                              if (args.Length == 0)
                                                              {
                                                                  return Cons.Empty;
                                                              }
                                                              var result = args.First();
                                                              foreach (var value in args.Skip(1))
                                                              {
                                                                  result = result.Append(value);
                                                              }
                                                              return result;
                                                          })));

            context.CurrentFrame.Bind("system-is-windows?",
                                      new Bool(GetRunningPlatform() == Platform.Windows));
            context.CurrentFrame.Bind("system-is-linux?",
                                      new Bool(GetRunningPlatform() == Platform.Linux));
            context.CurrentFrame.Bind("system-is-macos?",
                                      new Bool(GetRunningPlatform() == Platform.Mac));
            context.CurrentFrame.Bind("my-little-lispy-runtime-version",
                                      new String(Assembly.GetAssembly(typeof(Context)).GetName().Version.ToString()));
            
            foreach (var define in _builtins)
            {
                parser.Parse(define).Eval(context);
            }
        }
    }
}