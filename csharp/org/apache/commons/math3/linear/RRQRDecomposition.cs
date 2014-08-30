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

namespace org.apache.commons.math3.linear
{

	using FastMath = org.apache.commons.math3.util.FastMath;


	/// <summary>
	/// Calculates the rank-revealing QR-decomposition of a matrix, with column pivoting.
	/// <p>The rank-revealing QR-decomposition of a matrix A consists of three matrices Q,
	/// R and P such that AP=QR.  Q is orthogonal (Q<sup>T</sup>Q = I), and R is upper triangular.
	/// If A is m&times;n, Q is m&times;m and R is m&times;n and P is n&times;n.</p>
	/// <p>QR decomposition with column pivoting produces a rank-revealing QR
	/// decomposition and the <seealso cref="#getRank(double)"/> method may be used to return the rank of the
	/// input matrix A.</p>
	/// <p>This class compute the decomposition using Householder reflectors.</p>
	/// <p>For efficiency purposes, the decomposition in packed form is transposed.
	/// This allows inner loop to iterate inside rows, which is much more cache-efficient
	/// in Java.</p>
	/// <p>This class is based on the class with similar name from the
	/// <a href="http://math.nist.gov/javanumerics/jama/">JAMA</a> library, with the
	/// following changes:</p>
	/// <ul>
	///   <li>a <seealso cref="#getQT() getQT"/> method has been added,</li>
	///   <li>the {@code solve} and {@code isFullRank} methods have been replaced
	///   by a <seealso cref="#getSolver() getSolver"/> method and the equivalent methods
	///   provided by the returned <seealso cref="DecompositionSolver"/>.</li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://mathworld.wolfram.com/QRDecomposition.html">MathWorld</a> </seealso>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/QR_decomposition">Wikipedia</a>
	/// 
	/// @version $Id: RRQRDecomposition.java 1570994 2014-02-23 11:10:41Z luc $
	/// @since 3.2 </seealso>
	public class RRQRDecomposition : QRDecomposition
	{

		/// <summary>
		/// An array to record the column pivoting for later creation of P. </summary>
		private int[] p;

		/// <summary>
		/// Cached value of P. </summary>
		private RealMatrix cachedP;


		/// <summary>
		/// Calculates the QR-decomposition of the given matrix.
		/// The singularity threshold defaults to zero.
		/// </summary>
		/// <param name="matrix"> The matrix to decompose.
		/// </param>
		/// <seealso cref= #RRQRDecomposition(RealMatrix, double) </seealso>
		public RRQRDecomposition(RealMatrix matrix) : this(matrix, 0d)
		{
		}

	   /// <summary>
	   /// Calculates the QR-decomposition of the given matrix.
	   /// </summary>
	   /// <param name="matrix"> The matrix to decompose. </param>
	   /// <param name="threshold"> Singularity threshold. </param>
	   /// <seealso cref= #RRQRDecomposition(RealMatrix) </seealso>
		public RRQRDecomposition(RealMatrix matrix, double threshold) : base(matrix, threshold)
		{
		}

		/// <summary>
		/// Decompose matrix. </summary>
		/// <param name="qrt"> transposed matrix </param>
		protected internal override void decompose(double[][] qrt)
		{
			p = new int[qrt.Length];
			for (int i = 0; i < p.Length; i++)
			{
				p[i] = i;
			}
			base.decompose(qrt);
		}

		/// <summary>
		/// Perform Householder reflection for a minor A(minor, minor) of A. </summary>
		/// <param name="minor"> minor index </param>
		/// <param name="qrt"> transposed matrix </param>
		protected internal override void performHouseholderReflection(int minor, double[][] qrt)
		{

			double l2NormSquaredMax = 0;
			// Find the unreduced column with the greatest L2-Norm
			int l2NormSquaredMaxIndex = minor;
			for (int i = minor; i < qrt.Length; i++)
			{
				double l2NormSquared = 0;
				for (int j = 0; j < qrt[i].Length; j++)
				{
					l2NormSquared += qrt[i][j] * qrt[i][j];
				}
				if (l2NormSquared > l2NormSquaredMax)
				{
					l2NormSquaredMax = l2NormSquared;
					l2NormSquaredMaxIndex = i;
				}
			}
			// swap the current column with that with the greated L2-Norm and record in p
			if (l2NormSquaredMaxIndex != minor)
			{
				double[] tmp1 = qrt[minor];
				qrt[minor] = qrt[l2NormSquaredMaxIndex];
				qrt[l2NormSquaredMaxIndex] = tmp1;
				int tmp2 = p[minor];
				p[minor] = p[l2NormSquaredMaxIndex];
				p[l2NormSquaredMaxIndex] = tmp2;
			}

			base.performHouseholderReflection(minor, qrt);

		}


		/// <summary>
		/// Returns the pivot matrix, P, used in the QR Decomposition of matrix A such that AP = QR.
		/// 
		/// If no pivoting is used in this decomposition then P is equal to the identity matrix.
		/// </summary>
		/// <returns> a permutation matrix. </returns>
		public virtual RealMatrix P
		{
			get
			{
				if (cachedP == null)
				{
					int n = p.Length;
					cachedP = MatrixUtils.createRealMatrix(n,n);
					for (int i = 0; i < n; i++)
					{
						cachedP.setEntry(p[i], i, 1);
					}
				}
				return cachedP;
			}
		}

		/// <summary>
		/// Return the effective numerical matrix rank.
		/// <p>The effective numerical rank is the number of non-negligible
		/// singular values.</p>
		/// <p>This implementation looks at Frobenius norms of the sequence of
		/// bottom right submatrices.  When a large fall in norm is seen,
		/// the rank is returned. The drop is computed as:</p>
		/// <pre>
		///   (thisNorm/lastNorm) * rNorm < dropThreshold
		/// </pre>
		/// <p>
		/// where thisNorm is the Frobenius norm of the current submatrix,
		/// lastNorm is the Frobenius norm of the previous submatrix,
		/// rNorm is is the Frobenius norm of the complete matrix
		/// </p>
		/// </summary>
		/// <param name="dropThreshold"> threshold triggering rank computation </param>
		/// <returns> effective numerical matrix rank </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int getRank(final double dropThreshold)
		public virtual int getRank(double dropThreshold)
		{
			RealMatrix r = R;
			int rows = r.RowDimension;
			int columns = r.ColumnDimension;
			int rank = 1;
			double lastNorm = r.FrobeniusNorm;
			double rNorm = lastNorm;
			while (rank < FastMath.min(rows, columns))
			{
				double thisNorm = r.getSubMatrix(rank, rows - 1, rank, columns - 1).FrobeniusNorm;
				if (thisNorm == 0 || (thisNorm / lastNorm) * rNorm < dropThreshold)
				{
					break;
				}
				lastNorm = thisNorm;
				rank++;
			}
			return rank;
		}

		/// <summary>
		/// Get a solver for finding the A &times; X = B solution in least square sense.
		/// <p>
		/// Least Square sense means a solver can be computed for an overdetermined system,
		/// (i.e. a system with more equations than unknowns, which corresponds to a tall A
		/// matrix with more rows than columns). In any case, if the matrix is singular
		/// within the tolerance set at {@link RRQRDecomposition#RRQRDecomposition(RealMatrix,
		/// double) construction}, an error will be triggered when
		/// the <seealso cref="DecompositionSolver#solve(RealVector) solve"/> method will be called.
		/// </p> </summary>
		/// <returns> a solver </returns>
		public override DecompositionSolver Solver
		{
			get
			{
				return new Solver(base.Solver, this.P);
			}
		}

		/// <summary>
		/// Specialized solver. </summary>
		private class Solver : DecompositionSolver
		{

			/// <summary>
			/// Upper level solver. </summary>
			internal readonly DecompositionSolver upper;

			/// <summary>
			/// A permutation matrix for the pivots used in the QR decomposition </summary>
			internal RealMatrix p;

			/// <summary>
			/// Build a solver from decomposed matrix.
			/// </summary>
			/// <param name="upper"> upper level solver. </param>
			/// <param name="p"> permutation matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private Solver(final DecompositionSolver upper, final RealMatrix p)
			internal Solver(DecompositionSolver upper, RealMatrix p)
			{
				this.upper = upper;
				this.p = p;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual bool NonSingular
			{
				get
				{
					return upper.NonSingular;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealVector solve(RealVector b)
			{
				return p.operate(upper.solve(b));
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual RealMatrix solve(RealMatrix b)
			{
				return p.multiply(upper.solve(b));
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			/// <exception cref="SingularMatrixException"> if the decomposed matrix is singular. </exception>
			public virtual RealMatrix Inverse
			{
				get
				{
					return solve(MatrixUtils.createRealIdentityMatrix(p.RowDimension));
				}
			}
		}
	}

}