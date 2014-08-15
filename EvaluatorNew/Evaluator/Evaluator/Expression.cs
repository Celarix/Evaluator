using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public sealed class Expression
    {
        public static string SpaceElements(string input)
        {
            // Apply spaces in between all elements.
            // This allows us to pass in strings like 2+(5/3^^(20)), which is turned into 2 + (5 / 3 ^^ (20))
            // Rules for + and - characters, which are both unary and binary operators: if the +/- char is preceded by a space or another operator, it's unary
            // else, it's binary, and gets a space
            char[] singleCharacterOperatorChars = { '+', '-', '~', '*', '/', '%' };
            char[] doubleCharacterOperatorFirstChars = { '^', '=', '!', '<', '>', '&', '|' };
            char[] doubleCharacterOperatorSecondChars = { '^', '=', '<', '>' };

            input = input.Replace(" ", ""); // get rid of spaces in the first place

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char current = input[i];
                char last = (i > 0) ? input[i - 1] : '\0';
                char next = (i + 1 < input.Length) ? input[i + 1] : '\0';

                if (current.IsNumber())
                {
                    builder.Append(current);
                    if (!next.IsNumber() && !char.IsLetter(next) && next != '!' && next != ',' && !next.IsLeftParentheses() && !next.IsRightParentheses())
                    {
                        builder.Append(" ");
                    }
                }
                else if (char.IsLetter(current))
                {
                    builder.Append(current);
                    if (!char.IsLetter(next) && !next.IsLeftParentheses() && !next.IsRightParentheses() && next != ',')
                    {
                        builder.Append(" ");
                    }
                }
                else if (current == '+' || current == '-')
                {
                    builder.Append(current);

                    // if the last character is a number, the operator is binary
                    if (last.IsNumber() || last == '!' || last.IsRightParentheses())
                    {
                        builder.Append(" ");
                    }
                }
                else if (current == '!')
                {
                    if (next == '=')
                    {
                        builder.Append(" ");
                    }

                    builder.Append(current);

                    if (!next.IsNumber() && next != '=')
                    {
                        builder.Append(" ");
                    }
                }
                else if (current == '~')
                {
                    builder.Append(current);
                }
                else if (current == '*' || current == '/' || current == '%')
                {
                    builder.Append(current);
                    if (next.IsNumber() || next == '+' || next == '-' || next == '~' || next == '!')
                    {
                        builder.Append(" ");
                    }
                }
                else if (current == '=')
                {
                    builder.Append(current);
                    if (last == '=' || last == '!' || next.IsNumber() || char.IsLetter(next) || next.IsLeftParentheses())
                    {
                        builder.Append(" ");
                    }
                }
                else if (current == '<' || current == '>')
                {
                    builder.Append(current);
                    if (next != '=' && next != '<' && next != '>')
                    {
                        // greater/less than or equal to operator
                        builder.Append(" ");
                    }
                }
                else if (current == '&' || current == '|' || current == '^')
                {
                    builder.Append(current);
                    if (next != current)
                    {
                        // if next equals current, that means that it's &&, ||, or ^^ - all operators with the same chars
                        builder.Append(" ");
                    }
                }
                else if (current.IsLeftParentheses())
                {
                    builder.Append(current);
                }
                else if (current.IsRightParentheses())
                {
                    builder.Append(current);
                    if (!next.IsRightParentheses())
                    {
                        builder.Append(" ");
                    }
                }
                else if (current == ',')
                {
                    builder.Append(current);
                    builder.Append(" ");
                }
            }

            return builder.ToString();
        }

        private static List<Element> ParseElements(string input)
        {
            input = SpaceElements(input);
            List<Element> result = new List<Element>();

            foreach (string elementString in input.Split(' '))
            {
                if (elementString.Any(c => c == '+' || c == '-' || c == '~' || c == '!'))
                {
                    var elementComponents = Element.ParseNumberWithUnaryElements(elementString);
                    result.AddRange(elementComponents); // so much for that yield, I guess
                }

                result.Add(Element.Parse(elementString));
            }

            return result;
        }

        private static decimal Evaluate(string input)
        {
            List<Element> elements = ParseElements(input);
            Stack<BigDecimal> operands = new Stack<BigDecimal>();
            Stack<Element> operators = new Stack<Element>();
            Stack<Element> result = new Stack<Element>();

            foreach (Element element in elements)
            {
                if (element.IsOperand())
                {
                    operands.Push(element.GetValue());
                }
                else if (element.IsOperator())
                {

                }
            }

            return 0m;
        }
    }
}
