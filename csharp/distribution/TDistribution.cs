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

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Beta = org.apache.commons.math3.special.Beta;
	using Gamma = org.apache.commons.math3.special.Gamma;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of Student's t-distribution.
	/// </summary>
	/// <seealso cref= "<a href='http://en.wikipedia.org/wiki/Student&apos;s_t-distribution'>Student's t-distribution (Wikipedia)</a>" </seealso>
	/// <seealso cref= "<a href='http://mathworld.wolfram.com/Studentst-Distribution.html'>Student's t-distribution (MathWorld)</a>"
	/// @version $Id: TDistribution.java 1534358 2013-10-21 20:13:52Z tn $ </seealso>
	public class TDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -5852615386664158222L;
		/// <summary>
		/// The degrees of freedom. </summary>
		private readonly double degreesOfFreedom;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Create a t distribution using the given degrees of freedom.
		/// </summary>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code degreesOfFreedom <= 0} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TDistribution(double degreesOfFreedom) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public TDistribution(double degreesOfFreedom) : this(degreesOfFreedom, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create a t distribution using the given degrees of freedom and the
		/// specified inverse cumulative probability absolute accuracy.
		/// </summary>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates
		/// (defaults to <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code degreesOfFreedom <= 0}
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TDistribution(double degreesOfFreedom, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public TDistribution(double degreesOfFreedom, double inverseCumAccuracy) : this(new Well19937c(), degreesOfFreedom, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a t distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code degreesOfFreedom <= 0}
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TDistribution(org.apache.commons.math3.random.RandomGenerator rng, double degreesOfFreedom) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public TDistribution(RandomGenerator rng, double degreesOfFreedom) : this(rng, degreesOfFreedom, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a t distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="degreesOfFreedom"> Degrees of freedom. </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates
		/// (defaults to <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code degreesOfFreedom <= 0}
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TDistribution(org.apache.commons.math3.random.RandomGenerator rng, double degreesOfFreedom, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		public TDistribution(RandomGenerator rng, double degreesOfFreedom, double inverseCumAccuracy) : base(rng)
		{

			if (degreesOfFreedom <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.DEGREES_OF_FREEDOM, degreesOfFreedom);
			}
			this.degreesOfFreedom = degreesOfFreedom;
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Access the degrees of freedom.
		/// </summary>
		/// <returns> the degrees of freedom. </returns>
		public virtual double DegreesOfFreedom
		{
			get
			{
				return degreesOfFreedom;
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
//ORIGINAL LINE: final double n = degreesOfFreedom;
			double n = degreesOfFreedom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nPlus1Over2 = (n + 1) / 2;
			double nPlus1Over2 = (n + 1) / 2;
			return Gamma.logGamma(nPlus1Over2) - 0.5 * (FastMath.log(FastMath.PI) + FastMath.log(n)) - Gamma.logGamma(n / 2) - nPlus1Over2 * FastMath.log(1 + x * x / n);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(double x)
		{
			double ret;
			if (x == 0)
			{
				ret = 0.5;
			}
			else
			{
				double t = Beta.regularizedBeta(degreesOfFreedom / (degreesOfFreedom + (x * x)), 0.5 * degreesOfFreedom, 0.5);
				if (x < 0.0)
				{
					ret = 0.5 * t;
				}
				else
				{
					ret = 1.0 - 0.5 * t;
				}
			}

			return ret;
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
		/// For degrees of freedom parameter {@code df}, the mean is
		/// <ul>
		///  <li>if {@code df > 1} then {@code 0},</li>
		/// <li>else undefined ({@code Double.NaN}).</li>
		/// </ul>
		/// </summary>
		public override double NumericalMean
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double df = getDegreesOfFreedom();
				double df = DegreesOfFreedom;
    
				if (df > 1)
				{
					return 0;
				}
    
				return double.NaN;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For degrees of freedom parameter {@code df}, the variance is
		/// <ul>
		///  <li>if {@code df > 2} then {@code df / (df - 2)},</li>
		///  <li>if {@code 1 < df <= 2} then positive infinity
		///  ({@code Double.POSITIVE_INFINITY}),</li>
		///  <li>else undefined ({@code Double.NaN}).</li>
		/// </ul>
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double df = getDegreesOfFreedom();
				double df = DegreesOfFreedom;
    
				if (df > 2)
				{
					return df / (df - 2);
				}
    
				if (df > 1 && df <= 2)
				{
					return double.PositiveInfinity;
				}
    
				return double.NaN;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always negative infinity no matter the
		/// parameters.
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
		/// The upper bound of the support is always positive infinity no matter the
		/// parameters.
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
	}

}