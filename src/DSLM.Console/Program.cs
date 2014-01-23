
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
        public dynamic Value;

        public virtual dynamic Eval(EvalContext context)
        {
            return Value;
        }
    }

    public class IntNode : Node { }

    public class SymbolNode : Node { }

    public class ListNode : Node
    {
        public override object Eval(EvalContext context)

        {
            var nodes = (IEnumerable<Node>)Value;
            var head = nodes.First().Eval(context);
            var tail = nodes.Skip(1);
            if (context.HasFunction(head.ToString()))
            {
                return context.CallFunction(head.ToString(), tail);
            }

            if (head.ToString() == "defun")
            {
                var name = tail.First().Value;
                var args = ((IEnumerable<Node>)tail.Skip(1).First().Value).Select(n => (string)n.Value);
                var body = tail.Skip(2).First();
                context.DefFunction(name, args, body);
                return null;
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
                .Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void SetLine(string line)
        {
            _enumerator = Tokenize(line).GetEnumerator();
            _enumerator.MoveNext();
        }

        public Node Parse()
        {
            if (_enumerator.Current == "(")
            {
                if (!_enumerator.MoveNext())
                {
                    throw new Exception("Syntax error");
                }
                var nodes = new List<Node>();
                while (_enumerator.Current != ")")
                {
                    int value;
                    if (_enumerator.Current == "(")
                    {
                        nodes.Add(Parse());
                    }
                    else if (int.TryParse(_enumerator.Current, out value))
                    {
                        nodes.Add(new IntNode()
                        {
                            Value = value
                        });
                    }
                    else
                    {
                        nodes.Add(new SymbolNode()
                        {
                            Value = _enumerator.Current
                        });
                    }

                    if (!_enumerator.MoveNext())
                    {
                        throw new Exception("Syntax error");
                    }
                }
                return new ListNode()
                {
                    Value = nodes
                };
            }
            throw new Exception("Syntax error");
        }
    }

    public class EvalContext
    {
        private readonly Dictionary<string, Func<string, Node[], dynamic>> _funcs;
        private readonly Dictionary<string, string[]> _argMaps = new Dictionary<string, string[]>();

        public EvalContext()
        {
            _funcs = new Dictionary<string, Func<string, Node[], dynamic>>()
            {
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

        public ListNode ProvideArgs(string name, Node[] args, ListNode body)
        {
            var map = _argMaps[name];
            return new ListNode()
            {
                Value = ((IEnumerable<Node>)body.Value).Select<Node, Node>(node =>
                {
                    if (node is IntNode)
                    {
                        return new IntNode()
                        {
                            Value = ((IntNode) node).Value
                        };
                    }
                    if (node is ListNode)
                    {
                        return ProvideArgs(name, args, (ListNode) node);
                    }
                    if (map.Contains((string)node.Value))
                    {
                        return new IntNode()
                        {
                            Value = args[Array.IndexOf(map, node.Value)].Eval(this)
                        };
                    }
                    return new SymbolNode()
                    {
                        Value = node.Value
                    };
                })
            };
        }

        public void DefFunction(string name, IEnumerable<string> args, Node body)
        {
            _argMaps.Add(name, args.ToArray());
            _funcs.Add(name, (funcName, funcArgs) =>
            {
                var baseBody = body;
                return ProvideArgs(name, funcArgs, (ListNode) baseBody).Eval(this);
            });
        }
    }

    class Program
    {
        static readonly Parser Parser = new Parser();
        static readonly EvalContext Context = new EvalContext();

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
