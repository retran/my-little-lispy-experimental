using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.CLI
{
	public class Context
	{
		private readonly Stack<LocalContext> _callStack = new Stack<LocalContext>();
		private readonly Dictionary<string, Func<Node[], dynamic>> _definitions;
		private readonly Dictionary<string, dynamic> _globals = new Dictionary<string, dynamic>();

		public Context()
		{
			_definitions = new Dictionary<string, Func<Node[], dynamic>>
			{
				{
					"define", args =>
					{
						Define(args[0], args[1]);
						return null;
					}
				},
				{"+", args => args[0].Eval(this) + args[1].Eval(this)},
				{"-", args => args[0].Eval(this) - args[1].Eval(this)},
				{"*", args => args[0].Eval(this)*args[1].Eval(this)},
				{"/", args => args[0].Eval(this)/args[1].Eval(this)},
				{"=", args => args[0].Eval(this) == args[1].Eval(this) ? 1 : 0},
				{"<", args => args[0].Eval(this) < args[1].Eval(this) ? 1 : 0},
				{">", args => args[0].Eval(this) > args[1].Eval(this) ? 1 : 0},
				{"<=", args => args[0].Eval(this) <= args[1].Eval(this) ? 1 : 0},
				{">=", args => args[0].Eval(this) >= args[1].Eval(this) ? 1 : 0},
				{"<>", args => args[0].Eval(this) != args[1].Eval(this) ? 1 : 0},
				{"and", args => args[0].Eval(this) == 1 && args[1].Eval(this) == 1 ? 1 : 0},
				{"or", args => args[0].Eval(this) == 1 || args[1].Eval(this) == 1 ? 1 : 0},
				{"xor", args => args[0].Eval(this) == 1 ^ args[1].Eval(this) == 1 ? 1 : 0},
				{"not", args => args[0].Eval(this) != 1 ? 1 : 0},
				{
					"if", args =>
					{
						dynamic condition = args[0].Eval(this);
						if (condition == 1)
						{
							return args[1].Eval(this);
						}
						return args.Length > 2 ? args[2].Eval(this) : null;
					}
				},
				{
					"cond", args =>
					{
						dynamic result = null;
						
                        foreach (Node arg in args)
						{
							result = Invoke("if", arg.Value.ToArray());
							if (result != null)
							{
								break;
							}
						}
						
                        return result;
					}
				}
			};
		}

		public LocalContext LocalContext
		{
			get { return _callStack.Peek(); }
		}

	    public bool HasLocalContext()
	    {
	        return _callStack.Any();
	    }

		public bool HasDefinition(string name)
		{
			return _definitions.ContainsKey(name);
		}

		public dynamic Invoke(string name, IEnumerable<Node> args = null)
		{
            if (HasDefinition(name))
            {
                return _definitions[name].Invoke(args != null ? args.ToArray() : new Node[] { });
            }

            dynamic value = null;
		    if (HasLocalContext())
		    {
		        value = LocalContext.Lookup(name);
                if (value != null)
                {
                    return value;
                }
            }
		    
            if (_globals.TryGetValue(name, out value))
		    {
		        return value;
		    }
            
            throw new SymbolNotDefinedException();
        }

		public void Define(Node definition, Node body)
		{
		    string name = string.Empty;
            var args = new Node[] { }; 
		    
            if (definition is Call)
		    {
		        name = (definition as Call).Function;
		        args = ((IEnumerable<Node>) definition.Value).ToArray();
		        Syntax.Assert(args.All(node => node.Value is string));
		    }
		    else
		    {
		        Syntax.Assert(definition.Value is string);
		        name = (string) definition.Value;
		    }
		    
            if (body is Call)
            {
				_definitions.Add(name, values =>
				{
                    var localContext = new LocalContext(this, args.Select(node => (string)node.Value), values);
                    _callStack.Push(localContext);
					dynamic result = body.Eval(this);
					_callStack.Pop();
					return result;
				});
			}
			else
			{
				_globals.Add(name, body.Eval(this));
			}
		}
	}
}