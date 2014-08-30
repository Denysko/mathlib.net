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
namespace org.apache.commons.math3.stat.descriptive
{

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using MathArrays = org.apache.commons.math3.util.MathArrays;


	/// <summary>
	/// Base interface implemented by all statistics.
	/// 
	/// @version $Id: UnivariateStatistic.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface UnivariateStatistic : MathArrays.Function
	{
		/// <summary>
		/// Returns the result of evaluating the statistic over the input array.
		/// </summary>
		/// <param name="values"> input array </param>
		/// <returns> the value of the statistic applied to the input array </returns>
		/// <exception cref="MathIllegalArgumentException">  if values is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double evaluate(double[] values) throws org.apache.commons.math3.exception.MathIllegalArgumentException;
		double evaluate(double[] values);

		/// <summary>
		/// Returns the result of evaluating the statistic over the specified entries
		/// in the input array.
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="begin"> the index of the first element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the value of the statistic applied to the included array entries </returns>
		/// <exception cref="MathIllegalArgumentException"> if values is null or the indices are invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double evaluate(double[] values, int begin, int length) throws org.apache.commons.math3.exception.MathIllegalArgumentException;
		double evaluate(double[] values, int begin, int length);

		/// <summary>
		/// Returns a copy of the statistic with the same internal state.
		/// </summary>
		/// <returns> a copy of the statistic </returns>
		UnivariateStatistic copy();
	}

}