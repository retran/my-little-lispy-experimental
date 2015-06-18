using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public abstract class Value
    {
        public virtual Value<T> Cast<T>()
        {
            if (typeof(T) == typeof(bool) && !(this is Bool))
            {
                return (Value<T>)(object)new Bool(true);
            }

            return (Value<T>)this;
        }

        public virtual T To<T>()
        {
            return Cast<T>().GetClrValue();
        }

        public virtual Value Add(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Substract(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Negate()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Multiple(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Divide(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Remainder(Value arg)
        {
            throw new InvalidOperationException();
        }

        public Value EqualWithNull(Value arg)
        {
            return ReferenceEquals(arg, Cons.Empty)
                ? new Bool(ReferenceEquals(this, Cons.Empty))
                : Equal(arg);
        }

        public virtual Value Equal(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Lesser(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Greater(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Not()
        {
            return new Bool(!To<bool>());
        }

        public virtual Value Car()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Cdr()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Length()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Append(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value ListRef(Value arg)
        {
            throw new InvalidOperationException();
        }
        
        public abstract Node ToExpression();

        public virtual bool IsNull()
        {
            return false;
        }
    }

    public abstract class Value<T> : Value
    {
        protected T ClrValue;

        protected Value(T value)
        {
            ClrValue = value;
        }

        public T GetClrValue()
        {
            return ClrValue;
        }

        public override string ToString()
        {
            return ClrValue.ToString();
        }

        public override Node ToExpression()
        {
            return new Constant(this);
        }

        public override bool IsNull()
        {
            return ClrValue == null;
        }
    }
}