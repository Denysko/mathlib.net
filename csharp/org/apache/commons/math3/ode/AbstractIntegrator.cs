using System;
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

namespace org.apache.commons.math3.ode
{


	using BracketingNthOrderBrentSolver = org.apache.commons.math3.analysis.solvers.BracketingNthOrderBrentSolver;
	using UnivariateSolver = org.apache.commons.math3.analysis.solvers.UnivariateSolver;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using NoBracketingException = org.apache.commons.math3.exception.NoBracketingException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using EventHandler = org.apache.commons.math3.ode.events.EventHandler;
	using EventState = org.apache.commons.math3.ode.events.EventState;
	using AbstractStepInterpolator = org.apache.commons.math3.ode.sampling.AbstractStepInterpolator;
	using StepHandler = org.apache.commons.math3.ode.sampling.StepHandler;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using Incrementor = org.apache.commons.math3.util.Incrementor;
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// Base class managing common boilerplate for all integrators.
	/// @version $Id: AbstractIntegrator.java 1517418 2013-08-26 03:18:55Z dbrosius $
	/// @since 2.0
	/// </summary>
	public abstract class AbstractIntegrator : FirstOrderIntegrator
	{

		/// <summary>
		/// Step handler. </summary>
		protected internal ICollection<StepHandler> stepHandlers;

		/// <summary>
		/// Current step start time. </summary>
		protected internal double stepStart;

		/// <summary>
		/// Current stepsize. </summary>
		protected internal double stepSize;

		/// <summary>
		/// Indicator for last step. </summary>
		protected internal bool isLastStep;

		/// <summary>
		/// Indicator that a state or derivative reset was triggered by some event. </summary>
		protected internal bool resetOccurred;

		/// <summary>
		/// Events states. </summary>
		private ICollection<EventState> eventsStates;

		/// <summary>
		/// Initialization indicator of events states. </summary>
		private bool statesInitialized;

		/// <summary>
		/// Name of the method. </summary>
		private readonly string name;

		/// <summary>
		/// Counter for number of evaluations. </summary>
		private Incrementor evaluations;

		/// <summary>
		/// Differential equations to integrate. </summary>
		[NonSerialized]
		private ExpandableStatefulODE expandable;

		/// <summary>
		/// Build an instance. </summary>
		/// <param name="name"> name of the method </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public AbstractIntegrator(final String name)
		public AbstractIntegrator(string name)
		{
			this.name = name;
			stepHandlers = new List<StepHandler>();
			stepStart = double.NaN;
			stepSize = double.NaN;
			eventsStates = new List<EventState>();
			statesInitialized = false;
			evaluations = new Incrementor();
			MaxEvaluations = -1;
			evaluations.resetCount();
		}

		/// <summary>
		/// Build an instance with a null name.
		/// </summary>
		protected internal AbstractIntegrator() : this(null)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addStepHandler(final org.apache.commons.math3.ode.sampling.StepHandler handler)
		public virtual void addStepHandler(StepHandler handler)
		{
			stepHandlers.Add(handler);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ICollection<StepHandler> StepHandlers
		{
			get
			{
				return Collections.unmodifiableCollection(stepHandlers);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void clearStepHandlers()
		{
			stepHandlers.Clear();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addEventHandler(final org.apache.commons.math3.ode.events.EventHandler handler, final double maxCheckInterval, final double convergence, final int maxIterationCount)
		public virtual void addEventHandler(EventHandler handler, double maxCheckInterval, double convergence, int maxIterationCount)
		{
			addEventHandler(handler, maxCheckInterval, convergence, maxIterationCount, new BracketingNthOrderBrentSolver(convergence, 5));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addEventHandler(final org.apache.commons.math3.ode.events.EventHandler handler, final double maxCheckInterval, final double convergence, final int maxIterationCount, final org.apache.commons.math3.analysis.solvers.UnivariateSolver solver)
		public virtual void addEventHandler(EventHandler handler, double maxCheckInterval, double convergence, int maxIterationCount, UnivariateSolver solver)
		{
			eventsStates.Add(new EventState(handler, maxCheckInterval, convergence, maxIterationCount, solver));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ICollection<EventHandler> EventHandlers
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.ode.events.EventHandler> list = new java.util.ArrayList<org.apache.commons.math3.ode.events.EventHandler>(eventsStates.size());
				IList<EventHandler> list = new List<EventHandler>(eventsStates.Count);
				foreach (EventState state in eventsStates)
				{
					list.Add(state.EventHandler);
				}
				return Collections.unmodifiableCollection(list);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void clearEventHandlers()
		{
			eventsStates.Clear();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double CurrentStepStart
		{
			get
			{
				return stepStart;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double CurrentSignedStepsize
		{
			get
			{
				return stepSize;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int MaxEvaluations
		{
			set
			{
				evaluations.MaximalCount = (value < 0) ? int.MaxValue : value;
			}
			get
			{
				return evaluations.MaximalCount;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual int Evaluations
		{
			get
			{
				return evaluations.Count;
			}
		}

		/// <summary>
		/// Prepare the start of an integration. </summary>
		/// <param name="t0"> start value of the independent <i>time</i> variable </param>
		/// <param name="y0"> array containing the start value of the state vector </param>
		/// <param name="t"> target time for the integration </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void initIntegration(final double t0, final double[] y0, final double t)
		protected internal virtual void initIntegration(double t0, double[] y0, double t)
		{

			evaluations.resetCount();

			foreach (EventState state in eventsStates)
			{
				state.Expandable = expandable;
				state.EventHandler.init(t0, y0, t);
			}

			foreach (StepHandler handler in stepHandlers)
			{
				handler.init(t0, y0, t);
			}

			StateInitialized = false;

		}

		/// <summary>
		/// Set the equations. </summary>
		/// <param name="equations"> equations to set </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void setEquations(final ExpandableStatefulODE equations)
		protected internal virtual ExpandableStatefulODE Equations
		{
			set
			{
				this.expandable = value;
			}
		}

		/// <summary>
		/// Get the differential equations to integrate. </summary>
		/// <returns> differential equations to integrate
		/// @since 3.2 </returns>
		protected internal virtual ExpandableStatefulODE Expandable
		{
			get
			{
				return expandable;
			}
		}

		/// <summary>
		/// Get the evaluations counter. </summary>
		/// <returns> evaluations counter
		/// @since 3.2 </returns>
		protected internal virtual Incrementor EvaluationsCounter
		{
			get
			{
				return evaluations;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double integrate(final FirstOrderDifferentialEquations equations, final double t0, final double[] y0, final double t, final double[] y) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double integrate(FirstOrderDifferentialEquations equations, double t0, double[] y0, double t, double[] y)
		{

			if (y0.Length != equations.Dimension)
			{
				throw new DimensionMismatchException(y0.Length, equations.Dimension);
			}
			if (y.Length != equations.Dimension)
			{
				throw new DimensionMismatchException(y.Length, equations.Dimension);
			}

			// prepare expandable stateful equations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExpandableStatefulODE expandableODE = new ExpandableStatefulODE(equations);
			ExpandableStatefulODE expandableODE = new ExpandableStatefulODE(equations);
			expandableODE.Time = t0;
			expandableODE.PrimaryState = y0;

			// perform integration
			integrate(expandableODE, t);

			// extract results back from the stateful equations
			Array.Copy(expandableODE.PrimaryState, 0, y, 0, y.Length);
			return expandableODE.Time;

		}

		/// <summary>
		/// Integrate a set of differential equations up to the given time.
		/// <p>This method solves an Initial Value Problem (IVP).</p>
		/// <p>The set of differential equations is composed of a main set, which
		/// can be extended by some sets of secondary equations. The set of
		/// equations must be already set up with initial time and partial states.
		/// At integration completion, the final time and partial states will be
		/// available in the same object.</p>
		/// <p>Since this method stores some internal state variables made
		/// available in its public interface during integration ({@link
		/// #getCurrentSignedStepsize()}), it is <em>not</em> thread-safe.</p> </summary>
		/// <param name="equations"> complete set of differential equations to integrate </param>
		/// <param name="t"> target time for the integration
		/// (can be set to a value smaller than <code>t0</code> for backward integration) </param>
		/// <exception cref="NumberIsTooSmallException"> if integration step is too small </exception>
		/// <exception cref="DimensionMismatchException"> if the dimension of the complete state does not
		/// match the complete equations sets dimension </exception>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="NoBracketingException"> if the location of an event cannot be bracketed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void integrate(ExpandableStatefulODE equations, double t) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.NoBracketingException;
		public abstract void integrate(ExpandableStatefulODE equations, double t);

		/// <summary>
		/// Compute the derivatives and check the number of evaluations. </summary>
		/// <param name="t"> current value of the independent <I>time</I> variable </param>
		/// <param name="y"> array containing the current value of the state vector </param>
		/// <param name="yDot"> placeholder array where to put the time derivative of the state vector </param>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDerivatives(final double t, final double[] y, final double[] yDot) throws org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void computeDerivatives(double t, double[] y, double[] yDot)
		{
			evaluations.incrementCount();
			expandable.computeDerivatives(t, y, yDot);
		}

		/// <summary>
		/// Set the stateInitialized flag.
		/// <p>This method must be called by integrators with the value
		/// {@code false} before they start integration, so a proper lazy
		/// initialization is done automatically on the first step.</p> </summary>
		/// <param name="stateInitialized"> new value for the flag
		/// @since 2.2 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void setStateInitialized(final boolean stateInitialized)
		protected internal virtual bool StateInitialized
		{
			set
			{
				this.statesInitialized = value;
			}
		}

		/// <summary>
		/// Accept a step, triggering events and step handlers. </summary>
		/// <param name="interpolator"> step interpolator </param>
		/// <param name="y"> state vector at step end time, must be reset if an event
		/// asks for resetting or if an events stops integration during the step </param>
		/// <param name="yDot"> placeholder array where to put the time derivative of the state vector </param>
		/// <param name="tEnd"> final integration time </param>
		/// <returns> time at end of step </returns>
		/// <exception cref="MaxCountExceededException"> if the interpolator throws one because
		/// the number of functions evaluations is exceeded </exception>
		/// <exception cref="NoBracketingException"> if the location of an event cannot be bracketed </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings
		/// @since 2.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double acceptStep(final org.apache.commons.math3.ode.sampling.AbstractStepInterpolator interpolator, final double[] y, final double[] yDot, final double tEnd) throws org.apache.commons.math3.exception.MaxCountExceededException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual double acceptStep(AbstractStepInterpolator interpolator, double[] y, double[] yDot, double tEnd)
		{

				double previousT = interpolator.GlobalPreviousTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentT = interpolator.getGlobalCurrentTime();
				double currentT = interpolator.GlobalCurrentTime;

				// initialize the events states if needed
				if (!statesInitialized)
				{
					foreach (EventState state in eventsStates)
					{
						state.reinitializeBegin(interpolator);
					}
					statesInitialized = true;
				}

				// search for next events that may occur during the step
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int orderingSign = interpolator.isForward() ? +1 : -1;
				int orderingSign = interpolator.Forward ? + 1 : -1;
				SortedSet<EventState> occurringEvents = new SortedSet<EventState>(new ComparatorAnonymousInnerClassHelper(this, orderingSign));

				foreach (EventState state in eventsStates)
				{
					if (state.evaluateStep(interpolator))
					{
						// the event occurs during the current step
						occurringEvents.add(state);
					}
				}

				while (!occurringEvents.Empty)
				{

					// handle the chronologically first event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<org.apache.commons.math3.ode.events.EventState> iterator = occurringEvents.iterator();
					IEnumerator<EventState> iterator = occurringEvents.GetEnumerator();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.ode.events.EventState currentEvent = iterator.next();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					EventState currentEvent = iterator.next();
					iterator.remove();

					// restrict the interpolator to the first part of the step, up to the event
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double eventT = currentEvent.getEventTime();
					double eventT = currentEvent.EventTime;
					interpolator.SoftPreviousTime = previousT;
					interpolator.SoftCurrentTime = eventT;

					// get state at event time
					interpolator.InterpolatedTime = eventT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] eventYComplete = new double[y.length];
					double[] eventYComplete = new double[y.Length];
					expandable.PrimaryMapper.insertEquationData(interpolator.InterpolatedState, eventYComplete);
					int index = 0;
					foreach (EquationsMapper secondary in expandable.SecondaryMappers)
					{
						secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index++), eventYComplete);
					}

					// advance all event states to current time
					foreach (EventState state in eventsStates)
					{
						state.stepAccepted(eventT, eventYComplete);
						isLastStep = isLastStep || state.stop();
					}

					// handle the first part of the step, up to the event
					foreach (StepHandler handler in stepHandlers)
					{
						handler.handleStep(interpolator, isLastStep);
					}

					if (isLastStep)
					{
						// the event asked to stop integration
						Array.Copy(eventYComplete, 0, y, 0, y.Length);
						return eventT;
					}

					bool needReset = false;
					foreach (EventState state in eventsStates)
					{
						needReset = needReset || state.reset(eventT, eventYComplete);
					}
					if (needReset)
					{
						// some event handler has triggered changes that
						// invalidate the derivatives, we need to recompute them
						interpolator.InterpolatedTime = eventT;
						Array.Copy(eventYComplete, 0, y, 0, y.Length);
						computeDerivatives(eventT, y, yDot);
						resetOccurred = true;
						return eventT;
					}

					// prepare handling of the remaining part of the step
					previousT = eventT;
					interpolator.SoftPreviousTime = eventT;
					interpolator.SoftCurrentTime = currentT;

					// check if the same event occurs again in the remaining part of the step
					if (currentEvent.evaluateStep(interpolator))
					{
						// the event occurs during the current step
						occurringEvents.add(currentEvent);
					}

				}

				// last part of the step, after the last event
				interpolator.InterpolatedTime = currentT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] currentY = new double[y.length];
				double[] currentY = new double[y.Length];
				expandable.PrimaryMapper.insertEquationData(interpolator.InterpolatedState, currentY);
				int index = 0;
				foreach (EquationsMapper secondary in expandable.SecondaryMappers)
				{
					secondary.insertEquationData(interpolator.getInterpolatedSecondaryState(index++), currentY);
				}
				foreach (EventState state in eventsStates)
				{
					state.stepAccepted(currentT, currentY);
					isLastStep = isLastStep || state.stop();
				}
				isLastStep = isLastStep || Precision.Equals(currentT, tEnd, 1);

				// handle the remaining part of the step, after all events if any
				foreach (StepHandler handler in stepHandlers)
				{
					handler.handleStep(interpolator, isLastStep);
				}

				return currentT;

		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<EventState>
		{
			private readonly AbstractIntegrator outerInstance;

			private int orderingSign;

			public ComparatorAnonymousInnerClassHelper(AbstractIntegrator outerInstance, int orderingSign)
			{
				this.outerInstance = outerInstance;
				this.orderingSign = orderingSign;
			}


						/// <summary>
						/// {@inheritDoc} </summary>
			public virtual int Compare(EventState es0, EventState es1)
			{
				return orderingSign * es0.EventTime.CompareTo(es1.EventTime);
			}

		}

		/// <summary>
		/// Check the integration span. </summary>
		/// <param name="equations"> set of differential equations </param>
		/// <param name="t"> target time for the integration </param>
		/// <exception cref="NumberIsTooSmallException"> if integration span is too small </exception>
		/// <exception cref="DimensionMismatchException"> if adaptive step size integrators
		/// tolerance arrays dimensions are not compatible with equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void sanityChecks(final ExpandableStatefulODE equations, final double t) throws org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		protected internal virtual void sanityChecks(ExpandableStatefulODE equations, double t)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double threshold = 1000 * org.apache.commons.math3.util.FastMath.ulp(org.apache.commons.math3.util.FastMath.max(org.apache.commons.math3.util.FastMath.abs(equations.getTime()), org.apache.commons.math3.util.FastMath.abs(t)));
			double threshold = 1000 * FastMath.ulp(FastMath.max(FastMath.abs(equations.Time), FastMath.abs(t)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dt = org.apache.commons.math3.util.FastMath.abs(equations.getTime() - t);
			double dt = FastMath.abs(equations.Time - t);
			if (dt <= threshold)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.TOO_SMALL_INTEGRATION_INTERVAL, dt, threshold, false);
			}

		}

	}

}