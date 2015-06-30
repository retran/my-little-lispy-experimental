using System;
using CorvusAlba.MyLittleLispy.Hosting;
using Xunit;

namespace CorvusAlba.MyLittleLispy.Tests
{
    public class Logic
    {
        private readonly ScriptEngine _engine;

        public Logic()
        {
            _engine = new ScriptEngine();
        }

        [Theory]
        [InlineData("#t", true)]
        [InlineData("#f", false)]
        [InlineData("1", true)]
        [InlineData("1.2", true)]
        [InlineData("\"HELLO\"", true)]
        [InlineData("'(1 2 3)", true)]
        [InlineData("'()", true)]
        public void ConstShouldEvaluateToProperBooleanValue<T>(string expression, T expected)
        {
            Utility.EvaluateAndAssertEqual(_engine, expression, expected);
        }

        [Theory]
        [InlineData("(not #t)", false)]
        [InlineData("(not 1)", false)]
        [InlineData("(not 1.2)", false)]
        [InlineData("(not #f)", true)]
        [InlineData("(not \"HELLO\")", false)]
        [InlineData("(not '())", false)]
        [InlineData("(not '(1 2 3))", false)]
        public void NotInvertsBooleanValue<T>(string expression, T expected)
        {
            Utility.EvaluateAndAssertEqual(_engine, expression, expected);
        }

        [Theory]
        [InlineData("(or #f)", false)]
        [InlineData("(or #f #f)", false)]
        [InlineData("(or #f #f #f)", false)]
        [InlineData("(or #t #f)", true)]
        [InlineData("(or #f #t #f)", true)]
        [InlineData("(or #f 1)", 1)]
        [InlineData("(or #f 1.1)", 1.1f)]
        [InlineData("(or #f \"HELLO\")", "HELLO")]
        public void OrReturnsFirstTrueValueIfExistOrLastFalseOtherwise<T>(string expression,
                                                                          T expected)
        {
            Utility.EvaluateAndAssertEqual(_engine, expression, expected);
        }

        [Theory]
        [InlineData("(and #t)", true)]
        [InlineData("(and #t #t)", true)]
        [InlineData("(and #t #t #t)", true)]
        [InlineData("(and 1 2 3)", 3)]
        [InlineData("(and 1.1 1.2 1.3)", 1.3f)]
        [InlineData("(and \"1\" \"2\" \"3\")", "3")]
        [InlineData("(and #t #f)", false)]
        [InlineData("(and #f #t #f)", false)]
        [InlineData("(and #f 1)", false)]
        [InlineData("(and #f 1.1)", false)]
        [InlineData("(and #f \"HELLO\")", false)]
        public void AndReturnsFirstFalseValueIfExistOrLastTrueOtherwise<T>(string expression,
                                                                           T expected)
        {
            Utility.EvaluateAndAssertEqual(_engine, expression, expected);
        }

        [Theory]
        [InlineData("(xor #t #t)", false)]
        [InlineData("(xor #f #t)", true)]
        [InlineData("(xor #t #f)", true)]
        [InlineData("(xor #f #f)", false)]
        public void XorShouldReturnFalseIfArgumentsEqualOrTrueOtherwise<T>(string expression,
                                                                            T expected)
        {
            Utility.EvaluateAndAssertEqual(_engine, expression, expected);
        }
    }
}
