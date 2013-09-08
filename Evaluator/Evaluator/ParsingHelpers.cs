using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator
{
    public static class ParsingHelpers
    {
        public static string SeparateElements(this string input)
        {
            StringBuilder result = new StringBuilder();
            char[] operatorFirstChars = { '+', '-', '*', '/', '%', '^', '<', '>', '=', '!', '~', '&', '|', '?', ':' };
            char[] operatorSecondChars = { '^', '<', '>', '&', '|', '=' };

            for (int i = 0; i < input.Length; i++)
            {
                char current = input[i];

                if (i == input.Length - 1)
                {
                    result.Append(current);
                    continue;
                }

                if (char.IsDigit(current) || (current >= 'A' && current <= 'F') || char.IsLetter(current) 
                    || current == ' ' || current == ',' || current.IsLeftParentheses() || current.IsRightParentheses())
                {
                    result.Append(current);
                }
                else if (current == '+' || current == '-')
                {
                    int previousValidCharIndex;
                    char previousValidChar = input.GetPreviousNonSpaceCharacter(i, out previousValidCharIndex);

                    if (char.IsDigit(previousValidChar))
                    {
                        if (previousValidCharIndex == i - 1)
                        {
                            result.Append(' ');
                        }
                        result.Append(current);
                        if (input[i + 1] != ' ')
                        {
                            result.Append(' ');
                        }
                    }
                    else
                    {
                        result.Append(current);
                    }
                }
                else if (Array.IndexOf(operatorFirstChars, current) != -1)
                {
                    if (Array.IndexOf(operatorSecondChars, input[i + 1]) != -1)
                    {
                        if (i != 0 && input[i - 1] != ' ')
                        {
                            result.Append(' ');
                        }
                        result.Append(current);
                        if (i + 2 < input.Length)
                        {
                            result.Append(input[i + 1]);
                            if (input[i + 2] != ' ') result.Append(' ');
                            i++;
                        }
                    }
                    else
                    {
                        if (!current.IsUnaryOperator() && i != 0 && Array.IndexOf(operatorFirstChars, input[i - 1]) == -1 && input[i - 1] != ' ')
                        {
                            result.Append(' ');
                        }

                        result.Append(current);

                        if (input[i + 1] != ' ' && !current.ToString().IsUnaryOperator())
                        {
                            result.Append(' ');
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
