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

namespace org.apache.commons.math3.ml.neuralnet.sofm
{

	using ExponentialDecayFunction = org.apache.commons.math3.ml.neuralnet.sofm.util.ExponentialDecayFunction;
	using QuasiSigmoidDecayFunction = org.apache.commons.math3.ml.neuralnet.sofm.util.QuasiSigmoidDecayFunction;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;

	/// <summary>
	/// Factory for creating instances of <seealso cref="LearningFactorFunction"/>.
	/// 
	/// @version $Id: LearningFactorFunctionFactory.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class LearningFactorFunctionFactory
	{
		/// <summary>
		/// Class contains only static methods. </summary>
		private LearningFactorFunctionFactory()
		{
		}

		/// <summary>
		/// Creates an exponential decay <seealso cref="LearningFactorFunction function"/>.
		/// It will compute <code>a e<sup>-x / b</sup></code>,
		/// where {@code x} is the (integer) independent variable and
		/// <ul>
		///  <li><code>a = initValue</code>
		///  <li><code>b = -numCall / ln(valueAtNumCall / initValue)</code>
		/// </ul>
		/// </summary>
		/// <param name="initValue"> Initial value, i.e.
		/// <seealso cref="LearningFactorFunction#value(long) value(0)"/>. </param>
		/// <param name="valueAtNumCall"> Value of the function at {@code numCall}. </param>
		/// <param name="numCall"> Argument for which the function returns
		/// {@code valueAtNumCall}. </param>
		/// <returns> the learning factor function. </returns>
		/// <exception cref="org.apache.commons.math3.exception.OutOfRangeException">
		/// if {@code initValue <= 0} or {@code initValue > 1}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if {@code valueAtNumCall <= 0}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException">
		/// if {@code valueAtNumCall >= initValue}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if {@code numCall <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LearningFactorFunction exponentialDecay(final double initValue, final double valueAtNumCall, final long numCall)
		public static LearningFactorFunction exponentialDecay(double initValue, double valueAtNumCall, long numCall)
		{
			if (initValue <= 0 || initValue > 1)
			{
				throw new OutOfRangeException(initValue, 0, 1);
			}

			return new LearningFactorFunctionAnonymousInnerClassHelper(initValue, valueAtNumCall, numCall);
		}

		private class LearningFactorFunctionAnonymousInnerClassHelper : LearningFactorFunction
		{
			private double initValue;
			private double valueAtNumCall;
			private long numCall;

			public LearningFactorFunctionAnonymousInnerClassHelper(double initValue, double valueAtNumCall, long numCall)
			{
				this.initValue = initValue;
				this.valueAtNumCall = valueAtNumCall;
				this.numCall = numCall;
			}

					/// <summary>
					/// DecayFunction. </summary>
			private ExponentialDecayFunction decay = new ExponentialDecayFunction(initValue, valueAtNumCall, numCall);

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double value(long n)
			{
				return decay.value(n);
			}
		}

		/// <summary>
		/// Creates an sigmoid-like {@code LearningFactorFunction function}.
		/// The function {@code f} will have the following properties:
		/// <ul>
		///  <li>{@code f(0) = initValue}</li>
		///  <li>{@code numCall} is the inflexion point</li>
		///  <li>{@code slope = f'(numCall)}</li>
		/// </ul>
		/// </summary>
		/// <param name="initValue"> Initial value, i.e.
		/// <seealso cref="LearningFactorFunction#value(long) value(0)"/>. </param>
		/// <param name="slope"> Value of the function derivative at {@code numCall}. </param>
		/// <param name="numCall"> Inflexion point. </param>
		/// <returns> the learning factor function. </returns>
		/// <exception cref="org.apache.commons.math3.exception.OutOfRangeException">
		/// if {@code initValue <= 0} or {@code initValue > 1}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException">
		/// if {@code slope >= 0}. </exception>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if {@code numCall <= 0}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static LearningFactorFunction quasiSigmoidDecay(final double initValue, final double slope, final long numCall)
		public static LearningFactorFunction quasiSigmoidDecay(double initValue, double slope, long numCall)
		{
			if (initValue <= 0 || initValue > 1)
			{
				throw new OutOfRangeException(initValue, 0, 1);
			}

			return new LearningFactorFunctionAnonymousInnerClassHelper2(initValue, slope, numCall);
		}

		private class LearningFactorFunctionAnonymousInnerClassHelper2 : LearningFactorFunction
		{
			private double initValue;
			private double slope;
			private long numCall;

			public LearningFactorFunctionAnonymousInnerClassHelper2(double initValue, double slope, long numCall)
			{
				this.initValue = initValue;
				this.slope = slope;
				this.numCall = numCall;
			}

					/// <summary>
					/// DecayFunction. </summary>
			private QuasiSigmoidDecayFunction decay = new QuasiSigmoidDecayFunction(initValue, slope, numCall);

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual double value(long n)
			{
				return decay.value(n);
			}
		}
	}

}