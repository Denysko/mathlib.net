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
namespace mathlib.genetics
{

	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;

	/// <summary>
	/// Stops after a fixed number of generations. Each time <seealso cref="#isSatisfied(Population)"/> is invoked, a generation
	/// counter is incremented. Once the counter reaches the configured <code>maxGenerations</code> value,
	/// <seealso cref="#isSatisfied(Population)"/> returns true.
	/// 
	/// @version $Id: FixedGenerationCount.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public class FixedGenerationCount : StoppingCondition
	{
		/// <summary>
		/// Number of generations that have passed </summary>
		private int numGenerations = 0;

		/// <summary>
		/// Maximum number of generations (stopping criteria) </summary>
		private readonly int maxGenerations;

		/// <summary>
		/// Create a new FixedGenerationCount instance.
		/// </summary>
		/// <param name="maxGenerations"> number of generations to evolve </param>
		/// <exception cref="NumberIsTooSmallException"> if the number of generations is &lt; 1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FixedGenerationCount(final int maxGenerations) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FixedGenerationCount(int maxGenerations)
		{
			if (maxGenerations <= 0)
			{
				throw new NumberIsTooSmallException(maxGenerations, 1, true);
			}
			this.maxGenerations = maxGenerations;
		}

		/// <summary>
		/// Determine whether or not the given number of generations have passed. Increments the number of generations
		/// counter if the maximum has not been reached.
		/// </summary>
		/// <param name="population"> ignored (no impact on result) </param>
		/// <returns> <code>true</code> IFF the maximum number of generations has been exceeded </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isSatisfied(final Population population)
		public virtual bool isSatisfied(Population population)
		{
			if (this.numGenerations < this.maxGenerations)
			{
				numGenerations++;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the number of generations that have already passed. </summary>
		/// <returns> the number of generations that have passed </returns>
		public virtual int NumGenerations
		{
			get
			{
				return numGenerations;
			}
		}

	}

}