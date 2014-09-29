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
namespace mathlib.fraction
{


	using mathlib;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using ArithmeticUtils = mathlib.util.ArithmeticUtils;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Representation of a rational number.
	/// 
	/// implements Serializable since 2.0
	/// 
	/// @since 1.1
	/// @version $Id: Fraction.java 1519204 2013-08-31 19:43:02Z tn $
	/// </summary>
	[Serializable]
	public class Fraction : Number, FieldElement<Fraction>, IComparable<Fraction>
	{

		/// <summary>
		/// A fraction representing "2 / 1". </summary>
		public static readonly Fraction TWO = new Fraction(2, 1);

		/// <summary>
		/// A fraction representing "1". </summary>
		public static readonly Fraction ONE = new Fraction(1, 1);

		/// <summary>
		/// A fraction representing "0". </summary>
		public static readonly Fraction ZERO = new Fraction(0, 1);

		/// <summary>
		/// A fraction representing "4/5". </summary>
		public static readonly Fraction FOUR_FIFTHS = new Fraction(4, 5);

		/// <summary>
		/// A fraction representing "1/5". </summary>
		public static readonly Fraction ONE_FIFTH = new Fraction(1, 5);

		/// <summary>
		/// A fraction representing "1/2". </summary>
		public static readonly Fraction ONE_HALF = new Fraction(1, 2);

		/// <summary>
		/// A fraction representing "1/4". </summary>
		public static readonly Fraction ONE_QUARTER = new Fraction(1, 4);

		/// <summary>
		/// A fraction representing "1/3". </summary>
		public static readonly Fraction ONE_THIRD = new Fraction(1, 3);

		/// <summary>
		/// A fraction representing "3/5". </summary>
		public static readonly Fraction THREE_FIFTHS = new Fraction(3, 5);

		/// <summary>
		/// A fraction representing "3/4". </summary>
		public static readonly Fraction THREE_QUARTERS = new Fraction(3, 4);

		/// <summary>
		/// A fraction representing "2/5". </summary>
		public static readonly Fraction TWO_FIFTHS = new Fraction(2, 5);

		/// <summary>
		/// A fraction representing "2/4". </summary>
		public static readonly Fraction TWO_QUARTERS = new Fraction(2, 4);

		/// <summary>
		/// A fraction representing "2/3". </summary>
		public static readonly Fraction TWO_THIRDS = new Fraction(2, 3);

		/// <summary>
		/// A fraction representing "-1 / 1". </summary>
		public static readonly Fraction MINUS_ONE = new Fraction(-1, 1);

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 3698073679419233275L;

		/// <summary>
		/// The default epsilon used for convergence. </summary>
		private const double DEFAULT_EPSILON = 1e-5;

		/// <summary>
		/// The denominator. </summary>
		private readonly int denominator;

		/// <summary>
		/// The numerator. </summary>
		private readonly int numerator;

		/// <summary>
		/// Create a fraction given the double value. </summary>
		/// <param name="value"> the double value to convert to a fraction. </param>
		/// <exception cref="FractionConversionException"> if the continued fraction failed to
		///         converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Fraction(double value) throws FractionConversionException
		public Fraction(double value) : this(value, DEFAULT_EPSILON, 100)
		{
		}

		/// <summary>
		/// Create a fraction given the double value and maximum error allowed.
		/// <p>
		/// References:
		/// <ul>
		/// <li><a href="http://mathworld.wolfram.com/ContinuedFraction.html">
		/// Continued Fraction</a> equations (11) and (22)-(26)</li>
		/// </ul>
		/// </p> </summary>
		/// <param name="value"> the double value to convert to a fraction. </param>
		/// <param name="epsilon"> maximum error allowed.  The resulting fraction is within
		///        {@code epsilon} of {@code value}, in absolute terms. </param>
		/// <param name="maxIterations"> maximum number of convergents </param>
		/// <exception cref="FractionConversionException"> if the continued fraction failed to
		///         converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Fraction(double value, double epsilon, int maxIterations) throws FractionConversionException
		public Fraction(double value, double epsilon, int maxIterations) : this(value, epsilon, int.MaxValue, maxIterations)
		{
		}

		/// <summary>
		/// Create a fraction given the double value and maximum denominator.
		/// <p>
		/// References:
		/// <ul>
		/// <li><a href="http://mathworld.wolfram.com/ContinuedFraction.html">
		/// Continued Fraction</a> equations (11) and (22)-(26)</li>
		/// </ul>
		/// </p> </summary>
		/// <param name="value"> the double value to convert to a fraction. </param>
		/// <param name="maxDenominator"> The maximum allowed value for denominator </param>
		/// <exception cref="FractionConversionException"> if the continued fraction failed to
		///         converge </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Fraction(double value, int maxDenominator) throws FractionConversionException
		public Fraction(double value, int maxDenominator) : this(value, 0, maxDenominator, 100)
		{
		}

		/// <summary>
		/// Create a fraction given the double value and either the maximum error
		/// allowed or the maximum number of denominator digits.
		/// <p>
		/// 
		/// NOTE: This constructor is called with EITHER
		///   - a valid epsilon value and the maxDenominator set to Integer.MAX_VALUE
		///     (that way the maxDenominator has no effect).
		/// OR
		///   - a valid maxDenominator value and the epsilon value set to zero
		///     (that way epsilon only has effect if there is an exact match before
		///     the maxDenominator value is reached).
		/// </p><p>
		/// 
		/// It has been done this way so that the same code can be (re)used for both
		/// scenarios. However this could be confusing to users if it were part of
		/// the public API and this constructor should therefore remain PRIVATE.
		/// </p>
		/// 
		/// See JIRA issue ticket MATH-181 for more details:
		/// 
		///     https://issues.apache.org/jira/browse/MATH-181
		/// </summary>
		/// <param name="value"> the double value to convert to a fraction. </param>
		/// <param name="epsilon"> maximum error allowed.  The resulting fraction is within
		///        {@code epsilon} of {@code value}, in absolute terms. </param>
		/// <param name="maxDenominator"> maximum denominator value allowed. </param>
		/// <param name="maxIterations"> maximum number of convergents </param>
		/// <exception cref="FractionConversionException"> if the continued fraction failed to
		///         converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Fraction(double value, double epsilon, int maxDenominator, int maxIterations) throws FractionConversionException
		private Fraction(double value, double epsilon, int maxDenominator, int maxIterations)
		{
			long overflow = int.MaxValue;
			double r0 = value;
			long a0 = (long)FastMath.floor(r0);
			if (FastMath.abs(a0) > overflow)
			{
				throw new FractionConversionException(value, a0, 1l);
			}

			// check for (almost) integer arguments, which should not go to iterations.
			if (FastMath.abs(a0 - value) < epsilon)
			{
				this.numerator = (int) a0;
				this.denominator = 1;
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
				double r1 = 1.0 / (r0 - a0);
				long a1 = (long)FastMath.floor(r1);
				p2 = (a1 * p1) + p0;
				q2 = (a1 * q1) + q0;

				if ((FastMath.abs(p2) > overflow) || (FastMath.abs(q2) > overflow))
				{
					// in maxDenominator mode, if the last fraction was very close to the actual value
					// q2 may overflow in the next iteration; in this case return the last one.
					if (epsilon == 0.0 && FastMath.abs(q1) < maxDenominator)
					{
						break;
					}
					throw new FractionConversionException(value, p2, q2);
				}

				double convergent = (double)p2 / (double)q2;
				if (n < maxIterations && FastMath.abs(convergent - value) > epsilon && q2 < maxDenominator)
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
				this.numerator = (int) p2;
				this.denominator = (int) q2;
			}
			else
			{
				this.numerator = (int) p1;
				this.denominator = (int) q1;
			}

		}

		/// <summary>
		/// Create a fraction from an int.
		/// The fraction is num / 1. </summary>
		/// <param name="num"> the numerator. </param>
		public Fraction(int num) : this(num, 1)
		{
		}

		/// <summary>
		/// Create a fraction given the numerator and denominator.  The fraction is
		/// reduced to lowest terms. </summary>
		/// <param name="num"> the numerator. </param>
		/// <param name="den"> the denominator. </param>
		/// <exception cref="MathArithmeticException"> if the denominator is {@code zero} </exception>
		public Fraction(int num, int den)
		{
			if (den == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_DENOMINATOR_IN_FRACTION, num, den);
			}
			if (den < 0)
			{
				if (num == int.MinValue || den == int.MinValue)
				{
					throw new MathArithmeticException(LocalizedFormats.OVERFLOW_IN_FRACTION, num, den);
				}
				num = -num;
				den = -den;
			}
			// reduce numerator and denominator by greatest common denominator.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int d = mathlib.util.ArithmeticUtils.gcd(num, den);
			int d = ArithmeticUtils.gcd(num, den);
			if (d > 1)
			{
				num /= d;
				den /= d;
			}

			// move sign to numerator.
			if (den < 0)
			{
				num = -num;
				den = -den;
			}
			this.numerator = num;
			this.denominator = den;
		}

		/// <summary>
		/// Returns the absolute value of this fraction. </summary>
		/// <returns> the absolute value. </returns>
		public virtual Fraction abs()
		{
			Fraction ret;
			if (numerator >= 0)
			{
				ret = this;
			}
			else
			{
				ret = negate();
			}
			return ret;
		}

		/// <summary>
		/// Compares this object to another based on size. </summary>
		/// <param name="object"> the object to compare to </param>
		/// <returns> -1 if this is less than <tt>object</tt>, +1 if this is greater
		///         than <tt>object</tt>, 0 if they are equal. </returns>
		public virtual int CompareTo(Fraction @object)
		{
			long nOd = ((long) numerator) * @object.denominator;
			long dOn = ((long) denominator) * @object.numerator;
			return (nOd < dOn) ? - 1 : ((nOd > dOn) ? + 1 : 0);
		}

		/// <summary>
		/// Gets the fraction as a <tt>double</tt>. This calculates the fraction as
		/// the numerator divided by denominator. </summary>
		/// <returns> the fraction as a <tt>double</tt> </returns>
		public override double doubleValue()
		{
			return (double)numerator / (double)denominator;
		}

		/// <summary>
		/// Test for the equality of two fractions.  If the lowest term
		/// numerator and denominators are the same for both fractions, the two
		/// fractions are considered to be equal. </summary>
		/// <param name="other"> fraction to test for equality to this fraction </param>
		/// <returns> true if two fractions are equal, false if object is
		///         <tt>null</tt>, not an instance of <seealso cref="Fraction"/>, or not equal
		///         to this fraction instance. </returns>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other is Fraction)
			{
				// since fractions are always in lowest terms, numerators and
				// denominators can be compared directly for equality.
				Fraction rhs = (Fraction)other;
				return (numerator == rhs.numerator) && (denominator == rhs.denominator);
			}
			return false;
		}

		/// <summary>
		/// Gets the fraction as a <tt>float</tt>. This calculates the fraction as
		/// the numerator divided by denominator. </summary>
		/// <returns> the fraction as a <tt>float</tt> </returns>
		public override float floatValue()
		{
			return (float)doubleValue();
		}

		/// <summary>
		/// Access the denominator. </summary>
		/// <returns> the denominator. </returns>
		public virtual int Denominator
		{
			get
			{
				return denominator;
			}
		}

		/// <summary>
		/// Access the numerator. </summary>
		/// <returns> the numerator. </returns>
		public virtual int Numerator
		{
			get
			{
				return numerator;
			}
		}

		/// <summary>
		/// Gets a hashCode for the fraction. </summary>
		/// <returns> a hash code value for this object </returns>
		public override int GetHashCode()
		{
			return 37 * (37 * 17 + numerator) + denominator;
		}

		/// <summary>
		/// Gets the fraction as an <tt>int</tt>. This returns the whole number part
		/// of the fraction. </summary>
		/// <returns> the whole number fraction part </returns>
		public override int intValue()
		{
			return (int)doubleValue();
		}

		/// <summary>
		/// Gets the fraction as a <tt>long</tt>. This returns the whole number part
		/// of the fraction. </summary>
		/// <returns> the whole number fraction part </returns>
		public override long longValue()
		{
			return (long)doubleValue();
		}

		/// <summary>
		/// Return the additive inverse of this fraction. </summary>
		/// <returns> the negation of this fraction. </returns>
		public virtual Fraction negate()
		{
			if (numerator == int.MinValue)
			{
				throw new MathArithmeticException(LocalizedFormats.OVERFLOW_IN_FRACTION, numerator, denominator);
			}
			return new Fraction(-numerator, denominator);
		}

		/// <summary>
		/// Return the multiplicative inverse of this fraction. </summary>
		/// <returns> the reciprocal fraction </returns>
		public virtual Fraction reciprocal()
		{
			return new Fraction(denominator, numerator);
		}

		/// <summary>
		/// <p>Adds the value of this fraction to another, returning the result in reduced form.
		/// The algorithm follows Knuth, 4.5.1.</p>
		/// </summary>
		/// <param name="fraction">  the fraction to add, must not be {@code null} </param>
		/// <returns> a {@code Fraction} instance with the resulting values </returns>
		/// <exception cref="NullArgumentException"> if the fraction is {@code null} </exception>
		/// <exception cref="MathArithmeticException"> if the resulting numerator or denominator exceeds
		///  {@code Integer.MAX_VALUE} </exception>
		public virtual Fraction add(Fraction fraction)
		{
			return addSub(fraction, true); // add
		}

		/// <summary>
		/// Add an integer to the fraction. </summary>
		/// <param name="i"> the <tt>integer</tt> to add. </param>
		/// <returns> this + i </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Fraction add(final int i)
		public virtual Fraction add(int i)
		{
			return new Fraction(numerator + i * denominator, denominator);
		}

		/// <summary>
		/// <p>Subtracts the value of another fraction from the value of this one,
		/// returning the result in reduced form.</p>
		/// </summary>
		/// <param name="fraction">  the fraction to subtract, must not be {@code null} </param>
		/// <returns> a {@code Fraction} instance with the resulting values </returns>
		/// <exception cref="NullArgumentException"> if the fraction is {@code null} </exception>
		/// <exception cref="MathArithmeticException"> if the resulting numerator or denominator
		///   cannot be represented in an {@code int}. </exception>
		public virtual Fraction subtract(Fraction fraction)
		{
			return addSub(fraction, false); // subtract
		}

		/// <summary>
		/// Subtract an integer from the fraction. </summary>
		/// <param name="i"> the <tt>integer</tt> to subtract. </param>
		/// <returns> this - i </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Fraction subtract(final int i)
		public virtual Fraction subtract(int i)
		{
			return new Fraction(numerator - i * denominator, denominator);
		}

		/// <summary>
		/// Implement add and subtract using algorithm described in Knuth 4.5.1.
		/// </summary>
		/// <param name="fraction"> the fraction to subtract, must not be {@code null} </param>
		/// <param name="isAdd"> true to add, false to subtract </param>
		/// <returns> a {@code Fraction} instance with the resulting values </returns>
		/// <exception cref="NullArgumentException"> if the fraction is {@code null} </exception>
		/// <exception cref="MathArithmeticException"> if the resulting numerator or denominator
		///   cannot be represented in an {@code int}. </exception>
		private Fraction addSub(Fraction fraction, bool isAdd)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			// zero is identity for addition.
			if (numerator == 0)
			{
				return isAdd ? fraction : fraction.negate();
			}
			if (fraction.numerator == 0)
			{
				return this;
			}
			// if denominators are randomly distributed, d1 will be 1 about 61%
			// of the time.
			int d1 = ArithmeticUtils.gcd(denominator, fraction.denominator);
			if (d1 == 1)
			{
				// result is ( (u*v' +/- u'v) / u'v')
				int uvp = ArithmeticUtils.mulAndCheck(numerator, fraction.denominator);
				int upv = ArithmeticUtils.mulAndCheck(fraction.numerator, denominator);
				return new Fraction(isAdd ? ArithmeticUtils.addAndCheck(uvp, upv) : ArithmeticUtils.subAndCheck(uvp, upv), ArithmeticUtils.mulAndCheck(denominator, fraction.denominator));
			}
			// the quantity 't' requires 65 bits of precision; see knuth 4.5.1
			// exercise 7.  we're going to use a BigInteger.
			// t = u(v'/d1) +/- v(u'/d1)
			System.Numerics.BigInteger uvp = System.Numerics.BigInteger.valueOf(numerator) * System.Numerics.BigInteger.valueOf(fraction.denominator / d1);
			System.Numerics.BigInteger upv = System.Numerics.BigInteger.valueOf(fraction.numerator) * System.Numerics.BigInteger.valueOf(denominator / d1);
			System.Numerics.BigInteger t = isAdd ? uvp + upv : uvp - upv;
			// but d2 doesn't need extra precision because
			// d2 = gcd(t,d1) = gcd(t mod d1, d1)
			int tmodd1 = (int)t.mod(System.Numerics.BigInteger.valueOf(d1));
			int d2 = (tmodd1 == 0)?d1:ArithmeticUtils.gcd(tmodd1, d1);

			// result is (t/d2) / (u'/d1)(v'/d2)
			System.Numerics.BigInteger w = t / System.Numerics.BigInteger.valueOf(d2);
			if (w.bitLength() > 31)
			{
				throw new MathArithmeticException(LocalizedFormats.NUMERATOR_OVERFLOW_AFTER_MULTIPLY, w);
			}
			return new Fraction((int)w, ArithmeticUtils.mulAndCheck(denominator / d1, fraction.denominator / d2));
		}

		/// <summary>
		/// <p>Multiplies the value of this fraction by another, returning the
		/// result in reduced form.</p>
		/// </summary>
		/// <param name="fraction">  the fraction to multiply by, must not be {@code null} </param>
		/// <returns> a {@code Fraction} instance with the resulting values </returns>
		/// <exception cref="NullArgumentException"> if the fraction is {@code null} </exception>
		/// <exception cref="MathArithmeticException"> if the resulting numerator or denominator exceeds
		///  {@code Integer.MAX_VALUE} </exception>
		public virtual Fraction multiply(Fraction fraction)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (numerator == 0 || fraction.numerator == 0)
			{
				return ZERO;
			}
			// knuth 4.5.1
			// make sure we don't overflow unless the result *must* overflow.
			int d1 = ArithmeticUtils.gcd(numerator, fraction.denominator);
			int d2 = ArithmeticUtils.gcd(fraction.numerator, denominator);
			return getReducedFraction(ArithmeticUtils.mulAndCheck(numerator / d1, fraction.numerator / d2), ArithmeticUtils.mulAndCheck(denominator / d2, fraction.denominator / d1));
		}

		/// <summary>
		/// Multiply the fraction by an integer. </summary>
		/// <param name="i"> the <tt>integer</tt> to multiply by. </param>
		/// <returns> this * i </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Fraction multiply(final int i)
		public virtual Fraction multiply(int i)
		{
			return new Fraction(numerator * i, denominator);
		}

		/// <summary>
		/// <p>Divide the value of this fraction by another.</p>
		/// </summary>
		/// <param name="fraction">  the fraction to divide by, must not be {@code null} </param>
		/// <returns> a {@code Fraction} instance with the resulting values </returns>
		/// <exception cref="IllegalArgumentException"> if the fraction is {@code null} </exception>
		/// <exception cref="MathArithmeticException"> if the fraction to divide by is zero </exception>
		/// <exception cref="MathArithmeticException"> if the resulting numerator or denominator exceeds
		///  {@code Integer.MAX_VALUE} </exception>
		public virtual Fraction divide(Fraction fraction)
		{
			if (fraction == null)
			{
				throw new NullArgumentException(LocalizedFormats.FRACTION);
			}
			if (fraction.numerator == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_FRACTION_TO_DIVIDE_BY, fraction.numerator, fraction.denominator);
			}
			return multiply(fraction.reciprocal());
		}

		/// <summary>
		/// Divide the fraction by an integer. </summary>
		/// <param name="i"> the <tt>integer</tt> to divide by. </param>
		/// <returns> this * i </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Fraction divide(final int i)
		public virtual Fraction divide(int i)
		{
			return new Fraction(numerator, denominator * i);
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
			return 100 * doubleValue();
		}

		/// <summary>
		/// <p>Creates a {@code Fraction} instance with the 2 parts
		/// of a fraction Y/Z.</p>
		/// 
		/// <p>Any negative signs are resolved to be on the numerator.</p>
		/// </summary>
		/// <param name="numerator">  the numerator, for example the three in 'three sevenths' </param>
		/// <param name="denominator">  the denominator, for example the seven in 'three sevenths' </param>
		/// <returns> a new fraction instance, with the numerator and denominator reduced </returns>
		/// <exception cref="MathArithmeticException"> if the denominator is {@code zero} </exception>
		public static Fraction getReducedFraction(int numerator, int denominator)
		{
			if (denominator == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_DENOMINATOR_IN_FRACTION, numerator, denominator);
			}
			if (numerator == 0)
			{
				return ZERO; // normalize zero.
			}
			// allow 2^k/-2^31 as a valid fraction (where k>0)
			if (denominator == int.MinValue && (numerator & 1) == 0)
			{
				numerator /= 2;
				denominator /= 2;
			}
			if (denominator < 0)
			{
				if (numerator == int.MinValue || denominator == int.MinValue)
				{
					throw new MathArithmeticException(LocalizedFormats.OVERFLOW_IN_FRACTION, numerator, denominator);
				}
				numerator = -numerator;
				denominator = -denominator;
			}
			// simplify fraction.
			int gcd = ArithmeticUtils.gcd(numerator, denominator);
			numerator /= gcd;
			denominator /= gcd;
			return new Fraction(numerator, denominator);
		}

		/// <summary>
		/// <p>
		/// Returns the {@code String} representing this fraction, ie
		/// "num / dem" or just "num" if the denominator is one.
		/// </p>
		/// </summary>
		/// <returns> a string representation of the fraction. </returns>
		/// <seealso cref= java.lang.Object#toString() </seealso>
		public override string ToString()
		{
			string str = null;
			if (denominator == 1)
			{
				str = Convert.ToString(numerator);
			}
			else if (numerator == 0)
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
		public virtual FractionField Field
		{
			get
			{
				return FractionField.Instance;
			}
		}

	}

}