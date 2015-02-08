using System;
using System.Collections.Generic;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{  
    public class Context
    {
	private readonly Stack<Frame> _callStack = new Stack<Frame>();
	private readonly Dictionary<string, Func<Node[], Value>> _specialForms;
	private readonly Frame _globalFrame;

	public bool LexicalScopeMode = true;
	
	public Frame CurrentFrame
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
		    {"eval", args =>
		     {
			 var value = Trampoline(args[0].Eval(this)).ToExpression().Eval(this);
			 return value;
		     }
		    },
		    {"define", args => Define(args[0], args[1])},
		    {"quote", args => args[0].Quote(this)},
		    {"list", args => new Cons(args.Select(node => Trampoline(node.Eval(this))).ToArray())},
		    {"cons", args => new Cons(Trampoline(args[0].Eval(this)), Trampoline(args[1].Eval(this)))},
		    {"lambda", args => new Closure(this, args[0], args[1])},
		    {
			"cond", args =>
			{
			    var clause = args.Cast<Expression>().ToArray().FirstOrDefault(c => Trampoline(c.Head.Eval(this)).To<bool>());
			    return clause != null ? (Value) new Continuation(this, clause.Tail.Single(), LexicalScopeMode) : Null.Value;
			}
		    },
		    {
			"if", args =>
			{
			    var condition = Trampoline(args[0].Eval(this)).To<bool>();
			    if (condition)
			    {
				return new Continuation(this, args[1], LexicalScopeMode);
			    }
			    if (args.Length > 2)
			    {
				return new Continuation(this, args[2], LexicalScopeMode);
			    }
			    return Null.Value;
			}
		    },
		    {"let", Let},
		    {
			"set!", args =>
			{
			    var name = Trampoline(args[0].Eval(this)).To<string>();
			    var value = Trampoline(args[1].Eval(this));
			    CurrentFrame.Set(name, value);
			    return value;
			}
		    },
		    {
			"begin", args =>
			{	
			    var expression = new Expression(new Node[] { new Symbol(new String("eval-sequence")),
								     new Expression(new Node[] { new Symbol(new String("quote")),
											     new Expression(args) })});
			    return new Continuation(this, expression, false);
			}
		    },
		    {
			"while", args =>
			{
			    while (Trampoline(args[0].Eval(this)).To<bool>())
			    {
				Trampoline(args[1].Eval(this));
			    }
			    return Null.Value;
			}
		    }
		};

	    _globalFrame = new Frame();
	    _callStack.Push(_globalFrame);
	    CurrentFrame.BeginScope();
	}

	private Value Let(Node[] args)
	{
	    var frameArgs = new List<string>();
	    var frameValues = new List<Value>();

	    foreach (var clause in args[0].Quote(this).To<IEnumerable<Value>>().Select(v => v.ToExpression()).Cast<Expression>())
	    {
		frameArgs.Add(clause.Head.Quote(this).To<string>());
		frameValues.Add(Trampoline(clause.Tail.Single().Eval(this)));
	    }

	    CurrentFrame.BeginScope(frameArgs, frameValues);
	    var result = new Continuation(this, args[1], LexicalScopeMode);
	    CurrentFrame.EndScope();

	    return result;
	}

	public void BeginFrame()
	{
	    if (LexicalScopeMode)
	    {
		_callStack.Push(new Frame(_globalFrame));
	    }
	}

	public void EndFrame()
	{
	    if (LexicalScopeMode)
	    {
		_callStack.Pop();
	    }
	}
	
	public Value Lookup(string name)
	{
	    return CurrentFrame.Lookup(name);
	}

	public Value Trampoline(Value value)
	{
	    var tailCall = value as Continuation;
	    while (tailCall != null)
	    {
		value = tailCall.Call(this);
		tailCall = value as Continuation;
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
		    var value = _specialForms[name].Invoke(args != null ? args.ToArray() : new Node[] {});
		    if (CurrentFrame.IsTrampolin)
		    {
			value = Trampoline(value);
		    }
		    return value;
		}
	    }
	    else
	    {
		var lambda = call as Closure;
		if (lambda != null)
		{
		    var value = InvokeClosure(lambda, args != null ? args.ToArray() : new Node[] {});
		    if (CurrentFrame.IsTrampolin)
		    {
			value = Trampoline(value);
		    }
		    return value;
		}
	    }

	    throw new SymbolNotDefinedException("");
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
		CurrentFrame.Bind(name, new Closure(args, body));
	    }
	    else
	    {
		CurrentFrame.Bind(name, body.Eval(this));
	    }

	    return Null.Value;
	}

	private Value InvokeClosure(Closure closure, Node[] values)
	{
	    var arguments = values.Select(value => Trampoline(value.Eval(this))).ToArray();
	    BeginFrame();
	    CurrentFrame.Import(closure.Scopes);
	    try
	    {
		CurrentFrame.BeginScope(closure.Args, arguments);
		Value result = new Continuation(this, closure.Body, LexicalScopeMode);
		CurrentFrame.EndScope();
		return result;
	    }
	    finally
	    {
		if (closure.Scopes != null)
		{
		    for (int i = 0; i < closure.Scopes.Count(); i++)
		    {
			CurrentFrame.EndScope();
		    }
		}
	        EndFrame();
	    }
	}
    }
}