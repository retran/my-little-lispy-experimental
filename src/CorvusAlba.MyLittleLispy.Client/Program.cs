using System;
using System.IO;
using System.Linq;
using System.Collections;
using CorvusAlba.MyLittleLispy.Runtime;
using CorvusAlba.MyLittleLispy.Hosting;

namespace CorvusAlba.MyLittleLispy.Client
{
    internal class CommandLineArgs
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
    
    internal class Program
    {
        private static int Main(string[] args)
        {
            var arguments = new CommandLineArgs();
            arguments.Parse(args);

            var scriptEngine = new ScriptEngine();
            if (!string.IsNullOrEmpty(arguments.Script))
            {
                using (var stream = new FileStream(arguments.Script, FileMode.Open))
                {
                    try
                    {
                        scriptEngine.Execute(stream, true);
                        if (!arguments.Inspect)
                        {
                            return 0;
                        }
                    }
                    catch (HaltException e)
                    {
                        if (!arguments.Inspect)
                        {                       
                            return e.Code;
                        }
                    }
                }
            }
            
            if (arguments.Inspect || string.IsNullOrWhiteSpace(arguments.Script))
            {
                return new Repl(scriptEngine).Loop();
            }

            throw new InvalidOperationException();
        }
    }
}