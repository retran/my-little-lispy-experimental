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
            Assert.Equal(Null.Value, _engine.Evaluate("(if #f 'then)"));
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

        [Fact]
        public void XorShouldEvaluateProperly()
        {
            Assert.Equal(false, _engine.Evaluate("(xor #t #t)").To<bool>());
            Assert.Equal(true, _engine.Evaluate("(xor #f #t)").To<bool>());
            Assert.Equal(true, _engine.Evaluate("(xor #t #f)").To<bool>());
            Assert.Equal(false, _engine.Evaluate("(xor #f #f)").To<bool>());
        }
    }
}