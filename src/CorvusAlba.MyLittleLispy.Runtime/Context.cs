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
	private Parser _parser;
	
	public Frame CurrentFrame
	{
	    get
	    {
		return _callStack.Peek();
	    }
	}
	
	public Context(Parser parser)
	{
	    _parser = parser;
	    
	    _specialForms = new Dictionary<string, Func<Node[], Value>>
		{
		    {"eval", args =>
		     {
			 var clause = Trampoline(args[0].Eval(this)).ToExpression();
			 return clause.Eval(this);
		     }
		    },
		    {"define", args => Define(args[0], new Expression(new [] { new Symbol(new String("begin")) }.
								      Concat(args.Skip(1)).ToArray())) },
		    {"quote", args => args[0].Quote(this)},
		    {"quasiquote", Quasiquote},
		    {"unquote", args => Trampoline(args[0].Eval(this)) },
		    {"unquote-splicing", args => Trampoline(args[0].Eval(this)) },
		    {"list", args => new Cons(args.Select(node => Trampoline(node.Eval(this))).ToArray())},
		    {"cons", args => new Cons(Trampoline(args[0].Eval(this)), Trampoline(args[1].Eval(this)))},
		    {"lambda", args => new Closure(this, args[0],
						   new Expression(new [] { new Symbol(new String("begin")) }.
								  Concat(args.Skip(1)).ToArray())) },
		    {
			"cond", args =>
			{
			    var clauses = args.Cast<Expression>();
			    var clause = clauses.ToArray().Take(args.Count() - 1).FirstOrDefault(c => Trampoline(c.Head.Eval(this)).To<bool>());
			    if (clause != null)
			    {
				return  (Value) new Closure(this, null, clause.Tail.Single(), true);
			    }
			    clause = clauses.Last();
			    var head = clause.Head.Quote(this);
			    if (head is String && head.To<string>() == "else")
			    {
				return (Value) new Closure(this, null, clause.Tail.Single(), true);
			    }
			    if (Trampoline(clause.Head.Eval(this)).To<bool>())
			    {
				return (Value) new Closure(this, null, clause.Tail.Single(), true);				
			    }
			    return Null.Value;
			}
		    },
		    {
			"if", args =>
			{
			    var condition = Trampoline(args[0].Eval(this)).To<bool>();
			    if (condition)
			    {
				return new Closure(this, null, args[1], true);
			    }
			    if (args.Length > 2)
			    {
				return new Closure(this, null, args[2], true);
			    }
			    return Null.Value;
			}
		    },
		    {"let", Let},
		    {"set!", Set},
		    {
			"begin", args =>
			{
			    foreach (var arg in args.Take(args.Count() - 1))
			    {
				Trampoline(arg.Eval(this));
			    }
			    return new Closure(this, null, args.Last(), true);
			}
		    },
		    {"import", Import}
		};

	    _globalFrame = new Frame();
	    _callStack.Push(_globalFrame);
	    CurrentFrame.BeginScope();
	}

	private Value Quasiquote(Node[] args)
	{
	    var expression = args[0] as Expression;
	    if (expression == null)
	    {
		return args[0].Quote(this);
	    }
	    
	    return new Cons(expression.Nodes.SelectMany(node =>
		    {
			var expressionNode = node as Expression;
			if (expressionNode != null)
			{
			    var value = expressionNode.Head.Quote(this);
			    if (value is String)
			    {
				var call = value.To<string>();
				if (call == "unquote")
				{
				    return new[] { expressionNode.Eval(this).ToExpression() };
				}
				
				if (call == "unquote-splicing")
				{
				    var exp = expressionNode.Eval(this).ToExpression();
				    if (exp is Expression)
				    {
					return ((Expression)exp).Nodes;
				    }
				    return new[] { exp };
				}
			    }
			}
			return new[] { node };
		    }).Select(node => node.Quote(this)).ToArray());
	}

	private Value Set(Node[] args)
	{
	    var name = Trampoline(args[0].Eval(this)).To<string>();
	    var value = Trampoline(args[1].Eval(this));
	    CurrentFrame.Set(name, value);
	    return value;
	}
	
	private Value Import(Node[] args)
	{
	    var alias = args[0].Eval(this).To<string>();
	    var module = ModuleAttribute.Find(alias);
	    module.Import(_parser, this);
	    return Null.Value;
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
	    var result = new Closure(this, null, new Expression(new [] { new Symbol(new String("begin")) }.
								Concat(args.Skip(1)).ToArray()), true);
	    CurrentFrame.EndScope();

	    return result;
	}

	public void BeginFrame()
	{
	    _callStack.Push(new Frame(_globalFrame));
	}

	public void EndFrame()
	{
	    _callStack.Pop();
	}
	
	public Value Lookup(string name)
	{
	    return CurrentFrame.Lookup(name);
	}

	public Value Trampoline(Value value)
	{
	    var tailCall = value as Closure;
	    while (tailCall != null && tailCall.IsContinuation)
	    {		
		value = InvokeClosure(tailCall, new Node[0]);
		tailCall = value as Closure;
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
		Value result;
		if (!closure.IsContinuation)
		{
		    result = new Closure(this, null, closure.Body, true);
		}
		else
		{
		    result = closure.Body.Eval(this);
		}
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