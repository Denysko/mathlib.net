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
namespace mathlib.analysis.differentiation
{

	/// <summary>
	/// Class representing the gradient of a multivariate function.
	/// <p>
	/// The vectorial components of the function represent the derivatives
	/// with respect to each function parameters.
	/// </p>
	/// @version $Id: GradientFunction.java 1455194 2013-03-11 15:45:54Z luc $
	/// @since 3.1
	/// </summary>
	public class GradientFunction : MultivariateVectorFunction
	{

		/// <summary>
		/// Underlying real-valued function. </summary>
		private readonly MultivariateDifferentiableFunction f;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="f"> underlying real-valued function </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GradientFunction(final MultivariateDifferentiableFunction f)
		public GradientFunction(MultivariateDifferentiableFunction f)
		{
			this.f = f;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[] value(double[] point)
		{

			// set up parameters
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure[] dsX = new DerivativeStructure[point.length];
			DerivativeStructure[] dsX = new DerivativeStructure[point.Length];
			for (int i = 0; i < point.Length; ++i)
			{
				dsX[i] = new DerivativeStructure(point.Length, 1, i, point[i]);
			}

			// compute the derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DerivativeStructure dsY = f.value(dsX);
			DerivativeStructure dsY = f.value(dsX);

			// extract the gradient
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = new double[point.length];
			double[] y = new double[point.Length];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = new int[point.length];
			int[] orders = new int[point.Length];
			for (int i = 0; i < point.Length; ++i)
			{
				orders[i] = 1;
				y[i] = dsY.getPartialDerivative(orders);
				orders[i] = 0;
			}

			return y;

		}

	}

}