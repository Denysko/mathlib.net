using System.Collections.Generic;

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
	/// Interface for generating balls based on support points.
	/// <p>
	/// This generator is used in the <seealso cref="WelzlEncloser Emo Welzl"/> algorithm
	/// and its derivatives.
	/// </p> </summary>
	/// @param <S> Space type. </param>
	/// @param <P> Point type.
	/// @version $Id: SupportBallGenerator.java 1562220 2014-01-28 20:29:27Z luc $ </param>
	/// <seealso cref= EnclosingBall
	/// @since 3.3 </seealso>
	public interface SupportBallGenerator<S, P> where S : mathlib.geometry.Space where P : mathlib.geometry.Point<S>
	{

		/// <summary>
		/// Create a ball whose boundary lies on prescribed support points. </summary>
		/// <param name="support"> support points (may be empty) </param>
		/// <returns> ball whose boundary lies on the prescribed support points </returns>
		EnclosingBall<S, P> ballOnSupport(IList<P> support);

	}

}