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
	/// This interface represents a first order differential equations set.
	/// 
	/// <p>This interface should be implemented by all real first order
	/// differential equation problems before they can be handled by the
	/// integrators <seealso cref="FirstOrderIntegrator#integrate"/> method.</p>
	/// 
	/// <p>A first order differential equations problem, as seen by an
	/// integrator is the time derivative <code>dY/dt</code> of a state
	/// vector <code>Y</code>, both being one dimensional arrays. From the
	/// integrator point of view, this derivative depends only on the
	/// current time <code>t</code> and on the state vector
	/// <code>Y</code>.</p>
	/// 
	/// <p>For real problems, the derivative depends also on parameters
	/// that do not belong to the state vector (dynamical model constants
	/// for example). These constants are completely outside of the scope
	/// of this interface, the classes that implement it are allowed to
	/// handle them as they want.</p>
	/// </summary>
	/// <seealso cref= FirstOrderIntegrator </seealso>
	/// <seealso cref= FirstOrderConverter </seealso>
	/// <seealso cref= SecondOrderDifferentialEquations
	/// 
	/// @version $Id: FirstOrderDifferentialEquations.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public interface FirstOrderDifferentialEquations
	{

		/// <summary>
		/// Get the dimension of the problem. </summary>
		/// <returns> dimension of the problem </returns>
		int Dimension {get;}

		/// <summary>
		/// Get the current time derivative of the state vector. </summary>
		/// <param name="t"> current value of the independent <I>time</I> variable </param>
		/// <param name="y"> array containing the current value of the state vector </param>
		/// <param name="yDot"> placeholder array where to put the time derivative of the state vector </param>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void computeDerivatives(double t, double[] y, double[] yDot) throws org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.DimensionMismatchException;
		void computeDerivatives(double t, double[] y, double[] yDot);

	}

}