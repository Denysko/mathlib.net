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
	/// A pedantic implementation of <seealso cref="Optimum"/>.
	/// 
	/// @version $Id: OptimumImpl.java 1571306 2014-02-24 14:57:44Z luc $
	/// @since 3.3
	/// </summary>
	internal class OptimumImpl : LeastSquaresOptimizer_Optimum
	{

		/// <summary>
		/// abscissa and ordinate </summary>
		private readonly LeastSquaresProblem_Evaluation value;
		/// <summary>
		/// number of evaluations to compute this optimum </summary>
		private readonly int evaluations;
		/// <summary>
		/// number of iterations to compute this optimum </summary>
		private readonly int iterations;

		/// <summary>
		/// Construct an optimum from an evaluation and the values of the counters.
		/// </summary>
		/// <param name="value">       the function value </param>
		/// <param name="evaluations"> number of times the function was evaluated </param>
		/// <param name="iterations">  number of iterations of the algorithm </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: OptimumImpl(final org.apache.commons.math3.fitting.leastsquares.LeastSquaresProblem_Evaluation value, final int evaluations, final int iterations)
		internal OptimumImpl(LeastSquaresProblem_Evaluation value, int evaluations, int iterations)
		{
			this.value = value;
			this.evaluations = evaluations;
			this.iterations = iterations;
		}

		/* auto-generated implementations */

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Evaluations
		{
			get
			{
				return evaluations;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Iterations
		{
			get
			{
				return iterations;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix getCovariances(double threshold)
		{
			return value.getCovariances(threshold);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealVector getSigma(double covarianceSingularityThreshold)
		{
			return value.getSigma(covarianceSingularityThreshold);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double RMS
		{
			get
			{
				return value.RMS;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix Jacobian
		{
			get
			{
				return value.Jacobian;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Cost
		{
			get
			{
				return value.Cost;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealVector Residuals
		{
			get
			{
				return value.Residuals;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealVector Point
		{
			get
			{
				return value.Point;
			}
		}
	}

}