namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class String : Value<string>
    {
        public String(string value)
            : base(value)
        {
        }

        public override Value Equal(Value arg)
        {
            var str = arg as String;
            if (str != null)
            {
                return new Bool(this.ClrValue.Equals(str.GetClrValue()));
            }
            
            return new Bool(false);
        }
        
        public override Node ToExpression()
        {
            return new Constant(this);
        }
    }
}