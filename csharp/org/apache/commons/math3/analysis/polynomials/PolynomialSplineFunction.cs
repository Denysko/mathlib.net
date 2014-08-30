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
namespace org.apache.commons.math3.analysis.polynomials
{

	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// Represents a polynomial spline function.
	/// <p>
	/// A <strong>polynomial spline function</strong> consists of a set of
	/// <i>interpolating polynomials</i> and an ascending array of domain
	/// <i>knot points</i>, determining the intervals over which the spline function
	/// is defined by the constituent polynomials.  The polynomials are assumed to
	/// have been computed to match the values of another function at the knot
	/// points.  The value consistency constraints are not currently enforced by
	/// <code>PolynomialSplineFunction</code> itself, but are assumed to hold among
	/// the polynomials and knot points passed to the constructor.</p>
	/// <p>
	/// N.B.:  The polynomials in the <code>polynomials</code> property must be
	/// centered on the knot points to compute the spline function values.
	/// See below.</p>
	/// <p>
	/// The domain of the polynomial spline function is
	/// <code>[smallest knot, largest knot]</code>.  Attempts to evaluate the
	/// function at values outside of this range generate IllegalArgumentExceptions.
	/// </p>
	/// <p>
	/// The value of the polynomial spline function for an argument <code>x</code>
	/// is computed as follows:
	/// <ol>
	/// <li>The knot array is searched to find the segment to which <code>x</code>
	/// belongs.  If <code>x</code> is less than the smallest knot point or greater
	/// than the largest one, an <code>IllegalArgumentException</code>
	/// is thrown.</li>
	/// <li> Let <code>j</code> be the index of the largest knot point that is less
	/// than or equal to <code>x</code>.  The value returned is <br>
	/// <code>polynomials[j](x - knot[j])</code></li></ol></p>
	/// 
	/// @version $Id: PolynomialSplineFunction.java 1491625 2013-06-10 22:22:31Z erans $
	/// </summary>
	public class PolynomialSplineFunction : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Spline segment interval delimiters (knots).
		/// Size is n + 1 for n segments.
		/// </summary>
		private readonly double[] knots;
		/// <summary>
		/// The polynomial functions that make up the spline.  The first element
		/// determines the value of the spline over the first subinterval, the
		/// second over the second, etc.   Spline function values are determined by
		/// evaluating these functions at {@code (x - knot[i])} where i is the
		/// knot segment to which x belongs.
		/// </summary>
		private readonly PolynomialFunction[] polynomials;
		/// <summary>
		/// Number of spline segments. It is equal to the number of polynomials and
		/// to the number of partition points - 1.
		/// </summary>
		private readonly int n;


		/// <summary>
		/// Construct a polynomial spline function with the given segment delimiters
		/// and interpolating polynomials.
		/// The constructor copies both arrays and assigns the copies to the knots
		/// and polynomials properties, respectively.
		/// </summary>
		/// <param name="knots"> Spline segment interval delimiters. </param>
		/// <param name="polynomials"> Polynomial functions that make up the spline. </param>
		/// <exception cref="NullArgumentException"> if either of the input arrays is {@code null}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if knots has length less than 2. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code polynomials.length != knots.length - 1}. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if the {@code knots} array is not strictly increasing.
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PolynomialSplineFunction(double knots[] , PolynomialFunction polynomials[]) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NonMonotonicSequenceException
		public PolynomialSplineFunction(double[] knots, PolynomialFunction[] polynomials)
		{
			if (knots == null || polynomials == null)
			{
				throw new NullArgumentException();
			}
			if (knots.Length < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.NOT_ENOUGH_POINTS_IN_SPLINE_PARTITION, 2, knots.Length, false);
			}
			if (knots.Length - 1 != polynomials.Length)
			{
				throw new DimensionMismatchException(polynomials.Length, knots.Length);
			}
			MathArrays.checkOrder(knots);

			this.n = knots.Length - 1;
			this.knots = new double[n + 1];
			Array.Copy(knots, 0, this.knots, 0, n + 1);
			this.polynomials = new PolynomialFunction[n];
			Array.Copy(polynomials, 0, this.polynomials, 0, n);
		}

		/// <summary>
		/// Compute the value for the function.
		/// See <seealso cref="PolynomialSplineFunction"/> for details on the algorithm for
		/// computing the value of the function.
		/// </summary>
		/// <param name="v"> Point for which the function value should be computed. </param>
		/// <returns> the value. </returns>
		/// <exception cref="OutOfRangeException"> if {@code v} is outside of the domain of the
		/// spline function (smaller than the smallest knot point or larger than the
		/// largest knot point). </exception>
		public virtual double value(double v)
		{
			if (v < knots[0] || v > knots[n])
			{
				throw new OutOfRangeException(v, knots[0], knots[n]);
			}
			int i = Arrays.binarySearch(knots, v);
			if (i < 0)
			{
				i = -i - 2;
			}
			// This will handle the case where v is the last knot value
			// There are only n-1 polynomials, so if v is the last knot
			// then we will use the last polynomial to calculate the value.
			if (i >= polynomials.Length)
			{
				i--;
			}
			return polynomials[i].value(v - knots[i]);
		}

		/// <summary>
		/// Get the derivative of the polynomial spline function.
		/// </summary>
		/// <returns> the derivative function. </returns>
		public virtual UnivariateFunction derivative()
		{
			return polynomialSplineDerivative();
		}

		/// <summary>
		/// Get the derivative of the polynomial spline function.
		/// </summary>
		/// <returns> the derivative function. </returns>
		public virtual PolynomialSplineFunction polynomialSplineDerivative()
		{
			PolynomialFunction[] derivativePolynomials = new PolynomialFunction[n];
			for (int i = 0; i < n; i++)
			{
				derivativePolynomials[i] = polynomials[i].polynomialDerivative();
			}
			return new PolynomialSplineFunction(knots, derivativePolynomials);
		}


		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t)
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t0 = t.getValue();
			double t0 = t.Value;
			if (t0 < knots[0] || t0 > knots[n])
			{
				throw new OutOfRangeException(t0, knots[0], knots[n]);
			}
			int i = Arrays.binarySearch(knots, t0);
			if (i < 0)
			{
				i = -i - 2;
			}
			// This will handle the case where t is the last knot value
			// There are only n-1 polynomials, so if t is the last knot
			// then we will use the last polynomial to calculate the value.
			if (i >= polynomials.Length)
			{
				i--;
			}
			return polynomials[i].value(t.subtract(knots[i]));
		}

		/// <summary>
		/// Get the number of spline segments.
		/// It is also the number of polynomials and the number of knot points - 1.
		/// </summary>
		/// <returns> the number of spline segments. </returns>
		public virtual int N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Get a copy of the interpolating polynomials array.
		/// It returns a fresh copy of the array. Changes made to the copy will
		/// not affect the polynomials property.
		/// </summary>
		/// <returns> the interpolating polynomials. </returns>
		public virtual PolynomialFunction[] Polynomials
		{
			get
			{
				PolynomialFunction[] p = new PolynomialFunction[n];
				Array.Copy(polynomials, 0, p, 0, n);
				return p;
			}
		}

		/// <summary>
		/// Get an array copy of the knot points.
		/// It returns a fresh copy of the array. Changes made to the copy
		/// will not affect the knots property.
		/// </summary>
		/// <returns> the knot points. </returns>
		public virtual double[] Knots
		{
			get
			{
				double[] @out = new double[n + 1];
				Array.Copy(knots, 0, @out, 0, n + 1);
				return @out;
			}
		}

		/// <summary>
		/// Indicates whether a point is within the interpolation range.
		/// </summary>
		/// <param name="x"> Point. </param>
		/// <returns> {@code true} if {@code x} is a valid point. </returns>
		public virtual bool isValidPoint(double x)
		{
			if (x < knots[0] || x > knots[n])
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}

}