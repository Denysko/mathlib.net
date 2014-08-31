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

namespace org.apache.commons.math3.ml.neuralnet.sofm.util
{

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Exponential decay function: <code>a e<sup>-x / b</sup></code>,
	/// where {@code x} is the (integer) independent variable.
	/// <br/>
	/// Class is immutable.
	/// 
	/// @version $Id: ExponentialDecayFunction.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class ExponentialDecayFunction
	{
		/// <summary>
		/// Factor {@code a}. </summary>
		private readonly double a;
		/// <summary>
		/// Factor {@code 1 / b}. </summary>
		private readonly double oneOverB;

		/// <summary>
		/// Creates an instance. It will be such that
		/// <ul>
		///  <li>{@code a = initValue}</li>
		///  <li>{@code b = -numCall / ln(valueAtNumCall / initValue)}</li>
		/// </ul>
		/// </summary>
		/// <param name="initValue"> Initial value, i.e. <seealso cref="#value(long) value(0)"/>. </param>
		/// <param name="valueAtNumCall"> Value of the function at {@code numCall}. </param>
		/// <param name="numCall"> Argument for which the function returns
		/// {@code valueAtNumCall}. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code initValue <= 0}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code valueAtNumCall <= 0}. </exception>
		/// <exception cref="NumberIsTooLargeException"> if {@code valueAtNumCall >= initValue}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code numCall <= 0}. </exception>
		public ExponentialDecayFunction(double initValue, double valueAtNumCall, long numCall)
		{
			if (initValue <= 0)
			{
				throw new NotStrictlyPositiveException(initValue);
			}
			if (valueAtNumCall <= 0)
			{
				throw new NotStrictlyPositiveException(valueAtNumCall);
			}
			if (valueAtNumCall >= initValue)
			{
				throw new NumberIsTooLargeException(valueAtNumCall, initValue, false);
			}
			if (numCall <= 0)
			{
				throw new NotStrictlyPositiveException(numCall);
			}

			a = initValue;
			oneOverB = -FastMath.log(valueAtNumCall / initValue) / numCall;
		}

		/// <summary>
		/// Computes <code>a e<sup>-numCall / b</sup></code>.
		/// </summary>
		/// <param name="numCall"> Current step of the training task. </param>
		/// <returns> the value of the function at {@code numCall}. </returns>
		public virtual double value(long numCall)
		{
			return a * FastMath.exp(-numCall * oneOverB);
		}
	}

}