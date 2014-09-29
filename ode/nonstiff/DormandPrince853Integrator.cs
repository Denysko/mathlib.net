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
	/// This class implements the 8(5,3) Dormand-Prince integrator for Ordinary
	/// Differential Equations.
	/// 
	/// <p>This integrator is an embedded Runge-Kutta integrator
	/// of order 8(5,3) used in local extrapolation mode (i.e. the solution
	/// is computed using the high order formula) with stepsize control
	/// (and automatic step initialization) and continuous output. This
	/// method uses 12 functions evaluations per step for integration and 4
	/// evaluations for interpolation. However, since the first
	/// interpolation evaluation is the same as the first integration
	/// evaluation of the next step, we have included it in the integrator
	/// rather than in the interpolator and specified the method was an
	/// <i>fsal</i>. Hence, despite we have 13 stages here, the cost is
	/// really 12 evaluations per step even if no interpolation is done,
	/// and the overcost of interpolation is only 3 evaluations.</p>
	/// 
	/// <p>This method is based on an 8(6) method by Dormand and Prince
	/// (i.e. order 8 for the integration and order 6 for error estimation)
	/// modified by Hairer and Wanner to use a 5th order error estimator
	/// with 3rd order correction. This modification was introduced because
	/// the original method failed in some cases (wrong steps can be
	/// accepted when step size is too large, for example in the
	/// Brusselator problem) and also had <i>severe difficulties when
	/// applied to problems with discontinuities</i>. This modification is
	/// explained in the second edition of the first volume (Nonstiff
	/// Problems) of the reference book by Hairer, Norsett and Wanner:
	/// <i>Solving Ordinary Differential Equations</i> (Springer-Verlag,
	/// ISBN 3-540-56670-8).</p>
	/// 
	/// @version $Id: DormandPrince853Integrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>

	public class DormandPrince853Integrator : EmbeddedRungeKuttaIntegrator
	{

	  /// <summary>
	  /// Integrator method name. </summary>
	  private const string METHOD_NAME = "Dormand-Prince 8 (5, 3)";

	  /// <summary>
	  /// Time steps Butcher array. </summary>
	  private static readonly double[] STATIC_C = new double[] {(12.0 - 2.0 * FastMath.sqrt(6.0)) / 135.0, (6.0 - FastMath.sqrt(6.0)) / 45.0, (6.0 - FastMath.sqrt(6.0)) / 30.0, (6.0 + FastMath.sqrt(6.0)) / 30.0, 1.0 / 3.0, 1.0 / 4.0, 4.0 / 13.0, 127.0 / 195.0, 3.0 / 5.0, 6.0 / 7.0, 1.0, 1.0};

	  /// <summary>
	  /// Internal weights Butcher array. </summary>
	  private static readonly double[][] STATIC_A = new double[][] {new double[] {(12.0 - 2.0 * FastMath.sqrt(6.0)) / 135.0}, new double[] {(6.0 - FastMath.sqrt(6.0)) / 180.0, (6.0 - FastMath.sqrt(6.0)) / 60.0}, new double[] {(6.0 - FastMath.sqrt(6.0)) / 120.0, 0.0, (6.0 - FastMath.sqrt(6.0)) / 40.0}, new double[] {(462.0 + 107.0 * FastMath.sqrt(6.0)) / 3000.0, 0.0, (-402.0 - 197.0 * FastMath.sqrt(6.0)) / 1000.0, (168.0 + 73.0 * FastMath.sqrt(6.0)) / 375.0}, new double[] {1.0 / 27.0, 0.0, 0.0, (16.0 + FastMath.sqrt(6.0)) / 108.0, (16.0 - FastMath.sqrt(6.0)) / 108.0}, new double[] {19.0 / 512.0, 0.0, 0.0, (118.0 + 23.0 * FastMath.sqrt(6.0)) / 1024.0, (118.0 - 23.0 * FastMath.sqrt(6.0)) / 1024.0, -9.0 / 512.0}, new double[] {13772.0 / 371293.0, 0.0, 0.0, (51544.0 + 4784.0 * FastMath.sqrt(6.0)) / 371293.0, (51544.0 - 4784.0 * FastMath.sqrt(6.0)) / 371293.0, -5688.0 / 371293.0, 3072.0 / 371293.0}, new double[] {58656157643.0 / 93983540625.0, 0.0, 0.0, (-1324889724104.0 - 318801444819.0 * FastMath.sqrt(6.0)) / 626556937500.0, (-1324889724104.0 + 318801444819.0 * FastMath.sqrt(6.0)) / 626556937500.0, 96044563816.0 / 3480871875.0, 5682451879168.0 / 281950621875.0, -165125654.0 / 3796875.0}, new double[] {8909899.0 / 18653125.0, 0.0, 0.0, (-4521408.0 - 1137963.0 * FastMath.sqrt(6.0)) / 2937500.0, (-4521408.0 + 1137963.0 * FastMath.sqrt(6.0)) / 2937500.0, 96663078.0 / 4553125.0, 2107245056.0 / 137915625.0, -4913652016.0 / 147609375.0, -78894270.0 / 3880452869.0}, new double[] {-20401265806.0 / 21769653311.0, 0.0, 0.0, (354216.0 + 94326.0 * FastMath.sqrt(6.0)) / 112847.0, (354216.0 - 94326.0 * FastMath.sqrt(6.0)) / 112847.0, -43306765128.0 / 5313852383.0, -20866708358144.0 / 1126708119789.0, 14886003438020.0 / 654632330667.0, 35290686222309375.0 / 14152473387134411.0, -1477884375.0 / 485066827.0}, new double[] {39815761.0 / 17514443.0, 0.0, 0.0, (-3457480.0 - 960905.0 * FastMath.sqrt(6.0)) / 551636.0, (-3457480.0 + 960905.0 * FastMath.sqrt(6.0)) / 551636.0, -844554132.0 / 47026969.0, 8444996352.0 / 302158619.0, -2509602342.0 / 877790785.0, -28388795297996250.0 / 3199510091356783.0, 226716250.0 / 18341897.0, 1371316744.0 / 2131383595.0}, new double[] {104257.0 / 1920240.0, 0.0, 0.0, 0.0, 0.0, 3399327.0 / 763840.0, 66578432.0 / 35198415.0, -1674902723.0 / 288716400.0, 54980371265625.0 / 176692375811392.0, -734375.0 / 4826304.0, 171414593.0 / 851261400.0, 137909.0 / 3084480.0}};

	  /// <summary>
	  /// Propagation weights Butcher array. </summary>
	  private static readonly double[] STATIC_B = new double[] {104257.0 / 1920240.0, 0.0, 0.0, 0.0, 0.0, 3399327.0 / 763840.0, 66578432.0 / 35198415.0, -1674902723.0 / 288716400.0, 54980371265625.0 / 176692375811392.0, -734375.0 / 4826304.0, 171414593.0 / 851261400.0, 137909.0 / 3084480.0, 0.0};

	  /// <summary>
	  /// First error weights array, element 1. </summary>
	  private const double E1_01 = 116092271.0 / 8848465920.0;

	  // elements 2 to 5 are zero, so they are neither stored nor used

	  /// <summary>
	  /// First error weights array, element 6. </summary>
	  private const double E1_06 = -1871647.0 / 1527680.0;

	  /// <summary>
	  /// First error weights array, element 7. </summary>
	  private const double E1_07 = -69799717.0 / 140793660.0;

	  /// <summary>
	  /// First error weights array, element 8. </summary>
	  private const double E1_08 = 1230164450203.0 / 739113984000.0;

	  /// <summary>
	  /// First error weights array, element 9. </summary>
	  private const double E1_09 = -1980813971228885.0 / 5654156025964544.0;

	  /// <summary>
	  /// First error weights array, element 10. </summary>
	  private const double E1_10 = 464500805.0 / 1389975552.0;

	  /// <summary>
	  /// First error weights array, element 11. </summary>
	  private const double E1_11 = 1606764981773.0 / 19613062656000.0;

	  /// <summary>
	  /// First error weights array, element 12. </summary>
	  private const double E1_12 = -137909.0 / 6168960.0;


	  /// <summary>
	  /// Second error weights array, element 1. </summary>
	  private const double E2_01 = -364463.0 / 1920240.0;

	  // elements 2 to 5 are zero, so they are neither stored nor used

	  /// <summary>
	  /// Second error weights array, element 6. </summary>
	  private const double E2_06 = 3399327.0 / 763840.0;

	  /// <summary>
	  /// Second error weights array, element 7. </summary>
	  private const double E2_07 = 66578432.0 / 35198415.0;

	  /// <summary>
	  /// Second error weights array, element 8. </summary>
	  private const double E2_08 = -1674902723.0 / 288716400.0;

	  /// <summary>
	  /// Second error weights array, element 9. </summary>
	  private const double E2_09 = -74684743568175.0 / 176692375811392.0;

	  /// <summary>
	  /// Second error weights array, element 10. </summary>
	  private const double E2_10 = -734375.0 / 4826304.0;

	  /// <summary>
	  /// Second error weights array, element 11. </summary>
	  private const double E2_11 = 171414593.0 / 851261400.0;

	  /// <summary>
	  /// Second error weights array, element 12. </summary>
	  private const double E2_12 = 69869.0 / 3084480.0;

	  /// <summary>
	  /// Simple constructor.
	  /// Build an eighth order Dormand-Prince integrator with the given step bounds </summary>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="scalRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DormandPrince853Integrator(final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance)
	  public DormandPrince853Integrator(double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(METHOD_NAME, true, STATIC_C, STATIC_A, STATIC_B, new DormandPrince853StepInterpolator(), minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
	  {
	  }

	  /// <summary>
	  /// Simple constructor.
	  /// Build an eighth order Dormand-Prince integrator with the given step bounds </summary>
	  /// <param name="minStep"> minimal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
	  /// integration direction, forward or backward), the last step can
	  /// be smaller than this </param>
	  /// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
	  /// <param name="vecRelativeTolerance"> allowed relative error </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DormandPrince853Integrator(final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance)
	  public DormandPrince853Integrator(double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(METHOD_NAME, true, STATIC_C, STATIC_A, STATIC_B, new DormandPrince853StepInterpolator(), minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  public override int Order
	  {
		  get
		  {
			return 8;
		  }
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected double estimateError(final double[][] yDotK, final double[] y0, final double[] y1, final double h)
	  protected internal override double estimateError(double[][] yDotK, double[] y0, double[] y1, double h)
	  {
		double error1 = 0;
		double error2 = 0;

		for (int j = 0; j < mainSetDimension; ++j)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double errSum1 = E1_01 * yDotK[0][j] + E1_06 * yDotK[5][j] + E1_07 * yDotK[6][j] + E1_08 * yDotK[7][j] + E1_09 * yDotK[8][j] + E1_10 * yDotK[9][j] + E1_11 * yDotK[10][j] + E1_12 * yDotK[11][j];
		  double errSum1 = E1_01 * yDotK[0][j] + E1_06 * yDotK[5][j] + E1_07 * yDotK[6][j] + E1_08 * yDotK[7][j] + E1_09 * yDotK[8][j] + E1_10 * yDotK[9][j] + E1_11 * yDotK[10][j] + E1_12 * yDotK[11][j];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double errSum2 = E2_01 * yDotK[0][j] + E2_06 * yDotK[5][j] + E2_07 * yDotK[6][j] + E2_08 * yDotK[7][j] + E2_09 * yDotK[8][j] + E2_10 * yDotK[9][j] + E2_11 * yDotK[10][j] + E2_12 * yDotK[11][j];
		  double errSum2 = E2_01 * yDotK[0][j] + E2_06 * yDotK[5][j] + E2_07 * yDotK[6][j] + E2_08 * yDotK[7][j] + E2_09 * yDotK[8][j] + E2_10 * yDotK[9][j] + E2_11 * yDotK[10][j] + E2_12 * yDotK[11][j];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yScale = mathlib.util.FastMath.max(mathlib.util.FastMath.abs(y0[j]), mathlib.util.FastMath.abs(y1[j]));
		  double yScale = FastMath.max(FastMath.abs(y0[j]), FastMath.abs(y1[j]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[j] + vecRelativeTolerance[j] * yScale);
		  double tol = (vecAbsoluteTolerance == null) ? (scalAbsoluteTolerance + scalRelativeTolerance * yScale) : (vecAbsoluteTolerance[j] + vecRelativeTolerance[j] * yScale);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio1 = errSum1 / tol;
		  double ratio1 = errSum1 / tol;
		  error1 += ratio1 * ratio1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio2 = errSum2 / tol;
		  double ratio2 = errSum2 / tol;
		  error2 += ratio2 * ratio2;
		}

		double den = error1 + 0.01 * error2;
		if (den <= 0.0)
		{
		  den = 1.0;
		}

		return FastMath.abs(h) * error1 / FastMath.sqrt(mainSetDimension * den);

	  }

	}

}