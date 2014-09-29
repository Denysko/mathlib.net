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
	using Vector2D = mathlib.geometry.euclidean.twod.Vector2D;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// This class represents a point on the 1-sphere.
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: S1Point.java 1554654 2014-01-01 17:30:06Z luc $
	/// @since 3.3
	/// </summary>
	public class S1Point : Point<Sphere1D>
	{

	   // CHECKSTYLE: stop ConstantName
		/// <summary>
		/// A vector with all coordinates set to NaN. </summary>
		public static readonly S1Point NaN_Renamed = new S1Point(double.NaN, Vector2D.NaN_Renamed);
		// CHECKSTYLE: resume ConstantName

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20131218L;

		/// <summary>
		/// Azimuthal angle \( \alpha \). </summary>
		private readonly double alpha;

		/// <summary>
		/// Corresponding 2D normalized vector. </summary>
		private readonly Vector2D vector;

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="alpha"> azimuthal angle \( \alpha \) </param>
		/// <seealso cref= #getAlpha() </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public S1Point(final double alpha)
		public S1Point(double alpha) : this(MathUtils.normalizeAngle(alpha, FastMath.PI), new Vector2D(FastMath.cos(alpha), FastMath.sin(alpha)))
		{
		}

		/// <summary>
		/// Build a point from its internal components. </summary>
		/// <param name="alpha"> azimuthal angle \( \alpha \) </param>
		/// <param name="vector"> corresponding vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private S1Point(final double alpha, final mathlib.geometry.euclidean.twod.Vector2D vector)
		private S1Point(double alpha, Vector2D vector)
		{
			this.alpha = alpha;
			this.vector = vector;
		}

		/// <summary>
		/// Get the azimuthal angle \( \alpha \). </summary>
		/// <returns> azimuthal angle \( \alpha \) </returns>
		/// <seealso cref= #S1Point(double) </seealso>
		public virtual double Alpha
		{
			get
			{
				return alpha;
			}
		}

		/// <summary>
		/// Get the corresponding normalized vector in the 2D euclidean space. </summary>
		/// <returns> normalized vector </returns>
		public virtual Vector2D Vector
		{
			get
			{
				return vector;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Space Space
		{
			get
			{
				return Sphere1D.Instance;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(alpha);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distance(final mathlib.geometry.Point<Sphere1D> point)
		public virtual double distance(Point<Sphere1D> point)
		{
			return distance(this, (S1Point) point);
		}

		/// <summary>
		/// Compute the distance (angular separation) between two points. </summary>
		/// <param name="p1"> first vector </param>
		/// <param name="p2"> second vector </param>
		/// <returns> the angular separation between p1 and p2 </returns>
		public static double distance(S1Point p1, S1Point p2)
		{
			return Vector2D.angle(p1.vector, p2.vector);
		}

		/// <summary>
		/// Test for the equality of two points on the 2-sphere.
		/// <p>
		/// If all coordinates of two points are exactly the same, and none are
		/// <code>Double.NaN</code>, the two points are considered to be equal.
		/// </p>
		/// <p>
		/// <code>NaN</code> coordinates are considered to affect globally the vector
		/// and be equals to each other - i.e, if either (or all) coordinates of the
		/// 2D vector are equal to <code>Double.NaN</code>, the 2D vector is equal to
		/// <seealso cref="#NaN"/>.
		/// </p>
		/// </summary>
		/// <param name="other"> Object to test for equality to this </param>
		/// <returns> true if two points on the 2-sphere objects are equal, false if
		///         object is null, not an instance of S2Point, or
		///         not equal to this S2Point instance
		///  </returns>
		public override bool Equals(object other)
		{

			if (this == other)
			{
				return true;
			}

			if (other is S1Point)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final S1Point rhs = (S1Point) other;
				S1Point rhs = (S1Point) other;
				if (rhs.NaN)
				{
					return this.NaN;
				}

				return alpha == rhs.alpha;
			}

			return false;

		}

		/// <summary>
		/// Get a hashCode for the 2D vector.
		/// <p>
		/// All NaN values have the same hash code.</p>
		/// </summary>
		/// <returns> a hash code value for this object </returns>
		public override int GetHashCode()
		{
			if (NaN)
			{
				return 542;
			}
			return 1759 * MathUtils.hash(alpha);
		}

	}

}