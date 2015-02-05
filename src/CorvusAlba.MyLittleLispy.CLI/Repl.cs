using System;
using System.Diagnostics;
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
		    Stopwatch sw = new Stopwatch();
		    sw.Start();
		    var value = _engine.Evaluate(line);
		    sw.Stop();

		    TimeSpan ts = sw.Elapsed;
		    string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}:{3:000}",
						       ts.Hours, ts.Minutes, ts.Seconds,
						       ts.Milliseconds);

		    Console.WriteLine(" => {0}", value);
		    Console.WriteLine("(elapsed {0})", elapsedTime);
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