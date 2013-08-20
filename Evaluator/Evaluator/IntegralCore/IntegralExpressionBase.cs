using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator.IntegralCore
{
    public abstract class IntegralExpressionBase
    {
        public abstract void Parse(string input);
        public abstract long Evaluate();
    }
}
