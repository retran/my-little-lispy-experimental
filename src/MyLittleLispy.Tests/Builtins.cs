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
	    Assert.AreEqual(true, _engine.Evaluate("(>= 10 10)").To<bool>());
	    Assert.AreEqual(true, _engine.Evaluate("(>= 10 5)").To<bool>());
	    Assert.AreEqual(false, _engine.Evaluate("(>= 5 10)").To<bool>());
	}

	[Test]
	public void IfWithFalseExpressionReturnsElseClause()
	{
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(if #f 'then)"));
	    Assert.AreEqual("else", _engine.Evaluate("(if #f 'then 'else)").To<string>());
	}

	[Test]
	public void IfWithTrueExpressionReturnsThenClause()
	{
	    Assert.AreEqual("then", _engine.Evaluate("(if #t 'then)").To<string>());
	    Assert.AreEqual("then", _engine.Evaluate("(if #t 'then 'else)").To<string>());
	}

	[Test]
	public void LessEqualShouldEvaluateProperly()
	{
	    Assert.AreEqual(true, _engine.Evaluate("(<= 10 10)").To<bool>());
	    Assert.AreEqual(true, _engine.Evaluate("(<= 5 10)").To<bool>());
	    Assert.AreEqual(false, _engine.Evaluate("(<= 10 5)").To<bool>());
	}

	[Test]
	public void XorShouldEvaluateProperly()
	{
	    Assert.AreEqual(false, _engine.Evaluate("(xor #t #t)").To<bool>());
	    Assert.AreEqual(true, _engine.Evaluate("(xor #f #t)").To<bool>());
	    Assert.AreEqual(true, _engine.Evaluate("(xor #t #f)").To<bool>());
	    Assert.AreEqual(false, _engine.Evaluate("(xor #f #f)").To<bool>());
	}
    }
}