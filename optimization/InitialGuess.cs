using System;

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

namespace mathlib.optimization
{

	/// <summary>
	/// Starting point (first guess) of the optimization procedure.
	/// <br/>
	/// Immutable class.
	/// 
	/// @version $Id: InitialGuess.java 1591835 2014-05-02 09:04:01Z tn $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.1 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class InitialGuess : OptimizationData
	{
		/// <summary>
		/// Initial guess. </summary>
		private readonly double[] init;

		/// <param name="startPoint"> Initial guess. </param>
		public InitialGuess(double[] startPoint)
		{
			init = startPoint.clone();
		}

		/// <summary>
		/// Gets the initial guess.
		/// </summary>
		/// <returns> the initial guess. </returns>
		public virtual double[] InitialGuess
		{
			get
			{
				return init.clone();
			}
		}
	}

}