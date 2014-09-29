using System.Collections.Generic;

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

	using UnivariateSolver = mathlib.analysis.solvers.UnivariateSolver;
	using EventHandler = mathlib.ode.events.EventHandler;
	using StepHandler = mathlib.ode.sampling.StepHandler;

	/// <summary>
	/// This interface defines the common parts shared by integrators
	/// for first and second order differential equations. </summary>
	/// <seealso cref= FirstOrderIntegrator </seealso>
	/// <seealso cref= SecondOrderIntegrator
	/// @version $Id: ODEIntegrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0 </seealso>
	public interface ODEIntegrator
	{

		/// <summary>
		/// Get the name of the method. </summary>
		/// <returns> name of the method </returns>
		string Name {get;}

		/// <summary>
		/// Add a step handler to this integrator.
		/// <p>The handler will be called by the integrator for each accepted
		/// step.</p> </summary>
		/// <param name="handler"> handler for the accepted steps </param>
		/// <seealso cref= #getStepHandlers() </seealso>
		/// <seealso cref= #clearStepHandlers()
		/// @since 2.0 </seealso>
		void addStepHandler(StepHandler handler);

		/// <summary>
		/// Get all the step handlers that have been added to the integrator. </summary>
		/// <returns> an unmodifiable collection of the added events handlers </returns>
		/// <seealso cref= #addStepHandler(StepHandler) </seealso>
		/// <seealso cref= #clearStepHandlers()
		/// @since 2.0 </seealso>
		ICollection<StepHandler> StepHandlers {get;}

		/// <summary>
		/// Remove all the step handlers that have been added to the integrator. </summary>
		/// <seealso cref= #addStepHandler(StepHandler) </seealso>
		/// <seealso cref= #getStepHandlers()
		/// @since 2.0 </seealso>
		void clearStepHandlers();

		/// <summary>
		/// Add an event handler to the integrator.
		/// Uses a default <seealso cref="UnivariateSolver"/>
		/// with an absolute accuracy equal to the given convergence threshold,
		/// as root-finding algorithm to detect the state events. </summary>
		/// <param name="handler"> event handler </param>
		/// <param name="maxCheckInterval"> maximal time interval between switching
		/// function checks (this interval prevents missing sign changes in
		/// case the integration steps becomes very large) </param>
		/// <param name="convergence"> convergence threshold in the event time search </param>
		/// <param name="maxIterationCount"> upper limit of the iteration count in
		/// the event time search </param>
		/// <seealso cref= #getEventHandlers() </seealso>
		/// <seealso cref= #clearEventHandlers() </seealso>
		void addEventHandler(EventHandler handler, double maxCheckInterval, double convergence, int maxIterationCount);

		/// <summary>
		/// Add an event handler to the integrator. </summary>
		/// <param name="handler"> event handler </param>
		/// <param name="maxCheckInterval"> maximal time interval between switching
		/// function checks (this interval prevents missing sign changes in
		/// case the integration steps becomes very large) </param>
		/// <param name="convergence"> convergence threshold in the event time search </param>
		/// <param name="maxIterationCount"> upper limit of the iteration count in
		/// the event time search </param>
		/// <param name="solver"> The root-finding algorithm to use to detect the state
		/// events. </param>
		/// <seealso cref= #getEventHandlers() </seealso>
		/// <seealso cref= #clearEventHandlers() </seealso>
		void addEventHandler(EventHandler handler, double maxCheckInterval, double convergence, int maxIterationCount, UnivariateSolver solver);

		/// <summary>
		/// Get all the event handlers that have been added to the integrator. </summary>
		/// <returns> an unmodifiable collection of the added events handlers </returns>
		/// <seealso cref= #addEventHandler(EventHandler, double, double, int) </seealso>
		/// <seealso cref= #clearEventHandlers() </seealso>
		ICollection<EventHandler> EventHandlers {get;}

		/// <summary>
		/// Remove all the event handlers that have been added to the integrator. </summary>
		/// <seealso cref= #addEventHandler(EventHandler, double, double, int) </seealso>
		/// <seealso cref= #getEventHandlers() </seealso>
		void clearEventHandlers();

		/// <summary>
		/// Get the current value of the step start time t<sub>i</sub>.
		/// <p>This method can be called during integration (typically by
		/// the object implementing the {@link FirstOrderDifferentialEquations
		/// differential equations} problem) if the value of the current step that
		/// is attempted is needed.</p>
		/// <p>The result is undefined if the method is called outside of
		/// calls to <code>integrate</code>.</p> </summary>
		/// <returns> current value of the step start time t<sub>i</sub> </returns>
		double CurrentStepStart {get;}

		/// <summary>
		/// Get the current signed value of the integration stepsize.
		/// <p>This method can be called during integration (typically by
		/// the object implementing the {@link FirstOrderDifferentialEquations
		/// differential equations} problem) if the signed value of the current stepsize
		/// that is tried is needed.</p>
		/// <p>The result is undefined if the method is called outside of
		/// calls to <code>integrate</code>.</p> </summary>
		/// <returns> current signed value of the stepsize </returns>
		double CurrentSignedStepsize {get;}

		/// <summary>
		/// Set the maximal number of differential equations function evaluations.
		/// <p>The purpose of this method is to avoid infinite loops which can occur
		/// for example when stringent error constraints are set or when lots of
		/// discrete events are triggered, thus leading to many rejected steps.</p> </summary>
		/// <param name="maxEvaluations"> maximal number of function evaluations (negative
		/// values are silently converted to maximal integer value, thus representing
		/// almost unlimited evaluations) </param>
		int MaxEvaluations {set;get;}


		/// <summary>
		/// Get the number of evaluations of the differential equations function.
		/// <p>
		/// The number of evaluations corresponds to the last call to the
		/// <code>integrate</code> method. It is 0 if the method has not been called yet.
		/// </p> </summary>
		/// <returns> number of evaluations of the differential equations function </returns>
		int Evaluations {get;}

	}

}