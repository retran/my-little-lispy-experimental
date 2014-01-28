namespace MyLittleLispy.CLI
{
    public class Bool : Value<bool>
    {
        public Bool(bool value) : base(value) { }

        public override Value And(Value arg)
        {
            return new Bool(_value && arg.Get<bool>());
        }

        public override Value Or(Value arg)
        {
            return new Bool(_value || arg.Get<bool>());
        }

        public override Value Not()
        {
            return new Bool(!_value);
        }

        public override Value Xor(Value arg)
        {
            return new Bool(_value ^ arg.Get<bool>());
        }

        public override Value Equal(Value arg)
        {
            return new Bool(_value == arg.Get<bool>());
        }

        public override Value NotEqual(Value arg)
        {
            return new Bool(_value != arg.Get<bool>());
        }
    }
}