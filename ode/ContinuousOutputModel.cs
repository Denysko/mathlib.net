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

namespace mathlib.ode
{


	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using StepHandler = mathlib.ode.sampling.StepHandler;
	using StepInterpolator = mathlib.ode.sampling.StepInterpolator;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class stores all information provided by an ODE integrator
	/// during the integration process and build a continuous model of the
	/// solution from this.
	/// 
	/// <p>This class act as a step handler from the integrator point of
	/// view. It is called iteratively during the integration process and
	/// stores a copy of all steps information in a sorted collection for
	/// later use. Once the integration process is over, the user can use
	/// the <seealso cref="#setInterpolatedTime setInterpolatedTime"/> and {@link
	/// #getInterpolatedState getInterpolatedState} to retrieve this
	/// information at any time. It is important to wait for the
	/// integration to be over before attempting to call {@link
	/// #setInterpolatedTime setInterpolatedTime} because some internal
	/// variables are set only once the last step has been handled.</p>
	/// 
	/// <p>This is useful for example if the main loop of the user
	/// application should remain independent from the integration process
	/// or if one needs to mimic the behaviour of an analytical model
	/// despite a numerical model is used (i.e. one needs the ability to
	/// get the model value at any time or to navigate through the
	/// data).</p>
	/// 
	/// <p>If problem modeling is done with several separate
	/// integration phases for contiguous intervals, the same
	/// ContinuousOutputModel can be used as step handler for all
	/// integration phases as long as they are performed in order and in
	/// the same direction. As an example, one can extrapolate the
	/// trajectory of a satellite with one model (i.e. one set of
	/// differential equations) up to the beginning of a maneuver, use
	/// another more complex model including thrusters modeling and
	/// accurate attitude control during the maneuver, and revert to the
	/// first model after the end of the maneuver. If the same continuous
	/// output model handles the steps of all integration phases, the user
	/// do not need to bother when the maneuver begins or ends, he has all
	/// the data available in a transparent manner.</p>
	/// 
	/// <p>An important feature of this class is that it implements the
	/// <code>Serializable</code> interface. This means that the result of
	/// an integration can be serialized and reused later (if stored into a
	/// persistent medium like a filesystem or a database) or elsewhere (if
	/// sent to another application). Only the result of the integration is
	/// stored, there is no reference to the integrated problem by
	/// itself.</p>
	/// 
	/// <p>One should be aware that the amount of data stored in a
	/// ContinuousOutputModel instance can be important if the state vector
	/// is large, if the integration interval is long or if the steps are
	/// small (which can result from small tolerance settings in {@link
	/// mathlib.ode.nonstiff.AdaptiveStepsizeIntegrator adaptive
	/// step size integrators}).</p>
	/// </summary>
	/// <seealso cref= StepHandler </seealso>
	/// <seealso cref= StepInterpolator
	/// @version $Id: ContinuousOutputModel.java 1463684 2013-04-02 19:04:13Z luc $
	/// @since 1.2 </seealso>

	[Serializable]
	public class ContinuousOutputModel : StepHandler
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -1417964919405031606L;

		/// <summary>
		/// Initial integration time. </summary>
		private double initialTime;

		/// <summary>
		/// Final integration time. </summary>
		private double finalTime;

		/// <summary>
		/// Integration direction indicator. </summary>
		private bool forward;

		/// <summary>
		/// Current interpolator index. </summary>
		private int index;

		/// <summary>
		/// Steps table. </summary>
		private IList<StepInterpolator> steps;

	  /// <summary>
	  /// Simple constructor.
	  /// Build an empty continuous output model.
	  /// </summary>
	  public ContinuousOutputModel()
	  {
		steps = new List<StepInterpolator>();
		initialTime = double.NaN;
		finalTime = double.NaN;
		forward = true;
		index = 0;
	  }

	  /// <summary>
	  /// Append another model at the end of the instance. </summary>
	  /// <param name="model"> model to add at the end of the instance </param>
	  /// <exception cref="MathIllegalArgumentException"> if the model to append is not
	  /// compatible with the instance (dimension of the state vector,
	  /// propagation direction, hole between the dates) </exception>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded
	  /// during step finalization </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void append(final ContinuousOutputModel model) throws mathlib.exception.MathIllegalArgumentException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual void append(ContinuousOutputModel model)
	  {

		if (model.steps.Count == 0)
		{
		  return;
		}

		if (steps.Count == 0)
		{
		  initialTime = model.initialTime;
		  forward = model.forward;
		}
		else
		{

		  if (InterpolatedState.Length != model.InterpolatedState.Length)
		  {
			  throw new DimensionMismatchException(model.InterpolatedState.Length, InterpolatedState.Length);
		  }

		  if (forward ^ model.forward)
		  {
			  throw new MathIllegalArgumentException(LocalizedFormats.PROPAGATION_DIRECTION_MISMATCH);
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.ode.sampling.StepInterpolator lastInterpolator = steps.get(index);
		  StepInterpolator lastInterpolator = steps[index];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double current = lastInterpolator.getCurrentTime();
		  double current = lastInterpolator.CurrentTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previous = lastInterpolator.getPreviousTime();
		  double previous = lastInterpolator.PreviousTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double step = current - previous;
		  double step = current - previous;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double gap = model.getInitialTime() - current;
		  double gap = model.InitialTime - current;
		  if (FastMath.abs(gap) > 1.0e-3 * FastMath.abs(step))
		  {
			throw new MathIllegalArgumentException(LocalizedFormats.HOLE_BETWEEN_MODELS_TIME_RANGES, FastMath.abs(gap));
		  }

		}

		foreach (StepInterpolator interpolator in model.steps)
		{
		  steps.Add(interpolator.copy());
		}

		index = steps.Count - 1;
		finalTime = (steps[index]).CurrentTime;

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public virtual void init(double t0, double[] y0, double t)
	  {
		initialTime = double.NaN;
		finalTime = double.NaN;
		forward = true;
		index = 0;
		steps.Clear();
	  }

	  /// <summary>
	  /// Handle the last accepted step.
	  /// A copy of the information provided by the last step is stored in
	  /// the instance for later use. </summary>
	  /// <param name="interpolator"> interpolator for the last accepted step. </param>
	  /// <param name="isLast"> true if the step is the last one </param>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded
	  /// during step finalization </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void handleStep(final mathlib.ode.sampling.StepInterpolator interpolator, final boolean isLast) throws mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual void handleStep(StepInterpolator interpolator, bool isLast)
	  {

		if (steps.Count == 0)
		{
		  initialTime = interpolator.PreviousTime;
		  forward = interpolator.Forward;
		}

		steps.Add(interpolator.copy());

		if (isLast)
		{
		  finalTime = interpolator.CurrentTime;
		  index = steps.Count - 1;
		}

	  }

	  /// <summary>
	  /// Get the initial integration time. </summary>
	  /// <returns> initial integration time </returns>
	  public virtual double InitialTime
	  {
		  get
		  {
			return initialTime;
		  }
	  }

	  /// <summary>
	  /// Get the final integration time. </summary>
	  /// <returns> final integration time </returns>
	  public virtual double FinalTime
	  {
		  get
		  {
			return finalTime;
		  }
	  }

	  /// <summary>
	  /// Get the time of the interpolated point.
	  /// If <seealso cref="#setInterpolatedTime"/> has not been called, it returns
	  /// the final integration time. </summary>
	  /// <returns> interpolation point time </returns>
	  public virtual double InterpolatedTime
	  {
		  get
		  {
			return steps[index].InterpolatedTime;
		  }
		  set
		  {
    
			  // initialize the search with the complete steps table
			  int iMin = 0;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.ode.sampling.StepInterpolator sMin = steps.get(iMin);
			  StepInterpolator sMin = steps[iMin];
			  double tMin = 0.5 * (sMin.PreviousTime + sMin.CurrentTime);
    
			  int iMax = steps.Count - 1;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.ode.sampling.StepInterpolator sMax = steps.get(iMax);
			  StepInterpolator sMax = steps[iMax];
			  double tMax = 0.5 * (sMax.PreviousTime + sMax.CurrentTime);
    
			  // handle points outside of the integration interval
			  // or in the first and last step
			  if (locatePoint(value, sMin) <= 0)
			  {
				index = iMin;
				sMin.InterpolatedTime = value;
				return;
			  }
			  if (locatePoint(value, sMax) >= 0)
			  {
				index = iMax;
				sMax.InterpolatedTime = value;
				return;
			  }
    
			  // reduction of the table slice size
			  while (iMax - iMin > 5)
			  {
    
				// use the last estimated index as the splitting index
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.ode.sampling.StepInterpolator si = steps.get(index);
				StepInterpolator si = steps[index];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int location = locatePoint(value, si);
				int location = locatePoint(value, si);
				if (location < 0)
				{
				  iMax = index;
				  tMax = 0.5 * (si.PreviousTime + si.CurrentTime);
				}
				else if (location > 0)
				{
				  iMin = index;
				  tMin = 0.5 * (si.PreviousTime + si.CurrentTime);
				}
				else
				{
				  // we have found the target step, no need to continue searching
				  si.InterpolatedTime = value;
				  return;
				}
    
				// compute a new estimate of the index in the reduced table slice
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int iMed = (iMin + iMax) / 2;
				int iMed = (iMin + iMax) / 2;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final mathlib.ode.sampling.StepInterpolator sMed = steps.get(iMed);
				StepInterpolator sMed = steps[iMed];
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double tMed = 0.5 * (sMed.getPreviousTime() + sMed.getCurrentTime());
				double tMed = 0.5 * (sMed.PreviousTime + sMed.CurrentTime);
    
				if ((FastMath.abs(tMed - tMin) < 1e-6) || (FastMath.abs(tMax - tMed) < 1e-6))
				{
				  // too close to the bounds, we estimate using a simple dichotomy
				  index = iMed;
				}
				else
				{
				  // estimate the index using a reverse quadratic polynom
				  // (reverse means we have i = P(t), thus allowing to simply
				  // compute index = P(value) rather than solving a quadratic equation)
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double d12 = tMax - tMed;
				  double d12 = tMax - tMed;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double d23 = tMed - tMin;
				  double d23 = tMed - tMin;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double d13 = tMax - tMin;
				  double d13 = tMax - tMin;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double dt1 = value - tMax;
				  double dt1 = value - tMax;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double dt2 = value - tMed;
				  double dt2 = value - tMed;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double dt3 = value - tMin;
				  double dt3 = value - tMin;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double iLagrange = ((dt2 * dt3 * d23) * iMax - (dt1 * dt3 * d13) * iMed + (dt1 * dt2 * d12) * iMin) / (d12 * d23 * d13);
				  double iLagrange = ((dt2 * dt3 * d23) * iMax - (dt1 * dt3 * d13) * iMed + (dt1 * dt2 * d12) * iMin) / (d12 * d23 * d13);
				  index = (int) FastMath.rint(iLagrange);
				}
    
				// force the next size reduction to be at least one tenth
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int low = mathlib.util.FastMath.max(iMin + 1, (9 * iMin + iMax) / 10);
				int low = FastMath.max(iMin + 1, (9 * iMin + iMax) / 10);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int high = mathlib.util.FastMath.min(iMax - 1, (iMin + 9 * iMax) / 10);
				int high = FastMath.min(iMax - 1, (iMin + 9 * iMax) / 10);
				if (index < low)
				{
				  index = low;
				}
				else if (index > high)
				{
				  index = high;
				}
    
			  }
    
			  // now the table slice is very small, we perform an iterative search
			  index = iMin;
			  while ((index <= iMax) && (locatePoint(value, steps[index]) > 0))
			  {
				++index;
			  }
    
			  steps[index].InterpolatedTime = value;
    
		  }
	  }

	  /// <summary>
	  /// Set the time of the interpolated point.
	  /// <p>This method should <strong>not</strong> be called before the
	  /// integration is over because some internal variables are set only
	  /// once the last step has been handled.</p>
	  /// <p>Setting the time outside of the integration interval is now
	  /// allowed (it was not allowed up to version 5.9 of Mantissa), but
	  /// should be used with care since the accuracy of the interpolator
	  /// will probably be very poor far from this interval. This allowance
	  /// has been added to simplify implementation of search algorithms
	  /// near the interval endpoints.</p> </summary>
	  /// <param name="time"> time of the interpolated point </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setInterpolatedTime(final double time)

	  /// <summary>
	  /// Get the state vector of the interpolated point. </summary>
	  /// <returns> state vector at time <seealso cref="#getInterpolatedTime"/> </returns>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
	  /// <seealso cref= #getInterpolatedSecondaryState(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedState() throws mathlib.exception.MaxCountExceededException
	  public virtual double[] InterpolatedState
	  {
		  get
		  {
			return steps[index].InterpolatedState;
		  }
	  }

	  /// <summary>
	  /// Get the interpolated secondary state corresponding to the secondary equations. </summary>
	  /// <param name="secondaryStateIndex"> index of the secondary set, as returned by {@link
	  /// mathlib.ode.ExpandableStatefulODE#addSecondaryEquations(
	  /// mathlib.ode.SecondaryEquations)
	  /// ExpandableStatefulODE.addSecondaryEquations(SecondaryEquations)} </param>
	  /// <returns> interpolated secondary state at the current interpolation date </returns>
	  /// <seealso cref= #getInterpolatedState()
	  /// @since 3.2 </seealso>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedSecondaryState(final int secondaryStateIndex) throws mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual double[] getInterpolatedSecondaryState(int secondaryStateIndex)
	  {
		return steps[index].getInterpolatedSecondaryState(secondaryStateIndex);
	  }

	  /// <summary>
	  /// Compare a step interval and a double. </summary>
	  /// <param name="time"> point to locate </param>
	  /// <param name="interval"> step interval </param>
	  /// <returns> -1 if the double is before the interval, 0 if it is in
	  /// the interval, and +1 if it is after the interval, according to
	  /// the interval direction </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int locatePoint(final double time, final mathlib.ode.sampling.StepInterpolator interval)
	  private int locatePoint(double time, StepInterpolator interval)
	  {
		if (forward)
		{
		  if (time < interval.PreviousTime)
		  {
			return -1;
		  }
		  else if (time > interval.CurrentTime)
		  {
			return +1;
		  }
		  else
		  {
			return 0;
		  }
		}
		if (time > interval.PreviousTime)
		{
		  return -1;
		}
		else if (time < interval.CurrentTime)
		{
		  return +1;
		}
		else
		{
		  return 0;
		}
	  }

	}

}