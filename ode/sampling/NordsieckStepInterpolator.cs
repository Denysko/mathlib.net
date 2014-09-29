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


	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class implements an interpolator for integrators using Nordsieck representation.
	/// 
	/// <p>This interpolator computes dense output around the current point.
	/// The interpolation equation is based on Taylor series formulas.
	/// </summary>
	/// <seealso cref= mathlib.ode.nonstiff.AdamsBashforthIntegrator </seealso>
	/// <seealso cref= mathlib.ode.nonstiff.AdamsMoultonIntegrator
	/// @version $Id: NordsieckStepInterpolator.java 1503712 2013-07-16 13:35:19Z luc $
	/// @since 2.0 </seealso>

	public class NordsieckStepInterpolator : AbstractStepInterpolator
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -7179861704951334960L;

		/// <summary>
		/// State variation. </summary>
		protected internal double[] stateVariation;

		/// <summary>
		/// Step size used in the first scaled derivative and Nordsieck vector. </summary>
		private double scalingH;

		/// <summary>
		/// Reference time for all arrays.
		/// <p>Sometimes, the reference time is the same as previousTime,
		/// sometimes it is the same as currentTime, so we use a separate
		/// field to avoid any confusion.
		/// </p>
		/// </summary>
		private double referenceTime;

		/// <summary>
		/// First scaled derivative. </summary>
		private double[] scaled;

		/// <summary>
		/// Nordsieck vector. </summary>
		private Array2DRowRealMatrix nordsieck;

		/// <summary>
		/// Simple constructor.
		/// This constructor builds an instance that is not usable yet, the
		/// <seealso cref="AbstractStepInterpolator#reinitialize"/> method should be called
		/// before using the instance in order to initialize the internal arrays. This
		/// constructor is used only in order to delay the initialization in
		/// some cases.
		/// </summary>
		public NordsieckStepInterpolator()
		{
		}

		/// <summary>
		/// Copy constructor. </summary>
		/// <param name="interpolator"> interpolator to copy from. The copy is a deep
		/// copy: its arrays are separated from the original arrays of the
		/// instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NordsieckStepInterpolator(final NordsieckStepInterpolator interpolator)
		public NordsieckStepInterpolator(NordsieckStepInterpolator interpolator) : base(interpolator)
		{
			scalingH = interpolator.scalingH;
			referenceTime = interpolator.referenceTime;
			if (interpolator.scaled != null)
			{
				scaled = interpolator.scaled.clone();
			}
			if (interpolator.nordsieck != null)
			{
				nordsieck = new Array2DRowRealMatrix(interpolator.nordsieck.DataRef, true);
			}
			if (interpolator.stateVariation != null)
			{
				stateVariation = interpolator.stateVariation.clone();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override StepInterpolator doCopy()
		{
			return new NordsieckStepInterpolator(this);
		}

		/// <summary>
		/// Reinitialize the instance.
		/// <p>Beware that all arrays <em>must</em> be references to integrator
		/// arrays, in order to ensure proper update without copy.</p> </summary>
		/// <param name="y"> reference to the integrator array holding the state at
		/// the end of the step </param>
		/// <param name="forward"> integration direction indicator </param>
		/// <param name="primaryMapper"> equations mapper for the primary equations set </param>
		/// <param name="secondaryMappers"> equations mappers for the secondary equations sets </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void reinitialize(final double[] y, final boolean forward, final mathlib.ode.EquationsMapper primaryMapper, final mathlib.ode.EquationsMapper[] secondaryMappers)
		public override void reinitialize(double[] y, bool forward, EquationsMapper primaryMapper, EquationsMapper[] secondaryMappers)
		{
			base.reinitialize(y, forward, primaryMapper, secondaryMappers);
			stateVariation = new double[y.Length];
		}

		/// <summary>
		/// Reinitialize the instance.
		/// <p>Beware that all arrays <em>must</em> be references to integrator
		/// arrays, in order to ensure proper update without copy.</p> </summary>
		/// <param name="time"> time at which all arrays are defined </param>
		/// <param name="stepSize"> step size used in the scaled and Nordsieck arrays </param>
		/// <param name="scaledDerivative"> reference to the integrator array holding the first
		/// scaled derivative </param>
		/// <param name="nordsieckVector"> reference to the integrator matrix holding the
		/// Nordsieck vector </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void reinitialize(final double time, final double stepSize, final double[] scaledDerivative, final mathlib.linear.Array2DRowRealMatrix nordsieckVector)
		public virtual void reinitialize(double time, double stepSize, double[] scaledDerivative, Array2DRowRealMatrix nordsieckVector)
		{
			this.referenceTime = time;
			this.scalingH = stepSize;
			this.scaled = scaledDerivative;
			this.nordsieck = nordsieckVector;

			// make sure the state and derivatives will depend on the new arrays
			InterpolatedTime = InterpolatedTime;

		}

		/// <summary>
		/// Rescale the instance.
		/// <p>Since the scaled and Nordsieck arrays are shared with the caller,
		/// this method has the side effect of rescaling this arrays in the caller too.</p> </summary>
		/// <param name="stepSize"> new step size to use in the scaled and Nordsieck arrays </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void rescale(final double stepSize)
		public virtual void rescale(double stepSize)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = stepSize / scalingH;
			double ratio = stepSize / scalingH;
			for (int i = 0; i < scaled.Length; ++i)
			{
				scaled[i] *= ratio;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] nData = nordsieck.getDataRef();
			double[][] nData = nordsieck.DataRef;
			double power = ratio;
			for (int i = 0; i < nData.Length; ++i)
			{
				power *= ratio;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] nDataI = nData[i];
				double[] nDataI = nData[i];
				for (int j = 0; j < nDataI.Length; ++j)
				{
					nDataI[j] *= power;
				}
			}

			scalingH = stepSize;

		}

		/// <summary>
		/// Get the state vector variation from current to interpolated state.
		/// <p>This method is aimed at computing y(t<sub>interpolation</sub>)
		/// -y(t<sub>current</sub>) accurately by avoiding the cancellation errors
		/// that would occur if the subtraction were performed explicitly.</p>
		/// <p>The returned vector is a reference to a reused array, so
		/// it should not be modified and it should be copied if it needs
		/// to be preserved across several calls.</p> </summary>
		/// <returns> state vector at time <seealso cref="#getInterpolatedTime"/> </returns>
		/// <seealso cref= #getInterpolatedDerivatives() </seealso>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getInterpolatedStateVariation() throws mathlib.exception.MaxCountExceededException
		public virtual double[] InterpolatedStateVariation
		{
			get
			{
				// compute and ignore interpolated state
				// to make sure state variation is computed as a side effect
				InterpolatedState;
				return stateVariation;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected void computeInterpolatedStateAndDerivatives(final double theta, final double oneMinusThetaH)
		protected internal override void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = interpolatedTime - referenceTime;
			double x = interpolatedTime - referenceTime;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double normalizedAbscissa = x / scalingH;
			double normalizedAbscissa = x / scalingH;

			Arrays.fill(stateVariation, 0.0);
			Arrays.fill(interpolatedDerivatives, 0.0);

			// apply Taylor formula from high order to low order,
			// for the sake of numerical accuracy
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] nData = nordsieck.getDataRef();
			double[][] nData = nordsieck.DataRef;
			for (int i = nData.Length - 1; i >= 0; --i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int order = i + 2;
				int order = i + 2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] nDataI = nData[i];
				double[] nDataI = nData[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double power = mathlib.util.FastMath.pow(normalizedAbscissa, order);
				double power = FastMath.pow(normalizedAbscissa, order);
				for (int j = 0; j < nDataI.Length; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = nDataI[j] * power;
					double d = nDataI[j] * power;
					stateVariation[j] += d;
					interpolatedDerivatives[j] += order * d;
				}
			}

			for (int j = 0; j < currentState.Length; ++j)
			{
				stateVariation[j] += scaled[j] * normalizedAbscissa;
				interpolatedState[j] = currentState[j] + stateVariation[j];
				interpolatedDerivatives[j] = (interpolatedDerivatives[j] + scaled[j] * normalizedAbscissa) / x;
			}

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
			@out.writeDouble(scalingH);
			@out.writeDouble(referenceTime);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = (currentState == null) ? -1 : currentState.length;
			int n = (currentState == null) ? - 1 : currentState.Length;
			if (scaled == null)
			{
				@out.writeBoolean(false);
			}
			else
			{
				@out.writeBoolean(true);
				for (int j = 0; j < n; ++j)
				{
					@out.writeDouble(scaled[j]);
				}
			}

			if (nordsieck == null)
			{
				@out.writeBoolean(false);
			}
			else
			{
				@out.writeBoolean(true);
				@out.writeObject(nordsieck);
			}

			// we don't save state variation, it will be recomputed

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
			scalingH = @in.readDouble();
			referenceTime = @in.readDouble();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = (currentState == null) ? -1 : currentState.length;
			int n = (currentState == null) ? - 1 : currentState.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasScaled = in.readBoolean();
			bool hasScaled = @in.readBoolean();
			if (hasScaled)
			{
				scaled = new double[n];
				for (int j = 0; j < n; ++j)
				{
					scaled[j] = @in.readDouble();
				}
			}
			else
			{
				scaled = null;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean hasNordsieck = in.readBoolean();
			bool hasNordsieck = @in.readBoolean();
			if (hasNordsieck)
			{
				nordsieck = (Array2DRowRealMatrix) @in.readObject();
			}
			else
			{
				nordsieck = null;
			}

			if (hasScaled && hasNordsieck)
			{
				// we can now set the interpolated time and state
				stateVariation = new double[n];
				InterpolatedTime = t;
			}
			else
			{
				stateVariation = null;
			}

		}

	}

}