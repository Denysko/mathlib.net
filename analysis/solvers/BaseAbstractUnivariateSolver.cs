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

namespace mathlib.analysis.solvers
{

    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
    using NoBracketingException = mathlib.exception.NoBracketingException;
    using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
    using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
    using NullArgumentException = mathlib.exception.NullArgumentException;
    using Incrementor = mathlib.util.Incrementor;
    using MathUtils = mathlib.util.MathUtils;

    /// <summary>
    /// Provide a default implementation for several functions useful to generic
    /// solvers.
    /// </summary>
    /// @param <FUNC> Type of function to solve.
    /// 
    /// @since 2.0
    /// @version $Id: BaseAbstractUnivariateSolver.java 1455194 2013-03-11 15:45:54Z luc $ </param>
    public abstract class BaseAbstractUnivariateSolver<FUNC> : BaseUnivariateSolver<FUNC> where FUNC : org.apache.commons.math3.analysis.UnivariateFunction
    {
        /// <summary>
        /// Default relative accuracy. </summary>
        private const double DEFAULT_RELATIVE_ACCURACY = 1e-14;
        /// <summary>
        /// Default function value accuracy. </summary>
        private const double DEFAULT_FUNCTION_VALUE_ACCURACY = 1e-15;
        /// <summary>
        /// Function value accuracy. </summary>
        private readonly double functionValueAccuracy;
        /// <summary>
        /// Absolute accuracy. </summary>
        private readonly double absoluteAccuracy;
        /// <summary>
        /// Relative accuracy. </summary>
        private readonly double relativeAccuracy;
        /// <summary>
        /// Evaluations counter. </summary>
        private readonly Incrementor evaluations = new Incrementor();
        /// <summary>
        /// Lower end of search interval. </summary>
        private double searchMin;
        /// <summary>
        /// Higher end of search interval. </summary>
        private double searchMax;
        /// <summary>
        /// Initial guess. </summary>
        private double searchStart;
        /// <summary>
        /// Function to solve. </summary>
        private FUNC function;

        /// <summary>
        /// Construct a solver with given absolute accuracy.
        /// </summary>
        /// <param name="absoluteAccuracy"> Maximum absolute error. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected BaseAbstractUnivariateSolver(final double absoluteAccuracy)
        protected internal BaseAbstractUnivariateSolver(double absoluteAccuracy)
            : this(DEFAULT_RELATIVE_ACCURACY, absoluteAccuracy, DEFAULT_FUNCTION_VALUE_ACCURACY)
        {
        }

        /// <summary>
        /// Construct a solver with given accuracies.
        /// </summary>
        /// <param name="relativeAccuracy"> Maximum relative error. </param>
        /// <param name="absoluteAccuracy"> Maximum absolute error. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected BaseAbstractUnivariateSolver(final double relativeAccuracy, final double absoluteAccuracy)
        protected internal BaseAbstractUnivariateSolver(double relativeAccuracy, double absoluteAccuracy)
            : this(relativeAccuracy, absoluteAccuracy, DEFAULT_FUNCTION_VALUE_ACCURACY)
        {
        }

        /// <summary>
        /// Construct a solver with given accuracies.
        /// </summary>
        /// <param name="relativeAccuracy"> Maximum relative error. </param>
        /// <param name="absoluteAccuracy"> Maximum absolute error. </param>
        /// <param name="functionValueAccuracy"> Maximum function value error. </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected BaseAbstractUnivariateSolver(final double relativeAccuracy, final double absoluteAccuracy, final double functionValueAccuracy)
        protected internal BaseAbstractUnivariateSolver(double relativeAccuracy, double absoluteAccuracy, double functionValueAccuracy)
        {
            this.absoluteAccuracy = absoluteAccuracy;
            this.relativeAccuracy = relativeAccuracy;
            this.functionValueAccuracy = functionValueAccuracy;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual int MaxEvaluations
        {
            get
            {
                return evaluations.MaximalCount;
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
        /// <returns> the lower end of the search interval. </returns>
        public virtual double Min
        {
            get
            {
                return searchMin;
            }
        }
        /// <returns> the higher end of the search interval. </returns>
        public virtual double Max
        {
            get
            {
                return searchMax;
            }
        }
        /// <returns> the initial guess. </returns>
        public virtual double StartValue
        {
            get
            {
                return searchStart;
            }
        }
        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        public virtual double AbsoluteAccuracy
        {
            get
            {
                return absoluteAccuracy;
            }
        }
        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        public virtual double RelativeAccuracy
        {
            get
            {
                return relativeAccuracy;
            }
        }
        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        public virtual double FunctionValueAccuracy
        {
            get
            {
                return functionValueAccuracy;
            }
        }

        /// <summary>
        /// Compute the objective function value.
        /// </summary>
        /// <param name="point"> Point at which the objective function must be evaluated. </param>
        /// <returns> the objective function value at specified point. </returns>
        /// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
        /// is exceeded. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected double computeObjectiveValue(double point) throws org.apache.commons.math3.exception.TooManyEvaluationsException
        protected internal virtual double computeObjectiveValue(double point)
        {
            incrementEvaluationCount();
            return function.value(point);
        }

        /// <summary>
        /// Prepare for computation.
        /// Subclasses must call this method if they override any of the
        /// {@code solve} methods.
        /// </summary>
        /// <param name="f"> Function to solve. </param>
        /// <param name="min"> Lower bound for the interval. </param>
        /// <param name="max"> Upper bound for the interval. </param>
        /// <param name="startValue"> Start value to use. </param>
        /// <param name="maxEval"> Maximum number of evaluations. </param>
        /// <exception cref="NullArgumentException"> if f is null </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void setup(int maxEval, FUNC f, double min, double max, double startValue) throws org.apache.commons.math3.exception.NullArgumentException
        protected internal virtual void setup(int maxEval, FUNC f, double min, double max, double startValue)
        {
            // Checks.
            MathUtils.checkNotNull(f);

            // Reset.
            searchMin = min;
            searchMax = max;
            searchStart = startValue;
            function = f;
            evaluations.MaximalCount = maxEval;
            evaluations.resetCount();
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public double solve(int maxEval, FUNC f, double min, double max, double startValue) throws org.apache.commons.math3.exception.TooManyEvaluationsException, org.apache.commons.math3.exception.NoBracketingException
        public virtual double solve(int maxEval, FUNC f, double min, double max, double startValue)
        {
            // Initialization.
            setup(maxEval, f, min, max, startValue);

            // Perform computation.
            return doSolve();
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual double solve(int maxEval, FUNC f, double min, double max)
        {
            return solve(maxEval, f, min, max, min + 0.5 * (max - min));
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public double solve(int maxEval, FUNC f, double startValue) throws org.apache.commons.math3.exception.TooManyEvaluationsException, org.apache.commons.math3.exception.NoBracketingException
        public virtual double solve(int maxEval, FUNC f, double startValue)
        {
            return solve(maxEval, f, double.NaN, double.NaN, startValue);
        }

        /// <summary>
        /// Method for implementing actual optimization algorithms in derived
        /// classes.
        /// </summary>
        /// <returns> the root. </returns>
        /// <exception cref="TooManyEvaluationsException"> if the maximal number of evaluations
        /// is exceeded. </exception>
        /// <exception cref="NoBracketingException"> if the initial search interval does not bracket
        /// a root and the solver requires it. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected abstract double doSolve() throws org.apache.commons.math3.exception.TooManyEvaluationsException, org.apache.commons.math3.exception.NoBracketingException;
        protected internal abstract double doSolve();

        /// <summary>
        /// Check whether the function takes opposite signs at the endpoints.
        /// </summary>
        /// <param name="lower"> Lower endpoint. </param>
        /// <param name="upper"> Upper endpoint. </param>
        /// <returns> {@code true} if the function values have opposite signs at the
        /// given points. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected boolean isBracketing(final double lower, final double upper)
        protected internal virtual bool isBracketing(double lower, double upper)
        {
            return UnivariateSolverUtils.isBracketing(function, lower, upper);
        }

        /// <summary>
        /// Check whether the arguments form a (strictly) increasing sequence.
        /// </summary>
        /// <param name="start"> First number. </param>
        /// <param name="mid"> Second number. </param>
        /// <param name="end"> Third number. </param>
        /// <returns> {@code true} if the arguments form an increasing sequence. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: protected boolean isSequence(final double start, final double mid, final double end)
        protected internal virtual bool isSequence(double start, double mid, double end)
        {
            return UnivariateSolverUtils.isSequence(start, mid, end);
        }

        /// <summary>
        /// Check that the endpoints specify an interval.
        /// </summary>
        /// <param name="lower"> Lower endpoint. </param>
        /// <param name="upper"> Upper endpoint. </param>
        /// <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void verifyInterval(final double lower, final double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal virtual void verifyInterval(double lower, double upper)
        {
            UnivariateSolverUtils.verifyInterval(lower, upper);
        }

        /// <summary>
        /// Check that {@code lower < initial < upper}.
        /// </summary>
        /// <param name="lower"> Lower endpoint. </param>
        /// <param name="initial"> Initial value. </param>
        /// <param name="upper"> Upper endpoint. </param>
        /// <exception cref="NumberIsTooLargeException"> if {@code lower >= initial} or
        /// {@code initial >= upper}. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void verifySequence(final double lower, final double initial, final double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal virtual void verifySequence(double lower, double initial, double upper)
        {
            UnivariateSolverUtils.verifySequence(lower, initial, upper);
        }

        /// <summary>
        /// Check that the endpoints specify an interval and the function takes
        /// opposite signs at the endpoints.
        /// </summary>
        /// <param name="lower"> Lower endpoint. </param>
        /// <param name="upper"> Upper endpoint. </param>
        /// <exception cref="NullArgumentException"> if the function has not been set. </exception>
        /// <exception cref="NoBracketingException"> if the function has the same sign at
        /// the endpoints. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void verifyBracketing(final double lower, final double upper) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoBracketingException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        protected internal virtual void verifyBracketing(double lower, double upper)
        {
            UnivariateSolverUtils.verifyBracketing(function, lower, upper);
        }

        /// <summary>
        /// Increment the evaluation count by one.
        /// Method <seealso cref="#computeObjectiveValue(double)"/> calls this method internally.
        /// It is provided for subclasses that do not exclusively use
        /// {@code computeObjectiveValue} to solve the function.
        /// See e.g. <seealso cref="AbstractUnivariateDifferentiableSolver"/>.
        /// </summary>
        /// <exception cref="TooManyEvaluationsException"> when the allowed number of function
        /// evaluations has been exhausted. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: protected void incrementEvaluationCount() throws org.apache.commons.math3.exception.TooManyEvaluationsException
        protected internal virtual void incrementEvaluationCount()
        {
            try
            {
                evaluations.incrementCount();
            }
            catch (MaxCountExceededException e)
            {
                throw new TooManyEvaluationsException(e.Max);
            }
        }
    }

}