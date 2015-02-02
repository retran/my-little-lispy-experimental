using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Integer : Value<int>
    {

	public Integer(int value) : base(value)
	{
	}

	public override Value Add(Value arg)
	{
	    return new Integer(ClrValue + arg.To<int>());
	}

	public override Value Substract(Value arg)
	{
	    return new Integer(ClrValue - arg.To<int>());
	}

	public override Value Multiple(Value arg)
	{
	    return new Integer(ClrValue*arg.To<int>());
	}

	public override Value Divide(Value arg)
	{
	    return new Integer(ClrValue/arg.To<int>());
	}

	public override Value Equal(Value arg)
	{
	    return new Bool(ClrValue == arg.To<int>());
	}

	public override Value Greater(Value arg)
	{
	    return new Bool(ClrValue > arg.To<int>());
	}

	public override Value Lesser(Value arg)
	{
	    return new Bool(ClrValue < arg.To<int>());
	}
    }
}