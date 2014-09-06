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



    using mathlib;
    using mathlib;
    using MathArithmeticException = mathlib.exception.MathArithmeticException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Arbitrary precision decimal number.
	/// <p>
	/// This class is a simple wrapper around the standard <code>BigDecimal</code>
	/// in order to implement the <seealso cref="FieldElement"/> interface.
	/// </p>
	/// @since 2.0
	/// @version $Id: BigReal.java 1505938 2013-07-23 08:50:10Z luc $
	/// </summary>
	[Serializable]
	public class BigReal : FieldElement<BigReal>, IComparable<BigReal>
	{

		/// <summary>
		/// A big real representing 0. </summary>
		public static readonly BigReal ZERO = new BigReal(decimal.Zero);

		/// <summary>
		/// A big real representing 1. </summary>
		public static readonly BigReal ONE = new BigReal(decimal.One);

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 4984534880991310382L;

		/// <summary>
		/// Underlying BigDecimal. </summary>
		private readonly decimal d;

		/// <summary>
		/// Rounding mode for divisions. * </summary>
		private RoundingMode roundingMode = RoundingMode.HALF_UP;

		/// <summary>
		///* BigDecimal scale ** </summary>
		private int scale = 64;

		/// <summary>
		/// Build an instance from a BigDecimal. </summary>
		/// <param name="val"> value of the instance </param>
		public BigReal(decimal val)
		{
			d = val;
		}

		/// <summary>
		/// Build an instance from a BigInteger. </summary>
		/// <param name="val"> value of the instance </param>
		public BigReal(System.Numerics.BigInteger val)
		{
			d = new decimal(val);
		}

		/// <summary>
		/// Build an instance from an unscaled BigInteger. </summary>
		/// <param name="unscaledVal"> unscaled value </param>
		/// <param name="scale"> scale to use </param>
		public BigReal(System.Numerics.BigInteger unscaledVal, int scale)
		{
			d = new decimal(unscaledVal, scale);
		}

		/// <summary>
		/// Build an instance from an unscaled BigInteger. </summary>
		/// <param name="unscaledVal"> unscaled value </param>
		/// <param name="scale"> scale to use </param>
		/// <param name="mc"> to used </param>
		public BigReal(System.Numerics.BigInteger unscaledVal, int scale, MathContext mc)
		{
			d = new decimal(unscaledVal, scale, mc);
		}

		/// <summary>
		/// Build an instance from a BigInteger. </summary>
		/// <param name="val"> value of the instance </param>
		/// <param name="mc"> context to use </param>
		public BigReal(System.Numerics.BigInteger val, MathContext mc)
		{
			d = new decimal(val, mc);
		}

		/// <summary>
		/// Build an instance from a characters representation. </summary>
		/// <param name="in"> character representation of the value </param>
		public BigReal(char[] @in)
		{
			d = new decimal(@in);
		}

		/// <summary>
		/// Build an instance from a characters representation. </summary>
		/// <param name="in"> character representation of the value </param>
		/// <param name="offset"> offset of the first character to analyze </param>
		/// <param name="len"> length of the array slice to analyze </param>
		public BigReal(char[] @in, int offset, int len)
		{
			d = new decimal(@in, offset, len);
		}

		/// <summary>
		/// Build an instance from a characters representation. </summary>
		/// <param name="in"> character representation of the value </param>
		/// <param name="offset"> offset of the first character to analyze </param>
		/// <param name="len"> length of the array slice to analyze </param>
		/// <param name="mc"> context to use </param>
		public BigReal(char[] @in, int offset, int len, MathContext mc)
		{
			d = new decimal(@in, offset, len, mc);
		}

		/// <summary>
		/// Build an instance from a characters representation. </summary>
		/// <param name="in"> character representation of the value </param>
		/// <param name="mc"> context to use </param>
		public BigReal(char[] @in, MathContext mc)
		{
			d = new decimal(@in, mc);
		}

		/// <summary>
		/// Build an instance from a double. </summary>
		/// <param name="val"> value of the instance </param>
		public BigReal(double val)
		{
			d = new decimal(val);
		}

		/// <summary>
		/// Build an instance from a double. </summary>
		/// <param name="val"> value of the instance </param>
		/// <param name="mc"> context to use </param>
		public BigReal(double val, MathContext mc)
		{
			d = new decimal(val, mc);
		}

		/// <summary>
		/// Build an instance from an int. </summary>
		/// <param name="val"> value of the instance </param>
		public BigReal(int val)
		{
			d = new decimal(val);
		}

		/// <summary>
		/// Build an instance from an int. </summary>
		/// <param name="val"> value of the instance </param>
		/// <param name="mc"> context to use </param>
		public BigReal(int val, MathContext mc)
		{
			d = new decimal(val, mc);
		}

		/// <summary>
		/// Build an instance from a long. </summary>
		/// <param name="val"> value of the instance </param>
		public BigReal(long val)
		{
			d = new decimal(val);
		}

		/// <summary>
		/// Build an instance from a long. </summary>
		/// <param name="val"> value of the instance </param>
		/// <param name="mc"> context to use </param>
		public BigReal(long val, MathContext mc)
		{
			d = new decimal(val, mc);
		}

		/// <summary>
		/// Build an instance from a String representation. </summary>
		/// <param name="val"> character representation of the value </param>
		public BigReal(string val)
		{
			d = new decimal(val);
		}

		/// <summary>
		/// Build an instance from a String representation. </summary>
		/// <param name="val"> character representation of the value </param>
		/// <param name="mc"> context to use </param>
		public BigReal(string val, MathContext mc)
		{
			d = new decimal(val, mc);
		}

		/// <summary>
		///*
		/// Gets the rounding mode for division operations
		/// The default is {@code RoundingMode.HALF_UP} </summary>
		/// <returns> the rounding mode.
		/// @since 2.1 </returns>
		public virtual RoundingMode RoundingMode
		{
			get
			{
				return roundingMode;
			}
			set
			{
				this.roundingMode = value;
			}
		}


		/// <summary>
		///*
		/// Sets the scale for division operations.
		/// The default is 64 </summary>
		/// <returns> the scale
		/// @since 2.1 </returns>
		public virtual int Scale
		{
			get
			{
				return scale;
			}
			set
			{
				this.scale = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual BigReal add(BigReal a)
		{
			return new BigReal(d + a.d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual BigReal subtract(BigReal a)
		{
			return new BigReal(d - a.d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual BigReal negate()
		{
			return new BigReal(-d);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathArithmeticException"> if {@code a} is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BigReal divide(BigReal a) throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual BigReal divide(BigReal a)
		{
			try
			{
				return new BigReal(d.divide(a.d, scale, roundingMode));
			}
			catch (ArithmeticException e)
			{
				// Division by zero has occurred
				throw new MathArithmeticException(LocalizedFormats.ZERO_NOT_ALLOWED);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <exception cref="MathArithmeticException"> if {@code this} is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BigReal reciprocal() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual BigReal reciprocal()
		{
			try
			{
				return new BigReal(decimal.One.divide(d, scale, roundingMode));
			}
			catch (ArithmeticException e)
			{
				// Division by zero has occurred
				throw new MathArithmeticException(LocalizedFormats.ZERO_NOT_ALLOWED);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual BigReal multiply(BigReal a)
		{
			return new BigReal(d * a.d);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public BigReal multiply(final int n)
		public virtual BigReal multiply(int n)
		{
			return new BigReal(d * (new decimal(n)));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int CompareTo(BigReal a)
		{
			return d.compareTo(a.d);
		}

		/// <summary>
		/// Get the double value corresponding to the instance. </summary>
		/// <returns> double value corresponding to the instance </returns>
		public virtual double doubleValue()
		{
			return (double)d;
		}

		/// <summary>
		/// Get the BigDecimal value corresponding to the instance. </summary>
		/// <returns> BigDecimal value corresponding to the instance </returns>
		public virtual decimal bigDecimalValue()
		{
			return d;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}

			if (other is BigReal)
			{
				return d.Equals(((BigReal) other).d);
			}
			return false;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			return d.GetHashCode();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Field<BigReal> Field
		{
			get
			{
				return BigRealField.Instance;
			}
		}
	}

}