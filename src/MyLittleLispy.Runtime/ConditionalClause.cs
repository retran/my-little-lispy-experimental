using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.Runtime
{
    class ConditionalClause : Expression
    {
        public ConditionalClause(IEnumerable<Node> nodes) : base(nodes) { }

        public bool IsTrue(Context context)
        {
            return Nodes.First().Eval(context).Get<bool>();
        }

        public bool IsElse(Context context)
        {
            return Nodes.First() is Symbol && Nodes.First().Quote(context).Get<string>() == "else";
        }

        public override Value Eval(Context context)
        {
            return Nodes.Last().Eval(context);
        }

        public override Value Quote(Context context)
        {
            throw new InvalidOperationException();
        }
    }
}