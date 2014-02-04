using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;

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
            Assert.AreEqual(12, _engine.Execute("(eval '(+ 2 (* 5 2)))").To<int>());
            Assert.AreEqual("true", _engine.Execute("(eval '(cond ((< 5 10) 'true) (#t 'false)))").To<string>());
            Assert.AreEqual(4, _engine.Execute("(eval (cons '+ (cons 2 2)))").To<int>());
            Assert.AreEqual(4, _engine.Execute("(eval (list '+ 2 2))").To<int>());

            Assert.AreEqual(Null.Value, _engine.Execute("(eval '(define x 10))"));
            Assert.AreEqual(10, _engine.Execute("(eval 'x)").To<int>());
        }
    }
}