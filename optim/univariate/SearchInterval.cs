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
namespace org.apache.commons.math3.optim.univariate
{

	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Search interval and (optional) start value.
	/// <br/>
	/// Immutable class.
	/// 
	/// @version $Id: SearchInterval.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.1
	/// </summary>
	public class SearchInterval : OptimizationData
	{
		/// <summary>
		/// Lower bound. </summary>
		private readonly double lower;
		/// <summary>
		/// Upper bound. </summary>
		private readonly double upper;
		/// <summary>
		/// Start value. </summary>
		private readonly double start;

		/// <param name="lo"> Lower bound. </param>
		/// <param name="hi"> Upper bound. </param>
		/// <param name="init"> Start value. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lo >= hi}. </exception>
		/// <exception cref="OutOfRangeException"> if {@code init < lo} or {@code init > hi}. </exception>
		public SearchInterval(double lo, double hi, double init)
		{
			if (lo >= hi)
			{
				throw new NumberIsTooLargeException(lo, hi, false);
			}
			if (init < lo || init > hi)
			{
				throw new OutOfRangeException(init, lo, hi);
			}

			lower = lo;
			upper = hi;
			start = init;
		}

		/// <param name="lo"> Lower bound. </param>
		/// <param name="hi"> Upper bound. </param>
		/// <exception cref="NumberIsTooLargeException"> if {@code lo >= hi}. </exception>
		public SearchInterval(double lo, double hi) : this(lo, hi, 0.5 * (lo + hi))
		{
		}

		/// <summary>
		/// Gets the lower bound.
		/// </summary>
		/// <returns> the lower bound. </returns>
		public virtual double Min
		{
			get
			{
				return lower;
			}
		}
		/// <summary>
		/// Gets the upper bound.
		/// </summary>
		/// <returns> the upper bound. </returns>
		public virtual double Max
		{
			get
			{
				return upper;
			}
		}
		/// <summary>
		/// Gets the start value.
		/// </summary>
		/// <returns> the start value. </returns>
		public virtual double StartValue
		{
			get
			{
				return start;
			}
		}
	}

}