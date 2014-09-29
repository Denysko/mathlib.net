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
	/// Weighted evaluation for statistics.
	/// 
	/// @since 2.1
	/// @version $Id: WeightedEvaluation.java 1382332 2012-09-08 17:27:47Z psteitz $
	/// </summary>
	public interface WeightedEvaluation
	{

		/// <summary>
		/// Returns the result of evaluating the statistic over the input array,
		/// using the supplied weights.
		/// </summary>
		/// <param name="values"> input array </param>
		/// <param name="weights"> array of weights </param>
		/// <returns> the value of the weighted statistic applied to the input array </returns>
		/// <exception cref="MathIllegalArgumentException"> if either array is null, lengths
		/// do not match, weights contain NaN, negative or infinite values, or
		/// weights does not include at least on positive value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double evaluate(double[] values, double[] weights) throws mathlib.exception.MathIllegalArgumentException;
		double evaluate(double[] values, double[] weights);

		/// <summary>
		/// Returns the result of evaluating the statistic over the specified entries
		/// in the input array, using corresponding entries in the supplied weights array.
		/// </summary>
		/// <param name="values"> the input array </param>
		/// <param name="weights"> array of weights </param>
		/// <param name="begin"> the index of the first element to include </param>
		/// <param name="length"> the number of elements to include </param>
		/// <returns> the value of the weighted statistic applied to the included array entries </returns>
		/// <exception cref="MathIllegalArgumentException"> if either array is null, lengths
		/// do not match, indices are invalid, weights contain NaN, negative or
		/// infinite values, or weights does not include at least on positive value </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double evaluate(double[] values, double[] weights, int begin, int length) throws mathlib.exception.MathIllegalArgumentException;
		double evaluate(double[] values, double[] weights, int begin, int length);

	}

}