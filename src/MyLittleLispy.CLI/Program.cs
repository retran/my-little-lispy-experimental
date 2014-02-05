using MyLittleLispy.Hosting;

namespace MyLittleLispy.CLI
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			new Repl(new ScriptEngine()).Loop();
		}
	}
}