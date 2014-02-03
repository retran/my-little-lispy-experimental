using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;

namespace MyLittleLispy.Tests
{
    [TestClass]
    public class Builtins
    {
        private ScriptEngine _engine;

        [TestInitialize]
        public void SetUp()
        {
            _engine = new ScriptEngine();
        }

        [TestMethod]
        public void LessEqualShouldEvaluateProperly()
        {
            Assert.AreEqual(true, _engine.Execute("(<= 10 10)").To<bool>());
            Assert.AreEqual(true, _engine.Execute("(<= 5 10)").To<bool>());
            Assert.AreEqual(false, _engine.Execute("(<= 10 5)").To<bool>());
        }

        [TestMethod]
        public void GreaterEqualShouldEvaluateProperly()
        {
            Assert.AreEqual(true, _engine.Execute("(>= 10 10)").To<bool>());
            Assert.AreEqual(true, _engine.Execute("(>= 10 5)").To<bool>());
            Assert.AreEqual(false, _engine.Execute("(>= 5 10)").To<bool>());
        }

        [TestMethod]
        public void XorShouldEvaluateProperly()
        {
            Assert.AreEqual(false, _engine.Execute("(xor #t #t)").To<bool>());
            Assert.AreEqual(true, _engine.Execute("(xor #f #t)").To<bool>());
            Assert.AreEqual(true, _engine.Execute("(xor #t #f)").To<bool>());
            Assert.AreEqual(false, _engine.Execute("(xor #f #f)").To<bool>());
        }

        [TestMethod]
        public void IfWithTrueExpressionReturnsThenClause()
        {
            Assert.AreEqual("then", _engine.Execute("(if #t `then)").To<string>());            
            Assert.AreEqual("then", _engine.Execute("(if #t `then `else)").To<string>());            
        }

        [TestMethod]
        public void IfWithFalseExpressionReturnsThenClause()
        {
            Assert.AreEqual(Null.Value, _engine.Execute("(if #f `then)"));
            Assert.AreEqual("else", _engine.Execute("(if #f `then `else)").To<string>());
        }

    }
}