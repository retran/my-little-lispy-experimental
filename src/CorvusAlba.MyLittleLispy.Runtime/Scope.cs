using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Scope
    {
	private readonly Dictionary<string, Value> _locals;

	public Scope(IEnumerable<string> args, IEnumerable<Value> values)
	{
	    _locals = new Dictionary<string, Value>();
	    foreach (var pair in args.Zip(values, (s, value) => new KeyValuePair<string, Value>(s, value)))
	    {
		_locals.Add(pair.Key, pair.Value);
	    }
	}

	public Value Lookup(string name)
	{
	    Value value;
	    return _locals.TryGetValue(name, out value) ? value : null;
	}

	public bool Set(string name, Value value)
	{
	    if (!_locals.ContainsKey(name))
	    {
		return false;
	    }
	    _locals[name] = value;
	    return true;
	}
	
	public void Bind(string name, Value value)
	{
	    if (_locals.ContainsKey(name))
	    {
		_locals[name] = value;
	    }
	    _locals.Add(name, value);
	}
    }
}