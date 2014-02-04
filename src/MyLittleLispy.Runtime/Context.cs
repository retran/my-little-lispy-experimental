using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.Runtime
{
    public class Context
    {
        private readonly Stack<LocalContext> _callStack = new Stack<LocalContext>();
        private readonly Dictionary<string, Func<Node[], Value>> _specialForms;
        private readonly Dictionary<string, Value> _globals = new Dictionary<string, Value>();

        public Context()
        {
            _specialForms = new Dictionary<string, Func<Node[], Value>>
			{
                {"eval", args => args[0].Eval(this).ToExpression().Eval(this)},
                {"define", args => Define(args[0], args[1])},
            	{"quote", args => args[0].Quote(this)},
                {"list", args => new Cons(args.Select(node => node.Eval(this)))},
	            {"cons", args => new Cons(args[0].Eval(this), args[1].Eval(this))},
                {"lambda", args => new Lambda(this, args[0], args[1])},
                {"cond", args =>
                    {
                        var clause = args.Cast<Expression>().ToArray().FirstOrDefault(c => c.Head.Eval(this).To<bool>());
                        return clause != null ? clause.Tail.Single().Eval(this) : Null.Value;
                    }
                },
            };
        }

        public Value Lookup(string name)
        {
            Value value;
            if (_callStack.Any())
            {
                value = _callStack.Peek().Lookup(name);
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
            var call = head.Eval(this);
            if (call == Null.Value && head is Symbol)
            {
                call = head.Quote(this);
            }

            if (call is String)
            {
                var name = call.To<string>();
                if (_specialForms.ContainsKey(name))
                {
                    return _specialForms[name].Invoke(args != null ? args.ToArray() : new Node[] { });
                }
            }
            else if (call is Lambda)
            {
                return InvokeLambda((Lambda)call, args != null ? args.ToArray() : new Node[] { });
            }

            throw new SymbolNotDefinedException();
        }

        public Value Define(Node definition, Node body)
        {
            var def = definition is Expression
                ? definition.Quote(this).To<IEnumerable<Value>>().Select(value => value.To<string>()).ToArray()
                : new[] { definition.Quote(this).To<string>() };

            var name = def.First();
            var args = def.Skip(1).ToArray();

            if (definition is Expression)
            {                
                SetGlobal(name, new Lambda(args, body));
            }
            else
            {
                SetGlobal(name, body.Eval(this));
            }

            return Null.Value;
        }

        public void SetGlobal(string name, Value value)
        {
            if (_globals.ContainsKey(name))
            {
                _globals[name] = value;
            }
            else
            {
                _globals.Add(name, value);
            }
        }

        private Value InvokeLambda(Lambda lambda, Node[] values)
        {
            var localContext = new LocalContext(this, lambda.Args, values.Select(value => value.Eval(this)));
            _callStack.Push(localContext);
            var result = lambda.Body.Eval(this);
            _callStack.Pop();
            return result;
        }
    }
}