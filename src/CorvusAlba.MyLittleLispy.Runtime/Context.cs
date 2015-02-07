using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class TailCall : Value
    {
	private IEnumerable<Frame> _frames;
	public Node Body { get; private set; }

	public TailCall(Context context, Node body)
	{
	    _frames = context.Scope.Export();
	    Body = body;
	}

	public override Node ToExpression()
	{
	    return Body;
	}

	public Value Call(Context context)
	{
	    context.BeginScope();
	    try
	    {
		context.Scope.Import(_frames);
		Value result = Body.Eval(context);
		return result;
	    }
	    finally
	    {
		context.EndScope();
	    }
	}
    }
    
    public class Context
    {
	private readonly Stack<Scope> _callStack = new Stack<Scope>();
	private readonly Dictionary<string, Func<Node[], Value>> _specialForms;
	private readonly Scope _globalScope;
	
	public Scope Scope
	{
	    get
	    {
		return _callStack.Peek();
	    }
	}
	
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
			    return clause != null ? (Value) new TailCall(this, clause.Tail.Single()) : Null.Value;
			}
		    },
		    {
			"if", args =>
			{
			    var condition = args[0].Eval(this).To<bool>();
			    if (condition)
			    {
				return new TailCall(this, args[1]);
			    }
			    if (args.Length > 2)
			    {
				return new TailCall(this, args[2]);
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

			    Scope.Bind(name, value);
					
			    return value;
			}
		    },
		    {
			"begin", args =>
			{
			    foreach (var arg in args.Take(args.Count() - 1))
			    {
				arg.Eval(this);
			    }
			    return new TailCall(this, args.Last());
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

	    _globalScope = new Scope();
	    _callStack.Push(_globalScope);
	    Scope.BeginFrame();
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

	    Scope.BeginFrame(frameArgs, frameValues);
	    var result = new TailCall(this, args[1]);
	    Scope.EndFrame();

	    return result;
	}

	public void BeginScope()
	{
	    _callStack.Push(new Scope(_globalScope));
	}

	public void EndScope()
	{
	    _callStack.Pop();
	}
	
	public Value Lookup(string name)
	{
	    return Scope.Lookup(name);
	}

	public Value Trampolin(Value value)
	{
	    var tailCall = value as TailCall;
	    while (tailCall != null)
	    {
		value = tailCall.Call(this);
		tailCall = value as TailCall;
	    }
	    return value;
	}
	
	public Value Invoke(Node head, IEnumerable<Node> args = null)
	{
	    Value call;
	    try
	    {
		call = head.Eval(this);
	    }
	    catch (SymbolNotDefinedException)
	    {
		if (head is Symbol)
		{
		    call = head.Quote(this);
		}
		else
		{
		    throw;
		}
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
		    var value = InvokeLambda(lambda, args != null ? args.ToArray() : new Node[] {});
		    if (Scope.IsTrampolin)
		    {
			value = Trampolin(value);
		    }
		    return value;
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
		Scope.Bind(name, new Lambda(args, body));
	    }
	    else
	    {
		Scope.Bind(name, body.Eval(this));
	    }

	    return Null.Value;
	}

	private Value InvokeLambda(Lambda lambda, Node[] values)
	{
	    var arguments = values.Select(value => value.Eval(this)).ToArray();
	    BeginScope();
	    try
	    {
		Scope.Import(lambda.Frames);
		Scope.BeginFrame(lambda.Args, arguments);
		Value result = lambda.Body.Eval(this);
		Scope.EndFrame();
		return result;
	    }
	    finally
	    {
	        EndScope();
	    }
	}
    }
}