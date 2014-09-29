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


	using mathlib;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// This class is a re-implementation of <seealso cref="Vector3D"/> using <seealso cref="RealFieldElement"/>.
	/// <p>Instance of this class are guaranteed to be immutable.</p> </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: FieldVector3D.java 1591835 2014-05-02 09:04:01Z tn $
	/// @since 3.2 </param>
	[Serializable]
	public class FieldVector3D<T> where T : mathlib.RealFieldElement<T>
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20130224L;

		/// <summary>
		/// Abscissa. </summary>
		private readonly T x;

		/// <summary>
		/// Ordinate. </summary>
		private readonly T y;

		/// <summary>
		/// Height. </summary>
		private readonly T z;

		/// <summary>
		/// Simple constructor.
		/// Build a vector from its coordinates </summary>
		/// <param name="x"> abscissa </param>
		/// <param name="y"> ordinate </param>
		/// <param name="z"> height </param>
		/// <seealso cref= #getX() </seealso>
		/// <seealso cref= #getY() </seealso>
		/// <seealso cref= #getZ() </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T x, final T y, final T z)
		public FieldVector3D(T x, T y, T z)
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
//ORIGINAL LINE: public FieldVector3D(final T[] v) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FieldVector3D(T[] v)
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T alpha, final T delta)
		public FieldVector3D(T alpha, T delta)
		{
			T cosDelta = delta.cos();
			this.x = alpha.cos().multiply(cosDelta);
			this.y = alpha.sin().multiply(cosDelta);
			this.z = delta.sin();
		}

		/// <summary>
		/// Multiplicative constructor
		/// Build a vector from another one and a scale factor.
		/// The vector built will be a * u </summary>
		/// <param name="a"> scale factor </param>
		/// <param name="u"> base (unscaled) vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a, final FieldVector3D<T>u)
		public FieldVector3D(T a, FieldVector3D<T>u)
		{
			this.x = a.multiply(u.x);
			this.y = a.multiply(u.y);
			this.z = a.multiply(u.z);
		}

		/// <summary>
		/// Multiplicative constructor
		/// Build a vector from another one and a scale factor.
		/// The vector built will be a * u </summary>
		/// <param name="a"> scale factor </param>
		/// <param name="u"> base (unscaled) vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a, final Vector3D u)
		public FieldVector3D(T a, Vector3D u)
		{
			this.x = a.multiply(u.X);
			this.y = a.multiply(u.Y);
			this.z = a.multiply(u.Z);
		}

		/// <summary>
		/// Multiplicative constructor
		/// Build a vector from another one and a scale factor.
		/// The vector built will be a * u </summary>
		/// <param name="a"> scale factor </param>
		/// <param name="u"> base (unscaled) vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final double a, final FieldVector3D<T> u)
		public FieldVector3D(double a, FieldVector3D<T> u)
		{
			this.x = u.x.multiply(a);
			this.y = u.y.multiply(a);
			this.z = u.z.multiply(a);
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from two other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a1, final FieldVector3D<T> u1, final T a2, final FieldVector3D<T> u2)
		public FieldVector3D(T a1, FieldVector3D<T> u1, T a2, FieldVector3D<T> u2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = a1;
			T prototype = a1;
			this.x = prototype.linearCombination(a1, u1.X, a2, u2.X);
			this.y = prototype.linearCombination(a1, u1.Y, a2, u2.Y);
			this.z = prototype.linearCombination(a1, u1.Z, a2, u2.Z);
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from two other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a1, final Vector3D u1, final T a2, final Vector3D u2)
		public FieldVector3D(T a1, Vector3D u1, T a2, Vector3D u2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = a1;
			T prototype = a1;
			this.x = prototype.linearCombination(u1.X, a1, u2.X, a2);
			this.y = prototype.linearCombination(u1.Y, a1, u2.Y, a2);
			this.z = prototype.linearCombination(u1.Z, a1, u2.Z, a2);
		}

		/// <summary>
		/// Linear constructor
		/// Build a vector from two other ones and corresponding scale factors.
		/// The vector built will be a1 * u1 + a2 * u2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="u1"> first base (unscaled) vector </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="u2"> second base (unscaled) vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final double a1, final FieldVector3D<T> u1, final double a2, final FieldVector3D<T> u2)
		public FieldVector3D(double a1, FieldVector3D<T> u1, double a2, FieldVector3D<T> u2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = u1.getX();
			T prototype = u1.X;
			this.x = prototype.linearCombination(a1, u1.X, a2, u2.X);
			this.y = prototype.linearCombination(a1, u1.Y, a2, u2.Y);
			this.z = prototype.linearCombination(a1, u1.Z, a2, u2.Z);
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a1, final FieldVector3D<T> u1, final T a2, final FieldVector3D<T> u2, final T a3, final FieldVector3D<T> u3)
		public FieldVector3D(T a1, FieldVector3D<T> u1, T a2, FieldVector3D<T> u2, T a3, FieldVector3D<T> u3)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = a1;
			T prototype = a1;
			this.x = prototype.linearCombination(a1, u1.X, a2, u2.X, a3, u3.X);
			this.y = prototype.linearCombination(a1, u1.Y, a2, u2.Y, a3, u3.Y);
			this.z = prototype.linearCombination(a1, u1.Z, a2, u2.Z, a3, u3.Z);
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a1, final Vector3D u1, final T a2, final Vector3D u2, final T a3, final Vector3D u3)
		public FieldVector3D(T a1, Vector3D u1, T a2, Vector3D u2, T a3, Vector3D u3)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = a1;
			T prototype = a1;
			this.x = prototype.linearCombination(u1.X, a1, u2.X, a2, u3.X, a3);
			this.y = prototype.linearCombination(u1.Y, a1, u2.Y, a2, u3.Y, a3);
			this.z = prototype.linearCombination(u1.Z, a1, u2.Z, a2, u3.Z, a3);
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final double a1, final FieldVector3D<T> u1, final double a2, final FieldVector3D<T> u2, final double a3, final FieldVector3D<T> u3)
		public FieldVector3D(double a1, FieldVector3D<T> u1, double a2, FieldVector3D<T> u2, double a3, FieldVector3D<T> u3)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = u1.getX();
			T prototype = u1.X;
			this.x = prototype.linearCombination(a1, u1.X, a2, u2.X, a3, u3.X);
			this.y = prototype.linearCombination(a1, u1.Y, a2, u2.Y, a3, u3.Y);
			this.z = prototype.linearCombination(a1, u1.Z, a2, u2.Z, a3, u3.Z);
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a1, final FieldVector3D<T> u1, final T a2, final FieldVector3D<T> u2, final T a3, final FieldVector3D<T> u3, final T a4, final FieldVector3D<T> u4)
		public FieldVector3D(T a1, FieldVector3D<T> u1, T a2, FieldVector3D<T> u2, T a3, FieldVector3D<T> u3, T a4, FieldVector3D<T> u4)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = a1;
			T prototype = a1;
			this.x = prototype.linearCombination(a1, u1.X, a2, u2.X, a3, u3.X, a4, u4.X);
			this.y = prototype.linearCombination(a1, u1.Y, a2, u2.Y, a3, u3.Y, a4, u4.Y);
			this.z = prototype.linearCombination(a1, u1.Z, a2, u2.Z, a3, u3.Z, a4, u4.Z);
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final T a1, final Vector3D u1, final T a2, final Vector3D u2, final T a3, final Vector3D u3, final T a4, final Vector3D u4)
		public FieldVector3D(T a1, Vector3D u1, T a2, Vector3D u2, T a3, Vector3D u3, T a4, Vector3D u4)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = a1;
			T prototype = a1;
			this.x = prototype.linearCombination(u1.X, a1, u2.X, a2, u3.X, a3, u4.X, a4);
			this.y = prototype.linearCombination(u1.Y, a1, u2.Y, a2, u3.Y, a3, u4.Y, a4);
			this.z = prototype.linearCombination(u1.Z, a1, u2.Z, a2, u3.Z, a3, u4.Z, a4);
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D(final double a1, final FieldVector3D<T> u1, final double a2, final FieldVector3D<T> u2, final double a3, final FieldVector3D<T> u3, final double a4, final FieldVector3D<T> u4)
		public FieldVector3D(double a1, FieldVector3D<T> u1, double a2, FieldVector3D<T> u2, double a3, FieldVector3D<T> u3, double a4, FieldVector3D<T> u4)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T prototype = u1.getX();
			T prototype = u1.X;
			this.x = prototype.linearCombination(a1, u1.X, a2, u2.X, a3, u3.X, a4, u4.X);
			this.y = prototype.linearCombination(a1, u1.Y, a2, u2.Y, a3, u3.Y, a4, u4.Y);
			this.z = prototype.linearCombination(a1, u1.Z, a2, u2.Z, a3, u3.Z, a4, u4.Z);
		}

		/// <summary>
		/// Get the abscissa of the vector. </summary>
		/// <returns> abscissa of the vector </returns>
		/// <seealso cref= #FieldVector3D(RealFieldElement, RealFieldElement, RealFieldElement) </seealso>
		public virtual T X
		{
			get
			{
				return x;
			}
		}

		/// <summary>
		/// Get the ordinate of the vector. </summary>
		/// <returns> ordinate of the vector </returns>
		/// <seealso cref= #FieldVector3D(RealFieldElement, RealFieldElement, RealFieldElement) </seealso>
		public virtual T Y
		{
			get
			{
				return y;
			}
		}

		/// <summary>
		/// Get the height of the vector. </summary>
		/// <returns> height of the vector </returns>
		/// <seealso cref= #FieldVector3D(RealFieldElement, RealFieldElement, RealFieldElement) </seealso>
		public virtual T Z
		{
			get
			{
				return z;
			}
		}

		/// <summary>
		/// Get the vector coordinates as a dimension 3 array. </summary>
		/// <returns> vector coordinates </returns>
		/// <seealso cref= #FieldVector3D(RealFieldElement[]) </seealso>
		public virtual T[] toArray()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] array = mathlib.util.MathArrays.buildArray(x.getField(), 3);
			T[] array = MathArrays.buildArray(x.Field, 3);
			array[0] = x;
			array[1] = y;
			array[2] = z;
			return array;
		}

		/// <summary>
		/// Convert to a constant vector without derivatives. </summary>
		/// <returns> a constant vector </returns>
		public virtual Vector3D toVector3D()
		{
			return new Vector3D(x.Real, y.Real, z.Real);
		}

		/// <summary>
		/// Get the L<sub>1</sub> norm for the vector. </summary>
		/// <returns> L<sub>1</sub> norm for the vector </returns>
		public virtual T Norm1
		{
			get
			{
				return x.abs().add(y.abs()).add(z.abs());
			}
		}

		/// <summary>
		/// Get the L<sub>2</sub> norm for the vector. </summary>
		/// <returns> Euclidean norm for the vector </returns>
		public virtual T Norm
		{
			get
			{
				// there are no cancellation problems here, so we use the straightforward formula
				return x.multiply(x).add(y.multiply(y)).add(z.multiply(z)).sqrt();
			}
		}

		/// <summary>
		/// Get the square of the norm for the vector. </summary>
		/// <returns> square of the Euclidean norm for the vector </returns>
		public virtual T NormSq
		{
			get
			{
				// there are no cancellation problems here, so we use the straightforward formula
				return x.multiply(x).add(y.multiply(y)).add(z.multiply(z));
			}
		}

		/// <summary>
		/// Get the L<sub>&infin;</sub> norm for the vector. </summary>
		/// <returns> L<sub>&infin;</sub> norm for the vector </returns>
		public virtual T NormInf
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T xAbs = x.abs();
				T xAbs = x.abs();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T yAbs = y.abs();
				T yAbs = y.abs();
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T zAbs = z.abs();
				T zAbs = z.abs();
				if (xAbs.Real <= yAbs.Real)
				{
					if (yAbs.Real <= zAbs.Real)
					{
						return zAbs;
					}
					else
					{
						return yAbs;
					}
				}
				else
				{
					if (xAbs.Real <= zAbs.Real)
					{
						return zAbs;
					}
					else
					{
						return xAbs;
					}
				}
			}
		}

		/// <summary>
		/// Get the azimuth of the vector. </summary>
		/// <returns> azimuth (&alpha;) of the vector, between -&pi; and +&pi; </returns>
		/// <seealso cref= #FieldVector3D(RealFieldElement, RealFieldElement) </seealso>
		public virtual T Alpha
		{
			get
			{
				return y.atan2(x);
			}
		}

		/// <summary>
		/// Get the elevation of the vector. </summary>
		/// <returns> elevation (&delta;) of the vector, between -&pi;/2 and +&pi;/2 </returns>
		/// <seealso cref= #FieldVector3D(RealFieldElement, RealFieldElement) </seealso>
		public virtual T Delta
		{
			get
			{
				return z.divide(Norm).asin();
			}
		}

		/// <summary>
		/// Add a vector to the instance. </summary>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> add(final FieldVector3D<T> v)
		public virtual FieldVector3D<T> add(FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(x.add(v.x), y.add(v.y), z.add(v.z));
		}

		/// <summary>
		/// Add a vector to the instance. </summary>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> add(final Vector3D v)
		public virtual FieldVector3D<T> add(Vector3D v)
		{
			return new FieldVector3D<T>(x.add(v.X), y.add(v.Y), z.add(v.Z));
		}

		/// <summary>
		/// Add a scaled vector to the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before adding it </param>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> add(final T factor, final FieldVector3D<T> v)
		public virtual FieldVector3D<T> add(T factor, FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(x.Field.One, this, factor, v);
		}

		/// <summary>
		/// Add a scaled vector to the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before adding it </param>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> add(final T factor, final Vector3D v)
		public virtual FieldVector3D<T> add(T factor, Vector3D v)
		{
			return new FieldVector3D<T>(x.add(factor.multiply(v.X)), y.add(factor.multiply(v.Y)), z.add(factor.multiply(v.Z)));
		}

		/// <summary>
		/// Add a scaled vector to the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before adding it </param>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> add(final double factor, final FieldVector3D<T> v)
		public virtual FieldVector3D<T> add(double factor, FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(1.0, this, factor, v);
		}

		/// <summary>
		/// Add a scaled vector to the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before adding it </param>
		/// <param name="v"> vector to add </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> add(final double factor, final Vector3D v)
		public virtual FieldVector3D<T> add(double factor, Vector3D v)
		{
			return new FieldVector3D<T>(x.add(factor * v.X), y.add(factor * v.Y), z.add(factor * v.Z));
		}

		/// <summary>
		/// Subtract a vector from the instance. </summary>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> subtract(final FieldVector3D<T> v)
		public virtual FieldVector3D<T> subtract(FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(x.subtract(v.x), y.subtract(v.y), z.subtract(v.z));
		}

		/// <summary>
		/// Subtract a vector from the instance. </summary>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> subtract(final Vector3D v)
		public virtual FieldVector3D<T> subtract(Vector3D v)
		{
			return new FieldVector3D<T>(x.subtract(v.X), y.subtract(v.Y), z.subtract(v.Z));
		}

		/// <summary>
		/// Subtract a scaled vector from the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before subtracting it </param>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> subtract(final T factor, final FieldVector3D<T> v)
		public virtual FieldVector3D<T> subtract(T factor, FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(x.Field.One, this, factor.negate(), v);
		}

		/// <summary>
		/// Subtract a scaled vector from the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before subtracting it </param>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> subtract(final T factor, final Vector3D v)
		public virtual FieldVector3D<T> subtract(T factor, Vector3D v)
		{
			return new FieldVector3D<T>(x.subtract(factor.multiply(v.X)), y.subtract(factor.multiply(v.Y)), z.subtract(factor.multiply(v.Z)));
		}

		/// <summary>
		/// Subtract a scaled vector from the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before subtracting it </param>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> subtract(final double factor, final FieldVector3D<T> v)
		public virtual FieldVector3D<T> subtract(double factor, FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(1.0, this, -factor, v);
		}

		/// <summary>
		/// Subtract a scaled vector from the instance. </summary>
		/// <param name="factor"> scale factor to apply to v before subtracting it </param>
		/// <param name="v"> vector to subtract </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> subtract(final double factor, final Vector3D v)
		public virtual FieldVector3D<T> subtract(double factor, Vector3D v)
		{
			return new FieldVector3D<T>(x.subtract(factor * v.X), y.subtract(factor * v.Y), z.subtract(factor * v.Z));
		}

		/// <summary>
		/// Get a normalized vector aligned with the instance. </summary>
		/// <returns> a new normalized vector </returns>
		/// <exception cref="MathArithmeticException"> if the norm is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldVector3D<T> normalize() throws mathlib.exception.MathArithmeticException
		public virtual FieldVector3D<T> normalize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = getNorm();
			T s = Norm;
			if (s.Real == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.CANNOT_NORMALIZE_A_ZERO_NORM_VECTOR);
			}
			return scalarMultiply(s.reciprocal());
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
//ORIGINAL LINE: public FieldVector3D<T> orthogonal() throws mathlib.exception.MathArithmeticException
		public virtual FieldVector3D<T> orthogonal()
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double threshold = 0.6 * getNorm().getReal();
			double threshold = 0.6 * Norm.Real;
			if (threshold == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}

			if (FastMath.abs(x.Real) <= threshold)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T inverse = y.multiply(y).add(z.multiply(z)).sqrt().reciprocal();
				T inverse = y.multiply(y).add(z.multiply(z)).sqrt().reciprocal();
				return new FieldVector3D<T>(inverse.Field.Zero, inverse.multiply(z), inverse.multiply(y).negate());
			}
			else if (FastMath.abs(y.Real) <= threshold)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T inverse = x.multiply(x).add(z.multiply(z)).sqrt().reciprocal();
				T inverse = x.multiply(x).add(z.multiply(z)).sqrt().reciprocal();
				return new FieldVector3D<T>(inverse.multiply(z).negate(), inverse.Field.Zero, inverse.multiply(x));
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T inverse = x.multiply(x).add(y.multiply(y)).sqrt().reciprocal();
				T inverse = x.multiply(x).add(y.multiply(y)).sqrt().reciprocal();
				return new FieldVector3D<T>(inverse.multiply(y), inverse.multiply(x).negate(), inverse.Field.Zero);
			}

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
		/// @param <T> the type of the field elements </param>
		/// <returns> angular separation between v1 and v2 </returns>
		/// <exception cref="MathArithmeticException"> if either vector has a null norm </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T angle(final FieldVector3D<T> v1, final FieldVector3D<T> v2) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static T angle<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T normProduct = v1.getNorm().multiply(v2.getNorm());
			T normProduct = v1.Norm.multiply(v2.Norm);
			if (normProduct.Real == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dot = dotProduct(v1, v2);
			T dot = dotProduct(v1, v2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double threshold = normProduct.getReal() * 0.9999;
			double threshold = normProduct.Real * 0.9999;
			if ((dot.Real < -threshold) || (dot.Real > threshold))
			{
				// the vectors are almost aligned, compute using the sine
				FieldVector3D<T> v3 = crossProduct(v1, v2);
				if (dot.Real >= 0)
				{
					return v3.Norm.divide(normProduct).asin();
				}
				return v3.Norm.divide(normProduct).asin().subtract(FastMath.PI).negate();
			}

			// the vectors are sufficiently separated to use the cosine
			return dot.divide(normProduct).acos();

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
		/// @param <T> the type of the field elements </param>
		/// <returns> angular separation between v1 and v2 </returns>
		/// <exception cref="MathArithmeticException"> if either vector has a null norm </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T angle(final FieldVector3D<T> v1, final Vector3D v2) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static T angle<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T normProduct = v1.getNorm().multiply(v2.getNorm());
			T normProduct = v1.Norm.multiply(v2.Norm);
			if (normProduct.Real == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dot = dotProduct(v1, v2);
			T dot = dotProduct(v1, v2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double threshold = normProduct.getReal() * 0.9999;
			double threshold = normProduct.Real * 0.9999;
			if ((dot.Real < -threshold) || (dot.Real > threshold))
			{
				// the vectors are almost aligned, compute using the sine
				FieldVector3D<T> v3 = crossProduct(v1, v2);
				if (dot.Real >= 0)
				{
					return v3.Norm.divide(normProduct).asin();
				}
				return v3.Norm.divide(normProduct).asin().subtract(FastMath.PI).negate();
			}

			// the vectors are sufficiently separated to use the cosine
			return dot.divide(normProduct).acos();

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
		/// @param <T> the type of the field elements </param>
		/// <returns> angular separation between v1 and v2 </returns>
		/// <exception cref="MathArithmeticException"> if either vector has a null norm </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T angle(final Vector3D v1, final FieldVector3D<T> v2) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static T angle<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return angle(v2, v1);
		}

		/// <summary>
		/// Get the opposite of the instance. </summary>
		/// <returns> a new vector which is opposite to the instance </returns>
		public virtual FieldVector3D<T> negate()
		{
			return new FieldVector3D<T>(x.negate(), y.negate(), z.negate());
		}

		/// <summary>
		/// Multiply the instance by a scalar. </summary>
		/// <param name="a"> scalar </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> scalarMultiply(final T a)
		public virtual FieldVector3D<T> scalarMultiply(T a)
		{
			return new FieldVector3D<T>(x.multiply(a), y.multiply(a), z.multiply(a));
		}

		/// <summary>
		/// Multiply the instance by a scalar. </summary>
		/// <param name="a"> scalar </param>
		/// <returns> a new vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> scalarMultiply(final double a)
		public virtual FieldVector3D<T> scalarMultiply(double a)
		{
			return new FieldVector3D<T>(x.multiply(a), y.multiply(a), z.multiply(a));
		}

		/// <summary>
		/// Returns true if any coordinate of this vector is NaN; false otherwise </summary>
		/// <returns>  true if any coordinate of this vector is NaN; false otherwise </returns>
		public virtual bool NaN
		{
			get
			{
				return double.IsNaN(x.Real) || double.IsNaN(y.Real) || double.IsNaN(z.Real);
			}
		}

		/// <summary>
		/// Returns true if any coordinate of this vector is infinite and none are NaN;
		/// false otherwise </summary>
		/// <returns>  true if any coordinate of this vector is infinite and none are NaN;
		/// false otherwise </returns>
		public virtual bool Infinite
		{
			get
			{
				return !NaN && (double.IsInfinity(x.Real) || double.IsInfinity(y.Real) || double.IsInfinity(z.Real));
			}
		}

		/// <summary>
		/// Test for the equality of two 3D vectors.
		/// <p>
		/// If all coordinates of two 3D vectors are exactly the same, and none of their
		/// <seealso cref="RealFieldElement#getReal() real part"/> are <code>NaN</code>, the
		/// two 3D vectors are considered to be equal.
		/// </p>
		/// <p>
		/// <code>NaN</code> coordinates are considered to affect globally the vector
		/// and be equals to each other - i.e, if either (or all) real part of the
		/// coordinates of the 3D vector are <code>NaN</code>, the 3D vector is <code>NaN</code>.
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

			if (other is FieldVector3D)
			{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") final FieldVector3D<T> rhs = (FieldVector3D<T>) other;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
				FieldVector3D<T> rhs = (FieldVector3D<T>) other;
				if (rhs.NaN)
				{
					return this.NaN;
				}

				return x.Equals(rhs.x) && y.Equals(rhs.y) && z.Equals(rhs.z);

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
				return 409;
			}
			return 311 * (107 * x.GetHashCode() + 83 * y.GetHashCode() + z.GetHashCode());
		}

		/// <summary>
		/// Compute the dot-product of the instance and another vector.
		/// <p>
		/// The implementation uses specific multiplication and addition
		/// algorithms to preserve accuracy and reduce cancellation effects.
		/// It should be very accurate even for nearly orthogonal vectors.
		/// </p> </summary>
		/// <seealso cref= MathArrays#linearCombination(double, double, double, double, double, double) </seealso>
		/// <param name="v"> second vector </param>
		/// <returns> the dot product this.v </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T dotProduct(final FieldVector3D<T> v)
		public virtual T dotProduct(FieldVector3D<T> v)
		{
			return x.linearCombination(x, v.x, y, v.y, z, v.z);
		}

		/// <summary>
		/// Compute the dot-product of the instance and another vector.
		/// <p>
		/// The implementation uses specific multiplication and addition
		/// algorithms to preserve accuracy and reduce cancellation effects.
		/// It should be very accurate even for nearly orthogonal vectors.
		/// </p> </summary>
		/// <seealso cref= MathArrays#linearCombination(double, double, double, double, double, double) </seealso>
		/// <param name="v"> second vector </param>
		/// <returns> the dot product this.v </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T dotProduct(final Vector3D v)
		public virtual T dotProduct(Vector3D v)
		{
			return x.linearCombination(v.X, x, v.Y, y, v.Z, z);
		}

		/// <summary>
		/// Compute the cross-product of the instance with another vector. </summary>
		/// <param name="v"> other vector </param>
		/// <returns> the cross product this ^ v as a new Vector3D </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> crossProduct(final FieldVector3D<T> v)
		public virtual FieldVector3D<T> crossProduct(FieldVector3D<T> v)
		{
			return new FieldVector3D<T>(x.linearCombination(y, v.z, z.negate(), v.y), y.linearCombination(z, v.x, x.negate(), v.z), z.linearCombination(x, v.y, y.negate(), v.x));
		}

		/// <summary>
		/// Compute the cross-product of the instance with another vector. </summary>
		/// <param name="v"> other vector </param>
		/// <returns> the cross product this ^ v as a new Vector3D </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> crossProduct(final Vector3D v)
		public virtual FieldVector3D<T> crossProduct(Vector3D v)
		{
			return new FieldVector3D<T>(x.linearCombination(v.Z, y, -v.Y, z), y.linearCombination(v.X, z, -v.Z, x), z.linearCombination(v.Y, x, -v.X, y));
		}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>1</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distance1(final FieldVector3D<T> v)
		public virtual T distance1(FieldVector3D<T> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = v.x.subtract(x).abs();
			T dx = v.x.subtract(x).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = v.y.subtract(y).abs();
			T dy = v.y.subtract(y).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = v.z.subtract(z).abs();
			T dz = v.z.subtract(z).abs();
			return dx.add(dy).add(dz);
		}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>1</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distance1(final Vector3D v)
		public virtual T distance1(Vector3D v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = x.subtract(v.getX()).abs();
			T dx = x.subtract(v.X).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = y.subtract(v.getY()).abs();
			T dy = y.subtract(v.Y).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = z.subtract(v.getZ()).abs();
			T dz = z.subtract(v.Z).abs();
			return dx.add(dy).add(dz);
		}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>2</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distance(final FieldVector3D<T> v)
		public virtual T distance(FieldVector3D<T> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = v.x.subtract(x);
			T dx = v.x.subtract(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = v.y.subtract(y);
			T dy = v.y.subtract(y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = v.z.subtract(z);
			T dz = v.z.subtract(z);
			return dx.multiply(dx).add(dy.multiply(dy)).add(dz.multiply(dz)).sqrt();
		}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>2</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distance(final Vector3D v)
		public virtual T distance(Vector3D v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = x.subtract(v.getX());
			T dx = x.subtract(v.X);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = y.subtract(v.getY());
			T dy = y.subtract(v.Y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = z.subtract(v.getZ());
			T dz = z.subtract(v.Z);
			return dx.multiply(dx).add(dy.multiply(dy)).add(dz.multiply(dz)).sqrt();
		}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>&infin;</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distanceInf(final FieldVector3D<T> v)
		public virtual T distanceInf(FieldVector3D<T> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = v.x.subtract(x).abs();
			T dx = v.x.subtract(x).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = v.y.subtract(y).abs();
			T dy = v.y.subtract(y).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = v.z.subtract(z).abs();
			T dz = v.z.subtract(z).abs();
			if (dx.Real <= dy.Real)
			{
				if (dy.Real <= dz.Real)
				{
					return dz;
				}
				else
				{
					return dy;
				}
			}
			else
			{
				if (dx.Real <= dz.Real)
				{
					return dz;
				}
				else
				{
					return dx;
				}
			}
		}

		/// <summary>
		/// Compute the distance between the instance and another vector according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the distance between the instance and p according to the L<sub>&infin;</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distanceInf(final Vector3D v)
		public virtual T distanceInf(Vector3D v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = x.subtract(v.getX()).abs();
			T dx = x.subtract(v.X).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = y.subtract(v.getY()).abs();
			T dy = y.subtract(v.Y).abs();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = z.subtract(v.getZ()).abs();
			T dz = z.subtract(v.Z).abs();
			if (dx.Real <= dy.Real)
			{
				if (dy.Real <= dz.Real)
				{
					return dz;
				}
				else
				{
					return dy;
				}
			}
			else
			{
				if (dx.Real <= dz.Real)
				{
					return dz;
				}
				else
				{
					return dx;
				}
			}
		}

		/// <summary>
		/// Compute the square of the distance between the instance and another vector.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the square of the distance between the instance and p </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distanceSq(final FieldVector3D<T> v)
		public virtual T distanceSq(FieldVector3D<T> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = v.x.subtract(x);
			T dx = v.x.subtract(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = v.y.subtract(y);
			T dy = v.y.subtract(y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = v.z.subtract(z);
			T dz = v.z.subtract(z);
			return dx.multiply(dx).add(dy.multiply(dy)).add(dz.multiply(dz));
		}

		/// <summary>
		/// Compute the square of the distance between the instance and another vector.
		/// <p>Calling this method is equivalent to calling:
		/// <code>q.subtract(p).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v"> second vector </param>
		/// <returns> the square of the distance between the instance and p </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public T distanceSq(final Vector3D v)
		public virtual T distanceSq(Vector3D v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dx = x.subtract(v.getX());
			T dx = x.subtract(v.X);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dy = y.subtract(v.getY());
			T dy = y.subtract(v.Y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dz = z.subtract(v.getZ());
			T dz = z.subtract(v.Z);
			return dx.multiply(dx).add(dy.multiply(dy)).add(dz.multiply(dz));
		}

		/// <summary>
		/// Compute the dot-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the dot product v1.v2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T dotProduct(final FieldVector3D<T> v1, final FieldVector3D<T> v2)
		public static T dotProduct<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.dotProduct(v2);
		}

		/// <summary>
		/// Compute the dot-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the dot product v1.v2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T dotProduct(final FieldVector3D<T> v1, final Vector3D v2)
		public static T dotProduct<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.dotProduct(v2);
		}

		/// <summary>
		/// Compute the dot-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the dot product v1.v2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T dotProduct(final Vector3D v1, final FieldVector3D<T> v2)
		public static T dotProduct<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v2.dotProduct(v1);
		}

		/// <summary>
		/// Compute the cross-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the cross product v1 ^ v2 as a new Vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldVector3D<T> crossProduct(final FieldVector3D<T> v1, final FieldVector3D<T> v2)
		public static FieldVector3D<T> crossProduct<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.crossProduct(v2);
		}

		/// <summary>
		/// Compute the cross-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the cross product v1 ^ v2 as a new Vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldVector3D<T> crossProduct(final FieldVector3D<T> v1, final Vector3D v2)
		public static FieldVector3D<T> crossProduct<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.crossProduct(v2);
		}

		/// <summary>
		/// Compute the cross-product of two vectors. </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the cross product v1 ^ v2 as a new Vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldVector3D<T> crossProduct(final Vector3D v1, final FieldVector3D<T> v2)
		public static FieldVector3D<T> crossProduct<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return new FieldVector3D<T>(v2.x.linearCombination(v1.Y, v2.z, -v1.Z, v2.y), v2.y.linearCombination(v1.Z, v2.x, -v1.X, v2.z), v2.z.linearCombination(v1.X, v2.y, -v1.Y, v2.x));
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>1</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance1(final FieldVector3D<T> v1, final FieldVector3D<T> v2)
		public static T distance1<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distance1(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>1</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance1(final FieldVector3D<T> v1, final Vector3D v2)
		public static T distance1<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distance1(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>1</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm1()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>1</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance1(final Vector3D v1, final FieldVector3D<T> v2)
		public static T distance1<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v2.distance1(v1);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>2</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance(final FieldVector3D<T> v1, final FieldVector3D<T> v2)
		public static T distance<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distance(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>2</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance(final FieldVector3D<T> v1, final Vector3D v2)
		public static T distance<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distance(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>2</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNorm()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>2</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance(final Vector3D v1, final FieldVector3D<T> v2)
		public static T distance<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v2.distance(v1);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>&infin;</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distanceInf(final FieldVector3D<T> v1, final FieldVector3D<T> v2)
		public static T distanceInf<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distanceInf(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>&infin;</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distanceInf(final FieldVector3D<T> v1, final Vector3D v2)
		public static T distanceInf<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distanceInf(v2);
		}

		/// <summary>
		/// Compute the distance between two vectors according to the L<sub>&infin;</sub> norm.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormInf()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the distance between v1 and v2 according to the L<sub>&infin;</sub> norm </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distanceInf(final Vector3D v1, final FieldVector3D<T> v2)
		public static T distanceInf<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v2.distanceInf(v1);
		}

		/// <summary>
		/// Compute the square of the distance between two vectors.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the square of the distance between v1 and v2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distanceSq(final FieldVector3D<T> v1, final FieldVector3D<T> v2)
		public static T distanceSq<T>(FieldVector3D<T> v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distanceSq(v2);
		}

		/// <summary>
		/// Compute the square of the distance between two vectors.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the square of the distance between v1 and v2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distanceSq(final FieldVector3D<T> v1, final Vector3D v2)
		public static T distanceSq<T>(FieldVector3D<T> v1, Vector3D v2) where T : mathlib.RealFieldElement<T>
		{
			return v1.distanceSq(v2);
		}

		/// <summary>
		/// Compute the square of the distance between two vectors.
		/// <p>Calling this method is equivalent to calling:
		/// <code>v1.subtract(v2).getNormSq()</code> except that no intermediate
		/// vector is built</p> </summary>
		/// <param name="v1"> first vector </param>
		/// <param name="v2"> second vector </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> the square of the distance between v1 and v2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distanceSq(final Vector3D v1, final FieldVector3D<T> v2)
		public static T distanceSq<T>(Vector3D v1, FieldVector3D<T> v2) where T : mathlib.RealFieldElement<T>
		{
			return v2.distanceSq(v1);
		}

		/// <summary>
		/// Get a string representation of this vector. </summary>
		/// <returns> a string representation of this vector </returns>
		public override string ToString()
		{
			return Vector3DFormat.Instance.format(toVector3D());
		}

		/// <summary>
		/// Get a string representation of this vector. </summary>
		/// <param name="format"> the custom format for components </param>
		/// <returns> a string representation of this vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public String toString(final java.text.NumberFormat format)
		public virtual string ToString(NumberFormat format)
		{
			return (new Vector3DFormat(format)).format(toVector3D());
		}

	}

}