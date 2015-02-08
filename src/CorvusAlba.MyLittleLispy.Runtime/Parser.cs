using System;
using System.Collections.Generic;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Parser
    {
	private IEnumerator<string> _enumerator;

	private IEnumerable<string> Tokenize(string script)
	{
	    return script
		.Replace("(", " ( ")
		.Replace(")", " ) ")
		.Replace("'", " ' ")
		.Replace("`", " ` ")
		.Split(new[] {' ', '\t', '\n'}, StringSplitOptions.RemoveEmptyEntries);
	}

	private Node Expression()
	{
	    Syntax.Assert(_enumerator.Current == "(");
	    _enumerator.MoveNext();

	    var nodes = new List<Node>();
	    while (_enumerator.Current != ")")
	    {
		nodes.Add(Atom());
	    }
	    Syntax.Assert(_enumerator.Current == ")");
	    _enumerator.MoveNext();

	    return new Expression(nodes);
	}

	private Node Atom()
	{
	    if (_enumerator.Current == "(")
	    {
		return Expression();
	    }

	    if (_enumerator.Current == "'")
	    {
		return Quote();
	    }

	    string rawValue = _enumerator.Current;
	    _enumerator.MoveNext();

	    if (rawValue == "#t")
	    {
		return new Constant(new Bool(true));
	    }

	    if (rawValue == "#f")
	    {
		return new Constant(new Bool(false));
	    }

	    int value;
	    if (int.TryParse(rawValue, out value))
	    {
		return new Constant(new Integer(value));
	    }

	    float dvalue;
	    if (float.TryParse(rawValue, out dvalue))
	    {
		return new Constant(new Float(dvalue));
	    }

	    return new Symbol(new String(rawValue));
	}

	private Node Quote()
	{
	    _enumerator.MoveNext();
	    return new Expression(new[]
		    {
			new Symbol(new String("quote")),
			Atom()
		    });
	}

	public Node Parse(string line)
	{
	    _enumerator = Tokenize(line).GetEnumerator();
	    _enumerator.MoveNext();

	    return Atom();
	}
    }
}