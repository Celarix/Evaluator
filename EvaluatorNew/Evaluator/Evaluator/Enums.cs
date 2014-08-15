﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public enum ElementType
    {
        Invalid = 0,
        Empty,
        Delimiter,
        BinaryNumber,
        OctalNumber,
        DecimalNumber,
        HexadecimalNumber,
        UnaryPrefixOperator,
        UnaryPostfixOperator,
        BinaryOperator
    }
    
    public enum Operator
    {
        Invalid = 0,
        NotAnOperator,
        UnaryIdentity,
        UnaryInverse,
        UnaryFactorial,
        UnaryConditionalNot,
        UnaryLogicalNot,
        BinaryExponentiation,
        BinaryMultiplication,
        BinaryDivision,
        BinaryModulus,
        BinaryAddition,
        BinarySubtraction,
        BinaryShiftLeft,
        BinaryShiftRight,
        BinaryLessThan,
        BinaryGreaterThan,
        BinaryLessThanOrEqualTo,
        BinaryGreterThanOrEqualTo,
        BinaryEquality,
        BinaryInequality,
        BinaryLogicalAnd,
        BinaryLogicalXor,
        BinaryLogicalOr,
        BinaryConditionalAnd,
        BinaryConditionalOr,
        Function
    }
}
