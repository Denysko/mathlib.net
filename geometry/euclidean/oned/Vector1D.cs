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

	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.geometry;
	using mathlib.geometry;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// This class represents a 1D vector.
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: Vector1D.java 1554651 2014-01-01 17:27:48Z luc $
	/// @since 3.0
	/// </summary>
	public class Vector1D : Vector<Euclidean1D>
	{

		/// <summary>
		/// Origin (coordinates: 0). </summary>
		public static readonly Vector1D ZERO = new Vector1D(0.0);

		/// <summary>
		/// Unit (coordinates: 1). </summary>
		public static readonly Vector1D ONE = new Vector1D(1.0);

		// CHECKSTYLE: stop ConstantName
		/// <summary>
		/// A vector with all coordinates set to NaN. </summary>
		public static readonly Vector1D NaN_Renamed = new Vector1D(double.NaN);
		// CHECKSTYLE: resume ConstantName

		/// <summary>
		/// A vector with all coordinates set to positive infinity. </summary>
		public static readonly Vector1D POSITIVE_INFINITY = new Vector1D(double.PositiveInfinity);

		/// <summary>
		/// A vector with all coordinates set to negative infinity. </summary>
		public static readonly Vector1D NEGATIVE_INFINITY = new Vector1D(double.NegativeInfinity);

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 7556674948671647925L;

		/// <summary>
		/// Abscissa. </summary>
		private readonly double x;

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="x"> abscissa </param>
		/// <seealso cref= #getX() </seealso>
		public Vector1D(double x)
		{
			this.x = x;
		}

		/// <summary>
		/// Multiplicative constructor
		/// Build a vector from another one and a scale factor.
		/// The vector built will be a * u </summary>
		/// <param name="a"> scale factor </param>
		/// <param name="u"> base (unscaled) vector </param>
		public Vector1D(double a, Vector1D u)
		{
			this.x = a * u.x;
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from two other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
		public Vector1D(double a1, Vector1D u1, double a2, Vector1D u2)
		{
			this.x = a1 * u1.x + a2 * u2.x;
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
		public Vector1D(double a1, Vector1D u1, double a2, Vector1D u2, double a3, Vector1D u3)
		{
			this.x = a1 * u1.x + a2 * u2.x + a3 * u3.x;
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
		public Vector1D(double a1, Vector1D u1, double a2, Vector1D u2, double a3, Vector1D u3, double a4, Vector1D u4)
		{
			this.x = a1 * u1.x + a2 * u2.x + a3 * u3.x + a4 * u4.x;
		}

		/// <summary>
		/// Get the abscissa of the vector. </summary>
		/// <returns> abscissa of the vector </returns>
		/// <seealso cref= #Vector1D(double) </seealso>
		public virtual double X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Space Space
		{
			get
			{
				return Euclidean1D.Instance;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D Zero
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
				return FastMath.abs(x);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Norm
		{
			get
			{
				return FastMath.abs(x);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double NormSq
		{
			get
			{
				return x * x;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double NormInf
		{
			get
			{
				return FastMath.abs(x);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D add(Vector<Euclidean1D> v)
		{
			Vector1D v1 = (Vector1D) v;
			return new Vector1D(x + v1.X);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D add(double factor, Vector<Euclidean1D> v)
		{
			Vector1D v1 = (Vector1D) v;
			return new Vector1D(x + factor * v1.X);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D subtract(Vector<Euclidean1D> p)
		{
			Vector1D p3 = (Vector1D) p;
			return new Vector1D(x - p3.x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D subtract(double factor, Vector<Euclidean1D> v)
		{
			Vector1D v1 = (Vector1D) v;
			return new Vector1D(x - factor * v1.X);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Vector1D normalize() throws mathlib.exception.MathArithmeticException
		public virtual Vector1D normalize()
		{
			double s = Norm;
			if (s == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.CANNOT_NORMALIZE_A_ZERO_NORM_VECTOR);
			}
			return scalarMultiply(1 / s);
		}
		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D negate()
		{
			return new Vector1D(-x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector1D scalarMultiply(double a)
		{
			return new Vector1D(a * x);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(x);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Infinite
		{
			get
			{
				return !NaN && double.IsInfinity(x);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance1(Vector<Euclidean1D> p)
		{
			Vector1D p3 = (Vector1D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = mathlib.util.FastMath.abs(p3.x - x);
			double dx = FastMath.abs(p3.x - x);
			return dx;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// @deprecated as of 3.3, replaced with <seealso cref="#distance(Point)"/> 
		[Obsolete("as of 3.3, replaced with <seealso cref="#distance(mathlib.geometry.Point)"/>")]
		public virtual double distance(Vector<Euclidean1D> p)
		{
			return distance((Point<Euclidean1D>) p);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance(Point<Euclidean1D> p)
		{
			Vector1D p3 = (Vector1D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = p3.x - x;
			double dx = p3.x - x;
			return FastMath.abs(dx);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distanceInf(Vector<Euclidean1D> p)
		{
			Vector1D p3 = (Vector1D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = mathlib.util.FastMath.abs(p3.x - x);
			double dx = FastMath.abs(p3.x - x);
			return dx;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distanceSq(Vector<Euclidean1D> p)
		{
			Vector1D p3 = (Vector1D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = p3.x - x;
			double dx = p3.x - x;
			return dx * dx;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double dotProduct(final mathlib.geometry.Vector<Euclidean1D> v)
		public virtual double dotProduct(Vector<Euclidean1D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector1D v1 = (Vector1D) v;
			Vector1D v1 = (Vector1D) v;
			return x * v1.x;
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>p1.subtract(p2).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="p1"> first vector </param>
		/// <param name="p2"> second vector </param>
		/// <returns> the distance between p1 and p2 according to the L<sub>2</sub> norm </returns>
		public static double distance(Vector1D p1, Vector1D p2)
		{
			return p1.distance(p2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>p1.subtract(p2).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="p1"> first vector </param>
		/// <param name="p2"> second vector </param>
		/// <returns> the distance between p1 and p2 according to the L<sub>&infin;</sub> norm </returns>
		public static double distanceInf(Vector1D p1, Vector1D p2)
		{
			return p1.distanceInf(p2);
		}

		/// <summary>
		/// Compute the square of the distance between two vectors.
		/// <p>Calling this method is equivalent to calling:
		/// <code>p1.subtract(p2).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="p1"> first vector </param>
		/// <param name="p2"> second vector </param>
		/// <returns> the square of the distance between p1 and p2 </returns>
		public static double distanceSq(Vector1D p1, Vector1D p2)
		{
			return p1.distanceSq(p2);
		}

		/// <summary>
		/// Test for the equality of two 1D vectors.
		/// <p>
		/// If all coordinates of two 1D vectors are exactly the same, and none are
		/// <code>Double.NaN</code>, the two 1D vectors are considered to be equal.
		/// </p>
		/// <p>
		/// <code>NaN</code> coordinates are considered to affect globally the vector
		/// and be equals to each other - i.e, if either (or all) coordinates of the
		/// 1D vector are equal to <code>Double.NaN</code>, the 1D vector is equal to
		/// <seealso cref="#NaN"/>.
		/// </p>
		/// </summary>
		/// <param name="other"> Object to test for equality to this </param>
		/// <returns> true if two 1D vector objects are equal, false if
		///         object is null, not an instance of Vector1D, or
		///         not equal to this Vector1D instance
		///  </returns>
		public override bool Equals(object other)
		{

			if (this == other)
			{
				return true;
			}

			if (other is Vector1D)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector1D rhs = (Vector1D)other;
				Vector1D rhs = (Vector1D)other;
				if (rhs.NaN)
				{
					return this.NaN;
				}

				return x == rhs.x;
			}
			return false;
		}

		/// <summary>
		/// Get a hashCode for the 1D vector.
		/// <p>
		/// All NaN values have the same hash code.</p>
		/// </summary>
		/// <returns> a hash code value for this object </returns>
		public override int GetHashCode()
		{
			if (NaN)
			{
				return 7785;
			}
			return 997 * MathUtils.hash(x);
		}

		/// <summary>
		/// Get a string representation of this vector. </summary>
		/// <returns> a string representation of this vector </returns>
		public override string ToString()
		{
			return Vector1DFormat.Instance.format(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public String toString(final java.text.NumberFormat format)
		public virtual string ToString(NumberFormat format)
		{
			return (new Vector1DFormat(format)).format(this);
		}

	}

}