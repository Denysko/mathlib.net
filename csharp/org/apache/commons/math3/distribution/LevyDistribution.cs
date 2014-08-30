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
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Erf = org.apache.commons.math3.special.Erf;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class implements the <a href="http://en.wikipedia.org/wiki/L%C3%A9vy_distribution">
	/// L&eacute;vy distribution</a>.
	/// 
	/// @version $Id: LevyDistribution.java 1533974 2013-10-20 20:42:41Z psteitz $
	/// @since 3.2
	/// </summary>
	public class LevyDistribution : AbstractRealDistribution
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20130314L;

		/// <summary>
		/// Location parameter. </summary>
		private readonly double mu;

		/// <summary>
		/// Scale parameter. </summary>
		private readonly double c; // Setting this to 1 returns a cumProb of 1.0

		/// <summary>
		/// Half of c (for calculations). </summary>
		private readonly double halfC;

		/// <summary>
		/// Creates a LevyDistribution. </summary>
		/// <param name="rng"> random generator to be used for sampling </param>
		/// <param name="mu"> location </param>
		/// <param name="c"> scale parameter </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LevyDistribution(final org.apache.commons.math3.random.RandomGenerator rng, final double mu, final double c)
		public LevyDistribution(RandomGenerator rng, double mu, double c) : base(rng)
		{
			this.mu = mu;
			this.c = c;
			this.halfC = 0.5 * c;
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// From Wikipedia: The probability density function of the L&eacute;vy distribution
		/// over the domain is
		/// </p>
		/// <pre>
		/// f(x; &mu;, c) = &radic;(c / 2&pi;) * e<sup>-c / 2 (x - &mu;)</sup> / (x - &mu;)<sup>3/2</sup>
		/// </pre>
		/// <p>
		/// For this distribution, {@code X}, this method returns {@code P(X < x)}.
		/// If {@code x} is less than location parameter &mu;, {@code Double.NaN} is
		/// returned, as in these cases the distribution is not defined.
		/// </p>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double density(final double x)
		public override double density(double x)
		{
			if (x < mu)
			{
				return double.NaN;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = x - mu;
			double delta = x - mu;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = halfC / delta;
			double f = halfC / delta;
			return FastMath.sqrt(f / FastMath.PI) * FastMath.exp(-f) / delta;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// See documentation of <seealso cref="#density(double)"/> for computation details.
		/// </summary>
		public override double logDensity(double x)
		{
			if (x < mu)
			{
				return double.NaN;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = x - mu;
			double delta = x - mu;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = halfC / delta;
			double f = halfC / delta;
			return 0.5 * FastMath.log(f / FastMath.PI) - f - FastMath.log(delta);
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// From Wikipedia: the cumulative distribution function is
		/// </p>
		/// <pre>
		/// f(x; u, c) = erfc (&radic; (c / 2 (x - u )))
		/// </pre>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double cumulativeProbability(final double x)
		public override double cumulativeProbability(double x)
		{
			if (x < mu)
			{
				return double.NaN;
			}
			return Erf.erfc(FastMath.sqrt(halfC / (x - mu)));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double inverseCumulativeProbability(double p)
		{
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = org.apache.commons.math3.special.Erf.erfcInv(p);
			double t = Erf.erfcInv(p);
			return mu + halfC / (t * t);
		}

		/// <summary>
		/// Get the scale parameter of the distribution. </summary>
		/// <returns> scale parameter of the distribution </returns>
		public virtual double Scale
		{
			get
			{
				return c;
			}
		}

		/// <summary>
		/// Get the location parameter of the distribution. </summary>
		/// <returns> location parameter of the distribution </returns>
		public virtual double Location
		{
			get
			{
				return mu;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double NumericalMean
		{
			get
			{
				return double.PositiveInfinity;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double NumericalVariance
		{
			get
			{
				return double.PositiveInfinity;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double SupportLowerBound
		{
			get
			{
				return mu;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
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
				// there is a division by x-mu in the computation, so density
				// is not finite at lower bound, bound must be excluded
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportUpperBoundInclusive
		{
			get
			{
				// upper bound is infinite, so it must be excluded
				return false;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool SupportConnected
		{
			get
			{
				return true;
			}
		}

	}

}