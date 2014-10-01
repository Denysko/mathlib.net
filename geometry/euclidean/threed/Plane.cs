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
namespace mathlib.geometry.euclidean.threed
{

	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.geometry;
	using mathlib.geometry;
	using Euclidean1D = mathlib.geometry.euclidean.oned.Euclidean1D;
	using Vector1D = mathlib.geometry.euclidean.oned.Vector1D;
	using Euclidean2D = mathlib.geometry.euclidean.twod.Euclidean2D;
	using PolygonsSet = mathlib.geometry.euclidean.twod.PolygonsSet;
	using Vector2D = mathlib.geometry.euclidean.twod.Vector2D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// The class represent planes in a three dimensional space.
	/// @version $Id: Plane.java 1560115 2014-01-21 17:49:13Z luc $
	/// @since 3.0
	/// </summary>
	public class Plane : Hyperplane<Euclidean3D>, Embedding<Euclidean3D, Euclidean2D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Offset of the origin with respect to the plane. </summary>
		private double originOffset;

		/// <summary>
		/// Origin of the plane frame. </summary>
		private Vector3D origin;

		/// <summary>
		/// First vector of the plane frame (in plane). </summary>
		private Vector3D u;

		/// <summary>
		/// Second vector of the plane frame (in plane). </summary>
		private Vector3D v;

		/// <summary>
		/// Third vector of the plane frame (plane normal). </summary>
		private Vector3D w;

		/// <summary>
		/// Tolerance below which points are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Build a plane normal to a given direction and containing the origin. </summary>
		/// <param name="normal"> normal direction to the plane </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathArithmeticException"> if the normal norm is too small
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Plane(final Vector3D normal, final double tolerance) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Plane(Vector3D normal, double tolerance)
		{
			Normal = normal;
			this.tolerance = tolerance;
			originOffset = 0;
			setFrame();
		}

		/// <summary>
		/// Build a plane from a point and a normal. </summary>
		/// <param name="p"> point belonging to the plane </param>
		/// <param name="normal"> normal direction to the plane </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathArithmeticException"> if the normal norm is too small
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Plane(final Vector3D p, final Vector3D normal, final double tolerance) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Plane(Vector3D p, Vector3D normal, double tolerance)
		{
			Normal = normal;
			this.tolerance = tolerance;
			originOffset = -p.dotProduct(w);
			setFrame();
		}

		/// <summary>
		/// Build a plane from three points.
		/// <p>The plane is oriented in the direction of
		/// {@code (p2-p1) ^ (p3-p1)}</p> </summary>
		/// <param name="p1"> first point belonging to the plane </param>
		/// <param name="p2"> second point belonging to the plane </param>
		/// <param name="p3"> third point belonging to the plane </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathArithmeticException"> if the points do not constitute a plane
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Plane(final Vector3D p1, final Vector3D p2, final Vector3D p3, final double tolerance) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Plane(Vector3D p1, Vector3D p2, Vector3D p3, double tolerance) : this(p1, p2.subtract(p1).crossProduct(p3.subtract(p1)), tolerance)
		{
		}

		/// <summary>
		/// Build a plane normal to a given direction and containing the origin. </summary>
		/// <param name="normal"> normal direction to the plane </param>
		/// <exception cref="MathArithmeticException"> if the normal norm is too small </exception>
		/// @deprecated as of 3.3, replaced with <seealso cref="#Plane(Vector3D, double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#Plane(Vector3D, double)"/>") public Plane(final Vector3D normal) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#Plane(Vector3D, double)"/>")]
		public Plane(Vector3D normal) : this(normal, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a plane from a point and a normal. </summary>
		/// <param name="p"> point belonging to the plane </param>
		/// <param name="normal"> normal direction to the plane </param>
		/// <exception cref="MathArithmeticException"> if the normal norm is too small </exception>
		/// @deprecated as of 3.3, replaced with <seealso cref="#Plane(Vector3D, Vector3D, double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#Plane(Vector3D, Vector3D, double)"/>") public Plane(final Vector3D p, final Vector3D normal) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#Plane(Vector3D, Vector3D, double)"/>")]
		public Plane(Vector3D p, Vector3D normal) : this(p, normal, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a plane from three points.
		/// <p>The plane is oriented in the direction of
		/// {@code (p2-p1) ^ (p3-p1)}</p> </summary>
		/// <param name="p1"> first point belonging to the plane </param>
		/// <param name="p2"> second point belonging to the plane </param>
		/// <param name="p3"> third point belonging to the plane </param>
		/// <exception cref="MathArithmeticException"> if the points do not constitute a plane </exception>
		/// @deprecated as of 3.3, replaced with <seealso cref="#Plane(Vector3D, Vector3D, Vector3D, double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#Plane(Vector3D, Vector3D, Vector3D, double)"/>") public Plane(final Vector3D p1, final Vector3D p2, final Vector3D p3) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#Plane(Vector3D, Vector3D, Vector3D, double)"/>")]
		public Plane(Vector3D p1, Vector3D p2, Vector3D p3) : this(p1, p2, p3, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// <p>The instance created is completely independant of the original
		/// one. A deep copy is used, none of the underlying object are
		/// shared.</p> </summary>
		/// <param name="plane"> plane to copy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Plane(final Plane plane)
		public Plane(Plane plane)
		{
			originOffset = plane.originOffset;
			origin = plane.origin;
			u = plane.u;
			v = plane.v;
			w = plane.w;
			tolerance = plane.tolerance;
		}

		/// <summary>
		/// Copy the instance.
		/// <p>The instance created is completely independant of the original
		/// one. A deep copy is used, none of the underlying objects are
		/// shared (except for immutable objects).</p> </summary>
		/// <returns> a new hyperplane, copy of the instance </returns>
		public virtual Plane copySelf()
		{
			return new Plane(this);
		}

		/// <summary>
		/// Reset the instance as if built from a point and a normal. </summary>
		/// <param name="p"> point belonging to the plane </param>
		/// <param name="normal"> normal direction to the plane </param>
		/// <exception cref="MathArithmeticException"> if the normal norm is too small </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset(final Vector3D p, final Vector3D normal) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void reset(Vector3D p, Vector3D normal)
		{
			Normal = normal;
			originOffset = -p.dotProduct(w);
			setFrame();
		}

		/// <summary>
		/// Reset the instance from another one.
		/// <p>The updated instance is completely independant of the original
		/// one. A deep reset is used none of the underlying object is
		/// shared.</p> </summary>
		/// <param name="original"> plane to reset from </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void reset(final Plane original)
		public virtual void reset(Plane original)
		{
			originOffset = original.originOffset;
			origin = original.origin;
			u = original.u;
			v = original.v;
			w = original.w;
		}

		/// <summary>
		/// Set the normal vactor. </summary>
		/// <param name="normal"> normal direction to the plane (will be copied) </param>
		/// <exception cref="MathArithmeticException"> if the normal norm is too small </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void setNormal(final Vector3D normal) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private Vector3D Normal
		{
			set
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double norm = value.getNorm();
				double norm = value.Norm;
				if (norm < 1.0e-10)
				{
					throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
				}
				w = new Vector3D(1.0 / norm, value);
			}
			get
			{
				return w;
			}
		}

		/// <summary>
		/// Reset the plane frame.
		/// </summary>
		private void setFrame()
		{
			origin = new Vector3D(-originOffset, w);
			u = w.orthogonal();
			v = Vector3D.crossProduct(w, u);
		}

		/// <summary>
		/// Get the origin point of the plane frame.
		/// <p>The point returned is the orthogonal projection of the
		/// 3D-space origin in the plane.</p> </summary>
		/// <returns> the origin point of the plane frame (point closest to the
		/// 3D-space origin) </returns>
		public virtual Vector3D Origin
		{
			get
			{
				return origin;
			}
		}


		/// <summary>
		/// Get the plane first canonical vector.
		/// <p>The frame defined by (<seealso cref="#getU getU"/>, <seealso cref="#getV getV"/>,
		/// <seealso cref="#getNormal getNormal"/>) is a rigth-handed orthonormalized
		/// frame).</p> </summary>
		/// <returns> normalized first canonical vector </returns>
		/// <seealso cref= #getV </seealso>
		/// <seealso cref= #getNormal </seealso>
		public virtual Vector3D U
		{
			get
			{
				return u;
			}
		}

		/// <summary>
		/// Get the plane second canonical vector.
		/// <p>The frame defined by (<seealso cref="#getU getU"/>, <seealso cref="#getV getV"/>,
		/// <seealso cref="#getNormal getNormal"/>) is a rigth-handed orthonormalized
		/// frame).</p> </summary>
		/// <returns> normalized second canonical vector </returns>
		/// <seealso cref= #getU </seealso>
		/// <seealso cref= #getNormal </seealso>
		public virtual Vector3D V
		{
			get
			{
				return v;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
		public virtual Point<Euclidean3D> project(Point<Euclidean3D> point)
		{
			return toSpace(toSubSpace(point));
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
		public virtual double Tolerance
		{
			get
			{
				return tolerance;
			}
		}

		/// <summary>
		/// Revert the plane.
		/// <p>Replace the instance by a similar plane with opposite orientation.</p>
		/// <p>The new plane frame is chosen in such a way that a 3D point that had
		/// {@code (x, y)} in-plane coordinates and {@code z} offset with
		/// respect to the plane and is unaffected by the change will have
		/// {@code (y, x)} in-plane coordinates and {@code -z} offset with
		/// respect to the new plane. This means that the {@code u} and {@code v}
		/// vectors returned by the <seealso cref="#getU"/> and <seealso cref="#getV"/> methods are exchanged,
		/// and the {@code w} vector returned by the <seealso cref="#getNormal"/> method is
		/// reversed.</p>
		/// </summary>
		public virtual void revertSelf()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D tmp = u;
			Vector3D tmp = u;
			u = v;
			v = tmp;
			w = w.negate();
			originOffset = -originOffset;
		}

		/// <summary>
		/// Transform a space point into a sub-space point. </summary>
		/// <param name="vector"> n-dimension point of the space </param>
		/// <returns> (n-1)-dimension point of the sub-space corresponding to
		/// the specified space point </returns>
		public virtual Vector2D toSubSpace(Vector<Euclidean3D> vector)
		{
			return toSubSpace((Point<Euclidean3D>) vector);
		}

		/// <summary>
		/// Transform a sub-space point into a space point. </summary>
		/// <param name="vector"> (n-1)-dimension point of the sub-space </param>
		/// <returns> n-dimension point of the space corresponding to the
		/// specified sub-space point </returns>
		public virtual Vector3D toSpace(Vector<Euclidean2D> vector)
		{
			return toSpace((Point<Euclidean2D>) vector);
		}

		/// <summary>
		/// Transform a 3D space point into an in-plane point. </summary>
		/// <param name="point"> point of the space (must be a {@link Vector3D
		/// Vector3D} instance) </param>
		/// <returns> in-plane point (really a {@link
		/// mathlib.geometry.euclidean.twod.Vector2D Vector2D} instance) </returns>
		/// <seealso cref= #toSpace </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.euclidean.twod.Vector2D toSubSpace(final mathlib.geometry.Point<Euclidean3D> point)
		public virtual Vector2D toSubSpace(Point<Euclidean3D> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D p3D = (Vector3D) point;
			Vector3D p3D = (Vector3D) point;
			return new Vector2D(p3D.dotProduct(u), p3D.dotProduct(v));
		}

		/// <summary>
		/// Transform an in-plane point into a 3D space point. </summary>
		/// <param name="point"> in-plane point (must be a {@link
		/// mathlib.geometry.euclidean.twod.Vector2D Vector2D} instance) </param>
		/// <returns> 3D space point (really a <seealso cref="Vector3D Vector3D"/> instance) </returns>
		/// <seealso cref= #toSubSpace </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D toSpace(final mathlib.geometry.Point<mathlib.geometry.euclidean.twod.Euclidean2D> point)
		public virtual Vector3D toSpace(Point<Euclidean2D> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.twod.Vector2D p2D = (mathlib.geometry.euclidean.twod.Vector2D) point;
			Vector2D p2D = (Vector2D) point;
			return new Vector3D(p2D.X, u, p2D.Y, v, -originOffset, w);
		}

		/// <summary>
		/// Get one point from the 3D-space. </summary>
		/// <param name="inPlane"> desired in-plane coordinates for the point in the
		/// plane </param>
		/// <param name="offset"> desired offset for the point </param>
		/// <returns> one point in the 3D-space, with given coordinates and offset
		/// relative to the plane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D getPointAt(final mathlib.geometry.euclidean.twod.Vector2D inPlane, final double offset)
		public virtual Vector3D getPointAt(Vector2D inPlane, double offset)
		{
			return new Vector3D(inPlane.X, u, inPlane.Y, v, offset - originOffset, w);
		}

		/// <summary>
		/// Check if the instance is similar to another plane.
		/// <p>Planes are considered similar if they contain the same
		/// points. This does not mean they are equal since they can have
		/// opposite normals.</p> </summary>
		/// <param name="plane"> plane to which the instance is compared </param>
		/// <returns> true if the planes are similar </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isSimilarTo(final Plane plane)
		public virtual bool isSimilarTo(Plane plane)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double angle = Vector3D.angle(w, plane.w);
			double angle = Vector3D.angle(w, plane.w);
			return ((angle < 1.0e-10) && (FastMath.abs(originOffset - plane.originOffset) < 1.0e-10)) || ((angle > (FastMath.PI - 1.0e-10)) && (FastMath.abs(originOffset + plane.originOffset) < 1.0e-10));
		}

		/// <summary>
		/// Rotate the plane around the specified point.
		/// <p>The instance is not modified, a new instance is created.</p> </summary>
		/// <param name="center"> rotation center </param>
		/// <param name="rotation"> vectorial rotation operator </param>
		/// <returns> a new plane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Plane rotate(final Vector3D center, final Rotation rotation)
		public virtual Plane rotate(Vector3D center, Rotation rotation)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D delta = origin.subtract(center);
			Vector3D delta = origin.subtract(center);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane plane = new Plane(center.add(rotation.applyTo(delta)), rotation.applyTo(w), tolerance);
			Plane plane = new Plane(center.add(rotation.applyTo(delta)), rotation.applyTo(w), tolerance);

			// make sure the frame is transformed as desired
			plane.u = rotation.applyTo(u);
			plane.v = rotation.applyTo(v);

			return plane;

		}

		/// <summary>
		/// Translate the plane by the specified amount.
		/// <p>The instance is not modified, a new instance is created.</p> </summary>
		/// <param name="translation"> translation to apply </param>
		/// <returns> a new plane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Plane translate(final Vector3D translation)
		public virtual Plane translate(Vector3D translation)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Plane plane = new Plane(origin.add(translation), w, tolerance);
			Plane plane = new Plane(origin.add(translation), w, tolerance);

			// make sure the frame is transformed as desired
			plane.u = u;
			plane.v = v;

			return plane;

		}

		/// <summary>
		/// Get the intersection of a line with the instance. </summary>
		/// <param name="line"> line intersecting the instance </param>
		/// <returns> intersection point between between the line and the
		/// instance (null if the line is parallel to the instance) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D intersection(final Line line)
		public virtual Vector3D intersection(Line line)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D direction = line.getDirection();
			Vector3D direction = line.Direction;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dot = w.dotProduct(direction);
			double dot = w.dotProduct(direction);
			if (FastMath.abs(dot) < 1.0e-10)
			{
				return null;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D point = line.toSpace((mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D>) mathlib.geometry.euclidean.oned.Vector1D.ZERO);
			Vector3D point = line.toSpace((Point<Euclidean1D>) Vector1D.ZERO);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double k = -(originOffset + w.dotProduct(point)) / dot;
			double k = -(originOffset + w.dotProduct(point)) / dot;
			return new Vector3D(1.0, point, k, direction);
		}

		/// <summary>
		/// Build the line shared by the instance and another plane. </summary>
		/// <param name="other"> other plane </param>
		/// <returns> line at the intersection of the instance and the
		/// other plane (really a <seealso cref="Line Line"/> instance) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Line intersection(final Plane other)
		public virtual Line intersection(Plane other)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D direction = Vector3D.crossProduct(w, other.w);
			Vector3D direction = Vector3D.crossProduct(w, other.w);
			if (direction.Norm < 1.0e-10)
			{
				return null;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D point = intersection(this, other, new Plane(direction, tolerance));
			Vector3D point = intersection(this, other, new Plane(direction, tolerance));
			return new Line(point, point.add(direction), tolerance);
		}

		/// <summary>
		/// Get the intersection point of three planes. </summary>
		/// <param name="plane1"> first plane1 </param>
		/// <param name="plane2"> second plane2 </param>
		/// <param name="plane3"> third plane2 </param>
		/// <returns> intersection point of three planes, null if some planes are parallel </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Vector3D intersection(final Plane plane1, final Plane plane2, final Plane plane3)
		public static Vector3D intersection(Plane plane1, Plane plane2, Plane plane3)
		{

			// coefficients of the three planes linear equations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a1 = plane1.w.getX();
			double a1 = plane1.w.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b1 = plane1.w.getY();
			double b1 = plane1.w.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c1 = plane1.w.getZ();
			double c1 = plane1.w.Z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = plane1.originOffset;
			double d1 = plane1.originOffset;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a2 = plane2.w.getX();
			double a2 = plane2.w.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2 = plane2.w.getY();
			double b2 = plane2.w.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c2 = plane2.w.getZ();
			double c2 = plane2.w.Z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = plane2.originOffset;
			double d2 = plane2.originOffset;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a3 = plane3.w.getX();
			double a3 = plane3.w.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3 = plane3.w.getY();
			double b3 = plane3.w.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c3 = plane3.w.getZ();
			double c3 = plane3.w.Z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = plane3.originOffset;
			double d3 = plane3.originOffset;

			// direct Cramer resolution of the linear system
			// (this is still feasible for a 3x3 system)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a23 = b2 * c3 - b3 * c2;
			double a23 = b2 * c3 - b3 * c2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b23 = c2 * a3 - c3 * a2;
			double b23 = c2 * a3 - c3 * a2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c23 = a2 * b3 - a3 * b2;
			double c23 = a2 * b3 - a3 * b2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double determinant = a1 * a23 + b1 * b23 + c1 * c23;
			double determinant = a1 * a23 + b1 * b23 + c1 * c23;
			if (FastMath.abs(determinant) < 1.0e-10)
			{
				return null;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r = 1.0 / determinant;
			double r = 1.0 / determinant;
			return new Vector3D((-a23 * d1 - (c1 * b3 - c3 * b1) * d2 - (c2 * b1 - c1 * b2) * d3) * r, (-b23 * d1 - (c3 * a1 - c1 * a3) * d2 - (c1 * a2 - c2 * a1) * d3) * r, (-c23 * d1 - (b1 * a3 - b3 * a1) * d2 - (b2 * a1 - b1 * a2) * d3) * r);

		}

		/// <summary>
		/// Build a region covering the whole hyperplane. </summary>
		/// <returns> a region covering the whole hyperplane </returns>
		public virtual SubPlane wholeHyperplane()
		{
			return new SubPlane(this, new PolygonsSet(tolerance));
		}

		/// <summary>
		/// Build a region covering the whole space. </summary>
		/// <returns> a region containing the instance (really a {@link
		/// PolyhedronsSet PolyhedronsSet} instance) </returns>
		public virtual PolyhedronsSet wholeSpace()
		{
			return new PolyhedronsSet(tolerance);
		}

		/// <summary>
		/// Check if the instance contains a point. </summary>
		/// <param name="p"> point to check </param>
		/// <returns> true if p belongs to the plane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean contains(final Vector3D p)
		public virtual bool contains(Vector3D p)
		{
			return FastMath.abs(getOffset(p)) < 1.0e-10;
		}

		/// <summary>
		/// Get the offset (oriented distance) of a parallel plane.
		/// <p>This method should be called only for parallel planes otherwise
		/// the result is not meaningful.</p>
		/// <p>The offset is 0 if both planes are the same, it is
		/// positive if the plane is on the plus side of the instance and
		/// negative if it is on the minus side, according to its natural
		/// orientation.</p> </summary>
		/// <param name="plane"> plane to check </param>
		/// <returns> offset of the plane </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final Plane plane)
		public virtual double getOffset(Plane plane)
		{
			return originOffset + (sameOrientationAs(plane) ? - plane.originOffset : plane.originOffset);
		}

		/// <summary>
		/// Get the offset (oriented distance) of a vector. </summary>
		/// <param name="vector"> vector to check </param>
		/// <returns> offset of the vector </returns>
		public virtual double getOffset(Vector<Euclidean3D> vector)
		{
			return getOffset((Point<Euclidean3D>) vector);
		}

		/// <summary>
		/// Get the offset (oriented distance) of a point.
		/// <p>The offset is 0 if the point is on the underlying hyperplane,
		/// it is positive if the point is on one particular side of the
		/// hyperplane, and it is negative if the point is on the other side,
		/// according to the hyperplane natural orientation.</p> </summary>
		/// <param name="point"> point to check </param>
		/// <returns> offset of the point </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final mathlib.geometry.Point<Euclidean3D> point)
		public virtual double getOffset(Point<Euclidean3D> point)
		{
			return ((Vector3D) point).dotProduct(w) + originOffset;
		}

		/// <summary>
		/// Check if the instance has the same orientation as another hyperplane. </summary>
		/// <param name="other"> other hyperplane to check against the instance </param>
		/// <returns> true if the instance and the other hyperplane have
		/// the same orientation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean sameOrientationAs(final mathlib.geometry.partitioning.Hyperplane<Euclidean3D> other)
		public virtual bool sameOrientationAs(Hyperplane<Euclidean3D> other)
		{
			return (((Plane) other).w).dotProduct(w) > 0.0;
		}

	}

}