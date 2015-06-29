using System;
using System.IO;
using System.Text;
using CorvusAlba.MyLittleLispy.Runtime;

namespace CorvusAlba.MyLittleLispy.Hosting
{
    public class ScriptEngine : IDisposable
    {
        private readonly Context _context;
        private readonly Parser _parser;
        private readonly RemoteAgent _agent;

        public bool RemoteEnabled
        {
            get
            {
                return _agent != null;
            }
        }

        public ScriptEngine()
        {
            _parser = new Parser();
            _context = new Context(_parser);
            var builtins = new BuiltinsModule();
            builtins.Import(_parser, _context);
        }

        public ScriptEngine(int port, bool synchronized)
            : this()
        {
            _agent = new RemoteAgent(this, port, synchronized);
            _agent.Start();
        }

        public Value Evaluate(string line)
        {
            return _parser.Parse(line).Eval(_context);
        }

        public Value Execute(Stream stream, bool useGlobalFrame = false)
        {
            using (var sr = new StreamReader(stream))
            {
                return Execute(sr.ReadToEnd(), useGlobalFrame);
            }
        }

        public Value Execute(string script, bool useGlobalFrame = false)
        {
            Value result = Cons.Empty;
            var count = 0;

            if (!useGlobalFrame)
            {
                _context.Push();
                _context.CurrentFrame.Push();
            }

            var sb = new StringBuilder();
            foreach (var t in script)
            {
                sb.Append(t);
                if (t == '(')
                {
                    count++;
                }

                if (t == ')')
                {
                    count--;
                    if (count == 0)
                    {
                        result = _context.Trampoline(_parser.Parse(sb.ToString()).Eval(_context));
                        sb = new StringBuilder(); // TODO как-то можно очистить?
                    }
                }
            }

            if (!useGlobalFrame)
            {
                _context.CurrentFrame.Pop();
                _context.Pop();
            }

            return result;
        }

        public void Sync()
        {
            if (RemoteEnabled)
            {
                _agent.Sync();
            }
        }

        public void Import(IModule module)
        {
            module.Import(_parser, _context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (RemoteEnabled)
                {
                    _agent.Stop();
                }
            }
        }
    }
}
