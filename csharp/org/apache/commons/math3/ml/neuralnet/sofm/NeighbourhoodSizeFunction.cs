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

namespace org.apache.commons.math3.ml.neuralnet.sofm
{

	/// <summary>
	/// Provides the network neighbourhood's size as a function of the
	/// number of calls already performed during the learning task.
	/// The "neighbourhood" is the set of neurons that can be reached
	/// by traversing at most the number of links returned by this
	/// function.
	/// 
	/// @version $Id: NeighbourhoodSizeFunction.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public interface NeighbourhoodSizeFunction
	{
		/// <summary>
		/// Computes the neighbourhood size at the current call.
		/// </summary>
		/// <param name="numCall"> Current step of the training task. </param>
		/// <returns> the value of the function at {@code numCall}. </returns>
		int value(long numCall);
	}

}