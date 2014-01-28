namespace MyLittleLispy.CLI
{
    public class Null : Value
    {
        public static Null Value = new Null();

        public override string ToString()
        {
            return "null";
        }
    }
}