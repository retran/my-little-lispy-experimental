using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.Runtime
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
            Syntax.Assert(Head is Symbol);
            return context.Invoke(Head.Value.To<string>(), Tail);
        }

        public override Value Quote(Context context)
        {
            return new Cons(Nodes.Select(node => node.Quote(context)));
        }
    }
}