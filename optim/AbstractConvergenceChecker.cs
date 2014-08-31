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

	/// <summary>
	/// Base class for all convergence checker implementations.
	/// </summary>
	/// @param <PAIR> Type of (point, value) pair.
	/// 
	/// @version $Id: AbstractConvergenceChecker.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.0 </param>
	public abstract class AbstractConvergenceChecker<PAIR> : ConvergenceChecker<PAIR>
	{
		/// <summary>
		/// Relative tolerance threshold.
		/// </summary>
		private readonly double relativeThreshold;
		/// <summary>
		/// Absolute tolerance threshold.
		/// </summary>
		private readonly double absoluteThreshold;

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