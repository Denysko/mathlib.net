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

namespace org.apache.commons.math3.dfp
{

	using org.apache.commons.math3;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	///  Decimal floating point library for Java
	/// 
	///  <p>Another floating point class.  This one is built using radix 10000
	///  which is 10<sup>4</sup>, so its almost decimal.</p>
	/// 
	///  <p>The design goals here are:
	///  <ol>
	///    <li>Decimal math, or close to it</li>
	///    <li>Settable precision (but no mix between numbers using different settings)</li>
	///    <li>Portability.  Code should be kept as portable as possible.</li>
	///    <li>Performance</li>
	///    <li>Accuracy  - Results should always be +/- 1 ULP for basic
	///         algebraic operation</li>
	///    <li>Comply with IEEE 854-1987 as much as possible.
	///         (See IEEE 854-1987 notes below)</li>
	///  </ol></p>
	/// 
	///  <p>Trade offs:
	///  <ol>
	///    <li>Memory foot print.  I'm using more memory than necessary to
	///         represent numbers to get better performance.</li>
	///    <li>Digits are bigger, so rounding is a greater loss.  So, if you
	///         really need 12 decimal digits, better use 4 base 10000 digits
	///         there can be one partially filled.</li>
	///  </ol></p>
	/// 
	///  <p>Numbers are represented  in the following form:
	///  <pre>
	///  n  =  sign &times; mant &times; (radix)<sup>exp</sup>;</p>
	///  </pre>
	///  where sign is &plusmn;1, mantissa represents a fractional number between
	///  zero and one.  mant[0] is the least significant digit.
	///  exp is in the range of -32767 to 32768</p>
	/// 
	///  <p>IEEE 854-1987  Notes and differences</p>
	/// 
	///  <p>IEEE 854 requires the radix to be either 2 or 10.  The radix here is
	///  10000, so that requirement is not met, but  it is possible that a
	///  subclassed can be made to make it behave as a radix 10
	///  number.  It is my opinion that if it looks and behaves as a radix
	///  10 number then it is one and that requirement would be met.</p>
	/// 
	///  <p>The radix of 10000 was chosen because it should be faster to operate
	///  on 4 decimal digits at once instead of one at a time.  Radix 10 behavior
	///  can be realized by adding an additional rounding step to ensure that
	///  the number of decimal digits represented is constant.</p>
	/// 
	///  <p>The IEEE standard specifically leaves out internal data encoding,
	///  so it is reasonable to conclude that such a subclass of this radix
	///  10000 system is merely an encoding of a radix 10 system.</p>
	/// 
	///  <p>IEEE 854 also specifies the existence of "sub-normal" numbers.  This
	///  class does not contain any such entities.  The most significant radix
	///  10000 digit is always non-zero.  Instead, we support "gradual underflow"
	///  by raising the underflow flag for numbers less with exponent less than
	///  expMin, but don't flush to zero until the exponent reaches MIN_EXP-digits.
	///  Thus the smallest number we can represent would be:
	///  1E(-(MIN_EXP-digits-1)*4),  eg, for digits=5, MIN_EXP=-32767, that would
	///  be 1e-131092.</p>
	/// 
	///  <p>IEEE 854 defines that the implied radix point lies just to the right
	///  of the most significant digit and to the left of the remaining digits.
	///  This implementation puts the implied radix point to the left of all
	///  digits including the most significant one.  The most significant digit
	///  here is the one just to the right of the radix point.  This is a fine
	///  detail and is really only a matter of definition.  Any side effects of
	///  this can be rendered invisible by a subclass.</p> </summary>
	/// <seealso cref= DfpField
	/// @version $Id: Dfp.java 1539704 2013-11-07 16:34:51Z tn $
	/// @since 2.2 </seealso>
	public class Dfp : RealFieldElement<Dfp>
	{

		/// <summary>
		/// The radix, or base of this system.  Set to 10000 </summary>
		public const int RADIX = 10000;

		/// <summary>
		/// The minimum exponent before underflow is signaled.  Flush to zero
		///  occurs at minExp-DIGITS 
		/// </summary>
		public const int MIN_EXP = -32767;

		/// <summary>
		/// The maximum exponent before overflow is signaled and results flushed
		///  to infinity 
		/// </summary>
		public const int MAX_EXP = 32768;

		/// <summary>
		/// The amount under/overflows are scaled by before going to trap handler </summary>
		public const int ERR_SCALE = 32760;

		/// <summary>
		/// Indicator value for normal finite numbers. </summary>
		public const sbyte FINITE = 0;

		/// <summary>
		/// Indicator value for Infinity. </summary>
		public const sbyte INFINITE = 1;

		/// <summary>
		/// Indicator value for signaling NaN. </summary>
		public const sbyte SNAN = 2;

		/// <summary>
		/// Indicator value for quiet NaN. </summary>
		public const sbyte QNAN = 3;

		/// <summary>
		/// String for NaN representation. </summary>
		private const string NAN_STRING = "NaN";

		/// <summary>
		/// String for positive infinity representation. </summary>
		private const string POS_INFINITY_STRING = "Infinity";

		/// <summary>
		/// String for negative infinity representation. </summary>
		private const string NEG_INFINITY_STRING = "-Infinity";

		/// <summary>
		/// Name for traps triggered by addition. </summary>
		private const string ADD_TRAP = "add";

		/// <summary>
		/// Name for traps triggered by multiplication. </summary>
		private const string MULTIPLY_TRAP = "multiply";

		/// <summary>
		/// Name for traps triggered by division. </summary>
		private const string DIVIDE_TRAP = "divide";

		/// <summary>
		/// Name for traps triggered by square root. </summary>
		private const string SQRT_TRAP = "sqrt";

		/// <summary>
		/// Name for traps triggered by alignment. </summary>
		private const string ALIGN_TRAP = "align";

		/// <summary>
		/// Name for traps triggered by truncation. </summary>
		private const string TRUNC_TRAP = "trunc";

		/// <summary>
		/// Name for traps triggered by nextAfter. </summary>
		private const string NEXT_AFTER_TRAP = "nextAfter";

		/// <summary>
		/// Name for traps triggered by lessThan. </summary>
		private const string LESS_THAN_TRAP = "lessThan";

		/// <summary>
		/// Name for traps triggered by greaterThan. </summary>
		private const string GREATER_THAN_TRAP = "greaterThan";

		/// <summary>
		/// Name for traps triggered by newInstance. </summary>
		private const string NEW_INSTANCE_TRAP = "newInstance";

		/// <summary>
		/// Mantissa. </summary>
		protected internal int[] mant;

		/// <summary>
		/// Sign bit: 1 for positive, -1 for negative. </summary>
		protected internal sbyte sign;

		/// <summary>
		/// Exponent. </summary>
		protected internal int exp_Renamed;

		/// <summary>
		/// Indicator for non-finite / non-number values. </summary>
		protected internal sbyte nans;

		/// <summary>
		/// Factory building similar Dfp's. </summary>
		private readonly DfpField field;

		/// <summary>
		/// Makes an instance with a value of zero. </summary>
		/// <param name="field"> field to which this instance belongs </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field)
		protected internal Dfp(DfpField field)
		{
			mant = new int[field.RadixDigits];
			sign = 1;
			exp_Renamed = 0;
			nans = FINITE;
			this.field = field;
		}

		/// <summary>
		/// Create an instance from a byte value. </summary>
		/// <param name="field"> field to which this instance belongs </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field, byte x)
		protected internal Dfp(DfpField field, sbyte x) : this(field, (long) x)
		{
		}

		/// <summary>
		/// Create an instance from an int value. </summary>
		/// <param name="field"> field to which this instance belongs </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field, int x)
		protected internal Dfp(DfpField field, int x) : this(field, (long) x)
		{
		}

		/// <summary>
		/// Create an instance from a long value. </summary>
		/// <param name="field"> field to which this instance belongs </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field, long x)
		protected internal Dfp(DfpField field, long x)
		{

			// initialize as if 0
			mant = new int[field.RadixDigits];
			nans = FINITE;
			this.field = field;

			bool isLongMin = false;
			if (x == long.MinValue)
			{
				// special case for Long.MIN_VALUE (-9223372036854775808)
				// we must shift it before taking its absolute value
				isLongMin = true;
				++x;
			}

			// set the sign
			if (x < 0)
			{
				sign = -1;
				x = -x;
			}
			else
			{
				sign = 1;
			}

			exp_Renamed = 0;
			while (x != 0)
			{
				Array.Copy(mant, mant.Length - exp_Renamed, mant, mant.Length - 1 - exp_Renamed, exp_Renamed);
				mant[mant.Length - 1] = (int)(x % RADIX);
				x /= RADIX;
				exp_Renamed++;
			}

			if (isLongMin)
			{
				// remove the shift added for Long.MIN_VALUE
				// we know in this case that fixing the last digit is sufficient
				for (int i = 0; i < mant.Length - 1; i++)
				{
					if (mant[i] != 0)
					{
						mant[i]++;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Create an instance from a double value. </summary>
		/// <param name="field"> field to which this instance belongs </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field, double x)
		protected internal Dfp(DfpField field, double x)
		{

			// initialize as if 0
			mant = new int[field.RadixDigits];
			sign = 1;
			exp_Renamed = 0;
			nans = FINITE;
			this.field = field;

			long bits = double.doubleToLongBits(x);
			long mantissa = bits & 0x000fffffffffffffL;
			int exponent = (int)((bits & 0x7ff0000000000000L) >> 52) - 1023;

			if (exponent == -1023)
			{
				// Zero or sub-normal
				if (x == 0)
				{
					// make sure 0 has the right sign
					if ((bits & 0x8000000000000000L) != 0)
					{
						sign = -1;
					}
					return;
				}

				exponent++;

				// Normalize the subnormal number
				while ((mantissa & 0x0010000000000000L) == 0)
				{
					exponent--;
					mantissa <<= 1;
				}
				mantissa &= 0x000fffffffffffffL;
			}

			if (exponent == 1024)
			{
				// infinity or NAN
				if (x != x)
				{
					sign = (sbyte) 1;
					nans = QNAN;
				}
				else if (x < 0)
				{
					sign = (sbyte) - 1;
					nans = INFINITE;
				}
				else
				{
					sign = (sbyte) 1;
					nans = INFINITE;
				}
				return;
			}

			Dfp xdfp = new Dfp(field, mantissa);
			xdfp = xdfp.divide(new Dfp(field, unchecked((sbyte)4503599627370496l))).add(field.One); // Divide by 2^52, then add one
			xdfp = xdfp.multiply(DfpMath.pow(field.Two, exponent));

			if ((bits & 0x8000000000000000L) != 0)
			{
				xdfp = xdfp.negate();
			}

			Array.Copy(xdfp.mant, 0, mant, 0, mant.Length);
			sign = xdfp.sign;
			exp_Renamed = xdfp.exp_Renamed;
			nans = xdfp.nans;

		}

		/// <summary>
		/// Copy constructor. </summary>
		/// <param name="d"> instance to copy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp(final Dfp d)
		public Dfp(Dfp d)
		{
			mant = d.mant.clone();
			sign = d.sign;
			exp_Renamed = d.exp_Renamed;
			nans = d.nans;
			field = d.field;
		}

		/// <summary>
		/// Create an instance from a String representation. </summary>
		/// <param name="field"> field to which this instance belongs </param>
		/// <param name="s"> string representation of the instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field, final String s)
		protected internal Dfp(DfpField field, string s)
		{

			// initialize as if 0
			mant = new int[field.RadixDigits];
			sign = 1;
			exp_Renamed = 0;
			nans = FINITE;
			this.field = field;

			bool decimalFound = false;
			const int rsize = 4; // size of radix in decimal digits
			const int offset = 4; // Starting offset into Striped
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final char[] striped = new char[getRadixDigits() * rsize + offset * 2];
			char[] striped = new char[RadixDigits * rsize + offset * 2];

			// Check some special cases
			if (s.Equals(POS_INFINITY_STRING))
			{
				sign = (sbyte) 1;
				nans = INFINITE;
				return;
			}

			if (s.Equals(NEG_INFINITY_STRING))
			{
				sign = (sbyte) - 1;
				nans = INFINITE;
				return;
			}

			if (s.Equals(NAN_STRING))
			{
				sign = (sbyte) 1;
				nans = QNAN;
				return;
			}

			// Check for scientific notation
			int p = s.IndexOf("e");
			if (p == -1) // try upper case?
			{
				p = s.IndexOf("E");
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fpdecimal;
			string fpdecimal;
			int sciexp = 0;
			if (p != -1)
			{
				// scientific notation
				fpdecimal = s.Substring(0, p);
				string fpexp = s.Substring(p + 1);
				bool negative = false;

				for (int i = 0; i < fpexp.Length; i++)
				{
					if (fpexp[i] == '-')
					{
						negative = true;
						continue;
					}
					if (fpexp[i] >= '0' && fpexp[i] <= '9')
					{
						sciexp = sciexp * 10 + fpexp[i] - '0';
					}
				}

				if (negative)
				{
					sciexp = -sciexp;
				}
			}
			else
			{
				// normal case
				fpdecimal = s;
			}

			// If there is a minus sign in the number then it is negative
			if (fpdecimal.IndexOf("-") != -1)
			{
				sign = -1;
			}

			// First off, find all of the leading zeros, trailing zeros, and significant digits
			p = 0;

			// Move p to first significant digit
			int decimalPos = 0;
			for (;;)
			{
				if (fpdecimal[p] >= '1' && fpdecimal[p] <= '9')
				{
					break;
				}

				if (decimalFound && fpdecimal[p] == '0')
				{
					decimalPos--;
				}

				if (fpdecimal[p] == '.')
				{
					decimalFound = true;
				}

				p++;

				if (p == fpdecimal.Length)
				{
					break;
				}
			}

			// Copy the string onto Stripped
			int q = offset;
			striped[0] = '0';
			striped[1] = '0';
			striped[2] = '0';
			striped[3] = '0';
			int significantDigits = 0;
			for (;;)
			{
				if (p == (fpdecimal.Length))
				{
					break;
				}

				// Don't want to run pass the end of the array
				if (q == mant.Length * rsize + offset + 1)
				{
					break;
				}

				if (fpdecimal[p] == '.')
				{
					decimalFound = true;
					decimalPos = significantDigits;
					p++;
					continue;
				}

				if (fpdecimal[p] < '0' || fpdecimal[p] > '9')
				{
					p++;
					continue;
				}

				striped[q] = fpdecimal[p];
				q++;
				p++;
				significantDigits++;
			}


			// If the decimal point has been found then get rid of trailing zeros.
			if (decimalFound && q != offset)
			{
				for (;;)
				{
					q--;
					if (q == offset)
					{
						break;
					}
					if (striped[q] == '0')
					{
						significantDigits--;
					}
					else
					{
						break;
					}
				}
			}

			// special case of numbers like "0.00000"
			if (decimalFound && significantDigits == 0)
			{
				decimalPos = 0;
			}

			// Implicit decimal point at end of number if not present
			if (!decimalFound)
			{
				decimalPos = q - offset;
			}

			// Find the number of significant trailing zeros
			q = offset; // set q to point to first sig digit
			p = significantDigits - 1 + offset;

			while (p > q)
			{
				if (striped[p] != '0')
				{
					break;
				}
				p--;
			}

			// Make sure the decimal is on a mod 10000 boundary
			int i = ((rsize * 100) - decimalPos - sciexp % rsize) % rsize;
			q -= i;
			decimalPos += i;

			// Make the mantissa length right by adding zeros at the end if necessary
			while ((p - q) < (mant.Length * rsize))
			{
				for (i = 0; i < rsize; i++)
				{
					striped[++p] = '0';
				}
			}

			// Ok, now we know how many trailing zeros there are,
			// and where the least significant digit is
			for (i = mant.Length - 1; i >= 0; i--)
			{
				mant[i] = (striped[q] - '0') * 1000 + (striped[q + 1] - '0') * 100 + (striped[q + 2] - '0') * 10 + (striped[q + 3] - '0');
				q += 4;
			}


			exp_Renamed = (decimalPos + sciexp) / rsize;

			if (q < striped.Length)
			{
				// Is there possible another digit?
				round((striped[q] - '0') * 1000);
			}

		}

		/// <summary>
		/// Creates an instance with a non-finite value. </summary>
		/// <param name="field"> field to which this instance belongs </param>
		/// <param name="sign"> sign of the Dfp to create </param>
		/// <param name="nans"> code of the value, must be one of <seealso cref="#INFINITE"/>,
		/// <seealso cref="#SNAN"/>,  <seealso cref="#QNAN"/> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp(final DfpField field, final byte sign, final byte nans)
		protected internal Dfp(DfpField field, sbyte sign, sbyte nans)
		{
			this.field = field;
			this.mant = new int[field.RadixDigits];
			this.sign = sign;
			this.exp_Renamed = 0;
			this.nans = nans;
		}

		/// <summary>
		/// Create an instance with a value of 0.
		/// Use this internally in preference to constructors to facilitate subclasses </summary>
		/// <returns> a new instance with a value of 0 </returns>
		public virtual Dfp newInstance()
		{
			return new Dfp(Field);
		}

		/// <summary>
		/// Create an instance from a byte value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new instance with value x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final byte x)
		public virtual Dfp newInstance(sbyte x)
		{
			return new Dfp(Field, x);
		}

		/// <summary>
		/// Create an instance from an int value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new instance with value x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final int x)
		public virtual Dfp newInstance(int x)
		{
			return new Dfp(Field, x);
		}

		/// <summary>
		/// Create an instance from a long value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new instance with value x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final long x)
		public virtual Dfp newInstance(long x)
		{
			return new Dfp(Field, x);
		}

		/// <summary>
		/// Create an instance from a double value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new instance with value x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final double x)
		public virtual Dfp newInstance(double x)
		{
			return new Dfp(Field, x);
		}

		/// <summary>
		/// Create an instance by copying an existing one.
		/// Use this internally in preference to constructors to facilitate subclasses. </summary>
		/// <param name="d"> instance to copy </param>
		/// <returns> a new instance with the same value as d </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final Dfp d)
		public virtual Dfp newInstance(Dfp d)
		{

			// make sure we don't mix number with different precision
			if (field.RadixDigits != d.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, NEW_INSTANCE_TRAP, d, result);
			}

			return new Dfp(d);

		}

		/// <summary>
		/// Create an instance from a String representation.
		/// Use this internally in preference to constructors to facilitate subclasses. </summary>
		/// <param name="s"> string representation of the instance </param>
		/// <returns> a new instance parsed from specified string </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final String s)
		public virtual Dfp newInstance(string s)
		{
			return new Dfp(field, s);
		}

		/// <summary>
		/// Creates an instance with a non-finite value. </summary>
		/// <param name="sig"> sign of the Dfp to create </param>
		/// <param name="code"> code of the value, must be one of <seealso cref="#INFINITE"/>,
		/// <seealso cref="#SNAN"/>,  <seealso cref="#QNAN"/> </param>
		/// <returns> a new instance with a non-finite value </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newInstance(final byte sig, final byte code)
		public virtual Dfp newInstance(sbyte sig, sbyte code)
		{
			return field.newDfp(sig, code);
		}

		/// <summary>
		/// Get the <seealso cref="org.apache.commons.math3.Field Field"/> (really a <seealso cref="DfpField"/>) to which the instance belongs.
		/// <p>
		/// The field is linked to the number of digits and acts as a factory
		/// for <seealso cref="Dfp"/> instances.
		/// </p> </summary>
		/// <returns> <seealso cref="org.apache.commons.math3.Field Field"/> (really a <seealso cref="DfpField"/>) to which the instance belongs </returns>
		public virtual DfpField Field
		{
			get
			{
				return field;
			}
		}

		/// <summary>
		/// Get the number of radix digits of the instance. </summary>
		/// <returns> number of radix digits </returns>
		public virtual int RadixDigits
		{
			get
			{
				return field.RadixDigits;
			}
		}

		/// <summary>
		/// Get the constant 0. </summary>
		/// <returns> a Dfp with value zero </returns>
		public virtual Dfp Zero
		{
			get
			{
				return field.Zero;
			}
		}

		/// <summary>
		/// Get the constant 1. </summary>
		/// <returns> a Dfp with value one </returns>
		public virtual Dfp One
		{
			get
			{
				return field.One;
			}
		}

		/// <summary>
		/// Get the constant 2. </summary>
		/// <returns> a Dfp with value two </returns>
		public virtual Dfp Two
		{
			get
			{
				return field.Two;
			}
		}

		/// <summary>
		/// Shift the mantissa left, and adjust the exponent to compensate.
		/// </summary>
		protected internal virtual void shiftLeft()
		{
			for (int i = mant.Length - 1; i > 0; i--)
			{
				mant[i] = mant[i - 1];
			}
			mant[0] = 0;
			exp_Renamed--;
		}

		/* Note that shiftRight() does not call round() as that round() itself
		 uses shiftRight() */
		/// <summary>
		/// Shift the mantissa right, and adjust the exponent to compensate.
		/// </summary>
		protected internal virtual void shiftRight()
		{
			for (int i = 0; i < mant.Length - 1; i++)
			{
				mant[i] = mant[i + 1];
			}
			mant[mant.Length - 1] = 0;
			exp_Renamed++;
		}

		/// <summary>
		/// Make our exp equal to the supplied one, this may cause rounding.
		///  Also causes de-normalized numbers.  These numbers are generally
		///  dangerous because most routines assume normalized numbers.
		///  Align doesn't round, so it will return the last digit destroyed
		///  by shifting right. </summary>
		///  <param name="e"> desired exponent </param>
		///  <returns> last digit destroyed by shifting right </returns>
		protected internal virtual int align(int e)
		{
			int lostdigit = 0;
			bool inexact = false;

			int diff = exp_Renamed - e;

			int adiff = diff;
			if (adiff < 0)
			{
				adiff = -adiff;
			}

			if (diff == 0)
			{
				return 0;
			}

			if (adiff > (mant.Length + 1))
			{
				// Special case
				Arrays.fill(mant, 0);
				exp_Renamed = e;

				field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				dotrap(DfpField.FLAG_INEXACT, ALIGN_TRAP, this, this);

				return 0;
			}

			for (int i = 0; i < adiff; i++)
			{
				if (diff < 0)
				{
					/* Keep track of loss -- only signal inexact after losing 2 digits.
					 * the first lost digit is returned to add() and may be incorporated
					 * into the result.
					 */
					if (lostdigit != 0)
					{
						inexact = true;
					}

					lostdigit = mant[0];

					shiftRight();
				}
				else
				{
					shiftLeft();
				}
			}

			if (inexact)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				dotrap(DfpField.FLAG_INEXACT, ALIGN_TRAP, this, this);
			}

			return lostdigit;

		}

		/// <summary>
		/// Check if instance is less than x. </summary>
		/// <param name="x"> number to check instance against </param>
		/// <returns> true if instance is less than x and neither are NaN, false otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean lessThan(final Dfp x)
		public virtual bool lessThan(Dfp x)
		{

			// make sure we don't mix number with different precision
			if (field.RadixDigits != x.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, x, result);
				return false;
			}

			/* if a nan is involved, signal invalid and return false */
			if (NaN || x.NaN)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, x, newInstance(Zero));
				return false;
			}

			return compare(this, x) < 0;
		}

		/// <summary>
		/// Check if instance is greater than x. </summary>
		/// <param name="x"> number to check instance against </param>
		/// <returns> true if instance is greater than x and neither are NaN, false otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean greaterThan(final Dfp x)
		public virtual bool greaterThan(Dfp x)
		{

			// make sure we don't mix number with different precision
			if (field.RadixDigits != x.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				dotrap(DfpField.FLAG_INVALID, GREATER_THAN_TRAP, x, result);
				return false;
			}

			/* if a nan is involved, signal invalid and return false */
			if (NaN || x.NaN)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				dotrap(DfpField.FLAG_INVALID, GREATER_THAN_TRAP, x, newInstance(Zero));
				return false;
			}

			return compare(this, x) > 0;
		}

		/// <summary>
		/// Check if instance is less than or equal to 0. </summary>
		/// <returns> true if instance is not NaN and less than or equal to 0, false otherwise </returns>
		public virtual bool negativeOrNull()
		{

			if (NaN)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, this, newInstance(Zero));
				return false;
			}

			return (sign < 0) || ((mant[mant.Length - 1] == 0) && !Infinite);

		}

		/// <summary>
		/// Check if instance is strictly less than 0. </summary>
		/// <returns> true if instance is not NaN and less than or equal to 0, false otherwise </returns>
		public virtual bool strictlyNegative()
		{

			if (NaN)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, this, newInstance(Zero));
				return false;
			}

			return (sign < 0) && ((mant[mant.Length - 1] != 0) || Infinite);

		}

		/// <summary>
		/// Check if instance is greater than or equal to 0. </summary>
		/// <returns> true if instance is not NaN and greater than or equal to 0, false otherwise </returns>
		public virtual bool positiveOrNull()
		{

			if (NaN)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, this, newInstance(Zero));
				return false;
			}

			return (sign > 0) || ((mant[mant.Length - 1] == 0) && !Infinite);

		}

		/// <summary>
		/// Check if instance is strictly greater than 0. </summary>
		/// <returns> true if instance is not NaN and greater than or equal to 0, false otherwise </returns>
		public virtual bool strictlyPositive()
		{

			if (NaN)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, this, newInstance(Zero));
				return false;
			}

			return (sign > 0) && ((mant[mant.Length - 1] != 0) || Infinite);

		}

		/// <summary>
		/// Get the absolute value of instance. </summary>
		/// <returns> absolute value of instance
		/// @since 3.2 </returns>
		public virtual Dfp abs()
		{
			Dfp result = newInstance(this);
			result.sign = 1;
			return result;
		}

		/// <summary>
		/// Check if instance is infinite. </summary>
		/// <returns> true if instance is infinite </returns>
		public virtual bool Infinite
		{
			get
			{
				return nans == INFINITE;
			}
		}

		/// <summary>
		/// Check if instance is not a number. </summary>
		/// <returns> true if instance is not a number </returns>
		public virtual bool NaN
		{
			get
			{
				return (nans == QNAN) || (nans == SNAN);
			}
		}

		/// <summary>
		/// Check if instance is equal to zero. </summary>
		/// <returns> true if instance is equal to zero </returns>
		public virtual bool Zero
		{
			get
			{
    
				if (NaN)
				{
					field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					dotrap(DfpField.FLAG_INVALID, LESS_THAN_TRAP, this, newInstance(Zero));
					return false;
				}
    
				return (mant[mant.Length - 1] == 0) && !Infinite;
    
			}
		}

		/// <summary>
		/// Check if instance is equal to x. </summary>
		/// <param name="other"> object to check instance against </param>
		/// <returns> true if instance is equal to x and neither are NaN, false otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object other)
		public override bool Equals(object other)
		{

			if (other is Dfp)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp x = (Dfp) other;
				Dfp x = (Dfp) other;
				if (NaN || x.NaN || field.RadixDigits != x.field.RadixDigits)
				{
					return false;
				}

				return compare(this, x) == 0;
			}

			return false;

		}

		/// <summary>
		/// Gets a hashCode for the instance. </summary>
		/// <returns> a hash code value for this object </returns>
		public override int GetHashCode()
		{
			return 17 + (sign << 8) + (nans << 16) + exp_Renamed + Arrays.GetHashCode(mant);
		}

		/// <summary>
		/// Check if instance is not equal to x. </summary>
		/// <param name="x"> number to check instance against </param>
		/// <returns> true if instance is not equal to x and neither are NaN, false otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean unequal(final Dfp x)
		public virtual bool unequal(Dfp x)
		{
			if (NaN || x.NaN || field.RadixDigits != x.field.RadixDigits)
			{
				return false;
			}

			return greaterThan(x) || lessThan(x);
		}

		/// <summary>
		/// Compare two instances. </summary>
		/// <param name="a"> first instance in comparison </param>
		/// <param name="b"> second instance in comparison </param>
		/// <returns> -1 if a<b, 1 if a>b and 0 if a==b
		///  Note this method does not properly handle NaNs or numbers with different precision. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int compare(final Dfp a, final Dfp b)
		private static int compare(Dfp a, Dfp b)
		{
			// Ignore the sign of zero
			if (a.mant[a.mant.Length - 1] == 0 && b.mant[b.mant.Length - 1] == 0 && a.nans == FINITE && b.nans == FINITE)
			{
				return 0;
			}

			if (a.sign != b.sign)
			{
				if (a.sign == -1)
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}

			// deal with the infinities
			if (a.nans == INFINITE && b.nans == FINITE)
			{
				return a.sign;
			}

			if (a.nans == FINITE && b.nans == INFINITE)
			{
				return -b.sign;
			}

			if (a.nans == INFINITE && b.nans == INFINITE)
			{
				return 0;
			}

			// Handle special case when a or b is zero, by ignoring the exponents
			if (b.mant[b.mant.Length - 1] != 0 && a.mant[b.mant.Length - 1] != 0)
			{
				if (a.exp_Renamed < b.exp_Renamed)
				{
					return -a.sign;
				}

				if (a.exp_Renamed > b.exp_Renamed)
				{
					return a.sign;
				}
			}

			// compare the mantissas
			for (int i = a.mant.Length - 1; i >= 0; i--)
			{
				if (a.mant[i] > b.mant[i])
				{
					return a.sign;
				}

				if (a.mant[i] < b.mant[i])
				{
					return -a.sign;
				}
			}

			return 0;

		}

		/// <summary>
		/// Round to nearest integer using the round-half-even method.
		///  That is round to nearest integer unless both are equidistant.
		///  In which case round to the even one. </summary>
		///  <returns> rounded value
		/// @since 3.2 </returns>
		public virtual Dfp rint()
		{
			return trunc(DfpField.RoundingMode.ROUND_HALF_EVEN);
		}

		/// <summary>
		/// Round to an integer using the round floor mode.
		/// That is, round toward -Infinity </summary>
		///  <returns> rounded value
		/// @since 3.2 </returns>
		public virtual Dfp floor()
		{
			return trunc(DfpField.RoundingMode.ROUND_FLOOR);
		}

		/// <summary>
		/// Round to an integer using the round ceil mode.
		/// That is, round toward +Infinity </summary>
		///  <returns> rounded value
		/// @since 3.2 </returns>
		public virtual Dfp ceil()
		{
			return trunc(DfpField.RoundingMode.ROUND_CEIL);
		}

		/// <summary>
		/// Returns the IEEE remainder. </summary>
		/// <param name="d"> divisor </param>
		/// <returns> this less n &times; d, where n is the integer closest to this/d
		/// @since 3.2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp remainder(final Dfp d)
		public virtual Dfp remainder(Dfp d)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = this.subtract(this.divide(d).rint().multiply(d));
			Dfp result = this.subtract(this.divide(d).rint().multiply(d));

			// IEEE 854-1987 says that if the result is zero, then it carries the sign of this
			if (result.mant[mant.Length - 1] == 0)
			{
				result.sign = sign;
			}

			return result;

		}

		/// <summary>
		/// Does the integer conversions with the specified rounding. </summary>
		/// <param name="rmode"> rounding mode to use </param>
		/// <returns> truncated value </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected Dfp trunc(final DfpField.RoundingMode rmode)
		protected internal virtual Dfp trunc(DfpField.RoundingMode rmode)
		{
			bool changed = false;

			if (NaN)
			{
				return newInstance(this);
			}

			if (nans == INFINITE)
			{
				return newInstance(this);
			}

			if (mant[mant.Length - 1] == 0)
			{
				// a is zero
				return newInstance(this);
			}

			/* If the exponent is less than zero then we can certainly
			 * return zero */
			if (exp_Renamed < 0)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				Dfp result = newInstance(Zero);
				result = dotrap(DfpField.FLAG_INEXACT, TRUNC_TRAP, this, result);
				return result;
			}

			/* If the exponent is greater than or equal to digits, then it
			 * must already be an integer since there is no precision left
			 * for any fractional part */

			if (exp_Renamed >= mant.Length)
			{
				return newInstance(this);
			}

			/* General case:  create another dfp, result, that contains the
			 * a with the fractional part lopped off.  */

			Dfp result = newInstance(this);
			for (int i = 0; i < mant.Length - result.exp_Renamed; i++)
			{
				changed |= result.mant[i] != 0;
				result.mant[i] = 0;
			}

			if (changed)
			{
				switch (rmode)
				{
					case org.apache.commons.math3.dfp.DfpField.RoundingMode.ROUND_FLOOR:
						if (result.sign == -1)
						{
							// then we must increment the mantissa by one
							result = result.add(newInstance(-1));
						}
						break;

					case org.apache.commons.math3.dfp.DfpField.RoundingMode.ROUND_CEIL:
						if (result.sign == 1)
						{
							// then we must increment the mantissa by one
							result = result.add(One);
						}
						break;

					case org.apache.commons.math3.dfp.DfpField.RoundingMode.ROUND_HALF_EVEN:
					default:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp half = newInstance("0.5");
						Dfp half = newInstance("0.5");
						Dfp a = subtract(result); // difference between this and result
						a.sign = 1; // force positive (take abs)
						if (a.greaterThan(half))
						{
							a = newInstance(One);
							a.sign = sign;
							result = result.add(a);
						}

						/// <summary>
						/// If exactly equal to 1/2 and odd then increment </summary>
						if (a.Equals(half) && result.exp_Renamed > 0 && (result.mant[mant.Length - result.exp_Renamed] & 1) != 0)
						{
							a = newInstance(One);
							a.sign = sign;
							result = result.add(a);
						}
						break;
				}

				field.IEEEFlagsBits = DfpField.FLAG_INEXACT; // signal inexact
				result = dotrap(DfpField.FLAG_INEXACT, TRUNC_TRAP, this, result);
				return result;
			}

			return result;
		}

		/// <summary>
		/// Convert this to an integer.
		/// If greater than 2147483647, it returns 2147483647. If less than -2147483648 it returns -2147483648. </summary>
		/// <returns> converted number </returns>
		public virtual int intValue()
		{
			Dfp rounded;
			int result = 0;

			rounded = rint();

			if (rounded.greaterThan(newInstance(unchecked((sbyte)2147483647))))
			{
				return 2147483647;
			}

			if (rounded.lessThan(newInstance((sbyte) - unchecked((int)2147483648))))
			{
				return -unchecked((int)2147483648);
			}

			for (int i = mant.Length - 1; i >= mant.Length - rounded.exp_Renamed; i--)
			{
				result = result * RADIX + rounded.mant[i];
			}

			if (rounded.sign == -1)
			{
				result = -result;
			}

			return result;
		}

		/// <summary>
		/// Get the exponent of the greatest power of 10000 that is
		///  less than or equal to the absolute value of this.  I.E.  if
		///  this is 10<sup>6</sup> then log10K would return 1. </summary>
		///  <returns> integer base 10000 logarithm </returns>
		public virtual int log10K()
		{
			return exp_Renamed - 1;
		}

		/// <summary>
		/// Get the specified  power of 10000. </summary>
		/// <param name="e"> desired power </param>
		/// <returns> 10000<sup>e</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp power10K(final int e)
		public virtual Dfp power10K(int e)
		{
			Dfp d = newInstance(One);
			d.exp_Renamed = e + 1;
			return d;
		}

		/// <summary>
		/// Get the exponent of the greatest power of 10 that is less than or equal to abs(this). </summary>
		///  <returns> integer base 10 logarithm
		/// @since 3.2 </returns>
		public virtual int intLog10()
		{
			if (mant[mant.Length - 1] > 1000)
			{
				return exp_Renamed * 4 - 1;
			}
			if (mant[mant.Length - 1] > 100)
			{
				return exp_Renamed * 4 - 2;
			}
			if (mant[mant.Length - 1] > 10)
			{
				return exp_Renamed * 4 - 3;
			}
			return exp_Renamed * 4 - 4;
		}

		/// <summary>
		/// Return the specified  power of 10. </summary>
		/// <param name="e"> desired power </param>
		/// <returns> 10<sup>e</sup> </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp power10(final int e)
		public virtual Dfp power10(int e)
		{
			Dfp d = newInstance(One);

			if (e >= 0)
			{
				d.exp_Renamed = e / 4 + 1;
			}
			else
			{
				d.exp_Renamed = (e + 1) / 4;
			}

			switch ((e % 4 + 4) % 4)
			{
				case 0:
					break;
				case 1:
					d = d.multiply(10);
					break;
				case 2:
					d = d.multiply(100);
					break;
				default:
					d = d.multiply(1000);
				break;
			}

			return d;
		}

		/// <summary>
		/// Negate the mantissa of this by computing the complement.
		///  Leaves the sign bit unchanged, used internally by add.
		///  Denormalized numbers are handled properly here. </summary>
		///  <param name="extra"> ??? </param>
		///  <returns> ??? </returns>
		protected internal virtual int complement(int extra)
		{

			extra = RADIX - extra;
			for (int i = 0; i < mant.Length; i++)
			{
				mant[i] = RADIX - mant[i] - 1;
			}

			int rh = extra / RADIX;
			extra -= rh * RADIX;
			for (int i = 0; i < mant.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = mant[i] + rh;
				int r = mant[i] + rh;
				rh = r / RADIX;
				mant[i] = r - rh * RADIX;
			}

			return extra;
		}

		/// <summary>
		/// Add x to this. </summary>
		/// <param name="x"> number to add </param>
		/// <returns> sum of this and x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp add(final Dfp x)
		public virtual Dfp add(Dfp x)
		{

			// make sure we don't mix number with different precision
			if (field.RadixDigits != x.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, ADD_TRAP, x, result);
			}

			/* handle special cases */
			if (nans != FINITE || x.nans != FINITE)
			{
				if (NaN)
				{
					return this;
				}

				if (x.NaN)
				{
					return x;
				}

				if (nans == INFINITE && x.nans == FINITE)
				{
					return this;
				}

				if (x.nans == INFINITE && nans == FINITE)
				{
					return x;
				}

				if (x.nans == INFINITE && nans == INFINITE && sign == x.sign)
				{
					return x;
				}

				if (x.nans == INFINITE && nans == INFINITE && sign != x.sign)
				{
					field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					Dfp result = newInstance(Zero);
					result.nans = QNAN;
					result = dotrap(DfpField.FLAG_INVALID, ADD_TRAP, x, result);
					return result;
				}
			}

			/* copy this and the arg */
			Dfp a = newInstance(this);
			Dfp b = newInstance(x);

			/* initialize the result object */
			Dfp result = newInstance(Zero);

			/* Make all numbers positive, but remember their sign */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte asign = a.sign;
			sbyte asign = a.sign;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte bsign = b.sign;
			sbyte bsign = b.sign;

			a.sign = 1;
			b.sign = 1;

			/* The result will be signed like the arg with greatest magnitude */
			sbyte rsign = bsign;
			if (compare(a, b) > 0)
			{
				rsign = asign;
			}

			/* Handle special case when a or b is zero, by setting the exponent
		   of the zero number equal to the other one.  This avoids an alignment
		   which would cause catastropic loss of precision */
			if (b.mant[mant.Length - 1] == 0)
			{
				b.exp_Renamed = a.exp_Renamed;
			}

			if (a.mant[mant.Length - 1] == 0)
			{
				a.exp_Renamed = b.exp_Renamed;
			}

			/* align number with the smaller exponent */
			int aextradigit = 0;
			int bextradigit = 0;
			if (a.exp_Renamed < b.exp_Renamed)
			{
				aextradigit = a.align(b.exp_Renamed);
			}
			else
			{
				bextradigit = b.align(a.exp_Renamed);
			}

			/* complement the smaller of the two if the signs are different */
			if (asign != bsign)
			{
				if (asign == rsign)
				{
					bextradigit = b.complement(bextradigit);
				}
				else
				{
					aextradigit = a.complement(aextradigit);
				}
			}

			/* add the mantissas */
			int rh = 0; // acts as a carry
			for (int i = 0; i < mant.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = a.mant[i]+b.mant[i]+rh;
				int r = a.mant[i] + b.mant[i] + rh;
				rh = r / RADIX;
				result.mant[i] = r - rh * RADIX;
			}
			result.exp_Renamed = a.exp_Renamed;
			result.sign = rsign;

			/* handle overflow -- note, when asign!=bsign an overflow is
			 * normal and should be ignored.  */

			if (rh != 0 && (asign == bsign))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lostdigit = result.mant[0];
				int lostdigit = result.mant[0];
				result.shiftRight();
				result.mant[mant.Length - 1] = rh;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int excp = result.round(lostdigit);
				int excp = result.round(lostdigit);
				if (excp != 0)
				{
					result = dotrap(excp, ADD_TRAP, x, result);
				}
			}

			/* normalize the result */
			for (int i = 0; i < mant.Length; i++)
			{
				if (result.mant[mant.Length - 1] != 0)
				{
					break;
				}
				result.shiftLeft();
				if (i == 0)
				{
					result.mant[0] = aextradigit + bextradigit;
					aextradigit = 0;
					bextradigit = 0;
				}
			}

			/* result is zero if after normalization the most sig. digit is zero */
			if (result.mant[mant.Length - 1] == 0)
			{
				result.exp_Renamed = 0;

				if (asign != bsign)
				{
					// Unless adding 2 negative zeros, sign is positive
					result.sign = 1; // Per IEEE 854-1987 Section 6.3
				}
			}

			/* Call round to test for over/under flows */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int excp = result.round(aextradigit + bextradigit);
			int excp = result.round(aextradigit + bextradigit);
			if (excp != 0)
			{
				result = dotrap(excp, ADD_TRAP, x, result);
			}

			return result;
		}

		/// <summary>
		/// Returns a number that is this number with the sign bit reversed. </summary>
		/// <returns> the opposite of this </returns>
		public virtual Dfp negate()
		{
			Dfp result = newInstance(this);
			result.sign = (sbyte) - result.sign;
			return result;
		}

		/// <summary>
		/// Subtract x from this. </summary>
		/// <param name="x"> number to subtract </param>
		/// <returns> difference of this and a </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp subtract(final Dfp x)
		public virtual Dfp subtract(Dfp x)
		{
			return add(x.negate());
		}

		/// <summary>
		/// Round this given the next digit n using the current rounding mode. </summary>
		/// <param name="n"> ??? </param>
		/// <returns> the IEEE flag if an exception occurred </returns>
		protected internal virtual int round(int n)
		{
			bool inc = false;
			switch (field.RoundingMode)
			{
				case ROUND_DOWN:
					inc = false;
					break;

				case ROUND_UP:
					inc = n != 0; // round up if n!=0
					break;

				case ROUND_HALF_UP:
					inc = n >= 5000; // round half up
					break;

				case ROUND_HALF_DOWN:
					inc = n > 5000; // round half down
					break;

				case ROUND_HALF_EVEN:
					inc = n > 5000 || (n == 5000 && (mant[0] & 1) == 1); // round half-even
					break;

				case ROUND_HALF_ODD:
					inc = n > 5000 || (n == 5000 && (mant[0] & 1) == 0); // round half-odd
					break;

				case ROUND_CEIL:
					inc = sign == 1 && n != 0; // round ceil
					break;

				case ROUND_FLOOR:
				default:
					inc = sign == -1 && n != 0; // round floor
					break;
			}

			if (inc)
			{
				// increment if necessary
				int rh = 1;
				for (int i = 0; i < mant.Length; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = mant[i] + rh;
					int r = mant[i] + rh;
					rh = r / RADIX;
					mant[i] = r - rh * RADIX;
				}

				if (rh != 0)
				{
					shiftRight();
					mant[mant.Length - 1] = rh;
				}
			}

			// check for exceptional cases and raise signals if necessary
			if (exp_Renamed < MIN_EXP)
			{
				// Gradual Underflow
				field.IEEEFlagsBits = DfpField.FLAG_UNDERFLOW;
				return DfpField.FLAG_UNDERFLOW;
			}

			if (exp_Renamed > MAX_EXP)
			{
				// Overflow
				field.IEEEFlagsBits = DfpField.FLAG_OVERFLOW;
				return DfpField.FLAG_OVERFLOW;
			}

			if (n != 0)
			{
				// Inexact
				field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				return DfpField.FLAG_INEXACT;
			}

			return 0;

		}

		/// <summary>
		/// Multiply this by x. </summary>
		/// <param name="x"> multiplicand </param>
		/// <returns> product of this and x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp multiply(final Dfp x)
		public virtual Dfp multiply(Dfp x)
		{

			// make sure we don't mix number with different precision
			if (field.RadixDigits != x.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, MULTIPLY_TRAP, x, result);
			}

			Dfp result = newInstance(Zero);

			/* handle special cases */
			if (nans != FINITE || x.nans != FINITE)
			{
				if (NaN)
				{
					return this;
				}

				if (x.NaN)
				{
					return x;
				}

				if (nans == INFINITE && x.nans == FINITE && x.mant[mant.Length - 1] != 0)
				{
					result = newInstance(this);
					result.sign = (sbyte)(sign * x.sign);
					return result;
				}

				if (x.nans == INFINITE && nans == FINITE && mant[mant.Length - 1] != 0)
				{
					result = newInstance(x);
					result.sign = (sbyte)(sign * x.sign);
					return result;
				}

				if (x.nans == INFINITE && nans == INFINITE)
				{
					result = newInstance(this);
					result.sign = (sbyte)(sign * x.sign);
					return result;
				}

				if ((x.nans == INFINITE && nans == FINITE && mant[mant.Length - 1] == 0) || (nans == INFINITE && x.nans == FINITE && x.mant[mant.Length - 1] == 0))
				{
					field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					result = newInstance(Zero);
					result.nans = QNAN;
					result = dotrap(DfpField.FLAG_INVALID, MULTIPLY_TRAP, x, result);
					return result;
				}
			}

			int[] product = new int[mant.Length * 2]; // Big enough to hold even the largest result

			for (int i = 0; i < mant.Length; i++)
			{
				int rh = 0; // acts as a carry
				for (int j = 0; j < mant.Length; j++)
				{
					int r = mant[i] * x.mant[j]; // multiply the 2 digits
					r += product[i + j] + rh; // add to the product digit with carry in

					rh = r / RADIX;
					product[i + j] = r - rh * RADIX;
				}
				product[i + mant.Length] = rh;
			}

			// Find the most sig digit
			int md = mant.Length * 2 - 1; // default, in case result is zero
			for (int i = mant.Length * 2 - 1; i >= 0; i--)
			{
				if (product[i] != 0)
				{
					md = i;
					break;
				}
			}

			// Copy the digits into the result
			for (int i = 0; i < mant.Length; i++)
			{
				result.mant[mant.Length - i - 1] = product[md - i];
			}

			// Fixup the exponent.
			result.exp_Renamed = exp_Renamed + x.exp_Renamed + md - 2 * mant.Length + 1;
			result.sign = (sbyte)((sign == x.sign)?1:-1);

			if (result.mant[mant.Length - 1] == 0)
			{
				// if result is zero, set exp to zero
				result.exp_Renamed = 0;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int excp;
			int excp;
			if (md > (mant.Length - 1))
			{
				excp = result.round(product[md - mant.Length]);
			}
			else
			{
				excp = result.round(0); // has no effect except to check status
			}

			if (excp != 0)
			{
				result = dotrap(excp, MULTIPLY_TRAP, x, result);
			}

			return result;

		}

		/// <summary>
		/// Multiply this by a single digit x. </summary>
		/// <param name="x"> multiplicand </param>
		/// <returns> product of this and x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp multiply(final int x)
		public virtual Dfp multiply(int x)
		{
			if (x >= 0 && x < RADIX)
			{
				return multiplyFast(x);
			}
			else
			{
				return multiply(newInstance(x));
			}
		}

		/// <summary>
		/// Multiply this by a single digit 0&lt;=x&lt;radix.
		/// There are speed advantages in this special case. </summary>
		/// <param name="x"> multiplicand </param>
		/// <returns> product of this and x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Dfp multiplyFast(final int x)
		private Dfp multiplyFast(int x)
		{
			Dfp result = newInstance(this);

			/* handle special cases */
			if (nans != FINITE)
			{
				if (NaN)
				{
					return this;
				}

				if (nans == INFINITE && x != 0)
				{
					result = newInstance(this);
					return result;
				}

				if (nans == INFINITE && x == 0)
				{
					field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					result = newInstance(Zero);
					result.nans = QNAN;
					result = dotrap(DfpField.FLAG_INVALID, MULTIPLY_TRAP, newInstance(Zero), result);
					return result;
				}
			}

			/* range check x */
			if (x < 0 || x >= RADIX)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				result = newInstance(Zero);
				result.nans = QNAN;
				result = dotrap(DfpField.FLAG_INVALID, MULTIPLY_TRAP, result, result);
				return result;
			}

			int rh = 0;
			for (int i = 0; i < mant.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = mant[i] * x + rh;
				int r = mant[i] * x + rh;
				rh = r / RADIX;
				result.mant[i] = r - rh * RADIX;
			}

			int lostdigit = 0;
			if (rh != 0)
			{
				lostdigit = result.mant[0];
				result.shiftRight();
				result.mant[mant.Length - 1] = rh;
			}

			if (result.mant[mant.Length - 1] == 0) // if result is zero, set exp to zero
			{
				result.exp_Renamed = 0;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int excp = result.round(lostdigit);
			int excp = result.round(lostdigit);
			if (excp != 0)
			{
				result = dotrap(excp, MULTIPLY_TRAP, result, result);
			}

			return result;
		}

		/// <summary>
		/// Divide this by divisor. </summary>
		/// <param name="divisor"> divisor </param>
		/// <returns> quotient of this by divisor </returns>
		public virtual Dfp divide(Dfp divisor)
		{
			int[] dividend; // current status of the dividend
			int[] quotient; // quotient
			int[] remainder; // remainder
			int qd; // current quotient digit we're working with
			int nsqd; // number of significant quotient digits we have
			int trial = 0; // trial quotient digit
			int minadj; // minimum adjustment
			bool trialgood; // Flag to indicate a good trail digit
			int md = 0; // most sig digit in result
			int excp; // exceptions

			// make sure we don't mix number with different precision
			if (field.RadixDigits != divisor.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, DIVIDE_TRAP, divisor, result);
			}

			Dfp result = newInstance(Zero);

			/* handle special cases */
			if (nans != FINITE || divisor.nans != FINITE)
			{
				if (NaN)
				{
					return this;
				}

				if (divisor.NaN)
				{
					return divisor;
				}

				if (nans == INFINITE && divisor.nans == FINITE)
				{
					result = newInstance(this);
					result.sign = (sbyte)(sign * divisor.sign);
					return result;
				}

				if (divisor.nans == INFINITE && nans == FINITE)
				{
					result = newInstance(Zero);
					result.sign = (sbyte)(sign * divisor.sign);
					return result;
				}

				if (divisor.nans == INFINITE && nans == INFINITE)
				{
					field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					result = newInstance(Zero);
					result.nans = QNAN;
					result = dotrap(DfpField.FLAG_INVALID, DIVIDE_TRAP, divisor, result);
					return result;
				}
			}

			/* Test for divide by zero */
			if (divisor.mant[mant.Length - 1] == 0)
			{
				field.IEEEFlagsBits = DfpField.FLAG_DIV_ZERO;
				result = newInstance(Zero);
				result.sign = (sbyte)(sign * divisor.sign);
				result.nans = INFINITE;
				result = dotrap(DfpField.FLAG_DIV_ZERO, DIVIDE_TRAP, divisor, result);
				return result;
			}

			dividend = new int[mant.Length + 1]; // one extra digit needed
			quotient = new int[mant.Length + 2]; // two extra digits needed 1 for overflow, 1 for rounding
			remainder = new int[mant.Length + 1]; // one extra digit needed

			/* Initialize our most significant digits to zero */

			dividend[mant.Length] = 0;
			quotient[mant.Length] = 0;
			quotient[mant.Length + 1] = 0;
			remainder[mant.Length] = 0;

			/* copy our mantissa into the dividend, initialize the
		   quotient while we are at it */

			for (int i = 0; i < mant.Length; i++)
			{
				dividend[i] = mant[i];
				quotient[i] = 0;
				remainder[i] = 0;
			}

			/* outer loop.  Once per quotient digit */
			nsqd = 0;
			for (qd = mant.Length + 1; qd >= 0; qd--)
			{
				/* Determine outer limits of our quotient digit */

				// r =  most sig 2 digits of dividend
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int divMsb = dividend[mant.length]*RADIX+dividend[mant.length-1];
				int divMsb = dividend[mant.Length] * RADIX + dividend[mant.Length - 1];
				int min = divMsb / (divisor.mant[mant.Length - 1] + 1);
				int max = (divMsb + 1) / divisor.mant[mant.Length - 1];

				trialgood = false;
				while (!trialgood)
				{
					// try the mean
					trial = (min + max) / 2;

					/* Multiply by divisor and store as remainder */
					int rh = 0;
					for (int i = 0; i < mant.Length + 1; i++)
					{
						int dm = (i < mant.Length)?divisor.mant[i]:0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = (dm * trial) + rh;
						int r = (dm * trial) + rh;
						rh = r / RADIX;
						remainder[i] = r - rh * RADIX;
					}

					/* subtract the remainder from the dividend */
					rh = 1; // carry in to aid the subtraction
					for (int i = 0; i < mant.Length + 1; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = ((RADIX-1) - remainder[i]) + dividend[i] + rh;
						int r = ((RADIX - 1) - remainder[i]) + dividend[i] + rh;
						rh = r / RADIX;
						remainder[i] = r - rh * RADIX;
					}

					/* Lets analyze what we have here */
					if (rh == 0)
					{
						// trial is too big -- negative remainder
						max = trial - 1;
						continue;
					}

					/* find out how far off the remainder is telling us we are */
					minadj = (remainder[mant.Length] * RADIX) + remainder[mant.Length - 1];
					minadj /= divisor.mant[mant.Length - 1] + 1;

					if (minadj >= 2)
					{
						min = trial + minadj; // update the minimum
						continue;
					}

					/* May have a good one here, check more thoroughly.  Basically
			   its a good one if it is less than the divisor */
					trialgood = false; // assume false
					for (int i = mant.Length - 1; i >= 0; i--)
					{
						if (divisor.mant[i] > remainder[i])
						{
							trialgood = true;
						}
						if (divisor.mant[i] < remainder[i])
						{
							break;
						}
					}

					if (remainder[mant.Length] != 0)
					{
						trialgood = false;
					}

					if (trialgood == false)
					{
						min = trial + 1;
					}
				}

				/* Great we have a digit! */
				quotient[qd] = trial;
				if (trial != 0 || nsqd != 0)
				{
					nsqd++;
				}

				if (field.RoundingMode == DfpField.RoundingMode.ROUND_DOWN && nsqd == mant.Length)
				{
					// We have enough for this mode
					break;
				}

				if (nsqd > mant.Length)
				{
					// We have enough digits
					break;
				}

				/* move the remainder into the dividend while left shifting */
				dividend[0] = 0;
				for (int i = 0; i < mant.Length; i++)
				{
					dividend[i + 1] = remainder[i];
				}
			}

			/* Find the most sig digit */
			md = mant.Length; // default
			for (int i = mant.Length + 1; i >= 0; i--)
			{
				if (quotient[i] != 0)
				{
					md = i;
					break;
				}
			}

			/* Copy the digits into the result */
			for (int i = 0; i < mant.Length; i++)
			{
				result.mant[mant.Length - i - 1] = quotient[md - i];
			}

			/* Fixup the exponent. */
			result.exp_Renamed = exp_Renamed - divisor.exp_Renamed + md - mant.Length;
			result.sign = (sbyte)((sign == divisor.sign) ? 1 : -1);

			if (result.mant[mant.Length - 1] == 0) // if result is zero, set exp to zero
			{
				result.exp_Renamed = 0;
			}

			if (md > (mant.Length - 1))
			{
				excp = result.round(quotient[md - mant.Length]);
			}
			else
			{
				excp = result.round(0);
			}

			if (excp != 0)
			{
				result = dotrap(excp, DIVIDE_TRAP, divisor, result);
			}

			return result;
		}

		/// <summary>
		/// Divide by a single digit less than radix.
		///  Special case, so there are speed advantages. 0 &lt;= divisor &lt; radix </summary>
		/// <param name="divisor"> divisor </param>
		/// <returns> quotient of this by divisor </returns>
		public virtual Dfp divide(int divisor)
		{

			// Handle special cases
			if (nans != FINITE)
			{
				if (NaN)
				{
					return this;
				}

				if (nans == INFINITE)
				{
					return newInstance(this);
				}
			}

			// Test for divide by zero
			if (divisor == 0)
			{
				field.IEEEFlagsBits = DfpField.FLAG_DIV_ZERO;
				Dfp result = newInstance(Zero);
				result.sign = sign;
				result.nans = INFINITE;
				result = dotrap(DfpField.FLAG_DIV_ZERO, DIVIDE_TRAP, Zero, result);
				return result;
			}

			// range check divisor
			if (divisor < 0 || divisor >= RADIX)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				result = dotrap(DfpField.FLAG_INVALID, DIVIDE_TRAP, result, result);
				return result;
			}

			Dfp result = newInstance(this);

			int rl = 0;
			for (int i = mant.Length - 1; i >= 0; i--)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = rl*RADIX + result.mant[i];
				int r = rl * RADIX + result.mant[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rh = r / divisor;
				int rh = r / divisor;
				rl = r - rh * divisor;
				result.mant[i] = rh;
			}

			if (result.mant[mant.Length - 1] == 0)
			{
				// normalize
				result.shiftLeft();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = rl * RADIX;
				int r = rl * RADIX; // compute the next digit and put it in
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rh = r / divisor;
				int rh = r / divisor;
				rl = r - rh * divisor;
				result.mant[0] = rh;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int excp = result.round(rl * RADIX / divisor);
			int excp = result.round(rl * RADIX / divisor); // do the rounding
			if (excp != 0)
			{
				result = dotrap(excp, DIVIDE_TRAP, result, result);
			}

			return result;

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Dfp reciprocal()
		{
			return field.One.divide(this);
		}

		/// <summary>
		/// Compute the square root. </summary>
		/// <returns> square root of the instance
		/// @since 3.2 </returns>
		public virtual Dfp sqrt()
		{

			// check for unusual cases
			if (nans == FINITE && mant[mant.Length - 1] == 0)
			{
				// if zero
				return newInstance(this);
			}

			if (nans != FINITE)
			{
				if (nans == INFINITE && sign == 1)
				{
					// if positive infinity
					return newInstance(this);
				}

				if (nans == QNAN)
				{
					return newInstance(this);
				}

				if (nans == SNAN)
				{
					Dfp result;

					field.IEEEFlagsBits = DfpField.FLAG_INVALID;
					result = newInstance(this);
					result = dotrap(DfpField.FLAG_INVALID, SQRT_TRAP, null, result);
					return result;
				}
			}

			if (sign == -1)
			{
				// if negative
				Dfp result;

				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
				result = newInstance(this);
				result.nans = QNAN;
				result = dotrap(DfpField.FLAG_INVALID, SQRT_TRAP, null, result);
				return result;
			}

			Dfp x = newInstance(this);

			/* Lets make a reasonable guess as to the size of the square root */
			if (x.exp_Renamed < -1 || x.exp_Renamed > 1)
			{
				x.exp_Renamed = this.exp_Renamed / 2;
			}

			/* Coarsely estimate the mantissa */
			switch (x.mant[mant.Length - 1] / 2000)
			{
				case 0:
					x.mant[mant.Length - 1] = x.mant[mant.Length - 1] / 2 + 1;
					break;
				case 2:
					x.mant[mant.Length - 1] = 1500;
					break;
				case 3:
					x.mant[mant.Length - 1] = 2200;
					break;
				default:
					x.mant[mant.Length - 1] = 3000;
				break;
			}

			Dfp dx = newInstance(x);

			/* Now that we have the first pass estimate, compute the rest
		   by the formula dx = (y - x*x) / (2x); */

			Dfp px = Zero;
			Dfp ppx = Zero;
			while (x.unequal(px))
			{
				dx = newInstance(x);
				dx.sign = -1;
				dx = dx.add(this.divide(x));
				dx = dx.divide(2);
				ppx = px;
				px = x;
				x = x.add(dx);

				if (x.Equals(ppx))
				{
					// alternating between two values
					break;
				}

				// if dx is zero, break.  Note testing the most sig digit
				// is a sufficient test since dx is normalized
				if (dx.mant[mant.Length - 1] == 0)
				{
					break;
				}
			}

			return x;

		}

		/// <summary>
		/// Get a string representation of the instance. </summary>
		/// <returns> string representation of the instance </returns>
		public override string ToString()
		{
			if (nans != FINITE)
			{
				// if non-finite exceptional cases
				if (nans == INFINITE)
				{
					return (sign < 0) ? NEG_INFINITY_STRING : POS_INFINITY_STRING;
				}
				else
				{
					return NAN_STRING;
				}
			}

			if (exp_Renamed > mant.Length || exp_Renamed < -1)
			{
				return dfp2sci();
			}

			return dfp2string();

		}

		/// <summary>
		/// Convert an instance to a string using scientific notation. </summary>
		/// <returns> string representation of the instance in scientific notation </returns>
		protected internal virtual string dfp2sci()
		{
			char[] rawdigits = new char[mant.Length * 4];
			char[] outputbuffer = new char[mant.Length * 4 + 20];
			int p;
			int q;
			int e;
			int ae;
			int shf;

			// Get all the digits
			p = 0;
			for (int i = mant.Length - 1; i >= 0; i--)
			{
				rawdigits[p++] = (char)((mant[i] / 1000) + '0');
				rawdigits[p++] = (char)(((mant[i] / 100) % 10) + '0');
				rawdigits[p++] = (char)(((mant[i] / 10) % 10) + '0');
				rawdigits[p++] = (char)(((mant[i]) % 10) + '0');
			}

			// Find the first non-zero one
			for (p = 0; p < rawdigits.Length; p++)
			{
				if (rawdigits[p] != '0')
				{
					break;
				}
			}
			shf = p;

			// Now do the conversion
			q = 0;
			if (sign == -1)
			{
				outputbuffer[q++] = '-';
			}

			if (p != rawdigits.Length)
			{
				// there are non zero digits...
				outputbuffer[q++] = rawdigits[p++];
				outputbuffer[q++] = '.';

				while (p < rawdigits.Length)
				{
					outputbuffer[q++] = rawdigits[p++];
				}
			}
			else
			{
				outputbuffer[q++] = '0';
				outputbuffer[q++] = '.';
				outputbuffer[q++] = '0';
				outputbuffer[q++] = 'e';
				outputbuffer[q++] = '0';
				return new string(outputbuffer, 0, 5);
			}

			outputbuffer[q++] = 'e';

			// Find the msd of the exponent

			e = exp_Renamed * 4 - shf - 1;
			ae = e;
			if (e < 0)
			{
				ae = -e;
			}

			// Find the largest p such that p < e
			for (p = 1000000000; p > ae; p /= 10)
			{
				// nothing to do
			}

			if (e < 0)
			{
				outputbuffer[q++] = '-';
			}

			while (p > 0)
			{
				outputbuffer[q++] = (char)(ae / p + '0');
				ae %= p;
				p /= 10;
			}

			return new string(outputbuffer, 0, q);

		}

		/// <summary>
		/// Convert an instance to a string using normal notation. </summary>
		/// <returns> string representation of the instance in normal notation </returns>
		protected internal virtual string dfp2string()
		{
			char[] buffer = new char[mant.Length * 4 + 20];
			int p = 1;
			int q;
			int e = exp_Renamed;
			bool pointInserted = false;

			buffer[0] = ' ';

			if (e <= 0)
			{
				buffer[p++] = '0';
				buffer[p++] = '.';
				pointInserted = true;
			}

			while (e < 0)
			{
				buffer[p++] = '0';
				buffer[p++] = '0';
				buffer[p++] = '0';
				buffer[p++] = '0';
				e++;
			}

			for (int i = mant.Length - 1; i >= 0; i--)
			{
				buffer[p++] = (char)((mant[i] / 1000) + '0');
				buffer[p++] = (char)(((mant[i] / 100) % 10) + '0');
				buffer[p++] = (char)(((mant[i] / 10) % 10) + '0');
				buffer[p++] = (char)(((mant[i]) % 10) + '0');
				if (--e == 0)
				{
					buffer[p++] = '.';
					pointInserted = true;
				}
			}

			while (e > 0)
			{
				buffer[p++] = '0';
				buffer[p++] = '0';
				buffer[p++] = '0';
				buffer[p++] = '0';
				e--;
			}

			if (!pointInserted)
			{
				// Ensure we have a radix point!
				buffer[p++] = '.';
			}

			// Suppress leading zeros
			q = 1;
			while (buffer[q] == '0')
			{
				q++;
			}
			if (buffer[q] == '.')
			{
				q--;
			}

			// Suppress trailing zeros
			while (buffer[p - 1] == '0')
			{
				p--;
			}

			// Insert sign
			if (sign < 0)
			{
				buffer[--q] = '-';
			}

			return new string(buffer, q, p - q);

		}

		/// <summary>
		/// Raises a trap.  This does not set the corresponding flag however. </summary>
		///  <param name="type"> the trap type </param>
		///  <param name="what"> - name of routine trap occurred in </param>
		///  <param name="oper"> - input operator to function </param>
		///  <param name="result"> - the result computed prior to the trap </param>
		///  <returns> The suggested return value from the trap handler </returns>
		public virtual Dfp dotrap(int type, string what, Dfp oper, Dfp result)
		{
			Dfp def = result;

			switch (type)
			{
				case DfpField.FLAG_INVALID:
					def = newInstance(Zero);
					def.sign = result.sign;
					def.nans = QNAN;
					break;

				case DfpField.FLAG_DIV_ZERO:
					if (nans == FINITE && mant[mant.Length - 1] != 0)
					{
						// normal case, we are finite, non-zero
						def = newInstance(Zero);
						def.sign = (sbyte)(sign * oper.sign);
						def.nans = INFINITE;
					}

					if (nans == FINITE && mant[mant.Length - 1] == 0)
					{
						//  0/0
						def = newInstance(Zero);
						def.nans = QNAN;
					}

					if (nans == INFINITE || nans == QNAN)
					{
						def = newInstance(Zero);
						def.nans = QNAN;
					}

					if (nans == INFINITE || nans == SNAN)
					{
						def = newInstance(Zero);
						def.nans = QNAN;
					}
					break;

				case DfpField.FLAG_UNDERFLOW:
					if ((result.exp_Renamed + mant.Length) < MIN_EXP)
					{
						def = newInstance(Zero);
						def.sign = result.sign;
					}
					else
					{
						def = newInstance(result); // gradual underflow
					}
					result.exp_Renamed += ERR_SCALE;
					break;

				case DfpField.FLAG_OVERFLOW:
					result.exp_Renamed -= ERR_SCALE;
					def = newInstance(Zero);
					def.sign = result.sign;
					def.nans = INFINITE;
					break;

				default:
					def = result;
					break;
			}

			return trap(type, what, oper, def, result);

		}

		/// <summary>
		/// Trap handler.  Subclasses may override this to provide trap
		///  functionality per IEEE 854-1987.
		/// </summary>
		///  <param name="type">  The exception type - e.g. FLAG_OVERFLOW </param>
		///  <param name="what">  The name of the routine we were in e.g. divide() </param>
		///  <param name="oper">  An operand to this function if any </param>
		///  <param name="def">   The default return value if trap not enabled </param>
		///  <param name="result">    The result that is specified to be delivered per
		///                   IEEE 854, if any </param>
		///  <returns> the value that should be return by the operation triggering the trap </returns>
		protected internal virtual Dfp trap(int type, string what, Dfp oper, Dfp def, Dfp result)
		{
			return def;
		}

		/// <summary>
		/// Returns the type - one of FINITE, INFINITE, SNAN, QNAN. </summary>
		/// <returns> type of the number </returns>
		public virtual int classify()
		{
			return nans;
		}

		/// <summary>
		/// Creates an instance that is the same as x except that it has the sign of y.
		/// abs(x) = dfp.copysign(x, dfp.one) </summary>
		/// <param name="x"> number to get the value from </param>
		/// <param name="y"> number to get the sign from </param>
		/// <returns> a number with the value of x and the sign of y </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp copysign(final Dfp x, final Dfp y)
		public static Dfp copysign(Dfp x, Dfp y)
		{
			Dfp result = x.newInstance(x);
			result.sign = y.sign;
			return result;
		}

		/// <summary>
		/// Returns the next number greater than this one in the direction of x.
		/// If this==x then simply returns this. </summary>
		/// <param name="x"> direction where to look at </param>
		/// <returns> closest number next to instance in the direction of x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp nextAfter(final Dfp x)
		public virtual Dfp nextAfter(Dfp x)
		{

			// make sure we don't mix number with different precision
			if (field.RadixDigits != x.field.RadixDigits)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, NEXT_AFTER_TRAP, x, result);
			}

			// if this is greater than x
			bool up = false;
			if (this.lessThan(x))
			{
				up = true;
			}

			if (compare(this, x) == 0)
			{
				return newInstance(x);
			}

			if (lessThan(Zero))
			{
				up = !up;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp inc;
			Dfp inc;
			Dfp result;
			if (up)
			{
				inc = newInstance(One);
				inc.exp_Renamed = this.exp_Renamed - mant.Length + 1;
				inc.sign = this.sign;

				if (this.Equals(Zero))
				{
					inc.exp_Renamed = MIN_EXP - mant.Length;
				}

				result = add(inc);
			}
			else
			{
				inc = newInstance(One);
				inc.exp_Renamed = this.exp_Renamed;
				inc.sign = this.sign;

				if (this.Equals(inc))
				{
					inc.exp_Renamed = this.exp_Renamed - mant.Length;
				}
				else
				{
					inc.exp_Renamed = this.exp_Renamed - mant.Length + 1;
				}

				if (this.Equals(Zero))
				{
					inc.exp_Renamed = MIN_EXP - mant.Length;
				}

				result = this.subtract(inc);
			}

			if (result.classify() == INFINITE && this.classify() != INFINITE)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				result = dotrap(DfpField.FLAG_INEXACT, NEXT_AFTER_TRAP, x, result);
			}

			if (result.Equals(Zero) && this.Equals(Zero) == false)
			{
				field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				result = dotrap(DfpField.FLAG_INEXACT, NEXT_AFTER_TRAP, x, result);
			}

			return result;

		}

		/// <summary>
		/// Convert the instance into a double. </summary>
		/// <returns> a double approximating the instance </returns>
		/// <seealso cref= #toSplitDouble() </seealso>
		public virtual double toDouble()
		{

			if (Infinite)
			{
				if (lessThan(Zero))
				{
					return double.NegativeInfinity;
				}
				else
				{
					return double.PositiveInfinity;
				}
			}

			if (NaN)
			{
				return double.NaN;
			}

			Dfp y = this;
			bool negate = false;
			int cmp0 = compare(this, Zero);
			if (cmp0 == 0)
			{
				return sign < 0 ? - 0.0 : +0.0;
			}
			else if (cmp0 < 0)
			{
				y = negate();
				negate = true;
			}

			/* Find the exponent, first estimate by integer log10, then adjust.
			 Should be faster than doing a natural logarithm.  */
			int exponent = (int)(y.intLog10() * 3.32);
			if (exponent < 0)
			{
				exponent--;
			}

			Dfp tempDfp = DfpMath.pow(Two, exponent);
			while (tempDfp.lessThan(y) || tempDfp.Equals(y))
			{
				tempDfp = tempDfp.multiply(2);
				exponent++;
			}
			exponent--;

			/* We have the exponent, now work on the mantissa */

			y = y.divide(DfpMath.pow(Two, exponent));
			if (exponent > -1023)
			{
				y = y.subtract(One);
			}

			if (exponent < -1074)
			{
				return 0;
			}

			if (exponent > 1023)
			{
				return negate ? double.NegativeInfinity : double.PositiveInfinity;
			}


			y = y.multiply(newInstance(unchecked((sbyte)4503599627370496l))).rint();
			string str = y.ToString();
			str = str.Substring(0, str.Length - 1);
			long mantissa = Convert.ToInt64(str);

			if (mantissa == 4503599627370496L)
			{
				// Handle special case where we round up to next power of two
				mantissa = 0;
				exponent++;
			}

			/* Its going to be subnormal, so make adjustments */
			if (exponent <= -1023)
			{
				exponent--;
			}

			while (exponent < -1023)
			{
				exponent++;
				mantissa = (long)((ulong)mantissa >> 1);
			}

			long bits = mantissa | ((exponent + 1023L) << 52);
			double x = double.longBitsToDouble(bits);

			if (negate)
			{
				x = -x;
			}

			return x;

		}

		/// <summary>
		/// Convert the instance into a split double. </summary>
		/// <returns> an array of two doubles which sum represent the instance </returns>
		/// <seealso cref= #toDouble() </seealso>
		public virtual double[] toSplitDouble()
		{
			double[] split = new double[2];
			long mask = 0xffffffffc0000000L;

			split[0] = double.longBitsToDouble(double.doubleToLongBits(toDouble()) & mask);
			split[1] = subtract(newInstance(split[0])).toDouble();

			return split;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual double Real
		{
			get
			{
				return toDouble();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp add(final double a)
		public virtual Dfp add(double a)
		{
			return add(newInstance(a));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp subtract(final double a)
		public virtual Dfp subtract(double a)
		{
			return subtract(newInstance(a));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp multiply(final double a)
		public virtual Dfp multiply(double a)
		{
			return multiply(newInstance(a));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp divide(final double a)
		public virtual Dfp divide(double a)
		{
			return divide(newInstance(a));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp remainder(final double a)
		public virtual Dfp remainder(double a)
		{
			return remainder(newInstance(a));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual long round()
		{
			return FastMath.round(toDouble());
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp signum()
		{
			if (NaN || Zero)
			{
				return this;
			}
			else
			{
				return newInstance(sign > 0 ? + 1 : -1);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp copySign(final Dfp s)
		public virtual Dfp copySign(Dfp s)
		{
			if ((sign >= 0 && s.sign >= 0) || (sign < 0 && s.sign < 0)) // Sign is currently OK
			{
				return this;
			}
			return negate(); // flip sign
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp copySign(final double s)
		public virtual Dfp copySign(double s)
		{
			long sb = double.doubleToLongBits(s);
			if ((sign >= 0 && sb >= 0) || (sign < 0 && sb < 0)) // Sign is currently OK
			{
				return this;
			}
			return negate(); // flip sign
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp scalb(final int n)
		public virtual Dfp scalb(int n)
		{
			return multiply(DfpMath.pow(Two, n));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp hypot(final Dfp y)
		public virtual Dfp hypot(Dfp y)
		{
			return multiply(this).add(y.multiply(y)).sqrt();
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp cbrt()
		{
			return rootN(3);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp rootN(final int n)
		public virtual Dfp rootN(int n)
		{
			return (sign >= 0) ? DfpMath.pow(this, One.divide(n)) : DfpMath.pow(negate(), One.divide(n)).negate();
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp pow(final double p)
		public virtual Dfp pow(double p)
		{
			return DfpMath.pow(this, newInstance(p));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp pow(final int n)
		public virtual Dfp pow(int n)
		{
			return DfpMath.pow(this, n);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp pow(final Dfp e)
		public virtual Dfp pow(Dfp e)
		{
			return DfpMath.pow(this, e);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp exp()
		{
			return DfpMath.exp(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp expm1()
		{
			return DfpMath.exp(this).subtract(One);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp log()
		{
			return DfpMath.log(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp log1p()
		{
			return DfpMath.log(this.add(One));
		}

	//  TODO: deactivate this implementation (and return type) in 4.0
		/// <summary>
		/// Get the exponent of the greatest power of 10 that is less than or equal to abs(this). </summary>
		///  <returns> integer base 10 logarithm </returns>
		///  @deprecated as of 3.2, replaced by <seealso cref="#intLog10()"/>, in 4.0 the return type
		///  will be changed to Dfp 
		[Obsolete("as of 3.2, replaced by <seealso cref="#intLog10()"/>, in 4.0 the return type")]
		public virtual int log10()
		{
			return intLog10();
		}

	//    TODO: activate this implementation (and return type) in 4.0
	//    /** {@inheritDoc}
	//     * @since 3.2
	//     */
	//    public Dfp log10() {
	//        return DfpMath.log(this).divide(DfpMath.log(newInstance(10)));
	//    }

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp cos()
		{
			return DfpMath.cos(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp sin()
		{
			return DfpMath.sin(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp tan()
		{
			return DfpMath.tan(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp acos()
		{
			return DfpMath.acos(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp asin()
		{
			return DfpMath.asin(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp atan()
		{
			return DfpMath.atan(this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dfp atan2(final Dfp x) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Dfp atan2(Dfp x)
		{

			// compute r = sqrt(x^2+y^2)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp r = x.multiply(x).add(multiply(this)).sqrt();
			Dfp r = x.multiply(x).add(multiply(this)).sqrt();

			if (x.sign >= 0)
			{

				// compute atan2(y, x) = 2 atan(y / (r + x))
				return Two.multiply(divide(r.add(x)).atan());

			}
			else
			{

				// compute atan2(y, x) = +/- pi - 2 atan(y / (r - x))
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp tmp = getTwo().multiply(divide(r.subtract(x)).atan());
				Dfp tmp = Two.multiply(divide(r.subtract(x)).atan());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp pmPi = newInstance((tmp.sign <= 0) ? -org.apache.commons.math3.util.FastMath.PI : org.apache.commons.math3.util.FastMath.PI);
				Dfp pmPi = newInstance((tmp.sign <= 0) ? - FastMath.PI : FastMath.PI);
				return pmPi.subtract(tmp);

			}

		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp cosh()
		{
			return DfpMath.exp(this).add(DfpMath.exp(negate())).divide(2);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp sinh()
		{
			return DfpMath.exp(this).subtract(DfpMath.exp(negate())).divide(2);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp tanh()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp ePlus = DfpMath.exp(this);
			Dfp ePlus = DfpMath.exp(this);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp eMinus = DfpMath.exp(negate());
			Dfp eMinus = DfpMath.exp(negate());
			return ePlus.subtract(eMinus).divide(ePlus.add(eMinus));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp acosh()
		{
			return multiply(this).subtract(One).sqrt().add(this).log();
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp asinh()
		{
			return multiply(this).add(One).sqrt().add(this).log();
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Dfp atanh()
		{
			return One.add(this).divide(One.subtract(this)).log().divide(2);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final Dfp[] a, final Dfp[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Dfp linearCombination(Dfp[] a, Dfp[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}
			Dfp r = Zero;
			for (int i = 0; i < a.Length; ++i)
			{
				r = r.add(a[i].multiply(b[i]));
			}
			return r;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final double[] a, final Dfp[] b) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Dfp linearCombination(double[] a, Dfp[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}
			Dfp r = Zero;
			for (int i = 0; i < a.Length; ++i)
			{
				r = r.add(b[i].multiply(a[i]));
			}
			return r;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final Dfp a1, final Dfp b1, final Dfp a2, final Dfp b2)
		public virtual Dfp linearCombination(Dfp a1, Dfp b1, Dfp a2, Dfp b2)
		{
			return a1.multiply(b1).add(a2.multiply(b2));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final double a1, final Dfp b1, final double a2, final Dfp b2)
		public virtual Dfp linearCombination(double a1, Dfp b1, double a2, Dfp b2)
		{
			return b1.multiply(a1).add(b2.multiply(a2));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final Dfp a1, final Dfp b1, final Dfp a2, final Dfp b2, final Dfp a3, final Dfp b3)
		public virtual Dfp linearCombination(Dfp a1, Dfp b1, Dfp a2, Dfp b2, Dfp a3, Dfp b3)
		{
			return a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final double a1, final Dfp b1, final double a2, final Dfp b2, final double a3, final Dfp b3)
		public virtual Dfp linearCombination(double a1, Dfp b1, double a2, Dfp b2, double a3, Dfp b3)
		{
			return b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final Dfp a1, final Dfp b1, final Dfp a2, final Dfp b2, final Dfp a3, final Dfp b3, final Dfp a4, final Dfp b4)
		public virtual Dfp linearCombination(Dfp a1, Dfp b1, Dfp a2, Dfp b2, Dfp a3, Dfp b3, Dfp a4, Dfp b4)
		{
			return a1.multiply(b1).add(a2.multiply(b2)).add(a3.multiply(b3)).add(a4.multiply(b4));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp linearCombination(final double a1, final Dfp b1, final double a2, final Dfp b2, final double a3, final Dfp b3, final double a4, final Dfp b4)
		public virtual Dfp linearCombination(double a1, Dfp b1, double a2, Dfp b2, double a3, Dfp b3, double a4, Dfp b4)
		{
			return b1.multiply(a1).add(b2.multiply(a2)).add(b3.multiply(a3)).add(b4.multiply(a4));
		}

	}

}