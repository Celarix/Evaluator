using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator.IntegralCore
{
    public static class MathHelpers
    {
        public static long Factorial(long n)
        {
            if (n == 0) return 1;
            else if (n >= 21 || n < 0) throw new Exception("Factorial over/underflow.");

            long result = 1;
            while (n > 0)
            {
                result *= n;
                n--;
            }

            return result;
        }

        public static bool ConditionalIdentity(this long input)
        {
            return input != 0;
        }
    }
}
