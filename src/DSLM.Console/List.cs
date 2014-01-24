using System;
using System.Collections.Generic;
using System.Linq;

namespace DSLM.Console
{
    public class List : Node
    {
        public override dynamic Eval(Context context)
        {
            if (!Quote)
            {
                Node[] nodes = ((IEnumerable<Node>) Value).ToArray();
                dynamic head = nodes.First().Value;
                if (head is string)
                {
                    if (context.HasDefinition(head.ToString()))
                    {
                        return context.Invoke(head.ToString(), nodes.Skip(1));
                    }
                    throw new Exception(string.Format("Function '{0}' is not defined", head));
                }
                throw new Exception("Syntax error");
            }
            return Value;
        }
    }
}