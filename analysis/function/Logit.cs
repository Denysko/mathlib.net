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
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Logit">
	///  Logit</a> function.
	/// It is the inverse of the <seealso cref="Sigmoid sigmoid"/> function.
	/// 
	/// @since 3.0
	/// @version $Id: Logit.java 1391927 2012-09-30 00:03:30Z erans $
	/// </summary>
	public class Logit : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Lower bound. </summary>
		private readonly double lo;
		/// <summary>
		/// Higher bound. </summary>
		private readonly double hi;

		/// <summary>
		/// Usual logit function, where the lower bound is 0 and the higher
		/// bound is 1.
		/// </summary>
		public Logit() : this(0, 1)
		{
		}

		/// <summary>
		/// Logit function.
		/// </summary>
		/// <param name="lo"> Lower bound of the function domain. </param>
		/// <param name="hi"> Higher bound of the function domain. </param>
		public Logit(double lo, double hi)
		{
			this.lo = lo;
			this.hi = hi;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x) throws mathlib.exception.OutOfRangeException
		public virtual double value(double x)
		{
			return value(x, lo, hi);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete]//("as of 3.1, replaced by <seealso cref="#value(mathlib.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual UnivariateFunction derivative()
		{
			return FunctionUtils.toDifferentiableUnivariateFunction(this).derivative();
		}

		/// <summary>
		/// Parametric function where the input array contains the parameters of
		/// the logit function, ordered as follows:
		/// <ul>
		///  <li>Lower bound</li>
		///  <li>Higher bound</li>
		/// </ul>
		/// </summary>
		public class Parametric : ParametricUnivariateFunction
		{
			/// <summary>
			/// Computes the value of the logit at {@code x}.
			/// </summary>
			/// <param name="x"> Value for which the function must be computed. </param>
			/// <param name="param"> Values of lower bound and higher bounds. </param>
			/// <returns> the value of the function. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 2. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double... param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
			public virtual double value(double x, params double[] param)
			{
				validateParameters(param);
				return Logit.value(x, param[0], param[1]);
			}

			/// <summary>
			/// Computes the value of the gradient at {@code x}.
			/// The components of the gradient vector are the partial
			/// derivatives of the function with respect to each of the
			/// <em>parameters</em> (lower bound and higher bound).
			/// </summary>
			/// <param name="x"> Value at which the gradient must be computed. </param>
			/// <param name="param"> Values for lower and higher bounds. </param>
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
//ORIGINAL LINE: final double lo = param[0];
				double lo = param[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hi = param[1];
				double hi = param[1];

				return new double[] {1 / (lo - x), 1 / (hi - x)};
			}

			/// <summary>
			/// Validates parameters to ensure they are appropriate for the evaluation of
			/// the <seealso cref="#value(double,double[])"/> and <seealso cref="#gradient(double,double[])"/>
			/// methods.
			/// </summary>
			/// <param name="param"> Values for lower and higher bounds. </param>
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

		/// <param name="x"> Value at which to compute the logit. </param>
		/// <param name="lo"> Lower bound. </param>
		/// <param name="hi"> Higher bound. </param>
		/// <returns> the value of the logit function at {@code x}. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x < lo} or {@code x > hi}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static double value(double x, double lo, double hi) throws mathlib.exception.OutOfRangeException
		private static double value(double x, double lo, double hi)
		{
			if (x < lo || x > hi)
			{
				throw new OutOfRangeException(x, lo, hi);
			}
			return FastMath.log((x - lo) / (hi - x));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1 </summary>
		/// <exception cref="OutOfRangeException"> if parameter is outside of function domain </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public mathlib.analysis.differentiation.DerivativeStructure value(final mathlib.analysis.differentiation.DerivativeStructure t) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = t.getValue();
			double x = t.Value;
			if (x < lo || x > hi)
			{
				throw new OutOfRangeException(x, lo, hi);
			}
			double[] f = new double[t.Order + 1];

			// function value
			f[0] = FastMath.log((x - lo) / (hi - x));

			if (double.IsInfinity(f[0]))
			{

				if (f.Length > 1)
				{
					f[1] = double.PositiveInfinity;
				}
				// fill the array with infinities
				// (for x close to lo the signs will flip between -inf and +inf,
				//  for x close to hi the signs will always be +inf)
				// this is probably overkill, since the call to compose at the end
				// of the method will transform most infinities into NaN ...
				for (int i = 2; i < f.Length; ++i)
				{
					f[i] = f[i - 2];
				}

			}
			else
			{

				// function derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double invL = 1.0 / (x - lo);
				double invL = 1.0 / (x - lo);
				double xL = invL;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double invH = 1.0 / (hi - x);
				double invH = 1.0 / (hi - x);
				double xH = invH;
				for (int i = 1; i < f.Length; ++i)
				{
					f[i] = xL + xH;
					xL *= -i * invL;
					xH *= i * invH;
				}
			}

			return t.compose(f);
		}
	}

}