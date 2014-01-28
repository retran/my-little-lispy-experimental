using System.Collections.Generic;
using System.Linq;

namespace MyLittleLispy.CLI
{
    public class List : Value<IEnumerable<Value>>
    {
        public List(IEnumerable<Value> value) : base(value) { }

        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(", ", _value))    ;
        }
    }
}