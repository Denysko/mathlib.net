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

	/// <summary>
	/// Returns the arithmetic mean of the available vectors.
	/// @since 1.2
	/// @version $Id: VectorialMean.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class VectorialMean
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 8223009086481006892L;

		/// <summary>
		/// Means for each component. </summary>
		private readonly Mean[] means;

		/// <summary>
		/// Constructs a VectorialMean. </summary>
		/// <param name="dimension"> vectors dimension </param>
		public VectorialMean(int dimension)
		{
			means = new Mean[dimension];
			for (int i = 0; i < dimension; ++i)
			{
				means[i] = new Mean();
			}
		}

		/// <summary>
		/// Add a new vector to the sample. </summary>
		/// <param name="v"> vector to add </param>
		/// <exception cref="DimensionMismatchException"> if the vector does not have the right dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void increment(double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual void increment(double[] v)
		{
			if (v.Length != means.Length)
			{
				throw new DimensionMismatchException(v.Length, means.Length);
			}
			for (int i = 0; i < v.Length; ++i)
			{
				means[i].increment(v[i]);
			}
		}

		/// <summary>
		/// Get the mean vector. </summary>
		/// <returns> mean vector </returns>
		public virtual double[] Result
		{
			get
			{
				double[] result = new double[means.Length];
				for (int i = 0; i < result.Length; ++i)
				{
					result[i] = means[i].Result;
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
				return (means.Length == 0) ? 0 : means[0].N;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + Arrays.GetHashCode(means);
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
			if (!(obj is VectorialMean))
			{
				return false;
			}
			VectorialMean other = (VectorialMean) obj;
			if (!Arrays.Equals(means, other.means))
			{
				return false;
			}
			return true;
		}

	}

}