using MyLittleLispy.Hosting;

namespace MyLittleLispy.CLI
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			return new Repl(new ScriptEngine()).Loop();
		}
	}
}