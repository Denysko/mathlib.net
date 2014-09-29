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

namespace mathlib.ode.sampling
{


	/// <summary>
	/// This interface represents a handler that should be called after
	/// each successful fixed step.
	/// 
	/// <p>This interface should be implemented by anyone who is interested
	/// in getting the solution of an ordinary differential equation at
	/// fixed time steps. Objects implementing this interface should be
	/// wrapped within an instance of <seealso cref="StepNormalizer"/> that itself
	/// is used as the general <seealso cref="StepHandler"/> by the integrator. The
	/// <seealso cref="StepNormalizer"/> object is called according to the integrator
	/// internal algorithms and it calls objects implementing this
	/// interface as necessary at fixed time steps.</p>
	/// </summary>
	/// <seealso cref= StepHandler </seealso>
	/// <seealso cref= StepNormalizer
	/// @version $Id: FixedStepHandler.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public interface FixedStepHandler
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
	  /// <param name="t"> time of the current step </param>
	  /// <param name="y"> state vector at t. For efficiency purposes, the {@link
	  /// StepNormalizer} class reuses the same array on each call, so if
	  /// the instance wants to keep it across all calls (for example to
	  /// provide at the end of the integration a complete array of all
	  /// steps), it should build a local copy store this copy. </param>
	  /// <param name="yDot"> derivatives of the state vector state vector at t.
	  /// For efficiency purposes, the <seealso cref="StepNormalizer"/> class reuses
	  /// the same array on each call, so if
	  /// the instance wants to keep it across all calls (for example to
	  /// provide at the end of the integration a complete array of all
	  /// steps), it should build a local copy store this copy. </param>
	  /// <param name="isLast"> true if the step is the last one </param>
	  void handleStep(double t, double[] y, double[] yDot, bool isLast);

	}

}