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

    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
    using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
    using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
    using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
    using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
    using FastMath = mathlib.util.FastMath;

    /// <summary>
    /// Implements the <a href="http://en.wikipedia.org/wiki/Midpoint_method">
    /// Midpoint Rule</a> for integration of real univariate functions. For
    /// reference, see <b>Numerical Mathematics</b>, ISBN 0387989595,
    /// chapter 9.2.
    /// <p>
    /// The function should be integrable.</p>
    /// 
    /// @version $Id: MidPointIntegrator.java 1499813 2013-07-04 17:24:47Z sebb $
    /// @since 3.3
    /// </summary>
    public class MidPointIntegrator : BaseAbstractUnivariateIntegrator
    {

        /// <summary>
        /// Maximum number of iterations for midpoint. </summary>
        public const int MIDPOINT_MAX_ITERATIONS_COUNT = 64;

        /// <summary>
        /// Build a midpoint integrator with given accuracies and iterations counts. </summary>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations
        /// (must be less than or equal to <seealso cref="#MIDPOINT_MAX_ITERATIONS_COUNT"/> </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        /// <exception cref="NumberIsTooLargeException"> if maximal number of iterations
        /// is greater than <seealso cref="#MIDPOINT_MAX_ITERATIONS_COUNT"/> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public MidPointIntegrator(final double relativeAccuracy, final double absoluteAccuracy, final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NumberIsTooLargeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public MidPointIntegrator(double relativeAccuracy, double absoluteAccuracy, int minimalIterationCount, int maximalIterationCount)
            : base(relativeAccuracy, absoluteAccuracy, minimalIterationCount, maximalIterationCount)
        {
            if (maximalIterationCount > MIDPOINT_MAX_ITERATIONS_COUNT)
            {
                throw new NumberIsTooLargeException(maximalIterationCount, MIDPOINT_MAX_ITERATIONS_COUNT, false);
            }
        }

        /// <summary>
        /// Build a midpoint integrator with given iteration counts. </summary>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations
        /// (must be less than or equal to <seealso cref="#MIDPOINT_MAX_ITERATIONS_COUNT"/> </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        /// <exception cref="NumberIsTooLargeException"> if maximal number of iterations
        /// is greater than <seealso cref="#MIDPOINT_MAX_ITERATIONS_COUNT"/> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public MidPointIntegrator(final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.NumberIsTooLargeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public MidPointIntegrator(int minimalIterationCount, int maximalIterationCount)
            : base(minimalIterationCount, maximalIterationCount)
        {
            if (maximalIterationCount > MIDPOINT_MAX_ITERATIONS_COUNT)
            {
                throw new NumberIsTooLargeException(maximalIterationCount, MIDPOINT_MAX_ITERATIONS_COUNT, false);
            }
        }

        /// <summary>
        /// Construct a midpoint integrator with default settings.
        /// (max iteration count set to <seealso cref="#MIDPOINT_MAX_ITERATIONS_COUNT"/>)
        /// </summary>
        public MidPointIntegrator()
            : base(DEFAULT_MIN_ITERATIONS_COUNT, MIDPOINT_MAX_ITERATIONS_COUNT)
        {
        }

        /// <summary>
        /// Compute the n-th stage integral of midpoint rule.
        /// This function should only be called by API <code>integrate()</code> in the package.
        /// To save time it does not verify arguments - caller does.
        /// <p>
        /// The interval is divided equally into 2^n sections rather than an
        /// arbitrary m sections because this configuration can best utilize the
        /// already computed values.</p>
        /// </summary>
        /// <param name="n"> the stage of 1/2 refinement. Must be larger than 0. </param>
        /// <param name="previousStageResult"> Result from the previous call to the
        /// {@code stage} method. </param>
        /// <param name="min"> Lower bound of the integration interval. </param>
        /// <param name="diffMaxMin"> Difference between the lower bound and upper bound
        /// of the integration interval. </param>
        /// <returns> the value of n-th stage integral </returns>
        /// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
        /// is exceeded. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private double stage(final int n, double previousStageResult, double min, double diffMaxMin) throws mathlib.exception.TooManyEvaluationsException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        private double stage(int n, double previousStageResult, double min, double diffMaxMin)
        {

            // number of new points in this stage
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final long np = 1L << (n - 1);
            long np = 1L << (n - 1);
            double sum = 0;

            // spacing between adjacent new points
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double spacing = diffMaxMin / np;
            double spacing = diffMaxMin / np;

            // the first new point
            double x = min + 0.5 * spacing;
            for (long i = 0; i < np; i++)
            {
                sum += computeObjectiveValue(x);
                x += spacing;
            }
            // add the new sum to previously calculated result
            return 0.5 * (previousStageResult + sum * spacing);
        }


        /// <summary>
        /// {@inheritDoc} </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override protected double doIntegrate() throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.TooManyEvaluationsException, mathlib.exception.MaxCountExceededException
        protected internal override double doIntegrate()
        {

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double min = getMin();
            double min = Min;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double diff = getMax() - min;
            double diff = Max - min;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double midPoint = min + 0.5 * diff;
            double midPoint = min + 0.5 * diff;

            double oldt = diff * computeObjectiveValue(midPoint);

            while (true)
            {
                iterations.incrementCount();
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int i = iterations.getCount();
                int i = iterations.Count;
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double t = stage(i, oldt, min, diff);
                double t = stage(i, oldt, min, diff);
                if (i >= MinimalIterationCount)
                {
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final double delta = mathlib.util.FastMath.abs(t - oldt);
                    double delta = FastMath.abs(t - oldt);
                    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                    //ORIGINAL LINE: final double rLimit = getRelativeAccuracy() * (mathlib.util.FastMath.abs(oldt) + mathlib.util.FastMath.abs(t)) * 0.5;
                    double rLimit = RelativeAccuracy * (FastMath.abs(oldt) + FastMath.abs(t)) * 0.5;
                    if ((delta <= rLimit) || (delta <= AbsoluteAccuracy))
                    {
                        return t;
                    }
                }
                oldt = t;
            }

        }

    }

}