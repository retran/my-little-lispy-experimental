using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;
using NUnit.Framework;

namespace CorvusAlba.MyLittleLispy.Tests
{
    [TestFixture]
    public class Define
    {
	[SetUp]
	public void SetUp()
	{
	    _engine = new ScriptEngine();
	}

	private ScriptEngine _engine;

	[Test]
	public void DefinedGlobalShouldEvaluateToItsValue()
	{
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define ten 10)"));
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define twenty 20)"));
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define thirty 30)"));

	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define expr (+ ten 15))"));

	    Assert.AreEqual(10, _engine.Evaluate("ten").To<int>());
	    Assert.AreEqual(20, _engine.Evaluate("twenty").To<int>());
	    Assert.AreEqual(30, _engine.Evaluate("thirty").To<int>());
	    Assert.AreEqual(25, _engine.Evaluate("expr").To<int>());
	}

	[Test]
	public void DefinedGlobalShouldEvaluateToItsValueInExpression()
	{
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define ten 10)"));
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define twenty 20)"));
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define thirty 30)"));

	    Assert.IsTrue(_engine.Evaluate("(= thirty (+ ten twenty))").To<bool>());
	    Assert.IsTrue(_engine.Evaluate("(= ten (- thirty twenty))").To<bool>());
	    Assert.IsTrue(_engine.Evaluate("(= 60 (+ thirty (+ ten twenty)))").To<bool>());
	}

	[Test]
	public void RedefinedGlobalShouldEvaluateToItsNewValue()
	{
	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define ten 10)"));
	    Assert.AreEqual(10, _engine.Evaluate("ten").To<int>());

	    Assert.AreEqual(Null.Value, _engine.Evaluate("(define ten 100)"));
	    Assert.AreEqual(100, _engine.Evaluate("ten").To<int>());
	}
    }
}