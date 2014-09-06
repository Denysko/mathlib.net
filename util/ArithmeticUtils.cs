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
namespace mathlib.util
{

	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using Localizable = mathlib.exception.util.Localizable;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Some useful, arithmetics related, additions to the built-in functions in
	/// <seealso cref="Math"/>.
	/// 
	/// @version $Id: ArithmeticUtils.java 1591835 2014-05-02 09:04:01Z tn $
	/// </summary>
	public sealed class ArithmeticUtils
	{

		/// <summary>
		/// Private constructor. </summary>
		private ArithmeticUtils() : base()
		{
		}

		/// <summary>
		/// Add two integers, checking for overflow.
		/// </summary>
		/// <param name="x"> an addend </param>
		/// <param name="y"> an addend </param>
		/// <returns> the sum {@code x+y} </returns>
		/// <exception cref="MathArithmeticException"> if the result can not be represented
		/// as an {@code int}.
		/// @since 1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int addAndCheck(int x, int y) throws org.apache.commons.math3.exception.MathArithmeticException
		public static int addAndCheck(int x, int y)
		{
			long s = (long)x + (long)y;
			if (s < int.MinValue || s > int.MaxValue)
			{
				throw new MathArithmeticException(LocalizedFormats.OVERFLOW_IN_ADDITION, x, y);
			}
			return (int)s;
		}

		/// <summary>
		/// Add two long integers, checking for overflow.
		/// </summary>
		/// <param name="a"> an addend </param>
		/// <param name="b"> an addend </param>
		/// <returns> the sum {@code a+b} </returns>
		/// <exception cref="MathArithmeticException"> if the result can not be represented as an long
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long addAndCheck(long a, long b) throws org.apache.commons.math3.exception.MathArithmeticException
		public static long addAndCheck(long a, long b)
		{
			return addAndCheck(a, b, LocalizedFormats.OVERFLOW_IN_ADDITION);
		}

		/// <summary>
		/// Returns an exact representation of the <a
		/// href="http://mathworld.wolfram.com/BinomialCoefficient.html"> Binomial
		/// Coefficient</a>, "{@code n choose k}", the number of
		/// {@code k}-element subsets that can be selected from an
		/// {@code n}-element set.
		/// <p>
		/// <Strong>Preconditions</strong>:
		/// <ul>
		/// <li> {@code 0 <= k <= n } (otherwise
		/// {@code IllegalArgumentException} is thrown)</li>
		/// <li> The result is small enough to fit into a {@code long}. The
		/// largest value of {@code n} for which all coefficients are
		/// {@code  < Long.MAX_VALUE} is 66. If the computed value exceeds
		/// {@code Long.MAX_VALUE} an {@code ArithMeticException} is
		/// thrown.</li>
		/// </ul></p>
		/// </summary>
		/// <param name="n"> the size of the set </param>
		/// <param name="k"> the size of the subsets to be counted </param>
		/// <returns> {@code n choose k} </returns>
		/// <exception cref="NotPositiveException"> if {@code n < 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > n}. </exception>
		/// <exception cref="MathArithmeticException"> if the result is too large to be
		/// represented by a long integer. </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#binomialCoefficient(int, int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#binomialCoefficient(int, int)"/>") public static long binomialCoefficient(final int n, final int k) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#binomialCoefficient(int, int)"/>")]
		public static long binomialCoefficient(int n, int k)
		{
		   return CombinatoricsUtils.binomialCoefficient(n, k);
		}

		/// <summary>
		/// Returns a {@code double} representation of the <a
		/// href="http://mathworld.wolfram.com/BinomialCoefficient.html"> Binomial
		/// Coefficient</a>, "{@code n choose k}", the number of
		/// {@code k}-element subsets that can be selected from an
		/// {@code n}-element set.
		/// <p>
		/// <Strong>Preconditions</strong>:
		/// <ul>
		/// <li> {@code 0 <= k <= n } (otherwise
		/// {@code IllegalArgumentException} is thrown)</li>
		/// <li> The result is small enough to fit into a {@code double}. The
		/// largest value of {@code n} for which all coefficients are <
		/// Double.MAX_VALUE is 1029. If the computed value exceeds Double.MAX_VALUE,
		/// Double.POSITIVE_INFINITY is returned</li>
		/// </ul></p>
		/// </summary>
		/// <param name="n"> the size of the set </param>
		/// <param name="k"> the size of the subsets to be counted </param>
		/// <returns> {@code n choose k} </returns>
		/// <exception cref="NotPositiveException"> if {@code n < 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > n}. </exception>
		/// <exception cref="MathArithmeticException"> if the result is too large to be
		/// represented by a long integer. </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#binomialCoefficientDouble(int, int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#binomialCoefficientDouble(int, int)"/>") public static double binomialCoefficientDouble(final int n, final int k) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#binomialCoefficientDouble(int, int)"/>")]
		public static double binomialCoefficientDouble(int n, int k)
		{
			return CombinatoricsUtils.binomialCoefficientDouble(n, k);
		}

		/// <summary>
		/// Returns the natural {@code log} of the <a
		/// href="http://mathworld.wolfram.com/BinomialCoefficient.html"> Binomial
		/// Coefficient</a>, "{@code n choose k}", the number of
		/// {@code k}-element subsets that can be selected from an
		/// {@code n}-element set.
		/// <p>
		/// <Strong>Preconditions</strong>:
		/// <ul>
		/// <li> {@code 0 <= k <= n } (otherwise
		/// {@code IllegalArgumentException} is thrown)</li>
		/// </ul></p>
		/// </summary>
		/// <param name="n"> the size of the set </param>
		/// <param name="k"> the size of the subsets to be counted </param>
		/// <returns> {@code n choose k} </returns>
		/// <exception cref="NotPositiveException"> if {@code n < 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > n}. </exception>
		/// <exception cref="MathArithmeticException"> if the result is too large to be
		/// represented by a long integer. </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#binomialCoefficientLog(int, int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#binomialCoefficientLog(int, int)"/>") public static double binomialCoefficientLog(final int n, final int k) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#binomialCoefficientLog(int, int)"/>")]
		public static double binomialCoefficientLog(int n, int k)
		{
			return CombinatoricsUtils.binomialCoefficientLog(n, k);
		}

		/// <summary>
		/// Returns n!. Shorthand for {@code n} <a
		/// href="http://mathworld.wolfram.com/Factorial.html"> Factorial</a>, the
		/// product of the numbers {@code 1,...,n}.
		/// <p>
		/// <Strong>Preconditions</strong>:
		/// <ul>
		/// <li> {@code n >= 0} (otherwise
		/// {@code IllegalArgumentException} is thrown)</li>
		/// <li> The result is small enough to fit into a {@code long}. The
		/// largest value of {@code n} for which {@code n!} <
		/// Long.MAX_VALUE} is 20. If the computed value exceeds {@code Long.MAX_VALUE}
		/// an {@code ArithMeticException } is thrown.</li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <param name="n"> argument </param>
		/// <returns> {@code n!} </returns>
		/// <exception cref="MathArithmeticException"> if the result is too large to be represented
		/// by a {@code long}. </exception>
		/// <exception cref="NotPositiveException"> if {@code n < 0}. </exception>
		/// <exception cref="MathArithmeticException"> if {@code n > 20}: The factorial value is too
		/// large to fit in a {@code long}. </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#factorial(int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#factorial(int)"/>") public static long factorial(final int n) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#factorial(int)"/>")]
		public static long factorial(int n)
		{
			return CombinatoricsUtils.factorial(n);
		}

		/// <summary>
		/// Compute n!, the<a href="http://mathworld.wolfram.com/Factorial.html">
		/// factorial</a> of {@code n} (the product of the numbers 1 to n), as a
		/// {@code double}.
		/// The result should be small enough to fit into a {@code double}: The
		/// largest {@code n} for which {@code n! < Double.MAX_VALUE} is 170.
		/// If the computed value exceeds {@code Double.MAX_VALUE},
		/// {@code Double.POSITIVE_INFINITY} is returned.
		/// </summary>
		/// <param name="n"> Argument. </param>
		/// <returns> {@code n!} </returns>
		/// <exception cref="NotPositiveException"> if {@code n < 0}. </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#factorialDouble(int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#factorialDouble(int)"/>") public static double factorialDouble(final int n) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#factorialDouble(int)"/>")]
		public static double factorialDouble(int n)
		{
			 return CombinatoricsUtils.factorialDouble(n);
		}

		/// <summary>
		/// Compute the natural logarithm of the factorial of {@code n}.
		/// </summary>
		/// <param name="n"> Argument. </param>
		/// <returns> {@code n!} </returns>
		/// <exception cref="NotPositiveException"> if {@code n < 0}. </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#factorialLog(int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#factorialLog(int)"/>") public static double factorialLog(final int n) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#factorialLog(int)"/>")]
		public static double factorialLog(int n)
		{
			return CombinatoricsUtils.factorialLog(n);
		}

		/// <summary>
		/// Computes the greatest common divisor of the absolute value of two
		/// numbers, using a modified version of the "binary gcd" method.
		/// See Knuth 4.5.2 algorithm B.
		/// The algorithm is due to Josef Stein (1961).
		/// <br/>
		/// Special cases:
		/// <ul>
		///  <li>The invocations
		///   {@code gcd(Integer.MIN_VALUE, Integer.MIN_VALUE)},
		///   {@code gcd(Integer.MIN_VALUE, 0)} and
		///   {@code gcd(0, Integer.MIN_VALUE)} throw an
		///   {@code ArithmeticException}, because the result would be 2^31, which
		///   is too large for an int value.</li>
		///  <li>The result of {@code gcd(x, x)}, {@code gcd(0, x)} and
		///   {@code gcd(x, 0)} is the absolute value of {@code x}, except
		///   for the special cases above.</li>
		///  <li>The invocation {@code gcd(0, 0)} is the only one which returns
		///   {@code 0}.</li>
		/// </ul>
		/// </summary>
		/// <param name="p"> Number. </param>
		/// <param name="q"> Number. </param>
		/// <returns> the greatest common divisor (never negative). </returns>
		/// <exception cref="MathArithmeticException"> if the result cannot be represented as
		/// a non-negative {@code int} value.
		/// @since 1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int gcd(int p, int q) throws org.apache.commons.math3.exception.MathArithmeticException
		public static int gcd(int p, int q)
		{
			int a = p;
			int b = q;
			if (a == 0 || b == 0)
			{
				if (a == int.MinValue || b == int.MinValue)
				{
					throw new MathArithmeticException(LocalizedFormats.GCD_OVERFLOW_32_BITS, p, q);
				}
				return FastMath.abs(a + b);
			}

			long al = a;
			long bl = b;
			bool useLong = false;
			if (a < 0)
			{
				if (int.MinValue == a)
				{
					useLong = true;
				}
				else
				{
					a = -a;
				}
				al = -al;
			}
			if (b < 0)
			{
				if (int.MinValue == b)
				{
					useLong = true;
				}
				else
				{
					b = -b;
				}
				bl = -bl;
			}
			if (useLong)
			{
				if (al == bl)
				{
					throw new MathArithmeticException(LocalizedFormats.GCD_OVERFLOW_32_BITS, p, q);
				}
				long blbu = bl;
				bl = al;
				al = blbu % al;
				if (al == 0)
				{
					if (bl > int.MaxValue)
					{
						throw new MathArithmeticException(LocalizedFormats.GCD_OVERFLOW_32_BITS, p, q);
					}
					return (int) bl;
				}
				blbu = bl;

				// Now "al" and "bl" fit in an "int".
				b = (int) al;
				a = (int)(blbu % al);
			}

			return gcdPositive(a, b);
		}

		/// <summary>
		/// Computes the greatest common divisor of two <em>positive</em> numbers
		/// (this precondition is <em>not</em> checked and the result is undefined
		/// if not fulfilled) using the "binary gcd" method which avoids division
		/// and modulo operations.
		/// See Knuth 4.5.2 algorithm B.
		/// The algorithm is due to Josef Stein (1961).
		/// <br/>
		/// Special cases:
		/// <ul>
		///  <li>The result of {@code gcd(x, x)}, {@code gcd(0, x)} and
		///   {@code gcd(x, 0)} is the value of {@code x}.</li>
		///  <li>The invocation {@code gcd(0, 0)} is the only one which returns
		///   {@code 0}.</li>
		/// </ul>
		/// </summary>
		/// <param name="a"> Positive number. </param>
		/// <param name="b"> Positive number. </param>
		/// <returns> the greatest common divisor. </returns>
		private static int gcdPositive(int a, int b)
		{
			if (a == 0)
			{
				return b;
			}
			else if (b == 0)
			{
				return a;
			}

			// Make "a" and "b" odd, keeping track of common power of 2.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int aTwos = Integer.numberOfTrailingZeros(a);
			int aTwos = int.numberOfTrailingZeros(a);
			a >>= aTwos;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int bTwos = Integer.numberOfTrailingZeros(b);
			int bTwos = int.numberOfTrailingZeros(b);
			b >>= bTwos;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int shift = FastMath.min(aTwos, bTwos);
			int shift = FastMath.min(aTwos, bTwos);

			// "a" and "b" are positive.
			// If a > b then "gdc(a, b)" is equal to "gcd(a - b, b)".
			// If a < b then "gcd(a, b)" is equal to "gcd(b - a, a)".
			// Hence, in the successive iterations:
			//  "a" becomes the absolute difference of the current values,
			//  "b" becomes the minimum of the current values.
			while (a != b)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int delta = a - b;
				int delta = a - b;
				b = Math.Min(a, b);
				a = Math.Abs(delta);

				// Remove any power of 2 in "a" ("b" is guaranteed to be odd).
				a >>= int.numberOfTrailingZeros(a);
			}

			// Recover the common power of 2.
			return a << shift;
		}

		/// <summary>
		/// <p>
		/// Gets the greatest common divisor of the absolute value of two numbers,
		/// using the "binary gcd" method which avoids division and modulo
		/// operations. See Knuth 4.5.2 algorithm B. This algorithm is due to Josef
		/// Stein (1961).
		/// </p>
		/// Special cases:
		/// <ul>
		/// <li>The invocations
		/// {@code gcd(Long.MIN_VALUE, Long.MIN_VALUE)},
		/// {@code gcd(Long.MIN_VALUE, 0L)} and
		/// {@code gcd(0L, Long.MIN_VALUE)} throw an
		/// {@code ArithmeticException}, because the result would be 2^63, which
		/// is too large for a long value.</li>
		/// <li>The result of {@code gcd(x, x)}, {@code gcd(0L, x)} and
		/// {@code gcd(x, 0L)} is the absolute value of {@code x}, except
		/// for the special cases above.
		/// <li>The invocation {@code gcd(0L, 0L)} is the only one which returns
		/// {@code 0L}.</li>
		/// </ul>
		/// </summary>
		/// <param name="p"> Number. </param>
		/// <param name="q"> Number. </param>
		/// <returns> the greatest common divisor, never negative. </returns>
		/// <exception cref="MathArithmeticException"> if the result cannot be represented as
		/// a non-negative {@code long} value.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long gcd(final long p, final long q) throws org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static long gcd(long p, long q)
		{
			long u = p;
			long v = q;
			if ((u == 0) || (v == 0))
			{
				if ((u == long.MinValue) || (v == long.MinValue))
				{
					throw new MathArithmeticException(LocalizedFormats.GCD_OVERFLOW_64_BITS, p, q);
				}
				return FastMath.abs(u) + FastMath.abs(v);
			}
			// keep u and v negative, as negative integers range down to
			// -2^63, while positive numbers can only be as large as 2^63-1
			// (i.e. we can't necessarily negate a negative number without
			// overflow)
			/* assert u!=0 && v!=0; */
			if (u > 0)
			{
				u = -u;
			} // make u negative
			if (v > 0)
			{
				v = -v;
			} // make v negative
			// B1. [Find power of 2]
			int k = 0;
			while ((u & 1) == 0 && (v & 1) == 0 && k < 63) // while u and v are
			{
																// both even...
				u /= 2;
				v /= 2;
				k++; // cast out twos.
			}
			if (k == 63)
			{
				throw new MathArithmeticException(LocalizedFormats.GCD_OVERFLOW_64_BITS, p, q);
			}
			// B2. Initialize: u and v have been divided by 2^k and at least
			// one is odd.
			long t = ((u & 1) == 1) ? v : -(u / 2); // B3
			// t negative: u was odd, v may be even (t replaces v)
			// t positive: u was even, v is odd (t replaces u)
			do
			{
				/* assert u<0 && v<0; */
				// B4/B3: cast out twos from t.
				while ((t & 1) == 0) // while t is even..
				{
					t /= 2; // cast out twos
				}
				// B5 [reset max(u,v)]
				if (t > 0)
				{
					u = -t;
				}
				else
				{
					v = t;
				}
				// B6/B3. at this point both u and v should be odd.
				t = (v - u) / 2;
				// |u| larger: t positive (replace u)
				// |v| larger: t negative (replace v)
			} while (t != 0);
			return -u * (1L << k); // gcd is u*2^k
		}

		/// <summary>
		/// <p>
		/// Returns the least common multiple of the absolute value of two numbers,
		/// using the formula {@code lcm(a,b) = (a / gcd(a,b)) * b}.
		/// </p>
		/// Special cases:
		/// <ul>
		/// <li>The invocations {@code lcm(Integer.MIN_VALUE, n)} and
		/// {@code lcm(n, Integer.MIN_VALUE)}, where {@code abs(n)} is a
		/// power of 2, throw an {@code ArithmeticException}, because the result
		/// would be 2^31, which is too large for an int value.</li>
		/// <li>The result of {@code lcm(0, x)} and {@code lcm(x, 0)} is
		/// {@code 0} for any {@code x}.
		/// </ul>
		/// </summary>
		/// <param name="a"> Number. </param>
		/// <param name="b"> Number. </param>
		/// <returns> the least common multiple, never negative. </returns>
		/// <exception cref="MathArithmeticException"> if the result cannot be represented as
		/// a non-negative {@code int} value.
		/// @since 1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int lcm(int a, int b) throws org.apache.commons.math3.exception.MathArithmeticException
		public static int lcm(int a, int b)
		{
			if (a == 0 || b == 0)
			{
				return 0;
			}
			int lcm = FastMath.abs(ArithmeticUtils.mulAndCheck(a / gcd(a, b), b));
			if (lcm == int.MinValue)
			{
				throw new MathArithmeticException(LocalizedFormats.LCM_OVERFLOW_32_BITS, a, b);
			}
			return lcm;
		}

		/// <summary>
		/// <p>
		/// Returns the least common multiple of the absolute value of two numbers,
		/// using the formula {@code lcm(a,b) = (a / gcd(a,b)) * b}.
		/// </p>
		/// Special cases:
		/// <ul>
		/// <li>The invocations {@code lcm(Long.MIN_VALUE, n)} and
		/// {@code lcm(n, Long.MIN_VALUE)}, where {@code abs(n)} is a
		/// power of 2, throw an {@code ArithmeticException}, because the result
		/// would be 2^63, which is too large for an int value.</li>
		/// <li>The result of {@code lcm(0L, x)} and {@code lcm(x, 0L)} is
		/// {@code 0L} for any {@code x}.
		/// </ul>
		/// </summary>
		/// <param name="a"> Number. </param>
		/// <param name="b"> Number. </param>
		/// <returns> the least common multiple, never negative. </returns>
		/// <exception cref="MathArithmeticException"> if the result cannot be represented
		/// as a non-negative {@code long} value.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long lcm(long a, long b) throws org.apache.commons.math3.exception.MathArithmeticException
		public static long lcm(long a, long b)
		{
			if (a == 0 || b == 0)
			{
				return 0;
			}
			long lcm = FastMath.abs(ArithmeticUtils.mulAndCheck(a / gcd(a, b), b));
			if (lcm == long.MinValue)
			{
				throw new MathArithmeticException(LocalizedFormats.LCM_OVERFLOW_64_BITS, a, b);
			}
			return lcm;
		}

		/// <summary>
		/// Multiply two integers, checking for overflow.
		/// </summary>
		/// <param name="x"> Factor. </param>
		/// <param name="y"> Factor. </param>
		/// <returns> the product {@code x * y}. </returns>
		/// <exception cref="MathArithmeticException"> if the result can not be
		/// represented as an {@code int}.
		/// @since 1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int mulAndCheck(int x, int y) throws org.apache.commons.math3.exception.MathArithmeticException
		public static int mulAndCheck(int x, int y)
		{
			long m = ((long)x) * ((long)y);
			if (m < int.MinValue || m > int.MaxValue)
			{
				throw new MathArithmeticException();
			}
			return (int)m;
		}

		/// <summary>
		/// Multiply two long integers, checking for overflow.
		/// </summary>
		/// <param name="a"> Factor. </param>
		/// <param name="b"> Factor. </param>
		/// <returns> the product {@code a * b}. </returns>
		/// <exception cref="MathArithmeticException"> if the result can not be represented
		/// as a {@code long}.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long mulAndCheck(long a, long b) throws org.apache.commons.math3.exception.MathArithmeticException
		public static long mulAndCheck(long a, long b)
		{
			long ret;
			if (a > b)
			{
				// use symmetry to reduce boundary cases
				ret = mulAndCheck(b, a);
			}
			else
			{
				if (a < 0)
				{
					if (b < 0)
					{
						// check for positive overflow with negative a, negative b
						if (a >= long.MaxValue / b)
						{
							ret = a * b;
						}
						else
						{
							throw new MathArithmeticException();
						}
					}
					else if (b > 0)
					{
						// check for negative overflow with negative a, positive b
						if (long.MinValue / b <= a)
						{
							ret = a * b;
						}
						else
						{
							throw new MathArithmeticException();

						}
					}
					else
					{
						// assert b == 0
						ret = 0;
					}
				}
				else if (a > 0)
				{
					// assert a > 0
					// assert b > 0

					// check for positive overflow with positive a, positive b
					if (a <= long.MaxValue / b)
					{
						ret = a * b;
					}
					else
					{
						throw new MathArithmeticException();
					}
				}
				else
				{
					// assert a == 0
					ret = 0;
				}
			}
			return ret;
		}

		/// <summary>
		/// Subtract two integers, checking for overflow.
		/// </summary>
		/// <param name="x"> Minuend. </param>
		/// <param name="y"> Subtrahend. </param>
		/// <returns> the difference {@code x - y}. </returns>
		/// <exception cref="MathArithmeticException"> if the result can not be represented
		/// as an {@code int}.
		/// @since 1.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int subAndCheck(int x, int y) throws org.apache.commons.math3.exception.MathArithmeticException
		public static int subAndCheck(int x, int y)
		{
			long s = (long)x - (long)y;
			if (s < int.MinValue || s > int.MaxValue)
			{
				throw new MathArithmeticException(LocalizedFormats.OVERFLOW_IN_SUBTRACTION, x, y);
			}
			return (int)s;
		}

		/// <summary>
		/// Subtract two long integers, checking for overflow.
		/// </summary>
		/// <param name="a"> Value. </param>
		/// <param name="b"> Value. </param>
		/// <returns> the difference {@code a - b}. </returns>
		/// <exception cref="MathArithmeticException"> if the result can not be represented as a
		/// {@code long}.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long subAndCheck(long a, long b) throws org.apache.commons.math3.exception.MathArithmeticException
		public static long subAndCheck(long a, long b)
		{
			long ret;
			if (b == long.MinValue)
			{
				if (a < 0)
				{
					ret = a - b;
				}
				else
				{
					throw new MathArithmeticException(LocalizedFormats.OVERFLOW_IN_ADDITION, a, -b);
				}
			}
			else
			{
				// use additive inverse
				ret = addAndCheck(a, -b, LocalizedFormats.OVERFLOW_IN_ADDITION);
			}
			return ret;
		}

		/// <summary>
		/// Raise an int to an int power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> \( k^e \) </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
		/// <exception cref="MathArithmeticException"> if the result would overflow. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static int pow(final int k, final int e) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static int pow(int k, int e)
		{
			if (e < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			try
			{
				int exp = e;
				int result = 1;
				int k2p = k;
				while (true)
				{
					if ((exp & 0x1) != 0)
					{
						result = mulAndCheck(result, k2p);
					}

					exp >>= 1;
					if (exp == 0)
					{
						break;
					}

					k2p = mulAndCheck(k2p, k2p);
				}

				return result;
			}
			catch (MathArithmeticException mae)
			{
				// Add context information.
				mae.Context.addMessage(LocalizedFormats.OVERFLOW);
				mae.Context.addMessage(LocalizedFormats.BASE, k);
				mae.Context.addMessage(LocalizedFormats.EXPONENT, e);

				// Rethrow.
				throw mae;
			}
		}

		/// <summary>
		/// Raise an int to a long power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> k<sup>e</sup> </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
		/// @deprecated As of 3.3. Please use <seealso cref="#pow(int,int)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.3. Please use <seealso cref="#pow(int,int)"/> instead.") public static int pow(final int k, long e) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("As of 3.3. Please use <seealso cref="#pow(int,int)"/> instead.")]
		public static int pow(int k, long e)
		{
			if (e < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			int result = 1;
			int k2p = k;
			while (e != 0)
			{
				if ((e & 0x1) != 0)
				{
					result *= k2p;
				}
				k2p *= k2p;
				e >>= 1;
			}

			return result;
		}

		/// <summary>
		/// Raise a long to an int power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> \( k^e \) </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
		/// <exception cref="MathArithmeticException"> if the result would overflow. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static long pow(final long k, final int e) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static long pow(long k, int e)
		{
			if (e < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			try
			{
				int exp = e;
				long result = 1;
				long k2p = k;
				while (true)
				{
					if ((exp & 0x1) != 0)
					{
						result = mulAndCheck(result, k2p);
					}

					exp >>= 1;
					if (exp == 0)
					{
						break;
					}

					k2p = mulAndCheck(k2p, k2p);
				}

				return result;
			}
			catch (MathArithmeticException mae)
			{
				// Add context information.
				mae.Context.addMessage(LocalizedFormats.OVERFLOW);
				mae.Context.addMessage(LocalizedFormats.BASE, k);
				mae.Context.addMessage(LocalizedFormats.EXPONENT, e);

				// Rethrow.
				throw mae;
			}
		}

		/// <summary>
		/// Raise a long to a long power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> k<sup>e</sup> </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
		/// @deprecated As of 3.3. Please use <seealso cref="#pow(long,int)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.3. Please use <seealso cref="#pow(long,int)"/> instead.") public static long pow(final long k, long e) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("As of 3.3. Please use <seealso cref="#pow(long,int)"/> instead.")]
		public static long pow(long k, long e)
		{
			if (e < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			long result = 1l;
			long k2p = k;
			while (e != 0)
			{
				if ((e & 0x1) != 0)
				{
					result *= k2p;
				}
				k2p *= k2p;
				e >>= 1;
			}

			return result;
		}

		/// <summary>
		/// Raise a BigInteger to an int power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> k<sup>e</sup> </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.math.BigInteger pow(final java.math.BigInteger k, int e) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static System.Numerics.BigInteger pow(System.Numerics.BigInteger k, int e)
		{
			if (e < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			return k.pow(e);
		}

		/// <summary>
		/// Raise a BigInteger to a long power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> k<sup>e</sup> </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.math.BigInteger pow(final java.math.BigInteger k, long e) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static System.Numerics.BigInteger pow(System.Numerics.BigInteger k, long e)
		{
			if (e < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			System.Numerics.BigInteger result = System.Numerics.BigInteger.ONE;
			System.Numerics.BigInteger k2p = k;
			while (e != 0)
			{
				if ((e & 0x1) != 0)
				{
					result = result * k2p;
				}
				k2p = k2p * k2p;
				e >>= 1;
			}

			return result;

		}

		/// <summary>
		/// Raise a BigInteger to a BigInteger power.
		/// </summary>
		/// <param name="k"> Number to raise. </param>
		/// <param name="e"> Exponent (must be positive or zero). </param>
		/// <returns> k<sup>e</sup> </returns>
		/// <exception cref="NotPositiveException"> if {@code e < 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.math.BigInteger pow(final java.math.BigInteger k, java.math.BigInteger e) throws org.apache.commons.math3.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static System.Numerics.BigInteger pow(System.Numerics.BigInteger k, System.Numerics.BigInteger e)
		{
			if (e.compareTo(System.Numerics.BigInteger.ZERO) < 0)
			{
				throw new NotPositiveException(LocalizedFormats.EXPONENT, e);
			}

			System.Numerics.BigInteger result = System.Numerics.BigInteger.ONE;
			System.Numerics.BigInteger k2p = k;
			while (!System.Numerics.BigInteger.ZERO.Equals(e))
			{
				if (e.testBit(0))
				{
					result = result * k2p;
				}
				k2p = k2p * k2p;
				e = e.shiftRight(1);
			}

			return result;
		}

		/// <summary>
		/// Returns the <a
		/// href="http://mathworld.wolfram.com/StirlingNumberoftheSecondKind.html">
		/// Stirling number of the second kind</a>, "{@code S(n,k)}", the number of
		/// ways of partitioning an {@code n}-element set into {@code k} non-empty
		/// subsets.
		/// <p>
		/// The preconditions are {@code 0 <= k <= n } (otherwise
		/// {@code NotPositiveException} is thrown)
		/// </p> </summary>
		/// <param name="n"> the size of the set </param>
		/// <param name="k"> the number of non-empty subsets </param>
		/// <returns> {@code S(n,k)} </returns>
		/// <exception cref="NotPositiveException"> if {@code k < 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code k > n}. </exception>
		/// <exception cref="MathArithmeticException"> if some overflow happens, typically for n exceeding 25 and
		/// k between 20 and n-2 (S(n,n-1) is handled specifically and does not overflow)
		/// @since 3.1 </exception>
		/// @deprecated use <seealso cref="CombinatoricsUtils#stirlingS2(int, int)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("use <seealso cref="CombinatoricsUtils#stirlingS2(int, int)"/>") public static long stirlingS2(final int n, final int k) throws org.apache.commons.math3.exception.NotPositiveException, org.apache.commons.math3.exception.NumberIsTooLargeException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete("use <seealso cref="CombinatoricsUtils#stirlingS2(int, int)"/>")]
		public static long stirlingS2(int n, int k)
		{
			return CombinatoricsUtils.stirlingS2(n, k);

		}

		/// <summary>
		/// Add two long integers, checking for overflow.
		/// </summary>
		/// <param name="a"> Addend. </param>
		/// <param name="b"> Addend. </param>
		/// <param name="pattern"> Pattern to use for any thrown exception. </param>
		/// <returns> the sum {@code a + b}. </returns>
		/// <exception cref="MathArithmeticException"> if the result cannot be represented
		/// as a {@code long}.
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static long addAndCheck(long a, long b, org.apache.commons.math3.exception.util.Localizable pattern) throws org.apache.commons.math3.exception.MathArithmeticException
		 private static long addAndCheck(long a, long b, Localizable pattern)
		 {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long result = a + b;
			 long result = a + b;
			 if (!((a ^ b) < 0 | (a ^ result) >= 0))
			 {
				 throw new MathArithmeticException(pattern, a, b);
			 }
			 return result;
		 }

		/// <summary>
		/// Returns true if the argument is a power of two.
		/// </summary>
		/// <param name="n"> the number to test </param>
		/// <returns> true if the argument is a power of two </returns>
		public static bool isPowerOfTwo(long n)
		{
			return (n > 0) && ((n & (n - 1)) == 0);
		}
	}

}