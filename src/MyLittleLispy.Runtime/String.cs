namespace MyLittleLispy.Runtime
{
    public class String : Value<string>
    {
        public String(string value) : base(value) { }

        public override Node ToExpression()
        {
            return new Symbol(this);
        }
    }

    public class Lambda : Value
    {
        public Lambda(Node args, Node body)
        {
            Args = args;
            Body = body;
        }

        public Node Args { get; private set; }

        public Node Body { get; private set; }

        public override Node ToExpression()
        {
            throw new System.NotImplementedException();
        }
    }
}