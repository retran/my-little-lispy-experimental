namespace MyLittleLispy.CLI
{
    public abstract class Node
    {
        public bool Quote = false;
        public dynamic Value;

        public virtual dynamic Eval(Context context)
        {
            return Value;
        }
    }
}