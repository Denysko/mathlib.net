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
	using mathlib;
	using MathArithmeticException = mathlib.exception.MathArithmeticException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// This class is a re-implementation of <seealso cref="Rotation"/> using <seealso cref="RealFieldElement"/>.
	/// <p>Instance of this class are guaranteed to be immutable.</p>
	/// </summary>
	/// @param <T> the type of the field elements
	/// @version $Id: FieldRotation.java 1591835 2014-05-02 09:04:01Z tn $ </param>
	/// <seealso cref= FieldVector3D </seealso>
	/// <seealso cref= RotationOrder
	/// @since 3.2 </seealso>

	[Serializable]
	public class FieldRotation<T> where T : mathlib.RealFieldElement<T>
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 20130224L;

		/// <summary>
		/// Scalar coordinate of the quaternion. </summary>
		private readonly T q0;

		/// <summary>
		/// First coordinate of the vectorial part of the quaternion. </summary>
		private readonly T q1;

		/// <summary>
		/// Second coordinate of the vectorial part of the quaternion. </summary>
		private readonly T q2;

		/// <summary>
		/// Third coordinate of the vectorial part of the quaternion. </summary>
		private readonly T q3;

		/// <summary>
		/// Build a rotation from the quaternion coordinates.
		/// <p>A rotation can be built from a <em>normalized</em> quaternion,
		/// i.e. a quaternion for which q<sub>0</sub><sup>2</sup> +
		/// q<sub>1</sub><sup>2</sup> + q<sub>2</sub><sup>2</sup> +
		/// q<sub>3</sub><sup>2</sup> = 1. If the quaternion is not normalized,
		/// the constructor can normalize it in a preprocessing step.</p>
		/// <p>Note that some conventions put the scalar part of the quaternion
		/// as the 4<sup>th</sup> component and the vector part as the first three
		/// components. This is <em>not</em> our convention. We put the scalar part
		/// as the first component.</p> </summary>
		/// <param name="q0"> scalar part of the quaternion </param>
		/// <param name="q1"> first coordinate of the vectorial part of the quaternion </param>
		/// <param name="q2"> second coordinate of the vectorial part of the quaternion </param>
		/// <param name="q3"> third coordinate of the vectorial part of the quaternion </param>
		/// <param name="needsNormalization"> if true, the coordinates are considered
		/// not to be normalized, a normalization preprocessing step is performed
		/// before using them </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldRotation(final T q0, final T q1, final T q2, final T q3, final boolean needsNormalization)
		public FieldRotation(T q0, T q1, T q2, T q3, bool needsNormalization)
		{

			if (needsNormalization)
			{
				// normalization preprocessing
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T inv = q0.multiply(q0).add(q1.multiply(q1)).add(q2.multiply(q2)).add(q3.multiply(q3)).sqrt().reciprocal();
				T inv = q0.multiply(q0).add(q1.multiply(q1)).add(q2.multiply(q2)).add(q3.multiply(q3)).sqrt().reciprocal();
				this.q0 = inv.multiply(q0);
				this.q1 = inv.multiply(q1);
				this.q2 = inv.multiply(q2);
				this.q3 = inv.multiply(q3);
			}
			else
			{
				this.q0 = q0;
				this.q1 = q1;
				this.q2 = q2;
				this.q3 = q3;
			}

		}

		/// <summary>
		/// Build a rotation from an axis and an angle.
		/// <p>We use the convention that angles are oriented according to
		/// the effect of the rotation on vectors around the axis. That means
		/// that if (i, j, k) is a direct frame and if we first provide +k as
		/// the axis and &pi;/2 as the angle to this constructor, and then
		/// <seealso cref="#applyTo(FieldVector3D) apply"/> the instance to +i, we will get
		/// +j.</p>
		/// <p>Another way to represent our convention is to say that a rotation
		/// of angle &theta; about the unit vector (x, y, z) is the same as the
		/// rotation build from quaternion components { cos(-&theta;/2),
		/// x * sin(-&theta;/2), y * sin(-&theta;/2), z * sin(-&theta;/2) }.
		/// Note the minus sign on the angle!</p>
		/// <p>On the one hand this convention is consistent with a vectorial
		/// perspective (moving vectors in fixed frames), on the other hand it
		/// is different from conventions with a frame perspective (fixed vectors
		/// viewed from different frames) like the ones used for example in spacecraft
		/// attitude community or in the graphics community.</p> </summary>
		/// <param name="axis"> axis around which to rotate </param>
		/// <param name="angle"> rotation angle. </param>
		/// <exception cref="MathIllegalArgumentException"> if the axis norm is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldRotation(final FieldVector3D<T> axis, final T angle) throws mathlib.exception.MathIllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FieldRotation(FieldVector3D<T> axis, T angle)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T norm = axis.getNorm();
			T norm = axis.Norm;
			if (norm.Real == 0)
			{
				throw new MathIllegalArgumentException(LocalizedFormats.ZERO_NORM_FOR_ROTATION_AXIS);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T halfAngle = angle.multiply(-0.5);
			T halfAngle = angle.multiply(-0.5);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T coeff = halfAngle.sin().divide(norm);
			T coeff = halfAngle.sin().divide(norm);

			q0 = halfAngle.cos();
			q1 = coeff.multiply(axis.X);
			q2 = coeff.multiply(axis.Y);
			q3 = coeff.multiply(axis.Z);

		}

		/// <summary>
		/// Build a rotation from a 3X3 matrix.
		/// 
		/// <p>Rotation matrices are orthogonal matrices, i.e. unit matrices
		/// (which are matrices for which m.m<sup>T</sup> = I) with real
		/// coefficients. The module of the determinant of unit matrices is
		/// 1, among the orthogonal 3X3 matrices, only the ones having a
		/// positive determinant (+1) are rotation matrices.</p>
		/// 
		/// <p>When a rotation is defined by a matrix with truncated values
		/// (typically when it is extracted from a technical sheet where only
		/// four to five significant digits are available), the matrix is not
		/// orthogonal anymore. This constructor handles this case
		/// transparently by using a copy of the given matrix and applying a
		/// correction to the copy in order to perfect its orthogonality. If
		/// the Frobenius norm of the correction needed is above the given
		/// threshold, then the matrix is considered to be too far from a
		/// true rotation matrix and an exception is thrown.<p>
		/// </summary>
		/// <param name="m"> rotation matrix </param>
		/// <param name="threshold"> convergence threshold for the iterative
		/// orthogonality correction (convergence is reached when the
		/// difference between two steps of the Frobenius norm of the
		/// correction is below this threshold)
		/// </param>
		/// <exception cref="NotARotationMatrixException"> if the matrix is not a 3X3
		/// matrix, or if it cannot be transformed into an orthogonal matrix
		/// with the given threshold, or if the determinant of the resulting
		/// orthogonal matrix is negative
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldRotation(final T[][] m, final double threshold) throws NotARotationMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FieldRotation(T[][] m, double threshold)
		{

			// dimension check
			if ((m.Length != 3) || (m[0].Length != 3) || (m[1].Length != 3) || (m[2].Length != 3))
			{
				throw new NotARotationMatrixException(LocalizedFormats.ROTATION_MATRIX_DIMENSIONS, m.Length, m[0].Length);
			}

			// compute a "close" orthogonal matrix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] ort = orthogonalizeMatrix(m, threshold);
			T[][] ort = orthogonalizeMatrix(m, threshold);

			// check the sign of the determinant
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T d0 = ort[1][1].multiply(ort[2][2]).subtract(ort[2][1].multiply(ort[1][2]));
			T d0 = ort[1][1].multiply(ort[2][2]).subtract(ort[2][1].multiply(ort[1][2]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T d1 = ort[0][1].multiply(ort[2][2]).subtract(ort[2][1].multiply(ort[0][2]));
			T d1 = ort[0][1].multiply(ort[2][2]).subtract(ort[2][1].multiply(ort[0][2]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T d2 = ort[0][1].multiply(ort[1][2]).subtract(ort[1][1].multiply(ort[0][2]));
			T d2 = ort[0][1].multiply(ort[1][2]).subtract(ort[1][1].multiply(ort[0][2]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T det = ort[0][0].multiply(d0).subtract(ort[1][0].multiply(d1)).add(ort[2][0].multiply(d2));
			T det = ort[0][0].multiply(d0).subtract(ort[1][0].multiply(d1)).add(ort[2][0].multiply(d2));
			if (det.Real < 0.0)
			{
				throw new NotARotationMatrixException(LocalizedFormats.CLOSEST_ORTHOGONAL_MATRIX_HAS_NEGATIVE_DETERMINANT, det);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] quat = mat2quat(ort);
			T[] quat = mat2quat(ort);
			q0 = quat[0];
			q1 = quat[1];
			q2 = quat[2];
			q3 = quat[3];

		}

		/// <summary>
		/// Build the rotation that transforms a pair of vector into another pair.
		/// 
		/// <p>Except for possible scale factors, if the instance were applied to
		/// the pair (u<sub>1</sub>, u<sub>2</sub>) it will produce the pair
		/// (v<sub>1</sub>, v<sub>2</sub>).</p>
		/// 
		/// <p>If the angular separation between u<sub>1</sub> and u<sub>2</sub> is
		/// not the same as the angular separation between v<sub>1</sub> and
		/// v<sub>2</sub>, then a corrected v'<sub>2</sub> will be used rather than
		/// v<sub>2</sub>, the corrected vector will be in the (v<sub>1</sub>,
		/// v<sub>2</sub>) plane.</p>
		/// </summary>
		/// <param name="u1"> first vector of the origin pair </param>
		/// <param name="u2"> second vector of the origin pair </param>
		/// <param name="v1"> desired image of u1 by the rotation </param>
		/// <param name="v2"> desired image of u2 by the rotation </param>
		/// <exception cref="MathArithmeticException"> if the norm of one of the vectors is zero,
		/// or if one of the pair is degenerated (i.e. the vectors of the pair are colinear) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldRotation(FieldVector3D<T> u1, FieldVector3D<T> u2, FieldVector3D<T> v1, FieldVector3D<T> v2) throws mathlib.exception.MathArithmeticException
		public FieldRotation(FieldVector3D<T> u1, FieldVector3D<T> u2, FieldVector3D<T> v1, FieldVector3D<T> v2)
		{

			// build orthonormalized base from u1, u2
			// this fails when vectors are null or colinear, which is forbidden to define a rotation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> u3 = FieldVector3D.crossProduct(u1, u2).normalize();
			FieldVector3D<T> u3 = FieldVector3D.crossProduct(u1, u2).normalize();
			u2 = FieldVector3D.crossProduct(u3, u1).normalize();
			u1 = u1.normalize();

			// build an orthonormalized base from v1, v2
			// this fails when vectors are null or colinear, which is forbidden to define a rotation
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v3 = FieldVector3D.crossProduct(v1, v2).normalize();
			FieldVector3D<T> v3 = FieldVector3D.crossProduct(v1, v2).normalize();
			v2 = FieldVector3D.crossProduct(v3, v1).normalize();
			v1 = v1.normalize();

			// buid a matrix transforming the first base into the second one
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] array = mathlib.util.MathArrays.buildArray(u1.getX().getField(), 3, 3);
			T[][] array = MathArrays.buildArray(u1.X.Field, 3, 3);
			array[0][0] = u1.X.multiply(v1.X).add(u2.X.multiply(v2.X)).add(u3.X.multiply(v3.X));
			array[0][1] = u1.Y.multiply(v1.X).add(u2.Y.multiply(v2.X)).add(u3.Y.multiply(v3.X));
			array[0][2] = u1.Z.multiply(v1.X).add(u2.Z.multiply(v2.X)).add(u3.Z.multiply(v3.X));
			array[1][0] = u1.X.multiply(v1.Y).add(u2.X.multiply(v2.Y)).add(u3.X.multiply(v3.Y));
			array[1][1] = u1.Y.multiply(v1.Y).add(u2.Y.multiply(v2.Y)).add(u3.Y.multiply(v3.Y));
			array[1][2] = u1.Z.multiply(v1.Y).add(u2.Z.multiply(v2.Y)).add(u3.Z.multiply(v3.Y));
			array[2][0] = u1.X.multiply(v1.Z).add(u2.X.multiply(v2.Z)).add(u3.X.multiply(v3.Z));
			array[2][1] = u1.Y.multiply(v1.Z).add(u2.Y.multiply(v2.Z)).add(u3.Y.multiply(v3.Z));
			array[2][2] = u1.Z.multiply(v1.Z).add(u2.Z.multiply(v2.Z)).add(u3.Z.multiply(v3.Z));

			T[] quat = mat2quat(array);
			q0 = quat[0];
			q1 = quat[1];
			q2 = quat[2];
			q3 = quat[3];

		}

		/// <summary>
		/// Build one of the rotations that transform one vector into another one.
		/// 
		/// <p>Except for a possible scale factor, if the instance were
		/// applied to the vector u it will produce the vector v. There is an
		/// infinite number of such rotations, this constructor choose the
		/// one with the smallest associated angle (i.e. the one whose axis
		/// is orthogonal to the (u, v) plane). If u and v are colinear, an
		/// arbitrary rotation axis is chosen.</p>
		/// </summary>
		/// <param name="u"> origin vector </param>
		/// <param name="v"> desired image of u by the rotation </param>
		/// <exception cref="MathArithmeticException"> if the norm of one of the vectors is zero </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FieldRotation(final FieldVector3D<T> u, final FieldVector3D<T> v) throws mathlib.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public FieldRotation(FieldVector3D<T> u, FieldVector3D<T> v)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T normProduct = u.getNorm().multiply(v.getNorm());
			T normProduct = u.Norm.multiply(v.Norm);
			if (normProduct.Real == 0)
			{
				throw new MathArithmeticException(LocalizedFormats.ZERO_NORM_FOR_ROTATION_DEFINING_VECTOR);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T dot = FieldVector3D.dotProduct(u, v);
			T dot = FieldVector3D.dotProduct(u, v);

			if (dot.Real < ((2.0e-15 - 1.0) * normProduct.Real))
			{
				// special case u = -v: we select a PI angle rotation around
				// an arbitrary vector orthogonal to u
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> w = u.orthogonal();
				FieldVector3D<T> w = u.orthogonal();
				q0 = normProduct.Field.Zero;
				q1 = w.X.negate();
				q2 = w.Y.negate();
				q3 = w.Z.negate();
			}
			else
			{
				// general case: (u, v) defines a plane, we select
				// the shortest possible rotation: axis orthogonal to this plane
				q0 = dot.divide(normProduct).add(1.0).multiply(0.5).sqrt();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T coeff = q0.multiply(normProduct).multiply(2.0).reciprocal();
				T coeff = q0.multiply(normProduct).multiply(2.0).reciprocal();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> q = FieldVector3D.crossProduct(v, u);
				FieldVector3D<T> q = FieldVector3D.crossProduct(v, u);
				q1 = coeff.multiply(q.X);
				q2 = coeff.multiply(q.Y);
				q3 = coeff.multiply(q.Z);
			}

		}

		/// <summary>
		/// Build a rotation from three Cardan or Euler elementary rotations.
		/// 
		/// <p>Cardan rotations are three successive rotations around the
		/// canonical axes X, Y and Z, each axis being used once. There are
		/// 6 such sets of rotations (XYZ, XZY, YXZ, YZX, ZXY and ZYX). Euler
		/// rotations are three successive rotations around the canonical
		/// axes X, Y and Z, the first and last rotations being around the
		/// same axis. There are 6 such sets of rotations (XYX, XZX, YXY,
		/// YZY, ZXZ and ZYZ), the most popular one being ZXZ.</p>
		/// <p>Beware that many people routinely use the term Euler angles even
		/// for what really are Cardan angles (this confusion is especially
		/// widespread in the aerospace business where Roll, Pitch and Yaw angles
		/// are often wrongly tagged as Euler angles).</p>
		/// </summary>
		/// <param name="order"> order of rotations to use </param>
		/// <param name="alpha1"> angle of the first elementary rotation </param>
		/// <param name="alpha2"> angle of the second elementary rotation </param>
		/// <param name="alpha3"> angle of the third elementary rotation </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldRotation(final RotationOrder order, final T alpha1, final T alpha2, final T alpha3)
		public FieldRotation(RotationOrder order, T alpha1, T alpha2, T alpha3)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T one = alpha1.getField().getOne();
			T one = alpha1.Field.One;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldRotation<T> r1 = new FieldRotation<T>(new FieldVector3D<T>(one, order.getA1()), alpha1);
			FieldRotation<T> r1 = new FieldRotation<T>(new FieldVector3D<T>(one, order.A1), alpha1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldRotation<T> r2 = new FieldRotation<T>(new FieldVector3D<T>(one, order.getA2()), alpha2);
			FieldRotation<T> r2 = new FieldRotation<T>(new FieldVector3D<T>(one, order.A2), alpha2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldRotation<T> r3 = new FieldRotation<T>(new FieldVector3D<T>(one, order.getA3()), alpha3);
			FieldRotation<T> r3 = new FieldRotation<T>(new FieldVector3D<T>(one, order.A3), alpha3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldRotation<T> composed = r1.applyTo(r2.applyTo(r3));
			FieldRotation<T> composed = r1.applyTo(r2.applyTo(r3));
			q0 = composed.q0;
			q1 = composed.q1;
			q2 = composed.q2;
			q3 = composed.q3;
		}

		/// <summary>
		/// Convert an orthogonal rotation matrix to a quaternion. </summary>
		/// <param name="ort"> orthogonal rotation matrix </param>
		/// <returns> quaternion corresponding to the matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private T[] mat2quat(final T[][] ort)
		private T[] mat2quat(T[][] ort)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] quat = mathlib.util.MathArrays.buildArray(ort[0][0].getField(), 4);
			T[] quat = MathArrays.buildArray(ort[0][0].Field, 4);

			// There are different ways to compute the quaternions elements
			// from the matrix. They all involve computing one element from
			// the diagonal of the matrix, and computing the three other ones
			// using a formula involving a division by the first element,
			// which unfortunately can be zero. Since the norm of the
			// quaternion is 1, we know at least one element has an absolute
			// value greater or equal to 0.5, so it is always possible to
			// select the right formula and avoid division by zero and even
			// numerical inaccuracy. Checking the elements in turn and using
			// the first one greater than 0.45 is safe (this leads to a simple
			// test since qi = 0.45 implies 4 qi^2 - 1 = -0.19)
			T s = ort[0][0].add(ort[1][1]).add(ort[2][2]);
			if (s.Real > -0.19)
			{
				// compute q0 and deduce q1, q2 and q3
				quat[0] = s.add(1.0).sqrt().multiply(0.5);
				T inv = quat[0].reciprocal().multiply(0.25);
				quat[1] = inv.multiply(ort[1][2].subtract(ort[2][1]));
				quat[2] = inv.multiply(ort[2][0].subtract(ort[0][2]));
				quat[3] = inv.multiply(ort[0][1].subtract(ort[1][0]));
			}
			else
			{
				s = ort[0][0].subtract(ort[1][1]).subtract(ort[2][2]);
				if (s.Real > -0.19)
				{
					// compute q1 and deduce q0, q2 and q3
					quat[1] = s.add(1.0).sqrt().multiply(0.5);
					T inv = quat[1].reciprocal().multiply(0.25);
					quat[0] = inv.multiply(ort[1][2].subtract(ort[2][1]));
					quat[2] = inv.multiply(ort[0][1].add(ort[1][0]));
					quat[3] = inv.multiply(ort[0][2].add(ort[2][0]));
				}
				else
				{
					s = ort[1][1].subtract(ort[0][0]).subtract(ort[2][2]);
					if (s.Real > -0.19)
					{
						// compute q2 and deduce q0, q1 and q3
						quat[2] = s.add(1.0).sqrt().multiply(0.5);
						T inv = quat[2].reciprocal().multiply(0.25);
						quat[0] = inv.multiply(ort[2][0].subtract(ort[0][2]));
						quat[1] = inv.multiply(ort[0][1].add(ort[1][0]));
						quat[3] = inv.multiply(ort[2][1].add(ort[1][2]));
					}
					else
					{
						// compute q3 and deduce q0, q1 and q2
						s = ort[2][2].subtract(ort[0][0]).subtract(ort[1][1]);
						quat[3] = s.add(1.0).sqrt().multiply(0.5);
						T inv = quat[3].reciprocal().multiply(0.25);
						quat[0] = inv.multiply(ort[0][1].subtract(ort[1][0]));
						quat[1] = inv.multiply(ort[0][2].add(ort[2][0]));
						quat[2] = inv.multiply(ort[2][1].add(ort[1][2]));
					}
				}
			}

			return quat;

		}

		/// <summary>
		/// Revert a rotation.
		/// Build a rotation which reverse the effect of another
		/// rotation. This means that if r(u) = v, then r.revert(v) = u. The
		/// instance is not changed. </summary>
		/// <returns> a new rotation whose effect is the reverse of the effect
		/// of the instance </returns>
		public virtual FieldRotation<T> revert()
		{
			return new FieldRotation<T>(q0.negate(), q1, q2, q3, false);
		}

		/// <summary>
		/// Get the scalar coordinate of the quaternion. </summary>
		/// <returns> scalar coordinate of the quaternion </returns>
		public virtual T Q0
		{
			get
			{
				return q0;
			}
		}

		/// <summary>
		/// Get the first coordinate of the vectorial part of the quaternion. </summary>
		/// <returns> first coordinate of the vectorial part of the quaternion </returns>
		public virtual T Q1
		{
			get
			{
				return q1;
			}
		}

		/// <summary>
		/// Get the second coordinate of the vectorial part of the quaternion. </summary>
		/// <returns> second coordinate of the vectorial part of the quaternion </returns>
		public virtual T Q2
		{
			get
			{
				return q2;
			}
		}

		/// <summary>
		/// Get the third coordinate of the vectorial part of the quaternion. </summary>
		/// <returns> third coordinate of the vectorial part of the quaternion </returns>
		public virtual T Q3
		{
			get
			{
				return q3;
			}
		}

		/// <summary>
		/// Get the normalized axis of the rotation. </summary>
		/// <returns> normalized axis of the rotation </returns>
		/// <seealso cref= #FieldRotation(FieldVector3D, RealFieldElement) </seealso>
		public virtual FieldVector3D<T> Axis
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T squaredSine = q1.multiply(q1).add(q2.multiply(q2)).add(q3.multiply(q3));
				T squaredSine = q1.multiply(q1).add(q2.multiply(q2)).add(q3.multiply(q3));
				if (squaredSine.Real == 0)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.Field<T> field = squaredSine.getField();
					Field<T> field = squaredSine.Field;
					return new FieldVector3D<T>(field.One, field.Zero, field.Zero);
				}
				else if (q0.Real < 0)
				{
					T inverse = squaredSine.sqrt().reciprocal();
					return new FieldVector3D<T>(q1.multiply(inverse), q2.multiply(inverse), q3.multiply(inverse));
				}
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T inverse = squaredSine.sqrt().reciprocal().negate();
				T inverse = squaredSine.sqrt().reciprocal().negate();
				return new FieldVector3D<T>(q1.multiply(inverse), q2.multiply(inverse), q3.multiply(inverse));
			}
		}

		/// <summary>
		/// Get the angle of the rotation. </summary>
		/// <returns> angle of the rotation (between 0 and &pi;) </returns>
		/// <seealso cref= #FieldRotation(FieldVector3D, RealFieldElement) </seealso>
		public virtual T Angle
		{
			get
			{
				if ((q0.Real < -0.1) || (q0.Real > 0.1))
				{
					return q1.multiply(q1).add(q2.multiply(q2)).add(q3.multiply(q3)).sqrt().asin().multiply(2);
				}
				else if (q0.Real < 0)
				{
					return q0.negate().acos().multiply(2);
				}
				return q0.acos().multiply(2);
			}
		}

		/// <summary>
		/// Get the Cardan or Euler angles corresponding to the instance.
		/// 
		/// <p>The equations show that each rotation can be defined by two
		/// different values of the Cardan or Euler angles set. For example
		/// if Cardan angles are used, the rotation defined by the angles
		/// a<sub>1</sub>, a<sub>2</sub> and a<sub>3</sub> is the same as
		/// the rotation defined by the angles &pi; + a<sub>1</sub>, &pi;
		/// - a<sub>2</sub> and &pi; + a<sub>3</sub>. This method implements
		/// the following arbitrary choices:</p>
		/// <ul>
		///   <li>for Cardan angles, the chosen set is the one for which the
		///   second angle is between -&pi;/2 and &pi;/2 (i.e its cosine is
		///   positive),</li>
		///   <li>for Euler angles, the chosen set is the one for which the
		///   second angle is between 0 and &pi; (i.e its sine is positive).</li>
		/// </ul>
		/// 
		/// <p>Cardan and Euler angle have a very disappointing drawback: all
		/// of them have singularities. This means that if the instance is
		/// too close to the singularities corresponding to the given
		/// rotation order, it will be impossible to retrieve the angles. For
		/// Cardan angles, this is often called gimbal lock. There is
		/// <em>nothing</em> to do to prevent this, it is an intrinsic problem
		/// with Cardan and Euler representation (but not a problem with the
		/// rotation itself, which is perfectly well defined). For Cardan
		/// angles, singularities occur when the second angle is close to
		/// -&pi;/2 or +&pi;/2, for Euler angle singularities occur when the
		/// second angle is close to 0 or &pi;, this implies that the identity
		/// rotation is always singular for Euler angles!</p>
		/// </summary>
		/// <param name="order"> rotation order to use </param>
		/// <returns> an array of three angles, in the order specified by the set </returns>
		/// <exception cref="CardanEulerSingularityException"> if the rotation is
		/// singular with respect to the angles set specified </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public T[] getAngles(final RotationOrder order) throws CardanEulerSingularityException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual T[] getAngles(RotationOrder order)
		{

			if (order == RotationOrder.XYZ)
			{

				// r (+K) coordinates are :
				//  sin (theta), -cos (theta) sin (phi), cos (theta) cos (phi)
				// (-r) (+I) coordinates are :
				// cos (psi) cos (theta), -sin (psi) cos (theta), sin (theta)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
				FieldVector3D<T> v1 = applyTo(vector(0, 0, 1)); // and we can choose to have theta in the interval [-PI/2 ; +PI/2]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				if ((v2.Z.Real < -0.9999999999) || (v2.Z.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(true);
				}
				return buildArray(v1.Y.negate().atan2(v1.Z), v2.Z.asin(), v2.Y.negate().atan2(v2.X));

			}
			else if (order == RotationOrder.XZY)
			{

				// r (+J) coordinates are :
				// -sin (psi), cos (psi) cos (phi), cos (psi) sin (phi)
				// (-r) (+I) coordinates are :
				// cos (theta) cos (psi), -sin (psi), sin (theta) cos (psi)
				// and we can choose to have psi in the interval [-PI/2 ; +PI/2]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
				FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				if ((v2.Y.Real < -0.9999999999) || (v2.Y.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(true);
				}
				return buildArray(v1.Z.atan2(v1.Y), v2.Y.asin().negate(), v2.Z.atan2(v2.X));

			}
			else if (order == RotationOrder.YXZ)
			{

				// r (+K) coordinates are :
				//  cos (phi) sin (theta), -sin (phi), cos (phi) cos (theta)
				// (-r) (+J) coordinates are :
				// sin (psi) cos (phi), cos (psi) cos (phi), -sin (phi)
				// and we can choose to have phi in the interval [-PI/2 ; +PI/2]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
				FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				if ((v2.Z.Real < -0.9999999999) || (v2.Z.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(true);
				}
				return buildArray(v1.X.atan2(v1.Z), v2.Z.asin().negate(), v2.X.atan2(v2.Y));

			}
			else if (order == RotationOrder.YZX)
			{

				// r (+I) coordinates are :
				// cos (psi) cos (theta), sin (psi), -cos (psi) sin (theta)
				// (-r) (+J) coordinates are :
				// sin (psi), cos (phi) cos (psi), -sin (phi) cos (psi)
				// and we can choose to have psi in the interval [-PI/2 ; +PI/2]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
				FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				if ((v2.X.Real < -0.9999999999) || (v2.X.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(true);
				}
				return buildArray(v1.Z.negate().atan2(v1.X), v2.X.asin(), v2.Z.negate().atan2(v2.Y));

			}
			else if (order == RotationOrder.ZXY)
			{

				// r (+J) coordinates are :
				// -cos (phi) sin (psi), cos (phi) cos (psi), sin (phi)
				// (-r) (+K) coordinates are :
				// -sin (theta) cos (phi), sin (phi), cos (theta) cos (phi)
				// and we can choose to have phi in the interval [-PI/2 ; +PI/2]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
				FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				if ((v2.Y.Real < -0.9999999999) || (v2.Y.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(true);
				}
				return buildArray(v1.X.negate().atan2(v1.Y), v2.Y.asin(), v2.X.negate().atan2(v2.Z));

			}
			else if (order == RotationOrder.ZYX)
			{

				// r (+I) coordinates are :
				//  cos (theta) cos (psi), cos (theta) sin (psi), -sin (theta)
				// (-r) (+K) coordinates are :
				// -sin (theta), sin (phi) cos (theta), cos (phi) cos (theta)
				// and we can choose to have theta in the interval [-PI/2 ; +PI/2]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
				FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				if ((v2.X.Real < -0.9999999999) || (v2.X.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(true);
				}
				return buildArray(v1.Y.atan2(v1.X), v2.X.asin().negate(), v2.Y.atan2(v2.Z));

			}
			else if (order == RotationOrder.XYX)
			{

				// r (+I) coordinates are :
				//  cos (theta), sin (phi1) sin (theta), -cos (phi1) sin (theta)
				// (-r) (+I) coordinates are :
				// cos (theta), sin (theta) sin (phi2), sin (theta) cos (phi2)
				// and we can choose to have theta in the interval [0 ; PI]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
				FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				if ((v2.X.Real < -0.9999999999) || (v2.X.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(false);
				}
				return buildArray(v1.Y.atan2(v1.Z.negate()), v2.X.acos(), v2.Y.atan2(v2.Z));

			}
			else if (order == RotationOrder.XZX)
			{

				// r (+I) coordinates are :
				//  cos (psi), cos (phi1) sin (psi), sin (phi1) sin (psi)
				// (-r) (+I) coordinates are :
				// cos (psi), -sin (psi) cos (phi2), sin (psi) sin (phi2)
				// and we can choose to have psi in the interval [0 ; PI]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
				FieldVector3D<T> v1 = applyTo(vector(1, 0, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(1, 0, 0));
				if ((v2.X.Real < -0.9999999999) || (v2.X.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(false);
				}
				return buildArray(v1.Z.atan2(v1.Y), v2.X.acos(), v2.Z.atan2(v2.Y.negate()));

			}
			else if (order == RotationOrder.YXY)
			{

				// r (+J) coordinates are :
				//  sin (theta1) sin (phi), cos (phi), cos (theta1) sin (phi)
				// (-r) (+J) coordinates are :
				// sin (phi) sin (theta2), cos (phi), -sin (phi) cos (theta2)
				// and we can choose to have phi in the interval [0 ; PI]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
				FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				if ((v2.Y.Real < -0.9999999999) || (v2.Y.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(false);
				}
				return buildArray(v1.X.atan2(v1.Z), v2.Y.acos(), v2.X.atan2(v2.Z.negate()));

			}
			else if (order == RotationOrder.YZY)
			{

				// r (+J) coordinates are :
				//  -cos (theta1) sin (psi), cos (psi), sin (theta1) sin (psi)
				// (-r) (+J) coordinates are :
				// sin (psi) cos (theta2), cos (psi), sin (psi) sin (theta2)
				// and we can choose to have psi in the interval [0 ; PI]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
				FieldVector3D<T> v1 = applyTo(vector(0, 1, 0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 1, 0));
				if ((v2.Y.Real < -0.9999999999) || (v2.Y.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(false);
				}
				return buildArray(v1.Z.atan2(v1.X.negate()), v2.Y.acos(), v2.Z.atan2(v2.X));

			}
			else if (order == RotationOrder.ZXZ)
			{

				// r (+K) coordinates are :
				//  sin (psi1) sin (phi), -cos (psi1) sin (phi), cos (phi)
				// (-r) (+K) coordinates are :
				// sin (phi) sin (psi2), sin (phi) cos (psi2), cos (phi)
				// and we can choose to have phi in the interval [0 ; PI]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
				FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				if ((v2.Z.Real < -0.9999999999) || (v2.Z.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(false);
				}
				return buildArray(v1.X.atan2(v1.Y.negate()), v2.Z.acos(), v2.X.atan2(v2.Y));

			} // last possibility is ZYZ
			else
			{

				// r (+K) coordinates are :
				//  cos (psi1) sin (theta), sin (psi1) sin (theta), cos (theta)
				// (-r) (+K) coordinates are :
				// -sin (theta) cos (psi2), sin (theta) sin (psi2), cos (theta)
				// and we can choose to have theta in the interval [0 ; PI]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
				FieldVector3D<T> v1 = applyTo(vector(0, 0, 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				FieldVector3D<T> v2 = applyInverseTo(vector(0, 0, 1));
				if ((v2.Z.Real < -0.9999999999) || (v2.Z.Real > 0.9999999999))
				{
					throw new CardanEulerSingularityException(false);
				}
				return buildArray(v1.Y.atan2(v1.X), v2.Z.acos(), v2.Y.atan2(v2.X.negate()));

			}

		}

		/// <summary>
		/// Create a dimension 3 array. </summary>
		/// <param name="a0"> first array element </param>
		/// <param name="a1"> second array element </param>
		/// <param name="a2"> third array element </param>
		/// <returns> new array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private T[] buildArray(final T a0, final T a1, final T a2)
		private T[] buildArray(T a0, T a1, T a2)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[] array = mathlib.util.MathArrays.buildArray(a0.getField(), 3);
			T[] array = MathArrays.buildArray(a0.Field, 3);
			array[0] = a0;
			array[1] = a1;
			array[2] = a2;
			return array;
		}

		/// <summary>
		/// Create a constant vector. </summary>
		/// <param name="x"> abscissa </param>
		/// <param name="y"> ordinate </param>
		/// <param name="z"> height </param>
		/// <returns> a constant vector </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private FieldVector3D<T> vector(final double x, final double y, final double z)
		private FieldVector3D<T> vector(double x, double y, double z)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T zero = q0.getField().getZero();
			T zero = q0.Field.Zero;
			return new FieldVector3D<T>(zero.add(x), zero.add(y), zero.add(z));
		}

		/// <summary>
		/// Get the 3X3 matrix corresponding to the instance </summary>
		/// <returns> the matrix corresponding to the instance </returns>
		public virtual T[][] Matrix
		{
			get
			{
    
				// products
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q0q0 = q0.multiply(q0);
				T q0q0 = q0.multiply(q0);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q0q1 = q0.multiply(q1);
				T q0q1 = q0.multiply(q1);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q0q2 = q0.multiply(q2);
				T q0q2 = q0.multiply(q2);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q0q3 = q0.multiply(q3);
				T q0q3 = q0.multiply(q3);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q1q1 = q1.multiply(q1);
				T q1q1 = q1.multiply(q1);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q1q2 = q1.multiply(q2);
				T q1q2 = q1.multiply(q2);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q1q3 = q1.multiply(q3);
				T q1q3 = q1.multiply(q3);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q2q2 = q2.multiply(q2);
				T q2q2 = q2.multiply(q2);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q2q3 = q2.multiply(q3);
				T q2q3 = q2.multiply(q3);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T q3q3 = q3.multiply(q3);
				T q3q3 = q3.multiply(q3);
    
				// create the matrix
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final T[][] m = mathlib.util.MathArrays.buildArray(q0.getField(), 3, 3);
				T[][] m = MathArrays.buildArray(q0.Field, 3, 3);
    
				m [0][0] = q0q0.add(q1q1).multiply(2).subtract(1);
				m [1][0] = q1q2.subtract(q0q3).multiply(2);
				m [2][0] = q1q3.add(q0q2).multiply(2);
    
				m [0][1] = q1q2.add(q0q3).multiply(2);
				m [1][1] = q0q0.add(q2q2).multiply(2).subtract(1);
				m [2][1] = q2q3.subtract(q0q1).multiply(2);
    
				m [0][2] = q1q3.subtract(q0q2).multiply(2);
				m [1][2] = q2q3.add(q0q1).multiply(2);
				m [2][2] = q0q0.add(q3q3).multiply(2).subtract(1);
    
				return m;
    
			}
		}

		/// <summary>
		/// Convert to a constant vector without derivatives. </summary>
		/// <returns> a constant vector </returns>
		public virtual Rotation toRotation()
		{
			return new Rotation(q0.Real, q1.Real, q2.Real, q3.Real, false);
		}

		/// <summary>
		/// Apply the rotation to a vector. </summary>
		/// <param name="u"> vector to apply the rotation to </param>
		/// <returns> a new vector which is the image of u by the rotation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> applyTo(final FieldVector3D<T> u)
		public virtual FieldVector3D<T> applyTo(FieldVector3D<T> u)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T x = u.getX();
			T x = u.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T y = u.getY();
			T y = u.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T z = u.getZ();
			T z = u.Z;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));

			return new FieldVector3D<T>(q0.multiply(x.multiply(q0).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x), q0.multiply(y.multiply(q0).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y), q0.multiply(z.multiply(q0).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z));

		}

		/// <summary>
		/// Apply the rotation to a vector. </summary>
		/// <param name="u"> vector to apply the rotation to </param>
		/// <returns> a new vector which is the image of u by the rotation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> applyTo(final Vector3D u)
		public virtual FieldVector3D<T> applyTo(Vector3D u)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = u.getX();
			double x = u.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = u.getY();
			double y = u.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = u.getZ();
			double z = u.Z;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));

			return new FieldVector3D<T>(q0.multiply(q0.multiply(x).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x), q0.multiply(q0.multiply(y).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y), q0.multiply(q0.multiply(z).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z));

		}

		/// <summary>
		/// Apply the rotation to a vector stored in an array. </summary>
		/// <param name="in"> an array with three items which stores vector to rotate </param>
		/// <param name="out"> an array with three items to put result to (it can be the same
		/// array as in) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void applyTo(final T[] in, final T[] out)
		public virtual void applyTo(T[] @in, T[] @out)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T x = in[0];
			T x = @in[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T y = in[1];
			T y = @in[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T z = in[2];
			T z = @in[2];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));

			@out[0] = q0.multiply(x.multiply(q0).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x);
			@out[1] = q0.multiply(y.multiply(q0).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y);
			@out[2] = q0.multiply(z.multiply(q0).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z);

		}

		/// <summary>
		/// Apply the rotation to a vector stored in an array. </summary>
		/// <param name="in"> an array with three items which stores vector to rotate </param>
		/// <param name="out"> an array with three items to put result to </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void applyTo(final double[] in, final T[] out)
		public virtual void applyTo(double[] @in, T[] @out)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = in[0];
			double x = @in[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = in[1];
			double y = @in[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = in[2];
			double z = @in[2];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));

			@out[0] = q0.multiply(q0.multiply(x).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x);
			@out[1] = q0.multiply(q0.multiply(y).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y);
			@out[2] = q0.multiply(q0.multiply(z).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z);

		}

		/// <summary>
		/// Apply a rotation to a vector. </summary>
		/// <param name="r"> rotation to apply </param>
		/// <param name="u"> vector to apply the rotation to </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> a new vector which is the image of u by the rotation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldVector3D<T> applyTo(final Rotation r, final FieldVector3D<T> u)
		public static FieldVector3D<T> applyTo<T>(Rotation r, FieldVector3D<T> u) where T : mathlib.RealFieldElement<T>
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T x = u.getX();
			T x = u.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T y = u.getY();
			T y = u.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T z = u.getZ();
			T z = u.Z;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = x.multiply(r.getQ1()).add(y.multiply(r.getQ2())).add(z.multiply(r.getQ3()));
			T s = x.multiply(r.Q1).add(y.multiply(r.Q2)).add(z.multiply(r.Q3));

			return new FieldVector3D<T>(x.multiply(r.Q0).subtract(z.multiply(r.Q2).subtract(y.multiply(r.Q3))).multiply(r.Q0).add(s.multiply(r.Q1)).multiply(2).subtract(x), y.multiply(r.Q0).subtract(x.multiply(r.Q3).subtract(z.multiply(r.Q1))).multiply(r.Q0).add(s.multiply(r.Q2)).multiply(2).subtract(y), z.multiply(r.Q0).subtract(y.multiply(r.Q1).subtract(x.multiply(r.Q2))).multiply(r.Q0).add(s.multiply(r.Q3)).multiply(2).subtract(z));

		}

		/// <summary>
		/// Apply the inverse of the rotation to a vector. </summary>
		/// <param name="u"> vector to apply the inverse of the rotation to </param>
		/// <returns> a new vector which such that u is its image by the rotation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> applyInverseTo(final FieldVector3D<T> u)
		public virtual FieldVector3D<T> applyInverseTo(FieldVector3D<T> u)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T x = u.getX();
			T x = u.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T y = u.getY();
			T y = u.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T z = u.getZ();
			T z = u.Z;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T m0 = q0.negate();
			T m0 = q0.negate();

			return new FieldVector3D<T>(m0.multiply(x.multiply(m0).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x), m0.multiply(y.multiply(m0).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y), m0.multiply(z.multiply(m0).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z));

		}

		/// <summary>
		/// Apply the inverse of the rotation to a vector. </summary>
		/// <param name="u"> vector to apply the inverse of the rotation to </param>
		/// <returns> a new vector which such that u is its image by the rotation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldVector3D<T> applyInverseTo(final Vector3D u)
		public virtual FieldVector3D<T> applyInverseTo(Vector3D u)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = u.getX();
			double x = u.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = u.getY();
			double y = u.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = u.getZ();
			double z = u.Z;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T m0 = q0.negate();
			T m0 = q0.negate();

			return new FieldVector3D<T>(m0.multiply(m0.multiply(x).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x), m0.multiply(m0.multiply(y).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y), m0.multiply(m0.multiply(z).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z));

		}

		/// <summary>
		/// Apply the inverse of the rotation to a vector stored in an array. </summary>
		/// <param name="in"> an array with three items which stores vector to rotate </param>
		/// <param name="out"> an array with three items to put result to (it can be the same
		/// array as in) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void applyInverseTo(final T[] in, final T[] out)
		public virtual void applyInverseTo(T[] @in, T[] @out)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T x = in[0];
			T x = @in[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T y = in[1];
			T y = @in[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T z = in[2];
			T z = @in[2];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T m0 = q0.negate();
			T m0 = q0.negate();

			@out[0] = m0.multiply(x.multiply(m0).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x);
			@out[1] = m0.multiply(y.multiply(m0).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y);
			@out[2] = m0.multiply(z.multiply(m0).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z);

		}

		/// <summary>
		/// Apply the inverse of the rotation to a vector stored in an array. </summary>
		/// <param name="in"> an array with three items which stores vector to rotate </param>
		/// <param name="out"> an array with three items to put result to </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void applyInverseTo(final double[] in, final T[] out)
		public virtual void applyInverseTo(double[] @in, T[] @out)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = in[0];
			double x = @in[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = in[1];
			double y = @in[1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = in[2];
			double z = @in[2];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
			T s = q1.multiply(x).add(q2.multiply(y)).add(q3.multiply(z));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T m0 = q0.negate();
			T m0 = q0.negate();

			@out[0] = m0.multiply(m0.multiply(x).subtract(q2.multiply(z).subtract(q3.multiply(y)))).add(s.multiply(q1)).multiply(2).subtract(x);
			@out[1] = m0.multiply(m0.multiply(y).subtract(q3.multiply(x).subtract(q1.multiply(z)))).add(s.multiply(q2)).multiply(2).subtract(y);
			@out[2] = m0.multiply(m0.multiply(z).subtract(q1.multiply(y).subtract(q2.multiply(x)))).add(s.multiply(q3)).multiply(2).subtract(z);

		}

		/// <summary>
		/// Apply the inverse of a rotation to a vector. </summary>
		/// <param name="r"> rotation to apply </param>
		/// <param name="u"> vector to apply the inverse of the rotation to </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> a new vector which such that u is its image by the rotation </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldVector3D<T> applyInverseTo(final Rotation r, final FieldVector3D<T> u)
		public static FieldVector3D<T> applyInverseTo<T>(Rotation r, FieldVector3D<T> u) where T : mathlib.RealFieldElement<T>
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T x = u.getX();
			T x = u.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T y = u.getY();
			T y = u.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T z = u.getZ();
			T z = u.Z;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T s = x.multiply(r.getQ1()).add(y.multiply(r.getQ2())).add(z.multiply(r.getQ3()));
			T s = x.multiply(r.Q1).add(y.multiply(r.Q2)).add(z.multiply(r.Q3));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m0 = -r.getQ0();
			double m0 = -r.Q0;

			return new FieldVector3D<T>(x.multiply(m0).subtract(z.multiply(r.Q2).subtract(y.multiply(r.Q3))).multiply(m0).add(s.multiply(r.Q1)).multiply(2).subtract(x), y.multiply(m0).subtract(x.multiply(r.Q3).subtract(z.multiply(r.Q1))).multiply(m0).add(s.multiply(r.Q2)).multiply(2).subtract(y), z.multiply(m0).subtract(y.multiply(r.Q1).subtract(x.multiply(r.Q2))).multiply(m0).add(s.multiply(r.Q3)).multiply(2).subtract(z));

		}

		/// <summary>
		/// Apply the instance to another rotation.
		/// Applying the instance to a rotation is computing the composition
		/// in an order compliant with the following rule : let u be any
		/// vector and v its image by r (i.e. r.applyTo(u) = v), let w be the image
		/// of v by the instance (i.e. applyTo(v) = w), then w = comp.applyTo(u),
		/// where comp = applyTo(r). </summary>
		/// <param name="r"> rotation to apply the rotation to </param>
		/// <returns> a new rotation which is the composition of r by the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldRotation<T> applyTo(final FieldRotation<T> r)
		public virtual FieldRotation<T> applyTo(FieldRotation<T> r)
		{
			return new FieldRotation<T>(r.q0.multiply(q0).subtract(r.q1.multiply(q1).add(r.q2.multiply(q2)).add(r.q3.multiply(q3))), r.q1.multiply(q0).add(r.q0.multiply(q1)).add(r.q2.multiply(q3).subtract(r.q3.multiply(q2))), r.q2.multiply(q0).add(r.q0.multiply(q2)).add(r.q3.multiply(q1).subtract(r.q1.multiply(q3))), r.q3.multiply(q0).add(r.q0.multiply(q3)).add(r.q1.multiply(q2).subtract(r.q2.multiply(q1))), false);
		}

		/// <summary>
		/// Apply the instance to another rotation.
		/// Applying the instance to a rotation is computing the composition
		/// in an order compliant with the following rule : let u be any
		/// vector and v its image by r (i.e. r.applyTo(u) = v), let w be the image
		/// of v by the instance (i.e. applyTo(v) = w), then w = comp.applyTo(u),
		/// where comp = applyTo(r). </summary>
		/// <param name="r"> rotation to apply the rotation to </param>
		/// <returns> a new rotation which is the composition of r by the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldRotation<T> applyTo(final Rotation r)
		public virtual FieldRotation<T> applyTo(Rotation r)
		{
			return new FieldRotation<T>(q0.multiply(r.Q0).subtract(q1.multiply(r.Q1).add(q2.multiply(r.Q2)).add(q3.multiply(r.Q3))), q0.multiply(r.Q1).add(q1.multiply(r.Q0)).add(q3.multiply(r.Q2).subtract(q2.multiply(r.Q3))), q0.multiply(r.Q2).add(q2.multiply(r.Q0)).add(q1.multiply(r.Q3).subtract(q3.multiply(r.Q1))), q0.multiply(r.Q3).add(q3.multiply(r.Q0)).add(q2.multiply(r.Q1).subtract(q1.multiply(r.Q2))), false);
		}

		/// <summary>
		/// Apply a rotation to another rotation.
		/// Applying a rotation to another rotation is computing the composition
		/// in an order compliant with the following rule : let u be any
		/// vector and v its image by rInner (i.e. rInner.applyTo(u) = v), let w be the image
		/// of v by rOuter (i.e. rOuter.applyTo(v) = w), then w = comp.applyTo(u),
		/// where comp = applyTo(rOuter, rInner). </summary>
		/// <param name="r1"> rotation to apply </param>
		/// <param name="rInner"> rotation to apply the rotation to </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> a new rotation which is the composition of r by the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldRotation<T> applyTo(final Rotation r1, final FieldRotation<T> rInner)
		public static FieldRotation<T> applyTo<T>(Rotation r1, FieldRotation<T> rInner) where T : mathlib.RealFieldElement<T>
		{
			return new FieldRotation<T>(rInner.q0.multiply(r1.Q0).subtract(rInner.q1.multiply(r1.Q1).add(rInner.q2.multiply(r1.Q2)).add(rInner.q3.multiply(r1.Q3))), rInner.q1.multiply(r1.Q0).add(rInner.q0.multiply(r1.Q1)).add(rInner.q2.multiply(r1.Q3).subtract(rInner.q3.multiply(r1.Q2))), rInner.q2.multiply(r1.Q0).add(rInner.q0.multiply(r1.Q2)).add(rInner.q3.multiply(r1.Q1).subtract(rInner.q1.multiply(r1.Q3))), rInner.q3.multiply(r1.Q0).add(rInner.q0.multiply(r1.Q3)).add(rInner.q1.multiply(r1.Q2).subtract(rInner.q2.multiply(r1.Q1))), false);
		}

		/// <summary>
		/// Apply the inverse of the instance to another rotation.
		/// Applying the inverse of the instance to a rotation is computing
		/// the composition in an order compliant with the following rule :
		/// let u be any vector and v its image by r (i.e. r.applyTo(u) = v),
		/// let w be the inverse image of v by the instance
		/// (i.e. applyInverseTo(v) = w), then w = comp.applyTo(u), where
		/// comp = applyInverseTo(r). </summary>
		/// <param name="r"> rotation to apply the rotation to </param>
		/// <returns> a new rotation which is the composition of r by the inverse
		/// of the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldRotation<T> applyInverseTo(final FieldRotation<T> r)
		public virtual FieldRotation<T> applyInverseTo(FieldRotation<T> r)
		{
			return new FieldRotation<T>(r.q0.multiply(q0).add(r.q1.multiply(q1).add(r.q2.multiply(q2)).add(r.q3.multiply(q3))).negate(), r.q0.multiply(q1).add(r.q2.multiply(q3).subtract(r.q3.multiply(q2))).subtract(r.q1.multiply(q0)), r.q0.multiply(q2).add(r.q3.multiply(q1).subtract(r.q1.multiply(q3))).subtract(r.q2.multiply(q0)), r.q0.multiply(q3).add(r.q1.multiply(q2).subtract(r.q2.multiply(q1))).subtract(r.q3.multiply(q0)), false);
		}

		/// <summary>
		/// Apply the inverse of the instance to another rotation.
		/// Applying the inverse of the instance to a rotation is computing
		/// the composition in an order compliant with the following rule :
		/// let u be any vector and v its image by r (i.e. r.applyTo(u) = v),
		/// let w be the inverse image of v by the instance
		/// (i.e. applyInverseTo(v) = w), then w = comp.applyTo(u), where
		/// comp = applyInverseTo(r). </summary>
		/// <param name="r"> rotation to apply the rotation to </param>
		/// <returns> a new rotation which is the composition of r by the inverse
		/// of the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public FieldRotation<T> applyInverseTo(final Rotation r)
		public virtual FieldRotation<T> applyInverseTo(Rotation r)
		{
			return new FieldRotation<T>(q0.multiply(r.Q0).add(q1.multiply(r.Q1).add(q2.multiply(r.Q2)).add(q3.multiply(r.Q3))).negate(), q1.multiply(r.Q0).add(q3.multiply(r.Q2).subtract(q2.multiply(r.Q3))).subtract(q0.multiply(r.Q1)), q2.multiply(r.Q0).add(q1.multiply(r.Q3).subtract(q3.multiply(r.Q1))).subtract(q0.multiply(r.Q2)), q3.multiply(r.Q0).add(q2.multiply(r.Q1).subtract(q1.multiply(r.Q2))).subtract(q0.multiply(r.Q3)), false);
		}

		/// <summary>
		/// Apply the inverse of a rotation to another rotation.
		/// Applying the inverse of a rotation to another rotation is computing
		/// the composition in an order compliant with the following rule :
		/// let u be any vector and v its image by rInner (i.e. rInner.applyTo(u) = v),
		/// let w be the inverse image of v by rOuter
		/// (i.e. rOuter.applyInverseTo(v) = w), then w = comp.applyTo(u), where
		/// comp = applyInverseTo(rOuter, rInner). </summary>
		/// <param name="rOuter"> rotation to apply the rotation to </param>
		/// <param name="rInner"> rotation to apply the rotation to </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> a new rotation which is the composition of r by the inverse
		/// of the instance </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> FieldRotation<T> applyInverseTo(final Rotation rOuter, final FieldRotation<T> rInner)
		public static FieldRotation<T> applyInverseTo<T>(Rotation rOuter, FieldRotation<T> rInner) where T : mathlib.RealFieldElement<T>
		{
			return new FieldRotation<T>(rInner.q0.multiply(rOuter.Q0).add(rInner.q1.multiply(rOuter.Q1).add(rInner.q2.multiply(rOuter.Q2)).add(rInner.q3.multiply(rOuter.Q3))).negate(), rInner.q0.multiply(rOuter.Q1).add(rInner.q2.multiply(rOuter.Q3).subtract(rInner.q3.multiply(rOuter.Q2))).subtract(rInner.q1.multiply(rOuter.Q0)), rInner.q0.multiply(rOuter.Q2).add(rInner.q3.multiply(rOuter.Q1).subtract(rInner.q1.multiply(rOuter.Q3))).subtract(rInner.q2.multiply(rOuter.Q0)), rInner.q0.multiply(rOuter.Q3).add(rInner.q1.multiply(rOuter.Q2).subtract(rInner.q2.multiply(rOuter.Q1))).subtract(rInner.q3.multiply(rOuter.Q0)), false);
		}

		/// <summary>
		/// Perfect orthogonality on a 3X3 matrix. </summary>
		/// <param name="m"> initial matrix (not exactly orthogonal) </param>
		/// <param name="threshold"> convergence threshold for the iterative
		/// orthogonality correction (convergence is reached when the
		/// difference between two steps of the Frobenius norm of the
		/// correction is below this threshold) </param>
		/// <returns> an orthogonal matrix close to m </returns>
		/// <exception cref="NotARotationMatrixException"> if the matrix cannot be
		/// orthogonalized with the given threshold after 10 iterations </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private T[][] orthogonalizeMatrix(final T[][] m, final double threshold) throws NotARotationMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private T[][] orthogonalizeMatrix(T[][] m, double threshold)
		{

			T x00 = m[0][0];
			T x01 = m[0][1];
			T x02 = m[0][2];
			T x10 = m[1][0];
			T x11 = m[1][1];
			T x12 = m[1][2];
			T x20 = m[2][0];
			T x21 = m[2][1];
			T x22 = m[2][2];
			double fn = 0;
			double fn1;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T[][] o = mathlib.util.MathArrays.buildArray(m[0][0].getField(), 3, 3);
			T[][] o = MathArrays.buildArray(m[0][0].Field, 3, 3);

			// iterative correction: Xn+1 = Xn - 0.5 * (Xn.Mt.Xn - M)
			int i = 0;
			while (++i < 11)
			{

				// Mt.Xn
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx00 = m[0][0].multiply(x00).add(m[1][0].multiply(x10)).add(m[2][0].multiply(x20));
				T mx00 = m[0][0].multiply(x00).add(m[1][0].multiply(x10)).add(m[2][0].multiply(x20));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx10 = m[0][1].multiply(x00).add(m[1][1].multiply(x10)).add(m[2][1].multiply(x20));
				T mx10 = m[0][1].multiply(x00).add(m[1][1].multiply(x10)).add(m[2][1].multiply(x20));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx20 = m[0][2].multiply(x00).add(m[1][2].multiply(x10)).add(m[2][2].multiply(x20));
				T mx20 = m[0][2].multiply(x00).add(m[1][2].multiply(x10)).add(m[2][2].multiply(x20));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx01 = m[0][0].multiply(x01).add(m[1][0].multiply(x11)).add(m[2][0].multiply(x21));
				T mx01 = m[0][0].multiply(x01).add(m[1][0].multiply(x11)).add(m[2][0].multiply(x21));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx11 = m[0][1].multiply(x01).add(m[1][1].multiply(x11)).add(m[2][1].multiply(x21));
				T mx11 = m[0][1].multiply(x01).add(m[1][1].multiply(x11)).add(m[2][1].multiply(x21));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx21 = m[0][2].multiply(x01).add(m[1][2].multiply(x11)).add(m[2][2].multiply(x21));
				T mx21 = m[0][2].multiply(x01).add(m[1][2].multiply(x11)).add(m[2][2].multiply(x21));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx02 = m[0][0].multiply(x02).add(m[1][0].multiply(x12)).add(m[2][0].multiply(x22));
				T mx02 = m[0][0].multiply(x02).add(m[1][0].multiply(x12)).add(m[2][0].multiply(x22));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx12 = m[0][1].multiply(x02).add(m[1][1].multiply(x12)).add(m[2][1].multiply(x22));
				T mx12 = m[0][1].multiply(x02).add(m[1][1].multiply(x12)).add(m[2][1].multiply(x22));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final T mx22 = m[0][2].multiply(x02).add(m[1][2].multiply(x12)).add(m[2][2].multiply(x22));
				T mx22 = m[0][2].multiply(x02).add(m[1][2].multiply(x12)).add(m[2][2].multiply(x22));

				// Xn+1
				o[0][0] = x00.subtract(x00.multiply(mx00).add(x01.multiply(mx10)).add(x02.multiply(mx20)).subtract(m[0][0]).multiply(0.5));
				o[0][1] = x01.subtract(x00.multiply(mx01).add(x01.multiply(mx11)).add(x02.multiply(mx21)).subtract(m[0][1]).multiply(0.5));
				o[0][2] = x02.subtract(x00.multiply(mx02).add(x01.multiply(mx12)).add(x02.multiply(mx22)).subtract(m[0][2]).multiply(0.5));
				o[1][0] = x10.subtract(x10.multiply(mx00).add(x11.multiply(mx10)).add(x12.multiply(mx20)).subtract(m[1][0]).multiply(0.5));
				o[1][1] = x11.subtract(x10.multiply(mx01).add(x11.multiply(mx11)).add(x12.multiply(mx21)).subtract(m[1][1]).multiply(0.5));
				o[1][2] = x12.subtract(x10.multiply(mx02).add(x11.multiply(mx12)).add(x12.multiply(mx22)).subtract(m[1][2]).multiply(0.5));
				o[2][0] = x20.subtract(x20.multiply(mx00).add(x21.multiply(mx10)).add(x22.multiply(mx20)).subtract(m[2][0]).multiply(0.5));
				o[2][1] = x21.subtract(x20.multiply(mx01).add(x21.multiply(mx11)).add(x22.multiply(mx21)).subtract(m[2][1]).multiply(0.5));
				o[2][2] = x22.subtract(x20.multiply(mx02).add(x21.multiply(mx12)).add(x22.multiply(mx22)).subtract(m[2][2]).multiply(0.5));

				// correction on each elements
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr00 = o[0][0].getReal() - m[0][0].getReal();
				double corr00 = o[0][0].Real - m[0][0].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr01 = o[0][1].getReal() - m[0][1].getReal();
				double corr01 = o[0][1].Real - m[0][1].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr02 = o[0][2].getReal() - m[0][2].getReal();
				double corr02 = o[0][2].Real - m[0][2].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr10 = o[1][0].getReal() - m[1][0].getReal();
				double corr10 = o[1][0].Real - m[1][0].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr11 = o[1][1].getReal() - m[1][1].getReal();
				double corr11 = o[1][1].Real - m[1][1].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr12 = o[1][2].getReal() - m[1][2].getReal();
				double corr12 = o[1][2].Real - m[1][2].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr20 = o[2][0].getReal() - m[2][0].getReal();
				double corr20 = o[2][0].Real - m[2][0].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr21 = o[2][1].getReal() - m[2][1].getReal();
				double corr21 = o[2][1].Real - m[2][1].Real;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double corr22 = o[2][2].getReal() - m[2][2].getReal();
				double corr22 = o[2][2].Real - m[2][2].Real;

				// Frobenius norm of the correction
				fn1 = corr00 * corr00 + corr01 * corr01 + corr02 * corr02 + corr10 * corr10 + corr11 * corr11 + corr12 * corr12 + corr20 * corr20 + corr21 * corr21 + corr22 * corr22;

				// convergence test
				if (FastMath.abs(fn1 - fn) <= threshold)
				{
					return o;
				}

				// prepare next iteration
				x00 = o[0][0];
				x01 = o[0][1];
				x02 = o[0][2];
				x10 = o[1][0];
				x11 = o[1][1];
				x12 = o[1][2];
				x20 = o[2][0];
				x21 = o[2][1];
				x22 = o[2][2];
				fn = fn1;

			}

			// the algorithm did not converge after 10 iterations
			throw new NotARotationMatrixException(LocalizedFormats.UNABLE_TO_ORTHOGONOLIZE_MATRIX, i - 1);

		}

		/// <summary>
		/// Compute the <i>distance</i> between two rotations.
		/// <p>The <i>distance</i> is intended here as a way to check if two
		/// rotations are almost similar (i.e. they transform vectors the same way)
		/// or very different. It is mathematically defined as the angle of
		/// the rotation r that prepended to one of the rotations gives the other
		/// one:</p>
		/// <pre>
		///        r<sub>1</sub>(r) = r<sub>2</sub>
		/// </pre>
		/// <p>This distance is an angle between 0 and &pi;. Its value is the smallest
		/// possible upper bound of the angle in radians between r<sub>1</sub>(v)
		/// and r<sub>2</sub>(v) for all possible vectors v. This upper bound is
		/// reached for some v. The distance is equal to 0 if and only if the two
		/// rotations are identical.</p>
		/// <p>Comparing two rotations should always be done using this value rather
		/// than for example comparing the components of the quaternions. It is much
		/// more stable, and has a geometric meaning. Also comparing quaternions
		/// components is error prone since for example quaternions (0.36, 0.48, -0.48, -0.64)
		/// and (-0.36, -0.48, 0.48, 0.64) represent exactly the same rotation despite
		/// their components are different (they are exact opposites).</p> </summary>
		/// <param name="r1"> first rotation </param>
		/// <param name="r2"> second rotation </param>
		/// @param <T> the type of the field elements </param>
		/// <returns> <i>distance</i> between r1 and r2 </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static <T extends mathlib.RealFieldElement<T>> T distance(final FieldRotation<T> r1, final FieldRotation<T> r2)
		public static T distance<T>(FieldRotation<T> r1, FieldRotation<T> r2) where T : mathlib.RealFieldElement<T>
		{
			return r1.applyInverseTo(r2).Angle;
		}

	}

}