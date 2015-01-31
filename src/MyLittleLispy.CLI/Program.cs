using System.IO;
using System.Linq;
using MyLittleLispy.Runtime;
using MyLittleLispy.Hosting;

namespace MyLittleLispy.CLI
{
    internal class Program
    {
	private static int Main(string[] args)
	{
	    if (!args.Any())
	    {
		return new Repl(new ScriptEngine()).Loop();
	    }
	    else
	    {
		using (var stream = new FileStream(args[0], FileMode.Open))
		{
		    try
		    {
			(new ScriptEngine()).Execute(stream);
			return 0;
		    }
		    catch (HaltException e)
		    {
			return e.Code;
		    }
		}
	    }
	}
    }
}