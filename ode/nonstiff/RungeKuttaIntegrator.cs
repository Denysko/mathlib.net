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

namespace mathlib.ode.nonstiff
{


	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoBracketingException = mathlib.exception.NoBracketingException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// This class implements the common part of all fixed step Runge-Kutta
	/// integrators for Ordinary Differential Equations.
	/// 
	/// <p>These methods are explicit Runge-Kutta methods, their Butcher
	/// arrays are as follows :
	/// <pre>
	///    0  |
	///   c2  | a21
	///   c3  | a31  a32
	///   ... |        ...
	///   cs  | as1  as2  ...  ass-1
	///       |--------------------------
	///       |  b1   b2  ...   bs-1  bs
	/// </pre>
	/// </p>
	/// </summary>
	/// <seealso cref= EulerIntegrator </seealso>
	/// <seealso cref= ClassicalRungeKuttaIntegrator </seealso>
	/// <seealso cref= GillIntegrator </seealso>
	/// <seealso cref= MidpointIntegrator
	/// @version $Id: RungeKuttaIntegrator.java 1588769 2014-04-20 14:29:42Z luc $
	/// @since 1.2 </seealso>

	public abstract class RungeKuttaIntegrator : AbstractIntegrator
	{

		/// <summary>
		/// Time steps from Butcher array (without the first zero). </summary>
		private readonly double[] c;

		/// <summary>
		/// Internal weights from Butcher array (without the first empty row). </summary>
		private readonly double[][] a;

		/// <summary>
		/// External weights for the high order method from Butcher array. </summary>
		private readonly double[] b;

		/// <summary>
		/// Prototype of the step interpolator. </summary>
		private readonly RungeKuttaStepInterpolator prototype;

		/// <summary>
		/// Integration step. </summary>
		private readonly double step;

	  /// <summary>
	  /// Simple constructor.
	  /// Build a Runge-Kutta integrator with the given
	  /// step. The default step handler does nothing. </summary>
	  /// <param name="name"> name of the method </param>
	  /// <param name="c"> time steps from Butcher array (without the first zero) </param>
	  /// <param name="a"> internal weights from Butcher array (without the first empty row) </param>
	  /// <param name="b"> propagation weights for the high order method from Butcher array </param>
	  /// <param name="prototype"> prototype of the step interpolator to use </param>
	  /// <param name="step"> integration step </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected RungeKuttaIntegrator(final String name, final double[] c, final double[][] a, final double[] b, final RungeKuttaStepInterpolator prototype, final double step)
	  protected internal RungeKuttaIntegrator(string name, double[] c, double[][] a, double[] b, RungeKuttaStepInterpolator prototype, double step) : base(name)
	  {
		this.c = c;
		this.a = a;
		this.b = b;
		this.prototype = prototype;
		this.step = FastMath.abs(step);
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void integrate(final mathlib.ode.ExpandableStatefulODE equations, final double t) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException, mathlib.exception.NoBracketingException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public override void integrate(ExpandableStatefulODE equations, double t)
	  {

		sanityChecks(equations, t);
		Equations = equations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean forward = t > equations.getTime();
		bool forward = t > equations.Time;

		// create some internal working arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y0 = equations.getCompleteState();
		double[] y0 = equations.CompleteState;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = y0.clone();
		double[] y = y0.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stages = c.length + 1;
		int stages = c.Length + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] yDotK = new double[stages][];
		double[][] yDotK = new double[stages][];
		for (int i = 0; i < stages; ++i)
		{
		  yDotK [i] = new double[y0.Length];
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmp = y0.clone();
		double[] yTmp = y0.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDotTmp = new double[y0.length];
		double[] yDotTmp = new double[y0.Length];

		// set up an interpolator sharing the integrator arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final RungeKuttaStepInterpolator interpolator = (RungeKuttaStepInterpolator) prototype.copy();
		RungeKuttaStepInterpolator interpolator = (RungeKuttaStepInterpolator) prototype.copy();
		interpolator.reinitialize(this, yTmp, yDotK, forward, equations.PrimaryMapper, equations.SecondaryMappers);
		interpolator.storeTime(equations.Time);

		// set up integration control objects
		stepStart = equations.Time;
		stepSize = forward ? step : -step;
		initIntegration(equations.Time, y0, t);

		// main integration loop
		isLastStep = false;
		do
		{

		  interpolator.shift();

		  // first stage
		  computeDerivatives(stepStart, y, yDotK[0]);

		  // next stages
		  for (int k = 1; k < stages; ++k)
		  {

			  for (int j = 0; j < y0.Length; ++j)
			  {
				  double sum = a[k - 1][0] * yDotK[0][j];
				  for (int l = 1; l < k; ++l)
				  {
					  sum += a[k - 1][l] * yDotK[l][j];
				  }
				  yTmp[j] = y[j] + stepSize * sum;
			  }

			  computeDerivatives(stepStart + c[k - 1] * stepSize, yTmp, yDotK[k]);

		  }

		  // estimate the state at the end of the step
		  for (int j = 0; j < y0.Length; ++j)
		  {
			  double sum = b[0] * yDotK[0][j];
			  for (int l = 1; l < stages; ++l)
			  {
				  sum += b[l] * yDotK[l][j];
			  }
			  yTmp[j] = y[j] + stepSize * sum;
		  }

		  // discrete events handling
		  interpolator.storeTime(stepStart + stepSize);
		  Array.Copy(yTmp, 0, y, 0, y0.Length);
		  Array.Copy(yDotK[stages - 1], 0, yDotTmp, 0, y0.Length);
		  stepStart = acceptStep(interpolator, y, yDotTmp, t);

		  if (!isLastStep)
		  {

			  // prepare next step
			  interpolator.storeTime(stepStart);

			  // stepsize control for next step
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nextT = stepStart + stepSize;
			  double nextT = stepStart + stepSize;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final boolean nextIsLast = forward ? (nextT >= t) : (nextT <= t);
			  bool nextIsLast = forward ? (nextT >= t) : (nextT <= t);
			  if (nextIsLast)
			  {
				  stepSize = t - stepStart;
			  }
		  }

		} while (!isLastStep);

		// dispatch results
		equations.Time = stepStart;
		equations.CompleteState = y;

		stepStart = double.NaN;
		stepSize = double.NaN;

	  }

	  /// <summary>
	  /// Fast computation of a single step of ODE integration.
	  /// <p>This method is intended for the limited use case of
	  /// very fast computation of only one step without using any of the
	  /// rich features of general integrators that may take some time
	  /// to set up (i.e. no step handlers, no events handlers, no additional
	  /// states, no interpolators, no error control, no evaluations count,
	  /// no sanity checks ...). It handles the strict minimum of computation,
	  /// so it can be embedded in outer loops.</p>
	  /// <p>
	  /// This method is <em>not</em> used at all by the <seealso cref="#integrate(ExpandableStatefulODE, double)"/>
	  /// method. It also completely ignores the step set at construction time, and
	  /// uses only a single step to go from {@code t0} to {@code t}.
	  /// </p>
	  /// <p>
	  /// As this method does not use any of the state-dependent features of the integrator,
	  /// it should be reasonably thread-safe <em>if and only if</em> the provided differential
	  /// equations are themselves thread-safe.
	  /// </p> </summary>
	  /// <param name="equations"> differential equations to integrate </param>
	  /// <param name="t0"> initial time </param>
	  /// <param name="y0"> initial value of the state vector at t0 </param>
	  /// <param name="t"> target time for the integration
	  /// (can be set to a value smaller than {@code t0} for backward integration) </param>
	  /// <returns> state vector at {@code t} </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] singleStep(final mathlib.ode.FirstOrderDifferentialEquations equations, final double t0, final double[] y0, final double t)
	  public virtual double[] singleStep(FirstOrderDifferentialEquations equations, double t0, double[] y0, double t)
	  {

		  // create some internal working arrays
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y = y0.clone();
		  double[] y = y0.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int stages = c.length + 1;
		  int stages = c.Length + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] yDotK = new double[stages][];
		  double[][] yDotK = new double[stages][];
		  for (int i = 0; i < stages; ++i)
		  {
			  yDotK [i] = new double[y0.Length];
		  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yTmp = y0.clone();
		  double[] yTmp = y0.clone();

		  // first stage
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double h = t - t0;
		  double h = t - t0;
		  equations.computeDerivatives(t0, y, yDotK[0]);

		  // next stages
		  for (int k = 1; k < stages; ++k)
		  {

			  for (int j = 0; j < y0.Length; ++j)
			  {
				  double sum = a[k - 1][0] * yDotK[0][j];
				  for (int l = 1; l < k; ++l)
				  {
					  sum += a[k - 1][l] * yDotK[l][j];
				  }
				  yTmp[j] = y[j] + h * sum;
			  }

			  equations.computeDerivatives(t0 + c[k - 1] * h, yTmp, yDotK[k]);

		  }

		  // estimate the state at the end of the step
		  for (int j = 0; j < y0.Length; ++j)
		  {
			  double sum = b[0] * yDotK[0][j];
			  for (int l = 1; l < stages; ++l)
			  {
				  sum += b[l] * yDotK[l][j];
			  }
			  y[j] += h * sum;
		  }

		  return y;

	  }

	}

}