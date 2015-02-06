using System;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class ClrLambdaBody : Node
    {
	private readonly Func<Context, Value> _implementation;

	public ClrLambdaBody(Func<Context, Value> implementation)
	{

	    _implementation = implementation;
	}

	public override Value Eval(Context context)
	{
	    return _implementation(context);
	}

	public override Value Quote(Context context)
	{
	    throw new NotImplementedException();
	}
    }
}