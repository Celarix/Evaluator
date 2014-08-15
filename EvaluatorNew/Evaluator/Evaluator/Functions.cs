using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public static class Functions
    {
        public static BigDecimal Call(string functionName, params BigDecimal[] args)
        {
            switch (functionName.ToLowerInvariant())
            {
                case "abs":
                    if (args.Length != 1) return 0;
                    return BigDecimal.Abs(args[0]);
                case "ceil":
                case "ceiling":
                    if (args.Length != 1) return 0;
                    return BigDecimal.Ceiling(args[0]);
                case "floor":
                    if (args.Length != 1) return 0;
                    return BigDecimal.Floor(args[0]);
                case "ln":
                    if (args.Length != 1) return 0;
                    return BigDecimal.Ln(args[0]);
                case "log":
                    if (args.Length != 2) return 0;
                    return BigDecimal.Log((int)args[0], args[1]);
                case "max":
                    if (args.Length != 2) return 0;
                    return (args[0] > args[1]) ? args[0] : args[1];
                case "min":
                    if (args.Length != 2) return 0;
                    return (args[0] < args[1]) ? args[0] : args[1];
                default:
                    return new BigDecimal(0, 0);
            }
        }
    }
}
