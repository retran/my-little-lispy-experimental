using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;

namespace MyLittleLispy.Tests
{
    [TestClass]
    public class Define
    {
        private ScriptEngine _engine;

        [TestInitialize]
        public void SetUp()
        {
            _engine = new ScriptEngine();
        }

        [TestMethod]
        public void DefinedGlobalShouldEvaluateToItsValue()
        {
            Assert.AreEqual(Null.Value, _engine.Execute("(define ten 10)"));
            Assert.AreEqual(Null.Value, _engine.Execute("(define twenty 20)"));
            Assert.AreEqual(Null.Value, _engine.Execute("(define thirty 30)"));

            Assert.AreEqual(Null.Value, _engine.Execute("(define expr (+ ten 15))"));

            Assert.AreEqual(10, _engine.Execute("(ten)").To<int>());
            Assert.AreEqual(20, _engine.Execute("(twenty)").To<int>());
            Assert.AreEqual(30, _engine.Execute("(thirty)").To<int>());
            Assert.AreEqual(25, _engine.Execute("(expr)").To<int>());

            Assert.AreEqual(10, _engine.Execute("ten").To<int>());
            Assert.AreEqual(20, _engine.Execute("twenty").To<int>());
            Assert.AreEqual(30, _engine.Execute("thirty").To<int>());
            Assert.AreEqual(25, _engine.Execute("(expr)").To<int>());
        }

        [TestMethod]
        public void RedefinedGlobalShouldEvaluateToItsNewValue()
        {
            Assert.AreEqual(Null.Value, _engine.Execute("(define ten 10)"));
            Assert.AreEqual(10, _engine.Execute("(ten)").To<int>());
            Assert.AreEqual(10, _engine.Execute("ten").To<int>());

            Assert.AreEqual(Null.Value, _engine.Execute("(define ten 100)"));
            Assert.AreEqual(100, _engine.Execute("(ten)").To<int>());
            Assert.AreEqual(100, _engine.Execute("ten").To<int>());
        }

        [TestMethod]
        public void DefinedGlobalShouldEvaluateToItsValueInExpression()
        {
            Assert.AreEqual(Null.Value, _engine.Execute("(define ten 10)"));
            Assert.AreEqual(Null.Value, _engine.Execute("(define twenty 20)"));
            Assert.AreEqual(Null.Value, _engine.Execute("(define thirty 30)"));

            Assert.IsTrue(_engine.Execute("(= thirty (+ ten twenty))").To<bool>());
            Assert.IsTrue(_engine.Execute("(= ten (- thirty twenty))").To<bool>());
            Assert.IsTrue(_engine.Execute("(= 60 (+ thirty (+ ten twenty)))").To<bool>());
        }
    }
}