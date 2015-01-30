using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;
using NUnit.Framework;

namespace MyLittleLispy.Tests
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
	    Assert.AreEqual(Null.Value, _engine.Execute("(define ten 10)"));
	    Assert.AreEqual(Null.Value, _engine.Execute("(define twenty 20)"));
	    Assert.AreEqual(Null.Value, _engine.Execute("(define thirty 30)"));

	    Assert.AreEqual(Null.Value, _engine.Execute("(define expr (+ ten 15))"));

	    Assert.AreEqual(10, _engine.Execute("ten").To<int>());
	    Assert.AreEqual(20, _engine.Execute("twenty").To<int>());
	    Assert.AreEqual(30, _engine.Execute("thirty").To<int>());
	    Assert.AreEqual(25, _engine.Execute("expr").To<int>());
	}

	[Test]
	public void DefinedGlobalShouldEvaluateToItsValueInExpression()
	{
	    Assert.AreEqual(Null.Value, _engine.Execute("(define ten 10)"));
	    Assert.AreEqual(Null.Value, _engine.Execute("(define twenty 20)"));
	    Assert.AreEqual(Null.Value, _engine.Execute("(define thirty 30)"));

	    Assert.IsTrue(_engine.Execute("(= thirty (+ ten twenty))").To<bool>());
	    Assert.IsTrue(_engine.Execute("(= ten (- thirty twenty))").To<bool>());
	    Assert.IsTrue(_engine.Execute("(= 60 (+ thirty (+ ten twenty)))").To<bool>());
	}

	[Test]
	public void RedefinedGlobalShouldEvaluateToItsNewValue()
	{
	    Assert.AreEqual(Null.Value, _engine.Execute("(define ten 10)"));
	    Assert.AreEqual(10, _engine.Execute("ten").To<int>());

	    Assert.AreEqual(Null.Value, _engine.Execute("(define ten 100)"));
	    Assert.AreEqual(100, _engine.Execute("ten").To<int>());
	}
    }
}