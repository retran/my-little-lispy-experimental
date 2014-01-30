using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLittleLispy.Parser;

namespace MyLittleLispy.Tests
{
    [TestClass]
    public class Define
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
        public void DefinedGlobalShouldEvaluateToItsValue()
        {
            Assert.AreEqual(Null.Value, _parser.Parse("(define ten 10)").Eval(_context));
            Assert.AreEqual(Null.Value, _parser.Parse("(define twenty 20)").Eval(_context));
            Assert.AreEqual(Null.Value, _parser.Parse("(define thirty 30)").Eval(_context));

            Assert.AreEqual(Null.Value, _parser.Parse("(define expr (+ ten 15))").Eval(_context));

            Assert.AreEqual(10, _parser.Parse("(ten)").Eval(_context).Get<int>());
            Assert.AreEqual(20, _parser.Parse("(twenty)").Eval(_context).Get<int>());
            Assert.AreEqual(30, _parser.Parse("(thirty)").Eval(_context).Get<int>());
            Assert.AreEqual(25, _parser.Parse("(expr)").Eval(_context).Get<int>());

            Assert.AreEqual(10, _parser.Parse("ten").Eval(_context).Get<int>());
            Assert.AreEqual(20, _parser.Parse("twenty").Eval(_context).Get<int>());
            Assert.AreEqual(30, _parser.Parse("thirty").Eval(_context).Get<int>());
            Assert.AreEqual(25, _parser.Parse("(expr)").Eval(_context).Get<int>());        
        }

        [TestMethod]
        public void RedefinedGlobalShouldEvaluateToItsNewValue()
        {
            Assert.AreEqual(Null.Value, _parser.Parse("(define ten 10)").Eval(_context));
            Assert.AreEqual(10, _parser.Parse("(ten)").Eval(_context).Get<int>());
            Assert.AreEqual(10, _parser.Parse("ten").Eval(_context).Get<int>());

            Assert.AreEqual(Null.Value, _parser.Parse("(define ten 100)").Eval(_context));
            Assert.AreEqual(100, _parser.Parse("(ten)").Eval(_context).Get<int>());
            Assert.AreEqual(100, _parser.Parse("ten").Eval(_context).Get<int>());
        }

        [TestMethod]
        public void DefinedGlobalShouldEvaluateToItsValueInExpression()
        {
            Assert.AreEqual(Null.Value, _parser.Parse("(define ten 10)").Eval(_context));
            Assert.AreEqual(Null.Value, _parser.Parse("(define twenty 20)").Eval(_context));
            Assert.AreEqual(Null.Value, _parser.Parse("(define thirty 30)").Eval(_context));
         
            Assert.IsTrue(_parser.Parse("(= thirty (+ ten twenty))").Eval(_context).Get<bool>());
            Assert.IsTrue(_parser.Parse("(= ten (- thirty twenty))").Eval(_context).Get<bool>());
            Assert.IsTrue(_parser.Parse("(= 60 (+ thirty (+ ten twenty)))").Eval(_context).Get<bool>());
        }
    }
}