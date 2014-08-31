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
namespace org.apache.commons.math3.ode
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;

	/// <summary>
	/// Interface to compute exactly Jacobian matrix for some parameter
	///  when computing <seealso cref="JacobianMatrices partial derivatives equations"/>.
	/// 
	/// @version $Id: ParameterJacobianProvider.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	public interface ParameterJacobianProvider : Parameterizable
	{

		/// <summary>
		/// Compute the Jacobian matrix of ODE with respect to one parameter.
		/// <p>If the parameter does not belong to the collection returned by
		/// <seealso cref="#getParametersNames()"/>, the Jacobian will be set to 0,
		/// but no errors will be triggered.</p> </summary>
		/// <param name="t"> current value of the independent <I>time</I> variable </param>
		/// <param name="y"> array containing the current value of the main state vector </param>
		/// <param name="yDot"> array containing the current value of the time derivative
		/// of the main state vector </param>
		/// <param name="paramName"> name of the parameter to consider </param>
		/// <param name="dFdP"> placeholder array where to put the Jacobian matrix of the
		/// ODE with respect to the parameter </param>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
		/// <exception cref="UnknownParameterException"> if the parameter is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void computeParameterJacobian(double t, double[] y, double[] yDot, String paramName, double[] dFdP) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException, UnknownParameterException;
		void computeParameterJacobian(double t, double[] y, double[] yDot, string paramName, double[] dFdP);

	}

}