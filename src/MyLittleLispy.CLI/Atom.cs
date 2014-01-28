namespace MyLittleLispy.CLI
{
    public abstract class Atom : Node
    {
    }

    public class Symbol : Atom
    {
        private readonly string _value;

        public override dynamic Eval(Context context, bool qoute = false)
        {
            return Quote || qoute ? _value : context.Invoke(_value);
        }

        public Symbol(string value)
        {
            _value = value;
        }
    }

    public class Int : Atom
    {
        private readonly int _value;

        public Int(int value)
        {
            _value = value;
        }

        public override dynamic Eval(Context context, bool qoute = false)
        {
            return _value;
        }
    }

}