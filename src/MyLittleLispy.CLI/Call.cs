namespace MyLittleLispy.CLI
{
    public class Call : Node
    {
        public string Function { get; set; }

        public override dynamic Eval(Context context)
        {
            return context.Invoke(Function, Value);
        }
    }
}