using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.Runtime
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
				{"define", args => Define(args[0], args[1])},

            	{"quote", args => args[0].Quote(this)},
                {"list", args => new Cons(args.Select(node => node.Eval(this)))},
	            {"cons", args => new Cons(args[0].Eval(this), args[1].Eval(this))},
                {"lambda", args => new Lambda(args[0], args[1])},

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
			    
                {"cond", Cond},    
                
                {"car", args => args[0].Eval(this).Car()},
                {"cdr", args => args[0].Eval(this).Cdr()},
			    {"eval", args => args[0].Eval(this).ToExpression().Eval(this)}
            };
        }

        private Value Cond(Node[] args)
        {
            var clauses = args.Cast<Expression>().ToArray();
            var last = clauses.Last();

            var checkElse = new Func<Expression, bool>(clause =>
                clause.Head is Symbol && clause.Head.Quote(this).To<string>() == "else");

            if (checkElse(last))
            {
                clauses = clauses.Take(clauses.Length - 1).ToArray();
            }
            else
            {
                last = null;
            }

            Syntax.Assert(clauses.All(clause => !checkElse(clause)));
            foreach (var clause in clauses)
            {
                if (clause.Head.Eval(this).To<bool>())
                {
                    return clause.Tail.Single().Eval(this);
                }
            }
            return last != null ? last.Tail.Single().Eval(this) : Null.Value;
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

        public Value Lookup(string name)
        {
            Value value;
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
                if (HasDefinition(name))
                {
                    return _definitions[name].Invoke(args != null ? args.ToArray() : new Node[] { });
                }
            }
            else if (call is Lambda)
            {
                var lambda = (Lambda)call;
                return InvokeExpression(lambda.Body,
                    lambda.Args.Quote(this).To<IEnumerable<Value>>().Select(value => value.To<string>()),
                    args != null ? args.ToArray() : new Node[] { });
            }

            throw new SymbolNotDefinedException();
        }

        public Value Define(Node definition, Node body)
        {
            var def = definition is Expression
                ? definition.Quote(this).To<IEnumerable<Value>>().Select(value => value.To<string>()).ToArray()
                : new[] { definition.Quote(this).To<string>() };

            var name = def.First();
            var args = def.Skip(1);

            if (body is Expression)
            {
                _definitions.Add(name, values => InvokeExpression(body, args, values));
            }
            else
            {
                if (_globals.ContainsKey(name))
                {
                    _globals[name] = body.Eval(this);
                }
                else
                {
                    _globals.Add(name, body.Eval(this));
                }
            }
            return Null.Value;
        }

        private Value InvokeExpression(Node body, IEnumerable<string> args, Node[] values)
        {
            var localContext = new LocalContext(this, args, values.Select(value => value.Eval(this)));
            _callStack.Push(localContext);
            dynamic result = body.Eval(this);
            _callStack.Pop();
            return result;
        }
    }
}