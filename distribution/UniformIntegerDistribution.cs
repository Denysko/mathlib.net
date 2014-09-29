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

	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the uniform integer distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Uniform_distribution_(discrete)"
	/// >Uniform distribution (discrete), at Wikipedia</a>
	/// 
	/// @version $Id: UniformIntegerDistribution.java 1510924 2013-08-06 12:22:47Z erans $
	/// @since 3.0 </seealso>
	public class UniformIntegerDistribution : AbstractIntegerDistribution
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20120109L;
		/// <summary>
		/// Lower bound (inclusive) of this distribution. </summary>
		private readonly int lower;
		/// <summary>
		/// Upper bound (inclusive) of this distribution. </summary>
		private readonly int upper;

		/// <summary>
		/// Creates a new uniform integer distribution using the given lower and
		/// upper bounds (both inclusive).
		/// </summary>
		/// <param name="lower"> Lower bound (inclusive) of this distribution. </param>
		/// <param name="upper"> Upper bound (inclusive) of this distribution. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UniformIntegerDistribution(int lower, int upper) throws mathlib.exception.NumberIsTooLargeException
		public UniformIntegerDistribution(int lower, int upper) : this(new Well19937c(), lower, upper)
		{
		}

		/// <summary>
		/// Creates a new uniform integer distribution using the given lower and
		/// upper bounds (both inclusive).
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="lower"> Lower bound (inclusive) of this distribution. </param>
		/// <param name="upper"> Upper bound (inclusive) of this distribution. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UniformIntegerDistribution(mathlib.random.RandomGenerator rng, int lower, int upper) throws mathlib.exception.NumberIsTooLargeException
		public UniformIntegerDistribution(RandomGenerator rng, int lower, int upper) : base(rng)
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
		public override double probability(int x)
		{
			if (x < lower || x > upper)
			{
				return 0;
			}
			return 1.0 / (upper - lower + 1);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(int x)
		{
			if (x < lower)
			{
				return 0;
			}
			if (x > upper)
			{
				return 1;
			}
			return (x - lower + 1.0) / (upper - lower + 1.0);
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
		/// For lower bound {@code lower} and upper bound {@code upper}, and
		/// {@code n = upper - lower + 1}, the variance is {@code (n^2 - 1) / 12}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				double n = upper - lower + 1;
				return (n * n - 1) / 12.0;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is equal to the lower bound parameter
		/// of the distribution.
		/// </summary>
		/// <returns> lower bound of the support </returns>
		public override int SupportLowerBound
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
		public override int SupportUpperBound
		{
			get
			{
				return upper;
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
		public override int sample()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int max = (upper - lower) + 1;
			int max = (upper - lower) + 1;
			if (max <= 0)
			{
				// The range is too wide to fit in a positive int (larger
				// than 2^31); as it covers more than half the integer range,
				// we use a simple rejection method.
				while (true)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = random.nextInt();
					int r = random.Next();
					if (r >= lower && r <= upper)
					{
						return r;
					}
				}
			}
			else
			{
				// We can shift the range and directly generate a positive int.
				return lower + random.Next(max);
			}
		}
	}

}