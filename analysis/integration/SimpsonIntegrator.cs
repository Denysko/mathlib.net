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
namespace mathlib.analysis.integration
{

    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
    using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
    using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
    using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
    using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
    using FastMath = mathlib.util.FastMath;

    /// <summary>
    /// Implements <a href="http://mathworld.wolfram.com/SimpsonsRule.html">
    /// Simpson's Rule</a> for integration of real univariate functions. For
    /// reference, see <b>Introduction to Numerical Analysis</b>, ISBN 038795452X,
    /// chapter 3.
    /// <p>
    /// This implementation employs the basic trapezoid rule to calculate Simpson's
    /// rule.</p>
    /// 
    /// @version $Id: SimpsonIntegrator.java 1364387 2012-07-22 18:14:11Z tn $
    /// @since 1.2
    /// </summary>
    public class SimpsonIntegrator : BaseAbstractUnivariateIntegrator
    {

        /// <summary>
        /// Maximal number of iterations for Simpson. </summary>
        public const int SIMPSON_MAX_ITERATIONS_COUNT = 64;

        /// <summary>
        /// Build a Simpson integrator with given accuracies and iterations counts. </summary>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations
        /// (must be less than or equal to <seealso cref="#SIMPSON_MAX_ITERATIONS_COUNT"/>) </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        /// <exception cref="NumberIsTooLargeException"> if maximal number of iterations
        /// is greater than <seealso cref="#SIMPSON_MAX_ITERATIONS_COUNT"/> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public SimpsonIntegrator(final double relativeAccuracy, final double absoluteAccuracy, final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NumberIsTooLargeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public SimpsonIntegrator(double relativeAccuracy, double absoluteAccuracy, int minimalIterationCount, int maximalIterationCount)
            : base(relativeAccuracy, absoluteAccuracy, minimalIterationCount, maximalIterationCount)
        {
            if (maximalIterationCount > SIMPSON_MAX_ITERATIONS_COUNT)
            {
                throw new NumberIsTooLargeException(maximalIterationCount, SIMPSON_MAX_ITERATIONS_COUNT, false);
            }
        }

        /// <summary>
        /// Build a Simpson integrator with given iteration counts. </summary>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations
        /// (must be less than or equal to <seealso cref="#SIMPSON_MAX_ITERATIONS_COUNT"/>) </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        /// <exception cref="NumberIsTooLargeException"> if maximal number of iterations
        /// is greater than <seealso cref="#SIMPSON_MAX_ITERATIONS_COUNT"/> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public SimpsonIntegrator(final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NumberIsTooLargeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public SimpsonIntegrator(int minimalIterationCount, int maximalIterationCount)
            : base(minimalIterationCount, maximalIterationCount)
        {
            if (maximalIterationCount > SIMPSON_MAX_ITERATIONS_COUNT)
            {
                throw new NumberIsTooLargeException(maximalIterationCount, SIMPSON_MAX_ITERATIONS_COUNT, false);
            }
        }

        /// <summary>
        /// Construct an integrator with default settings.
        /// (max iteration count set to <seealso cref="#SIMPSON_MAX_ITERATIONS_COUNT"/>)
        /// </summary>
        public SimpsonIntegrator()
            : base(DEFAULT_MIN_ITERATIONS_COUNT, SIMPSON_MAX_ITERATIONS_COUNT)
        {
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override protected double doIntegrate() throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.MaxCountExceededException
        protected internal override double doIntegrate()
        {

            TrapezoidIntegrator qtrap = new TrapezoidIntegrator();
            if (MinimalIterationCount == 1)
            {
                return (4 * qtrap.stage(this, 1) - qtrap.stage(this, 0)) / 3.0;
            }

            // Simpson's rule requires at least two trapezoid stages.
            double olds = 0;
            double oldt = qtrap.stage(this, 0);
            while (true)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double t = qtrap.stage(this, iterations.getCount());
                double t = qtrap.stage(this, iterations.Count);
                iterations.incrementCount();
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double s = (4 * t - oldt) / 3.0;
                double s = (4 * t - oldt) / 3.0;
                if (iterations.Count >= MinimalIterationCount)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final double delta = mathlib.util.FastMath.abs(s - olds);
                    double delta = FastMath.abs(s - olds);
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final double rLimit = getRelativeAccuracy() * (mathlib.util.FastMath.abs(olds) + mathlib.util.FastMath.abs(s)) * 0.5;
                    double rLimit = RelativeAccuracy * (FastMath.abs(olds) + FastMath.abs(s)) * 0.5;
                    if ((delta <= rLimit) || (delta <= AbsoluteAccuracy))
                    {
                        return s;
                    }
                }
                olds = s;
                oldt = t;
            }

        }

    }

}