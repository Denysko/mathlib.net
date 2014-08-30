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

namespace org.apache.commons.math3.ode.nonstiff
{

	using StepInterpolator = org.apache.commons.math3.ode.sampling.StepInterpolator;

	/// <summary>
	/// This class implements a linear interpolator for step.
	/// 
	/// <p>This interpolator computes dense output inside the last
	/// step computed. The interpolation equation is consistent with the
	/// integration scheme :
	/// <ul>
	///   <li>Using reference point at step start:<br>
	///     y(t<sub>n</sub> + &theta; h) = y (t<sub>n</sub>) + &theta; h y'
	///   </li>
	///   <li>Using reference point at step end:<br>
	///     y(t<sub>n</sub> + &theta; h) = y (t<sub>n</sub> + h) - (1-&theta;) h y'
	///   </li>
	/// </ul>
	/// </p>
	/// 
	/// where &theta; belongs to [0 ; 1] and where y' is the evaluation of
	/// the derivatives already computed during the step.</p>
	/// </summary>
	/// <seealso cref= EulerIntegrator
	/// @version $Id: EulerStepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	internal class EulerStepInterpolator : RungeKuttaStepInterpolator
	{

	  /// <summary>
	  /// Serializable version identifier. </summary>
	  private const long serialVersionUID = 20111120L;

	  /// <summary>
	  /// Simple constructor.
	  /// This constructor builds an instance that is not usable yet, the
	  /// {@link
	  /// org.apache.commons.math3.ode.sampling.AbstractStepInterpolator#reinitialize}
	  /// method should be called before using the instance in order to
	  /// initialize the internal arrays. This constructor is used only
	  /// in order to delay the initialization in some cases. The {@link
	  /// RungeKuttaIntegrator} class uses the prototyping design pattern
	  /// to create the step interpolators by cloning an uninitialized model
	  /// and later initializing the copy.
	  /// </summary>
	  public EulerStepInterpolator()
	  {
	  }

	  /// <summary>
	  /// Copy constructor. </summary>
	  /// <param name="interpolator"> interpolator to copy from. The copy is a deep
	  /// copy: its arrays are separated from the original arrays of the
	  /// instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EulerStepInterpolator(final EulerStepInterpolator interpolator)
	  public EulerStepInterpolator(EulerStepInterpolator interpolator) : base(interpolator)
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  protected internal override StepInterpolator doCopy()
	  {
		return new EulerStepInterpolator(this);
	  }


	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected void computeInterpolatedStateAndDerivatives(final double theta, final double oneMinusThetaH)
	  protected internal override void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH)
	  {
		  if ((previousState != null) && (theta <= 0.5))
		  {
			  for (int i = 0; i < interpolatedState.Length; ++i)
			  {
				  interpolatedState[i] = previousState[i] + theta * h * yDotK[0][i];
			  }
			  Array.Copy(yDotK[0], 0, interpolatedDerivatives, 0, interpolatedDerivatives.Length);
		  }
		  else
		  {
			  for (int i = 0; i < interpolatedState.Length; ++i)
			  {
				  interpolatedState[i] = currentState[i] - oneMinusThetaH * yDotK[0][i];
			  }
			  Array.Copy(yDotK[0], 0, interpolatedDerivatives, 0, interpolatedDerivatives.Length);
		  }

	  }

	}

}