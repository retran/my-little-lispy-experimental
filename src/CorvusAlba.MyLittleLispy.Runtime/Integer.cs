using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Integer : Value<int>
    {

        public Integer(int value)
            : base(value)
        {
        }

        public override Value Add(Value arg)
        {
            if (arg is Float)
            {
                return new Float(ClrValue + arg.To<float>());
            }
            else if (arg is Integer)
            {
                return new Integer(ClrValue + arg.To<int>());
            }

            throw new InvalidOperationException();
        }

        public override Value Substract(Value arg)
        {
            if (arg is Float)
            {
                return new Float(ClrValue - arg.To<float>());
            }
            else if (arg is Integer)
            {
                return new Integer(ClrValue - arg.To<int>());
            }

            throw new InvalidOperationException();
        }

        public override Value Multiple(Value arg)
        {
            if (arg is Float)
            {
                return new Float(ClrValue * arg.To<float>());
            }
            else if (arg is Integer)
            {
                return new Integer(ClrValue * arg.To<int>());
            }

            throw new InvalidOperationException();
        }

        public override Value Divide(Value arg)
        {
            if (arg is Float)
            {
                return new Float(ClrValue / arg.To<float>());
            }
            else if (arg is Integer)
            {
                return new Integer(ClrValue / arg.To<int>());
            }

            throw new InvalidOperationException();
        }

        public override Value Negate()
        {
            return new Integer(-ClrValue);
        }

        public override Value Equal(Value arg)
        {
            if (arg is Integer)
            {
                return new Bool(ClrValue == arg.To<int>());
            }
            else if (arg is Float)
            {
                return new Bool(Math.Abs(ClrValue - arg.To<float>()) < float.Epsilon);
            }

            return new Bool(false);
        }

        public override Value Greater(Value arg)
        {
            if (arg is Integer)
            {
                return new Bool(ClrValue > arg.To<int>());
            }
            else if (arg is Float)
            {
                return new Bool(ClrValue > arg.To<float>());
            }

            throw new InvalidOperationException();
        }

        public override Value Lesser(Value arg)
        {
            if (arg is Integer)
            {
                return new Bool(ClrValue < arg.To<int>());
            }
            else if (arg is Float)
            {
                return new Bool(ClrValue < arg.To<float>());
            }

            throw new InvalidOperationException();
        }
    }
}