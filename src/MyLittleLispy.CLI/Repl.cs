using System;
using System.Linq;
using MyLittleLispy.Hosting;

namespace MyLittleLispy.CLI
{
	internal class Repl
	{
		private readonly ScriptEngine _engine;

		public Repl(ScriptEngine engine)
		{
			_engine = engine;
		}

		public void Loop()
		{
			while (true)
			{
				Console.Write(" > ");
				try
				{
					string line = Console.ReadLine();

					while (true)
					{
						int count = line.Count(c => c == '(') - line.Count(c => c == ')');
						if (count == 0)
						{
							break;
						}

						Console.Write(" ... ");
						line = line + Console.ReadLine();
					}
					Console.WriteLine(" => {0}", _engine.Execute(line));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
	}
}