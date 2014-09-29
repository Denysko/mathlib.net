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

	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the chi-squared distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Chi-squared_distribution">Chi-squared distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/Chi-SquaredDistribution.html">Chi-squared Distribution (MathWorld)</a>
	/// @version $Id: ChiSquaredDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $ </seealso>
	public class ChiSquaredDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -8352658048349159782L;
		/// <summary>
		/// Internal Gamma distribution. </summary>
		private readonly GammaDistribution gamma;
		/// <summary>
		/// Inverse cumulative probability accuracy </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Create a Chi-Squared distribution with the given degrees of freedom.
		/// </summary>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		public ChiSquaredDistribution(double degreesOfFreedom) : this(degreesOfFreedom, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create a Chi-Squared distribution with the given degrees of freedom and
		/// inverse cumulative probability accuracy.
		/// </summary>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>).
		/// @since 2.1 </param>
		public ChiSquaredDistribution(double degreesOfFreedom, double inverseCumAccuracy) : this(new Well19937c(), degreesOfFreedom, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Create a Chi-Squared distribution with the given degrees of freedom.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="degreesOfFreedom"> Degrees of freedom.
		/// @since 3.3 </param>
		public ChiSquaredDistribution(RandomGenerator rng, double degreesOfFreedom) : this(rng, degreesOfFreedom, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create a Chi-Squared distribution with the given degrees of freedom and
		/// inverse cumulative probability accuracy.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>).
		/// @since 3.1 </param>
		public ChiSquaredDistribution(RandomGenerator rng, double degreesOfFreedom, double inverseCumAccuracy) : base(rng)
		{

			gamma = new GammaDistribution(degreesOfFreedom / 2, 2);
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Access the number of degrees of freedom.
		/// </summary>
		/// <returns> the degrees of freedom. </returns>
		public virtual double DegreesOfFreedom
		{
			get
			{
				return gamma.Shape * 2.0;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double density(double x)
		{
			return gamma.density(x);
		}

		/// <summary>
		/// {@inheritDoc} * </summary>
		public override double logDensity(double x)
		{
			return gamma.logDensity(x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(double x)
		{
			return gamma.cumulativeProbability(x);
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
		/// 
		/// For {@code k} degrees of freedom, the mean is {@code k}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return DegreesOfFreedom;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> {@code 2 * k}, where {@code k} is the number of degrees of freedom. </returns>
		public override double NumericalVariance
		{
			get
			{
				return 2 * DegreesOfFreedom;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the
		/// degrees of freedom.
		/// </summary>
		/// <returns> zero. </returns>
		public override double SupportLowerBound
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is always positive infinity no matter the
		/// degrees of freedom.
		/// </summary>
		/// <returns> {@code Double.POSITIVE_INFINITY}. </returns>
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
		/// 
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
	}

}