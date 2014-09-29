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
namespace mathlib.analysis.polynomials
{

	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Implements the representation of a real polynomial function in
	/// <a href="http://mathworld.wolfram.com/LagrangeInterpolatingPolynomial.html">
	/// Lagrange Form</a>. For reference, see <b>Introduction to Numerical
	/// Analysis</b>, ISBN 038795452X, chapter 2.
	/// <p>
	/// The approximated function should be smooth enough for Lagrange polynomial
	/// to work well. Otherwise, consider using splines instead.</p>
	/// 
	/// @version $Id: PolynomialFunctionLagrangeForm.java 1455194 2013-03-11 15:45:54Z luc $
	/// @since 1.2
	/// </summary>
	public class PolynomialFunctionLagrangeForm : UnivariateFunction
	{
		/// <summary>
		/// The coefficients of the polynomial, ordered by degree -- i.e.
		/// coefficients[0] is the constant term and coefficients[n] is the
		/// coefficient of x^n where n is the degree of the polynomial.
		/// </summary>
		private double[] coefficients;
		/// <summary>
		/// Interpolating points (abscissas).
		/// </summary>
		private readonly double[] x;
		/// <summary>
		/// Function values at interpolating points.
		/// </summary>
		private readonly double[] y;
		/// <summary>
		/// Whether the polynomial coefficients are available.
		/// </summary>
		private bool coefficientsComputed;

		/// <summary>
		/// Construct a Lagrange polynomial with the given abscissas and function
		/// values. The order of interpolating points are not important.
		/// <p>
		/// The constructor makes copy of the input arrays and assigns them.</p>
		/// </summary>
		/// <param name="x"> interpolating points </param>
		/// <param name="y"> function values at interpolating points </param>
		/// <exception cref="DimensionMismatchException"> if the array lengths are different. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of points is less than 2. </exception>
		/// <exception cref="NonMonotonicSequenceException">
		/// if two abscissae have the same value. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PolynomialFunctionLagrangeForm(double x[] , double y[]) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NonMonotonicSequenceException
		public PolynomialFunctionLagrangeForm(double[] x, double[] y)
		{
			this.x = new double[x.Length];
			this.y = new double[y.Length];
			Array.Copy(x, 0, this.x, 0, x.Length);
			Array.Copy(y, 0, this.y, 0, y.Length);
			coefficientsComputed = false;

			if (!verifyInterpolationArray(x, y, false))
			{
				MathArrays.sortInPlace(this.x, this.y);
				// Second check in case some abscissa is duplicated.
				verifyInterpolationArray(this.x, this.y, true);
			}
		}

		/// <summary>
		/// Calculate the function value at the given point.
		/// </summary>
		/// <param name="z"> Point at which the function value is to be computed. </param>
		/// <returns> the function value. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} have
		/// different lengths. </exception>
		/// <exception cref="mathlib.exception.NonMonotonicSequenceException">
		/// if {@code x} is not sorted in strictly increasing order. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the size of {@code x} is less
		/// than 2. </exception>
		public virtual double value(double z)
		{
			return evaluateInternal(x, y, z);
		}

		/// <summary>
		/// Returns the degree of the polynomial.
		/// </summary>
		/// <returns> the degree of the polynomial </returns>
		public virtual int degree()
		{
			return x.Length - 1;
		}

		/// <summary>
		/// Returns a copy of the interpolating points array.
		/// <p>
		/// Changes made to the returned copy will not affect the polynomial.</p>
		/// </summary>
		/// <returns> a fresh copy of the interpolating points array </returns>
		public virtual double[] InterpolatingPoints
		{
			get
			{
				double[] @out = new double[x.Length];
				Array.Copy(x, 0, @out, 0, x.Length);
				return @out;
			}
		}

		/// <summary>
		/// Returns a copy of the interpolating values array.
		/// <p>
		/// Changes made to the returned copy will not affect the polynomial.</p>
		/// </summary>
		/// <returns> a fresh copy of the interpolating values array </returns>
		public virtual double[] InterpolatingValues
		{
			get
			{
				double[] @out = new double[y.Length];
				Array.Copy(y, 0, @out, 0, y.Length);
				return @out;
			}
		}

		/// <summary>
		/// Returns a copy of the coefficients array.
		/// <p>
		/// Changes made to the returned copy will not affect the polynomial.</p>
		/// <p>
		/// Note that coefficients computation can be ill-conditioned. Use with caution
		/// and only when it is necessary.</p>
		/// </summary>
		/// <returns> a fresh copy of the coefficients array </returns>
		public virtual double[] Coefficients
		{
			get
			{
				if (!coefficientsComputed)
				{
					computeCoefficients();
				}
				double[] @out = new double[coefficients.Length];
				Array.Copy(coefficients, 0, @out, 0, coefficients.Length);
				return @out;
			}
		}

		/// <summary>
		/// Evaluate the Lagrange polynomial using
		/// <a href="http://mathworld.wolfram.com/NevillesAlgorithm.html">
		/// Neville's Algorithm</a>. It takes O(n^2) time.
		/// </summary>
		/// <param name="x"> Interpolating points array. </param>
		/// <param name="y"> Interpolating values array. </param>
		/// <param name="z"> Point at which the function value is to be computed. </param>
		/// <returns> the function value. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} have
		/// different lengths. </exception>
		/// <exception cref="NonMonotonicSequenceException">
		/// if {@code x} is not sorted in strictly increasing order. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the size of {@code x} is less
		/// than 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double evaluate(double x[] , double y[], double z) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NonMonotonicSequenceException
		public static double evaluate(double[] x, double[] y, double z)
		{
			if (verifyInterpolationArray(x, y, false))
			{
				return evaluateInternal(x, y, z);
			}

			// Array is not sorted.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xNew = new double[x.length];
			double[] xNew = new double[x.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yNew = new double[y.length];
			double[] yNew = new double[y.Length];
			Array.Copy(x, 0, xNew, 0, x.Length);
			Array.Copy(y, 0, yNew, 0, y.Length);

			MathArrays.sortInPlace(xNew, yNew);
			// Second check in case some abscissa is duplicated.
			verifyInterpolationArray(xNew, yNew, true);
			return evaluateInternal(xNew, yNew, z);
		}

		/// <summary>
		/// Evaluate the Lagrange polynomial using
		/// <a href="http://mathworld.wolfram.com/NevillesAlgorithm.html">
		/// Neville's Algorithm</a>. It takes O(n^2) time.
		/// </summary>
		/// <param name="x"> Interpolating points array. </param>
		/// <param name="y"> Interpolating values array. </param>
		/// <param name="z"> Point at which the function value is to be computed. </param>
		/// <returns> the function value. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} have
		/// different lengths. </exception>
		/// <exception cref="mathlib.exception.NonMonotonicSequenceException">
		/// if {@code x} is not sorted in strictly increasing order. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the size of {@code x} is less
		/// than 2. </exception>
		private static double evaluateInternal(double[] x, double[] y, double z)
		{
			int nearest = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = x.length;
			int n = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] c = new double[n];
			double[] c = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] d = new double[n];
			double[] d = new double[n];
			double min_dist = double.PositiveInfinity;
			for (int i = 0; i < n; i++)
			{
				// initialize the difference arrays
				c[i] = y[i];
				d[i] = y[i];
				// find out the abscissa closest to z
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dist = mathlib.util.FastMath.abs(z - x[i]);
				double dist = FastMath.abs(z - x[i]);
				if (dist < min_dist)
				{
					nearest = i;
					min_dist = dist;
				}
			}

			// initial approximation to the function value at z
			double value = y[nearest];

			for (int i = 1; i < n; i++)
			{
				for (int j = 0; j < n - i; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tc = x[j] - z;
					double tc = x[j] - z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double td = x[i+j] - z;
					double td = x[i + j] - z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double divider = x[j] - x[i+j];
					double divider = x[j] - x[i + j];
					// update the difference arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = (c[j+1] - d[j]) / divider;
					double w = (c[j + 1] - d[j]) / divider;
					c[j] = tc * w;
					d[j] = td * w;
				}
				// sum up the difference terms to get the final value
				if (nearest < 0.5 * (n - i + 1))
				{
					value += c[nearest]; // fork down
				}
				else
				{
					nearest--;
					value += d[nearest]; // fork up
				}
			}

			return value;
		}

		/// <summary>
		/// Calculate the coefficients of Lagrange polynomial from the
		/// interpolation data. It takes O(n^2) time.
		/// Note that this computation can be ill-conditioned: Use with caution
		/// and only when it is necessary.
		/// </summary>
		protected internal virtual void computeCoefficients()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = degree() + 1;
			int n = degree() + 1;
			coefficients = new double[n];
			for (int i = 0; i < n; i++)
			{
				coefficients[i] = 0.0;
			}

			// c[] are the coefficients of P(x) = (x-x[0])(x-x[1])...(x-x[n-1])
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] c = new double[n+1];
			double[] c = new double[n + 1];
			c[0] = 1.0;
			for (int i = 0; i < n; i++)
			{
				for (int j = i; j > 0; j--)
				{
					c[j] = c[j - 1] - c[j] * x[i];
				}
				c[0] *= -x[i];
				c[i + 1] = 1;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tc = new double[n];
			double[] tc = new double[n];
			for (int i = 0; i < n; i++)
			{
				// d = (x[i]-x[0])...(x[i]-x[i-1])(x[i]-x[i+1])...(x[i]-x[n-1])
				double d = 1;
				for (int j = 0; j < n; j++)
				{
					if (i != j)
					{
						d *= x[i] - x[j];
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = y[i] / d;
				double t = y[i] / d;
				// Lagrange polynomial is the sum of n terms, each of which is a
				// polynomial of degree n-1. tc[] are the coefficients of the i-th
				// numerator Pi(x) = (x-x[0])...(x-x[i-1])(x-x[i+1])...(x-x[n-1]).
				tc[n - 1] = c[n]; // actually c[n] = 1
				coefficients[n - 1] += t * tc[n - 1];
				for (int j = n - 2; j >= 0; j--)
				{
					tc[j] = c[j + 1] + tc[j + 1] * x[i];
					coefficients[j] += t * tc[j];
				}
			}

			coefficientsComputed = true;
		}

		/// <summary>
		/// Check that the interpolation arrays are valid.
		/// The arrays features checked by this method are that both arrays have the
		/// same length and this length is at least 2.
		/// </summary>
		/// <param name="x"> Interpolating points array. </param>
		/// <param name="y"> Interpolating values array. </param>
		/// <param name="abort"> Whether to throw an exception if {@code x} is not sorted. </param>
		/// <exception cref="DimensionMismatchException"> if the array lengths are different. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of points is less than 2. </exception>
		/// <exception cref="mathlib.exception.NonMonotonicSequenceException">
		/// if {@code x} is not sorted in strictly increasing order and {@code abort}
		/// is {@code true}. </exception>
		/// <returns> {@code false} if the {@code x} is not sorted in increasing order,
		/// {@code true} otherwise. </returns>
		/// <seealso cref= #evaluate(double[], double[], double) </seealso>
		/// <seealso cref= #computeCoefficients() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean verifyInterpolationArray(double x[] , double y[], boolean abort) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NonMonotonicSequenceException
		public static bool verifyInterpolationArray(double[] x, double[] y, bool abort)
		{
			if (x.Length != y.Length)
			{
				throw new DimensionMismatchException(x.Length, y.Length);
			}
			if (x.Length < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.WRONG_NUMBER_OF_POINTS, 2, x.Length, true);
			}

			return MathArrays.checkOrder(x, MathArrays.OrderDirection.INCREASING, true, abort);
		}
	}

}