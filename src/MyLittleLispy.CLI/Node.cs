namespace MyLittleLispy.CLI
{
	public abstract class Node
	{
		public bool Quote = false;
	    public abstract dynamic Eval(Context context, bool qoute = false);
	}
}