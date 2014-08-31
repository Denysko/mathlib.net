using System;

// CHECKSTYLE: stop all
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
namespace org.apache.commons.math3.optim.nonlinear.scalar.noderiv
{

	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// Powell's BOBYQA algorithm. This implementation is translated and
	/// adapted from the Fortran version available
	/// <a href="http://plato.asu.edu/ftp/other_software/bobyqa.zip">here</a>.
	/// See <a href="http://www.optimization-online.org/DB_HTML/2010/05/2616.html">
	/// this paper</a> for an introduction.
	/// <br/>
	/// BOBYQA is particularly well suited for high dimensional problems
	/// where derivatives are not available. In most cases it outperforms the
	/// <seealso cref="PowellOptimizer"/> significantly. Stochastic algorithms like
	/// <seealso cref="CMAESOptimizer"/> succeed more often than BOBYQA, but are more
	/// expensive. BOBYQA could also be considered as a replacement of any
	/// derivative-based optimizer when the derivatives are approximated by
	/// finite differences.
	/// 
	/// @version $Id: BOBYQAOptimizer.java 1540165 2013-11-08 19:53:58Z tn $
	/// @since 3.0
	/// </summary>
	public class BOBYQAOptimizer : MultivariateOptimizer
	{
		/// <summary>
		/// Minimum dimension of the problem: {@value} </summary>
		public const int MINIMUM_PROBLEM_DIMENSION = 2;
		/// <summary>
		/// Default value for <seealso cref="#initialTrustRegionRadius"/>: {@value} . </summary>
		public const double DEFAULT_INITIAL_RADIUS = 10.0;
		/// <summary>
		/// Default value for <seealso cref="#stoppingTrustRegionRadius"/>: {@value} . </summary>
		public const double DEFAULT_STOPPING_RADIUS = 1E-8;

		private const double ZERO = 0d;
		private const double ONE = 1d;
		private const double TWO = 2d;
		private const double TEN = 10d;
		private const double SIXTEEN = 16d;
		private const double TWO_HUNDRED_FIFTY = 250d;
		private static readonly double MINUS_ONE = -ONE;
		private static readonly double HALF = ONE / 2;
		private static readonly double ONE_OVER_FOUR = ONE / 4;
		private static readonly double ONE_OVER_EIGHT = ONE / 8;
		private static readonly double ONE_OVER_TEN = ONE / 10;
		private static readonly double ONE_OVER_A_THOUSAND = ONE / 1000;

		/// <summary>
		/// numberOfInterpolationPoints XXX
		/// </summary>
		private readonly int numberOfInterpolationPoints;
		/// <summary>
		/// initialTrustRegionRadius XXX
		/// </summary>
		private double initialTrustRegionRadius;
		/// <summary>
		/// stoppingTrustRegionRadius XXX
		/// </summary>
		private readonly double stoppingTrustRegionRadius;
		/// <summary>
		/// Goal type (minimize or maximize). </summary>
		private bool isMinimize;
		/// <summary>
		/// Current best values for the variables to be optimized.
		/// The vector will be changed in-place to contain the values of the least
		/// calculated objective function values.
		/// </summary>
		private ArrayRealVector currentBest;
		/// <summary>
		/// Differences between the upper and lower bounds. </summary>
		private double[] boundDifference;
		/// <summary>
		/// Index of the interpolation point at the trust region center.
		/// </summary>
		private int trustRegionCenterInterpolationPointIndex;
		/// <summary>
		/// Last <em>n</em> columns of matrix H (where <em>n</em> is the dimension
		/// of the problem).
		/// XXX "bmat" in the original code.
		/// </summary>
		private Array2DRowRealMatrix bMatrix;
		/// <summary>
		/// Factorization of the leading <em>npt</em> square submatrix of H, this
		/// factorization being Z Z<sup>T</sup>, which provides both the correct
		/// rank and positive semi-definiteness.
		/// XXX "zmat" in the original code.
		/// </summary>
		private Array2DRowRealMatrix zMatrix;
		/// <summary>
		/// Coordinates of the interpolation points relative to <seealso cref="#originShift"/>.
		/// XXX "xpt" in the original code.
		/// </summary>
		private Array2DRowRealMatrix interpolationPoints;
		/// <summary>
		/// Shift of origin that should reduce the contributions from rounding
		/// errors to values of the model and Lagrange functions.
		/// XXX "xbase" in the original code.
		/// </summary>
		private ArrayRealVector originShift;
		/// <summary>
		/// Values of the objective function at the interpolation points.
		/// XXX "fval" in the original code.
		/// </summary>
		private ArrayRealVector fAtInterpolationPoints;
		/// <summary>
		/// Displacement from <seealso cref="#originShift"/> of the trust region center.
		/// XXX "xopt" in the original code.
		/// </summary>
		private ArrayRealVector trustRegionCenterOffset;
		/// <summary>
		/// Gradient of the quadratic model at <seealso cref="#originShift"/> +
		/// <seealso cref="#trustRegionCenterOffset"/>.
		/// XXX "gopt" in the original code.
		/// </summary>
		private ArrayRealVector gradientAtTrustRegionCenter;
		/// <summary>
		/// Differences <seealso cref="#getLowerBound()"/> - <seealso cref="#originShift"/>.
		/// All the components of every <seealso cref="#trustRegionCenterOffset"/> are going
		/// to satisfy the bounds<br/>
		/// <seealso cref="#getLowerBound() lowerBound"/><sub>i</sub> &le;
		/// <seealso cref="#trustRegionCenterOffset"/><sub>i</sub>,<br/>
		/// with appropriate equalities when <seealso cref="#trustRegionCenterOffset"/> is
		/// on a constraint boundary.
		/// XXX "sl" in the original code.
		/// </summary>
		private ArrayRealVector lowerDifference;
		/// <summary>
		/// Differences <seealso cref="#getUpperBound()"/> - <seealso cref="#originShift"/>
		/// All the components of every <seealso cref="#trustRegionCenterOffset"/> are going
		/// to satisfy the bounds<br/>
		///  <seealso cref="#trustRegionCenterOffset"/><sub>i</sub> &le;
		///  <seealso cref="#getUpperBound() upperBound"/><sub>i</sub>,<br/>
		/// with appropriate equalities when <seealso cref="#trustRegionCenterOffset"/> is
		/// on a constraint boundary.
		/// XXX "su" in the original code.
		/// </summary>
		private ArrayRealVector upperDifference;
		/// <summary>
		/// Parameters of the implicit second derivatives of the quadratic model.
		/// XXX "pq" in the original code.
		/// </summary>
		private ArrayRealVector modelSecondDerivativesParameters;
		/// <summary>
		/// Point chosen by function {@link #trsbox(double,ArrayRealVector,
		/// ArrayRealVector, ArrayRealVector,ArrayRealVector,ArrayRealVector) trsbox}
		/// or <seealso cref="#altmov(int,double) altmov"/>.
		/// Usually <seealso cref="#originShift"/> + <seealso cref="#newPoint"/> is the vector of
		/// variables for the next evaluation of the objective function.
		/// It also satisfies the constraints indicated in <seealso cref="#lowerDifference"/>
		/// and <seealso cref="#upperDifference"/>.
		/// XXX "xnew" in the original code.
		/// </summary>
		private ArrayRealVector newPoint;
		/// <summary>
		/// Alternative to <seealso cref="#newPoint"/>, chosen by
		/// <seealso cref="#altmov(int,double) altmov"/>.
		/// It may replace <seealso cref="#newPoint"/> in order to increase the denominator
		/// in the <seealso cref="#update(double, double, int) updating procedure"/>.
		/// XXX "xalt" in the original code.
		/// </summary>
		private ArrayRealVector alternativeNewPoint;
		/// <summary>
		/// Trial step from <seealso cref="#trustRegionCenterOffset"/> which is usually
		/// <seealso cref="#newPoint"/> - <seealso cref="#trustRegionCenterOffset"/>.
		/// XXX "d__" in the original code.
		/// </summary>
		private ArrayRealVector trialStepPoint;
		/// <summary>
		/// Values of the Lagrange functions at a new point.
		/// XXX "vlag" in the original code.
		/// </summary>
		private ArrayRealVector lagrangeValuesAtNewPoint;
		/// <summary>
		/// Explicit second derivatives of the quadratic model.
		/// XXX "hq" in the original code.
		/// </summary>
		private ArrayRealVector modelSecondDerivativesValues;

		/// <param name="numberOfInterpolationPoints"> Number of interpolation conditions.
		/// For a problem of dimension {@code n}, its value must be in the interval
		/// {@code [n+2, (n+1)(n+2)/2]}.
		/// Choices that exceed {@code 2n+1} are not recommended. </param>
		public BOBYQAOptimizer(int numberOfInterpolationPoints) : this(numberOfInterpolationPoints, DEFAULT_INITIAL_RADIUS, DEFAULT_STOPPING_RADIUS)
		{
		}

		/// <param name="numberOfInterpolationPoints"> Number of interpolation conditions.
		/// For a problem of dimension {@code n}, its value must be in the interval
		/// {@code [n+2, (n+1)(n+2)/2]}.
		/// Choices that exceed {@code 2n+1} are not recommended. </param>
		/// <param name="initialTrustRegionRadius"> Initial trust region radius. </param>
		/// <param name="stoppingTrustRegionRadius"> Stopping trust region radius. </param>
		public BOBYQAOptimizer(int numberOfInterpolationPoints, double initialTrustRegionRadius, double stoppingTrustRegionRadius) : base(null); / / No custom convergence criterion.
		{
			this.numberOfInterpolationPoints = numberOfInterpolationPoints;
			this.initialTrustRegionRadius = initialTrustRegionRadius;
			this.stoppingTrustRegionRadius = stoppingTrustRegionRadius;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PointValuePair doOptimize()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] lowerBound = getLowerBound();
			double[] lowerBound = LowerBound;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] upperBound = getUpperBound();
			double[] upperBound = UpperBound;

			// Validity checks.
			setup(lowerBound, upperBound);

			isMinimize = (GoalType == GoalType.MINIMIZE);
			currentBest = new ArrayRealVector(StartPoint);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double value = bobyqa(lowerBound, upperBound);
			double value = bobyqa(lowerBound, upperBound);

			return new PointValuePair(currentBest.DataRef, isMinimize ? value : -value);
		}

		/// <summary>
		///     This subroutine seeks the least value of a function of many variables,
		///     by applying a trust region method that forms quadratic models by
		///     interpolation. There is usually some freedom in the interpolation
		///     conditions, which is taken up by minimizing the Frobenius norm of
		///     the change to the second derivative of the model, beginning with the
		///     zero matrix. The values of the variables are constrained by upper and
		///     lower bounds. The arguments of the subroutine are as follows.
		/// 
		///     N must be set to the number of variables and must be at least two.
		///     NPT is the number of interpolation conditions. Its value must be in
		///       the interval [N+2,(N+1)(N+2)/2]. Choices that exceed 2*N+1 are not
		///       recommended.
		///     Initial values of the variables must be set in X(1),X(2),...,X(N). They
		///       will be changed to the values that give the least calculated F.
		///     For I=1,2,...,N, XL(I) and XU(I) must provide the lower and upper
		///       bounds, respectively, on X(I). The construction of quadratic models
		///       requires XL(I) to be strictly less than XU(I) for each I. Further,
		///       the contribution to a model from changes to the I-th variable is
		///       damaged severely by rounding errors if XU(I)-XL(I) is too small.
		///     RHOBEG and RHOEND must be set to the initial and final values of a trust
		///       region radius, so both must be positive with RHOEND no greater than
		///       RHOBEG. Typically, RHOBEG should be about one tenth of the greatest
		///       expected change to a variable, while RHOEND should indicate the
		///       accuracy that is required in the final values of the variables. An
		///       error return occurs if any of the differences XU(I)-XL(I), I=1,...,N,
		///       is less than 2*RHOBEG.
		///     MAXFUN must be set to an upper bound on the number of calls of CALFUN.
		///     The array W will be used for working space. Its length must be at least
		///       (NPT+5)*(NPT+N)+3*N*(N+5)/2.
		/// </summary>
		/// <param name="lowerBound"> Lower bounds. </param>
		/// <param name="upperBound"> Upper bounds. </param>
		/// <returns> the value of the objective at the optimum. </returns>
		private double bobyqa(double[] lowerBound, double[] upperBound)
		{
			printMethod(); // XXX

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = currentBest.getDimension();
			int n = currentBest.Dimension;

			// Return if there is insufficient space between the bounds. Modify the
			// initial X if necessary in order to avoid conflicts between the bounds
			// and the construction of the first quadratic model. The lower and upper
			// bounds on moves from the updated X are set now, in the ISL and ISU
			// partitions of W, in order to provide useful and exact information about
			// components of X that become within distance RHOBEG from their bounds.

			for (int j = 0; j < n; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double boundDiff = boundDifference[j];
				double boundDiff = boundDifference[j];
				lowerDifference.setEntry(j, lowerBound[j] - currentBest.getEntry(j));
				upperDifference.setEntry(j, upperBound[j] - currentBest.getEntry(j));
				if (lowerDifference.getEntry(j) >= -initialTrustRegionRadius)
				{
					if (lowerDifference.getEntry(j) >= ZERO)
					{
						currentBest.setEntry(j, lowerBound[j]);
						lowerDifference.setEntry(j, ZERO);
						upperDifference.setEntry(j, boundDiff);
					}
					else
					{
						currentBest.setEntry(j, lowerBound[j] + initialTrustRegionRadius);
						lowerDifference.setEntry(j, -initialTrustRegionRadius);
						// Computing MAX
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaOne = upperBound[j] - currentBest.getEntry(j);
						double deltaOne = upperBound[j] - currentBest.getEntry(j);
						upperDifference.setEntry(j, FastMath.max(deltaOne, initialTrustRegionRadius));
					}
				}
				else if (upperDifference.getEntry(j) <= initialTrustRegionRadius)
				{
					if (upperDifference.getEntry(j) <= ZERO)
					{
						currentBest.setEntry(j, upperBound[j]);
						lowerDifference.setEntry(j, -boundDiff);
						upperDifference.setEntry(j, ZERO);
					}
					else
					{
						currentBest.setEntry(j, upperBound[j] - initialTrustRegionRadius);
						// Computing MIN
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaOne = lowerBound[j] - currentBest.getEntry(j);
						double deltaOne = lowerBound[j] - currentBest.getEntry(j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaTwo = -initialTrustRegionRadius;
						double deltaTwo = -initialTrustRegionRadius;
						lowerDifference.setEntry(j, FastMath.min(deltaOne, deltaTwo));
						upperDifference.setEntry(j, initialTrustRegionRadius);
					}
				}
			}

			// Make the call of BOBYQB.

			return bobyqb(lowerBound, upperBound);
		} // bobyqa

		// ----------------------------------------------------------------------------------------

		/// <summary>
		///     The arguments N, NPT, X, XL, XU, RHOBEG, RHOEND, IPRINT and MAXFUN
		///       are identical to the corresponding arguments in SUBROUTINE BOBYQA.
		///     XBASE holds a shift of origin that should reduce the contributions
		///       from rounding errors to values of the model and Lagrange functions.
		///     XPT is a two-dimensional array that holds the coordinates of the
		///       interpolation points relative to XBASE.
		///     FVAL holds the values of F at the interpolation points.
		///     XOPT is set to the displacement from XBASE of the trust region centre.
		///     GOPT holds the gradient of the quadratic model at XBASE+XOPT.
		///     HQ holds the explicit second derivatives of the quadratic model.
		///     PQ contains the parameters of the implicit second derivatives of the
		///       quadratic model.
		///     BMAT holds the last N columns of H.
		///     ZMAT holds the factorization of the leading NPT by NPT submatrix of H,
		///       this factorization being ZMAT times ZMAT^T, which provides both the
		///       correct rank and positive semi-definiteness.
		///     NDIM is the first dimension of BMAT and has the value NPT+N.
		///     SL and SU hold the differences XL-XBASE and XU-XBASE, respectively.
		///       All the components of every XOPT are going to satisfy the bounds
		///       SL(I) .LEQ. XOPT(I) .LEQ. SU(I), with appropriate equalities when
		///       XOPT is on a constraint boundary.
		///     XNEW is chosen by SUBROUTINE TRSBOX or ALTMOV. Usually XBASE+XNEW is the
		///       vector of variables for the next call of CALFUN. XNEW also satisfies
		///       the SL and SU constraints in the way that has just been mentioned.
		///     XALT is an alternative to XNEW, chosen by ALTMOV, that may replace XNEW
		///       in order to increase the denominator in the updating of UPDATE.
		///     D is reserved for a trial step from XOPT, which is usually XNEW-XOPT.
		///     VLAG contains the values of the Lagrange functions at a new point X.
		///       They are part of a product that requires VLAG to be of length NDIM.
		///     W is a one-dimensional array that is used for working space. Its length
		///       must be at least 3*NDIM = 3*(NPT+N).
		/// </summary>
		/// <param name="lowerBound"> Lower bounds. </param>
		/// <param name="upperBound"> Upper bounds. </param>
		/// <returns> the value of the objective at the optimum. </returns>
		private double bobyqb(double[] lowerBound, double[] upperBound)
		{
			printMethod(); // XXX

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = currentBest.getDimension();
			int n = currentBest.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int npt = numberOfInterpolationPoints;
			int npt = numberOfInterpolationPoints;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int np = n + 1;
			int np = n + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nptm = npt - np;
			int nptm = npt - np;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nh = n * np / 2;
			int nh = n * np / 2;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector work1 = new org.apache.commons.math3.linear.ArrayRealVector(n);
			ArrayRealVector work1 = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector work2 = new org.apache.commons.math3.linear.ArrayRealVector(npt);
			ArrayRealVector work2 = new ArrayRealVector(npt);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector work3 = new org.apache.commons.math3.linear.ArrayRealVector(npt);
			ArrayRealVector work3 = new ArrayRealVector(npt);

			double cauchy = double.NaN;
			double alpha = double.NaN;
			double dsq = double.NaN;
			double crvmin = double.NaN;

			// Set some constants.
			// Parameter adjustments

			// Function Body

			// The call of PRELIM sets the elements of XBASE, XPT, FVAL, GOPT, HQ, PQ,
			// BMAT and ZMAT for the first iteration, with the corresponding values of
			// of NF and KOPT, which are the number of calls of CALFUN so far and the
			// index of the interpolation point at the trust region centre. Then the
			// initial XOPT is set too. The branch to label 720 occurs if MAXFUN is
			// less than NPT. GOPT will be updated if KOPT is different from KBASE.

			trustRegionCenterInterpolationPointIndex = 0;

			prelim(lowerBound, upperBound);
			double xoptsq = ZERO;
			for (int i = 0; i < n; i++)
			{
				trustRegionCenterOffset.setEntry(i, interpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex, i));
				// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double deltaOne = trustRegionCenterOffset.getEntry(i);
				double deltaOne = trustRegionCenterOffset.getEntry(i);
				xoptsq += deltaOne * deltaOne;
			}
			double fsave = fAtInterpolationPoints.getEntry(0);
			const int kbase = 0;

			// Complete the settings that are required for the iterative procedure.

			int ntrits = 0;
			int itest = 0;
			int knew = 0;
			int nfsav = Evaluations;
			double rho = initialTrustRegionRadius;
			double delta = rho;
			double diffa = ZERO;
			double diffb = ZERO;
			double diffc = ZERO;
			double f = ZERO;
			double beta = ZERO;
			double adelt = ZERO;
			double denom = ZERO;
			double ratio = ZERO;
			double dnorm = ZERO;
			double scaden = ZERO;
			double biglsq = ZERO;
			double distsq = ZERO;

			// Update GOPT if necessary before the first iteration and after each
			// call of RESCUE that makes a call of CALFUN.

			int state = 20;
			for (;;)
			{
				switch (state)
				{
			case 20:
			{
				printState(20); // XXX
				if (trustRegionCenterInterpolationPointIndex != kbase)
				{
					int ih = 0;
					for (int j = 0; j < n; j++)
					{
						for (int i = 0; i <= j; i++)
						{
							if (i < j)
							{
								gradientAtTrustRegionCenter.setEntry(j, gradientAtTrustRegionCenter.getEntry(j) + modelSecondDerivativesValues.getEntry(ih) * trustRegionCenterOffset.getEntry(i));
							}
							gradientAtTrustRegionCenter.setEntry(i, gradientAtTrustRegionCenter.getEntry(i) + modelSecondDerivativesValues.getEntry(ih) * trustRegionCenterOffset.getEntry(j));
							ih++;
						}
					}
					if (Evaluations > npt)
					{
						for (int k = 0; k < npt; k++)
						{
							double temp = ZERO;
							for (int j = 0; j < n; j++)
							{
								temp += interpolationPoints.getEntry(k, j) * trustRegionCenterOffset.getEntry(j);
							}
							temp *= modelSecondDerivativesParameters.getEntry(k);
							for (int i = 0; i < n; i++)
							{
								gradientAtTrustRegionCenter.setEntry(i, gradientAtTrustRegionCenter.getEntry(i) + temp * interpolationPoints.getEntry(k, i));
							}
						}
						// throw new PathIsExploredException(); // XXX
					}
				}

				// Generate the next point in the trust region that provides a small value
				// of the quadratic model subject to the constraints on the variables.
				// The int NTRITS is set to the number "trust region" iterations that
				// have occurred since the last "alternative" iteration. If the length
				// of XNEW-XOPT is less than HALF*RHO, however, then there is a branch to
				// label 650 or 680 with NTRITS=-1, instead of calculating F at XNEW.

			}
				goto case 60;
			case 60:
			{
				printState(60); // XXX
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector gnew = new org.apache.commons.math3.linear.ArrayRealVector(n);
				ArrayRealVector gnew = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector xbdi = new org.apache.commons.math3.linear.ArrayRealVector(n);
				ArrayRealVector xbdi = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector s = new org.apache.commons.math3.linear.ArrayRealVector(n);
				ArrayRealVector s = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector hs = new org.apache.commons.math3.linear.ArrayRealVector(n);
				ArrayRealVector hs = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector hred = new org.apache.commons.math3.linear.ArrayRealVector(n);
				ArrayRealVector hred = new ArrayRealVector(n);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dsqCrvmin = trsbox(delta, gnew, xbdi, s, hs, hred);
				double[] dsqCrvmin = trsbox(delta, gnew, xbdi, s, hs, hred);
				dsq = dsqCrvmin[0];
				crvmin = dsqCrvmin[1];

				// Computing MIN
				double deltaOne = delta;
				double deltaTwo = FastMath.sqrt(dsq);
				dnorm = FastMath.min(deltaOne, deltaTwo);
				if (dnorm < HALF * rho)
				{
					ntrits = -1;
					// Computing 2nd power
					deltaOne = TEN * rho;
					distsq = deltaOne * deltaOne;
					if (Evaluations <= nfsav + 2)
					{
						state = 650;
						break;
					}

					// The following choice between labels 650 and 680 depends on whether or
					// not our work with the current RHO seems to be complete. Either RHO is
					// decreased or termination occurs if the errors in the quadratic model at
					// the last three interpolation points compare favourably with predictions
					// of likely improvements to the model within distance HALF*RHO of XOPT.

					// Computing MAX
					deltaOne = FastMath.max(diffa, diffb);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double errbig = org.apache.commons.math3.util.FastMath.max(deltaOne, diffc);
					double errbig = FastMath.max(deltaOne, diffc);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double frhosq = rho * ONE_OVER_EIGHT * rho;
					double frhosq = rho * ONE_OVER_EIGHT * rho;
					if (crvmin > ZERO && errbig > frhosq * crvmin)
					{
						state = 650;
						break;
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bdtol = errbig / rho;
					double bdtol = errbig / rho;
					for (int j = 0; j < n; j++)
					{
						double bdtest = bdtol;
						if (newPoint.getEntry(j) == lowerDifference.getEntry(j))
						{
							bdtest = work1.getEntry(j);
						}
						if (newPoint.getEntry(j) == upperDifference.getEntry(j))
						{
							bdtest = -work1.getEntry(j);
						}
						if (bdtest < bdtol)
						{
							double curv = modelSecondDerivativesValues.getEntry((j + j * j) / 2);
							for (int k = 0; k < npt; k++)
							{
								// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = interpolationPoints.getEntry(k, j);
								double d1 = interpolationPoints.getEntry(k, j);
								curv += modelSecondDerivativesParameters.getEntry(k) * (d1 * d1);
							}
							bdtest += HALF * curv * rho;
							if (bdtest < bdtol)
							{
								state = 650;
								break;
							}
							// throw new PathIsExploredException(); // XXX
						}
					}
					state = 680;
					break;
				}
				++ntrits;

				// Severe cancellation is likely to occur if XOPT is too far from XBASE.
				// If the following test holds, then XBASE is shifted so that XOPT becomes
				// zero. The appropriate changes are made to BMAT and to the second
				// derivatives of the current model, beginning with the changes to BMAT
				// that do not depend on ZMAT. VLAG is used temporarily for working space.

			}
				goto case 90;
			case 90:
			{
				printState(90); // XXX
				if (dsq <= xoptsq * ONE_OVER_A_THOUSAND)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fracsq = xoptsq * ONE_OVER_FOUR;
					double fracsq = xoptsq * ONE_OVER_FOUR;
					double sumpq = ZERO;
					// final RealVector sumVector
					//     = new ArrayRealVector(npt, -HALF * xoptsq).add(interpolationPoints.operate(trustRegionCenter));
					for (int k = 0; k < npt; k++)
					{
						sumpq += modelSecondDerivativesParameters.getEntry(k);
						double sum = -HALF * xoptsq;
						for (int i = 0; i < n; i++)
						{
							sum += interpolationPoints.getEntry(k, i) * trustRegionCenterOffset.getEntry(i);
						}
						// sum = sumVector.getEntry(k); // XXX "testAckley" and "testDiffPow" fail.
						work2.setEntry(k, sum);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = fracsq - HALF * sum;
						double temp = fracsq - HALF * sum;
						for (int i = 0; i < n; i++)
						{
							work1.setEntry(i, bMatrix.getEntry(k, i));
							lagrangeValuesAtNewPoint.setEntry(i, sum * interpolationPoints.getEntry(k, i) + temp * trustRegionCenterOffset.getEntry(i));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ip = npt + i;
							int ip = npt + i;
							for (int j = 0; j <= i; j++)
							{
								bMatrix.setEntry(ip, j, bMatrix.getEntry(ip, j) + work1.getEntry(i) * lagrangeValuesAtNewPoint.getEntry(j) + lagrangeValuesAtNewPoint.getEntry(i) * work1.getEntry(j));
							}
						}
					}

					// Then the revisions of BMAT that depend on ZMAT are calculated.

					for (int m = 0; m < nptm; m++)
					{
						double sumz = ZERO;
						double sumw = ZERO;
						for (int k = 0; k < npt; k++)
						{
							sumz += zMatrix.getEntry(k, m);
							lagrangeValuesAtNewPoint.setEntry(k, work2.getEntry(k) * zMatrix.getEntry(k, m));
							sumw += lagrangeValuesAtNewPoint.getEntry(k);
						}
						for (int j = 0; j < n; j++)
						{
							double sum = (fracsq * sumz - HALF * sumw) * trustRegionCenterOffset.getEntry(j);
							for (int k = 0; k < npt; k++)
							{
								sum += lagrangeValuesAtNewPoint.getEntry(k) * interpolationPoints.getEntry(k, j);
							}
							work1.setEntry(j, sum);
							for (int k = 0; k < npt; k++)
							{
								bMatrix.setEntry(k, j, bMatrix.getEntry(k, j) + sum * zMatrix.getEntry(k, m));
							}
						}
						for (int i = 0; i < n; i++)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ip = i + npt;
							int ip = i + npt;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = work1.getEntry(i);
							double temp = work1.getEntry(i);
							for (int j = 0; j <= i; j++)
							{
								bMatrix.setEntry(ip, j, bMatrix.getEntry(ip, j) + temp * work1.getEntry(j));
							}
						}
					}

					// The following instructions complete the shift, including the changes
					// to the second derivative parameters of the quadratic model.

					int ih = 0;
					for (int j = 0; j < n; j++)
					{
						work1.setEntry(j, -HALF * sumpq * trustRegionCenterOffset.getEntry(j));
						for (int k = 0; k < npt; k++)
						{
							work1.setEntry(j, work1.getEntry(j) + modelSecondDerivativesParameters.getEntry(k) * interpolationPoints.getEntry(k, j));
							interpolationPoints.setEntry(k, j, interpolationPoints.getEntry(k, j) - trustRegionCenterOffset.getEntry(j));
						}
						for (int i = 0; i <= j; i++)
						{
							 modelSecondDerivativesValues.setEntry(ih, modelSecondDerivativesValues.getEntry(ih) + work1.getEntry(i) * trustRegionCenterOffset.getEntry(j) + trustRegionCenterOffset.getEntry(i) * work1.getEntry(j));
							bMatrix.setEntry(npt + i, j, bMatrix.getEntry(npt + j, i));
							ih++;
						}
					}
					for (int i = 0; i < n; i++)
					{
						originShift.setEntry(i, originShift.getEntry(i) + trustRegionCenterOffset.getEntry(i));
						newPoint.setEntry(i, newPoint.getEntry(i) - trustRegionCenterOffset.getEntry(i));
						lowerDifference.setEntry(i, lowerDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i));
						upperDifference.setEntry(i, upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i));
						trustRegionCenterOffset.setEntry(i, ZERO);
					}
					xoptsq = ZERO;
				}
				if (ntrits == 0)
				{
					state = 210;
					break;
				}
				state = 230;
				break;

				// XBASE is also moved to XOPT by a call of RESCUE. This calculation is
				// more expensive than the previous shift, because new matrices BMAT and
				// ZMAT are generated from scratch, which may include the replacement of
				// interpolation points whose positions seem to be causing near linear
				// dependence in the interpolation conditions. Therefore RESCUE is called
				// only if rounding errors have reduced by at least a factor of two the
				// denominator of the formula for updating the H matrix. It provides a
				// useful safeguard, but is not invoked in most applications of BOBYQA.

			}
			case 210:
			{
				printState(210); // XXX
				// Pick two alternative vectors of variables, relative to XBASE, that
				// are suitable as new positions of the KNEW-th interpolation point.
				// Firstly, XNEW is set to the point on a line through XOPT and another
				// interpolation point that minimizes the predicted value of the next
				// denominator, subject to ||XNEW - XOPT|| .LEQ. ADELT and to the SL
				// and SU bounds. Secondly, XALT is set to the best feasible point on
				// a constrained version of the Cauchy step of the KNEW-th Lagrange
				// function, the corresponding value of the square of this function
				// being returned in CAUCHY. The choice between these alternatives is
				// going to be made when the denominator is calculated.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] alphaCauchy = altmov(knew, adelt);
				double[] alphaCauchy = altmov(knew, adelt);
				alpha = alphaCauchy[0];
				cauchy = alphaCauchy[1];

				for (int i = 0; i < n; i++)
				{
					trialStepPoint.setEntry(i, newPoint.getEntry(i) - trustRegionCenterOffset.getEntry(i));
				}

				// Calculate VLAG and BETA for the current choice of D. The scalar
				// product of D with XPT(K,.) is going to be held in W(NPT+K) for
				// use when VQUAD is calculated.

			}
				goto case 230;
			case 230:
			{
				printState(230); // XXX
				for (int k = 0; k < npt; k++)
				{
					double suma = ZERO;
					double sumb = ZERO;
					double sum = ZERO;
					for (int j = 0; j < n; j++)
					{
						suma += interpolationPoints.getEntry(k, j) * trialStepPoint.getEntry(j);
						sumb += interpolationPoints.getEntry(k, j) * trustRegionCenterOffset.getEntry(j);
						sum += bMatrix.getEntry(k, j) * trialStepPoint.getEntry(j);
					}
					work3.setEntry(k, suma * (HALF * suma + sumb));
					lagrangeValuesAtNewPoint.setEntry(k, sum);
					work2.setEntry(k, suma);
				}
				beta = ZERO;
				for (int m = 0; m < nptm; m++)
				{
					double sum = ZERO;
					for (int k = 0; k < npt; k++)
					{
						sum += zMatrix.getEntry(k, m) * work3.getEntry(k);
					}
					beta -= sum * sum;
					for (int k = 0; k < npt; k++)
					{
						lagrangeValuesAtNewPoint.setEntry(k, lagrangeValuesAtNewPoint.getEntry(k) + sum * zMatrix.getEntry(k, m));
					}
				}
				dsq = ZERO;
				double bsum = ZERO;
				double dx = ZERO;
				for (int j = 0; j < n; j++)
				{
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = trialStepPoint.getEntry(j);
					double d1 = trialStepPoint.getEntry(j);
					dsq += d1 * d1;
					double sum = ZERO;
					for (int k = 0; k < npt; k++)
					{
						sum += work3.getEntry(k) * bMatrix.getEntry(k, j);
					}
					bsum += sum * trialStepPoint.getEntry(j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jp = npt + j;
					int jp = npt + j;
					for (int i = 0; i < n; i++)
					{
						sum += bMatrix.getEntry(jp, i) * trialStepPoint.getEntry(i);
					}
					lagrangeValuesAtNewPoint.setEntry(jp, sum);
					bsum += sum * trialStepPoint.getEntry(j);
					dx += trialStepPoint.getEntry(j) * trustRegionCenterOffset.getEntry(j);
				}

				beta = dx * dx + dsq * (xoptsq + dx + dx + HALF * dsq) + beta - bsum; // Original
				// beta += dx * dx + dsq * (xoptsq + dx + dx + HALF * dsq) - bsum; // XXX "testAckley" and "testDiffPow" fail.
				// beta = dx * dx + dsq * (xoptsq + 2 * dx + HALF * dsq) + beta - bsum; // XXX "testDiffPow" fails.

				lagrangeValuesAtNewPoint.setEntry(trustRegionCenterInterpolationPointIndex, lagrangeValuesAtNewPoint.getEntry(trustRegionCenterInterpolationPointIndex) + ONE);

				// If NTRITS is zero, the denominator may be increased by replacing
				// the step D of ALTMOV by a Cauchy step. Then RESCUE may be called if
				// rounding errors have damaged the chosen denominator.

				if (ntrits == 0)
				{
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = lagrangeValuesAtNewPoint.getEntry(knew);
					double d1 = lagrangeValuesAtNewPoint.getEntry(knew);
					denom = d1 * d1 + alpha * beta;
					if (denom < cauchy && cauchy > ZERO)
					{
						for (int i = 0; i < n; i++)
						{
							newPoint.setEntry(i, alternativeNewPoint.getEntry(i));
							trialStepPoint.setEntry(i, newPoint.getEntry(i) - trustRegionCenterOffset.getEntry(i));
						}
						cauchy = ZERO; // XXX Useful statement?
						state = 230;
						break;
					}
					// Alternatively, if NTRITS is positive, then set KNEW to the index of
					// the next interpolation point to be deleted to make room for a trust
					// region step. Again RESCUE may be called if rounding errors have damaged_
					// the chosen denominator, which is the reason for attempting to select
					// KNEW before calculating the next value of the objective function.

				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delsq = delta * delta;
					double delsq = delta * delta;
					scaden = ZERO;
					biglsq = ZERO;
					knew = 0;
					for (int k = 0; k < npt; k++)
					{
						if (k == trustRegionCenterInterpolationPointIndex)
						{
							continue;
						}
						double hdiag = ZERO;
						for (int m = 0; m < nptm; m++)
						{
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = zMatrix.getEntry(k, m);
							double d1 = zMatrix.getEntry(k, m);
							hdiag += d1 * d1;
						}
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = lagrangeValuesAtNewPoint.getEntry(k);
						double d2 = lagrangeValuesAtNewPoint.getEntry(k);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double den = beta * hdiag + d2 * d2;
						double den = beta * hdiag + d2 * d2;
						distsq = ZERO;
						for (int j = 0; j < n; j++)
						{
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = interpolationPoints.getEntry(k, j) - trustRegionCenterOffset.getEntry(j);
							double d3 = interpolationPoints.getEntry(k, j) - trustRegionCenterOffset.getEntry(j);
							distsq += d3 * d3;
						}
						// Computing MAX
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = distsq / delsq;
						double d4 = distsq / delsq;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = org.apache.commons.math3.util.FastMath.max(ONE, d4 * d4);
						double temp = FastMath.max(ONE, d4 * d4);
						if (temp * den > scaden)
						{
							scaden = temp * den;
							knew = k;
							denom = den;
						}
						// Computing MAX
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d5 = lagrangeValuesAtNewPoint.getEntry(k);
						double d5 = lagrangeValuesAtNewPoint.getEntry(k);
						biglsq = FastMath.max(biglsq, temp * (d5 * d5));
					}
				}

				// Put the variables for the next calculation of the objective function
				//   in XNEW, with any adjustments for the bounds.

				// Calculate the value of the objective function at XBASE+XNEW, unless
				//   the limit on the number of calculations of F has been reached.

			}
				goto case 360;
			case 360:
			{
				printState(360); // XXX
				for (int i = 0; i < n; i++)
				{
					// Computing MIN
					// Computing MAX
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = lowerBound[i];
					double d3 = lowerBound[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = originShift.getEntry(i) + newPoint.getEntry(i);
					double d4 = originShift.getEntry(i) + newPoint.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = org.apache.commons.math3.util.FastMath.max(d3, d4);
					double d1 = FastMath.max(d3, d4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = upperBound[i];
					double d2 = upperBound[i];
					currentBest.setEntry(i, FastMath.min(d1, d2));
					if (newPoint.getEntry(i) == lowerDifference.getEntry(i))
					{
						currentBest.setEntry(i, lowerBound[i]);
					}
					if (newPoint.getEntry(i) == upperDifference.getEntry(i))
					{
						currentBest.setEntry(i, upperBound[i]);
					}
				}

				f = computeObjectiveValue(currentBest.toArray());

				if (!isMinimize)
				{
					f = -f;
				}
				if (ntrits == -1)
				{
					fsave = f;
					state = 720;
					break;
				}

				// Use the quadratic model to predict the change in F due to the step D,
				//   and set DIFF to the error of this prediction.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fopt = fAtInterpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex);
				double fopt = fAtInterpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex);
				double vquad = ZERO;
				int ih = 0;
				for (int j = 0; j < n; j++)
				{
					vquad += trialStepPoint.getEntry(j) * gradientAtTrustRegionCenter.getEntry(j);
					for (int i = 0; i <= j; i++)
					{
						double temp = trialStepPoint.getEntry(i) * trialStepPoint.getEntry(j);
						if (i == j)
						{
							temp *= HALF;
						}
						vquad += modelSecondDerivativesValues.getEntry(ih) * temp;
						ih++;
					}
				}
				for (int k = 0; k < npt; k++)
				{
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = work2.getEntry(k);
					double d1 = work2.getEntry(k);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = d1 * d1;
					double d2 = d1 * d1; // "d1" must be squared first to prevent test failures.
					vquad += HALF * modelSecondDerivativesParameters.getEntry(k) * d2;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diff = f - fopt - vquad;
				double diff = f - fopt - vquad;
				diffc = diffb;
				diffb = diffa;
				diffa = FastMath.abs(diff);
				if (dnorm > rho)
				{
					nfsav = Evaluations;
				}

				// Pick the next value of DELTA after a trust region step.

				if (ntrits > 0)
				{
					if (vquad >= ZERO)
					{
						throw new MathIllegalStateException(LocalizedFormats.TRUST_REGION_STEP_FAILED, vquad);
					}
					ratio = (f - fopt) / vquad;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hDelta = HALF * delta;
					double hDelta = HALF * delta;
					if (ratio <= ONE_OVER_TEN)
					{
						// Computing MIN
						delta = FastMath.min(hDelta, dnorm);
					}
					else if (ratio <= .7)
					{
						// Computing MAX
						delta = FastMath.max(hDelta, dnorm);
					}
					else
					{
						// Computing MAX
						delta = FastMath.max(hDelta, 2 * dnorm);
					}
					if (delta <= rho * 1.5)
					{
						delta = rho;
					}

					// Recalculate KNEW and DENOM if the new F is less than FOPT.

					if (f < fopt)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ksav = knew;
						int ksav = knew;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double densav = denom;
						double densav = denom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double delsq = delta * delta;
						double delsq = delta * delta;
						scaden = ZERO;
						biglsq = ZERO;
						knew = 0;
						for (int k = 0; k < npt; k++)
						{
							double hdiag = ZERO;
							for (int m = 0; m < nptm; m++)
							{
								// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = zMatrix.getEntry(k, m);
								double d1 = zMatrix.getEntry(k, m);
								hdiag += d1 * d1;
							}
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = lagrangeValuesAtNewPoint.getEntry(k);
							double d1 = lagrangeValuesAtNewPoint.getEntry(k);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double den = beta * hdiag + d1 * d1;
							double den = beta * hdiag + d1 * d1;
							distsq = ZERO;
							for (int j = 0; j < n; j++)
							{
								// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = interpolationPoints.getEntry(k, j) - newPoint.getEntry(j);
								double d2 = interpolationPoints.getEntry(k, j) - newPoint.getEntry(j);
								distsq += d2 * d2;
							}
							// Computing MAX
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = distsq / delsq;
							double d3 = distsq / delsq;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = org.apache.commons.math3.util.FastMath.max(ONE, d3 * d3);
							double temp = FastMath.max(ONE, d3 * d3);
							if (temp * den > scaden)
							{
								scaden = temp * den;
								knew = k;
								denom = den;
							}
							// Computing MAX
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = lagrangeValuesAtNewPoint.getEntry(k);
							double d4 = lagrangeValuesAtNewPoint.getEntry(k);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d5 = temp * (d4 * d4);
							double d5 = temp * (d4 * d4);
							biglsq = FastMath.max(biglsq, d5);
						}
						if (scaden <= HALF * biglsq)
						{
							knew = ksav;
							denom = densav;
						}
					}
				}

				// Update BMAT and ZMAT, so that the KNEW-th interpolation point can be
				// moved. Also update the second derivative terms of the model.

				update(beta, denom, knew);

				ih = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pqold = modelSecondDerivativesParameters.getEntry(knew);
				double pqold = modelSecondDerivativesParameters.getEntry(knew);
				modelSecondDerivativesParameters.setEntry(knew, ZERO);
				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = pqold * interpolationPoints.getEntry(knew, i);
					double temp = pqold * interpolationPoints.getEntry(knew, i);
					for (int j = 0; j <= i; j++)
					{
						modelSecondDerivativesValues.setEntry(ih, modelSecondDerivativesValues.getEntry(ih) + temp * interpolationPoints.getEntry(knew, j));
						ih++;
					}
				}
				for (int m = 0; m < nptm; m++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = diff * zMatrix.getEntry(knew, m);
					double temp = diff * zMatrix.getEntry(knew, m);
					for (int k = 0; k < npt; k++)
					{
						modelSecondDerivativesParameters.setEntry(k, modelSecondDerivativesParameters.getEntry(k) + temp * zMatrix.getEntry(k, m));
					}
				}

				// Include the new interpolation point, and make the changes to GOPT at
				// the old XOPT that are caused by the updating of the quadratic model.

				fAtInterpolationPoints.setEntry(knew, f);
				for (int i = 0; i < n; i++)
				{
					interpolationPoints.setEntry(knew, i, newPoint.getEntry(i));
					work1.setEntry(i, bMatrix.getEntry(knew, i));
				}
				for (int k = 0; k < npt; k++)
				{
					double suma = ZERO;
					for (int m = 0; m < nptm; m++)
					{
						suma += zMatrix.getEntry(knew, m) * zMatrix.getEntry(k, m);
					}
					double sumb = ZERO;
					for (int j = 0; j < n; j++)
					{
						sumb += interpolationPoints.getEntry(k, j) * trustRegionCenterOffset.getEntry(j);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double temp = suma * sumb;
					double temp = suma * sumb;
					for (int i = 0; i < n; i++)
					{
						work1.setEntry(i, work1.getEntry(i) + temp * interpolationPoints.getEntry(k, i));
					}
				}
				for (int i = 0; i < n; i++)
				{
					gradientAtTrustRegionCenter.setEntry(i, gradientAtTrustRegionCenter.getEntry(i) + diff * work1.getEntry(i));
				}

				// Update XOPT, GOPT and KOPT if the new calculated F is less than FOPT.

				if (f < fopt)
				{
					trustRegionCenterInterpolationPointIndex = knew;
					xoptsq = ZERO;
					ih = 0;
					for (int j = 0; j < n; j++)
					{
						trustRegionCenterOffset.setEntry(j, newPoint.getEntry(j));
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = trustRegionCenterOffset.getEntry(j);
						double d1 = trustRegionCenterOffset.getEntry(j);
						xoptsq += d1 * d1;
						for (int i = 0; i <= j; i++)
						{
							if (i < j)
							{
								gradientAtTrustRegionCenter.setEntry(j, gradientAtTrustRegionCenter.getEntry(j) + modelSecondDerivativesValues.getEntry(ih) * trialStepPoint.getEntry(i));
							}
							gradientAtTrustRegionCenter.setEntry(i, gradientAtTrustRegionCenter.getEntry(i) + modelSecondDerivativesValues.getEntry(ih) * trialStepPoint.getEntry(j));
							ih++;
						}
					}
					for (int k = 0; k < npt; k++)
					{
						double temp = ZERO;
						for (int j = 0; j < n; j++)
						{
							temp += interpolationPoints.getEntry(k, j) * trialStepPoint.getEntry(j);
						}
						temp *= modelSecondDerivativesParameters.getEntry(k);
						for (int i = 0; i < n; i++)
						{
							gradientAtTrustRegionCenter.setEntry(i, gradientAtTrustRegionCenter.getEntry(i) + temp * interpolationPoints.getEntry(k, i));
						}
					}
				}

				// Calculate the parameters of the least Frobenius norm interpolant to
				// the current data, the gradient of this interpolant at XOPT being put
				// into VLAG(NPT+I), I=1,2,...,N.

				if (ntrits > 0)
				{
					for (int k = 0; k < npt; k++)
					{
						lagrangeValuesAtNewPoint.setEntry(k, fAtInterpolationPoints.getEntry(k) - fAtInterpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex));
						work3.setEntry(k, ZERO);
					}
					for (int j = 0; j < nptm; j++)
					{
						double sum = ZERO;
						for (int k = 0; k < npt; k++)
						{
							sum += zMatrix.getEntry(k, j) * lagrangeValuesAtNewPoint.getEntry(k);
						}
						for (int k = 0; k < npt; k++)
						{
							work3.setEntry(k, work3.getEntry(k) + sum * zMatrix.getEntry(k, j));
						}
					}
					for (int k = 0; k < npt; k++)
					{
						double sum = ZERO;
						for (int j = 0; j < n; j++)
						{
							sum += interpolationPoints.getEntry(k, j) * trustRegionCenterOffset.getEntry(j);
						}
						work2.setEntry(k, work3.getEntry(k));
						work3.setEntry(k, sum * work3.getEntry(k));
					}
					double gqsq = ZERO;
					double gisq = ZERO;
					for (int i = 0; i < n; i++)
					{
						double sum = ZERO;
						for (int k = 0; k < npt; k++)
						{
							sum += bMatrix.getEntry(k, i) * lagrangeValuesAtNewPoint.getEntry(k) + interpolationPoints.getEntry(k, i) * work3.getEntry(k);
						}
						if (trustRegionCenterOffset.getEntry(i) == lowerDifference.getEntry(i))
						{
							// Computing MIN
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = org.apache.commons.math3.util.FastMath.min(ZERO, gradientAtTrustRegionCenter.getEntry(i));
							double d1 = FastMath.min(ZERO, gradientAtTrustRegionCenter.getEntry(i));
							gqsq += d1 * d1;
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = org.apache.commons.math3.util.FastMath.min(ZERO, sum);
							double d2 = FastMath.min(ZERO, sum);
							gisq += d2 * d2;
						}
						else if (trustRegionCenterOffset.getEntry(i) == upperDifference.getEntry(i))
						{
							// Computing MAX
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = org.apache.commons.math3.util.FastMath.max(ZERO, gradientAtTrustRegionCenter.getEntry(i));
							double d1 = FastMath.max(ZERO, gradientAtTrustRegionCenter.getEntry(i));
							gqsq += d1 * d1;
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = org.apache.commons.math3.util.FastMath.max(ZERO, sum);
							double d2 = FastMath.max(ZERO, sum);
							gisq += d2 * d2;
						}
						else
						{
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = gradientAtTrustRegionCenter.getEntry(i);
							double d1 = gradientAtTrustRegionCenter.getEntry(i);
							gqsq += d1 * d1;
							gisq += sum * sum;
						}
						lagrangeValuesAtNewPoint.setEntry(npt + i, sum);
					}

					// Test whether to replace the new quadratic model by the least Frobenius
					// norm interpolant, making the replacement if the test is satisfied.

					++itest;
					if (gqsq < TEN * gisq)
					{
						itest = 0;
					}
					if (itest >= 3)
					{
						for (int i = 0, max = FastMath.max(npt, nh); i < max; i++)
						{
							if (i < n)
							{
								gradientAtTrustRegionCenter.setEntry(i, lagrangeValuesAtNewPoint.getEntry(npt + i));
							}
							if (i < npt)
							{
								modelSecondDerivativesParameters.setEntry(i, work2.getEntry(i));
							}
							if (i < nh)
							{
								modelSecondDerivativesValues.setEntry(i, ZERO);
							}
							itest = 0;
						}
					}
				}

				// If a trust region step has provided a sufficient decrease in F, then
				// branch for another trust region calculation. The case NTRITS=0 occurs
				// when the new interpolation point was reached by an alternative step.

				if (ntrits == 0)
				{
					state = 60;
					break;
				}
				if (f <= fopt + ONE_OVER_TEN * vquad)
				{
					state = 60;
					break;
				}

				// Alternatively, find out if the interpolation points are close enough
				//   to the best point so far.

				// Computing MAX
				// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = TWO * delta;
				double d1 = TWO * delta;
				// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = TEN * rho;
				double d2 = TEN * rho;
				distsq = FastMath.max(d1 * d1, d2 * d2);
			}
				goto case 650;
			case 650:
			{
				printState(650); // XXX
				knew = -1;
				for (int k = 0; k < npt; k++)
				{
					double sum = ZERO;
					for (int j = 0; j < n; j++)
					{
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = interpolationPoints.getEntry(k, j) - trustRegionCenterOffset.getEntry(j);
						double d1 = interpolationPoints.getEntry(k, j) - trustRegionCenterOffset.getEntry(j);
						sum += d1 * d1;
					}
					if (sum > distsq)
					{
						knew = k;
						distsq = sum;
					}
				}

				// If KNEW is positive, then ALTMOV finds alternative new positions for
				// the KNEW-th interpolation point within distance ADELT of XOPT. It is
				// reached via label 90. Otherwise, there is a branch to label 60 for
				// another trust region iteration, unless the calculations with the
				// current RHO are complete.

				if (knew >= 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dist = org.apache.commons.math3.util.FastMath.sqrt(distsq);
					double dist = FastMath.sqrt(distsq);
					if (ntrits == -1)
					{
						// Computing MIN
						delta = FastMath.min(ONE_OVER_TEN * delta, HALF * dist);
						if (delta <= rho * 1.5)
						{
							delta = rho;
						}
					}
					ntrits = 0;
					// Computing MAX
					// Computing MIN
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = org.apache.commons.math3.util.FastMath.min(ONE_OVER_TEN * dist, delta);
					double d1 = FastMath.min(ONE_OVER_TEN * dist, delta);
					adelt = FastMath.max(d1, rho);
					dsq = adelt * adelt;
					state = 90;
					break;
				}
				if (ntrits == -1)
				{
					state = 680;
					break;
				}
				if (ratio > ZERO)
				{
					state = 60;
					break;
				}
				if (FastMath.max(delta, dnorm) > rho)
				{
					state = 60;
					break;
				}

				// The calculations with the current value of RHO are complete. Pick the
				//   next values of RHO and DELTA.
			}
				goto case 680;
			case 680:
			{
				printState(680); // XXX
				if (rho > stoppingTrustRegionRadius)
				{
					delta = HALF * rho;
					ratio = rho / stoppingTrustRegionRadius;
					if (ratio <= SIXTEEN)
					{
						rho = stoppingTrustRegionRadius;
					}
					else if (ratio <= TWO_HUNDRED_FIFTY)
					{
						rho = FastMath.sqrt(ratio) * stoppingTrustRegionRadius;
					}
					else
					{
						rho *= ONE_OVER_TEN;
					}
					delta = FastMath.max(delta, rho);
					ntrits = 0;
					nfsav = Evaluations;
					state = 60;
					break;
				}

				// Return from the calculation, after another Newton-Raphson step, if
				//   it is too short to have been tried before.

				if (ntrits == -1)
				{
					state = 360;
					break;
				}
			}
				goto case 720;
			case 720:
			{
				printState(720); // XXX
				if (fAtInterpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex) <= fsave)
				{
					for (int i = 0; i < n; i++)
					{
						// Computing MIN
						// Computing MAX
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = lowerBound[i];
						double d3 = lowerBound[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = originShift.getEntry(i) + trustRegionCenterOffset.getEntry(i);
						double d4 = originShift.getEntry(i) + trustRegionCenterOffset.getEntry(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = org.apache.commons.math3.util.FastMath.max(d3, d4);
						double d1 = FastMath.max(d3, d4);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = upperBound[i];
						double d2 = upperBound[i];
						currentBest.setEntry(i, FastMath.min(d1, d2));
						if (trustRegionCenterOffset.getEntry(i) == lowerDifference.getEntry(i))
						{
							currentBest.setEntry(i, lowerBound[i]);
						}
						if (trustRegionCenterOffset.getEntry(i) == upperDifference.getEntry(i))
						{
							currentBest.setEntry(i, upperBound[i]);
						}
					}
					f = fAtInterpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex);
				}
				return f;
			}
			default:
			{
				throw new MathIllegalStateException(LocalizedFormats.SIMPLE_MESSAGE, "bobyqb");
			}
				}
			}
		} // bobyqb

		// ----------------------------------------------------------------------------------------

		/// <summary>
		///     The arguments N, NPT, XPT, XOPT, BMAT, ZMAT, NDIM, SL and SU all have
		///       the same meanings as the corresponding arguments of BOBYQB.
		///     KOPT is the index of the optimal interpolation point.
		///     KNEW is the index of the interpolation point that is going to be moved.
		///     ADELT is the current trust region bound.
		///     XNEW will be set to a suitable new position for the interpolation point
		///       XPT(KNEW,.). Specifically, it satisfies the SL, SU and trust region
		///       bounds and it should provide a large denominator in the next call of
		///       UPDATE. The step XNEW-XOPT from XOPT is restricted to moves along the
		///       straight lines through XOPT and another interpolation point.
		///     XALT also provides a large value of the modulus of the KNEW-th Lagrange
		///       function subject to the constraints that have been mentioned, its main
		///       difference from XNEW being that XALT-XOPT is a constrained version of
		///       the Cauchy step within the trust region. An exception is that XALT is
		///       not calculated if all components of GLAG (see below) are zero.
		///     ALPHA will be set to the KNEW-th diagonal element of the H matrix.
		///     CAUCHY will be set to the square of the KNEW-th Lagrange function at
		///       the step XALT-XOPT from XOPT for the vector XALT that is returned,
		///       except that CAUCHY is set to zero if XALT is not calculated.
		///     GLAG is a working space vector of length N for the gradient of the
		///       KNEW-th Lagrange function at XOPT.
		///     HCOL is a working space vector of length NPT for the second derivative
		///       coefficients of the KNEW-th Lagrange function.
		///     W is a working space vector of length 2N that is going to hold the
		///       constrained Cauchy step from XOPT of the Lagrange function, followed
		///       by the downhill version of XALT when the uphill step is calculated.
		/// 
		///     Set the first NPT components of W to the leading elements of the
		///     KNEW-th column of the H matrix. </summary>
		/// <param name="knew"> </param>
		/// <param name="adelt"> </param>
		private double[] altmov(int knew, double adelt)
		{
			printMethod(); // XXX

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = currentBest.getDimension();
			int n = currentBest.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int npt = numberOfInterpolationPoints;
			int npt = numberOfInterpolationPoints;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector glag = new org.apache.commons.math3.linear.ArrayRealVector(n);
			ArrayRealVector glag = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector hcol = new org.apache.commons.math3.linear.ArrayRealVector(npt);
			ArrayRealVector hcol = new ArrayRealVector(npt);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector work1 = new org.apache.commons.math3.linear.ArrayRealVector(n);
			ArrayRealVector work1 = new ArrayRealVector(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector work2 = new org.apache.commons.math3.linear.ArrayRealVector(n);
			ArrayRealVector work2 = new ArrayRealVector(n);

			for (int k = 0; k < npt; k++)
			{
				hcol.setEntry(k, ZERO);
			}
			for (int j = 0, max = npt - n - 1; j < max; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = zMatrix.getEntry(knew, j);
				double tmp = zMatrix.getEntry(knew, j);
				for (int k = 0; k < npt; k++)
				{
					hcol.setEntry(k, hcol.getEntry(k) + tmp * zMatrix.getEntry(k, j));
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = hcol.getEntry(knew);
			double alpha = hcol.getEntry(knew);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ha = HALF * alpha;
			double ha = HALF * alpha;

			// Calculate the gradient of the KNEW-th Lagrange function at XOPT.

			for (int i = 0; i < n; i++)
			{
				glag.setEntry(i, bMatrix.getEntry(knew, i));
			}
			for (int k = 0; k < npt; k++)
			{
				double tmp = ZERO;
				for (int j = 0; j < n; j++)
				{
					tmp += interpolationPoints.getEntry(k, j) * trustRegionCenterOffset.getEntry(j);
				}
				tmp *= hcol.getEntry(k);
				for (int i = 0; i < n; i++)
				{
					glag.setEntry(i, glag.getEntry(i) + tmp * interpolationPoints.getEntry(k, i));
				}
			}

			// Search for a large denominator along the straight lines through XOPT
			// and another interpolation point. SLBD and SUBD will be lower and upper
			// bounds on the step along each of these lines in turn. PREDSQ will be
			// set to the square of the predicted denominator for each line. PRESAV
			// will be set to the largest admissible value of PREDSQ that occurs.

			double presav = ZERO;
			double step = double.NaN;
			int ksav = 0;
			int ibdsav = 0;
			double stpsav = 0;
			for (int k = 0; k < npt; k++)
			{
				if (k == trustRegionCenterInterpolationPointIndex)
				{
					continue;
				}
				double dderiv = ZERO;
				double distsq = ZERO;
				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = interpolationPoints.getEntry(k, i) - trustRegionCenterOffset.getEntry(i);
					double tmp = interpolationPoints.getEntry(k, i) - trustRegionCenterOffset.getEntry(i);
					dderiv += glag.getEntry(i) * tmp;
					distsq += tmp * tmp;
				}
				double subd = adelt / FastMath.sqrt(distsq);
				double slbd = -subd;
				int ilbd = 0;
				int iubd = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sumin = org.apache.commons.math3.util.FastMath.min(ONE, subd);
				double sumin = FastMath.min(ONE, subd);

				// Revise SLBD and SUBD if necessary because of the bounds in SL and SU.

				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = interpolationPoints.getEntry(k, i) - trustRegionCenterOffset.getEntry(i);
					double tmp = interpolationPoints.getEntry(k, i) - trustRegionCenterOffset.getEntry(i);
					if (tmp > ZERO)
					{
						if (slbd * tmp < lowerDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i))
						{
							slbd = (lowerDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i)) / tmp;
							ilbd = -i - 1;
						}
						if (subd * tmp > upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i))
						{
							// Computing MAX
							subd = FastMath.max(sumin, (upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i)) / tmp);
							iubd = i + 1;
						}
					}
					else if (tmp < ZERO)
					{
						if (slbd * tmp > upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i))
						{
							slbd = (upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i)) / tmp;
							ilbd = i + 1;
						}
						if (subd * tmp < lowerDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i))
						{
							// Computing MAX
							subd = FastMath.max(sumin, (lowerDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i)) / tmp);
							iubd = -i - 1;
						}
					}
				}

				// Seek a large modulus of the KNEW-th Lagrange function when the index
				// of the other interpolation point on the line through XOPT is KNEW.

				step = slbd;
				int isbd = ilbd;
				double vlag = double.NaN;
				if (k == knew)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diff = dderiv - ONE;
					double diff = dderiv - ONE;
					vlag = slbd * (dderiv - slbd * diff);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = subd * (dderiv - subd * diff);
					double d1 = subd * (dderiv - subd * diff);
					if (FastMath.abs(d1) > FastMath.abs(vlag))
					{
						step = subd;
						vlag = d1;
						isbd = iubd;
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = HALF * dderiv;
					double d2 = HALF * dderiv;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = d2 - diff * slbd;
					double d3 = d2 - diff * slbd;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = d2 - diff * subd;
					double d4 = d2 - diff * subd;
					if (d3 * d4 < ZERO)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d5 = d2 * d2 / diff;
						double d5 = d2 * d2 / diff;
						if (FastMath.abs(d5) > FastMath.abs(vlag))
						{
							step = d2 / diff;
							vlag = d5;
							isbd = 0;
						}
					}

					// Search along each of the other lines through XOPT and another point.

				}
				else
				{
					vlag = slbd * (ONE - slbd);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = subd * (ONE - subd);
					double tmp = subd * (ONE - subd);
					if (FastMath.abs(tmp) > FastMath.abs(vlag))
					{
						step = subd;
						vlag = tmp;
						isbd = iubd;
					}
					if (subd > HALF && FastMath.abs(vlag) < ONE_OVER_FOUR)
					{
						step = HALF;
						vlag = ONE_OVER_FOUR;
						isbd = 0;
					}
					vlag *= dderiv;
				}

				// Calculate PREDSQ for the current line search and maintain PRESAV.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = step * (ONE - step) * distsq;
				double tmp = step * (ONE - step) * distsq;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double predsq = vlag * vlag * (vlag * vlag + ha * tmp * tmp);
				double predsq = vlag * vlag * (vlag * vlag + ha * tmp * tmp);
				if (predsq > presav)
				{
					presav = predsq;
					ksav = k;
					stpsav = step;
					ibdsav = isbd;
				}
			}

			// Construct XNEW in a way that satisfies the bound constraints exactly.

			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = trustRegionCenterOffset.getEntry(i) + stpsav * (interpolationPoints.getEntry(ksav, i) - trustRegionCenterOffset.getEntry(i));
				double tmp = trustRegionCenterOffset.getEntry(i) + stpsav * (interpolationPoints.getEntry(ksav, i) - trustRegionCenterOffset.getEntry(i));
				newPoint.setEntry(i, FastMath.max(lowerDifference.getEntry(i), FastMath.min(upperDifference.getEntry(i), tmp)));
			}
			if (ibdsav < 0)
			{
				newPoint.setEntry(-ibdsav - 1, lowerDifference.getEntry(-ibdsav - 1));
			}
			if (ibdsav > 0)
			{
				newPoint.setEntry(ibdsav - 1, upperDifference.getEntry(ibdsav - 1));
			}

			// Prepare for the iterative method that assembles the constrained Cauchy
			// step in W. The sum of squares of the fixed components of W is formed in
			// WFIXSQ, and the free components of W are set to BIGSTP.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bigstp = adelt + adelt;
			double bigstp = adelt + adelt;
			int iflag = 0;
			double cauchy = double.NaN;
			double csave = ZERO;
			while (true)
			{
				double wfixsq = ZERO;
				double ggfree = ZERO;
				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double glagValue = glag.getEntry(i);
					double glagValue = glag.getEntry(i);
					work1.setEntry(i, ZERO);
					if (FastMath.min(trustRegionCenterOffset.getEntry(i) - lowerDifference.getEntry(i), glagValue) > ZERO || FastMath.max(trustRegionCenterOffset.getEntry(i) - upperDifference.getEntry(i), glagValue) < ZERO)
					{
						work1.setEntry(i, bigstp);
						// Computing 2nd power
						ggfree += glagValue * glagValue;
					}
				}
				if (ggfree == ZERO)
				{
					return new double[] {alpha, ZERO};
				}

				// Investigate whether more components of W can be fixed.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp1 = adelt * adelt - wfixsq;
				double tmp1 = adelt * adelt - wfixsq;
				if (tmp1 > ZERO)
				{
					step = FastMath.sqrt(tmp1 / ggfree);
					ggfree = ZERO;
					for (int i = 0; i < n; i++)
					{
						if (work1.getEntry(i) == bigstp)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp2 = trustRegionCenterOffset.getEntry(i) - step * glag.getEntry(i);
							double tmp2 = trustRegionCenterOffset.getEntry(i) - step * glag.getEntry(i);
							if (tmp2 <= lowerDifference.getEntry(i))
							{
								work1.setEntry(i, lowerDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i));
								// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = work1.getEntry(i);
								double d1 = work1.getEntry(i);
								wfixsq += d1 * d1;
							}
							else if (tmp2 >= upperDifference.getEntry(i))
							{
								work1.setEntry(i, upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i));
								// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = work1.getEntry(i);
								double d1 = work1.getEntry(i);
								wfixsq += d1 * d1;
							}
							else
							{
								// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = glag.getEntry(i);
								double d1 = glag.getEntry(i);
								ggfree += d1 * d1;
							}
						}
					}
				}

				// Set the remaining free components of W and all components of XALT,
				// except that W may be scaled later.

				double gw = ZERO;
				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double glagValue = glag.getEntry(i);
					double glagValue = glag.getEntry(i);
					if (work1.getEntry(i) == bigstp)
					{
						work1.setEntry(i, -step * glagValue);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double min = org.apache.commons.math3.util.FastMath.min(upperDifference.getEntry(i), trustRegionCenterOffset.getEntry(i) + work1.getEntry(i));
						double min = FastMath.min(upperDifference.getEntry(i), trustRegionCenterOffset.getEntry(i) + work1.getEntry(i));
						alternativeNewPoint.setEntry(i, FastMath.max(lowerDifference.getEntry(i), min));
					}
					else if (work1.getEntry(i) == ZERO)
					{
						alternativeNewPoint.setEntry(i, trustRegionCenterOffset.getEntry(i));
					}
					else if (glagValue > ZERO)
					{
						alternativeNewPoint.setEntry(i, lowerDifference.getEntry(i));
					}
					else
					{
						alternativeNewPoint.setEntry(i, upperDifference.getEntry(i));
					}
					gw += glagValue * work1.getEntry(i);
				}

				// Set CURV to the curvature of the KNEW-th Lagrange function along W.
				// Scale W by a factor less than one if that can reduce the modulus of
				// the Lagrange function at XOPT+W. Set CAUCHY to the final value of
				// the square of this function.

				double curv = ZERO;
				for (int k = 0; k < npt; k++)
				{
					double tmp = ZERO;
					for (int j = 0; j < n; j++)
					{
						tmp += interpolationPoints.getEntry(k, j) * work1.getEntry(j);
					}
					curv += hcol.getEntry(k) * tmp * tmp;
				}
				if (iflag == 1)
				{
					curv = -curv;
				}
				if (curv > -gw && curv < -gw * (ONE + FastMath.sqrt(TWO)))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scale = -gw / curv;
					double scale = -gw / curv;
					for (int i = 0; i < n; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = trustRegionCenterOffset.getEntry(i) + scale * work1.getEntry(i);
						double tmp = trustRegionCenterOffset.getEntry(i) + scale * work1.getEntry(i);
						alternativeNewPoint.setEntry(i, FastMath.max(lowerDifference.getEntry(i), FastMath.min(upperDifference.getEntry(i), tmp)));
					}
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = HALF * gw * scale;
					double d1 = HALF * gw * scale;
					cauchy = d1 * d1;
				}
				else
				{
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = gw + HALF * curv;
					double d1 = gw + HALF * curv;
					cauchy = d1 * d1;
				}

				// If IFLAG is zero, then XALT is calculated as before after reversing
				// the sign of GLAG. Thus two XALT vectors become available. The one that
				// is chosen is the one that gives the larger value of CAUCHY.

				if (iflag == 0)
				{
					for (int i = 0; i < n; i++)
					{
						glag.setEntry(i, -glag.getEntry(i));
						work2.setEntry(i, alternativeNewPoint.getEntry(i));
					}
					csave = cauchy;
					iflag = 1;
				}
				else
				{
					break;
				}
			}
			if (csave > cauchy)
			{
				for (int i = 0; i < n; i++)
				{
					alternativeNewPoint.setEntry(i, work2.getEntry(i));
				}
				cauchy = csave;
			}

			return new double[] {alpha, cauchy};
		} // altmov

		// ----------------------------------------------------------------------------------------

		/// <summary>
		///     SUBROUTINE PRELIM sets the elements of XBASE, XPT, FVAL, GOPT, HQ, PQ,
		///     BMAT and ZMAT for the first iteration, and it maintains the values of
		///     NF and KOPT. The vector X is also changed by PRELIM.
		/// 
		///     The arguments N, NPT, X, XL, XU, RHOBEG, IPRINT and MAXFUN are the
		///       same as the corresponding arguments in SUBROUTINE BOBYQA.
		///     The arguments XBASE, XPT, FVAL, HQ, PQ, BMAT, ZMAT, NDIM, SL and SU
		///       are the same as the corresponding arguments in BOBYQB, the elements
		///       of SL and SU being set in BOBYQA.
		///     GOPT is usually the gradient of the quadratic model at XOPT+XBASE, but
		///       it is set by PRELIM to the gradient of the quadratic model at XBASE.
		///       If XOPT is nonzero, BOBYQB will change it to its usual value later.
		///     NF is maintaned as the number of calls of CALFUN so far.
		///     KOPT will be such that the least calculated value of F so far is at
		///       the point XPT(KOPT,.)+XBASE in the space of the variables.
		/// </summary>
		/// <param name="lowerBound"> Lower bounds. </param>
		/// <param name="upperBound"> Upper bounds. </param>
		private void prelim(double[] lowerBound, double[] upperBound)
		{
			printMethod(); // XXX

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = currentBest.getDimension();
			int n = currentBest.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int npt = numberOfInterpolationPoints;
			int npt = numberOfInterpolationPoints;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ndim = bMatrix.getRowDimension();
			int ndim = bMatrix.RowDimension;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rhosq = initialTrustRegionRadius * initialTrustRegionRadius;
			double rhosq = initialTrustRegionRadius * initialTrustRegionRadius;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double recip = 1d / rhosq;
			double recip = 1d / rhosq;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int np = n + 1;
			int np = n + 1;

			// Set XBASE to the initial vector of variables, and set the initial
			// elements of XPT, BMAT, HQ, PQ and ZMAT to zero.

			for (int j = 0; j < n; j++)
			{
				originShift.setEntry(j, currentBest.getEntry(j));
				for (int k = 0; k < npt; k++)
				{
					interpolationPoints.setEntry(k, j, ZERO);
				}
				for (int i = 0; i < ndim; i++)
				{
					bMatrix.setEntry(i, j, ZERO);
				}
			}
			for (int i = 0, max = n * np / 2; i < max; i++)
			{
				modelSecondDerivativesValues.setEntry(i, ZERO);
			}
			for (int k = 0; k < npt; k++)
			{
				modelSecondDerivativesParameters.setEntry(k, ZERO);
				for (int j = 0, max = npt - np; j < max; j++)
				{
					zMatrix.setEntry(k, j, ZERO);
				}
			}

			// Begin the initialization procedure. NF becomes one more than the number
			// of function values so far. The coordinates of the displacement of the
			// next initial interpolation point from XBASE are set in XPT(NF+1,.).

			int ipt = 0;
			int jpt = 0;
			double fbeg = double.NaN;
			do
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nfm = getEvaluations();
				int nfm = Evaluations;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nfx = nfm - n;
				int nfx = nfm - n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nfmm = nfm - 1;
				int nfmm = nfm - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nfxm = nfx - 1;
				int nfxm = nfx - 1;
				double stepa = 0;
				double stepb = 0;
				if (nfm <= 2 * n)
				{
					if (nfm >= 1 && nfm <= n)
					{
						stepa = initialTrustRegionRadius;
						if (upperDifference.getEntry(nfmm) == ZERO)
						{
							stepa = -stepa;
							// throw new PathIsExploredException(); // XXX
						}
						interpolationPoints.setEntry(nfm, nfmm, stepa);
					}
					else if (nfm > n)
					{
						stepa = interpolationPoints.getEntry(nfx, nfxm);
						stepb = -initialTrustRegionRadius;
						if (lowerDifference.getEntry(nfxm) == ZERO)
						{
							stepb = FastMath.min(TWO * initialTrustRegionRadius, upperDifference.getEntry(nfxm));
							// throw new PathIsExploredException(); // XXX
						}
						if (upperDifference.getEntry(nfxm) == ZERO)
						{
							stepb = FastMath.max(-TWO * initialTrustRegionRadius, lowerDifference.getEntry(nfxm));
							// throw new PathIsExploredException(); // XXX
						}
						interpolationPoints.setEntry(nfm, nfxm, stepb);
					}
				}
				else
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int tmp1 = (nfm - np) / n;
					int tmp1 = (nfm - np) / n;
					jpt = nfm - tmp1 * n - n;
					ipt = jpt + tmp1;
					if (ipt > n)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int tmp2 = jpt;
						int tmp2 = jpt;
						jpt = ipt - n;
						ipt = tmp2;
	//                     throw new PathIsExploredException(); // XXX
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iptMinus1 = ipt - 1;
					int iptMinus1 = ipt - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jptMinus1 = jpt - 1;
					int jptMinus1 = jpt - 1;
					interpolationPoints.setEntry(nfm, iptMinus1, interpolationPoints.getEntry(ipt, iptMinus1));
					interpolationPoints.setEntry(nfm, jptMinus1, interpolationPoints.getEntry(jpt, jptMinus1));
				}

				// Calculate the next value of F. The least function value so far and
				// its index are required.

				for (int j = 0; j < n; j++)
				{
					currentBest.setEntry(j, FastMath.min(FastMath.max(lowerBound[j], originShift.getEntry(j) + interpolationPoints.getEntry(nfm, j)), upperBound[j]));
					if (interpolationPoints.getEntry(nfm, j) == lowerDifference.getEntry(j))
					{
						currentBest.setEntry(j, lowerBound[j]);
					}
					if (interpolationPoints.getEntry(nfm, j) == upperDifference.getEntry(j))
					{
						currentBest.setEntry(j, upperBound[j]);
					}
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double objectiveValue = computeObjectiveValue(currentBest.toArray());
				double objectiveValue = computeObjectiveValue(currentBest.toArray());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = isMinimize ? objectiveValue : -objectiveValue;
				double f = isMinimize ? objectiveValue : -objectiveValue;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numEval = getEvaluations();
				int numEval = Evaluations; // nfm + 1
				fAtInterpolationPoints.setEntry(nfm, f);

				if (numEval == 1)
				{
					fbeg = f;
					trustRegionCenterInterpolationPointIndex = 0;
				}
				else if (f < fAtInterpolationPoints.getEntry(trustRegionCenterInterpolationPointIndex))
				{
					trustRegionCenterInterpolationPointIndex = nfm;
				}

				// Set the nonzero initial elements of BMAT and the quadratic model in the
				// cases when NF is at most 2*N+1. If NF exceeds N+1, then the positions
				// of the NF-th and (NF-N)-th interpolation points may be switched, in
				// order that the function value at the first of them contributes to the
				// off-diagonal second derivative terms of the initial quadratic model.

				if (numEval <= 2 * n + 1)
				{
					if (numEval >= 2 && numEval <= n + 1)
					{
						gradientAtTrustRegionCenter.setEntry(nfmm, (f - fbeg) / stepa);
						if (npt < numEval + n)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oneOverStepA = ONE / stepa;
							double oneOverStepA = ONE / stepa;
							bMatrix.setEntry(0, nfmm, -oneOverStepA);
							bMatrix.setEntry(nfm, nfmm, oneOverStepA);
							bMatrix.setEntry(npt + nfmm, nfmm, -HALF * rhosq);
							// throw new PathIsExploredException(); // XXX
						}
					}
					else if (numEval >= n + 2)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ih = nfx * (nfx + 1) / 2 - 1;
						int ih = nfx * (nfx + 1) / 2 - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = (f - fbeg) / stepb;
						double tmp = (f - fbeg) / stepb;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double diff = stepb - stepa;
						double diff = stepb - stepa;
						modelSecondDerivativesValues.setEntry(ih, TWO * (tmp - gradientAtTrustRegionCenter.getEntry(nfxm)) / diff);
						gradientAtTrustRegionCenter.setEntry(nfxm, (gradientAtTrustRegionCenter.getEntry(nfxm) * stepb - tmp * stepa) / diff);
						if (stepa * stepb < ZERO && f < fAtInterpolationPoints.getEntry(nfm - n))
						{
							fAtInterpolationPoints.setEntry(nfm, fAtInterpolationPoints.getEntry(nfm - n));
							fAtInterpolationPoints.setEntry(nfm - n, f);
							if (trustRegionCenterInterpolationPointIndex == nfm)
							{
								trustRegionCenterInterpolationPointIndex = nfm - n;
							}
							interpolationPoints.setEntry(nfm - n, nfxm, stepb);
							interpolationPoints.setEntry(nfm, nfxm, stepa);
						}
						bMatrix.setEntry(0, nfxm, -(stepa + stepb) / (stepa * stepb));
						bMatrix.setEntry(nfm, nfxm, -HALF / interpolationPoints.getEntry(nfm - n, nfxm));
						bMatrix.setEntry(nfm - n, nfxm, -bMatrix.getEntry(0, nfxm) - bMatrix.getEntry(nfm, nfxm));
						zMatrix.setEntry(0, nfxm, FastMath.sqrt(TWO) / (stepa * stepb));
						zMatrix.setEntry(nfm, nfxm, FastMath.sqrt(HALF) / rhosq);
						// zMatrix.setEntry(nfm, nfxm, FastMath.sqrt(HALF) * recip); // XXX "testAckley" and "testDiffPow" fail.
						zMatrix.setEntry(nfm - n, nfxm, -zMatrix.getEntry(0, nfxm) - zMatrix.getEntry(nfm, nfxm));
					}

					// Set the off-diagonal second derivatives of the Lagrange functions and
					// the initial quadratic model.

				}
				else
				{
					zMatrix.setEntry(0, nfxm, recip);
					zMatrix.setEntry(nfm, nfxm, recip);
					zMatrix.setEntry(ipt, nfxm, -recip);
					zMatrix.setEntry(jpt, nfxm, -recip);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ih = ipt * (ipt - 1) / 2 + jpt - 1;
					int ih = ipt * (ipt - 1) / 2 + jpt - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = interpolationPoints.getEntry(nfm, ipt - 1) * interpolationPoints.getEntry(nfm, jpt - 1);
					double tmp = interpolationPoints.getEntry(nfm, ipt - 1) * interpolationPoints.getEntry(nfm, jpt - 1);
					modelSecondDerivativesValues.setEntry(ih, (fbeg - fAtInterpolationPoints.getEntry(ipt) - fAtInterpolationPoints.getEntry(jpt) + f) / tmp);
	//                 throw new PathIsExploredException(); // XXX
				}
			} while (Evaluations < npt);
		} // prelim


		// ----------------------------------------------------------------------------------------

		/// <summary>
		///     A version of the truncated conjugate gradient is applied. If a line
		///     search is restricted by a constraint, then the procedure is restarted,
		///     the values of the variables that are at their bounds being fixed. If
		///     the trust region boundary is reached, then further changes may be made
		///     to D, each one being in the two dimensional space that is spanned
		///     by the current D and the gradient of Q at XOPT+D, staying on the trust
		///     region boundary. Termination occurs when the reduction in Q seems to
		///     be close to the greatest reduction that can be achieved.
		///     The arguments N, NPT, XPT, XOPT, GOPT, HQ, PQ, SL and SU have the same
		///       meanings as the corresponding arguments of BOBYQB.
		///     DELTA is the trust region radius for the present calculation, which
		///       seeks a small value of the quadratic model within distance DELTA of
		///       XOPT subject to the bounds on the variables.
		///     XNEW will be set to a new vector of variables that is approximately
		///       the one that minimizes the quadratic model within the trust region
		///       subject to the SL and SU constraints on the variables. It satisfies
		///       as equations the bounds that become active during the calculation.
		///     D is the calculated trial step from XOPT, generated iteratively from an
		///       initial value of zero. Thus XNEW is XOPT+D after the final iteration.
		///     GNEW holds the gradient of the quadratic model at XOPT+D. It is updated
		///       when D is updated.
		///     xbdi.get( is a working space vector. For I=1,2,...,N, the element xbdi.get((I) is
		///       set to -1.0, 0.0, or 1.0, the value being nonzero if and only if the
		///       I-th variable has become fixed at a bound, the bound being SL(I) or
		///       SU(I) in the case xbdi.get((I)=-1.0 or xbdi.get((I)=1.0, respectively. This
		///       information is accumulated during the construction of XNEW.
		///     The arrays S, HS and HRED are also used for working space. They hold the
		///       current search direction, and the changes in the gradient of Q along S
		///       and the reduced D, respectively, where the reduced D is the same as D,
		///       except that the components of the fixed variables are zero.
		///     DSQ will be set to the square of the length of XNEW-XOPT.
		///     CRVMIN is set to zero if D reaches the trust region boundary. Otherwise
		///       it is set to the least curvature of H that occurs in the conjugate
		///       gradient searches that are not restricted by any constraints. The
		///       value CRVMIN=-1.0D0 is set, however, if all of these searches are
		///       constrained. </summary>
		/// <param name="delta"> </param>
		/// <param name="gnew"> </param>
		/// <param name="xbdi"> </param>
		/// <param name="s"> </param>
		/// <param name="hs"> </param>
		/// <param name="hred"> </param>
		private double[] trsbox(double delta, ArrayRealVector gnew, ArrayRealVector xbdi, ArrayRealVector s, ArrayRealVector hs, ArrayRealVector hred)
		{
			printMethod(); // XXX

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = currentBest.getDimension();
			int n = currentBest.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int npt = numberOfInterpolationPoints;
			int npt = numberOfInterpolationPoints;

			double dsq = double.NaN;
			double crvmin = double.NaN;

			// Local variables
			double ds;
			int iu;
			double dhd , dhs , cth , shs , sth , ssq , beta = 0, sdec , blen ;
			int iact = -1;
			int nact = 0;
			double angt = 0, qred ;
			int isav;
			double temp = 0, xsav = 0, xsum = 0, angbd = 0, dredg = 0, sredg = 0;
			int iterc;
			double resid = 0, delsq = 0, ggsav = 0, tempa = 0, tempb = 0, redmax = 0, dredsq = 0, redsav = 0, gredsq = 0, rednew = 0;
			int itcsav = 0;
			double rdprev = 0, rdnext = 0, stplen = 0, stepsq = 0;
			int itermax = 0;

			// Set some constants.

			// Function Body

			// The sign of GOPT(I) gives the sign of the change to the I-th variable
			// that will reduce Q from its value at XOPT. Thus xbdi.get((I) shows whether
			// or not to fix the I-th variable at one of its bounds initially, with
			// NACT being set to the number of fixed variables. D and GNEW are also
			// set for the first iteration. DELSQ is the upper bound on the sum of
			// squares of the free variables. QRED is the reduction in Q so far.

			iterc = 0;
			nact = 0;
			for (int i = 0; i < n; i++)
			{
				xbdi.setEntry(i, ZERO);
				if (trustRegionCenterOffset.getEntry(i) <= lowerDifference.getEntry(i))
				{
					if (gradientAtTrustRegionCenter.getEntry(i) >= ZERO)
					{
						xbdi.setEntry(i, MINUS_ONE);
					}
				}
				else if (trustRegionCenterOffset.getEntry(i) >= upperDifference.getEntry(i) && gradientAtTrustRegionCenter.getEntry(i) <= ZERO)
				{
					xbdi.setEntry(i, ONE);
				}
				if (xbdi.getEntry(i) != ZERO)
				{
					++nact;
				}
				trialStepPoint.setEntry(i, ZERO);
				gnew.setEntry(i, gradientAtTrustRegionCenter.getEntry(i));
			}
			delsq = delta * delta;
			qred = ZERO;
			crvmin = MINUS_ONE;

			// Set the next search direction of the conjugate gradient method. It is
			// the steepest descent direction initially and when the iterations are
			// restarted because a variable has just been fixed by a bound, and of
			// course the components of the fixed variables are zero. ITERMAX is an
			// upper bound on the indices of the conjugate gradient iterations.

			int state = 20;
			for (;;)
			{
				switch (state)
				{
			case 20:
			{
				printState(20); // XXX
				beta = ZERO;
			}
				goto case 30;
			case 30:
			{
				printState(30); // XXX
				stepsq = ZERO;
				for (int i = 0; i < n; i++)
				{
					if (xbdi.getEntry(i) != ZERO)
					{
						s.setEntry(i, ZERO);
					}
					else if (beta == ZERO)
					{
						s.setEntry(i, -gnew.getEntry(i));
					}
					else
					{
						s.setEntry(i, beta * s.getEntry(i) - gnew.getEntry(i));
					}
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = s.getEntry(i);
					double d1 = s.getEntry(i);
					stepsq += d1 * d1;
				}
				if (stepsq == ZERO)
				{
					state = 190;
					break;
				}
				if (beta == ZERO)
				{
					gredsq = stepsq;
					itermax = iterc + n - nact;
				}
				if (gredsq * delsq <= qred * 1e-4 * qred)
				{
					state = 190;
					break;
				}

				// Multiply the search direction by the second derivative matrix of Q and
				// calculate some scalars for the choice of steplength. Then set BLEN to
				// the length of the the step to the trust region boundary and STPLEN to
				// the steplength, ignoring the simple bounds.

				state = 210;
				break;
			}
			case 50:
			{
				printState(50); // XXX
				resid = delsq;
				ds = ZERO;
				shs = ZERO;
				for (int i = 0; i < n; i++)
				{
					if (xbdi.getEntry(i) == ZERO)
					{
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = trialStepPoint.getEntry(i);
						double d1 = trialStepPoint.getEntry(i);
						resid -= d1 * d1;
						ds += s.getEntry(i) * trialStepPoint.getEntry(i);
						shs += s.getEntry(i) * hs.getEntry(i);
					}
				}
				if (resid <= ZERO)
				{
					state = 90;
					break;
				}
				temp = FastMath.sqrt(stepsq * resid + ds * ds);
				if (ds < ZERO)
				{
					blen = (temp - ds) / stepsq;
				}
				else
				{
					blen = resid / (temp + ds);
				}
				stplen = blen;
				if (shs > ZERO)
				{
					// Computing MIN
					stplen = FastMath.min(blen, gredsq / shs);
				}

				// Reduce STPLEN if necessary in order to preserve the simple bounds,
				// letting IACT be the index of the new constrained variable.

				iact = -1;
				for (int i = 0; i < n; i++)
				{
					if (s.getEntry(i) != ZERO)
					{
						xsum = trustRegionCenterOffset.getEntry(i) + trialStepPoint.getEntry(i);
						if (s.getEntry(i) > ZERO)
						{
							temp = (upperDifference.getEntry(i) - xsum) / s.getEntry(i);
						}
						else
						{
							temp = (lowerDifference.getEntry(i) - xsum) / s.getEntry(i);
						}
						if (temp < stplen)
						{
							stplen = temp;
							iact = i;
						}
					}
				}

				// Update CRVMIN, GNEW and D. Set SDEC to the decrease that occurs in Q.

				sdec = ZERO;
				if (stplen > ZERO)
				{
					++iterc;
					temp = shs / stepsq;
					if (iact == -1 && temp > ZERO)
					{
						crvmin = FastMath.min(crvmin,temp);
						if (crvmin == MINUS_ONE)
						{
							crvmin = temp;
						}
					}
					ggsav = gredsq;
					gredsq = ZERO;
					for (int i = 0; i < n; i++)
					{
						gnew.setEntry(i, gnew.getEntry(i) + stplen * hs.getEntry(i));
						if (xbdi.getEntry(i) == ZERO)
						{
							// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = gnew.getEntry(i);
							double d1 = gnew.getEntry(i);
							gredsq += d1 * d1;
						}
						trialStepPoint.setEntry(i, trialStepPoint.getEntry(i) + stplen * s.getEntry(i));
					}
					// Computing MAX
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = stplen * (ggsav - HALF * stplen * shs);
					double d1 = stplen * (ggsav - HALF * stplen * shs);
					sdec = FastMath.max(d1, ZERO);
					qred += sdec;
				}

				// Restart the conjugate gradient method if it has hit a new bound.

				if (iact >= 0)
				{
					++nact;
					xbdi.setEntry(iact, ONE);
					if (s.getEntry(iact) < ZERO)
					{
						xbdi.setEntry(iact, MINUS_ONE);
					}
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = trialStepPoint.getEntry(iact);
					double d1 = trialStepPoint.getEntry(iact);
					delsq -= d1 * d1;
					if (delsq <= ZERO)
					{
						state = 190;
						break;
					}
					state = 20;
					break;
				}

				// If STPLEN is less than BLEN, then either apply another conjugate
				// gradient iteration or RETURN.

				if (stplen < blen)
				{
					if (iterc == itermax)
					{
						state = 190;
						break;
					}
					if (sdec <= qred * .01)
					{
						state = 190;
						break;
					}
					beta = gredsq / ggsav;
					state = 30;
					break;
				}
			}
				goto case 90;
			case 90:
			{
				printState(90); // XXX
				crvmin = ZERO;

				// Prepare for the alternative iteration by calculating some scalars
				// and by multiplying the reduced D by the second derivative matrix of
				// Q, where S holds the reduced D in the call of GGMULT.

			}
				goto case 100;
			case 100:
			{
				printState(100); // XXX
				if (nact >= n - 1)
				{
					state = 190;
					break;
				}
				dredsq = ZERO;
				dredg = ZERO;
				gredsq = ZERO;
				for (int i = 0; i < n; i++)
				{
					if (xbdi.getEntry(i) == ZERO)
					{
						// Computing 2nd power
						double d1 = trialStepPoint.getEntry(i);
						dredsq += d1 * d1;
						dredg += trialStepPoint.getEntry(i) * gnew.getEntry(i);
						// Computing 2nd power
						d1 = gnew.getEntry(i);
						gredsq += d1 * d1;
						s.setEntry(i, trialStepPoint.getEntry(i));
					}
					else
					{
						s.setEntry(i, ZERO);
					}
				}
				itcsav = iterc;
				state = 210;
				break;
				// Let the search direction S be a linear combination of the reduced D
				// and the reduced G that is orthogonal to the reduced D.
			}
			case 120:
			{
				printState(120); // XXX
				++iterc;
				temp = gredsq * dredsq - dredg * dredg;
				if (temp <= qred * 1e-4 * qred)
				{
					state = 190;
					break;
				}
				temp = FastMath.sqrt(temp);
				for (int i = 0; i < n; i++)
				{
					if (xbdi.getEntry(i) == ZERO)
					{
						s.setEntry(i, (dredg * trialStepPoint.getEntry(i) - dredsq * gnew.getEntry(i)) / temp);
					}
					else
					{
						s.setEntry(i, ZERO);
					}
				}
				sredg = -temp;

				// By considering the simple bounds on the variables, calculate an upper
				// bound on the tangent of half the angle of the alternative iteration,
				// namely ANGBD, except that, if already a free variable has reached a
				// bound, there is a branch back to label 100 after fixing that variable.

				angbd = ONE;
				iact = -1;
				for (int i = 0; i < n; i++)
				{
					if (xbdi.getEntry(i) == ZERO)
					{
						tempa = trustRegionCenterOffset.getEntry(i) + trialStepPoint.getEntry(i) - lowerDifference.getEntry(i);
						tempb = upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i) - trialStepPoint.getEntry(i);
						if (tempa <= ZERO)
						{
							++nact;
							xbdi.setEntry(i, MINUS_ONE);
							state = 100;
							break;
						}
						else if (tempb <= ZERO)
						{
							++nact;
							xbdi.setEntry(i, ONE);
							state = 100;
							break;
						}
						// Computing 2nd power
						double d1 = trialStepPoint.getEntry(i);
						// Computing 2nd power
						double d2 = s.getEntry(i);
						ssq = d1 * d1 + d2 * d2;
						// Computing 2nd power
						d1 = trustRegionCenterOffset.getEntry(i) - lowerDifference.getEntry(i);
						temp = ssq - d1 * d1;
						if (temp > ZERO)
						{
							temp = FastMath.sqrt(temp) - s.getEntry(i);
							if (angbd * temp > tempa)
							{
								angbd = tempa / temp;
								iact = i;
								xsav = MINUS_ONE;
							}
						}
						// Computing 2nd power
						d1 = upperDifference.getEntry(i) - trustRegionCenterOffset.getEntry(i);
						temp = ssq - d1 * d1;
						if (temp > ZERO)
						{
							temp = FastMath.sqrt(temp) + s.getEntry(i);
							if (angbd * temp > tempb)
							{
								angbd = tempb / temp;
								iact = i;
								xsav = ONE;
							}
						}
					}
				}

				// Calculate HHD and some curvatures for the alternative iteration.

				state = 210;
				break;
			}
			case 150:
			{
				printState(150); // XXX
				shs = ZERO;
				dhs = ZERO;
				dhd = ZERO;
				for (int i = 0; i < n; i++)
				{
					if (xbdi.getEntry(i) == ZERO)
					{
						shs += s.getEntry(i) * hs.getEntry(i);
						dhs += trialStepPoint.getEntry(i) * hs.getEntry(i);
						dhd += trialStepPoint.getEntry(i) * hred.getEntry(i);
					}
				}

				// Seek the greatest reduction in Q for a range of equally spaced values
				// of ANGT in [0,ANGBD], where ANGT is the tangent of half the angle of
				// the alternative iteration.

				redmax = ZERO;
				isav = -1;
				redsav = ZERO;
				iu = (int)(angbd * 17.0 + 3.1);
				for (int i = 0; i < iu; i++)
				{
					angt = angbd * i / iu;
					sth = (angt + angt) / (ONE + angt * angt);
					temp = shs + angt * (angt * dhd - dhs - dhs);
					rednew = sth * (angt * dredg - sredg - HALF * sth * temp);
					if (rednew > redmax)
					{
						redmax = rednew;
						isav = i;
						rdprev = redsav;
					}
					else if (i == isav + 1)
					{
						rdnext = rednew;
					}
					redsav = rednew;
				}

				// Return if the reduction is zero. Otherwise, set the sine and cosine
				// of the angle of the alternative iteration, and calculate SDEC.

				if (isav < 0)
				{
					state = 190;
					break;
				}
				if (isav < iu)
				{
					temp = (rdnext - rdprev) / (redmax + redmax - rdprev - rdnext);
					angt = angbd * (isav + HALF * temp) / iu;
				}
				cth = (ONE - angt * angt) / (ONE + angt * angt);
				sth = (angt + angt) / (ONE + angt * angt);
				temp = shs + angt * (angt * dhd - dhs - dhs);
				sdec = sth * (angt * dredg - sredg - HALF * sth * temp);
				if (sdec <= ZERO)
				{
					state = 190;
					break;
				}

				// Update GNEW, D and HRED. If the angle of the alternative iteration
				// is restricted by a bound on a free variable, that variable is fixed
				// at the bound.

				dredg = ZERO;
				gredsq = ZERO;
				for (int i = 0; i < n; i++)
				{
					gnew.setEntry(i, gnew.getEntry(i) + (cth - ONE) * hred.getEntry(i) + sth * hs.getEntry(i));
					if (xbdi.getEntry(i) == ZERO)
					{
						trialStepPoint.setEntry(i, cth * trialStepPoint.getEntry(i) + sth * s.getEntry(i));
						dredg += trialStepPoint.getEntry(i) * gnew.getEntry(i);
						// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = gnew.getEntry(i);
						double d1 = gnew.getEntry(i);
						gredsq += d1 * d1;
					}
					hred.setEntry(i, cth * hred.getEntry(i) + sth * hs.getEntry(i));
				}
				qred += sdec;
				if (iact >= 0 && isav == iu)
				{
					++nact;
					xbdi.setEntry(iact, xsav);
					state = 100;
					break;
				}

				// If SDEC is sufficiently small, then RETURN after setting XNEW to
				// XOPT+D, giving careful attention to the bounds.

				if (sdec > qred * .01)
				{
					state = 120;
					break;
				}
			}
				goto case 190;
			case 190:
			{
				printState(190); // XXX
				dsq = ZERO;
				for (int i = 0; i < n; i++)
				{
					// Computing MAX
					// Computing MIN
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double min = org.apache.commons.math3.util.FastMath.min(trustRegionCenterOffset.getEntry(i) + trialStepPoint.getEntry(i), upperDifference.getEntry(i));
					double min = FastMath.min(trustRegionCenterOffset.getEntry(i) + trialStepPoint.getEntry(i), upperDifference.getEntry(i));
					newPoint.setEntry(i, FastMath.max(min, lowerDifference.getEntry(i)));
					if (xbdi.getEntry(i) == MINUS_ONE)
					{
						newPoint.setEntry(i, lowerDifference.getEntry(i));
					}
					if (xbdi.getEntry(i) == ONE)
					{
						newPoint.setEntry(i, upperDifference.getEntry(i));
					}
					trialStepPoint.setEntry(i, newPoint.getEntry(i) - trustRegionCenterOffset.getEntry(i));
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = trialStepPoint.getEntry(i);
					double d1 = trialStepPoint.getEntry(i);
					dsq += d1 * d1;
				}
				return new double[] {dsq, crvmin};
				// The following instructions multiply the current S-vector by the second
				// derivative matrix of the quadratic model, putting the product in HS.
				// They are reached from three different parts of the software above and
				// they can be regarded as an external subroutine.
			}
			case 210:
			{
				printState(210); // XXX
				int ih = 0;
				for (int j = 0; j < n; j++)
				{
					hs.setEntry(j, ZERO);
					for (int i = 0; i <= j; i++)
					{
						if (i < j)
						{
							hs.setEntry(j, hs.getEntry(j) + modelSecondDerivativesValues.getEntry(ih) * s.getEntry(i));
						}
						hs.setEntry(i, hs.getEntry(i) + modelSecondDerivativesValues.getEntry(ih) * s.getEntry(j));
						ih++;
					}
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealVector tmp = interpolationPoints.operate(s).ebeMultiply(modelSecondDerivativesParameters);
				RealVector tmp = interpolationPoints.operate(s).ebeMultiply(modelSecondDerivativesParameters);
				for (int k = 0; k < npt; k++)
				{
					if (modelSecondDerivativesParameters.getEntry(k) != ZERO)
					{
						for (int i = 0; i < n; i++)
						{
							hs.setEntry(i, hs.getEntry(i) + tmp.getEntry(k) * interpolationPoints.getEntry(k, i));
						}
					}
				}
				if (crvmin != ZERO)
				{
					state = 50;
					break;
				}
				if (iterc > itcsav)
				{
					state = 150;
					break;
				}
				for (int i = 0; i < n; i++)
				{
					hred.setEntry(i, hs.getEntry(i));
				}
				state = 120;
				break;
			}
			default:
			{
				throw new MathIllegalStateException(LocalizedFormats.SIMPLE_MESSAGE, "trsbox");
			}
				}
			}
		} // trsbox

		// ----------------------------------------------------------------------------------------

		/// <summary>
		///     The arrays BMAT and ZMAT are updated, as required by the new position
		///     of the interpolation point that has the index KNEW. The vector VLAG has
		///     N+NPT components, set on entry to the first NPT and last N components
		///     of the product Hw in equation (4.11) of the Powell (2006) paper on
		///     NEWUOA. Further, BETA is set on entry to the value of the parameter
		///     with that name, and DENOM is set to the denominator of the updating
		///     formula. Elements of ZMAT may be treated as zero if their moduli are
		///     at most ZTEST. The first NDIM elements of W are used for working space. </summary>
		/// <param name="beta"> </param>
		/// <param name="denom"> </param>
		/// <param name="knew"> </param>
		private void update(double beta, double denom, int knew)
		{
			printMethod(); // XXX

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = currentBest.getDimension();
			int n = currentBest.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int npt = numberOfInterpolationPoints;
			int npt = numberOfInterpolationPoints;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nptm = npt - n - 1;
			int nptm = npt - n - 1;

			// XXX Should probably be split into two arrays.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.ArrayRealVector work = new org.apache.commons.math3.linear.ArrayRealVector(npt + n);
			ArrayRealVector work = new ArrayRealVector(npt + n);

			double ztest = ZERO;
			for (int k = 0; k < npt; k++)
			{
				for (int j = 0; j < nptm; j++)
				{
					// Computing MAX
					ztest = FastMath.max(ztest, FastMath.abs(zMatrix.getEntry(k, j)));
				}
			}
			ztest *= 1e-20;

			// Apply the rotations that put zeros in the KNEW-th row of ZMAT.

			for (int j = 1; j < nptm; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = zMatrix.getEntry(knew, j);
				double d1 = zMatrix.getEntry(knew, j);
				if (FastMath.abs(d1) > ztest)
				{
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = zMatrix.getEntry(knew, 0);
					double d2 = zMatrix.getEntry(knew, 0);
					// Computing 2nd power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = zMatrix.getEntry(knew, j);
					double d3 = zMatrix.getEntry(knew, j);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = org.apache.commons.math3.util.FastMath.sqrt(d2 * d2 + d3 * d3);
					double d4 = FastMath.sqrt(d2 * d2 + d3 * d3);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d5 = zMatrix.getEntry(knew, 0) / d4;
					double d5 = zMatrix.getEntry(knew, 0) / d4;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d6 = zMatrix.getEntry(knew, j) / d4;
					double d6 = zMatrix.getEntry(knew, j) / d4;
					for (int i = 0; i < npt; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d7 = d5 * zMatrix.getEntry(i, 0) + d6 * zMatrix.getEntry(i, j);
						double d7 = d5 * zMatrix.getEntry(i, 0) + d6 * zMatrix.getEntry(i, j);
						zMatrix.setEntry(i, j, d5 * zMatrix.getEntry(i, j) - d6 * zMatrix.getEntry(i, 0));
						zMatrix.setEntry(i, 0, d7);
					}
				}
				zMatrix.setEntry(knew, j, ZERO);
			}

			// Put the first NPT components of the KNEW-th column of HLAG into W,
			// and calculate the parameters of the updating formula.

			for (int i = 0; i < npt; i++)
			{
				work.setEntry(i, zMatrix.getEntry(knew, 0) * zMatrix.getEntry(i, 0));
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = work.getEntry(knew);
			double alpha = work.getEntry(knew);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tau = lagrangeValuesAtNewPoint.getEntry(knew);
			double tau = lagrangeValuesAtNewPoint.getEntry(knew);
			lagrangeValuesAtNewPoint.setEntry(knew, lagrangeValuesAtNewPoint.getEntry(knew) - ONE);

			// Complete the updating of ZMAT.

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sqrtDenom = org.apache.commons.math3.util.FastMath.sqrt(denom);
			double sqrtDenom = FastMath.sqrt(denom);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d1 = tau / sqrtDenom;
			double d1 = tau / sqrtDenom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d2 = zMatrix.getEntry(knew, 0) / sqrtDenom;
			double d2 = zMatrix.getEntry(knew, 0) / sqrtDenom;
			for (int i = 0; i < npt; i++)
			{
				zMatrix.setEntry(i, 0, d1 * zMatrix.getEntry(i, 0) - d2 * lagrangeValuesAtNewPoint.getEntry(i));
			}

			// Finally, update the matrix BMAT.

			for (int j = 0; j < n; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jp = npt + j;
				int jp = npt + j;
				work.setEntry(jp, bMatrix.getEntry(knew, j));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d3 = (alpha * lagrangeValuesAtNewPoint.getEntry(jp) - tau * work.getEntry(jp)) / denom;
				double d3 = (alpha * lagrangeValuesAtNewPoint.getEntry(jp) - tau * work.getEntry(jp)) / denom;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d4 = (-beta * work.getEntry(jp) - tau * lagrangeValuesAtNewPoint.getEntry(jp)) / denom;
				double d4 = (-beta * work.getEntry(jp) - tau * lagrangeValuesAtNewPoint.getEntry(jp)) / denom;
				for (int i = 0; i <= jp; i++)
				{
					bMatrix.setEntry(i, j, bMatrix.getEntry(i, j) + d3 * lagrangeValuesAtNewPoint.getEntry(i) + d4 * work.getEntry(i));
					if (i >= npt)
					{
						bMatrix.setEntry(jp, (i - npt), bMatrix.getEntry(i, j));
					}
				}
			}
		} // update

		/// <summary>
		/// Performs validity checks.
		/// </summary>
		/// <param name="lowerBound"> Lower bounds (constraints) of the objective variables. </param>
		/// <param name="upperBound"> Upperer bounds (constraints) of the objective variables. </param>
		private void setup(double[] lowerBound, double[] upperBound)
		{
			printMethod(); // XXX

			double[] init = StartPoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = init.length;
			int dimension = init.Length;

			// Check problem dimension.
			if (dimension < MINIMUM_PROBLEM_DIMENSION)
			{
				throw new NumberIsTooSmallException(dimension, MINIMUM_PROBLEM_DIMENSION, true);
			}
			// Check number of interpolation points.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] nPointsInterval = { dimension + 2, (dimension + 2) * (dimension + 1) / 2 };
			int[] nPointsInterval = new int[] {dimension + 2, (dimension + 2) * (dimension + 1) / 2};
			if (numberOfInterpolationPoints < nPointsInterval[0] || numberOfInterpolationPoints > nPointsInterval[1])
			{
				throw new OutOfRangeException(LocalizedFormats.NUMBER_OF_INTERPOLATION_POINTS, numberOfInterpolationPoints, nPointsInterval[0], nPointsInterval[1]);
			}

			// Initialize bound differences.
			boundDifference = new double[dimension];

			double requiredMinDiff = 2 * initialTrustRegionRadius;
			double minDiff = double.PositiveInfinity;
			for (int i = 0; i < dimension; i++)
			{
				boundDifference[i] = upperBound[i] - lowerBound[i];
				minDiff = FastMath.min(minDiff, boundDifference[i]);
			}
			if (minDiff < requiredMinDiff)
			{
				initialTrustRegionRadius = minDiff / 3.0;
			}

			// Initialize the data structures used by the "bobyqa" method.
			bMatrix = new Array2DRowRealMatrix(dimension + numberOfInterpolationPoints, dimension);
			zMatrix = new Array2DRowRealMatrix(numberOfInterpolationPoints, numberOfInterpolationPoints - dimension - 1);
			interpolationPoints = new Array2DRowRealMatrix(numberOfInterpolationPoints, dimension);
			originShift = new ArrayRealVector(dimension);
			fAtInterpolationPoints = new ArrayRealVector(numberOfInterpolationPoints);
			trustRegionCenterOffset = new ArrayRealVector(dimension);
			gradientAtTrustRegionCenter = new ArrayRealVector(dimension);
			lowerDifference = new ArrayRealVector(dimension);
			upperDifference = new ArrayRealVector(dimension);
			modelSecondDerivativesParameters = new ArrayRealVector(numberOfInterpolationPoints);
			newPoint = new ArrayRealVector(dimension);
			alternativeNewPoint = new ArrayRealVector(dimension);
			trialStepPoint = new ArrayRealVector(dimension);
			lagrangeValuesAtNewPoint = new ArrayRealVector(dimension + numberOfInterpolationPoints);
			modelSecondDerivativesValues = new ArrayRealVector(dimension * (dimension + 1) / 2);
		}

		// XXX utility for figuring out call sequence.
		private static string caller(int n)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Throwable t = new Throwable();
			Exception t = new Exception();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StackTraceElement[] elements = t.getStackTrace();
			StackTraceElement[] elements = t.StackTrace;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final StackTraceElement e = elements[n];
			StackTraceElement e = elements[n];
			return e.MethodName + " (at line " + e.LineNumber + ")";
		}
		// XXX utility for figuring out call sequence.
		private static void printState(int s)
		{
			//        System.out.println(caller(2) + ": state " + s);
		}
		// XXX utility for figuring out call sequence.
		private static void printMethod()
		{
			//        System.out.println(caller(2));
		}

		/// <summary>
		/// Marker for code paths that are not explored with the current unit tests.
		/// If the path becomes explored, it should just be removed from the code.
		/// </summary>
		private class PathIsExploredException : Exception
		{
			internal const long serialVersionUID = 745350979634801853L;

			internal const string PATH_IS_EXPLORED = "If this exception is thrown, just remove it from the code";

			internal PathIsExploredException() : base(PATH_IS_EXPLORED + " " + BOBYQAOptimizer.caller(3))
			{
			}
		}
	}
	//CHECKSTYLE: resume all

}