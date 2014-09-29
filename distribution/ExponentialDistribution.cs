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
	using CombinatoricsUtils = mathlib.util.CombinatoricsUtils;
	using FastMath = mathlib.util.FastMath;
	using ResizableDoubleArray = mathlib.util.ResizableDoubleArray;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the exponential distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Exponential_distribution">Exponential distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/ExponentialDistribution.html">Exponential distribution (MathWorld)</a>
	/// @version $Id: ExponentialDistribution.java 1534358 2013-10-21 20:13:52Z tn $ </seealso>
	public class ExponentialDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 2401296428283614780L;
		/// <summary>
		/// Used when generating Exponential samples.
		/// Table containing the constants
		/// q_i = sum_{j=1}^i (ln 2)^j/j! = ln 2 + (ln 2)^2/2 + ... + (ln 2)^i/i!
		/// until the largest representable fraction below 1 is exceeded.
		/// 
		/// Note that
		/// 1 = 2 - 1 = exp(ln 2) - 1 = sum_{n=1}^infty (ln 2)^n / n!
		/// thus q_i -> 1 as i -> +inf,
		/// so the higher i, the closer to one we get (the series is not alternating).
		/// 
		/// By trying, n = 16 in Java is enough to reach 1.0.
		/// </summary>
		private static readonly double[] EXPONENTIAL_SA_QI;
		/// <summary>
		/// The mean of this distribution. </summary>
		private readonly double mean;
		/// <summary>
		/// The logarithm of the mean, stored to reduce computing time. * </summary>
		private readonly double logMean;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Initialize tables.
		/// </summary>
		static ExponentialDistribution()
		{
			/// <summary>
			/// Filling EXPONENTIAL_SA_QI table.
			/// Note that we don't want qi = 0 in the table.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double LN2 = mathlib.util.FastMath.log(2);
			double LN2 = FastMath.log(2);
			double qi = 0;
			int i = 1;

			/// <summary>
			/// ArithmeticUtils provides factorials up to 20, so let's use that
			/// limit together with Precision.EPSILON to generate the following
			/// code (a priori, we know that there will be 16 elements, but it is
			/// better to not hardcode it).
			/// </summary>
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.util.ResizableDoubleArray ra = new mathlib.util.ResizableDoubleArray(20);
			ResizableDoubleArray ra = new ResizableDoubleArray(20);

			while (qi < 1)
			{
				qi += FastMath.pow(LN2, i) / CombinatoricsUtils.factorial(i);
				ra.addElement(qi);
				++i;
			}

			EXPONENTIAL_SA_QI = ra.Elements;
		}

		/// <summary>
		/// Create an exponential distribution with the given mean. </summary>
		/// <param name="mean"> mean of this distribution. </param>
		public ExponentialDistribution(double mean) : this(mean, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create an exponential distribution with the given mean.
		/// </summary>
		/// <param name="mean"> Mean of this distribution. </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code mean <= 0}.
		/// @since 2.1 </exception>
		public ExponentialDistribution(double mean, double inverseCumAccuracy) : this(new Well19937c(), mean, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates an exponential distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="mean"> Mean of this distribution. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code mean <= 0}.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ExponentialDistribution(mathlib.random.RandomGenerator rng, double mean) throws mathlib.exception.NotStrictlyPositiveException
		public ExponentialDistribution(RandomGenerator rng, double mean) : this(rng, mean, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates an exponential distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="mean"> Mean of this distribution. </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code mean <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ExponentialDistribution(mathlib.random.RandomGenerator rng, double mean, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public ExponentialDistribution(RandomGenerator rng, double mean, double inverseCumAccuracy) : base(rng)
		{

			if (mean <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.MEAN, mean);
			}
			this.mean = mean;
			logMean = FastMath.log(mean);
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Access the mean.
		/// </summary>
		/// <returns> the mean. </returns>
		public virtual double Mean
		{
			get
			{
				return mean;
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
			if (x < 0)
			{
				return double.NegativeInfinity;
			}
			return -x / mean - logMean;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The implementation of this method is based on:
		/// <ul>
		/// <li>
		/// <a href="http://mathworld.wolfram.com/ExponentialDistribution.html">
		/// Exponential Distribution</a>, equation (1).</li>
		/// </ul>
		/// </summary>
		public override double cumulativeProbability(double x)
		{
			double ret;
			if (x <= 0.0)
			{
				ret = 0.0;
			}
			else
			{
				ret = 1.0 - FastMath.exp(-x / mean);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns {@code 0} when {@code p= = 0} and
		/// {@code Double.POSITIVE_INFINITY} when {@code p == 1}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(double p) throws mathlib.exception.OutOfRangeException
		public override double inverseCumulativeProbability(double p)
		{
			double ret;

			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0.0, 1.0);
			}
			else if (p == 1.0)
			{
				ret = double.PositiveInfinity;
			}
			else
			{
				ret = -mean * FastMath.log(1.0 - p);
			}

			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p><strong>Algorithm Description</strong>: this implementation uses the
		/// <a href="http://www.jesus.ox.ac.uk/~clifford/a5/chap1/node5.html">
		/// Inversion Method</a> to generate exponentially distributed random values
		/// from uniform deviates.</p>
		/// </summary>
		/// <returns> a random value.
		/// @since 2.2 </returns>
		public override double sample()
		{
			// Step 1:
			double a = 0;
			double u = random.NextDouble();

			// Step 2 and 3:
			while (u < 0.5)
			{
				a += EXPONENTIAL_SA_QI[0];
				u *= 2;
			}

			// Step 4 (now u >= 0.5):
			u += u - 1;

			// Step 5:
			if (u <= EXPONENTIAL_SA_QI[0])
			{
				return mean * (a + u);
			}

			// Step 6:
			int i = 0; // Should be 1, be we iterate before it in while using 0
			double u2 = random.NextDouble();
			double umin = u2;

			// Step 7 and 8:
			do
			{
				++i;
				u2 = random.NextDouble();

				if (u2 < umin)
				{
					umin = u2;
				}

				// Step 8:
			} while (u > EXPONENTIAL_SA_QI[i]); // Ensured to exit since EXPONENTIAL_SA_QI[MAX] = 1

			return mean * (a + umin * EXPONENTIAL_SA_QI[0]);
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
		/// For mean parameter {@code k}, the mean is {@code k}.
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
		/// For mean parameter {@code k}, the variance is {@code k^2}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double m = getMean();
				double m = Mean;
				return m * m;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the mean parameter.
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
		/// The upper bound of the support is always positive infinity
		/// no matter the mean parameter.
		/// </summary>
		/// <returns> upper bound of the support (always Double.POSITIVE_INFINITY) </returns>
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