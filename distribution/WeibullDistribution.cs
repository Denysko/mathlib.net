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

	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Gamma = mathlib.special.Gamma;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the Weibull distribution. This implementation uses the
	/// two parameter form of the distribution defined by
	/// <a href="http://mathworld.wolfram.com/WeibullDistribution.html">
	/// Weibull Distribution</a>, equations (1) and (2).
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Weibull_distribution">Weibull distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/WeibullDistribution.html">Weibull distribution (MathWorld)</a>
	/// @since 1.1 (changed to concrete class in 3.0)
	/// @version $Id: WeibullDistribution.java 1538998 2013-11-05 13:51:24Z erans $ </seealso>
	public class WeibullDistribution : AbstractRealDistribution
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
		/// The shape parameter. </summary>
		private readonly double shape;
		/// <summary>
		/// The scale parameter. </summary>
		private readonly double scale;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;
		/// <summary>
		/// Cached numerical mean </summary>
		private double numericalMean = double.NaN;
		/// <summary>
		/// Whether or not the numerical mean has been calculated </summary>
		private bool numericalMeanIsCalculated = false;
		/// <summary>
		/// Cached numerical variance </summary>
		private double numericalVariance = double.NaN;
		/// <summary>
		/// Whether or not the numerical variance has been calculated </summary>
		private bool numericalVarianceIsCalculated = false;

		/// <summary>
		/// Create a Weibull distribution with the given shape and scale and a
		/// location equal to zero.
		/// </summary>
		/// <param name="alpha"> Shape parameter. </param>
		/// <param name="beta"> Scale parameter. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code alpha <= 0} or
		/// {@code beta <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public WeibullDistribution(double alpha, double beta) throws mathlib.exception.NotStrictlyPositiveException
		public WeibullDistribution(double alpha, double beta) : this(alpha, beta, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Create a Weibull distribution with the given shape, scale and inverse
		/// cumulative probability accuracy and a location equal to zero.
		/// </summary>
		/// <param name="alpha"> Shape parameter. </param>
		/// <param name="beta"> Scale parameter. </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates
		/// (defaults to <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code alpha <= 0} or
		/// {@code beta <= 0}.
		/// @since 2.1 </exception>
		public WeibullDistribution(double alpha, double beta, double inverseCumAccuracy) : this(new Well19937c(), alpha, beta, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a Weibull distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="alpha"> Shape parameter. </param>
		/// <param name="beta"> Scale parameter. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code alpha <= 0} or {@code beta <= 0}.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public WeibullDistribution(mathlib.random.RandomGenerator rng, double alpha, double beta) throws mathlib.exception.NotStrictlyPositiveException
		public WeibullDistribution(RandomGenerator rng, double alpha, double beta) : this(rng, alpha, beta, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a Weibull distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="alpha"> Shape parameter. </param>
		/// <param name="beta"> Scale parameter. </param>
		/// <param name="inverseCumAccuracy"> Maximum absolute error in inverse
		/// cumulative probability estimates
		/// (defaults to <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code alpha <= 0} or {@code beta <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public WeibullDistribution(mathlib.random.RandomGenerator rng, double alpha, double beta, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public WeibullDistribution(RandomGenerator rng, double alpha, double beta, double inverseCumAccuracy) : base(rng)
		{

			if (alpha <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SHAPE, alpha);
			}
			if (beta <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SCALE, beta);
			}
			scale = beta;
			shape = alpha;
			solverAbsoluteAccuracy = inverseCumAccuracy;
		}

		/// <summary>
		/// Access the shape parameter, {@code alpha}.
		/// </summary>
		/// <returns> the shape parameter, {@code alpha}. </returns>
		public virtual double Shape
		{
			get
			{
				return shape;
			}
		}

		/// <summary>
		/// Access the scale parameter, {@code beta}.
		/// </summary>
		/// <returns> the scale parameter, {@code beta}. </returns>
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
			if (x < 0)
			{
				return 0;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xscale = x / scale;
			double xscale = x / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xscalepow = mathlib.util.FastMath.pow(xscale, shape - 1);
			double xscalepow = FastMath.pow(xscale, shape - 1);

			/*
			 * FastMath.pow(x / scale, shape) =
			 * FastMath.pow(xscale, shape) =
			 * FastMath.pow(xscale, shape - 1) * xscale
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xscalepowshape = xscalepow * xscale;
			double xscalepowshape = xscalepow * xscale;

			return (shape / scale) * xscalepow * FastMath.exp(-xscalepowshape);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double logDensity(double x)
		{
			if (x < 0)
			{
				return double.NegativeInfinity;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xscale = x / scale;
			double xscale = x / scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double logxscalepow = mathlib.util.FastMath.log(xscale) * (shape - 1);
			double logxscalepow = FastMath.log(xscale) * (shape - 1);

			/*
			 * FastMath.pow(x / scale, shape) =
			 * FastMath.pow(xscale, shape) =
			 * FastMath.pow(xscale, shape - 1) * xscale
			 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xscalepowshape = mathlib.util.FastMath.exp(logxscalepow) * xscale;
			double xscalepowshape = FastMath.exp(logxscalepow) * xscale;

			return FastMath.log(shape / scale) + logxscalepow - xscalepowshape;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double cumulativeProbability(double x)
		{
			double ret;
			if (x <= 0.0)
			{
				ret = 0.0;
			}
			else
			{
				ret = 1.0 - FastMath.exp(-FastMath.pow(x / scale, shape));
			}
			return ret;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// Returns {@code 0} when {@code p == 0} and
		/// {@code Double.POSITIVE_INFINITY} when {@code p == 1}.
		/// </summary>
		public override double inverseCumulativeProbability(double p)
		{
			double ret;
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0.0, 1.0);
			}
			else if (p == 0)
			{
				ret = 0.0;
			}
			else if (p == 1)
			{
				ret = double.PositiveInfinity;
			}
			else
			{
				ret = scale * FastMath.pow(-FastMath.log1p(-p), 1.0 / shape);
			}
			return ret;
		}

		/// <summary>
		/// Return the absolute accuracy setting of the solver used to estimate
		/// inverse cumulative probabilities.
		/// </summary>
		/// <returns> the solver absolute accuracy.
		/// @since 2.1 </returns>
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
		/// The mean is {@code scale * Gamma(1 + (1 / shape))}, where {@code Gamma()}
		/// is the Gamma-function.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				if (!numericalMeanIsCalculated)
				{
					numericalMean = calculateNumericalMean();
					numericalMeanIsCalculated = true;
				}
				return numericalMean;
			}
		}

		/// <summary>
		/// used by <seealso cref="#getNumericalMean()"/>
		/// </summary>
		/// <returns> the mean of this distribution </returns>
		protected internal virtual double calculateNumericalMean()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sh = getShape();
			double sh = Shape;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sc = getScale();
			double sc = Scale;

			return sc * FastMath.exp(Gamma.logGamma(1 + (1 / sh)));
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The variance is {@code scale^2 * Gamma(1 + (2 / shape)) - mean^2}
		/// where {@code Gamma()} is the Gamma-function.
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				if (!numericalVarianceIsCalculated)
				{
					numericalVariance = calculateNumericalVariance();
					numericalVarianceIsCalculated = true;
				}
				return numericalVariance;
			}
		}

		/// <summary>
		/// used by <seealso cref="#getNumericalVariance()"/>
		/// </summary>
		/// <returns> the variance of this distribution </returns>
		protected internal virtual double calculateNumericalVariance()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sh = getShape();
			double sh = Shape;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sc = getScale();
			double sc = Scale;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mn = getNumericalMean();
			double mn = NumericalMean;

			return (sc * sc) * FastMath.exp(Gamma.logGamma(1 + (2 / sh))) - (mn * mn);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The lower bound of the support is always 0 no matter the parameters.
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