using System;
using System.Collections.Generic;
using System.Linq;

namespace DSLM.Console
{
    public abstract class Node
    {
        public abstract object Eval(EvalContext context);
    }

    public class IntNode : Node
    {
        public int Value;
        public override object Eval(EvalContext context)
        {
            return Value;
        }
    }

    public class ListNode : Node
    {
        public IEnumerable<Node> Nodes;

        public override object Eval(EvalContext context)
        {
            var head = Nodes.First().Eval(context);
            var tail = Nodes.Skip(1);
            if (context.HasFunction(head.ToString()))
            {
                return context.CallFunction(head.ToString(), tail);
            }

            if (head.ToString() == "def")
            {
                var name = ((SymbolNode)tail.First()).Value;
                var args = ((ListNode)tail.Skip(1).First()).Nodes.Select(n => (n as SymbolNode).Value);
                var body = ((ListNode)tail.Skip(2).First());
                context.DefFunction(name, args, body);
                return null;
            }
            return Nodes;
        }
    }

    public class SymbolNode : Node
    {
        public string Value;

        public override object Eval(EvalContext context)
        {
            return Value;
        }
    }

    public class Lexer
    {
        private readonly IEnumerator<string> _enumerator;

        public Lexer(string script)
        {
            _enumerator = Tokenize(script).GetEnumerator();
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public string Current
        {
            get { return _enumerator.Current; }
        }

        private IEnumerable<string> Tokenize(string script)
        {
            return script
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public class Parser
    {
        private readonly Lexer _lexer;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            if (!_lexer.MoveNext())
            {
                throw new Exception("Unexpected eof");
            }
        }

        public Node Parse()
        {
            return ParseImpl();
        }

        private Node ParseImpl()
        {
            if (_lexer.Current == "(")
            {
                if (!_lexer.MoveNext())
                {
                    throw new Exception("Syntax error");
                }
                var nodes = new List<Node>();
                while (_lexer.Current != ")")
                {
                    int value;
                    if (_lexer.Current == "(")
                    {
                        nodes.Add(ParseImpl());
                    }
                    else if (int.TryParse(_lexer.Current, out value))
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
                            Value = _lexer.Current
                        });
                    }

                    if (!_lexer.MoveNext())
                    {
                        throw new Exception("Syntax error");
                    }
                }
                return new ListNode()
                {
                    Nodes = nodes
                };
            }
            else
            {
                throw new Exception("Syntax error");
            }
        }
    }

    public class EvalContext
    {
        private readonly Dictionary<string, Func<string, Node[], object>> _funcs;
        private readonly Dictionary<string, string[]> _argMaps = new Dictionary<string, string[]>();

        public EvalContext()
        {
            _funcs = new Dictionary<string, Func<string, Node[], object>>()
            {
                {"+", (name, args) => (int)args[0].Eval(this) + (int)args[1].Eval(this)},
                {"-", (name, args) => (int)args[0].Eval(this) - (int)args[1].Eval(this)},
                {"*", (name, args) => (int)args[0].Eval(this) * (int)args[1].Eval(this)},
                {"/", (name, args) => (int)args[0].Eval(this) / (int)args[1].Eval(this)},
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
                Nodes = body.Nodes.Select<Node, Node>(node =>
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
                    if (map.Contains(((SymbolNode) node).Value))
                    {
                        return new IntNode()
                        {
                            Value = (int) args[Array.IndexOf(map, ((SymbolNode) node).Value)].Eval(this)
                        };
                    }
                    return new SymbolNode()
                    {
                        Value = ((SymbolNode)node).Value
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
        static readonly EvalContext Context = new EvalContext();

        private static void Eval(string text)
        {
            System.Console.WriteLine(new Parser(new Lexer(text)).Parse().Eval(Context));
        }

        static void Main(string[] args)
        {
//            Eval("(def square (x) (* x x))");
//            Eval("(def distance (x y) (+ (square x) (square y)))");
//            Eval("(distance 4 5)");
//            Eval("(distance 2 3)");

            while (true)
            {
                System.Console.Write(" > ");
                Eval(System.Console.ReadLine());
            }
        }
    }
}
