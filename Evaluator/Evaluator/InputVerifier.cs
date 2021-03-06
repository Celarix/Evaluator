﻿using System;
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
            if ((!input.StartsWith("-") && !char.IsDigit(input.First())) || (input.StartsWith("-") && input.Length == 1))
            {
                return false;
            }

            bool foundDecimalPoint = false;
            foreach (char c in input.Substring(1))
            {
                if (!char.IsDigit(c))
                {
                    if (c == '.' && !foundDecimalPoint)
                    {
                        foundDecimalPoint = true;
                    }
                    else
                    {
                        return false;
                    }
                }
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

        public static bool IsUnaryOperator(this char input)
        {
            return input == '+' || input == '-' || input == '~' || input == '!';
        }

        public static bool IsBinaryOperator(this char input)
        {
            return input == '+' || input == '-' || input == '*' || input == '\\'
                || input == '%' || input == '&' || input == '|' || input == '^'
                || input == '=' || input == '!' || input == '<' || input == '>';
        }

        public static bool IsTernaryOperator(this char input)
        {
            return input == '?' || input == ':';
        }

        public static bool IsDecimalNumberWithUnaryOperators(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.IsDecimalNumber()) return false;

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

        public static char GetPreviousNonSpaceCharacter(this string input, int startingIndex)
        {
            for (int i = startingIndex - 1; i >= 0; i--)
            {
                if (input[i] != ' ')
                {
                    return input[i];
                }
            }
            return '\0';
        }

        public static char GetNextNonSpaceCharacter(this string input, int startingIndex)
        {
            for (int i = startingIndex + 1; i < input.Length; i++)
            {
                if (input[i] != ' ')
                {
                    return input[i];
                }
            }
            return '\0';
        }

        public static char GetPreviousNonSpaceCharacter(this string input, int startingIndex, out int resultIndex)
        {
            for (int i = startingIndex - 1; i >= 0; i--)
            {
                if (input[i] != ' ')
                {
                    resultIndex = i;
                    return input[i];
                }
            }
            resultIndex = -1;
            return '\0';
        }

        public static char GetNextNonSpaceCharacter(this string input, int startingIndex, out int resultIndex)
        {
            for (int i = startingIndex + 1; i < input.Length; i++)
            {
                if (input[i] != ' ')
                {
                    resultIndex = i;
                    return input[i];
                }
            }
            resultIndex = -1;
            return '\0';
        }

        public static string GetPreviousNonSpaceCharacterString(this string input, int startingIndex)
        {
            return input.GetPreviousNonSpaceCharacter(startingIndex).ToString();
        }

        public static string GetNextNonSpaceCharacterString(this string input, int startingIndex)
        {
            return input.GetNextNonSpaceCharacter(startingIndex).ToString();
        }

        public static string GetPreviousNonSpaceCharacterString(this string input, int startingIndex, out int resultIndex)
        {
            return input.GetPreviousNonSpaceCharacter(startingIndex, out resultIndex).ToString();
        }

        public static string GetNextNonSpaceCharacterString(this string input, int startingIndex, out int resultIndex)
        {
            return input.GetNextNonSpaceCharacter(startingIndex, out resultIndex).ToString();
        }

        private static bool IsHexadecimalDigit(char input)
        {
            return (input >= '0' && input <= '9') || (input >= 'A' && input <= 'F') || (input >= 'a' && input <= 'f');
        }

        private static bool Contains<T>(T[] array, T item)
        {
            return Array.IndexOf(array, item) != -1;
        }

        public static string[] SplitUnaryElement(this string input)
        {
            StringBuilder prefixOperators = new StringBuilder();
            StringBuilder number = new StringBuilder();
            StringBuilder postfixOperators = new StringBuilder();
            char[] unaryOperatorChars = { '+', '-', '~', '!' };
            bool encounteredNumber = false;

            foreach (char c in input)
            {
                if (Contains(unaryOperatorChars, c))
                {
                    if (!encounteredNumber)
                    {
                        prefixOperators.Append(c);
                    }
                    else
                    {
                        postfixOperators.Append(c);
                    }
                }
                else if (char.IsDigit(c))
                {
                    number.Append(c);
                    encounteredNumber = true;
                }
            }

            return new string[] { prefixOperators.ToString(), number.ToString(), postfixOperators.ToString() };
        }
    }
}
