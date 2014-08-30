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

namespace org.apache.commons.math3.geometry.euclidean.threed
{


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using org.apache.commons.math3.geometry;
	using org.apache.commons.math3.geometry;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// This class implements vectors in a three-dimensional space.
	/// <p>Instance of this class are guaranteed to be immutable.</p>
	/// @version $Id: Vector3D.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 1.2
	/// </summary>
	[Serializable]
	public class Vector3D : Vector<Euclidean3D>
	{

		/// <summary>
		/// Null vector (coordinates: 0, 0, 0). </summary>
		public static readonly Vector3D ZERO = new Vector3D(0, 0, 0);

		/// <summary>
		/// First canonical vector (coordinates: 1, 0, 0). </summary>
		public static readonly Vector3D PLUS_I = new Vector3D(1, 0, 0);

		/// <summary>
		/// Opposite of the first canonical vector (coordinates: -1, 0, 0). </summary>
		public static readonly Vector3D MINUS_I = new Vector3D(-1, 0, 0);

		/// <summary>
		/// Second canonical vector (coordinates: 0, 1, 0). </summary>
		public static readonly Vector3D PLUS_J = new Vector3D(0, 1, 0);

		/// <summary>
		/// Opposite of the second canonical vector (coordinates: 0, -1, 0). </summary>
		public static readonly Vector3D MINUS_J = new Vector3D(0, -1, 0);

		/// <summary>
		/// Third canonical vector (coordinates: 0, 0, 1). </summary>
		public static readonly Vector3D PLUS_K = new Vector3D(0, 0, 1);

		/// <summary>
		/// Opposite of the third canonical vector (coordinates: 0, 0, -1). </summary>
		public static readonly Vector3D MINUS_K = new Vector3D(0, 0, -1);

		// CHECKSTYLE: stop ConstantName
		/// <summary>
		/// A vector with all coordinates set to NaN. </summary>
		public static readonly Vector3D NaN_Renamed = new Vector3D(double.NaN, double.NaN, double.NaN);
		// CHECKSTYLE: resume ConstantName

		/// <summary>
		/// A vector with all coordinates set to positive infinity. </summary>
		public static readonly Vector3D POSITIVE_INFINITY = new Vector3D(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);

		/// <summary>
		/// A vector with all coordinates set to negative infinity. </summary>
		public static readonly Vector3D NEGATIVE_INFINITY = new Vector3D(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 1313493323784566947L;

		/// <summary>
		/// Abscissa. </summary>
		private readonly double x;

		/// <summary>
		/// Ordinate. </summary>
		private readonly double y;

		/// <summary>
		/// Height. </summary>
		private readonly double z;

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="x"> abscissa </param>
		/// <param name="y"> ordinate </param>
		/// <param name="z"> height </param>
		/// <seealso cref= #getX() </seealso>
		/// <seealso cref= #getY() </seealso>
		/// <seealso cref= #getZ() </seealso>
		public Vector3D(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="v"> coordinates array </param>
		/// <exception cref="DimensionMismatchException"> if array does not have 3 elements </exception>
		/// <seealso cref= #toArray() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Vector3D(double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public Vector3D(double[] v)
		{
			if (v.Length != 3)
			{
				throw new DimensionMismatchException(v.Length, 3);
			}
			this.x = v[0];
			this.y = v[1];
			this.z = v[2];
		}

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its azimuthal coordinates </summary>
		/// <param name="alpha"> azimuth (&alpha;) around Z
		///              (0 is +X, &pi;/2 is +Y, &pi; is -X and 3&pi;/2 is -Y) </param>
		/// <param name="delta"> elevation (&delta;) above (XY) plane, from -&pi;/2 to +&pi;/2 </param>
		/// <seealso cref= #getAlpha() </seealso>
		/// <seealso cref= #getDelta() </seealso>
		public Vector3D(double alpha, double delta)
		{
			double cosDelta = FastMath.cos(delta);
			this.x = FastMath.cos(alpha) * cosDelta;
			this.y = FastMath.sin(alpha) * cosDelta;
			this.z = FastMath.sin(delta);
		}

		/// <summary>
		/// Multiplicative constructor
		/// Build a vector from another one and a scale factor.
		/// The vector built will be a * u </summary>
		/// <param name="a"> scale factor </param>
		/// <param name="u"> base (unscaled) vector </param>
		public Vector3D(double a, Vector3D u)
		{
			this.x = a * u.x;
			this.y = a * u.y;
			this.z = a * u.z;
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from two other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
		public Vector3D(double a1, Vector3D u1, double a2, Vector3D u2)
		{
			this.x = MathArrays.linearCombination(a1, u1.x, a2, u2.x);
			this.y = MathArrays.linearCombination(a1, u1.y, a2, u2.y);
			this.z = MathArrays.linearCombination(a1, u1.z, a2, u2.z);
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from three other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 + a3 * u3 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
		/// <param name="a3"> third scale factor </param>
		/// <param name="u3"> third base (unscaled) vector </param>
		public Vector3D(double a1, Vector3D u1, double a2, Vector3D u2, double a3, Vector3D u3)
		{
			this.x = MathArrays.linearCombination(a1, u1.x, a2, u2.x, a3, u3.x);
			this.y = MathArrays.linearCombination(a1, u1.y, a2, u2.y, a3, u3.y);
			this.z = MathArrays.linearCombination(a1, u1.z, a2, u2.z, a3, u3.z);
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from four other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 + a3 * u3 + a4 * u4 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
		/// <param name="a3"> third scale factor </param>
		/// <param name="u3"> third base (unscaled) vector </param>
		/// <param name="a4"> fourth scale factor </param>
		/// <param name="u4"> fourth base (unscaled) vector </param>
		public Vector3D(double a1, Vector3D u1, double a2, Vector3D u2, double a3, Vector3D u3, double a4, Vector3D u4)
		{
			this.x = MathArrays.linearCombination(a1, u1.x, a2, u2.x, a3, u3.x, a4, u4.x);
			this.y = MathArrays.linearCombination(a1, u1.y, a2, u2.y, a3, u3.y, a4, u4.y);
			this.z = MathArrays.linearCombination(a1, u1.z, a2, u2.z, a3, u3.z, a4, u4.z);
		}

		/// <summary>
		/// Get the abscissa of the vector. </summary>
		/// <returns> abscissa of the vector </returns>
		/// <seealso cref= #Vector3D(double, double, double) </seealso>
		public virtual double X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Get the ordinate of the vector. </summary>
		/// <returns> ordinate of the vector </returns>
		/// <seealso cref= #Vector3D(double, double, double) </seealso>
		public virtual double Y
		{
			get
			{
				return y;
			}
		}

		/// <summary>
		/// Get the height of the vector. </summary>
		/// <returns> height of the vector </returns>
		/// <seealso cref= #Vector3D(double, double, double) </seealso>
		public virtual double Z
		{
			get
			{
				return z;
			}
		}

		/// <summary>
		/// Get the vector coordinates as a dimension 3 array. </summary>
		/// <returns> vector coordinates </returns>
		/// <seealso cref= #Vector3D(double[]) </seealso>
		public virtual double[] toArray()
		{
			return new double[] {x, y, z};
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Space Space
		{
			get
			{
				return Euclidean3D.Instance;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector3D Zero
		{
			get
			{
				return ZERO;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Norm1
		{
			get
			{
				return FastMath.abs(x) + FastMath.abs(y) + FastMath.abs(z);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Norm
		{
			get
			{
				// there are no cancellation problems here, so we use the straightforward formula
				return FastMath.sqrt(x * x + y * y + z * z);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double NormSq
		{
			get
			{
				// there are no cancellation problems here, so we use the straightforward formula
				return x * x + y * y + z * z;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double NormInf
		{
			get
			{
				return FastMath.max(FastMath.max(FastMath.abs(x), FastMath.abs(y)), FastMath.abs(z));
			}
		}

		/// <summary>
		/// Get the azimuth of the vector. </summary>
		/// <returns> azimuth (&alpha;) of the vector, between -&pi; and +&pi; </returns>
		/// <seealso cref= #Vector3D(double, double) </seealso>
		public virtual double Alpha
		{
			get
			{
				return FastMath.atan2(y, x);
			}
		}

		/// <summary>
		/// Get the elevation of the vector. </summary>
		/// <returns> elevation (&delta;) of the vector, between -&pi;/2 and +&pi;/2 </returns>
		/// <seealso cref= #Vector3D(double, double) </seealso>
		public virtual double Delta
		{
			get
			{
				return FastMath.asin(z / Norm);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D add(final org.apache.commons.math3.geometry.Vector<Euclidean3D> v)
		public virtual Vector3D add(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
			return new Vector3D(x + v3.x, y + v3.y, z + v3.z);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D add(double factor, final org.apache.commons.math3.geometry.Vector<Euclidean3D> v)
		public virtual Vector3D add(double factor, Vector<Euclidean3D> v)
		{
			return new Vector3D(1, this, factor, (Vector3D) v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D subtract(final org.apache.commons.math3.geometry.Vector<Euclidean3D> v)
		public virtual Vector3D subtract(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
			return new Vector3D(x - v3.x, y - v3.y, z - v3.z);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D subtract(final double factor, final org.apache.commons.math3.geometry.Vector<Euclidean3D> v)
		public virtual Vector3D subtract(double factor, Vector<Euclidean3D> v)
		{
			return new Vector3D(1, this, -factor, (Vector3D) v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Vector3D normalize() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual Vector3D normalize()
		{
			double s = Norm;
			if (s == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.CANNOT_NORMALIZE_A_ZERO_NORM_VECTOR);
			}
			return scalarMultiply(1 / s);
		}

		/// <summary>
		/// Get a vector orthogonal to the instance.
		/// <p>There are an infinite number of normalized vectors orthogonal
		/// to the instance. This method picks up one of them almost
		/// arbitrarily. It is useful when one needs to compute a reference
		/// frame with one of the axes in a predefined direction. The
		/// following example shows how to build a frame having the k axis
		/// aligned with the known vector u :
		/// <pre><code>
		///   Vector3D k = u.normalize();
		///   Vector3D i = k.orthogonal();
		///   Vector3D j = Vector3D.crossProduct(k, i);
		/// </code></pre></p> </summary>
		/// <returns> a new normalized vector orthogonal to the instance </returns>
		/// <exception cref="MathArithmeticException"> if the norm of the instance is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Vector3D orthogonal() throws org.apache.commons.math3.exception.MathArithmeticException
		public virtual Vector3D orthogonal()
		{

			double threshold = 0.6 * Norm;
			if (threshold == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}

			if (FastMath.abs(x) <= threshold)
			{
				double inverse = 1 / FastMath.sqrt(y * y + z * z);
				return new Vector3D(0, inverse * z, -inverse * y);
			}
			else if (FastMath.abs(y) <= threshold)
			{
				double inverse = 1 / FastMath.sqrt(x * x + z * z);
				return new Vector3D(-inverse * z, 0, inverse * x);
			}
			double inverse = 1 / FastMath.sqrt(x * x + y * y);
			return new Vector3D(inverse * y, -inverse * x, 0);

		}

		/// <summary>
		/// Compute the angular separation between two vectors.
		/// <p>This method computes the angular separation between two
		/// vectors using the dot product for well separated vectors and the
		/// cross product for almost aligned vectors. This allows to have a
		/// good accuracy in all cases, even for vectors very close to each
		/// other.</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> angular separation between v1 and v2 </returns>
		/// <exception cref="MathArithmeticException"> if either vector has a null norm </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double angle(Vector3D v1, Vector3D v2) throws org.apache.commons.math3.exception.MathArithmeticException
		public static double angle(Vector3D v1, Vector3D v2)
		{

			double normProduct = v1.Norm * v2.Norm;
			if (normProduct == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}

			double dot = v1.dotProduct(v2);
			double threshold = normProduct * 0.9999;
			if ((dot < -threshold) || (dot > threshold))
			{
				// the vectors are almost aligned, compute using the sine
				Vector3D v3 = crossProduct(v1, v2);
				if (dot >= 0)
				{
					return FastMath.asin(v3.Norm / normProduct);
				}
				return FastMath.PI - FastMath.asin(v3.Norm / normProduct);
			}

			// the vectors are sufficiently separated to use the cosine
			return FastMath.acos(dot / normProduct);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector3D negate()
		{
			return new Vector3D(-x, -y, -z);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector3D scalarMultiply(double a)
		{
			return new Vector3D(a * x, a * y, a * z);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Infinite
		{
			get
			{
				return !NaN && (double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z));
			}
		}

		/// <summary>
		/// Test for the equality of two 3D vectors.
		/// <p>
		/// If all coordinates of two 3D vectors are exactly the same, and none are
		/// <code>Double.NaN</code>, the two 3D vectors are considered to be equal.
		/// </p>
		/// <p>
		/// <code>NaN</code> coordinates are considered to affect globally the vector
		/// and be equals to each other - i.e, if either (or all) coordinates of the
		/// 3D vector are equal to <code>Double.NaN</code>, the 3D vector is equal to
		/// <seealso cref="#NaN"/>.
		/// </p>
		/// </summary>
		/// <param name="other"> Object to test for equality to this </param>
		/// <returns> true if two 3D vector objects are equal, false if
		///         object is null, not an instance of Vector3D, or
		///         not equal to this Vector3D instance
		///  </returns>
		public override bool Equals(object other)
		{

			if (this == other)
			{
				return true;
			}

			if (other is Vector3D)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D rhs = (Vector3D)other;
				Vector3D rhs = (Vector3D)other;
				if (rhs.NaN)
				{
					return this.NaN;
				}

				return (x == rhs.x) && (y == rhs.y) && (z == rhs.z);
			}
			return false;
		}

		/// <summary>
		/// Get a hashCode for the 3D vector.
		/// <p>
		/// All NaN values have the same hash code.</p>
		/// </summary>
		/// <returns> a hash code value for this object </returns>
		public override int GetHashCode()
		{
			if (NaN)
			{
				return 642;
			}
			return 643 * (164 * MathUtils.hash(x) + 3 * MathUtils.hash(y) + MathUtils.hash(z));
		}

		/// <summary>
		/// {@inheritDoc}
		/// <p>
		/// The implementation uses specific multiplication and addition
		/// algorithms to preserve accuracy and reduce cancellation effects.
		/// It should be very accurate even for nearly orthogonal vectors.
		/// </p> </summary>
		/// <seealso cref= MathArrays#linearCombination(double, double, double, double, double, double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double dotProduct(final org.apache.commons.math3.geometry.Vector<Euclidean3D> v)
		public virtual double dotProduct(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
			return MathArrays.linearCombination(x, v3.x, y, v3.y, z, v3.z);
		}

		/// <summary>
		/// Compute the cross-product of the instance with another vector. </summary>
		/// <param name="v"> other vector </param>
		/// <returns> the cross product this ^ v as a new Vector3D </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector3D crossProduct(final org.apache.commons.math3.geometry.Vector<Euclidean3D> v)
		public virtual Vector3D crossProduct(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
			return new Vector3D(MathArrays.linearCombination(y, v3.z, -z, v3.y), MathArrays.linearCombination(z, v3.x, -x, v3.z), MathArrays.linearCombination(x, v3.y, -y, v3.x));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance1(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = org.apache.commons.math3.util.FastMath.abs(v3.x - x);
			double dx = FastMath.abs(v3.x - x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = org.apache.commons.math3.util.FastMath.abs(v3.y - y);
			double dy = FastMath.abs(v3.y - y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dz = org.apache.commons.math3.util.FastMath.abs(v3.z - z);
			double dz = FastMath.abs(v3.z - z);
			return dx + dy + dz;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance(Vector<Euclidean3D> v)
		{
			return distance((Point<Euclidean3D>) v);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance(Point<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = v3.x - x;
			double dx = v3.x - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = v3.y - y;
			double dy = v3.y - y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dz = v3.z - z;
			double dz = v3.z - z;
			return FastMath.sqrt(dx * dx + dy * dy + dz * dz);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distanceInf(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = org.apache.commons.math3.util.FastMath.abs(v3.x - x);
			double dx = FastMath.abs(v3.x - x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = org.apache.commons.math3.util.FastMath.abs(v3.y - y);
			double dy = FastMath.abs(v3.y - y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dz = org.apache.commons.math3.util.FastMath.abs(v3.z - z);
			double dz = FastMath.abs(v3.z - z);
			return FastMath.max(FastMath.max(dx, dy), dz);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distanceSq(Vector<Euclidean3D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector3D v3 = (Vector3D) v;
			Vector3D v3 = (Vector3D) v;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = v3.x - x;
			double dx = v3.x - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = v3.y - y;
			double dy = v3.y - y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dz = v3.z - z;
			double dz = v3.z - z;
			return dx * dx + dy * dy + dz * dz;
		}

		/// <summary>
		/// Compute the dot-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> the dot product v1.v2 </returns>
		public static double dotProduct(Vector3D v1, Vector3D v2)
		{
			return v1.dotProduct(v2);
		}

		/// <summary>
		/// Compute the cross-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> the cross product v1 ^ v2 as a new Vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Vector3D crossProduct(final Vector3D v1, final Vector3D v2)
		public static Vector3D crossProduct(Vector3D v1, Vector3D v2)
		{
			return v1.crossProduct(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>1</sub> norm </returns>
		public static double distance1(Vector3D v1, Vector3D v2)
		{
			return v1.distance1(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>2</sub> norm </returns>
		public static double distance(Vector3D v1, Vector3D v2)
		{
			return v1.distance(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>&infin;</sub> norm </returns>
		public static double distanceInf(Vector3D v1, Vector3D v2)
		{
			return v1.distanceInf(v2);
		}

		/// <summary>
		/// Compute the square of the distance between two vectors.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// <returns> the square of the distance between v1 and v2 </returns>
		public static double distanceSq(Vector3D v1, Vector3D v2)
		{
			return v1.distanceSq(v2);
		}

		/// <summary>
		/// Get a string representation of this vector. </summary>
		/// <returns> a string representation of this vector </returns>
		public override string ToString()
		{
			return Vector3DFormat.Instance.format(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public String toString(final java.text.NumberFormat format)
		public virtual string ToString(NumberFormat format)
		{
			return (new Vector3DFormat(format)).format(this);
		}

	}

}