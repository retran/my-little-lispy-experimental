namespace MyLittleLispy.Parser
{
    public class Symbol : Node
    {
        public Symbol(String value)
        {
            Value = value;
        }

        public override Value Eval(Context context)
        {
            return context.Invoke(Value.Get<string>());
        }

        public override Value Quote(Context context)
        {
            return Value;
        }
    }
}