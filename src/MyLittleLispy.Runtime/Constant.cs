namespace MyLittleLispy.Runtime
{
    public class Constant : Node
    {
        public Constant(Value value)
        {
            Value = value;
        }

        public override Value Eval(Context context)
        {
            return Value;
        }

        public override Value Quote(Context context)
        {
            return Value;
        }
    }
}