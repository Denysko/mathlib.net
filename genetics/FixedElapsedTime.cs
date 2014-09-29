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
	/// Stops after a fixed amount of time has elapsed.
	/// <p>
	/// The first time <seealso cref="#isSatisfied(Population)"/> is invoked, the end time of the evolution is determined based on the
	/// provided <code>maxTime</code> value. Once the elapsed time reaches the configured <code>maxTime</code> value,
	/// <seealso cref="#isSatisfied(Population)"/> returns true.
	/// 
	/// @version $Id: FixedElapsedTime.java 1385297 2012-09-16 16:05:57Z tn $
	/// @since 3.1
	/// </summary>
	public class FixedElapsedTime : StoppingCondition
	{
		/// <summary>
		/// Maximum allowed time period (in nanoseconds). </summary>
		private readonly long maxTimePeriod;

		/// <summary>
		/// The predetermined termination time (stopping condition). </summary>
		private long endTime = -1;

		/// <summary>
		/// Create a new <seealso cref="FixedElapsedTime"/> instance.
		/// </summary>
		/// <param name="maxTime"> maximum number of seconds generations are allowed to evolve </param>
		/// <exception cref="NumberIsTooSmallException"> if the provided time is &lt; 0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FixedElapsedTime(final long maxTime) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FixedElapsedTime(long maxTime) : this(maxTime, TimeUnit.SECONDS)
		{
		}

		/// <summary>
		/// Create a new <seealso cref="FixedElapsedTime"/> instance.
		/// </summary>
		/// <param name="maxTime"> maximum time generations are allowed to evolve </param>
		/// <param name="unit"> <seealso cref="TimeUnit"/> of the maxTime argument </param>
		/// <exception cref="NumberIsTooSmallException"> if the provided time is &lt; 0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FixedElapsedTime(final long maxTime, final java.util.concurrent.TimeUnit unit) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FixedElapsedTime(long maxTime, TimeUnit unit)
		{
			if (maxTime < 0)
			{
				throw new NumberIsTooSmallException(maxTime, 0, true);
			}
			maxTimePeriod = unit.toNanos(maxTime);
		}

		/// <summary>
		/// Determine whether or not the maximum allowed time has passed.
		/// The termination time is determined after the first generation.
		/// </summary>
		/// <param name="population"> ignored (no impact on result) </param>
		/// <returns> <code>true</code> IFF the maximum allowed time period has elapsed </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isSatisfied(final Population population)
		public virtual bool isSatisfied(Population population)
		{
			if (endTime < 0)
			{
				endTime = System.nanoTime() + maxTimePeriod;
			}

			return System.nanoTime() >= endTime;
		}
	}

}