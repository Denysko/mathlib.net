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
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Beta = mathlib.special.Beta;
	using CombinatoricsUtils = mathlib.util.CombinatoricsUtils;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// <p>
	/// Implementation of the Pascal distribution. The Pascal distribution is a
	/// special case of the Negative Binomial distribution where the number of
	/// successes parameter is an integer.
	/// </p>
	/// <p>
	/// There are various ways to express the probability mass and distribution
	/// functions for the Pascal distribution. The present implementation represents
	/// the distribution of the number of failures before {@code r} successes occur.
	/// This is the convention adopted in e.g.
	/// <a href="http://mathworld.wolfram.com/NegativeBinomialDistribution.html">MathWorld</a>,
	/// but <em>not</em> in
	/// <a href="http://en.wikipedia.org/wiki/Negative_binomial_distribution">Wikipedia</a>.
	/// </p>
	/// <p>
	/// For a random variable {@code X} whose values are distributed according to this
	/// distribution, the probability mass function is given by<br/>
	/// {@code P(X = k) = C(k + r - 1, r - 1) * p^r * (1 - p)^k,}<br/>
	/// where {@code r} is the number of successes, {@code p} is the probability of
	/// success, and {@code X} is the total number of failures. {@code C(n, k)} is
	/// the binomial coefficient ({@code n} choose {@code k}). The mean and variance
	/// of {@code X} are<br/>
	/// {@code E(X) = (1 - p) * r / p, var(X) = (1 - p) * r / p^2.}<br/>
	/// Finally, the cumulative distribution function is given by<br/>
	/// {@code P(X <= k) = I(p, r, k + 1)},
	/// where I is the regularized incomplete Beta function.
	/// </p>
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Negative_binomial_distribution">
	/// Negative binomial distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/NegativeBinomialDistribution.html">
	/// Negative binomial distribution (MathWorld)</a>
	/// @version $Id: PascalDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $
	/// @since 1.2 (changed to concrete class in 3.0) </seealso>
	public class PascalDistribution : AbstractIntegerDistribution
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 6751309484392813623L;
		/// <summary>
		/// The number of successes. </summary>
		private readonly int numberOfSuccesses;
		/// <summary>
		/// The probability of success. </summary>
		private readonly double probabilityOfSuccess;
		/// <summary>
		/// The value of {@code log(p)}, where {@code p} is the probability of success,
		/// stored for faster computation. 
		/// </summary>
		private readonly double logProbabilityOfSuccess;
		/// <summary>
		/// The value of {@code log(1-p)}, where {@code p} is the probability of success,
		/// stored for faster computation. 
		/// </summary>
		private readonly double log1mProbabilityOfSuccess;

		/// <summary>
		/// Create a Pascal distribution with the given number of successes and
		/// probability of success.
		/// </summary>
		/// <param name="r"> Number of successes. </param>
		/// <param name="p"> Probability of success. </param>
		/// <exception cref="NotStrictlyPositiveException"> if the number of successes is not positive </exception>
		/// <exception cref="OutOfRangeException"> if the probability of success is not in the
		/// range {@code [0, 1]}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PascalDistribution(int r, double p) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.OutOfRangeException
		public PascalDistribution(int r, double p) : this(new Well19937c(), r, p)
		{
		}

		/// <summary>
		/// Create a Pascal distribution with the given number of successes and
		/// probability of success.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="r"> Number of successes. </param>
		/// <param name="p"> Probability of success. </param>
		/// <exception cref="NotStrictlyPositiveException"> if the number of successes is not positive </exception>
		/// <exception cref="OutOfRangeException"> if the probability of success is not in the
		/// range {@code [0, 1]}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PascalDistribution(mathlib.random.RandomGenerator rng, int r, double p) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.OutOfRangeException
		public PascalDistribution(RandomGenerator rng, int r, double p) : base(rng)
		{

			if (r <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SUCCESSES, r);
			}
			if (p < 0 || p > 1)
			{
				throw new OutOfRangeException(p, 0, 1);
			}

			numberOfSuccesses = r;
			probabilityOfSuccess = p;
			logProbabilityOfSuccess = FastMath.log(p);
			log1mProbabilityOfSuccess = FastMath.log1p(-p);
		}

		/// <summary>
		/// Access the number of successes for this distribution.
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
			double ret;
			if (x < 0)
			{
				ret = 0.0;
			}
			else
			{
				ret = CombinatoricsUtils.binomialCoefficientDouble(x + numberOfSuccesses - 1, numberOfSuccesses - 1) * FastMath.pow(probabilityOfSuccess, numberOfSuccesses) * FastMath.pow(1.0 - probabilityOfSuccess, x);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double logProbability(int x)
		{
			double ret;
			if (x < 0)
			{
				ret = double.NegativeInfinity;
			}
			else
			{
				ret = CombinatoricsUtils.binomialCoefficientLog(x + numberOfSuccesses - 1, numberOfSuccesses - 1) + logProbabilityOfSuccess * numberOfSuccesses + log1mProbabilityOfSuccess * x;
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
			else
			{
				ret = Beta.regularizedBeta(probabilityOfSuccess, numberOfSuccesses, x + 1.0);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For number of successes {@code r} and probability of success {@code p},
		/// the mean is {@code r * (1 - p) / p}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double p = getProbabilityOfSuccess();
				double p = ProbabilityOfSuccess;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double r = getNumberOfSuccesses();
				double r = NumberOfSuccesses;
				return (r * (1 - p)) / p;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For number of successes {@code r} and probability of success {@code p},
		/// the variance is {@code r * (1 - p) / p^2}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double p = getProbabilityOfSuccess();
				double p = ProbabilityOfSuccess;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double r = getNumberOfSuccesses();
				double r = NumberOfSuccesses;
				return r * (1 - p) / (p * p);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the parameters.
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
		/// The upper bound of the support is always positive infinity no matter the
		/// parameters. Positive infinity is symbolized by {@code Integer.MAX_VALUE}.
		/// </summary>
		/// <returns> upper bound of the support (always {@code Integer.MAX_VALUE}
		/// for positive infinity) </returns>
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
	}

}