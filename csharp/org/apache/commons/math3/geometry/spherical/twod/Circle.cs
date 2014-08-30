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
namespace org.apache.commons.math3.geometry.spherical.twod
{

	using org.apache.commons.math3.geometry;
	using Rotation = org.apache.commons.math3.geometry.euclidean.threed.Rotation;
	using Vector3D = org.apache.commons.math3.geometry.euclidean.threed.Vector3D;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using org.apache.commons.math3.geometry.partitioning;
	using Arc = org.apache.commons.math3.geometry.spherical.oned.Arc;
	using ArcsSet = org.apache.commons.math3.geometry.spherical.oned.ArcsSet;
	using S1Point = org.apache.commons.math3.geometry.spherical.oned.S1Point;
	using Sphere1D = org.apache.commons.math3.geometry.spherical.oned.Sphere1D;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class represents an oriented great circle on the 2-sphere.
	/// 
	/// <p>An oriented circle can be defined by a center point. The circle
	/// is the the set of points that are in the normal plan the center.</p>
	/// 
	/// <p>Since it is oriented the two spherical caps at its two sides are
	/// unambiguously identified as a left cap and a right cap. This can be
	/// used to identify the interior and the exterior in a simple way by
	/// local properties only when part of a line is used to define part of
	/// a spherical polygon boundary.</p>
	/// 
	/// @version $Id: Circle.java 1560115 2014-01-21 17:49:13Z luc $
	/// @since 3.3
	/// </summary>
	public class Circle : Hyperplane<Sphere2D>, Embedding<Sphere2D, Sphere1D>
	{

		/// <summary>
		/// Pole or circle center. </summary>
		private Vector3D pole;

		/// <summary>
		/// First axis in the equator plane, origin of the phase angles. </summary>
		private Vector3D x;

		/// <summary>
		/// Second axis in the equator plane, in quadrature with respect to x. </summary>
		private Vector3D y;

		/// <summary>
		/// Tolerance below which close sub-arcs are merged together. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Build a great circle from its pole.
		/// <p>The circle is oriented in the trigonometric direction around pole.</p> </summary>
		/// <param name="pole"> circle pole </param>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Circle(final org.apache.commons.math3.geometry.euclidean.threed.Vector3D pole, final double tolerance)
		public Circle(Vector3D pole, double tolerance)
		{
			reset(pole);
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a great circle from two non-aligned points.
		/// <p>The circle is oriented from first to second point using the path smaller than \( \pi \).</p> </summary>
		/// <param name="first"> first point contained in the great circle </param>
		/// <param name="second"> second point contained in the great circle </param>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Circle(final S2Point first, final S2Point second, final double tolerance)
		public Circle(S2Point first, S2Point second, double tolerance)
		{
			reset(first.Vector.crossProduct(second.Vector));
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a circle from its internal components.
		/// <p>The circle is oriented in the trigonometric direction around center.</p> </summary>
		/// <param name="pole"> circle pole </param>
		/// <param name="x"> first axis in the equator plane </param>
		/// <param name="y"> second axis in the equator plane </param>
		/// <param name="tolerance"> tolerance below which close sub-arcs are merged together </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Circle(final org.apache.commons.math3.geometry.euclidean.threed.Vector3D pole, final org.apache.commons.math3.geometry.euclidean.threed.Vector3D x, final org.apache.commons.math3.geometry.euclidean.threed.Vector3D y, final double tolerance)
		private Circle(Vector3D pole, Vector3D x, Vector3D y, double tolerance)
		{
			this.pole = pole;
			this.x = x;
			this.y = y;
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Copy constructor.
		/// <p>The created instance is completely independent from the
		/// original instance, it is a deep copy.</p> </summary>
		/// <param name="circle"> circle to copy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Circle(final Circle circle)
		public Circle(Circle circle) : this(circle.pole, circle.x, circle.y, circle.tolerance)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Circle copySelf()
		{
			return new Circle(this);
		}

		/// <summary>
		/// Reset the instance as if built from a pole.
		/// <p>The circle is oriented in the trigonometric direction around pole.</p> </summary>
		/// <param name="newPole"> circle pole </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void reset(final org.apache.commons.math3.geometry.euclidean.threed.Vector3D newPole)
		public virtual void reset(Vector3D newPole)
		{
			this.pole = newPole.normalize();
			this.x = newPole.orthogonal();
			this.y = Vector3D.crossProduct(newPole, x).normalize();
		}

		/// <summary>
		/// Revert the instance.
		/// </summary>
		public virtual void revertSelf()
		{
			// x remains the same
			y = y.negate();
			pole = pole.negate();
		}

		/// <summary>
		/// Get the reverse of the instance.
		/// <p>Get a circle with reversed orientation with respect to the
		/// instance. A new object is built, the instance is untouched.</p> </summary>
		/// <returns> a new circle, with orientation opposite to the instance orientation </returns>
		public virtual Circle Reverse
		{
			get
			{
				return new Circle(pole.negate(), x, y.negate(), tolerance);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Point<Sphere2D> project(Point<Sphere2D> point)
		{
			return toSpace(toSubSpace(point));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Tolerance
		{
			get
			{
				return tolerance;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <seealso cref= #getPhase(Vector3D) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.spherical.oned.S1Point toSubSpace(final org.apache.commons.math3.geometry.Point<Sphere2D> point)
		public virtual S1Point toSubSpace(Point<Sphere2D> point)
		{
			return new S1Point(getPhase(((S2Point) point).Vector));
		}

		/// <summary>
		/// Get the phase angle of a direction.
		/// <p>
		/// The direction may not belong to the circle as the
		/// phase is computed for the meridian plane between the circle
		/// pole and the direction.
		/// </p> </summary>
		/// <param name="direction"> direction for which phase is requested </param>
		/// <returns> phase angle of the direction around the circle </returns>
		/// <seealso cref= #toSubSpace(Point) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getPhase(final org.apache.commons.math3.geometry.euclidean.threed.Vector3D direction)
		public virtual double getPhase(Vector3D direction)
		{
			return FastMath.PI + FastMath.atan2(-direction.dotProduct(y), -direction.dotProduct(x));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <seealso cref= #getPointAt(double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public S2Point toSpace(final org.apache.commons.math3.geometry.Point<org.apache.commons.math3.geometry.spherical.oned.Sphere1D> point)
		public virtual S2Point toSpace(Point<Sphere1D> point)
		{
			return new S2Point(getPointAt(((S1Point) point).Alpha));
		}

		/// <summary>
		/// Get a circle point from its phase around the circle. </summary>
		/// <param name="alpha"> phase around the circle </param>
		/// <returns> circle point on the sphere </returns>
		/// <seealso cref= #toSpace(Point) </seealso>
		/// <seealso cref= #getXAxis() </seealso>
		/// <seealso cref= #getYAxis() </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.euclidean.threed.Vector3D getPointAt(final double alpha)
		public virtual Vector3D getPointAt(double alpha)
		{
			return new Vector3D(FastMath.cos(alpha), x, FastMath.sin(alpha), y);
		}

		/// <summary>
		/// Get the X axis of the circle.
		/// <p>
		/// This method returns the same value as {@link #getPointAt(double)
		/// getPointAt(0.0)} but it does not do any computation and always
		/// return the same instance.
		/// </p> </summary>
		/// <returns> an arbitrary x axis on the circle </returns>
		/// <seealso cref= #getPointAt(double) </seealso>
		/// <seealso cref= #getYAxis() </seealso>
		/// <seealso cref= #getPole() </seealso>
		public virtual Vector3D XAxis
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Get the Y axis of the circle.
		/// <p>
		/// This method returns the same value as {@link #getPointAt(double)
		/// getPointAt(0.5 * FastMath.PI)} but it does not do any computation and always
		/// return the same instance.
		/// </p> </summary>
		/// <returns> an arbitrary y axis point on the circle </returns>
		/// <seealso cref= #getPointAt(double) </seealso>
		/// <seealso cref= #getXAxis() </seealso>
		/// <seealso cref= #getPole() </seealso>
		public virtual Vector3D YAxis
		{
			get
			{
				return y;
			}
		}

		/// <summary>
		/// Get the pole of the circle.
		/// <p>
		/// As the circle is a great circle, the pole does <em>not</em>
		/// belong to it.
		/// </p> </summary>
		/// <returns> pole of the circle </returns>
		/// <seealso cref= #getXAxis() </seealso>
		/// <seealso cref= #getYAxis() </seealso>
		public virtual Vector3D Pole
		{
			get
			{
				return pole;
			}
		}

		/// <summary>
		/// Get the arc of the instance that lies inside the other circle. </summary>
		/// <param name="other"> other circle </param>
		/// <returns> arc of the instance that lies inside the other circle </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.spherical.oned.Arc getInsideArc(final Circle other)
		public virtual Arc getInsideArc(Circle other)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = getPhase(other.pole);
			double alpha = getPhase(other.pole);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double halfPi = 0.5 * org.apache.commons.math3.util.FastMath.PI;
			double halfPi = 0.5 * FastMath.PI;
			return new Arc(alpha - halfPi, alpha + halfPi, tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual SubCircle wholeHyperplane()
		{
			return new SubCircle(this, new ArcsSet(tolerance));
		}

		/// <summary>
		/// Build a region covering the whole space. </summary>
		/// <returns> a region containing the instance (really a {@link
		/// SphericalPolygonsSet SphericalPolygonsSet} instance) </returns>
		public virtual SphericalPolygonsSet wholeSpace()
		{
			return new SphericalPolygonsSet(tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <seealso cref= #getOffset(Vector3D) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final org.apache.commons.math3.geometry.Point<Sphere2D> point)
		public virtual double getOffset(Point<Sphere2D> point)
		{
			return getOffset(((S2Point) point).Vector);
		}

		/// <summary>
		/// Get the offset (oriented distance) of a direction.
		/// <p>The offset is defined as the angular distance between the
		/// circle center and the direction minus the circle radius. It
		/// is therefore 0 on the circle, positive for directions outside of
		/// the cone delimited by the circle, and negative inside the cone.</p> </summary>
		/// <param name="direction"> direction to check </param>
		/// <returns> offset of the direction </returns>
		/// <seealso cref= #getOffset(Point) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final org.apache.commons.math3.geometry.euclidean.threed.Vector3D direction)
		public virtual double getOffset(Vector3D direction)
		{
			return Vector3D.angle(pole, direction) - 0.5 * FastMath.PI;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean sameOrientationAs(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Sphere2D> other)
		public virtual bool sameOrientationAs(Hyperplane<Sphere2D> other)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle otherC = (Circle) other;
			Circle otherC = (Circle) other;
			return Vector3D.dotProduct(pole, otherC.pole) >= 0.0;
		}

		/// <summary>
		/// Get a {@link org.apache.commons.math3.geometry.partitioning.Transform
		/// Transform} embedding a 3D rotation. </summary>
		/// <param name="rotation"> rotation to use </param>
		/// <returns> a new transform that can be applied to either {@link
		/// Point Point}, <seealso cref="Circle Line"/> or {@link
		/// org.apache.commons.math3.geometry.partitioning.SubHyperplane
		/// SubHyperplane} instances </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.geometry.partitioning.Transform<Sphere2D, org.apache.commons.math3.geometry.spherical.oned.Sphere1D> getTransform(final org.apache.commons.math3.geometry.euclidean.threed.Rotation rotation)
		public static Transform<Sphere2D, Sphere1D> getTransform(Rotation rotation)
		{
			return new CircleTransform(rotation);
		}

		/// <summary>
		/// Class embedding a 3D rotation. </summary>
		private class CircleTransform : Transform<Sphere2D, Sphere1D>
		{

			/// <summary>
			/// Underlying rotation. </summary>
			internal readonly Rotation rotation;

			/// <summary>
			/// Build a transform from a {@code Rotation}. </summary>
			/// <param name="rotation"> rotation to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public CircleTransform(final org.apache.commons.math3.geometry.euclidean.threed.Rotation rotation)
			public CircleTransform(Rotation rotation)
			{
				this.rotation = rotation;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public S2Point apply(final org.apache.commons.math3.geometry.Point<Sphere2D> point)
			public virtual S2Point apply(Point<Sphere2D> point)
			{
				return new S2Point(rotation.applyTo(((S2Point) point).Vector));
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Circle apply(final org.apache.commons.math3.geometry.partitioning.Hyperplane<Sphere2D> hyperplane)
			public virtual Circle apply(Hyperplane<Sphere2D> hyperplane)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Circle circle = (Circle) hyperplane;
				Circle circle = (Circle) hyperplane;
				return new Circle(rotation.applyTo(circle.pole), rotation.applyTo(circle.x), rotation.applyTo(circle.y), circle.tolerance);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.spherical.oned.Sphere1D> apply(final org.apache.commons.math3.geometry.partitioning.SubHyperplane<org.apache.commons.math3.geometry.spherical.oned.Sphere1D> sub, final org.apache.commons.math3.geometry.partitioning.Hyperplane<Sphere2D> original, final org.apache.commons.math3.geometry.partitioning.Hyperplane<Sphere2D> transformed)
			public virtual SubHyperplane<Sphere1D> apply(SubHyperplane<Sphere1D> sub, Hyperplane<Sphere2D> original, Hyperplane<Sphere2D> transformed)
			{
				// as the circle is rotated, the limit angles are rotated too
				return sub;
			}

		}

	}

}