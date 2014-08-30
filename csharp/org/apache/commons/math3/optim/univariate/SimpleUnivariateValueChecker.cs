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
namespace org.apache.commons.math3.optim.univariate
{

	using FastMath = org.apache.commons.math3.util.FastMath;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using org.apache.commons.math3.optim;

	/// <summary>
	/// Simple implementation of the
	/// <seealso cref="org.apache.commons.math3.optimization.ConvergenceChecker"/> interface
	/// that uses only objective function values.
	/// 
	/// Convergence is considered to have been reached if either the relative
	/// difference between the objective function values is smaller than a
	/// threshold or if either the absolute difference between the objective
	/// function values is smaller than another threshold.
	/// <br/>
	/// The {@link #converged(int,UnivariatePointValuePair,UnivariatePointValuePair)
	/// converged} method will also return {@code true} if the number of iterations
	/// has been set (see {@link #SimpleUnivariateValueChecker(double,double,int)
	/// this constructor}).
	/// 
	/// @version $Id: SimpleUnivariateValueChecker.java 1462503 2013-03-29 15:48:27Z luc $
	/// @since 3.1
	/// </summary>
	public class SimpleUnivariateValueChecker : AbstractConvergenceChecker<UnivariatePointValuePair>
	{
		/// <summary>
		/// If <seealso cref="#maxIterationCount"/> is set to this value, the number of
		/// iterations will never cause
		/// <seealso cref="#converged(int,UnivariatePointValuePair,UnivariatePointValuePair)"/>
		/// to return {@code true}.
		/// </summary>
		private const int ITERATION_CHECK_DISABLED = -1;
		/// <summary>
		/// Number of iterations after which the
		/// <seealso cref="#converged(int,UnivariatePointValuePair,UnivariatePointValuePair)"/>
		/// method will return true (unless the check is disabled).
		/// </summary>
		private readonly int maxIterationCount;

		/// <summary>
		/// Build an instance with specified thresholds.
		/// 
		/// In order to perform only relative checks, the absolute tolerance
		/// must be set to a negative value. In order to perform only absolute
		/// checks, the relative tolerance must be set to a negative value.
		/// </summary>
		/// <param name="relativeThreshold"> relative tolerance threshold </param>
		/// <param name="absoluteThreshold"> absolute tolerance threshold </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimpleUnivariateValueChecker(final double relativeThreshold, final double absoluteThreshold)
		public SimpleUnivariateValueChecker(double relativeThreshold, double absoluteThreshold) : base(relativeThreshold, absoluteThreshold)
		{
			maxIterationCount = ITERATION_CHECK_DISABLED;
		}

		/// <summary>
		/// Builds an instance with specified thresholds.
		/// 
		/// In order to perform only relative checks, the absolute tolerance
		/// must be set to a negative value. In order to perform only absolute
		/// checks, the relative tolerance must be set to a negative value.
		/// </summary>
		/// <param name="relativeThreshold"> relative tolerance threshold </param>
		/// <param name="absoluteThreshold"> absolute tolerance threshold </param>
		/// <param name="maxIter"> Maximum iteration count. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code maxIter <= 0}.
		/// 
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimpleUnivariateValueChecker(final double relativeThreshold, final double absoluteThreshold, final int maxIter)
		public SimpleUnivariateValueChecker(double relativeThreshold, double absoluteThreshold, int maxIter) : base(relativeThreshold, absoluteThreshold)
		{

			if (maxIter <= 0)
			{
				throw new NotStrictlyPositiveException(maxIter);
			}
			maxIterationCount = maxIter;
		}

		/// <summary>
		/// Check if the optimization algorithm has converged considering the
		/// last two points.
		/// This method may be called several time from the same algorithm
		/// iteration with different points. This can be detected by checking the
		/// iteration number at each call if needed. Each time this method is
		/// called, the previous and current point correspond to points with the
		/// same role at each iteration, so they can be compared. As an example,
		/// simplex-based algorithms call this method for all points of the simplex,
		/// not only for the best or worst ones.
		/// </summary>
		/// <param name="iteration"> Index of current iteration </param>
		/// <param name="previous"> Best point in the previous iteration. </param>
		/// <param name="current"> Best point in the current iteration. </param>
		/// <returns> {@code true} if the algorithm has converged. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean converged(final int iteration, final UnivariatePointValuePair previous, final UnivariatePointValuePair current)
		public override bool converged(int iteration, UnivariatePointValuePair previous, UnivariatePointValuePair current)
		{
			if (maxIterationCount != ITERATION_CHECK_DISABLED && iteration >= maxIterationCount)
			{
				return true;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = previous.getValue();
			double p = previous.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = current.getValue();
			double c = current.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double difference = org.apache.commons.math3.util.FastMath.abs(p - c);
			double difference = FastMath.abs(p - c);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double size = org.apache.commons.math3.util.FastMath.max(org.apache.commons.math3.util.FastMath.abs(p), org.apache.commons.math3.util.FastMath.abs(c));
			double size = FastMath.max(FastMath.abs(p), FastMath.abs(c));
			return difference <= size * RelativeThreshold || difference <= AbsoluteThreshold;
		}
	}

}