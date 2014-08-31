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
	using org.apache.commons.math3;

	/// <summary>
	/// Field for Decimal floating point instances.
	/// @version $Id: DfpField.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 2.2
	/// </summary>
	public class DfpField : Field<Dfp>
	{

		/// <summary>
		/// Enumerate for rounding modes. </summary>
		public enum RoundingMode
		{

			/// <summary>
			/// Rounds toward zero (truncation). </summary>
			ROUND_DOWN,

			/// <summary>
			/// Rounds away from zero if discarded digit is non-zero. </summary>
			ROUND_UP,

			/// <summary>
			/// Rounds towards nearest unless both are equidistant in which case it rounds away from zero. </summary>
			ROUND_HALF_UP,

			/// <summary>
			/// Rounds towards nearest unless both are equidistant in which case it rounds toward zero. </summary>
			ROUND_HALF_DOWN,

			/// <summary>
			/// Rounds towards nearest unless both are equidistant in which case it rounds toward the even neighbor.
			/// This is the default as  specified by IEEE 854-1987
			/// </summary>
			ROUND_HALF_EVEN,

			/// <summary>
			/// Rounds towards nearest unless both are equidistant in which case it rounds toward the odd neighbor. </summary>
			ROUND_HALF_ODD,

			/// <summary>
			/// Rounds towards positive infinity. </summary>
			ROUND_CEIL,

			/// <summary>
			/// Rounds towards negative infinity. </summary>
			ROUND_FLOOR

		}

		/// <summary>
		/// IEEE 854-1987 flag for invalid operation. </summary>
		public const int FLAG_INVALID = 1;

		/// <summary>
		/// IEEE 854-1987 flag for division by zero. </summary>
		public const int FLAG_DIV_ZERO = 2;

		/// <summary>
		/// IEEE 854-1987 flag for overflow. </summary>
		public const int FLAG_OVERFLOW = 4;

		/// <summary>
		/// IEEE 854-1987 flag for underflow. </summary>
		public const int FLAG_UNDERFLOW = 8;

		/// <summary>
		/// IEEE 854-1987 flag for inexact result. </summary>
		public const int FLAG_INEXACT = 16;

		/// <summary>
		/// High precision string representation of &radic;2. </summary>
		private static string sqr2String;

		// Note: the static strings are set up (once) by the ctor and @GuardedBy("DfpField.class")

		/// <summary>
		/// High precision string representation of &radic;2 / 2. </summary>
		private static string sqr2ReciprocalString;

		/// <summary>
		/// High precision string representation of &radic;3. </summary>
		private static string sqr3String;

		/// <summary>
		/// High precision string representation of &radic;3 / 3. </summary>
		private static string sqr3ReciprocalString;

		/// <summary>
		/// High precision string representation of &pi;. </summary>
		private static string piString;

		/// <summary>
		/// High precision string representation of e. </summary>
		private static string eString;

		/// <summary>
		/// High precision string representation of ln(2). </summary>
		private static string ln2String;

		/// <summary>
		/// High precision string representation of ln(5). </summary>
		private static string ln5String;

		/// <summary>
		/// High precision string representation of ln(10). </summary>
		private static string ln10String;

		/// <summary>
		/// The number of radix digits.
		/// Note these depend on the radix which is 10000 digits,
		/// so each one is equivalent to 4 decimal digits.
		/// </summary>
		private readonly int radixDigits;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value 0. </summary>
		private readonly Dfp zero;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value 1. </summary>
		private readonly Dfp one;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value 2. </summary>
		private readonly Dfp two;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value &radic;2. </summary>
		private readonly Dfp sqr2;

		/// <summary>
		/// A two elements <seealso cref="Dfp"/> array with value &radic;2 split in two pieces. </summary>
		private readonly Dfp[] sqr2Split;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value &radic;2 / 2. </summary>
		private readonly Dfp sqr2Reciprocal;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value &radic;3. </summary>
		private readonly Dfp sqr3;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value &radic;3 / 3. </summary>
		private readonly Dfp sqr3Reciprocal;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value &pi;. </summary>
		private readonly Dfp pi;

		/// <summary>
		/// A two elements <seealso cref="Dfp"/> array with value &pi; split in two pieces. </summary>
		private readonly Dfp[] piSplit;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value e. </summary>
		private readonly Dfp e;

		/// <summary>
		/// A two elements <seealso cref="Dfp"/> array with value e split in two pieces. </summary>
		private readonly Dfp[] eSplit;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value ln(2). </summary>
		private readonly Dfp ln2;

		/// <summary>
		/// A two elements <seealso cref="Dfp"/> array with value ln(2) split in two pieces. </summary>
		private readonly Dfp[] ln2Split;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value ln(5). </summary>
		private readonly Dfp ln5;

		/// <summary>
		/// A two elements <seealso cref="Dfp"/> array with value ln(5) split in two pieces. </summary>
		private readonly Dfp[] ln5Split;

		/// <summary>
		/// A <seealso cref="Dfp"/> with value ln(10). </summary>
		private readonly Dfp ln10;

		/// <summary>
		/// Current rounding mode. </summary>
		private RoundingMode rMode;

		/// <summary>
		/// IEEE 854-1987 signals. </summary>
		private int ieeeFlags;

		/// <summary>
		/// Create a factory for the specified number of radix digits.
		/// <p>
		/// Note that since the <seealso cref="Dfp"/> class uses 10000 as its radix, each radix
		/// digit is equivalent to 4 decimal digits. This implies that asking for
		/// 13, 14, 15 or 16 decimal digits will really lead to a 4 radix 10000 digits in
		/// all cases.
		/// </p> </summary>
		/// <param name="decimalDigits"> minimal number of decimal digits. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DfpField(final int decimalDigits)
		public DfpField(int decimalDigits) : this(decimalDigits, true)
		{
		}

		/// <summary>
		/// Create a factory for the specified number of radix digits.
		/// <p>
		/// Note that since the <seealso cref="Dfp"/> class uses 10000 as its radix, each radix
		/// digit is equivalent to 4 decimal digits. This implies that asking for
		/// 13, 14, 15 or 16 decimal digits will really lead to a 4 radix 10000 digits in
		/// all cases.
		/// </p> </summary>
		/// <param name="decimalDigits"> minimal number of decimal digits </param>
		/// <param name="computeConstants"> if true, the transcendental constants for the given precision
		/// must be computed (setting this flag to false is RESERVED for the internal recursive call) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private DfpField(final int decimalDigits, final boolean computeConstants)
		private DfpField(int decimalDigits, bool computeConstants)
		{

			this.radixDigits = (decimalDigits < 13) ? 4 : (decimalDigits + 3) / 4;
			this.rMode = RoundingMode.ROUND_HALF_EVEN;
			this.ieeeFlags = 0;
			this.zero = new Dfp(this, 0);
			this.one = new Dfp(this, 1);
			this.two = new Dfp(this, 2);

			if (computeConstants)
			{
				// set up transcendental constants
				lock (typeof(DfpField))
				{

					// as a heuristic to circumvent Table-Maker's Dilemma, we set the string
					// representation of the constants to be at least 3 times larger than the
					// number of decimal digits, also as an attempt to really compute these
					// constants only once, we set a minimum number of digits
					computeStringConstants((decimalDigits < 67) ? 200 : (3 * decimalDigits));

					// set up the constants at current field accuracy
					sqr2 = new Dfp(this, sqr2String);
					sqr2Split = Split(sqr2String);
					sqr2Reciprocal = new Dfp(this, sqr2ReciprocalString);
					sqr3 = new Dfp(this, sqr3String);
					sqr3Reciprocal = new Dfp(this, sqr3ReciprocalString);
					pi = new Dfp(this, piString);
					piSplit = Split(piString);
					e = new Dfp(this, eString);
					eSplit = Split(eString);
					ln2 = new Dfp(this, ln2String);
					ln2Split = Split(ln2String);
					ln5 = new Dfp(this, ln5String);
					ln5Split = Split(ln5String);
					ln10 = new Dfp(this, ln10String);

				}
			}
			else
			{
				// dummy settings for unused constants
				sqr2 = null;
				sqr2Split = null;
				sqr2Reciprocal = null;
				sqr3 = null;
				sqr3Reciprocal = null;
				pi = null;
				piSplit = null;
				e = null;
				eSplit = null;
				ln2 = null;
				ln2Split = null;
				ln5 = null;
				ln5Split = null;
				ln10 = null;
			}

		}

		/// <summary>
		/// Get the number of radix digits of the <seealso cref="Dfp"/> instances built by this factory. </summary>
		/// <returns> number of radix digits </returns>
		public virtual int RadixDigits
		{
			get
			{
				return radixDigits;
			}
		}

		/// <summary>
		/// Set the rounding mode.
		///  If not set, the default value is <seealso cref="RoundingMode#ROUND_HALF_EVEN"/>. </summary>
		/// <param name="mode"> desired rounding mode
		/// Note that the rounding mode is common to all <seealso cref="Dfp"/> instances
		/// belonging to the current <seealso cref="DfpField"/> in the system and will
		/// affect all future calculations. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setRoundingMode(final RoundingMode mode)
		public virtual RoundingMode RoundingMode
		{
			set
			{
				rMode = value;
			}
			get
			{
				return rMode;
			}
		}


		/// <summary>
		/// Get the IEEE 854 status flags. </summary>
		/// <returns> IEEE 854 status flags </returns>
		/// <seealso cref= #clearIEEEFlags() </seealso>
		/// <seealso cref= #setIEEEFlags(int) </seealso>
		/// <seealso cref= #setIEEEFlagsBits(int) </seealso>
		/// <seealso cref= #FLAG_INVALID </seealso>
		/// <seealso cref= #FLAG_DIV_ZERO </seealso>
		/// <seealso cref= #FLAG_OVERFLOW </seealso>
		/// <seealso cref= #FLAG_UNDERFLOW </seealso>
		/// <seealso cref= #FLAG_INEXACT </seealso>
		public virtual int IEEEFlags
		{
			get
			{
				return ieeeFlags;
			}
			set
			{
				ieeeFlags = value & (FLAG_INVALID | FLAG_DIV_ZERO | FLAG_OVERFLOW | FLAG_UNDERFLOW | FLAG_INEXACT);
			}
		}

		/// <summary>
		/// Clears the IEEE 854 status flags. </summary>
		/// <seealso cref= #getIEEEFlags() </seealso>
		/// <seealso cref= #setIEEEFlags(int) </seealso>
		/// <seealso cref= #setIEEEFlagsBits(int) </seealso>
		/// <seealso cref= #FLAG_INVALID </seealso>
		/// <seealso cref= #FLAG_DIV_ZERO </seealso>
		/// <seealso cref= #FLAG_OVERFLOW </seealso>
		/// <seealso cref= #FLAG_UNDERFLOW </seealso>
		/// <seealso cref= #FLAG_INEXACT </seealso>
		public virtual void clearIEEEFlags()
		{
			ieeeFlags = 0;
		}

		/// <summary>
		/// Sets the IEEE 854 status flags. </summary>
		/// <param name="flags"> desired value for the flags </param>
		/// <seealso cref= #getIEEEFlags() </seealso>
		/// <seealso cref= #clearIEEEFlags() </seealso>
		/// <seealso cref= #setIEEEFlagsBits(int) </seealso>
		/// <seealso cref= #FLAG_INVALID </seealso>
		/// <seealso cref= #FLAG_DIV_ZERO </seealso>
		/// <seealso cref= #FLAG_OVERFLOW </seealso>
		/// <seealso cref= #FLAG_UNDERFLOW </seealso>
		/// <seealso cref= #FLAG_INEXACT </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setIEEEFlags(final int flags)

		/// <summary>
		/// Sets some bits in the IEEE 854 status flags, without changing the already set bits.
		/// <p>
		/// Calling this method is equivalent to call {@code setIEEEFlags(getIEEEFlags() | bits)}
		/// </p> </summary>
		/// <param name="bits"> bits to set </param>
		/// <seealso cref= #getIEEEFlags() </seealso>
		/// <seealso cref= #clearIEEEFlags() </seealso>
		/// <seealso cref= #setIEEEFlags(int) </seealso>
		/// <seealso cref= #FLAG_INVALID </seealso>
		/// <seealso cref= #FLAG_DIV_ZERO </seealso>
		/// <seealso cref= #FLAG_OVERFLOW </seealso>
		/// <seealso cref= #FLAG_UNDERFLOW </seealso>
		/// <seealso cref= #FLAG_INEXACT </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setIEEEFlagsBits(final int bits)
		public virtual int IEEEFlagsBits
		{
			set
			{
				ieeeFlags |= value & (FLAG_INVALID | FLAG_DIV_ZERO | FLAG_OVERFLOW | FLAG_UNDERFLOW | FLAG_INEXACT);
			}
		}

		/// <summary>
		/// Makes a <seealso cref="Dfp"/> with a value of 0. </summary>
		/// <returns> a new <seealso cref="Dfp"/> with a value of 0 </returns>
		public virtual Dfp newDfp()
		{
			return new Dfp(this);
		}

		/// <summary>
		/// Create an instance from a byte value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new <seealso cref="Dfp"/> with the same value as x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newDfp(final byte x)
		public virtual Dfp newDfp(sbyte x)
		{
			return new Dfp(this, x);
		}

		/// <summary>
		/// Create an instance from an int value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new <seealso cref="Dfp"/> with the same value as x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newDfp(final int x)
		public virtual Dfp newDfp(int x)
		{
			return new Dfp(this, x);
		}

		/// <summary>
		/// Create an instance from a long value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new <seealso cref="Dfp"/> with the same value as x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newDfp(final long x)
		public virtual Dfp newDfp(long x)
		{
			return new Dfp(this, x);
		}

		/// <summary>
		/// Create an instance from a double value. </summary>
		/// <param name="x"> value to convert to an instance </param>
		/// <returns> a new <seealso cref="Dfp"/> with the same value as x </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newDfp(final double x)
		public virtual Dfp newDfp(double x)
		{
			return new Dfp(this, x);
		}

		/// <summary>
		/// Copy constructor. </summary>
		/// <param name="d"> instance to copy </param>
		/// <returns> a new <seealso cref="Dfp"/> with the same value as d </returns>
		public virtual Dfp newDfp(Dfp d)
		{
			return new Dfp(d);
		}

		/// <summary>
		/// Create a <seealso cref="Dfp"/> given a String representation. </summary>
		/// <param name="s"> string representation of the instance </param>
		/// <returns> a new <seealso cref="Dfp"/> parsed from specified string </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newDfp(final String s)
		public virtual Dfp newDfp(string s)
		{
			return new Dfp(this, s);
		}

		/// <summary>
		/// Creates a <seealso cref="Dfp"/> with a non-finite value. </summary>
		/// <param name="sign"> sign of the Dfp to create </param>
		/// <param name="nans"> code of the value, must be one of <seealso cref="Dfp#INFINITE"/>,
		/// <seealso cref="Dfp#SNAN"/>,  <seealso cref="Dfp#QNAN"/> </param>
		/// <returns> a new <seealso cref="Dfp"/> with a non-finite value </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Dfp newDfp(final byte sign, final byte nans)
		public virtual Dfp newDfp(sbyte sign, sbyte nans)
		{
			return new Dfp(this, sign, nans);
		}

		/// <summary>
		/// Get the constant 0. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value 0 </returns>
		public virtual Dfp Zero
		{
			get
			{
				return zero;
			}
		}

		/// <summary>
		/// Get the constant 1. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value 1 </returns>
		public virtual Dfp One
		{
			get
			{
				return one;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Type RuntimeClass
		{
			get
			{
				return typeof(Dfp);
			}
		}

		/// <summary>
		/// Get the constant 2. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value 2 </returns>
		public virtual Dfp Two
		{
			get
			{
				return two;
			}
		}

		/// <summary>
		/// Get the constant &radic;2. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &radic;2 </returns>
		public virtual Dfp Sqr2
		{
			get
			{
				return sqr2;
			}
		}

		/// <summary>
		/// Get the constant &radic;2 split in two pieces. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &radic;2 split in two pieces </returns>
		public virtual Dfp[] Sqr2Split
		{
			get
			{
				return sqr2Split.clone();
			}
		}

		/// <summary>
		/// Get the constant &radic;2 / 2. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &radic;2 / 2 </returns>
		public virtual Dfp Sqr2Reciprocal
		{
			get
			{
				return sqr2Reciprocal;
			}
		}

		/// <summary>
		/// Get the constant &radic;3. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &radic;3 </returns>
		public virtual Dfp Sqr3
		{
			get
			{
				return sqr3;
			}
		}

		/// <summary>
		/// Get the constant &radic;3 / 3. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &radic;3 / 3 </returns>
		public virtual Dfp Sqr3Reciprocal
		{
			get
			{
				return sqr3Reciprocal;
			}
		}

		/// <summary>
		/// Get the constant &pi;. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &pi; </returns>
		public virtual Dfp Pi
		{
			get
			{
				return pi;
			}
		}

		/// <summary>
		/// Get the constant &pi; split in two pieces. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value &pi; split in two pieces </returns>
		public virtual Dfp[] PiSplit
		{
			get
			{
				return piSplit.clone();
			}
		}

		/// <summary>
		/// Get the constant e. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value e </returns>
		public virtual Dfp E
		{
			get
			{
				return e;
			}
		}

		/// <summary>
		/// Get the constant e split in two pieces. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value e split in two pieces </returns>
		public virtual Dfp[] ESplit
		{
			get
			{
				return eSplit.clone();
			}
		}

		/// <summary>
		/// Get the constant ln(2). </summary>
		/// <returns> a <seealso cref="Dfp"/> with value ln(2) </returns>
		public virtual Dfp Ln2
		{
			get
			{
				return ln2;
			}
		}

		/// <summary>
		/// Get the constant ln(2) split in two pieces. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value ln(2) split in two pieces </returns>
		public virtual Dfp[] Ln2Split
		{
			get
			{
				return ln2Split.clone();
			}
		}

		/// <summary>
		/// Get the constant ln(5). </summary>
		/// <returns> a <seealso cref="Dfp"/> with value ln(5) </returns>
		public virtual Dfp Ln5
		{
			get
			{
				return ln5;
			}
		}

		/// <summary>
		/// Get the constant ln(5) split in two pieces. </summary>
		/// <returns> a <seealso cref="Dfp"/> with value ln(5) split in two pieces </returns>
		public virtual Dfp[] Ln5Split
		{
			get
			{
				return ln5Split.clone();
			}
		}

		/// <summary>
		/// Get the constant ln(10). </summary>
		/// <returns> a <seealso cref="Dfp"/> with value ln(10) </returns>
		public virtual Dfp Ln10
		{
			get
			{
				return ln10;
			}
		}

		/// <summary>
		/// Breaks a string representation up into two <seealso cref="Dfp"/>'s.
		/// The split is such that the sum of them is equivalent to the input string,
		/// but has higher precision than using a single Dfp. </summary>
		/// <param name="a"> string representation of the number to split </param>
		/// <returns> an array of two <seealso cref="Dfp Dfp"/> instances which sum equals a </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Dfp[] split(final String a)
		private Dfp[] Split(string a)
		{
		  Dfp[] result = new Dfp[2];
		  bool leading = true;
		  int sp = 0;
		  int sig = 0;

		  char[] buf = new char[a.Length];

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

			if (sig == (radixDigits / 2) * 4)
			{
			  sp = i;
			  break;
			}

			if (buf[i] >= '0' && buf[i] <= '9' && !leading)
			{
				sig++;
			}
		  }

		  result[0] = new Dfp(this, new string(buf, 0, sp));

		  for (int i = 0; i < buf.Length; i++)
		  {
			buf[i] = a[i];
			if (buf[i] >= '0' && buf[i] <= '9' && i < sp)
			{
				buf[i] = '0';
			}
		  }

		  result[1] = new Dfp(this, new string(buf));

		  return result;

		}

		/// <summary>
		/// Recompute the high precision string constants. </summary>
		/// <param name="highPrecisionDecimalDigits"> precision at which the string constants mus be computed </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void computeStringConstants(final int highPrecisionDecimalDigits)
		private static void computeStringConstants(int highPrecisionDecimalDigits)
		{
			if (sqr2String == null || sqr2String.Length < highPrecisionDecimalDigits - 3)
			{

				// recompute the string representation of the transcendental constants
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DfpField highPrecisionField = new DfpField(highPrecisionDecimalDigits, false);
				DfpField highPrecisionField = new DfpField(highPrecisionDecimalDigits, false);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp highPrecisionOne = new Dfp(highPrecisionField, 1);
				Dfp highPrecisionOne = new Dfp(highPrecisionField, 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp highPrecisionTwo = new Dfp(highPrecisionField, 2);
				Dfp highPrecisionTwo = new Dfp(highPrecisionField, 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp highPrecisionThree = new Dfp(highPrecisionField, 3);
				Dfp highPrecisionThree = new Dfp(highPrecisionField, 3);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp highPrecisionSqr2 = highPrecisionTwo.sqrt();
				Dfp highPrecisionSqr2 = highPrecisionTwo.sqrt();
				sqr2String = highPrecisionSqr2.ToString();
				sqr2ReciprocalString = highPrecisionOne.divide(highPrecisionSqr2).ToString();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp highPrecisionSqr3 = highPrecisionThree.sqrt();
				Dfp highPrecisionSqr3 = highPrecisionThree.sqrt();
				sqr3String = highPrecisionSqr3.ToString();
				sqr3ReciprocalString = highPrecisionOne.divide(highPrecisionSqr3).ToString();

				piString = computePi(highPrecisionOne, highPrecisionTwo, highPrecisionThree).ToString();
				eString = computeExp(highPrecisionOne, highPrecisionOne).ToString();
				ln2String = computeLn(highPrecisionTwo, highPrecisionOne, highPrecisionTwo).ToString();
				ln5String = computeLn(new Dfp(highPrecisionField, 5), highPrecisionOne, highPrecisionTwo).ToString();
				ln10String = computeLn(new Dfp(highPrecisionField, 10), highPrecisionOne, highPrecisionTwo).ToString();

			}
		}

		/// <summary>
		/// Compute &pi; using Jonathan and Peter Borwein quartic formula. </summary>
		/// <param name="one"> constant with value 1 at desired precision </param>
		/// <param name="two"> constant with value 2 at desired precision </param>
		/// <param name="three"> constant with value 3 at desired precision </param>
		/// <returns> &pi; </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static Dfp computePi(final Dfp one, final Dfp two, final Dfp three)
		private static Dfp computePi(Dfp one, Dfp two, Dfp three)
		{

			Dfp sqrt2 = two.sqrt();
			Dfp yk = sqrt2.subtract(one);
			Dfp four = two.add(two);
			Dfp two2kp3 = two;
			Dfp ak = two.multiply(three.subtract(two.multiply(sqrt2)));

			// The formula converges quartically. This means the number of correct
			// digits is multiplied by 4 at each iteration! Five iterations are
			// sufficient for about 160 digits, eight iterations give about
			// 10000 digits (this has been checked) and 20 iterations more than
			// 160 billions of digits (this has NOT been checked).
			// So the limit here is considered sufficient for most purposes ...
			for (int i = 1; i < 20; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp ykM1 = yk;
				Dfp ykM1 = yk;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp y2 = yk.multiply(yk);
				Dfp y2 = yk.multiply(yk);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp oneMinusY4 = one.subtract(y2.multiply(y2));
				Dfp oneMinusY4 = one.subtract(y2.multiply(y2));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp s = oneMinusY4.sqrt().sqrt();
				Dfp s = oneMinusY4.sqrt().sqrt();
				yk = one.subtract(s).divide(one.add(s));

				two2kp3 = two2kp3.multiply(four);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp p = one.add(yk);
				Dfp p = one.add(yk);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp p2 = p.multiply(p);
				Dfp p2 = p.multiply(p);
				ak = ak.multiply(p2.multiply(p2)).subtract(two2kp3.multiply(yk).multiply(one.add(yk).add(yk.multiply(yk))));

				if (yk.Equals(ykM1))
				{
					break;
				}
			}

			return one.divide(ak);

		}

		/// <summary>
		/// Compute exp(a). </summary>
		/// <param name="a"> number for which we want the exponential </param>
		/// <param name="one"> constant with value 1 at desired precision </param>
		/// <returns> exp(a) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp computeExp(final Dfp a, final Dfp one)
		public static Dfp computeExp(Dfp a, Dfp one)
		{

			Dfp y = new Dfp(one);
			Dfp py = new Dfp(one);
			Dfp f = new Dfp(one);
			Dfp fi = new Dfp(one);
			Dfp x = new Dfp(one);

			for (int i = 0; i < 10000; i++)
			{
				x = x.multiply(a);
				y = y.add(x.divide(f));
				fi = fi.add(one);
				f = f.multiply(fi);
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			return y;

		}


		/// <summary>
		/// Compute ln(a).
		/// 
		///  Let f(x) = ln(x),
		/// 
		///  We know that f'(x) = 1/x, thus from Taylor's theorem we have:
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
		/// <param name="a"> number for which we want the exponential </param>
		/// <param name="one"> constant with value 1 at desired precision </param>
		/// <param name="two"> constant with value 2 at desired precision </param>
		/// <returns> ln(a) </returns>

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Dfp computeLn(final Dfp a, final Dfp one, final Dfp two)
		public static Dfp computeLn(Dfp a, Dfp one, Dfp two)
		{

			int den = 1;
			Dfp x = a.add(new Dfp(a.Field, -1)).divide(a.add(one));

			Dfp y = new Dfp(x);
			Dfp num = new Dfp(x);
			Dfp py = new Dfp(y);
			for (int i = 0; i < 10000; i++)
			{
				num = num.multiply(x);
				num = num.multiply(x);
				den += 2;
				Dfp t = num.divide(den);
				y = y.add(t);
				if (y.Equals(py))
				{
					break;
				}
				py = new Dfp(y);
			}

			return y.multiply(two);

		}

	}

}