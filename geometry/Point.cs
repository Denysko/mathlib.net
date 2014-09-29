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
namespace mathlib.geometry
{

	/// <summary>
	/// This interface represents a generic geometrical point. </summary>
	/// @param <S> Type of the space.
	/// @version $Id: Point.java 1554651 2014-01-01 17:27:48Z luc $ </param>
	/// <seealso cref= Space </seealso>
	/// <seealso cref= Vector
	/// @since 3.3 </seealso>
	public interface Point<S> : Serializable where S : Space
	{

		/// <summary>
		/// Get the space to which the point belongs. </summary>
		/// <returns> containing space </returns>
		Space Space {get;}

		/// <summary>
		/// Returns true if any coordinate of this point is NaN; false otherwise </summary>
		/// <returns>  true if any coordinate of this point is NaN; false otherwise </returns>
		bool NaN {get;}

		/// <summary>
		/// Compute the distance between the instance and another point. </summary>
		/// <param name="p"> second point </param>
		/// <returns> the distance between the instance and p </returns>
		double distance(Point<S> p);

	}

}