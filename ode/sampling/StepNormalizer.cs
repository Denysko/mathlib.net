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

namespace org.apache.commons.math3.ode.sampling
{

	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using Precision = org.apache.commons.math3.util.Precision;

	/// <summary>
	/// This class wraps an object implementing <seealso cref="FixedStepHandler"/>
	/// into a <seealso cref="StepHandler"/>.
	/// 
	/// <p>This wrapper allows to use fixed step handlers with general
	/// integrators which cannot guaranty their integration steps will
	/// remain constant and therefore only accept general step
	/// handlers.</p>
	/// 
	/// <p>The stepsize used is selected at construction time. The {@link
	/// FixedStepHandler#handleStep handleStep} method of the underlying
	/// <seealso cref="FixedStepHandler"/> object is called at normalized times. The
	/// normalized times can be influenced by the <seealso cref="StepNormalizerMode"/> and
	/// <seealso cref="StepNormalizerBounds"/>.</p>
	/// 
	/// <p>There is no constraint on the integrator, it can use any time step
	/// it needs (time steps longer or shorter than the fixed time step and
	/// non-integer ratios are all allowed).</p>
	/// 
	/// <p>
	/// <table border="1" align="center">
	/// <tr BGCOLOR="#CCCCFF"><td colspan=6><font size="+2">Examples (step size = 0.5)</font></td></tr>
	/// <tr BGCOLOR="#EEEEFF"><font size="+1"><td>Start time</td><td>End time</td>
	///  <td>Direction</td><td><seealso cref="StepNormalizerMode Mode"/></td>
	///  <td><seealso cref="StepNormalizerBounds Bounds"/></td><td>Output</td></font></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>0.8, 1.3, 1.8, 2.3, 2.8</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>0.3, 0.8, 1.3, 1.8, 2.3, 2.8</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>0.8, 1.3, 1.8, 2.3, 2.8, 3.1</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>0.3, 0.8, 1.3, 1.8, 2.3, 2.8, 3.1</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>0.3, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>0.5, 1.0, 1.5, 2.0, 2.5, 3.0, 3.1</td></tr>
	/// <tr><td>0.3</td><td>3.1</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>0.3, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0, 3.1</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>0.0, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>0.0, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>0.0, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>0.0</td><td>3.0</td><td>forward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>0.0, 0.5, 1.0, 1.5, 2.0, 2.5, 3.0</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>2.6, 2.1, 1.6, 1.1, 0.6</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>3.1, 2.6, 2.1, 1.6, 1.1, 0.6</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>2.6, 2.1, 1.6, 1.1, 0.6, 0.3</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>3.1, 2.6, 2.1, 1.6, 1.1, 0.6, 0.3</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>3.0, 2.5, 2.0, 1.5, 1.0, 0.5</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>3.1, 3.0, 2.5, 2.0, 1.5, 1.0, 0.5</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>3.0, 2.5, 2.0, 1.5, 1.0, 0.5, 0.3</td></tr>
	/// <tr><td>3.1</td><td>0.3</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>3.1, 3.0, 2.5, 2.0, 1.5, 1.0, 0.5, 0.3</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>3.0, 2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>3.0, 2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#NEITHER NEITHER"/></td><td>2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#FIRST FIRST"/></td><td>3.0, 2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#LAST LAST"/></td><td>2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// <tr><td>3.0</td><td>0.0</td><td>backward</td><td><seealso cref="StepNormalizerMode#MULTIPLES MULTIPLES"/></td><td><seealso cref="StepNormalizerBounds#BOTH BOTH"/></td><td>3.0, 2.5, 2.0, 1.5, 1.0, 0.5, 0.0</td></tr>
	/// </table>
	/// </p>
	/// </summary>
	/// <seealso cref= StepHandler </seealso>
	/// <seealso cref= FixedStepHandler </seealso>
	/// <seealso cref= StepNormalizerMode </seealso>
	/// <seealso cref= StepNormalizerBounds
	/// @version $Id: StepNormalizer.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public class StepNormalizer : StepHandler
	{
		/// <summary>
		/// Fixed time step. </summary>
		private double h;

		/// <summary>
		/// Underlying step handler. </summary>
		private readonly FixedStepHandler handler;

		/// <summary>
		/// First step time. </summary>
		private double firstTime;

		/// <summary>
		/// Last step time. </summary>
		private double lastTime;

		/// <summary>
		/// Last state vector. </summary>
		private double[] lastState;

		/// <summary>
		/// Last derivatives vector. </summary>
		private double[] lastDerivatives;

		/// <summary>
		/// Integration direction indicator. </summary>
		private bool forward;

		/// <summary>
		/// The step normalizer bounds settings to use. </summary>
		private readonly StepNormalizerBounds bounds;

		/// <summary>
		/// The step normalizer mode to use. </summary>
		private readonly StepNormalizerMode mode;

		/// <summary>
		/// Simple constructor. Uses <seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/>
		/// mode, and <seealso cref="StepNormalizerBounds#FIRST FIRST"/> bounds setting, for
		/// backwards compatibility. </summary>
		/// <param name="h"> fixed time step (sign is not used) </param>
		/// <param name="handler"> fixed time step handler to wrap </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StepNormalizer(final double h, final FixedStepHandler handler)
		public StepNormalizer(double h, FixedStepHandler handler) : this(h, handler, StepNormalizerMode.INCREMENT, StepNormalizerBounds.FIRST)
		{
		}

		/// <summary>
		/// Simple constructor. Uses <seealso cref="StepNormalizerBounds#FIRST FIRST"/>
		/// bounds setting. </summary>
		/// <param name="h"> fixed time step (sign is not used) </param>
		/// <param name="handler"> fixed time step handler to wrap </param>
		/// <param name="mode"> step normalizer mode to use
		/// @since 3.0 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StepNormalizer(final double h, final FixedStepHandler handler, final StepNormalizerMode mode)
		public StepNormalizer(double h, FixedStepHandler handler, StepNormalizerMode mode) : this(h, handler, mode, StepNormalizerBounds.FIRST)
		{
		}

		/// <summary>
		/// Simple constructor. Uses <seealso cref="StepNormalizerMode#INCREMENT INCREMENT"/>
		/// mode. </summary>
		/// <param name="h"> fixed time step (sign is not used) </param>
		/// <param name="handler"> fixed time step handler to wrap </param>
		/// <param name="bounds"> step normalizer bounds setting to use
		/// @since 3.0 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StepNormalizer(final double h, final FixedStepHandler handler, final StepNormalizerBounds bounds)
		public StepNormalizer(double h, FixedStepHandler handler, StepNormalizerBounds bounds) : this(h, handler, StepNormalizerMode.INCREMENT, bounds)
		{
		}

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="h"> fixed time step (sign is not used) </param>
		/// <param name="handler"> fixed time step handler to wrap </param>
		/// <param name="mode"> step normalizer mode to use </param>
		/// <param name="bounds"> step normalizer bounds setting to use
		/// @since 3.0 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public StepNormalizer(final double h, final FixedStepHandler handler, final StepNormalizerMode mode, final StepNormalizerBounds bounds)
		public StepNormalizer(double h, FixedStepHandler handler, StepNormalizerMode mode, StepNormalizerBounds bounds)
		{
			this.h = FastMath.abs(h);
			this.handler = handler;
			this.mode = mode;
			this.bounds = bounds;
			firstTime = double.NaN;
			lastTime = double.NaN;
			lastState = null;
			lastDerivatives = null;
			forward = true;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void init(double t0, double[] y0, double t)
		{

			firstTime = double.NaN;
			lastTime = double.NaN;
			lastState = null;
			lastDerivatives = null;
			forward = true;

			// initialize the underlying handler
			handler.init(t0, y0, t);

		}

		/// <summary>
		/// Handle the last accepted step </summary>
		/// <param name="interpolator"> interpolator for the last accepted step. For
		/// efficiency purposes, the various integrators reuse the same
		/// object on each call, so if the instance wants to keep it across
		/// all calls (for example to provide at the end of the integration a
		/// continuous model valid throughout the integration range), it
		/// should build a local copy using the clone method and store this
		/// copy. </param>
		/// <param name="isLast"> true if the step is the last one </param>
		/// <exception cref="MaxCountExceededException"> if the interpolator throws one because
		/// the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void handleStep(final StepInterpolator interpolator, final boolean isLast) throws org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void handleStep(StepInterpolator interpolator, bool isLast)
		{
			// The first time, update the last state with the start information.
			if (lastState == null)
			{
				firstTime = interpolator.PreviousTime;
				lastTime = interpolator.PreviousTime;
				interpolator.InterpolatedTime = lastTime;
				lastState = interpolator.InterpolatedState.clone();
				lastDerivatives = interpolator.InterpolatedDerivatives.clone();

				// Take the integration direction into account.
				forward = interpolator.CurrentTime >= lastTime;
				if (!forward)
				{
					h = -h;
				}
			}

			// Calculate next normalized step time.
			double nextTime = (mode == StepNormalizerMode.INCREMENT) ? lastTime + h : (FastMath.floor(lastTime / h) + 1) * h;
			if (mode == StepNormalizerMode.MULTIPLES && Precision.Equals(nextTime, lastTime, 1))
			{
				nextTime += h;
			}

			// Process normalized steps as long as they are in the current step.
			bool nextInStep = isNextInStep(nextTime, interpolator);
			while (nextInStep)
			{
				// Output the stored previous step.
				doNormalizedStep(false);

				// Store the next step as last step.
				storeStep(interpolator, nextTime);

				// Move on to the next step.
				nextTime += h;
				nextInStep = isNextInStep(nextTime, interpolator);
			}

			if (isLast)
			{
				// There will be no more steps. The stored one should be given to
				// the handler. We may have to output one more step. Only the last
				// one of those should be flagged as being the last.
				bool addLast = bounds.lastIncluded() && lastTime != interpolator.CurrentTime;
				doNormalizedStep(!addLast);
				if (addLast)
				{
					storeStep(interpolator, interpolator.CurrentTime);
					doNormalizedStep(true);
				}
			}
		}

		/// <summary>
		/// Returns a value indicating whether the next normalized time is in the
		/// current step. </summary>
		/// <param name="nextTime"> the next normalized time </param>
		/// <param name="interpolator"> interpolator for the last accepted step, to use to
		/// get the end time of the current step </param>
		/// <returns> value indicating whether the next normalized time is in the
		/// current step </returns>
		private bool isNextInStep(double nextTime, StepInterpolator interpolator)
		{
			return forward ? nextTime <= interpolator.CurrentTime : nextTime >= interpolator.CurrentTime;
		}

		/// <summary>
		/// Invokes the underlying step handler for the current normalized step. </summary>
		/// <param name="isLast"> true if the step is the last one </param>
		private void doNormalizedStep(bool isLast)
		{
			if (!bounds.firstIncluded() && firstTime == lastTime)
			{
				return;
			}
			handler.handleStep(lastTime, lastState, lastDerivatives, isLast);
		}

		/// <summary>
		/// Stores the interpolated information for the given time in the current
		/// state. </summary>
		/// <param name="interpolator"> interpolator for the last accepted step, to use to
		/// get the interpolated information </param>
		/// <param name="t"> the time for which to store the interpolated information </param>
		/// <exception cref="MaxCountExceededException"> if the interpolator throws one because
		/// the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void storeStep(StepInterpolator interpolator, double t) throws org.apache.commons.math3.exception.MaxCountExceededException
		private void storeStep(StepInterpolator interpolator, double t)
		{
			lastTime = t;
			interpolator.InterpolatedTime = lastTime;
			Array.Copy(interpolator.InterpolatedState, 0, lastState, 0, lastState.Length);
			Array.Copy(interpolator.InterpolatedDerivatives, 0, lastDerivatives, 0, lastDerivatives.Length);
		}
	}

}