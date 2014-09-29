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

	using DerivativeStructure = mathlib.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = mathlib.analysis.differentiation.UnivariateDifferentiableFunction;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Implements the representation of a real polynomial function in
	/// Newton Form. For reference, see <b>Elementary Numerical Analysis</b>,
	/// ISBN 0070124477, chapter 2.
	/// <p>
	/// The formula of polynomial in Newton form is
	///     p(x) = a[0] + a[1](x-c[0]) + a[2](x-c[0])(x-c[1]) + ... +
	///            a[n](x-c[0])(x-c[1])...(x-c[n-1])
	/// Note that the length of a[] is one more than the length of c[]</p>
	/// 
	/// @version $Id: PolynomialFunctionNewtonForm.java 1455194 2013-03-11 15:45:54Z luc $
	/// @since 1.2
	/// </summary>
	public class PolynomialFunctionNewtonForm : UnivariateDifferentiableFunction
	{

		/// <summary>
		/// The coefficients of the polynomial, ordered by degree -- i.e.
		/// coefficients[0] is the constant term and coefficients[n] is the
		/// coefficient of x^n where n is the degree of the polynomial.
		/// </summary>
		private double[] coefficients;

		/// <summary>
		/// Centers of the Newton polynomial.
		/// </summary>
		private readonly double[] c;

		/// <summary>
		/// When all c[i] = 0, a[] becomes normal polynomial coefficients,
		/// i.e. a[i] = coefficients[i].
		/// </summary>
		private readonly double[] a;

		/// <summary>
		/// Whether the polynomial coefficients are available.
		/// </summary>
		private bool coefficientsComputed;

		/// <summary>
		/// Construct a Newton polynomial with the given a[] and c[]. The order of
		/// centers are important in that if c[] shuffle, then values of a[] would
		/// completely change, not just a permutation of old a[].
		/// <p>
		/// The constructor makes copy of the input arrays and assigns them.</p>
		/// </summary>
		/// <param name="a"> Coefficients in Newton form formula. </param>
		/// <param name="c"> Centers. </param>
		/// <exception cref="NullArgumentException"> if any argument is {@code null}. </exception>
		/// <exception cref="NoDataException"> if any array has zero length. </exception>
		/// <exception cref="DimensionMismatchException"> if the size difference between
		/// {@code a} and {@code c} is not equal to 1. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PolynomialFunctionNewtonForm(double a[] , double c[]) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException
		public PolynomialFunctionNewtonForm(double[] a, double[] c)
		{

			verifyInputArray(a, c);
			this.a = new double[a.Length];
			this.c = new double[c.Length];
			Array.Copy(a, 0, this.a, 0, a.Length);
			Array.Copy(c, 0, this.c, 0, c.Length);
			coefficientsComputed = false;
		}

		/// <summary>
		/// Calculate the function value at the given point.
		/// </summary>
		/// <param name="z"> Point at which the function value is to be computed. </param>
		/// <returns> the function value. </returns>
		public virtual double value(double z)
		{
		   return evaluate(a, c, z);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.analysis.differentiation.DerivativeStructure value(final mathlib.analysis.differentiation.DerivativeStructure t)
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
			verifyInputArray(a, c);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = c.length;
			int n = c.Length;
			DerivativeStructure value = new DerivativeStructure(t.FreeParameters, t.Order, a[n]);
			for (int i = n - 1; i >= 0; i--)
			{
				value = t.subtract(c[i]).multiply(value).add(a[i]);
			}

			return value;

		}

		/// <summary>
		/// Returns the degree of the polynomial.
		/// </summary>
		/// <returns> the degree of the polynomial </returns>
		public virtual int degree()
		{
			return c.Length;
		}

		/// <summary>
		/// Returns a copy of coefficients in Newton form formula.
		/// <p>
		/// Changes made to the returned copy will not affect the polynomial.</p>
		/// </summary>
		/// <returns> a fresh copy of coefficients in Newton form formula </returns>
		public virtual double[] NewtonCoefficients
		{
			get
			{
				double[] @out = new double[a.Length];
				Array.Copy(a, 0, @out, 0, a.Length);
				return @out;
			}
		}

		/// <summary>
		/// Returns a copy of the centers array.
		/// <p>
		/// Changes made to the returned copy will not affect the polynomial.</p>
		/// </summary>
		/// <returns> a fresh copy of the centers array. </returns>
		public virtual double[] Centers
		{
			get
			{
				double[] @out = new double[c.Length];
				Array.Copy(c, 0, @out, 0, c.Length);
				return @out;
			}
		}

		/// <summary>
		/// Returns a copy of the coefficients array.
		/// <p>
		/// Changes made to the returned copy will not affect the polynomial.</p>
		/// </summary>
		/// <returns> a fresh copy of the coefficients array. </returns>
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
		/// Evaluate the Newton polynomial using nested multiplication. It is
		/// also called <a href="http://mathworld.wolfram.com/HornersRule.html">
		/// Horner's Rule</a> and takes O(N) time.
		/// </summary>
		/// <param name="a"> Coefficients in Newton form formula. </param>
		/// <param name="c"> Centers. </param>
		/// <param name="z"> Point at which the function value is to be computed. </param>
		/// <returns> the function value. </returns>
		/// <exception cref="NullArgumentException"> if any argument is {@code null}. </exception>
		/// <exception cref="NoDataException"> if any array has zero length. </exception>
		/// <exception cref="DimensionMismatchException"> if the size difference between
		/// {@code a} and {@code c} is not equal to 1. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double evaluate(double a[] , double c[], double z) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.NoDataException
		public static double evaluate(double[] a, double[] c, double z)
		{
			verifyInputArray(a, c);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = c.length;
			int n = c.Length;
			double value = a[n];
			for (int i = n - 1; i >= 0; i--)
			{
				value = a[i] + (z - c[i]) * value;
			}

			return value;
		}

		/// <summary>
		/// Calculate the normal polynomial coefficients given the Newton form.
		/// It also uses nested multiplication but takes O(N^2) time.
		/// </summary>
		protected internal virtual void computeCoefficients()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = degree();
			int n = degree();

			coefficients = new double[n + 1];
			for (int i = 0; i <= n; i++)
			{
				coefficients[i] = 0.0;
			}

			coefficients[0] = a[n];
			for (int i = n - 1; i >= 0; i--)
			{
				for (int j = n - i; j > 0; j--)
				{
					coefficients[j] = coefficients[j - 1] - c[i] * coefficients[j];
				}
				coefficients[0] = a[i] - c[i] * coefficients[0];
			}

			coefficientsComputed = true;
		}

		/// <summary>
		/// Verifies that the input arrays are valid.
		/// <p>
		/// The centers must be distinct for interpolation purposes, but not
		/// for general use. Thus it is not verified here.</p>
		/// </summary>
		/// <param name="a"> the coefficients in Newton form formula </param>
		/// <param name="c"> the centers </param>
		/// <exception cref="NullArgumentException"> if any argument is {@code null}. </exception>
		/// <exception cref="NoDataException"> if any array has zero length. </exception>
		/// <exception cref="DimensionMismatchException"> if the size difference between
		/// {@code a} and {@code c} is not equal to 1. </exception>
		/// <seealso cref= mathlib.analysis.interpolation.DividedDifferenceInterpolator#computeDividedDifference(double[],
		/// double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static void verifyInputArray(double a[] , double c[]) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException
		protected internal static void verifyInputArray(double[] a, double[] c)
		{
			MathUtils.checkNotNull(a);
			MathUtils.checkNotNull(c);
			if (a.Length == 0 || c.Length == 0)
			{
				throw new NoDataException(LocalizedFormats.EMPTY_POLYNOMIALS_COEFFICIENTS_ARRAY);
			}
			if (a.Length != c.Length + 1)
			{
				throw new DimensionMismatchException(LocalizedFormats.ARRAY_SIZES_SHOULD_HAVE_DIFFERENCE_1, a.Length, c.Length);
			}
		}

	}

}