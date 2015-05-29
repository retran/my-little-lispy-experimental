using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;
using Xunit;

namespace CorvusAlba.MyLittleLispy.Tests
{
    public class Eval
    {
        public Eval()
        {
            _engine = new ScriptEngine();
        }

        private ScriptEngine _engine;

        [Fact]
        public void EvalShouldEvaluteQuotedExpression()
        {
            Assert.Equal(12, _engine.Evaluate("(eval '(+ 2 (* 5 2)))").To<int>());
            Assert.Equal("true", _engine.Evaluate("(eval '(cond ((< 5 10) 'true) (#t 'false)))").To<string>());
            Assert.Equal(4, _engine.Evaluate("(eval (cons '+ (cons 2 2)))").To<int>());
            Assert.Equal(4, _engine.Evaluate("(eval (list '+ 2 2))").To<int>());

            Assert.Equal(Null.Value, _engine.Evaluate("(eval '(define x 10))"));
            Assert.Equal(10, _engine.Evaluate("(eval 'x)").To<int>());
        }
    }
}