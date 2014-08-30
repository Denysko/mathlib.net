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
namespace org.apache.commons.math3.analysis.interpolation
{


	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableVectorFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableVectorFunction;
	using PolynomialFunction = org.apache.commons.math3.analysis.polynomials.PolynomialFunction;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using CombinatoricsUtils = org.apache.commons.math3.util.CombinatoricsUtils;

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
	/// 
	/// @version $Id: HermiteInterpolator.java 1517203 2013-08-24 21:55:35Z psteitz $
	/// @since 3.1
	/// </summary>
	public class HermiteInterpolator : UnivariateDifferentiableVectorFunction
	{

		/// <summary>
		/// Sample abscissae. </summary>
		private readonly IList<double?> abscissae;

		/// <summary>
		/// Top diagonal of the divided differences array. </summary>
		private readonly IList<double[]> topDiagonal;

		/// <summary>
		/// Bottom diagonal of the divided differences array. </summary>
		private readonly IList<double[]> bottomDiagonal;

		/// <summary>
		/// Create an empty interpolator.
		/// </summary>
		public HermiteInterpolator()
		{
			this.abscissae = new List<double?>();
			this.topDiagonal = new List<double[]>();
			this.bottomDiagonal = new List<double[]>();
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addSamplePoint(final double x, final double[] ... value) throws org.apache.commons.math3.exception.ZeroException, org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void addSamplePoint(double x, params double[] [] value)
		{

			for (int i = 0; i < value.Length; ++i)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = value[i].clone();
				double[] y = value[i].clone();
				if (i > 1)
				{
					double inv = 1.0 / CombinatoricsUtils.factorial(i);
					for (int j = 0; j < y.Length; ++j)
					{
						y[j] *= inv;
					}
				}

				// update the bottom diagonal of the divided differences array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = abscissae.size();
				int n = abscissae.Count;
				bottomDiagonal.Insert(n - i, y);
				double[] bottom0 = y;
				for (int j = i; j < n; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bottom1 = bottomDiagonal.get(n - (j + 1));
					double[] bottom1 = bottomDiagonal[n - (j + 1)];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inv = 1.0 / (x - abscissae.get(n - (j + 1)));
					double inv = 1.0 / (x - abscissae[n - (j + 1)]);
					if (double.IsInfinity(inv))
					{
						throw new ZeroException(LocalizedFormats.DUPLICATED_ABSCISSA_DIVISION_BY_ZERO, x);
					}
					for (int k = 0; k < y.Length; ++k)
					{
						bottom1[k] = inv * (bottom0[k] - bottom1[k]);
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
		/// Compute the interpolation polynomials. </summary>
		/// <returns> interpolation polynomials array </returns>
		/// <exception cref="NoDataException"> if sample is empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.polynomials.PolynomialFunction[] getPolynomials() throws org.apache.commons.math3.exception.NoDataException
		public virtual PolynomialFunction[] Polynomials
		{
			get
			{
    
				// safety check
				checkInterpolation();
    
				// iteration initialization
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.apache.commons.math3.analysis.polynomials.PolynomialFunction zero = polynomial(0);
				PolynomialFunction zero = polynomial(0);
				PolynomialFunction[] polynomials = new PolynomialFunction[topDiagonal[0].Length];
				for (int i = 0; i < polynomials.Length; ++i)
				{
					polynomials[i] = zero;
				}
				PolynomialFunction coeff = polynomial(1);
    
				// build the polynomials by iterating on the top diagonal of the divided differences array
				for (int i = 0; i < topDiagonal.Count; ++i)
				{
					double[] tdi = topDiagonal[i];
					for (int k = 0; k < polynomials.Length; ++k)
					{
						polynomials[k] = polynomials[k].add(coeff.multiply(polynomial(tdi[k])));
					}
					coeff = coeff.multiply(polynomial(-abscissae[i], 1.0));
				}
    
				return polynomials;
    
			}
		}

		/// <summary>
		/// Interpolate value at a specified abscissa.
		/// <p>
		/// Calling this method is equivalent to call the {@link PolynomialFunction#value(double)
		/// value} methods of all polynomials returned by <seealso cref="#getPolynomials() getPolynomials"/>,
		/// except it does not build the intermediate polynomials, so this method is faster and
		/// numerically more stable.
		/// </p> </summary>
		/// <param name="x"> interpolation abscissa </param>
		/// <returns> interpolated value </returns>
		/// <exception cref="NoDataException"> if sample is empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] value(double x) throws org.apache.commons.math3.exception.NoDataException
		public virtual double[] value(double x)
		{

			// safety check
			checkInterpolation();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] value = new double[topDiagonal.get(0).length];
			double[] value = new double[topDiagonal[0].Length];
			double valueCoeff = 1;
			for (int i = 0; i < topDiagonal.Count; ++i)
			{
				double[] dividedDifference = topDiagonal[i];
				for (int k = 0; k < value.Length; ++k)
				{
					value[k] += dividedDifference[k] * valueCoeff;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaX = x - abscissae.get(i);
				double deltaX = x - abscissae[i];
				valueCoeff *= deltaX;
			}

			return value;

		}

		/// <summary>
		/// Interpolate value at a specified abscissa.
		/// <p>
		/// Calling this method is equivalent to call the {@link
		/// PolynomialFunction#value(DerivativeStructure) value} methods of all polynomials
		/// returned by <seealso cref="#getPolynomials() getPolynomials"/>, except it does not build the
		/// intermediate polynomials, so this method is faster and numerically more stable.
		/// </p> </summary>
		/// <param name="x"> interpolation abscissa </param>
		/// <returns> interpolated value </returns>
		/// <exception cref="NoDataException"> if sample is empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure x) throws org.apache.commons.math3.exception.NoDataException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure[] value(DerivativeStructure x)
		{

			// safety check
			checkInterpolation();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure[] value = new org.apache.commons.math3.analysis.differentiation.DerivativeStructure[topDiagonal.get(0).length];
			DerivativeStructure[] value = new DerivativeStructure[topDiagonal[0].Length];
			Arrays.fill(value, x.Field.Zero);
			DerivativeStructure valueCoeff = x.Field.One;
			for (int i = 0; i < topDiagonal.Count; ++i)
			{
				double[] dividedDifference = topDiagonal[i];
				for (int k = 0; k < value.Length; ++k)
				{
					value[k] = value[k].add(valueCoeff.multiply(dividedDifference[k]));
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.differentiation.DerivativeStructure deltaX = x.subtract(abscissae.get(i));
				DerivativeStructure deltaX = x.subtract(abscissae[i]);
				valueCoeff = valueCoeff.multiply(deltaX);
			}

			return value;

		}

		/// <summary>
		/// Check interpolation can be performed. </summary>
		/// <exception cref="NoDataException"> if interpolation cannot be performed
		/// because sample is empty </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkInterpolation() throws org.apache.commons.math3.exception.NoDataException
		private void checkInterpolation()
		{
			if (abscissae.Count == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_INTERPOLATION_SAMPLE);
			}
		}

		/// <summary>
		/// Create a polynomial from its coefficients. </summary>
		/// <param name="c"> polynomials coefficients </param>
		/// <returns> polynomial </returns>
		private PolynomialFunction polynomial(params double[] c)
		{
			return new PolynomialFunction(c);
		}

	}

}