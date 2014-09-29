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

namespace mathlib.ml.neuralnet
{

	/// <summary>
	/// Defines neighbourhood types.
	/// 
	/// @version $Id: SquareNeighbourhood.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public enum SquareNeighbourhood
	{
		/// <summary>
		/// <a href="http://en.wikipedia.org/wiki/Von_Neumann_neighborhood"
		///  Von Neumann neighbourhood</a>: in two dimensions, each (internal)
		/// neuron has four neighbours.
		/// </summary>
		VON_NEUMANN,
		/// <summary>
		/// <a href="http://en.wikipedia.org/wiki/Moore_neighborhood"
		///  Moore neighbourhood</a>: in two dimensions, each (internal)
		/// neuron has eight neighbours.
		/// </summary>
		MOORE,
	}

}