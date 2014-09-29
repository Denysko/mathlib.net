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
namespace mathlib.ode
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;

	/// <summary>
	/// Interface expanding {@link FirstOrderDifferentialEquations first order
	///  differential equations} in order to compute exactly the main state jacobian
	///  matrix for <seealso cref="JacobianMatrices partial derivatives equations"/>.
	/// 
	/// @version $Id: MainStateJacobianProvider.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public interface MainStateJacobianProvider : FirstOrderDifferentialEquations
	{

		/// <summary>
		/// Compute the jacobian matrix of ODE with respect to main state. </summary>
		/// <param name="t"> current value of the independent <I>time</I> variable </param>
		/// <param name="y"> array containing the current value of the main state vector </param>
		/// <param name="yDot"> array containing the current value of the time derivative of the main state vector </param>
		/// <param name="dFdY"> placeholder array where to put the jacobian matrix of the ODE w.r.t. the main state vector </param>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void computeMainStateJacobian(double t, double[] y, double[] yDot, double[][] dFdY) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException;
		void computeMainStateJacobian(double t, double[] y, double[] yDot, double[][] dFdY);

	}

}