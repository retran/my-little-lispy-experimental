using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Continuation : Value
    {
	private IEnumerable<Scope> _scopes;
	private bool _lexicalScopeMode;
	
	public Node Body { get; private set; }

	public Continuation(Context context, Node body, bool lexicalScopeMode = true)
	{
	    _scopes = context.CurrentFrame.Export();
	    Body = body;
	    _lexicalScopeMode = lexicalScopeMode;
	}

	public override Node ToExpression()
	{
	    return Body;
	}

	public Value Call(Context context)
	{
	    if (!_lexicalScopeMode && context.LexicalScopeMode)
	    {
		context.LexicalScopeMode = false;
	    }

	    if (_lexicalScopeMode && !context.LexicalScopeMode)
	    {
		context.LexicalScopeMode = true;
	    }
	    
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
		if (!_lexicalScopeMode && !context.LexicalScopeMode)
		{
		    context.LexicalScopeMode = true;
		}
		
		if (_lexicalScopeMode && !context.LexicalScopeMode)
		{
		    context.LexicalScopeMode = false;
		}
	    }
	}
    }
}
