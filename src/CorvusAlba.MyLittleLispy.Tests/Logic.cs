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
        public void ConstShouldEvaluateToProperBooleanValue<T>(string expression, T expected)
        {
            Utility.EvaluateAndAssertEqual(_engine, expression, expected);
        }
    }
}