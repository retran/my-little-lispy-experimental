using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Closure : Value
    {
	public IEnumerable<Scope> Frames { get; private set; }
	
	public Closure(string[] args, Node body)
	{
	    Args = args;
	    Body = body;
	}

	public Closure(Context context, Node args, Node body)
	{
	    Args = args.Quote(context).To<IEnumerable<Value>>().Select(v => v.To<string>()).ToArray();
	    Body = body;
	    Frames = context.CurrentFrame.Export();
	}

	public string[] Args { get; private set; }

	public Node Body { get; private set; }

	public override Node ToExpression()
	{
	    throw new NotImplementedException();
	}
    }
}