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

namespace mathlib.distribution
{

	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the Pareto distribution.
	/// 
	/// <p>
	/// <strong>Parameters:</strong>
	/// The probability distribution function of {@code X} is given by (for {@code x >= k}):
	/// <pre>
	///  α * k^α / x^(α + 1)
	/// </pre>
	/// <p>
	/// <ul>
	/// <li>{@code k} is the <em>scale</em> parameter: this is the minimum possible value of {@code X},</li>
	/// <li>{@code α} is the <em>shape</em> parameter: this is the Pareto index</li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Pareto_distribution">
	/// Pareto distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/ParetoDistribution.html">
	/// Pareto distribution (MathWorld)</a>
	/// 
	/// @version $Id: ParetoDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $
	/// @since 3.3 </seealso>
	public class ParetoDistribution : AbstractRealDistribution
	{

		/// <summary>
		/// Default inverse cumulative probability accuracy. </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20130424;

		/// <summary>
		/// The scale parameter of this distribution. </summary>
		private readonly double scale;

		/// <summary>
		/// The shape parameter of this distribution. </summary>
		private readonly double shape;

		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Create a Pareto distribution with a scale of {@code 1} and a shape of {@code 1}.
		/// </summary>
		public ParetoDistribution() : this(1, 1)
		{
		}

		/// <summary>
		/// Create a Pareto distribution using the specified scale and shape.
		/// </summary>
		/// <param name="scale"> the scale parameter of this distribution </param>
		/// <param name="shape"> the shape parameter of this distribution </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0} or {@code shape <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParetoDistribution(double scale, double shape) throws mathlib.exception.NotStrictlyPositiveException
		public ParetoDistribution(double scale, double shape) : this(scale, shape, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create a Pareto distribution using the specified scale, shape and
		/// inverse cumulative distribution accuracy.
		/// </summary>
		/// <param name="scale"> the scale parameter of this distribution </param>
		/// <param name="shape"> the shape parameter of this distribution </param>
		/// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0} or {@code shape <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParetoDistribution(double scale, double shape, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public ParetoDistribution(double scale, double shape, double inverseCumAccuracy) : this(new Well19937c(), scale, shape, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a log-normal distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="scale"> Scale parameter of this distribution. </param>
		/// <param name="shape"> Shape parameter of this distribution. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0} or {@code shape <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParetoDistribution(mathlib.random.RandomGenerator rng, double scale, double shape) throws mathlib.exception.NotStrictlyPositiveException
		public ParetoDistribution(RandomGenerator rng, double scale, double shape) : this(rng, scale, shape, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a log-normal distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="scale"> Scale parameter of this distribution. </param>
		/// <param name="shape"> Shape parameter of this distribution. </param>
		/// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0} or {@code shape <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ParetoDistribution(mathlib.random.RandomGenerator rng, double scale, double shape, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public ParetoDistribution(RandomGenerator rng, double scale, double shape, double inverseCumAccuracy) : base(rng)
		{

			if (scale <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SCALE, scale);
			}

			if (shape <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SHAPE, shape);
			}

			this.scale = scale;
			this.shape = shape;
			this.solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Returns the scale parameter of this distribution.
		/// </summary>
		/// <returns> the scale parameter </returns>
		public virtual double Scale
		{
			get
			{
				return scale;
			}
		}

		/// <summary>
		/// Returns the shape parameter of this distribution.
		/// </summary>
		/// <returns> the shape parameter </returns>
		public virtual double Shape
		{
			get
			{
				return shape;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// For scale {@code k}, and shape {@code α} of this distribution, the PDF
		/// is given by
		/// <ul>
		/// <li>{@code 0} if {@code x < k},</li>
		/// <li>{@code α * k^α / x^(α + 1)} otherwise.</li>
		/// </ul>
		/// </summary>
		public override double density(double x)
		{
			if (x < scale)
			{
				return 0;
			}
			return FastMath.pow(scale, shape) / FastMath.pow(x, shape + 1) * shape;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// See documentation of <seealso cref="#density(double)"/> for computation details.
		/// </summary>
		public override double logDensity(double x)
		{
			if (x < scale)
			{
				return double.NegativeInfinity;
			}
			return FastMath.log(scale) * shape - FastMath.log(x) * (shape + 1) + FastMath.log(shape);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// For scale {@code k}, and shape {@code α} of this distribution, the CDF is given by
		/// <ul>
		/// <li>{@code 0} if {@code x < k},</li>
		/// <li>{@code 1 - (k / x)^α} otherwise.</li>
		/// </ul>
		/// </summary>
		public override double cumulativeProbability(double x)
		{
			if (x <= scale)
			{
				return 0;
			}
			return 1 - FastMath.pow(scale / x, shape);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// @deprecated See <seealso cref="RealDistribution#cumulativeProbability(double,double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override @Deprecated public double cumulativeProbability(double x0, double x1) throws mathlib.exception.NumberIsTooLargeException
		[Obsolete]
		public override double cumulativeProbability(double x0, double x1)
		{
			return probability(x0, x1);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override double SolverAbsoluteAccuracy
		{
			get
			{
				return solverAbsoluteAccuracy;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// For scale {@code k} and shape {@code α}, the mean is given by
		/// <ul>
		/// <li>{@code ∞} if {@code α <= 1},</li>
		/// <li>{@code α * k / (α - 1)} otherwise.</li>
		/// </ul>
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				if (shape <= 1)
				{
					return double.PositiveInfinity;
				}
				return shape * scale / (shape - 1);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// For scale {@code k} and shape {@code α}, the variance is given by
		/// <ul>
		/// <li>{@code ∞} if {@code 1 < α <= 2},</li>
		/// <li>{@code k^2 * α / ((α - 1)^2 * (α - 2))} otherwise.</li>
		/// </ul>
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				if (shape <= 2)
				{
					return double.PositiveInfinity;
				}
				double s = shape - 1;
				return scale * scale * shape / (s * s) / (shape - 2);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// The lower bound of the support is equal to the scale parameter {@code k}.
		/// </summary>
		/// <returns> lower bound of the support </returns>
		public override double SupportLowerBound
		{
			get
			{
				return scale;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// The upper bound of the support is always positive infinity no matter the parameters.
		/// </summary>
		/// <returns> upper bound of the support (always {@code Double.POSITIVE_INFINITY}) </returns>
		public override double SupportUpperBound
		{
			get
			{
				return double.PositiveInfinity;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportLowerBoundInclusive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportUpperBoundInclusive
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// The support of this distribution is connected.
		/// </summary>
		/// <returns> {@code true} </returns>
		public override bool SupportConnected
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double sample()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = random.nextDouble();
			double n = random.NextDouble();
			return scale / FastMath.pow(n, 1 / shape);
		}
	}

}