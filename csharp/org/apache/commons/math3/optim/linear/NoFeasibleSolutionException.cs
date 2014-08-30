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
namespace org.apache.commons.math3.optim.linear
{

	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// This class represents exceptions thrown by optimizers when no solution fulfills the constraints.
	/// 
	/// @version $Id: NoFeasibleSolutionException.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 2.0
	/// </summary>
	public class NoFeasibleSolutionException : MathIllegalStateException
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -3044253632189082760L;

		/// <summary>
		/// Simple constructor using a default message.
		/// </summary>
		public NoFeasibleSolutionException() : base(LocalizedFormats.NO_FEASIBLE_SOLUTION)
		{
		}
	}

}