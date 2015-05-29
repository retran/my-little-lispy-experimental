using System;
using CorvusAlba.MyLittleLispy.Hosting;
using Xunit;

namespace CorvusAlba.MyLittleLispy.Tests
{
    public class Arithmetics
    {
        private readonly ScriptEngine _engine;

        public Arithmetics()
        {
            _engine = new ScriptEngine();           
        }

        [Theory]
        [InlineData("2", 2)]
        [InlineData("-200", -200)]
        [InlineData("0", 0)]
        [InlineData("2.1", 2.1f)]
        [InlineData("-200.2", -200.2f)]
        [InlineData("0.1", 0.1f)]
        public void NumberConstShouldEvaluateToItsValue<T>(string expression, T expected)
        {
            Assert.Equal(_engine.Evaluate(expression).To<T>(), expected, Utility.GetEqualityComparerFor<T>());
        }

        [Theory]
        [InlineData("(+ 1)", 1)]
        [InlineData("(+ 2 2)", 4)]
        [InlineData("(+ 2 -2)", 0)]
        [InlineData("(+ 0 2)", 2)]
        [InlineData("(+ 1 1 1)", 3)]
        [InlineData("(+ 1 2 3 4 5)", 15)]
        [InlineData("(+ 1.0)", 1f)]
        [InlineData("(+ 2.0 2.0)", 4f)]
        [InlineData("(+ 2.0 -2.0)", 0f)]
        [InlineData("(+ 0.0 2.0)", 2f)]
        [InlineData("(+ 1.0 1.0 1.0)", 3f)]
        [InlineData("(+ 1.0 2.0 3.0 4.0 5.0)", 15f)]
        [InlineData("(+ 1 2.0)", 3f)]
        [InlineData("(+ 1 2.0 7)", 10f)]
        public void AddOperationShouldAddNumbersProperly<T>(string expression, T expected)
        {
            Assert.Equal(_engine.Evaluate(expression).To<T>(), expected, Utility.GetEqualityComparerFor<T>());
        }

        [Theory]
        [InlineData("(- 10 6)", 4)]
        [InlineData("(- 10 -6)", 16)]
        [InlineData("(- -10 6)", -16)]
        [InlineData("(- 0 6)", -6)]
        [InlineData("(- 0 -6)", 6)]
        [InlineData("(- 10.1 6.0)", 4.1f)]
        [InlineData("(- 10.1 -6.0)", 16.1f)]
        [InlineData("(- -10.1 6.0)", -16.1f)]
        [InlineData("(- 0.1 6.0)", -5.9f)]
        [InlineData("(- 0.1 -6.0)", 6.1f)]
        [InlineData("(- 0.1 6)", -5.9f)]
        [InlineData("(- 0.1 -6)", 6.1f)]
        [InlineData("(- 0 6.1)", -6.1f)]
        [InlineData("(- 0 -6.1)", 6.1f)]
        public void SubstractOperationShouldSubstractNumbersProperly<T>(string expression, T expected)
        {
            Assert.Equal(_engine.Evaluate(expression).To<T>(), expected, Utility.GetEqualityComparerFor<T>());
        }

        [Theory]
        [InlineData("(* 1)", 1)]
        [InlineData("(* 2 2)", 4)]
        [InlineData("(* 2 -2)", -4)]
        [InlineData("(* 0 2)", 0)]
        [InlineData("(* 1 1 1)", 1)]
        [InlineData("(* 1 2 3 4 5)", 120)]
        [InlineData("(* 1.0)", 1f)]
        [InlineData("(* 2.0 2.0)", 4f)]
        [InlineData("(* 2.0 -2.0)", -4f)]
        [InlineData("(* 0.0 2.0)", 0f)]
        [InlineData("(* 1.0 1.0 1.0)", 1f)]
        [InlineData("(* 1.0 2.0 3.0 4.0 5.0)", 120f)]
        [InlineData("(* 2 -2.0)", -4f)]
        [InlineData("(* 2.0 -2)", -4f)]
        public void MultipleOperationShouldMultipleNumbersProperly<T>(string expression, T expected)
        {
            Assert.Equal(_engine.Evaluate(expression).To<T>(), expected, Utility.GetEqualityComparerFor<T>());
        }

        [Theory]
        [InlineData("(/ 10 5)", 2)]
        [InlineData("(/ 11 5)", 2)]
        [InlineData("(/ -15 5)", -3)]
        [InlineData("(/ -15 -5)", 3)]
        [InlineData("(/ 0 5)", 0)]
        [InlineData("(/ 0 -5)", 0)]
        [InlineData("(/ 10.0 5.0)", 2f)]
        [InlineData("(/ 11.0 5.0)", 2.2f)]
        [InlineData("(/ -15.0 5.0)", -3f)]
        [InlineData("(/ -15.0 -5.0)", 3f)]
        [InlineData("(/ 0.0 5.0)", 0f)]
        [InlineData("(/ 0.0 -5.0)", 0f)]
        [InlineData("(/ 15 5.0)", 3f)]
        [InlineData("(/ 15.0 5)", 3f)]
        public void DivideOperationShouldDivideNumbersProperly<T>(string expression, T expected)
        {
            Assert.Equal(_engine.Evaluate(expression).To<T>(), expected, Utility.GetEqualityComparerFor<T>());
        }

        [Fact]
        public void DivideOperationShouldThrowExceptionOnNumberDivideByZero()
        {
            Assert.Throws<DivideByZeroException>(() => 
                _engine.Evaluate("(/ 10 0)").To<int>());
        }

        [Theory]
        [InlineData("(+ (+ (* 10 3) 2) (- 10 (- 20 15)))", 37)]
        [InlineData("(* (/ 10.0  5.0) (* 5.0 5.0))", 50f)]
        [InlineData("(* (/ 10.0  5) (* 5 5.0))", 50f)]
        public void ComplexExpressionWithNumbersShouldEvaluateProperly<T>(string expression, T expected)
        {
            Assert.Equal(_engine.Evaluate(expression).To<T>(), expected, Utility.GetEqualityComparerFor<T>());
        }

    }
}