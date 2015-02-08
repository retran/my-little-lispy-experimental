using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Continuation : Value
    {
	private IEnumerable<Scope> _frames;
	public Node Body { get; private set; }

	public Continuation(Context context, Node body)
	{
	    _frames = context.CurrentFrame.Export();
	    Body = body;
	}

	public override Node ToExpression()
	{
	    return Body;
	}

	public Value Call(Context context)
	{
	    context.BeginFrame();
	    try
	    {
		context.CurrentFrame.Import(_frames);
		Value result = Body.Eval(context);
		return result;
	    }
	    finally
	    {
		context.EndFrame();
	    }
	}
    }
}
