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
	/// This abstract class represents an interpolator over the last step
	/// during an ODE integration.
	/// 
	/// <p>The various ODE integrators provide objects extending this class
	/// to the step handlers. The handlers can use these objects to
	/// retrieve the state vector at intermediate times between the
	/// previous and the current grid points (dense output).</p>
	/// </summary>
	/// <seealso cref= org.apache.commons.math3.ode.FirstOrderIntegrator </seealso>
	/// <seealso cref= org.apache.commons.math3.ode.SecondOrderIntegrator </seealso>
	/// <seealso cref= StepHandler
	/// 
	/// @version $Id: AbstractStepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	///  </seealso>

	public abstract class AbstractStepInterpolator : StepInterpolator
	{

	  /// <summary>
	  /// current time step </summary>
	  protected internal double h;

	  /// <summary>
	  /// current state </summary>
	  protected internal double[] currentState;

	  /// <summary>
	  /// interpolated time </summary>
	  protected internal double interpolatedTime;

	  /// <summary>
	  /// interpolated state </summary>
	  protected internal double[] interpolatedState;

	  /// <summary>
	  /// interpolated derivatives </summary>
	  protected internal double[] interpolatedDerivatives;

	  /// <summary>
	  /// interpolated primary state </summary>
	  protected internal double[] interpolatedPrimaryState;

	  /// <summary>
	  /// interpolated primary derivatives </summary>
	  protected internal double[] interpolatedPrimaryDerivatives;

	  /// <summary>
	  /// interpolated secondary state </summary>
	  protected internal double[][] interpolatedSecondaryState;

	  /// <summary>
	  /// interpolated secondary derivatives </summary>
	  protected internal double[][] interpolatedSecondaryDerivatives;

	  /// <summary>
	  /// global previous time </summary>
	  private double globalPreviousTime;

	  /// <summary>
	  /// global current time </summary>
	  private double globalCurrentTime;

	  /// <summary>
	  /// soft previous time </summary>
	  private double softPreviousTime;

	  /// <summary>
	  /// soft current time </summary>
	  private double softCurrentTime;

	  /// <summary>
	  /// indicate if the step has been finalized or not. </summary>
	  private bool finalized;

	  /// <summary>
	  /// integration direction. </summary>
	  private bool forward;

	  /// <summary>
	  /// indicator for dirty state. </summary>
	  private bool dirtyState;

	  /// <summary>
	  /// Equations mapper for the primary equations set. </summary>
	  private EquationsMapper primaryMapper;

	  /// <summary>
	  /// Equations mappers for the secondary equations sets. </summary>
	  private EquationsMapper[] secondaryMappers;

	  /// <summary>
	  /// Simple constructor.
	  /// This constructor builds an instance that is not usable yet, the
	  /// <seealso cref="#reinitialize"/> method should be called before using the
	  /// instance in order to initialize the internal arrays. This
	  /// constructor is used only in order to delay the initialization in
	  /// some cases. As an example, the {@link
	  /// org.apache.commons.math3.ode.nonstiff.EmbeddedRungeKuttaIntegrator}
	  /// class uses the prototyping design pattern to create the step
	  /// interpolators by cloning an uninitialized model and latter
	  /// initializing the copy.
	  /// </summary>
	  protected internal AbstractStepInterpolator()
	  {
		globalPreviousTime = double.NaN;
		globalCurrentTime = double.NaN;
		softPreviousTime = double.NaN;
		softCurrentTime = double.NaN;
		h = double.NaN;
		interpolatedTime = double.NaN;
		currentState = null;
		finalized = false;
		this.forward = true;
		this.dirtyState = true;
		primaryMapper = null;
		secondaryMappers = null;
		allocateInterpolatedArrays(-1);
	  }

	  /// <summary>
	  /// Simple constructor. </summary>
	  /// <param name="y"> reference to the integrator array holding the state at
	  /// the end of the step </param>
	  /// <param name="forward"> integration direction indicator </param>
	  /// <param name="primaryMapper"> equations mapper for the primary equations set </param>
	  /// <param name="secondaryMappers"> equations mappers for the secondary equations sets </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractStepInterpolator(final double[] y, final boolean forward, final org.apache.commons.math3.ode.EquationsMapper primaryMapper, final org.apache.commons.math3.ode.EquationsMapper[] secondaryMappers)
	  protected internal AbstractStepInterpolator(double[] y, bool forward, EquationsMapper primaryMapper, EquationsMapper[] secondaryMappers)
	  {

		globalPreviousTime = double.NaN;
		globalCurrentTime = double.NaN;
		softPreviousTime = double.NaN;
		softCurrentTime = double.NaN;
		h = double.NaN;
		interpolatedTime = double.NaN;
		currentState = y;
		finalized = false;
		this.forward = forward;
		this.dirtyState = true;
		this.primaryMapper = primaryMapper;
		this.secondaryMappers = (secondaryMappers == null) ? null : secondaryMappers.clone();
		allocateInterpolatedArrays(y.Length);

	  }

	  /// <summary>
	  /// Copy constructor.
	  /// 
	  /// <p>The copied interpolator should have been finalized before the
	  /// copy, otherwise the copy will not be able to perform correctly
	  /// any derivative computation and will throw a {@link
	  /// NullPointerException} later. Since we don't want this constructor
	  /// to throw the exceptions finalization may involve and since we
	  /// don't want this method to modify the state of the copied
	  /// interpolator, finalization is <strong>not</strong> done
	  /// automatically, it remains under user control.</p>
	  /// 
	  /// <p>The copy is a deep copy: its arrays are separated from the
	  /// original arrays of the instance.</p>
	  /// </summary>
	  /// <param name="interpolator"> interpolator to copy from.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractStepInterpolator(final AbstractStepInterpolator interpolator)
	  protected internal AbstractStepInterpolator(AbstractStepInterpolator interpolator)
	  {

		globalPreviousTime = interpolator.globalPreviousTime;
		globalCurrentTime = interpolator.globalCurrentTime;
		softPreviousTime = interpolator.softPreviousTime;
		softCurrentTime = interpolator.softCurrentTime;
		h = interpolator.h;
		interpolatedTime = interpolator.interpolatedTime;

		if (interpolator.currentState == null)
		{
			currentState = null;
			primaryMapper = null;
			secondaryMappers = null;
			allocateInterpolatedArrays(-1);
		}
		else
		{
		  currentState = interpolator.currentState.clone();
		  interpolatedState = interpolator.interpolatedState.clone();
		  interpolatedDerivatives = interpolator.interpolatedDerivatives.clone();
		  interpolatedPrimaryState = interpolator.interpolatedPrimaryState.clone();
		  interpolatedPrimaryDerivatives = interpolator.interpolatedPrimaryDerivatives.clone();
		  interpolatedSecondaryState = new double[interpolator.interpolatedSecondaryState.Length][];
		  interpolatedSecondaryDerivatives = new double[interpolator.interpolatedSecondaryDerivatives.Length][];
		  for (int i = 0; i < interpolatedSecondaryState.Length; ++i)
		  {
			  interpolatedSecondaryState[i] = interpolator.interpolatedSecondaryState[i].clone();
			  interpolatedSecondaryDerivatives[i] = interpolator.interpolatedSecondaryDerivatives[i].clone();
		  }
		}

		finalized = interpolator.finalized;
		forward = interpolator.forward;
		dirtyState = interpolator.dirtyState;
		primaryMapper = interpolator.primaryMapper;
		secondaryMappers = (interpolator.secondaryMappers == null) ? null : interpolator.secondaryMappers.clone();

	  }

	  /// <summary>
	  /// Allocate the various interpolated states arrays. </summary>
	  /// <param name="dimension"> total dimension (negative if arrays should be set to null) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void allocateInterpolatedArrays(final int dimension)
	  private void allocateInterpolatedArrays(int dimension)
	  {
		  if (dimension < 0)
		  {
			  interpolatedState = null;
			  interpolatedDerivatives = null;
			  interpolatedPrimaryState = null;
			  interpolatedPrimaryDerivatives = null;
			  interpolatedSecondaryState = null;
			  interpolatedSecondaryDerivatives = null;
		  }
		  else
		  {
			  interpolatedState = new double[dimension];
			  interpolatedDerivatives = new double[dimension];
			  interpolatedPrimaryState = new double[primaryMapper.Dimension];
			  interpolatedPrimaryDerivatives = new double[primaryMapper.Dimension];
			  if (secondaryMappers == null)
			  {
				  interpolatedSecondaryState = null;
				  interpolatedSecondaryDerivatives = null;
			  }
			  else
			  {
				  interpolatedSecondaryState = new double[secondaryMappers.Length][];
				  interpolatedSecondaryDerivatives = new double[secondaryMappers.Length][];
				  for (int i = 0; i < secondaryMappers.Length; ++i)
				  {
					  interpolatedSecondaryState[i] = new double[secondaryMappers[i].Dimension];
					  interpolatedSecondaryDerivatives[i] = new double[secondaryMappers[i].Dimension];
				  }
			  }
		  }
	  }

	  /// <summary>
	  /// Reinitialize the instance </summary>
	  /// <param name="y"> reference to the integrator array holding the state at the end of the step </param>
	  /// <param name="isForward"> integration direction indicator </param>
	  /// <param name="primary"> equations mapper for the primary equations set </param>
	  /// <param name="secondary"> equations mappers for the secondary equations sets </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void reinitialize(final double[] y, final boolean isForward, final org.apache.commons.math3.ode.EquationsMapper primary, final org.apache.commons.math3.ode.EquationsMapper[] secondary)
	  protected internal virtual void reinitialize(double[] y, bool isForward, EquationsMapper primary, EquationsMapper[] secondary)
	  {

		globalPreviousTime = double.NaN;
		globalCurrentTime = double.NaN;
		softPreviousTime = double.NaN;
		softCurrentTime = double.NaN;
		h = double.NaN;
		interpolatedTime = double.NaN;
		currentState = y;
		finalized = false;
		this.forward = isForward;
		this.dirtyState = true;
		this.primaryMapper = primary;
		this.secondaryMappers = secondary.clone();
		allocateInterpolatedArrays(y.Length);

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public StepInterpolator copy() throws org.apache.commons.math3.exception.MaxCountExceededException
	   public virtual StepInterpolator copy()
	   {

		 // finalize the step before performing copy
		 finalizeStep();

		 // create the new independent instance
		 return doCopy();

	   }

	   /// <summary>
	   /// Really copy the finalized instance.
	   /// <p>This method is called by <seealso cref="#copy()"/> after the
	   /// step has been finalized. It must perform a deep copy
	   /// to have an new instance completely independent for the
	   /// original instance. </summary>
	   /// <returns> a copy of the finalized instance </returns>
	   protected internal abstract StepInterpolator doCopy();

	  /// <summary>
	  /// Shift one step forward.
	  /// Copy the current time into the previous time, hence preparing the
	  /// interpolator for future calls to <seealso cref="#storeTime storeTime"/>
	  /// </summary>
	  public virtual void shift()
	  {
		globalPreviousTime = globalCurrentTime;
		softPreviousTime = globalPreviousTime;
		softCurrentTime = globalCurrentTime;
	  }

	  /// <summary>
	  /// Store the current step time. </summary>
	  /// <param name="t"> current time </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void storeTime(final double t)
	  public virtual void storeTime(double t)
	  {

		globalCurrentTime = t;
		softCurrentTime = globalCurrentTime;
		h = globalCurrentTime - globalPreviousTime;
		InterpolatedTime = t;

		// the step is not finalized anymore
		finalized = false;

	  }

	  /// <summary>
	  /// Restrict step range to a limited part of the global step.
	  /// <p>
	  /// This method can be used to restrict a step and make it appear
	  /// as if the original step was smaller. Calling this method
	  /// <em>only</em> changes the value returned by <seealso cref="#getPreviousTime()"/>,
	  /// it does not change any other property
	  /// </p> </summary>
	  /// <param name="softPreviousTime"> start of the restricted step
	  /// @since 2.2 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setSoftPreviousTime(final double softPreviousTime)
	  public virtual double SoftPreviousTime
	  {
		  set
		  {
			  this.softPreviousTime = value;
		  }
	  }

	  /// <summary>
	  /// Restrict step range to a limited part of the global step.
	  /// <p>
	  /// This method can be used to restrict a step and make it appear
	  /// as if the original step was smaller. Calling this method
	  /// <em>only</em> changes the value returned by <seealso cref="#getCurrentTime()"/>,
	  /// it does not change any other property
	  /// </p> </summary>
	  /// <param name="softCurrentTime"> end of the restricted step
	  /// @since 2.2 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setSoftCurrentTime(final double softCurrentTime)
	  public virtual double SoftCurrentTime
	  {
		  set
		  {
			  this.softCurrentTime = value;
		  }
	  }

	  /// <summary>
	  /// Get the previous global grid point time. </summary>
	  /// <returns> previous global grid point time </returns>
	  public virtual double GlobalPreviousTime
	  {
		  get
		  {
			return globalPreviousTime;
		  }
	  }

	  /// <summary>
	  /// Get the current global grid point time. </summary>
	  /// <returns> current global grid point time </returns>
	  public virtual double GlobalCurrentTime
	  {
		  get
		  {
			return globalCurrentTime;
		  }
	  }

	  /// <summary>
	  /// Get the previous soft grid point time. </summary>
	  /// <returns> previous soft grid point time </returns>
	  /// <seealso cref= #setSoftPreviousTime(double) </seealso>
	  public virtual double PreviousTime
	  {
		  get
		  {
			return softPreviousTime;
		  }
	  }

	  /// <summary>
	  /// Get the current soft grid point time. </summary>
	  /// <returns> current soft grid point time </returns>
	  /// <seealso cref= #setSoftCurrentTime(double) </seealso>
	  public virtual double CurrentTime
	  {
		  get
		  {
			return softCurrentTime;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public virtual double InterpolatedTime
	  {
		  get
		  {
			return interpolatedTime;
		  }
		  set
		  {
			  interpolatedTime = value;
			  dirtyState = true;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setInterpolatedTime(final double time)

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public virtual bool Forward
	  {
		  get
		  {
			return forward;
		  }
	  }

	  /// <summary>
	  /// Compute the state and derivatives at the interpolated time.
	  /// This is the main processing method that should be implemented by
	  /// the derived classes to perform the interpolation. </summary>
	  /// <param name="theta"> normalized interpolation abscissa within the step
	  /// (theta is zero at the previous time step and one at the current time step) </param>
	  /// <param name="oneMinusThetaH"> time gap between the interpolated time and
	  /// the current time </param>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected abstract void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH) throws org.apache.commons.math3.exception.MaxCountExceededException;
	  protected internal abstract void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH);

	  /// <summary>
	  /// Lazy evaluation of complete interpolated state. </summary>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void evaluateCompleteInterpolatedState() throws org.apache.commons.math3.exception.MaxCountExceededException
	  private void evaluateCompleteInterpolatedState()
	  {
		  // lazy evaluation of the state
		  if (dirtyState)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oneMinusThetaH = globalCurrentTime - interpolatedTime;
			  double oneMinusThetaH = globalCurrentTime - interpolatedTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double theta = (h == 0) ? 0 : (h - oneMinusThetaH) / h;
			  double theta = (h == 0) ? 0 : (h - oneMinusThetaH) / h;
			  computeInterpolatedStateAndDerivatives(theta, oneMinusThetaH);
			  dirtyState = false;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedState() throws org.apache.commons.math3.exception.MaxCountExceededException
	  public virtual double[] InterpolatedState
	  {
		  get
		  {
			  evaluateCompleteInterpolatedState();
			  primaryMapper.extractEquationData(interpolatedState, interpolatedPrimaryState);
			  return interpolatedPrimaryState;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedDerivatives() throws org.apache.commons.math3.exception.MaxCountExceededException
	  public virtual double[] InterpolatedDerivatives
	  {
		  get
		  {
			  evaluateCompleteInterpolatedState();
			  primaryMapper.extractEquationData(interpolatedDerivatives, interpolatedPrimaryDerivatives);
			  return interpolatedPrimaryDerivatives;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedSecondaryState(final int index) throws org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual double[] getInterpolatedSecondaryState(int index)
	  {
		  evaluateCompleteInterpolatedState();
		  secondaryMappers[index].extractEquationData(interpolatedState, interpolatedSecondaryState[index]);
		  return interpolatedSecondaryState[index];
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedSecondaryDerivatives(final int index) throws org.apache.commons.math3.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public virtual double[] getInterpolatedSecondaryDerivatives(int index)
	  {
		  evaluateCompleteInterpolatedState();
		  secondaryMappers[index].extractEquationData(interpolatedDerivatives, interpolatedSecondaryDerivatives[index]);
		  return interpolatedSecondaryDerivatives[index];
	  }

	  /// <summary>
	  /// Finalize the step.
	  /// 
	  /// <p>Some embedded Runge-Kutta integrators need fewer functions
	  /// evaluations than their counterpart step interpolators. These
	  /// interpolators should perform the last evaluations they need by
	  /// themselves only if they need them. This method triggers these
	  /// extra evaluations. It can be called directly by the user step
	  /// handler and it is called automatically if {@link
	  /// #setInterpolatedTime} is called.</p>
	  /// 
	  /// <p>Once this method has been called, <strong>no</strong> other
	  /// evaluation will be performed on this step. If there is a need to
	  /// have some side effects between the step handler and the
	  /// differential equations (for example update some data in the
	  /// equations once the step has been done), it is advised to call
	  /// this method explicitly from the step handler before these side
	  /// effects are set up. If the step handler induces no side effect,
	  /// then this method can safely be ignored, it will be called
	  /// transparently as needed.</p>
	  /// 
	  /// <p><strong>Warning</strong>: since the step interpolator provided
	  /// to the step handler as a parameter of the {@link
	  /// StepHandler#handleStep handleStep} is valid only for the duration
	  /// of the <seealso cref="StepHandler#handleStep handleStep"/> call, one cannot
	  /// simply store a reference and reuse it later. One should first
	  /// finalize the instance, then copy this finalized instance into a
	  /// new object that can be kept.</p>
	  /// 
	  /// <p>This method calls the protected <code>doFinalize</code> method
	  /// if it has never been called during this step and set a flag
	  /// indicating that it has been called once. It is the <code>
	  /// doFinalize</code> method which should perform the evaluations.
	  /// This wrapping prevents from calling <code>doFinalize</code> several
	  /// times and hence evaluating the differential equations too often.
	  /// Therefore, subclasses are not allowed not reimplement it, they
	  /// should rather reimplement <code>doFinalize</code>.</p>
	  /// </summary>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded
	  ///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final void finalizeStep() throws org.apache.commons.math3.exception.MaxCountExceededException
	  public void finalizeStep()
	  {
		if (!finalized)
		{
		  doFinalize();
		  finalized = true;
		}
	  }

	  /// <summary>
	  /// Really finalize the step.
	  /// The default implementation of this method does nothing. </summary>
	  /// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void doFinalize() throws org.apache.commons.math3.exception.MaxCountExceededException
	  protected internal virtual void doFinalize()
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void writeExternal(java.io.ObjectOutput out) throws java.io.IOException;
	  public abstract void writeExternal(ObjectOutput @out);

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void readExternal(java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException;
	  public abstract void readExternal(ObjectInput @in);

	  /// <summary>
	  /// Save the base state of the instance.
	  /// This method performs step finalization if it has not been done
	  /// before. </summary>
	  /// <param name="out"> stream where to save the state </param>
	  /// <exception cref="IOException"> in case of write error </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void writeBaseExternal(final java.io.ObjectOutput out) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  protected internal virtual void writeBaseExternal(ObjectOutput @out)
	  {

		if (currentState == null)
		{
			@out.writeInt(-1);
		}
		else
		{
			@out.writeInt(currentState.Length);
		}
		@out.writeDouble(globalPreviousTime);
		@out.writeDouble(globalCurrentTime);
		@out.writeDouble(softPreviousTime);
		@out.writeDouble(softCurrentTime);
		@out.writeDouble(h);
		@out.writeBoolean(forward);
		@out.writeObject(primaryMapper);
		@out.write(secondaryMappers.Length);
		foreach (EquationsMapper mapper in secondaryMappers)
		{
			@out.writeObject(mapper);
		}

		if (currentState != null)
		{
			for (int i = 0; i < currentState.Length; ++i)
			{
				@out.writeDouble(currentState[i]);
			}
		}

		@out.writeDouble(interpolatedTime);

		// we do not store the interpolated state,
		// it will be recomputed as needed after reading

		try
		{
			// finalize the step (and don't bother saving the now true flag)
			finalizeStep();
		}
		catch (MaxCountExceededException mcee)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.IOException ioe = new java.io.IOException(mcee.getLocalizedMessage());
			IOException ioe = new IOException(mcee.LocalizedMessage);
			ioe.initCause(mcee);
			throw ioe;
		}

	  }

	  /// <summary>
	  /// Read the base state of the instance.
	  /// This method does <strong>neither</strong> set the interpolated
	  /// time nor state. It is up to the derived class to reset it
	  /// properly calling the <seealso cref="#setInterpolatedTime"/> method later,
	  /// once all rest of the object state has been set up properly. </summary>
	  /// <param name="in"> stream where to read the state from </param>
	  /// <returns> interpolated time to be set later by the caller </returns>
	  /// <exception cref="IOException"> in case of read error </exception>
	  /// <exception cref="ClassNotFoundException"> if an equation mapper class
	  /// cannot be found </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double readBaseExternal(final java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  protected internal virtual double readBaseExternal(ObjectInput @in)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = in.readInt();
		int dimension = @in.readInt();
		globalPreviousTime = @in.readDouble();
		globalCurrentTime = @in.readDouble();
		softPreviousTime = @in.readDouble();
		softCurrentTime = @in.readDouble();
		h = @in.readDouble();
		forward = @in.readBoolean();
		primaryMapper = (EquationsMapper) @in.readObject();
		secondaryMappers = new EquationsMapper[@in.read()];
		for (int i = 0; i < secondaryMappers.Length; ++i)
		{
			secondaryMappers[i] = (EquationsMapper) @in.readObject();
		}
		dirtyState = true;

		if (dimension < 0)
		{
			currentState = null;
		}
		else
		{
			currentState = new double[dimension];
			for (int i = 0; i < currentState.Length; ++i)
			{
				currentState[i] = @in.readDouble();
			}
		}

		// we do NOT handle the interpolated time and state here
		interpolatedTime = double.NaN;
		allocateInterpolatedArrays(dimension);

		finalized = true;

		return @in.readDouble();

	  }

	}

}