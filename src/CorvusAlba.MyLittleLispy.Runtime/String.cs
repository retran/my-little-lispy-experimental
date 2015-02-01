using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class String : Value<string>
    {
	public String(string value) : base(value)
	{
	}

	public override Node ToExpression()
	{
	    return new Symbol(this);
	}
    }

    public class Lambda : Value
    {
	public Lambda(string[] args, Node body)
	{
	    Args = args;
	    Body = body;
	}

	public Lambda(Context context, Node args, Node body)
	{
	    Args = args.Quote(context).To<IEnumerable<Value>>().Select(v => v.To<string>()).ToArray();
	    Body = body;
	}

	public string[] Args { get; private set; }

	public Node Body { get; private set; }

	public override Node ToExpression()
	{
	    throw new NotImplementedException();
	}
    }
}