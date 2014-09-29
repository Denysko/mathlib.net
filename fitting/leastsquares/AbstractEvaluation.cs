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
namespace mathlib.fitting.leastsquares
{

	using ArrayRealVector = mathlib.linear.ArrayRealVector;
	using DecompositionSolver = mathlib.linear.DecompositionSolver;
	using QRDecomposition = mathlib.linear.QRDecomposition;
	using RealMatrix = mathlib.linear.RealMatrix;
	using RealVector = mathlib.linear.RealVector;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// An implementation of <seealso cref="Evaluation"/> that is designed for extension. All of the
	/// methods implemented here use the methods that are left unimplemented.
	/// <p/>
	/// TODO cache results?
	/// 
	/// @version $Id: AbstractEvaluation.java 1571307 2014-02-24 14:58:07Z luc $
	/// @since 3.3
	/// </summary>
	public abstract class AbstractEvaluation : LeastSquaresProblem_Evaluation
	{
		public abstract RealVector Point {get;}
		public abstract RealVector Residuals {get;}
		public abstract RealMatrix Jacobian {get;}

		/// <summary>
		/// number of observations </summary>
		private readonly int observationSize;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="observationSize"> the number of observation. Needed for {@link
		///                        #getRMS()}. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: AbstractEvaluation(final int observationSize)
		internal AbstractEvaluation(int observationSize)
		{
			this.observationSize = observationSize;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix getCovariances(double threshold)
		{
			// Set up the Jacobian.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix j = this.getJacobian();
			RealMatrix j = this.Jacobian;

			// Compute transpose(J)J.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix jTj = j.transpose().multiply(j);
			RealMatrix jTj = j.transpose().multiply(j);

			// Compute the covariances matrix.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.DecompositionSolver solver = new mathlib.linear.QRDecomposition(jTj, threshold).getSolver();
			DecompositionSolver solver = (new QRDecomposition(jTj, threshold)).Solver;
			return solver.Inverse;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealVector getSigma(double covarianceSingularityThreshold)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix cov = this.getCovariances(covarianceSingularityThreshold);
			RealMatrix cov = this.getCovariances(covarianceSingularityThreshold);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nC = cov.getColumnDimension();
			int nC = cov.ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealVector sig = new mathlib.linear.ArrayRealVector(nC);
			RealVector sig = new ArrayRealVector(nC);
			for (int i = 0; i < nC; ++i)
			{
				sig.setEntry(i, FastMath.sqrt(cov.getEntry(i,i)));
			}
			return sig;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double RMS
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double cost = this.getCost();
				double cost = this.Cost;
				return FastMath.sqrt(cost * cost / this.observationSize);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double Cost
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.linear.ArrayRealVector r = new mathlib.linear.ArrayRealVector(this.getResiduals());
				ArrayRealVector r = new ArrayRealVector(this.Residuals);
				return FastMath.sqrt(r.dotProduct(r));
			}
		}

	}

}