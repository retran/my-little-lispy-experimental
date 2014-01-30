using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLittleLispy.Hosting;

namespace MyLittleLispy.Tests
{
    [TestClass]
    public class Eval
    {
        private ScriptEngine _engine;

        [TestInitialize]
        public void SetUp()
        {
            _engine = new ScriptEngine();
        }

        [TestMethod]
        public void EvalShouldEvaluteQuotedExpression()
        {
            Assert.AreEqual(12, _engine.Execute("(eval `(+ 2 (* 5 2)))").To<int>());
        }
    }
}