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
namespace mathlib.geometry.euclidean.oned
{

	using mathlib.geometry;
	using mathlib.geometry;
	using mathlib.geometry.partitioning;

	/// <summary>
	/// This class represents a 1D oriented hyperplane.
	/// <p>An hyperplane in 1D is a simple point, its orientation being a
	/// boolean.</p>
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: OrientedPoint.java 1560115 2014-01-21 17:49:13Z luc $
	/// @since 3.0
	/// </summary>
	public class OrientedPoint : Hyperplane<Euclidean1D>
	{

		/// <summary>
		/// Default value for tolerance. </summary>
		private const double DEFAULT_TOLERANCE = 1.0e-10;

		/// <summary>
		/// Vector location. </summary>
		private Vector1D location;

		/// <summary>
		/// Orientation. </summary>
		private bool direct;

		/// <summary>
		/// Tolerance below which points are considered to belong to the hyperplane. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="location"> location of the hyperplane </param>
		/// <param name="direct"> if true, the plus side of the hyperplane is towards
		/// abscissas greater than {@code location} </param>
		/// <param name="tolerance"> tolerance below which points are considered to belong to the hyperplane
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public OrientedPoint(final Vector1D location, final boolean direct, final double tolerance)
		public OrientedPoint(Vector1D location, bool direct, double tolerance)
		{
			this.location = location;
			this.direct = direct;
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="location"> location of the hyperplane </param>
		/// <param name="direct"> if true, the plus side of the hyperplane is towards
		/// abscissas greater than {@code location} </param>
		/// @deprecated as of 3.3, replaced with <seealso cref="#OrientedPoint(Vector1D, boolean, double)"/> 
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Deprecated("as of 3.3, replaced with <seealso cref="#OrientedPoint(Vector1D, boolean, double)"/>") public OrientedPoint(final Vector1D location, final boolean direct)
		[Obsolete("as of 3.3, replaced with <seealso cref="#OrientedPoint(Vector1D, boolean, double)"/>")]
		public OrientedPoint(Vector1D location, bool direct) : this(location, direct, DEFAULT_TOLERANCE)
		{
		}

		/// <summary>
		/// Copy the instance.
		/// <p>Since instances are immutable, this method directly returns
		/// the instance.</p> </summary>
		/// <returns> the instance itself </returns>
		public virtual OrientedPoint copySelf()
		{
			return this;
		}

		/// <summary>
		/// Get the offset (oriented distance) of a vector. </summary>
		/// <param name="vector"> vector to check </param>
		/// <returns> offset of the vector </returns>
		public virtual double getOffset(Vector<Euclidean1D> vector)
		{
			return getOffset((Point<Euclidean1D>) vector);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final mathlib.geometry.Point<Euclidean1D> point)
		public virtual double getOffset(Point<Euclidean1D> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = ((Vector1D) point).getX() - location.getX();
			double delta = ((Vector1D) point).X - location.X;
			return direct ? delta : -delta;
		}

		/// <summary>
		/// Build a region covering the whole hyperplane.
		/// <p>Since this class represent zero dimension spaces which does
		/// not have lower dimension sub-spaces, this method returns a dummy
		/// implementation of a {@link
		/// mathlib.geometry.partitioning.SubHyperplane SubHyperplane}.
		/// This implementation is only used to allow the {@link
		/// mathlib.geometry.partitioning.SubHyperplane
		/// SubHyperplane} class implementation to work properly, it should
		/// <em>not</em> be used otherwise.</p> </summary>
		/// <returns> a dummy sub hyperplane </returns>
		public virtual SubOrientedPoint wholeHyperplane()
		{
			return new SubOrientedPoint(this, null);
		}

		/// <summary>
		/// Build a region covering the whole space. </summary>
		/// <returns> a region containing the instance (really an {@link
		/// IntervalsSet IntervalsSet} instance) </returns>
		public virtual IntervalsSet wholeSpace()
		{
			return new IntervalsSet(tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean sameOrientationAs(final mathlib.geometry.partitioning.Hyperplane<Euclidean1D> other)
		public virtual bool sameOrientationAs(Hyperplane<Euclidean1D> other)
		{
			return !(direct ^ ((OrientedPoint) other).direct);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.3
		/// </summary>
		public virtual Point<Euclidean1D> project(Point<Euclidean1D> point)
		{
			return location;
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
		/// Get the hyperplane location on the real line. </summary>
		/// <returns> the hyperplane location </returns>
		public virtual Vector1D Location
		{
			get
			{
				return location;
			}
		}

		/// <summary>
		/// Check if the hyperplane orientation is direct. </summary>
		/// <returns> true if the plus side of the hyperplane is towards
		/// abscissae greater than hyperplane location </returns>
		public virtual bool Direct
		{
			get
			{
				return direct;
			}
		}

		/// <summary>
		/// Revert the instance.
		/// </summary>
		public virtual void revertSelf()
		{
			direct = !direct;
		}

	}

}