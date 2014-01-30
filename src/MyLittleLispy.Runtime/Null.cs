namespace MyLittleLispy.Runtime
{
    public class Null : Value
    {
        public static Null Value = new Null();

        public override string ToString()
        {
            return "null";
        }

        public override Node ToExpression()
        {
            return new Constant(this);
        }
    }
}