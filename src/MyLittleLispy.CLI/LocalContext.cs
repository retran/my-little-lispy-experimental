using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.CLI
{
	public class LocalContext
	{
		private readonly Dictionary<string, dynamic> _locals;

		public LocalContext(Context context, IEnumerable<string> args, IEnumerable<Node> argValues)
		{
			_locals = new Dictionary<string, dynamic>();
			foreach (var pair in args.Zip(argValues, (s, node) => new KeyValuePair<string, Node>(s, node)))
			{
				_locals.Add(pair.Key, pair.Value.Eval(context));
			}
		}

		public dynamic Lookup(string name)
		{
			return _locals[name];
		}
	}
}