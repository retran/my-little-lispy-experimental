
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

    public class Atom : Node
    {
        public override dynamic Eval(Context context)
        {
            if (Value is string && !Quote)
            {
                return context.LocalContext.Lookup(Value);
            }
            return Value;
        }
    }

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

                    if (context.HasDefinition(head.ToString()))
                    {
                        return context.Invoke(head.ToString(), nodes.Skip(1));
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

    public class LocalContext
    {
        private Dictionary<string, dynamic> _locals; 

        public LocalContext(Context context, IEnumerable<string> args, Node[] argValues)
        {
            _locals = new Dictionary<string, dynamic>();
            foreach (var pair in args.Zip(argValues, (s, node) => new KeyValuePair<string, Node>(s, node)))
            {
                _locals.Add(pair.Key, pair.Value.Eval(context));
            }
        }

        public dynamic Lookup(string name)
        {
            return _locals[name];
        }
    }

    public class Context
    {
        private readonly Dictionary<string, Func<Node[], dynamic>> _definitions;
        private readonly Stack<LocalContext> _callStack = new Stack<LocalContext>(); 

        public Context()
        {
            _definitions = new Dictionary<string, Func<Node[], dynamic>>()
            {
                {
                    "defun", (args) =>
                    {
                        var funcName = args[0].Value.ToString();
                        var funcArgs = ((IEnumerable<Node>)args[1].Value).Select(n => (string)n.Value);
                        var body = args[2];
                        this.Define(funcName, funcArgs, body);
                        return null;
                    }
                },
                {"+", (args) => args[0].Eval(this) + args[1].Eval(this)},
                {"-", (args) => args[0].Eval(this) - args[1].Eval(this)},
                {"*", (args) => args[0].Eval(this) * args[1].Eval(this)},
                {"/", (args) => args[0].Eval(this) / args[1].Eval(this)},
                {"=", (args) => args[0].Eval(this) == args[1].Eval(this) ? 1 : 0},
                {"<", (args) => args[0].Eval(this) < args[1].Eval(this) ? 1 : 0},
                {">", (args) => args[0].Eval(this) > args[1].Eval(this) ? 1 : 0},
                {"<=", (args) => args[0].Eval(this) <= args[1].Eval(this) ? 1 : 0},
                {">=", (args) => args[0].Eval(this) >= args[1].Eval(this) ? 1 : 0},
                {"<>", (args) => args[0].Eval(this) != args[1].Eval(this) ? 1 : 0},
                {"and", (args) => args[0].Eval(this) == 1 && args[1].Eval(this) == 1 ? 1 : 0},
                {"or", (args) => args[0].Eval(this) == 1 || args[1].Eval(this) == 1 ? 1 : 0},
                {"xor", (args) => args[0].Eval(this) == 1 ^ args[1].Eval(this) == 1 ? 1 : 0},
                {"not", (args) => args[0].Eval(this) != 1 ? 1 : 0},
                {
                    "if", (args) =>
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
                    "cond", (args) =>
                    {
                        dynamic result = null;
                        foreach (var arg in args)
                        {
                            result = this.Invoke("if", arg.Value.ToArray());
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

        public bool HasDefinition(string name)
        {
            return _definitions.ContainsKey(name);
        }

        public LocalContext LocalContext
        {
            get { return _callStack.Peek(); }
        }

        public object Invoke(string name, IEnumerable<Node> args)
        {
            return _definitions[name].Invoke(args.ToArray());
        }

        public void Define(string name, IEnumerable<string> args, Node body)
        {
            _definitions.Add(name, (values) =>
            {
                var localContext = new LocalContext(this, args, values);
                _callStack.Push(localContext);
                var result = body.Eval(this);
                _callStack.Pop();
                return result;
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
