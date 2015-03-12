namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class ObjectValue : Value<object>
    {
        public ObjectValue(object value) : base(value) { }
    }
}