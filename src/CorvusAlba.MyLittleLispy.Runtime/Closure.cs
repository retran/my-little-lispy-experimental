using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Closure : Value
    {
        public IEnumerable<Scope> Scopes { get; private set; }
        public bool IsTailCall { get; set; }
        public bool HasRestArg { get; private set; }

        public Closure(string[] args, Node body, bool isTailCall = false)
        {
            Args = args;
            Body = body;
            IsTailCall = isTailCall;
            if (Args != null)
            {
                // TODO optimisation
                if (Args.Any(a => a == "."))
                {
                    HasRestArg = true;
                    Args = Args.Where(a => a != ".").ToArray();
                }
            }
            else
            {
                Args = new string[] {};
            }
        }

        public Closure(Context context, Node args, Node body, bool isTailCall = false)
        {
            Body = body;
            Scopes = context.CurrentFrame.Export();
            IsTailCall = isTailCall;
            if (args != null)
            {
                var argsExpression = args as Expression;
                if (argsExpression != null)
                {
                    Args = args.Quote(context).To<IEnumerable<Value>>().Select(v => v.To<string>()).ToArray();
                    // TODO optimisation
                    if (Args.Any(a => a == "."))
                    {
                        HasRestArg = true;
                        Args = Args.Where(a => a != ".").ToArray();
                    }
                }
                else
                {
                    HasRestArg = true;
                    Args = new[] {args.Quote(context).To<string>()};
                }
            }
            else
            {
                Args = new string[] {};
            }
        }

        public string[] Args { get; private set; }

        public Node Body { get; private set; }

        public override Node ToExpression()
        {
            throw new NotImplementedException();
        }
    }
}