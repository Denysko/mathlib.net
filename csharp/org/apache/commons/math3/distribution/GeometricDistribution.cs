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
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of the geometric distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Geometric_distribution">Geometric distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/GeometricDistribution.html">Geometric Distribution (MathWorld)</a>
	/// @version $Id: GeometricDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $
	/// @since 3.3 </seealso>
	public class GeometricDistribution : AbstractIntegerDistribution
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20130507L;
		/// <summary>
		/// The probability of success. </summary>
		private readonly double probabilityOfSuccess;

		/// <summary>
		/// Create a geometric distribution with the given probability of success.
		/// </summary>
		/// <param name="p"> probability of success. </param>
		/// <exception cref="OutOfRangeException"> if {@code p <= 0} or {@code p > 1}. </exception>
		public GeometricDistribution(double p) : this(new Well19937c(), p)
		{
		}

		/// <summary>
		/// Creates a geometric distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="p"> Probability of success. </param>
		/// <exception cref="OutOfRangeException"> if {@code p <= 0} or {@code p > 1}. </exception>
		public GeometricDistribution(RandomGenerator rng, double p) : base(rng)
		{

			if (p <= 0 || p > 1)
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_RANGE_LEFT, p, 0, 1);
			}

			probabilityOfSuccess = p;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = probabilityOfSuccess;
				double p = probabilityOfSuccess;
				ret = FastMath.pow(1 - p, x) * p;
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = probabilityOfSuccess;
				double p = probabilityOfSuccess;
				ret = x * FastMath.log1p(-p) + FastMath.log(p);
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = probabilityOfSuccess;
				double p = probabilityOfSuccess;
				ret = 1.0 - FastMath.pow(1 - p, x + 1);
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For probability parameter {@code p}, the mean is {@code (1 - p) / p}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double p = probabilityOfSuccess;
				double p = probabilityOfSuccess;
				return (1 - p) / p;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For probability parameter {@code p}, the variance is
		/// {@code (1 - p) / (p * p)}.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double p = probabilityOfSuccess;
				double p = probabilityOfSuccess;
				return (1 - p) / (p * p);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0.
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
		/// The upper bound of the support is infinite (which we approximate as
		/// {@code Integer.MAX_VALUE}).
		/// </summary>
		/// <returns> upper bound of the support (always Integer.MAX_VALUE) </returns>
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