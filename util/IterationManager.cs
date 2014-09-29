using System.Collections.Generic;

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
namespace mathlib.util
{


    using MaxCountExceededException = mathlib.exception.MaxCountExceededException;

	/// <summary>
	/// This abstract class provides a general framework for managing iterative
	/// algorithms. The maximum number of iterations can be set, and methods are
	/// provided to monitor the current iteration count. A lightweight event
	/// framework is also provided.
	/// 
	/// @version $Id: IterationManager.java 1422313 2012-12-15 18:53:41Z psteitz $
	/// </summary>
	public class IterationManager
	{

		/// <summary>
		/// Keeps a count of the number of iterations. </summary>
		private readonly Incrementor iterations;

		/// <summary>
		/// The collection of all listeners attached to this iterative algorithm. </summary>
		private readonly ICollection<IterationListener> listeners;

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="maxIterations"> the maximum number of iterations </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IterationManager(final int maxIterations)
		public IterationManager(int maxIterations)
		{
			this.iterations = new Incrementor(maxIterations);
			this.listeners = new CopyOnWriteArrayList<IterationListener>();
		}

		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="maxIterations"> the maximum number of iterations </param>
		/// <param name="callBack"> the function to be called when the maximum number of
		/// iterations has been reached </param>
		/// <exception cref="mathlib.exception.NullArgumentException"> if {@code callBack} is {@code null}
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public IterationManager(final int maxIterations, final Incrementor.MaxCountExceededCallback callBack)
		public IterationManager(int maxIterations, Incrementor.MaxCountExceededCallback callBack)
		{
			this.iterations = new Incrementor(maxIterations, callBack);
			this.listeners = new CopyOnWriteArrayList<IterationListener>();
		}

		/// <summary>
		/// Attaches a listener to this manager.
		/// </summary>
		/// <param name="listener"> A {@code IterationListener} object. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addIterationListener(final IterationListener listener)
		public virtual void addIterationListener(IterationListener listener)
		{
			listeners.Add(listener);
		}

		/// <summary>
		/// Informs all registered listeners that the initial phase (prior to the
		/// main iteration loop) has been completed.
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void fireInitializationEvent(final IterationEvent e)
		public virtual void fireInitializationEvent(IterationEvent e)
		{
			foreach (IterationListener l in listeners)
			{
				l.initializationPerformed(e);
			}
		}

		/// <summary>
		/// Informs all registered listeners that a new iteration (in the main
		/// iteration loop) has been performed.
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void fireIterationPerformedEvent(final IterationEvent e)
		public virtual void fireIterationPerformedEvent(IterationEvent e)
		{
			foreach (IterationListener l in listeners)
			{
				l.iterationPerformed(e);
			}
		}

		/// <summary>
		/// Informs all registered listeners that a new iteration (in the main
		/// iteration loop) has been started.
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void fireIterationStartedEvent(final IterationEvent e)
		public virtual void fireIterationStartedEvent(IterationEvent e)
		{
			foreach (IterationListener l in listeners)
			{
				l.iterationStarted(e);
			}
		}

		/// <summary>
		/// Informs all registered listeners that the final phase (post-iterations)
		/// has been completed.
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void fireTerminationEvent(final IterationEvent e)
		public virtual void fireTerminationEvent(IterationEvent e)
		{
			foreach (IterationListener l in listeners)
			{
				l.terminationPerformed(e);
			}
		}

		/// <summary>
		/// Returns the number of iterations of this solver, 0 if no iterations has
		/// been performed yet.
		/// </summary>
		/// <returns> the number of iterations. </returns>
		public virtual int Iterations
		{
			get
			{
				return iterations.Count;
			}
		}

		/// <summary>
		/// Returns the maximum number of iterations.
		/// </summary>
		/// <returns> the maximum number of iterations. </returns>
		public virtual int MaxIterations
		{
			get
			{
				return iterations.MaximalCount;
			}
		}

		/// <summary>
		/// Increments the iteration count by one, and throws an exception if the
		/// maximum number of iterations is reached. This method should be called at
		/// the beginning of a new iteration.
		/// </summary>
		/// <exception cref="MaxCountExceededException"> if the maximum number of iterations is
		/// reached. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementIterationCount() throws mathlib.exception.MaxCountExceededException
		public virtual void incrementIterationCount()
		{
			iterations.incrementCount();
		}

		/// <summary>
		/// Removes the specified iteration listener from the list of listeners
		/// currently attached to {@code this} object. Attempting to remove a
		/// listener which was <em>not</em> previously registered does not cause any
		/// error.
		/// </summary>
		/// <param name="listener"> The <seealso cref="IterationListener"/> to be removed. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void removeIterationListener(final IterationListener listener)
		public virtual void removeIterationListener(IterationListener listener)
		{
			listeners.Remove(listener);
		}

		/// <summary>
		/// Sets the iteration count to 0. This method must be called during the
		/// initial phase.
		/// </summary>
		public virtual void resetIterationCount()
		{
			iterations.resetCount();
		}
	}

}