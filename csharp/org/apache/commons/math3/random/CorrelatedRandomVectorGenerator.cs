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

namespace org.apache.commons.math3.random
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RectangularCholeskyDecomposition = org.apache.commons.math3.linear.RectangularCholeskyDecomposition;

	/// <summary>
	/// A <seealso cref="RandomVectorGenerator"/> that generates vectors with with
	/// correlated components.
	/// <p>Random vectors with correlated components are built by combining
	/// the uncorrelated components of another random vector in such a way that
	/// the resulting correlations are the ones specified by a positive
	/// definite covariance matrix.</p>
	/// <p>The main use for correlated random vector generation is for Monte-Carlo
	/// simulation of physical problems with several variables, for example to
	/// generate error vectors to be added to a nominal vector. A particularly
	/// interesting case is when the generated vector should be drawn from a <a
	/// href="http://en.wikipedia.org/wiki/Multivariate_normal_distribution">
	/// Multivariate Normal Distribution</a>. The approach using a Cholesky
	/// decomposition is quite usual in this case. However, it can be extended
	/// to other cases as long as the underlying random generator provides
	/// <seealso cref="NormalizedRandomGenerator normalized values"/> like {@link
	/// GaussianRandomGenerator} or <seealso cref="UniformRandomGenerator"/>.</p>
	/// <p>Sometimes, the covariance matrix for a given simulation is not
	/// strictly positive definite. This means that the correlations are
	/// not all independent from each other. In this case, however, the non
	/// strictly positive elements found during the Cholesky decomposition
	/// of the covariance matrix should not be negative either, they
	/// should be null. Another non-conventional extension handling this case
	/// is used here. Rather than computing <code>C = U<sup>T</sup>.U</code>
	/// where <code>C</code> is the covariance matrix and <code>U</code>
	/// is an upper-triangular matrix, we compute <code>C = B.B<sup>T</sup></code>
	/// where <code>B</code> is a rectangular matrix having
	/// more rows than columns. The number of columns of <code>B</code> is
	/// the rank of the covariance matrix, and it is the dimension of the
	/// uncorrelated random vector that is needed to compute the component
	/// of the correlated vector. This class handles this situation
	/// automatically.</p>
	/// 
	/// @version $Id: CorrelatedRandomVectorGenerator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class CorrelatedRandomVectorGenerator : RandomVectorGenerator
	{
		/// <summary>
		/// Mean vector. </summary>
		private readonly double[] mean;
		/// <summary>
		/// Underlying generator. </summary>
		private readonly NormalizedRandomGenerator generator;
		/// <summary>
		/// Storage for the normalized vector. </summary>
		private readonly double[] normalized;
		/// <summary>
		/// Root of the covariance matrix. </summary>
		private readonly RealMatrix root;

		/// <summary>
		/// Builds a correlated random vector generator from its mean
		/// vector and covariance matrix.
		/// </summary>
		/// <param name="mean"> Expected mean values for all components. </param>
		/// <param name="covariance"> Covariance matrix. </param>
		/// <param name="small"> Diagonal elements threshold under which  column are
		/// considered to be dependent on previous ones and are discarded </param>
		/// <param name="generator"> underlying generator for uncorrelated normalized
		/// components. </param>
		/// <exception cref="org.apache.commons.math3.linear.NonPositiveDefiniteMatrixException">
		/// if the covariance matrix is not strictly positive definite. </exception>
		/// <exception cref="DimensionMismatchException"> if the mean and covariance
		/// arrays dimensions do not match. </exception>
		public CorrelatedRandomVectorGenerator(double[] mean, RealMatrix covariance, double small, NormalizedRandomGenerator generator)
		{
			int order = covariance.RowDimension;
			if (mean.Length != order)
			{
				throw new DimensionMismatchException(mean.Length, order);
			}
			this.mean = mean.clone();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RectangularCholeskyDecomposition decomposition = new org.apache.commons.math3.linear.RectangularCholeskyDecomposition(covariance, small);
			RectangularCholeskyDecomposition decomposition = new RectangularCholeskyDecomposition(covariance, small);
			root = decomposition.RootMatrix;

			this.generator = generator;
			normalized = new double[decomposition.Rank];

		}

		/// <summary>
		/// Builds a null mean random correlated vector generator from its
		/// covariance matrix.
		/// </summary>
		/// <param name="covariance"> Covariance matrix. </param>
		/// <param name="small"> Diagonal elements threshold under which  column are
		/// considered to be dependent on previous ones and are discarded. </param>
		/// <param name="generator"> Underlying generator for uncorrelated normalized
		/// components. </param>
		/// <exception cref="org.apache.commons.math3.linear.NonPositiveDefiniteMatrixException">
		/// if the covariance matrix is not strictly positive definite. </exception>
		public CorrelatedRandomVectorGenerator(RealMatrix covariance, double small, NormalizedRandomGenerator generator)
		{
			int order = covariance.RowDimension;
			mean = new double[order];
			for (int i = 0; i < order; ++i)
			{
				mean[i] = 0;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RectangularCholeskyDecomposition decomposition = new org.apache.commons.math3.linear.RectangularCholeskyDecomposition(covariance, small);
			RectangularCholeskyDecomposition decomposition = new RectangularCholeskyDecomposition(covariance, small);
			root = decomposition.RootMatrix;

			this.generator = generator;
			normalized = new double[decomposition.Rank];

		}

		/// <summary>
		/// Get the underlying normalized components generator. </summary>
		/// <returns> underlying uncorrelated components generator </returns>
		public virtual NormalizedRandomGenerator Generator
		{
			get
			{
				return generator;
			}
		}

		/// <summary>
		/// Get the rank of the covariance matrix.
		/// The rank is the number of independent rows in the covariance
		/// matrix, it is also the number of columns of the root matrix. </summary>
		/// <returns> rank of the square matrix. </returns>
		/// <seealso cref= #getRootMatrix() </seealso>
		public virtual int Rank
		{
			get
			{
				return normalized.Length;
			}
		}

		/// <summary>
		/// Get the root of the covariance matrix.
		/// The root is the rectangular matrix <code>B</code> such that
		/// the covariance matrix is equal to <code>B.B<sup>T</sup></code> </summary>
		/// <returns> root of the square matrix </returns>
		/// <seealso cref= #getRank() </seealso>
		public virtual RealMatrix RootMatrix
		{
			get
			{
				return root;
			}
		}

		/// <summary>
		/// Generate a correlated random vector. </summary>
		/// <returns> a random vector as an array of double. The returned array
		/// is created at each call, the caller can do what it wants with it. </returns>
		public virtual double[] nextVector()
		{

			// generate uncorrelated vector
			for (int i = 0; i < normalized.Length; ++i)
			{
				normalized[i] = generator.nextNormalizedDouble();
			}

			// compute correlated vector
			double[] correlated = new double[mean.Length];
			for (int i = 0; i < correlated.Length; ++i)
			{
				correlated[i] = mean[i];
				for (int j = 0; j < root.ColumnDimension; ++j)
				{
					correlated[i] += root.getEntry(i, j) * normalized[j];
				}
			}

			return correlated;

		}

	}

}