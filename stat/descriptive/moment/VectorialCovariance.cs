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
namespace org.apache.commons.math3.stat.descriptive.moment
{


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;

	/// <summary>
	/// Returns the covariance matrix of the available vectors.
	/// @since 1.2
	/// @version $Id: VectorialCovariance.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class VectorialCovariance
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 4118372414238930270L;

		/// <summary>
		/// Sums for each component. </summary>
		private readonly double[] sums;

		/// <summary>
		/// Sums of products for each component. </summary>
		private readonly double[] productsSums;

		/// <summary>
		/// Indicator for bias correction. </summary>
		private readonly bool isBiasCorrected;

		/// <summary>
		/// Number of vectors in the sample. </summary>
		private long n;

		/// <summary>
		/// Constructs a VectorialCovariance. </summary>
		/// <param name="dimension"> vectors dimension </param>
		/// <param name="isBiasCorrected"> if true, computed the unbiased sample covariance,
		/// otherwise computes the biased population covariance </param>
		public VectorialCovariance(int dimension, bool isBiasCorrected)
		{
			sums = new double[dimension];
			productsSums = new double[dimension * (dimension + 1) / 2];
			n = 0;
			this.isBiasCorrected = isBiasCorrected;
		}

		/// <summary>
		/// Add a new vector to the sample. </summary>
		/// <param name="v"> vector to add </param>
		/// <exception cref="DimensionMismatchException"> if the vector does not have the right dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void increment(double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual void increment(double[] v)
		{
			if (v.Length != sums.Length)
			{
				throw new DimensionMismatchException(v.Length, sums.Length);
			}
			int k = 0;
			for (int i = 0; i < v.Length; ++i)
			{
				sums[i] += v[i];
				for (int j = 0; j <= i; ++j)
				{
					productsSums[k++] += v[i] * v[j];
				}
			}
			n++;
		}

		/// <summary>
		/// Get the covariance matrix. </summary>
		/// <returns> covariance matrix </returns>
		public virtual RealMatrix Result
		{
			get
			{
    
				int dimension = sums.Length;
				RealMatrix result = MatrixUtils.createRealMatrix(dimension, dimension);
    
				if (n > 1)
				{
					double c = 1.0 / (n * (isBiasCorrected ? (n - 1) : n));
					int k = 0;
					for (int i = 0; i < dimension; ++i)
					{
						for (int j = 0; j <= i; ++j)
						{
							double e = c * (n * productsSums[k++] - sums[i] * sums[j]);
							result.setEntry(i, j, e);
							result.setEntry(j, i, e);
						}
					}
				}
    
				return result;
    
			}
		}

		/// <summary>
		/// Get the number of vectors in the sample. </summary>
		/// <returns> number of vectors in the sample </returns>
		public virtual long N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Clears the internal state of the Statistic
		/// </summary>
		public virtual void clear()
		{
			n = 0;
			Arrays.fill(sums, 0.0);
			Arrays.fill(productsSums, 0.0);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + (isBiasCorrected ? 1231 : 1237);
			result = prime * result + (int)(n ^ ((long)((ulong)n >> 32)));
			result = prime * result + Arrays.GetHashCode(productsSums);
			result = prime * result + Arrays.GetHashCode(sums);
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is VectorialCovariance))
			{
				return false;
			}
			VectorialCovariance other = (VectorialCovariance) obj;
			if (isBiasCorrected != other.isBiasCorrected)
			{
				return false;
			}
			if (n != other.n)
			{
				return false;
			}
			if (!Arrays.Equals(productsSums, other.productsSums))
			{
				return false;
			}
			if (!Arrays.Equals(sums, other.sums))
			{
				return false;
			}
			return true;
		}

	}

}