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
	/// This class implements a step interpolator for the classical fourth
	/// order Runge-Kutta integrator.
	/// 
	/// <p>This interpolator allows to compute dense output inside the last
	/// step computed. The interpolation equation is consistent with the
	/// integration scheme :
	/// <ul>
	///   <li>Using reference point at step start:<br>
	///   y(t<sub>n</sub> + &theta; h) = y (t<sub>n</sub>)
	///                    + &theta; (h/6) [  (6 - 9 &theta; + 4 &theta;<sup>2</sup>) y'<sub>1</sub>
	///                                     + (    6 &theta; - 4 &theta;<sup>2</sup>) (y'<sub>2</sub> + y'<sub>3</sub>)
	///                                     + (   -3 &theta; + 4 &theta;<sup>2</sup>) y'<sub>4</sub>
	///                                    ]
	///   </li>
	///   <li>Using reference point at step end:<br>
	///   y(t<sub>n</sub> + &theta; h) = y (t<sub>n</sub> + h)
	///                    + (1 - &theta;) (h/6) [ (-4 &theta;^2 + 5 &theta; - 1) y'<sub>1</sub>
	///                                          +(4 &theta;^2 - 2 &theta; - 2) (y'<sub>2</sub> + y'<sub>3</sub>)
	///                                          -(4 &theta;^2 +   &theta; + 1) y'<sub>4</sub>
	///                                        ]
	///   </li>
	/// </ul>
	/// </p>
	/// 
	/// where &theta; belongs to [0 ; 1] and where y'<sub>1</sub> to y'<sub>4</sub> are the four
	/// evaluations of the derivatives already computed during the
	/// step.</p>
	/// </summary>
	/// <seealso cref= ClassicalRungeKuttaIntegrator
	/// @version $Id: ClassicalRungeKuttaStepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	internal class ClassicalRungeKuttaStepInterpolator : RungeKuttaStepInterpolator
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20111120L;

		/// <summary>
		/// Simple constructor.
		/// This constructor builds an instance that is not usable yet, the
		/// <seealso cref="RungeKuttaStepInterpolator#reinitialize"/> method should be
		/// called before using the instance in order to initialize the
		/// internal arrays. This constructor is used only in order to delay
		/// the initialization in some cases. The <seealso cref="RungeKuttaIntegrator"/>
		/// class uses the prototyping design pattern to create the step
		/// interpolators by cloning an uninitialized model and latter initializing
		/// the copy.
		/// </summary>
		public ClassicalRungeKuttaStepInterpolator()
		{
		}

		/// <summary>
		/// Copy constructor. </summary>
		/// <param name="interpolator"> interpolator to copy from. The copy is a deep
		/// copy: its arrays are separated from the original arrays of the
		/// instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ClassicalRungeKuttaStepInterpolator(final ClassicalRungeKuttaStepInterpolator interpolator)
		public ClassicalRungeKuttaStepInterpolator(ClassicalRungeKuttaStepInterpolator interpolator) : base(interpolator)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override StepInterpolator doCopy()
		{
			return new ClassicalRungeKuttaStepInterpolator(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected void computeInterpolatedStateAndDerivatives(final double theta, final double oneMinusThetaH)
		protected internal override void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oneMinusTheta = 1 - theta;
			double oneMinusTheta = 1 - theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oneMinus2Theta = 1 - 2 * theta;
			double oneMinus2Theta = 1 - 2 * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeffDot1 = oneMinusTheta * oneMinus2Theta;
			double coeffDot1 = oneMinusTheta * oneMinus2Theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeffDot23 = 2 * theta * oneMinusTheta;
			double coeffDot23 = 2 * theta * oneMinusTheta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeffDot4 = -theta * oneMinus2Theta;
			double coeffDot4 = -theta * oneMinus2Theta;
			if ((previousState != null) && (theta <= 0.5))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fourTheta2 = 4 * theta * theta;
				double fourTheta2 = 4 * theta * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = theta * h / 6.0;
				double s = theta * h / 6.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff1 = s * (6 - 9 * theta + fourTheta2);
				double coeff1 = s * (6 - 9 * theta + fourTheta2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff23 = s * (6 * theta - fourTheta2);
				double coeff23 = s * (6 * theta - fourTheta2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff4 = s * (-3 * theta + fourTheta2);
				double coeff4 = s * (-3 * theta + fourTheta2);
				for (int i = 0; i < interpolatedState.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot1 = yDotK[0][i];
					double yDot1 = yDotK[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot23 = yDotK[1][i] + yDotK[2][i];
					double yDot23 = yDotK[1][i] + yDotK[2][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot4 = yDotK[3][i];
					double yDot4 = yDotK[3][i];
					interpolatedState[i] = previousState[i] + coeff1 * yDot1 + coeff23 * yDot23 + coeff4 * yDot4;
					interpolatedDerivatives[i] = coeffDot1 * yDot1 + coeffDot23 * yDot23 + coeffDot4 * yDot4;
				}
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fourTheta = 4 * theta;
				double fourTheta = 4 * theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = oneMinusThetaH / 6.0;
				double s = oneMinusThetaH / 6.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff1 = s * ((-fourTheta + 5) * theta - 1);
				double coeff1 = s * ((-fourTheta + 5) * theta - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff23 = s * ((fourTheta - 2) * theta - 2);
				double coeff23 = s * ((fourTheta - 2) * theta - 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double coeff4 = s * ((-fourTheta - 1) * theta - 1);
				double coeff4 = s * ((-fourTheta - 1) * theta - 1);
				for (int i = 0; i < interpolatedState.Length; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot1 = yDotK[0][i];
					double yDot1 = yDotK[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot23 = yDotK[1][i] + yDotK[2][i];
					double yDot23 = yDotK[1][i] + yDotK[2][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yDot4 = yDotK[3][i];
					double yDot4 = yDotK[3][i];
					interpolatedState[i] = currentState[i] + coeff1 * yDot1 + coeff23 * yDot23 + coeff4 * yDot4;
					interpolatedDerivatives[i] = coeffDot1 * yDot1 + coeffDot23 * yDot23 + coeffDot4 * yDot4;
				}
			}

		}

	}

}