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
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using Erf = mathlib.special.Erf;
    using FastMath = mathlib.util.FastMath;
    using RandomGenerator = mathlib.random.RandomGenerator;
    using Well19937c = mathlib.random.Well19937c;

    /// <summary>
    /// Implementation of the log-normal (gaussian) distribution.
    /// 
    /// <p>
    /// <strong>Parameters:</strong>
    /// {@code X} is log-normally distributed if its natural logarithm {@code log(X)}
    /// is normally distributed. The probability distribution function of {@code X}
    /// is given by (for {@code x > 0})
    /// </p>
    /// <p>
    /// {@code exp(-0.5 * ((ln(x) - m) / s)^2) / (s * sqrt(2 * pi) * x)}
    /// </p>
    /// <ul>
    /// <li>{@code m} is the <em>scale</em> parameter: this is the mean of the
    /// normally distributed natural logarithm of this distribution,</li>
    /// <li>{@code s} is the <em>shape</em> parameter: this is the standard
    /// deviation of the normally distributed natural logarithm of this
    /// distribution.
    /// </ul>
    /// </summary>
    /// <seealso cref= <a href="http://en.wikipedia.org/wiki/Log-normal_distribution">
    /// Log-normal distribution (Wikipedia)</a> </seealso>
    /// <seealso cref= <a href="http://mathworld.wolfram.com/LogNormalDistribution.html">
    /// Log Normal distribution (MathWorld)</a>
    /// 
    /// @version $Id: LogNormalDistribution.java 1538998 2013-11-05 13:51:24Z erans $
    /// @since 3.0 </seealso>
    public class LogNormalDistribution : AbstractRealDistribution
    {
        /// <summary>
        /// Default inverse cumulative probability accuracy. </summary>
        public const double DEFAULT_INVERSE_ABSOLUTE_ACCURACY = 1e-9;

        /// <summary>
        /// Serializable version identifier. </summary>
        private const long serialVersionUID = 20120112;

        /// <summary>
        /// &radic;(2 &pi;) </summary>
        private static readonly double SQRT2PI = FastMath.sqrt(2 * FastMath.PI);

        /// <summary>
        /// &radic;(2) </summary>
        private static readonly double SQRT2 = FastMath.sqrt(2.0);

        /// <summary>
        /// The scale parameter of this distribution. </summary>
        private readonly double scale;

        /// <summary>
        /// The shape parameter of this distribution. </summary>
        private readonly double shape;
        /// <summary>
        /// The value of {@code log(shape) + 0.5 * log(2*PI)} stored for faster computation. </summary>
        private readonly double logShapePlusHalfLog2Pi;

        /// <summary>
        /// Inverse cumulative probability accuracy. </summary>
        private readonly double solverAbsoluteAccuracy;

        /// <summary>
        /// Create a log-normal distribution, where the mean and standard deviation
        /// of the <seealso cref="NormalDistribution normally distributed"/> natural
        /// logarithm of the log-normal distribution are equal to zero and one
        /// respectively. In other words, the scale of the returned distribution is
        /// {@code 0}, while its shape is {@code 1}.
        /// </summary>
        public LogNormalDistribution()
            : this(0, 1)
        {
        }

        /// <summary>
        /// Create a log-normal distribution using the specified scale and shape.
        /// </summary>
        /// <param name="scale"> the scale parameter of this distribution </param>
        /// <param name="shape"> the shape parameter of this distribution </param>
        /// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LogNormalDistribution(double scale, double shape) throws mathlib.exception.NotStrictlyPositiveException
        public LogNormalDistribution(double scale, double shape)
            : this(scale, shape, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
        {
        }

        /// <summary>
        /// Create a log-normal distribution using the specified scale, shape and
        /// inverse cumulative distribution accuracy.
        /// </summary>
        /// <param name="scale"> the scale parameter of this distribution </param>
        /// <param name="shape"> the shape parameter of this distribution </param>
        /// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
        /// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LogNormalDistribution(double scale, double shape, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
        public LogNormalDistribution(double scale, double shape, double inverseCumAccuracy)
            : this(new Well19937c(), scale, shape, inverseCumAccuracy)
        {
        }

        /// <summary>
        /// Creates a log-normal distribution.
        /// </summary>
        /// <param name="rng"> Random number generator. </param>
        /// <param name="scale"> Scale parameter of this distribution. </param>
        /// <param name="shape"> Shape parameter of this distribution. </param>
        /// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0}.
        /// @since 3.3 </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LogNormalDistribution(mathlib.random.RandomGenerator rng, double scale, double shape) throws mathlib.exception.NotStrictlyPositiveException
        public LogNormalDistribution(RandomGenerator rng, double scale, double shape)
            : this(rng, scale, shape, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
        {
        }

        /// <summary>
        /// Creates a log-normal distribution.
        /// </summary>
        /// <param name="rng"> Random number generator. </param>
        /// <param name="scale"> Scale parameter of this distribution. </param>
        /// <param name="shape"> Shape parameter of this distribution. </param>
        /// <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
        /// <exception cref="NotStrictlyPositiveException"> if {@code shape <= 0}.
        /// @since 3.1 </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LogNormalDistribution(mathlib.random.RandomGenerator rng, double scale, double shape, double inverseCumAccuracy) throws mathlib.exception.NotStrictlyPositiveException
        public LogNormalDistribution(RandomGenerator rng, double scale, double shape, double inverseCumAccuracy)
            : base(rng)
        {

            if (shape <= 0)
            {
                throw new NotStrictlyPositiveException(LocalizedFormats.SHAPE, shape);
            }

            this.scale = scale;
            this.shape = shape;
            this.logShapePlusHalfLog2Pi = FastMath.log(shape) + 0.5 * FastMath.log(2 * FastMath.PI);
            this.solverAbsoluteAccuracy = inverseCumAccuracy;
        }

        /// <summary>
        /// Returns the scale parameter of this distribution.
        /// </summary>
        /// <returns> the scale parameter </returns>
        public virtual double Scale
        {
            get
            {
                return scale;
            }
        }

        /// <summary>
        /// Returns the shape parameter of this distribution.
        /// </summary>
        /// <returns> the shape parameter </returns>
        public virtual double Shape
        {
            get
            {
                return shape;
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// For scale {@code m}, and shape {@code s} of this distribution, the PDF
        /// is given by
        /// <ul>
        /// <li>{@code 0} if {@code x <= 0},</li>
        /// <li>{@code exp(-0.5 * ((ln(x) - m) / s)^2) / (s * sqrt(2 * pi) * x)}
        /// otherwise.</li>
        /// </ul>
        /// </summary>
        public override double density(double x)
        {
            if (x <= 0)
            {
                return 0;
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double x0 = mathlib.util.FastMath.log(x) - scale;
            double x0 = FastMath.log(x) - scale;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double x1 = x0 / shape;
            double x1 = x0 / shape;
            return FastMath.exp(-0.5 * x1 * x1) / (shape * SQRT2PI * x);
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// See documentation of <seealso cref="#density(double)"/> for computation details.
        /// </summary>
        public override double logDensity(double x)
        {
            if (x <= 0)
            {
                return double.NegativeInfinity;
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double logX = mathlib.util.FastMath.log(x);
            double logX = FastMath.log(x);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double x0 = logX - scale;
            double x0 = logX - scale;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double x1 = x0 / shape;
            double x1 = x0 / shape;
            return -0.5 * x1 * x1 - (logShapePlusHalfLog2Pi + logX);
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// For scale {@code m}, and shape {@code s} of this distribution, the CDF
        /// is given by
        /// <ul>
        /// <li>{@code 0} if {@code x <= 0},</li>
        /// <li>{@code 0} if {@code ln(x) - m < 0} and {@code m - ln(x) > 40 * s}, as
        /// in these cases the actual value is within {@code Double.MIN_VALUE} of 0,
        /// <li>{@code 1} if {@code ln(x) - m >= 0} and {@code ln(x) - m > 40 * s},
        /// as in these cases the actual value is within {@code Double.MIN_VALUE} of
        /// 1,</li>
        /// <li>{@code 0.5 + 0.5 * erf((ln(x) - m) / (s * sqrt(2))} otherwise.</li>
        /// </ul>
        /// </summary>
        public override double cumulativeProbability(double x)
        {
            if (x <= 0)
            {
                return 0;
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double dev = mathlib.util.FastMath.log(x) - scale;
            double dev = FastMath.log(x) - scale;
            if (FastMath.abs(dev) > 40 * shape)
            {
                return dev < 0 ? 0.0d : 1.0d;
            }
            return 0.5 + 0.5 * Erf.erf(dev / (shape * SQRT2));
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        /// @deprecated See <seealso cref="RealDistribution#cumulativeProbability(double,double)"/> 
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override@Deprecated public double cumulativeProbability(double x0, double x1) throws mathlib.exception.NumberIsTooLargeException
        public override double cumulativeProbability(double x0, double x1)
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
            if (x0 <= 0 || x1 <= 0)
            {
                return base.probability(x0, x1);
            }
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double denom = shape * SQRT2;
            double denom = shape * SQRT2;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double v0 = (mathlib.util.FastMath.log(x0) - scale) / denom;
            double v0 = (FastMath.log(x0) - scale) / denom;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double v1 = (mathlib.util.FastMath.log(x1) - scale) / denom;
            double v1 = (FastMath.log(x1) - scale) / denom;
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
        /// For scale {@code m} and shape {@code s}, the mean is
        /// {@code exp(m + s^2 / 2)}.
        /// </summary>
        public override double NumericalMean
        {
            get
            {
                double s = shape;
                return FastMath.exp(scale + (s * s / 2));
            }
        }

        /// <summary>
        /// {@inheritDoc}
        /// 
        /// For scale {@code m} and shape {@code s}, the variance is
        /// {@code (exp(s^2) - 1) * exp(2 * m + s^2)}.
        /// </summary>
        public override double NumericalVariance
        {
            get
            {
                double s = shape;
                double ss = s * s;
                return (FastMath.expm1(ss)) * FastMath.exp(2 * scale + ss);
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

        /// <summary>
        /// {@inheritDoc} </summary>
        public override double sample()
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double n = random.nextGaussian();
            double n = random.nextGaussian();
            return FastMath.exp(scale + shape * n);
        }
    }

}