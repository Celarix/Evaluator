using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator
{
    public enum UnaryOperator
    {
        Default = 0,
        Identity,
        Inverse,
        Factorial,
        LogicalNot,
        ConditionalNot
    }

    public enum BinaryOperator
    {
        Default = 0,
        Addition,
        Subtration,
        Multiplication,
        Division,
        IntegralDivision,
        Modulus,
        Exponent,
        LogicalAnd,
        LogicalOr,
        LogicalXor,
        ConditionalAnd,
        ConditionalOr,
        Equality,
        Inequality,
        LessThan,
        LessThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo
    }

    public enum TernaryOperator
    {
        Default = 0,
        Conditional
    }

    public enum ExpressionElement
    {
        Invalid = 0,
        Delimiter,
        BinaryNumber,
        OctalNumber,
        DecimalNumber,
        HexadecimalNumber,
        UnaryPrefixOperator,
        UnaryPostfixOperator,
        BinaryOperator,
        TernaryOperator,
    }
}
