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
namespace org.apache.commons.math3.analysis.differentiation
{

	/// <summary>
	/// Class representing the Jacobian of a multivariate vector function.
	/// <p>
	/// The rows iterate on the model functions while the columns iterate on the parameters; thus,
	/// the numbers of rows is equal to the dimension of the underlying function vector
	/// value and the number of columns is equal to the number of free parameters of
	/// the underlying function.
	/// </p>
	/// @version $Id: JacobianFunction.java 1455194 2013-03-11 15:45:54Z luc $
	/// @since 3.1
	/// </summary>
	public class JacobianFunction : MultivariateMatrixFunction
	{

		/// <summary>
		/// Underlying vector-valued function. </summary>
		private readonly MultivariateDifferentiableVectorFunction f;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="f"> underlying vector-valued function </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public JacobianFunction(final MultivariateDifferentiableVectorFunction f)
		public JacobianFunction(MultivariateDifferentiableVectorFunction f)
		{
			this.f = f;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[][] value(double[] point)
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
//ORIGINAL LINE: final DerivativeStructure[] dsY = f.value(dsX);
			DerivativeStructure[] dsY = f.value(dsX);

			// extract the Jacobian
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] y = new double[dsY.length][point.length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] y = new double[dsY.Length][point.Length];
			double[][] y = RectangularArrays.ReturnRectangularDoubleArray(dsY.Length, point.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = new int[point.length];
			int[] orders = new int[point.Length];
			for (int i = 0; i < dsY.Length; ++i)
			{
				for (int j = 0; j < point.Length; ++j)
				{
					orders[j] = 1;
					y[i][j] = dsY[i].getPartialDerivative(orders);
					orders[j] = 0;
				}
			}

			return y;

		}

	}

}