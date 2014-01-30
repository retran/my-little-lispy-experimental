using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.Runtime
{
    public class Cons : Value<Tuple<Value, Value>>
    {
        public Cons(Value head, Value tail) : base(new Tuple<Value, Value>(head, tail)) { }
            
        public Cons(Value head) : base(new Tuple<Value, Value>(head, Null.Value)) { }

        public Cons(IEnumerable<Value> values) 
            : base(values.Skip(1).Any() 
                ? new Tuple<Value, Value>(values.First(), new Cons(values.Skip(1))) 
                : new Tuple<Value, Value>(values.First(), Null.Value)) { }

        public override Value Car()
        {
            return _value.Item1;
        }

        public override Value Cdr()
        {
            return _value.Item2;
        }

        public override string ToString()
        {
            return string.Format("[{0} . {1}]", Car(), Cdr());
        }

        public override T To<T>()
        {
            if (typeof (T) == typeof (IEnumerable<Value>))
            {
                return (T)Flatten();
            }
            return ((Value) this).To<T>();
        }

        private IEnumerable<Value> Flatten()
        {
            var list = new List<Value>();
            Value current = this;
            do
            {
                list.Add(current.Car());
                current = current.Cdr();
            } while (current is Cons);
            if (current != Null.Value)
            {
                list.Add(current);
            }
            return list;
        }

        public override Node ToExpression()
        {
            return new Expression(this.To<IEnumerable<Value>>().Select(v => v.ToExpression()));
        }
    }
}