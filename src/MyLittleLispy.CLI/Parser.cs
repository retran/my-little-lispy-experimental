using System;
using System.Collections.Generic;

namespace MyLittleLispy.CLI
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
                .Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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

            var rawValue = _enumerator.Current;
            _enumerator.MoveNext();

            int value;
            if (int.TryParse(rawValue, out value))
            {
                return new Int(value);
            }

            return new Symbol(rawValue);
        }

        public Node Parse(string line)
        {
            _enumerator = Tokenize(line).GetEnumerator();
            _enumerator.MoveNext();

            return Expression();
        }
    }
}