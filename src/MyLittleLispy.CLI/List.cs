using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.CLI
{
	public class List : Node
	{
		public override dynamic Eval(Context context)
		{
			return ((IEnumerable<Node>) Value).ToArray();
		}
	}
}