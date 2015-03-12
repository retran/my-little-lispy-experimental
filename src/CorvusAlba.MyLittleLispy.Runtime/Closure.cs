using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Closure : Value
    {
        public IEnumerable<Scope> Scopes { get; private set; }
        public bool IsTailCall { get; set; }

        public Closure(string[] args, Node body, bool isTailCall = false)
        {
            Args = args ?? new string[0];
            Body = body;
            IsTailCall = isTailCall;
        }

        public Closure(Context context, Node args, Node body, bool isTailCall = false)
        {
            Args = args != null 
                ? args.Quote(context).To<IEnumerable<Value>>().Select(v => v.To<string>()).ToArray() 
                : new string[0];
            Body = body;
            Scopes = context.CurrentFrame.Export();
            IsTailCall = isTailCall;
        }

        public string[] Args { get; private set; }

        public Node Body { get; private set; }

        public override Node ToExpression()
        {
            throw new NotImplementedException();
        }
    }
}