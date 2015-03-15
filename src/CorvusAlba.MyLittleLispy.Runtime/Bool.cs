namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Bool : Value<bool>
    {
        public Bool(bool value)
            : base(value)
        {
        }

        public override string ToString()
        {
            return ClrValue ? "#t" : "#f";
        }

        public override Value Equal(Value arg)
        {
            var boolValue = arg as Bool;
            if (boolValue != null)
            {
                return new Bool(ClrValue == boolValue.GetClrValue());
            }

            return new Bool(false);
        }
    }
}