using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Continuation : Value
    {
	private IEnumerable<Scope> _scopes;
	private bool _callStackEnabled;
	
	public Node Body { get; private set; }

	public Continuation(Context context, Node body, bool callStackEnabled = true)
	{
	    _scopes = context.CurrentFrame.Export();
	    Body = body;
	    _callStackEnabled = callStackEnabled;
	}

	public override Node ToExpression()
	{
	    return Body;
	}

	public Value Call(Context context)
	{
	    context.CallStackEnabled = _callStackEnabled;
	    context.BeginFrame();
	    context.CurrentFrame.Import(_scopes);
	    try
	    {
		return Body.Eval(context);		
	    }
	    finally
	    {
		if (_scopes != null)
		{
		    for (int i = 0; i < _scopes.Count(); i++)
		    {
			context.CurrentFrame.EndScope();
		    }
		}
		context.EndFrame();
		context.CallStackEnabled = !_callStackEnabled;
	    }
	}
    }
}
