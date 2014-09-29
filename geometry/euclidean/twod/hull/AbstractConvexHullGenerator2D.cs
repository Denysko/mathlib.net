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
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Abstract base class for convex hull generators in the two-dimensional euclidean space.
	/// 
	/// @since 3.3
	/// @version $Id: AbstractConvexHullGenerator2D.java 1568752 2014-02-16 12:19:51Z tn $
	/// </summary>
	internal abstract class AbstractConvexHullGenerator2D : ConvexHullGenerator2D
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1e-10;

		/// <summary>
		/// Tolerance below which points are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Indicates if collinear points on the hull shall be present in the output.
		/// If {@code false}, only the extreme points are added to the hull.
		/// </summary>
		private readonly bool includeCollinearPoints;

		/// <summary>
		/// Simple constructor.
		/// <p>
		/// The default tolerance (1e-10) will be used to determine identical points.
		/// </summary>
		/// <param name="includeCollinearPoints"> indicates if collinear points on the hull shall be
		/// added as hull vertices </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractConvexHullGenerator2D(final boolean includeCollinearPoints)
		protected internal AbstractConvexHullGenerator2D(bool includeCollinearPoints) : this(includeCollinearPoints, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Simple constructor.
		/// </summary>
		/// <param name="includeCollinearPoints"> indicates if collinear points on the hull shall be
		/// added as hull vertices </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractConvexHullGenerator2D(final boolean includeCollinearPoints, final double tolerance)
		protected internal AbstractConvexHullGenerator2D(bool includeCollinearPoints, double tolerance)
		{
			this.includeCollinearPoints = includeCollinearPoints;
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Get the tolerance below which points are considered identical. </summary>
		/// <returns> the tolerance below which points are considered identical </returns>
		public virtual double Tolerance
		{
			get
			{
				return tolerance;
			}
		}

		/// <summary>
		/// Returns if collinear points on the hull will be added as hull vertices. </summary>
		/// <returns> {@code true} if collinear points are added as hull vertices, or {@code false}
		/// if only extreme points are present. </returns>
		public virtual bool IncludeCollinearPoints
		{
			get
			{
				return includeCollinearPoints;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConvexHull2D generate(final java.util.Collection<mathlib.geometry.euclidean.twod.Vector2D> points) throws mathlib.exception.NullArgumentException, mathlib.exception.ConvergenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual ConvexHull2D generate(ICollection<Vector2D> points)
		{
			// check for null points
			MathUtils.checkNotNull(points);

			ICollection<Vector2D> hullVertices = null;
			if (points.Count < 2)
			{
				hullVertices = points;
			}
			else
			{
				hullVertices = findHullVertices(points);
			}

			try
			{
				return new ConvexHull2D(hullVertices.toArray(new Vector2D[hullVertices.Count]), tolerance);
			}
			catch (MathIllegalArgumentException e)
			{
				// the hull vertices may not form a convex hull if the tolerance value is to large
				throw new ConvergenceException();
			}
		}

		/// <summary>
		/// Find the convex hull vertices from the set of input points. </summary>
		/// <param name="points"> the set of input points </param>
		/// <returns> the convex hull vertices in CCW winding </returns>
		protected internal abstract ICollection<Vector2D> findHullVertices(ICollection<Vector2D> points);

	}

}