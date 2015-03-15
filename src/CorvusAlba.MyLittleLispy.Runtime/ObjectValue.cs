namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class ObjectValue : Value<object>
    {
        public ObjectValue(object value) : base(value) { }

        public override Value Equal(Value arg)
        {
            var objectValue = arg as ObjectValue;
            if (objectValue != null)
            {
                return new Bool(object.ReferenceEquals(this.ClrValue, objectValue.GetClrValue()));
            }
            
            return new Bool(false);
        }
    }
}