using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evaluator.Exceptions;

namespace Evaluator.IntegralCore
{
    public sealed class UnaryExpression : IntegralExpressionBase
    {
        private UnaryOperator mOperator = UnaryOperator.Default;
        private long value;

        public override void Parse(string message)
        {
            // TODO: implement
        }

        public override long Evaluate()
        {
            switch (mOperator)
            {
                case UnaryOperator.Default:
                    throw new InvalidOperatorException();
                case UnaryOperator.Identity:
                    return value;
                case UnaryOperator.Inverse:
                    return -value;
                case UnaryOperator.Factorial:
                    if (value > 20) // 21! is 5.10 * 10^19, Int64.MaxValue is 9.8 * 10^18
                    {
                        throw new OverflowException(string.Format("{0} factorial is greater than the maximum value of a 64-bit signed integer. Try Decimal mode instead."));
                    }

                    long result = 1L;
                    int i = (int)value;

                    while (i > 0)
                    {
                        result *= i;
                        i--;
                    }

                    return result;
                case UnaryOperator.LogicalNot:
                    return ~value;
                case UnaryOperator.ConditionalNot:
                    return (value != 0L) ? 1L : 0L;
                default:
                    throw new InvalidOperatorException();
            }
        }
    }
}
