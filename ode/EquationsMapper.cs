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

namespace org.apache.commons.math3.ode
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;

	/// <summary>
	/// Class mapping the part of a complete state or derivative that pertains
	/// to a specific differential equation.
	/// <p>
	/// Instances of this class are guaranteed to be immutable.
	/// </p> </summary>
	/// <seealso cref= SecondaryEquations
	/// @version $Id: EquationsMapper.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </seealso>
	[Serializable]
	public class EquationsMapper
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 20110925L;

		/// <summary>
		/// Index of the first equation element in complete state arrays. </summary>
		private readonly int firstIndex;

		/// <summary>
		/// Dimension of the secondary state parameters. </summary>
		private readonly int dimension;

		/// <summary>
		/// simple constructor. </summary>
		/// <param name="firstIndex"> index of the first equation element in complete state arrays </param>
		/// <param name="dimension"> dimension of the secondary state parameters </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EquationsMapper(final int firstIndex, final int dimension)
		public EquationsMapper(int firstIndex, int dimension)
		{
			this.firstIndex = firstIndex;
			this.dimension = dimension;
		}

		/// <summary>
		/// Get the index of the first equation element in complete state arrays. </summary>
		/// <returns> index of the first equation element in complete state arrays </returns>
		public virtual int FirstIndex
		{
			get
			{
				return firstIndex;
			}
		}

		/// <summary>
		/// Get the dimension of the secondary state parameters. </summary>
		/// <returns> dimension of the secondary state parameters </returns>
		public virtual int Dimension
		{
			get
			{
				return dimension;
			}
		}

		/// <summary>
		/// Extract equation data from a complete state or derivative array. </summary>
		/// <param name="complete"> complete state or derivative array from which
		/// equation data should be retrieved </param>
		/// <param name="equationData"> placeholder where to put equation data </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of the equation data does not
		/// match the mapper dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void extractEquationData(double[] complete, double[] equationData) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual void extractEquationData(double[] complete, double[] equationData)
		{
			if (equationData.Length != dimension)
			{
				throw new DimensionMismatchException(equationData.Length, dimension);
			}
			Array.Copy(complete, firstIndex, equationData, 0, dimension);
		}

		/// <summary>
		/// Insert equation data into a complete state or derivative array. </summary>
		/// <param name="equationData"> equation data to be inserted into the complete array </param>
		/// <param name="complete"> placeholder where to put equation data (only the
		/// part corresponding to the equation will be overwritten) </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of the equation data does not
		/// match the mapper dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void insertEquationData(double[] equationData, double[] complete) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual void insertEquationData(double[] equationData, double[] complete)
		{
			if (equationData.Length != dimension)
			{
				throw new DimensionMismatchException(equationData.Length, dimension);
			}
			Array.Copy(equationData, 0, complete, firstIndex, dimension);
		}

	}

}