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
namespace org.apache.commons.math3.optim
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;

	/// <summary>
	/// Base class for implementing optimizers for multivariate functions.
	/// It contains the boiler-plate code for initial guess and bounds
	/// specifications.
	/// <em>It is not a "user" class.</em>
	/// </summary>
	/// @param <PAIR> Type of the point/value pair returned by the optimization
	/// algorithm.
	/// 
	/// @version $Id: BaseMultivariateOptimizer.java 1443444 2013-02-07 12:41:36Z erans $
	/// @since 3.1 </param>
	public abstract class BaseMultivariateOptimizer<PAIR> : BaseOptimizer<PAIR>
	{
		/// <summary>
		/// Initial guess. </summary>
		private double[] start;
		/// <summary>
		/// Lower bounds. </summary>
		private double[] lowerBound;
		/// <summary>
		/// Upper bounds. </summary>
		private double[] upperBound;

		/// <param name="checker"> Convergence checker. </param>
		protected internal BaseMultivariateOptimizer(ConvergenceChecker<PAIR> checker) : base(checker)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		/// <param name="optData"> Optimization data. In addition to those documented in
		/// <seealso cref="BaseOptimizer#parseOptimizationData(OptimizationData[]) BaseOptimizer"/>,
		/// this method will register the following data:
		/// <ul>
		///  <li><seealso cref="InitialGuess"/></li>
		///  <li><seealso cref="SimpleBounds"/></li>
		/// </ul> </param>
		/// <returns> {@inheritDoc} </returns>
		public override PAIR optimize(params OptimizationData[] optData)
		{
			// Perform optimization.
			return base.optimize(optData);
		}

		/// <summary>
		/// Scans the list of (required and optional) optimization data that
		/// characterize the problem.
		/// </summary>
		/// <param name="optData"> Optimization data. The following data will be looked for:
		/// <ul>
		///  <li><seealso cref="InitialGuess"/></li>
		///  <li><seealso cref="SimpleBounds"/></li>
		/// </ul> </param>
		protected internal override void parseOptimizationData(params OptimizationData[] optData)
		{
			// Allow base class to register its own data.
			base.parseOptimizationData(optData);

			// The existing values (as set by the previous call) are reused if
			// not provided in the argument list.
			foreach (OptimizationData data in optData)
			{
				if (data is InitialGuess)
				{
					start = ((InitialGuess) data).InitialGuess;
					continue;
				}
				if (data is SimpleBounds)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SimpleBounds bounds = (SimpleBounds) data;
					SimpleBounds bounds = (SimpleBounds) data;
					lowerBound = bounds.Lower;
					upperBound = bounds.Upper;
					continue;
				}
			}

			// Check input consistency.
			checkParameters();
		}

		/// <summary>
		/// Gets the initial guess.
		/// </summary>
		/// <returns> the initial guess, or {@code null} if not set. </returns>
		public virtual double[] StartPoint
		{
			get
			{
				return start == null ? null : start.clone();
			}
		}
		/// <returns> the lower bounds, or {@code null} if not set. </returns>
		public virtual double[] LowerBound
		{
			get
			{
				return lowerBound == null ? null : lowerBound.clone();
			}
		}
		/// <returns> the upper bounds, or {@code null} if not set. </returns>
		public virtual double[] UpperBound
		{
			get
			{
				return upperBound == null ? null : upperBound.clone();
			}
		}

		/// <summary>
		/// Check parameters consistency.
		/// </summary>
		private void checkParameters()
		{
			if (start != null)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = start.length;
				int dim = start.Length;
				if (lowerBound != null)
				{
					if (lowerBound.Length != dim)
					{
						throw new DimensionMismatchException(lowerBound.Length, dim);
					}
					for (int i = 0; i < dim; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v = start[i];
						double v = start[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lo = lowerBound[i];
						double lo = lowerBound[i];
						if (v < lo)
						{
							throw new NumberIsTooSmallException(v, lo, true);
						}
					}
				}
				if (upperBound != null)
				{
					if (upperBound.Length != dim)
					{
						throw new DimensionMismatchException(upperBound.Length, dim);
					}
					for (int i = 0; i < dim; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v = start[i];
						double v = start[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hi = upperBound[i];
						double hi = upperBound[i];
						if (v > hi)
						{
							throw new NumberIsTooLargeException(v, hi, true);
						}
					}
				}
			}
		}
	}

}