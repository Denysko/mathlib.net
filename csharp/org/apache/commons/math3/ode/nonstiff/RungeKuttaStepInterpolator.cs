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

namespace org.apache.commons.math3.ode.nonstiff
{


	using AbstractStepInterpolator = org.apache.commons.math3.ode.sampling.AbstractStepInterpolator;

	/// <summary>
	/// This class represents an interpolator over the last step during an
	/// ODE integration for Runge-Kutta and embedded Runge-Kutta integrators.
	/// </summary>
	/// <seealso cref= RungeKuttaIntegrator </seealso>
	/// <seealso cref= EmbeddedRungeKuttaIntegrator
	/// 
	/// @version $Id: RungeKuttaStepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	internal abstract class RungeKuttaStepInterpolator : AbstractStepInterpolator
	{

		/// <summary>
		/// Previous state. </summary>
		protected internal double[] previousState;

		/// <summary>
		/// Slopes at the intermediate points </summary>
		protected internal double[][] yDotK;

		/// <summary>
		/// Reference to the integrator. </summary>
		protected internal AbstractIntegrator integrator;

	  /// <summary>
	  /// Simple constructor.
	  /// This constructor builds an instance that is not usable yet, the
	  /// <seealso cref="#reinitialize"/> method should be called before using the
	  /// instance in order to initialize the internal arrays. This
	  /// constructor is used only in order to delay the initialization in
	  /// some cases. The <seealso cref="RungeKuttaIntegrator"/> and {@link
	  /// EmbeddedRungeKuttaIntegrator} classes use the prototyping design
	  /// pattern to create the step interpolators by cloning an
	  /// uninitialized model and latter initializing the copy.
	  /// </summary>
	  protected internal RungeKuttaStepInterpolator()
	  {
		previousState = null;
		yDotK = null;
		integrator = null;
	  }

	  /// <summary>
	  /// Copy constructor.
	  /// 
	  /// <p>The copied interpolator should have been finalized before the
	  /// copy, otherwise the copy will not be able to perform correctly any
	  /// interpolation and will throw a <seealso cref="NullPointerException"/>
	  /// later. Since we don't want this constructor to throw the
	  /// exceptions finalization may involve and since we don't want this
	  /// method to modify the state of the copied interpolator,
	  /// finalization is <strong>not</strong> done automatically, it
	  /// remains under user control.</p>
	  /// 
	  /// <p>The copy is a deep copy: its arrays are separated from the
	  /// original arrays of the instance.</p>
	  /// </summary>
	  /// <param name="interpolator"> interpolator to copy from.
	  ///  </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public RungeKuttaStepInterpolator(final RungeKuttaStepInterpolator interpolator)
	  public RungeKuttaStepInterpolator(RungeKuttaStepInterpolator interpolator) : base(interpolator)
	  {


		if (interpolator.currentState != null)
		{

		  previousState = interpolator.previousState.clone();

		  yDotK = new double[interpolator.yDotK.Length][];
		  for (int k = 0; k < interpolator.yDotK.Length; ++k)
		  {
			yDotK[k] = interpolator.yDotK[k].clone();
		  }

		}
		else
		{
		  previousState = null;
		  yDotK = null;
		}

		// we cannot keep any reference to the equations in the copy
		// the interpolator should have been finalized before
		integrator = null;

	  }

	  /// <summary>
	  /// Reinitialize the instance
	  /// <p>Some Runge-Kutta integrators need fewer functions evaluations
	  /// than their counterpart step interpolators. So the interpolator
	  /// should perform the last evaluations they need by themselves. The
	  /// <seealso cref="RungeKuttaIntegrator RungeKuttaIntegrator"/> and {@link
	  /// EmbeddedRungeKuttaIntegrator EmbeddedRungeKuttaIntegrator}
	  /// abstract classes call this method in order to let the step
	  /// interpolator perform the evaluations it needs. These evaluations
	  /// will be performed during the call to <code>doFinalize</code> if
	  /// any, i.e. only if the step handler either calls the {@link
	  /// AbstractStepInterpolator#finalizeStep finalizeStep} method or the
	  /// {@link AbstractStepInterpolator#getInterpolatedState
	  /// getInterpolatedState} method (for an interpolator which needs a
	  /// finalization) or if it clones the step interpolator.</p> </summary>
	  /// <param name="rkIntegrator"> integrator being used </param>
	  /// <param name="y"> reference to the integrator array holding the state at
	  /// the end of the step </param>
	  /// <param name="yDotArray"> reference to the integrator array holding all the
	  /// intermediate slopes </param>
	  /// <param name="forward"> integration direction indicator </param>
	  /// <param name="primaryMapper"> equations mapper for the primary equations set </param>
	  /// <param name="secondaryMappers"> equations mappers for the secondary equations sets </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void reinitialize(final org.apache.commons.math3.ode.AbstractIntegrator rkIntegrator, final double[] y, final double[][] yDotArray, final boolean forward, final org.apache.commons.math3.ode.EquationsMapper primaryMapper, final org.apache.commons.math3.ode.EquationsMapper[] secondaryMappers)
	  public virtual void reinitialize(AbstractIntegrator rkIntegrator, double[] y, double[][] yDotArray, bool forward, EquationsMapper primaryMapper, EquationsMapper[] secondaryMappers)
	  {
		reinitialize(y, forward, primaryMapper, secondaryMappers);
		this.previousState = null;
		this.yDotK = yDotArray;
		this.integrator = rkIntegrator;
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public override void shift()
	  {
		previousState = currentState.clone();
		base.shift();
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeExternal(final java.io.ObjectOutput out) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public override void writeExternal(ObjectOutput @out)
	  {

		// save the state of the base class
		writeBaseExternal(@out);

		// save the local attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = (currentState == null) ? -1 : currentState.length;
		int n = (currentState == null) ? - 1 : currentState.Length;
		for (int i = 0; i < n; ++i)
		{
		  @out.writeDouble(previousState[i]);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kMax = (yDotK == null) ? -1 : yDotK.length;
		int kMax = (yDotK == null) ? - 1 : yDotK.Length;
		@out.writeInt(kMax);
		for (int k = 0; k < kMax; ++k)
		{
		  for (int i = 0; i < n; ++i)
		  {
			@out.writeDouble(yDotK[k][i]);
		  }
		}

		// we do not save any reference to the equations

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void readExternal(final java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public override void readExternal(ObjectInput @in)
	  {

		// read the base class
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = readBaseExternal(in);
		double t = readBaseExternal(@in);

		// read the local attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = (currentState == null) ? -1 : currentState.length;
		int n = (currentState == null) ? - 1 : currentState.Length;
		if (n < 0)
		{
		  previousState = null;
		}
		else
		{
		  previousState = new double[n];
		  for (int i = 0; i < n; ++i)
		  {
			previousState[i] = @in.readDouble();
		  }
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kMax = in.readInt();
		int kMax = @in.readInt();
		yDotK = (kMax < 0) ? null : new double[kMax][];
		for (int k = 0; k < kMax; ++k)
		{
		  yDotK[k] = (n < 0) ? null : new double[n];
		  for (int i = 0; i < n; ++i)
		  {
			yDotK[k][i] = @in.readDouble();
		  }
		}

		integrator = null;

		if (currentState != null)
		{
			// we can now set the interpolated time and state
			InterpolatedTime = t;
		}
		else
		{
			interpolatedTime = t;
		}

	  }

	}

}