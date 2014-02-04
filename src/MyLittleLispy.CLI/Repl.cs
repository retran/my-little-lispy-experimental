using System;
using System.Linq;
using MyLittleLispy.Hosting;

namespace MyLittleLispy.CLI
{
    class Repl
    {
        private readonly ScriptEngine _engine;
        private int _brackets;

        public Repl(ScriptEngine engine)
        {
            _engine = engine;
            _brackets = 0;
        }

        public void Loop()
        {
            while (true)
            {
                Console.Write(" > ");
                try
                {
                    var line = Console.ReadLine();

                    int count = 0;
                    while (true)
                    {
                        count = line.Count(c => c == '(') - line.Count(c => c == ')');
                        if (count == 0)
                        {
                            break;
                        }

                        Console.Write(" ... ");
                        line = line + Console.ReadLine();
                    }
                    Console.WriteLine(" => {0}", _engine.Execute(line));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}