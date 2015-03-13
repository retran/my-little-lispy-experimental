using System.Collections.Generic;
using System.Text;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Parser
    {
        private IEnumerator<string> _enumerator;
        private readonly HashSet<char> _whitespaces = new HashSet<char>(new[] { ' ', '\t', '\n', '\r' });

        private bool IsValidForIdentifier(char c)
        {
            return !(c == '(' || c == ')' || c == '\'' || c == '`' || c == ',' || c == '@') && !_whitespaces.Contains(c);
        }

        private IEnumerable<string> Tokenize(string script)
        {
            var chars = script.ToCharArray();
            var i = 0;
            while (i < chars.Length)
            {
                if (_whitespaces.Contains(chars[i]))
                {
                    i++;
                    continue;
                }

                var sb = new StringBuilder();
                if (chars[i] == '\"')
                {
                    // WTF?!
                    sb.Append(chars[i]);
                    i++;
                    while (chars[i] != '\"')
                    {
                        sb.Append(chars[i]);
                        i++;
                    }
                    sb.Append(chars[i]);
                    i++;
                }
                else if (IsValidForIdentifier(chars[i]))
                {
                    while (i < chars.Length && IsValidForIdentifier(chars[i]))
                    {
                        sb.Append(chars[i]);
                        i++;
                    }
                }
                else
                {
                    sb.Append(chars[i]);
                    i++;
                }
                yield return sb.ToString();
            }
        }

        private Node Expression()
        {
            Syntax.Assert(_enumerator.Current == "(");
            _enumerator.MoveNext();

            var nodes = new List<Node>();
            while (_enumerator.Current != ")")
            {
                nodes.Add(Atom());
            }
            Syntax.Assert(_enumerator.Current == ")");
            _enumerator.MoveNext();

            return new Expression(nodes);
        }

        private Node Atom()
        {
            if (_enumerator.Current == "(")
            {
                return Expression();
            }

            if (_enumerator.Current == "'")
            {
                _enumerator.MoveNext();
                return Wrap("quote");
            }

            if (_enumerator.Current == "`")
            {
                _enumerator.MoveNext();
                return Wrap("quasiquote");
            }

            if (_enumerator.Current == ",")
            {
                _enumerator.MoveNext();
                if (_enumerator.Current == "@")
                {
                    _enumerator.MoveNext();
                    return Wrap("unquote-splicing");
                }
                return Wrap("unquote");
            }

            string rawValue = _enumerator.Current;
            _enumerator.MoveNext();

            if (rawValue == "#t")
            {
                return new Constant(new Bool(true));
            }

            if (rawValue == "#f")
            {
                return new Constant(new Bool(false));
            }

            if (rawValue.StartsWith("\""))
            {
                return new Constant(new String(rawValue.Substring(1, rawValue.Length - 2)));
            }

            int value;
            if (int.TryParse(rawValue, out value))
            {
                return new Constant(new Integer(value));
            }

            float dvalue;
            if (float.TryParse(rawValue, out dvalue))
            {
                return new Constant(new Float(dvalue));
            }

            return new Symbol(new SymbolValue(rawValue));
        }

        private Node Wrap(string function)
        {
            return new Expression(new[]
		    {
    			new Symbol(new SymbolValue(function)),
			    Atom()
		    });
        }

        public Node Parse(string line)
        {
            _enumerator = Tokenize(line).GetEnumerator();
            _enumerator.MoveNext();

            return Atom();
        }
    }
}