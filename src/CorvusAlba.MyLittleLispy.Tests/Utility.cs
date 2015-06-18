using System;
using System.Collections.Generic;
using CorvusAlba.MyLittleLispy.Hosting;
using Xunit;

namespace CorvusAlba.MyLittleLispy.Tests
{
    public static class Utility
    {
        public class FloatEqualityComparer : EqualityComparer<float>
        {
            public readonly float Epsilon = 0.000001f;

            public override bool Equals(float x, float y)
            {
                return Math.Abs(x - y) < Epsilon;
            }

            public override int GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        public static IEqualityComparer<T> GetEqualityComparerFor<T>()
        {
            if (typeof(T) == typeof(Single))
            {
                return (IEqualityComparer<T>)new FloatEqualityComparer();
            }

            return EqualityComparer<T>.Default;
        }

        public static void EvaluateAndAssertEqual<T>(ScriptEngine engine, string expression, T expected)
        {
            Assert.Equal(expected, engine.Evaluate(expression).To<T>(),
                         GetEqualityComparerFor<T>());
        }
    }
}