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
	/// Subclass of <seealso cref="Dfp"/> which hides the radix-10000 artifacts of the superclass.
	/// This should give outward appearances of being a decimal number with DIGITS*4-3
	/// decimal digits. This class can be subclassed to appear to be an arbitrary number
	/// of decimal digits less than DIGITS*4-3.
	/// @version $Id: DfpDec.java 1449529 2013-02-24 19:13:17Z luc $
	/// @since 2.2
	/// </summary>
	public class DfpDec : Dfp
	{

		/// <summary>
		/// Makes an instance with a value of zero. </summary>
		/// <param name="factory"> factory linked to this instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory)
		protected internal DfpDec(DfpField factory) : base(factory)
		{
		}

		/// <summary>
		/// Create an instance from a byte value. </summary>
		/// <param name="factory"> factory linked to this instance </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory, byte x)
		protected internal DfpDec(DfpField factory, sbyte x) : base(factory, x)
		{
		}

		/// <summary>
		/// Create an instance from an int value. </summary>
		/// <param name="factory"> factory linked to this instance </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory, int x)
		protected internal DfpDec(DfpField factory, int x) : base(factory, x)
		{
		}

		/// <summary>
		/// Create an instance from a long value. </summary>
		/// <param name="factory"> factory linked to this instance </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory, long x)
		protected internal DfpDec(DfpField factory, long x) : base(factory, x)
		{
		}

		/// <summary>
		/// Create an instance from a double value. </summary>
		/// <param name="factory"> factory linked to this instance </param>
		/// <param name="x"> value to convert to an instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory, double x)
		protected internal DfpDec(DfpField factory, double x) : base(factory, x)
		{
			round(0);
		}

		/// <summary>
		/// Copy constructor. </summary>
		/// <param name="d"> instance to copy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DfpDec(final Dfp d)
		public DfpDec(Dfp d) : base(d)
		{
			round(0);
		}

		/// <summary>
		/// Create an instance from a String representation. </summary>
		/// <param name="factory"> factory linked to this instance </param>
		/// <param name="s"> string representation of the instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory, final String s)
		protected internal DfpDec(DfpField factory, string s) : base(factory, s)
		{
			round(0);
		}

		/// <summary>
		/// Creates an instance with a non-finite value. </summary>
		/// <param name="factory"> factory linked to this instance </param>
		/// <param name="sign"> sign of the Dfp to create </param>
		/// <param name="nans"> code of the value, must be one of <seealso cref="#INFINITE"/>,
		/// <seealso cref="#SNAN"/>,  <seealso cref="#QNAN"/> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected DfpDec(final DfpField factory, final byte sign, final byte nans)
		protected internal DfpDec(DfpField factory, sbyte sign, sbyte nans) : base(factory, sign, nans)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override Dfp newInstance()
		{
			return new DfpDec(Field);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final byte x)
		public override Dfp newInstance(sbyte x)
		{
			return new DfpDec(Field, x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final int x)
		public override Dfp newInstance(int x)
		{
			return new DfpDec(Field, x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final long x)
		public override Dfp newInstance(long x)
		{
			return new DfpDec(Field, x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final double x)
		public override Dfp newInstance(double x)
		{
			return new DfpDec(Field, x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final Dfp d)
		public override Dfp newInstance(Dfp d)
		{

			// make sure we don't mix number with different precision
			if (Field.RadixDigits != d.Field.RadixDigits)
			{
				Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, "newInstance", d, result);
			}

			return new DfpDec(d);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final String s)
		public override Dfp newInstance(string s)
		{
			return new DfpDec(Field, s);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Dfp newInstance(final byte sign, final byte nans)
		public override Dfp newInstance(sbyte sign, sbyte nans)
		{
			return new DfpDec(Field, sign, nans);
		}

		/// <summary>
		/// Get the number of decimal digits this class is going to represent.
		/// Default implementation returns <seealso cref="#getRadixDigits()"/>*4-3. Subclasses can
		/// override this to return something less. </summary>
		/// <returns> number of decimal digits this class is going to represent </returns>
		protected internal virtual int DecimalDigits
		{
			get
			{
				return RadixDigits * 4 - 3;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override int round(int @in)
		{

			int msb = mant[mant.Length - 1];
			if (msb == 0)
			{
				// special case -- this == zero
				return 0;
			}

			int cmaxdigits = mant.Length * 4;
			int lsbthreshold = 1000;
			while (lsbthreshold > msb)
			{
				lsbthreshold /= 10;
				cmaxdigits--;
			}


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int digits = getDecimalDigits();
			int digits = DecimalDigits;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lsbshift = cmaxdigits - digits;
			int lsbshift = cmaxdigits - digits;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lsd = lsbshift / 4;
			int lsd = lsbshift / 4;

			lsbthreshold = 1;
			for (int i = 0; i < lsbshift % 4; i++)
			{
				lsbthreshold *= 10;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lsb = mant[lsd];
			int lsb = mant[lsd];

			if (lsbthreshold <= 1 && digits == 4 * mant.Length - 3)
			{
				return base.round(@in);
			}

			int discarded = @in; // not looking at this after this point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n;
			int n;
			if (lsbthreshold == 1)
			{
				// look to the next digit for rounding
				n = (mant[lsd - 1] / 1000) % 10;
				mant[lsd - 1] %= 1000;
				discarded |= mant[lsd - 1];
			}
			else
			{
				n = (lsb * 10 / lsbthreshold) % 10;
				discarded |= lsb % (lsbthreshold / 10);
			}

			for (int i = 0; i < lsd; i++)
			{
				discarded |= mant[i]; // need to know if there are any discarded bits
				mant[i] = 0;
			}

			mant[lsd] = lsb / lsbthreshold * lsbthreshold;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean inc;
			bool inc;
			switch (Field.RoundingMode)
			{
				case ROUND_DOWN:
					inc = false;
					break;

				case ROUND_UP:
					inc = (n != 0) || (discarded != 0); // round up if n!=0
					break;

				case ROUND_HALF_UP:
					inc = n >= 5; // round half up
					break;

				case ROUND_HALF_DOWN:
					inc = n > 5; // round half down
					break;

				case ROUND_HALF_EVEN:
					inc = (n > 5) || (n == 5 && discarded != 0) || (n == 5 && discarded == 0 && ((lsb / lsbthreshold) & 1) == 1); // round half-even
					break;

				case ROUND_HALF_ODD:
					inc = (n > 5) || (n == 5 && discarded != 0) || (n == 5 && discarded == 0 && ((lsb / lsbthreshold) & 1) == 0); // round half-odd
					break;

				case ROUND_CEIL:
					inc = (sign == 1) && (n != 0 || discarded != 0); // round ceil
					break;

				case ROUND_FLOOR:
				default:
					inc = (sign == -1) && (n != 0 || discarded != 0); // round floor
					break;
			}

			if (inc)
			{
				// increment if necessary
				int rh = lsbthreshold;
				for (int i = lsd; i < mant.Length; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = mant[i] + rh;
					int r = mant[i] + rh;
					rh = r / RADIX;
					mant[i] = r % RADIX;
				}

				if (rh != 0)
				{
					shiftRight();
					mant[mant.Length - 1] = rh;
				}
			}

			// Check for exceptional cases and raise signals if necessary
			if (exp_Renamed < MIN_EXP)
			{
				// Gradual Underflow
				Field.IEEEFlagsBits = DfpField.FLAG_UNDERFLOW;
				return DfpField.FLAG_UNDERFLOW;
			}

			if (exp_Renamed > MAX_EXP)
			{
				// Overflow
				Field.IEEEFlagsBits = DfpField.FLAG_OVERFLOW;
				return DfpField.FLAG_OVERFLOW;
			}

			if (n != 0 || discarded != 0)
			{
				// Inexact
				Field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				return DfpField.FLAG_INEXACT;
			}
			return 0;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override Dfp nextAfter(Dfp x)
		{

			const string trapName = "nextAfter";

			// make sure we don't mix number with different precision
			if (Field.RadixDigits != x.Field.RadixDigits)
			{
				Field.IEEEFlagsBits = DfpField.FLAG_INVALID;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Dfp result = newInstance(getZero());
				Dfp result = newInstance(Zero);
				result.nans = QNAN;
				return dotrap(DfpField.FLAG_INVALID, trapName, x, result);
			}

			bool up = false;
			Dfp result;
			Dfp inc;

			// if this is greater than x
			if (this.lessThan(x))
			{
				up = true;
			}

			if (Equals(x))
			{
				return newInstance(x);
			}

			if (lessThan(Zero))
			{
				up = !up;
			}

			if (up)
			{
				inc = power10(intLog10() - DecimalDigits + 1);
				inc = copysign(inc, this);

				if (this.Equals(Zero))
				{
					inc = power10K(MIN_EXP - mant.Length - 1);
				}

				if (inc.Equals(Zero))
				{
					result = copysign(newInstance(Zero), this);
				}
				else
				{
					result = add(inc);
				}
			}
			else
			{
				inc = power10(intLog10());
				inc = copysign(inc, this);

				if (this.Equals(inc))
				{
					inc = inc.divide(power10(DecimalDigits));
				}
				else
				{
					inc = inc.divide(power10(DecimalDigits - 1));
				}

				if (this.Equals(Zero))
				{
					inc = power10K(MIN_EXP - mant.Length - 1);
				}

				if (inc.Equals(Zero))
				{
					result = copysign(newInstance(Zero), this);
				}
				else
				{
					result = subtract(inc);
				}
			}

			if (result.classify() == INFINITE && this.classify() != INFINITE)
			{
				Field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				result = dotrap(DfpField.FLAG_INEXACT, trapName, x, result);
			}

			if (result.Equals(Zero) && this.Equals(Zero) == false)
			{
				Field.IEEEFlagsBits = DfpField.FLAG_INEXACT;
				result = dotrap(DfpField.FLAG_INEXACT, trapName, x, result);
			}

			return result;
		}

	}

}