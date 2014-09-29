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

	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the hypergeometric distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Hypergeometric_distribution">Hypergeometric distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/HypergeometricDistribution.html">Hypergeometric distribution (MathWorld)</a>
	/// @version $Id: HypergeometricDistribution.java 1534358 2013-10-21 20:13:52Z tn $ </seealso>
	public class HypergeometricDistribution : AbstractIntegerDistribution
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -436928820673516179L;
		/// <summary>
		/// The number of successes in the population. </summary>
		private readonly int numberOfSuccesses;
		/// <summary>
		/// The population size. </summary>
		private readonly int populationSize;
		/// <summary>
		/// The sample size. </summary>
		private readonly int sampleSize;
		/// <summary>
		/// Cached numerical variance </summary>
		private double numericalVariance = double.NaN;
		/// <summary>
		/// Whether or not the numerical variance has been calculated </summary>
		private bool numericalVarianceIsCalculated = false;

		/// <summary>
		/// Construct a new hypergeometric distribution with the specified population
		/// size, number of successes in the population, and sample size.
		/// </summary>
		/// <param name="populationSize"> Population size. </param>
		/// <param name="numberOfSuccesses"> Number of successes in the population. </param>
		/// <param name="sampleSize"> Sample size. </param>
		/// <exception cref="NotPositiveException"> if {@code numberOfSuccesses < 0}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code populationSize <= 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code numberOfSuccesses > populationSize},
		/// or {@code sampleSize > populationSize}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HypergeometricDistribution(int populationSize, int numberOfSuccesses, int sampleSize) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooLargeException
		public HypergeometricDistribution(int populationSize, int numberOfSuccesses, int sampleSize) : this(new Well19937c(), populationSize, numberOfSuccesses, sampleSize)
		{
		}

		/// <summary>
		/// Creates a new hypergeometric distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="populationSize"> Population size. </param>
		/// <param name="numberOfSuccesses"> Number of successes in the population. </param>
		/// <param name="sampleSize"> Sample size. </param>
		/// <exception cref="NotPositiveException"> if {@code numberOfSuccesses < 0}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code populationSize <= 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code numberOfSuccesses > populationSize},
		/// or {@code sampleSize > populationSize}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public HypergeometricDistribution(mathlib.random.RandomGenerator rng, int populationSize, int numberOfSuccesses, int sampleSize) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooLargeException
		public HypergeometricDistribution(RandomGenerator rng, int populationSize, int numberOfSuccesses, int sampleSize) : base(rng)
		{

			if (populationSize <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.POPULATION_SIZE, populationSize);
			}
			if (numberOfSuccesses < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_SUCCESSES, numberOfSuccesses);
			}
			if (sampleSize < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize);
			}

			if (numberOfSuccesses > populationSize)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.NUMBER_OF_SUCCESS_LARGER_THAN_POPULATION_SIZE, numberOfSuccesses, populationSize, true);
			}
			if (sampleSize > populationSize)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.SAMPLE_SIZE_LARGER_THAN_POPULATION_SIZE, sampleSize, populationSize, true);
			}

			this.numberOfSuccesses = numberOfSuccesses;
			this.populationSize = populationSize;
			this.sampleSize = sampleSize;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(int x)
		{
			double ret;

			int[] domain = getDomain(populationSize, numberOfSuccesses, sampleSize);
			if (x < domain[0])
			{
				ret = 0.0;
			}
			else if (x >= domain[1])
			{
				ret = 1.0;
			}
			else
			{
				ret = innerCumulativeProbability(domain[0], x, 1);
			}

			return ret;
		}

		/// <summary>
		/// Return the domain for the given hypergeometric distribution parameters.
		/// </summary>
		/// <param name="n"> Population size. </param>
		/// <param name="m"> Number of successes in the population. </param>
		/// <param name="k"> Sample size. </param>
		/// <returns> a two element array containing the lower and upper bounds of the
		/// hypergeometric distribution. </returns>
		private int[] getDomain(int n, int m, int k)
		{
			return new int[] {getLowerDomain(n, m, k), getUpperDomain(m, k)};
		}

		/// <summary>
		/// Return the lowest domain value for the given hypergeometric distribution
		/// parameters.
		/// </summary>
		/// <param name="n"> Population size. </param>
		/// <param name="m"> Number of successes in the population. </param>
		/// <param name="k"> Sample size. </param>
		/// <returns> the lowest domain value of the hypergeometric distribution. </returns>
		private int getLowerDomain(int n, int m, int k)
		{
			return FastMath.max(0, m - (n - k));
		}

		/// <summary>
		/// Access the number of successes.
		/// </summary>
		/// <returns> the number of successes. </returns>
		public virtual int NumberOfSuccesses
		{
			get
			{
				return numberOfSuccesses;
			}
		}

		/// <summary>
		/// Access the population size.
		/// </summary>
		/// <returns> the population size. </returns>
		public virtual int PopulationSize
		{
			get
			{
				return populationSize;
			}
		}

		/// <summary>
		/// Access the sample size.
		/// </summary>
		/// <returns> the sample size. </returns>
		public virtual int SampleSize
		{
			get
			{
				return sampleSize;
			}
		}

		/// <summary>
		/// Return the highest domain value for the given hypergeometric distribution
		/// parameters.
		/// </summary>
		/// <param name="m"> Number of successes in the population. </param>
		/// <param name="k"> Sample size. </param>
		/// <returns> the highest domain value of the hypergeometric distribution. </returns>
		private int getUpperDomain(int m, int k)
		{
			return FastMath.min(k, m);
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

			int[] domain = getDomain(populationSize, numberOfSuccesses, sampleSize);
			if (x < domain[0] || x > domain[1])
			{
				ret = double.NegativeInfinity;
			}
			else
			{
				double p = (double) sampleSize / (double) populationSize;
				double q = (double)(populationSize - sampleSize) / (double) populationSize;
				double p1 = SaddlePointExpansion.logBinomialProbability(x, numberOfSuccesses, p, q);
				double p2 = SaddlePointExpansion.logBinomialProbability(sampleSize - x, populationSize - numberOfSuccesses, p, q);
				double p3 = SaddlePointExpansion.logBinomialProbability(sampleSize, populationSize, p, q);
				ret = p1 + p2 - p3;
			}

			return ret;
		}

		/// <summary>
		/// For this distribution, {@code X}, this method returns {@code P(X >= x)}.
		/// </summary>
		/// <param name="x"> Value at which the CDF is evaluated. </param>
		/// <returns> the upper tail CDF for this distribution.
		/// @since 1.1 </returns>
		public virtual double upperCumulativeProbability(int x)
		{
			double ret;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] domain = getDomain(populationSize, numberOfSuccesses, sampleSize);
			int[] domain = getDomain(populationSize, numberOfSuccesses, sampleSize);
			if (x <= domain[0])
			{
				ret = 1.0;
			}
			else if (x > domain[1])
			{
				ret = 0.0;
			}
			else
			{
				ret = innerCumulativeProbability(domain[1], x, -1);
			}

			return ret;
		}

		/// <summary>
		/// For this distribution, {@code X}, this method returns
		/// {@code P(x0 <= X <= x1)}.
		/// This probability is computed by summing the point probabilities for the
		/// values {@code x0, x0 + 1, x0 + 2, ..., x1}, in the order directed by
		/// {@code dx}.
		/// </summary>
		/// <param name="x0"> Inclusive lower bound. </param>
		/// <param name="x1"> Inclusive upper bound. </param>
		/// <param name="dx"> Direction of summation (1 indicates summing from x0 to x1, and
		/// 0 indicates summing from x1 to x0). </param>
		/// <returns> {@code P(x0 <= X <= x1)}. </returns>
		private double innerCumulativeProbability(int x0, int x1, int dx)
		{
			double ret = probability(x0);
			while (x0 != x1)
			{
				x0 += dx;
				ret += probability(x0);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For population size {@code N}, number of successes {@code m}, and sample
		/// size {@code n}, the mean is {@code n * m / N}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return SampleSize * (NumberOfSuccesses / (double) PopulationSize);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For population size {@code N}, number of successes {@code m}, and sample
		/// size {@code n}, the variance is
		/// {@code [n * m * (N - n) * (N - m)] / [N^2 * (N - 1)]}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				if (!numericalVarianceIsCalculated)
				{
					numericalVariance = calculateNumericalVariance();
					numericalVarianceIsCalculated = true;
				}
				return numericalVariance;
			}
		}

		/// <summary>
		/// Used by <seealso cref="#getNumericalVariance()"/>.
		/// </summary>
		/// <returns> the variance of this distribution </returns>
		protected internal virtual double calculateNumericalVariance()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double N = getPopulationSize();
			double N = PopulationSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = getNumberOfSuccesses();
			double m = NumberOfSuccesses;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = getSampleSize();
			double n = SampleSize;
			return (n * m * (N - n) * (N - m)) / (N * N * (N - 1));
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For population size {@code N}, number of successes {@code m}, and sample
		/// size {@code n}, the lower bound of the support is
		/// {@code max(0, n + m - N)}.
		/// </summary>
		/// <returns> lower bound of the support </returns>
		public override int SupportLowerBound
		{
			get
			{
				return FastMath.max(0, SampleSize + NumberOfSuccesses - PopulationSize);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For number of successes {@code m} and sample size {@code n}, the upper
		/// bound of the support is {@code min(m, n)}.
		/// </summary>
		/// <returns> upper bound of the support </returns>
		public override int SupportUpperBound
		{
			get
			{
				return FastMath.min(NumberOfSuccesses, SampleSize);
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