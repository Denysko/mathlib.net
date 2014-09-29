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
namespace mathlib.optim
{

	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;

	/// <summary>
	/// Maximum number of iterations performed by an (iterative) algorithm.
	/// 
	/// @version $Id: MaxIter.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.1
	/// </summary>
	public class MaxIter : OptimizationData
	{
		/// <summary>
		/// Allowed number of evalutations. </summary>
		private readonly int maxIter;

		/// <param name="max"> Allowed number of iterations. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code max <= 0}. </exception>
		public MaxIter(int max)
		{
			if (max <= 0)
			{
				throw new NotStrictlyPositiveException(max);
			}

			maxIter = max;
		}

		/// <summary>
		/// Gets the maximum number of evaluations.
		/// </summary>
		/// <returns> the allowed number of evaluations. </returns>
		public virtual int MaxIter
		{
			get
			{
				return maxIter;
			}
		}

		/// <summary>
		/// Factory method that creates instance of this class that represents
		/// a virtually unlimited number of iterations.
		/// </summary>
		/// <returns> a new instance suitable for allowing <seealso cref="Integer#MAX_VALUE"/>
		/// evaluations. </returns>
		public static MaxIter unlimited()
		{
			return new MaxIter(int.MaxValue);
		}
	}

}