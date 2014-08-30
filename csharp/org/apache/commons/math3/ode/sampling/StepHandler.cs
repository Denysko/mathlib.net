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

namespace org.apache.commons.math3.ode.sampling
{

	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;


	/// <summary>
	/// This interface represents a handler that should be called after
	/// each successful step.
	/// 
	/// <p>The ODE integrators compute the evolution of the state vector at
	/// some grid points that depend on their own internal algorithm. Once
	/// they have found a new grid point (possibly after having computed
	/// several evaluation of the derivative at intermediate points), they
	/// provide it to objects implementing this interface. These objects
	/// typically either ignore the intermediate steps and wait for the
	/// last one, store the points in an ephemeris, or forward them to
	/// specialized processing or output methods.</p>
	/// </summary>
	/// <seealso cref= org.apache.commons.math3.ode.FirstOrderIntegrator </seealso>
	/// <seealso cref= org.apache.commons.math3.ode.SecondOrderIntegrator </seealso>
	/// <seealso cref= StepInterpolator
	/// @version $Id: StepHandler.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public interface StepHandler
	{

		/// <summary>
		/// Initialize step handler at the start of an ODE integration.
		/// <p>
		/// This method is called once at the start of the integration. It
		/// may be used by the step handler to initialize some internal data
		/// if needed.
		/// </p> </summary>
		/// <param name="t0"> start value of the independent <i>time</i> variable </param>
		/// <param name="y0"> array containing the start value of the state vector </param>
		/// <param name="t"> target time for the integration </param>
		void init(double t0, double[] y0, double t);

		/// <summary>
		/// Handle the last accepted step </summary>
		/// <param name="interpolator"> interpolator for the last accepted step. For
		/// efficiency purposes, the various integrators reuse the same
		/// object on each call, so if the instance wants to keep it across
		/// all calls (for example to provide at the end of the integration a
		/// continuous model valid throughout the integration range, as the
		/// {@link org.apache.commons.math3.ode.ContinuousOutputModel
		/// ContinuousOutputModel} class does), it should build a local copy
		/// using the clone method of the interpolator and store this copy.
		/// Keeping only a reference to the interpolator and reusing it will
		/// result in unpredictable behavior (potentially crashing the application). </param>
		/// <param name="isLast"> true if the step is the last one </param>
		/// <exception cref="MaxCountExceededException"> if the interpolator throws one because
		/// the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void handleStep(StepInterpolator interpolator, boolean isLast) throws org.apache.commons.math3.exception.MaxCountExceededException;
		void handleStep(StepInterpolator interpolator, bool isLast);

	}

}