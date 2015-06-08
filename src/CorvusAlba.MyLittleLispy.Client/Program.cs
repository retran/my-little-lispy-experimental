using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using CorvusAlba.MyLittleLispy.Hosting;

namespace CorvusAlba.MyLittleLispy.Client
{
    internal class Program
    {
        class CommandLineArgs
        {
            public string Script = string.Empty;
            public bool Inspect = false;

            private IEnumerator _enumerator = null;

            public void Parse(string[] args)
            {
                _enumerator = args.GetEnumerator();
                while (_enumerator.MoveNext() && _enumerator.Current != null)
                {
                    Arg(_enumerator.Current.ToString());
                }
            }

            public void Arg(string arg)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg.Equals("-i", StringComparison.OrdinalIgnoreCase)
                    || arg.Equals("--inspect", StringComparison.OrdinalIgnoreCase))
                    {
                        Inspect = true;
                    }
                }
                else
                {
                    Script = arg;
                }
            }
        }

        private static int Main(string[] args)
        {
            var arguments = new CommandLineArgs();
            arguments.Parse(args);
            using (var scriptEngine = new ScriptEngine(55555, false))
            {
                if (!string.IsNullOrEmpty(arguments.Script))
                {
                    using (var stream = new FileStream(arguments.Script, FileMode.Open))
                    {
                        scriptEngine.Execute(stream, true);
                        if (!arguments.Inspect)
                        {
                            return 0;
                        }
                    }
                }
                if (arguments.Inspect || string.IsNullOrWhiteSpace(arguments.Script))
                {
                    var task = new Repl("localhost", 55555).Loop();
                    Task.WaitAll(task);
                    return task.Result;
                }
            }
            throw new InvalidOperationException();
        }
    }
}