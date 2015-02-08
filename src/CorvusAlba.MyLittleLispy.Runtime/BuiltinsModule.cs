﻿using System.Collections.Generic;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class BuiltinsModule : IModule
    {
	private readonly IEnumerable<string> _builtins = new[]
	    {
		"(define (<= x y) (or (< x y) (= x y)))",
		"(define (>= x y) (or (> x y) (= x y)))",
		"(define (xor x y) (and (or x y) (not (and x y))))",
		@"(define (eval-sequence list)
                    (let ((value (eval (car list)))
	                  (tail (cdr list)))
                      (if (= tail '())
	                  value
	                  (eval-sequence tail))))"
	    };

	public void Import(Parser parser, Context context)
	{   	 
	    context.CurrentFrame.Bind("halt",
			 new Lambda(new string[] { "code" },
				    new ClrLambdaBody(c =>
					    {
						throw new HaltException(c.Lookup("code").To<int>());
					    })));

	    context.CurrentFrame.Bind("+",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Add(c.Lookup("b")))));

	    context.CurrentFrame.Bind("-",
			 new Lambda(new[] {"a", "b"},
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
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Multiple(c.Lookup("b")))));

	    context.CurrentFrame.Bind("/",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Divide(c.Lookup("b")))));

	    context.CurrentFrame.Bind("=",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").EqualWithNull(c.Lookup("b")))));

	    context.CurrentFrame.Bind("<",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Lesser(c.Lookup("b")))));

	    context.CurrentFrame.Bind(">",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Greater(c.Lookup("b")))));

	    context.CurrentFrame.Bind("and",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").And(c.Lookup("b")))));

	    context.CurrentFrame.Bind("or",
			 new Lambda(new[] {"a", "b"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Or(c.Lookup("b")))));

	    context.CurrentFrame.Bind("not",
			 new Lambda(new[] {"a"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Not())));

	    context.CurrentFrame.Bind("car",
			 new Lambda(new[] {"a"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Car())));

	    context.CurrentFrame.Bind("cdr",
			 new Lambda(new[] {"a"},
				    new ClrLambdaBody(c =>
						      c.Lookup("a").Cdr())));

	    context.CurrentFrame.Bind("p",
			 new Lambda(new[] {"a"},
				    new ClrLambdaBody(c =>
					    {
						System.Console.WriteLine(c.Lookup("a").ToString());
						return Null.Value;
					    })));

	    foreach (var define in _builtins)
	    {
		parser.Parse(define).Eval(context);
	    }
	}
    }
}