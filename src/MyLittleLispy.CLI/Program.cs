using System;

namespace MyLittleLispy.CLI
{
    internal class Program
    {
        private static readonly Parser Parser = new Parser();
        private static readonly Context Context = new Context();

        private static void Eval(string line)
        {
            Parser.SetLine(line);
            dynamic result = Parser.Parse().Eval(Context);
            if (result != null)
            {
                System.Console.WriteLine(result.ToString());
            }
        }

        private static void Main(string[] args)
        {
            while (true)
            {
                System.Console.Write(" > ");
                try
                {
                    Eval(System.Console.ReadLine());
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
        }
    }
}