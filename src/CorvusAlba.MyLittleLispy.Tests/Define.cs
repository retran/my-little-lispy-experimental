using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;
using Xunit;

namespace CorvusAlba.MyLittleLispy.Tests
{
    public class Define
    {
        public Define()
        {
            _engine = new ScriptEngine();
        }

        private ScriptEngine _engine;

        [Fact]
        public void DefinedGlobalShouldEvaluateToItsValue()
        {
            Assert.Equal(Null.Value, _engine.Evaluate("(define ten 10)"));
            Assert.Equal(Null.Value, _engine.Evaluate("(define twenty 20)"));
            Assert.Equal(Null.Value, _engine.Evaluate("(define thirty 30)"));

            Assert.Equal(Null.Value, _engine.Evaluate("(define expr (+ ten 15))"));

            Assert.Equal(10, _engine.Evaluate("ten").To<int>());
            Assert.Equal(20, _engine.Evaluate("twenty").To<int>());
            Assert.Equal(30, _engine.Evaluate("thirty").To<int>());
            Assert.Equal(25, _engine.Evaluate("expr").To<int>());
        }

        [Fact]
        public void DefinedGlobalShouldEvaluateToItsValueInExpression()
        {
            Assert.Equal(Null.Value, _engine.Evaluate("(define ten 10)"));
            Assert.Equal(Null.Value, _engine.Evaluate("(define twenty 20)"));
            Assert.Equal(Null.Value, _engine.Evaluate("(define thirty 30)"));

            Assert.True(_engine.Evaluate("(= thirty (+ ten twenty))").To<bool>());
            Assert.True(_engine.Evaluate("(= ten (- thirty twenty))").To<bool>());
            Assert.True(_engine.Evaluate("(= 60 (+ thirty (+ ten twenty)))").To<bool>());
        }

        [Fact]
        public void RedefinedGlobalShouldEvaluateToItsNewValue()
        {
            Assert.Equal(Null.Value, _engine.Evaluate("(define ten 10)"));
            Assert.Equal(10, _engine.Evaluate("ten").To<int>());

            Assert.Equal(Null.Value, _engine.Evaluate("(define ten 100)"));
            Assert.Equal(100, _engine.Evaluate("ten").To<int>());
        }
    }
}