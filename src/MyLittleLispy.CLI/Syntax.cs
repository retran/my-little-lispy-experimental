namespace MyLittleLispy.CLI
{
    public static class Syntax
    {
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new SyntaxErrorException();
            }
        }
    }
}