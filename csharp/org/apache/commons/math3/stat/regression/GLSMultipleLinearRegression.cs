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
namespace org.apache.commons.math3.stat.regression
{

	using LUDecomposition = org.apache.commons.math3.linear.LUDecomposition;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;

	/// <summary>
	/// The GLS implementation of multiple linear regression.
	/// 
	/// GLS assumes a general covariance matrix Omega of the error
	/// <pre>
	/// u ~ N(0, Omega)
	/// </pre>
	/// 
	/// Estimated by GLS,
	/// <pre>
	/// b=(X' Omega^-1 X)^-1X'Omega^-1 y
	/// </pre>
	/// whose variance is
	/// <pre>
	/// Var(b)=(X' Omega^-1 X)^-1
	/// </pre>
	/// @version $Id: GLSMultipleLinearRegression.java 1553598 2013-12-26 22:18:02Z psteitz $
	/// @since 2.0
	/// </summary>
	public class GLSMultipleLinearRegression : AbstractMultipleLinearRegression
	{

		/// <summary>
		/// Covariance matrix. </summary>
		private RealMatrix Omega;

		/// <summary>
		/// Inverse of covariance matrix. </summary>
		private RealMatrix OmegaInverse_Renamed;

		/// <summary>
		/// Replace sample data, overriding any previous sample. </summary>
		/// <param name="y"> y values of the sample </param>
		/// <param name="x"> x values of the sample </param>
		/// <param name="covariance"> array representing the covariance matrix </param>
		public virtual void newSampleData(double[] y, double[][] x, double[][] covariance)
		{
			validateSampleData(x, y);
			newYSampleData(y);
			newXSampleData(x);
			validateCovarianceData(x, covariance);
			newCovarianceData(covariance);
		}

		/// <summary>
		/// Add the covariance data.
		/// </summary>
		/// <param name="omega"> the [n,n] array representing the covariance </param>
		protected internal virtual void newCovarianceData(double[][] omega)
		{
			this.Omega = new Array2DRowRealMatrix(omega);
			this.OmegaInverse_Renamed = null;
		}

		/// <summary>
		/// Get the inverse of the covariance.
		/// <p>The inverse of the covariance matrix is lazily evaluated and cached.</p> </summary>
		/// <returns> inverse of the covariance </returns>
		protected internal virtual RealMatrix OmegaInverse
		{
			get
			{
				if (OmegaInverse_Renamed == null)
				{
					OmegaInverse_Renamed = (new LUDecomposition(Omega)).Solver.Inverse;
				}
				return OmegaInverse_Renamed;
			}
		}

		/// <summary>
		/// Calculates beta by GLS.
		/// <pre>
		///  b=(X' Omega^-1 X)^-1X'Omega^-1 y
		/// </pre> </summary>
		/// <returns> beta </returns>
		protected internal override RealVector calculateBeta()
		{
			RealMatrix OI = OmegaInverse;
			RealMatrix XT = X.transpose();
			RealMatrix XTOIX = XT.multiply(OI).multiply(X);
			RealMatrix inverse = (new LUDecomposition(XTOIX)).Solver.Inverse;
			return inverse.multiply(XT).multiply(OI).operate(Y);
		}

		/// <summary>
		/// Calculates the variance on the beta.
		/// <pre>
		///  Var(b)=(X' Omega^-1 X)^-1
		/// </pre> </summary>
		/// <returns> The beta variance matrix </returns>
		protected internal override RealMatrix calculateBetaVariance()
		{
			RealMatrix OI = OmegaInverse;
			RealMatrix XTOIX = X.transpose().multiply(OI).multiply(X);
			return (new LUDecomposition(XTOIX)).Solver.Inverse;
		}


		/// <summary>
		/// Calculates the estimated variance of the error term using the formula
		/// <pre>
		///  Var(u) = Tr(u' Omega^-1 u)/(n-k)
		/// </pre>
		/// where n and k are the row and column dimensions of the design
		/// matrix X.
		/// </summary>
		/// <returns> error variance
		/// @since 2.2 </returns>
		protected internal override double calculateErrorVariance()
		{
			RealVector residuals = calculateResiduals();
			double t = residuals.dotProduct(OmegaInverse.operate(residuals));
			return t / (X.RowDimension - X.ColumnDimension);

		}

	}

}