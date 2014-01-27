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
                },
                {
                    "quote", args =>
                    {
                        foreach (var node in args)
                        {
                            Quote(node);
                        }
                        return args;
                    }
                },
            };
        }

        private void Quote(Node node)
        {
            node.Quote = true;
            if (node is List)
            {
                foreach (var child in node.Value)
                {
                    Quote(child);
                }
            }
        }

        public LocalContext LocalContext
        {
            get { return _callStack.Peek(); }
        }

        public bool HasDefinition(string name)
        {
            return _definitions.ContainsKey(name);
        }

        public dynamic Invoke(string name, IEnumerable<Node> args)
        {
            return _definitions[name].Invoke(args.ToArray());
        }

        public dynamic Lookup(string name)
        {
            dynamic value;
            if (!_globals.TryGetValue(name, out value))
            {
                value = LocalContext.Lookup(name);
            }
            return value;
        }

        public void Define(Node definition, Node body)
        {
            if (definition is List)
            {
                var name = (string) ((IEnumerable<Node>) definition.Value).First().Value;
                IEnumerable<string> args =
                    ((IEnumerable<Node>) definition.Value).Skip(1).Select(node => (string) node.Value);
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
                _globals.Add((string) definition.Value, body.Eval(this));
            }
        }
    }
}