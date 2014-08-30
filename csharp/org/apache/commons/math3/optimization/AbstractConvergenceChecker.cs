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

namespace org.apache.commons.math3.optimization
{

	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// Base class for all convergence checker implementations.
	/// </summary>
	/// @param <PAIR> Type of (point, value) pair.
	/// 
	/// @version $Id: AbstractConvergenceChecker.java 1422230 2012-12-15 12:11:13Z erans $ </param>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public abstract class AbstractConvergenceChecker<PAIR> : ConvergenceChecker<PAIR>
		/// <summary>
		/// Default relative threshold. </summary>
		/// @deprecated in 3.1 (to be removed in 4.0) because this value is too small
		/// to be useful as a default (cf. MATH-798). 
	{
		[Obsolete("in 3.1 (to be removed in 4.0) because this value is too small")]
		private static readonly double DEFAULT_RELATIVE_THRESHOLD = 100 * Precision.EPSILON;
		/// <summary>
		/// Default absolute threshold. </summary>
		/// @deprecated in 3.1 (to be removed in 4.0) because this value is too small
		/// to be useful as a default (cf. MATH-798). 
		[Obsolete("in 3.1 (to be removed in 4.0) because this value is too small")]
		private static readonly double DEFAULT_ABSOLUTE_THRESHOLD = 100 * Precision.SAFE_MIN;
		/// <summary>
		/// Relative tolerance threshold.
		/// </summary>
		private readonly double relativeThreshold;
		/// <summary>
		/// Absolute tolerance threshold.
		/// </summary>
		private readonly double absoluteThreshold;

		/// <summary>
		/// Build an instance with default thresholds. </summary>
		/// @deprecated in 3.1 (to be removed in 4.0). Convergence thresholds are
		/// problem-dependent. As this class is intended for users who want to set
		/// their own convergence criterion instead of relying on an algorithm's
		/// default procedure, they should also set the thresholds appropriately
		/// (cf. MATH-798). 
		[Obsolete("in 3.1 (to be removed in 4.0). Convergence thresholds are")]
		public AbstractConvergenceChecker()
		{
			this.relativeThreshold = DEFAULT_RELATIVE_THRESHOLD;
			this.absoluteThreshold = DEFAULT_ABSOLUTE_THRESHOLD;
		}

		/// <summary>
		/// Build an instance with a specified thresholds.
		/// </summary>
		/// <param name="relativeThreshold"> relative tolerance threshold </param>
		/// <param name="absoluteThreshold"> absolute tolerance threshold </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractConvergenceChecker(final double relativeThreshold, final double absoluteThreshold)
		public AbstractConvergenceChecker(double relativeThreshold, double absoluteThreshold)
		{
			this.relativeThreshold = relativeThreshold;
			this.absoluteThreshold = absoluteThreshold;
		}

		/// <returns> the relative threshold. </returns>
		public virtual double RelativeThreshold
		{
			get
			{
				return relativeThreshold;
			}
		}

		/// <returns> the absolute threshold. </returns>
		public virtual double AbsoluteThreshold
		{
			get
			{
				return absoluteThreshold;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public abstract bool converged(int iteration, PAIR previous, PAIR current);
	}

}