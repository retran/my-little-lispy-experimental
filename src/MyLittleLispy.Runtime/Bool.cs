namespace MyLittleLispy.Runtime
{
	public class Bool : Value<bool>
	{
		public Bool(bool value) : base(value)
		{
		}

		public override string ToString()
		{
			return ClrValue ? "#t" : "#f";
		}

		public override Value And(Value arg)
		{
			return new Bool(ClrValue && arg.To<bool>());
		}

		public override Value Or(Value arg)
		{
			return new Bool(ClrValue || arg.To<bool>());
		}

		public override Value Not()
		{
			return new Bool(!ClrValue);
		}

		public override Value Equal(Value arg)
		{
			return new Bool(ClrValue == arg.To<bool>());
		}
	}
}