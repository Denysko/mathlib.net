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

namespace mathlib.analysis.function
{

	using DerivativeStructure = mathlib.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = mathlib.analysis.differentiation.UnivariateDifferentiableFunction;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Sigmoid_function">
	///  Sigmoid</a> function.
	/// It is the inverse of the <seealso cref="Logit logit"/> function.
	/// A more flexible version, the generalised logistic, is implemented
	/// by the <seealso cref="Logistic"/> class.
	/// 
	/// @since 3.0
	/// @version $Id: Sigmoid.java 1513430 2013-08-13 10:46:48Z erans $
	/// </summary>
	public class Sigmoid : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Lower asymptote. </summary>
		private readonly double lo;
		/// <summary>
		/// Higher asymptote. </summary>
		private readonly double hi;

		/// <summary>
		/// Usual sigmoid function, where the lower asymptote is 0 and the higher
		/// asymptote is 1.
		/// </summary>
		public Sigmoid() : this(0, 1)
		{
		}

		/// <summary>
		/// Sigmoid function.
		/// </summary>
		/// <param name="lo"> Lower asymptote. </param>
		/// <param name="hi"> Higher asymptote. </param>
		public Sigmoid(double lo, double hi)
		{
			this.lo = lo;
			this.hi = hi;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#value(mathlib.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual UnivariateFunction derivative()
		{
			return FunctionUtils.toDifferentiableUnivariateFunction(this).derivative();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return value(x, lo, hi);
		}

		/// <summary>
		/// Parametric function where the input array contains the parameters of
		/// the <seealso cref="Sigmoid#Sigmoid(double,double) sigmoid function"/>, ordered
		/// as follows:
		/// <ul>
		///  <li>Lower asymptote</li>
		///  <li>Higher asymptote</li>
		/// </ul>
		/// </summary>
		public class Parametric : ParametricUnivariateFunction
		{
			/// <summary>
			/// Computes the value of the sigmoid at {@code x}.
			/// </summary>
			/// <param name="x"> Value for which the function must be computed. </param>
			/// <param name="param"> Values of lower asymptote and higher asymptote. </param>
			/// <returns> the value of the function. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double... param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
			public virtual double value(double x, params double[] param)
			{
				validateParameters(param);
				return Sigmoid.value(x, param[0], param[1]);
			}

			/// <summary>
			/// Computes the value of the gradient at {@code x}.
			/// The components of the gradient vector are the partial
			/// derivatives of the function with respect to each of the
			/// <em>parameters</em> (lower asymptote and higher asymptote).
			/// </summary>
			/// <param name="x"> Value at which the gradient must be computed. </param>
			/// <param name="param"> Values for lower asymptote and higher asymptote. </param>
			/// <returns> the gradient vector at {@code x}. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] gradient(double x, double... param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
			public virtual double[] gradient(double x, params double[] param)
			{
				validateParameters(param);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double invExp1 = 1 / (1 + mathlib.util.FastMath.exp(-x));
				double invExp1 = 1 / (1 + FastMath.exp(-x));

				return new double[] {1 - invExp1, invExp1};
			}

			/// <summary>
			/// Validates parameters to ensure they are appropriate for the evaluation of
			/// the <seealso cref="#value(double,double[])"/> and <seealso cref="#gradient(double,double[])"/>
			/// methods.
			/// </summary>
			/// <param name="param"> Values for lower and higher asymptotes. </param>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void validateParameters(double[] param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
			internal virtual void validateParameters(double[] param)
			{
				if (param == null)
				{
					throw new NullArgumentException();
				}
				if (param.Length != 2)
				{
					throw new DimensionMismatchException(param.Length, 2);
				}
			}
		}

		/// <param name="x"> Value at which to compute the sigmoid. </param>
		/// <param name="lo"> Lower asymptote. </param>
		/// <param name="hi"> Higher asymptote. </param>
		/// <returns> the value of the sigmoid function at {@code x}. </returns>
		private static double value(double x, double lo, double hi)
		{
			return lo + (hi - lo) / (1 + FastMath.exp(-x));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.analysis.differentiation.DerivativeStructure value(final mathlib.analysis.differentiation.DerivativeStructure t) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure value(DerivativeStructure t)
		{

			double[] f = new double[t.Order + 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double exp = mathlib.util.FastMath.exp(-t.getValue());
			double exp = FastMath.exp(-t.Value);
			if (double.IsInfinity(exp))
			{

				// special handling near lower boundary, to avoid NaN
				f[0] = lo;
				Arrays.fill(f, 1, f.Length, 0.0);

			}
			else
			{

				// the nth order derivative of sigmoid has the form:
				// dn(sigmoid(x)/dxn = P_n(exp(-x)) / (1+exp(-x))^(n+1)
				// where P_n(t) is a degree n polynomial with normalized higher term
				// P_0(t) = 1, P_1(t) = t, P_2(t) = t^2 - t, P_3(t) = t^3 - 4 t^2 + t...
				// the general recurrence relation for P_n is:
				// P_n(x) = n t P_(n-1)(t) - t (1 + t) P_(n-1)'(t)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[f.length];
				double[] p = new double[f.Length];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inv = 1 / (1 + exp);
				double inv = 1 / (1 + exp);
				double coeff = hi - lo;
				for (int n = 0; n < f.Length; ++n)
				{

					// update and evaluate polynomial P_n(t)
					double v = 0;
					p[n] = 1;
					for (int k = n; k >= 0; --k)
					{
						v = v * exp + p[k];
						if (k > 1)
						{
							p[k - 1] = (n - k + 2) * p[k - 2] - (k - 1) * p[k - 1];
						}
						else
						{
							p[0] = 0;
						}
					}

					coeff *= inv;
					f[n] = coeff * v;

				}

				// fix function value
				f[0] += lo;

			}

			return t.compose(f);

		}

	}

}