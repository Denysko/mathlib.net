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


	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class provides conversions related to <a
	/// href="http://mathworld.wolfram.com/SphericalCoordinates.html">spherical coordinates</a>.
	/// <p>
	/// The conventions used here are the mathematical ones, i.e. spherical coordinates are
	/// related to Cartesian coordinates as follows:
	/// </p>
	/// <ul>
	///   <li>x = r cos(&theta;) sin(&Phi;)</li>
	///   <li>y = r sin(&theta;) sin(&Phi;)</li>
	///   <li>z = r cos(&Phi;)</li>
	/// </ul>
	/// <ul>
	///   <li>r       = &radic;(x<sup>2</sup>+y<sup>2</sup>+z<sup>2</sup>)</li>
	///   <li>&theta; = atan2(y, x)</li>
	///   <li>&Phi;   = acos(z/r)</li>
	/// </ul>
	/// <p>
	/// r is the radius, &theta; is the azimuthal angle in the x-y plane and &Phi; is the polar
	/// (co-latitude) angle. These conventions are <em>different</em> from the conventions used
	/// in physics (and in particular in spherical harmonics) where the meanings of &theta; and
	/// &Phi; are reversed.
	/// </p>
	/// <p>
	/// This class provides conversion of coordinates and also of gradient and Hessian
	/// between spherical and Cartesian coordinates.
	/// </p>
	/// @since 3.2
	/// @version $Id: SphericalCoordinates.java 1443364 2013-02-07 09:28:04Z luc $
	/// </summary>
	[Serializable]
	public class SphericalCoordinates
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20130206L;

		/// <summary>
		/// Cartesian coordinates. </summary>
		private readonly Vector3D v;

		/// <summary>
		/// Radius. </summary>
		private readonly double r;

		/// <summary>
		/// Azimuthal angle in the x-y plane &theta;. </summary>
		private readonly double theta;

		/// <summary>
		/// Polar angle (co-latitude) &Phi;. </summary>
		private readonly double phi;

		/// <summary>
		/// Jacobian of (r, &theta; &Phi). </summary>
		private double[][] jacobian;

		/// <summary>
		/// Hessian of radius. </summary>
		private double[][] rHessian;

		/// <summary>
		/// Hessian of azimuthal angle in the x-y plane &theta;. </summary>
		private double[][] thetaHessian;

		/// <summary>
		/// Hessian of polar (co-latitude) angle &Phi;. </summary>
		private double[][] phiHessian;

		/// <summary>
		/// Build a spherical coordinates transformer from Cartesian coordinates. </summary>
		/// <param name="v"> Cartesian coordinates </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalCoordinates(final Vector3D v)
		public SphericalCoordinates(Vector3D v)
		{

			// Cartesian coordinates
			this.v = v;

			// remaining spherical coordinates
			this.r = v.Norm;
			this.theta = v.Alpha;
			this.phi = FastMath.acos(v.Z / r);

		}

		/// <summary>
		/// Build a spherical coordinates transformer from spherical coordinates. </summary>
		/// <param name="r"> radius </param>
		/// <param name="theta"> azimuthal angle in x-y plane </param>
		/// <param name="phi"> polar (co-latitude) angle </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SphericalCoordinates(final double r, final double theta, final double phi)
		public SphericalCoordinates(double r, double theta, double phi)
		{

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

			// spherical coordinates
			this.r = r;
			this.theta = theta;
			this.phi = phi;

			// Cartesian coordinates
			this.v = new Vector3D(r * cosTheta * sinPhi, r * sinTheta * sinPhi, r * cosPhi);

		}

		/// <summary>
		/// Get the Cartesian coordinates. </summary>
		/// <returns> Cartesian coordinates </returns>
		public virtual Vector3D Cartesian
		{
			get
			{
				return v;
			}
		}

		/// <summary>
		/// Get the radius. </summary>
		/// <returns> radius r </returns>
		/// <seealso cref= #getTheta() </seealso>
		/// <seealso cref= #getPhi() </seealso>
		public virtual double R
		{
			get
			{
				return r;
			}
		}

		/// <summary>
		/// Get the azimuthal angle in x-y plane. </summary>
		/// <returns> azimuthal angle in x-y plane &theta; </returns>
		/// <seealso cref= #getR() </seealso>
		/// <seealso cref= #getPhi() </seealso>
		public virtual double Theta
		{
			get
			{
				return theta;
			}
		}

		/// <summary>
		/// Get the polar (co-latitude) angle. </summary>
		/// <returns> polar (co-latitude) angle &Phi; </returns>
		/// <seealso cref= #getR() </seealso>
		/// <seealso cref= #getTheta() </seealso>
		public virtual double Phi
		{
			get
			{
				return phi;
			}
		}

		/// <summary>
		/// Convert a gradient with respect to spherical coordinates into a gradient
		/// with respect to Cartesian coordinates. </summary>
		/// <param name="sGradient"> gradient with respect to spherical coordinates
		/// {df/dr, df/d&theta;, df/d&Phi;} </param>
		/// <returns> gradient with respect to Cartesian coordinates
		/// {df/dx, df/dy, df/dz} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] toCartesianGradient(final double[] sGradient)
		public virtual double[] toCartesianGradient(double[] sGradient)
		{

			// lazy evaluation of Jacobian
			computeJacobian();

			// compose derivatives as gradient^T . J
			// the expressions have been simplified since we know jacobian[1][2] = dTheta/dZ = 0
			return new double[] {sGradient[0] * jacobian[0][0] + sGradient[1] * jacobian[1][0] + sGradient[2] * jacobian[2][0], sGradient[0] * jacobian[0][1] + sGradient[1] * jacobian[1][1] + sGradient[2] * jacobian[2][1], sGradient[0] * jacobian[0][2] + sGradient[2] * jacobian[2][2]};

		}

		/// <summary>
		/// Convert a Hessian with respect to spherical coordinates into a Hessian
		/// with respect to Cartesian coordinates.
		/// <p>
		/// As Hessian are always symmetric, we use only the lower left part of the provided
		/// spherical Hessian, so the upper part may not be initialized. However, we still
		/// do fill up the complete array we create, with guaranteed symmetry.
		/// </p> </summary>
		/// <param name="sHessian"> Hessian with respect to spherical coordinates
		/// {{d<sup>2</sup>f/dr<sup>2</sup>, d<sup>2</sup>f/drd&theta;, d<sup>2</sup>f/drd&Phi;},
		///  {d<sup>2</sup>f/drd&theta;, d<sup>2</sup>f/d&theta;<sup>2</sup>, d<sup>2</sup>f/d&theta;d&Phi;},
		///  {d<sup>2</sup>f/drd&Phi;, d<sup>2</sup>f/d&theta;d&Phi;, d<sup>2</sup>f/d&Phi;<sup>2</sup>} </param>
		/// <param name="sGradient"> gradient with respect to spherical coordinates
		/// {df/dr, df/d&theta;, df/d&Phi;} </param>
		/// <returns> Hessian with respect to Cartesian coordinates
		/// {{d<sup>2</sup>f/dx<sup>2</sup>, d<sup>2</sup>f/dxdy, d<sup>2</sup>f/dxdz},
		///  {d<sup>2</sup>f/dxdy, d<sup>2</sup>f/dy<sup>2</sup>, d<sup>2</sup>f/dydz},
		///  {d<sup>2</sup>f/dxdz, d<sup>2</sup>f/dydz, d<sup>2</sup>f/dz<sup>2</sup>}} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[][] toCartesianHessian(final double[][] sHessian, final double[] sGradient)
		public virtual double[][] toCartesianHessian(double[][] sHessian, double[] sGradient)
		{

			computeJacobian();
			computeHessians();

			// compose derivative as J^T . H_f . J + df/dr H_r + df/dtheta H_theta + df/dphi H_phi
			// the expressions have been simplified since we know jacobian[1][2] = dTheta/dZ = 0
			// and H_theta is only a 2x2 matrix as it does not depend on z
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] hj = new double[3][3];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] hj = new double[3][3];
			double[][] hj = RectangularArrays.ReturnRectangularDoubleArray(3, 3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] cHessian = new double[3][3];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] cHessian = new double[3][3];
			double[][] cHessian = RectangularArrays.ReturnRectangularDoubleArray(3, 3);

			// compute H_f . J
			// beware we use ONLY the lower-left part of sHessian
			hj[0][0] = sHessian[0][0] * jacobian[0][0] + sHessian[1][0] * jacobian[1][0] + sHessian[2][0] * jacobian[2][0];
			hj[0][1] = sHessian[0][0] * jacobian[0][1] + sHessian[1][0] * jacobian[1][1] + sHessian[2][0] * jacobian[2][1];
			hj[0][2] = sHessian[0][0] * jacobian[0][2] + sHessian[2][0] * jacobian[2][2];
			hj[1][0] = sHessian[1][0] * jacobian[0][0] + sHessian[1][1] * jacobian[1][0] + sHessian[2][1] * jacobian[2][0];
			hj[1][1] = sHessian[1][0] * jacobian[0][1] + sHessian[1][1] * jacobian[1][1] + sHessian[2][1] * jacobian[2][1];
			// don't compute hj[1][2] as it is not used below
			hj[2][0] = sHessian[2][0] * jacobian[0][0] + sHessian[2][1] * jacobian[1][0] + sHessian[2][2] * jacobian[2][0];
			hj[2][1] = sHessian[2][0] * jacobian[0][1] + sHessian[2][1] * jacobian[1][1] + sHessian[2][2] * jacobian[2][1];
			hj[2][2] = sHessian[2][0] * jacobian[0][2] + sHessian[2][2] * jacobian[2][2];

			// compute lower-left part of J^T . H_f . J
			cHessian[0][0] = jacobian[0][0] * hj[0][0] + jacobian[1][0] * hj[1][0] + jacobian[2][0] * hj[2][0];
			cHessian[1][0] = jacobian[0][1] * hj[0][0] + jacobian[1][1] * hj[1][0] + jacobian[2][1] * hj[2][0];
			cHessian[2][0] = jacobian[0][2] * hj[0][0] + jacobian[2][2] * hj[2][0];
			cHessian[1][1] = jacobian[0][1] * hj[0][1] + jacobian[1][1] * hj[1][1] + jacobian[2][1] * hj[2][1];
			cHessian[2][1] = jacobian[0][2] * hj[0][1] + jacobian[2][2] * hj[2][1];
			cHessian[2][2] = jacobian[0][2] * hj[0][2] + jacobian[2][2] * hj[2][2];

			// add gradient contribution
			cHessian[0][0] += sGradient[0] * rHessian[0][0] + sGradient[1] * thetaHessian[0][0] + sGradient[2] * phiHessian[0][0];
			cHessian[1][0] += sGradient[0] * rHessian[1][0] + sGradient[1] * thetaHessian[1][0] + sGradient[2] * phiHessian[1][0];
			cHessian[2][0] += sGradient[0] * rHessian[2][0] + sGradient[2] * phiHessian[2][0];
			cHessian[1][1] += sGradient[0] * rHessian[1][1] + sGradient[1] * thetaHessian[1][1] + sGradient[2] * phiHessian[1][1];
			cHessian[2][1] += sGradient[0] * rHessian[2][1] + sGradient[2] * phiHessian[2][1];
			cHessian[2][2] += sGradient[0] * rHessian[2][2] + sGradient[2] * phiHessian[2][2];

			// ensure symmetry
			cHessian[0][1] = cHessian[1][0];
			cHessian[0][2] = cHessian[2][0];
			cHessian[1][2] = cHessian[2][1];

			return cHessian;

		}

		/// <summary>
		/// Lazy evaluation of (r, &theta;, &phi;) Jacobian.
		/// </summary>
		private void computeJacobian()
		{
			if (jacobian == null)
			{

				// intermediate variables
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = v.getX();
				double x = v.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = v.getY();
				double y = v.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = v.getZ();
				double z = v.Z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rho2 = x * x + y * y;
				double rho2 = x * x + y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rho = org.apache.commons.math3.util.FastMath.sqrt(rho2);
				double rho = FastMath.sqrt(rho2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r2 = rho2 + z * z;
				double r2 = rho2 + z * z;

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: jacobian = new double[3][3];
				jacobian = RectangularArrays.ReturnRectangularDoubleArray(3, 3);

				// row representing the gradient of r
				jacobian[0][0] = x / r;
				jacobian[0][1] = y / r;
				jacobian[0][2] = z / r;

				// row representing the gradient of theta
				jacobian[1][0] = -y / rho2;
				jacobian[1][1] = x / rho2;
				// jacobian[1][2] is already set to 0 at allocation time

				// row representing the gradient of phi
				jacobian[2][0] = x * z / (rho * r2);
				jacobian[2][1] = y * z / (rho * r2);
				jacobian[2][2] = -rho / r2;

			}
		}

		/// <summary>
		/// Lazy evaluation of Hessians.
		/// </summary>
		private void computeHessians()
		{

			if (rHessian == null)
			{

				// intermediate variables
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = v.getX();
				double x = v.X;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = v.getY();
				double y = v.Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = v.getZ();
				double z = v.Z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
				double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z2 = z * z;
				double z2 = z * z;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rho2 = x2 + y2;
				double rho2 = x2 + y2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rho = org.apache.commons.math3.util.FastMath.sqrt(rho2);
				double rho = FastMath.sqrt(rho2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r2 = rho2 + z2;
				double r2 = rho2 + z2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xOr = x / r;
				double xOr = x / r;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yOr = y / r;
				double yOr = y / r;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zOr = z / r;
				double zOr = z / r;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xOrho2 = x / rho2;
				double xOrho2 = x / rho2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yOrho2 = y / rho2;
				double yOrho2 = y / rho2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xOr3 = xOr / r2;
				double xOr3 = xOr / r2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yOr3 = yOr / r2;
				double yOr3 = yOr / r2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double zOr3 = zOr / r2;
				double zOr3 = zOr / r2;

				// lower-left part of Hessian of r
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: rHessian = new double[3][3];
				rHessian = RectangularArrays.ReturnRectangularDoubleArray(3, 3);
				rHessian[0][0] = y * yOr3 + z * zOr3;
				rHessian[1][0] = -x * yOr3;
				rHessian[2][0] = -z * xOr3;
				rHessian[1][1] = x * xOr3 + z * zOr3;
				rHessian[2][1] = -y * zOr3;
				rHessian[2][2] = x * xOr3 + y * yOr3;

				// upper-right part is symmetric
				rHessian[0][1] = rHessian[1][0];
				rHessian[0][2] = rHessian[2][0];
				rHessian[1][2] = rHessian[2][1];

				// lower-left part of Hessian of azimuthal angle theta
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: thetaHessian = new double[2][2];
				thetaHessian = RectangularArrays.ReturnRectangularDoubleArray(2, 2);
				thetaHessian[0][0] = 2 * xOrho2 * yOrho2;
				thetaHessian[1][0] = yOrho2 * yOrho2 - xOrho2 * xOrho2;
				thetaHessian[1][1] = -2 * xOrho2 * yOrho2;

				// upper-right part is symmetric
				thetaHessian[0][1] = thetaHessian[1][0];

				// lower-left part of Hessian of polar (co-latitude) angle phi
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rhor2 = rho * r2;
				double rhor2 = rho * r2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rho2r2 = rho * rhor2;
				double rho2r2 = rho * rhor2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rhor4 = rhor2 * r2;
				double rhor4 = rhor2 * r2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rho3r4 = rhor4 * rho2;
				double rho3r4 = rhor4 * rho2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double r2P2rho2 = 3 * rho2 + z2;
				double r2P2rho2 = 3 * rho2 + z2;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: phiHessian = new double[3][3];
				phiHessian = RectangularArrays.ReturnRectangularDoubleArray(3, 3);
				phiHessian[0][0] = z * (rho2r2 - x2 * r2P2rho2) / rho3r4;
				phiHessian[1][0] = -x * y * z * r2P2rho2 / rho3r4;
				phiHessian[2][0] = x * (rho2 - z2) / rhor4;
				phiHessian[1][1] = z * (rho2r2 - y2 * r2P2rho2) / rho3r4;
				phiHessian[2][1] = y * (rho2 - z2) / rhor4;
				phiHessian[2][2] = 2 * rho * zOr3 / r;

				// upper-right part is symmetric
				phiHessian[0][1] = phiHessian[1][0];
				phiHessian[0][2] = phiHessian[2][0];
				phiHessian[1][2] = phiHessian[2][1];

			}

		}

		/// <summary>
		/// Replace the instance with a data transfer object for serialization. </summary>
		/// <returns> data transfer object that will be serialized </returns>
		private object writeReplace()
		{
			return new DataTransferObject(v.X, v.Y, v.Z);
		}

		/// <summary>
		/// Internal class used only for serialization. </summary>
		[Serializable]
		private class DataTransferObject
		{

			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20130206L;

			/// <summary>
			/// Abscissa.
			/// @serial
			/// </summary>
			internal readonly double x;

			/// <summary>
			/// Ordinate.
			/// @serial
			/// </summary>
			internal readonly double y;

			/// <summary>
			/// Height.
			/// @serial
			/// </summary>
			internal readonly double z;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="x"> abscissa </param>
			/// <param name="y"> ordinate </param>
			/// <param name="z"> height </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DataTransferObject(final double x, final double y, final double z)
			public DataTransferObject(double x, double y, double z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			/// <summary>
			/// Replace the deserialized data transfer object with a <seealso cref="SphericalCoordinates"/>. </summary>
			/// <returns> replacement <seealso cref="SphericalCoordinates"/> </returns>
			internal virtual object readResolve()
			{
				return new SphericalCoordinates(new Vector3D(x, y, z));
			}

		}

	}

}