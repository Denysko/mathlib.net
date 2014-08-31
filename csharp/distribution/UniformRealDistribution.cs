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

	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of the uniform real distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Uniform_distribution_(continuous)"
	/// >Uniform distribution (continuous), at Wikipedia</a>
	/// 
	/// @version $Id: UniformRealDistribution.java 1462020 2013-03-28 10:24:45Z luc $
	/// @since 3.0 </seealso>
	public class UniformRealDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy. </summary>
		/// @deprecated as of 3.2 not used anymore, will be removed in 4.0 
		[Obsolete("as of 3.2 not used anymore, will be removed in 4.0")]
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20120109L;
		/// <summary>
		/// Lower bound of this distribution (inclusive). </summary>
		private readonly double lower;
		/// <summary>
		/// Upper bound of this distribution (exclusive). </summary>
		private readonly double upper;

		/// <summary>
		/// Create a standard uniform real distribution with lower bound (inclusive)
		/// equal to zero and upper bound (exclusive) equal to one.
		/// </summary>
		public UniformRealDistribution() : this(0, 1)
		{
		}

		/// <summary>
		/// Create a uniform real distribution using the given lower and upper
		/// bounds.
		/// </summary>
		/// <param name="lower"> Lower bound of this distribution (inclusive). </param>
		/// <param name="upper"> Upper bound of this distribution (exclusive). </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UniformRealDistribution(double lower, double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		public UniformRealDistribution(double lower, double upper) : this(new Well19937c(), lower, upper)
		{
		}

		/// <summary>
		/// Create a uniform distribution.
		/// </summary>
		/// <param name="lower"> Lower bound of this distribution (inclusive). </param>
		/// <param name="upper"> Upper bound of this distribution (exclusive). </param>
		/// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
		/// @deprecated as of 3.2, inverse CDF is now calculated analytically, use
		///             <seealso cref="#UniformRealDistribution(double, double)"/> instead. 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.2, inverse CDF is now calculated analytically, use") public UniformRealDistribution(double lower, double upper, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		[Obsolete("as of 3.2, inverse CDF is now calculated analytically, use")]
		public UniformRealDistribution(double lower, double upper, double inverseCumAccuracy) : this(new Well19937c(), lower, upper)
		{
		}

		/// <summary>
		/// Creates a uniform distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="lower"> Lower bound of this distribution (inclusive). </param>
		/// <param name="upper"> Upper bound of this distribution (exclusive). </param>
		/// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}.
		/// @since 3.1 </exception>
		/// @deprecated as of 3.2, inverse CDF is now calculated analytically, use
		///             <seealso cref="#UniformRealDistribution(RandomGenerator, double, double)"/>
		///             instead. 
		[Obsolete("as of 3.2, inverse CDF is now calculated analytically, use")]
		public UniformRealDistribution(RandomGenerator rng, double lower, double upper, double inverseCumAccuracy) : this(rng, lower, upper)
		{
		}

		/// <summary>
		/// Creates a uniform distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="lower"> Lower bound of this distribution (inclusive). </param>
		/// <param name="upper"> Upper bound of this distribution (exclusive). </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UniformRealDistribution(org.apache.commons.math3.random.RandomGenerator rng, double lower, double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		public UniformRealDistribution(RandomGenerator rng, double lower, double upper) : base(rng)
		{
			if (lower >= upper)
			{
				throw new NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper, false);
			}

			this.lower = lower;
			this.upper = upper;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double density(double x)
		{
			if (x < lower || x > upper)
			{
				return 0.0;
			}
			return 1 / (upper - lower);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(double x)
		{
			if (x <= lower)
			{
				return 0;
			}
			if (x >= upper)
			{
				return 1;
			}
			return (x - lower) / (upper - lower);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double inverseCumulativeProbability(double p)
		{
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}
			return p * (upper - lower) + lower;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For lower bound {@code lower} and upper bound {@code upper}, the mean is
		/// {@code 0.5 * (lower + upper)}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return 0.5 * (lower + upper);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For lower bound {@code lower} and upper bound {@code upper}, the
		/// variance is {@code (upper - lower)^2 / 12}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				double ul = upper - lower;
				return ul * ul / 12;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is equal to the lower bound parameter
		/// of the distribution.
		/// </summary>
		/// <returns> lower bound of the support </returns>
		public override double SupportLowerBound
		{
			get
			{
				return lower;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The upper bound of the support is equal to the upper bound parameter
		/// of the distribution.
		/// </summary>
		/// <returns> upper bound of the support </returns>
		public override double SupportUpperBound
		{
			get
			{
				return upper;
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
				return true;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u = random.nextDouble();
			double u = random.NextDouble();
			return u * upper + (1 - u) * lower;
		}
	}

}