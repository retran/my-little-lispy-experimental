using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;
using NUnit.Framework;

namespace MyLittleLispy.Tests
{
	[TestFixture]
	public class Builtins
	{
		[SetUp]
		public void SetUp()
		{
			_engine = new ScriptEngine();
		}

		private ScriptEngine _engine;

		[Test]
		public void GreaterEqualShouldEvaluateProperly()
		{
			Assert.AreEqual(true, _engine.Execute("(>= 10 10)").To<bool>());
			Assert.AreEqual(true, _engine.Execute("(>= 10 5)").To<bool>());
			Assert.AreEqual(false, _engine.Execute("(>= 5 10)").To<bool>());
		}

		[Test]
		public void IfWithFalseExpressionReturnsThenClause()
		{
			Assert.AreEqual(Null.Value, _engine.Execute("(if #f `then)"));
			Assert.AreEqual("else", _engine.Execute("(if #f `then `else)").To<string>());
		}

		[Test]
		public void IfWithTrueExpressionReturnsThenClause()
		{
			Assert.AreEqual("then", _engine.Execute("(if #t `then)").To<string>());
			Assert.AreEqual("then", _engine.Execute("(if #t `then `else)").To<string>());
		}

		[Test]
		public void LessEqualShouldEvaluateProperly()
		{
			Assert.AreEqual(true, _engine.Execute("(<= 10 10)").To<bool>());
			Assert.AreEqual(true, _engine.Execute("(<= 5 10)").To<bool>());
			Assert.AreEqual(false, _engine.Execute("(<= 10 5)").To<bool>());
		}

		[Test]
		public void XorShouldEvaluateProperly()
		{
			Assert.AreEqual(false, _engine.Execute("(xor #t #t)").To<bool>());
			Assert.AreEqual(true, _engine.Execute("(xor #f #t)").To<bool>());
			Assert.AreEqual(true, _engine.Execute("(xor #t #f)").To<bool>());
			Assert.AreEqual(false, _engine.Execute("(xor #f #f)").To<bool>());
		}
	}
}