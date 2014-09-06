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

	/// <summary>
	/// The listener interface for receiving events occurring in an iterative
	/// algorithm.
	/// 
	/// @version $Id: IterationListener.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface IterationListener : EventListener
	{
		/// <summary>
		/// Invoked after completion of the initial phase of the iterative algorithm
		/// (prior to the main iteration loop).
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
		void initializationPerformed(IterationEvent e);

		/// <summary>
		/// Invoked each time an iteration is completed (in the main iteration loop).
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
		void iterationPerformed(IterationEvent e);

		/// <summary>
		/// Invoked each time a new iteration is completed (in the main iteration
		/// loop).
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
		void iterationStarted(IterationEvent e);

		/// <summary>
		/// Invoked after completion of the operations which occur after breaking out
		/// of the main iteration loop.
		/// </summary>
		/// <param name="e"> The <seealso cref="IterationEvent"/> object. </param>
		void terminationPerformed(IterationEvent e);
	}

}