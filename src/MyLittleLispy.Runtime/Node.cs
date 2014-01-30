namespace MyLittleLispy.Runtime
{
	public abstract class Node
	{
	    public Value Value { get; protected set; }

	    public abstract Value Eval(Context context);
        public abstract Value Quote(Context context);
    }
}