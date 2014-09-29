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
namespace mathlib.geometry.euclidean.twod.hull
{

	using ConvergenceException = mathlib.exception.ConvergenceException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using mathlib.geometry.hull;

	/// <summary>
	/// Interface for convex hull generators in the two-dimensional euclidean space.
	/// 
	/// @since 3.3
	/// @version $Id: ConvexHullGenerator2D.java 1568752 2014-02-16 12:19:51Z tn $
	/// </summary>
	public interface ConvexHullGenerator2D : ConvexHullGenerator<Euclidean2D, Vector2D>
	{

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: ConvexHull2D generate(java.util.Collection<mathlib.geometry.euclidean.twod.Vector2D> points) throws mathlib.exception.NullArgumentException, mathlib.exception.ConvergenceException;
		ConvexHull2D generate(ICollection<Vector2D> points);

	}

}