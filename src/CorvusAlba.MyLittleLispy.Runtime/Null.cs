namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Null : Value
    {
	public static Null Value = new Null();

	public override string ToString()
	{
	    return "()";
	}

	public override Node ToExpression()
	{
	    return new Constant(this);
	}
    }
}