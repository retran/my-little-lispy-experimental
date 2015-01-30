using MyLittleLispy.Runtime;

namespace MyLittleLispy.Hosting
{
    public class ScriptEngine
    {
	private readonly Context _context;
	private readonly Parser _parser;

	public ScriptEngine()
	{
	    _context = new Context();
	    _parser = new Parser();

	    var builtins = new BuiltinsModule();
	    builtins.Import(_parser, _context);
	}

	public Value Execute(string line)
	{
	    return _parser.Parse(line).Eval(_context);
	}
    }
}