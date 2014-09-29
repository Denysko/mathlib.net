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
namespace mathlib.stat.descriptive
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;

	/// <summary>
	/// Extends the definition of <seealso cref="UnivariateStatistic"/> with
	/// <seealso cref="#increment"/> and <seealso cref="#incrementAll(double[])"/> methods for adding
	/// values and updating internal state.
	/// <p>
	/// This interface is designed to be used for calculating statistics that can be
	/// computed in one pass through the data without storing the full array of
	/// sample values.</p>
	/// 
	/// @version $Id: StorelessUnivariateStatistic.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface StorelessUnivariateStatistic : UnivariateStatistic
	{

		/// <summary>
		/// Updates the internal state of the statistic to reflect the addition of the new value. </summary>
		/// <param name="d">  the new value. </param>
		void increment(double d);

		/// <summary>
		/// Updates the internal state of the statistic to reflect addition of
		/// all values in the values array.  Does not clear the statistic first --
		/// i.e., the values are added <strong>incrementally</strong> to the dataset.
		/// </summary>
		/// <param name="values">  array holding the new values to add </param>
		/// <exception cref="MathIllegalArgumentException"> if the array is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void incrementAll(double[] values) throws mathlib.exception.MathIllegalArgumentException;
		void incrementAll(double[] values);

		/// <summary>
		/// Updates the internal state of the statistic to reflect addition of
		/// the values in the designated portion of the values array.  Does not
		/// clear the statistic first -- i.e., the values are added
		/// <strong>incrementally</strong> to the dataset.
		/// </summary>
		/// <param name="values">  array holding the new values to add </param>
		/// <param name="start">  the array index of the first value to add </param>
		/// <param name="length">  the number of elements to add </param>
		/// <exception cref="MathIllegalArgumentException"> if the array is null or the index </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void incrementAll(double[] values, int start, int length) throws mathlib.exception.MathIllegalArgumentException;
		void incrementAll(double[] values, int start, int length);

		/// <summary>
		/// Returns the current value of the Statistic. </summary>
		/// <returns> value of the statistic, <code>Double.NaN</code> if it
		/// has been cleared or just instantiated. </returns>
		double Result {get;}

		/// <summary>
		/// Returns the number of values that have been added. </summary>
		/// <returns> the number of values. </returns>
		long N {get;}

		/// <summary>
		/// Clears the internal state of the Statistic
		/// </summary>
		void clear();

		/// <summary>
		/// Returns a copy of the statistic with the same internal state.
		/// </summary>
		/// <returns> a copy of the statistic </returns>
		StorelessUnivariateStatistic copy();

	}

}