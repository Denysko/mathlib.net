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
namespace mathlib.special
{

    using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
    using OutOfRangeException = mathlib.exception.OutOfRangeException;
    using ContinuedFraction = mathlib.util.ContinuedFraction;
    using FastMath = mathlib.util.FastMath;

    /// <summary>
    /// <p>
    /// This is a utility class that provides computation methods related to the
    /// Beta family of functions.
    /// </p>
    /// <p>
    /// Implementation of <seealso cref="#logBeta(double, double)"/> is based on the
    /// algorithms described in
    /// <ul>
    /// <li><a href="http://dx.doi.org/10.1145/22721.23109">Didonato and Morris
    ///     (1986)</a>, <em>Computation of the Incomplete Gamma Function Ratios
    ///     and their Inverse</em>, TOMS 12(4), 377-393,</li>
    /// <li><a href="http://dx.doi.org/10.1145/131766.131776">Didonato and Morris
    ///     (1992)</a>, <em>Algorithm 708: Significant Digit Computation of the
    ///     Incomplete Beta Function Ratios</em>, TOMS 18(3), 360-373,</li>
    /// </ul>
    /// and implemented in the
    /// <a href="http://www.dtic.mil/docs/citations/ADA476840">NSWC Library of Mathematical Functions</a>,
    /// available
    /// <a href="http://www.ualberta.ca/CNS/RESEARCH/Software/NumericalNSWC/site.html">here</a>.
    /// This library is "approved for public release", and the
    /// <a href="http://www.dtic.mil/dtic/pdf/announcements/CopyrightGuidance.pdf">Copyright guidance</a>
    /// indicates that unless otherwise stated in the code, all FORTRAN functions in
    /// this library are license free. Since no such notice appears in the code these
    /// functions can safely be ported to Commons-Math.
    /// </p>
    /// 
    /// 
    /// @version $Id: Beta.java 1546350 2013-11-28 11:41:12Z erans $
    /// </summary>
    public class Beta
    {
        /// <summary>
        /// Maximum allowed numerical error. </summary>
        private const double DEFAULT_EPSILON = 1E-14;

        /// <summary>
        /// The constant value of Â½log 2Ï€. </summary>
        private const double HALF_LOG_TWO_PI = .9189385332046727;

        /// <summary>
        /// <p>
        /// The coefficients of the series expansion of the Î" function. This function
        /// is defined as follows
        /// </p>
        /// <center>Î"(x) = log Î"(x) - (x - 0.5) log a + a - 0.5 log 2Ï€,</center>
        /// <p>
        /// see equation (23) in Didonato and Morris (1992). The series expansion,
        /// which applies for x â‰¥ 10, reads
        /// </p>
        /// <pre>
        ///                 14
        ///                ====
        ///             1  \                2 n
        ///     Î"(x) = ---  >    d  (10 / x)
        ///             x  /      n
        ///                ====
        ///                n = 0
        /// <pre>
        /// </summary>
        private static readonly double[] DELTA = new double[] {
            .833333333333333333333333333333E-1,
            -.277777777777777777777777752282E-4,
            .793650793650793650791732130419E-7, 
            -.595238095238095232389839236182E-9,
            .841750841750832853294451671990E-11, 
            -.191752691751854612334149171243E-12,
            .641025640510325475730918472625E-14, 
            -.295506514125338232839867823991E-15,
            .179643716359402238723287696452E-16, 
            -.139228964661627791231203060395E-17,
            .133802855014020915603275339093E-18, 
            -.154246009867966094273710216533E-19,
            .197701992980957427278370133333E-20, 
            -.234065664793997056856992426667E-21,
            .171348014966398575409015466667E-22
        };

        /// <summary>
        /// Default constructor.  Prohibit instantiation.
        /// </summary>
        private Beta()
        {
        }

        /// <summary>
        /// Returns the
        /// <a href="http://mathworld.wolfram.com/RegularizedBetaFunction.html">
        /// regularized beta function</a> I(x, a, b).
        /// </summary>
        /// <param name="x"> Value. </param>
        /// <param name="a"> Parameter {@code a}. </param>
        /// <param name="b"> Parameter {@code b}. </param>
        /// <returns> the regularized beta function I(x, a, b). </returns>
        /// <exception cref="mathlib.exception.MaxCountExceededException">
        /// if the algorithm fails to converge. </exception>
        public static double regularizedBeta(double x, double a, double b)
        {
            return regularizedBeta(x, a, b, DEFAULT_EPSILON, int.MaxValue);
        }

        /// <summary>
        /// Returns the
        /// <a href="http://mathworld.wolfram.com/RegularizedBetaFunction.html">
        /// regularized beta function</a> I(x, a, b).
        /// </summary>
        /// <param name="x"> Value. </param>
        /// <param name="a"> Parameter {@code a}. </param>
        /// <param name="b"> Parameter {@code b}. </param>
        /// <param name="epsilon"> When the absolute value of the nth item in the
        /// series is less than epsilon the approximation ceases to calculate
        /// further elements in the series. </param>
        /// <returns> the regularized beta function I(x, a, b) </returns>
        /// <exception cref="mathlib.exception.MaxCountExceededException">
        /// if the algorithm fails to converge. </exception>
        public static double regularizedBeta(double x, double a, double b, double epsilon)
        {
            return regularizedBeta(x, a, b, epsilon, int.MaxValue);
        }

        /// <summary>
        /// Returns the regularized beta function I(x, a, b).
        /// </summary>
        /// <param name="x"> the value. </param>
        /// <param name="a"> Parameter {@code a}. </param>
        /// <param name="b"> Parameter {@code b}. </param>
        /// <param name="maxIterations"> Maximum number of "iterations" to complete. </param>
        /// <returns> the regularized beta function I(x, a, b) </returns>
        /// <exception cref="mathlib.exception.MaxCountExceededException">
        /// if the algorithm fails to converge. </exception>
        public static double regularizedBeta(double x, double a, double b, int maxIterations)
        {
            return regularizedBeta(x, a, b, DEFAULT_EPSILON, maxIterations);
        }

        /// <summary>
        /// Returns the regularized beta function I(x, a, b).
        /// 
        /// The implementation of this method is based on:
        /// <ul>
        /// <li>
        /// <a href="http://mathworld.wolfram.com/RegularizedBetaFunction.html">
        /// Regularized Beta Function</a>.</li>
        /// <li>
        /// <a href="http://functions.wolfram.com/06.21.10.0001.01">
        /// Regularized Beta Function</a>.</li>
        /// </ul>
        /// </summary>
        /// <param name="x"> the value. </param>
        /// <param name="a"> Parameter {@code a}. </param>
        /// <param name="b"> Parameter {@code b}. </param>
        /// <param name="epsilon"> When the absolute value of the nth item in the
        /// series is less than epsilon the approximation ceases to calculate
        /// further elements in the series. </param>
        /// <param name="maxIterations"> Maximum number of "iterations" to complete. </param>
        /// <returns> the regularized beta function I(x, a, b) </returns>
        /// <exception cref="mathlib.exception.MaxCountExceededException">
        /// if the algorithm fails to converge. </exception>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public static double regularizedBeta(double x, final double a, final double b, double epsilon, int maxIterations)
        public static double regularizedBeta(double x, double a, double b, double epsilon, int maxIterations)
        {
            double ret;

            if (double.IsNaN(x) || double.IsNaN(a) || double.IsNaN(b) || x < 0 || x > 1 || a <= 0 || b <= 0)
            {
                ret = double.NaN;
            }
            else if (x > (a + 1) / (2 + b + a) && 1 - x <= (b + 1) / (2 + b + a))
            {
                ret = 1 - regularizedBeta(1 - x, b, a, epsilon, maxIterations);
            }
            else
            {
                ContinuedFraction fraction = new ContinuedFractionAnonymousInnerClassHelper(x, a, b, ret);
                ret = FastMath.exp((a * FastMath.log(x)) + (b * FastMath.log1p(-x)) - FastMath.log(a) - logBeta(a, b)) * 1.0 / fraction.evaluate(x, epsilon, maxIterations);
            }

            return ret;
        }

        private class ContinuedFractionAnonymousInnerClassHelper : ContinuedFraction
        {
            private double x;
            private double a;
            private double b;
            private double ret;

            public ContinuedFractionAnonymousInnerClassHelper(double x, double a, double b, double ret)
            {
                this.x = x;
                this.a = a;
                this.b = b;
                this.ret = ret;
            }


            protected internal override double getB(int n, double x)
            {
                double ret;
                double m;
                if (n % 2 == 0) // even
                {
                    m = n / 2.0;
                    ret = (m * (b - m) * x) / ((a + (2 * m) - 1) * (a + (2 * m)));
                }
                else
                {
                    m = (n - 1.0) / 2.0;
                    ret = -((a + m) * (a + b + m) * x) / ((a + (2 * m)) * (a + (2 * m) + 1.0));
                }
                return ret;
            }

            protected internal override double getA(int n, double x)
            {
                return 1.0;
            }
        }

        /// <summary>
        /// Returns the natural logarithm of the beta function B(a, b).
        /// 
        /// The implementation of this method is based on:
        /// <ul>
        /// <li><a href="http://mathworld.wolfram.com/BetaFunction.html">
        /// Beta Function</a>, equation (1).</li>
        /// </ul>
        /// </summary>
        /// <param name="a"> Parameter {@code a}. </param>
        /// <param name="b"> Parameter {@code b}. </param>
        /// <param name="epsilon"> This parameter is ignored. </param>
        /// <param name="maxIterations"> This parameter is ignored. </param>
        /// <returns> log(B(a, b)). </returns>
        /// @deprecated as of version 3.1, this method is deprecated as the
        /// computation of the beta function is no longer iterative; it will be
        /// removed in version 4.0. Current implementation of this method
        /// internally calls <seealso cref="#logBeta(double, double)"/>. 
        [Obsolete("as of version 3.1, this method is deprecated as the")]
        public static double logBeta(double a, double b, double epsilon, int maxIterations)
        {

            return logBeta(a, b);
        }


        /// <summary>
        /// Returns the value of log Î"(a + b) for 1 â‰¤ a, b â‰¤ 2. Based on the
        /// <em>NSWC Library of Mathematics Subroutines</em> double precision
        /// implementation, {@code DGSMLN}. In {@code BetaTest.testLogGammaSum()},
        /// this private method is accessed through reflection.
        /// </summary>
        /// <param name="a"> First argument. </param>
        /// <param name="b"> Second argument. </param>
        /// <returns> the value of {@code log(Gamma(a + b))}. </returns>
        /// <exception cref="OutOfRangeException"> if {@code a} or {@code b} is lower than
        /// {@code 1.0} or greater than {@code 2.0}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static double logGammaSum(final double a, final double b) throws mathlib.exception.OutOfRangeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        private static double logGammaSum(double a, double b)
        {

            if ((a < 1.0) || (a > 2.0))
            {
                throw new OutOfRangeException(a, 1.0, 2.0);
            }
            if ((b < 1.0) || (b > 2.0))
            {
                throw new OutOfRangeException(b, 1.0, 2.0);
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double x = (a - 1.0) + (b - 1.0);
            double x = (a - 1.0) + (b - 1.0);
            if (x <= 0.5)
            {
                return Gamma.logGamma1p(1.0 + x);
            }
            else if (x <= 1.5)
            {
                return Gamma.logGamma1p(x) + FastMath.log1p(x);
            }
            else
            {
                return Gamma.logGamma1p(x - 1.0) + FastMath.log(x * (1.0 + x));
            }
        }

        /// <summary>
        /// Returns the value of log[Î"(b) / Î"(a + b)] for a â‰¥ 0 and b â‰¥ 10. Based on
        /// the <em>NSWC Library of Mathematics Subroutines</em> double precision
        /// implementation, {@code DLGDIV}. In
        /// {@code BetaTest.testLogGammaMinusLogGammaSum()}, this private method is
        /// accessed through reflection.
        /// </summary>
        /// <param name="a"> First argument. </param>
        /// <param name="b"> Second argument. </param>
        /// <returns> the value of {@code log(Gamma(b) / Gamma(a + b))}. </returns>
        /// <exception cref="NumberIsTooSmallException"> if {@code a < 0.0} or {@code b < 10.0}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static double logGammaMinusLogGammaSum(final double a, final double b) throws mathlib.exception.NumberIsTooSmallException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        private static double logGammaMinusLogGammaSum(double a, double b)
        {

            if (a < 0.0)
            {
                throw new NumberIsTooSmallException(a, 0.0, true);
            }
            if (b < 10.0)
            {
                throw new NumberIsTooSmallException(b, 10.0, true);
            }

            /*
             * d = a + b - 0.5
             */
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double d;
            double d;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double w;
            double w;
            if (a <= b)
            {
                d = b + (a - 0.5);
                w = deltaMinusDeltaSum(a, b);
            }
            else
            {
                d = a + (b - 0.5);
                w = deltaMinusDeltaSum(b, a);
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double u = d * mathlib.util.FastMath.log1p(a / b);
            double u = d * FastMath.log1p(a / b);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double v = a * (mathlib.util.FastMath.log(b) - 1.0);
            double v = a * (FastMath.log(b) - 1.0);

            return u <= v ? (w - u) - v : (w - v) - u;
        }

        /// <summary>
        /// Returns the value of Î"(b) - Î"(a + b), with 0 â‰¤ a â‰¤ b and b â‰¥ 10. Based
        /// on equations (26), (27) and (28) in Didonato and Morris (1992).
        /// </summary>
        /// <param name="a"> First argument. </param>
        /// <param name="b"> Second argument. </param>
        /// <returns> the value of {@code Delta(b) - Delta(a + b)} </returns>
        /// <exception cref="OutOfRangeException"> if {@code a < 0} or {@code a > b} </exception>
        /// <exception cref="NumberIsTooSmallException"> if {@code b < 10} </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static double deltaMinusDeltaSum(final double a, final double b) throws mathlib.exception.OutOfRangeException, mathlib.exception.NumberIsTooSmallException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        private static double deltaMinusDeltaSum(double a, double b)
        {

            if ((a < 0) || (a > b))
            {
                throw new OutOfRangeException(a, 0, b);
            }
            if (b < 10)
            {
                throw new NumberIsTooSmallException(b, 10, true);
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double h = a / b;
            double h = a / b;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double p = h / (1.0 + h);
            double p = h / (1.0 + h);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double q = 1.0 / (1.0 + h);
            double q = 1.0 / (1.0 + h);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double q2 = q * q;
            double q2 = q * q;
            /*
             * s[i] = 1 + q + ... - q**(2 * i)
             */
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double[] s = new double[DELTA.length];
            double[] s = new double[DELTA.Length];
            s[0] = 1.0;
            for (int i = 1; i < s.Length; i++)
            {
                s[i] = 1.0 + (q + q2 * s[i - 1]);
            }
            /*
             * w = Delta(b) - Delta(a + b)
             */
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double sqrtT = 10.0 / b;
            double sqrtT = 10.0 / b;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double t = sqrtT * sqrtT;
            double t = sqrtT * sqrtT;
            double w = DELTA[DELTA.Length - 1] * s[s.Length - 1];
            for (int i = DELTA.Length - 2; i >= 0; i--)
            {
                w = t * w + DELTA[i] * s[i];
            }
            return w * p / b;
        }

        /// <summary>
        /// Returns the value of Î"(p) + Î"(q) - Î"(p + q), with p, q â‰¥ 10. Based on
        /// the <em>NSWC Library of Mathematics Subroutines</em> double precision
        /// implementation, {@code DBCORR}. In
        /// {@code BetaTest.testSumDeltaMinusDeltaSum()}, this private method is
        /// accessed through reflection.
        /// </summary>
        /// <param name="p"> First argument. </param>
        /// <param name="q"> Second argument. </param>
        /// <returns> the value of {@code Delta(p) + Delta(q) - Delta(p + q)}. </returns>
        /// <exception cref="NumberIsTooSmallException"> if {@code p < 10.0} or {@code q < 10.0}. </exception>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: private static double sumDeltaMinusDeltaSum(final double p, final double q)
        private static double sumDeltaMinusDeltaSum(double p, double q)
        {

            if (p < 10.0)
            {
                throw new NumberIsTooSmallException(p, 10.0, true);
            }
            if (q < 10.0)
            {
                throw new NumberIsTooSmallException(q, 10.0, true);
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double a = mathlib.util.FastMath.min(p, q);
            double a = FastMath.min(p, q);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double b = mathlib.util.FastMath.max(p, q);
            double b = FastMath.max(p, q);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double sqrtT = 10.0 / a;
            double sqrtT = 10.0 / a;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double t = sqrtT * sqrtT;
            double t = sqrtT * sqrtT;
            double z = DELTA[DELTA.Length - 1];
            for (int i = DELTA.Length - 2; i >= 0; i--)
            {
                z = t * z + DELTA[i];
            }
            return z / a + deltaMinusDeltaSum(a, b);
        }

        /// <summary>
        /// Returns the value of log B(p, q) for 0 â‰¤ x â‰¤ 1 and p, q > 0. Based on the
        /// <em>NSWC Library of Mathematics Subroutines</em> implementation,
        /// {@code DBETLN}.
        /// </summary>
        /// <param name="p"> First argument. </param>
        /// <param name="q"> Second argument. </param>
        /// <returns> the value of {@code log(Beta(p, q))}, {@code NaN} if
        /// {@code p <= 0} or {@code q <= 0}. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public static double logBeta(final double p, final double q)
        public static double logBeta(double p, double q)
        {
            if (double.IsNaN(p) || double.IsNaN(q) || (p <= 0.0) || (q <= 0.0))
            {
                return double.NaN;
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double a = mathlib.util.FastMath.min(p, q);
            double a = FastMath.min(p, q);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double b = mathlib.util.FastMath.max(p, q);
            double b = FastMath.max(p, q);
            if (a >= 10.0)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double w = sumDeltaMinusDeltaSum(a, b);
                double w = sumDeltaMinusDeltaSum(a, b);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double h = a / b;
                double h = a / b;
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double c = h / (1.0 + h);
                double c = h / (1.0 + h);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double u = -(a - 0.5) * mathlib.util.FastMath.log(c);
                double u = -(a - 0.5) * FastMath.log(c);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double v = b * mathlib.util.FastMath.log1p(h);
                double v = b * FastMath.log1p(h);
                if (u <= v)
                {
                    return (((-0.5 * FastMath.log(b) + HALF_LOG_TWO_PI) + w) - u) - v;
                }
                else
                {
                    return (((-0.5 * FastMath.log(b) + HALF_LOG_TWO_PI) + w) - v) - u;
                }
            }
            else if (a > 2.0)
            {
                if (b > 1000.0)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final int n = (int) mathlib.util.FastMath.floor(a - 1.0);
                    int n = (int)FastMath.floor(a - 1.0);
                    double prod = 1.0;
                    double ared = a;
                    for (int i = 0; i < n; i++)
                    {
                        ared -= 1.0;
                        prod *= ared / (1.0 + ared / b);
                    }
                    return (FastMath.log(prod) - n * FastMath.log(b)) + (Gamma.logGamma(ared) + logGammaMinusLogGammaSum(ared, b));
                }
                else
                {
                    double prod1 = 1.0;
                    double ared = a;
                    while (ared > 2.0)
                    {
                        ared -= 1.0;
                        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                        //ORIGINAL LINE: final double h = ared / b;
                        double h = ared / b;
                        prod1 *= h / (1.0 + h);
                    }
                    if (b < 10.0)
                    {
                        double prod2 = 1.0;
                        double bred = b;
                        while (bred > 2.0)
                        {
                            bred -= 1.0;
                            prod2 *= bred / (ared + bred);
                        }
                        return FastMath.log(prod1) + FastMath.log(prod2) + (Gamma.logGamma(ared) + (Gamma.logGamma(bred) - logGammaSum(ared, bred)));
                    }
                    else
                    {
                        return FastMath.log(prod1) + Gamma.logGamma(ared) + logGammaMinusLogGammaSum(ared, b);
                    }
                }
            }
            else if (a >= 1.0)
            {
                if (b > 2.0)
                {
                    if (b < 10.0)
                    {
                        double prod = 1.0;
                        double bred = b;
                        while (bred > 2.0)
                        {
                            bred -= 1.0;
                            prod *= bred / (a + bred);
                        }
                        return FastMath.log(prod) + (Gamma.logGamma(a) + (Gamma.logGamma(bred) - logGammaSum(a, bred)));
                    }
                    else
                    {
                        return Gamma.logGamma(a) + logGammaMinusLogGammaSum(a, b);
                    }
                }
                else
                {
                    return Gamma.logGamma(a) + Gamma.logGamma(b) - logGammaSum(a, b);
                }
            }
            else
            {
                if (b >= 10.0)
                {
                    return Gamma.logGamma(a) + logGammaMinusLogGammaSum(a, b);
                }
                else
                {
                    // The following command is the original NSWC implementation.
                    // return Gamma.logGamma(a) +
                    // (Gamma.logGamma(b) - Gamma.logGamma(a + b));
                    // The following command turns out to be more accurate.
                    return FastMath.log(Gamma.gamma(a) * Gamma.gamma(b) / Gamma.gamma(a + b));
                }
            }
        }
    }

}