using System.Collections.Generic;

namespace MyLittleLispy.Runtime
{
	public class BuiltinsModule : IModule
	{
		private readonly IEnumerable<string> _builtins = new[]
		{
			"(define (<= x y) (or (< x y) (= x y)))",
			"(define (>= x y) (or (> x y) (= x y)))",
			"(define (xor x y) (and (or x y) (not (and x y))))",
			"(define (if condition then-clause else-clause) (cond ((condition) then-clause) (else else-clause)))"
		};

		public void Import(Parser parser, Context context)
		{
			context.Bind("+", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Add(c.Lookup("b"))))
				);

			context.Bind("-", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Substract(c.Lookup("b"))))
				);

			context.Bind("*", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Multiple(c.Lookup("b"))))
				);

			context.Bind("/", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Divide(c.Lookup("b"))))
				);

			context.Bind("=", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Equal(c.Lookup("b"))))
				);

			context.Bind("<", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Lesser(c.Lookup("b"))))
				);

			context.Bind(">", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Greater(c.Lookup("b"))))
				);

			context.Bind("and", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").And(c.Lookup("b"))))
				);

			context.Bind("or", new Lambda(new[] {"a", "b"}, new ClrLambdaBody(
				c => c.Lookup("a").Or(c.Lookup("b"))))
				);

			context.Bind("not", new Lambda(new[] {"a"}, new ClrLambdaBody(
				c => c.Lookup("a").Not()))
				);

			context.Bind("car", new Lambda(new[] {"a"}, new ClrLambdaBody(
				c => c.Lookup("a").Car()))
				);

			foreach (string define in _builtins)
			{
				parser.Parse(define).Eval(context);
			}
		}
	}
}