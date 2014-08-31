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
namespace org.apache.commons.math3.fitting.leastsquares
{

	using MultivariateMatrixFunction = org.apache.commons.math3.analysis.MultivariateMatrixFunction;
	using MultivariateVectorFunction = org.apache.commons.math3.analysis.MultivariateVectorFunction;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using DiagonalMatrix = org.apache.commons.math3.linear.DiagonalMatrix;
	using EigenDecomposition = org.apache.commons.math3.linear.EigenDecomposition;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using org.apache.commons.math3.optim;
	using org.apache.commons.math3.optim;
	using PointVectorValuePair = org.apache.commons.math3.optim.PointVectorValuePair;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using Incrementor = org.apache.commons.math3.util.Incrementor;
	using org.apache.commons.math3.util;

	/// <summary>
	/// A Factory for creating <seealso cref="LeastSquaresProblem"/>s.
	/// 
	/// @version $Id: LeastSquaresFactory.java 1573307 2014-03-02 14:02:21Z luc $
	/// @since 3.3
	/// </summary>
	public class LeastSquaresFactory
	{

		/// <summary>
		/// Prevent instantiation. </summary>
		private LeastSquaresFactory()
		{
		}

		 /// <summary>
		 /// Create a <seealso cref="org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem"/>
		 /// from the given elements. There will be no weights applied (Identity weights).
		 /// </summary>
		 /// <param name="model">          the model function. Produces the computed values. </param>
		 /// <param name="observed">       the observed (target) values </param>
		 /// <param name="start">          the initial guess. </param>
		 /// <param name="checker">        convergence checker </param>
		 /// <param name="maxEvaluations"> the maximum number of times to evaluate the model </param>
		 /// <param name="maxIterations">  the maximum number to times to iterate in the algorithm </param>
		 /// <returns> the specified General Least Squares problem. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LeastSquaresProblem create(final MultivariateJacobianFunction model, final org.apache.commons.math3.linear.RealVector observed, final org.apache.commons.math3.linear.RealVector start, final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation> checker, final int maxEvaluations, final int maxIterations)
		public static LeastSquaresProblem create(MultivariateJacobianFunction model, RealVector observed, RealVector start, ConvergenceChecker<LeastSquaresProblem_Evaluation> checker, int maxEvaluations, int maxIterations)
		{
			return new LocalLeastSquaresProblem(model, observed, start, checker, maxEvaluations, maxIterations);
		}

		/// <summary>
		/// Create a <seealso cref="org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem"/>
		/// from the given elements.
		/// </summary>
		/// <param name="model">          the model function. Produces the computed values. </param>
		/// <param name="observed">       the observed (target) values </param>
		/// <param name="start">          the initial guess. </param>
		/// <param name="weight">         the weight matrix </param>
		/// <param name="checker">        convergence checker </param>
		/// <param name="maxEvaluations"> the maximum number of times to evaluate the model </param>
		/// <param name="maxIterations">  the maximum number to times to iterate in the algorithm </param>
		/// <returns> the specified General Least Squares problem. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LeastSquaresProblem create(final MultivariateJacobianFunction model, final org.apache.commons.math3.linear.RealVector observed, final org.apache.commons.math3.linear.RealVector start, final org.apache.commons.math3.linear.RealMatrix weight, final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation> checker, final int maxEvaluations, final int maxIterations)
		public static LeastSquaresProblem create(MultivariateJacobianFunction model, RealVector observed, RealVector start, RealMatrix weight, ConvergenceChecker<LeastSquaresProblem_Evaluation> checker, int maxEvaluations, int maxIterations)
		{
			return weightMatrix(create(model, observed, start, checker, maxEvaluations, maxIterations), weight);
		}

		/// <summary>
		/// Create a <seealso cref="org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem"/>
		/// from the given elements.
		/// <p/>
		/// This factory method is provided for continuity with previous interfaces. Newer
		/// applications should use {@link #create(MultivariateJacobianFunction, RealVector,
		/// RealVector, ConvergenceChecker, int, int)}, or {@link #create(MultivariateJacobianFunction,
		/// RealVector, RealVector, RealMatrix, ConvergenceChecker, int, int)}.
		/// </summary>
		/// <param name="model">          the model function. Produces the computed values. </param>
		/// <param name="jacobian">       the jacobian of the model with respect to the parameters </param>
		/// <param name="observed">       the observed (target) values </param>
		/// <param name="start">          the initial guess. </param>
		/// <param name="weight">         the weight matrix </param>
		/// <param name="checker">        convergence checker </param>
		/// <param name="maxEvaluations"> the maximum number of times to evaluate the model </param>
		/// <param name="maxIterations">  the maximum number to times to iterate in the algorithm </param>
		/// <returns> the specified General Least Squares problem. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LeastSquaresProblem create(final org.apache.commons.math3.analysis.MultivariateVectorFunction model, final org.apache.commons.math3.analysis.MultivariateMatrixFunction jacobian, final double[] observed, final double[] start, final org.apache.commons.math3.linear.RealMatrix weight, final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation> checker, final int maxEvaluations, final int maxIterations)
		public static LeastSquaresProblem create(MultivariateVectorFunction model, MultivariateMatrixFunction jacobian, double[] observed, double[] start, RealMatrix weight, ConvergenceChecker<LeastSquaresProblem_Evaluation> checker, int maxEvaluations, int maxIterations)
		{
			return create(model(model, jacobian), new ArrayRealVector(observed, false), new ArrayRealVector(start, false), weight, checker, maxEvaluations, maxIterations);
		}

		/// <summary>
		/// Apply a dense weight matrix to the <seealso cref="LeastSquaresProblem"/>.
		/// </summary>
		/// <param name="problem"> the unweighted problem </param>
		/// <param name="weights"> the matrix of weights </param>
		/// <returns> a new <seealso cref="LeastSquaresProblem"/> with the weights applied. The original
		///         {@code problem} is not modified. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LeastSquaresProblem weightMatrix(final LeastSquaresProblem problem, final org.apache.commons.math3.linear.RealMatrix weights)
		public static LeastSquaresProblem weightMatrix(LeastSquaresProblem problem, RealMatrix weights)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix weightSquareRoot = squareRoot(weights);
			RealMatrix weightSquareRoot = squareRoot(weights);
			return new LeastSquaresAdapterAnonymousInnerClassHelper(problem, weightSquareRoot);
		}

		private class LeastSquaresAdapterAnonymousInnerClassHelper : LeastSquaresAdapter
		{
			private RealMatrix weightSquareRoot;

			public LeastSquaresAdapterAnonymousInnerClassHelper(org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem problem, RealMatrix weightSquareRoot) : base(problem)
			{
				this.weightSquareRoot = weightSquareRoot;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation evaluate(final org.apache.commons.math3.linear.RealVector point)
			public override LeastSquaresProblem_Evaluation evaluate(RealVector point)
			{
				return new DenseWeightedEvaluation(base.evaluate(point), weightSquareRoot);
			}
		}

		/// <summary>
		/// Apply a diagonal weight matrix to the <seealso cref="LeastSquaresProblem"/>.
		/// </summary>
		/// <param name="problem"> the unweighted problem </param>
		/// <param name="weights"> the diagonal of the weight matrix </param>
		/// <returns> a new <seealso cref="LeastSquaresProblem"/> with the weights applied. The original
		///         {@code problem} is not modified. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LeastSquaresProblem weightDiagonal(final LeastSquaresProblem problem, final org.apache.commons.math3.linear.RealVector weights)
		public static LeastSquaresProblem weightDiagonal(LeastSquaresProblem problem, RealVector weights)
		{
			//TODO more efficient implementation
			return weightMatrix(problem, new DiagonalMatrix(weights.toArray()));
		}

		/// <summary>
		/// Count the evaluations of a particular problem. The {@code counter} will be
		/// incremented every time <seealso cref="LeastSquaresProblem#evaluate(RealVector)"/> is called on
		/// the <em>returned</em> problem.
		/// </summary>
		/// <param name="problem"> the problem to track. </param>
		/// <param name="counter"> the counter to increment. </param>
		/// <returns> a least squares problem that tracks evaluations </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LeastSquaresProblem countEvaluations(final LeastSquaresProblem problem, final org.apache.commons.math3.util.Incrementor counter)
		public static LeastSquaresProblem countEvaluations(LeastSquaresProblem problem, Incrementor counter)
		{
			return new LeastSquaresAdapterAnonymousInnerClassHelper2(problem, counter);
		}

		private class LeastSquaresAdapterAnonymousInnerClassHelper2 : LeastSquaresAdapter
		{
			private Incrementor counter;

			public LeastSquaresAdapterAnonymousInnerClassHelper2(org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem problem, Incrementor counter) : base(problem)
			{
				this.counter = counter;
			}


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation evaluate(final org.apache.commons.math3.linear.RealVector point)
			public override LeastSquaresProblem_Evaluation evaluate(RealVector point)
			{
				counter.incrementCount();
				return base.evaluate(point);
			}

			/* delegate the rest */

		}

		/// <summary>
		/// View a convergence checker specified for a <seealso cref="PointVectorValuePair"/> as one
		/// specified for an <seealso cref="Evaluation"/>.
		/// </summary>
		/// <param name="checker"> the convergence checker to adapt. </param>
		/// <returns> a convergence checker that delegates to {@code checker}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation> evaluationChecker(final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointVectorValuePair> checker)
		public static ConvergenceChecker<LeastSquaresProblem_Evaluation> evaluationChecker(ConvergenceChecker<PointVectorValuePair> checker)
		{
			return new ConvergenceCheckerAnonymousInnerClassHelper(checker);
		}

		private class ConvergenceCheckerAnonymousInnerClassHelper : ConvergenceChecker<LeastSquaresProblem_Evaluation>
		{
			private ConvergenceChecker<PointVectorValuePair> checker;

			public ConvergenceCheckerAnonymousInnerClassHelper(ConvergenceChecker<PointVectorValuePair> checker)
			{
				this.checker = checker;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean converged(final int iteration, final org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation previous, final org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation current)
			public virtual bool converged(int iteration, LeastSquaresProblem_Evaluation previous, LeastSquaresProblem_Evaluation current)
			{
				return checker.converged(iteration, new PointVectorValuePair(previous.Point.toArray(), previous.Residuals.toArray(), false), new PointVectorValuePair(current.Point.toArray(), current.Residuals.toArray(), false)
			   );
			}
		}

		/// <summary>
		/// Computes the square-root of the weight matrix.
		/// </summary>
		/// <param name="m"> Symmetric, positive-definite (weight) matrix. </param>
		/// <returns> the square-root of the weight matrix. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static org.apache.commons.math3.linear.RealMatrix squareRoot(final org.apache.commons.math3.linear.RealMatrix m)
		private static RealMatrix squareRoot(RealMatrix m)
		{
			if (m is DiagonalMatrix)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = m.getRowDimension();
				int dim = m.RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix sqrtM = new org.apache.commons.math3.linear.DiagonalMatrix(dim);
				RealMatrix sqrtM = new DiagonalMatrix(dim);
				for (int i = 0; i < dim; i++)
				{
					sqrtM.setEntry(i, i, FastMath.sqrt(m.getEntry(i, i)));
				}
				return sqrtM;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.EigenDecomposition dec = new org.apache.commons.math3.linear.EigenDecomposition(m);
				EigenDecomposition dec = new EigenDecomposition(m);
				return dec.SquareRoot;
			}
		}

		/// <summary>
		/// Combine a <seealso cref="MultivariateVectorFunction"/> with a {@link
		/// MultivariateMatrixFunction} to produce a <seealso cref="MultivariateJacobianFunction"/>.
		/// </summary>
		/// <param name="value">    the vector value function </param>
		/// <param name="jacobian"> the Jacobian function </param>
		/// <returns> a function that computes both at the same time </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static MultivariateJacobianFunction model(final org.apache.commons.math3.analysis.MultivariateVectorFunction value, final org.apache.commons.math3.analysis.MultivariateMatrixFunction jacobian)
		public static MultivariateJacobianFunction model(MultivariateVectorFunction value, MultivariateMatrixFunction jacobian)
		{
			return new MultivariateJacobianFunctionAnonymousInnerClassHelper(value, jacobian);
		}

		private class MultivariateJacobianFunctionAnonymousInnerClassHelper : MultivariateJacobianFunction
		{
			private MultivariateVectorFunction value;
			private MultivariateMatrixFunction jacobian;

			public MultivariateJacobianFunctionAnonymousInnerClassHelper(MultivariateVectorFunction value, MultivariateMatrixFunction jacobian)
			{
				this.value = value;
				this.jacobian = jacobian;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.util.Pair<org.apache.commons.math3.linear.RealVector, org.apache.commons.math3.linear.RealMatrix> value(final org.apache.commons.math3.linear.RealVector point)
			public virtual Pair<RealVector, RealMatrix> value(RealVector point)
			{
				//TODO get array from RealVector without copying?
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pointArray = point.toArray();
				double[] pointArray = point.toArray();
				//evaluate and return data without copying
				return new Pair<RealVector, RealMatrix>(new ArrayRealVector(value.value(pointArray), false), new Array2DRowRealMatrix(jacobian.value(pointArray), false));
			}
		}

		/// <summary>
		/// A private, "field" immutable (not "real" immutable) implementation of {@link
		/// LeastSquaresProblem}.
		/// @since 3.3
		/// </summary>
		private class LocalLeastSquaresProblem : AbstractOptimizationProblem<LeastSquaresProblem_Evaluation>, LeastSquaresProblem
		{

			/// <summary>
			/// Target values for the model function at optimum. </summary>
			internal RealVector target;
			/// <summary>
			/// Model function. </summary>
			internal MultivariateJacobianFunction model;
			/// <summary>
			/// Initial guess. </summary>
			internal RealVector start;

			/// <summary>
			/// Create a <seealso cref="LeastSquaresProblem"/> from the given data.
			/// </summary>
			/// <param name="model">          the model function </param>
			/// <param name="target">         the observed data </param>
			/// <param name="start">          the initial guess </param>
			/// <param name="checker">        the convergence checker </param>
			/// <param name="maxEvaluations"> the allowed evaluations </param>
			/// <param name="maxIterations">  the allowed iterations </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: LocalLeastSquaresProblem(final MultivariateJacobianFunction model, final org.apache.commons.math3.linear.RealVector target, final org.apache.commons.math3.linear.RealVector start, final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation> checker, final int maxEvaluations, final int maxIterations)
			internal LocalLeastSquaresProblem(MultivariateJacobianFunction model, RealVector target, RealVector start, ConvergenceChecker<LeastSquaresProblem_Evaluation> checker, int maxEvaluations, int maxIterations) : base(maxEvaluations, maxIterations, checker)
			{
				this.target = target;
				this.model = model;
				this.start = start;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int ObservationSize
			{
				get
				{
					return target.Dimension;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int ParameterSize
			{
				get
				{
					return start.Dimension;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealVector Start
			{
				get
				{
					return start == null ? null : start.copy();
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation evaluate(final org.apache.commons.math3.linear.RealVector point)
			public virtual LeastSquaresProblem_Evaluation evaluate(RealVector point)
			{
				//evaluate value and jacobian in one function call
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.util.Pair<org.apache.commons.math3.linear.RealVector, org.apache.commons.math3.linear.RealMatrix> value = this.model.value(point);
				Pair<RealVector, RealMatrix> value = this.model.value(point);
				return new UnweightedEvaluation(value.First, value.Second, this.target, point.copy());
						// copy so optimizer can change point without changing our instance
			}

			/// <summary>
			/// Container with the model evaluation at a particular point.
			/// <p/>
			/// TODO revisit lazy evaluation
			/// </summary>
			private class UnweightedEvaluation : AbstractEvaluation
			{

				/// <summary>
				/// the point of evaluation </summary>
				internal readonly RealVector point;
				/// <summary>
				/// deriviative at point </summary>
				internal readonly RealMatrix jacobian;
				/// <summary>
				/// the computed residuals. </summary>
				internal readonly RealVector residuals;

				/// <summary>
				/// Create an <seealso cref="Evaluation"/> with no weights.
				/// </summary>
				/// <param name="values">   the computed function values </param>
				/// <param name="jacobian"> the computed function Jacobian </param>
				/// <param name="target">   the observed values </param>
				/// <param name="point">    the abscissa </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private UnweightedEvaluation(final org.apache.commons.math3.linear.RealVector values, final org.apache.commons.math3.linear.RealMatrix jacobian, final org.apache.commons.math3.linear.RealVector target, final org.apache.commons.math3.linear.RealVector point)
				internal UnweightedEvaluation(RealVector values, RealMatrix jacobian, RealVector target, RealVector point) : base(target.Dimension)
				{
					this.jacobian = jacobian;
					this.point = point;
					this.residuals = target.subtract(values);
				}

				/// <summary>
				/// {@inheritDoc} </summary>
				public override RealMatrix Jacobian
				{
					get
					{
						return this.jacobian;
					}
				}

				/// <summary>
				/// {@inheritDoc} </summary>
				public override RealVector Point
				{
					get
					{
						return this.point;
					}
				}

				/// <summary>
				/// {@inheritDoc} </summary>
				public override RealVector Residuals
				{
					get
					{
						return this.residuals;
					}
				}

			}

		}

	}


}