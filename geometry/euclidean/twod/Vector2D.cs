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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using mathlib.geometry;
	using mathlib.geometry;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// This class represents a 2D vector.
	/// <p>Instances of this class are guaranteed to be immutable.</p>
	/// @version $Id: Vector2D.java 1563684 2014-02-02 17:51:06Z tn $
	/// @since 3.0
	/// </summary>
	public class Vector2D : Vector<Euclidean2D>
	{

		/// <summary>
		/// Origin (coordinates: 0, 0). </summary>
		public static readonly Vector2D ZERO = new Vector2D(0, 0);

		// CHECKSTYLE: stop ConstantName
		/// <summary>
		/// A vector with all coordinates set to NaN. </summary>
		public static readonly Vector2D NaN_Renamed = new Vector2D(double.NaN, double.NaN);
		// CHECKSTYLE: resume ConstantName

		/// <summary>
		/// A vector with all coordinates set to positive infinity. </summary>
		public static readonly Vector2D POSITIVE_INFINITY = new Vector2D(double.PositiveInfinity, double.PositiveInfinity);

		/// <summary>
		/// A vector with all coordinates set to negative infinity. </summary>
		public static readonly Vector2D NEGATIVE_INFINITY = new Vector2D(double.NegativeInfinity, double.NegativeInfinity);

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 266938651998679754L;

		/// <summary>
		/// Abscissa. </summary>
		private readonly double x;

		/// <summary>
		/// Ordinate. </summary>
		private readonly double y;

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="x"> abscissa </param>
		/// <param name="y"> ordinate </param>
		/// <seealso cref= #getX() </seealso>
		/// <seealso cref= #getY() </seealso>
		public Vector2D(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="v"> coordinates array </param>
		/// <exception cref="DimensionMismatchException"> if array does not have 2 elements </exception>
		/// <seealso cref= #toArray() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Vector2D(double[] v) throws mathlib.exception.DimensionMismatchException
		public Vector2D(double[] v)
		{
			if (v.Length != 2)
			{
				throw new DimensionMismatchException(v.Length, 2);
			}
			this.x = v[0];
			this.y = v[1];
		}

		/// <summary>
		/// Multiplicative constructor
		/// Build a vector from another one and a scale factor.
		/// The vector built will be a * u </summary>
		/// <param name="a"> scale factor </param>
		/// <param name="u"> base (unscaled) vector </param>
		public Vector2D(double a, Vector2D u)
		{
			this.x = a * u.x;
			this.y = a * u.y;
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from two other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
		public Vector2D(double a1, Vector2D u1, double a2, Vector2D u2)
		{
			this.x = a1 * u1.x + a2 * u2.x;
			this.y = a1 * u1.y + a2 * u2.y;
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
		public Vector2D(double a1, Vector2D u1, double a2, Vector2D u2, double a3, Vector2D u3)
		{
			this.x = a1 * u1.x + a2 * u2.x + a3 * u3.x;
			this.y = a1 * u1.y + a2 * u2.y + a3 * u3.y;
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
		public Vector2D(double a1, Vector2D u1, double a2, Vector2D u2, double a3, Vector2D u3, double a4, Vector2D u4)
		{
			this.x = a1 * u1.x + a2 * u2.x + a3 * u3.x + a4 * u4.x;
			this.y = a1 * u1.y + a2 * u2.y + a3 * u3.y + a4 * u4.y;
		}

		/// <summary>
		/// Get the abscissa of the vector. </summary>
		/// <returns> abscissa of the vector </returns>
		/// <seealso cref= #Vector2D(double, double) </seealso>
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
		/// <seealso cref= #Vector2D(double, double) </seealso>
		public virtual double Y
		{
			get
			{
				return y;
			}
		}

		/// <summary>
		/// Get the vector coordinates as a dimension 2 array. </summary>
		/// <returns> vector coordinates </returns>
		/// <seealso cref= #Vector2D(double[]) </seealso>
		public virtual double[] toArray()
		{
			return new double[] {x, y};
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Space Space
		{
			get
			{
				return Euclidean2D.Instance;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D Zero
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
				return FastMath.abs(x) + FastMath.abs(y);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Norm
		{
			get
			{
				return FastMath.sqrt(x * x + y * y);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double NormSq
		{
			get
			{
				return x * x + y * y;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double NormInf
		{
			get
			{
				return FastMath.max(FastMath.abs(x), FastMath.abs(y));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D add(Vector<Euclidean2D> v)
		{
			Vector2D v2 = (Vector2D) v;
			return new Vector2D(x + v2.X, y + v2.Y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D add(double factor, Vector<Euclidean2D> v)
		{
			Vector2D v2 = (Vector2D) v;
			return new Vector2D(x + factor * v2.X, y + factor * v2.Y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D subtract(Vector<Euclidean2D> p)
		{
			Vector2D p3 = (Vector2D) p;
			return new Vector2D(x - p3.x, y - p3.y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D subtract(double factor, Vector<Euclidean2D> v)
		{
			Vector2D v2 = (Vector2D) v;
			return new Vector2D(x - factor * v2.X, y - factor * v2.Y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Vector2D normalize() throws mathlib.exception.MathArithmeticException
		public virtual Vector2D normalize()
		{
			double s = Norm;
			if (s == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.CANNOT_NORMALIZE_A_ZERO_NORM_VECTOR);
			}
			return scalarMultiply(1 / s);
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
//ORIGINAL LINE: public static double angle(Vector2D v1, Vector2D v2) throws mathlib.exception.MathArithmeticException
		public static double angle(Vector2D v1, Vector2D v2)
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
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = mathlib.util.FastMath.abs(mathlib.util.MathArrays.linearCombination(v1.x, v2.y, -v1.y, v2.x));
				double n = FastMath.abs(MathArrays.linearCombination(v1.x, v2.y, -v1.y, v2.x));
				if (dot >= 0)
				{
					return FastMath.asin(n / normProduct);
				}
				return FastMath.PI - FastMath.asin(n / normProduct);
			}

			// the vectors are sufficiently separated to use the cosine
			return FastMath.acos(dot / normProduct);

		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D negate()
		{
			return new Vector2D(-x, -y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual Vector2D scalarMultiply(double a)
		{
			return new Vector2D(a * x, a * y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(x) || double.IsNaN(y);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool Infinite
		{
			get
			{
				return !NaN && (double.IsInfinity(x) || double.IsInfinity(y));
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance1(Vector<Euclidean2D> p)
		{
			Vector2D p3 = (Vector2D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = mathlib.util.FastMath.abs(p3.x - x);
			double dx = FastMath.abs(p3.x - x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = mathlib.util.FastMath.abs(p3.y - y);
			double dy = FastMath.abs(p3.y - y);
			return dx + dy;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double distance(Vector<Euclidean2D> p)
		{
			return distance((Point<Euclidean2D>) p);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distance(Point<Euclidean2D> p)
		{
			Vector2D p3 = (Vector2D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = p3.x - x;
			double dx = p3.x - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = p3.y - y;
			double dy = p3.y - y;
			return FastMath.sqrt(dx * dx + dy * dy);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distanceInf(Vector<Euclidean2D> p)
		{
			Vector2D p3 = (Vector2D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = mathlib.util.FastMath.abs(p3.x - x);
			double dx = FastMath.abs(p3.x - x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = mathlib.util.FastMath.abs(p3.y - y);
			double dy = FastMath.abs(p3.y - y);
			return FastMath.max(dx, dy);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double distanceSq(Vector<Euclidean2D> p)
		{
			Vector2D p3 = (Vector2D) p;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = p3.x - x;
			double dx = p3.x - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = p3.y - y;
			double dy = p3.y - y;
			return dx * dx + dy * dy;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double dotProduct(final mathlib.geometry.Vector<Euclidean2D> v)
		public virtual double dotProduct(Vector<Euclidean2D> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D v2 = (Vector2D) v;
			Vector2D v2 = (Vector2D) v;
			return MathArrays.linearCombination(x, v2.x, y, v2.y);
		}

		/// <summary>
		/// Compute the cross-product of the instance and the given points.
		/// <p>
		/// The cross product can be used to determine the location of a point
		/// with regard to the line formed by (p1, p2) and is calculated as:
		/// \[
		///    P = (x_2 - x_1)(y_3 - y_1) - (y_2 - y_1)(x_3 - x_1)
		/// \]
		/// with \(p3 = (x_3, y_3)\) being this instance.
		/// <p>
		/// If the result is 0, the points are collinear, i.e. lie on a single straight line L;
		/// if it is positive, this point lies to the left, otherwise to the right of the line
		/// formed by (p1, p2).
		/// </summary>
		/// <param name="p1"> first point of the line </param>
		/// <param name="p2"> second point of the line </param>
		/// <returns> the cross-product
		/// </returns>
		/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Cross_product">Cross product (Wikipedia)</a> </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double crossProduct(final Vector2D p1, final Vector2D p2)
		public virtual double crossProduct(Vector2D p1, Vector2D p2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x1 = p2.getX() - p1.getX();
			double x1 = p2.X - p1.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y1 = getY() - p1.getY();
			double y1 = Y - p1.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = getX() - p1.getX();
			double x2 = X - p1.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = p2.getY() - p1.getY();
			double y2 = p2.Y - p1.Y;
			return MathArrays.linearCombination(x1, y1, -x2, y2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>p1.subtract(p2).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="p1"> first vector </param>
		/// <param name="p2"> second vector </param>
		/// <returns> the distance between p1 and p2 according to the L<sub>2</sub> norm </returns>
		public static double distance(Vector2D p1, Vector2D p2)
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
		public static double distanceInf(Vector2D p1, Vector2D p2)
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
		public static double distanceSq(Vector2D p1, Vector2D p2)
		{
			return p1.distanceSq(p2);
		}

		/// <summary>
		/// Test for the equality of two 2D vectors.
		/// <p>
		/// If all coordinates of two 2D vectors are exactly the same, and none are
		/// <code>Double.NaN</code>, the two 2D vectors are considered to be equal.
		/// </p>
		/// <p>
		/// <code>NaN</code> coordinates are considered to affect globally the vector
		/// and be equals to each other - i.e, if either (or all) coordinates of the
		/// 2D vector are equal to <code>Double.NaN</code>, the 2D vector is equal to
		/// <seealso cref="#NaN"/>.
		/// </p>
		/// </summary>
		/// <param name="other"> Object to test for equality to this </param>
		/// <returns> true if two 2D vector objects are equal, false if
		///         object is null, not an instance of Vector2D, or
		///         not equal to this Vector2D instance
		///  </returns>
		public override bool Equals(object other)
		{

			if (this == other)
			{
				return true;
			}

			if (other is Vector2D)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D rhs = (Vector2D)other;
				Vector2D rhs = (Vector2D)other;
				if (rhs.NaN)
				{
					return this.NaN;
				}

				return (x == rhs.x) && (y == rhs.y);
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
			return 122 * (76 * MathUtils.hash(x) + MathUtils.hash(y));
		}

		/// <summary>
		/// Get a string representation of this vector. </summary>
		/// <returns> a string representation of this vector </returns>
		public override string ToString()
		{
			return Vector2DFormat.Instance.format(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public String toString(final java.text.NumberFormat format)
		public virtual string ToString(NumberFormat format)
		{
			return (new Vector2DFormat(format)).format(this);
		}

	}

}