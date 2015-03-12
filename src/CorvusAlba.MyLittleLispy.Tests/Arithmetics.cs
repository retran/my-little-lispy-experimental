using CorvusAlba.MyLittleLispy.Hosting;
using NUnit.Framework;

namespace CorvusAlba.MyLittleLispy.Tests
{
    [TestFixture]
    public class Arithmetics
    {
        [SetUp]
        public void SetUp()
        {
            _engine = new ScriptEngine();
        }

        private ScriptEngine _engine;

        [Test]
        public void AddOperationShouldAddIntegersProperly()
        {
            Assert.AreEqual(4, _engine.Evaluate("(+ 2 2)").To<int>());
            Assert.AreEqual(0, _engine.Evaluate("(+ -2 2)").To<int>());
        }

        [Test]
        public void ComplexExpressionWithIntegersShouldEvaluateProperly()
        {
            Assert.AreEqual(6, _engine.Evaluate("(+ 2 (* 2 2))").To<int>());
            Assert.AreEqual(94, _engine.Evaluate("(- (* 10 10) 6)").To<int>());
            Assert.AreEqual(50, _engine.Evaluate("(* (/ 10  5) (* 5 5))").To<int>());
            Assert.AreEqual(37, _engine.Evaluate("(+ (+ (* 10 3) 2) (- 10 (- 20 15)))").To<int>());
        }

        [Test]
        public void DivideOperationShouldDivideIntegersProperly()
        {
            Assert.AreEqual(5, _engine.Evaluate("(/ 10 2)").To<int>());
            Assert.AreEqual(5, _engine.Evaluate("(/ 11 2)").To<int>());
            Assert.AreEqual(0, _engine.Evaluate("(/ 0 2)").To<int>());
        }

        [Test]
        [ExpectedException("System.DivideByZeroException")]
        public void DivideOperationThrowsOnIntegerDivideByZero()
        {
            _engine.Evaluate("(/ 10 0)").To<int>();
        }

        [Test]
        public void IntegerConstShouldEvaluateToItsValue()
        {
            Assert.AreEqual(2, _engine.Evaluate("2").To<int>());
            Assert.AreEqual(-200, _engine.Evaluate("-200").To<int>());
            Assert.AreEqual(0, _engine.Evaluate("0").To<int>());
        }

        [Test]
        public void MultipleOperationShouldMultipleIntegersProperly()
        {
            Assert.AreEqual(50, _engine.Evaluate("(* 10 5)").To<int>());
            Assert.AreEqual(-50, _engine.Evaluate("(* -10 5)").To<int>());
            Assert.AreEqual(0, _engine.Evaluate("(* 0 5)").To<int>());
        }

        [Test]
        public void SubstractOperationShouldSubstractIntegersProperly()
        {
            Assert.AreEqual(4, _engine.Evaluate("(- 10 6)").To<int>());
            Assert.AreEqual(16, _engine.Evaluate("(- 10 -6)").To<int>());
            Assert.AreEqual(-16, _engine.Evaluate("(- -10 6)").To<int>());
        }
    }
}