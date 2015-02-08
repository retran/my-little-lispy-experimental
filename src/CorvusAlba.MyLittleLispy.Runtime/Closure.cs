using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Closure : Value
    {
	public IEnumerable<Scope> Scopes { get; private set; }
	public bool IsContinuation { get; set; }
	
	public Closure(string[] args, Node body, bool isContinuation = false)
	{
	    if (args != null)
	    {
		Args = args;
	    }
	    else
	    {
		Args = new string[0];
	    }
	    Body = body;
	    IsContinuation = isContinuation;
	}

	public Closure(Context context, Node args, Node body, bool isContinuation = false)
	{
	    if (args != null)
	    {
		Args = args.Quote(context).To<IEnumerable<Value>>().Select(v => v.To<string>()).ToArray();
	    }
	    else
	    {
		Args = new string[0];
	    }
	    Body = body;
	    Scopes = context.CurrentFrame.Export();
	    IsContinuation = isContinuation;
	}

	public string[] Args { get; private set; }

	public Node Body { get; private set; }

	public override Node ToExpression()
	{
	    throw new NotImplementedException();
	}
    }
}