using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Cons : Value<Tuple<Value, Value>>
    {
        public Cons(Value head, Value tail)
            : base(new Tuple<Value, Value>(head, tail))
        {
        }

        public Cons(Value head)
            : base(new Tuple<Value, Value>(head, Null.Value))
        {
        }

        public Cons(Value[] values)
            : base(
                values.Any() 
                    ? values.Skip(1).Any()
                       ? new Tuple<Value, Value>(values.First(), new Cons(values.Skip(1).ToArray()))
                       : new Tuple<Value, Value>(values.First(), Null.Value)
                    : new Tuple<Value, Value>(Null.Value, Null.Value))
        {
        }

        public override Value Car()
        {
            return ClrValue.Item1;
        }

        public override Value Cdr()
        {
            return ClrValue.Item2;
        }

        public override string ToString()
        {
            var left = Car();
            var right = Cdr();

            if (right == Null.Value)
            {
                return string.Format("({0})", left);
            }

            if (right is Cons)
            {
                return string.Format("({0})", string.Join(" ", Flatten()));
            }

            return string.Format("({0} . {1})", Car(), Cdr());
        }

        public override T To<T>()
        {
            if (typeof(T) == typeof(IEnumerable<Value>))
            {
                return (T)Flatten();
            }
            return ((Value)this).To<T>();
        }

        public override Value Equal(Value arg)
        {
            if (arg is Null && this.IsNull())
            {
                return new Bool(true);
            }

            var cons = arg as Cons;
            if (cons != null)
            {
                return new Bool(object.ReferenceEquals(this, cons));
            }

            return new Bool(false);
        }

        public override Value Length()
        {
            return new Integer(To<IEnumerable<Value>>().Count());
        }

        public override Value Append(Value arg)
        {
            if (arg is Null)
            {
                return new Cons(To<IEnumerable<Value>>().ToArray());
            }

            var cons = arg as Cons;
            if (cons == null)            
                throw new InvalidOperationException();

            return new Cons(To<IEnumerable<Value>>().Concat(cons.To<IEnumerable<Value>>()).ToArray());
        }

        public Value EqualRecursive(Cons value)
        {
            var leftA = this.Car();
            var leftB = value.Car();

            var rightA = this.Cdr();
            var rightB = value.Cdr();

            var left = leftA is Cons && leftB is Cons
                ? ((Cons)leftA).EqualRecursive((Cons)leftB)
                : leftA.EqualWithNull(leftB);

            var right = rightA is Cons && rightB is Cons
                ? ((Cons)rightA).EqualRecursive((Cons)rightB)
                : rightA.EqualWithNull(rightB);

            return new Bool(left.To<bool>() && right.To<bool>());
        }

        private IEnumerable<Value> Flatten()
        {
            var list = new List<Value>();
            if (!IsNull())
            {
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
            }
            return list;
        }

        public bool IsNull()
        {
            return Car().Equal(Null.Value).To<bool>()
                && Cdr().Equal(Null.Value).To<bool>();             
        }

        public override Node ToExpression()
        {
            return new Expression(To<IEnumerable<Value>>().Select(v => v.ToExpression()));
        }
    }
}