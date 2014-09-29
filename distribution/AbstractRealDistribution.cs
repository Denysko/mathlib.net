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

	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using UnivariateSolverUtils = mathlib.analysis.solvers.UnivariateSolverUtils;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using RandomDataImpl = mathlib.random.RandomDataImpl;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Base class for probability distributions on the reals.
	/// Default implementations are provided for some of the methods
	/// that do not vary from distribution to distribution.
	/// 
	/// @version $Id: AbstractRealDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $
	/// @since 3.0
	/// </summary>
	[Serializable]
	public abstract class AbstractRealDistribution : RealDistribution
	{
		public abstract bool SupportConnected {get;}
		public abstract bool SupportUpperBoundInclusive {get;}
		public abstract bool SupportLowerBoundInclusive {get;}
		public abstract double SupportUpperBound {get;}
		public abstract double SupportLowerBound {get;}
		public abstract double NumericalVariance {get;}
		public abstract double NumericalMean {get;}
		public abstract double cumulativeProbability(double x);
		public abstract double density(double x);
		/// <summary>
		/// Default accuracy. </summary>
		public const double SOLVER_DEFAULT_ABSOLUTE_ACCURACY = 1e-6;
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -38038050983108802L;
		 /// <summary>
		 /// RandomData instance used to generate samples from the distribution. </summary>
		 /// @deprecated As of 3.1, to be removed in 4.0. Please use the
		 /// <seealso cref="#random"/> instance variable instead. 
		[Obsolete("As of 3.1, to be removed in 4.0. Please use the")]
		protected internal RandomDataImpl randomData = new RandomDataImpl();

		/// <summary>
		/// RNG instance used to generate samples from the distribution.
		/// @since 3.1
		/// </summary>
		protected internal readonly RandomGenerator random;

		/// <summary>
		/// Solver absolute accuracy for inverse cumulative computation </summary>
		private double solverAbsoluteAccuracy = SOLVER_DEFAULT_ABSOLUTE_ACCURACY;

		/// @deprecated As of 3.1, to be removed in 4.0. Please use
		/// <seealso cref="#AbstractRealDistribution(RandomGenerator)"/> instead. 
		[Obsolete("As of 3.1, to be removed in 4.0. Please use")]
		protected internal AbstractRealDistribution()
		{
			// Legacy users are only allowed to access the deprecated "randomData".
			// New users are forbidden to use this constructor.
			random = null;
		}
		/// <param name="rng"> Random number generator.
		/// @since 3.1 </param>
		protected internal AbstractRealDistribution(RandomGenerator rng)
		{
			random = rng;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The default implementation uses the identity
		/// <p>{@code P(x0 < X <= x1) = P(X <= x1) - P(X <= x0)}</p>
		/// </summary>
		/// @deprecated As of 3.1 (to be removed in 4.0). Please use
		/// <seealso cref="#probability(double,double)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("As of 3.1 (to be removed in 4.0). Please use") public double cumulativeProbability(double x0, double x1) throws mathlib.exception.NumberIsTooLargeException
		[Obsolete("As of 3.1 (to be removed in 4.0). Please use")]
		public virtual double cumulativeProbability(double x0, double x1)
		{
			return probability(x0, x1);
		}

		/// <summary>
		/// For a random variable {@code X} whose values are distributed according
		/// to this distribution, this method returns {@code P(x0 < X <= x1)}.
		/// </summary>
		/// <param name="x0"> Lower bound (excluded). </param>
		/// <param name="x1"> Upper bound (included). </param>
		/// <returns> the probability that a random variable with this distribution
		/// takes a value between {@code x0} and {@code x1}, excluding the lower
		/// and including the upper endpoint. </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code x0 > x1}.
		/// 
		/// The default implementation uses the identity
		/// {@code P(x0 < X <= x1) = P(X <= x1) - P(X <= x0)}
		/// 
		/// @since 3.1 </exception>
		public virtual double probability(double x0, double x1)
		{
			if (x0 > x1)
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
		/// <li><seealso cref="#getSupportUpperBound()"/> for {@code p = 1}.</li>
		/// </ul>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double inverseCumulativeProbability(final double p) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double inverseCumulativeProbability(double p)
		{
			/*
			 * IMPLEMENTATION NOTES
			 * --------------------
			 * Where applicable, use is made of the one-sided Chebyshev inequality
			 * to bracket the root. This inequality states that
			 * P(X - mu >= k * sig) <= 1 / (1 + k^2),
			 * mu: mean, sig: standard deviation. Equivalently
			 * 1 - P(X < mu + k * sig) <= 1 / (1 + k^2),
			 * F(mu + k * sig) >= k^2 / (1 + k^2).
			 *
			 * For k = sqrt(p / (1 - p)), we find
			 * F(mu + k * sig) >= p,
			 * and (mu + k * sig) is an upper-bound for the root.
			 *
			 * Then, introducing Y = -X, mean(Y) = -mu, sd(Y) = sig, and
			 * P(Y >= -mu + k * sig) <= 1 / (1 + k^2),
			 * P(-X >= -mu + k * sig) <= 1 / (1 + k^2),
			 * P(X <= mu - k * sig) <= 1 / (1 + k^2),
			 * F(mu - k * sig) <= 1 / (1 + k^2).
			 *
			 * For k = sqrt((1 - p) / p), we find
			 * F(mu - k * sig) <= p,
			 * and (mu - k * sig) is a lower-bound for the root.
			 *
			 * In cases where the Chebyshev inequality does not apply, geometric
			 * progressions 1, 2, 4, ... and -1, -2, -4, ... are used to bracket
			 * the root.
			 */
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}

			double lowerBound = SupportLowerBound;
			if (p == 0.0)
			{
				return lowerBound;
			}

			double upperBound = SupportUpperBound;
			if (p == 1.0)
			{
				return upperBound;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mu = getNumericalMean();
			double mu = NumericalMean;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sig = mathlib.util.FastMath.sqrt(getNumericalVariance());
			double sig = FastMath.sqrt(NumericalVariance);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean chebyshevApplies;
			bool chebyshevApplies;
			chebyshevApplies = !(double.IsInfinity(mu) || double.IsNaN(mu) || double.IsInfinity(sig) || double.IsNaN(sig));

			if (lowerBound == double.NegativeInfinity)
			{
				if (chebyshevApplies)
				{
					lowerBound = mu - sig * FastMath.sqrt((1.0 - p) / p);
				}
				else
				{
					lowerBound = -1.0;
					while (cumulativeProbability(lowerBound) >= p)
					{
						lowerBound *= 2.0;
					}
				}
			}

			if (upperBound == double.PositiveInfinity)
			{
				if (chebyshevApplies)
				{
					upperBound = mu + sig * FastMath.sqrt(p / (1.0 - p));
				}
				else
				{
					upperBound = 1.0;
					while (cumulativeProbability(upperBound) < p)
					{
						upperBound *= 2.0;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.UnivariateFunction toSolve = new mathlib.analysis.UnivariateFunction()
			UnivariateFunction toSolve = new UnivariateFunctionAnonymousInnerClassHelper(this, p);

			double x = UnivariateSolverUtils.solve(toSolve, lowerBound, upperBound, SolverAbsoluteAccuracy);

			if (!SupportConnected)
			{
				/* Test for plateau. */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = getSolverAbsoluteAccuracy();
				double dx = SolverAbsoluteAccuracy;
				if (x - dx >= SupportLowerBound)
				{
					double px = cumulativeProbability(x);
					if (cumulativeProbability(x - dx) == px)
					{
						upperBound = x;
						while (upperBound - lowerBound > dx)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double midPoint = 0.5 * (lowerBound + upperBound);
							double midPoint = 0.5 * (lowerBound + upperBound);
							if (cumulativeProbability(midPoint) < px)
							{
								lowerBound = midPoint;
							}
							else
							{
								upperBound = midPoint;
							}
						}
						return upperBound;
					}
				}
			}
			return x;
		}

		private class UnivariateFunctionAnonymousInnerClassHelper : UnivariateFunction
		{
			private readonly AbstractRealDistribution outerInstance;

			private double p;

			public UnivariateFunctionAnonymousInnerClassHelper(AbstractRealDistribution outerInstance, double p)
			{
				this.outerInstance = outerInstance;
				this.p = p;
			}


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double x)
			public virtual double value(double x)
			{
				return outerInstance.cumulativeProbability(x) - p;
			}
		}

		/// <summary>
		/// Returns the solver absolute accuracy for inverse cumulative computation.
		/// You can override this method in order to use a Brent solver with an
		/// absolute accuracy different from the default.
		/// </summary>
		/// <returns> the maximum absolute error in inverse cumulative probability estimates </returns>
		protected internal virtual double SolverAbsoluteAccuracy
		{
			get
			{
				return solverAbsoluteAccuracy;
			}
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
		/// inversion method.
		/// </a>
		/// </summary>
		public virtual double sample()
		{
			return inverseCumulativeProbability(random.NextDouble());
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The default implementation generates the sample by calling
		/// <seealso cref="#sample()"/> in a loop.
		/// </summary>
		public virtual double[] sample(int sampleSize)
		{
			if (sampleSize <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize);
			}
			double[] @out = new double[sampleSize];
			for (int i = 0; i < sampleSize; i++)
			{
				@out[i] = sample();
			}
			return @out;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <returns> zero.
		/// @since 3.1 </returns>
		public virtual double probability(double x)
		{
			return 0d;
		}

		/// <summary>
		/// Returns the natural logarithm of the probability density function (PDF) of this distribution
		/// evaluated at the specified point {@code x}. In general, the PDF is the derivative of the
		/// <seealso cref="#cumulativeProbability(double) CDF"/>. If the derivative does not exist at {@code x},
		/// then an appropriate replacement should be returned, e.g. {@code Double.POSITIVE_INFINITY},
		/// {@code Double.NaN}, or the limit inferior or limit superior of the difference quotient. Note
		/// that due to the floating point precision and under/overflow issues, this method will for some
		/// distributions be more precise and faster than computing the logarithm of
		/// <seealso cref="#density(double)"/>. The default implementation simply computes the logarithm of
		/// {@code density(x)}.
		/// </summary>
		/// <param name="x"> the point at which the PDF is evaluated </param>
		/// <returns> the logarithm of the value of the probability density function at point {@code x} </returns>
		public virtual double logDensity(double x)
		{
			return FastMath.log(density(x));
		}
	}


}