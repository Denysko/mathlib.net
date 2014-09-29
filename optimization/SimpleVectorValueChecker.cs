using System;

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

namespace mathlib.optimization
{

	using FastMath = mathlib.util.FastMath;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;

	/// <summary>
	/// Simple implementation of the <seealso cref="ConvergenceChecker"/> interface using
	/// only objective function values.
	/// 
	/// Convergence is considered to have been reached if either the relative
	/// difference between the objective function values is smaller than a
	/// threshold or if either the absolute difference between the objective
	/// function values is smaller than another threshold for all vectors elements.
	/// <br/>
	/// The <seealso cref="#converged(int,PointVectorValuePair,PointVectorValuePair) converged"/>
	/// method will also return {@code true} if the number of iterations has been set
	/// (see <seealso cref="#SimpleVectorValueChecker(double,double,int) this constructor"/>).
	/// 
	/// @version $Id: SimpleVectorValueChecker.java 1591835 2014-05-02 09:04:01Z tn $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class SimpleVectorValueChecker : AbstractConvergenceChecker<PointVectorValuePair>
	{
		/// <summary>
		/// If <seealso cref="#maxIterationCount"/> is set to this value, the number of
		/// iterations will never cause
		/// <seealso cref="#converged(int,PointVectorValuePair,PointVectorValuePair)"/>
		/// to return {@code true}.
		/// </summary>
		private const int ITERATION_CHECK_DISABLED = -1;
		/// <summary>
		/// Number of iterations after which the
		/// <seealso cref="#converged(int,PointVectorValuePair,PointVectorValuePair)"/> method
		/// will return true (unless the check is disabled).
		/// </summary>
		private readonly int maxIterationCount;

		/// <summary>
		/// Build an instance with default thresholds. </summary>
		/// @deprecated See <seealso cref="AbstractConvergenceChecker#AbstractConvergenceChecker()"/> 
		[Obsolete("See <seealso cref="AbstractConvergenceChecker#AbstractConvergenceChecker()"/>")]
		public SimpleVectorValueChecker()
		{
			maxIterationCount = ITERATION_CHECK_DISABLED;
		}

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
//ORIGINAL LINE: public SimpleVectorValueChecker(final double relativeThreshold, final double absoluteThreshold)
		public SimpleVectorValueChecker(double relativeThreshold, double absoluteThreshold) : base(relativeThreshold, absoluteThreshold)
		{
			maxIterationCount = ITERATION_CHECK_DISABLED;
		}

		/// <summary>
		/// Builds an instance with specified tolerance thresholds and
		/// iteration count.
		/// 
		/// In order to perform only relative checks, the absolute tolerance
		/// must be set to a negative value. In order to perform only absolute
		/// checks, the relative tolerance must be set to a negative value.
		/// </summary>
		/// <param name="relativeThreshold"> Relative tolerance threshold. </param>
		/// <param name="absoluteThreshold"> Absolute tolerance threshold. </param>
		/// <param name="maxIter"> Maximum iteration count. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code maxIter <= 0}.
		/// 
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SimpleVectorValueChecker(final double relativeThreshold, final double absoluteThreshold, final int maxIter)
		public SimpleVectorValueChecker(double relativeThreshold, double absoluteThreshold, int maxIter) : base(relativeThreshold, absoluteThreshold)
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
		/// This method may be called several times from the same algorithm
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
		/// <returns> {@code true} if the arguments satify the convergence criterion. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public boolean converged(final int iteration, final PointVectorValuePair previous, final PointVectorValuePair current)
		public override bool converged(int iteration, PointVectorValuePair previous, PointVectorValuePair current)
		{
			if (maxIterationCount != ITERATION_CHECK_DISABLED && iteration >= maxIterationCount)
			{
				return true;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = previous.getValueRef();
			double[] p = previous.ValueRef;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] c = current.getValueRef();
			double[] c = current.ValueRef;
			for (int i = 0; i < p.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pi = p[i];
				double pi = p[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ci = c[i];
				double ci = c[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double difference = mathlib.util.FastMath.abs(pi - ci);
				double difference = FastMath.abs(pi - ci);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double size = mathlib.util.FastMath.max(mathlib.util.FastMath.abs(pi), mathlib.util.FastMath.abs(ci));
				double size = FastMath.max(FastMath.abs(pi), FastMath.abs(ci));
				if (difference > size * RelativeThreshold && difference > AbsoluteThreshold)
				{
					return false;
				}
			}
			return true;
		}
	}

}