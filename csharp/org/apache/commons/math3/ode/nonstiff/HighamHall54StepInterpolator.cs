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
	/// This class represents an interpolator over the last step during an
	/// ODE integration for the 5(4) Higham and Hall integrator.
	/// </summary>
	/// <seealso cref= HighamHall54Integrator
	/// 
	/// @version $Id: HighamHall54StepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	internal class HighamHall54StepInterpolator : RungeKuttaStepInterpolator
	{

	  /// <summary>
	  /// Serializable version identifier </summary>
	  private const long serialVersionUID = 20111120L;

	  /// <summary>
	  /// Simple constructor.
	  /// This constructor builds an instance that is not usable yet, the
	  /// {@link
	  /// org.apache.commons.math3.ode.sampling.AbstractStepInterpolator#reinitialize}
	  /// method should be called before using the instance in order to
	  /// initialize the internal arrays. This constructor is used only
	  /// in order to delay the initialization in some cases. The {@link
	  /// EmbeddedRungeKuttaIntegrator} uses the prototyping design pattern
	  /// to create the step interpolators by cloning an uninitialized model
	  /// and later initializing the copy.
	  /// </summary>
	  public HighamHall54StepInterpolator() : base()
	  {
	  }

	  /// <summary>
	  /// Copy constructor. </summary>
	  /// <param name="interpolator"> interpolator to copy from. The copy is a deep
	  /// copy: its arrays are separated from the original arrays of the
	  /// instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public HighamHall54StepInterpolator(final HighamHall54StepInterpolator interpolator)
	  public HighamHall54StepInterpolator(HighamHall54StepInterpolator interpolator) : base(interpolator)
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  protected internal override StepInterpolator doCopy()
	  {
		return new HighamHall54StepInterpolator(this);
	  }


	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected void computeInterpolatedStateAndDerivatives(final double theta, final double oneMinusThetaH)
	  protected internal override void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bDot0 = 1 + theta * (-15.0/2.0 + theta * (16.0 - 10.0 * theta));
		double bDot0 = 1 + theta * (-15.0 / 2.0 + theta * (16.0 - 10.0 * theta));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bDot2 = theta * (459.0/16.0 + theta * (-729.0/8.0 + 135.0/2.0 * theta));
		double bDot2 = theta * (459.0 / 16.0 + theta * (-729.0 / 8.0 + 135.0 / 2.0 * theta));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bDot3 = theta * (-44.0 + theta * (152.0 - 120.0 * theta));
		double bDot3 = theta * (-44.0 + theta * (152.0 - 120.0 * theta));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bDot4 = theta * (375.0/16.0 + theta * (-625.0/8.0 + 125.0/2.0 * theta));
		double bDot4 = theta * (375.0 / 16.0 + theta * (-625.0 / 8.0 + 125.0 / 2.0 * theta));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bDot5 = theta * 5.0/8.0 * (2 * theta - 1);
		double bDot5 = theta * 5.0 / 8.0 * (2 * theta - 1);

		if ((previousState != null) && (theta <= 0.5))
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hTheta = h * theta;
			double hTheta = h * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b0 = hTheta * (1.0 + theta * (-15.0/4.0 + theta * (16.0/3.0 - 5.0/2.0 * theta)));
			double b0 = hTheta * (1.0 + theta * (-15.0 / 4.0 + theta * (16.0 / 3.0 - 5.0 / 2.0 * theta)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2 = hTheta * (theta * (459.0/32.0 + theta * (-243.0/8.0 + theta * 135.0/8.0)));
			double b2 = hTheta * (theta * (459.0 / 32.0 + theta * (-243.0 / 8.0 + theta * 135.0 / 8.0)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3 = hTheta * (theta * (-22.0 + theta * (152.0/3.0 + theta * -30.0)));
			double b3 = hTheta * (theta * (-22.0 + theta * (152.0 / 3.0 + theta * -30.0)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b4 = hTheta * (theta * (375.0/32.0 + theta * (-625.0/24.0 + theta * 125.0/8.0)));
			double b4 = hTheta * (theta * (375.0 / 32.0 + theta * (-625.0 / 24.0 + theta * 125.0 / 8.0)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b5 = hTheta * (theta * (-5.0/16.0 + theta * 5.0/12.0));
			double b5 = hTheta * (theta * (-5.0 / 16.0 + theta * 5.0 / 12.0));
			for (int i = 0; i < interpolatedState.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot0 = yDotK[0][i];
				double yDot0 = yDotK[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot2 = yDotK[2][i];
				double yDot2 = yDotK[2][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot3 = yDotK[3][i];
				double yDot3 = yDotK[3][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot4 = yDotK[4][i];
				double yDot4 = yDotK[4][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot5 = yDotK[5][i];
				double yDot5 = yDotK[5][i];
				interpolatedState[i] = previousState[i] + b0 * yDot0 + b2 * yDot2 + b3 * yDot3 + b4 * yDot4 + b5 * yDot5;
				interpolatedDerivatives[i] = bDot0 * yDot0 + bDot2 * yDot2 + bDot3 * yDot3 + bDot4 * yDot4 + bDot5 * yDot5;
			}
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double theta2 = theta * theta;
			double theta2 = theta * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b0 = h * (-1.0/12.0 + theta * (1.0 + theta * (-15.0/4.0 + theta * (16.0/3.0 + theta * -5.0/2.0))));
			double b0 = h * (-1.0 / 12.0 + theta * (1.0 + theta * (-15.0 / 4.0 + theta * (16.0 / 3.0 + theta * -5.0 / 2.0))));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b2 = h * (-27.0/32.0 + theta2 * (459.0/32.0 + theta * (-243.0/8.0 + theta * 135.0/8.0)));
			double b2 = h * (-27.0 / 32.0 + theta2 * (459.0 / 32.0 + theta * (-243.0 / 8.0 + theta * 135.0 / 8.0)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b3 = h * (4.0/3.0 + theta2 * (-22.0 + theta * (152.0/3.0 + theta * -30.0)));
			double b3 = h * (4.0 / 3.0 + theta2 * (-22.0 + theta * (152.0 / 3.0 + theta * -30.0)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b4 = h * (-125.0/96.0 + theta2 * (375.0/32.0 + theta * (-625.0/24.0 + theta * 125.0/8.0)));
			double b4 = h * (-125.0 / 96.0 + theta2 * (375.0 / 32.0 + theta * (-625.0 / 24.0 + theta * 125.0 / 8.0)));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double b5 = h * (-5.0/48.0 + theta2 * (-5.0/16.0 + theta * 5.0/12.0));
			double b5 = h * (-5.0 / 48.0 + theta2 * (-5.0 / 16.0 + theta * 5.0 / 12.0));
			for (int i = 0; i < interpolatedState.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot0 = yDotK[0][i];
				double yDot0 = yDotK[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot2 = yDotK[2][i];
				double yDot2 = yDotK[2][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot3 = yDotK[3][i];
				double yDot3 = yDotK[3][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot4 = yDotK[4][i];
				double yDot4 = yDotK[4][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot5 = yDotK[5][i];
				double yDot5 = yDotK[5][i];
				interpolatedState[i] = currentState[i] + b0 * yDot0 + b2 * yDot2 + b3 * yDot3 + b4 * yDot4 + b5 * yDot5;
				interpolatedDerivatives[i] = bDot0 * yDot0 + bDot2 * yDot2 + bDot3 * yDot3 + bDot4 * yDot4 + bDot5 * yDot5;
			}
		}

	  }

	}

}