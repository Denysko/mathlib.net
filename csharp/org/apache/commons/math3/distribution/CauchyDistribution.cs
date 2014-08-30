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
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using RandomGenerator = org.apache.commons.math3.random.RandomGenerator;
	using Well19937c = org.apache.commons.math3.random.Well19937c;

	/// <summary>
	/// Implementation of the Cauchy distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Cauchy_distribution">Cauchy distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/CauchyDistribution.html">Cauchy Distribution (MathWorld)</a>
	/// @since 1.1 (changed to concrete class in 3.0)
	/// @version $Id: CauchyDistribution.java 1519842 2013-09-03 20:38:59Z tn $ </seealso>
	public class CauchyDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 8589540077390120676L;
		/// <summary>
		/// The median of this distribution. </summary>
		private readonly double median;
		/// <summary>
		/// The scale of this distribution. </summary>
		private readonly double scale;
		/// <summary>
		/// Inverse cumulative probability accuracy </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Creates a Cauchy distribution with the median equal to zero and scale
		/// equal to one.
		/// </summary>
		public CauchyDistribution() : this(0, 1)
		{
		}

		/// <summary>
		/// Creates a Cauchy distribution using the given median and scale.
		/// </summary>
		/// <param name="median"> Median for this distribution. </param>
		/// <param name="scale"> Scale parameter for this distribution. </param>
		public CauchyDistribution(double median, double scale) : this(median, scale, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a Cauchy distribution using the given median and scale.
		/// </summary>
		/// <param name="median"> Median for this distribution. </param>
		/// <param name="scale"> Scale parameter for this distribution. </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates
		/// (defaults to <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0}.
		/// @since 2.1 </exception>
		public CauchyDistribution(double median, double scale, double inverseCumAccuracy) : this(new Well19937c(), median, scale, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a Cauchy distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="median"> Median for this distribution. </param>
		/// <param name="scale"> Scale parameter for this distribution. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0}.
		/// @since 3.3 </exception>
		public CauchyDistribution(RandomGenerator rng, double median, double scale) : this(rng, median, scale, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a Cauchy distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="median"> Median for this distribution. </param>
		/// <param name="scale"> Scale parameter for this distribution. </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates
		/// (defaults to <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code scale <= 0}.
		/// @since 3.1 </exception>
		public CauchyDistribution(RandomGenerator rng, double median, double scale, double inverseCumAccuracy) : base(rng)
		{
			if (scale <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SCALE, scale);
			}
			this.scale = scale;
			this.median = median;
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(double x)
		{
			return 0.5 + (FastMath.atan((x - median) / scale) / FastMath.PI);
		}

		/// <summary>
		/// Access the median.
		/// </summary>
		/// <returns> the median for this distribution. </returns>
		public virtual double Median
		{
			get
			{
				return median;
			}
		}

		/// <summary>
		/// Access the scale parameter.
		/// </summary>
		/// <returns> the scale parameter for this distribution. </returns>
		public virtual double Scale
		{
			get
			{
				return scale;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double density(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dev = x - median;
			double dev = x - median;
			return (1 / FastMath.PI) * (scale / (dev * dev + scale * scale));
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns {@code Double.NEGATIVE_INFINITY} when {@code p == 0}
		/// and {@code Double.POSITIVE_INFINITY} when {@code p == 1}.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(double p) throws org.apache.commons.math3.exception.OutOfRangeException
		public override double inverseCumulativeProbability(double p)
		{
			double ret;
			if (p < 0 || p > 1)
			{
				throw new OutOfRangeException(p, 0, 1);
			}
			else if (p == 0)
			{
				ret = double.NegativeInfinity;
			}
			else if (p == 1)
			{
				ret = double.PositiveInfinity;
			}
			else
			{
				ret = median + scale * FastMath.tan(FastMath.PI * (p - .5));
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
		/// The mean is always undefined no matter the parameters.
		/// </summary>
		/// <returns> mean (always Double.NaN) </returns>
		public override double NumericalMean
		{
			get
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The variance is always undefined no matter the parameters.
		/// </summary>
		/// <returns> variance (always Double.NaN) </returns>
		public override double NumericalVariance
		{
			get
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always negative infinity no matter
		/// the parameters.
		/// </summary>
		/// <returns> lower bound of the support (always Double.NEGATIVE_INFINITY) </returns>
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
		/// The upper bound of the support is always positive infinity no matter
		/// the parameters.
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