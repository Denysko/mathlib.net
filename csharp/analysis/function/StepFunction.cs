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

namespace org.apache.commons.math3.analysis.function
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// <a href="http://en.wikipedia.org/wiki/Step_function">
	///  Step function</a>.
	/// 
	/// @since 3.0
	/// @version $Id: StepFunction.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class StepFunction : UnivariateFunction
	{
		/// <summary>
		/// Abscissae. </summary>
		private readonly double[] abscissa;
		/// <summary>
		/// Ordinates. </summary>
		private readonly double[] ordinate;

		/// <summary>
		/// Builds a step function from a list of arguments and the corresponding
		/// values. Specifically, returns the function h(x) defined by <pre><code>
		/// h(x) = y[0] for all x < x[1]
		///        y[1] for x[1] <= x < x[2]
		///        ...
		///        y[y.length - 1] for x >= x[x.length - 1]
		/// </code></pre>
		/// The value of {@code x[0]} is ignored, but it must be strictly less than
		/// {@code x[1]}.
		/// </summary>
		/// <param name="x"> Domain values where the function changes value. </param>
		/// <param name="y"> Values of the function. </param>
		/// <exception cref="NonMonotonicSequenceException">
		/// if the {@code x} array is not sorted in strictly increasing order. </exception>
		/// <exception cref="NullArgumentException"> if {@code x} or {@code y} are {@code null}. </exception>
		/// <exception cref="NoDataException"> if {@code x} or {@code y} are zero-length. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code x} and {@code y} do not
		/// have the same length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StepFunction(double[] x, double[] y) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NonMonotonicSequenceException
		public StepFunction(double[] x, double[] y)
		{
			if (x == null || y == null)
			{
				throw new NullArgumentException();
			}
			if (x.Length == 0 || y.Length == 0)
			{
				throw new NoDataException();
			}
			if (y.Length != x.Length)
			{
				throw new DimensionMismatchException(y.Length, x.Length);
			}
			MathArrays.checkOrder(x);

			abscissa = MathArrays.copyOf(x);
			ordinate = MathArrays.copyOf(y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double value(double x)
		{
			int index = Arrays.binarySearch(abscissa, x);
			double fx = 0;

			if (index < -1)
			{
				// "x" is between "abscissa[-index-2]" and "abscissa[-index-1]".
				fx = ordinate[-index - 2];
			}
			else if (index >= 0)
			{
				// "x" is exactly "abscissa[index]".
				fx = ordinate[index];
			}
			else
			{
				// Otherwise, "x" is smaller than the first value in "abscissa"
				// (hence the returned value should be "ordinate[0]").
				fx = ordinate[0];
			}

			return fx;
		}
	}

}