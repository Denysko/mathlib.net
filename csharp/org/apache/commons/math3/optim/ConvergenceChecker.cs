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
	/// This interface specifies how to check if an optimization algorithm has
	/// converged.
	/// <br/>
	/// Deciding if convergence has been reached is a problem-dependent issue. The
	/// user should provide a class implementing this interface to allow the
	/// optimization algorithm to stop its search according to the problem at hand.
	/// <br/>
	/// For convenience, three implementations that fit simple needs are already
	/// provided: <seealso cref="SimpleValueChecker"/>, <seealso cref="SimpleVectorValueChecker"/> and
	/// <seealso cref="SimplePointChecker"/>. The first two consider that convergence is
	/// reached when the objective function value does not change much anymore, it
	/// does not use the point set at all.
	/// The third one considers that convergence is reached when the input point
	/// set does not change much anymore, it does not use objective function value
	/// at all.
	/// </summary>
	/// @param <PAIR> Type of the (point, objective value) pair.
	/// </param>
	/// <seealso cref= org.apache.commons.math3.optim.SimplePointChecker </seealso>
	/// <seealso cref= org.apache.commons.math3.optim.SimpleValueChecker </seealso>
	/// <seealso cref= org.apache.commons.math3.optim.SimpleVectorValueChecker
	/// 
	/// @version $Id: ConvergenceChecker.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.0 </seealso>
	public interface ConvergenceChecker<PAIR>
	{
		/// <summary>
		/// Check if the optimization algorithm has converged.
		/// </summary>
		/// <param name="iteration"> Current iteration. </param>
		/// <param name="previous"> Best point in the previous iteration. </param>
		/// <param name="current"> Best point in the current iteration. </param>
		/// <returns> {@code true} if the algorithm is considered to have converged. </returns>
		bool converged(int iteration, PAIR previous, PAIR current);
	}

}