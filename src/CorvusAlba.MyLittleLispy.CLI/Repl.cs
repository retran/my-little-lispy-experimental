using System;
using System.Linq;
using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;

namespace CorvusAlba.MyLittleLispy.CLI
{
    internal class Repl
    {
	private readonly ScriptEngine _engine;

	public Repl(ScriptEngine engine)
	{
	    _engine = engine;
	}

	public int Loop()
	{
	    while (true)
	    {
		Console.Write(" > ");
		try
		{
		    var line = Console.ReadLine();

		    while (true)
		    {
			var count = line.Count(c => c == '(') - line.Count(c => c == ')');
			if (count == 0)
			{
			    break;
			}

			Console.Write(" ... ");
			line = line + Console.ReadLine();
		    }
		    Console.WriteLine(" => {0}", _engine.Evaluate(line));
		}
		catch (HaltException e)
		{
		    return e.Code;
		}
		catch (Exception e)
		{
		    Console.WriteLine(e.Message);
		}
	    }
	}
    }
}