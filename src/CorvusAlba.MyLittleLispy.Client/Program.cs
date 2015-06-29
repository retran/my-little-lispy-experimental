using System;
using System.Collections;
using System.IO;
using System.Reflection;
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
            public string Host = "localhost";
            public int Port = 55555;
            public bool Remote = false;

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
                    else if (arg.Equals("-r", StringComparison.OrdinalIgnoreCase)
                             || arg.Equals("--remote", StringComparison.OrdinalIgnoreCase))
                    {
                        Remote = true;
                    }
                    else if (arg.Equals("-h", StringComparison.OrdinalIgnoreCase)
                             || arg.Equals("--host", StringComparison.OrdinalIgnoreCase))
                    {
                        _enumerator.MoveNext();
                        Host = _enumerator.Current.ToString();
                    }
                    else if (arg.Equals("-p", StringComparison.OrdinalIgnoreCase)
                             || arg.Equals("--port", StringComparison.OrdinalIgnoreCase))
                    {
                        _enumerator.MoveNext();
                        Port = int.Parse(_enumerator.Current.ToString());
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
            Console.WriteLine("My Little Lispy {0}\nCopyright (C) 2014-2015 Corvus Alba\n",
                              Assembly.GetAssembly(typeof(Repl)).GetName().Version);
            var arguments = new CommandLineArgs();
            arguments.Parse(args);
            if (!arguments.Remote)
            {
                using (var scriptEngine = new ScriptEngine(arguments.Port, true))
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
                        var task = new Repl("localhost", arguments.Port, arguments.Remote)
                            .Loop();
                        Task.WaitAll(task);
                        return task.Result;
                    }
                }
            }
            else
            {
                var task = new Repl(arguments.Host, arguments.Port, arguments.Remote).Loop();
                Task.WaitAll(task);
                return task.Result;
            }
            throw new InvalidOperationException();
        }
    }
}