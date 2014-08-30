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
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using org.apache.commons.math3.optim;
	using PointVectorValuePair = org.apache.commons.math3.optim.PointVectorValuePair;

	/// <summary>
	/// A mutable builder for <seealso cref="LeastSquaresProblem"/>s.
	/// 
	/// @version $Id: LeastSquaresBuilder.java 1569362 2014-02-18 14:33:49Z luc $ </summary>
	/// <seealso cref= LeastSquaresFactory
	/// @since 3.3 </seealso>
	public class LeastSquaresBuilder
	{

		/// <summary>
		/// max evaluations </summary>
		private int maxEvaluations_Renamed;
		/// <summary>
		/// max iterations </summary>
		private int maxIterations_Renamed;
		/// <summary>
		/// convergence checker </summary>
		private ConvergenceChecker<LeastSquaresProblem_Evaluation> checker_Renamed;
		/// <summary>
		/// model function </summary>
		private MultivariateJacobianFunction model_Renamed;
		/// <summary>
		/// observed values </summary>
		private RealVector target_Renamed;
		/// <summary>
		/// initial guess </summary>
		private RealVector start_Renamed;
		/// <summary>
		/// weight matrix </summary>
		private RealMatrix weight_Renamed;


		/// <summary>
		/// Construct a <seealso cref="LeastSquaresProblem"/> from the data in this builder.
		/// </summary>
		/// <returns> a new <seealso cref="LeastSquaresProblem"/>. </returns>
		public virtual LeastSquaresProblem build()
		{
			return LeastSquaresFactory.create(model_Renamed, target_Renamed, start_Renamed, weight_Renamed, checker_Renamed, maxEvaluations_Renamed, maxIterations_Renamed);
		}

		/// <summary>
		/// Configure the max evaluations.
		/// </summary>
		/// <param name="newMaxEvaluations"> the maximum number of evaluations permitted. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder maxEvaluations(final int newMaxEvaluations)
		public virtual LeastSquaresBuilder maxEvaluations(int newMaxEvaluations)
		{
			this.maxEvaluations_Renamed = newMaxEvaluations;
			return this;
		}

		/// <summary>
		/// Configure the max iterations.
		/// </summary>
		/// <param name="newMaxIterations"> the maximum number of iterations permitted. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder maxIterations(final int newMaxIterations)
		public virtual LeastSquaresBuilder maxIterations(int newMaxIterations)
		{
			this.maxIterations_Renamed = newMaxIterations;
			return this;
		}

		/// <summary>
		/// Configure the convergence checker.
		/// </summary>
		/// <param name="newChecker"> the convergence checker. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder checker(final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation> newChecker)
		public virtual LeastSquaresBuilder checker(ConvergenceChecker<LeastSquaresProblem_Evaluation> newChecker)
		{
			this.checker_Renamed = newChecker;
			return this;
		}

		/// <summary>
		/// Configure the convergence checker.
		/// <p/>
		/// This function is an overloaded version of <seealso cref="#checker(ConvergenceChecker)"/>.
		/// </summary>
		/// <param name="newChecker"> the convergence checker. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder checkerPair(final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointVectorValuePair> newChecker)
		public virtual LeastSquaresBuilder checkerPair(ConvergenceChecker<PointVectorValuePair> newChecker)
		{
			return this.checker(LeastSquaresFactory.evaluationChecker(newChecker));
		}

		/// <summary>
		/// Configure the model function.
		/// </summary>
		/// <param name="value"> the model function value </param>
		/// <param name="jacobian"> the Jacobian of {@code value} </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder model(final org.apache.commons.math3.analysis.MultivariateVectorFunction value, final org.apache.commons.math3.analysis.MultivariateMatrixFunction jacobian)
		public virtual LeastSquaresBuilder model(MultivariateVectorFunction value, MultivariateMatrixFunction jacobian)
		{
			return model(LeastSquaresFactory.model(value, jacobian));
		}

		/// <summary>
		/// Configure the model function.
		/// </summary>
		/// <param name="newModel"> the model function value and Jacobian </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder model(final MultivariateJacobianFunction newModel)
		public virtual LeastSquaresBuilder model(MultivariateJacobianFunction newModel)
		{
			this.model_Renamed = newModel;
			return this;
		}

		/// <summary>
		/// Configure the observed data.
		/// </summary>
		/// <param name="newTarget"> the observed data. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder target(final org.apache.commons.math3.linear.RealVector newTarget)
		public virtual LeastSquaresBuilder target(RealVector newTarget)
		{
			this.target_Renamed = newTarget;
			return this;
		}

		/// <summary>
		/// Configure the observed data.
		/// </summary>
		/// <param name="newTarget"> the observed data. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder target(final double[] newTarget)
		public virtual LeastSquaresBuilder target(double[] newTarget)
		{
			return target(new ArrayRealVector(newTarget, false));
		}

		/// <summary>
		/// Configure the initial guess.
		/// </summary>
		/// <param name="newStart"> the initial guess. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder start(final org.apache.commons.math3.linear.RealVector newStart)
		public virtual LeastSquaresBuilder start(RealVector newStart)
		{
			this.start_Renamed = newStart;
			return this;
		}

		/// <summary>
		/// Configure the initial guess.
		/// </summary>
		/// <param name="newStart"> the initial guess. </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder start(final double[] newStart)
		public virtual LeastSquaresBuilder start(double[] newStart)
		{
			return start(new ArrayRealVector(newStart, false));
		}

		/// <summary>
		/// Configure the weight matrix.
		/// </summary>
		/// <param name="newWeight"> the weight matrix </param>
		/// <returns> this </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresBuilder weight(final org.apache.commons.math3.linear.RealMatrix newWeight)
		public virtual LeastSquaresBuilder weight(RealMatrix newWeight)
		{
			this.weight_Renamed = newWeight;
			return this;
		}

	}

}