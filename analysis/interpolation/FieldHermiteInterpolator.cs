using System.Collections.Generic;

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
namespace mathlib.analysis.interpolation
{


	using mathlib;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using ZeroException = mathlib.exception.ZeroException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathArrays = mathlib.util.MathArrays;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Polynomial interpolator using both sample values and sample derivatives.
	/// <p>
	/// The interpolation polynomials match all sample points, including both values
	/// and provided derivatives. There is one polynomial for each component of
	/// the values vector. All polynomials have the same degree. The degree of the
	/// polynomials depends on the number of points and number of derivatives at each
	/// point. For example the interpolation polynomials for n sample points without
	/// any derivatives all have degree n-1. The interpolation polynomials for n
	/// sample points with the two extreme points having value and first derivative
	/// and the remaining points having value only all have degree n+1. The
	/// interpolation polynomial for n sample points with value, first and second
	/// derivative for all points all have degree 3n-1.
	/// </p>
	/// </summary>
	/// @param <T> Type of the field elements.
	/// 
	/// @version $Id: FieldHermiteInterpolator.java 1455194 2013-03-11 15:45:54Z luc $
	/// @since 3.2 </param>
	public class FieldHermiteInterpolator<T> where T : mathlib.FieldElement<T>
	{

		/// <summary>
		/// Sample abscissae. </summary>
		private readonly IList<T> abscissae;

		/// <summary>
		/// Top diagonal of the divided differences array. </summary>
		private readonly IList<T[]> topDiagonal;

		/// <summary>
		/// Bottom diagonal of the divided differences array. </summary>
		private readonly IList<T[]> bottomDiagonal;

		/// <summary>
		/// Create an empty interpolator.
		/// </summary>
		public FieldHermiteInterpolator()
		{
			this.abscissae = new List<T>();
			this.topDiagonal = new List<T[]>();
			this.bottomDiagonal = new List<T[]>();
		}

		/// <summary>
		/// Add a sample point.
		/// <p>
		/// This method must be called once for each sample point. It is allowed to
		/// mix some calls with values only with calls with values and first
		/// derivatives.
		/// </p>
		/// <p>
		/// The point abscissae for all calls <em>must</em> be different.
		/// </p> </summary>
		/// <param name="x"> abscissa of the sample point </param>
		/// <param name="value"> value and derivatives of the sample point
		/// (if only one row is passed, it is the value, if two rows are
		/// passed the first one is the value and the second the derivative
		/// and so on) </param>
		/// <exception cref="ZeroException"> if the abscissa difference between added point
		/// and a previous point is zero (i.e. the two points are at same abscissa) </exception>
		/// <exception cref="MathArithmeticException"> if the number of derivatives is larger
		/// than 20, which prevents computation of a factorial </exception>
		/// <exception cref="DimensionMismatchException"> if derivative structures are inconsistent </exception>
		/// <exception cref="NullArgumentException"> if x is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addSamplePoint(final T x, final T[] ... value) throws mathlib.exception.ZeroException, mathlib.exception.MathArithmeticException, mathlib.exception.DimensionMismatchException, mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addSamplePoint(T x, params T[] [] value)
		{

			MathUtils.checkNotNull(x);
			T factorial = x.Field.One;
			for (int i = 0; i < value.Length; ++i)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] y = value[i].clone();
				T[] y = value[i].clone();
				if (i > 1)
				{
					factorial = factorial.multiply(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T inv = factorial.reciprocal();
					T inv = factorial.reciprocal();
					for (int j = 0; j < y.Length; ++j)
					{
						y[j] = y[j].multiply(inv);
					}
				}

				// update the bottom diagonal of the divided differences array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = abscissae.size();
				int n = abscissae.Count;
				bottomDiagonal.Insert(n - i, y);
				T[] bottom0 = y;
				for (int j = i; j < n; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] bottom1 = bottomDiagonal.get(n - (j + 1));
					T[] bottom1 = bottomDiagonal[n - (j + 1)];
					if (x.Equals(abscissae[n - (j + 1)]))
					{
						throw new ZeroException(LocalizedFormats.DUPLICATED_ABSCISSA_DIVISION_BY_ZERO, x);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T inv = x.subtract(abscissae.get(n - (j + 1))).reciprocal();
					T inv = x.subtract(abscissae[n - (j + 1)]).reciprocal();
					for (int k = 0; k < y.Length; ++k)
					{
						bottom1[k] = inv.multiply(bottom0[k].subtract(bottom1[k]));
					}
					bottom0 = bottom1;
				}

				// update the top diagonal of the divided differences array
				topDiagonal.Add(bottom0.clone());

				// update the abscissae array
				abscissae.Add(x);

			}

		}

		/// <summary>
		/// Interpolate value at a specified abscissa. </summary>
		/// <param name="x"> interpolation abscissa </param>
		/// <returns> interpolated value </returns>
		/// <exception cref="NoDataException"> if sample is empty </exception>
		/// <exception cref="NullArgumentException"> if x is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T[] value(T x) throws mathlib.exception.NoDataException, mathlib.exception.NullArgumentException
		public virtual T[] value(T x)
		{

			// safety check
			MathUtils.checkNotNull(x);
			if (abscissae.Count == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_INTERPOLATION_SAMPLE);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] value = mathlib.util.MathArrays.buildArray(x.getField(), topDiagonal.get(0).length);
			T[] value = MathArrays.buildArray(x.Field, topDiagonal[0].Length);
			T valueCoeff = x.Field.One;
			for (int i = 0; i < topDiagonal.Count; ++i)
			{
				T[] dividedDifference = topDiagonal[i];
				for (int k = 0; k < value.Length; ++k)
				{
					value[k] = value[k].add(dividedDifference[k].multiply(valueCoeff));
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T deltaX = x.subtract(abscissae.get(i));
				T deltaX = x.subtract(abscissae[i]);
				valueCoeff = valueCoeff.multiply(deltaX);
			}

			return value;

		}

		/// <summary>
		/// Interpolate value and first derivatives at a specified abscissa. </summary>
		/// <param name="x"> interpolation abscissa </param>
		/// <param name="order"> maximum derivation order </param>
		/// <returns> interpolated value and derivatives (value in row 0,
		/// 1<sup>st</sup> derivative in row 1, ... n<sup>th</sup> derivative in row n) </returns>
		/// <exception cref="NoDataException"> if sample is empty </exception>
		/// <exception cref="NullArgumentException"> if x is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T[][] derivatives(T x, int order) throws mathlib.exception.NoDataException, mathlib.exception.NullArgumentException
		public virtual T[][] derivatives(T x, int order)
		{

			// safety check
			MathUtils.checkNotNull(x);
			if (abscissae.Count == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_INTERPOLATION_SAMPLE);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T zero = x.getField().getZero();
			T zero = x.Field.Zero;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T one = x.getField().getOne();
			T one = x.Field.One;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] tj = mathlib.util.MathArrays.buildArray(x.getField(), order + 1);
			T[] tj = MathArrays.buildArray(x.Field, order + 1);
			tj[0] = zero;
			for (int i = 0; i < order; ++i)
			{
				tj[i + 1] = tj[i].add(one);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] derivatives = mathlib.util.MathArrays.buildArray(x.getField(), order + 1, topDiagonal.get(0).length);
			T[][] derivatives = MathArrays.buildArray(x.Field, order + 1, topDiagonal[0].Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] valueCoeff = mathlib.util.MathArrays.buildArray(x.getField(), order + 1);
			T[] valueCoeff = MathArrays.buildArray(x.Field, order + 1);
			valueCoeff[0] = x.Field.One;
			for (int i = 0; i < topDiagonal.Count; ++i)
			{
				T[] dividedDifference = topDiagonal[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T deltaX = x.subtract(abscissae.get(i));
				T deltaX = x.subtract(abscissae[i]);
				for (int j = order; j >= 0; --j)
				{
					for (int k = 0; k < derivatives[j].Length; ++k)
					{
						derivatives[j][k] = derivatives[j][k].add(dividedDifference[k].multiply(valueCoeff[j]));
					}
					valueCoeff[j] = valueCoeff[j].multiply(deltaX);
					if (j > 0)
					{
						valueCoeff[j] = valueCoeff[j].add(tj[j].multiply(valueCoeff[j - 1]));
					}
				}
			}

			return derivatives;

		}

	}

}