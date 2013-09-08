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
            this.EvaluateUnaryPrefixElements(Operator.UnaryIdentity, (value => value));
            this.EvaluateUnaryPrefixElements(Operator.UnaryInverse, (value => -value));
            this.EvaluateUnaryPostfixElements(Operator.UnaryFactorial, (value => MathHelpers.Factorial(value)));
            this.EvaluateUnaryPrefixElements(Operator.UnaryConditionalNot, (value => (value == 0L) ? 1L : 0L));
            this.EvaluateUnaryPrefixElements(Operator.UnaryLogicalNot, (value => ~value));

            this.EvaluateBinaryElements(Operator.BinaryExponentiation, ((left, right) => (long)Math.Pow(left, right)));
            this.EvaluateBinaryElements(Operator.BinaryMultiplication, ((left, right) => left * right));
            this.EvaluateBinaryElements(Operator.BinaryDivision, ((left, right) => left / right));
            this.EvaluateBinaryElements(Operator.BinaryModulus, ((left, right) => left % right));
            this.EvaluateBinaryElements(Operator.BinaryAddition, ((left, right) => left + right));
            this.EvaluateBinaryElements(Operator.BinarySubtraction, ((left, right) => left - right));
            this.EvaluateBinaryElements(Operator.BinaryShiftLeft, ((left, right) => left << (int)right));
            this.EvaluateBinaryElements(Operator.BinaryShiftRight, ((left, right) => left >> (int)right));
            this.EvaluateBinaryElements(Operator.BinaryLessThan, ((left, right) => (left < right) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryGreaterThan, ((left, right) => (left > right) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryLessThanOrEqualTo, ((left, right) => (left <= right) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryGreterThanOrEqualTo, ((left, right) => (left >= right) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryEquality, ((left, right) => (left == right) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryInequality, ((left, right) => (left != right) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryLogicalAnd, ((left, right) => left & right));
            this.EvaluateBinaryElements(Operator.BinaryLogicalXor, ((left, right) => left ^ right));
            this.EvaluateBinaryElements(Operator.BinaryLogicalOr, ((left, right) => left | right));
            this.EvaluateBinaryElements(Operator.BinaryConditionalAnd, ((left, right) => (left.ConditionalIdentity() && right.ConditionalIdentity()) ? 1 : 0));
            this.EvaluateBinaryElements(Operator.BinaryConditionalOr, ((left, right) => (left.ConditionalIdentity() || right.ConditionalIdentity()) ? 1 : 0));

            this.EvaluateTernaryElements(Operator.TernaryConditional, ((a, b, c) => (a != 0) ? b : c));

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

        private void EvaluateUnaryPrefixElements(Operator operatorType, Func<long, long> expression)
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == operatorType)
                {
                    int operandIndex;
                    var operand = this.GetNextDecimalValue(i, out operandIndex);

                    if (operand == null)
                    {
                        throw new Exception("Expected operand.");
                    }

                    long value = operand.GetValue();
                    value = expression(value);

                    elements[operandIndex] = new IntegralExpressionElement(value.ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateUnaryPostfixElements(Operator operatorType, Func<long, long> expression)
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == operatorType)
                {
                    int operandIndex;
                    var operand = this.GetPreviousDecimalValue(i, out operandIndex);

                    if (operand == null)
                    {
                        throw new Exception("Expected operand.");
                    }

                    long value = operand.GetValue();
                    value = expression(value);

                    elements[operandIndex] = new IntegralExpressionElement(value.ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[i] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateBinaryElements(Operator operatorType, Func<long, long, long> expression)
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == operatorType)
                {
                    int leftOperandIndex, rightOperandIndex;
                    var leftOperand = this.GetPreviousDecimalValue(i, out leftOperandIndex);
                    var rightOperand = this.GetNextDecimalValue(i, out rightOperandIndex);

                    if (leftOperand == null || rightOperand == null)
                    {
                        throw new Exception("Expected operand.");
                    }

                    long left = leftOperand.GetValue();
                    long right = rightOperand.GetValue();
                    long result = expression(left, right);

                    elements[i] = new IntegralExpressionElement(result.ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[leftOperandIndex] = elements[rightOperandIndex] = IntegralExpressionElement.Empty;
                }
            }
        }

        private void EvaluateTernaryElements(Operator operatorType, Func<long, long, long, long> expression)
        {
            for (int i = 0; i < elementCount; i++)
            {
                var current = elements[i];

                if (current.Operator == operatorType)
                {
                    int operand1Index, operand2Index, operand3Index;
                    var operand1 = GetPreviousDecimalValue(i, out operand1Index);
                    var operand2 = GetNextDecimalValue(i, out operand2Index);
                    var operand3 = GetNextDecimalValue(operand2Index, out operand3Index);
                    int secondOperatorIndex = -1;

                    for (int j = operand2Index; j < operand3Index; j++)
                    {
                        if (elements[j].ElementType == ExpressionElement.TernaryOperator)
                        {
                            secondOperatorIndex = j;
                            break;
                        }
                        else if (j == operand3Index - 1)
                        {
                            throw new Exception("Expected matching operator.");
                        }
                    }

                    elements[secondOperatorIndex] = IntegralExpressionElement.Empty;

                    long value1 = operand1.GetValue();
                    long value2 = operand2.GetValue();
                    long value3 = operand3.GetValue();
                    long result = expression(value1, value2, value3);

                    elements[i] = new IntegralExpressionElement(result.ToString(), ExpressionElement.DecimalNumber, Operator.NotAnOperator);
                    elements[operand1Index] = elements[operand2Index] = elements[operand3Index] = IntegralExpressionElement.Empty;
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

        private IntegralExpressionElement GetPreviousDecimalValue(int startingIndex, out int resultIndex)
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

