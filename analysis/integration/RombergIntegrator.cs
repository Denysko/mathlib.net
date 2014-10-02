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
    /// Implements the <a href="http://mathworld.wolfram.com/RombergIntegration.html">
    /// Romberg Algorithm</a> for integration of real univariate functions. For
    /// reference, see <b>Introduction to Numerical Analysis</b>, ISBN 038795452X,
    /// chapter 3.
    /// <p>
    /// Romberg integration employs k successive refinements of the trapezoid
    /// rule to remove error terms less than order O(N^(-2k)). Simpson's rule
    /// is a special case of k = 2.</p>
    /// 
    /// @version $Id: RombergIntegrator.java 1364387 2012-07-22 18:14:11Z tn $
    /// @since 1.2
    /// </summary>
    public class RombergIntegrator : BaseAbstractUnivariateIntegrator
    {

        /// <summary>
        /// Maximal number of iterations for Romberg. 
        /// </summary>
        public const int ROMBERG_MAX_ITERATIONS_COUNT = 32;

        /// <summary>
        /// Build a Romberg integrator with given accuracies and iterations counts. 
        /// </summary>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations
        /// (must be less than or equal to <seealso cref="#ROMBERG_MAX_ITERATIONS_COUNT"/>) </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        /// <exception cref="NumberIsTooLargeException"> if maximal number of iterations
        /// is greater than <seealso cref="#ROMBERG_MAX_ITERATIONS_COUNT"/> </exception>
        public RombergIntegrator(double relativeAccuracy, double absoluteAccuracy, int minimalIterationCount, int maximalIterationCount)
            : base(relativeAccuracy, absoluteAccuracy, minimalIterationCount, maximalIterationCount)
        {
            if (maximalIterationCount > ROMBERG_MAX_ITERATIONS_COUNT)
            {
                throw new NumberIsTooLargeException(maximalIterationCount, ROMBERG_MAX_ITERATIONS_COUNT, false);
            }
        }

        /// <summary>
        /// Build a Romberg integrator with given iteration counts. </summary>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations
        /// (must be less than or equal to <seealso cref="#ROMBERG_MAX_ITERATIONS_COUNT"/>) </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        /// <exception cref="NumberIsTooLargeException"> if maximal number of iterations
        /// is greater than <seealso cref="#ROMBERG_MAX_ITERATIONS_COUNT"/> </exception>
        public RombergIntegrator(int minimalIterationCount, int maximalIterationCount)
            : base(minimalIterationCount, maximalIterationCount)
        {
            if (maximalIterationCount > ROMBERG_MAX_ITERATIONS_COUNT)
            {
                throw new NumberIsTooLargeException(maximalIterationCount, ROMBERG_MAX_ITERATIONS_COUNT, false);
            }
        }

        /// <summary>
        /// Construct a Romberg integrator with default settings
        /// (max iteration count set to <seealso cref="#ROMBERG_MAX_ITERATIONS_COUNT"/>)
        /// </summary>
        public RombergIntegrator()
            : base(DEFAULT_MIN_ITERATIONS_COUNT, ROMBERG_MAX_ITERATIONS_COUNT)
        {
        }

        /// <summary>
        /// {@inheritDoc} 
        /// </summary>
        protected internal override double doIntegrate()
        {
            int m = iterations.MaximalCount + 1;
            double[] previousRow = new double[m];
            double[] currentRow = new double[m];

            TrapezoidIntegrator qtrap = new TrapezoidIntegrator();
            currentRow[0] = qtrap.stage(this, 0);
            iterations.incrementCount();
            double olds = currentRow[0];
            while (true)
            {
                int i = iterations.Count;

                // switch rows
                double[] tmpRow = previousRow;
                previousRow = currentRow;
                currentRow = tmpRow;

                currentRow[0] = qtrap.stage(this, i);
                iterations.incrementCount();
                for (int j = 1; j <= i; j++)
                {
                    // Richardson extrapolation coefficient
                    double r = (1L << (2 * j)) - 1;
                    double tIJm1 = currentRow[j - 1];
                    currentRow[j] = tIJm1 + (tIJm1 - previousRow[j - 1]) / r;
                }
                double s = currentRow[i];
                if (i >= MinimalIterationCount)
                {
                    double delta = FastMath.abs(s - olds);
                    double rLimit = RelativeAccuracy * (FastMath.abs(olds) + FastMath.abs(s)) * 0.5;
                    if ((delta <= rLimit) || (delta <= AbsoluteAccuracy))
                    {
                        return s;
                    }
                }
                olds = s;
            }

        }

    }

}