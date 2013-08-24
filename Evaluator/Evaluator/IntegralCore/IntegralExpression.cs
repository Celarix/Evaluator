using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Evaluator.IntegralCore
{
    internal sealed class IntegralExpression
    {
        private List<IntegralExpressionElement> elements = new List<IntegralExpressionElement>();
        private int elementCount = 0;

        public void Parse(string input)
        {
            if (input.ContainsParentheses())
            {
                throw new Exception("Expressions must not contain parenthetical blocks.");
            }

            IEnumerable<string> elementStrings = input.Split(' ').Where(s => !string.IsNullOrEmpty(s));
            var result = new List<IntegralExpressionElement>();

            foreach (string element in elementStrings)
            {
                if (string.IsNullOrEmpty(element))
                {
                    result.Add(new IntegralExpressionElement("", ExpressionElement.Empty, Operator.NotAnOperator));
                }

                int elementLength = element.Length;

                // There are three cases of what a given element string might be.
                // The first case is that it's a decimal number (e.g. 1048).
                // The second case is that it's a binary or ternary operator (e.g. *, ^^, ?).
                // The third case is that it's a decimal number with one or more unary operators (e.g. 5!, ~265, ~!3665!).

                if (element.IsDecimalNumber())
                {
                    result.Add(new IntegralExpressionElement(element, ExpressionElement.DecimalNumber, Operator.NotAnOperator));
                }
                else if (element.IsDecimalNumberWithUnaryOperators())
                {
                    var split = element.SplitUnaryElement();

                    foreach (char c in split[0])
                    {
                        string cStr = c.ToString();
                        result.Add(new IntegralExpressionElement(cStr, ExpressionElement.UnaryPrefixOperator, cStr.GetUnaryOperator(true)));
                    }

                    result.Add(new IntegralExpressionElement(split[1], ExpressionElement.DecimalNumber, Operator.NotAnOperator));

                    foreach (char c in split[2])
                    {
                        string cStr = c.ToString();
                        result.Add(new IntegralExpressionElement(cStr, ExpressionElement.UnaryPostfixOperator, cStr.GetUnaryOperator(false)));
                    }
                }
                else if (element.IsBinaryOperator())
                {
                    result.Add(new IntegralExpressionElement(element, ExpressionElement.BinaryOperator, element.GetBinaryOperator()));
                }
                else if (element.IsTernaryOperator())
                {
                    result.Add(new IntegralExpressionElement(element, ExpressionElement.TernaryOperator, element.GetTernaryOperator()));
                }
                else
                {
                    result.Add(new IntegralExpressionElement("", ExpressionElement.Invalid, Operator.NotAnOperator));
                }
            }

            this.elements = result;
            this.elementCount = elements.Count;
        }

        public string Evaluate()
        {
            this.EvaluateUnaryIdentity();
            this.EvaluateUnaryInverse();
            this.EvaluateUnaryFactorial();
            this.EvaluateUnaryConditionalNot();
            this.EvaluateUnaryLogicalNot();
            this.ClearEmptyValues();

            return elements[0].Value.ToString();
        }

        private void ClearEmptyValues()
        {
            var result = new List<IntegralExpressionElement>();

            foreach (var element in elements)
            {
                if (element.ElementType != ExpressionElement.Empty)
                {
                    result.Add(element);
                }
            }

            elements = result;
        }

        private void EvaluateUnaryIdentity()
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == Operator.UnaryIdentity)
                {
                    int operandIndex;
                    var operand = this.GetNextDecimalValue(i, out operandIndex);
                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    elements[operandIndex] = operand;
                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateUnaryInverse()
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == Operator.UnaryInverse)
                {
                    int operandIndex;
                    var operand = this.GetNextDecimalValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    elements[operandIndex] = new IntegralExpressionElement((-long.Parse(operand.Value)).ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateUnaryFactorial()
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == Operator.UnaryFactorial)
                {
                    int operandIndex;
                    var operand = this.GetPreviousDecValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    long value = operand.GetValue();
                    if (value >= 21 || value <= 0)
                    {
                        throw new Exception("Logarithm over/underflow.");
                    }
                    long result = 1L;

                    while (value > 0)
                    {
                        result *= value;
                        value--;
                    }

                    elements[operandIndex] = new IntegralExpressionElement(result.ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateUnaryConditionalNot()
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == Operator.UnaryConditionalNot)
                {
                    int operandIndex;
                    var operand = this.GetNextDecimalValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    if (operand.Value == "0")
                    {
                        elements[operandIndex] = new IntegralExpressionElement("1", ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    }
                    else
                    {
                        elements[operandIndex] = new IntegralExpressionElement("0", ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    }

                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateUnaryLogicalNot()
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == Operator.UnaryLogicalNot)
                {
                    int operandIndex;
                    var operand = this.GetNextDecimalValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    elements[operandIndex] = new IntegralExpressionElement((~operand.GetValue()).ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private IntegralExpressionElement GetNextDecimalValue(int startingIndex)
        {
            if (startingIndex + 1 >= elementCount)
            {
                return null;
            }

            for (int i = startingIndex + 1; i < elementCount; i++)
            {
                if (elements[i].ElementType == ExpressionElement.DecimalNumber)
                {
                    return elements[i];
                }
            }
            return null;
        }

        private IntegralExpressionElement GetPreviousDecimalValue(int startingIndex)
        {
            if (startingIndex <= 0)
            {
                return null;
            }

            for (int i = startingIndex - 1; i >= 0; i--)
            {
                if (elements[i].ElementType == ExpressionElement.DecimalNumber)
                {
                    return elements[i];
                }
            }
            return null;
        }

        private IntegralExpressionElement GetNextDecimalValue(int startingIndex, out int resultIndex)
        {
            if (startingIndex + 1 >= elementCount)
            {
                resultIndex = -1;
                return null;
            }

            for (int i = startingIndex + 1; i < elementCount; i++)
            {
                if (elements[i].ElementType == ExpressionElement.DecimalNumber)
                {
                    resultIndex = i;
                    return elements[i];
                }
            }
            resultIndex = 1;
            return null;
        }

        private IntegralExpressionElement GetPreviousDecValue(int startingIndex, out int resultIndex)
        {
            if (startingIndex <= 0)
            {
                resultIndex = -1;
                return null;
            }

            for (int i = startingIndex - 1; i >= 0; i--)
            {
                if (elements[i].ElementType == ExpressionElement.DecimalNumber)
                {
                    resultIndex = i;
                    return elements[i];
                }
            }
            resultIndex = -1;
            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (this.elements == null || !this.elements.Any())
            {
                return "Empty expression.";
            }
            elements.ForEach(e => builder.AppendLine(e.ToString()));

            return builder.ToString();
        }
    }
}

