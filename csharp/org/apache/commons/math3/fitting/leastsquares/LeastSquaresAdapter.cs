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

	using RealVector = org.apache.commons.math3.linear.RealVector;
	using org.apache.commons.math3.optim;
	using Incrementor = org.apache.commons.math3.util.Incrementor;

	/// <summary>
	/// An adapter that delegates to another implementation of <seealso cref="LeastSquaresProblem"/>.
	/// 
	/// @version $Id: LeastSquaresAdapter.java 1569362 2014-02-18 14:33:49Z luc $
	/// @since 3.3
	/// </summary>
	public class LeastSquaresAdapter : LeastSquaresProblem
	{

		/// <summary>
		/// the delegate problem </summary>
		private readonly LeastSquaresProblem problem;

		/// <summary>
		/// Delegate the <seealso cref="LeastSquaresProblem"/> interface to the given implementation.
		/// </summary>
		/// <param name="problem"> the delegate </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresAdapter(final LeastSquaresProblem problem)
		public LeastSquaresAdapter(LeastSquaresProblem problem)
		{
			this.problem = problem;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealVector Start
		{
			get
			{
				return problem.Start;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int ObservationSize
		{
			get
			{
				return problem.ObservationSize;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int ParameterSize
		{
			get
			{
				return problem.ParameterSize;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <param name="point"> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LeastSquaresProblem_Evaluation evaluate(final org.apache.commons.math3.linear.RealVector point)
		public virtual LeastSquaresProblem_Evaluation evaluate(RealVector point)
		{
			return problem.evaluate(point);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Incrementor EvaluationCounter
		{
			get
			{
				return problem.EvaluationCounter;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Incrementor IterationCounter
		{
			get
			{
				return problem.IterationCounter;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ConvergenceChecker<LeastSquaresProblem_Evaluation> ConvergenceChecker
		{
			get
			{
				return problem.ConvergenceChecker;
			}
		}
	}

}