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

	using FastMath = mathlib.util.FastMath;


	/// <summary>
	/// This class implements the 5(4) Higham and Hall integrator for
	/// Ordinary Differential Equations.
	/// 
	/// <p>This integrator is an embedded Runge-Kutta integrator
	/// of order 5(4) used in local extrapolation mode (i.e. the solution
	/// is computed using the high order formula) with stepsize control
	/// (and automatic step initialization) and continuous output. This
	/// method uses 7 functions evaluations per step.</p>
	/// 
	/// @version $Id: HighamHall54Integrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class HighamHall54Integrator : EmbeddedRungeKuttaIntegrator
	{

	  /// <summary>
	  /// Integrator method name. </summary>
	  private const string METHOD_NAME = "Higham-Hall 5(4)";

	  /// <summary>
	  /// Time steps Butcher array. </summary>
	  private static readonly double[] STATIC_C = new double[] {2.0 / 9.0, 1.0 / 3.0, 1.0 / 2.0, 3.0 / 5.0, 1.0, 1.0};

	  /// <summary>
	  /// Internal weights Butcher array. </summary>
	  private static readonly double[][] STATIC_A = new double[][] {new double[] {2.0 / 9.0}, new double[] {1.0 / 12.0, 1.0 / 4.0}, new double[] {1.0 / 8.0, 0.0, 3.0 / 8.0}, new double[] {91.0 / 500.0, -27.0 / 100.0, 78.0 / 125.0, 8.0 / 125.0}, new double[] {-11.0 / 20.0, 27.0 / 20.0, 12.0 / 5.0, -36.0 / 5.0, 5.0}, new double[] {1.0 / 12.0, 0.0, 27.0 / 32.0, -4.0 / 3.0, 125.0 / 96.0, 5.0 / 48.0}};

	  /// <summary>
	  /// Propagation weights Butcher array. </summary>
	  private static readonly double[] STATIC_B = new double[] {1.0 / 12.0, 0.0, 27.0 / 32.0, -4.0 / 3.0, 125.0 / 96.0, 5.0 / 48.0, 0.0};

	  /// <summary>
	  /// Error weights Butcher array. </summary>
	  private static readonly double[] STATIC_E = new double[] {-1.0 / 20.0, 0.0, 81.0 / 160.0, -6.0 / 5.0, 25.0 / 32.0, 1.0 / 16.0, -1.0 / 10.0};

	  /// <summary>
	  /// Simple constructor.
	  /// Build a fifth order Higham and Hall integrator with the given step bounds </summary>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="scalRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public HighamHall54Integrator(final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance)
	  public HighamHall54Integrator(double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(METHOD_NAME, false, STATIC_C, STATIC_A, STATIC_B, new HighamHall54StepInterpolator(), minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
	  {
	  }

	  /// <summary>
	  /// Simple constructor.
	  /// Build a fifth order Higham and Hall integrator with the given step bounds </summary>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="vecRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public HighamHall54Integrator(final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance)
	  public HighamHall54Integrator(double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(METHOD_NAME, false, STATIC_C, STATIC_A, STATIC_B, new HighamHall54StepInterpolator(), minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public override int Order
	  {
		  get
		  {
			return 5;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected double estimateError(final double[][] yDotK, final double[] y0, final double[] y1, final double h)
	  protected internal override double estimateError(double[][] yDotK, double[] y0, double[] y1, double h)
	  {

		double error = 0;

		for (int j = 0; j < mainSetDimension; ++j)
		{
		  double errSum = STATIC_E[0] * yDotK[0][j];
		  for (int l = 1; l < STATIC_E.Length; ++l)
		  {
			errSum += STATIC_E[l] * yDotK[l][j];
		  }

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yScale = mathlib.util.FastMath.max(mathlib.util.FastMath.abs(y0[j]), mathlib.util.FastMath.abs(y1[j]));
		  double yScale = FastMath.max(FastMath.abs(y0[j]), FastMath.abs(y1[j]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[j] + vecRelativeTolerance[j] * yScale);
		  double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[j] + vecRelativeTolerance[j] * yScale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = h * errSum / tol;
		  double ratio = h * errSum / tol;
		  error += ratio * ratio;

		}

		return FastMath.sqrt(error / mainSetDimension);

	  }

	}

}