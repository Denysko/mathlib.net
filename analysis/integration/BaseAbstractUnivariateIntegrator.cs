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

    using UnivariateSolverUtils = mathlib.analysis.solvers.UnivariateSolverUtils;
    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
    using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
    using NullArgumentException = mathlib.exception.NullArgumentException;
    using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
    using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
    using Incrementor = mathlib.util.Incrementor;
    using MathUtils = mathlib.util.MathUtils;

    /// <summary>
    /// Provide a default implementation for several generic functions.
    /// 
    /// @version $Id: BaseAbstractUnivariateIntegrator.java 1455194 2013-03-11 15:45:54Z luc $
    /// @since 1.2
    /// </summary>
    public abstract class BaseAbstractUnivariateIntegrator : UnivariateIntegrator
    {

        /// <summary>
        /// Default absolute accuracy. </summary>
        public const double DEFAULT_ABSOLUTE_ACCURACY = 1.0e-15;

        /// <summary>
        /// Default relative accuracy. </summary>
        public const double DEFAULT_RELATIVE_ACCURACY = 1.0e-6;

        /// <summary>
        /// Default minimal iteration count. </summary>
        public const int DEFAULT_MIN_ITERATIONS_COUNT = 3;

        /// <summary>
        /// Default maximal iteration count. </summary>
        public static readonly int DEFAULT_MAX_ITERATIONS_COUNT = int.MaxValue;

        /// <summary>
        /// The iteration count. </summary>
        protected internal readonly Incrementor iterations;

        /// <summary>
        /// Maximum absolute error. </summary>
        private readonly double absoluteAccuracy;

        /// <summary>
        /// Maximum relative error. </summary>
        private readonly double relativeAccuracy;

        /// <summary>
        /// minimum number of iterations </summary>
        private readonly int minimalIterationCount;

        /// <summary>
        /// The functions evaluation count. </summary>
        private readonly Incrementor evaluations;

        /// <summary>
        /// Function to integrate. </summary>
        private UnivariateFunction function;

        /// <summary>
        /// Lower bound for the interval. </summary>
        private double min;

        /// <summary>
        /// Upper bound for the interval. </summary>
        private double max;

        /// <summary>
        /// Construct an integrator with given accuracies and iteration counts.
        /// <p>
        /// The meanings of the various parameters are:
        /// <ul>
        ///   <li>relative accuracy:
        ///       this is used to stop iterations if the absolute accuracy can't be
        ///       achieved due to large values or short mantissa length. If this
        ///       should be the primary criterion for convergence rather then a
        ///       safety measure, set the absolute accuracy to a ridiculously small value,
        ///       like <seealso cref="mathlib.util.Precision#SAFE_MIN Precision.SAFE_MIN"/>.</li>
        ///   <li>absolute accuracy:
        ///       The default is usually chosen so that results in the interval
        ///       -10..-0.1 and +0.1..+10 can be found with a reasonable accuracy. If the
        ///       expected absolute value of your results is of much smaller magnitude, set
        ///       this to a smaller value.</li>
        ///   <li>minimum number of iterations:
        ///       minimal iteration is needed to avoid false early convergence, e.g.
        ///       the sample points happen to be zeroes of the function. Users can
        ///       use the default value or choose one that they see as appropriate.</li>
        ///   <li>maximum number of iterations:
        ///       usually a high iteration count indicates convergence problems. However,
        ///       the "reasonable value" varies widely for different algorithms. Users are
        ///       advised to use the default value supplied by the algorithm.</li>
        /// </ul>
        /// </p> </summary>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected BaseAbstractUnivariateIntegrator(final double relativeAccuracy, final double absoluteAccuracy, final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal BaseAbstractUnivariateIntegrator(double relativeAccuracy, double absoluteAccuracy, int minimalIterationCount, int maximalIterationCount)
        {

            // accuracy settings
            this.relativeAccuracy = relativeAccuracy;
            this.absoluteAccuracy = absoluteAccuracy;

            // iterations count settings
            if (minimalIterationCount <= 0)
            {
                throw new NotStrictlyPositiveException(minimalIterationCount);
            }
            if (maximalIterationCount <= minimalIterationCount)
            {
                throw new NumberIsTooSmallException(maximalIterationCount, minimalIterationCount, false);
            }
            this.minimalIterationCount = minimalIterationCount;
            this.iterations = new Incrementor();
            iterations.MaximalCount = maximalIterationCount;

            // prepare evaluations counter, but do not set it yet
            evaluations = new Incrementor();

        }

        /// <summary>
        /// Construct an integrator with given accuracies. </summary>
        /// <param name="relativeAccuracy"> relative accuracy of the result </param>
        /// <param name="absoluteAccuracy"> absolute accuracy of the result </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected BaseAbstractUnivariateIntegrator(final double relativeAccuracy, final double absoluteAccuracy)
        protected internal BaseAbstractUnivariateIntegrator(double relativeAccuracy, double absoluteAccuracy)
            : this(relativeAccuracy, absoluteAccuracy, DEFAULT_MIN_ITERATIONS_COUNT, DEFAULT_MAX_ITERATIONS_COUNT)
        {
        }

        /// <summary>
        /// Construct an integrator with given iteration counts. </summary>
        /// <param name="minimalIterationCount"> minimum number of iterations </param>
        /// <param name="maximalIterationCount"> maximum number of iterations </param>
        /// <exception cref="NotStrictlyPositiveException"> if minimal number of iterations
        /// is not strictly positive </exception>
        /// <exception cref="NumberIsTooSmallException"> if maximal number of iterations
        /// is lesser than or equal to the minimal number of iterations </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected BaseAbstractUnivariateIntegrator(final int minimalIterationCount, final int maximalIterationCount) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.NumberIsTooSmallException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal BaseAbstractUnivariateIntegrator(int minimalIterationCount, int maximalIterationCount)
            : this(DEFAULT_RELATIVE_ACCURACY, DEFAULT_ABSOLUTE_ACCURACY, minimalIterationCount, maximalIterationCount)
        {
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual double RelativeAccuracy
        {
            get
            {
                return relativeAccuracy;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual double AbsoluteAccuracy
        {
            get
            {
                return absoluteAccuracy;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual int MinimalIterationCount
        {
            get
            {
                return minimalIterationCount;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual int MaximalIterationCount
        {
            get
            {
                return iterations.MaximalCount;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual int Evaluations
        {
            get
            {
                return evaluations.Count;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual int Iterations
        {
            get
            {
                return iterations.Count;
            }
        }

        /// <returns> the lower bound. </returns>
        protected internal virtual double Min
        {
            get
            {
                return min;
            }
        }
        /// <returns> the upper bound. </returns>
        protected internal virtual double Max
        {
            get
            {
                return max;
            }
        }

        /// <summary>
        /// Compute the objective function value.
        /// </summary>
        /// <param name="point"> Point at which the objective function must be evaluated. </param>
        /// <returns> the objective function value at specified point. </returns>
        /// <exception cref="TooManyEvaluationsException"> if the maximal number of function
        /// evaluations is exceeded. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected double computeObjectiveValue(final double point) throws mathlib.exception.TooManyEvaluationsException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal virtual double computeObjectiveValue(double point)
        {
            try
            {
                evaluations.incrementCount();
            }
            catch (MaxCountExceededException e)
            {
                throw new TooManyEvaluationsException(e.Max);
            }
            return function.value(point);
        }

        /// <summary>
        /// Prepare for computation.
        /// Subclasses must call this method if they override any of the
        /// {@code solve} methods.
        /// </summary>
        /// <param name="maxEval"> Maximum number of evaluations. </param>
        /// <param name="f"> the integrand function </param>
        /// <param name="lower"> the min bound for the interval </param>
        /// <param name="upper"> the upper bound for the interval </param>
        /// <exception cref="NullArgumentException"> if {@code f} is {@code null}. </exception>
        /// <exception cref="MathIllegalArgumentException"> if {@code min >= max}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void setup(final int maxEval, final mathlib.analysis.UnivariateFunction f, final double lower, final double upper) throws mathlib.exception.NullArgumentException, mathlib.exception.MathIllegalArgumentException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal virtual void setup(int maxEval, UnivariateFunction f, double lower, double upper)
        {

            // Checks.
            MathUtils.checkNotNull(f);
            UnivariateSolverUtils.verifyInterval(lower, upper);

            // Reset.
            min = lower;
            max = upper;
            function = f;
            evaluations.MaximalCount = maxEval;
            evaluations.resetCount();
            iterations.resetCount();

        }

        /// <summary>
        /// {@inheritDoc} </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public double integrate(final int maxEval, final mathlib.analysis.UnivariateFunction f, final double lower, final double upper) throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.MaxCountExceededException, mathlib.exception.MathIllegalArgumentException, mathlib.exception.NullArgumentException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        public virtual double integrate(int maxEval, UnivariateFunction f, double lower, double upper)
        {

            // Initialization.
            setup(maxEval, f, lower, upper);

            // Perform computation.
            return doIntegrate();

        }

        /// <summary>
        /// Method for implementing actual integration algorithms in derived
        /// classes.
        /// </summary>
        /// <returns> the root. </returns>
        /// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
        /// is exceeded. </exception>
        /// <exception cref="MaxCountExceededException"> if the maximum iteration count is exceeded
        /// or the integrator detects convergence problems otherwise </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected abstract double doIntegrate() throws mathlib.exception.TooManyEvaluationsException, mathlib.exception.MaxCountExceededException;
        protected internal abstract double doIntegrate();

    }

}