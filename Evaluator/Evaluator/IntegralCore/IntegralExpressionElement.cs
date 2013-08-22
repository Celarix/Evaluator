using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evaluator.IntegralCore
{
    internal sealed class IntegralExpressionElement
    {
        public string Value { get; private set; }
        public ExpressionElement ElementType { get; private set; }
        public Operator Operator { get; private set; }

        public static IntegralExpressionElement Empty
        {
            get
            {
                return new IntegralExpressionElement("", ExpressionElement.Empty, Operator.NotAnOperator);
            }
        }

        public IntegralExpressionElement()
        {
            this.Operator = Operator.NotAnOperator;
        }

        public IntegralExpressionElement(string value, ExpressionElement elementType, Operator @operator)
        {
            this.Value = value;
            this.ElementType = elementType;
            this.Operator = @operator;
        }

        public void Parse(string input)
        {
            this.Value = input;

            if (string.IsNullOrEmpty(input))
            {
                this.ElementType = ExpressionElement.Empty;
                return;
            }

            if (input.IsDecimalNumber())
            {
                this.ElementType = ExpressionElement.DecimalNumber;
            }
            else if (input.IsBinaryOperator())
            {
                this.ElementType = ExpressionElement.BinaryOperator;
                this.Operator = input.GetBinaryOperator();
            }
            else if (input.IsUnaryOperator())
            {
                throw new Exception("Cannot parse a unary operator from within a single expression element.");
            }
            else if (input.IsTernaryOperator())
            {
                this.ElementType = ExpressionElement.TernaryOperator;
                this.Operator = input.GetTernaryOperator();
            }
            else if (input.IsHexadecimalNumber())
            {
                this.ElementType = ExpressionElement.HexadecimalNumber;
            }
            else if (input.IsBinaryNumber())
            {
                this.ElementType = ExpressionElement.BinaryNumber;
            }
            else if (input.IsOctalNumber())
            {
                this.ElementType = ExpressionElement.OctalNumber;
            }
        }

        public long GetValue()
        {
            if (this.ElementType == ExpressionElement.DecimalNumber)
            {
                return long.Parse(this.Value);
            }
            else
            {
                throw new Exception(string.Format("Could not parse the value of {0}.", this.Value));
            }
        }

        public override string ToString()
        {
            return string.Format("Value: {0}, Element Type: {1}, Operator: {2}", this.Value, this.ElementType, this.Operator);
        }
    }
}
