namespace MyLittleLispy.CLI
{
	public class Atom : Node
	{
		public override dynamic Eval(Context context)
		{
			if (Value is string && !Quote)
			{
				return context.Lookup(Value);
			}
			return Value;
		}
	}
}