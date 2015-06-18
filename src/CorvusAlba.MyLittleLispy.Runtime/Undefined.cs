using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Undefined : Value
    {
        public static Undefined Value = new Undefined();

        public override string ToString()
        {
            return "#undefined";
        }

        public override T To<T>()
        {
            if (typeof(T) == (typeof(bool)))
            {
                return (T)(object)false;
            }

            return base.To<T>();
        }

        public override Value Equal(Value arg)
        {
            if (object.ReferenceEquals(this, arg))
            {
                return new Bool(true);
            }

            return new Bool(false);
        }

        public override Node ToExpression()
        {
            return new Constant(this);
        }
    }
}