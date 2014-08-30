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
namespace org.apache.commons.math3.analysis.interpolation
{

	using PolynomialFunctionLagrangeForm = org.apache.commons.math3.analysis.polynomials.PolynomialFunctionLagrangeForm;
	using PolynomialFunctionNewtonForm = org.apache.commons.math3.analysis.polynomials.PolynomialFunctionNewtonForm;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;

	/// <summary>
	/// Implements the <a href="
	/// http://mathworld.wolfram.com/NewtonsDividedDifferenceInterpolationFormula.html">
	/// Divided Difference Algorithm</a> for interpolation of real univariate
	/// functions. For reference, see <b>Introduction to Numerical Analysis</b>,
	/// ISBN 038795452X, chapter 2.
	/// <p>
	/// The actual code of Neville's evaluation is in PolynomialFunctionLagrangeForm,
	/// this class provides an easy-to-use interface to it.</p>
	/// 
	/// @version $Id: DividedDifferenceInterpolator.java 1385313 2012-09-16 16:35:23Z tn $
	/// @since 1.2
	/// </summary>
	[Serializable]
	public class DividedDifferenceInterpolator : UnivariateInterpolator
	{
		/// <summary>
		/// serializable version identifier </summary>
		private const long serialVersionUID = 107049519551235069L;

		/// <summary>
		/// Compute an interpolating function for the dataset.
		/// </summary>
		/// <param name="x"> Interpolating points array. </param>
		/// <param name="y"> Interpolating values array. </param>
		/// <returns> a function which interpolates the dataset. </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths are different. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of points is less than 2. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if {@code x} is not sorted in
		/// strictly increasing order. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.polynomials.PolynomialFunctionNewtonForm interpolate(double x[] , double y[]) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.NonMonotonicSequenceException
		public virtual PolynomialFunctionNewtonForm interpolate(double[] x, double[] y)
		{
			/// <summary>
			/// a[] and c[] are defined in the general formula of Newton form:
			/// p(x) = a[0] + a[1](x-c[0]) + a[2](x-c[0])(x-c[1]) + ... +
			///        a[n](x-c[0])(x-c[1])...(x-c[n-1])
			/// </summary>
			PolynomialFunctionLagrangeForm.verifyInterpolationArray(x, y, true);

			/// <summary>
			/// When used for interpolation, the Newton form formula becomes
			/// p(x) = f[x0] + f[x0,x1](x-x0) + f[x0,x1,x2](x-x0)(x-x1) + ... +
			///        f[x0,x1,...,x[n-1]](x-x0)(x-x1)...(x-x[n-2])
			/// Therefore, a[k] = f[x0,x1,...,xk], c[k] = x[k].
			/// <p>
			/// Note x[], y[], a[] have the same length but c[]'s size is one less.</p>
			/// </summary>
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] c = new double[x.length-1];
			double[] c = new double[x.Length - 1];
			Array.Copy(x, 0, c, 0, c.Length);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] a = computeDividedDifference(x, y);
			double[] a = computeDividedDifference(x, y);
			return new PolynomialFunctionNewtonForm(a, c);
		}

		/// <summary>
		/// Return a copy of the divided difference array.
		/// <p>
		/// The divided difference array is defined recursively by <pre>
		/// f[x0] = f(x0)
		/// f[x0,x1,...,xk] = (f[x1,...,xk] - f[x0,...,x[k-1]]) / (xk - x0)
		/// </pre></p>
		/// <p>
		/// The computational complexity is O(N^2).</p>
		/// </summary>
		/// <param name="x"> Interpolating points array. </param>
		/// <param name="y"> Interpolating values array. </param>
		/// <returns> a fresh copy of the divided difference array. </returns>
		/// <exception cref="DimensionMismatchException"> if the array lengths are different. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of points is less than 2. </exception>
		/// <exception cref="NonMonotonicSequenceException">
		/// if {@code x} is not sorted in strictly increasing order. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static double[] computeDividedDifference(final double x[], final double y[]) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.NonMonotonicSequenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal static double[] computeDividedDifference(double[] x, double[] y)
		{
			PolynomialFunctionLagrangeForm.verifyInterpolationArray(x, y, true);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] divdiff = y.clone();
			double[] divdiff = y.clone(); // initialization

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
			int n = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] a = new double [n];
			double[] a = new double [n];
			a[0] = divdiff[0];
			for (int i = 1; i < n; i++)
			{
				for (int j = 0; j < n - i; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double denominator = x[j+i] - x[j];
					double denominator = x[j + i] - x[j];
					divdiff[j] = (divdiff[j + 1] - divdiff[j]) / denominator;
				}
				a[i] = divdiff[0];
			}

			return a;
		}
	}

}