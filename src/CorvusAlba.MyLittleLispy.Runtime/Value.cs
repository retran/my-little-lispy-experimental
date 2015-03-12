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

        public Value EqualWithNull(Value arg)
        {
            if (object.ReferenceEquals(arg, Null.Value))
            {
                return new Bool(object.ReferenceEquals(this, Null.Value));
            }

            return Equal(arg);
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
            return new Bool(!this.To<bool>());
        }

        public virtual Value Car()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Cdr()
        {
            throw new InvalidOperationException();
        }

        public abstract Node ToExpression();
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
    }

    public class ObjectValue : Value<object>
    {
        public ObjectValue(object value) : base(value) { }
    }
}