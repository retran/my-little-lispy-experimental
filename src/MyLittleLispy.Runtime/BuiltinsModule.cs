using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyLittleLispy.Runtime
{
    public class BuiltinsModule : IModule
    {
        private readonly IEnumerable<string> _defines = new[]
        {
            "(define (<= x y) (or (< x y) (= x y)))",
            "(define (>= x y) (or (> x y) (= x y)))",
            "(define (xor x y) (and (or x y) (not (and x y))))",

            "(define (if condition then-clause else-clause) (cond ((condition) then-clause) (else else-clause)))"
        };

        public void Import(Parser parser, Context context)
        {
            foreach (var define in _defines)
            {
                parser.Parse(define).Eval(context);
            }
        }
    }
}
