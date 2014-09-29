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
namespace mathlib.geometry.spherical.oned
{

	using mathlib.geometry;
	using mathlib.geometry.partitioning;

	/// <summary>
	/// This class represents a 1D oriented hyperplane on the circle.
	/// <p>An hyperplane on the 1-sphere is an angle with an orientation.</p>
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: LimitAngle.java 1560115 2014-01-21 17:49:13Z luc $
	/// @since 3.3
	/// </summary>
	public class LimitAngle : Hyperplane<Sphere1D>
	{

		/// <summary>
		/// Angle location. </summary>
		private S1Point location;

		/// <summary>
		/// Orientation. </summary>
		private bool direct;

		/// <summary>
		/// Tolerance below which angles are considered identical. </summary>
		private readonly double tolerance;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="location"> location of the hyperplane </param>
		/// <param name="direct"> if true, the plus side of the hyperplane is towards
		/// angles greater than {@code location} </param>
		/// <param name="tolerance"> tolerance below which angles are considered identical </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LimitAngle(final S1Point location, final boolean direct, final double tolerance)
		public LimitAngle(S1Point location, bool direct, double tolerance)
		{
			this.location = location;
			this.direct = direct;
			this.tolerance = tolerance;
		}

		/// <summary>
		/// Copy the instance.
		/// <p>Since instances are immutable, this method directly returns
		/// the instance.</p> </summary>
		/// <returns> the instance itself </returns>
		public virtual LimitAngle copySelf()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double getOffset(final mathlib.geometry.Point<Sphere1D> point)
		public virtual double getOffset(Point<Sphere1D> point)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delta = ((S1Point) point).getAlpha() - location.getAlpha();
			double delta = ((S1Point) point).Alpha - location.Alpha;
			return direct ? delta : -delta;
		}

		/// <summary>
		/// Check if the hyperplane orientation is direct. </summary>
		/// <returns> true if the plus side of the hyperplane is towards
		/// angles greater than hyperplane location </returns>
		public virtual bool Direct
		{
			get
			{
				return direct;
			}
		}

		/// <summary>
		/// Get the reverse of the instance.
		/// <p>Get a limit angle with reversed orientation with respect to the
		/// instance. A new object is built, the instance is untouched.</p> </summary>
		/// <returns> a new limit angle, with orientation opposite to the instance orientation </returns>
		public virtual LimitAngle Reverse
		{
			get
			{
				return new LimitAngle(location, !direct, tolerance);
			}
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
		public virtual SubLimitAngle wholeHyperplane()
		{
			return new SubLimitAngle(this, null);
		}

		/// <summary>
		/// Build a region covering the whole space. </summary>
		/// <returns> a region containing the instance (really an {@link
		/// ArcsSet IntervalsSet} instance) </returns>
		public virtual ArcsSet wholeSpace()
		{
			return new ArcsSet(tolerance);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean sameOrientationAs(final mathlib.geometry.partitioning.Hyperplane<Sphere1D> other)
		public virtual bool sameOrientationAs(Hyperplane<Sphere1D> other)
		{
			return !(direct ^ ((LimitAngle) other).direct);
		}

		/// <summary>
		/// Get the hyperplane location on the circle. </summary>
		/// <returns> the hyperplane location </returns>
		public virtual S1Point Location
		{
			get
			{
				return location;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Point<Sphere1D> project(Point<Sphere1D> point)
		{
			return location;
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

	}

}