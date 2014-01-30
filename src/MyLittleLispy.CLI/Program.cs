﻿using System;

using MyLittleLispy.Runtime;

namespace MyLittleLispy.CLI
{
	internal class Program
	{
		private static readonly Runtime.Parser Parser = new Runtime.Parser();
		private static readonly Context Context = new Context();

		private static dynamic Eval(string line)
		{
			return Parser.Parse(line).Eval(Context);
		}

		private static void Main(string[] args)
		{
			while (true)
			{
				Console.Write(" > ");
				try
				{   
					var value = Eval(Console.ReadLine());
					Console.WriteLine(" => {0}", value.ToString());
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}
	}
}