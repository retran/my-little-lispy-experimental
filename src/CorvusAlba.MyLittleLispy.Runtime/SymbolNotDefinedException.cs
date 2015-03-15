using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class SymbolNotDefinedException : Exception
    {
        public SymbolNotDefinedException(string symbol)
            : base(string.Format("Symbol not defined: {0}", symbol))
        {
        }

        public override string ToString()
        {
            return string.Format("Exception: {0}", Message);
        }
    }
}
