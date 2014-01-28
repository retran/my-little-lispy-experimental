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
            var args = new string[] { }; 
		    
            if (definition is Expression)
            {
                var nodes = (IEnumerable<Node>)definition.Eval(this, true).ToArray();
                Syntax.Assert(nodes.All(node => node is Symbol));
		        name = nodes.First().Eval(this, true);
                args = nodes.Skip(1).Select(node => (string)node.Eval(this, true)).ToArray();
		    }
		    else
		    {
		        Syntax.Assert(definition is Symbol);
		        name = (string) definition.Eval(this, true);
		    }
		    
            if (body is Expression)
            {
				_definitions.Add(name, values =>
				{
                    var localContext = new LocalContext(this, args, values);
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