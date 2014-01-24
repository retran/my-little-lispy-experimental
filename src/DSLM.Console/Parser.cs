using System;
using System.Collections.Generic;

namespace DSLM.Console
{
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
                .Split(new[] {' ', '\t', '\n'}, StringSplitOptions.RemoveEmptyEntries);
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
                    bool valueQuote = listQuote;
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
                        nodes.Add(new Atom
                        {
                            Value = value,
                            Quote = valueQuote
                        });
                    }
                    else
                    {
                        nodes.Add(new Atom
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
                return new List
                {
                    Value = nodes,
                    Quote = listQuote
                };
            }
            throw new Exception("Syntax error");
        }
    }
}