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


	/// <summary>
	/// This class implements the classical fourth order Runge-Kutta
	/// integrator for Ordinary Differential Equations (it is the most
	/// often used Runge-Kutta method).
	/// 
	/// <p>This method is an explicit Runge-Kutta method, its Butcher-array
	/// is the following one :
	/// <pre>
	///    0  |  0    0    0    0
	///   1/2 | 1/2   0    0    0
	///   1/2 |  0   1/2   0    0
	///    1  |  0    0    1    0
	///       |--------------------
	///       | 1/6  1/3  1/3  1/6
	/// </pre>
	/// </p>
	/// </summary>
	/// <seealso cref= EulerIntegrator </seealso>
	/// <seealso cref= GillIntegrator </seealso>
	/// <seealso cref= MidpointIntegrator </seealso>
	/// <seealso cref= ThreeEighthesIntegrator </seealso>
	/// <seealso cref= LutherIntegrator
	/// @version $Id: ClassicalRungeKuttaIntegrator.java 1588755 2014-04-20 13:30:16Z luc $
	/// @since 1.2 </seealso>

	public class ClassicalRungeKuttaIntegrator : RungeKuttaIntegrator
	{

	  /// <summary>
	  /// Time steps Butcher array. </summary>
	  private static readonly double[] STATIC_C = new double[] {1.0 / 2.0, 1.0 / 2.0, 1.0};

	  /// <summary>
	  /// Internal weights Butcher array. </summary>
	  private static readonly double[][] STATIC_A = new double[][] {new double[] {1.0 / 2.0}, new double[] {0.0, 1.0 / 2.0}, new double[] {0.0, 0.0, 1.0}};

	  /// <summary>
	  /// Propagation weights Butcher array. </summary>
	  private static readonly double[] STATIC_B = new double[] {1.0 / 6.0, 1.0 / 3.0, 1.0 / 3.0, 1.0 / 6.0};

	  /// <summary>
	  /// Simple constructor.
	  /// Build a fourth-order Runge-Kutta integrator with the given
	  /// step. </summary>
	  /// <param name="step"> integration step </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ClassicalRungeKuttaIntegrator(final double step)
	  public ClassicalRungeKuttaIntegrator(double step) : base("classical Runge-Kutta", STATIC_C, STATIC_A, STATIC_B, new ClassicalRungeKuttaStepInterpolator(), step)
	  {
	  }

	}

}