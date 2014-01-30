using System;

namespace MyLittleLispy.Runtime
{
    public abstract class Value
    {
        public virtual Value<T> Cast<T>()
        {
            return (Value<T>)this;
        }

        public virtual T Get<T>()
        {
            return Cast<T>().Get();
        }

        public virtual Value Add(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Substract(Value arg)
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

        public virtual Value And(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Or(Value arg)
        {
            throw new InvalidOperationException();
        }

        public virtual Value Not()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Car()
        {
            throw new InvalidOperationException();
        }

        public virtual Value Cdr()
        {
            throw new InvalidOperationException();
        }
    }
    
    public abstract class Value<T> : Value
    {
        protected readonly T _value;

        protected Value(T value)
        {
            _value = value;
        }

        public T Get()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }   
    }
}