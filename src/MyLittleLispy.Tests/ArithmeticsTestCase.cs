using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MyLittleLispy.Parser;

namespace MyLittleLispy.Tests
{
    [TestClass]
    public class ArithmeticsTestCase
    {
        [TestMethod]
        public void SimpleIntegerArithmetics()
        {
            var context = new Context();
            var parser = new Parser.Parser();

            Assert.AreEqual(4, parser.Parse("(+ 2 2)").Eval(context).Get<int>());
            Assert.AreEqual(4, parser.Parse("(- 10 6)").Eval(context).Get<int>());
            Assert.AreEqual(50, parser.Parse("(* 10 5)").Eval(context).Get<int>());
            Assert.AreEqual(5, parser.Parse("(/ 10 2)").Eval(context).Get<int>());
        }

        [TestMethod]
        public void ComplexIntegerArithmetics()
        {
            var context = new Context();
            var parser = new Parser.Parser();

            Assert.AreEqual(6, parser.Parse("(+ 2 (* 2 2))").Eval(context).Get<int>());
            Assert.AreEqual(94, parser.Parse("(- (* 10 10) 6)").Eval(context).Get<int>());
            Assert.AreEqual(50, parser.Parse("(* (/ 10  5) (* 5 5))").Eval(context).Get<int>());
            Assert.AreEqual(37, parser.Parse("(+ (+ (* 10 3) 2) (- 10 (- 20 15)))").Eval(context).Get<int>());
        }

    }
}
