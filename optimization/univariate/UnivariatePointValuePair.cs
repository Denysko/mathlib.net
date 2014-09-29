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

namespace mathlib.optimization.univariate
{

	/// <summary>
	/// This class holds a point and the value of an objective function at this
	/// point.
	/// This is a simple immutable container.
	/// 
	/// @version $Id: UnivariatePointValuePair.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0)."), Serializable]
	public class UnivariatePointValuePair
	{
		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 1003888396256744753L;
		/// <summary>
		/// Point. </summary>
		private readonly double point;
		/// <summary>
		/// Value of the objective function at the point. </summary>
		private readonly double value;

		/// <summary>
		/// Build a point/objective function value pair.
		/// </summary>
		/// <param name="point"> Point. </param>
		/// <param name="value"> Value of an objective function at the point </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public UnivariatePointValuePair(final double point, final double value)
		public UnivariatePointValuePair(double point, double value)
		{
			this.point = point;
			this.value = value;
		}

		/// <summary>
		/// Get the point.
		/// </summary>
		/// <returns> the point. </returns>
		public virtual double Point
		{
			get
			{
				return point;
			}
		}

		/// <summary>
		/// Get the value of the objective function.
		/// </summary>
		/// <returns> the stored value of the objective function. </returns>
		public virtual double Value
		{
			get
			{
				return value;
			}
		}
	}

}