using System.IO;
using System.Text;
using CorvusAlba.MyLittleLispy.Runtime;

namespace CorvusAlba.MyLittleLispy.Hosting
{
    public class ScriptEngine
    {
        private readonly Context _context;
        private readonly Parser _parser;

        public ScriptEngine()
        {
            _parser = new Parser();
            _context = new Context(_parser);

            var builtins = new BuiltinsModule();
            builtins.Import(_parser, _context);
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
            Value result = Null.Value;
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
    }
}