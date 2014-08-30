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

	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;

	/// <summary>
	/// Applies a dense weight matrix to an evaluation.
	/// 
	/// @version $Id: DenseWeightedEvaluation.java 1571306 2014-02-24 14:57:44Z luc $
	/// @since 3.3
	/// </summary>
	internal class DenseWeightedEvaluation : AbstractEvaluation
	{

		/// <summary>
		/// the unweighted evaluation </summary>
		private readonly LeastSquaresProblem_Evaluation unweighted;
		/// <summary>
		/// reference to the weight square root matrix </summary>
		private readonly RealMatrix weightSqrt;

		/// <summary>
		/// Create a weighted evaluation from an unweighted one.
		/// </summary>
		/// <param name="unweighted"> the evalutation before weights are applied </param>
		/// <param name="weightSqrt"> the matrix square root of the weight matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: DenseWeightedEvaluation(final org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation unweighted, final org.apache.commons.math3.linear.RealMatrix weightSqrt)
		internal DenseWeightedEvaluation(LeastSquaresProblem_Evaluation unweighted, RealMatrix weightSqrt) : base(weightSqrt.ColumnDimension)
		{
			// weight square root is square, nR=nC=number of observations
			this.unweighted = unweighted;
			this.weightSqrt = weightSqrt;
		}

		/* apply weights */

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealMatrix Jacobian
		{
			get
			{
				return weightSqrt.multiply(this.unweighted.Jacobian);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector Residuals
		{
			get
			{
				return this.weightSqrt.operate(this.unweighted.Residuals);
			}
		}

		/* delegate */

		/// <summary>
		/// {@inheritDoc} </summary>
		public override RealVector Point
		{
			get
			{
				return unweighted.Point;
			}
		}

	}

}