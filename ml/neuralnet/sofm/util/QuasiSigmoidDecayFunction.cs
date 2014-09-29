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

namespace mathlib.ml.neuralnet.sofm.util
{

	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = mathlib.exception.NumberIsTooLargeException;
	using Logistic = mathlib.analysis.function.Logistic;

	/// <summary>
	/// Decay function whose shape is similar to a sigmoid.
	/// <br/>
	/// Class is immutable.
	/// 
	/// @version $Id: QuasiSigmoidDecayFunction.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class QuasiSigmoidDecayFunction
	{
		/// <summary>
		/// Sigmoid. </summary>
		private readonly Logistic sigmoid;
		/// <summary>
		/// See <seealso cref="#value(long)"/>. </summary>
		private readonly double scale;

		/// <summary>
		/// Creates an instance.
		/// The function {@code f} will have the following properties:
		/// <ul>
		///  <li>{@code f(0) = initValue}</li>
		///  <li>{@code numCall} is the inflexion point</li>
		///  <li>{@code slope = f'(numCall)}</li>
		/// </ul>
		/// </summary>
		/// <param name="initValue"> Initial value, i.e. <seealso cref="#value(long) value(0)"/>. </param>
		/// <param name="slope"> Value of the function derivative at {@code numCall}. </param>
		/// <param name="numCall"> Inflexion point. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code initValue <= 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code slope >= 0}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numCall <= 0}. </exception>
		public QuasiSigmoidDecayFunction(double initValue, double slope, long numCall)
		{
			if (initValue <= 0)
			{
				throw new NotStrictlyPositiveException(initValue);
			}
			if (slope >= 0)
			{
				throw new NumberIsTooLargeException(slope, 0, false);
			}
			if (numCall <= 1)
			{
				throw new NotStrictlyPositiveException(numCall);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double k = initValue;
			double k = initValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = numCall;
			double m = numCall;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b = 4 * slope / initValue;
			double b = 4 * slope / initValue;
			const double q = 1;
			const double a = 0;
			const double n = 1;
			sigmoid = new Logistic(k, m, b, q, a, n);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y0 = sigmoid.value(0);
			double y0 = sigmoid.value(0);
			scale = k / y0;
		}

		/// <summary>
		/// Computes the value of the learning factor.
		/// </summary>
		/// <param name="numCall"> Current step of the training task. </param>
		/// <returns> the value of the function at {@code numCall}. </returns>
		public virtual double value(long numCall)
		{
			return scale * sigmoid.value(numCall);
		}
	}

}