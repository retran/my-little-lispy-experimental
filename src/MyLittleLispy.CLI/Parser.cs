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

        public void SetLine(string line)
        {
            _enumerator = Tokenize(line).GetEnumerator();
            _enumerator.MoveNext();
        }

        private Node Call()
        {
            Syntax.Assert(_enumerator.Current == "(");
            _enumerator.MoveNext();
            
            var function = Atom().Value as string;
            Syntax.Assert(function != null);
            
            var arguments = List().Value as IEnumerable<Node>;
            Syntax.Assert(arguments != null);            
            
            Syntax.Assert(_enumerator.Current == ")");
            _enumerator.MoveNext();
            
            return new Call()
            {
                Function = function,
                Value = arguments,
                Quote = false
            };
        }

        private Node List()
        {
            var list = new List<Node>();
            
            while (_enumerator.Current != ")")
            {
                list.Add(Atom());
            }

            return new List()
            {
                Quote = false,
                Value = list
            };
        }

        private Node Atom()
        {
            if (_enumerator.Current == "(")
            {
                return Call();
            }

            string rawValue = _enumerator.Current;
            _enumerator.MoveNext();

            int value;
            if (int.TryParse(rawValue, out value))
            {
                return new Atom()
                {
                    Quote = false,
                    Value = value
                };   
            }
            
            return new Atom()
            {
                Quote = false,
                Value = rawValue
            };
        }

        public Node Parse()
        {
            return Call();
        }
    }
}