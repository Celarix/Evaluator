using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evaluator.Exceptions;

namespace Evaluator.IntegralCore
{
    public sealed class TernaryExpression : IntegralExpressionBase
    {
        private TernaryOperator mOperator = TernaryOperator.Default;
        private long a;
        private long b;
        private long c;

        public override void Parse(string input)
        {
            // TODO: implement
        }

        public override long Evaluate()
        {
            switch (mOperator)
            {
                case TernaryOperator.Default:
                    throw new InvalidOperatorException();
                case TernaryOperator.Conditional:
                    return (a != 0) ? b : c;
                default:
                    throw new InvalidOperatorException();
            }
        }
    }
}
