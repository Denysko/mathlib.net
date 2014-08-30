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

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Gamma = org.apache.commons.math3.special.Gamma;
	using CombinatoricsUtils = org.apache.commons.math3.util.CombinatoricsUtils;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of the Poisson distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Poisson_distribution">Poisson distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/PoissonDistribution.html">Poisson distribution (MathWorld)</a>
	/// @version $Id: PoissonDistribution.java 1540217 2013-11-08 23:27:49Z psteitz $ </seealso>
	public class PoissonDistribution : AbstractIntegerDistribution
	{
		/// <summary>
		/// Default maximum number of iterations for cumulative probability calculations.
		/// @since 2.1
		/// </summary>
		public const int DEFAULT_MAX_ITERATIONS = 10000000;
		/// <summary>
		/// Default convergence criterion.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_EPSILON = 1e-12;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3349935121172596109L;
		/// <summary>
		/// Distribution used to compute normal approximation. </summary>
		private readonly NormalDistribution normal;
		/// <summary>
		/// Distribution needed for the <seealso cref="#sample()"/> method. </summary>
		private readonly ExponentialDistribution exponential;
		/// <summary>
		/// Mean of the distribution. </summary>
		private readonly double mean;

		/// <summary>
		/// Maximum number of iterations for cumulative probability. Cumulative
		/// probabilities are estimated using either Lanczos series approximation
		/// of <seealso cref="Gamma#regularizedGammaP(double, double, double, int)"/>
		/// or continued fraction approximation of
		/// <seealso cref="Gamma#regularizedGammaQ(double, double, double, int)"/>.
		/// </summary>
		private readonly int maxIterations;

		/// <summary>
		/// Convergence criterion for cumulative probability. </summary>
		private readonly double epsilon;

		/// <summary>
		/// Creates a new Poisson distribution with specified mean.
		/// </summary>
		/// <param name="p"> the Poisson mean </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code p <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PoissonDistribution(double p) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public PoissonDistribution(double p) : this(p, DEFAULT_EPSILON, DEFAULT_MAX_ITERATIONS)
		{
		}

		/// <summary>
		/// Creates a new Poisson distribution with specified mean, convergence
		/// criterion and maximum number of iterations.
		/// </summary>
		/// <param name="p"> Poisson mean. </param>
		/// <param name="epsilon"> Convergence criterion for cumulative probabilities. </param>
		/// <param name="maxIterations"> the maximum number of iterations for cumulative
		/// probabilities. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code p <= 0}.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PoissonDistribution(double p, double epsilon, int maxIterations) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public PoissonDistribution(double p, double epsilon, int maxIterations) : this(new Well19937c(), p, epsilon, maxIterations)
		{
		}

		/// <summary>
		/// Creates a new Poisson distribution with specified mean, convergence
		/// criterion and maximum number of iterations.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="p"> Poisson mean. </param>
		/// <param name="epsilon"> Convergence criterion for cumulative probabilities. </param>
		/// <param name="maxIterations"> the maximum number of iterations for cumulative
		/// probabilities. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code p <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PoissonDistribution(org.apache.commons.math3.random.RandomGenerator rng, double p, double epsilon, int maxIterations) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public PoissonDistribution(RandomGenerator rng, double p, double epsilon, int maxIterations) : base(rng)
		{

			if (p <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.MEAN, p);
			}
			mean = p;
			this.epsilon = epsilon;
			this.maxIterations = maxIterations;

			// Use the same RNG instance as the parent class.
			normal = new NormalDistribution(rng, p, FastMath.sqrt(p), NormalDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY);
			exponential = new ExponentialDistribution(rng, 1, ExponentialDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY);
		}

		/// <summary>
		/// Creates a new Poisson distribution with the specified mean and
		/// convergence criterion.
		/// </summary>
		/// <param name="p"> Poisson mean. </param>
		/// <param name="epsilon"> Convergence criterion for cumulative probabilities. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code p <= 0}.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PoissonDistribution(double p, double epsilon) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public PoissonDistribution(double p, double epsilon) : this(p, epsilon, DEFAULT_MAX_ITERATIONS)
		{
		}

		/// <summary>
		/// Creates a new Poisson distribution with the specified mean and maximum
		/// number of iterations.
		/// </summary>
		/// <param name="p"> Poisson mean. </param>
		/// <param name="maxIterations"> Maximum number of iterations for cumulative
		/// probabilities.
		/// @since 2.1 </param>
		public PoissonDistribution(double p, int maxIterations) : this(p, DEFAULT_EPSILON, maxIterations)
		{
		}

		/// <summary>
		/// Get the mean for the distribution.
		/// </summary>
		/// <returns> the mean for the distribution. </returns>
		public virtual double Mean
		{
			get
			{
				return mean;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double probability(int x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logProbability = logProbability(x);
			double logProbability = logProbability(x);
			return logProbability == double.NegativeInfinity ? 0 : FastMath.exp(logProbability);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double logProbability(int x)
		{
			double ret;
			if (x < 0 || x == int.MaxValue)
			{
				ret = double.NegativeInfinity;
			}
			else if (x == 0)
			{
				ret = -mean;
			}
			else
			{
				ret = -SaddlePointExpansion.getStirlingError(x) - SaddlePointExpansion.getDeviancePart(x, mean) - 0.5 * FastMath.log(MathUtils.TWO_PI) - 0.5 * FastMath.log(x);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(int x)
		{
			if (x < 0)
			{
				return 0;
			}
			if (x == int.MaxValue)
			{
				return 1;
			}
			return Gamma.regularizedGammaQ((double) x + 1, mean, epsilon, maxIterations);
		}

		/// <summary>
		/// Calculates the Poisson distribution function using a normal
		/// approximation. The {@code N(mean, sqrt(mean))} distribution is used
		/// to approximate the Poisson distribution. The computation uses
		/// "half-correction" (evaluating the normal distribution function at
		/// {@code x + 0.5}).
		/// </summary>
		/// <param name="x"> Upper bound, inclusive. </param>
		/// <returns> the distribution function value calculated using a normal
		/// approximation. </returns>
		public virtual double normalApproximateProbability(int x)
		{
			// calculate the probability using half-correction
			return normal.cumulativeProbability(x + 0.5);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For mean parameter {@code p}, the mean is {@code p}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return Mean;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For mean parameter {@code p}, the variance is {@code p}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				return Mean;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the mean parameter.
		/// </summary>
		/// <returns> lower bound of the support (always 0) </returns>
		public override int SupportLowerBound
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is positive infinity,
		/// regardless of the parameter values. There is no integer infinity,
		/// so this method returns {@code Integer.MAX_VALUE}.
		/// </summary>
		/// <returns> upper bound of the support (always {@code Integer.MAX_VALUE} for
		/// positive infinity) </returns>
		public override int SupportUpperBound
		{
			get
			{
				return int.MaxValue;
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

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// <strong>Algorithm Description</strong>:
		/// <ul>
		///  <li>For small means, uses simulation of a Poisson process
		///   using Uniform deviates, as described
		///   <a href="http://irmi.epfl.ch/cmos/Pmmi/interactive/rng7.htm"> here</a>.
		///   The Poisson process (and hence value returned) is bounded by 1000 * mean.
		///  </li>
		///  <li>For large means, uses the rejection algorithm described in
		///   <quote>
		///    Devroye, Luc. (1981).<i>The Computer Generation of Poisson Random Variables</i>
		///    <strong>Computing</strong> vol. 26 pp. 197-207.
		///   </quote>
		///  </li>
		/// </ul>
		/// </p>
		/// </summary>
		/// <returns> a random value.
		/// @since 2.2 </returns>
		public override int sample()
		{
			return (int) FastMath.min(nextPoisson(mean), int.MaxValue);
		}

		/// <param name="meanPoisson"> Mean of the Poisson distribution. </param>
		/// <returns> the next sample. </returns>
		private long nextPoisson(double meanPoisson)
		{
			const double pivot = 40.0d;
			if (meanPoisson < pivot)
			{
				double p = FastMath.exp(-meanPoisson);
				long n = 0;
				double r = 1.0d;
				double rnd = 1.0d;

				while (n < 1000 * meanPoisson)
				{
					rnd = random.NextDouble();
					r *= rnd;
					if (r >= p)
					{
						n++;
					}
					else
					{
						return n;
					}
				}
				return n;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lambda = org.apache.commons.math3.util.FastMath.floor(meanPoisson);
				double lambda = FastMath.floor(meanPoisson);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lambdaFractional = meanPoisson - lambda;
				double lambdaFractional = meanPoisson - lambda;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logLambda = org.apache.commons.math3.util.FastMath.log(lambda);
				double logLambda = FastMath.log(lambda);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logLambdaFactorial = org.apache.commons.math3.util.CombinatoricsUtils.factorialLog((int) lambda);
				double logLambdaFactorial = CombinatoricsUtils.factorialLog((int) lambda);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long y2 = lambdaFractional < Double.MIN_VALUE ? 0 : nextPoisson(lambdaFractional);
				long y2 = lambdaFractional < double.MinValue ? 0 : nextPoisson(lambdaFractional);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = org.apache.commons.math3.util.FastMath.sqrt(lambda * org.apache.commons.math3.util.FastMath.log(32 * lambda / org.apache.commons.math3.util.FastMath.PI + 1));
				double delta = FastMath.sqrt(lambda * FastMath.log(32 * lambda / FastMath.PI + 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double halfDelta = delta / 2;
				double halfDelta = delta / 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double twolpd = 2 * lambda + delta;
				double twolpd = 2 * lambda + delta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1 = org.apache.commons.math3.util.FastMath.sqrt(org.apache.commons.math3.util.FastMath.PI * twolpd) * org.apache.commons.math3.util.FastMath.exp(1 / (8 * lambda));
				double a1 = FastMath.sqrt(FastMath.PI * twolpd) * FastMath.exp(1 / (8 * lambda));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2 = (twolpd / delta) * org.apache.commons.math3.util.FastMath.exp(-delta * (1 + delta) / twolpd);
				double a2 = (twolpd / delta) * FastMath.exp(-delta * (1 + delta) / twolpd);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aSum = a1 + a2 + 1;
				double aSum = a1 + a2 + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p1 = a1 / aSum;
				double p1 = a1 / aSum;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p2 = a2 / aSum;
				double p2 = a2 / aSum;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c1 = 1 / (8 * lambda);
				double c1 = 1 / (8 * lambda);

				double x = 0;
				double y = 0;
				double v = 0;
				int a = 0;
				double t = 0;
				double qr = 0;
				double qa = 0;
				for (;;)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u = random.nextDouble();
					double u = random.NextDouble();
					if (u <= p1)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = random.nextGaussian();
						double n = random.nextGaussian();
						x = n * FastMath.sqrt(lambda + halfDelta) - 0.5d;
						if (x > delta || x < -lambda)
						{
							continue;
						}
						y = x < 0 ? FastMath.floor(x) : FastMath.ceil(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double e = exponential.sample();
						double e = exponential.sample();
						v = -e - (n * n / 2) + c1;
					}
					else
					{
						if (u > p1 + p2)
						{
							y = lambda;
							break;
						}
						else
						{
							x = delta + (twolpd / delta) * exponential.sample();
							y = FastMath.ceil(x);
							v = -exponential.sample() - delta * (x + 1) / twolpd;
						}
					}
					a = x < 0 ? 1 : 0;
					t = y * (y + 1) / (2 * lambda);
					if (v < -t && a == 0)
					{
						y = lambda + y;
						break;
					}
					qr = t * ((2 * y + 1) / (6 * lambda) - 1);
					qa = qr - (t * t) / (3 * (lambda + a * (y + 1)));
					if (v < qa)
					{
						y = lambda + y;
						break;
					}
					if (v > qr)
					{
						continue;
					}
					if (v < y * logLambda - CombinatoricsUtils.factorialLog((int)(y + lambda)) + logLambdaFactorial)
					{
						y = lambda + y;
						break;
					}
				}
				return y2 + (long) y;
			}
		}
	}

}