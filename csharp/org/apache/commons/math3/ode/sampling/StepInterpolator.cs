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
	/// This interface represents an interpolator over the last step
	/// during an ODE integration.
	/// 
	/// <p>The various ODE integrators provide objects implementing this
	/// interface to the step handlers. These objects are often custom
	/// objects tightly bound to the integrator internal algorithms. The
	/// handlers can use these objects to retrieve the state vector at
	/// intermediate times between the previous and the current grid points
	/// (this feature is often called dense output).</p>
	/// <p>One important thing to note is that the step handlers may be so
	/// tightly bound to the integrators that they often share some internal
	/// state arrays. This imply that one should <em>never</em> use a direct
	/// reference to a step interpolator outside of the step handler, either
	/// for future use or for use in another thread. If such a need arise, the
	/// step interpolator <em>must</em> be copied using the dedicated
	/// <seealso cref="#copy()"/> method.
	/// </p>
	/// </summary>
	/// <seealso cref= org.apache.commons.math3.ode.FirstOrderIntegrator </seealso>
	/// <seealso cref= org.apache.commons.math3.ode.SecondOrderIntegrator </seealso>
	/// <seealso cref= StepHandler
	/// @version $Id: StepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public interface StepInterpolator : Externalizable
	{

	  /// <summary>
	  /// Get the previous grid point time. </summary>
	  /// <returns> previous grid point time </returns>
	  double PreviousTime {get;}

	  /// <summary>
	  /// Get the current grid point time. </summary>
	  /// <returns> current grid point time </returns>
	  double CurrentTime {get;}

	  /// <summary>
	  /// Get the time of the interpolated point.
	  /// If <seealso cref="#setInterpolatedTime"/> has not been called, it returns
	  /// the current grid point time. </summary>
	  /// <returns> interpolation point time </returns>
	  double InterpolatedTime {get;set;}


	  /// <summary>
	  /// Get the state vector of the interpolated point.
	  /// <p>The returned vector is a reference to a reused array, so
	  /// it should not be modified and it should be copied if it needs
	  /// to be preserved across several calls.</p> </summary>
	  /// <returns> state vector at time <seealso cref="#getInterpolatedTime"/> </returns>
	  /// <seealso cref= #getInterpolatedDerivatives() </seealso>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] getInterpolatedState() throws org.apache.commons.math3.exception.MaxCountExceededException;
	  double[] InterpolatedState {get;}

	  /// <summary>
	  /// Get the derivatives of the state vector of the interpolated point.
	  /// <p>The returned vector is a reference to a reused array, so
	  /// it should not be modified and it should be copied if it needs
	  /// to be preserved across several calls.</p> </summary>
	  /// <returns> derivatives of the state vector at time <seealso cref="#getInterpolatedTime"/> </returns>
	  /// <seealso cref= #getInterpolatedState()
	  /// @since 2.0 </seealso>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] getInterpolatedDerivatives() throws org.apache.commons.math3.exception.MaxCountExceededException;
	  double[] InterpolatedDerivatives {get;}

	  /// <summary>
	  /// Get the interpolated secondary state corresponding to the secondary equations.
	  /// <p>The returned vector is a reference to a reused array, so
	  /// it should not be modified and it should be copied if it needs
	  /// to be preserved across several calls.</p> </summary>
	  /// <param name="index"> index of the secondary set, as returned by {@link
	  /// org.apache.commons.math3.ode.ExpandableStatefulODE#addSecondaryEquations(
	  /// org.apache.commons.math3.ode.SecondaryEquations)
	  /// ExpandableStatefulODE.addSecondaryEquations(SecondaryEquations)} </param>
	  /// <returns> interpolated secondary state at the current interpolation date </returns>
	  /// <seealso cref= #getInterpolatedState() </seealso>
	  /// <seealso cref= #getInterpolatedDerivatives() </seealso>
	  /// <seealso cref= #getInterpolatedSecondaryDerivatives(int) </seealso>
	  /// <seealso cref= #setInterpolatedTime(double)
	  /// @since 3.0 </seealso>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] getInterpolatedSecondaryState(int index) throws org.apache.commons.math3.exception.MaxCountExceededException;
	  double[] getInterpolatedSecondaryState(int index);

	  /// <summary>
	  /// Get the interpolated secondary derivatives corresponding to the secondary equations.
	  /// <p>The returned vector is a reference to a reused array, so
	  /// it should not be modified and it should be copied if it needs
	  /// to be preserved across several calls.</p> </summary>
	  /// <param name="index"> index of the secondary set, as returned by {@link
	  /// org.apache.commons.math3.ode.ExpandableStatefulODE#addSecondaryEquations(
	  /// org.apache.commons.math3.ode.SecondaryEquations)
	  /// ExpandableStatefulODE.addSecondaryEquations(SecondaryEquations)} </param>
	  /// <returns> interpolated secondary derivatives at the current interpolation date </returns>
	  /// <seealso cref= #getInterpolatedState() </seealso>
	  /// <seealso cref= #getInterpolatedDerivatives() </seealso>
	  /// <seealso cref= #getInterpolatedSecondaryState(int) </seealso>
	  /// <seealso cref= #setInterpolatedTime(double)
	  /// @since 3.0 </seealso>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: double[] getInterpolatedSecondaryDerivatives(int index) throws org.apache.commons.math3.exception.MaxCountExceededException;
	  double[] getInterpolatedSecondaryDerivatives(int index);

	  /// <summary>
	  /// Check if the natural integration direction is forward.
	  /// <p>This method provides the integration direction as specified by
	  /// the integrator itself, it avoid some nasty problems in
	  /// degenerated cases like null steps due to cancellation at step
	  /// initialization, step control or discrete events
	  /// triggering.</p> </summary>
	  /// <returns> true if the integration variable (time) increases during
	  /// integration </returns>
	  bool Forward {get;}

	  /// <summary>
	  /// Copy the instance.
	  /// <p>The copied instance is guaranteed to be independent from the
	  /// original one. Both can be used with different settings for
	  /// interpolated time without any side effect.</p> </summary>
	  /// <returns> a deep copy of the instance, which can be used independently. </returns>
	  /// <seealso cref= #setInterpolatedTime(double) </seealso>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded
	  /// during step finalization </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: StepInterpolator copy() throws org.apache.commons.math3.exception.MaxCountExceededException;
	   StepInterpolator copy();

	}

}