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
namespace mathlib.geometry.hull
{

	using ConvergenceException = mathlib.exception.ConvergenceException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using mathlib.geometry;

	/// <summary>
	/// Interface for convex hull generators.
	/// </summary>
	/// @param <S> Type of the <seealso cref="Space"/> </param>
	/// @param <P> Type of the <seealso cref="Point"/>
	/// </param>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Convex_hull">Convex Hull (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/ConvexHull.html">Convex Hull (MathWorld)</a>
	/// 
	/// @since 3.3
	/// @version $Id: ConvexHullGenerator.java 1568752 2014-02-16 12:19:51Z tn $ </seealso>
	public interface ConvexHullGenerator<S, P> where S : mathlib.geometry.Space where P : mathlib.geometry.Point<S>
	{

		/// <summary>
		/// Builds the convex hull from the set of input points.
		/// </summary>
		/// <param name="points"> the set of input points </param>
		/// <returns> the convex hull </returns>
		/// <exception cref="NullArgumentException"> if the input collection is {@code null} </exception>
		/// <exception cref="ConvergenceException"> if generator fails to generate a convex hull for
		/// the given set of input points </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ConvexHull<S, P> generate(java.util.Collection<P> points) throws mathlib.exception.NullArgumentException, mathlib.exception.ConvergenceException;
		ConvexHull<S, P> generate(ICollection<P> points);
	}

}