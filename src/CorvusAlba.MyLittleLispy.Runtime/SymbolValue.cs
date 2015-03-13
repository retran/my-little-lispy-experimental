namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class SymbolValue : Value<string>
    {
        public SymbolValue(string value)
            : base(value)
        {
        }

        public override Node ToExpression()
        {
            return new Symbol(this);
        }
    }
}