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
    using DimensionMismatchException = mathlib.exception.DimensionMismatchException;

	/// <summary>
	/// This class wraps a {@code double} value in an object. It is similar to the
	/// standard class <seealso cref="Double"/>, while also implementing the
	/// <seealso cref="RealFieldElement"/> interface.
	/// 
	/// @since 3.1
	/// @version $Id: Decimal64.java 1462423 2013-03-29 07:25:18Z luc $
	/// </summary>
	public class Decimal64 : Number, RealFieldElement<Decimal64>, IComparable<Decimal64>
	{

		/// <summary>
		/// The constant value of {@code 0d} as a {@code Decimal64}. </summary>
		public static readonly Decimal64 ZERO;

		/// <summary>
		/// The constant value of {@code 1d} as a {@code Decimal64}. </summary>
		public static readonly Decimal64 ONE;

		/// <summary>
		/// The constant value of <seealso cref="Double#NEGATIVE_INFINITY"/> as a
		/// {@code Decimal64}.
		/// </summary>
		public static readonly Decimal64 NEGATIVE_INFINITY;

		/// <summary>
		/// The constant value of <seealso cref="Double#POSITIVE_INFINITY"/> as a
		/// {@code Decimal64}.
		/// </summary>
		public static readonly Decimal64 POSITIVE_INFINITY;

		/// <summary>
		/// The constant value of <seealso cref="Double#NaN"/> as a {@code Decimal64}. </summary>
		public static readonly Decimal64 NAN;

		private const long serialVersionUID = 20120227L;

		static Decimal64()
		{
			ZERO = new Decimal64(0d);
			ONE = new Decimal64(1d);
			NEGATIVE_INFINITY = new Decimal64(double.NegativeInfinity);
			POSITIVE_INFINITY = new Decimal64(double.PositiveInfinity);
			NAN = new Decimal64(double.NaN);
		}

		/// <summary>
		/// The primitive {@code double} value of this object. </summary>
		private readonly double value;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="x"> the primitive {@code double} value of the object to be created </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64(final double x)
		public Decimal64(double x)
		{
			this.value = x;
		}

		/*
		 * Methods from the FieldElement interface.
		 */

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Field<Decimal64> Field
		{
			get
			{
				return Decimal64Field.Instance;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.add(a).equals(new Decimal64(this.doubleValue()
		/// + a.doubleValue()))}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 add(final Decimal64 a)
		public virtual Decimal64 add(Decimal64 a)
		{
			return new Decimal64(this.value + a.value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.subtract(a).equals(new Decimal64(this.doubleValue()
		/// - a.doubleValue()))}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 subtract(final Decimal64 a)
		public virtual Decimal64 subtract(Decimal64 a)
		{
			return new Decimal64(this.value - a.value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.negate().equals(new Decimal64(-this.doubleValue()))}.
		/// </summary>
		public virtual Decimal64 negate()
		{
			return new Decimal64(-this.value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.multiply(a).equals(new Decimal64(this.doubleValue()
		/// * a.doubleValue()))}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 multiply(final Decimal64 a)
		public virtual Decimal64 multiply(Decimal64 a)
		{
			return new Decimal64(this.value * a.value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.multiply(n).equals(new Decimal64(n * this.doubleValue()))}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 multiply(final int n)
		public virtual Decimal64 multiply(int n)
		{
			return new Decimal64(n * this.value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.divide(a).equals(new Decimal64(this.doubleValue()
		/// / a.doubleValue()))}.
		/// 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 divide(final Decimal64 a)
		public virtual Decimal64 divide(Decimal64 a)
		{
			return new Decimal64(this.value / a.value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation strictly enforces
		/// {@code this.reciprocal().equals(new Decimal64(1.0
		/// / this.doubleValue()))}.
		/// </summary>
		public virtual Decimal64 reciprocal()
		{
			return new Decimal64(1.0 / this.value);
		}

		/*
		 * Methods from the Number abstract class
		 */

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation performs casting to a {@code byte}.
		/// </summary>
		public override sbyte byteValue()
		{
			return (sbyte) value;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation performs casting to a {@code short}.
		/// </summary>
		public override short shortValue()
		{
			return (short) value;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation performs casting to a {@code int}.
		/// </summary>
		public override int intValue()
		{
			return (int) value;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation performs casting to a {@code long}.
		/// </summary>
		public override long longValue()
		{
			return (long) value;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation performs casting to a {@code float}.
		/// </summary>
		public override float floatValue()
		{
			return (float) value;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double doubleValue()
		{
			return value;
		}

		/*
		 * Methods from the Comparable interface.
		 */

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation returns the same value as
		/// <center> {@code new Double(this.doubleValue()).compareTo(new
		/// Double(o.doubleValue()))} </center>
		/// </summary>
		/// <seealso cref= Double#compareTo(Double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compareTo(final Decimal64 o)
		public virtual int CompareTo(Decimal64 o)
		{
			return this.value.CompareTo(o.value);
		}

		/*
		 * Methods from the Object abstract class.
		 */

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object obj)
		public override bool Equals(object obj)
		{
			if (obj is Decimal64)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Decimal64 that = (Decimal64) obj;
				Decimal64 that = (Decimal64) obj;
				return double.doubleToLongBits(this.value) == double.doubleToLongBits(that.value);
			}
			return false;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The current implementation returns the same value as
		/// {@code new Double(this.doubleValue()).hashCode()}
		/// </summary>
		/// <seealso cref= Double#hashCode() </seealso>
		public override int GetHashCode()
		{
			long v = double.doubleToLongBits(value);
			return (int)(v ^ ((long)((ulong)v >> 32)));
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The returned {@code String} is equal to
		/// {@code Double.toString(this.doubleValue())}
		/// </summary>
		/// <seealso cref= Double#toString(double) </seealso>
		public override string ToString()
		{
			return Convert.ToString(value);
		}

		/*
		 * Methods inspired by the Double class.
		 */

		/// <summary>
		/// Returns {@code true} if {@code this} double precision number is infinite
		/// (<seealso cref="Double#POSITIVE_INFINITY"/> or <seealso cref="Double#NEGATIVE_INFINITY"/>).
		/// </summary>
		/// <returns> {@code true} if {@code this} number is infinite </returns>
		public virtual bool Infinite
		{
			get
			{
				return double.IsInfinity(value);
			}
		}

		/// <summary>
		/// Returns {@code true} if {@code this} double precision number is
		/// Not-a-Number ({@code NaN}), false otherwise.
		/// </summary>
		/// <returns> {@code true} if {@code this} is {@code NaN} </returns>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(value);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual double Real
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 add(final double a)
		public virtual Decimal64 add(double a)
		{
			return new Decimal64(value + a);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 subtract(final double a)
		public virtual Decimal64 subtract(double a)
		{
			return new Decimal64(value - a);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 multiply(final double a)
		public virtual Decimal64 multiply(double a)
		{
			return new Decimal64(value * a);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 divide(final double a)
		public virtual Decimal64 divide(double a)
		{
			return new Decimal64(value / a);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 remainder(final double a)
		public virtual Decimal64 remainder(double a)
		{
			return new Decimal64(FastMath.IEEEremainder(value, a));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 remainder(final Decimal64 a)
		public virtual Decimal64 remainder(Decimal64 a)
		{
			return new Decimal64(FastMath.IEEEremainder(value, a.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 abs()
		{
			return new Decimal64(FastMath.abs(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 ceil()
		{
			return new Decimal64(FastMath.ceil(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 floor()
		{
			return new Decimal64(FastMath.floor(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 rint()
		{
			return new Decimal64(FastMath.rint(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual long round()
		{
			return FastMath.round(value);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 signum()
		{
			return new Decimal64(FastMath.signum(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 copySign(final Decimal64 sign)
		public virtual Decimal64 copySign(Decimal64 sign)
		{
			return new Decimal64(FastMath.copySign(value, sign.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 copySign(final double sign)
		public virtual Decimal64 copySign(double sign)
		{
			return new Decimal64(FastMath.copySign(value, sign));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 scalb(final int n)
		public virtual Decimal64 scalb(int n)
		{
			return new Decimal64(FastMath.scalb(value, n));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 hypot(final Decimal64 y)
		public virtual Decimal64 hypot(Decimal64 y)
		{
			return new Decimal64(FastMath.hypot(value, y.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 sqrt()
		{
			return new Decimal64(FastMath.sqrt(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 cbrt()
		{
			return new Decimal64(FastMath.cbrt(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 rootN(final int n)
		public virtual Decimal64 rootN(int n)
		{
			if (value < 0)
			{
				return new Decimal64(-FastMath.pow(-value, 1.0 / n));
			}
			else
			{
				return new Decimal64(FastMath.pow(value, 1.0 / n));
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 pow(final double p)
		public virtual Decimal64 pow(double p)
		{
			return new Decimal64(FastMath.pow(value, p));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 pow(final int n)
		public virtual Decimal64 pow(int n)
		{
			return new Decimal64(FastMath.pow(value, n));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 pow(final Decimal64 e)
		public virtual Decimal64 pow(Decimal64 e)
		{
			return new Decimal64(FastMath.pow(value, e.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 exp()
		{
			return new Decimal64(FastMath.exp(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 expm1()
		{
			return new Decimal64(FastMath.expm1(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 log()
		{
			return new Decimal64(FastMath.log(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 log1p()
		{
			return new Decimal64(FastMath.log1p(value));
		}

		/// <summary>
		/// Base 10 logarithm. </summary>
		/// <returns> base 10 logarithm of the instance
		/// @since 3.2 </returns>
		public virtual Decimal64 log10()
		{
			return new Decimal64(FastMath.log10(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 cos()
		{
			return new Decimal64(FastMath.cos(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 sin()
		{
			return new Decimal64(FastMath.sin(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 tan()
		{
			return new Decimal64(FastMath.tan(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 acos()
		{
			return new Decimal64(FastMath.acos(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 asin()
		{
			return new Decimal64(FastMath.asin(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 atan()
		{
			return new Decimal64(FastMath.atan(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 atan2(final Decimal64 x)
		public virtual Decimal64 atan2(Decimal64 x)
		{
			return new Decimal64(FastMath.atan2(value, x.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 cosh()
		{
			return new Decimal64(FastMath.cosh(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 sinh()
		{
			return new Decimal64(FastMath.sinh(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 tanh()
		{
			return new Decimal64(FastMath.tanh(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 acosh()
		{
			return new Decimal64(FastMath.acosh(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 asinh()
		{
			return new Decimal64(FastMath.asinh(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
		public virtual Decimal64 atanh()
		{
			return new Decimal64(FastMath.atanh(value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final Decimal64[] a, final Decimal64[] b) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Decimal64 linearCombination(Decimal64[] a, Decimal64[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] aDouble = new double[a.length];
			double[] aDouble = new double[a.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bDouble = new double[b.length];
			double[] bDouble = new double[b.Length];
			for (int i = 0; i < a.Length; ++i)
			{
				aDouble[i] = a[i].value;
				bDouble[i] = b[i].value;
			}
			return new Decimal64(MathArrays.linearCombination(aDouble, bDouble));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final double[] a, final Decimal64[] b) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual Decimal64 linearCombination(double[] a, Decimal64[] b)
		{
			if (a.Length != b.Length)
			{
				throw new DimensionMismatchException(a.Length, b.Length);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bDouble = new double[b.length];
			double[] bDouble = new double[b.Length];
			for (int i = 0; i < a.Length; ++i)
			{
				bDouble[i] = b[i].value;
			}
			return new Decimal64(MathArrays.linearCombination(a, bDouble));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final Decimal64 a1, final Decimal64 b1, final Decimal64 a2, final Decimal64 b2)
		public virtual Decimal64 linearCombination(Decimal64 a1, Decimal64 b1, Decimal64 a2, Decimal64 b2)
		{
			return new Decimal64(MathArrays.linearCombination(a1.value, b1.value, a2.value, b2.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final double a1, final Decimal64 b1, final double a2, final Decimal64 b2)
		public virtual Decimal64 linearCombination(double a1, Decimal64 b1, double a2, Decimal64 b2)
		{
			return new Decimal64(MathArrays.linearCombination(a1, b1.value, a2, b2.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final Decimal64 a1, final Decimal64 b1, final Decimal64 a2, final Decimal64 b2, final Decimal64 a3, final Decimal64 b3)
		public virtual Decimal64 linearCombination(Decimal64 a1, Decimal64 b1, Decimal64 a2, Decimal64 b2, Decimal64 a3, Decimal64 b3)
		{
			return new Decimal64(MathArrays.linearCombination(a1.value, b1.value, a2.value, b2.value, a3.value, b3.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final double a1, final Decimal64 b1, final double a2, final Decimal64 b2, final double a3, final Decimal64 b3)
		public virtual Decimal64 linearCombination(double a1, Decimal64 b1, double a2, Decimal64 b2, double a3, Decimal64 b3)
		{
			return new Decimal64(MathArrays.linearCombination(a1, b1.value, a2, b2.value, a3, b3.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final Decimal64 a1, final Decimal64 b1, final Decimal64 a2, final Decimal64 b2, final Decimal64 a3, final Decimal64 b3, final Decimal64 a4, final Decimal64 b4)
		public virtual Decimal64 linearCombination(Decimal64 a1, Decimal64 b1, Decimal64 a2, Decimal64 b2, Decimal64 a3, Decimal64 b3, Decimal64 a4, Decimal64 b4)
		{
			return new Decimal64(MathArrays.linearCombination(a1.value, b1.value, a2.value, b2.value, a3.value, b3.value, a4.value, b4.value));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Decimal64 linearCombination(final double a1, final Decimal64 b1, final double a2, final Decimal64 b2, final double a3, final Decimal64 b3, final double a4, final Decimal64 b4)
		public virtual Decimal64 linearCombination(double a1, Decimal64 b1, double a2, Decimal64 b2, double a3, Decimal64 b3, double a4, Decimal64 b4)
		{
			return new Decimal64(MathArrays.linearCombination(a1, b1.value, a2, b2.value, a3, b3.value, a4, b4.value));
		}

	}

}