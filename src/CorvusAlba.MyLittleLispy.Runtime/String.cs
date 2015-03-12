using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class String : Value<string>
    {
        public String(string value)
            : base(value)
        {
        }

        public override Node ToExpression()
        {
            return new Symbol(this);
        }
    }
}