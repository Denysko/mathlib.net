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

namespace mathlib.optimization.fitting
{

	/// <summary>
	/// This class is a simple container for weighted observed point in
	/// <seealso cref="CurveFitter curve fitting"/>.
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: WeightedObservedPoint.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 2.0 
	[Obsolete("As of 3.1 (to be removed in 4.0)."), Serializable]
	public class WeightedObservedPoint
	{

		/// <summary>
		/// Serializable version id. </summary>
		private const long serialVersionUID = 5306874947404636157L;

		/// <summary>
		/// Weight of the measurement in the fitting process. </summary>
		private readonly double weight;

		/// <summary>
		/// Abscissa of the point. </summary>
		private readonly double x;

		/// <summary>
		/// Observed value of the function at x. </summary>
		private readonly double y;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="weight"> weight of the measurement in the fitting process </param>
		/// <param name="x"> abscissa of the measurement </param>
		/// <param name="y"> ordinate of the measurement </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public WeightedObservedPoint(final double weight, final double x, final double y)
		public WeightedObservedPoint(double weight, double x, double y)
		{
			this.weight = weight;
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Get the weight of the measurement in the fitting process. </summary>
		/// <returns> weight of the measurement in the fitting process </returns>
		public virtual double Weight
		{
			get
			{
				return weight;
			}
		}

		/// <summary>
		/// Get the abscissa of the point. </summary>
		/// <returns> abscissa of the point </returns>
		public virtual double X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Get the observed value of the function at x. </summary>
		/// <returns> observed value of the function at x </returns>
		public virtual double Y
		{
			get
			{
				return y;
			}
		}

	}


}