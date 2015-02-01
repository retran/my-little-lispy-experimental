using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Context
    {
	private readonly Stack<Frame> _callStack = new Stack<Frame>();
	private readonly Dictionary<string, Func<Node[], Value>> _specialForms;

	public Context()
	{
	    _specialForms = new Dictionary<string, Func<Node[], Value>>
		{
		    {"eval", args => args[0].Eval(this).ToExpression().Eval(this)},
		    {"define", args => Define(args[0], args[1])},
		    {"quote", args => args[0].Quote(this)},
		    {"list", args => new Cons(args.Select(node => node.Eval(this)).ToArray())},
		    {"cons", args => new Cons(args[0].Eval(this), args[1].Eval(this))},
		    {"lambda", args => new Lambda(this, args[0], args[1])},
		    {
			"cond", args =>
			{
			    var clause = args.Cast<Expression>().ToArray().FirstOrDefault(c => c.Head.Eval(this).To<bool>());
			    return clause != null ? clause.Tail.Single().Eval(this) : Null.Value;
			}
		    },
		    {
			"if", args =>
			{
			    var condition = args[0].Eval(this).To<bool>();
			    if (condition)
			    {
				return args[1].Eval(this);
			    }
			    if (args.Length > 2)
			    {
				return args[2].Eval(this);
			    }
			    return Null.Value;
			}
		    },
		    {"let", Let},
		    {
			"set!", args =>
			{
			    var name = args[0].Eval(this).To<string>();
			    var value = args[1].Eval(this);

			    _callStack.Peek().Bind(name, value);
					
			    return value;
			}
		    },
		    {
			"begin", args =>
			{
			    Value value = Null.Value;
			    foreach (var arg in args)
			    {
				value = arg.Eval(this);
			    }
			    return value;
			}
		    },
		    {
			"while", args =>
			{
			    while (args[0].Eval(this).To<bool>())
			    {
				args[1].Eval(this);
			    }
			    return Null.Value;
			}
		    }
		};

	    BeginFrame();
	}

	private Value Let(Node[] args)
	{
	    var frameArgs = new List<string>();
	    var frameValues = new List<Value>();

	    foreach (var clause in args[0].Quote(this).To<IEnumerable<Value>>().Select(v => v.ToExpression()).Cast<Expression>())
	    {
		frameArgs.Add(clause.Head.Quote(this).To<string>());
		frameValues.Add(clause.Tail.Single().Eval(this));
	    }

	    BeginFrame(frameArgs, frameValues);
	    var result = args[1].Eval(this);
	    EndFrame();

	    return result;
	}

	public Value Lookup(string name)
	{
	    foreach (var frame in _callStack)
	    {
		Value value = frame.Lookup(name);
		if (value != null)
		{
		    return value;
		}
	    }
	    return Null.Value;
	}

	public Value Invoke(Node head, IEnumerable<Node> args = null)
	{
	    Value call = head.Eval(this);
	    if (call == Null.Value && head is Symbol)
	    {
		call = head.Quote(this);
	    }

	    if (call is String)
	    {
		var name = call.To<string>();
		if (_specialForms.ContainsKey(name))
		{
		    return _specialForms[name].Invoke(args != null ? args.ToArray() : new Node[] {});
		}
	    }
	    else
	    {
		var lambda = call as Lambda;
		if (lambda != null)
		{
		    return InvokeLambda(lambda, args != null ? args.ToArray() : new Node[] {});
		}
	    }

	    throw new SymbolNotDefinedException();
	}

	public Value Define(Node definition, Node body)
	{
	    string[] def = definition is Expression
		? definition.Quote(this).To<IEnumerable<Value>>().Select(value => value.To<string>()).ToArray()
		: new[] {definition.Quote(this).To<string>()};

	    string name = def.First();
	    string[] args = def.Skip(1).ToArray();

	    if (definition is Expression)
	    {
		Bind(name, new Lambda(args, body));
	    }
	    else
	    {
		Bind(name, body.Eval(this));
	    }

	    return Null.Value;
	}

	public void Bind(string name, Value value)
	{
	    _callStack.Peek().Bind(name, value);
	}

	public void BeginFrame()
	{
	    _callStack.Push(new Frame(new string[] { }, new Value[] { }));
	}

	public void BeginFrame(IEnumerable<string> args, IEnumerable<Value> values )
	{
	    _callStack.Push(new Frame(args, values));
	}

	public void EndFrame()
	{
	    _callStack.Pop();			
	}

	private Value InvokeLambda(Lambda lambda, Node[] values)
	{
	    BeginFrame(lambda.Args, values.Select(value => value.Eval(this)));
	    Value result = lambda.Body.Eval(this);
	    EndFrame();
	    return result;
	}
    }
}