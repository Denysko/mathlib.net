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
namespace org.apache.commons.math3.distribution
{

	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Gamma = org.apache.commons.math3.special.Gamma;
	using Beta = org.apache.commons.math3.special.Beta;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implements the Beta distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Beta_distribution">Beta distribution</a>
	/// @version $Id: BetaDistribution.java 1533990 2013-10-20 21:24:45Z tn $
	/// @since 2.0 (changed to concrete class in 3.0) </seealso>
	public class BetaDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -1221965979403477668L;
		/// <summary>
		/// First shape parameter. </summary>
		private readonly double alpha;
		/// <summary>
		/// Second shape parameter. </summary>
		private readonly double beta;
		/// <summary>
		/// Normalizing factor used in density computations.
		/// updated whenever alpha or beta are changed.
		/// </summary>
		private double z;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Build a new instance.
		/// </summary>
		/// <param name="alpha"> First shape parameter (must be positive). </param>
		/// <param name="beta"> Second shape parameter (must be positive). </param>
		public BetaDistribution(double alpha, double beta) : this(alpha, beta, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Build a new instance.
		/// </summary>
		/// <param name="alpha"> First shape parameter (must be positive). </param>
		/// <param name="beta"> Second shape parameter (must be positive). </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>).
		/// @since 2.1 </param>
		public BetaDistribution(double alpha, double beta, double inverseCumAccuracy) : this(new Well19937c(), alpha, beta, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a &beta; distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="alpha"> First shape parameter (must be positive). </param>
		/// <param name="beta"> Second shape parameter (must be positive).
		/// @since 3.3 </param>
		public BetaDistribution(RandomGenerator rng, double alpha, double beta) : this(rng, alpha, beta, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a &beta; distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="alpha"> First shape parameter (must be positive). </param>
		/// <param name="beta"> Second shape parameter (must be positive). </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>).
		/// @since 3.1 </param>
		public BetaDistribution(RandomGenerator rng, double alpha, double beta, double inverseCumAccuracy) : base(rng)
		{

			this.alpha = alpha;
			this.beta = beta;
			z = double.NaN;
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Access the first shape parameter, {@code alpha}.
		/// </summary>
		/// <returns> the first shape parameter. </returns>
		public virtual double Alpha
		{
			get
			{
				return alpha;
			}
		}

		/// <summary>
		/// Access the second shape parameter, {@code beta}.
		/// </summary>
		/// <returns> the second shape parameter. </returns>
		public virtual double Beta
		{
			get
			{
				return beta;
			}
		}

		/// <summary>
		/// Recompute the normalization factor. </summary>
		private void recomputeZ()
		{
			if (double.IsNaN(z))
			{
				z = Gamma.logGamma(alpha) + Gamma.logGamma(beta) - Gamma.logGamma(alpha + beta);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double density(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logDensity = logDensity(x);
			double logDensity = logDensity(x);
			return logDensity == double.NegativeInfinity ? 0 : FastMath.exp(logDensity);
		}

		/// <summary>
		/// {@inheritDoc} * </summary>
		public override double logDensity(double x)
		{
			recomputeZ();
			if (x < 0 || x > 1)
			{
				return double.NegativeInfinity;
			}
			else if (x == 0)
			{
				if (alpha < 1)
				{
					throw new NumberIsTooSmallException(LocalizedFormats.CANNOT_COMPUTE_BETA_DENSITY_AT_0_FOR_SOME_ALPHA, alpha, 1, false);
				}
				return double.NegativeInfinity;
			}
			else if (x == 1)
			{
				if (beta < 1)
				{
					throw new NumberIsTooSmallException(LocalizedFormats.CANNOT_COMPUTE_BETA_DENSITY_AT_1_FOR_SOME_BETA, beta, 1, false);
				}
				return double.NegativeInfinity;
			}
			else
			{
				double logX = FastMath.log(x);
				double log1mX = FastMath.log1p(-x);
				return (alpha - 1) * logX + (beta - 1) * log1mX - z;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(double x)
		{
			if (x <= 0)
			{
				return 0;
			}
			else if (x >= 1)
			{
				return 1;
			}
			else
			{
				return Beta.regularizedBeta(x, alpha, beta);
			}
		}

		/// <summary>
		/// Return the absolute accuracy setting of the solver used to estimate
		/// inverse cumulative probabilities.
		/// </summary>
		/// <returns> the solver absolute accuracy.
		/// @since 2.1 </returns>
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
		/// For first shape parameter {@code alpha} and second shape parameter
		/// {@code beta}, the mean is {@code alpha / (alpha + beta)}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double a = getAlpha();
				double a = Alpha;
				return a / (a + Beta);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For first shape parameter {@code alpha} and second shape parameter
		/// {@code beta}, the variance is
		/// {@code (alpha * beta) / [(alpha + beta)^2 * (alpha + beta + 1)]}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double a = getAlpha();
				double a = Alpha;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double b = getBeta();
				double b = Beta;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double alphabetasum = a + b;
				double alphabetasum = a + b;
				return (a * b) / ((alphabetasum * alphabetasum) * (alphabetasum + 1));
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the parameters.
		/// </summary>
		/// <returns> lower bound of the support (always 0) </returns>
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
		/// The upper bound of the support is always 1 no matter the parameters.
		/// </summary>
		/// <returns> upper bound of the support (always 1) </returns>
		public override double SupportUpperBound
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportLowerBoundInclusive
		{
			get
			{
				return false;
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