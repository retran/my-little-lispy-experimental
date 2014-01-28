using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.CLI
{
    public class Expression : Node
    {
        private readonly IEnumerable<Node> _nodes;

        public Expression(IEnumerable<Node> nodes)
        {
            _nodes = nodes;
        }

        public Node Head
        {
            get { return _nodes.First(); }
        }

        public IEnumerable<Node> Tail
        {
            get { return _nodes.Skip(1); }
        }

        public override Value Eval(Context context)
        {          
            Syntax.Assert(Head is Symbol);
            return context.Invoke(Head.Value.Get<string>(), Tail);
        }

        public override Value Quote(Context context)
        {
            return new List(_nodes.Select(node => node.Quote(context)));
        }
    }
}