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
namespace org.apache.commons.math3.geometry.euclidean.twod.hull
{

	using InsufficientDataException = org.apache.commons.math3.exception.InsufficientDataException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using org.apache.commons.math3.geometry.hull;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// This class represents a convex hull in an two-dimensional euclidean space.
	/// 
	/// @version $Id: ConvexHull2D.java 1568752 2014-02-16 12:19:51Z tn $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class ConvexHull2D : ConvexHull<Euclidean2D, Vector2D>
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20140129L;

		/// <summary>
		/// Vertices of the hull. </summary>
		private readonly Vector2D[] vertices;

		/// <summary>
		/// Tolerance threshold used during creation of the hull vertices. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Line segments of the hull.
		/// The array is not serialized and will be created from the vertices on first access.
		/// </summary>
		[NonSerialized]
		private Segment[] lineSegments;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="vertices"> the vertices of the convex hull, must be ordered </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathIllegalArgumentException"> if the vertices do not form a convex hull </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ConvexHull2D(final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[] vertices, final double tolerance) throws org.apache.commons.math3.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public ConvexHull2D(Vector2D[] vertices, double tolerance)
		{

			if (!isConvex(vertices))
			{
				throw new MathIllegalArgumentException(LocalizedFormats.NOT_CONVEX);
			}

			this.vertices = vertices.clone();
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Checks whether the given hull vertices form a convex hull. </summary>
		/// <param name="hullVertices"> the hull vertices </param>
		/// <returns> {@code true} if the vertices form a convex hull, {@code false} otherwise </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private boolean isConvex(final org.apache.commons.math3.geometry.euclidean.twod.Vector2D[] hullVertices)
		private bool isConvex(Vector2D[] hullVertices)
		{
			if (hullVertices.Length < 3)
			{
				return true;
			}

			double sign = 0.0;
			for (int i = 0; i < hullVertices.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D p1 = hullVertices[i == 0 ? hullVertices.length - 1 : i - 1];
				Vector2D p1 = hullVertices[i == 0 ? hullVertices.Length - 1 : i - 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D p2 = hullVertices[i];
				Vector2D p2 = hullVertices[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D p3 = hullVertices[i == hullVertices.length - 1 ? 0 : i + 1];
				Vector2D p3 = hullVertices[i == hullVertices.Length - 1 ? 0 : i + 1];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D d1 = p2.subtract(p1);
				Vector2D d1 = p2.subtract(p1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D d2 = p3.subtract(p2);
				Vector2D d2 = p3.subtract(p2);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cross = org.apache.commons.math3.util.FastMath.signum(org.apache.commons.math3.util.MathArrays.linearCombination(d1.getX(), d2.getY(), -d1.getY(), d2.getX()));
				double cross = FastMath.signum(MathArrays.linearCombination(d1.X, d2.Y, -d1.Y, d2.X));
				// in case of collinear points the cross product will be zero
				if (cross != 0.0)
				{
					if (sign != 0.0 && cross != sign)
					{
						return false;
					}
					sign = cross;
				}
			}

			return true;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D[] Vertices
		{
			get
			{
				return vertices.clone();
			}
		}

		/// <summary>
		/// Get the line segments of the convex hull, ordered. </summary>
		/// <returns> the line segments of the convex hull </returns>
		public virtual Segment[] LineSegments
		{
			get
			{
				return retrieveLineSegments().clone();
			}
		}

		/// <summary>
		/// Retrieve the line segments from the cached array or create them if needed.
		/// </summary>
		/// <returns> the array of line segments </returns>
		private Segment[] retrieveLineSegments()
		{
			if (lineSegments == null)
			{
				// construct the line segments - handle special cases of 1 or 2 points
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int size = vertices.length;
				int size = vertices.Length;
				if (size <= 1)
				{
					this.lineSegments = new Segment[0];
				}
				else if (size == 2)
				{
					this.lineSegments = new Segment[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D p1 = vertices[0];
					Vector2D p1 = vertices[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Vector2D p2 = vertices[1];
					Vector2D p2 = vertices[1];
					this.lineSegments[0] = new Segment(p1, p2, new Line(p1, p2, tolerance));
				}
				else
				{
					this.lineSegments = new Segment[size];
					Vector2D firstPoint = null;
					Vector2D lastPoint = null;
					int index = 0;
					foreach (Vector2D point in vertices)
					{
						if (lastPoint == null)
						{
							firstPoint = point;
							lastPoint = point;
						}
						else
						{
							this.lineSegments[index++] = new Segment(lastPoint, point, new Line(lastPoint, point, tolerance));
							lastPoint = point;
						}
					}
					this.lineSegments[index] = new Segment(lastPoint, firstPoint, new Line(lastPoint, firstPoint, tolerance));
				}
			}
			return lineSegments;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.Region<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> createRegion() throws org.apache.commons.math3.exception.InsufficientDataException
		public virtual Region<Euclidean2D> createRegion()
		{
			if (vertices.Length < 3)
			{
				throw new InsufficientDataException();
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.partitioning.RegionFactory<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D> factory = new org.apache.commons.math3.geometry.partitioning.RegionFactory<org.apache.commons.math3.geometry.euclidean.twod.Euclidean2D>();
			RegionFactory<Euclidean2D> factory = new RegionFactory<Euclidean2D>();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Segment[] segments = retrieveLineSegments();
			Segment[] segments = retrieveLineSegments();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.geometry.euclidean.twod.Line[] lineArray = new org.apache.commons.math3.geometry.euclidean.twod.Line[segments.length];
			Line[] lineArray = new Line[segments.Length];
			for (int i = 0; i < segments.Length; i++)
			{
				lineArray[i] = segments[i].Line;
			}
			return factory.buildConvex(lineArray);
		}
	}

}