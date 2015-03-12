using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;
using NUnit.Framework;

namespace CorvusAlba.MyLittleLispy.Tests
{
    [TestFixture]
    public class Eval
    {
        [SetUp]
        public void SetUp()
        {
            _engine = new ScriptEngine();
        }

        private ScriptEngine _engine;

        [Test]
        public void EvalShouldEvaluteQuotedExpression()
        {
            Assert.AreEqual(12, _engine.Evaluate("(eval '(+ 2 (* 5 2)))").To<int>());
            Assert.AreEqual("true", _engine.Evaluate("(eval '(cond ((< 5 10) 'true) (#t 'false)))").To<string>());
            Assert.AreEqual(4, _engine.Evaluate("(eval (cons '+ (cons 2 2)))").To<int>());
            Assert.AreEqual(4, _engine.Evaluate("(eval (list '+ 2 2))").To<int>());

            Assert.AreEqual(Null.Value, _engine.Evaluate("(eval '(define x 10))"));
            Assert.AreEqual(10, _engine.Evaluate("(eval 'x)").To<int>());
        }
    }
}