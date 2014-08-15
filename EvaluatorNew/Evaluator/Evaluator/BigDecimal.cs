using System;
using System.Numerics;

namespace Evaluator
{
    /// <summary>
    /// Arbitrary precision decimal.
    /// All operations are exact, except for division. Division never determines more digits than the given precision.
    /// Based on http://stackoverflow.com/a/4524254
    /// Author: Jan Christoph Bernack (contact: jc.bernack at googlemail.com)
    /// </summary>
    public struct BigDecimal
        : IComparable
        , IComparable<BigDecimal>
    {
        /// <summary>
        /// Specifies whether the significant digits should be truncated to the given precision after each operation.
        /// </summary>
        public static bool AlwaysTruncate = false;

        /// <summary>
        /// Sets the maximum precision of division operations.
        /// If AlwaysTruncate is set to true all operations are affected.
        /// </summary>
        public static int Precision = 50;

        public static readonly BigDecimal MinusOne = new BigDecimal(-1, 0);
        public static readonly BigDecimal Zero = new BigDecimal(0, 0);
        public static readonly BigDecimal One = new BigDecimal(1, 0);

        public BigInteger Mantissa { get; set; }
        public int Exponent { get; set; }

        public BigDecimal(BigInteger mantissa, int exponent)
            : this()
        {
            Mantissa = mantissa;
            Exponent = exponent;
            Normalize();
            if (AlwaysTruncate)
            {
                Truncate();
            }
        }

        /// <summary>
        /// Removes trailing zeros on the mantissa
        /// </summary>
        public void Normalize()
        {
            if (Mantissa.IsZero)
            {
                Exponent = 0;
            }
            else
            {
                BigInteger remainder = 0;
                while (remainder == 0)
                {
                    var shortened = BigInteger.DivRem(Mantissa, 10, out remainder);
                    if (remainder == 0)
                    {
                        Mantissa = shortened;
                        Exponent++;
                    }
                }
            }
        }

        /// <summary>
        /// Truncate the number to the given precision by removing the least significant digits.
        /// </summary>
        /// <returns>The truncated number</returns>
        public BigDecimal Truncate(int precision)
        {
            // copy this instance (remember its a struct)
            var shortened = this;
            // save some time because the number of digits is not needed to remove trailing zeros
            shortened.Normalize();
            // remove the least significant digits, as long as the number of digits is higher than the given Precision
            while (NumberOfDigits(shortened.Mantissa) > precision)
            {
                shortened.Mantissa /= 10;
                shortened.Exponent++;
            }
            return shortened;
        }

        public BigDecimal Truncate()
        {
            return Truncate(Precision);
        }

        private static int NumberOfDigits(BigInteger value)
        {
            // do not count the sign
            return (value * value.Sign).ToString().Length;
        }

        #region Conversions

        public static implicit operator BigDecimal(int value)
        {
            return new BigDecimal(value, 0);
        }

        public static implicit operator BigDecimal(double value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            double scaleFactor = 1;
            while (Math.Abs(value * scaleFactor - (double)mantissa) > 0)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new BigDecimal(mantissa, exponent);
        }

        public static implicit operator BigDecimal(decimal value)
        {
            var mantissa = (BigInteger)value;
            var exponent = 0;
            decimal scaleFactor = 1;
            while ((decimal)mantissa != value * scaleFactor)
            {
                exponent -= 1;
                scaleFactor *= 10;
                mantissa = (BigInteger)(value * scaleFactor);
            }
            return new BigDecimal(mantissa, exponent);
        }

        public static explicit operator double(BigDecimal value)
        {
            return (double)value.Mantissa * Math.Pow(10, value.Exponent);
        }

        public static explicit operator float(BigDecimal value)
        {
            return Convert.ToSingle((double)value);
        }

        public static explicit operator decimal(BigDecimal value)
        {
            return (decimal)value.Mantissa * (decimal)Math.Pow(10, value.Exponent);
        }

        public static explicit operator int(BigDecimal value)
        {
            return (int)(value.Mantissa * BigInteger.Pow(10, value.Exponent));
        }

        public static explicit operator uint(BigDecimal value)
        {
            return (uint)(value.Mantissa * BigInteger.Pow(10, value.Exponent));
        }

        #endregion

        #region Operators

        public static BigDecimal operator +(BigDecimal value)
        {
            return value;
        }

        public static BigDecimal operator -(BigDecimal value)
        {
            value.Mantissa *= -1;
            return value;
        }

        public static BigDecimal operator ++(BigDecimal value)
        {
            return value + 1;
        }

        public static BigDecimal operator --(BigDecimal value)
        {
            return value - 1;
        }

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            return Add(left, right);
        }

        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            return Add(left, -right);
        }

        private static BigDecimal Add(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent
                ? new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
                : new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
        }

        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            return new BigDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);
        }

        public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor)
        {
            var exponentChange = Precision - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa));
            if (exponentChange < 0)
            {
                exponentChange = 0;
            }
            dividend.Mantissa *= BigInteger.Pow(10, exponentChange);
            return new BigDecimal(dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange);
        }

        public static bool operator ==(BigDecimal left, BigDecimal right)
        {
            return left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;
        }

        public static bool operator !=(BigDecimal left, BigDecimal right)
        {
            return left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;
        }

        public static bool operator <(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);
        }

        public static bool operator >(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);
        }

        public static bool operator <=(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);
        }

        public static bool operator >=(BigDecimal left, BigDecimal right)
        {
            return left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);
        }

        public static BigDecimal operator &(BigDecimal left, BigDecimal right)
        {
            BigInteger mantissa = left.Mantissa & right.Mantissa;
            int exponent = (left.Exponent > right.Exponent) ? left.Exponent : right.Exponent;
            return new BigDecimal(mantissa, exponent);
        }

        public static BigDecimal operator |(BigDecimal left, BigDecimal right)
        {
            BigInteger mantissa = left.Mantissa | right.Mantissa;
            int exponent = (left.Exponent > right.Exponent) ? left.Exponent : right.Exponent;
            return new BigDecimal(mantissa, exponent);
        }

        public static BigDecimal operator ^(BigDecimal left, BigDecimal right)
        {
            BigInteger mantissa = left.Mantissa ^ right.Mantissa;
            int exponent = (left.Exponent > right.Exponent) ? left.Exponent : right.Exponent;
            return new BigDecimal(mantissa, exponent);
        }

        public static BigDecimal operator ~(BigDecimal value)
        {
            return new BigDecimal(~value.Mantissa, value.Exponent);
        }

        public static BigDecimal operator <<(BigDecimal value, int positions)
        {
            return new BigDecimal(value.Mantissa << positions, value.Exponent);
        }

        public static BigDecimal operator >>(BigDecimal value, int positions)
        {
            return new BigDecimal(value.Mantissa >> positions, value.Exponent);
        }

        /// <summary>
        /// Returns the mantissa of value, aligned to the exponent of reference.
        /// Assumes the exponent of value is larger than of reference.
        /// </summary>
        private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference)
        {
            return value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent);
        }

        #endregion

        #region Additional mathematical functions

        public static BigDecimal Exp(double exponent)
        {
            var tmp = (BigDecimal)1;
            while (Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Exp(diff);
                exponent -= diff;
            }
            return tmp * Math.Exp(exponent);
        }

        public static BigDecimal Pow(double basis, double exponent)
        {
            var tmp = (BigDecimal)1;
            while (Math.Abs(exponent) > 100)
            {
                var diff = exponent > 0 ? 100 : -100;
                tmp *= Math.Pow(basis, diff);
                exponent -= diff;
            }
            return tmp * Math.Pow(basis, exponent);
        }

        public static BigDecimal Abs(BigDecimal value)
        {
            if (value < 0)
            {
                return -value;
            }
            return value;
        }

        public static BigDecimal Truncate(BigDecimal value)
        {
            if (value.Exponent >= 0)
            {
                return value;
            }

            while (value.Exponent < 0)
            {
                value.Mantissa /= 10;
                value.Exponent++;
            }

            return value;
        }

        public static BigDecimal Ceiling(BigDecimal value)
        {
            if (value.Exponent >= 0)
            {
                return value;
            }

            return BigDecimal.Truncate(value) + 1;
        }

        public static BigDecimal Floor(BigDecimal value)
        {
            return BigDecimal.Truncate(value);
        }

        public static BigDecimal Log(int logBase, BigDecimal value)
        {
            return Math.Log((double)value, logBase);
        }

        public static BigDecimal Ln(BigDecimal value)
        {
            return Math.Log((double)value, Math.E);
        }

        #endregion

        public override string ToString()
        {
            return string.Concat(Mantissa.ToString(), " * 10^", Exponent);
        }

        public bool Equals(BigDecimal other)
        {
            return other.Mantissa.Equals(Mantissa) && other.Exponent == Exponent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is BigDecimal && Equals((BigDecimal)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Mantissa.GetHashCode() * 397) ^ Exponent;
            }
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null) || !(obj is BigDecimal))
            {
                throw new ArgumentException();
            }
            return CompareTo((BigDecimal)obj);
        }

        public int CompareTo(BigDecimal other)
        {
            return this < other ? -1 : (this > other ? 1 : 0);
        }

        // Celarix's methods
        public static BigDecimal FromBinary(string binary)
        {
            BigInteger result = new BigInteger(0);
            
            for (int i = binary.Length - 1; i >= 0; i--)
            {
                BigInteger multiplier = BigInteger.Pow(2, binary.Length - 1 - i);

                if (binary[i] == '1')
                {
                    result += multiplier;
                }
            }

            return new BigDecimal(result, 0);
        }

        public static BigDecimal FromHexadecimal(string hexadecimal)
        {
            BigInteger result = new BigInteger(0);

            for (int i = hexadecimal.Length - 1; i >= 0; i--)
            {
                BigInteger multiplier = BigInteger.Pow(16, hexadecimal.Length - 1 - i);

                if (char.IsDigit(hexadecimal[i]))
                {
                    result += multiplier * (int)(hexadecimal[i] - 0x30);
                }
                else if (hexadecimal[i] >= 'a' && hexadecimal[i] <= 'f')
                {
                    result += multiplier * (int)(hexadecimal[i] - 0x61);
                }
                else if (hexadecimal[i] >= 'A' && hexadecimal[i] <= 'F')
                {
                    result += multiplier * (int)(hexadecimal[i] + 0x0A - 0x41);
                }
            }

            return new BigDecimal(result, 0);
        }

        public static BigDecimal FromOctal(string octal)
        {
            BigInteger result = new BigInteger(0);

            for (int i = octal.Length - 1; i >= 0; i--)
            {
                BigInteger multiplier = BigInteger.Pow(8, octal.Length - 1 - i);

                if (octal[i] >= '0' && octal[i] <= '7')
                {
                    result += multiplier * (int)(octal[i] - 0x30);
                }
            }

            return new BigDecimal(result, 0);
        }

        public static BigDecimal Parse(string value)
        {
            BigInteger digits = BigInteger.Parse(value.Replace(".", ""));

            if (!value.Contains("."))
            {
                return new BigDecimal(digits, 0);
            }

            int exponent = value.Length - 1 - value.IndexOf('.');
            return new BigDecimal(digits, -exponent);
        }

        public static bool TryParse(string value, out BigDecimal result)
        {
            BigInteger digits;
            if (!BigInteger.TryParse(value.Replace(".", ""), out digits))
            {
                result = new BigDecimal(0, 0);
                return false;
            }

            if (!value.Contains("."))
            {
                result = new BigDecimal(digits, 0);
                return true;
            }

            int exponent = value.Length - 1 - value.IndexOf('.');
            result = new BigDecimal(digits, -exponent);
            return true;
        }
    }
}