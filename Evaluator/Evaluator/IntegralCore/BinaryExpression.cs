using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evaluator.Exceptions;

namespace Evaluator.IntegralCore
{
    public sealed class BinaryExpression : IntegralExpressionBase
    {
        private BinaryOperator mOperator = BinaryOperator.Default;
        private long left;
        private long right;

        public override void Parse(string input)
        {
            // TODO: implement
        }

        public override long Evaluate()
        {
            checked
            {
                switch (mOperator)
                {
                    case BinaryOperator.Default:
                        throw new InvalidOperatorException();
                    case BinaryOperator.Addition:
                        return left + right;
                    case BinaryOperator.Subtration:
                        return left - right;
                    case BinaryOperator.Multiplication:
                        return left * right;
                    case BinaryOperator.Division:
                        return left / right;
                    case BinaryOperator.IntegralDivision:
                        return left / right;
                    case BinaryOperator.Modulus:
                        return left % right;
                    case BinaryOperator.Exponent:
                        return (long)Math.Pow(left, right);
                    case BinaryOperator.LogicalAnd:
                        return left & right;
                    case BinaryOperator.LogicalOr:
                        return left | right;
                    case BinaryOperator.LogicalXor:
                        return left ^ right;
                    case BinaryOperator.ConditionalAnd:
                        return ((left != 0L) && (right != 0L)) ? 1L : 0L;
                    case BinaryOperator.ConditionalOr:
                        return ((left != 0L) || (right != 0L)) ? 1L : 0L;
                    case BinaryOperator.Equality:
                        return (left == right) ? 1L : 0L;
                    case BinaryOperator.Inequality:
                        return (left != right) ? 1L : 0L;
                    case BinaryOperator.LessThan:
                        return (left < right) ? 1L : 0L;
                    case BinaryOperator.LessThanOrEqualTo:
                        return (left <= right) ? 1L : 0L;
                    case BinaryOperator.GreaterThan:
                        return (left > right) ? 1L : 0L;
                    case BinaryOperator.GreaterThanOrEqualTo:
                        return (left >= right) ? 1L : 0L;
                    default:
                        throw new InvalidOperatorException();
                }
            }
        }
    }
}
