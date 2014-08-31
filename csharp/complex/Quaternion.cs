using System;
using System.Text;

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

namespace org.apache.commons.math3.complex
{

	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// This class implements <a href="http://mathworld.wolfram.com/Quaternion.html">
	/// quaternions</a> (Hamilton's hypercomplex numbers).
	/// <br/>
	/// Instance of this class are guaranteed to be immutable.
	/// 
	/// @since 3.1
	/// @version $Id: Quaternion.java 1421249 2012-12-13 12:32:03Z erans $
	/// </summary>
	[Serializable]
	public sealed class Quaternion
	{
		/// <summary>
		/// Identity quaternion. </summary>
		public static readonly Quaternion IDENTITY = new Quaternion(1, 0, 0, 0);
		/// <summary>
		/// Zero quaternion. </summary>
		public static readonly Quaternion ZERO = new Quaternion(0, 0, 0, 0);
		/// <summary>
		/// i </summary>
		public static readonly Quaternion I = new Quaternion(0, 1, 0, 0);
		/// <summary>
		/// j </summary>
		public static readonly Quaternion J = new Quaternion(0, 0, 1, 0);
		/// <summary>
		/// k </summary>
		public static readonly Quaternion K = new Quaternion(0, 0, 0, 1);

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20092012L;

		/// <summary>
		/// First component (scalar part). </summary>
		private readonly double q0;
		/// <summary>
		/// Second component (first vector part). </summary>
		private readonly double q1;
		/// <summary>
		/// Third component (second vector part). </summary>
		private readonly double q2;
		/// <summary>
		/// Fourth component (third vector part). </summary>
		private readonly double q3;

		/// <summary>
		/// Builds a quaternion from its components.
		/// </summary>
		/// <param name="a"> Scalar component. </param>
		/// <param name="b"> First vector component. </param>
		/// <param name="c"> Second vector component. </param>
		/// <param name="d"> Third vector component. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Quaternion(final double a, final double b, final double c, final double d)
		public Quaternion(double a, double b, double c, double d)
		{
			this.q0 = a;
			this.q1 = b;
			this.q2 = c;
			this.q3 = d;
		}

		/// <summary>
		/// Builds a quaternion from scalar and vector parts.
		/// </summary>
		/// <param name="scalar"> Scalar part of the quaternion. </param>
		/// <param name="v"> Components of the vector part of the quaternion.
		/// </param>
		/// <exception cref="DimensionMismatchException"> if the array length is not 3. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Quaternion(final double scalar, final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public Quaternion(double scalar, double[] v)
		{
			if (v.Length != 3)
			{
				throw new DimensionMismatchException(v.Length, 3);
			}
			this.q0 = scalar;
			this.q1 = v[0];
			this.q2 = v[1];
			this.q3 = v[2];
		}

		/// <summary>
		/// Builds a pure quaternion from a vector (assuming that the scalar
		/// part is zero).
		/// </summary>
		/// <param name="v"> Components of the vector part of the pure quaternion. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Quaternion(final double[] v)
		public Quaternion(double[] v) : this(0, v)
		{
		}

		/// <summary>
		/// Returns the conjugate quaternion of the instance.
		/// </summary>
		/// <returns> the conjugate quaternion </returns>
		public Quaternion Conjugate
		{
			get
			{
				return new Quaternion(q0, -q1, -q2, -q3);
			}
		}

		/// <summary>
		/// Returns the Hamilton product of two quaternions.
		/// </summary>
		/// <param name="q1"> First quaternion. </param>
		/// <param name="q2"> Second quaternion. </param>
		/// <returns> the product {@code q1} and {@code q2}, in that order. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Quaternion multiply(final Quaternion q1, final Quaternion q2)
		public static Quaternion multiply(Quaternion q1, Quaternion q2)
		{
			// Components of the first quaternion.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q1a = q1.getQ0();
			double q1a = q1.Q0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q1b = q1.getQ1();
			double q1b = q1.Q1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q1c = q1.getQ2();
			double q1c = q1.Q2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q1d = q1.getQ3();
			double q1d = q1.Q3;

			// Components of the second quaternion.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q2a = q2.getQ0();
			double q2a = q2.Q0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q2b = q2.getQ1();
			double q2b = q2.Q1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q2c = q2.getQ2();
			double q2c = q2.Q2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double q2d = q2.getQ3();
			double q2d = q2.Q3;

			// Components of the product.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = q1a * q2a - q1b * q2b - q1c * q2c - q1d * q2d;
			double w = q1a * q2a - q1b * q2b - q1c * q2c - q1d * q2d;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = q1a * q2b + q1b * q2a + q1c * q2d - q1d * q2c;
			double x = q1a * q2b + q1b * q2a + q1c * q2d - q1d * q2c;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = q1a * q2c - q1b * q2d + q1c * q2a + q1d * q2b;
			double y = q1a * q2c - q1b * q2d + q1c * q2a + q1d * q2b;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = q1a * q2d + q1b * q2c - q1c * q2b + q1d * q2a;
			double z = q1a * q2d + q1b * q2c - q1c * q2b + q1d * q2a;

			return new Quaternion(w, x, y, z);
		}

		/// <summary>
		/// Returns the Hamilton product of the instance by a quaternion.
		/// </summary>
		/// <param name="q"> Quaternion. </param>
		/// <returns> the product of this instance with {@code q}, in that order. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Quaternion multiply(final Quaternion q)
		public Quaternion multiply(Quaternion q)
		{
			return multiply(this, q);
		}

		/// <summary>
		/// Computes the sum of two quaternions.
		/// </summary>
		/// <param name="q1"> Quaternion. </param>
		/// <param name="q2"> Quaternion. </param>
		/// <returns> the sum of {@code q1} and {@code q2}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Quaternion add(final Quaternion q1, final Quaternion q2)
		public static Quaternion add(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q1.Q0 + q2.Q0, q1.Q1 + q2.Q1, q1.Q2 + q2.Q2, q1.Q3 + q2.Q3);
		}

		/// <summary>
		/// Computes the sum of the instance and another quaternion.
		/// </summary>
		/// <param name="q"> Quaternion. </param>
		/// <returns> the sum of this instance and {@code q} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Quaternion add(final Quaternion q)
		public Quaternion add(Quaternion q)
		{
			return add(this, q);
		}

		/// <summary>
		/// Subtracts two quaternions.
		/// </summary>
		/// <param name="q1"> First Quaternion. </param>
		/// <param name="q2"> Second quaternion. </param>
		/// <returns> the difference between {@code q1} and {@code q2}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Quaternion subtract(final Quaternion q1, final Quaternion q2)
		public static Quaternion subtract(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q1.Q0 - q2.Q0, q1.Q1 - q2.Q1, q1.Q2 - q2.Q2, q1.Q3 - q2.Q3);
		}

		/// <summary>
		/// Subtracts a quaternion from the instance.
		/// </summary>
		/// <param name="q"> Quaternion. </param>
		/// <returns> the difference between this instance and {@code q}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Quaternion subtract(final Quaternion q)
		public Quaternion subtract(Quaternion q)
		{
			return subtract(this, q);
		}

		/// <summary>
		/// Computes the dot-product of two quaternions.
		/// </summary>
		/// <param name="q1"> Quaternion. </param>
		/// <param name="q2"> Quaternion. </param>
		/// <returns> the dot product of {@code q1} and {@code q2}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double dotProduct(final Quaternion q1, final Quaternion q2)
		public static double dotProduct(Quaternion q1, Quaternion q2)
		{
			return q1.Q0 * q2.Q0 + q1.Q1 * q2.Q1 + q1.Q2 * q2.Q2 + q1.Q3 * q2.Q3;
		}

		/// <summary>
		/// Computes the dot-product of the instance by a quaternion.
		/// </summary>
		/// <param name="q"> Quaternion. </param>
		/// <returns> the dot product of this instance and {@code q}. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double dotProduct(final Quaternion q)
		public double dotProduct(Quaternion q)
		{
			return dotProduct(this, q);
		}

		/// <summary>
		/// Computes the norm of the quaternion.
		/// </summary>
		/// <returns> the norm. </returns>
		public double Norm
		{
			get
			{
				return FastMath.sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
			}
		}

		/// <summary>
		/// Computes the normalized quaternion (the versor of the instance).
		/// The norm of the quaternion must not be zero.
		/// </summary>
		/// <returns> a normalized quaternion. </returns>
		/// <exception cref="ZeroException"> if the norm of the quaternion is zero. </exception>
		public Quaternion normalize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double norm = getNorm();
			double norm = Norm;

			if (norm < Precision.SAFE_MIN)
			{
				throw new ZeroException(LocalizedFormats.NORM, norm);
			}

			return new Quaternion(q0 / norm, q1 / norm, q2 / norm, q3 / norm);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other is Quaternion)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Quaternion q = (Quaternion) other;
				Quaternion q = (Quaternion) other;
				return q0 == q.Q0 && q1 == q.Q1 && q2 == q.Q2 && q3 == q.Q3;
			}

			return false;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int GetHashCode()
		{
			// "Effective Java" (second edition, p. 47).
			int result = 17;
			foreach (double comp in new double[] {q0, q1, q2, q3})
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int c = org.apache.commons.math3.util.MathUtils.hash(comp);
				int c = MathUtils.hash(comp);
				result = 31 * result + c;
			}
			return result;
		}

		/// <summary>
		/// Checks whether this instance is equal to another quaternion
		/// within a given tolerance.
		/// </summary>
		/// <param name="q"> Quaternion with which to compare the current quaternion. </param>
		/// <param name="eps"> Tolerance. </param>
		/// <returns> {@code true} if the each of the components are equal
		/// within the allowed absolute error. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean equals(final Quaternion q, final double eps)
		public bool Equals(Quaternion q, double eps)
		{
			return Precision.Equals(q0, q.Q0, eps) && Precision.Equals(q1, q.Q1, eps) && Precision.Equals(q2, q.Q2, eps) && Precision.Equals(q3, q.Q3, eps);
		}

		/// <summary>
		/// Checks whether the instance is a unit quaternion within a given
		/// tolerance.
		/// </summary>
		/// <param name="eps"> Tolerance (absolute error). </param>
		/// <returns> {@code true} if the norm is 1 within the given tolerance,
		/// {@code false} otherwise </returns>
		public bool isUnitQuaternion(double eps)
		{
			return Precision.Equals(Norm, 1d, eps);
		}

		/// <summary>
		/// Checks whether the instance is a pure quaternion within a given
		/// tolerance.
		/// </summary>
		/// <param name="eps"> Tolerance (absolute error). </param>
		/// <returns> {@code true} if the scalar part of the quaternion is zero. </returns>
		public bool isPureQuaternion(double eps)
		{
			return FastMath.abs(Q0) <= eps;
		}

		/// <summary>
		/// Returns the polar form of the quaternion.
		/// </summary>
		/// <returns> the unit quaternion with positive scalar part. </returns>
		public Quaternion PositivePolarForm
		{
			get
			{
				if (Q0 < 0)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Quaternion unitQ = normalize();
					Quaternion unitQ = normalize();
					// The quaternion of rotation (normalized quaternion) q and -q
					// are equivalent (i.e. represent the same rotation).
					return new Quaternion(-unitQ.Q0, -unitQ.Q1, -unitQ.Q2, -unitQ.Q3);
				}
				else
				{
					return this.normalize();
				}
			}
		}

		/// <summary>
		/// Returns the inverse of this instance.
		/// The norm of the quaternion must not be zero.
		/// </summary>
		/// <returns> the inverse. </returns>
		/// <exception cref="ZeroException"> if the norm (squared) of the quaternion is zero. </exception>
		public Quaternion Inverse
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double squareNorm = q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3;
				double squareNorm = q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3;
				if (squareNorm < Precision.SAFE_MIN)
				{
					throw new ZeroException(LocalizedFormats.NORM, squareNorm);
				}
    
				return new Quaternion(q0 / squareNorm, -q1 / squareNorm, -q2 / squareNorm, -q3 / squareNorm);
			}
		}

		/// <summary>
		/// Gets the first component of the quaternion (scalar part).
		/// </summary>
		/// <returns> the scalar part. </returns>
		public double Q0
		{
			get
			{
				return q0;
			}
		}

		/// <summary>
		/// Gets the second component of the quaternion (first component
		/// of the vector part).
		/// </summary>
		/// <returns> the first component of the vector part. </returns>
		public double Q1
		{
			get
			{
				return q1;
			}
		}

		/// <summary>
		/// Gets the third component of the quaternion (second component
		/// of the vector part).
		/// </summary>
		/// <returns> the second component of the vector part. </returns>
		public double Q2
		{
			get
			{
				return q2;
			}
		}

		/// <summary>
		/// Gets the fourth component of the quaternion (third component
		/// of the vector part).
		/// </summary>
		/// <returns> the third component of the vector part. </returns>
		public double Q3
		{
			get
			{
				return q3;
			}
		}

		/// <summary>
		/// Gets the scalar part of the quaternion.
		/// </summary>
		/// <returns> the scalar part. </returns>
		/// <seealso cref= #getQ0() </seealso>
		public double ScalarPart
		{
			get
			{
				return Q0;
			}
		}

		/// <summary>
		/// Gets the three components of the vector part of the quaternion.
		/// </summary>
		/// <returns> the vector part. </returns>
		/// <seealso cref= #getQ1() </seealso>
		/// <seealso cref= #getQ2() </seealso>
		/// <seealso cref= #getQ3() </seealso>
		public double[] VectorPart
		{
			get
			{
				return new double[] {Q1, Q2, Q3};
			}
		}

		/// <summary>
		/// Multiplies the instance by a scalar.
		/// </summary>
		/// <param name="alpha"> Scalar factor. </param>
		/// <returns> a scaled quaternion. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Quaternion multiply(final double alpha)
		public Quaternion multiply(double alpha)
		{
			return new Quaternion(alpha * q0, alpha * q1, alpha * q2, alpha * q3);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override string ToString()
		{
			const string sp = " ";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StringBuilder s = new StringBuilder();
			StringBuilder s = new StringBuilder();
			s.Append("[").Append(q0).Append(sp).Append(q1).Append(sp).Append(q2).Append(sp).Append(q3).Append("]");

			return s.ToString();
		}
	}

}