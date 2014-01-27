using System.Runtime.Remoting.Messaging;

namespace MyLittleLispy.CLI
{
    public class Atom : Node
    {
        public override dynamic Eval(Context context)
        {
            if (!(Value is string) || Quote)
            {
                return Value;
            }
    
            return context.Invoke(Value);
        }
    }
}