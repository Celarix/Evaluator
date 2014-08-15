using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Evaluator
{
    public class Element
    {
        public string Value { get; private set; }
        public ElementType Type { get; private set; }
        public Operator Operator { get; private set; }
        
        public Element(string value, ElementType type, Operator @operator)
        {
            this.Value = value;
            this.Type = type;
            this.Operator = @operator;
        }

        public Element()
        {
            this.Operator = Operator.NotAnOperator;
        }

        public BigDecimal GetValue()
        {
            switch (this.Type)
            {
                case ElementType.Invalid:
                    throw new InvalidOperationException("Cannot return the value of an invalid element.");
                case ElementType.Empty:
                    throw new InvalidOperationException("Cannot return the value of an empty element.");
                case ElementType.Delimiter:
                    throw new InvalidOperationException("Cannot return the value of a delimiter.");
                case ElementType.BinaryNumber:
                    return BigDecimal.FromBinary(this.Value.Substring(2));
                case ElementType.OctalNumber:
                    return BigDecimal.FromOctal(this.Value.Substring(2));
                case ElementType.DecimalNumber:
                    return BigDecimal.Parse(this.Value);
                case ElementType.HexadecimalNumber:
                    return BigDecimal.FromHexadecimal(this.Value.Substring(2));
                case ElementType.UnaryPrefixOperator:
                    throw new InvalidOperationException("Cannot return the value of a unary operator.");
                case ElementType.UnaryPostfixOperator:
                    throw new InvalidOperationException("Cannot return the value of a unary operator.");
                case ElementType.BinaryOperator:
                    throw new InvalidOperationException("Cannot return the value of a binary operator.");
                default:
                    throw new InvalidProgramException("This exception should never be thrown. It probably will be, though.");
            }
        }

        public bool IsOperand()
        {
            return this.Type == ElementType.BinaryNumber || this.Type == ElementType.DecimalNumber || this.Type == ElementType.HexadecimalNumber || this.Type == ElementType.OctalNumber;
        }

        public bool IsOperator()
        {
            return this.Type == ElementType.BinaryOperator || this.Type == ElementType.UnaryPrefixOperator || this.Type == ElementType.UnaryPostfixOperator;
        }

        public static Element Parse(string input)
        {
            if (input.Contains(" "))
            {
                throw new ArgumentException(string.Format("Spaces are not allowed in individual elements. Tried to parse {0}.", input), "input");
            }

            Element result;
            if (input.IsDecimalNumber())
            {
                result = new Element(input, ElementType.DecimalNumber, Operator.NotAnOperator);
            }
            else if (input.IsHexadecimalNumber())
            {
                result = new Element(input, ElementType.HexadecimalNumber, Operator.NotAnOperator);
            }
            else if (input.IsBinaryNumber())
            {
                result = new Element(input, ElementType.BinaryNumber, Operator.NotAnOperator);
            }
            else if (input.IsOctalNumber())
            {
                result = new Element(input, ElementType.OctalNumber, Operator.NotAnOperator);
            }
            else if (input.IsUnaryOperator())
            {
                throw new ArgumentException("Cannot parse a single unary operator.", "input");
            }
            else if (input.IsBinaryOperator())
            {
                result = new Element(input, ElementType.BinaryOperator, input.GetBinaryOperator());
            }
            else if (input.IsDecimalNumberWithUnaryOperators())
            {
                throw new ArgumentException("Cannot parse a number with unary operators.", "input");
            }
            throw new ArgumentException(string.Format("Could not recognize the input to parse. Input: \"{0}\"", input), "input");
        }

        private static Element ParseUnaryOperator(string @operator, bool isPrefixOperator)
        {
            return new Element(@operator, (isPrefixOperator) ? ElementType.UnaryPrefixOperator : ElementType.UnaryPostfixOperator, @operator.GetUnaryOperator(isPrefixOperator));
        }

        public static IEnumerable<Element> ParseNumberWithUnaryElements(string input)
        {
            string[] components = input.SeparateUnaryElements();

            // the nice thing about all the unary operators is that they're one character
            int currentComponentIndex = 0;
            int currentCharacterIndex = 0;

            if (currentComponentIndex == 1) // if we're in the number
            {
                currentComponentIndex = 2;
                yield return Element.Parse(components[1]);
            }
            else
            {
                if (currentCharacterIndex < components[currentComponentIndex].Length)
                {
                    yield return Element.ParseUnaryOperator(components[currentComponentIndex][currentCharacterIndex++].ToString(), currentComponentIndex == 0);
                }
                else if (currentCharacterIndex == components[currentComponentIndex].Length)
                {
                    if (currentComponentIndex == 0)
                    {
                        currentComponentIndex = 1;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }

        public static int GetOperatorPrecedence(Operator op)
        {
            switch (op)
            {
                case Operator.Invalid:
                    throw new ArgumentException("The invalid operator has no precedence.");
                case Operator.NotAnOperator:
                    throw new ArgumentException("This is not an operator.");
                case Operator.UnaryIdentity:
                case Operator.UnaryInverse:
                case Operator.UnaryFactorial:
                case Operator.UnaryConditionalNot:
                case Operator.UnaryLogicalNot:
                    return 12;
                case Operator.BinaryExponentiation:
                    return 11;
                case Operator.BinaryMultiplication:
                case Operator.BinaryDivision:
                case Operator.BinaryModulus:
                    return 10;
                case Operator.BinaryAddition:
                case Operator.BinarySubtraction:
                    return 9;
                case Operator.BinaryShiftLeft:
                case Operator.BinaryShiftRight:
                    return 8;
                case Operator.BinaryLessThan:
                case Operator.BinaryGreaterThan:
                case Operator.BinaryLessThanOrEqualTo:
                case Operator.BinaryGreterThanOrEqualTo:
                    return 7;
                case Operator.BinaryEquality:
                case Operator.BinaryInequality:
                    return 6;
                case Operator.BinaryLogicalAnd:
                    return 5;
                case Operator.BinaryLogicalXor:
                    return 4;
                case Operator.BinaryLogicalOr:
                    return 3;
                case Operator.BinaryConditionalAnd:
                    return 2;
                case Operator.BinaryConditionalOr:
                    return 1;
                case Operator.Function:
                    return 13; // TODO: see if this is right
                default:
                    throw new InvalidProgramException("You should never see this exception. You probably will, though.");
            }     
        }
    }
}
