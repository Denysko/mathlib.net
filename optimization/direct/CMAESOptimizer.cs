using System;
using System.Collections.Generic;

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

namespace mathlib.optimization.direct
{


	using MultivariateFunction = mathlib.analysis.MultivariateFunction;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using TooManyEvaluationsException = mathlib.exception.TooManyEvaluationsException;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using EigenDecomposition = mathlib.linear.EigenDecomposition;
	using MatrixUtils = mathlib.linear.MatrixUtils;
	using RealMatrix = mathlib.linear.RealMatrix;
	using mathlib.optimization;
	using MersenneTwister = mathlib.random.MersenneTwister;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// <p>An implementation of the active Covariance Matrix Adaptation Evolution Strategy (CMA-ES)
	/// for non-linear, non-convex, non-smooth, global function minimization.
	/// The CMA-Evolution Strategy (CMA-ES) is a reliable stochastic optimization method
	/// which should be applied if derivative-based methods, e.g. quasi-Newton BFGS or
	/// conjugate gradient, fail due to a rugged search landscape (e.g. noise, local
	/// optima, outlier, etc.) of the objective function. Like a
	/// quasi-Newton method, the CMA-ES learns and applies a variable metric
	/// on the underlying search space. Unlike a quasi-Newton method, the
	/// CMA-ES neither estimates nor uses gradients, making it considerably more
	/// reliable in terms of finding a good, or even close to optimal, solution.</p>
	/// 
	/// <p>In general, on smooth objective functions the CMA-ES is roughly ten times
	/// slower than BFGS (counting objective function evaluations, no gradients provided).
	/// For up to <math>N=10</math> variables also the derivative-free simplex
	/// direct search method (Nelder and Mead) can be faster, but it is
	/// far less reliable than CMA-ES.</p>
	/// 
	/// <p>The CMA-ES is particularly well suited for non-separable
	/// and/or badly conditioned problems. To observe the advantage of CMA compared
	/// to a conventional evolution strategy, it will usually take about
	/// <math>30 N</math> function evaluations. On difficult problems the complete
	/// optimization (a single run) is expected to take <em>roughly</em> between
	/// <math>30 N</math> and <math>300 N<sup>2</sup></math>
	/// function evaluations.</p>
	/// 
	/// <p>This implementation is translated and adapted from the Matlab version
	/// of the CMA-ES algorithm as implemented in module {@code cmaes.m} version 3.51.</p>
	/// 
	/// For more information, please refer to the following links:
	/// <ul>
	///  <li><a href="http://www.lri.fr/~hansen/cmaes.m">Matlab code</a></li>
	///  <li><a href="http://www.lri.fr/~hansen/cmaesintro.html">Introduction to CMA-ES</a></li>
	///  <li><a href="http://en.wikipedia.org/wiki/CMA-ES">Wikipedia</a></li>
	/// </ul>
	/// 
	/// @version $Id: CMAESOptimizer.java 1540165 2013-11-08 19:53:58Z tn $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class CMAESOptimizer : BaseAbstractMultivariateSimpleBoundsOptimizer<MultivariateFunction>, MultivariateOptimizer
	{
		/// <summary>
		/// Default value for <seealso cref="#checkFeasableCount"/>: {@value}. </summary>
		public const int DEFAULT_CHECKFEASABLECOUNT = 0;
		/// <summary>
		/// Default value for <seealso cref="#stopFitness"/>: {@value}. </summary>
		public const double DEFAULT_STOPFITNESS = 0;
		/// <summary>
		/// Default value for <seealso cref="#isActiveCMA"/>: {@value}. </summary>
		public const bool DEFAULT_ISACTIVECMA = true;
		/// <summary>
		/// Default value for <seealso cref="#maxIterations"/>: {@value}. </summary>
		public const int DEFAULT_MAXITERATIONS = 30000;
		/// <summary>
		/// Default value for <seealso cref="#diagonalOnly"/>: {@value}. </summary>
		public const int DEFAULT_DIAGONALONLY = 0;
		/// <summary>
		/// Default value for <seealso cref="#random"/>. </summary>
		public static readonly RandomGenerator DEFAULT_RANDOMGENERATOR = new MersenneTwister();

		// global search parameters
		/// <summary>
		/// Population size, offspring number. The primary strategy parameter to play
		/// with, which can be increased from its default value. Increasing the
		/// population size improves global search properties in exchange to speed.
		/// Speed decreases, as a rule, at most linearly with increasing population
		/// size. It is advisable to begin with the default small population size.
		/// </summary>
		private int lambda; // population size
		/// <summary>
		/// Covariance update mechanism, default is active CMA. isActiveCMA = true
		/// turns on "active CMA" with a negative update of the covariance matrix and
		/// checks for positive definiteness. OPTS.CMA.active = 2 does not check for
		/// pos. def. and is numerically faster. Active CMA usually speeds up the
		/// adaptation.
		/// </summary>
		private bool isActiveCMA;
		/// <summary>
		/// Determines how often a new random offspring is generated in case it is
		/// not feasible / beyond the defined limits, default is 0.
		/// </summary>
		private int checkFeasableCount;
		/// <seealso cref= Sigma </seealso>
		private double[] inputSigma;
		/// <summary>
		/// Number of objective variables/problem dimension </summary>
		private int dimension;
		/// <summary>
		/// Defines the number of initial iterations, where the covariance matrix
		/// remains diagonal and the algorithm has internally linear time complexity.
		/// diagonalOnly = 1 means keeping the covariance matrix always diagonal and
		/// this setting also exhibits linear space complexity. This can be
		/// particularly useful for dimension > 100. </summary>
		/// <seealso cref= <a href="http://hal.archives-ouvertes.fr/inria-00287367/en">A Simple Modification in CMA-ES</a> </seealso>
		private int diagonalOnly = 0;
		/// <summary>
		/// Number of objective variables/problem dimension </summary>
		private bool isMinimize = true;
		/// <summary>
		/// Indicates whether statistic data is collected. </summary>
		private bool generateStatistics = false;

		// termination criteria
		/// <summary>
		/// Maximal number of iterations allowed. </summary>
		private int maxIterations;
		/// <summary>
		/// Limit for fitness value. </summary>
		private double stopFitness;
		/// <summary>
		/// Stop if x-changes larger stopTolUpX. </summary>
		private double stopTolUpX;
		/// <summary>
		/// Stop if x-change smaller stopTolX. </summary>
		private double stopTolX;
		/// <summary>
		/// Stop if fun-changes smaller stopTolFun. </summary>
		private double stopTolFun;
		/// <summary>
		/// Stop if back fun-changes smaller stopTolHistFun. </summary>
		private double stopTolHistFun;

		// selection strategy parameters
		/// <summary>
		/// Number of parents/points for recombination. </summary>
		private int mu;
		/// <summary>
		/// log(mu + 0.5), stored for efficiency. </summary>
		private double logMu2;
		/// <summary>
		/// Array for weighted recombination. </summary>
		private RealMatrix weights;
		/// <summary>
		/// Variance-effectiveness of sum w_i x_i. </summary>
		private double mueff;

		// dynamic strategy parameters and constants
		/// <summary>
		/// Overall standard deviation - search volume. </summary>
		private double sigma;
		/// <summary>
		/// Cumulation constant. </summary>
		private double cc;
		/// <summary>
		/// Cumulation constant for step-size. </summary>
		private double cs;
		/// <summary>
		/// Damping for step-size. </summary>
		private double damps;
		/// <summary>
		/// Learning rate for rank-one update. </summary>
		private double ccov1;
		/// <summary>
		/// Learning rate for rank-mu update' </summary>
		private double ccovmu;
		/// <summary>
		/// Expectation of ||N(0,I)|| == norm(randn(N,1)). </summary>
		private double chiN;
		/// <summary>
		/// Learning rate for rank-one update - diagonalOnly </summary>
		private double ccov1Sep;
		/// <summary>
		/// Learning rate for rank-mu update - diagonalOnly </summary>
		private double ccovmuSep;

		// CMA internal values - updated each generation
		/// <summary>
		/// Objective variables. </summary>
		private RealMatrix xmean;
		/// <summary>
		/// Evolution path. </summary>
		private RealMatrix pc;
		/// <summary>
		/// Evolution path for sigma. </summary>
		private RealMatrix ps;
		/// <summary>
		/// Norm of ps, stored for efficiency. </summary>
		private double normps;
		/// <summary>
		/// Coordinate system. </summary>
		private RealMatrix B;
		/// <summary>
		/// Scaling. </summary>
		private RealMatrix D;
		/// <summary>
		/// B*D, stored for efficiency. </summary>
		private RealMatrix BD;
		/// <summary>
		/// Diagonal of sqrt(D), stored for efficiency. </summary>
		private RealMatrix diagD;
		/// <summary>
		/// Covariance matrix. </summary>
		private RealMatrix C;
		/// <summary>
		/// Diagonal of C, used for diagonalOnly. </summary>
		private RealMatrix diagC;
		/// <summary>
		/// Number of iterations already performed. </summary>
		private int iterations;

		/// <summary>
		/// History queue of best values. </summary>
		private double[] fitnessHistory;
		/// <summary>
		/// Size of history queue of best values. </summary>
		private int historySize;

		/// <summary>
		/// Random generator. </summary>
		private RandomGenerator random;

		/// <summary>
		/// History of sigma values. </summary>
		private IList<double?> statisticsSigmaHistory = new List<double?>();
		/// <summary>
		/// History of mean matrix. </summary>
		private IList<RealMatrix> statisticsMeanHistory = new List<RealMatrix>();
		/// <summary>
		/// History of fitness values. </summary>
		private IList<double?> statisticsFitnessHistory = new List<double?>();
		/// <summary>
		/// History of D matrix. </summary>
		private IList<RealMatrix> statisticsDHistory = new List<RealMatrix>();

		/// <summary>
		/// Default constructor, uses default parameters
		/// </summary>
		/// @deprecated As of version 3.1: Parameter {@code lambda} must be
		/// passed with the call to {@link #optimize(int,MultivariateFunction,GoalType,OptimizationData[])
		/// optimize} (whereas in the current code it is set to an undocumented value). 
		[Obsolete("As of version 3.1: Parameter {@code lambda} must be")]
		public CMAESOptimizer() : this(0)
		{
		}

		/// <param name="lambda"> Population size. </param>
		/// @deprecated As of version 3.1: Parameter {@code lambda} must be
		/// passed with the call to {@link #optimize(int,MultivariateFunction,GoalType,OptimizationData[])
		/// optimize} (whereas in the current code it is set to an undocumented value).. 
		[Obsolete("As of version 3.1: Parameter {@code lambda} must be")]
		public CMAESOptimizer(int lambda) : this(lambda, null, DEFAULT_MAXITERATIONS, DEFAULT_STOPFITNESS, DEFAULT_ISACTIVECMA, DEFAULT_DIAGONALONLY, DEFAULT_CHECKFEASABLECOUNT, DEFAULT_RANDOMGENERATOR, false, null)
		{
		}

		/// <param name="lambda"> Population size. </param>
		/// <param name="inputSigma"> Initial standard deviations to sample new points
		/// around the initial guess. </param>
		/// @deprecated As of version 3.1: Parameters {@code lambda} and {@code inputSigma} must be
		/// passed with the call to {@link #optimize(int,MultivariateFunction,GoalType,OptimizationData[])
		/// optimize}. 
		[Obsolete("As of version 3.1: Parameters {@code lambda} and {@code inputSigma} must be")]
		public CMAESOptimizer(int lambda, double[] inputSigma) : this(lambda, inputSigma, DEFAULT_MAXITERATIONS, DEFAULT_STOPFITNESS, DEFAULT_ISACTIVECMA, DEFAULT_DIAGONALONLY, DEFAULT_CHECKFEASABLECOUNT, DEFAULT_RANDOMGENERATOR, false)
		{
		}

		/// <param name="lambda"> Population size. </param>
		/// <param name="inputSigma"> Initial standard deviations to sample new points
		/// around the initial guess. </param>
		/// <param name="maxIterations"> Maximal number of iterations. </param>
		/// <param name="stopFitness"> Whether to stop if objective function value is smaller than
		/// {@code stopFitness}. </param>
		/// <param name="isActiveCMA"> Chooses the covariance matrix update method. </param>
		/// <param name="diagonalOnly"> Number of initial iterations, where the covariance matrix
		/// remains diagonal. </param>
		/// <param name="checkFeasableCount"> Determines how often new random objective variables are
		/// generated in case they are out of bounds. </param>
		/// <param name="random"> Random generator. </param>
		/// <param name="generateStatistics"> Whether statistic data is collected. </param>
		/// @deprecated See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/> 
		[Obsolete("See <seealso cref="SimpleValueChecker#SimpleValueChecker()"/>")]
		public CMAESOptimizer(int lambda, double[] inputSigma, int maxIterations, double stopFitness, bool isActiveCMA, int diagonalOnly, int checkFeasableCount, RandomGenerator random, bool generateStatistics) : this(lambda, inputSigma, maxIterations, stopFitness, isActiveCMA, diagonalOnly, checkFeasableCount, random, generateStatistics, new SimpleValueChecker())
		{
		}

		/// <param name="lambda"> Population size. </param>
		/// <param name="inputSigma"> Initial standard deviations to sample new points
		/// around the initial guess. </param>
		/// <param name="maxIterations"> Maximal number of iterations. </param>
		/// <param name="stopFitness"> Whether to stop if objective function value is smaller than
		/// {@code stopFitness}. </param>
		/// <param name="isActiveCMA"> Chooses the covariance matrix update method. </param>
		/// <param name="diagonalOnly"> Number of initial iterations, where the covariance matrix
		/// remains diagonal. </param>
		/// <param name="checkFeasableCount"> Determines how often new random objective variables are
		/// generated in case they are out of bounds. </param>
		/// <param name="random"> Random generator. </param>
		/// <param name="generateStatistics"> Whether statistic data is collected. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// @deprecated As of version 3.1: Parameters {@code lambda} and {@code inputSigma} must be
		/// passed with the call to {@link #optimize(int,MultivariateFunction,GoalType,OptimizationData[])
		/// optimize}. 
		[Obsolete("As of version 3.1: Parameters {@code lambda} and {@code inputSigma} must be")]
		public CMAESOptimizer(int lambda, double[] inputSigma, int maxIterations, double stopFitness, bool isActiveCMA, int diagonalOnly, int checkFeasableCount, RandomGenerator random, bool generateStatistics, ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
			this.lambda = lambda;
			this.inputSigma = inputSigma == null ? null : (double[]) inputSigma.clone();
			this.maxIterations = maxIterations;
			this.stopFitness = stopFitness;
			this.isActiveCMA = isActiveCMA;
			this.diagonalOnly = diagonalOnly;
			this.checkFeasableCount = checkFeasableCount;
			this.random = random;
			this.generateStatistics = generateStatistics;
		}

		/// <param name="maxIterations"> Maximal number of iterations. </param>
		/// <param name="stopFitness"> Whether to stop if objective function value is smaller than
		/// {@code stopFitness}. </param>
		/// <param name="isActiveCMA"> Chooses the covariance matrix update method. </param>
		/// <param name="diagonalOnly"> Number of initial iterations, where the covariance matrix
		/// remains diagonal. </param>
		/// <param name="checkFeasableCount"> Determines how often new random objective variables are
		/// generated in case they are out of bounds. </param>
		/// <param name="random"> Random generator. </param>
		/// <param name="generateStatistics"> Whether statistic data is collected. </param>
		/// <param name="checker"> Convergence checker.
		/// 
		/// @since 3.1 </param>
		public CMAESOptimizer(int maxIterations, double stopFitness, bool isActiveCMA, int diagonalOnly, int checkFeasableCount, RandomGenerator random, bool generateStatistics, ConvergenceChecker<PointValuePair> checker) : base(checker)
		{
			this.maxIterations = maxIterations;
			this.stopFitness = stopFitness;
			this.isActiveCMA = isActiveCMA;
			this.diagonalOnly = diagonalOnly;
			this.checkFeasableCount = checkFeasableCount;
			this.random = random;
			this.generateStatistics = generateStatistics;
		}

		/// <returns> History of sigma values. </returns>
		public virtual IList<double?> StatisticsSigmaHistory
		{
			get
			{
				return statisticsSigmaHistory;
			}
		}

		/// <returns> History of mean matrix. </returns>
		public virtual IList<RealMatrix> StatisticsMeanHistory
		{
			get
			{
				return statisticsMeanHistory;
			}
		}

		/// <returns> History of fitness values. </returns>
		public virtual IList<double?> StatisticsFitnessHistory
		{
			get
			{
				return statisticsFitnessHistory;
			}
		}

		/// <returns> History of D matrix. </returns>
		public virtual IList<RealMatrix> StatisticsDHistory
		{
			get
			{
				return statisticsDHistory;
			}
		}

		/// <summary>
		/// Input sigma values.
		/// They define the initial coordinate-wise standard deviations for
		/// sampling new search points around the initial guess.
		/// It is suggested to set them to the estimated distance from the
		/// initial to the desired optimum.
		/// Small values induce the search to be more local (and very small
		/// values are more likely to find a local optimum close to the initial
		/// guess).
		/// Too small values might however lead to early termination.
		/// @since 3.1
		/// </summary>
		public class Sigma : OptimizationData
		{
			/// <summary>
			/// Sigma values. </summary>
			internal readonly double[] sigma;

			/// <param name="s"> Sigma values. </param>
			/// <exception cref="NotPositiveException"> if any of the array entries is smaller
			/// than zero. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Sigma(double[] s) throws mathlib.exception.NotPositiveException
			public Sigma(double[] s)
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (s[i] < 0)
					{
						throw new NotPositiveException(s[i]);
					}
				}

				sigma = s.clone();
			}

			/// <returns> the sigma values. </returns>
			public virtual double[] Sigma
			{
				get
				{
					return sigma.clone();
				}
			}
		}

		/// <summary>
		/// Population size.
		/// The number of offspring is the primary strategy parameter.
		/// In the absence of better clues, a good default could be an
		/// integer close to {@code 4 + 3 ln(n)}, where {@code n} is the
		/// number of optimized parameters.
		/// Increasing the population size improves global search properties
		/// at the expense of speed (which in general decreases at most
		/// linearly with increasing population size).
		/// @since 3.1
		/// </summary>
		public class PopulationSize : OptimizationData
		{
			/// <summary>
			/// Population size. </summary>
			internal readonly int lambda;

			/// <param name="size"> Population size. </param>
			/// <exception cref="NotStrictlyPositiveException"> if {@code size <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PopulationSize(int size) throws mathlib.exception.NotStrictlyPositiveException
			public PopulationSize(int size)
			{
				if (size <= 0)
				{
					throw new NotStrictlyPositiveException(size);
				}
				lambda = size;
			}

			/// <returns> the population size. </returns>
			public virtual int PopulationSize
			{
				get
				{
					return lambda;
				}
			}
		}

		/// <summary>
		/// Optimize an objective function.
		/// </summary>
		/// <param name="maxEval"> Allowed number of evaluations of the objective function. </param>
		/// <param name="f"> Objective function. </param>
		/// <param name="goalType"> Optimization type. </param>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="mathlib.optimization.InitialGuess InitialGuess"/></li>
		///  <li><seealso cref="Sigma"/></li>
		///  <li><seealso cref="PopulationSize"/></li>
		/// </ul> </param>
		/// <returns> the point/value pair giving the optimal value for objective
		/// function. </returns>
		protected internal override PointValuePair optimizeInternal(int maxEval, MultivariateFunction f, GoalType goalType, params OptimizationData[] optData)
		{
			// Scan "optData" for the input specific to this optimizer.
			parseOptimizationData(optData);

			// The parent's method will retrieve the common parameters from
			// "optData" and call "doOptimize".
			return base.optimizeInternal(maxEval, f, goalType, optData);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PointValuePair doOptimize()
		{
			checkParameters();
			 // -------------------- Initialization --------------------------------
			isMinimize = GoalType.Equals(GoalType.MINIMIZE);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FitnessFunction fitfun = new FitnessFunction();
			FitnessFunction fitfun = new FitnessFunction(this);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] guess = getStartPoint();
			double[] guess = StartPoint;
			// number of objective variables/problem dimension
			dimension = guess.Length;
			initializeCMA(guess);
			iterations = 0;
			double bestValue = fitfun.value(guess);
			push(fitnessHistory, bestValue);
			PointValuePair optimum = new PointValuePair(StartPoint, isMinimize ? bestValue : -bestValue);
			PointValuePair lastResult = null;

			// -------------------- Generation Loop --------------------------------

			for (iterations = 1; iterations <= maxIterations; iterations++)
			{
				// Generate and evaluate lambda offspring
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix arz = randn1(dimension, lambda);
				RealMatrix arz = randn1(dimension, lambda);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix arx = zeros(dimension, lambda);
				RealMatrix arx = zeros(dimension, lambda);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] fitness = new double[lambda];
				double[] fitness = new double[lambda];
				// generate random offspring
				for (int k = 0; k < lambda; k++)
				{
					RealMatrix arxk = null;
					for (int i = 0; i < checkFeasableCount + 1; i++)
					{
						if (diagonalOnly <= 0)
						{
							arxk = xmean.add(BD.multiply(arz.getColumnMatrix(k)).scalarMultiply(sigma)); // m + sig * Normal(0,C)
						}
						else
						{
							arxk = xmean.add(times(diagD,arz.getColumnMatrix(k)).scalarMultiply(sigma));
						}
						if (i >= checkFeasableCount || fitfun.isFeasible(arxk.getColumn(0)))
						{
							break;
						}
						// regenerate random arguments for row
						arz.setColumn(k, randn(dimension));
					}
					copyColumn(arxk, 0, arx, k);
					try
					{
						fitness[k] = fitfun.value(arx.getColumn(k)); // compute fitness
					}
					catch (TooManyEvaluationsException e)
					{
						goto generationLoopBreak;
					}
				}
				// Sort by fitness and compute weighted mean into xmean
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] arindex = sortedIndices(fitness);
				int[] arindex = sortedIndices(fitness);
				// Calculate new xmean, this is selection and recombination
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix xold = xmean;
				RealMatrix xold = xmean; // for speed up of Eq. (2) and (3)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix bestArx = selectColumns(arx, mathlib.util.MathArrays.copyOf(arindex, mu));
				RealMatrix bestArx = selectColumns(arx, MathArrays.copyOf(arindex, mu));
				xmean = bestArx.multiply(weights);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix bestArz = selectColumns(arz, mathlib.util.MathArrays.copyOf(arindex, mu));
				RealMatrix bestArz = selectColumns(arz, MathArrays.copyOf(arindex, mu));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix zmean = bestArz.multiply(weights);
				RealMatrix zmean = bestArz.multiply(weights);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hsig = updateEvolutionPaths(zmean, xold);
				bool hsig = updateEvolutionPaths(zmean, xold);
				if (diagonalOnly <= 0)
				{
					updateCovariance(hsig, bestArx, arz, arindex, xold);
				}
				else
				{
					updateCovarianceDiagonalOnly(hsig, bestArz);
				}
				// Adapt step size sigma - Eq. (5)
				sigma *= FastMath.exp(FastMath.min(1, (normps / chiN - 1) * cs / damps));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bestFitness = fitness[arindex[0]];
				double bestFitness = fitness[arindex[0]];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double worstFitness = fitness[arindex[arindex.length - 1]];
				double worstFitness = fitness[arindex[arindex.Length - 1]];
				if (bestValue > bestFitness)
				{
					bestValue = bestFitness;
					lastResult = optimum;
					optimum = new PointValuePair(fitfun.repair(bestArx.getColumn(0)), isMinimize ? bestFitness : -bestFitness);
					if (ConvergenceChecker != null && lastResult != null && ConvergenceChecker.converged(iterations, optimum, lastResult))
					{
						goto generationLoopBreak;
					}
				}
				// handle termination criteria
				// Break, if fitness is good enough
				if (stopFitness != 0 && bestFitness < (isMinimize ? stopFitness : -stopFitness))
				{
					goto generationLoopBreak;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] sqrtDiagC = sqrt(diagC).getColumn(0);
				double[] sqrtDiagC = sqrt(diagC).getColumn(0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pcCol = pc.getColumn(0);
				double[] pcCol = pc.getColumn(0);
				for (int i = 0; i < dimension; i++)
				{
					if (sigma * FastMath.max(FastMath.abs(pcCol[i]), sqrtDiagC[i]) > stopTolX)
					{
						break;
					}
					if (i >= dimension - 1)
					{
						goto generationLoopBreak;
					}
				}
				for (int i = 0; i < dimension; i++)
				{
					if (sigma * sqrtDiagC[i] > stopTolUpX)
					{
						goto generationLoopBreak;
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double historyBest = min(fitnessHistory);
				double historyBest = min(fitnessHistory);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double historyWorst = max(fitnessHistory);
				double historyWorst = max(fitnessHistory);
				if (iterations > 2 && FastMath.max(historyWorst, worstFitness) - FastMath.min(historyBest, bestFitness) < stopTolFun)
				{
					goto generationLoopBreak;
				}
				if (iterations > fitnessHistory.Length && historyWorst - historyBest < stopTolHistFun)
				{
					goto generationLoopBreak;
				}
				// condition number of the covariance matrix exceeds 1e14
				if (max(diagD) / min(diagD) > 1e7)
				{
					goto generationLoopBreak;
				}
				// user defined termination
				if (ConvergenceChecker != null)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.optimization.PointValuePair current = new mathlib.optimization.PointValuePair(bestArx.getColumn(0), isMinimize ? bestFitness : -bestFitness);
					PointValuePair current = new PointValuePair(bestArx.getColumn(0), isMinimize ? bestFitness : -bestFitness);
					if (lastResult != null && ConvergenceChecker.converged(iterations, current, lastResult))
					{
						goto generationLoopBreak;
					}
					lastResult = current;
				}
				// Adjust step size in case of equal function values (flat fitness)
				if (bestValue == fitness[arindex[(int)(0.1 + lambda / 4.0)]])
				{
					sigma *= FastMath.exp(0.2 + cs / damps);
				}
				if (iterations > 2 && FastMath.max(historyWorst, bestFitness) - FastMath.min(historyBest, bestFitness) == 0)
				{
					sigma *= FastMath.exp(0.2 + cs / damps);
				}
				// store best in history
				push(fitnessHistory,bestFitness);
				fitfun.ValueRange = worstFitness - bestFitness;
				if (generateStatistics)
				{
					statisticsSigmaHistory.Add(sigma);
					statisticsFitnessHistory.Add(bestFitness);
					statisticsMeanHistory.Add(xmean.transpose());
					statisticsDHistory.Add(diagD.transpose().scalarMultiply(1E5));
				}
				generationLoopContinue:;
			}
			generationLoopBreak:
			return optimum;
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="Sigma"/></li>
		///  <li><seealso cref="PopulationSize"/></li>
		/// </ul> </param>
		private void parseOptimizationData(params OptimizationData[] optData)
		{
			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is Sigma)
				{
					inputSigma = ((Sigma) data).Sigma;
					continue;
				}
				if (data is PopulationSize)
				{
					lambda = ((PopulationSize) data).PopulationSize;
					continue;
				}
			}
		}

		/// <summary>
		/// Checks dimensions and values of boundaries and inputSigma if defined.
		/// </summary>
		private void checkParameters()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] init = getStartPoint();
			double[] init = StartPoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lB = getLowerBound();
			double[] lB = LowerBound;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] uB = getUpperBound();
			double[] uB = UpperBound;

			if (inputSigma != null)
			{
				if (inputSigma.Length != init.Length)
				{
					throw new DimensionMismatchException(inputSigma.Length, init.Length);
				}
				for (int i = 0; i < init.Length; i++)
				{
					if (inputSigma[i] < 0)
					{
						// XXX Remove this block in 4.0 (check performed in "Sigma" class).
						throw new NotPositiveException(inputSigma[i]);
					}
					if (inputSigma[i] > uB[i] - lB[i])
					{
						throw new OutOfRangeException(inputSigma[i], 0, uB[i] - lB[i]);
					}
				}
			}
		}

		/// <summary>
		/// Initialization of the dynamic search parameters
		/// </summary>
		/// <param name="guess"> Initial guess for the arguments of the fitness function. </param>
		private void initializeCMA(double[] guess)
		{
			if (lambda <= 0)
			{
				// XXX Line below to replace the current one in 4.0 (MATH-879).
				// throw new NotStrictlyPositiveException(lambda);
				lambda = 4 + (int)(3 * FastMath.log(dimension));
			}
			// initialize sigma
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] sigmaArray = new double[guess.length][1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] sigmaArray = new double[guess.Length][1];
			double[][] sigmaArray = RectangularArrays.ReturnRectangularDoubleArray(guess.Length, 1);
			for (int i = 0; i < guess.Length; i++)
			{
				// XXX Line below to replace the current one in 4.0 (MATH-868).
				// sigmaArray[i][0] = inputSigma[i];
				sigmaArray[i][0] = inputSigma == null ? 0.3 : inputSigma[i];
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix insigma = new mathlib.linear.Array2DRowRealMatrix(sigmaArray, false);
			RealMatrix insigma = new Array2DRowRealMatrix(sigmaArray, false);
			sigma = max(insigma); // overall standard deviation

			// initialize termination criteria
			stopTolUpX = 1e3 * max(insigma);
			stopTolX = 1e-11 * max(insigma);
			stopTolFun = 1e-12;
			stopTolHistFun = 1e-13;

			// initialize selection strategy parameters
			mu = lambda / 2; // number of parents/points for recombination
			logMu2 = FastMath.log(mu + 0.5);
			weights = log(sequence(1, mu, 1)).scalarMultiply(-1).scalarAdd(logMu2);
			double sumw = 0;
			double sumwq = 0;
			for (int i = 0; i < mu; i++)
			{
				double w = weights.getEntry(i, 0);
				sumw += w;
				sumwq += w * w;
			}
			weights = weights.scalarMultiply(1 / sumw);
			mueff = sumw * sumw / sumwq; // variance-effectiveness of sum w_i x_i

			// initialize dynamic strategy parameters and constants
			cc = (4 + mueff / dimension) / (dimension + 4 + 2 * mueff / dimension);
			cs = (mueff + 2) / (dimension + mueff + 3.0);
			damps = (1 + 2 * FastMath.max(0, FastMath.sqrt((mueff - 1) / (dimension + 1)) - 1)) * FastMath.max(0.3, 1 - dimension / (1e-6 + maxIterations)) + cs; // minor increment
			ccov1 = 2 / ((dimension + 1.3) * (dimension + 1.3) + mueff);
			ccovmu = FastMath.min(1 - ccov1, 2 * (mueff - 2 + 1 / mueff) / ((dimension + 2) * (dimension + 2) + mueff));
			ccov1Sep = FastMath.min(1, ccov1 * (dimension + 1.5) / 3);
			ccovmuSep = FastMath.min(1 - ccov1, ccovmu * (dimension + 1.5) / 3);
			chiN = FastMath.sqrt(dimension) * (1 - 1 / ((double) 4 * dimension) + 1 / ((double) 21 * dimension * dimension));
			// intialize CMA internal values - updated each generation
			xmean = MatrixUtils.createColumnRealMatrix(guess); // objective variables
			diagD = insigma.scalarMultiply(1 / sigma);
			diagC = square(diagD);
			pc = zeros(dimension, 1); // evolution paths for C and sigma
			ps = zeros(dimension, 1); // B defines the coordinate system
			normps = ps.FrobeniusNorm;

			B = eye(dimension, dimension);
			D = ones(dimension, 1); // diagonal D defines the scaling
			BD = times(B, repmat(diagD.transpose(), dimension, 1));
			C = B.multiply(diag(square(D)).multiply(B.transpose())); // covariance
			historySize = 10 + (int)(3 * 10 * dimension / (double) lambda);
			fitnessHistory = new double[historySize]; // history of fitness values
			for (int i = 0; i < historySize; i++)
			{
				fitnessHistory[i] = double.MaxValue;
			}
		}

		/// <summary>
		/// Update of the evolution paths ps and pc.
		/// </summary>
		/// <param name="zmean"> Weighted row matrix of the gaussian random numbers generating
		/// the current offspring. </param>
		/// <param name="xold"> xmean matrix of the previous generation. </param>
		/// <returns> hsig flag indicating a small correction. </returns>
		private bool updateEvolutionPaths(RealMatrix zmean, RealMatrix xold)
		{
			ps = ps.scalarMultiply(1 - cs).add(B.multiply(zmean).scalarMultiply(FastMath.sqrt(cs * (2 - cs) * mueff)));
			normps = ps.FrobeniusNorm;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hsig = normps / mathlib.util.FastMath.sqrt(1 - mathlib.util.FastMath.pow(1 - cs, 2 * iterations)) / chiN < 1.4 + 2 / ((double) dimension + 1);
			bool hsig = normps / FastMath.sqrt(1 - FastMath.pow(1 - cs, 2 * iterations)) / chiN < 1.4 + 2 / ((double) dimension + 1);
			pc = pc.scalarMultiply(1 - cc);
			if (hsig)
			{
				pc = pc.add(xmean.subtract(xold).scalarMultiply(FastMath.sqrt(cc * (2 - cc) * mueff) / sigma));
			}
			return hsig;
		}

		/// <summary>
		/// Update of the covariance matrix C for diagonalOnly > 0
		/// </summary>
		/// <param name="hsig"> Flag indicating a small correction. </param>
		/// <param name="bestArz"> Fitness-sorted matrix of the gaussian random values of the
		/// current offspring. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void updateCovarianceDiagonalOnly(boolean hsig, final mathlib.linear.RealMatrix bestArz)
		private void updateCovarianceDiagonalOnly(bool hsig, RealMatrix bestArz)
		{
			// minor correction if hsig==false
			double oldFac = hsig ? 0 : ccov1Sep * cc * (2 - cc);
			oldFac += 1 - ccov1Sep - ccovmuSep;
			diagC = diagC.scalarMultiply(oldFac).add(square(pc).scalarMultiply(ccov1Sep)).add((times(diagC, square(bestArz).multiply(weights))).scalarMultiply(ccovmuSep)); // plus rank mu update -  plus rank one update -  regard old matrix
			diagD = sqrt(diagC); // replaces eig(C)
			if (diagonalOnly > 1 && iterations > diagonalOnly)
			{
				// full covariance matrix from now on
				diagonalOnly = 0;
				B = eye(dimension, dimension);
				BD = diag(diagD);
				C = diag(diagC);
			}
		}

		/// <summary>
		/// Update of the covariance matrix C.
		/// </summary>
		/// <param name="hsig"> Flag indicating a small correction. </param>
		/// <param name="bestArx"> Fitness-sorted matrix of the argument vectors producing the
		/// current offspring. </param>
		/// <param name="arz"> Unsorted matrix containing the gaussian random values of the
		/// current offspring. </param>
		/// <param name="arindex"> Indices indicating the fitness-order of the current offspring. </param>
		/// <param name="xold"> xmean matrix of the previous generation. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void updateCovariance(boolean hsig, final mathlib.linear.RealMatrix bestArx, final mathlib.linear.RealMatrix arz, final int[] arindex, final mathlib.linear.RealMatrix xold)
		private void updateCovariance(bool hsig, RealMatrix bestArx, RealMatrix arz, int[] arindex, RealMatrix xold)
		{
			double negccov = 0;
			if (ccov1 + ccovmu > 0)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix arpos = bestArx.subtract(repmat(xold, 1, mu)).scalarMultiply(1 / sigma);
				RealMatrix arpos = bestArx.subtract(repmat(xold, 1, mu)).scalarMultiply(1 / sigma); // mu difference vectors
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix roneu = pc.multiply(pc.transpose()).scalarMultiply(ccov1);
				RealMatrix roneu = pc.multiply(pc.transpose()).scalarMultiply(ccov1); // rank one update
				// minor correction if hsig==false
				double oldFac = hsig ? 0 : ccov1 * cc * (2 - cc);
				oldFac += 1 - ccov1 - ccovmu;
				if (isActiveCMA)
				{
					// Adapt covariance matrix C active CMA
					negccov = (1 - ccovmu) * 0.25 * mueff / (FastMath.pow(dimension + 2, 1.5) + 2 * mueff);
					// keep at least 0.66 in all directions, small popsize are most
					// critical
					const double negminresidualvariance = 0.66;
					// where to make up for the variance loss
					const double negalphaold = 0.5;
					// prepare vectors, compute negative updating matrix Cneg
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] arReverseIndex = reverse(arindex);
					int[] arReverseIndex = reverse(arindex);
					RealMatrix arzneg = selectColumns(arz, MathArrays.copyOf(arReverseIndex, mu));
					RealMatrix arnorms = sqrt(sumRows(square(arzneg)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] idxnorms = sortedIndices(arnorms.getRow(0));
					int[] idxnorms = sortedIndices(arnorms.getRow(0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix arnormsSorted = selectColumns(arnorms, idxnorms);
					RealMatrix arnormsSorted = selectColumns(arnorms, idxnorms);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] idxReverse = reverse(idxnorms);
					int[] idxReverse = reverse(idxnorms);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix arnormsReverse = selectColumns(arnorms, idxReverse);
					RealMatrix arnormsReverse = selectColumns(arnorms, idxReverse);
					arnorms = divide(arnormsReverse, arnormsSorted);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] idxInv = inverse(idxnorms);
					int[] idxInv = inverse(idxnorms);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix arnormsInv = selectColumns(arnorms, idxInv);
					RealMatrix arnormsInv = selectColumns(arnorms, idxInv);
					// check and set learning rate negccov
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double negcovMax = (1 - negminresidualvariance) / square(arnormsInv).multiply(weights).getEntry(0, 0);
					double negcovMax = (1 - negminresidualvariance) / square(arnormsInv).multiply(weights).getEntry(0, 0);
					if (negccov > negcovMax)
					{
						negccov = negcovMax;
					}
					arzneg = times(arzneg, repmat(arnormsInv, dimension, 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix artmp = BD.multiply(arzneg);
					RealMatrix artmp = BD.multiply(arzneg);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix Cneg = artmp.multiply(diag(weights)).multiply(artmp.transpose());
					RealMatrix Cneg = artmp.multiply(diag(weights)).multiply(artmp.transpose());
					oldFac += negalphaold * negccov;
					C = C.scalarMultiply(oldFac).add(roneu).add(arpos.scalarMultiply(ccovmu + (1 - negalphaold) * negccov).multiply(times(repmat(weights, 1, dimension), arpos.transpose()))).subtract(Cneg.scalarMultiply(negccov)); // plus rank mu update -  plus rank one update -  regard old matrix
				}
				else
				{
					// Adapt covariance matrix C - nonactive
					C = C.scalarMultiply(oldFac).add(roneu).add(arpos.scalarMultiply(ccovmu).multiply(times(repmat(weights, 1, dimension), arpos.transpose()))); // plus rank mu update -  plus rank one update -  regard old matrix
				}
			}
			updateBD(negccov);
		}

		/// <summary>
		/// Update B and D from C.
		/// </summary>
		/// <param name="negccov"> Negative covariance factor. </param>
		private void updateBD(double negccov)
		{
			if (ccov1 + ccovmu + negccov > 0 && (iterations % 1.0 / (ccov1 + ccovmu + negccov) / dimension / 10.0) < 1)
			{
				// to achieve O(N^2)
				C = triu(C, 0).add(triu(C, 1).transpose());
				// enforce symmetry to prevent complex numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.EigenDecomposition eig = new mathlib.linear.EigenDecomposition(C);
				EigenDecomposition eig = new EigenDecomposition(C);
				B = eig.V; // eigen decomposition, B==normalized eigenvectors
				D = eig.D;
				diagD = diag(D);
				if (min(diagD) <= 0)
				{
					for (int i = 0; i < dimension; i++)
					{
						if (diagD.getEntry(i, 0) < 0)
						{
							diagD.setEntry(i, 0, 0);
						}
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tfac = max(diagD) / 1e14;
					double tfac = max(diagD) / 1e14;
					C = C.add(eye(dimension, dimension).scalarMultiply(tfac));
					diagD = diagD.add(ones(dimension, 1).scalarMultiply(tfac));
				}
				if (max(diagD) > 1e14 * min(diagD))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tfac = max(diagD) / 1e14 - min(diagD);
					double tfac = max(diagD) / 1e14 - min(diagD);
					C = C.add(eye(dimension, dimension).scalarMultiply(tfac));
					diagD = diagD.add(ones(dimension, 1).scalarMultiply(tfac));
				}
				diagC = diag(C);
				diagD = sqrt(diagD); // D contains standard deviations now
				BD = times(B, repmat(diagD.transpose(), dimension, 1)); // O(n^2)
			}
		}

		/// <summary>
		/// Pushes the current best fitness value in a history queue.
		/// </summary>
		/// <param name="vals"> History queue. </param>
		/// <param name="val"> Current best fitness value. </param>
		private static void push(double[] vals, double val)
		{
			for (int i = vals.Length - 1; i > 0; i--)
			{
				vals[i] = vals[i - 1];
			}
			vals[0] = val;
		}

		/// <summary>
		/// Sorts fitness values.
		/// </summary>
		/// <param name="doubles"> Array of values to be sorted. </param>
		/// <returns> a sorted array of indices pointing into doubles. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int[] sortedIndices(final double[] doubles)
		private int[] sortedIndices(double[] doubles)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DoubleIndex[] dis = new DoubleIndex[doubles.length];
			DoubleIndex[] dis = new DoubleIndex[doubles.Length];
			for (int i = 0; i < doubles.Length; i++)
			{
				dis[i] = new DoubleIndex(doubles[i], i);
			}
			Arrays.sort(dis);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] indices = new int[doubles.length];
			int[] indices = new int[doubles.Length];
			for (int i = 0; i < doubles.Length; i++)
			{
				indices[i] = dis[i].index;
			}
			return indices;
		}

		/// <summary>
		/// Used to sort fitness values. Sorting is always in lower value first
		/// order.
		/// </summary>
		private class DoubleIndex : IComparable<DoubleIndex>
		{
			/// <summary>
			/// Value to compare. </summary>
			internal readonly double value;
			/// <summary>
			/// Index into sorted array. </summary>
			internal readonly int index;

			/// <param name="value"> Value to compare. </param>
			/// <param name="index"> Index into sorted array. </param>
			internal DoubleIndex(double value, int index)
			{
				this.value = value;
				this.index = index;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int CompareTo(DoubleIndex o)
			{
				return value.CompareTo(o.value);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override bool Equals(object other)
			{

				if (this == other)
				{
					return true;
				}

				if (other is DoubleIndex)
				{
					return value.CompareTo(((DoubleIndex) other).value) == 0;
				}

				return false;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override int GetHashCode()
			{
				long bits = double.doubleToLongBits(value);
				return unchecked((int)((1438542 ^ ((long)((ulong)bits >> 32)) ^ bits) & 0xffffffff));
			}
		}

		/// <summary>
		/// Normalizes fitness values to the range [0,1]. Adds a penalty to the
		/// fitness value if out of range. The penalty is adjusted by calling
		/// setValueRange().
		/// </summary>
		private class FitnessFunction
		{
			private readonly CMAESOptimizer outerInstance;

			/// <summary>
			/// Determines the penalty for boundary violations </summary>
			internal double valueRange;
			/// <summary>
			/// Flag indicating whether the objective variables are forced into their
			/// bounds if defined
			/// </summary>
			internal readonly bool isRepairMode;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			public FitnessFunction(CMAESOptimizer outerInstance)
			{
				this.outerInstance = outerInstance;
				valueRange = 1;
				isRepairMode = true;
			}

			/// <param name="point"> Normalized objective variables. </param>
			/// <returns> the objective value + penalty for violated bounds. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double value(final double[] point)
			public virtual double value(double[] point)
			{
				double value;
				if (isRepairMode)
				{
					double[] repaired = repair(point);
					value = outerInstance.computeObjectiveValue(repaired) + penalty(point, repaired);
				}
				else
				{
					value = outerInstance.computeObjectiveValue(point);
				}
				return outerInstance.isMinimize ? value : -value;
			}

			/// <param name="x"> Normalized objective variables. </param>
			/// <returns> {@code true} if in bounds. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isFeasible(final double[] x)
			public virtual bool isFeasible(double[] x)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lB = CMAESOptimizer.this.getLowerBound();
				double[] lB = outerInstance.LowerBound;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] uB = CMAESOptimizer.this.getUpperBound();
				double[] uB = outerInstance.UpperBound;

				for (int i = 0; i < x.Length; i++)
				{
					if (x[i] < lB[i])
					{
						return false;
					}
					if (x[i] > uB[i])
					{
						return false;
					}
				}
				return true;
			}

			/// <param name="valueRange"> Adjusts the penalty computation. </param>
			public virtual double ValueRange
			{
				set
				{
					this.valueRange = value;
				}
			}

			/// <param name="x"> Normalized objective variables. </param>
			/// <returns> the repaired (i.e. all in bounds) objective variables. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double[] repair(final double[] x)
			internal virtual double[] repair(double[] x)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lB = CMAESOptimizer.this.getLowerBound();
				double[] lB = outerInstance.LowerBound;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] uB = CMAESOptimizer.this.getUpperBound();
				double[] uB = outerInstance.UpperBound;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] repaired = new double[x.length];
				double[] repaired = new double[x.Length];
				for (int i = 0; i < x.Length; i++)
				{
					if (x[i] < lB[i])
					{
						repaired[i] = lB[i];
					}
					else if (x[i] > uB[i])
					{
						repaired[i] = uB[i];
					}
					else
					{
						repaired[i] = x[i];
					}
				}
				return repaired;
			}

			/// <param name="x"> Normalized objective variables. </param>
			/// <param name="repaired"> Repaired objective variables. </param>
			/// <returns> Penalty value according to the violation of the bounds. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double penalty(final double[] x, final double[] repaired)
			internal virtual double penalty(double[] x, double[] repaired)
			{
				double penalty = 0;
				for (int i = 0; i < x.Length; i++)
				{
					double diff = FastMath.abs(x[i] - repaired[i]);
					penalty += diff * valueRange;
				}
				return outerInstance.isMinimize ? penalty : -penalty;
			}
		}

		// -----Matrix utility functions similar to the Matlab build in functions------

		/// <param name="m"> Input matrix </param>
		/// <returns> Matrix representing the element-wise logarithm of m. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix log(final mathlib.linear.RealMatrix m)
		private static RealMatrix log(RealMatrix m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.ColumnDimension);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					d[r][c] = FastMath.log(m.getEntry(r, c));
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <returns> Matrix representing the element-wise square root of m. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix sqrt(final mathlib.linear.RealMatrix m)
		private static RealMatrix sqrt(RealMatrix m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.ColumnDimension);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					d[r][c] = FastMath.sqrt(m.getEntry(r, c));
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <returns> Matrix representing the element-wise square of m. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix square(final mathlib.linear.RealMatrix m)
		private static RealMatrix square(RealMatrix m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.ColumnDimension);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					double e = m.getEntry(r, c);
					d[r][c] = e * e;
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix 1. </param>
		/// <param name="n"> Input matrix 2. </param>
		/// <returns> the matrix where the elements of m and n are element-wise multiplied. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix times(final mathlib.linear.RealMatrix m, final mathlib.linear.RealMatrix n)
		private static RealMatrix times(RealMatrix m, RealMatrix n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.ColumnDimension);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					d[r][c] = m.getEntry(r, c) * n.getEntry(r, c);
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix 1. </param>
		/// <param name="n"> Input matrix 2. </param>
		/// <returns> Matrix where the elements of m and n are element-wise divided. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix divide(final mathlib.linear.RealMatrix m, final mathlib.linear.RealMatrix n)
		private static RealMatrix divide(RealMatrix m, RealMatrix n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.ColumnDimension);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					d[r][c] = m.getEntry(r, c) / n.getEntry(r, c);
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <param name="cols"> Columns to select. </param>
		/// <returns> Matrix representing the selected columns. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix selectColumns(final mathlib.linear.RealMatrix m, final int[] cols)
		private static RealMatrix selectColumns(RealMatrix m, int[] cols)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][cols.length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][cols.Length];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, cols.Length);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < cols.Length; c++)
				{
					d[r][c] = m.getEntry(r, cols[c]);
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <param name="k"> Diagonal position. </param>
		/// <returns> Upper triangular part of matrix. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix triu(final mathlib.linear.RealMatrix m, int k)
		private static RealMatrix triu(RealMatrix m, int k)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.ColumnDimension);
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					d[r][c] = r <= c - k ? m.getEntry(r, c) : 0;
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <returns> Row matrix representing the sums of the rows. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix sumRows(final mathlib.linear.RealMatrix m)
		private static RealMatrix sumRows(RealMatrix m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[1][m.getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[1][m.ColumnDimension];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(1, m.ColumnDimension);
			for (int c = 0; c < m.ColumnDimension; c++)
			{
				double sum = 0;
				for (int r = 0; r < m.RowDimension; r++)
				{
					sum += m.getEntry(r, c);
				}
				d[0][c] = sum;
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <returns> the diagonal n-by-n matrix if m is a column matrix or the column
		/// matrix representing the diagonal if m is a n-by-n matrix. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix diag(final mathlib.linear.RealMatrix m)
		private static RealMatrix diag(RealMatrix m)
		{
			if (m.ColumnDimension == 1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][m.getRowDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][m.RowDimension];
				double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, m.RowDimension);
				for (int i = 0; i < m.RowDimension; i++)
				{
					d[i][i] = m.getEntry(i, 0);
				}
				return new Array2DRowRealMatrix(d, false);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[m.getRowDimension()][1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[m.RowDimension][1];
				double[][] d = RectangularArrays.ReturnRectangularDoubleArray(m.RowDimension, 1);
				for (int i = 0; i < m.ColumnDimension; i++)
				{
					d[i][0] = m.getEntry(i, i);
				}
				return new Array2DRowRealMatrix(d, false);
			}
		}

		/// <summary>
		/// Copies a column from m1 to m2.
		/// </summary>
		/// <param name="m1"> Source matrix. </param>
		/// <param name="col1"> Source column. </param>
		/// <param name="m2"> Target matrix. </param>
		/// <param name="col2"> Target column. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void copyColumn(final mathlib.linear.RealMatrix m1, int col1, mathlib.linear.RealMatrix m2, int col2)
		private static void copyColumn(RealMatrix m1, int col1, RealMatrix m2, int col2)
		{
			for (int i = 0; i < m1.RowDimension; i++)
			{
				m2.setEntry(i, col2, m1.getEntry(i, col1));
			}
		}

		/// <param name="n"> Number of rows. </param>
		/// <param name="m"> Number of columns. </param>
		/// <returns> n-by-m matrix filled with 1. </returns>
		private static RealMatrix ones(int n, int m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[n][m];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[n][m];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(n, m);
			for (int r = 0; r < n; r++)
			{
				Arrays.fill(d[r], 1);
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="n"> Number of rows. </param>
		/// <param name="m"> Number of columns. </param>
		/// <returns> n-by-m matrix of 0 values out of diagonal, and 1 values on
		/// the diagonal. </returns>
		private static RealMatrix eye(int n, int m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[n][m];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[n][m];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(n, m);
			for (int r = 0; r < n; r++)
			{
				if (r < m)
				{
					d[r][r] = 1;
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="n"> Number of rows. </param>
		/// <param name="m"> Number of columns. </param>
		/// <returns> n-by-m matrix of zero values. </returns>
		private static RealMatrix zeros(int n, int m)
		{
			return new Array2DRowRealMatrix(n, m);
		}

		/// <param name="mat"> Input matrix. </param>
		/// <param name="n"> Number of row replicates. </param>
		/// <param name="m"> Number of column replicates. </param>
		/// <returns> a matrix which replicates the input matrix in both directions. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static mathlib.linear.RealMatrix repmat(final mathlib.linear.RealMatrix mat, int n, int m)
		private static RealMatrix repmat(RealMatrix mat, int n, int m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rd = mat.getRowDimension();
			int rd = mat.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int cd = mat.getColumnDimension();
			int cd = mat.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[n * rd][m * cd];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[n * rd][m * cd];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(n * rd, m * cd);
			for (int r = 0; r < n * rd; r++)
			{
				for (int c = 0; c < m * cd; c++)
				{
					d[r][c] = mat.getEntry(r % rd, c % cd);
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="start"> Start value. </param>
		/// <param name="end"> End value. </param>
		/// <param name="step"> Step size. </param>
		/// <returns> a sequence as column matrix. </returns>
		private static RealMatrix sequence(double start, double end, double step)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = (int)((end - start) / step + 1);
			int size = (int)((end - start) / step + 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[size][1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[size][1];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(size, 1);
			double value = start;
			for (int r = 0; r < size; r++)
			{
				d[r][0] = value;
				value += step;
			}
			return new Array2DRowRealMatrix(d, false);
		}

		/// <param name="m"> Input matrix. </param>
		/// <returns> the maximum of the matrix element values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static double max(final mathlib.linear.RealMatrix m)
		private static double max(RealMatrix m)
		{
			double max = -double.MaxValue;
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					double e = m.getEntry(r, c);
					if (max < e)
					{
						max = e;
					}
				}
			}
			return max;
		}

		/// <param name="m"> Input matrix. </param>
		/// <returns> the minimum of the matrix element values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static double min(final mathlib.linear.RealMatrix m)
		private static double min(RealMatrix m)
		{
			double min = double.MaxValue;
			for (int r = 0; r < m.RowDimension; r++)
			{
				for (int c = 0; c < m.ColumnDimension; c++)
				{
					double e = m.getEntry(r, c);
					if (min > e)
					{
						min = e;
					}
				}
			}
			return min;
		}

		/// <param name="m"> Input array. </param>
		/// <returns> the maximum of the array values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static double max(final double[] m)
		private static double max(double[] m)
		{
			double max = -double.MaxValue;
			for (int r = 0; r < m.Length; r++)
			{
				if (max < m[r])
				{
					max = m[r];
				}
			}
			return max;
		}

		/// <param name="m"> Input array. </param>
		/// <returns> the minimum of the array values. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static double min(final double[] m)
		private static double min(double[] m)
		{
			double min = double.MaxValue;
			for (int r = 0; r < m.Length; r++)
			{
				if (min > m[r])
				{
					min = m[r];
				}
			}
			return min;
		}

		/// <param name="indices"> Input index array. </param>
		/// <returns> the inverse of the mapping defined by indices. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int[] inverse(final int[] indices)
		private static int[] inverse(int[] indices)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] inverse = new int[indices.length];
			int[] inverse = new int[indices.Length];
			for (int i = 0; i < indices.Length; i++)
			{
				inverse[indices[i]] = i;
			}
			return inverse;
		}

		/// <param name="indices"> Input index array. </param>
		/// <returns> the indices in inverse order (last is first). </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int[] reverse(final int[] indices)
		private static int[] reverse(int[] indices)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] reverse = new int[indices.length];
			int[] reverse = new int[indices.Length];
			for (int i = 0; i < indices.Length; i++)
			{
				reverse[i] = indices[indices.Length - i - 1];
			}
			return reverse;
		}

		/// <param name="size"> Length of random array. </param>
		/// <returns> an array of Gaussian random numbers. </returns>
		private double[] randn(int size)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] randn = new double[size];
			double[] randn = new double[size];
			for (int i = 0; i < size; i++)
			{
				randn[i] = random.nextGaussian();
			}
			return randn;
		}

		/// <param name="size"> Number of rows. </param>
		/// <param name="popSize"> Population size. </param>
		/// <returns> a 2-dimensional matrix of Gaussian random numbers. </returns>
		private RealMatrix randn1(int size, int popSize)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] d = new double[size][popSize];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] d = new double[size][popSize];
			double[][] d = RectangularArrays.ReturnRectangularDoubleArray(size, popSize);
			for (int r = 0; r < size; r++)
			{
				for (int c = 0; c < popSize; c++)
				{
					d[r][c] = random.nextGaussian();
				}
			}
			return new Array2DRowRealMatrix(d, false);
		}
	}

}