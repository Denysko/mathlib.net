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
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Harmonic_oscillator">
	///  simple harmonic oscillator</a> function.
	/// 
	/// @since 3.0
	/// @version $Id: HarmonicOscillator.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class HarmonicOscillator : UnivariateDifferentiableFunction, DifferentiableUnivariateFunction
	{
		/// <summary>
		/// Amplitude. </summary>
		private readonly double amplitude;
		/// <summary>
		/// Angular frequency. </summary>
		private readonly double omega;
		/// <summary>
		/// Phase. </summary>
		private readonly double phase;

		/// <summary>
		/// Harmonic oscillator function.
		/// </summary>
		/// <param name="amplitude"> Amplitude. </param>
		/// <param name="omega"> Angular frequency. </param>
		/// <param name="phase"> Phase. </param>
		public HarmonicOscillator(double amplitude, double omega, double phase)
		{
			this.amplitude = amplitude;
			this.omega = omega;
			this.phase = phase;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			return value(omega * x + phase, amplitude);
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
		/// Parametric function where the input array contains the parameters of
		/// the harmonic oscillator function, ordered as follows:
		/// <ul>
		///  <li>Amplitude</li>
		///  <li>Angular frequency</li>
		///  <li>Phase</li>
		/// </ul>
		/// </summary>
		public class Parametric : ParametricUnivariateFunction
		{
			/// <summary>
			/// Computes the value of the harmonic oscillator at {@code x}.
			/// </summary>
			/// <param name="x"> Value for which the function must be computed. </param>
			/// <param name="param"> Values of norm, mean and standard deviation. </param>
			/// <returns> the value of the function. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 3. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double... param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
			public virtual double value(double x, params double[] param)
			{
				validateParameters(param);
				return HarmonicOscillator.value(x * param[1] + param[2], param[0]);
			}

			/// <summary>
			/// Computes the value of the gradient at {@code x}.
			/// The components of the gradient vector are the partial
			/// derivatives of the function with respect to each of the
			/// <em>parameters</em> (amplitude, angular frequency and phase).
			/// </summary>
			/// <param name="x"> Value at which the gradient must be computed. </param>
			/// <param name="param"> Values of amplitude, angular frequency and phase. </param>
			/// <returns> the gradient vector at {@code x}. </returns>
			/// <exception cref="NullArgumentException"> if {@code param} is {@code null}. </exception>
			/// <exception cref="DimensionMismatchException"> if the size of {@code param} is
			/// not 3. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] gradient(double x, double... param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
			public virtual double[] gradient(double x, params double[] param)
			{
				validateParameters(param);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double amplitude = param[0];
				double amplitude = param[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double omega = param[1];
				double omega = param[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double phase = param[2];
				double phase = param[2];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xTimesOmegaPlusPhase = omega * x + phase;
				double xTimesOmegaPlusPhase = omega * x + phase;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = HarmonicOscillator.value(xTimesOmegaPlusPhase, 1);
				double a = HarmonicOscillator.value(xTimesOmegaPlusPhase, 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = -amplitude * mathlib.util.FastMath.sin(xTimesOmegaPlusPhase);
				double p = -amplitude * FastMath.sin(xTimesOmegaPlusPhase);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = p * x;
				double w = p * x;

				return new double[] {a, w, p};
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
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void validateParameters(double[] param) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
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
			}
		}

		/// <param name="xTimesOmegaPlusPhase"> {@code omega * x + phase}. </param>
		/// <param name="amplitude"> Amplitude. </param>
		/// <returns> the value of the harmonic oscillator function at {@code x}. </returns>
		private static double value(double xTimesOmegaPlusPhase, double amplitude)
		{
			return amplitude * FastMath.cos(xTimesOmegaPlusPhase);
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = t.getValue();
			double x = t.Value;
			double[] f = new double[t.Order + 1];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = omega * x + phase;
			double alpha = omega * x + phase;
			f[0] = amplitude * FastMath.cos(alpha);
			if (f.Length > 1)
			{
				f[1] = -amplitude * omega * FastMath.sin(alpha);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mo2 = - omega * omega;
				double mo2 = - omega * omega;
				for (int i = 2; i < f.Length; ++i)
				{
					f[i] = mo2 * f[i - 2];
				}
			}

			return t.compose(f);

		}

	}

}