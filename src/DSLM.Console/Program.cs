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
            throw new NotImplementedException();
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
        private readonly Dictionary<string, Func<IEnumerable<Node>, object>> _funcs;

        public EvalContext()
        {
            _funcs = new Dictionary<string, Func<IEnumerable<Node>, object>>()
            {
                {"+", (nodes) =>
                    {
                        var values = nodes.ToArray();
                        return (int)values[0].Eval(this) + (int)values[1].Eval(this);
                    } 
                },
                {"-", (nodes) =>
                    {
                        var values = nodes.ToArray();
                        return (int)values[0].Eval(this) - (int)values[1].Eval(this);
                    } 
                },
                {"*", (nodes) =>
                    {
                        var values = nodes.ToArray();
                        return (int)values[0].Eval(this) * (int)values[1].Eval(this);
                    } 
                },
                {"/", (nodes) =>
                    {
                        var values = nodes.ToArray();
                        return (int)values[0].Eval(this) / (int)values[1].Eval(this);
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
            return _funcs[name].Invoke(args);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string script = "(* (+ (* 2 2) (- 5 3)) 10)";
            var lexer = new Lexer(script);
            var parser = new Parser(lexer);
            var node = parser.Parse();

            System.Console.WriteLine(node.Eval(new EvalContext()));
            System.Console.ReadKey();
        }
    }
}
