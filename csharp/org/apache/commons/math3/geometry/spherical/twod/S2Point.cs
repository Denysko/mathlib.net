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

	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using org.apache.commons.math3.geometry;
	using Vector3D = org.apache.commons.math3.geometry.euclidean.threed.Vector3D;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// This class represents a point on the 2-sphere.
	/// <p>
	/// We use the mathematical convention to use the azimuthal angle \( \theta \)
	/// in the x-y plane as the first coordinate, and the polar angle \( \varphi \)
	/// as the second coordinate (see <a
	/// href="http://mathworld.wolfram.com/SphericalCoordinates.html">Spherical
	/// Coordinates</a> in MathWorld).
	/// </p>
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: S2Point.java 1554651 2014-01-01 17:27:48Z luc $
	/// @since 3.3
	/// </summary>
	public class S2Point : Point<Sphere2D>
	{

		/// <summary>
		/// +I (coordinates: \( \theta = 0, \varphi = \pi/2 \)). </summary>
		public static readonly S2Point PLUS_I = new S2Point(0, 0.5 * FastMath.PI, Vector3D.PLUS_I);

		/// <summary>
		/// +J (coordinates: \( \theta = \pi/2, \varphi = \pi/2 \))). </summary>
		public static readonly S2Point PLUS_J = new S2Point(0.5 * FastMath.PI, 0.5 * FastMath.PI, Vector3D.PLUS_J);

		/// <summary>
		/// +K (coordinates: \( \theta = any angle, \varphi = 0 \)). </summary>
		public static readonly S2Point PLUS_K = new S2Point(0, 0, Vector3D.PLUS_K);

		/// <summary>
		/// -I (coordinates: \( \theta = \pi, \varphi = \pi/2 \)). </summary>
		public static readonly S2Point MINUS_I = new S2Point(FastMath.PI, 0.5 * FastMath.PI, Vector3D.MINUS_I);

		/// <summary>
		/// -J (coordinates: \( \theta = 3\pi/2, \varphi = \pi/2 \)). </summary>
		public static readonly S2Point MINUS_J = new S2Point(1.5 * FastMath.PI, 0.5 * FastMath.PI, Vector3D.MINUS_J);

		/// <summary>
		/// -K (coordinates: \( \theta = any angle, \varphi = \pi \)). </summary>
		public static readonly S2Point MINUS_K = new S2Point(0, FastMath.PI, Vector3D.MINUS_K);

		// CHECKSTYLE: stop ConstantName
		/// <summary>
		/// A vector with all coordinates set to NaN. </summary>
		public static readonly S2Point NaN_Renamed = new S2Point(double.NaN, double.NaN, Vector3D.NaN_Renamed);
		// CHECKSTYLE: resume ConstantName

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20131218L;

		/// <summary>
		/// Azimuthal angle \( \theta \) in the x-y plane. </summary>
		private readonly double theta;

		/// <summary>
		/// Polar angle \( \varphi \). </summary>
		private readonly double phi;

		/// <summary>
		/// Corresponding 3D normalized vector. </summary>
		private readonly Vector3D vector_Renamed;

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its spherical coordinates </summary>
		/// <param name="theta"> azimuthal angle \( \theta \) in the x-y plane </param>
		/// <param name="phi"> polar angle \( \varphi \) </param>
		/// <seealso cref= #getTheta() </seealso>
		/// <seealso cref= #getPhi() </seealso>
		/// <exception cref="OutOfRangeException"> if \( \varphi \) is not in the [\( 0; \pi \)] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public S2Point(final double theta, final double phi) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public S2Point(double theta, double phi) : this(theta, phi, vector(theta, phi))
		{
		}

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its underlying 3D vector </summary>
		/// <param name="vector"> 3D vector </param>
		/// <exception cref="MathArithmeticException"> if vector norm is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public S2Point(final org.apache.commons.math3.geometry.euclidean.threed.Vector3D vector) throws org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public S2Point(Vector3D vector) : this(FastMath.atan2(vector.Y, vector.X), Vector3D.angle(Vector3D.PLUS_K, vector), vector.normalize())
		{
		}

		/// <summary>
		/// Build a point from its internal components. </summary>
		/// <param name="theta"> azimuthal angle \( \theta \) in the x-y plane </param>
		/// <param name="phi"> polar angle \( \varphi \) </param>
		/// <param name="vector"> corresponding vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private S2Point(final double theta, final double phi, final org.apache.commons.math3.geometry.euclidean.threed.Vector3D vector)
		private S2Point(double theta, double phi, Vector3D vector)
		{
			this.theta = theta;
			this.phi = phi;
			this.vector_Renamed = vector;
		}

		/// <summary>
		/// Build the normalized vector corresponding to spherical coordinates. </summary>
		/// <param name="theta"> azimuthal angle \( \theta \) in the x-y plane </param>
		/// <param name="phi"> polar angle \( \varphi \) </param>
		/// <returns> normalized vector </returns>
		/// <exception cref="OutOfRangeException"> if \( \varphi \) is not in the [\( 0; \pi \)] range </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static org.apache.commons.math3.geometry.euclidean.threed.Vector3D vector(final double theta, final double phi) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static Vector3D vector(double theta, double phi)
		{

			if (phi < 0 || phi > FastMath.PI)
			{
				throw new OutOfRangeException(phi, 0, FastMath.PI);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cosTheta = org.apache.commons.math3.util.FastMath.cos(theta);
			double cosTheta = FastMath.cos(theta);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sinTheta = org.apache.commons.math3.util.FastMath.sin(theta);
			double sinTheta = FastMath.sin(theta);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double cosPhi = org.apache.commons.math3.util.FastMath.cos(phi);
			double cosPhi = FastMath.cos(phi);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sinPhi = org.apache.commons.math3.util.FastMath.sin(phi);
			double sinPhi = FastMath.sin(phi);

			return new Vector3D(cosTheta * sinPhi, sinTheta * sinPhi, cosPhi);

		}

		/// <summary>
		/// Get the azimuthal angle \( \theta \) in the x-y plane. </summary>
		/// <returns> azimuthal angle \( \theta \) in the x-y plane </returns>
		/// <seealso cref= #S2Point(double, double) </seealso>
		public virtual double Theta
		{
			get
			{
				return theta;
			}
		}

		/// <summary>
		/// Get the polar angle \( \varphi \). </summary>
		/// <returns> polar angle \( \varphi \) </returns>
		/// <seealso cref= #S2Point(double, double) </seealso>
		public virtual double Phi
		{
			get
			{
				return phi;
			}
		}

		/// <summary>
		/// Get the corresponding normalized vector in the 3D euclidean space. </summary>
		/// <returns> normalized vector </returns>
		public virtual Vector3D Vector
		{
			get
			{
				return vector_Renamed;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Space Space
		{
			get
			{
				return Sphere2D.Instance;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(theta) || double.IsNaN(phi);
			}
		}

		/// <summary>
		/// Get the opposite of the instance. </summary>
		/// <returns> a new vector which is opposite to the instance </returns>
		public virtual S2Point negate()
		{
			return new S2Point(-theta, FastMath.PI - phi, vector_Renamed.negate());
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double distance(final org.apache.commons.math3.geometry.Point<Sphere2D> point)
		public virtual double distance(Point<Sphere2D> point)
		{
			return distance(this, (S2Point) point);
		}

		/// <summary>
		/// Compute the distance (angular separation) between two points. </summary>
		/// <param name="p1"> first vector </param>
		/// <param name="p2"> second vector </param>
		/// <returns> the angular separation between p1 and p2 </returns>
		public static double distance(S2Point p1, S2Point p2)
		{
			return Vector3D.angle(p1.vector_Renamed, p2.vector_Renamed);
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

			if (other is S2Point)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final S2Point rhs = (S2Point) other;
				S2Point rhs = (S2Point) other;
				if (rhs.NaN)
				{
					return this.NaN;
				}

				return (theta == rhs.theta) && (phi == rhs.phi);
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
			return 134 * (37 * MathUtils.hash(theta) + MathUtils.hash(phi));
		}

	}

}