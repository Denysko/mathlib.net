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

	using PolynomialFunction = org.apache.commons.math3.analysis.polynomials.PolynomialFunction;
	using PolynomialSplineFunction = org.apache.commons.math3.analysis.polynomials.PolynomialSplineFunction;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Implements a linear function for interpolation of real univariate functions.
	/// 
	/// @version $Id: LinearInterpolator.java 1379904 2012-09-01 23:54:52Z erans $
	/// </summary>
	public class LinearInterpolator : UnivariateInterpolator
	{
		/// <summary>
		/// Computes a linear interpolating function for the data set.
		/// </summary>
		/// <param name="x"> the arguments for the interpolation points </param>
		/// <param name="y"> the values for the interpolation points </param>
		/// <returns> a function which interpolates the data set </returns>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y}
		/// have different sizes. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if {@code x} is not sorted in
		/// strict increasing order. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the size of {@code x} is smaller
		/// than 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.polynomials.PolynomialSplineFunction interpolate(double x[] , double y[]) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.NonMonotonicSequenceException
		public virtual PolynomialSplineFunction interpolate(double[] x, double[] y)
		{
			if (x.Length != y.Length)
			{
				throw new DimensionMismatchException(x.Length, y.Length);
			}

			if (x.Length < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.NUMBER_OF_POINTS, x.Length, 2, true);
			}

			// Number of intervals.  The number of data points is n + 1.
			int n = x.Length - 1;

			MathArrays.checkOrder(x);

			// Slope of the lines between the datapoints.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m[] = new double[n];
			double[] m = new double[n];
			for (int i = 0; i < n; i++)
			{
				m[i] = (y[i + 1] - y[i]) / (x[i + 1] - x[i]);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.polynomials.PolynomialFunction polynomials[] = new org.apache.commons.math3.analysis.polynomials.PolynomialFunction[n];
			PolynomialFunction[] polynomials = new PolynomialFunction[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coefficients[] = new double[2];
			double[] coefficients = new double[2];
			for (int i = 0; i < n; i++)
			{
				coefficients[0] = y[i];
				coefficients[1] = m[i];
				polynomials[i] = new PolynomialFunction(coefficients);
			}

			return new PolynomialSplineFunction(x, polynomials);
		}
	}

}