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

        public override dynamic Eval(Context context, bool qoute = false)
        {
            if (qoute || Quote)
            {
                return _nodes;
            }
            
            Syntax.Assert(Head is Symbol);
            return context.Invoke(Head.Eval(context, true), Tail);
        }
    }
}