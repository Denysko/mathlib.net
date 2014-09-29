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
namespace mathlib.analysis.integration
{

    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
    using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
    using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
    using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using FastMath = mathlib.util.FastMath;

    /// <summary>
    /// Implements the <a href="http://mathworld.wolfram.com/Legendre-GaussQuadrature.html">
    /// Legendre-Gauss</a> quadrature formula.
    /// <p>
    /// Legendre-Gauss integrators are efficient integrators that can
    /// accurately integrate functions with few function evaluations. A
    /// Legendre-Gauss integrator using an n-points quadrature formula can
    /// integrate 2n-1 degree polynomials exactly.
    /// </p>
    /// <p>
    /// These integrators evaluate the function on n carefully chosen
    /// abscissas in each step interval (mapped to the canonical [-1,1] interval).
    /// The evaluation abscissas are not evenly spaced and none of them are
    /// at the interval endpoints. This implies the function integrated can be
    /// undefined at integration interval endpoints.
    /// </p>
    /// <p>
    /// The evaluation abscissas x<sub>i</sub> are the roots of the degree n
    /// Legendre polynomial. The weights a<sub>i</sub> of the quadrature formula
    /// integrals from -1 to +1 &int; Li<sup>2</sup> where Li (x) =
    /// &prod; (x-x<sub>k</sub>)/(x<sub>i</sub>-x<sub>k</sub>) for k != i.
    /// </p>
    /// <p>
    /// @version $Id: LegendreGaussIntegrator.java 1455194 2013-03-11 15:45:54Z luc $
    /// @since 1.2 </summary>
    /// @deprecated As of 3.1 (to be removed in 4.0). Please use
    /// <seealso cref="IterativeLegendreGaussIntegrator"/> instead. 
    [Obsolete("As of 3.1 (to be removed in 4.0). Please use")]
    public class LegendreGaussIntegrator : BaseAbstractUnivariateIntegrator
    {

        /// <summary>
        /// Abscissas for the 2 points method. </summary>
        private static readonly double[] ABSCISSAS_2 = new double[] { -1.0 / FastMath.sqrt(3.0), 1.0 / FastMath.sqrt(3.0) };

        /// <summary>
        /// Weights for the 2 points method. </summary>
        private static readonly double[] WEIGHTS_2 = new double[] { 1.0, 1.0 };

        /// <summary>
        /// Abscissas for the 3 points method. </summary>
        private static readonly double[] ABSCISSAS_3 = new double[] { -FastMath.sqrt(0.6), 0.0, FastMath.sqrt(0.6) };

        /// <summary>
        /// Weights for the 3 points method. </summary>
        private static readonly double[] WEIGHTS_3 = new double[] { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };

        /// <summary>
        /// Abscissas for the 4 points method. </summary>
        private static readonly double[] ABSCISSAS_4 = new double[] { -FastMath.sqrt((15.0 + 2.0 * FastMath.sqrt(30.0)) / 35.0), -FastMath.sqrt((15.0 - 2.0 * FastMath.sqrt(30.0)) / 35.0), FastMath.sqrt((15.0 - 2.0 * FastMath.sqrt(30.0)) / 35.0), FastMath.sqrt((15.0 + 2.0 * FastMath.sqrt(30.0)) / 35.0) };

        /// <summary>
        /// Weights for the 4 points method. </summary>
        private static readonly double[] WEIGHTS_4 = new double[] { (90.0 - 5.0 * FastMath.sqrt(30.0)) / 180.0, (90.0 + 5.0 * FastMath.sqrt(30.0)) / 180.0, (90.0 + 5.0 * FastMath.sqrt(30.0)) / 180.0, (90.0 - 5.0 * FastMath.sqrt(30.0)) / 180.0 };

        /// <summary>
        /// Abscissas for the 5 points method. </summary>
        private static readonly double[] ABSCISSAS_5 = new double[] { -FastMath.sqrt((35.0 + 2.0 * FastMath.sqrt(70.0)) / 63.0), -FastMath.sqrt((35.0 - 2.0 * FastMath.sqrt(70.0)) / 63.0), 0.0, FastMath.sqrt((35.0 - 2.0 * FastMath.sqrt(70.0)) / 63.0), FastMath.sqrt((35.0 + 2.0 * FastMath.sqrt(70.0)) / 63.0) };

        /// <summary>
        /// Weights for the 5 points method. </summary>
        private static readonly double[] WEIGHTS_5 = new double[] { (322.0 - 13.0 * FastMath.sqrt(70.0)) / 900.0, (322.0 + 13.0 * FastMath.sqrt(70.0)) / 900.0, 128.0 / 225.0, (322.0 + 13.0 * FastMath.sqrt(70.0)) / 900.0, (322.0 - 13.0 * FastMath.sqrt(70.0)) / 900.0 };

        /// <summary>
        /// Abscissas for the current method. </summary>
        private readonly double[] abscissas;

        /// <summary>
        /// Weights for the current method. </summary>
        private readonly double[] weights;

        /// <summary>
        /// Build a Legendre-Gauss integrator with given accuracies and iterations counts. </summary>
        /// <param name="n"> number of points desired (must be between 2 and 5 inclusive) </param>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations </param>
        /// <exception cref="MathIllegalArgumentException"> if number of points is out of [2; 5] </exception>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LegendreGaussIntegrator(final int n, final double relativeAccuracy, final double absoluteAccuracy, final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public LegendreGaussIntegrator(int n, double relativeAccuracy, double absoluteAccuracy, int minimalIterationCount, int maximalIterationCount)
            : base(relativeAccuracy, absoluteAccuracy, minimalIterationCount, maximalIterationCount)
        {
            switch (n)
            {
                case 2:
                    abscissas = ABSCISSAS_2;
                    weights = WEIGHTS_2;
                    break;
                case 3:
                    abscissas = ABSCISSAS_3;
                    weights = WEIGHTS_3;
                    break;
                case 4:
                    abscissas = ABSCISSAS_4;
                    weights = WEIGHTS_4;
                    break;
                case 5:
                    abscissas = ABSCISSAS_5;
                    weights = WEIGHTS_5;
                    break;
                default:
                    throw new MathIllegalArgumentException(LocalizedFormats.N_POINTS_GAUSS_LEGENDRE_INTEGRATOR_NOT_SUPPORTED, n, 2, 5);
            }

        }

        /// <summary>
        /// Build a Legendre-Gauss integrator with given accuracies. </summary>
        /// <param name="n"> number of points desired (must be between 2 and 5 inclusive) </param>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        /// <exception cref="MathIllegalArgumentException"> if number of points is out of [2; 5] </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LegendreGaussIntegrator(final int n, final double relativeAccuracy, final double absoluteAccuracy) throws mathlib.exception.MathIllegalArgumentException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public LegendreGaussIntegrator(int n, double relativeAccuracy, double absoluteAccuracy)
            : this(n, relativeAccuracy, absoluteAccuracy, DEFAULT_MIN_ITERATIONS_COUNT, DEFAULT_MAX_ITERATIONS_COUNT)
        {
        }

        /// <summary>
        /// Build a Legendre-Gauss integrator with given iteration counts. </summary>
        /// <param name="n"> number of points desired (must be between 2 and 5 inclusive) </param>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations </param>
        /// <exception cref="MathIllegalArgumentException"> if number of points is out of [2; 5] </exception>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public LegendreGaussIntegrator(final int n, final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.MathIllegalArgumentException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public LegendreGaussIntegrator(int n, int minimalIterationCount, int maximalIterationCount)
            : this(n, DEFAULT_RELATIVE_ACCURACY, DEFAULT_ABSOLUTE_ACCURACY, minimalIterationCount, maximalIterationCount)
        {
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: @Override protected double doIntegrate() throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.TooManyEvaluationsException, mathlib.exception.MaxCountExceededException
        protected internal override double doIntegrate()
        {

            // compute first estimate with a single step
            double oldt = stage(1);

            int n = 2;
            while (true)
            {

                // improve integral with a larger number of steps
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double t = stage(n);
                double t = stage(n);

                // estimate error
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double delta = mathlib.util.FastMath.abs(t - oldt);
                double delta = FastMath.abs(t - oldt);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double limit = mathlib.util.FastMath.max(getAbsoluteAccuracy(), getRelativeAccuracy() * (mathlib.util.FastMath.abs(oldt) + mathlib.util.FastMath.abs(t)) * 0.5);
                double limit = FastMath.max(AbsoluteAccuracy, RelativeAccuracy * (FastMath.abs(oldt) + FastMath.abs(t)) * 0.5);

                // check convergence
                if ((iterations.Count + 1 >= MinimalIterationCount) && (delta <= limit))
                {
                    return t;
                }

                // prepare next iteration
                double ratio = FastMath.min(4, FastMath.pow(delta / limit, 0.5 / abscissas.Length));
                n = FastMath.max((int)(ratio * n), n + 1);
                oldt = t;
                iterations.incrementCount();

            }

        }

        /// <summary>
        /// Compute the n-th stage integral. </summary>
        /// <param name="n"> number of steps </param>
        /// <returns> the value of n-th stage integral </returns>
        /// <exception cref="TooManyEvaluationsException"> if the maximum number of evaluations
        /// is exceeded. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private double stage(final int n) throws mathlib.exception.TooManyEvaluationsException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        private double stage(int n)
        {

            // set up the step for the current stage
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double step = (getMax() - getMin()) / n;
            double step = (Max - Min) / n;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double halfStep = step / 2.0;
            double halfStep = step / 2.0;

            // integrate over all elementary steps
            double midPoint = Min + halfStep;
            double sum = 0.0;
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < abscissas.Length; ++j)
                {
                    sum += weights[j] * computeObjectiveValue(midPoint + halfStep * abscissas[j]);
                }
                midPoint += step;
            }

            return halfStep * sum;

        }

    }

}