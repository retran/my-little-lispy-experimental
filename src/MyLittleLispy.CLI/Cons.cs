using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.CLI
{
    public class Cons : Value<Tuple<Value, Value>>
    {
        public Cons(Value head, Value tail) : base(new Tuple<Value, Value>(head, tail)) { }

        public Cons(Value head) : base(new Tuple<Value, Value>(head, Null.Value)) { }

        public Cons(IEnumerable<Value> values) 
            : base(new Tuple<Value, Value>(values.First(), 
                values.Skip(1).Any() 
                ? (Value) new Cons(values.Skip(1)) 
                : Null.Value)) { }

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
            return Cdr() == Null.Value
                ? Car().ToString()
                : string.Format("[{0} . {1}]", Car(), Cdr());
        }
    }
}