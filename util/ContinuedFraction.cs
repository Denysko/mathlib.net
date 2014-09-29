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

    using ConvergenceException = mathlib.exception.ConvergenceException;
    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Provides a generic means to evaluate continued fractions.  Subclasses simply
	/// provided the a and b coefficients to evaluate the continued fraction.
	/// 
	/// <p>
	/// References:
	/// <ul>
	/// <li><a href="http://mathworld.wolfram.com/ContinuedFraction.html">
	/// Continued Fraction</a></li>
	/// </ul>
	/// </p>
	/// 
	/// @version $Id: ContinuedFraction.java 1591835 2014-05-02 09:04:01Z tn $
	/// </summary>
	public abstract class ContinuedFraction
	{
		/// <summary>
		/// Maximum allowed numerical error. </summary>
		private const double DEFAULT_EPSILON = 10e-9;

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected internal ContinuedFraction() : base()
		{
		}

		/// <summary>
		/// Access the n-th a coefficient of the continued fraction.  Since a can be
		/// a function of the evaluation point, x, that is passed in as well. </summary>
		/// <param name="n"> the coefficient index to retrieve. </param>
		/// <param name="x"> the evaluation point. </param>
		/// <returns> the n-th a coefficient. </returns>
		protected internal abstract double getA(int n, double x);

		/// <summary>
		/// Access the n-th b coefficient of the continued fraction.  Since b can be
		/// a function of the evaluation point, x, that is passed in as well. </summary>
		/// <param name="n"> the coefficient index to retrieve. </param>
		/// <param name="x"> the evaluation point. </param>
		/// <returns> the n-th b coefficient. </returns>
		protected internal abstract double getB(int n, double x);

		/// <summary>
		/// Evaluates the continued fraction at the value x. </summary>
		/// <param name="x"> the evaluation point. </param>
		/// <returns> the value of the continued fraction evaluated at x. </returns>
		/// <exception cref="ConvergenceException"> if the algorithm fails to converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(double x) throws mathlib.exception.ConvergenceException
		public virtual double evaluate(double x)
		{
			return evaluate(x, DEFAULT_EPSILON, int.MaxValue);
		}

		/// <summary>
		/// Evaluates the continued fraction at the value x. </summary>
		/// <param name="x"> the evaluation point. </param>
		/// <param name="epsilon"> maximum error allowed. </param>
		/// <returns> the value of the continued fraction evaluated at x. </returns>
		/// <exception cref="ConvergenceException"> if the algorithm fails to converge. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(double x, double epsilon) throws mathlib.exception.ConvergenceException
		public virtual double evaluate(double x, double epsilon)
		{
			return evaluate(x, epsilon, int.MaxValue);
		}

		/// <summary>
		/// Evaluates the continued fraction at the value x. </summary>
		/// <param name="x"> the evaluation point. </param>
		/// <param name="maxIterations"> maximum number of convergents </param>
		/// <returns> the value of the continued fraction evaluated at x. </returns>
		/// <exception cref="ConvergenceException"> if the algorithm fails to converge. </exception>
		/// <exception cref="MaxCountExceededException"> if maximal number of iterations is reached </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(double x, int maxIterations) throws mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
		public virtual double evaluate(double x, int maxIterations)
		{
			return evaluate(x, DEFAULT_EPSILON, maxIterations);
		}

		/// <summary>
		/// Evaluates the continued fraction at the value x.
		/// <p>
		/// The implementation of this method is based on the modified Lentz algorithm as described
		/// on page 18 ff. in:
		/// <ul>
		///   <li>
		///   I. J. Thompson,  A. R. Barnett. "Coulomb and Bessel Functions of Complex Arguments and Order."
		///   <a target="_blank" href="http://www.fresco.org.uk/papers/Thompson-JCP64p490.pdf">
		///   http://www.fresco.org.uk/papers/Thompson-JCP64p490.pdf</a>
		///   </li>
		/// </ul>
		/// <b>Note:</b> the implementation uses the terms a<sub>i</sub> and b<sub>i</sub> as defined in
		/// <a href="http://mathworld.wolfram.com/ContinuedFraction.html">Continued Fraction @ MathWorld</a>.
		/// </p>
		/// </summary>
		/// <param name="x"> the evaluation point. </param>
		/// <param name="epsilon"> maximum error allowed. </param>
		/// <param name="maxIterations"> maximum number of convergents </param>
		/// <returns> the value of the continued fraction evaluated at x. </returns>
		/// <exception cref="ConvergenceException"> if the algorithm fails to converge. </exception>
		/// <exception cref="MaxCountExceededException"> if maximal number of iterations is reached </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double evaluate(double x, double epsilon, int maxIterations) throws mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
		public virtual double evaluate(double x, double epsilon, int maxIterations)
		{
			const double small = 1e-50;
			double hPrev = getA(0, x);

			// use the value of small as epsilon criteria for zero checks
			if (Precision.Equals(hPrev, 0.0, small))
			{
				hPrev = small;
			}

			int n = 1;
			double dPrev = 0.0;
			double cPrev = hPrev;
			double hN = hPrev;

			while (n < maxIterations)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = getA(n, x);
				double a = getA(n, x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = getB(n, x);
				double b = getB(n, x);

				double dN = a + b * dPrev;
				if (Precision.Equals(dN, 0.0, small))
				{
					dN = small;
				}
				double cN = a + b / cPrev;
				if (Precision.Equals(cN, 0.0, small))
				{
					cN = small;
				}

				dN = 1 / dN;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaN = cN * dN;
				double deltaN = cN * dN;
				hN = hPrev * deltaN;

				if (double.IsInfinity(hN))
				{
					throw new ConvergenceException(LocalizedFormats.CONTINUED_FRACTION_INFINITY_DIVERGENCE, x);
				}
				if (double.IsNaN(hN))
				{
					throw new ConvergenceException(LocalizedFormats.CONTINUED_FRACTION_NAN_DIVERGENCE, x);
				}

				if (FastMath.abs(deltaN - 1.0) < epsilon)
				{
					break;
				}

				dPrev = dN;
				cPrev = cN;
				hPrev = hN;
				n++;
			}

			if (n >= maxIterations)
			{
				throw new MaxCountExceededException(LocalizedFormats.NON_CONVERGENT_CONTINUED_FRACTION, maxIterations, x);
			}

			return hN;
		}

	}

}