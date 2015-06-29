using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;
using Xunit;

namespace CorvusAlba.MyLittleLispy.Tests
{
    public class Builtins
    {
        public Builtins()
        {
            _engine = new ScriptEngine();
        }

        private ScriptEngine _engine;

        [Fact]
        public void GreaterEqualShouldEvaluateProperly()
        {
            Assert.Equal(true, _engine.Evaluate("(>= 10 10)").To<bool>());
            Assert.Equal(true, _engine.Evaluate("(>= 10 5)").To<bool>());
            Assert.Equal(false, _engine.Evaluate("(>= 5 10)").To<bool>());
        }

        [Fact]
        public void IfWithFalseExpressionReturnsElseClause()
        {
            Assert.Equal(Cons.Empty, _engine.Evaluate("(if #f 'then)"));
            Assert.Equal("else", _engine.Evaluate("(if #f 'then 'else)").To<string>());
        }

        [Fact]
        public void IfWithTrueExpressionReturnsThenClause()
        {
            Assert.Equal("then", _engine.Evaluate("(if #t 'then)").To<string>());
            Assert.Equal("then", _engine.Evaluate("(if #t 'then 'else)").To<string>());
        }

        [Fact]
        public void LessEqualShouldEvaluateProperly()
        {
            Assert.Equal(true, _engine.Evaluate("(<= 10 10)").To<bool>());
            Assert.Equal(true, _engine.Evaluate("(<= 5 10)").To<bool>());
            Assert.Equal(false, _engine.Evaluate("(<= 10 5)").To<bool>());
        }
    }
}