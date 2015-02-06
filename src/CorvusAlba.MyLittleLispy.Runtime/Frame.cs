using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Scope
    {
	private readonly Stack<Frame> _frames;
	private readonly Scope _globalScope;
	
	public Scope()
	{
	    _frames = new Stack<Frame>();
	}

	public Scope(Scope globalScope) : this()
	{
	    _globalScope = globalScope;
	}

	public Value Lookup(string name)
	{
	    foreach (var frame in _frames)
	    {
		Value value = frame.Lookup(name);
		if (value != null)
		{
		    return value;
		}
	    }

	    if (_globalScope != null)
	    {
		return _globalScope.Lookup(name);
	    }

	    return Null.Value;
	}

	public void Bind(string name, Value value)
	{
	    _frames.Peek().Bind(name, value);
	}
	
	public void BeginFrame()
	{
	    _frames.Push(new Frame(new string[] { }, new Value[] { }));
	}

	public void BeginFrame(IEnumerable<string> args, IEnumerable<Value> values)
	{
	    _frames.Push(new Frame(args, values));
	}

	public void EndFrame()
	{
	    _frames.Pop();			
	}
    }
    
    public class Frame
    {
	private readonly Dictionary<string, Value> _locals;

	public Frame(IEnumerable<string> args, IEnumerable<Value> values)
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

	public void Bind(string name, Value value)
	{
	    if (_locals.ContainsKey(name))
	    {
		_locals[name] = value;
	    }
	    else
	    {
		_locals.Add(name, value);
	    }
	}
    }
}