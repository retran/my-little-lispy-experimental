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

	public Value Execute(Stream stream)
	{
	    using(var sr = new StreamReader(stream))
	    {
		return Execute(sr.ReadToEnd());
	    }
	}
	
	public Value Execute(string script)
	{	    
	    Value result = Null.Value;	    
	    var count = 0;

	    var sb = new StringBuilder();
	    for (var i = 0; i < script.Length; i++)
	    {
		sb.Append(script[i]);
		if (script[i] == '(')
		{
		    count++;
		}

		if (script[i] == ')')
		{
		    count--;
		    if (count == 0)
		    {
			result = _parser.Parse(sb.ToString()).Eval(_context);
			sb = new StringBuilder(); // TODO как-то можно очистить?
		    }
		}
	    }

	    return result;
	}
    }
}