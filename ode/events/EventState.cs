using System;

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

namespace mathlib.ode.events
{

	using UnivariateFunction = mathlib.analysis.UnivariateFunction;
	using AllowedSolution = mathlib.analysis.solvers.AllowedSolution;
	using mathlib.analysis.solvers;
	using PegasusSolver = mathlib.analysis.solvers.PegasusSolver;
	using UnivariateSolver = mathlib.analysis.solvers.UnivariateSolver;
	using UnivariateSolverUtils = mathlib.analysis.solvers.UnivariateSolverUtils;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoBracketingException = mathlib.exception.NoBracketingException;
	using StepInterpolator = mathlib.ode.sampling.StepInterpolator;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class handles the state for one {@link EventHandler
	/// event handler} during integration steps.
	/// 
	/// <p>Each time the integrator proposes a step, the event handler
	/// switching function should be checked. This class handles the state
	/// of one handler during one integration step, with references to the
	/// state at the end of the preceding step. This information is used to
	/// decide if the handler should trigger an event or not during the
	/// proposed step.</p>
	/// 
	/// @version $Id: EventState.java 1558462 2014-01-15 16:48:25Z luc $
	/// @since 1.2
	/// </summary>
	public class EventState
	{

		/// <summary>
		/// Event handler. </summary>
		private readonly EventHandler handler;

		/// <summary>
		/// Maximal time interval between events handler checks. </summary>
		private readonly double maxCheckInterval;

		/// <summary>
		/// Convergence threshold for event localization. </summary>
		private readonly double convergence;

		/// <summary>
		/// Upper limit in the iteration count for event localization. </summary>
		private readonly int maxIterationCount;

		/// <summary>
		/// Equation being integrated. </summary>
		private ExpandableStatefulODE expandable;

		/// <summary>
		/// Time at the beginning of the step. </summary>
		private double t0;

		/// <summary>
		/// Value of the events handler at the beginning of the step. </summary>
		private double g0;

		/// <summary>
		/// Simulated sign of g0 (we cheat when crossing events). </summary>
		private bool g0Positive;

		/// <summary>
		/// Indicator of event expected during the step. </summary>
		private bool pendingEvent;

		/// <summary>
		/// Occurrence time of the pending event. </summary>
		private double pendingEventTime;

		/// <summary>
		/// Occurrence time of the previous event. </summary>
		private double previousEventTime;

		/// <summary>
		/// Integration direction. </summary>
		private bool forward;

		/// <summary>
		/// Variation direction around pending event.
		///  (this is considered with respect to the integration direction)
		/// </summary>
		private bool increasing;

		/// <summary>
		/// Next action indicator. </summary>
		private EventHandler_Action nextAction;

		/// <summary>
		/// Root-finding algorithm to use to detect state events. </summary>
		private readonly UnivariateSolver solver;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="handler"> event handler </param>
		/// <param name="maxCheckInterval"> maximal time interval between switching
		/// function checks (this interval prevents missing sign changes in
		/// case the integration steps becomes very large) </param>
		/// <param name="convergence"> convergence threshold in the event time search </param>
		/// <param name="maxIterationCount"> upper limit of the iteration count in
		/// the event time search </param>
		/// <param name="solver"> Root-finding algorithm to use to detect state events </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EventState(final EventHandler handler, final double maxCheckInterval, final double convergence, final int maxIterationCount, final mathlib.analysis.solvers.UnivariateSolver solver)
		public EventState(EventHandler handler, double maxCheckInterval, double convergence, int maxIterationCount, UnivariateSolver solver)
		{
			this.handler = handler;
			this.maxCheckInterval = maxCheckInterval;
			this.convergence = FastMath.abs(convergence);
			this.maxIterationCount = maxIterationCount;
			this.solver = solver;

			// some dummy values ...
			expandable = null;
			t0 = double.NaN;
			g0 = double.NaN;
			g0Positive = true;
			pendingEvent = false;
			pendingEventTime = double.NaN;
			previousEventTime = double.NaN;
			increasing = true;
			nextAction = EventHandler_Action.CONTINUE;

		}

		/// <summary>
		/// Get the underlying event handler. </summary>
		/// <returns> underlying event handler </returns>
		public virtual EventHandler EventHandler
		{
			get
			{
				return handler;
			}
		}

		/// <summary>
		/// Set the equation. </summary>
		/// <param name="expandable"> equation being integrated </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setExpandable(final mathlib.ode.ExpandableStatefulODE expandable)
		public virtual ExpandableStatefulODE Expandable
		{
			set
			{
				this.expandable = value;
			}
		}

		/// <summary>
		/// Get the maximal time interval between events handler checks. </summary>
		/// <returns> maximal time interval between events handler checks </returns>
		public virtual double MaxCheckInterval
		{
			get
			{
				return maxCheckInterval;
			}
		}

		/// <summary>
		/// Get the convergence threshold for event localization. </summary>
		/// <returns> convergence threshold for event localization </returns>
		public virtual double Convergence
		{
			get
			{
				return convergence;
			}
		}

		/// <summary>
		/// Get the upper limit in the iteration count for event localization. </summary>
		/// <returns> upper limit in the iteration count for event localization </returns>
		public virtual int MaxIterationCount
		{
			get
			{
				return maxIterationCount;
			}
		}

		/// <summary>
		/// Reinitialize the beginning of the step. </summary>
		/// <param name="interpolator"> valid for the current step </param>
		/// <exception cref="MaxCountExceededException"> if the interpolator throws one because
		/// the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void reinitializeBegin(final mathlib.ode.sampling.StepInterpolator interpolator) throws mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void reinitializeBegin(StepInterpolator interpolator)
		{

			t0 = interpolator.PreviousTime;
			interpolator.InterpolatedTime = t0;
			g0 = handler.g(t0, getCompleteState(interpolator));
			if (g0 == 0)
			{
				// excerpt from MATH-421 issue:
				// If an ODE solver is setup with an EventHandler that return STOP
				// when the even is triggered, the integrator stops (which is exactly
				// the expected behavior). If however the user wants to restart the
				// solver from the final state reached at the event with the same
				// configuration (expecting the event to be triggered again at a
				// later time), then the integrator may fail to start. It can get stuck
				// at the previous event. The use case for the bug MATH-421 is fairly
				// general, so events occurring exactly at start in the first step should
				// be ignored.

				// extremely rare case: there is a zero EXACTLY at interval start
				// we will use the sign slightly after step beginning to force ignoring this zero
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double epsilon = mathlib.util.FastMath.max(solver.getAbsoluteAccuracy(), mathlib.util.FastMath.abs(solver.getRelativeAccuracy() * t0));
				double epsilon = FastMath.max(solver.AbsoluteAccuracy, FastMath.abs(solver.RelativeAccuracy * t0));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tStart = t0 + 0.5 * epsilon;
				double tStart = t0 + 0.5 * epsilon;
				interpolator.InterpolatedTime = tStart;
				g0 = handler.g(tStart, getCompleteState(interpolator));
			}
			g0Positive = g0 >= 0;

		}

		/// <summary>
		/// Get the complete state (primary and secondary). </summary>
		/// <param name="interpolator"> interpolator to use </param>
		/// <returns> complete state </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private double[] getCompleteState(final mathlib.ode.sampling.StepInterpolator interpolator)
		private double[] getCompleteState(StepInterpolator interpolator)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] complete = new double[expandable.getTotalDimension()];
			double[] complete = new double[expandable.TotalDimension];

			expandable.PrimaryMapper.insertEquationData(interpolator.InterpolatedState, complete);
			int index = 0;
			foreach (EquationsMapper secondary in expandable.SecondaryMappers)
			{
				secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index++), complete);
			}

			return complete;

		}

		/// <summary>
		/// Evaluate the impact of the proposed step on the event handler. </summary>
		/// <param name="interpolator"> step interpolator for the proposed step </param>
		/// <returns> true if the event handler triggers an event before
		/// the end of the proposed step </returns>
		/// <exception cref="MaxCountExceededException"> if the interpolator throws one because
		/// the number of functions evaluations is exceeded </exception>
		/// <exception cref="NoBracketingException"> if the event cannot be bracketed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean evaluateStep(final mathlib.ode.sampling.StepInterpolator interpolator) throws mathlib.exception.MaxCountExceededException, mathlib.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool evaluateStep(StepInterpolator interpolator)
		{

			try
			{
				forward = interpolator.Forward;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t1 = interpolator.getCurrentTime();
				double t1 = interpolator.CurrentTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dt = t1 - t0;
				double dt = t1 - t0;
				if (FastMath.abs(dt) < convergence)
				{
					// we cannot do anything on such a small step, don't trigger any events
					return false;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = mathlib.util.FastMath.max(1, (int) mathlib.util.FastMath.ceil(mathlib.util.FastMath.abs(dt) / maxCheckInterval));
				int n = FastMath.max(1, (int) FastMath.ceil(FastMath.abs(dt) / maxCheckInterval));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double h = dt / n;
				double h = dt / n;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.UnivariateFunction f = new mathlib.analysis.UnivariateFunction()
				UnivariateFunction f = new UnivariateFunctionAnonymousInnerClassHelper(this, interpolator);

				double ta = t0;
				double ga = g0;
				for (int i = 0; i < n; ++i)
				{

					// evaluate handler value at the end of the substep
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tb = t0 + (i + 1) * h;
					double tb = t0 + (i + 1) * h;
					interpolator.InterpolatedTime = tb;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gb = handler.g(tb, getCompleteState(interpolator));
					double gb = handler.g(tb, getCompleteState(interpolator));

					// check events occurrence
					if (g0Positive ^ (gb >= 0))
					{
						// there is a sign change: an event is expected during this step

						// variation direction, with respect to the integration direction
						increasing = gb >= ga;

						// find the event time making sure we select a solution just at or past the exact root
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double root;
						double root;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: if (solver instanceof mathlib.analysis.solvers.BracketedUnivariateSolver<?>)
						if (solver is BracketedUnivariateSolver<?>)
						{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") mathlib.analysis.solvers.BracketedUnivariateSolver<mathlib.analysis.UnivariateFunction> bracketing = (mathlib.analysis.solvers.BracketedUnivariateSolver<mathlib.analysis.UnivariateFunction>) solver;
							BracketedUnivariateSolver<UnivariateFunction> bracketing = (BracketedUnivariateSolver<UnivariateFunction>) solver;
							root = forward ? bracketing.solve(maxIterationCount, f, ta, tb, AllowedSolution.RIGHT_SIDE) : bracketing.solve(maxIterationCount, f, tb, ta, AllowedSolution.LEFT_SIDE);
						}
						else
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double baseRoot = forward ? solver.solve(maxIterationCount, f, ta, tb) : solver.solve(maxIterationCount, f, tb, ta);
							double baseRoot = forward ? solver.solve(maxIterationCount, f, ta, tb) : solver.solve(maxIterationCount, f, tb, ta);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int remainingEval = maxIterationCount - solver.getEvaluations();
							int remainingEval = maxIterationCount - solver.Evaluations;
							BracketedUnivariateSolver<UnivariateFunction> bracketing = new PegasusSolver(solver.RelativeAccuracy, solver.AbsoluteAccuracy);
							root = forward ? UnivariateSolverUtils.forceSide(remainingEval, f, bracketing, baseRoot, ta, tb, AllowedSolution.RIGHT_SIDE) : UnivariateSolverUtils.forceSide(remainingEval, f, bracketing, baseRoot, tb, ta, AllowedSolution.LEFT_SIDE);
						}

						if ((!double.IsNaN(previousEventTime)) && (FastMath.abs(root - ta) <= convergence) && (FastMath.abs(root - previousEventTime) <= convergence))
						{
							// we have either found nothing or found (again ?) a past event,
							// retry the substep excluding this value, and taking care to have the
							// required sign in case the g function is noisy around its zero and
							// crosses the axis several times
							do
							{
								ta = forward ? ta + convergence : ta - convergence;
								ga = f.value(ta);
							} while ((g0Positive ^ (ga >= 0)) && (forward ^ (ta >= tb)));
							--i;
						}
						else if (double.IsNaN(previousEventTime) || (FastMath.abs(previousEventTime - root) > convergence))
						{
							pendingEventTime = root;
							pendingEvent = true;
							return true;
						}
						else
						{
							// no sign change: there is no event for now
							ta = tb;
							ga = gb;
						}

					}
					else
					{
						// no sign change: there is no event for now
						ta = tb;
						ga = gb;
					}

				}

				// no event during the whole step
				pendingEvent = false;
				pendingEventTime = double.NaN;
				return false;

			}
			catch (LocalMaxCountExceededException lmcee)
			{
				throw lmcee.Exception;
			}

		}

		private class UnivariateFunctionAnonymousInnerClassHelper : UnivariateFunction
		{
			private readonly EventState outerInstance;

			private StepInterpolator interpolator;

			public UnivariateFunctionAnonymousInnerClassHelper(EventState outerInstance, StepInterpolator interpolator)
			{
				this.outerInstance = outerInstance;
				this.interpolator = interpolator;
			}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(final double t) throws LocalMaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual double value(double t)
			{
				try
				{
					interpolator.InterpolatedTime = t;
					return outerInstance.handler.g(t, outerInstance.getCompleteState(interpolator));
				}
				catch (MaxCountExceededException mcee)
				{
					throw new LocalMaxCountExceededException(mcee);
				}
			}
		}

		/// <summary>
		/// Get the occurrence time of the event triggered in the current step. </summary>
		/// <returns> occurrence time of the event triggered in the current
		/// step or infinity if no events are triggered </returns>
		public virtual double EventTime
		{
			get
			{
				return pendingEvent ? pendingEventTime : (forward ? double.PositiveInfinity : double.NegativeInfinity);
			}
		}

		/// <summary>
		/// Acknowledge the fact the step has been accepted by the integrator. </summary>
		/// <param name="t"> value of the independent <i>time</i> variable at the
		/// end of the step </param>
		/// <param name="y"> array containing the current value of the state vector
		/// at the end of the step </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void stepAccepted(final double t, final double[] y)
		public virtual void stepAccepted(double t, double[] y)
		{

			t0 = t;
			g0 = handler.g(t, y);

			if (pendingEvent && (FastMath.abs(pendingEventTime - t) <= convergence))
			{
				// force the sign to its value "just after the event"
				previousEventTime = t;
				g0Positive = increasing;
				nextAction = handler.eventOccurred(t, y, !(increasing ^ forward));
			}
			else
			{
				g0Positive = g0 >= 0;
				nextAction = EventHandler_Action.CONTINUE;
			}
		}

		/// <summary>
		/// Check if the integration should be stopped at the end of the
		/// current step. </summary>
		/// <returns> true if the integration should be stopped </returns>
		public virtual bool stop()
		{
			return nextAction == EventHandler_Action.STOP;
		}

		/// <summary>
		/// Let the event handler reset the state if it wants. </summary>
		/// <param name="t"> value of the independent <i>time</i> variable at the
		/// beginning of the next step </param>
		/// <param name="y"> array were to put the desired state vector at the beginning
		/// of the next step </param>
		/// <returns> true if the integrator should reset the derivatives too </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public boolean reset(final double t, final double[] y)
		public virtual bool reset(double t, double[] y)
		{

			if (!(pendingEvent && (FastMath.abs(pendingEventTime - t) <= convergence)))
			{
				return false;
			}

			if (nextAction == EventHandler_Action.RESET_STATE)
			{
				handler.resetState(t, y);
			}
			pendingEvent = false;
			pendingEventTime = double.NaN;

			return (nextAction == EventHandler_Action.RESET_STATE) || (nextAction == EventHandler_Action.RESET_DERIVATIVES);

		}

		/// <summary>
		/// Local wrapper to propagate exceptions. </summary>
		private class LocalMaxCountExceededException : Exception
		{

			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20120901L;

			/// <summary>
			/// Wrapped exception. </summary>
			internal readonly MaxCountExceededException wrapped;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="exception"> exception to wrap </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public LocalMaxCountExceededException(final mathlib.exception.MaxCountExceededException exception)
			public LocalMaxCountExceededException(MaxCountExceededException exception)
			{
				wrapped = exception;
			}

			/// <summary>
			/// Get the wrapped exception. </summary>
			/// <returns> wrapped exception </returns>
			public virtual MaxCountExceededException Exception
			{
				get
				{
					return wrapped;
				}
			}

		}

	}

}