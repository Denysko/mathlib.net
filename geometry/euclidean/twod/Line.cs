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
namespace mathlib.geometry.euclidean.twod
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.geometry;
	using mathlib.geometry;
	using Euclidean1D = mathlib.geometry.euclidean.oned.Euclidean1D;
	using IntervalsSet = mathlib.geometry.euclidean.oned.IntervalsSet;
	using OrientedPoint = mathlib.geometry.euclidean.oned.OrientedPoint;
	using Vector1D = mathlib.geometry.euclidean.oned.Vector1D;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using mathlib.geometry.partitioning;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// This class represents an oriented line in the 2D plane.
	/// 
	/// <p>An oriented line can be defined either by prolongating a line
	/// segment between two points past these points, or by one point and
	/// an angular direction (in trigonometric orientation).</p>
	/// 
	/// <p>Since it is oriented the two half planes at its two sides are
	/// unambiguously identified as a left half plane and a right half
	/// plane. This can be used to identify the interior and the exterior
	/// in a simple way by local properties only when part of a line is
	/// used to define part of a polygon boundary.</p>
	/// 
	/// <p>A line can also be used to completely define a reference frame
	/// in the plane. It is sufficient to select one specific point in the
	/// line (the orthogonal projection of the original reference frame on
	/// the line) and to use the unit vector in the line direction and the
	/// orthogonal vector oriented from left half plane to right half
	/// plane. We define two coordinates by the process, the
	/// <em>abscissa</em> along the line, and the <em>offset</em> across
	/// the line. All points of the plane are uniquely identified by these
	/// two coordinates. The line is the set of points at zero offset, the
	/// left half plane is the set of points with negative offsets and the
	/// right half plane is the set of points with positive offsets.</p>
	/// 
	/// @version $Id: Line.java 1560115 2014-01-21 17:49:13Z luc $
	/// @since 3.0
	/// </summary>
	public class Line : Hyperplane<Euclidean2D>, Embedding<Euclidean2D, Euclidean1D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Angle with respect to the abscissa axis. </summary>
		private double angle;

		/// <summary>
		/// Cosine of the line angle. </summary>
		private double cos;

		/// <summary>
		/// Sine of the line angle. </summary>
		private double sin;

		/// <summary>
		/// Offset of the frame origin. </summary>
		private double originOffset;

		/// <summary>
		/// Tolerance below which points are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Build a line from two points.
		/// <p>The line is oriented from p1 to p2</p> </summary>
		/// <param name="p1"> first point </param>
		/// <param name="p2"> second point </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Line(final Vector2D p1, final Vector2D p2, final double tolerance)
		public Line(Vector2D p1, Vector2D p2, double tolerance)
		{
			reset(p1, p2);
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a line from a point and an angle. </summary>
		/// <param name="p"> point belonging to the line </param>
		/// <param name="angle"> angle of the line with respect to abscissa axis </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Line(final Vector2D p, final double angle, final double tolerance)
		public Line(Vector2D p, double angle, double tolerance)
		{
			reset(p, angle);
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a line from its internal characteristics. </summary>
		/// <param name="angle"> angle of the line with respect to abscissa axis </param>
		/// <param name="cos"> cosine of the angle </param>
		/// <param name="sin"> sine of the angle </param>
		/// <param name="originOffset"> offset of the origin </param>
		/// <param name="tolerance"> tolerance below which points are considered identical
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Line(final double angle, final double cos, final double sin, final double originOffset, final double tolerance)
		private Line(double angle, double cos, double sin, double originOffset, double tolerance)
		{
			this.angle = angle;
			this.cos = cos;
			this.sin = sin;
			this.originOffset = originOffset;
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Build a line from two points.
		/// <p>The line is oriented from p1 to p2</p> </summary>
		/// <param name="p1"> first point </param>
		/// <param name="p2"> second point </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#Line(Vector2D, Vector2D, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#Line(Vector2D, Vector2D, double)"/>") public Line(final Vector2D p1, final Vector2D p2)
		[Obsolete("as of 3.3, replaced with <seealso cref="#Line(Vector2D, Vector2D, double)"/>")]
		public Line(Vector2D p1, Vector2D p2) : this(p1, p2, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Build a line from a point and an angle. </summary>
		/// <param name="p"> point belonging to the line </param>
		/// <param name="angle"> angle of the line with respect to abscissa axis </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#Line(Vector2D, double, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#Line(Vector2D, double, double)"/>") public Line(final Vector2D p, final double angle)
		[Obsolete("as of 3.3, replaced with <seealso cref="#Line(Vector2D, double, double)"/>")]
		public Line(Vector2D p, double angle) : this(p, angle, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// <p>The created instance is completely independent from the
		/// original instance, it is a deep copy.</p> </summary>
		/// <param name="line"> line to copy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Line(final Line line)
		public Line(Line line)
		{
			angle = MathUtils.normalizeAngle(line.angle, FastMath.PI);
			cos = FastMath.cos(angle);
			sin = FastMath.sin(angle);
			originOffset = line.originOffset;
			tolerance = line.tolerance;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Line copySelf()
		{
			return new Line(this);
		}

		/// <summary>
		/// Reset the instance as if built from two points.
		/// <p>The line is oriented from p1 to p2</p> </summary>
		/// <param name="p1"> first point </param>
		/// <param name="p2"> second point </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void reset(final Vector2D p1, final Vector2D p2)
		public virtual void reset(Vector2D p1, Vector2D p2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = p2.getX() - p1.getX();
			double dx = p2.X - p1.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = p2.getY() - p1.getY();
			double dy = p2.Y - p1.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = mathlib.util.FastMath.hypot(dx, dy);
			double d = FastMath.hypot(dx, dy);
			if (d == 0.0)
			{
				angle = 0.0;
				cos = 1.0;
				sin = 0.0;
				originOffset = p1.Y;
			}
			else
			{
				angle = FastMath.PI + FastMath.atan2(-dy, -dx);
				cos = FastMath.cos(angle);
				sin = FastMath.sin(angle);
				originOffset = (p2.X * p1.Y - p1.X * p2.Y) / d;
			}
		}

		/// <summary>
		/// Reset the instance as if built from a line and an angle. </summary>
		/// <param name="p"> point belonging to the line </param>
		/// <param name="alpha"> angle of the line with respect to abscissa axis </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void reset(final Vector2D p, final double alpha)
		public virtual void reset(Vector2D p, double alpha)
		{
			this.angle = MathUtils.normalizeAngle(alpha, FastMath.PI);
			cos = FastMath.cos(this.angle);
			sin = FastMath.sin(this.angle);
			originOffset = cos * p.Y - sin * p.X;
		}

		/// <summary>
		/// Revert the instance.
		/// </summary>
		public virtual void revertSelf()
		{
			if (angle < FastMath.PI)
			{
				angle += FastMath.PI;
			}
			else
			{
				angle -= FastMath.PI;
			}
			cos = -cos;
			sin = -sin;
			originOffset = -originOffset;
		}

		/// <summary>
		/// Get the reverse of the instance.
		/// <p>Get a line with reversed orientation with respect to the
		/// instance. A new object is built, the instance is untouched.</p> </summary>
		/// <returns> a new line, with orientation opposite to the instance orientation </returns>
		public virtual Line Reverse
		{
			get
			{
				return new Line((angle < FastMath.PI) ? (angle + FastMath.PI) : (angle - FastMath.PI), -cos, -sin, -originOffset, tolerance);
			}
		}

		/// <summary>
		/// Transform a space point into a sub-space point. </summary>
		/// <param name="vector"> n-dimension point of the space </param>
		/// <returns> (n-1)-dimension point of the sub-space corresponding to
		/// the specified space point </returns>
		public virtual Vector1D toSubSpace(Vector<Euclidean2D> vector)
		{
			return toSubSpace((Point<Euclidean2D>) vector);
		}

		/// <summary>
		/// Transform a sub-space point into a space point. </summary>
		/// <param name="vector"> (n-1)-dimension point of the sub-space </param>
		/// <returns> n-dimension point of the space corresponding to the
		/// specified sub-space point </returns>
		public virtual Vector2D toSpace(Vector<Euclidean1D> vector)
		{
			return toSpace((Point<Euclidean1D>) vector);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.euclidean.oned.Vector1D toSubSpace(final mathlib.geometry.Point<Euclidean2D> point)
		public virtual Vector1D toSubSpace(Point<Euclidean2D> point)
		{
			Vector2D p2 = (Vector2D) point;
			return new Vector1D(cos * p2.X + sin * p2.Y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2D toSpace(final mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D> point)
		public virtual Vector2D toSpace(Point<Euclidean1D> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double abscissa = ((mathlib.geometry.euclidean.oned.Vector1D) point).getX();
			double abscissa = ((Vector1D) point).X;
			return new Vector2D(abscissa * cos - originOffset * sin, abscissa * sin + originOffset * cos);
		}

		/// <summary>
		/// Get the intersection point of the instance and another line. </summary>
		/// <param name="other"> other line </param>
		/// <returns> intersection point of the instance and the other line
		/// or null if there are no intersection points </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2D intersection(final Line other)
		public virtual Vector2D intersection(Line other)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = sin * other.cos - other.sin * cos;
			double d = sin * other.cos - other.sin * cos;
			if (FastMath.abs(d) < tolerance)
			{
				return null;
			}
			return new Vector2D((cos * other.originOffset - other.cos * originOffset) / d, (sin * other.originOffset - other.sin * originOffset) / d);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
		public virtual Point<Euclidean2D> project(Point<Euclidean2D> point)
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
		/// {@inheritDoc} </summary>
		public virtual SubLine wholeHyperplane()
		{
			return new SubLine(this, new IntervalsSet(tolerance));
		}

		/// <summary>
		/// Build a region covering the whole space. </summary>
		/// <returns> a region containing the instance (really a {@link
		/// PolygonsSet PolygonsSet} instance) </returns>
		public virtual PolygonsSet wholeSpace()
		{
			return new PolygonsSet(tolerance);
		}

		/// <summary>
		/// Get the offset (oriented distance) of a parallel line.
		/// <p>This method should be called only for parallel lines otherwise
		/// the result is not meaningful.</p>
		/// <p>The offset is 0 if both lines are the same, it is
		/// positive if the line is on the right side of the instance and
		/// negative if it is on the left side, according to its natural
		/// orientation.</p> </summary>
		/// <param name="line"> line to check </param>
		/// <returns> offset of the line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final Line line)
		public virtual double getOffset(Line line)
		{
			return originOffset + ((cos * line.cos + sin * line.sin > 0) ? - line.originOffset : line.originOffset);
		}

		/// <summary>
		/// Get the offset (oriented distance) of a vector. </summary>
		/// <param name="vector"> vector to check </param>
		/// <returns> offset of the vector </returns>
		public virtual double getOffset(Vector<Euclidean2D> vector)
		{
			return getOffset((Point<Euclidean2D>) vector);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final mathlib.geometry.Point<Euclidean2D> point)
		public virtual double getOffset(Point<Euclidean2D> point)
		{
			Vector2D p2 = (Vector2D) point;
			return sin * p2.X - cos * p2.Y + originOffset;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean sameOrientationAs(final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> other)
		public virtual bool sameOrientationAs(Hyperplane<Euclidean2D> other)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line otherL = (Line) other;
			Line otherL = (Line) other;
			return (sin * otherL.sin + cos * otherL.cos) >= 0.0;
		}

		/// <summary>
		/// Get one point from the plane. </summary>
		/// <param name="abscissa"> desired abscissa for the point </param>
		/// <param name="offset"> desired offset for the point </param>
		/// <returns> one point in the plane, with given abscissa and offset
		/// relative to the line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2D getPointAt(final mathlib.geometry.euclidean.oned.Vector1D abscissa, final double offset)
		public virtual Vector2D getPointAt(Vector1D abscissa, double offset)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = abscissa.getX();
			double x = abscissa.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dOffset = offset - originOffset;
			double dOffset = offset - originOffset;
			return new Vector2D(x * cos + dOffset * sin, x * sin - dOffset * cos);
		}

		/// <summary>
		/// Check if the line contains a point. </summary>
		/// <param name="p"> point to check </param>
		/// <returns> true if p belongs to the line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean contains(final Vector2D p)
		public virtual bool contains(Vector2D p)
		{
			return FastMath.abs(getOffset(p)) < tolerance;
		}

		/// <summary>
		/// Compute the distance between the instance and a point.
		/// <p>This is a shortcut for invoking FastMath.abs(getOffset(p)),
		/// and provides consistency with what is in the
		/// mathlib.geometry.euclidean.threed.Line class.</p>
		/// </summary>
		/// <param name="p"> to check </param>
		/// <returns> distance between the instance and the point
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distance(final Vector2D p)
		public virtual double distance(Vector2D p)
		{
			return FastMath.abs(getOffset(p));
		}

		/// <summary>
		/// Check the instance is parallel to another line. </summary>
		/// <param name="line"> other line to check </param>
		/// <returns> true if the instance is parallel to the other line
		/// (they can have either the same or opposite orientations) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isParallelTo(final Line line)
		public virtual bool isParallelTo(Line line)
		{
			return FastMath.abs(sin * line.cos - cos * line.sin) < tolerance;
		}

		/// <summary>
		/// Translate the line to force it passing by a point. </summary>
		/// <param name="p"> point by which the line should pass </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void translateToPoint(final Vector2D p)
		public virtual void translateToPoint(Vector2D p)
		{
			originOffset = cos * p.Y - sin * p.X;
		}

		/// <summary>
		/// Get the angle of the line. </summary>
		/// <returns> the angle of the line with respect to the abscissa axis </returns>
		public virtual double Angle
		{
			get
			{
				return MathUtils.normalizeAngle(angle, FastMath.PI);
			}
			set
			{
				this.angle = MathUtils.normalizeAngle(value, FastMath.PI);
				cos = FastMath.cos(this.angle);
				sin = FastMath.sin(this.angle);
			}
		}

		/// <summary>
		/// Set the angle of the line. </summary>
		/// <param name="angle"> new angle of the line with respect to the abscissa axis </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setAngle(final double angle)

		/// <summary>
		/// Get the offset of the origin. </summary>
		/// <returns> the offset of the origin </returns>
		public virtual double OriginOffset
		{
			get
			{
				return originOffset;
			}
			set
			{
				originOffset = value;
			}
		}

		/// <summary>
		/// Set the offset of the origin. </summary>
		/// <param name="offset"> offset of the origin </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setOriginOffset(final double offset)

		/// <summary>
		/// Get a {@link mathlib.geometry.partitioning.Transform
		/// Transform} embedding an affine transform. </summary>
		/// <param name="transform"> affine transform to embed (must be inversible
		/// otherwise the {@link
		/// mathlib.geometry.partitioning.Transform#apply(Hyperplane)
		/// apply(Hyperplane)} method would work only for some lines, and
		/// fail for other ones) </param>
		/// <returns> a new transform that can be applied to either {@link
		/// Vector2D Vector2D}, <seealso cref="Line Line"/> or {@link
		/// mathlib.geometry.partitioning.SubHyperplane
		/// SubHyperplane} instances </returns>
		/// <exception cref="MathIllegalArgumentException"> if the transform is non invertible </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static mathlib.geometry.partitioning.Transform<Euclidean2D, mathlib.geometry.euclidean.oned.Euclidean1D> getTransform(final java.awt.geom.AffineTransform transform) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static Transform<Euclidean2D, Euclidean1D> getTransform(AffineTransform transform)
		{
			return new LineTransform(transform);
		}

		/// <summary>
		/// Class embedding an affine transform.
		/// <p>This class is used in order to apply an affine transform to a
		/// line. Using a specific object allow to perform some computations
		/// on the transform only once even if the same transform is to be
		/// applied to a large number of lines (for example to a large
		/// polygon)./<p>
		/// </summary>
		private class LineTransform : Transform<Euclidean2D, Euclidean1D>
		{

			// CHECKSTYLE: stop JavadocVariable check
			internal double cXX;
			internal double cXY;
			internal double cX1;
			internal double cYX;
			internal double cYY;
			internal double cY1;

			internal double c1Y;
			internal double c1X;
			internal double c11;
			// CHECKSTYLE: resume JavadocVariable check

			/// <summary>
			/// Build an affine line transform from a n {@code AffineTransform}. </summary>
			/// <param name="transform"> transform to use (must be invertible otherwise
			/// the <seealso cref="LineTransform#apply(Hyperplane)"/> method would work
			/// only for some lines, and fail for other ones) </param>
			/// <exception cref="MathIllegalArgumentException"> if the transform is non invertible </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LineTransform(final java.awt.geom.AffineTransform transform) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public LineTransform(AffineTransform transform)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] m = new double[6];
				double[] m = new double[6];
				transform.getMatrix(m);
				cXX = m[0];
				cXY = m[2];
				cX1 = m[4];
				cYX = m[1];
				cYY = m[3];
				cY1 = m[5];

				c1Y = cXY * cY1 - cYY * cX1;
				c1X = cXX * cY1 - cYX * cX1;
				c11 = cXX * cYY - cYX * cXY;

				if (FastMath.abs(c11) < 1.0e-20)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.NON_INVERTIBLE_TRANSFORM);
				}

			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2D apply(final mathlib.geometry.Point<Euclidean2D> point)
			public virtual Vector2D apply(Point<Euclidean2D> point)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D p2D = (Vector2D) point;
				Vector2D p2D = (Vector2D) point;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = p2D.getX();
				double x = p2D.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = p2D.getY();
				double y = p2D.Y;
				return new Vector2D(cXX * x + cXY * y + cX1, cYX * x + cYY * y + cY1);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Line apply(final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> hyperplane)
			public virtual Line apply(Hyperplane<Euclidean2D> hyperplane)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line line = (Line) hyperplane;
				Line line = (Line) hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rOffset = c1X * line.cos + c1Y * line.sin + c11 * line.originOffset;
				double rOffset = c1X * line.cos + c1Y * line.sin + c11 * line.originOffset;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rCos = cXX * line.cos + cXY * line.sin;
				double rCos = cXX * line.cos + cXY * line.sin;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rSin = cYX * line.cos + cYY * line.sin;
				double rSin = cYX * line.cos + cYY * line.sin;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inv = 1.0 / mathlib.util.FastMath.sqrt(rSin * rSin + rCos * rCos);
				double inv = 1.0 / FastMath.sqrt(rSin * rSin + rCos * rCos);
				return new Line(FastMath.PI + FastMath.atan2(-rSin, -rCos), inv * rCos, inv * rSin, inv * rOffset, line.tolerance);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.partitioning.SubHyperplane<mathlib.geometry.euclidean.oned.Euclidean1D> apply(final mathlib.geometry.partitioning.SubHyperplane<mathlib.geometry.euclidean.oned.Euclidean1D> sub, final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> original, final mathlib.geometry.partitioning.Hyperplane<Euclidean2D> transformed)
			public virtual SubHyperplane<Euclidean1D> apply(SubHyperplane<Euclidean1D> sub, Hyperplane<Euclidean2D> original, Hyperplane<Euclidean2D> transformed)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.oned.OrientedPoint op = (mathlib.geometry.euclidean.oned.OrientedPoint) sub.getHyperplane();
				OrientedPoint op = (OrientedPoint) sub.Hyperplane;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line originalLine = (Line) original;
				Line originalLine = (Line) original;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line transformedLine = (Line) transformed;
				Line transformedLine = (Line) transformed;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.geometry.euclidean.oned.Vector1D newLoc = transformedLine.toSubSpace(apply(originalLine.toSpace(op.getLocation())));
				Vector1D newLoc = transformedLine.toSubSpace(apply(originalLine.toSpace(op.Location)));
				return (new OrientedPoint(newLoc, op.Direct, originalLine.tolerance)).wholeHyperplane();
			}

		}

	}

}