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

	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Beta = org.apache.commons.math3.special.Beta;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of the binomial distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Binomial_distribution">Binomial distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/BinomialDistribution.html">Binomial Distribution (MathWorld)</a>
	/// @version $Id: BinomialDistribution.java 1534358 2013-10-21 20:13:52Z tn $ </seealso>
	public class BinomialDistribution : AbstractIntegerDistribution
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 6751309484392813623L;
		/// <summary>
		/// The number of trials. </summary>
		private readonly int numberOfTrials;
		/// <summary>
		/// The probability of success. </summary>
		private readonly double probabilityOfSuccess;

		/// <summary>
		/// Create a binomial distribution with the given number of trials and
		/// probability of success.
		/// </summary>
		/// <param name="trials"> Number of trials. </param>
		/// <param name="p"> Probability of success. </param>
		/// <exception cref="NotPositiveException"> if {@code trials < 0}. </exception>
		/// <exception cref="OutOfRangeException"> if {@code p < 0} or {@code p > 1}. </exception>
		public BinomialDistribution(int trials, double p) : this(new Well19937c(), trials, p)
		{
		}

		/// <summary>
		/// Creates a binomial distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="trials"> Number of trials. </param>
		/// <param name="p"> Probability of success. </param>
		/// <exception cref="NotPositiveException"> if {@code trials < 0}. </exception>
		/// <exception cref="OutOfRangeException"> if {@code p < 0} or {@code p > 1}.
		/// @since 3.1 </exception>
		public BinomialDistribution(RandomGenerator rng, int trials, double p) : base(rng)
		{

			if (trials < 0)
			{
				throw new NotPositiveException(LocalizedFormats.NUMBER_OF_TRIALS, trials);
			}
			if (p < 0 || p > 1)
			{
				throw new OutOfRangeException(p, 0, 1);
			}

			probabilityOfSuccess = p;
			numberOfTrials = trials;
		}

		/// <summary>
		/// Access the number of trials for this distribution.
		/// </summary>
		/// <returns> the number of trials. </returns>
		public virtual int NumberOfTrials
		{
			get
			{
				return numberOfTrials;
			}
		}

		/// <summary>
		/// Access the probability of success for this distribution.
		/// </summary>
		/// <returns> the probability of success. </returns>
		public virtual double ProbabilityOfSuccess
		{
			get
			{
				return probabilityOfSuccess;
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
		/// {@inheritDoc} * </summary>
		public override double logProbability(int x)
		{
			double ret;
			if (x < 0 || x > numberOfTrials)
			{
				ret = double.NegativeInfinity;
			}
			else
			{
				ret = SaddlePointExpansion.logBinomialProbability(x, numberOfTrials, probabilityOfSuccess, 1.0 - probabilityOfSuccess);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(int x)
		{
			double ret;
			if (x < 0)
			{
				ret = 0.0;
			}
			else if (x >= numberOfTrials)
			{
				ret = 1.0;
			}
			else
			{
				ret = 1.0 - Beta.regularizedBeta(probabilityOfSuccess, x + 1.0, numberOfTrials - x);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For {@code n} trials and probability parameter {@code p}, the mean is
		/// {@code n * p}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return numberOfTrials * probabilityOfSuccess;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For {@code n} trials and probability parameter {@code p}, the variance is
		/// {@code n * p * (1 - p)}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double p = probabilityOfSuccess;
				double p = probabilityOfSuccess;
				return numberOfTrials * p * (1 - p);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 except for the probability
		/// parameter {@code p = 1}.
		/// </summary>
		/// <returns> lower bound of the support (0 or the number of trials) </returns>
		public override int SupportLowerBound
		{
			get
			{
				return probabilityOfSuccess < 1.0 ? 0 : numberOfTrials;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is the number of trials except for the
		/// probability parameter {@code p = 0}.
		/// </summary>
		/// <returns> upper bound of the support (number of trials or 0) </returns>
		public override int SupportUpperBound
		{
			get
			{
				return probabilityOfSuccess > 0.0 ? numberOfTrials : 0;
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