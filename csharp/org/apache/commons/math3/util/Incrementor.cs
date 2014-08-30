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
namespace org.apache.commons.math3.util
{

	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;

	/// <summary>
	/// Utility that increments a counter until a maximum is reached, at
	/// which point, the instance will by default throw a
	/// <seealso cref="MaxCountExceededException"/>.
	/// However, the user is able to override this behaviour by defining a
	/// custom <seealso cref="MaxCountExceededCallback callback"/>, in order to e.g.
	/// select which exception must be thrown.
	/// 
	/// @since 3.0
	/// @version $Id: Incrementor.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class Incrementor
	{
		/// <summary>
		/// Upper limit for the counter.
		/// </summary>
		private int maximalCount;
		/// <summary>
		/// Current count.
		/// </summary>
		private int count = 0;
		/// <summary>
		/// Function called at counter exhaustion.
		/// </summary>
		private readonly MaxCountExceededCallback maxCountCallback;

		/// <summary>
		/// Default constructor.
		/// For the new instance to be useful, the maximal count must be set
		/// by calling <seealso cref="#setMaximalCount(int) setMaximalCount"/>.
		/// </summary>
		public Incrementor() : this(0)
		{
		}

		/// <summary>
		/// Defines a maximal count.
		/// </summary>
		/// <param name="max"> Maximal count. </param>
		public Incrementor(int max) : this(max, new MaxCountExceededCallbackAnonymousInnerClassHelper(this, max))
		{
		}

		private class MaxCountExceededCallbackAnonymousInnerClassHelper : MaxCountExceededCallback
		{
			private readonly Incrementor outerInstance;

			private int max;

			public MaxCountExceededCallbackAnonymousInnerClassHelper(Incrementor outerInstance, int max)
			{
				this.outerInstance = outerInstance;
				this.max = max;
			}

						 /// <summary>
						 /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void trigger(int max) throws org.apache.commons.math3.exception.MaxCountExceededException
			public virtual void trigger(int max)
			{
				throw new MaxCountExceededException(max);
			}
		}

		/// <summary>
		/// Defines a maximal count and a callback method to be triggered at
		/// counter exhaustion.
		/// </summary>
		/// <param name="max"> Maximal count. </param>
		/// <param name="cb"> Function to be called when the maximal count has been reached. </param>
		/// <exception cref="NullArgumentException"> if {@code cb} is {@code null} </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Incrementor(int max, MaxCountExceededCallback cb) throws org.apache.commons.math3.exception.NullArgumentException
		public Incrementor(int max, MaxCountExceededCallback cb)
		{
			if (cb == null)
			{
				throw new NullArgumentException();
			}
			maximalCount = max;
			maxCountCallback = cb;
		}

		/// <summary>
		/// Sets the upper limit for the counter.
		/// This does not automatically reset the current count to zero (see
		/// <seealso cref="#resetCount()"/>).
		/// </summary>
		/// <param name="max"> Upper limit of the counter. </param>
		public virtual int MaximalCount
		{
			set
			{
				maximalCount = value;
			}
			get
			{
				return maximalCount;
			}
		}


		/// <summary>
		/// Gets the current count.
		/// </summary>
		/// <returns> the current count. </returns>
		public virtual int Count
		{
			get
			{
				return count;
			}
		}

		/// <summary>
		/// Checks whether a single increment is allowed.
		/// </summary>
		/// <returns> {@code false} if the next call to {@link #incrementCount(int)
		/// incrementCount} will trigger a {@code MaxCountExceededException},
		/// {@code true} otherwise. </returns>
		public virtual bool canIncrement()
		{
			return count < maximalCount;
		}

		/// <summary>
		/// Performs multiple increments.
		/// See the other <seealso cref="#incrementCount() incrementCount"/> method).
		/// </summary>
		/// <param name="value"> Number of increments. </param>
		/// <exception cref="MaxCountExceededException"> at counter exhaustion. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementCount(int value) throws org.apache.commons.math3.exception.MaxCountExceededException
		public virtual void incrementCount(int value)
		{
			for (int i = 0; i < value; i++)
			{
				incrementCount();
			}
		}

		/// <summary>
		/// Adds one to the current iteration count.
		/// At counter exhaustion, this method will call the
		/// <seealso cref="MaxCountExceededCallback#trigger(int) trigger"/> method of the
		/// callback object passed to the
		/// <seealso cref="#Incrementor(int,MaxCountExceededCallback) constructor"/>.
		/// If not explictly set, a default callback is used that will throw
		/// a {@code MaxCountExceededException}.
		/// </summary>
		/// <exception cref="MaxCountExceededException"> at counter exhaustion, unless a
		/// custom <seealso cref="MaxCountExceededCallback callback"/> has been set at
		/// construction. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementCount() throws org.apache.commons.math3.exception.MaxCountExceededException
		public virtual void incrementCount()
		{
			if (++count > maximalCount)
			{
				maxCountCallback.trigger(maximalCount);
			}
		}

		/// <summary>
		/// Resets the counter to 0.
		/// </summary>
		public virtual void resetCount()
		{
			count = 0;
		}

		/// <summary>
		/// Defines a method to be called at counter exhaustion.
		/// The <seealso cref="#trigger(int) trigger"/> method should usually throw an exception.
		/// </summary>
		public interface MaxCountExceededCallback
		{
			/// <summary>
			/// Function called when the maximal count has been reached.
			/// </summary>
			/// <param name="maximalCount"> Maximal count. </param>
			/// <exception cref="MaxCountExceededException"> at counter exhaustion </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void trigger(int maximalCount) throws org.apache.commons.math3.exception.MaxCountExceededException;
			void trigger(int maximalCount);
		}
	}

}