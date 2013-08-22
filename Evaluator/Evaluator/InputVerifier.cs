using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator
{
    internal static class InputVerifier
    {
        private static string[] unaryOperatorStrings = new string[] { "+", "-", "~", "!" };
        private static string[] binaryOperatorStrings = new string[] { "+", "-", "*", "/", "^", "%", "<<", ">>", "<", ">", "<=", ">=", "==", "!=", "&", "|", "^^", "&&", "||" };
        private static string[] ternaryOperatorStrings = new string[] { "?", ":" };

        public static bool IsLeftParentheses(this char input)
        {
            return input == '(' || input == '[' || input == '{';
        }

        public static bool IsRightParentheses(this char input)
        {
            return input == ')' || input == ']' || input == '}';
        }

        public static bool IsBinaryNumber(this string input)
        {
            if (!input.StartsWith("0b") || input.Length <= 2) return false;

            foreach (char c in input.Substring(2))
            {
                if (c != '0' && c != '1') return false;
            }

            return true;
        }

        public static bool IsOctalNumber(this string input)
        {
            if (!input.StartsWith("0o") || input.Length <= 2) return false;

            foreach (char c in input.Substring(2))
            {
                if (c < '0' || c > '7') return false;
            }

            return true;
        }

        public static bool IsDecimalNumber(this string input)
        {
            if (!input.StartsWith("-") && !char.IsDigit(input.First()))
            {
                return false;
            }

            foreach (char c in input)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        public static bool IsHexadecimalNumber(this string input)
        {
            if (!input.StartsWith("0x") || input.Length <= 2) return false;

            foreach (char c in input.Substring(2))
            {
                if (!IsHexadecimalDigit(c)) return false;
            }

            return true;
        }

        public static bool IsUnaryOperator(this string input)
        {
            return Contains(unaryOperatorStrings, input);
        }

        public static bool IsBinaryOperator(this string input)
        {
            return Contains(binaryOperatorStrings, input);
        }

        public static bool IsTernaryOperator(this string input)
        {
            return Contains(ternaryOperatorStrings, input);
        }

        public static bool ContainsParentheses(this string input)
        {
            foreach (char c in input)
            {
                if (c.IsLeftParentheses() || c.IsRightParentheses()) return true;
            }
            return false;
        }

        public static Operator GetUnaryOperator(this string input, bool isPrefix)
        {
            if (!input.IsUnaryOperator())
            {
                throw new Exception(string.Format(@"The string ""{0}"" is not a unary operator."));
            }

            switch (input)
            {
                case "+":
                    return Operator.UnaryIdentity;
                case "-":
                    return Operator.UnaryInverse;
                case "!":
                    return (isPrefix) ? Operator.UnaryConditionalNot : Operator.UnaryFactorial;
                case "~":
                    return Operator.UnaryLogicalNot;
                default:
                    return Operator.NotAnOperator;
            }
        }

        public static Operator GetBinaryOperator(this string input)
        {
            if (!input.IsBinaryOperator())
            {
                throw new Exception(string.Format(@"The string ""{0}"" is not a binary operator."));
            }

            switch (input)
            {
                case "^^":
                    return Operator.BinaryExponentiation;
                case "*":
                    return Operator.BinaryMultiplication;
                case "/":
                    return Operator.BinaryDivision;
                case "%":
                    return Operator.BinaryModulus;
                case "+":
                    return Operator.BinaryAddition;
                case "-":
                    return Operator.BinarySubtraction;
                case "<<":
                    return Operator.BinaryShiftLeft;
                case ">>":
                    return Operator.BinaryShiftRight;
                case "<":
                    return Operator.BinaryLessThan;
                case ">":
                    return Operator.BinaryGreaterThan;
                case "<=":
                    return Operator.BinaryLessThanOrEqualTo;
                case ">=":
                    return Operator.BinaryGreterThanOrEqualTo;
                case "==":
                    return Operator.BinaryEquality;
                case "!=":
                    return Operator.BinaryInequality;
                case "&":
                    return Operator.BinaryLogicalAnd;
                case "^":
                    return Operator.BinaryLogicalXor;
                case "|":
                    return Operator.BinaryLogicalOr;
                case "&&":
                    return Operator.BinaryConditionalAnd;
                case "||":
                    return Operator.BinaryConditionalOr;
                default:
                    return Operator.NotAnOperator;
            }
        }

        public static Operator GetTernaryOperator(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new Exception(string.Format(@"The string ""{0}"" is not a ternary operator."));
            }

            switch (input)
            {
                case "?":
                    return Operator.TernaryConditional;
                case ":":
                    return Operator.TernaryConditional;
                default:
                    return Operator.NotAnOperator;
            }
        }

        private static bool IsHexadecimalDigit(char input)
        {
            return (input >= '0' && input <= '9') || (input >= 'A' && input <= 'F') || (input >= 'a' && input <= 'f');
        }

        private static bool Contains<T>(T[] array, T item)
        {
            return Array.IndexOf(array, item) != -1;
        }
    }
}
