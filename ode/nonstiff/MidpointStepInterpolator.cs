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

namespace mathlib.ode.nonstiff
{

	using StepInterpolator = mathlib.ode.sampling.StepInterpolator;

	/// <summary>
	/// This class implements a step interpolator for second order
	/// Runge-Kutta integrator.
	/// 
	/// <p>This interpolator computes dense output inside the last
	/// step computed. The interpolation equation is consistent with the
	/// integration scheme :
	/// <ul>
	///   <li>Using reference point at step start:<br>
	///   y(t<sub>n</sub> + &theta; h) = y (t<sub>n</sub>) + &theta; h [(1 - &theta;) y'<sub>1</sub> + &theta; y'<sub>2</sub>]
	///   </li>
	///   <li>Using reference point at step end:<br>
	///   y(t<sub>n</sub> + &theta; h) = y (t<sub>n</sub> + h) + (1-&theta;) h [&theta; y'<sub>1</sub> - (1+&theta;) y'<sub>2</sub>]
	///   </li>
	/// </ul>
	/// </p>
	/// 
	/// where &theta; belongs to [0 ; 1] and where y'<sub>1</sub> and y'<sub>2</sub> are the two
	/// evaluations of the derivatives already computed during the
	/// step.</p>
	/// </summary>
	/// <seealso cref= MidpointIntegrator
	/// @version $Id: MidpointStepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	internal class MidpointStepInterpolator : RungeKuttaStepInterpolator
	{

	  /// <summary>
	  /// Serializable version identifier </summary>
	  private const long serialVersionUID = 20111120L;

	  /// <summary>
	  /// Simple constructor.
	  /// This constructor builds an instance that is not usable yet, the
	  /// {@link
	  /// mathlib.ode.sampling.AbstractStepInterpolator#reinitialize}
	  /// method should be called before using the instance in order to
	  /// initialize the internal arrays. This constructor is used only
	  /// in order to delay the initialization in some cases. The {@link
	  /// RungeKuttaIntegrator} class uses the prototyping design pattern
	  /// to create the step interpolators by cloning an uninitialized model
	  /// and later initializing the copy.
	  /// </summary>
	  public MidpointStepInterpolator()
	  {
	  }

	  /// <summary>
	  /// Copy constructor. </summary>
	  /// <param name="interpolator"> interpolator to copy from. The copy is a deep
	  /// copy: its arrays are separated from the original arrays of the
	  /// instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MidpointStepInterpolator(final MidpointStepInterpolator interpolator)
	  public MidpointStepInterpolator(MidpointStepInterpolator interpolator) : base(interpolator)
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  protected internal override StepInterpolator doCopy()
	  {
		return new MidpointStepInterpolator(this);
	  }


	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected void computeInterpolatedStateAndDerivatives(final double theta, final double oneMinusThetaH)
	  protected internal override void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeffDot2 = 2 * theta;
		double coeffDot2 = 2 * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeffDot1 = 1 - coeffDot2;
		double coeffDot1 = 1 - coeffDot2;

		if ((previousState != null) && (theta <= 0.5))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff1 = theta * oneMinusThetaH;
			double coeff1 = theta * oneMinusThetaH;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff2 = theta * theta * h;
			double coeff2 = theta * theta * h;
			for (int i = 0; i < interpolatedState.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot1 = yDotK[0][i];
				double yDot1 = yDotK[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot2 = yDotK[1][i];
				double yDot2 = yDotK[1][i];
				interpolatedState[i] = previousState[i] + coeff1 * yDot1 + coeff2 * yDot2;
				interpolatedDerivatives[i] = coeffDot1 * yDot1 + coeffDot2 * yDot2;
			}
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff1 = oneMinusThetaH * theta;
			double coeff1 = oneMinusThetaH * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff2 = oneMinusThetaH * (1.0 + theta);
			double coeff2 = oneMinusThetaH * (1.0 + theta);
			for (int i = 0; i < interpolatedState.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot1 = yDotK[0][i];
				double yDot1 = yDotK[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot2 = yDotK[1][i];
				double yDot2 = yDotK[1][i];
				interpolatedState[i] = currentState[i] + coeff1 * yDot1 - coeff2 * yDot2;
				interpolatedDerivatives[i] = coeffDot1 * yDot1 + coeffDot2 * yDot2;
			}
		}

	  }

	}

}