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
                string firstChar = element[0].ToString();
                string lastChar = element[elementLength - 1].ToString();

                if (elementLength > 1)
                {
                    if (firstChar.IsUnaryOperator())
                    {
                        result.Add(new IntegralExpressionElement(firstChar, ExpressionElement.UnaryPrefixOperator, firstChar.GetUnaryOperator(true)));

                        string operand = element.Substring(1);
                        var operandElement = new IntegralExpressionElement();
                        operandElement.Parse(operand);
                        result.Add(operandElement);
                        continue;
                    }
                    else if (lastChar.IsUnaryOperator())
                    {
                        string operand = element.Substring(0, elementLength - 1);
                        var operandElement = new IntegralExpressionElement();
                        operandElement.Parse(operand);
                        result.Add(operandElement);

                        result.Add(new IntegralExpressionElement(lastChar, ExpressionElement.UnaryPostfixOperator, lastChar.GetUnaryOperator(false)));
                        continue;
                    }
                }

                var resultElement = new IntegralExpressionElement();
                resultElement.Parse(element);
                result.Add(resultElement);
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
                    var operand = this.GetNextNonEmptyValue(i, out operandIndex);
                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    elements[i] = operand;
                    elements[operandIndex] = IntegralExpressionElement.Empty;
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
                    var operand = this.GetNextNonEmptyValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    elements[i] = new IntegralExpressionElement((-long.Parse(operand.Value)).ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[operandIndex] = IntegralExpressionElement.Empty;
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
                    var operand = this.GetPreviousNonEmptyValue(i, out operandIndex);

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
                    var operand = this.GetNextNonEmptyValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    if (operand.Value == "0")
                    {
                        elements[i] = new IntegralExpressionElement("1", ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    }
                    else
                    {
                        elements[i] = new IntegralExpressionElement("0", ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    }

                    elements[operandIndex] = IntegralExpressionElement.Empty;
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
                    var operand = this.GetNextNonEmptyValue(i, out operandIndex);

                    if (operand == null || operand.ElementType != ExpressionElement.DecimalNumber)
                    {
                        throw new Exception("Expected operand.");
                    }

                    elements[i] = new IntegralExpressionElement((~operand.GetValue()).ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[operandIndex] = IntegralExpressionElement.Empty;
                }
            }
        }

        private IntegralExpressionElement GetNextNonEmptyValue(int startingIndex)
        {
            if (startingIndex + 1 >= elementCount)
            {
                return null;
            }

            for (int i = startingIndex + 1; i < elementCount; i++)
            {
                if (elements[i].ElementType != ExpressionElement.Empty)
                {
                    return elements[i];
                }
            }
            return null;
        }

        private IntegralExpressionElement GetPreviousNonEmptyValue(int startingIndex)
        {
            if (startingIndex <= 0)
            {
                return null;
            }

            for (int i = startingIndex - 1; i >= 0; i--)
            {
                if (elements[i].ElementType != ExpressionElement.Empty)
                {
                    return elements[i];
                }
            }
            return null;
        }

        private IntegralExpressionElement GetNextNonEmptyValue(int startingIndex, out int resultIndex)
        {
            if (startingIndex + 1 >= elementCount)
            {
                resultIndex = -1;
                return null;
            }

            for (int i = startingIndex + 1; i < elementCount; i++)
            {
                if (elements[i].ElementType != ExpressionElement.Empty)
                {
                    resultIndex = i;
                    return elements[i];
                }
            }
            resultIndex = 1;
            return null;
        }

        private IntegralExpressionElement GetPreviousNonEmptyValue(int startingIndex, out int resultIndex)
        {
            if (startingIndex <= 0)
            {
                resultIndex = -1;
                return null;
            }

            for (int i = startingIndex - 1; i >= 0; i--)
            {
                if (elements[i].ElementType != ExpressionElement.Empty)
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

