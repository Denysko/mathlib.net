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

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.geometry;
	using mathlib.geometry;
	using Euclidean1D = mathlib.geometry.euclidean.oned.Euclidean1D;
	using IntervalsSet = mathlib.geometry.euclidean.oned.IntervalsSet;
	using Vector1D = mathlib.geometry.euclidean.oned.Vector1D;
	using mathlib.geometry.partitioning;
	using FastMath = mathlib.util.FastMath;
	using Precision = mathlib.util.Precision;

	/// <summary>
	/// The class represent lines in a three dimensional space.
	/// 
	/// <p>Each oriented line is intrinsically associated with an abscissa
	/// which is a coordinate on the line. The point at abscissa 0 is the
	/// orthogonal projection of the origin on the line, another equivalent
	/// way to express this is to say that it is the point of the line
	/// which is closest to the origin. Abscissa increases in the line
	/// direction.</p>
	/// 
	/// @version $Id: Line.java 1555176 2014-01-03 18:07:59Z luc $
	/// @since 3.0
	/// </summary>
	public class Line : Embedding<Euclidean3D, Euclidean1D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Line direction. </summary>
		private Vector3D direction;

		/// <summary>
		/// Line point closest to the origin. </summary>
		private Vector3D zero;

		/// <summary>
		/// Tolerance below which points are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Build a line from two points. </summary>
		/// <param name="p1"> first point belonging to the line (this can be any point) </param>
		/// <param name="p2"> second point belonging to the line (this can be any point, different from p1) </param>
		/// <param name="tolerance"> tolerance below which points are considered identical </param>
		/// <exception cref="MathIllegalArgumentException"> if the points are equal
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Line(final Vector3D p1, final Vector3D p2, final double tolerance) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Line(Vector3D p1, Vector3D p2, double tolerance)
		{
			reset(p1, p2);
			this.tolerance = tolerance;
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
			this.direction = line.direction;
			this.zero = line.zero;
			this.tolerance = line.tolerance;
		}

		/// <summary>
		/// Build a line from two points. </summary>
		/// <param name="p1"> first point belonging to the line (this can be any point) </param>
		/// <param name="p2"> second point belonging to the line (this can be any point, different from p1) </param>
		/// <exception cref="MathIllegalArgumentException"> if the points are equal </exception>
		/// @deprecated as of 3.3, replaced with <seealso cref="#Line(Vector3D, Vector3D, double)"/> 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#Line(Vector3D, Vector3D, double)"/>") public Line(final Vector3D p1, final Vector3D p2) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		[Obsolete]//("as of 3.3, replaced with <seealso cref="#Line(Vector3D, Vector3D, double)"/>")]
		public Line(Vector3D p1, Vector3D p2) : this(p1, p2, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Reset the instance as if built from two points. </summary>
		/// <param name="p1"> first point belonging to the line (this can be any point) </param>
		/// <param name="p2"> second point belonging to the line (this can be any point, different from p1) </param>
		/// <exception cref="MathIllegalArgumentException"> if the points are equal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reset(final Vector3D p1, final Vector3D p2) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void reset(Vector3D p1, Vector3D p2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D delta = p2.subtract(p1);
			Vector3D delta = p2.subtract(p1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm2 = delta.getNormSq();
			double norm2 = delta.NormSq;
			if (norm2 == 0.0)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.ZERO_NORM);
			}
			this.direction = new Vector3D(1.0 / FastMath.sqrt(norm2), delta);
			zero = new Vector3D(1.0, p1, -p1.dotProduct(delta) / norm2, delta);
		}

		/// <summary>
		/// Get the tolerance below which points are considered identical. </summary>
		/// <returns> tolerance below which points are considered identical
		/// @since 3.3 </returns>
		public virtual double Tolerance
		{
			get
			{
				return tolerance;
			}
		}

		/// <summary>
		/// Get a line with reversed direction. </summary>
		/// <returns> a new instance, with reversed direction </returns>
		public virtual Line revert()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Line reverted = new Line(this);
			Line reverted = new Line(this);
			reverted.direction = reverted.direction.negate();
			return reverted;
		}

		/// <summary>
		/// Get the normalized direction vector. </summary>
		/// <returns> normalized direction vector </returns>
		public virtual Vector3D Direction
		{
			get
			{
				return direction;
			}
		}

		/// <summary>
		/// Get the line point closest to the origin. </summary>
		/// <returns> line point closest to the origin </returns>
		public virtual Vector3D Origin
		{
			get
			{
				return zero;
			}
		}

		/// <summary>
		/// Get the abscissa of a point with respect to the line.
		/// <p>The abscissa is 0 if the projection of the point and the
		/// projection of the frame origin on the line are the same
		/// point.</p> </summary>
		/// <param name="point"> point to check </param>
		/// <returns> abscissa of the point </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getAbscissa(final Vector3D point)
		public virtual double getAbscissa(Vector3D point)
		{
			return point.subtract(zero).dotProduct(direction);
		}

		/// <summary>
		/// Get one point from the line. </summary>
		/// <param name="abscissa"> desired abscissa for the point </param>
		/// <returns> one point belonging to the line, at specified abscissa </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D pointAt(final double abscissa)
		public virtual Vector3D pointAt(double abscissa)
		{
			return new Vector3D(1.0, zero, abscissa, direction);
		}

		/// <summary>
		/// Transform a space point into a sub-space point. </summary>
		/// <param name="vector"> n-dimension point of the space </param>
		/// <returns> (n-1)-dimension point of the sub-space corresponding to
		/// the specified space point </returns>
		public virtual Vector1D toSubSpace(Vector<Euclidean3D> vector)
		{
			return toSubSpace((Point<Euclidean3D>) vector);
		}

		/// <summary>
		/// Transform a sub-space point into a space point. </summary>
		/// <param name="vector"> (n-1)-dimension point of the sub-space </param>
		/// <returns> n-dimension point of the space corresponding to the
		/// specified sub-space point </returns>
		public virtual Vector3D toSpace(Vector<Euclidean1D> vector)
		{
			return toSpace((Point<Euclidean1D>) vector);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <seealso cref= #getAbscissa(Vector3D) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.geometry.euclidean.oned.Vector1D toSubSpace(final mathlib.geometry.Point<Euclidean3D> point)
		public virtual Vector1D toSubSpace(Point<Euclidean3D> point)
		{
			return new Vector1D(getAbscissa((Vector3D) point));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <seealso cref= #pointAt(double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D toSpace(final mathlib.geometry.Point<mathlib.geometry.euclidean.oned.Euclidean1D> point)
		public virtual Vector3D toSpace(Point<Euclidean1D> point)
		{
			return pointAt(((Vector1D) point).X);
		}

		/// <summary>
		/// Check if the instance is similar to another line.
		/// <p>Lines are considered similar if they contain the same
		/// points. This does not mean they are equal since they can have
		/// opposite directions.</p> </summary>
		/// <param name="line"> line to which instance should be compared </param>
		/// <returns> true if the lines are similar </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean isSimilarTo(final Line line)
		public virtual bool isSimilarTo(Line line)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double angle = Vector3D.angle(direction, line.direction);
			double angle = Vector3D.angle(direction, line.direction);
			return ((angle < tolerance) || (angle > (FastMath.PI - tolerance))) && contains(line.zero);
		}

		/// <summary>
		/// Check if the instance contains a point. </summary>
		/// <param name="p"> point to check </param>
		/// <returns> true if p belongs to the line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean contains(final Vector3D p)
		public virtual bool contains(Vector3D p)
		{
			return distance(p) < tolerance;
		}

		/// <summary>
		/// Compute the distance between the instance and a point. </summary>
		/// <param name="p"> to check </param>
		/// <returns> distance between the instance and the point </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distance(final Vector3D p)
		public virtual double distance(Vector3D p)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D d = p.subtract(zero);
			Vector3D d = p.subtract(zero);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D n = new Vector3D(1.0, d, -d.dotProduct(direction), direction);
			Vector3D n = new Vector3D(1.0, d, -d.dotProduct(direction), direction);
			return n.Norm;
		}

		/// <summary>
		/// Compute the shortest distance between the instance and another line. </summary>
		/// <param name="line"> line to check against the instance </param>
		/// <returns> shortest distance between the instance and the line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distance(final Line line)
		public virtual double distance(Line line)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D normal = Vector3D.crossProduct(direction, line.direction);
			Vector3D normal = Vector3D.crossProduct(direction, line.direction);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = normal.getNorm();
			double n = normal.Norm;
			if (n < Precision.SAFE_MIN)
			{
				// lines are parallel
				return distance(line.zero);
			}

			// signed separation of the two parallel planes that contains the lines
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double offset = line.zero.subtract(zero).dotProduct(normal) / n;
			double offset = line.zero.subtract(zero).dotProduct(normal) / n;

			return FastMath.abs(offset);

		}

		/// <summary>
		/// Compute the point of the instance closest to another line. </summary>
		/// <param name="line"> line to check against the instance </param>
		/// <returns> point of the instance closest to another line </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D closestPoint(final Line line)
		public virtual Vector3D closestPoint(Line line)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cos = direction.dotProduct(line.direction);
			double cos = direction.dotProduct(line.direction);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = 1 - cos * cos;
			double n = 1 - cos * cos;
			if (n < Precision.EPSILON)
			{
				// the lines are parallel
				return zero;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D delta0 = line.zero.subtract(zero);
			Vector3D delta0 = line.zero.subtract(zero);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double a = delta0.dotProduct(direction);
			double a = delta0.dotProduct(direction);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = delta0.dotProduct(line.direction);
			double b = delta0.dotProduct(line.direction);

			return new Vector3D(1, zero, (a - b * cos) / n, direction);

		}

		/// <summary>
		/// Get the intersection point of the instance and another line. </summary>
		/// <param name="line"> other line </param>
		/// <returns> intersection point of the instance and the other line
		/// or null if there are no intersection points </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D intersection(final Line line)
		public virtual Vector3D intersection(Line line)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D closest = closestPoint(line);
			Vector3D closest = closestPoint(line);
			return line.contains(closest) ? closest : null;
		}

		/// <summary>
		/// Build a sub-line covering the whole line. </summary>
		/// <returns> a sub-line covering the whole line </returns>
		public virtual SubLine wholeLine()
		{
			return new SubLine(this, new IntervalsSet(tolerance));
		}

	}

}