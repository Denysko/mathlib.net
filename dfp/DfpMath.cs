using System.Text;

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

namespace org.apache.commons.math3.dfp
{

	/// <summary>
	/// Mathematical routines for use with <seealso cref="Dfp"/>.
	/// The constants are defined in <seealso cref="DfpField"/>
	/// @version $Id: DfpMath.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 2.2
	/// </summary>
	public class DfpMath
	{

		/// <summary>
		/// Name for traps triggered by pow. </summary>
		private const string POW_TRAP = "pow";

		/// <summary>
		/// Private Constructor.
		/// </summary>
		private DfpMath()
		{
		}

		/// <summary>
		/// Breaks a string representation up into two dfp's.
		/// <p>The two dfp are such that the sum of them is equivalent
		/// to the input string, but has higher precision than using a
		/// single dfp. This is useful for improving accuracy of
		/// exponentiation and critical multiplies. </summary>
		/// <param name="field"> field to which the Dfp must belong </param>
		/// <param name="a"> string representation to split </param>
		/// <returns> an array of two <seealso cref="Dfp"/> which sum is a </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp[] split(final DfpField field, final String a)
		protected internal static Dfp[] Split(DfpField field, string a)
		{
			Dfp[] result = new Dfp[2];
			char[] buf;
			bool leading = true;
			int sp = 0;
			int sig = 0;

			buf = new char[a.Length];

			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = a[i];

				if (buf[i] >= '1' && buf[i] <= '9')
				{
					leading = false;
				}

				if (buf[i] == '.')
				{
					sig += (400 - sig) % 4;
					leading = false;
				}

				if (sig == (field.RadixDigits / 2) * 4)
				{
					sp = i;
					break;
				}

				if (buf[i] >= '0' && buf[i] <= '9' && !leading)
				{
					sig++;
				}
			}

			result[0] = field.newDfp(new string(buf, 0, sp));

			for (int i = 0; i < buf.Length; i++)
			{
				buf[i] = a[i];
				if (buf[i] >= '0' && buf[i] <= '9' && i < sp)
				{
					buf[i] = '0';
				}
			}

			result[1] = field.newDfp(new string(buf));

			return result;
		}

		/// <summary>
		/// Splits a <seealso cref="Dfp"/> into 2 <seealso cref="Dfp"/>'s such that their sum is equal to the input <seealso cref="Dfp"/>. </summary>
		/// <param name="a"> number to split </param>
		/// <returns> two elements array containing the split number </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp[] split(final Dfp a)
		protected internal static Dfp[] Split(Dfp a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] result = new Dfp[2];
			Dfp[] result = new Dfp[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp shift = a.multiply(a.power10K(a.getRadixDigits() / 2));
			Dfp shift = a.multiply(a.power10K(a.RadixDigits / 2));
			result[0] = a.add(shift).subtract(shift);
			result[1] = a.subtract(result[0]);
			return result;
		}

		/// <summary>
		/// Multiply two numbers that are split in to two pieces that are
		///  meant to be added together.
		///  Use binomial multiplication so ab = a0 b0 + a0 b1 + a1 b0 + a1 b1
		///  Store the first term in result0, the rest in result1 </summary>
		///  <param name="a"> first factor of the multiplication, in split form </param>
		///  <param name="b"> second factor of the multiplication, in split form </param>
		///  <returns> a &times; b, in split form </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp[] splitMult(final Dfp[] a, final Dfp[] b)
		protected internal static Dfp[] splitMult(Dfp[] a, Dfp[] b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] result = new Dfp[2];
			Dfp[] result = new Dfp[2];

			result[1] = a[0].Zero;
			result[0] = a[0].multiply(b[0]);

			/* If result[0] is infinite or zero, don't compute result[1].
			 * Attempting to do so may produce NaNs.
			 */

			if (result[0].classify() == Dfp.INFINITE || result[0].Equals(result[1]))
			{
				return result;
			}

			result[1] = a[0].multiply(b[1]).add(a[1].multiply(b[0])).add(a[1].multiply(b[1]));

			return result;
		}

		/// <summary>
		/// Divide two numbers that are split in to two pieces that are meant to be added together.
		/// Inverse of split multiply above:
		///  (a+b) / (c+d) = (a/c) + ( (bc-ad)/(c**2+cd) ) </summary>
		///  <param name="a"> dividend, in split form </param>
		///  <param name="b"> divisor, in split form </param>
		///  <returns> a / b, in split form </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp[] splitDiv(final Dfp[] a, final Dfp[] b)
		protected internal static Dfp[] splitDiv(Dfp[] a, Dfp[] b)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] result;
			Dfp[] result;

			result = new Dfp[2];

			result[0] = a[0].divide(b[0]);
			result[1] = a[1].multiply(b[0]).subtract(a[0].multiply(b[1]));
			result[1] = result[1].divide(b[0].multiply(b[0]).add(b[0].multiply(b[1])));

			return result;
		}

		/// <summary>
		/// Raise a split base to the a power. </summary>
		/// <param name="base"> number to raise </param>
		/// <param name="a"> power </param>
		/// <returns> base<sup>a</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp splitPow(final Dfp[] base, int a)
		protected internal static Dfp splitPow(Dfp[] @base, int a)
		{
			bool invert = false;

			Dfp[] r = new Dfp[2];

			Dfp[] result = new Dfp[2];
			result[0] = @base[0].One;
			result[1] = @base[0].Zero;

			if (a == 0)
			{
				// Special case a = 0
				return result[0].add(result[1]);
			}

			if (a < 0)
			{
				// If a is less than zero
				invert = true;
				a = -a;
			}

			// Exponentiate by successive squaring
			do
			{
				r[0] = new Dfp(@base[0]);
				r[1] = new Dfp(@base[1]);
				int trial = 1;

				int prevtrial;
				while (true)
				{
					prevtrial = trial;
					trial *= 2;
					if (trial > a)
					{
						break;
					}
					r = splitMult(r, r);
				}

				trial = prevtrial;

				a -= trial;
				result = splitMult(result, r);

			} while (a >= 1);

			result[0] = result[0].add(result[1]);

			if (invert)
			{
				result[0] = @base[0].One.divide(result[0]);
			}

			return result[0];

		}

		/// <summary>
		/// Raises base to the power a by successive squaring. </summary>
		/// <param name="base"> number to raise </param>
		/// <param name="a"> power </param>
		/// <returns> base<sup>a</sup> </returns>
		public static Dfp pow(Dfp @base, int a)
		{
			bool invert = false;

			Dfp result = @base.One;

			if (a == 0)
			{
				// Special case
				return result;
			}

			if (a < 0)
			{
				invert = true;
				a = -a;
			}

			// Exponentiate by successive squaring
			do
			{
				Dfp r = new Dfp(@base);
				Dfp prevr;
				int trial = 1;
				int prevtrial;

				do
				{
					prevr = new Dfp(r);
					prevtrial = trial;
					r = r.multiply(r);
					trial *= 2;
				} while (a > trial);

				r = prevr;
				trial = prevtrial;

				a -= trial;
				result = result.multiply(r);

			} while (a >= 1);

			if (invert)
			{
				result = @base.One.divide(result);
			}

			return @base.newInstance(result);

		}

		/// <summary>
		/// Computes e to the given power.
		/// a is broken into two parts, such that a = n+m  where n is an integer.
		/// We use pow() to compute e<sup>n</sup> and a Taylor series to compute
		/// e<sup>m</sup>.  We return e*<sup>n</sup> &times; e<sup>m</sup> </summary>
		/// <param name="a"> power at which e should be raised </param>
		/// <returns> e<sup>a</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp exp(final Dfp a)
		public static Dfp exp(Dfp a)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp inta = a.rint();
			Dfp inta = a.rint();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp fraca = a.subtract(inta);
			Dfp fraca = a.subtract(inta);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ia = inta.intValue();
			int ia = (int)inta;
			if (ia > 2147483646)
			{
				// return +Infinity
				return a.newInstance((sbyte)1, Dfp.INFINITE);
			}

			if (ia < -2147483646)
			{
				// return 0;
				return a.newInstance();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp einta = splitPow(a.getField().getESplit(), ia);
			Dfp einta = splitPow(a.Field.ESplit, ia);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp efraca = expInternal(fraca);
			Dfp efraca = expInternal(fraca);

			return einta.multiply(efraca);
		}

		/// <summary>
		/// Computes e to the given power.
		/// Where -1 < a < 1.  Use the classic Taylor series.  1 + x**2/2! + x**3/3! + x**4/4!  ... </summary>
		/// <param name="a"> power at which e should be raised </param>
		/// <returns> e<sup>a</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp expInternal(final Dfp a)
		protected internal static Dfp expInternal(Dfp a)
		{
			Dfp y = a.One;
			Dfp x = a.One;
			Dfp fact = a.One;
			Dfp py = new Dfp(y);

			for (int i = 1; i < 90; i++)
			{
				x = x.multiply(a);
				fact = fact.divide(i);
				y = y.add(x.multiply(fact));
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			return y;
		}

		/// <summary>
		/// Returns the natural logarithm of a.
		/// a is first split into three parts such that  a = (10000^h)(2^j)k.
		/// ln(a) is computed by ln(a) = ln(5)*h + ln(2)*(h+j) + ln(k)
		/// k is in the range 2/3 < k <4/3 and is passed on to a series expansion. </summary>
		/// <param name="a"> number from which logarithm is requested </param>
		/// <returns> log(a) </returns>
		public static Dfp log(Dfp a)
		{
			int lr;
			Dfp x;
			int ix;
			int p2 = 0;

			// Check the arguments somewhat here
			if (a.Equals(a.Zero) || a.lessThan(a.Zero) || a.NaN)
			{
				// negative, zero or NaN
				a.Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				return a.dotrap(DfpField.FLAG_INVALID, "ln", a, a.newInstance((sbyte)1, Dfp.QNAN));
			}

			if (a.classify() == Dfp.INFINITE)
			{
				return a;
			}

			x = new Dfp(a);
			lr = x.log10K();

			x = x.divide(pow(a.newInstance(unchecked((sbyte)10000)), lr)); // This puts x in the range 0-10000
			ix = (int)x.floor();

			while (ix > 2)
			{
				ix >>= 1;
				p2++;
			}


			Dfp[] spx = Split(x);
			Dfp[] spy = new Dfp[2];
			spy[0] = pow(a.Two, p2); // use spy[0] temporarily as a divisor
			spx[0] = spx[0].divide(spy[0]);
			spx[1] = spx[1].divide(spy[0]);

			spy[0] = a.newInstance("1.33333"); // Use spy[0] for comparison
			while (spx[0].add(spx[1]).greaterThan(spy[0]))
			{
				spx[0] = spx[0].divide(2);
				spx[1] = spx[1].divide(2);
				p2++;
			}

			// X is now in the range of 2/3 < x < 4/3
			Dfp[] spz = logInternal(spx);

			spx[0] = a.newInstance((new StringBuilder()).Append(p2 + 4 * lr).ToString());
			spx[1] = a.Zero;
			spy = splitMult(a.Field.Ln2Split, spx);

			spz[0] = spz[0].add(spy[0]);
			spz[1] = spz[1].add(spy[1]);

			spx[0] = a.newInstance((new StringBuilder()).Append(4 * lr).ToString());
			spx[1] = a.Zero;
			spy = splitMult(a.Field.Ln5Split, spx);

			spz[0] = spz[0].add(spy[0]);
			spz[1] = spz[1].add(spy[1]);

			return a.newInstance(spz[0].add(spz[1]));

		}

		/// <summary>
		/// Computes the natural log of a number between 0 and 2.
		///  Let f(x) = ln(x),
		/// 
		///  We know that f'(x) = 1/x, thus from Taylor's theorum we have:
		/// 
		///           -----          n+1         n
		///  f(x) =   \           (-1)    (x - 1)
		///           /          ----------------    for 1 <= n <= infinity
		///           -----             n
		/// 
		///  or
		///                       2        3       4
		///                   (x-1)   (x-1)    (x-1)
		///  ln(x) =  (x-1) - ----- + ------ - ------ + ...
		///                     2       3        4
		/// 
		///  alternatively,
		/// 
		///                  2    3   4
		///                 x    x   x
		///  ln(x+1) =  x - -  + - - - + ...
		///                 2    3   4
		/// 
		///  This series can be used to compute ln(x), but it converges too slowly.
		/// 
		///  If we substitute -x for x above, we get
		/// 
		///                   2    3    4
		///                  x    x    x
		///  ln(1-x) =  -x - -  - -  - - + ...
		///                  2    3    4
		/// 
		///  Note that all terms are now negative.  Because the even powered ones
		///  absorbed the sign.  Now, subtract the series above from the previous
		///  one to get ln(x+1) - ln(1-x).  Note the even terms cancel out leaving
		///  only the odd ones
		/// 
		///                             3     5      7
		///                           2x    2x     2x
		///  ln(x+1) - ln(x-1) = 2x + --- + --- + ---- + ...
		///                            3     5      7
		/// 
		///  By the property of logarithms that ln(a) - ln(b) = ln (a/b) we have:
		/// 
		///                                3        5        7
		///      x+1           /          x        x        x          \
		///  ln ----- =   2 *  |  x  +   ----  +  ----  +  ---- + ...  |
		///      x-1           \          3        5        7          /
		/// 
		///  But now we want to find ln(a), so we need to find the value of x
		///  such that a = (x+1)/(x-1).   This is easily solved to find that
		///  x = (a-1)/(a+1). </summary>
		/// <param name="a"> number from which logarithm is requested, in split form </param>
		/// <returns> log(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp[] logInternal(final Dfp a[])
		protected internal static Dfp[] logInternal(Dfp[] a)
		{

			/* Now we want to compute x = (a-1)/(a+1) but this is prone to
			 * loss of precision.  So instead, compute x = (a/4 - 1/4) / (a/4 + 1/4)
			 */
			Dfp t = a[0].divide(4).add(a[1].divide(4));
			Dfp x = t.add(a[0].newInstance("-0.25")).divide(t.add(a[0].newInstance("0.25")));

			Dfp y = new Dfp(x);
			Dfp num = new Dfp(x);
			Dfp py = new Dfp(y);
			int den = 1;
			for (int i = 0; i < 10000; i++)
			{
				num = num.multiply(x);
				num = num.multiply(x);
				den += 2;
				t = num.divide(den);
				y = y.add(t);
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			y = y.multiply(a[0].Two);

			return Split(y);

		}

		/// <summary>
		/// Computes x to the y power.<p>
		/// 
		///  Uses the following method:<p>
		/// 
		///  <ol>
		///  <li> Set u = rint(y), v = y-u
		///  <li> Compute a = v * ln(x)
		///  <li> Compute b = rint( a/ln(2) )
		///  <li> Compute c = a - b*ln(2)
		///  <li> x<sup>y</sup> = x<sup>u</sup>  *   2<sup>b</sup> * e<sup>c</sup>
		///  </ol>
		///  if |y| > 1e8, then we compute by exp(y*ln(x))   <p>
		/// 
		///  <b>Special Cases</b><p>
		///  <ul>
		///  <li>  if y is 0.0 or -0.0 then result is 1.0
		///  <li>  if y is 1.0 then result is x
		///  <li>  if y is NaN then result is NaN
		///  <li>  if x is NaN and y is not zero then result is NaN
		///  <li>  if |x| > 1.0 and y is +Infinity then result is +Infinity
		///  <li>  if |x| < 1.0 and y is -Infinity then result is +Infinity
		///  <li>  if |x| > 1.0 and y is -Infinity then result is +0
		///  <li>  if |x| < 1.0 and y is +Infinity then result is +0
		///  <li>  if |x| = 1.0 and y is +/-Infinity then result is NaN
		///  <li>  if x = +0 and y > 0 then result is +0
		///  <li>  if x = +Inf and y < 0 then result is +0
		///  <li>  if x = +0 and y < 0 then result is +Inf
		///  <li>  if x = +Inf and y > 0 then result is +Inf
		///  <li>  if x = -0 and y > 0, finite, not odd integer then result is +0
		///  <li>  if x = -0 and y < 0, finite, and odd integer then result is -Inf
		///  <li>  if x = -Inf and y > 0, finite, and odd integer then result is -Inf
		///  <li>  if x = -0 and y < 0, not finite odd integer then result is +Inf
		///  <li>  if x = -Inf and y > 0, not finite odd integer then result is +Inf
		///  <li>  if x < 0 and y > 0, finite, and odd integer then result is -(|x|<sup>y</sup>)
		///  <li>  if x < 0 and y > 0, finite, and not integer then result is NaN
		///  </ul> </summary>
		///  <param name="x"> base to be raised </param>
		///  <param name="y"> power to which base should be raised </param>
		///  <returns> x<sup>y</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp pow(Dfp x, final Dfp y)
		public static Dfp pow(Dfp x, Dfp y)
		{

			// make sure we don't mix number with different precision
			if (x.Field.RadixDigits != y.Field.RadixDigits)
			{
				x.Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = x.newInstance(x.getZero());
				Dfp result = x.newInstance(x.Zero);
				result.nans = Dfp.QNAN;
				return x.dotrap(DfpField.FLAG_INVALID, POW_TRAP, x, result);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp zero = x.getZero();
			Dfp zero = x.Zero;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp one = x.getOne();
			Dfp one = x.One;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp two = x.getTwo();
			Dfp two = x.Two;
			bool invert = false;
			int ui;

			/* Check for special cases */
			if (y.Equals(zero))
			{
				return x.newInstance(one);
			}

			if (y.Equals(one))
			{
				if (x.NaN)
				{
					// Test for NaNs
					x.Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					return x.dotrap(DfpField.FLAG_INVALID, POW_TRAP, x, x);
				}
				return x;
			}

			if (x.NaN || y.NaN)
			{
				// Test for NaNs
				x.Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				return x.dotrap(DfpField.FLAG_INVALID, POW_TRAP, x, x.newInstance((sbyte)1, Dfp.QNAN));
			}

			// X == 0
			if (x.Equals(zero))
			{
				if (Dfp.copysign(one, x).greaterThan(zero))
				{
					// X == +0
					if (y.greaterThan(zero))
					{
						return x.newInstance(zero);
					}
					else
					{
						return x.newInstance(x.newInstance((sbyte)1, Dfp.INFINITE));
					}
				}
				else
				{
					// X == -0
					if (y.classify() == Dfp.FINITE && y.rint().Equals(y) && !y.remainder(two).Equals(zero))
					{
						// If y is odd integer
						if (y.greaterThan(zero))
						{
							return x.newInstance(zero.negate());
						}
						else
						{
							return x.newInstance(x.newInstance((sbyte) - 1, Dfp.INFINITE));
						}
					}
					else
					{
						// Y is not odd integer
						if (y.greaterThan(zero))
						{
							return x.newInstance(zero);
						}
						else
						{
							return x.newInstance(x.newInstance((sbyte)1, Dfp.INFINITE));
						}
					}
				}
			}

			if (x.lessThan(zero))
			{
				// Make x positive, but keep track of it
				x = x.negate();
				invert = true;
			}

			if (x.greaterThan(one) && y.classify() == Dfp.INFINITE)
			{
				if (y.greaterThan(zero))
				{
					return y;
				}
				else
				{
					return x.newInstance(zero);
				}
			}

			if (x.lessThan(one) && y.classify() == Dfp.INFINITE)
			{
				if (y.greaterThan(zero))
				{
					return x.newInstance(zero);
				}
				else
				{
					return x.newInstance(Dfp.copysign(y, one));
				}
			}

			if (x.Equals(one) && y.classify() == Dfp.INFINITE)
			{
				x.Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				return x.dotrap(DfpField.FLAG_INVALID, POW_TRAP, x, x.newInstance((sbyte)1, Dfp.QNAN));
			}

			if (x.classify() == Dfp.INFINITE)
			{
				// x = +/- inf
				if (invert)
				{
					// negative infinity
					if (y.classify() == Dfp.FINITE && y.rint().Equals(y) && !y.remainder(two).Equals(zero))
					{
						// If y is odd integer
						if (y.greaterThan(zero))
						{
							return x.newInstance(x.newInstance((sbyte) - 1, Dfp.INFINITE));
						}
						else
						{
							return x.newInstance(zero.negate());
						}
					}
					else
					{
						// Y is not odd integer
						if (y.greaterThan(zero))
						{
							return x.newInstance(x.newInstance((sbyte)1, Dfp.INFINITE));
						}
						else
						{
							return x.newInstance(zero);
						}
					}
				}
				else
				{
					// positive infinity
					if (y.greaterThan(zero))
					{
						return x;
					}
					else
					{
						return x.newInstance(zero);
					}
				}
			}

			if (invert && !y.rint().Equals(y))
			{
				x.Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				return x.dotrap(DfpField.FLAG_INVALID, POW_TRAP, x, x.newInstance((sbyte)1, Dfp.QNAN));
			}

			// End special cases

			Dfp r;
			if (y.lessThan(x.newInstance(unchecked((sbyte)100000000))) && y.greaterThan(x.newInstance(unchecked((sbyte) - 100000000))))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp u = y.rint();
				Dfp u = y.rint();
				ui = (int)u;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp v = y.subtract(u);
				Dfp v = y.subtract(u);

				if (v.unequal(zero))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp a = v.multiply(log(x));
					Dfp a = v.multiply(log(x));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp b = a.divide(x.getField().getLn2()).rint();
					Dfp b = a.divide(x.Field.Ln2).rint();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp c = a.subtract(b.multiply(x.getField().getLn2()));
					Dfp c = a.subtract(b.multiply(x.Field.Ln2));
					r = splitPow(Split(x), ui);
					r = r.multiply(pow(two, (int)b));
					r = r.multiply(exp(c));
				}
				else
				{
					r = splitPow(Split(x), ui);
				}
			}
			else
			{
				// very large exponent.  |y| > 1e8
				r = exp(log(x).multiply(y));
			}

			if (invert && y.rint().Equals(y) && !y.remainder(two).Equals(zero))
			{
				// if y is odd integer
				r = r.negate();
			}

			return x.newInstance(r);

		}

		/// <summary>
		/// Computes sin(a)  Used when 0 < a < pi/4.
		/// Uses the classic Taylor series.  x - x**3/3! + x**5/5!  ... </summary>
		/// <param name="a"> number from which sine is desired, in split form </param>
		/// <returns> sin(a) </returns>
		protected internal static Dfp sinInternal(Dfp[] a)
		{

			Dfp c = a[0].add(a[1]);
			Dfp y = c;
			c = c.multiply(c);
			Dfp x = y;
			Dfp fact = a[0].One;
			Dfp py = new Dfp(y);

			for (int i = 3; i < 90; i += 2)
			{
				x = x.multiply(c);
				x = x.negate();

				fact = fact.divide((i - 1) * i); // 1 over fact
				y = y.add(x.multiply(fact));
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			return y;

		}

		/// <summary>
		/// Computes cos(a)  Used when 0 < a < pi/4.
		/// Uses the classic Taylor series for cosine.  1 - x**2/2! + x**4/4!  ... </summary>
		/// <param name="a"> number from which cosine is desired, in split form </param>
		/// <returns> cos(a) </returns>
		protected internal static Dfp cosInternal(Dfp[] a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp one = a[0].getOne();
			Dfp one = a[0].One;


			Dfp x = one;
			Dfp y = one;
			Dfp c = a[0].add(a[1]);
			c = c.multiply(c);

			Dfp fact = one;
			Dfp py = new Dfp(y);

			for (int i = 2; i < 90; i += 2)
			{
				x = x.multiply(c);
				x = x.negate();

				fact = fact.divide((i - 1) * i); // 1 over fact

				y = y.add(x.multiply(fact));
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			return y;

		}

		/// <summary>
		/// computes the sine of the argument. </summary>
		/// <param name="a"> number from which sine is desired </param>
		/// <returns> sin(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp sin(final Dfp a)
		public static Dfp sin(Dfp a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp pi = a.getField().getPi();
			Dfp pi = a.Field.Pi;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp zero = a.getField().getZero();
			Dfp zero = a.Field.Zero;
			bool neg = false;

			/* First reduce the argument to the range of +/- PI */
			Dfp x = a.remainder(pi.multiply(2));

			/* if x < 0 then apply identity sin(-x) = -sin(x) */
			/* This puts x in the range 0 < x < PI            */
			if (x.lessThan(zero))
			{
				x = x.negate();
				neg = true;
			}

			/* Since sine(x) = sine(pi - x) we can reduce the range to
			 * 0 < x < pi/2
			 */

			if (x.greaterThan(pi.divide(2)))
			{
				x = pi.subtract(x);
			}

			Dfp y;
			if (x.lessThan(pi.divide(4)))
			{
				Dfp[] c = new Dfp[2];
				c[0] = x;
				c[1] = zero;

				//y = sinInternal(c);
				y = sinInternal(Split(x));
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp c[] = new Dfp[2];
				Dfp[] c = new Dfp[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] piSplit = a.getField().getPiSplit();
				Dfp[] piSplit = a.Field.PiSplit;
				c[0] = piSplit[0].divide(2).subtract(x);
				c[1] = piSplit[1].divide(2);
				y = cosInternal(c);
			}

			if (neg)
			{
				y = y.negate();
			}

			return a.newInstance(y);

		}

		/// <summary>
		/// computes the cosine of the argument. </summary>
		/// <param name="a"> number from which cosine is desired </param>
		/// <returns> cos(a) </returns>
		public static Dfp cos(Dfp a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp pi = a.getField().getPi();
			Dfp pi = a.Field.Pi;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp zero = a.getField().getZero();
			Dfp zero = a.Field.Zero;
			bool neg = false;

			/* First reduce the argument to the range of +/- PI */
			Dfp x = a.remainder(pi.multiply(2));

			/* if x < 0 then apply identity cos(-x) = cos(x) */
			/* This puts x in the range 0 < x < PI           */
			if (x.lessThan(zero))
			{
				x = x.negate();
			}

			/* Since cos(x) = -cos(pi - x) we can reduce the range to
			 * 0 < x < pi/2
			 */

			if (x.greaterThan(pi.divide(2)))
			{
				x = pi.subtract(x);
				neg = true;
			}

			Dfp y;
			if (x.lessThan(pi.divide(4)))
			{
				Dfp[] c = new Dfp[2];
				c[0] = x;
				c[1] = zero;

				y = cosInternal(c);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp c[] = new Dfp[2];
				Dfp[] c = new Dfp[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] piSplit = a.getField().getPiSplit();
				Dfp[] piSplit = a.Field.PiSplit;
				c[0] = piSplit[0].divide(2).subtract(x);
				c[1] = piSplit[1].divide(2);
				y = sinInternal(c);
			}

			if (neg)
			{
				y = y.negate();
			}

			return a.newInstance(y);

		}

		/// <summary>
		/// computes the tangent of the argument. </summary>
		/// <param name="a"> number from which tangent is desired </param>
		/// <returns> tan(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp tan(final Dfp a)
		public static Dfp tan(Dfp a)
		{
			return sin(a).divide(cos(a));
		}

		/// <summary>
		/// computes the arc-tangent of the argument. </summary>
		/// <param name="a"> number from which arc-tangent is desired </param>
		/// <returns> atan(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static Dfp atanInternal(final Dfp a)
		protected internal static Dfp atanInternal(Dfp a)
		{

			Dfp y = new Dfp(a);
			Dfp x = new Dfp(y);
			Dfp py = new Dfp(y);

			for (int i = 3; i < 90; i += 2)
			{
				x = x.multiply(a);
				x = x.multiply(a);
				x = x.negate();
				y = y.add(x.divide(i));
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			return y;

		}

		/// <summary>
		/// computes the arc tangent of the argument
		/// 
		///  Uses the typical taylor series
		/// 
		///  but may reduce arguments using the following identity
		/// tan(x+y) = (tan(x) + tan(y)) / (1 - tan(x)*tan(y))
		/// 
		/// since tan(PI/8) = sqrt(2)-1,
		/// 
		/// atan(x) = atan( (x - sqrt(2) + 1) / (1+x*sqrt(2) - x) + PI/8.0 </summary>
		/// <param name="a"> number from which arc-tangent is desired </param>
		/// <returns> atan(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp atan(final Dfp a)
		public static Dfp atan(Dfp a)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp zero = a.getField().getZero();
			Dfp zero = a.Field.Zero;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp one = a.getField().getOne();
			Dfp one = a.Field.One;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] sqr2Split = a.getField().getSqr2Split();
			Dfp[] sqr2Split = a.Field.Sqr2Split;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp[] piSplit = a.getField().getPiSplit();
			Dfp[] piSplit = a.Field.PiSplit;
			bool recp = false;
			bool neg = false;
			bool sub = false;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp ty = sqr2Split[0].subtract(one).add(sqr2Split[1]);
			Dfp ty = sqr2Split[0].subtract(one).add(sqr2Split[1]);

			Dfp x = new Dfp(a);
			if (x.lessThan(zero))
			{
				neg = true;
				x = x.negate();
			}

			if (x.greaterThan(one))
			{
				recp = true;
				x = one.divide(x);
			}

			if (x.greaterThan(ty))
			{
				Dfp[] sty = new Dfp[2];
				sub = true;

				sty[0] = sqr2Split[0].subtract(one);
				sty[1] = sqr2Split[1];

				Dfp[] xs = Split(x);

				Dfp[] ds = splitMult(xs, sty);
				ds[0] = ds[0].add(one);

				xs[0] = xs[0].subtract(sty[0]);
				xs[1] = xs[1].subtract(sty[1]);

				xs = splitDiv(xs, ds);
				x = xs[0].add(xs[1]);

				//x = x.subtract(ty).divide(dfp.one.add(x.multiply(ty)));
			}

			Dfp y = atanInternal(x);

			if (sub)
			{
				y = y.add(piSplit[0].divide(8)).add(piSplit[1].divide(8));
			}

			if (recp)
			{
				y = piSplit[0].divide(2).subtract(y).add(piSplit[1].divide(2));
			}

			if (neg)
			{
				y = y.negate();
			}

			return a.newInstance(y);

		}

		/// <summary>
		/// computes the arc-sine of the argument. </summary>
		/// <param name="a"> number from which arc-sine is desired </param>
		/// <returns> asin(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp asin(final Dfp a)
		public static Dfp asin(Dfp a)
		{
			return atan(a.divide(a.One.subtract(a.multiply(a)).sqrt()));
		}

		/// <summary>
		/// computes the arc-cosine of the argument. </summary>
		/// <param name="a"> number from which arc-cosine is desired </param>
		/// <returns> acos(a) </returns>
		public static Dfp acos(Dfp a)
		{
			Dfp result;
			bool negative = false;

			if (a.lessThan(a.Zero))
			{
				negative = true;
			}

			a = Dfp.copysign(a, a.One); // absolute value

			result = atan(a.One.subtract(a.multiply(a)).sqrt().divide(a));

			if (negative)
			{
				result = a.Field.Pi.subtract(result);
			}

			return a.newInstance(result);
		}

	}

}