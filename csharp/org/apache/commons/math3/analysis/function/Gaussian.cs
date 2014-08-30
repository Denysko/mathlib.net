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
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Gaussian_function">
	///  Gaussian</a> function.
	/// 
	/// @since 3.0
	/// @version $Id: Gaussian.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class Gaussian : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Mean. </summary>
		private readonly double mean;
		/// <summary>
		/// Inverse of the standard deviation. </summary>
		private readonly double @is;
		/// <summary>
		/// Inverse of twice the square of the standard deviation. </summary>
		private readonly double i2s2;
		/// <summary>
		/// Normalization factor. </summary>
		private readonly double norm;

		/// <summary>
		/// Gaussian with given normalization factor, mean and standard deviation.
		/// </summary>
		/// <param name="norm"> Normalization factor. </param>
		/// <param name="mean"> Mean. </param>
		/// <param name="sigma"> Standard deviation. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sigma <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Gaussian(double norm, double mean, double sigma) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public Gaussian(double norm, double mean, double sigma)
		{
			if (sigma <= 0)
			{
				throw new NotStrictlyPositiveException(sigma);
			}

			this.norm = norm;
			this.mean = mean;
			this.@is = 1 / sigma;
			this.i2s2 = 0.5 * @is * @is;
		}

		/// <summary>
		/// Normalized gaussian with given mean and standard deviation.
		/// </summary>
		/// <param name="mean"> Mean. </param>
		/// <param name="sigma"> Standard deviation. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sigma <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Gaussian(double mean, double sigma) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public Gaussian(double mean, double sigma) : this(1 / (sigma * FastMath.sqrt(2 * Math.PI)), mean, sigma)
		{
		}

		/// <summary>
		/// Normalized gaussian with zero mean and unit standard deviation.
		/// </summary>
		public Gaussian() : this(0, 1)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return value(x - mean, norm, i2s2);
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
		/// the Gaussian, ordered as follows:
		/// <ul>
		///  <li>Norm</li>
		///  <li>Mean</li>
		///  <li>Standard deviation</li>
		/// </ul>
		/// </summary>
		public class Parametric : ParametricUnivariateFunction
		{
			/// <summary>
			/// Computes the value of the Gaussian at {@code x}.
			/// </summary>
			/// <param name="x"> Value for which the function must be computed. </param>
			/// <param name="param"> Values of norm, mean and standard deviation. </param>
			/// <returns> the value of the function. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 3. </exception>
			/// <exception cref="NotStrictlyPositiveException"> if {@code param[2]} is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double... param) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
			public virtual double value(double x, params double[] param)
			{
				validateParameters(param);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diff = x - param[1];
				double diff = x - param[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double i2s2 = 1 / (2 * param[2] * param[2]);
				double i2s2 = 1 / (2 * param[2] * param[2]);
				return Gaussian.value(diff, param[0], i2s2);
			}

			/// <summary>
			/// Computes the value of the gradient at {@code x}.
			/// The components of the gradient vector are the partial
			/// derivatives of the function with respect to each of the
			/// <em>parameters</em> (norm, mean and standard deviation).
			/// </summary>
			/// <param name="x"> Value at which the gradient must be computed. </param>
			/// <param name="param"> Values of norm, mean and standard deviation. </param>
			/// <returns> the gradient vector at {@code x}. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 3. </exception>
			/// <exception cref="NotStrictlyPositiveException"> if {@code param[2]} is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] gradient(double x, double... param) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
			public virtual double[] gradient(double x, params double[] param)
			{
				validateParameters(param);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm = param[0];
				double norm = param[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diff = x - param[1];
				double diff = x - param[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sigma = param[2];
				double sigma = param[2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double i2s2 = 1 / (2 * sigma * sigma);
				double i2s2 = 1 / (2 * sigma * sigma);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = Gaussian.value(diff, 1, i2s2);
				double n = Gaussian.value(diff, 1, i2s2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = norm * n * 2 * i2s2 * diff;
				double m = norm * n * 2 * i2s2 * diff;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = m * diff / sigma;
				double s = m * diff / sigma;

				return new double[] {n, m, s};
			}

			/// <summary>
			/// Validates parameters to ensure they are appropriate for the evaluation of
			/// the <seealso cref="#value(double,double[])"/> and <seealso cref="#gradient(double,double[])"/>
			/// methods.
			/// </summary>
			/// <param name="param"> Values of norm, mean and standard deviation. </param>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 3. </exception>
			/// <exception cref="NotStrictlyPositiveException"> if {@code param[2]} is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void validateParameters(double[] param) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
			internal virtual void validateParameters(double[] param)
			{
				if (param == null)
				{
					throw new NullArgumentException();
				}
				if (param.Length != 3)
				{
					throw new DimensionMismatchException(param.Length, 3);
				}
				if (param[2] <= 0)
				{
					throw new NotStrictlyPositiveException(param[2]);
				}
			}
		}

		/// <param name="xMinusMean"> {@code x - mean}. </param>
		/// <param name="norm"> Normalization factor. </param>
		/// <param name="i2s2"> Inverse of twice the square of the standard deviation. </param>
		/// <returns> the value of the Gaussian at {@code x}. </returns>
		private static double value(double xMinusMean, double norm, double i2s2)
		{
			return norm * FastMath.exp(-xMinusMean * xMinusMean * i2s2);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.analysis.differentiation.DerivativeStructure value(final org.apache.commons.math3.analysis.differentiation.DerivativeStructure t) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual DerivativeStructure value(DerivativeStructure t)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u = is * (t.getValue() - mean);
			double u = @is * (t.Value - mean);
			double[] f = new double[t.Order + 1];

			// the nth order derivative of the Gaussian has the form:
			// dn(g(x)/dxn = (norm / s^n) P_n(u) exp(-u^2/2) with u=(x-m)/s
			// where P_n(u) is a degree n polynomial with same parity as n
			// P_0(u) = 1, P_1(u) = -u, P_2(u) = u^2 - 1, P_3(u) = -u^3 + 3 u...
			// the general recurrence relation for P_n is:
			// P_n(u) = P_(n-1)'(u) - u P_(n-1)(u)
			// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[f.length];
			double[] p = new double[f.Length];
			p[0] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u2 = u * u;
			double u2 = u * u;
			double coeff = norm * FastMath.exp(-0.5 * u2);
			if (coeff <= Precision.SAFE_MIN)
			{
				Arrays.fill(f, 0.0);
			}
			else
			{
				f[0] = coeff;
				for (int n = 1; n < f.Length; ++n)
				{

					// update and evaluate polynomial P_n(x)
					double v = 0;
					p[n] = -p[n - 1];
					for (int k = n; k >= 0; k -= 2)
					{
						v = v * u2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (k - 1) * p[k - 1] - p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = p[1];
						}
					}
					if ((n & 0x1) == 1)
					{
						v *= u;
					}

					coeff *= @is;
					f[n] = coeff * v;

				}
			}

			return t.compose(f);

		}

	}

}