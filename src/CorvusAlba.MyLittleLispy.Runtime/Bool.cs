namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Bool : Value<bool>
    {
	public Bool(bool value) : base(value)
	{
	}

	public override string ToString()
	{
	    return ClrValue ? "#t" : "#f";
	}

	public override Value Equal(Value arg)
	{
	    return new Bool(ClrValue == arg.To<bool>());
	}
    }
}