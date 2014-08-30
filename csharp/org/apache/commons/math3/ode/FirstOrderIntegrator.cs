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
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;

	/// <summary>
	/// This interface represents a first order integrator for
	/// differential equations.
	/// 
	/// <p>The classes which are devoted to solve first order differential
	/// equations should implement this interface. The problems which can
	/// be handled should implement the {@link
	/// FirstOrderDifferentialEquations} interface.</p>
	/// </summary>
	/// <seealso cref= FirstOrderDifferentialEquations </seealso>
	/// <seealso cref= org.apache.commons.math3.ode.sampling.StepHandler </seealso>
	/// <seealso cref= org.apache.commons.math3.ode.events.EventHandler
	/// @version $Id: FirstOrderIntegrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public interface FirstOrderIntegrator : ODEIntegrator
	{

	  /// <summary>
	  /// Integrate the differential equations up to the given time.
	  /// <p>This method solves an Initial Value Problem (IVP).</p>
	  /// <p>Since this method stores some internal state variables made
	  /// available in its public interface during integration ({@link
	  /// #getCurrentSignedStepsize()}), it is <em>not</em> thread-safe.</p> </summary>
	  /// <param name="equations"> differential equations to integrate </param>
	  /// <param name="t0"> initial time </param>
	  /// <param name="y0"> initial value of the state vector at t0 </param>
	  /// <param name="t"> target time for the integration
	  /// (can be set to a value smaller than <code>t0</code> for backward integration) </param>
	  /// <param name="y"> placeholder where to put the state vector at each successful
	  ///  step (and hence at the end of integration), can be the same object as y0 </param>
	  /// <returns> stop time, will be the same as target time if integration reached its
	  /// target, but may be different if some {@link
	  /// org.apache.commons.math3.ode.events.EventHandler} stops it at some point. </returns>
	  /// <exception cref="DimensionMismatchException"> if arrays dimension do not match equations settings </exception>
	  /// <exception cref="NumberIsTooSmallException"> if integration step is too small </exception>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
	  /// <exception cref="NoBracketingException"> if the location of an event cannot be bracketed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double integrate(FirstOrderDifferentialEquations equations, double t0, double[] y0, double t, double[] y) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.NoBracketingException;
	  double integrate(FirstOrderDifferentialEquations equations, double t0, double[] y0, double t, double[] y);

	}

}