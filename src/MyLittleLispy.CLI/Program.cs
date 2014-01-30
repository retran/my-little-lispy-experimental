using System;
using MyLittleLispy.Hosting;
using MyLittleLispy.Runtime;

namespace MyLittleLispy.CLI
{
	internal class Program
	{
        private static readonly ScriptEngine Engine = new ScriptEngine();

		private static void Main(string[] args)
		{
			while (true)
			{
				Console.Write(" > ");
				try
				{
				    Console.WriteLine(" => {0}", Engine.Execute(Console.ReadLine()));
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
	}
}