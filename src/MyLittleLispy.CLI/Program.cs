using System.Text;
using System.Threading;
using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;

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