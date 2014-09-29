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
namespace mathlib.fitting
{

	/// <summary>
	/// This class is a simple container for weighted observed point in
	/// <seealso cref="CurveFitter curve fitting"/>.
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: WeightedObservedPoint.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
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
		/// Simple constructor.
		/// </summary>
		/// <param name="weight"> Weight of the measurement in the fitting process. </param>
		/// <param name="x"> Abscissa of the measurement. </param>
		/// <param name="y"> Ordinate of the measurement. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public WeightedObservedPoint(final double weight, final double x, final double y)
		public WeightedObservedPoint(double weight, double x, double y)
		{
			this.weight = weight;
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Gets the weight of the measurement in the fitting process.
		/// </summary>
		/// <returns> the weight of the measurement in the fitting process. </returns>
		public virtual double Weight
		{
			get
			{
				return weight;
			}
		}

		/// <summary>
		/// Gets the abscissa of the point.
		/// </summary>
		/// <returns> the abscissa of the point. </returns>
		public virtual double X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Gets the observed value of the function at x.
		/// </summary>
		/// <returns> the observed value of the function at x. </returns>
		public virtual double Y
		{
			get
			{
				return y;
			}
		}

	}


}