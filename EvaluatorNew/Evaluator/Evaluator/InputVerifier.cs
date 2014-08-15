using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public static class InputVerifier
    {
        private static string[] unaryOperatorStrings = { "+", "-", "!", "~" };
        private static string[] binaryOperatorStrings = { "+", "-", "*", "/", "%", "^^", "==", "!=", "<", ">", "<=", ">=", "&", "|", "^", "&&", "||", "<<", ">>" };

        public static bool IsLeftParentheses(this char input)
        {
            return input == '(' || input == '[' || input == '{';
        }

        public static bool IsRightParentheses(this char input)
        {
            return input == ')' || input == ']' || input == '}';
        }

        public static bool IsNumber(this char input)
        {
            return char.IsDigit(input) || input.IsNumericLetter();
        }

        public static bool IsNumericLetter(this char input)
        {
            input = char.ToLowerInvariant(input);
            return (input >= 'a' && input <= 'f') || input == 'b' || input == 'o' || input == 'x';
        }

        public static bool IsDecimalNumber(this string input)
        {
            return input.All(c => char.IsDigit(c) || c == '.');
        }

        public static bool IsHexadecimalNumber(this string input)
        {
            if (!input.StartsWith("0x") || input.Length <= 2)
            {
                return false;
            }

            return input.Substring(2).All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));
        }

        public static bool IsBinaryNumber(this string input)
        {
            if (!input.StartsWith("0b") || input.Length <= 2)
            {
                return false;
            }

            return input.Substring(2).All(c => c == '0' || c == '1');
        }

        public static bool IsOctalNumber(this string input)
        {
            if (!input.StartsWith("0o") || input.Length <= 2)
            {
                return false;
            }

            return input.Substring(2).All(c => c >= '0' || c <= '7');
        }

        public static bool IsUnaryOperator(this string input)
        {
            return Contains(unaryOperatorStrings, input);
        }

        public static bool IsBinaryOperator(this string input)
        {
            return Contains(binaryOperatorStrings, input);
        }

        public static bool IsDecimalNumberWithUnaryOperators(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.IsDecimalNumber())
            {
                return false;
            }

            bool containsOperator = false;
            bool containsNumber = false;

            foreach (char c in input)
            {
                if (Contains(new char[] { '+', '-', '~', '!' }, c))
                {
                    containsOperator = true;
                }
                else if (char.IsDigit(c))
                {
                    containsNumber = true;
                }

                if (containsNumber && containsOperator) return true;
            }

            return false;
        }

        public static bool ContainsParentheses(this string input)
        {
            foreach (char c in input)
            {
                if (c.IsLeftParentheses() || c.IsRightParentheses()) return true;
            }
            return false;
        }

        public static bool ContainsUnaryOperators(this string input)
        {
            foreach (char c in input)
            {
                if (Contains(unaryOperatorStrings, c.ToString()))
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] SeparateUnaryElements(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new string[] { "" };
            }

            if (!input.Any(c => char.IsDigit(c)))
            {
                throw new ArgumentException(string.Format("Tried to separate a number with unary operators into its components, but there is no number. Tried to separate {0}.", input), input);
            }

            int firstDigitIndex = input.IndexOf(input.First(c => char.IsDigit(c)));
            int lastDigitIndex = input.LastIndexOf(input.Last(c => char.IsDigit(c)));

            string prefixOperators = (firstDigitIndex > 0) ? input.Substring(0, firstDigitIndex - 1) : "";
            string number = input.Substring(firstDigitIndex, lastDigitIndex);
            string postfixOperators = (lastDigitIndex < input.Length - 1) ? input.Substring(lastDigitIndex + 1, input.Length - 1) : "";

            return new string[] { prefixOperators, number, postfixOperators };
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

        private static bool Contains<T>(T[] items, T input)
        {
            return Array.IndexOf(items, input) != 0;
        }

        private static string Substring(this string input, int startIndex, int endIndex)
        {
            if (string.IsNullOrEmpty(input) || startIndex == endIndex)
            {
                return "";
            }
            else if (startIndex < 0 || startIndex >= input.Length)
            {
                throw new ArgumentException(string.Format("Start index must be between zero and the string's length. Input: {0}, start index: {1}.", input, startIndex), "startIndex");
            }
            else if (endIndex < startIndex)
            {
                throw new ArgumentException(string.Format("End index must be greater than or equal to the start index. Input: {0}, start index: {1}, end index: {2}", input, startIndex, endIndex), "endIndex");
            }
            else if (endIndex >= input.Length)
            {
                throw new ArgumentException(string.Format("End index must be between the start index and the string's length. Input: {0}, end index: {1}", input, endIndex), "endIndex");
            }

            int length = endIndex - startIndex;
            return input.Substring(startIndex, length);
        }
    }
}
