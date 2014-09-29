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
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Erf = mathlib.special.Erf;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the normal (gaussian) distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Normal_distribution">Normal distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/NormalDistribution.html">Normal distribution (MathWorld)</a>
	/// @version $Id: NormalDistribution.java 1535290 2013-10-24 06:58:32Z luc $ </seealso>
	public class NormalDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 8589540077390120676L;
		/// <summary>
		/// &radic;(2) </summary>
		private static readonly double SQRT2 = FastMath.sqrt(2.0);
		/// <summary>
		/// Mean of this distribution. </summary>
		private readonly double mean;
		/// <summary>
		/// Standard deviation of this distribution. </summary>
		private readonly double standardDeviation;
		/// <summary>
		/// The value of {@code log(sd) + 0.5*log(2*pi)} stored for faster computation. </summary>
		private readonly double logStandardDeviationPlusHalfLog2Pi;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Create a normal distribution with mean equal to zero and standard
		/// deviation equal to one.
		/// </summary>
		public NormalDistribution() : this(0, 1)
		{
		}

		/// <summary>
		/// Create a normal distribution using the given mean and standard deviation.
		/// </summary>
		/// <param name="mean"> Mean for this distribution. </param>
		/// <param name="sd"> Standard deviation for this distribution. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NormalDistribution(double mean, double sd) throws mathlib.exception.NotStrictlyPositiveException
		public NormalDistribution(double mean, double sd) : this(mean, sd, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create a normal distribution using the given mean, standard deviation and
		/// inverse cumulative distribution accuracy.
		/// </summary>
		/// <param name="mean"> Mean for this distribution. </param>
		/// <param name="sd"> Standard deviation for this distribution. </param>
		/// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NormalDistribution(double mean, double sd, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public NormalDistribution(double mean, double sd, double inverseCumAccuracy) : this(new Well19937c(), mean, sd, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a normal distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="mean"> Mean for this distribution. </param>
		/// <param name="sd"> Standard deviation for this distribution. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NormalDistribution(mathlib.random.RandomGenerator rng, double mean, double sd) throws mathlib.exception.NotStrictlyPositiveException
		public NormalDistribution(RandomGenerator rng, double mean, double sd) : this(rng, mean, sd, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a normal distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="mean"> Mean for this distribution. </param>
		/// <param name="sd"> Standard deviation for this distribution. </param>
		/// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NormalDistribution(mathlib.random.RandomGenerator rng, double mean, double sd, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public NormalDistribution(RandomGenerator rng, double mean, double sd, double inverseCumAccuracy) : base(rng)
		{

			if (sd <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.STANDARD_DEVIATION, sd);
			}

			this.mean = mean;
			standardDeviation = sd;
			logStandardDeviationPlusHalfLog2Pi = FastMath.log(sd) + 0.5 * FastMath.log(2 * FastMath.PI);
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Access the mean.
		/// </summary>
		/// <returns> the mean for this distribution. </returns>
		public virtual double Mean
		{
			get
			{
				return mean;
			}
		}

		/// <summary>
		/// Access the standard deviation.
		/// </summary>
		/// <returns> the standard deviation for this distribution. </returns>
		public virtual double StandardDeviation
		{
			get
			{
				return standardDeviation;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double density(double x)
		{
			return FastMath.exp(logDensity(x));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double logDensity(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x0 = x - mean;
			double x0 = x - mean;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x1 = x0 / standardDeviation;
			double x1 = x0 / standardDeviation;
			return -0.5 * x1 * x1 - logStandardDeviationPlusHalfLog2Pi;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// If {@code x} is more than 40 standard deviations from the mean, 0 or 1
		/// is returned, as in these cases the actual value is within
		/// {@code Double.MIN_VALUE} of 0 or 1.
		/// </summary>
		public override double cumulativeProbability(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dev = x - mean;
			double dev = x - mean;
			if (FastMath.abs(dev) > 40 * standardDeviation)
			{
				return dev < 0 ? 0.0d : 1.0d;
			}
			return 0.5 * (1 + Erf.erf(dev / (standardDeviation * SQRT2)));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.2
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double inverseCumulativeProbability(double p)
		{
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}
			return mean + standardDeviation * SQRT2 * Erf.erfInv(2 * p - 1);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// @deprecated See <seealso cref="RealDistribution#cumulativeProbability(double,double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override@Deprecated public double cumulativeProbability(double x0, double x1) throws mathlib.exception.NumberIsTooLargeException
		Deprecated public override double cumulativeProbability(double x0, double x1)
		{
			return probability(x0, x1);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double probability(double x0, double x1) throws mathlib.exception.NumberIsTooLargeException
		public override double probability(double x0, double x1)
		{
			if (x0 > x1)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT, x0, x1, true);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double denom = standardDeviation * SQRT2;
			double denom = standardDeviation * SQRT2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v0 = (x0 - mean) / denom;
			double v0 = (x0 - mean) / denom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v1 = (x1 - mean) / denom;
			double v1 = (x1 - mean) / denom;
			return 0.5 * Erf.erf(v0, v1);
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
		/// For mean parameter {@code mu}, the mean is {@code mu}.
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
		/// For standard deviation parameter {@code s}, the variance is {@code s^2}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double s = getStandardDeviation();
				double s = StandardDeviation;
				return s * s;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always negative infinity
		/// no matter the parameters.
		/// </summary>
		/// <returns> lower bound of the support (always
		/// {@code Double.NEGATIVE_INFINITY}) </returns>
		public override double SupportLowerBound
		{
			get
			{
				return double.NegativeInfinity;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is always positive infinity
		/// no matter the parameters.
		/// </summary>
		/// <returns> upper bound of the support (always
		/// {@code Double.POSITIVE_INFINITY}) </returns>
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

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double sample()
		{
			return standardDeviation * random.nextGaussian() + mean;
		}
	}

}