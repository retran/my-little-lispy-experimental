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

	    throw new SymbolNotDefinedException();
	}

	public IEnumerable<Frame> Export()
	{
	    return _frames.Reverse().ToArray();
	}
	
	public void Import(IEnumerable<Frame> frames)
	{
	    if (frames != null)
	    {
		foreach (var frame in frames)
		{
		    _frames.Push(frame);
		}
	    }
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
}