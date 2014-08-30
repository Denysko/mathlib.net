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
namespace org.apache.commons.math3.distribution
{

	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using RandomDataImpl = org.apache.commons.math3.random.RandomDataImpl;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Base class for integer-valued discrete distributions.  Default
	/// implementations are provided for some of the methods that do not vary
	/// from distribution to distribution.
	/// 
	/// @version $Id: AbstractIntegerDistribution.java 1547633 2013-12-03 23:03:06Z tn $
	/// </summary>
	[Serializable]
	public abstract class AbstractIntegerDistribution : IntegerDistribution
	{
		public abstract bool SupportConnected {get;}
		public abstract int SupportUpperBound {get;}
		public abstract int SupportLowerBound {get;}
		public abstract double NumericalVariance {get;}
		public abstract double NumericalMean {get;}
		public abstract double cumulativeProbability(int x);
		public abstract double probability(int x);

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -1146319659338487221L;

		/// <summary>
		/// RandomData instance used to generate samples from the distribution. </summary>
		/// @deprecated As of 3.1, to be removed in 4.0. Please use the
		/// <seealso cref="#random"/> instance variable instead. 
		[Obsolete("As of 3.1, to be removed in 4.0. Please use the")]
		protected internal readonly RandomDataImpl randomData = new RandomDataImpl();

		/// <summary>
		/// RNG instance used to generate samples from the distribution.
		/// @since 3.1
		/// </summary>
		protected internal readonly RandomGenerator random;

		/// @deprecated As of 3.1, to be removed in 4.0. Please use
		/// <seealso cref="#AbstractIntegerDistribution(RandomGenerator)"/> instead. 
		[Obsolete("As of 3.1, to be removed in 4.0. Please use")]
		protected internal AbstractIntegerDistribution()
		{
			// Legacy users are only allowed to access the deprecated "randomData".
			// New users are forbidden to use this constructor.
			random = null;
		}

		/// <param name="rng"> Random number generator.
		/// @since 3.1 </param>
		protected internal AbstractIntegerDistribution(RandomGenerator rng)
		{
			random = rng;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The default implementation uses the identity
		/// <p>{@code P(x0 < X <= x1) = P(X <= x1) - P(X <= x0)}</p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double cumulativeProbability(int x0, int x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		public virtual double cumulativeProbability(int x0, int x1)
		{
			if (x1 < x0)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT, x0, x1, true);
			}
			return cumulativeProbability(x1) - cumulativeProbability(x0);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The default implementation returns
		/// <ul>
		/// <li><seealso cref="#getSupportLowerBound()"/> for {@code p = 0},</li>
		/// <li><seealso cref="#getSupportUpperBound()"/> for {@code p = 1}, and</li>
		/// <li><seealso cref="#solveInverseCumulativeProbability(double, int, int)"/> for
		///     {@code 0 < p < 1}.</li>
		/// </ul>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int inverseCumulativeProbability(final double p) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual int inverseCumulativeProbability(double p)
		{
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}

			int lower = SupportLowerBound;
			if (p == 0.0)
			{
				return lower;
			}
			if (lower == int.MinValue)
			{
				if (checkedCumulativeProbability(lower) >= p)
				{
					return lower;
				}
			}
			else
			{
				lower -= 1; // this ensures cumulativeProbability(lower) < p, which
							// is important for the solving step
			}

			int upper = SupportUpperBound;
			if (p == 1.0)
			{
				return upper;
			}

			// use the one-sided Chebyshev inequality to narrow the bracket
			// cf. AbstractRealDistribution.inverseCumulativeProbability(double)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mu = getNumericalMean();
			double mu = NumericalMean;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sigma = org.apache.commons.math3.util.FastMath.sqrt(getNumericalVariance());
			double sigma = FastMath.sqrt(NumericalVariance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean chebyshevApplies = !(Double.isInfinite(mu) || Double.isNaN(mu) || Double.isInfinite(sigma) || Double.isNaN(sigma) || sigma == 0.0);
			bool chebyshevApplies = !(double.IsInfinity(mu) || double.IsNaN(mu) || double.IsInfinity(sigma) || double.IsNaN(sigma) || sigma == 0.0);
			if (chebyshevApplies)
			{
				double k = FastMath.sqrt((1.0 - p) / p);
				double tmp = mu - k * sigma;
				if (tmp > lower)
				{
					lower = ((int) FastMath.ceil(tmp)) - 1;
				}
				k = 1.0 / k;
				tmp = mu + k * sigma;
				if (tmp < upper)
				{
					upper = ((int) FastMath.ceil(tmp)) - 1;
				}
			}

			return solveInverseCumulativeProbability(p, lower, upper);
		}

		/// <summary>
		/// This is a utility function used by {@link
		/// #inverseCumulativeProbability(double)}. It assumes {@code 0 < p < 1} and
		/// that the inverse cumulative probability lies in the bracket {@code
		/// (lower, upper]}. The implementation does simple bisection to find the
		/// smallest {@code p}-quantile <code>inf{x in Z | P(X<=x) >= p}</code>.
		/// </summary>
		/// <param name="p"> the cumulative probability </param>
		/// <param name="lower"> a value satisfying {@code cumulativeProbability(lower) < p} </param>
		/// <param name="upper"> a value satisfying {@code p <= cumulativeProbability(upper)} </param>
		/// <returns> the smallest {@code p}-quantile of this distribution </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected int solveInverseCumulativeProbability(final double p, int lower, int upper)
		protected internal virtual int solveInverseCumulativeProbability(double p, int lower, int upper)
		{
			while (lower + 1 < upper)
			{
				int xm = (lower + upper) / 2;
				if (xm < lower || xm > upper)
				{
					/*
					 * Overflow.
					 * There will never be an overflow in both calculation methods
					 * for xm at the same time
					 */
					xm = lower + (upper - lower) / 2;
				}

				double pm = checkedCumulativeProbability(xm);
				if (pm >= p)
				{
					upper = xm;
				}
				else
				{
					lower = xm;
				}
			}
			return upper;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void reseedRandomGenerator(long seed)
		{
			random.Seed = seed;
			randomData.reSeed(seed);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The default implementation uses the
		/// <a href="http://en.wikipedia.org/wiki/Inverse_transform_sampling">
		/// inversion method</a>.
		/// </summary>
		public virtual int sample()
		{
			return inverseCumulativeProbability(random.NextDouble());
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The default implementation generates the sample by calling
		/// <seealso cref="#sample()"/> in a loop.
		/// </summary>
		public virtual int[] sample(int sampleSize)
		{
			if (sampleSize <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize);
			}
			int[] @out = new int[sampleSize];
			for (int i = 0; i < sampleSize; i++)
			{
				@out[i] = sample();
			}
			return @out;
		}

		/// <summary>
		/// Computes the cumulative probability function and checks for {@code NaN}
		/// values returned. Throws {@code MathInternalError} if the value is
		/// {@code NaN}. Rethrows any exception encountered evaluating the cumulative
		/// probability function. Throws {@code MathInternalError} if the cumulative
		/// probability function returns {@code NaN}.
		/// </summary>
		/// <param name="argument"> input value </param>
		/// <returns> the cumulative probability </returns>
		/// <exception cref="MathInternalError"> if the cumulative probability is {@code NaN} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double checkedCumulativeProbability(int argument) throws org.apache.commons.math3.exception.MathInternalError
		private double checkedCumulativeProbability(int argument)
		{
			double result = double.NaN;
			result = cumulativeProbability(argument);
			if (double.IsNaN(result))
			{
				throw new MathInternalError(LocalizedFormats.DISCRETE_CUMULATIVE_PROBABILITY_RETURNED_NAN, argument);
			}
			return result;
		}

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according to
		/// this distribution, this method returns {@code log(P(X = x))}, where
		/// {@code log} is the natural logarithm. In other words, this method
		/// represents the logarithm of the probability mass function (PMF) for the
		/// distribution. Note that due to the floating point precision and
		/// under/overflow issues, this method will for some distributions be more
		/// precise and faster than computing the logarithm of
		/// <seealso cref="#probability(int)"/>.
		/// <p>
		/// The default implementation simply computes the logarithm of {@code probability(x)}.</p>
		/// </summary>
		/// <param name="x"> the point at which the PMF is evaluated </param>
		/// <returns> the logarithm of the value of the probability mass function at {@code x} </returns>
		public virtual double logProbability(int x)
		{
			return FastMath.log(probability(x));
		}
	}

}