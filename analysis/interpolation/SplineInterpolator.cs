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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using PolynomialFunction = mathlib.analysis.polynomials.PolynomialFunction;
	using PolynomialSplineFunction = mathlib.analysis.polynomials.PolynomialSplineFunction;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// Computes a natural (also known as "free", "unclamped") cubic spline interpolation for the data set.
	/// <p>
	/// The <seealso cref="#interpolate(double[], double[])"/> method returns a <seealso cref="PolynomialSplineFunction"/>
	/// consisting of n cubic polynomials, defined over the subintervals determined by the x values,
	/// x[0] < x[i] ... < x[n].  The x values are referred to as "knot points."</p>
	/// <p>
	/// The value of the PolynomialSplineFunction at a point x that is greater than or equal to the smallest
	/// knot point and strictly less than the largest knot point is computed by finding the subinterval to which
	/// x belongs and computing the value of the corresponding polynomial at <code>x - x[i] </code> where
	/// <code>i</code> is the index of the subinterval.  See <seealso cref="PolynomialSplineFunction"/> for more details.
	/// </p>
	/// <p>
	/// The interpolating polynomials satisfy: <ol>
	/// <li>The value of the PolynomialSplineFunction at each of the input x values equals the
	///  corresponding y value.</li>
	/// <li>Adjacent polynomials are equal through two derivatives at the knot points (i.e., adjacent polynomials
	///  "match up" at the knot points, as do their first and second derivatives).</li>
	/// </ol></p>
	/// <p>
	/// The cubic spline interpolation algorithm implemented is as described in R.L. Burden, J.D. Faires,
	/// <u>Numerical Analysis</u>, 4th Ed., 1989, PWS-Kent, ISBN 0-53491-585-X, pp 126-131.
	/// </p>
	/// 
	/// @version $Id: SplineInterpolator.java 1379905 2012-09-01 23:56:50Z erans $
	/// </summary>
	public class SplineInterpolator : UnivariateInterpolator
	{
		/// <summary>
		/// Computes an interpolating function for the data set. </summary>
		/// <param name="x"> the arguments for the interpolation points </param>
		/// <param name="y"> the values for the interpolation points </param>
		/// <returns> a function which interpolates the data set </returns>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y}
		/// have different sizes. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if {@code x} is not sorted in
		/// strict increasing order. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the size of {@code x} is smaller
		/// than 3. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.analysis.polynomials.PolynomialSplineFunction interpolate(double x[] , double y[]) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NonMonotonicSequenceException
		public virtual PolynomialSplineFunction interpolate(double[] x, double[] y)
		{
			if (x.Length != y.Length)
			{
				throw new DimensionMismatchException(x.Length, y.Length);
			}

			if (x.Length < 3)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.NUMBER_OF_POINTS, x.Length, 3, true);
			}

			// Number of intervals.  The number of data points is n + 1.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length - 1;
			int n = x.Length - 1;

			MathArrays.checkOrder(x);

			// Differences between knot points
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double h[] = new double[n];
			double[] h = new double[n];
			for (int i = 0; i < n; i++)
			{
				h[i] = x[i + 1] - x[i];
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mu[] = new double[n];
			double[] mu = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z[] = new double[n + 1];
			double[] z = new double[n + 1];
			mu[0] = 0d;
			z[0] = 0d;
			double g = 0;
			for (int i = 1; i < n; i++)
			{
				g = 2d * (x[i + 1] - x[i - 1]) - h[i - 1] * mu[i - 1];
				mu[i] = h[i] / g;
				z[i] = (3d * (y[i + 1] * h[i - 1] - y[i] * (x[i + 1] - x[i - 1]) + y[i - 1] * h[i]) / (h[i - 1] * h[i]) - h[i - 1] * z[i - 1]) / g;
			}

			// cubic spline coefficients --  b is linear, c quadratic, d is cubic (original y's are constants)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b[] = new double[n];
			double[] b = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c[] = new double[n + 1];
			double[] c = new double[n + 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d[] = new double[n];
			double[] d = new double[n];

			z[n] = 0d;
			c[n] = 0d;

			for (int j = n - 1; j >= 0; j--)
			{
				c[j] = z[j] - mu[j] * c[j + 1];
				b[j] = (y[j + 1] - y[j]) / h[j] - h[j] * (c[j + 1] + 2d * c[j]) / 3d;
				d[j] = (c[j + 1] - c[j]) / (3d * h[j]);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.polynomials.PolynomialFunction polynomials[] = new mathlib.analysis.polynomials.PolynomialFunction[n];
			PolynomialFunction[] polynomials = new PolynomialFunction[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coefficients[] = new double[4];
			double[] coefficients = new double[4];
			for (int i = 0; i < n; i++)
			{
				coefficients[0] = y[i];
				coefficients[1] = b[i];
				coefficients[2] = c[i];
				coefficients[3] = d[i];
				polynomials[i] = new PolynomialFunction(coefficients);
			}

			return new PolynomialSplineFunction(x, polynomials);
		}
	}

}