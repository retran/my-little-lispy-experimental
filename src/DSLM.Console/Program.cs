
//          Examples:
//
//            (defun square (x) (* x x))
//            (defun distance (x y) (+ (square x) (square y)))
//            (distance 4 5)
//            (distance 2 3)
//
//            (defun fact (v) (if (< v 2) 1 (* v (fact (- v 1)))))
//            (fact 5)
//            (fact 10)            

using System;
using System.Collections.Generic;
using System.Linq;

namespace DSLM.Console
{
    public abstract class Node
    {
        public bool Quote = false;
        public dynamic Value;

        public virtual dynamic Eval(Context context)
        {
            return Value;
        }
    }

    public class Atom : Node { }

    public class List : Node
    {
        public override dynamic Eval(Context context)
        {
            if (!Quote)
            {
                var nodes = (IEnumerable<Node>)Value;
                var head = nodes.First().Value;
                if (head is string)
                {

                    if (context.HasFunction(head.ToString()))
                    {
                        return context.CallFunction(head.ToString(), nodes.Skip(1));
                    }
                    throw new Exception(string.Format("Function '{0}' is not defined", head));
                }
                throw new Exception("Syntax error");
            }
            return Value;
        }
    }

    public class Parser
    {
        private IEnumerator<string> _enumerator;

        private IEnumerable<string> Tokenize(string script)
        {
            return script
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("'", " ' ")
                .Replace("quote", " quote ")
                .Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void SetLine(string line)
        {
            _enumerator = Tokenize(line).GetEnumerator();
            _enumerator.MoveNext();
        }

        public Node Parse(bool quote = false)
        {
            bool listQuote = quote;

            if (_enumerator.Current == "'" || _enumerator.Current == "quote")
            {
                if (!_enumerator.MoveNext() || quote)
                {
                    throw new Exception("Syntax error");
                }
                listQuote = true;
            }

            if (_enumerator.Current == "(")
            {
                if (!_enumerator.MoveNext())
                {
                    throw new Exception("Syntax error");
                }
                var nodes = new List<Node>();
                while (_enumerator.Current != ")")
                {
                    bool valueQuote = false;
                    int value;
                    if (_enumerator.Current == "'" || _enumerator.Current == "quote")
                    {
                        valueQuote = true;
                        if (!_enumerator.MoveNext())
                        {
                            throw new Exception("Syntax error");
                        }
                    }
                    if (_enumerator.Current == "(")
                    {
                        nodes.Add(Parse(valueQuote));
                    }
                    else if (int.TryParse(_enumerator.Current, out value))
                    {
                        nodes.Add(new Atom()
                        {
                            Value = value,
                            Quote = valueQuote
                        });
                    }
                    else
                    {
                        nodes.Add(new Atom()
                        {
                            Value = _enumerator.Current,
                            Quote = valueQuote
                        });
                    }

                    if (!_enumerator.MoveNext())
                    {
                        throw new Exception("Syntax error");
                    }
                }
                return new List()
                {
                    Value = nodes,
                    Quote = listQuote
                };
            }
            throw new Exception("Syntax error");
        }
    }

    public class Context
    {
        private readonly Dictionary<string, Func<string, Node[], dynamic>> _funcs;
        private readonly Dictionary<string, string[]> _argMaps = new Dictionary<string, string[]>();

        public Context()
        {
            _funcs = new Dictionary<string, Func<string, Node[], dynamic>>()
            {
                {
                    "defun", (name, args) =>
                    {
                        var funcName = args[0].Value.ToString();
                        var funcArgs = ((IEnumerable<Node>)args[1].Value).Select(n => (string)n.Value);
                        var body = args[2];
                        this.DefFunction(funcName, funcArgs, body);
                        return null;
                    }
                },
                {"+", (name, args) => args[0].Eval(this) + args[1].Eval(this)},
                {"-", (name, args) => args[0].Eval(this) - args[1].Eval(this)},
                {"*", (name, args) => args[0].Eval(this) * args[1].Eval(this)},
                {"/", (name, args) => args[0].Eval(this) / args[1].Eval(this)},
                {"=", (name, args) => args[0].Eval(this) == args[1].Eval(this) ? 1 : 0},
                {"<", (name, args) => args[0].Eval(this) < args[1].Eval(this) ? 1 : 0},
                {">", (name, args) => args[0].Eval(this) > args[1].Eval(this) ? 1 : 0},
                {"<=", (name, args) => args[0].Eval(this) <= args[1].Eval(this) ? 1 : 0},
                {">=", (name, args) => args[0].Eval(this) >= args[1].Eval(this) ? 1 : 0},
                {"<>", (name, args) => args[0].Eval(this) != args[1].Eval(this) ? 1 : 0},
                {"and", (name, args) => args[0].Eval(this) == 1 && args[1].Eval(this) == 1 ? 1 : 0},
                {"or", (name, args) => args[0].Eval(this) == 1 || args[1].Eval(this) == 1 ? 1 : 0},
                {"xor", (name, args) => args[0].Eval(this) == 1 ^ args[1].Eval(this) == 1 ? 1 : 0},
                {"not", (name, args) => args[0].Eval(this) != 1 ? 1 : 0},
                {
                    "if", (name, args) =>
                    {
                        var condition = args[0].Eval(this);
                        if (condition == 1)
                        {
                            return args[1].Eval(this);
                        }
                        return args.Length > 2 ? args[2].Eval(this) : null;
                    }
                },
                {
                    "cond", (name, args) =>
                    {
                        dynamic result = null;
                        foreach (var arg in args)
                        {
                            result = _funcs["if"].Invoke("if", arg.Value.ToArray());
                            if (result != null)
                            {
                                break;
                            }
                        }
                        return result;
                    }
                },
            };
        }

        public bool HasFunction(string name)
        {
            return _funcs.ContainsKey(name);
        }

        public object CallFunction(string name, IEnumerable<Node> args)
        {
            return _funcs[name].Invoke(name, args.ToArray());
        }

        public Node ProvideArgs(string name, Node[] args, Node body)
        {
            var map = _argMaps[name];
            var bodyNodes = body is List ? (IEnumerable<Node>) body.Value : new Node[] {body}; 
            var result = new List()
            {
                Quote = body.Quote,
                Value = bodyNodes.Select<Node, Node>(node =>
                {
                    if (node is List)
                    {
                        return ProvideArgs(name, args, (List)node);
                    }
                    if (map.Contains((string)node.Value.ToString()) && !node.Quote)
                    {
                        return new Atom()
                        {
                            Quote = node.Quote,
                            Value = args[Array.IndexOf(map, node.Value)].Eval(this)
                        };
                    }
                    return new Atom()
                    {
                        Quote = node.Quote,
                        Value = node.Value
                    };
                })
            };
            return body is List ? result : ((IEnumerable<Node>) result.Value).First();
        }

        public void DefFunction(string name, IEnumerable<string> args, Node body)
        {
            _argMaps.Add(name, args.ToArray());
            _funcs.Add(name, (funcName, funcArgs) =>
            {
                var baseBody = body;
                return ProvideArgs(name, funcArgs, baseBody).Eval(this);
            });
        }
    }

    class Program
    {
        static readonly Parser Parser = new Parser();
        static readonly Context Context = new Context();

        private static void Eval(string line)
        {
            Parser.SetLine(line);
            var result = Parser.Parse().Eval(Context);
            if (result != null)
            {
                System.Console.WriteLine(result.ToString());
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                System.Console.Write(" > ");
                try
                {
                    Eval(System.Console.ReadLine());
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
        }
    }
}
