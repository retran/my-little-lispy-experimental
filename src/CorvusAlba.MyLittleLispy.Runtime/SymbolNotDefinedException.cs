using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class SymbolNotDefinedException : Exception
    {
        private readonly string _symbol;

        public SymbolNotDefinedException(string symbol)
        {
            _symbol = symbol;
        }

        public override string ToString()
        {
            return string.Format("Symbol not defined: {0}", _symbol);
        }
    }
}