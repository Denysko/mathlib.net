using System;

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.apache.commons.math3.fraction
{


	using org.apache.commons.math3;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using ArithmeticUtils = org.apache.commons.math3.util.ArithmeticUtils;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Representation of a rational number without any overflow. This class is
	/// immutable.
	/// 
	/// @version $Id: BigFraction.java 1547633 2013-12-03 23:03:06Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class BigFraction : Number, FieldElement<BigFraction>, IComparable<BigFraction>
	{

		/// <summary>
		/// A fraction representing "2 / 1". </summary>
		public static readonly BigFraction TWO = new BigFraction(2);

		/// <summary>
		/// A fraction representing "1". </summary>
		public static readonly BigFraction ONE = new BigFraction(1);

		/// <summary>
		/// A fraction representing "0". </summary>
		public static readonly BigFraction ZERO = new BigFraction(0);

		/// <summary>
		/// A fraction representing "-1 / 1". </summary>
		public static readonly BigFraction MINUS_ONE = new BigFraction(-1);

		/// <summary>
		/// A fraction representing "4/5". </summary>
		public static readonly BigFraction FOUR_FIFTHS = new BigFraction(4, 5);

		/// <summary>
		/// A fraction representing "1/5". </summary>
		public static readonly BigFraction ONE_FIFTH = new BigFraction(1, 5);

		/// <summary>
		/// A fraction representing "1/2". </summary>
		public static readonly BigFraction ONE_HALF = new BigFraction(1, 2);

		/// <summary>
		/// A fraction representing "1/4". </summary>
		public static readonly BigFraction ONE_QUARTER = new BigFraction(1, 4);

		/// <summary>
		/// A fraction representing "1/3". </summary>
		public static readonly BigFraction ONE_THIRD = new BigFraction(1, 3);

		/// <summary>
		/// A fraction representing "3/5". </summary>
		public static readonly BigFraction THREE_FIFTHS = new BigFraction(3, 5);

		/// <summary>
		/// A fraction representing "3/4". </summary>
		public static readonly BigFraction THREE_QUARTERS = new BigFraction(3, 4);

		/// <summary>
		/// A fraction representing "2/5". </summary>
		public static readonly BigFraction TWO_FIFTHS = new BigFraction(2, 5);

		/// <summary>
		/// A fraction representing "2/4". </summary>
		public static readonly BigFraction TWO_QUARTERS = new BigFraction(2, 4);

		/// <summary>
		/// A fraction representing "2/3". </summary>
		public static readonly BigFraction TWO_THIRDS = new BigFraction(2, 3);

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -5630213147331578515L;

		/// <summary>
		/// <code>BigInteger</code> representation of 100. </summary>
		private static readonly System.Numerics.BigInteger ONE_HUNDRED = System.Numerics.BigInteger.valueOf(100);

		/// <summary>
		/// The numerator. </summary>
		private readonly System.Numerics.BigInteger numerator;

		/// <summary>
		/// The denominator. </summary>
		private readonly System.Numerics.BigInteger denominator;

		/// <summary>
		/// <p>
		/// Create a <seealso cref="BigFraction"/> equivalent to the passed <tt>BigInteger</tt>, ie
		/// "num / 1".
		/// </p>
		/// </summary>
		/// <param name="num">
		///            the numerator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction(final java.math.BigInteger num)
		public BigFraction(System.Numerics.BigInteger num) : this(num, System.Numerics.BigInteger.ONE)
		{
		}

		/// <summary>
		/// Create a <seealso cref="BigFraction"/> given the numerator and denominator as
		/// {@code BigInteger}. The <seealso cref="BigFraction"/> is reduced to lowest terms.
		/// </summary>
		/// <param name="num"> the numerator, must not be {@code null}. </param>
		/// <param name="den"> the denominator, must not be {@code null}. </param>
		/// <exception cref="ZeroException"> if the denominator is zero. </exception>
		/// <exception cref="NullArgumentException"> if either of the arguments is null </exception>
		public BigFraction(System.Numerics.BigInteger num, System.Numerics.BigInteger den)
		{
			MathUtils.checkNotNull(num, LocalizedFormats.NUMERATOR);
			MathUtils.checkNotNull(den, LocalizedFormats.DENOMINATOR);
			if (System.Numerics.BigInteger.ZERO.Equals(den))
			{
				throw new ZeroException(LocalizedFormats.ZERO_DENOMINATOR);
			}
			if (System.Numerics.BigInteger.ZERO.Equals(num))
			{
				numerator = System.Numerics.BigInteger.ZERO;
				denominator = System.Numerics.BigInteger.ONE;
			}
			else
			{

				// reduce numerator and denominator by greatest common denominator
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.math.BigInteger gcd = num.gcd(den);
				System.Numerics.BigInteger gcd = num.gcd(den);
				if (System.Numerics.BigInteger.ONE.compareTo(gcd) < 0)
				{
					num = num / gcd;
					den = den / gcd;
				}

				// move sign to numerator
				if (System.Numerics.BigInteger.ZERO.compareTo(den) > 0)
				{
					num = -num;
					den = -den;
				}

				// store the values in the final fields
				numerator = num;
				denominator = den;

			}
		}

		/// <summary>
		/// Create a fraction given the double value.
		/// <p>
		/// This constructor behaves <em>differently</em> from
		/// <seealso cref="#BigFraction(double, double, int)"/>. It converts the double value
		/// exactly, considering its internal bits representation. This works for all
		/// values except NaN and infinities and does not requires any loop or
		/// convergence threshold.
		/// </p>
		/// <p>
		/// Since this conversion is exact and since double numbers are sometimes
		/// approximated, the fraction created may seem strange in some cases. For example,
		/// calling <code>new BigFraction(1.0 / 3.0)</code> does <em>not</em> create
		/// the fraction 1/3, but the fraction 6004799503160661 / 18014398509481984
		/// because the double number passed to the constructor is not exactly 1/3
		/// (this number cannot be stored exactly in IEEE754).
		/// </p> </summary>
		/// <seealso cref= #BigFraction(double, double, int) </seealso>
		/// <param name="value"> the double value to convert to a fraction. </param>
		/// <exception cref="MathIllegalArgumentException"> if value is NaN or infinite </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BigFraction(final double value) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BigFraction(double value)
		{
			if (double.IsNaN(value))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NAN_VALUE_CONVERSION);
			}
			if (double.IsInfinity(value))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.INFINITE_VALUE_CONVERSION);
			}

			// compute m and k such that value = m * 2^k
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long bits = Double.doubleToLongBits(value);
			long bits = double.doubleToLongBits(value);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long sign = bits & 0x8000000000000000L;
			long sign = bits & 0x8000000000000000L;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long exponent = bits & 0x7ff0000000000000L;
			long exponent = bits & 0x7ff0000000000000L;
			long m = bits & 0x000fffffffffffffL;
			if (exponent != 0)
			{
				// this was a normalized number, add the implicit most significant bit
				m |= 0x0010000000000000L;
			}
			if (sign != 0)
			{
				m = -m;
			}
			int k = ((int)(exponent >> 52)) - 1075;
			while (((m & 0x001ffffffffffffeL) != 0) && ((m & 0x1) == 0))
			{
				m >>= 1;
				++k;
			}

			if (k < 0)
			{
				numerator = System.Numerics.BigInteger.valueOf(m);
				denominator = System.Numerics.BigInteger.ZERO.flipBit(-k);
			}
			else
			{
				numerator = System.Numerics.BigInteger.valueOf(m) * System.Numerics.BigInteger.ZERO.flipBit(k);
				denominator = System.Numerics.BigInteger.ONE;
			}

		}

		/// <summary>
		/// Create a fraction given the double value and maximum error allowed.
		/// <p>
		/// References:
		/// <ul>
		/// <li><a href="http://mathworld.wolfram.com/ContinuedFraction.html">
		/// Continued Fraction</a> equations (11) and (22)-(26)</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="value">
		///            the double value to convert to a fraction. </param>
		/// <param name="epsilon">
		///            maximum error allowed. The resulting fraction is within
		///            <code>epsilon</code> of <code>value</code>, in absolute terms. </param>
		/// <param name="maxIterations">
		///            maximum number of convergents. </param>
		/// <exception cref="FractionConversionException">
		///             if the continued fraction failed to converge. </exception>
		/// <seealso cref= #BigFraction(double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BigFraction(final double value, final double epsilon, final int maxIterations) throws FractionConversionException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BigFraction(double value, double epsilon, int maxIterations) : this(value, epsilon, int.MaxValue, maxIterations)
		{
		}

		/// <summary>
		/// Create a fraction given the double value and either the maximum error
		/// allowed or the maximum number of denominator digits.
		/// <p>
		/// 
		/// NOTE: This constructor is called with EITHER - a valid epsilon value and
		/// the maxDenominator set to Integer.MAX_VALUE (that way the maxDenominator
		/// has no effect). OR - a valid maxDenominator value and the epsilon value
		/// set to zero (that way epsilon only has effect if there is an exact match
		/// before the maxDenominator value is reached).
		/// </p>
		/// <p>
		/// 
		/// It has been done this way so that the same code can be (re)used for both
		/// scenarios. However this could be confusing to users if it were part of
		/// the public API and this constructor should therefore remain PRIVATE.
		/// </p>
		/// 
		/// See JIRA issue ticket MATH-181 for more details:
		/// 
		/// https://issues.apache.org/jira/browse/MATH-181
		/// </summary>
		/// <param name="value">
		///            the double value to convert to a fraction. </param>
		/// <param name="epsilon">
		///            maximum error allowed. The resulting fraction is within
		///            <code>epsilon</code> of <code>value</code>, in absolute terms. </param>
		/// <param name="maxDenominator">
		///            maximum denominator value allowed. </param>
		/// <param name="maxIterations">
		///            maximum number of convergents. </param>
		/// <exception cref="FractionConversionException">
		///             if the continued fraction failed to converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private BigFraction(final double value, final double epsilon, final int maxDenominator, int maxIterations) throws FractionConversionException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private BigFraction(double value, double epsilon, int maxDenominator, int maxIterations)
		{
			long overflow = int.MaxValue;
			double r0 = value;
			long a0 = (long) FastMath.floor(r0);

			if (FastMath.abs(a0) > overflow)
			{
				throw new FractionConversionException(value, a0, 1l);
			}

			// check for (almost) integer arguments, which should not go
			// to iterations.
			if (FastMath.abs(a0 - value) < epsilon)
			{
				numerator = System.Numerics.BigInteger.valueOf(a0);
				denominator = System.Numerics.BigInteger.ONE;
				return;
			}

			long p0 = 1;
			long q0 = 0;
			long p1 = a0;
			long q1 = 1;

			long p2 = 0;
			long q2 = 1;

			int n = 0;
			bool stop = false;
			do
			{
				++n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r1 = 1.0 / (r0 - a0);
				double r1 = 1.0 / (r0 - a0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long a1 = (long) org.apache.commons.math3.util.FastMath.floor(r1);
				long a1 = (long) FastMath.floor(r1);
				p2 = (a1 * p1) + p0;
				q2 = (a1 * q1) + q0;
				if ((p2 > overflow) || (q2 > overflow))
				{
					// in maxDenominator mode, if the last fraction was very close to the actual value
					// q2 may overflow in the next iteration; in this case return the last one.
					if (epsilon == 0.0 && FastMath.abs(q1) < maxDenominator)
					{
						break;
					}
					throw new FractionConversionException(value, p2, q2);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double convergent = (double) p2 / (double) q2;
				double convergent = (double) p2 / (double) q2;
				if ((n < maxIterations) && (FastMath.abs(convergent - value) > epsilon) && (q2 < maxDenominator))
				{
					p0 = p1;
					p1 = p2;
					q0 = q1;
					q1 = q2;
					a0 = a1;
					r0 = r1;
				}
				else
				{
					stop = true;
				}
			} while (!stop);

			if (n >= maxIterations)
			{
				throw new FractionConversionException(value, maxIterations);
			}

			if (q2 < maxDenominator)
			{
				numerator = System.Numerics.BigInteger.valueOf(p2);
				denominator = System.Numerics.BigInteger.valueOf(q2);
			}
			else
			{
				numerator = System.Numerics.BigInteger.valueOf(p1);
				denominator = System.Numerics.BigInteger.valueOf(q1);
			}
		}

		/// <summary>
		/// Create a fraction given the double value and maximum denominator.
		/// <p>
		/// References:
		/// <ul>
		/// <li><a href="http://mathworld.wolfram.com/ContinuedFraction.html">
		/// Continued Fraction</a> equations (11) and (22)-(26)</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="value">
		///            the double value to convert to a fraction. </param>
		/// <param name="maxDenominator">
		///            The maximum allowed value for denominator. </param>
		/// <exception cref="FractionConversionException">
		///             if the continued fraction failed to converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BigFraction(final double value, final int maxDenominator) throws FractionConversionException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BigFraction(double value, int maxDenominator) : this(value, 0, maxDenominator, 100)
		{
		}

		/// <summary>
		/// <p>
		/// Create a <seealso cref="BigFraction"/> equivalent to the passed <tt>int</tt>, ie
		/// "num / 1".
		/// </p>
		/// </summary>
		/// <param name="num">
		///            the numerator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction(final int num)
		public BigFraction(int num) : this(System.Numerics.BigInteger.valueOf(num), System.Numerics.BigInteger.ONE)
		{
		}

		/// <summary>
		/// <p>
		/// Create a <seealso cref="BigFraction"/> given the numerator and denominator as simple
		/// <tt>int</tt>. The <seealso cref="BigFraction"/> is reduced to lowest terms.
		/// </p>
		/// </summary>
		/// <param name="num">
		///            the numerator. </param>
		/// <param name="den">
		///            the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction(final int num, final int den)
		public BigFraction(int num, int den) : this(System.Numerics.BigInteger.valueOf(num), System.Numerics.BigInteger.valueOf(den))
		{
		}

		/// <summary>
		/// <p>
		/// Create a <seealso cref="BigFraction"/> equivalent to the passed long, ie "num / 1".
		/// </p>
		/// </summary>
		/// <param name="num">
		///            the numerator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction(final long num)
		public BigFraction(long num) : this(System.Numerics.BigInteger.valueOf(num), System.Numerics.BigInteger.ONE)
		{
		}

		/// <summary>
		/// <p>
		/// Create a <seealso cref="BigFraction"/> given the numerator and denominator as simple
		/// <tt>long</tt>. The <seealso cref="BigFraction"/> is reduced to lowest terms.
		/// </p>
		/// </summary>
		/// <param name="num">
		///            the numerator. </param>
		/// <param name="den">
		///            the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction(final long num, final long den)
		public BigFraction(long num, long den) : this(System.Numerics.BigInteger.valueOf(num), System.Numerics.BigInteger.valueOf(den))
		{
		}

		/// <summary>
		/// <p>
		/// Creates a <code>BigFraction</code> instance with the 2 parts of a fraction
		/// Y/Z.
		/// </p>
		/// 
		/// <p>
		/// Any negative signs are resolved to be on the numerator.
		/// </p>
		/// </summary>
		/// <param name="numerator">
		///            the numerator, for example the three in 'three sevenths'. </param>
		/// <param name="denominator">
		///            the denominator, for example the seven in 'three sevenths'. </param>
		/// <returns> a new fraction instance, with the numerator and denominator
		///         reduced. </returns>
		/// <exception cref="ArithmeticException">
		///             if the denominator is <code>zero</code>. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static BigFraction getReducedFraction(final int numerator, final int denominator)
		public static BigFraction getReducedFraction(int numerator, int denominator)
		{
			if (numerator == 0)
			{
				return ZERO; // normalize zero.
			}

			return new BigFraction(numerator, denominator);
		}

		/// <summary>
		/// <p>
		/// Returns the absolute value of this <seealso cref="BigFraction"/>.
		/// </p>
		/// </summary>
		/// <returns> the absolute value as a <seealso cref="BigFraction"/>. </returns>
		public virtual BigFraction abs()
		{
			return (System.Numerics.BigInteger.ZERO.compareTo(numerator) <= 0) ? this : negate();
		}

		/// <summary>
		/// <p>
		/// Adds the value of this fraction to the passed <seealso cref="BigInteger"/>,
		/// returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="bg">
		///            the <seealso cref="BigInteger"/> to add, must'nt be <code>null</code>. </param>
		/// <returns> a <code>BigFraction</code> instance with the resulting values. </returns>
		/// <exception cref="NullArgumentException">
		///             if the <seealso cref="BigInteger"/> is <code>null</code>. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BigFraction add(final java.math.BigInteger bg) throws org.apache.commons.math3.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual BigFraction add(System.Numerics.BigInteger bg)
		{
			MathUtils.checkNotNull(bg);
			return new BigFraction(numerator + denominator * bg, denominator);
		}

		/// <summary>
		/// <p>
		/// Adds the value of this fraction to the passed <tt>integer</tt>, returning
		/// the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="i">
		///            the <tt>integer</tt> to add. </param>
		/// <returns> a <code>BigFraction</code> instance with the resulting values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction add(final int i)
		public virtual BigFraction add(int i)
		{
			return add(System.Numerics.BigInteger.valueOf(i));
		}

		/// <summary>
		/// <p>
		/// Adds the value of this fraction to the passed <tt>long</tt>, returning
		/// the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="l">
		///            the <tt>long</tt> to add. </param>
		/// <returns> a <code>BigFraction</code> instance with the resulting values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction add(final long l)
		public virtual BigFraction add(long l)
		{
			return add(System.Numerics.BigInteger.valueOf(l));
		}

		/// <summary>
		/// <p>
		/// Adds the value of this fraction to another, returning the result in
		/// reduced form.
		/// </p>
		/// </summary>
		/// <param name="fraction">
		///            the <seealso cref="BigFraction"/> to add, must not be <code>null</code>. </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values. </returns>
		/// <exception cref="NullArgumentException"> if the <seealso cref="BigFraction"/> is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction add(final BigFraction fraction)
		public virtual BigFraction add(BigFraction fraction)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (ZERO.Equals(fraction))
			{
				return this;
			}

			System.Numerics.BigInteger num = null;
			System.Numerics.BigInteger den = null;

			if (denominator.Equals(fraction.denominator))
			{
				num = numerator + fraction.numerator;
				den = denominator;
			}
			else
			{
				num = (numerator * fraction.denominator).add((fraction.numerator).multiply(denominator));
				den = denominator * fraction.denominator;
			}
			return new BigFraction(num, den);

		}

		/// <summary>
		/// <p>
		/// Gets the fraction as a <code>BigDecimal</code>. This calculates the
		/// fraction as the numerator divided by denominator.
		/// </p>
		/// </summary>
		/// <returns> the fraction as a <code>BigDecimal</code>. </returns>
		/// <exception cref="ArithmeticException">
		///             if the exact quotient does not have a terminating decimal
		///             expansion. </exception>
		/// <seealso cref= BigDecimal </seealso>
		public virtual decimal bigDecimalValue()
		{
			return (new decimal(numerator)) / (new decimal(denominator));
		}

		/// <summary>
		/// <p>
		/// Gets the fraction as a <code>BigDecimal</code> following the passed
		/// rounding mode. This calculates the fraction as the numerator divided by
		/// denominator.
		/// </p>
		/// </summary>
		/// <param name="roundingMode">
		///            rounding mode to apply. see <seealso cref="BigDecimal"/> constants. </param>
		/// <returns> the fraction as a <code>BigDecimal</code>. </returns>
		/// <exception cref="IllegalArgumentException">
		///             if <tt>roundingMode</tt> does not represent a valid rounding
		///             mode. </exception>
		/// <seealso cref= BigDecimal </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public java.math.BigDecimal bigDecimalValue(final int roundingMode)
		public virtual decimal bigDecimalValue(int roundingMode)
		{
			return (new decimal(numerator)).divide(new decimal(denominator), roundingMode);
		}

		/// <summary>
		/// <p>
		/// Gets the fraction as a <code>BigDecimal</code> following the passed scale
		/// and rounding mode. This calculates the fraction as the numerator divided
		/// by denominator.
		/// </p>
		/// </summary>
		/// <param name="scale">
		///            scale of the <code>BigDecimal</code> quotient to be returned.
		///            see <seealso cref="BigDecimal"/> for more information. </param>
		/// <param name="roundingMode">
		///            rounding mode to apply. see <seealso cref="BigDecimal"/> constants. </param>
		/// <returns> the fraction as a <code>BigDecimal</code>. </returns>
		/// <seealso cref= BigDecimal </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public java.math.BigDecimal bigDecimalValue(final int scale, final int roundingMode)
		public virtual decimal bigDecimalValue(int scale, int roundingMode)
		{
			return (new decimal(numerator)).divide(new decimal(denominator), scale, roundingMode);
		}

		/// <summary>
		/// <p>
		/// Compares this object to another based on size.
		/// </p>
		/// </summary>
		/// <param name="object">
		///            the object to compare to, must not be <code>null</code>. </param>
		/// <returns> -1 if this is less than <tt>object</tt>, +1 if this is greater
		///         than <tt>object</tt>, 0 if they are equal. </returns>
		/// <seealso cref= java.lang.Comparable#compareTo(java.lang.Object) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compareTo(final BigFraction object)
		public virtual int CompareTo(BigFraction @object)
		{
			System.Numerics.BigInteger nOd = numerator * @object.denominator;
			System.Numerics.BigInteger dOn = denominator * @object.numerator;
			return nOd.compareTo(dOn);
		}

		/// <summary>
		/// <p>
		/// Divide the value of this fraction by the passed {@code BigInteger},
		/// ie {@code this * 1 / bg}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="bg"> the {@code BigInteger} to divide by, must not be {@code null} </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values </returns>
		/// <exception cref="NullArgumentException"> if the {@code BigInteger} is {@code null} </exception>
		/// <exception cref="MathArithmeticException"> if the fraction to divide by is zero </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction divide(final java.math.BigInteger bg)
		public virtual BigFraction divide(System.Numerics.BigInteger bg)
		{
			if (bg == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (System.Numerics.BigInteger.ZERO.Equals(bg))
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_DENOMINATOR);
			}
			return new BigFraction(numerator, denominator * bg);
		}

		/// <summary>
		/// <p>
		/// Divide the value of this fraction by the passed {@code int}, ie
		/// {@code this * 1 / i}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="i"> the {@code int} to divide by </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values </returns>
		/// <exception cref="MathArithmeticException"> if the fraction to divide by is zero </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction divide(final int i)
		public virtual BigFraction divide(int i)
		{
			return divide(System.Numerics.BigInteger.valueOf(i));
		}

		/// <summary>
		/// <p>
		/// Divide the value of this fraction by the passed {@code long}, ie
		/// {@code this * 1 / l}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="l"> the {@code long} to divide by </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values </returns>
		/// <exception cref="MathArithmeticException"> if the fraction to divide by is zero </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction divide(final long l)
		public virtual BigFraction divide(long l)
		{
			return divide(System.Numerics.BigInteger.valueOf(l));
		}

		/// <summary>
		/// <p>
		/// Divide the value of this fraction by another, returning the result in
		/// reduced form.
		/// </p>
		/// </summary>
		/// <param name="fraction"> Fraction to divide by, must not be {@code null}. </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values. </returns>
		/// <exception cref="NullArgumentException"> if the {@code fraction} is {@code null}. </exception>
		/// <exception cref="MathArithmeticException"> if the fraction to divide by is zero </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction divide(final BigFraction fraction)
		public virtual BigFraction divide(BigFraction fraction)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (System.Numerics.BigInteger.ZERO.Equals(fraction.numerator))
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_DENOMINATOR);
			}

			return multiply(fraction.reciprocal());
		}

		/// <summary>
		/// <p>
		/// Gets the fraction as a <tt>double</tt>. This calculates the fraction as
		/// the numerator divided by denominator.
		/// </p>
		/// </summary>
		/// <returns> the fraction as a <tt>double</tt> </returns>
		/// <seealso cref= java.lang.Number#doubleValue() </seealso>
		public override double doubleValue()
		{
			double result = (double)numerator / (double)denominator;
			if (double.IsNaN(result))
			{
				// Numerator and/or denominator must be out of range:
				// Calculate how far to shift them to put them in range.
				int shift = FastMath.max(numerator.bitLength(), denominator.bitLength()) - FastMath.getExponent(double.MaxValue);
				result = (double)numerator.shiftRight(shift) / (double)denominator.shiftRight(shift);
			}
			return result;
		}

		/// <summary>
		/// <p>
		/// Test for the equality of two fractions. If the lowest term numerator and
		/// denominators are the same for both fractions, the two fractions are
		/// considered to be equal.
		/// </p>
		/// </summary>
		/// <param name="other">
		///            fraction to test for equality to this fraction, can be
		///            <code>null</code>. </param>
		/// <returns> true if two fractions are equal, false if object is
		///         <code>null</code>, not an instance of <seealso cref="BigFraction"/>, or not
		///         equal to this fraction instance. </returns>
		/// <seealso cref= java.lang.Object#equals(java.lang.Object) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object other)
		public override bool Equals(object other)
		{
			bool ret = false;

			if (this == other)
			{
				ret = true;
			}
			else if (other is BigFraction)
			{
				BigFraction rhs = ((BigFraction) other).reduce();
				BigFraction thisOne = this.reduce();
				ret = thisOne.numerator.Equals(rhs.numerator) && thisOne.denominator.Equals(rhs.denominator);
			}

			return ret;
		}

		/// <summary>
		/// <p>
		/// Gets the fraction as a <tt>float</tt>. This calculates the fraction as
		/// the numerator divided by denominator.
		/// </p>
		/// </summary>
		/// <returns> the fraction as a <tt>float</tt>. </returns>
		/// <seealso cref= java.lang.Number#floatValue() </seealso>
		public override float floatValue()
		{
			float result = (float)numerator / (float)denominator;
			if (double.IsNaN(result))
			{
				// Numerator and/or denominator must be out of range:
				// Calculate how far to shift them to put them in range.
				int shift = FastMath.max(numerator.bitLength(), denominator.bitLength()) - FastMath.getExponent(float.MaxValue);
				result = (float)numerator.shiftRight(shift) / (float)denominator.shiftRight(shift);
			}
			return result;
		}

		/// <summary>
		/// <p>
		/// Access the denominator as a <code>BigInteger</code>.
		/// </p>
		/// </summary>
		/// <returns> the denominator as a <code>BigInteger</code>. </returns>
		public virtual System.Numerics.BigInteger Denominator
		{
			get
			{
				return denominator;
			}
		}

		/// <summary>
		/// <p>
		/// Access the denominator as a <tt>int</tt>.
		/// </p>
		/// </summary>
		/// <returns> the denominator as a <tt>int</tt>. </returns>
		public virtual int DenominatorAsInt
		{
			get
			{
				return (int)denominator;
			}
		}

		/// <summary>
		/// <p>
		/// Access the denominator as a <tt>long</tt>.
		/// </p>
		/// </summary>
		/// <returns> the denominator as a <tt>long</tt>. </returns>
		public virtual long DenominatorAsLong
		{
			get
			{
				return (long)denominator;
			}
		}

		/// <summary>
		/// <p>
		/// Access the numerator as a <code>BigInteger</code>.
		/// </p>
		/// </summary>
		/// <returns> the numerator as a <code>BigInteger</code>. </returns>
		public virtual System.Numerics.BigInteger Numerator
		{
			get
			{
				return numerator;
			}
		}

		/// <summary>
		/// <p>
		/// Access the numerator as a <tt>int</tt>.
		/// </p>
		/// </summary>
		/// <returns> the numerator as a <tt>int</tt>. </returns>
		public virtual int NumeratorAsInt
		{
			get
			{
				return (int)numerator;
			}
		}

		/// <summary>
		/// <p>
		/// Access the numerator as a <tt>long</tt>.
		/// </p>
		/// </summary>
		/// <returns> the numerator as a <tt>long</tt>. </returns>
		public virtual long NumeratorAsLong
		{
			get
			{
				return (long)numerator;
			}
		}

		/// <summary>
		/// <p>
		/// Gets a hashCode for the fraction.
		/// </p>
		/// </summary>
		/// <returns> a hash code value for this object. </returns>
		/// <seealso cref= java.lang.Object#hashCode() </seealso>
		public override int GetHashCode()
		{
			return 37 * (37 * 17 + numerator.GetHashCode()) + denominator.GetHashCode();
		}

		/// <summary>
		/// <p>
		/// Gets the fraction as an <tt>int</tt>. This returns the whole number part
		/// of the fraction.
		/// </p>
		/// </summary>
		/// <returns> the whole number fraction part. </returns>
		/// <seealso cref= java.lang.Number#intValue() </seealso>
		public override int intValue()
		{
			return numerator / (int)denominator;
		}

		/// <summary>
		/// <p>
		/// Gets the fraction as a <tt>long</tt>. This returns the whole number part
		/// of the fraction.
		/// </p>
		/// </summary>
		/// <returns> the whole number fraction part. </returns>
		/// <seealso cref= java.lang.Number#longValue() </seealso>
		public override long longValue()
		{
			return numerator / (long)denominator;
		}

		/// <summary>
		/// <p>
		/// Multiplies the value of this fraction by the passed
		/// <code>BigInteger</code>, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="bg"> the {@code BigInteger} to multiply by. </param>
		/// <returns> a {@code BigFraction} instance with the resulting values. </returns>
		/// <exception cref="NullArgumentException"> if {@code bg} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction multiply(final java.math.BigInteger bg)
		public virtual BigFraction multiply(System.Numerics.BigInteger bg)
		{
			if (bg == null)
			{
				throw new NullArgumentException();
			}
			return new BigFraction(bg * numerator, denominator);
		}

		/// <summary>
		/// <p>
		/// Multiply the value of this fraction by the passed <tt>int</tt>, returning
		/// the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="i">
		///            the <tt>int</tt> to multiply by. </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction multiply(final int i)
		public virtual BigFraction multiply(int i)
		{
			return multiply(System.Numerics.BigInteger.valueOf(i));
		}

		/// <summary>
		/// <p>
		/// Multiply the value of this fraction by the passed <tt>long</tt>,
		/// returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="l">
		///            the <tt>long</tt> to multiply by. </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction multiply(final long l)
		public virtual BigFraction multiply(long l)
		{
			return multiply(System.Numerics.BigInteger.valueOf(l));
		}

		/// <summary>
		/// <p>
		/// Multiplies the value of this fraction by another, returning the result in
		/// reduced form.
		/// </p>
		/// </summary>
		/// <param name="fraction"> Fraction to multiply by, must not be {@code null}. </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values. </returns>
		/// <exception cref="NullArgumentException"> if {@code fraction} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction multiply(final BigFraction fraction)
		public virtual BigFraction multiply(BigFraction fraction)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (numerator.Equals(System.Numerics.BigInteger.ZERO) || fraction.numerator.Equals(System.Numerics.BigInteger.ZERO))
			{
				return ZERO;
			}
			return new BigFraction(numerator * fraction.numerator, denominator * fraction.denominator);
		}

		/// <summary>
		/// <p>
		/// Return the additive inverse of this fraction, returning the result in
		/// reduced form.
		/// </p>
		/// </summary>
		/// <returns> the negation of this fraction. </returns>
		public virtual BigFraction negate()
		{
			return new BigFraction(-numerator, denominator);
		}

		/// <summary>
		/// <p>
		/// Gets the fraction percentage as a <tt>double</tt>. This calculates the
		/// fraction as the numerator divided by denominator multiplied by 100.
		/// </p>
		/// </summary>
		/// <returns> the fraction percentage as a <tt>double</tt>. </returns>
		public virtual double percentageValue()
		{
			return (double)multiply(ONE_HUNDRED);
		}

		/// <summary>
		/// <p>
		/// Returns a {@code BigFraction} whose value is
		/// {@code (this<sup>exponent</sup>)}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="exponent">
		///            exponent to which this {@code BigFraction} is to be
		///            raised. </param>
		/// <returns> <tt>this<sup>exponent</sup></tt>. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction pow(final int exponent)
		public virtual BigFraction pow(int exponent)
		{
			if (exponent < 0)
			{
				return new BigFraction(denominator.pow(-exponent), numerator.pow(-exponent));
			}
			return new BigFraction(numerator.pow(exponent), denominator.pow(exponent));
		}

		/// <summary>
		/// <p>
		/// Returns a <code>BigFraction</code> whose value is
		/// <tt>(this<sup>exponent</sup>)</tt>, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="exponent">
		///            exponent to which this <code>BigFraction</code> is to be raised. </param>
		/// <returns> <tt>this<sup>exponent</sup></tt> as a <code>BigFraction</code>. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction pow(final long exponent)
		public virtual BigFraction pow(long exponent)
		{
			if (exponent < 0)
			{
				return new BigFraction(ArithmeticUtils.pow(denominator, -exponent), ArithmeticUtils.pow(numerator, -exponent));
			}
			return new BigFraction(ArithmeticUtils.pow(numerator, exponent), ArithmeticUtils.pow(denominator, exponent));
		}

		/// <summary>
		/// <p>
		/// Returns a <code>BigFraction</code> whose value is
		/// <tt>(this<sup>exponent</sup>)</tt>, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="exponent">
		///            exponent to which this <code>BigFraction</code> is to be raised. </param>
		/// <returns> <tt>this<sup>exponent</sup></tt> as a <code>BigFraction</code>. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction pow(final java.math.BigInteger exponent)
		public virtual BigFraction pow(System.Numerics.BigInteger exponent)
		{
			if (exponent.compareTo(System.Numerics.BigInteger.ZERO) < 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.math.BigInteger eNeg = exponent.negate();
				System.Numerics.BigInteger eNeg = -exponent;
				return new BigFraction(ArithmeticUtils.pow(denominator, eNeg), ArithmeticUtils.pow(numerator, eNeg));
			}
			return new BigFraction(ArithmeticUtils.pow(numerator, exponent), ArithmeticUtils.pow(denominator, exponent));
		}

		/// <summary>
		/// <p>
		/// Returns a <code>double</code> whose value is
		/// <tt>(this<sup>exponent</sup>)</tt>, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="exponent">
		///            exponent to which this <code>BigFraction</code> is to be raised. </param>
		/// <returns> <tt>this<sup>exponent</sup></tt>. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double pow(final double exponent)
		public virtual double pow(double exponent)
		{
			return FastMath.pow((double)numerator, exponent) / FastMath.pow((double)denominator, exponent);
		}

		/// <summary>
		/// <p>
		/// Return the multiplicative inverse of this fraction.
		/// </p>
		/// </summary>
		/// <returns> the reciprocal fraction. </returns>
		public virtual BigFraction reciprocal()
		{
			return new BigFraction(denominator, numerator);
		}

		/// <summary>
		/// <p>
		/// Reduce this <code>BigFraction</code> to its lowest terms.
		/// </p>
		/// </summary>
		/// <returns> the reduced <code>BigFraction</code>. It doesn't change anything if
		///         the fraction can be reduced. </returns>
		public virtual BigFraction reduce()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.math.BigInteger gcd = numerator.gcd(denominator);
			System.Numerics.BigInteger gcd = numerator.gcd(denominator);
			return new BigFraction(numerator / gcd, denominator / gcd);
		}

		/// <summary>
		/// <p>
		/// Subtracts the value of an <seealso cref="BigInteger"/> from the value of this
		/// {@code BigFraction}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="bg"> the <seealso cref="BigInteger"/> to subtract, cannot be {@code null}. </param>
		/// <returns> a {@code BigFraction} instance with the resulting values. </returns>
		/// <exception cref="NullArgumentException"> if the <seealso cref="BigInteger"/> is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction subtract(final java.math.BigInteger bg)
		public virtual BigFraction subtract(System.Numerics.BigInteger bg)
		{
			if (bg == null)
			{
				throw new NullArgumentException();
			}
			return new BigFraction(numerator - denominator * bg, denominator);
		}

		/// <summary>
		/// <p>
		/// Subtracts the value of an {@code integer} from the value of this
		/// {@code BigFraction}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="i"> the {@code integer} to subtract. </param>
		/// <returns> a {@code BigFraction} instance with the resulting values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction subtract(final int i)
		public virtual BigFraction subtract(int i)
		{
			return subtract(System.Numerics.BigInteger.valueOf(i));
		}

		/// <summary>
		/// <p>
		/// Subtracts the value of a {@code long} from the value of this
		/// {@code BigFraction}, returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="l"> the {@code long} to subtract. </param>
		/// <returns> a {@code BigFraction} instance with the resulting values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction subtract(final long l)
		public virtual BigFraction subtract(long l)
		{
			return subtract(System.Numerics.BigInteger.valueOf(l));
		}

		/// <summary>
		/// <p>
		/// Subtracts the value of another fraction from the value of this one,
		/// returning the result in reduced form.
		/// </p>
		/// </summary>
		/// <param name="fraction"> <seealso cref="BigFraction"/> to subtract, must not be {@code null}. </param>
		/// <returns> a <seealso cref="BigFraction"/> instance with the resulting values </returns>
		/// <exception cref="NullArgumentException"> if the {@code fraction} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigFraction subtract(final BigFraction fraction)
		public virtual BigFraction subtract(BigFraction fraction)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (ZERO.Equals(fraction))
			{
				return this;
			}

			System.Numerics.BigInteger num = null;
			System.Numerics.BigInteger den = null;
			if (denominator.Equals(fraction.denominator))
			{
				num = numerator - fraction.numerator;
				den = denominator;
			}
			else
			{
				num = (numerator * fraction.denominator).subtract((fraction.numerator).multiply(denominator));
				den = denominator * fraction.denominator;
			}
			return new BigFraction(num, den);

		}

		/// <summary>
		/// <p>
		/// Returns the <code>String</code> representing this fraction, ie
		/// "num / dem" or just "num" if the denominator is one.
		/// </p>
		/// </summary>
		/// <returns> a string representation of the fraction. </returns>
		/// <seealso cref= java.lang.Object#toString() </seealso>
		public override string ToString()
		{
			string str = null;
			if (System.Numerics.BigInteger.ONE.Equals(denominator))
			{
				str = numerator.ToString();
			}
			else if (System.Numerics.BigInteger.ZERO.Equals(numerator))
			{
				str = "0";
			}
			else
			{
				str = numerator + " / " + denominator;
			}
			return str;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual BigFractionField Field
		{
			get
			{
				return BigFractionField.Instance;
			}
		}

	}

}