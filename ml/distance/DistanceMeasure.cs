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
namespace mathlib.ml.distance
{

	/// <summary>
	/// Interface for distance measures of n-dimensional vectors.
	/// 
	/// @version $Id: DistanceMeasure.java 1519184 2013-08-31 15:22:59Z tn $
	/// @since 3.2
	/// </summary>
	public interface DistanceMeasure : Serializable
	{

		/// <summary>
		/// Compute the distance between two n-dimensional vectors.
		/// <p>
		/// The two vectors are required to have the same dimension.
		/// </summary>
		/// <param name="a"> the first vector </param>
		/// <param name="b"> the second vector </param>
		/// <returns> the distance between the two vectors </returns>
		double compute(double[] a, double[] b);
	}

}