using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Expression : Node
    {
	protected readonly IEnumerable<Node> Nodes;

	public Expression(IEnumerable<Node> nodes)
	{
	    Nodes = nodes;
	}

	public Node Head
	{
	    get { return Nodes.First(); }
	}

	public IEnumerable<Node> Tail
	{
	    get { return Nodes.Skip(1); }
	}

	public override Value Eval(Context context)
	{
	    return context.Invoke(Head, Tail);
	}

	public override Value Quote(Context context)
	{
	    return new Cons(Nodes.Select(node => node.Quote(context)).ToArray());
	}
    }
}