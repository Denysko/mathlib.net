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
namespace mathlib.geometry.enclosing
{

	using mathlib.geometry;

	/// <summary>
	/// Interface for algorithms computing enclosing balls. </summary>
	/// @param <S> Space type. </param>
	/// @param <P> Point type.
	/// @version $Id: Encloser.java 1562882 2014-01-30 16:31:08Z luc $ </param>
	/// <seealso cref= EnclosingBall
	/// @since 3.3 </seealso>
	public interface Encloser<S, P> where S : mathlib.geometry.Space where P : mathlib.geometry.Point<S>
	{

		/// <summary>
		/// Find a ball enclosing a list of points. </summary>
		/// <param name="points"> points to enclose </param>
		/// <returns> enclosing ball </returns>
		EnclosingBall<S, P> enclose(IEnumerable<P> points);

	}

}