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
namespace mathlib.distribution
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using EigenDecomposition = mathlib.linear.EigenDecomposition;
	using NonPositiveDefiniteMatrixException = mathlib.linear.NonPositiveDefiniteMatrixException;
	using RealMatrix = mathlib.linear.RealMatrix;
	using SingularMatrixException = mathlib.linear.SingularMatrixException;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using Well19937c = mathlib.random.Well19937c;
	using FastMath = mathlib.util.FastMath;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// Implementation of the multivariate normal (Gaussian) distribution.
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Multivariate_normal_distribution">
	/// Multivariate normal distribution (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/MultivariateNormalDistribution.html">
	/// Multivariate normal distribution (MathWorld)</a>
	/// 
	/// @version $Id: MultivariateNormalDistribution.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.1 </seealso>
	public class MultivariateNormalDistribution : AbstractMultivariateRealDistribution
	{
		/// <summary>
		/// Vector of means. </summary>
		private readonly double[] means;
		/// <summary>
		/// Covariance matrix. </summary>
		private readonly RealMatrix covarianceMatrix;
		/// <summary>
		/// The matrix inverse of the covariance matrix. </summary>
		private readonly RealMatrix covarianceMatrixInverse;
		/// <summary>
		/// The determinant of the covariance matrix. </summary>
		private readonly double covarianceMatrixDeterminant;
		/// <summary>
		/// Matrix used in computation of samples. </summary>
		private readonly RealMatrix samplingMatrix;

		/// <summary>
		/// Creates a multivariate normal distribution with the given mean vector and
		/// covariance matrix.
		/// <br/>
		/// The number of dimensions is equal to the length of the mean vector
		/// and to the number of rows and columns of the covariance matrix.
		/// It is frequently written as "p" in formulae.
		/// </summary>
		/// <param name="means"> Vector of means. </param>
		/// <param name="covariances"> Covariance matrix. </param>
		/// <exception cref="DimensionMismatchException"> if the arrays length are
		/// inconsistent. </exception>
		/// <exception cref="SingularMatrixException"> if the eigenvalue decomposition cannot
		/// be performed on the provided covariance matrix. </exception>
		/// <exception cref="NonPositiveDefiniteMatrixException"> if any of the eigenvalues is
		/// negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MultivariateNormalDistribution(final double[] means, final double[][] covariances) throws mathlib.linear.SingularMatrixException, mathlib.exception.DimensionMismatchException, mathlib.linear.NonPositiveDefiniteMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public MultivariateNormalDistribution(double[] means, double[][] covariances) : this(new Well19937c(), means, covariances)
		{
		}

		/// <summary>
		/// Creates a multivariate normal distribution with the given mean vector and
		/// covariance matrix.
		/// <br/>
		/// The number of dimensions is equal to the length of the mean vector
		/// and to the number of rows and columns of the covariance matrix.
		/// It is frequently written as "p" in formulae.
		/// </summary>
		/// <param name="rng"> Random Number Generator. </param>
		/// <param name="means"> Vector of means. </param>
		/// <param name="covariances"> Covariance matrix. </param>
		/// <exception cref="DimensionMismatchException"> if the arrays length are
		/// inconsistent. </exception>
		/// <exception cref="SingularMatrixException"> if the eigenvalue decomposition cannot
		/// be performed on the provided covariance matrix. </exception>
		/// <exception cref="NonPositiveDefiniteMatrixException"> if any of the eigenvalues is
		/// negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MultivariateNormalDistribution(mathlib.random.RandomGenerator rng, final double[] means, final double[][] covariances) throws mathlib.linear.SingularMatrixException, mathlib.exception.DimensionMismatchException, mathlib.linear.NonPositiveDefiniteMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public MultivariateNormalDistribution(RandomGenerator rng, double[] means, double[][] covariances) : base(rng, means.Length)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = means.length;
			int dim = means.Length;

			if (covariances.Length != dim)
			{
				throw new DimensionMismatchException(covariances.Length, dim);
			}

			for (int i = 0; i < dim; i++)
			{
				if (dim != covariances[i].Length)
				{
					throw new DimensionMismatchException(covariances[i].Length, dim);
				}
			}

			this.means = MathArrays.copyOf(means);

			covarianceMatrix = new Array2DRowRealMatrix(covariances);

			// Covariance matrix eigen decomposition.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.EigenDecomposition covMatDec = new mathlib.linear.EigenDecomposition(covarianceMatrix);
			EigenDecomposition covMatDec = new EigenDecomposition(covarianceMatrix);

			// Compute and store the inverse.
			covarianceMatrixInverse = covMatDec.Solver.Inverse;
			// Compute and store the determinant.
			covarianceMatrixDeterminant = covMatDec.Determinant;

			// Eigenvalues of the covariance matrix.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] covMatEigenvalues = covMatDec.getRealEigenvalues();
			double[] covMatEigenvalues = covMatDec.RealEigenvalues;

			for (int i = 0; i < covMatEigenvalues.Length; i++)
			{
				if (covMatEigenvalues[i] < 0)
				{
					throw new NonPositiveDefiniteMatrixException(covMatEigenvalues[i], i, 0);
				}
			}

			// Matrix where each column is an eigenvector of the covariance matrix.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.Array2DRowRealMatrix covMatEigenvectors = new mathlib.linear.Array2DRowRealMatrix(dim, dim);
			Array2DRowRealMatrix covMatEigenvectors = new Array2DRowRealMatrix(dim, dim);
			for (int v = 0; v < dim; v++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] evec = covMatDec.getEigenvector(v).toArray();
				double[] evec = covMatDec.getEigenvector(v).toArray();
				covMatEigenvectors.setColumn(v, evec);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.linear.RealMatrix tmpMatrix = covMatEigenvectors.transpose();
			RealMatrix tmpMatrix = covMatEigenvectors.transpose();

			// Scale each eigenvector by the square root of its eigenvalue.
			for (int row = 0; row < dim; row++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double factor = mathlib.util.FastMath.sqrt(covMatEigenvalues[row]);
				double factor = FastMath.sqrt(covMatEigenvalues[row]);
				for (int col = 0; col < dim; col++)
				{
					tmpMatrix.multiplyEntry(row, col, factor);
				}
			}

			samplingMatrix = covMatEigenvectors.multiply(tmpMatrix);
		}

		/// <summary>
		/// Gets the mean vector.
		/// </summary>
		/// <returns> the mean vector. </returns>
		public virtual double[] Means
		{
			get
			{
				return MathArrays.copyOf(means);
			}
		}

		/// <summary>
		/// Gets the covariance matrix.
		/// </summary>
		/// <returns> the covariance matrix. </returns>
		public virtual RealMatrix Covariances
		{
			get
			{
				return covarianceMatrix.copy();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double density(final double[] vals) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double density(double[] vals)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			if (vals.Length != dim)
			{
				throw new DimensionMismatchException(vals.Length, dim);
			}

			return FastMath.pow(2 * FastMath.PI, -0.5 * dim) * FastMath.pow(covarianceMatrixDeterminant, -0.5) * getExponentTerm(vals);
		}

		/// <summary>
		/// Gets the square root of each element on the diagonal of the covariance
		/// matrix.
		/// </summary>
		/// <returns> the standard deviations. </returns>
		public virtual double[] StandardDeviations
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int dim = getDimension();
				int dim = Dimension;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] std = new double[dim];
				double[] std = new double[dim];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] s = covarianceMatrix.getData();
				double[][] s = covarianceMatrix.Data;
				for (int i = 0; i < dim; i++)
				{
					std[i] = FastMath.sqrt(s[i][i]);
				}
				return std;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[] sample()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] normalVals = new double[dim];
			double[] normalVals = new double[dim];

			for (int i = 0; i < dim; i++)
			{
				normalVals[i] = random.nextGaussian();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vals = samplingMatrix.operate(normalVals);
			double[] vals = samplingMatrix.operate(normalVals);

			for (int i = 0; i < dim; i++)
			{
				vals[i] += means[i];
			}

			return vals;
		}

		/// <summary>
		/// Computes the term used in the exponent (see definition of the distribution).
		/// </summary>
		/// <param name="values"> Values at which to compute density. </param>
		/// <returns> the multiplication factor of density calculations. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double getExponentTerm(final double[] values)
		private double getExponentTerm(double[] values)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] centered = new double[values.length];
			double[] centered = new double[values.Length];
			for (int i = 0; i < centered.Length; i++)
			{
				centered[i] = values[i] - Means[i];
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] preMultiplied = covarianceMatrixInverse.preMultiply(centered);
			double[] preMultiplied = covarianceMatrixInverse.preMultiply(centered);
			double sum = 0;
			for (int i = 0; i < preMultiplied.Length; i++)
			{
				sum += preMultiplied[i] * centered[i];
			}
			return FastMath.exp(-0.5 * sum);
		}
	}

}