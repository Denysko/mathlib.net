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
namespace org.apache.commons.math3.ml.distance
{

	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Calculates the Earh Mover's distance (also known as Wasserstein metric) between two distributions.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Earth_mover's_distance">Earth Mover's distance (Wikipedia)</a>
	/// 
	/// @version $Id: EarthMoversDistance.java 1519185 2013-08-31 15:32:49Z tn $
	/// @since 3.3 </seealso>
	public class EarthMoversDistance : DistanceMeasure
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -5406732779747414922L;

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double compute(double[] a, double[] b)
		{
			double lastDistance = 0;
			double totalDistance = 0;
			for (int i = 0; i < a.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentDistance = (a[i] + lastDistance) - b[i];
				double currentDistance = (a[i] + lastDistance) - b[i];
				totalDistance += FastMath.abs(currentDistance);
				lastDistance = currentDistance;
			}
			return totalDistance;
		}
	}

}