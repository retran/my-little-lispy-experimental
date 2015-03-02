using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Cons : Value<Tuple<Value, Value>>
    {
	public Cons(Value head, Value tail) : base(new Tuple<Value, Value>(head, tail))
	{
	}

	public Cons(Value head) : base(new Tuple<Value, Value>(head, Null.Value))
	{
	}

	public Cons(Value[] values)
	    : base(values.Skip(1).Any()
		   ? new Tuple<Value, Value>(values.First(), new Cons(values.Skip(1).ToArray()))
		   : new Tuple<Value, Value>(values.First(), Null.Value))
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
		return string.Format("({0})", string.Join(" ", this.Flatten()));
	    }
	    
	    return string.Format("({0} . {1})", Car(), Cdr());
	}

	public override T To<T>()
	{
	    if (typeof (T) == typeof (IEnumerable<Value>))
	    {
		return (T) Flatten();
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
	    return new Expression(To<IEnumerable<Value>>().Select(v => v.ToExpression()));
	}
    }
}