using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace MyLittleLispy.CLI
{
	public class LocalContext
	{
		private readonly Dictionary<string, Value> _locals;

		public LocalContext(Context context, IEnumerable<string> args, IEnumerable<Value> values)
		{
			_locals = new Dictionary<string, Value>();
			foreach (var pair in args.Zip(values, (s, value) => new KeyValuePair<string, Value>(s, value)))
			{
				_locals.Add(pair.Key, pair.Value);
			}
		}

		public Value Lookup(string name)
		{
		    Value value = null;
		    if (_locals.TryGetValue(name, out value))
		    {
		        return value;
		    }
			return null;
		}
	}
}