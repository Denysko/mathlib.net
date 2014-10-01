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

	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using Gamma = mathlib.special.Gamma;
	using FastMath = mathlib.util.FastMath;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;

	/// <summary>
	/// Implementation of the Gamma distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Gamma_distribution">Gamma distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/GammaDistribution.html">Gamma distribution (MathWorld)</a>
	/// @version $Id: GammaDistribution.java 1534362 2013-10-21 20:23:54Z tn $ </seealso>
	public class GammaDistribution : AbstractRealDistribution
	{
		/// <summary>
		/// Default inverse cumulative probability accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20120524L;
		/// <summary>
		/// The shape parameter. </summary>
		private readonly double shape;
		/// <summary>
		/// The scale parameter. </summary>
		private readonly double scale;
		/// <summary>
		/// The constant value of {@code shape + g + 0.5}, where {@code g} is the
		/// Lanczos constant <seealso cref="Gamma#LANCZOS_G"/>.
		/// </summary>
		private readonly double shiftedShape;
		/// <summary>
		/// The constant value of
		/// {@code shape / scale * sqrt(e / (2 * pi * (shape + g + 0.5))) / L(shape)},
		/// where {@code L(shape)} is the Lanczos approximation returned by
		/// <seealso cref="Gamma#lanczos(double)"/>. This prefactor is used in
		/// <seealso cref="#density(double)"/>, when no overflow occurs with the natural
		/// calculation.
		/// </summary>
		private readonly double densityPrefactor1;
		/// <summary>
		/// The constant value of
		/// {@code log(shape / scale * sqrt(e / (2 * pi * (shape + g + 0.5))) / L(shape))},
		/// where {@code L(shape)} is the Lanczos approximation returned by
		/// <seealso cref="Gamma#lanczos(double)"/>. This prefactor is used in
		/// <seealso cref="#logDensity(double)"/>, when no overflow occurs with the natural
		/// calculation.
		/// </summary>
		private readonly double logDensityPrefactor1;
		/// <summary>
		/// The constant value of
		/// {@code shape * sqrt(e / (2 * pi * (shape + g + 0.5))) / L(shape)},
		/// where {@code L(shape)} is the Lanczos approximation returned by
		/// <seealso cref="Gamma#lanczos(double)"/>. This prefactor is used in
		/// <seealso cref="#density(double)"/>, when overflow occurs with the natural
		/// calculation.
		/// </summary>
		private readonly double densityPrefactor2;
		/// <summary>
		/// The constant value of
		/// {@code log(shape * sqrt(e / (2 * pi * (shape + g + 0.5))) / L(shape))},
		/// where {@code L(shape)} is the Lanczos approximation returned by
		/// <seealso cref="Gamma#lanczos(double)"/>. This prefactor is used in
		/// <seealso cref="#logDensity(double)"/>, when overflow occurs with the natural
		/// calculation.
		/// </summary>
		private readonly double logDensityPrefactor2;
		/// <summary>
		/// Lower bound on {@code y = x / scale} for the selection of the computation
		/// method in <seealso cref="#density(double)"/>. For {@code y <= minY}, the natural
		/// calculation overflows.
		/// </summary>
		private readonly double minY;
		/// <summary>
		/// Upper bound on {@code log(y)} ({@code y = x / scale}) for the selection
		/// of the computation method in <seealso cref="#density(double)"/>. For
		/// {@code log(y) >= maxLogY}, the natural calculation overflows.
		/// </summary>
		private readonly double maxLogY;
		/// <summary>
		/// Inverse cumulative probability accuracy. </summary>
		private readonly double solverAbsoluteAccuracy;

		/// <summary>
		/// Creates a new gamma distribution with specified values of the shape and
		/// scale parameters.
		/// </summary>
		/// <param name="shape"> the shape parameter </param>
		/// <param name="scale"> the scale parameter </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GammaDistribution(double shape, double scale) throws mathlib.exception.NotStrictlyPositiveException
		public GammaDistribution(double shape, double scale) : this(shape, scale, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a new gamma distribution with specified values of the shape and
		/// scale parameters.
		/// </summary>
		/// <param name="shape"> the shape parameter </param>
		/// <param name="scale"> the scale parameter </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}.
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GammaDistribution(double shape, double scale, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public GammaDistribution(double shape, double scale, double inverseCumAccuracy) : this(new Well19937c(), shape, scale, inverseCumAccuracy)
		{
		}

		/// <summary>
		/// Creates a Gamma distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="shape"> the shape parameter </param>
		/// <param name="scale"> the scale parameter </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}.
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GammaDistribution(mathlib.random.RandomGenerator rng, double shape, double scale) throws mathlib.exception.NotStrictlyPositiveException
		public GammaDistribution(RandomGenerator rng, double shape, double scale) : this(rng, shape, scale, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		{
		}

		/// <summary>
		/// Creates a Gamma distribution.
		/// </summary>
		/// <param name="rng"> Random number generator. </param>
		/// <param name="shape"> the shape parameter </param>
		/// <param name="scale"> the scale parameter </param>
		/// <param name="inverseCumAccuracy"> the maximum absolute error in inverse
		/// cumulative probability estimates (defaults to
		/// <seealso cref="#DEFAULT_INVERSE_ABSOLUTE_ACCURACY"/>). </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0} or
		/// {@code scale <= 0}.
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public GammaDistribution(mathlib.random.RandomGenerator rng, double shape, double scale, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
		public GammaDistribution(RandomGenerator rng, double shape, double scale, double inverseCumAccuracy) : base(rng)
		{

			if (shape <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SHAPE, shape);
			}
			if (scale <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SCALE, scale);
			}

			this.shape = shape;
			this.scale = scale;
			this.solverAbsoluteAccuracy = inverseCumAccuracy;
			this.shiftedShape = shape + Gamma.LANCZOS_G + 0.5;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux = mathlib.util.FastMath.E / (2.0 * mathlib.util.FastMath.PI * shiftedShape);
			double aux = FastMath.E / (2.0 * FastMath.PI * shiftedShape);
			this.densityPrefactor2 = shape * FastMath.sqrt(aux) / Gamma.lanczos(shape);
			this.logDensityPrefactor2 = FastMath.log(shape) + 0.5 * FastMath.log(aux) - FastMath.log(Gamma.lanczos(shape));
			this.densityPrefactor1 = this.densityPrefactor2 / scale * FastMath.pow(shiftedShape, -shape) * FastMath.exp(shape + Gamma.LANCZOS_G);
			this.logDensityPrefactor1 = this.logDensityPrefactor2 - FastMath.log(scale) - FastMath.log(shiftedShape) * shape + shape + Gamma.LANCZOS_G;
			this.minY = shape + Gamma.LANCZOS_G - FastMath.log(double.MaxValue);
			this.maxLogY = FastMath.log(double.MaxValue) / (shape - 1.0);
		}

		/// <summary>
		/// Returns the shape parameter of {@code this} distribution.
		/// </summary>
		/// <returns> the shape parameter </returns>
		/// @deprecated as of version 3.1, <seealso cref="#getShape()"/> should be preferred.
		/// This method will be removed in version 4.0. 
		[Obsolete]//("as of version 3.1, <seealso cref="#getShape()"/> should be preferred.")]
		public virtual double Alpha
		{
			get
			{
				return shape;
			}
		}

		/// <summary>
		/// Returns the shape parameter of {@code this} distribution.
		/// </summary>
		/// <returns> the shape parameter
		/// @since 3.1 </returns>
		public virtual double Shape
		{
			get
			{
				return shape;
			}
		}

		/// <summary>
		/// Returns the scale parameter of {@code this} distribution.
		/// </summary>
		/// <returns> the scale parameter </returns>
		/// @deprecated as of version 3.1, <seealso cref="#getScale()"/> should be preferred.
		/// This method will be removed in version 4.0. 
		[Obsolete]//("as of version 3.1, <seealso cref="#getScale()"/> should be preferred.")]
		public virtual double Beta
		{
			get
			{
				return scale;
			}
		}

		/// <summary>
		/// Returns the scale parameter of {@code this} distribution.
		/// </summary>
		/// <returns> the scale parameter
		/// @since 3.1 </returns>
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
		   /* The present method must return the value of
		    *
		    *     1       x a     - x
		    * ---------- (-)  exp(---)
		    * x Gamma(a)  b        b
		    *
		    * where a is the shape parameter, and b the scale parameter.
		    * Substituting the Lanczos approximation of Gamma(a) leads to the
		    * following expression of the density
		    *
		    * a              e            1         y      a
		    * - sqrt(------------------) ---- (-----------)  exp(a - y + g),
		    * x      2 pi (a + g + 0.5)  L(a)  a + g + 0.5
		    *
		    * where y = x / b. The above formula is the "natural" computation, which
		    * is implemented when no overflow is likely to occur. If overflow occurs
		    * with the natural computation, the following identity is used. It is
		    * based on the BOOST library
		    * http://www.boost.org/doc/libs/1_35_0/libs/math/doc/sf_and_dist/html/math_toolkit/special/sf_gamma/igamma.html
		    * Formula (15) needs adaptations, which are detailed below.
		    *
		    *       y      a
		    * (-----------)  exp(a - y + g)
		    *  a + g + 0.5
		    *                              y - a - g - 0.5    y (g + 0.5)
		    *               = exp(a log1pm(---------------) - ----------- + g),
		    *                                a + g + 0.5      a + g + 0.5
		    *
		    *  where log1pm(z) = log(1 + z) - z. Therefore, the value to be
		    *  returned is
		    *
		    * a              e            1
		    * - sqrt(------------------) ----
		    * x      2 pi (a + g + 0.5)  L(a)
		    *                              y - a - g - 0.5    y (g + 0.5)
		    *               * exp(a log1pm(---------------) - ----------- + g).
		    *                                a + g + 0.5      a + g + 0.5
		    */
			if (x < 0)
			{
				return 0;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = x / scale;
			double y = x / scale;
			if ((y <= minY) || (FastMath.log(y) >= maxLogY))
			{
				/*
				 * Overflow.
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux1 = (y - shiftedShape) / shiftedShape;
				double aux1 = (y - shiftedShape) / shiftedShape;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux2 = shape * (mathlib.util.FastMath.log1p(aux1) - aux1);
				double aux2 = shape * (FastMath.log1p(aux1) - aux1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux3 = -y * (mathlib.special.Gamma.LANCZOS_G + 0.5) / shiftedShape + mathlib.special.Gamma.LANCZOS_G + aux2;
				double aux3 = -y * (Gamma.LANCZOS_G + 0.5) / shiftedShape + Gamma.LANCZOS_G + aux2;
				return densityPrefactor2 / x * FastMath.exp(aux3);
			}
			/*
			 * Natural calculation.
			 */
			return densityPrefactor1 * FastMath.exp(-y) * FastMath.pow(y, shape - 1);
		}

		/// <summary>
		/// {@inheritDoc} * </summary>
		public override double logDensity(double x)
		{
			/*
			 * see the comment in {@link #density(double)} for computation details
			 */
			if (x < 0)
			{
				return double.NegativeInfinity;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = x / scale;
			double y = x / scale;
			if ((y <= minY) || (FastMath.log(y) >= maxLogY))
			{
				/*
				 * Overflow.
				 */
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux1 = (y - shiftedShape) / shiftedShape;
				double aux1 = (y - shiftedShape) / shiftedShape;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux2 = shape * (mathlib.util.FastMath.log1p(aux1) - aux1);
				double aux2 = shape * (FastMath.log1p(aux1) - aux1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aux3 = -y * (mathlib.special.Gamma.LANCZOS_G + 0.5) / shiftedShape + mathlib.special.Gamma.LANCZOS_G + aux2;
				double aux3 = -y * (Gamma.LANCZOS_G + 0.5) / shiftedShape + Gamma.LANCZOS_G + aux2;
				return logDensityPrefactor2 - FastMath.log(x) + aux3;
			}
			/*
			 * Natural calculation.
			 */
			return logDensityPrefactor1 - y + FastMath.log(y) * (shape - 1);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// The implementation of this method is based on:
		/// <ul>
		///  <li>
		///   <a href="http://mathworld.wolfram.com/Chi-SquaredDistribution.html">
		///    Chi-Squared Distribution</a>, equation (9).
		///  </li>
		///  <li>Casella, G., & Berger, R. (1990). <i>Statistical Inference</i>.
		///    Belmont, CA: Duxbury Press.
		///  </li>
		/// </ul>
		/// </summary>
		public override double cumulativeProbability(double x)
		{
			double ret;

			if (x <= 0)
			{
				ret = 0;
			}
			else
			{
				ret = Gamma.regularizedGammaP(shape, x / scale);
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
		/// For shape parameter {@code alpha} and scale parameter {@code beta}, the
		/// mean is {@code alpha * beta}.
		/// </summary>
		public override double NumericalMean
		{
			get
			{
				return shape * scale;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// For shape parameter {@code alpha} and scale parameter {@code beta}, the
		/// variance is {@code alpha * beta^2}.
		/// </summary>
		/// <returns> {@inheritDoc} </returns>
		public override double NumericalVariance
		{
			get
			{
				return shape * scale * scale;
			}
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

		/// <summary>
		/// <p>This implementation uses the following algorithms: </p>
		/// 
		/// <p>For 0 < shape < 1: <br/>
		/// Ahrens, J. H. and Dieter, U., <i>Computer methods for
		/// sampling from gamma, beta, Poisson and binomial distributions.</i>
		/// Computing, 12, 223-246, 1974.</p>
		/// 
		/// <p>For shape >= 1: <br/>
		/// Marsaglia and Tsang, <i>A Simple Method for Generating
		/// Gamma Variables.</i> ACM Transactions on Mathematical Software,
		/// Volume 26 Issue 3, September, 2000.</p>
		/// </summary>
		/// <returns> random value sampled from the Gamma(shape, scale) distribution </returns>
		public override double sample()
		{
			if (shape < 1)
			{
				// [1]: p. 228, Algorithm GS

				while (true)
				{
					// Step 1:
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u = random.nextDouble();
					double u = random.NextDouble();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bGS = 1 + shape / mathlib.util.FastMath.E;
					double bGS = 1 + shape / FastMath.E;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = bGS * u;
					double p = bGS * u;

					if (p <= 1)
					{
						// Step 2:

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = mathlib.util.FastMath.pow(p, 1 / shape);
						double x = FastMath.pow(p, 1 / shape);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u2 = random.nextDouble();
						double u2 = random.NextDouble();

						if (u2 > FastMath.exp(-x))
						{
							// Reject
							continue;
						}
						else
						{
							return scale * x;
						}
					}
					else
					{
						// Step 3:

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = -1 * mathlib.util.FastMath.log((bGS - p) / shape);
						double x = -1 * FastMath.log((bGS - p) / shape);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u2 = random.nextDouble();
						double u2 = random.NextDouble();

						if (u2 > FastMath.pow(x, shape - 1))
						{
							// Reject
							continue;
						}
						else
						{
							return scale * x;
						}
					}
				}
			}

			// Now shape >= 1

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = shape - 0.333333333333333333;
			double d = shape - 0.333333333333333333;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = 1 / (3 * mathlib.util.FastMath.sqrt(d));
			double c = 1 / (3 * FastMath.sqrt(d));

			while (true)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = random.nextGaussian();
				double x = random.nextGaussian();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v = (1 + c * x) * (1 + c * x) * (1 + c * x);
				double v = (1 + c * x) * (1 + c * x) * (1 + c * x);

				if (v <= 0)
				{
					continue;
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double u = random.nextDouble();
				double u = random.NextDouble();

				// Squeeze
				if (u < 1 - 0.0331 * x2 * x2)
				{
					return scale * d * v;
				}

				if (FastMath.log(u) < 0.5 * x2 + d * (1 - v + FastMath.log(v)))
				{
					return scale * d * v;
				}
			}
		}
	}

}