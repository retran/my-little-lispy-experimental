using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MyLittleLispy.Parser;

namespace MyLittleLispy.Tests
{
    [TestClass]
    public class ArithmeticsTestCase
    {
        private Context _context;
        private Parser.Parser _parser;

        [TestInitialize]
        public void SetUp()
        {
            _context = new Context();
            _parser = new Parser.Parser();            
        }

        [TestMethod]
        public void IntegerConstShouldEvaluateToItsValue()
        {
            Assert.AreEqual(2, _parser.Parse("2").Eval(_context).Get<int>());
            Assert.AreEqual(-200, _parser.Parse("-200").Eval(_context).Get<int>());
            Assert.AreEqual(0, _parser.Parse("0").Eval(_context).Get<int>());
        }

        [TestMethod]
        public void AddOperationShouldAddIntegersProperly()
        {
            Assert.AreEqual(4, _parser.Parse("(+ 2 2)").Eval(_context).Get<int>());
            Assert.AreEqual(0, _parser.Parse("(+ -2 2)").Eval(_context).Get<int>());
        }

        [TestMethod]
        public void SubstractOperationShouldSubstractIntegersProperly()
        {
            Assert.AreEqual(4, _parser.Parse("(- 10 6)").Eval(_context).Get<int>());
            Assert.AreEqual(16, _parser.Parse("(- 10 -6)").Eval(_context).Get<int>());
            Assert.AreEqual(-16, _parser.Parse("(- -10 6)").Eval(_context).Get<int>());
        }

        [TestMethod]
        public void MultipleOperationShouldMultipleIntegersProperly()
        {
            Assert.AreEqual(50, _parser.Parse("(* 10 5)").Eval(_context).Get<int>());
            Assert.AreEqual(-50, _parser.Parse("(* -10 5)").Eval(_context).Get<int>());
            Assert.AreEqual(0, _parser.Parse("(* 0 5)").Eval(_context).Get<int>());
        }

        [TestMethod]
        public void DivideOperationShouldDivideIntegersProperly()
        {
            Assert.AreEqual(5, _parser.Parse("(/ 10 2)").Eval(_context).Get<int>());
            Assert.AreEqual(5, _parser.Parse("(/ 11 2)").Eval(_context).Get<int>());
            Assert.AreEqual(0, _parser.Parse("(/ 0 2)").Eval(_context).Get<int>());

            // TODO find way to assert exceptions 
//            Assert.AreEqual(5, _parser.Parse("(/ 10 0)").Eval(_context).Get<int>());
//            Assert.AreEqual(5, _parser.Parse("(/ 0 0)").Eval(_context).Get<int>());
        }

        [TestMethod]
        public void ComplexExpressionWithIntegersShouldCalculateProperly()
        {
            Assert.AreEqual(6, _parser.Parse("(+ 2 (* 2 2))").Eval(_context).Get<int>());
            Assert.AreEqual(94, _parser.Parse("(- (* 10 10) 6)").Eval(_context).Get<int>());
            Assert.AreEqual(50, _parser.Parse("(* (/ 10  5) (* 5 5))").Eval(_context).Get<int>());
            Assert.AreEqual(37, _parser.Parse("(+ (+ (* 10 3) 2) (- 10 (- 20 15)))").Eval(_context).Get<int>());
        }

    }
}
