using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Cons : Value<Tuple<Value, Value>>
    {
        public static readonly Value Empty = new Cons();

        private Cons()
            : base(null)
        {
        }

        public Cons(Value head, Value tail)
            : base(new Tuple<Value, Value>(head, tail))
        {
        }

        public Cons(Value head)
            : base(new Tuple<Value, Value>(head, Cons.Empty))
        {
        }

        public Cons(Value[] values)
            : base(
                values.Any()
                    ? values.Skip(1).Any()
                        ? new Tuple<Value, Value>(values.First(), new Cons(values.Skip(1).ToArray()))
                        : new Tuple<Value, Value>(values.First(), Cons.Empty)
                    : new Tuple<Value, Value>(Cons.Empty, Cons.Empty))
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
            if (IsNull())
            {
                return "()";
            }

            var left = Car();
            var right = Cdr();

            if (right.IsNull())
            {
                return string.Format("({0})", left);
            }

            if (right is Cons)
            {
                return string.Format("({0})", string.Join(" ", Flatten()));
            }

            return string.Format("({0} . {1})", left, right);
        }

        public override T To<T>()
        {
            if (typeof (T) == typeof (bool))
            {
                return (T) (object) true;
            }

            if (typeof (T) == typeof (string))
            {
                return (T) (object) ToString();
            }

            if (typeof (T) == typeof (IEnumerable<Value>))
            {
                return (T) Flatten();
            }

            throw new InvalidCastException();
        }

        public override Value Equal(Value arg)
        {
            if (arg.IsNull() && IsNull())
            {
                return new Bool(true);
            }

            var cons = arg as Cons;
            if (cons != null)
            {
                return new Bool(ReferenceEquals(this, cons));
            }

            return new Bool(false);
        }

        public override Value Length()
        {
            return new Integer(To<IEnumerable<Value>>().Count());
        }

        public override Value ListRef(Value arg)
        {
            var index = arg as Integer;
            if (index == null)
            {
                throw new InvalidOperationException();
            }

            return To<IEnumerable<Value>>().ElementAt(index.To<int>());
        }

        public override Value Append(Value arg)
        {
            if (arg.IsNull())
            {
                return new Cons(To<IEnumerable<Value>>().ToArray());
            }

            var cons = arg as Cons;
            if (cons == null)
            {
                throw new InvalidOperationException();
            }

            return new Cons(To<IEnumerable<Value>>().Concat(cons.To<IEnumerable<Value>>()).ToArray());
        }

        public Value EqualRecursive(Cons value)
        {
            var leftA = Car();
            var leftB = value.Car();

            var rightA = Cdr();
            var rightB = value.Cdr();

            var left = leftA is Cons && leftB is Cons
                ? ((Cons) leftA).EqualRecursive((Cons) leftB)
                : leftA.EqualWithNull(leftB);

            var right = rightA is Cons && rightB is Cons
                ? ((Cons) rightA).EqualRecursive((Cons) rightB)
                : rightA.EqualWithNull(rightB);

            return new Bool(left.To<bool>() && right.To<bool>());
        }

        private IEnumerable<Value> Flatten()
        {
            var list = new List<Value>();
            if (!IsNull())
            {
                Value current = this;
                while (current is Cons && !current.IsNull())
                {
                    list.Add(current.Car());
                    current = current.Cdr();
                }
                if (!current.IsNull())
                {
                    list.Add(current);
                }
            }
            return list;
        }

        public override Node ToExpression()
        {
            return new Expression(To<IEnumerable<Value>>().Select(v => v.ToExpression()));
        }
    }
}