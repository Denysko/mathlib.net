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

namespace org.apache.commons.math3.analysis.function
{

	using DerivativeStructure = org.apache.commons.math3.analysis.differentiation.DerivativeStructure;
	using UnivariateDifferentiableFunction = org.apache.commons.math3.analysis.differentiation.UnivariateDifferentiableFunction;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Generalised_logistic_function">
	///  Generalised logistic</a> function.
	/// 
	/// @since 3.0
	/// @version $Id: Logistic.java 1547633 2013-12-03 23:03:06Z tn $
	/// </summary>
	public class Logistic : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Lower asymptote. </summary>
		private readonly double a;
		/// <summary>
		/// Upper asymptote. </summary>
		private readonly double k;
		/// <summary>
		/// Growth rate. </summary>
		private readonly double b;
		/// <summary>
		/// Parameter that affects near which asymptote maximum growth occurs. </summary>
		private readonly double oneOverN;
		/// <summary>
		/// Parameter that affects the position of the curve along the ordinate axis. </summary>
		private readonly double q;
		/// <summary>
		/// Abscissa of maximum growth. </summary>
		private readonly double m;

		/// <param name="k"> If {@code b > 0}, value of the function for x going towards +&infin;.
		/// If {@code b < 0}, value of the function for x going towards -&infin;. </param>
		/// <param name="m"> Abscissa of maximum growth. </param>
		/// <param name="b"> Growth rate. </param>
		/// <param name="q"> Parameter that affects the position of the curve along the
		/// ordinate axis. </param>
		/// <param name="a"> If {@code b > 0}, value of the function for x going towards -&infin;.
		/// If {@code b < 0}, value of the function for x going towards +&infin;. </param>
		/// <param name="n"> Parameter that affects near which asymptote the maximum
		/// growth occurs. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code n <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Logistic(double k, double m, double b, double q, double a, double n) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public Logistic(double k, double m, double b, double q, double a, double n)
		{
			if (n <= 0)
			{
				throw new NotStrictlyPositiveException(n);
			}

			this.k = k;
			this.m = m;
			this.b = b;
			this.q = q;
			this.a = a;
			oneOverN = 1 / n;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return value(m - x, k, b, q, a, oneOverN);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.1, replaced by <seealso cref="#value(DerivativeStructure)"/> 
		[Obsolete("as of 3.1, replaced by <seealso cref="#value(org.apache.commons.math3.analysis.differentiation.DerivativeStructure)"/>")]
		public virtual UnivariateFunction derivative()
		{
			return FunctionUtils.toDifferentiableUnivariateFunction(this).derivative();
		}

		/// <summary>
		/// Parametric function where the input array contains the parameters of
		/// the {@link Logistic#Logistic(double,double,double,double,double,double)
		/// logistic function}, ordered as follows:
		/// <ul>
		///  <li>k</li>
		///  <li>m</li>
		///  <li>b</li>
		///  <li>q</li>
		///  <li>a</li>
		///  <li>n</li>
		/// </ul>
		/// </summary>
		public class Parametric : ParametricUnivariateFunction
		{
			/// <summary>
			/// Computes the value of the sigmoid at {@code x}.
			/// </summary>
			/// <param name="x"> Value for which the function must be computed. </param>
			/// <param name="param"> Values for {@code k}, {@code m}, {@code b}, {@code q},
			/// {@code a} and  {@code n}. </param>
			/// <returns> the value of the function. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 6. </exception>
			/// <exception cref="NotStrictlyPositiveException"> if {@code param[5] <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double... param) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
			public virtual double value(double x, params double[] param)
			{
				validateParameters(param);
				return Logistic.value(param[1] - x, param[0], param[2], param[3], param[4], 1 / param[5]);
			}

			/// <summary>
			/// Computes the value of the gradient at {@code x}.
			/// The components of the gradient vector are the partial
			/// derivatives of the function with respect to each of the
			/// <em>parameters</em>.
			/// </summary>
			/// <param name="x"> Value at which the gradient must be computed. </param>
			/// <param name="param"> Values for {@code k}, {@code m}, {@code b}, {@code q},
			/// {@code a} and  {@code n}. </param>
			/// <returns> the gradient vector at {@code x}. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 6. </exception>
			/// <exception cref="NotStrictlyPositiveException"> if {@code param[5] <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] gradient(double x, double... param) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
			public virtual double[] gradient(double x, params double[] param)
			{
				validateParameters(param);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = param[2];
				double b = param[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q = param[3];
				double q = param[3];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mMinusX = param[1] - x;
				double mMinusX = param[1] - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oneOverN = 1 / param[5];
				double oneOverN = 1 / param[5];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double exp = org.apache.commons.math3.util.FastMath.exp(b * mMinusX);
				double exp = FastMath.exp(b * mMinusX);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double qExp = q * exp;
				double qExp = q * exp;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double qExp1 = qExp + 1;
				double qExp1 = qExp + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor1 = (param[0] - param[4]) * oneOverN / org.apache.commons.math3.util.FastMath.pow(qExp1, oneOverN);
				double factor1 = (param[0] - param[4]) * oneOverN / FastMath.pow(qExp1, oneOverN);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor2 = -factor1 / qExp1;
				double factor2 = -factor1 / qExp1;

				// Components of the gradient.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gk = Logistic.value(mMinusX, 1, b, q, 0, oneOverN);
				double gk = Logistic.value(mMinusX, 1, b, q, 0, oneOverN);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gm = factor2 * b * qExp;
				double gm = factor2 * b * qExp;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gb = factor2 * mMinusX * qExp;
				double gb = factor2 * mMinusX * qExp;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gq = factor2 * exp;
				double gq = factor2 * exp;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ga = Logistic.value(mMinusX, 0, b, q, 1, oneOverN);
				double ga = Logistic.value(mMinusX, 0, b, q, 1, oneOverN);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gn = factor1 * org.apache.commons.math3.util.FastMath.log(qExp1) * oneOverN;
				double gn = factor1 * FastMath.log(qExp1) * oneOverN;

				return new double[] {gk, gm, gb, gq, ga, gn};
			}

			/// <summary>
			/// Validates parameters to ensure they are appropriate for the evaluation of
			/// the <seealso cref="#value(double,double[])"/> and <seealso cref="#gradient(double,double[])"/>
			/// methods.
			/// </summary>
			/// <param name="param"> Values for {@code k}, {@code m}, {@code b}, {@code q},
			/// {@code a} and {@code n}. </param>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 6. </exception>
			/// <exception cref="NotStrictlyPositiveException"> if {@code param[5] <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void validateParameters(double[] param) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
			internal virtual void validateParameters(double[] param)
			{
				if (param == null)
				{
					throw new NullArgumentException();
				}
				if (param.Length != 6)
				{
					throw new DimensionMismatchException(param.Length, 6);
				}
				if (param[5] <= 0)
				{
					throw new NotStrictlyPositiveException(param[5]);
				}
			}
		}

		/// <param name="mMinusX"> {@code m - x}. </param>
		/// <param name="k"> {@code k}. </param>
		/// <param name="b"> {@code b}. </param>
		/// <param name="q"> {@code q}. </param>
		/// <param name="a"> {@code a}. </param>
		/// <param name="oneOverN"> {@code 1 / n}. </param>
		/// <returns> the value of the function. </returns>
		private static double value(double mMinusX, double k, double b, double q, double a, double oneOverN)
		{
			return a + (k - a) / FastMath.pow(1 + q * FastMath.exp(b * mMinusX), oneOverN);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t)
		public virtual DerivativeStructure value(DerivativeStructure t)
		{
			return t.negate().add(m).multiply(b).exp().multiply(q).add(1).pow(oneOverN).reciprocal().multiply(k - a).add(a);
		}

	}

}