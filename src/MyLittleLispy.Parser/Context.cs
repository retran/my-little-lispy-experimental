using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MyLittleLispy.Parser
{
    public class Context
    {
        private readonly Stack<LocalContext> _callStack = new Stack<LocalContext>();
        private readonly Dictionary<string, Func<Node[], Value>> _definitions;
        private readonly Dictionary<string, Value> _globals = new Dictionary<string, Value>();

        public Context()
        {
            _definitions = new Dictionary<string, Func<Node[], Value>>
			{
				{
					"define", args =>
					{
						Define(args[0], args[1]);
						return Null.Value;
					}
				},
				{"quote", args => args[0].Quote(this)},
                {"list", args => new Cons(args.Select(node => node.Eval(this)))},
				{"+", args => args[0].Eval(this).Add(args[1].Eval(this)) },
				{"-", args => args[0].Eval(this).Substract(args[1].Eval(this))},
				{"*", args => args[0].Eval(this).Multiple(args[1].Eval(this))},
				{"/", args => args[0].Eval(this).Divide(args[1].Eval(this))},
				{"=", args => args[0].Eval(this).Equal(args[1].Eval(this))},
				{"<", args => args[0].Eval(this).Lesser(args[1].Eval(this))},
				{">", args => args[0].Eval(this).Greater(args[1].Eval(this))},
				{"and", args => args[0].Eval(this).And(args[1].Eval(this))},
				{"or", args => args[0].Eval(this).Or(args[1].Eval(this))},
				{"not", args => args[0].Eval(this).Not()},
			    {
			        "cond", args =>
			        {
			            Syntax.Assert(args.All(arg => arg is ConditionalClause));
			            
                        var clauses = args.Cast<ConditionalClause>();
			            var last = clauses.Last();
			            if (last.IsElse(this))
			            {
			                clauses = clauses.Take(clauses.Count() - 1);
			            }
			            else
			            {
			                last = null;
			            }

                        Syntax.Assert(clauses.All(clause => !clause.IsElse(this)));

			            foreach (var clause in clauses)
			            {
			                if (clause.IsTrue(this))
			                {
			                    return clause.Eval(this);
			                }
			            }

			            return last != null ? last.Eval(this) : Null.Value;
			        }
			    },
                {"cons", args => new Cons(new [] { args[0].Eval(this), args[1].Eval(this) })},
                {"car", args => args[0].Eval(this).Car()},
                {"cdr", args => args[0].Eval(this).Cdr()},
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

        public Value Invoke(string name, IEnumerable<Node> args = null)
        {
            if (HasDefinition(name))
            {
                return _definitions[name].Invoke(args != null ? args.ToArray() : new Node[] { });
            }

            Value value = null;
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
            var def = definition is Expression
                ? definition.Quote(this).Get<IEnumerable<Value>>().Select(value => value.Get<string>())
                : new[] { definition.Quote(this).Get<string>() };

            var name = def.First();
            var args = def.Skip(1);

            if (body is Expression)
            {
                _definitions.Add(name, values =>
                {
                    var localContext = new LocalContext(this, args, values.Select(value => value.Eval(this)));
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