namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class SymbolValue : Value<string>
    {
        public SymbolValue(string value)
            : base(value)
        {
        }

        public override Value Equal(Value arg)
        {
            var symbol = arg as SymbolValue;
            if (symbol != null)
            {
                return new Bool(this.ClrValue.Equals(symbol.GetClrValue()));
            }
            
            return new Bool(false);
        }
        
        public override Node ToExpression()
        {
            return new Symbol(this);
        }
    }
}