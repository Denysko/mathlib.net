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
namespace org.apache.commons.math3.analysis.interpolation
{


	using PolynomialSplineFunction = org.apache.commons.math3.analysis.polynomials.PolynomialSplineFunction;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using NotFiniteNumberException = org.apache.commons.math3.exception.NotFiniteNumberException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Implements the <a href="http://en.wikipedia.org/wiki/Local_regression">
	/// Local Regression Algorithm</a> (also Loess, Lowess) for interpolation of
	/// real univariate functions.
	/// <p/>
	/// For reference, see
	/// <a href="http://www.math.tau.ac.il/~yekutiel/MA seminar/Cleveland 1979.pdf">
	/// William S. Cleveland - Robust Locally Weighted Regression and Smoothing
	/// Scatterplots</a>
	/// <p/>
	/// This class implements both the loess method and serves as an interpolation
	/// adapter to it, allowing one to build a spline on the obtained loess fit.
	/// 
	/// @version $Id: LoessInterpolator.java 1379904 2012-09-01 23:54:52Z erans $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class LoessInterpolator : UnivariateInterpolator
	{
		/// <summary>
		/// Default value of the bandwidth parameter. </summary>
		public const double DEFAULT_BANDWIDTH = 0.3;
		/// <summary>
		/// Default value of the number of robustness iterations. </summary>
		public const int DEFAULT_ROBUSTNESS_ITERS = 2;
		/// <summary>
		/// Default value for accuracy.
		/// @since 2.1
		/// </summary>
		public const double DEFAULT_ACCURACY = 1e-12;
		/// <summary>
		/// serializable version identifier. </summary>
		private const long serialVersionUID = 5204927143605193821L;
		/// <summary>
		/// The bandwidth parameter: when computing the loess fit at
		/// a particular point, this fraction of source points closest
		/// to the current point is taken into account for computing
		/// a least-squares regression.
		/// <p/>
		/// A sensible value is usually 0.25 to 0.5.
		/// </summary>
		private readonly double bandwidth;
		/// <summary>
		/// The number of robustness iterations parameter: this many
		/// robustness iterations are done.
		/// <p/>
		/// A sensible value is usually 0 (just the initial fit without any
		/// robustness iterations) to 4.
		/// </summary>
		private readonly int robustnessIters;
		/// <summary>
		/// If the median residual at a certain robustness iteration
		/// is less than this amount, no more iterations are done.
		/// </summary>
		private readonly double accuracy;

		/// <summary>
		/// Constructs a new <seealso cref="LoessInterpolator"/>
		/// with a bandwidth of <seealso cref="#DEFAULT_BANDWIDTH"/>,
		/// <seealso cref="#DEFAULT_ROBUSTNESS_ITERS"/> robustness iterations
		/// and an accuracy of {#link #DEFAULT_ACCURACY}.
		/// See <seealso cref="#LoessInterpolator(double, int, double)"/> for an explanation of
		/// the parameters.
		/// </summary>
		public LoessInterpolator()
		{
			this.bandwidth = DEFAULT_BANDWIDTH;
			this.robustnessIters = DEFAULT_ROBUSTNESS_ITERS;
			this.accuracy = DEFAULT_ACCURACY;
		}

		/// <summary>
		/// Construct a new <seealso cref="LoessInterpolator"/>
		/// with given bandwidth and number of robustness iterations.
		/// <p>
		/// Calling this constructor is equivalent to calling {link {@link
		/// #LoessInterpolator(double, int, double) LoessInterpolator(bandwidth,
		/// robustnessIters, LoessInterpolator.DEFAULT_ACCURACY)}
		/// </p>
		/// </summary>
		/// <param name="bandwidth">  when computing the loess fit at
		/// a particular point, this fraction of source points closest
		/// to the current point is taken into account for computing
		/// a least-squares regression.</br>
		/// A sensible value is usually 0.25 to 0.5, the default value is
		/// <seealso cref="#DEFAULT_BANDWIDTH"/>. </param>
		/// <param name="robustnessIters"> This many robustness iterations are done.</br>
		/// A sensible value is usually 0 (just the initial fit without any
		/// robustness iterations) to 4, the default value is
		/// <seealso cref="#DEFAULT_ROBUSTNESS_ITERS"/>.
		/// </param>
		/// <seealso cref= #LoessInterpolator(double, int, double) </seealso>
		public LoessInterpolator(double bandwidth, int robustnessIters) : this(bandwidth, robustnessIters, DEFAULT_ACCURACY)
		{
		}

		/// <summary>
		/// Construct a new <seealso cref="LoessInterpolator"/>
		/// with given bandwidth, number of robustness iterations and accuracy.
		/// </summary>
		/// <param name="bandwidth">  when computing the loess fit at
		/// a particular point, this fraction of source points closest
		/// to the current point is taken into account for computing
		/// a least-squares regression.</br>
		/// A sensible value is usually 0.25 to 0.5, the default value is
		/// <seealso cref="#DEFAULT_BANDWIDTH"/>. </param>
		/// <param name="robustnessIters"> This many robustness iterations are done.</br>
		/// A sensible value is usually 0 (just the initial fit without any
		/// robustness iterations) to 4, the default value is
		/// <seealso cref="#DEFAULT_ROBUSTNESS_ITERS"/>. </param>
		/// <param name="accuracy"> If the median residual at a certain robustness iteration
		/// is less than this amount, no more iterations are done. </param>
		/// <exception cref="OutOfRangeException"> if bandwidth does not lie in the interval [0,1]. </exception>
		/// <exception cref="NotPositiveException"> if {@code robustnessIters} is negative. </exception>
		/// <seealso cref= #LoessInterpolator(double, int)
		/// @since 2.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public LoessInterpolator(double bandwidth, int robustnessIters, double accuracy) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NotPositiveException
		public LoessInterpolator(double bandwidth, int robustnessIters, double accuracy)
		{
			if (bandwidth < 0 || bandwidth > 1)
			{
				throw new OutOfRangeException(LocalizedFormats.BANDWIDTH, bandwidth, 0, 1);
			}
			this.bandwidth = bandwidth;
			if (robustnessIters < 0)
			{
				throw new NotPositiveException(LocalizedFormats.ROBUSTNESS_ITERATIONS, robustnessIters);
			}
			this.robustnessIters = robustnessIters;
			this.accuracy = accuracy;
		}

		/// <summary>
		/// Compute an interpolating function by performing a loess fit
		/// on the data at the original abscissae and then building a cubic spline
		/// with a
		/// <seealso cref="org.apache.commons.math3.analysis.interpolation.SplineInterpolator"/>
		/// on the resulting fit.
		/// </summary>
		/// <param name="xval"> the arguments for the interpolation points </param>
		/// <param name="yval"> the values for the interpolation points </param>
		/// <returns> A cubic spline built upon a loess fit to the data at the original abscissae </returns>
		/// <exception cref="NonMonotonicSequenceException"> if {@code xval} not sorted in
		/// strictly increasing order. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code xval} and {@code yval} have
		/// different sizes. </exception>
		/// <exception cref="NoDataException"> if {@code xval} or {@code yval} has zero size. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the arguments and values are
		/// not finite real numbers. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the bandwidth is too small to
		/// accomodate the size of the input data (i.e. the bandwidth must be
		/// larger than 2/n). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final org.apache.commons.math3.analysis.polynomials.PolynomialSplineFunction interpolate(final double[] xval, final double[] yval) throws org.apache.commons.math3.exception.NonMonotonicSequenceException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NotFiniteNumberException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public PolynomialSplineFunction interpolate(double[] xval, double[] yval)
		{
			return (new SplineInterpolator()).interpolate(xval, smooth(xval, yval));
		}

		/// <summary>
		/// Compute a weighted loess fit on the data at the original abscissae.
		/// </summary>
		/// <param name="xval"> Arguments for the interpolation points. </param>
		/// <param name="yval"> Values for the interpolation points. </param>
		/// <param name="weights"> point weights: coefficients by which the robustness weight
		/// of a point is multiplied. </param>
		/// <returns> the values of the loess fit at corresponding original abscissae. </returns>
		/// <exception cref="NonMonotonicSequenceException"> if {@code xval} not sorted in
		/// strictly increasing order. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code xval} and {@code yval} have
		/// different sizes. </exception>
		/// <exception cref="NoDataException"> if {@code xval} or {@code yval} has zero size. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the arguments and values are
		/// not finite real numbers. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the bandwidth is too small to
		/// accomodate the size of the input data (i.e. the bandwidth must be
		/// larger than 2/n).
		/// @since 2.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final double[] smooth(final double[] xval, final double[] yval, final double[] weights) throws org.apache.commons.math3.exception.NonMonotonicSequenceException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NotFiniteNumberException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public double[] smooth(double[] xval, double[] yval, double[] weights)
		{
			if (xval.Length != yval.Length)
			{
				throw new DimensionMismatchException(xval.Length, yval.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = xval.length;
			int n = xval.Length;

			if (n == 0)
			{
				throw new NoDataException();
			}

			checkAllFiniteReal(xval);
			checkAllFiniteReal(yval);
			checkAllFiniteReal(weights);

			MathArrays.checkOrder(xval);

			if (n == 1)
			{
				return new double[]{yval[0]};
			}

			if (n == 2)
			{
				return new double[]{yval[0], yval[1]};
			}

			int bandwidthInPoints = (int)(bandwidth * n);

			if (bandwidthInPoints < 2)
			{
				throw new NumberIsTooSmallException(LocalizedFormats.BANDWIDTH, bandwidthInPoints, 2, true);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] res = new double[n];
			double[] res = new double[n];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] residuals = new double[n];
			double[] residuals = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] sortedResiduals = new double[n];
			double[] sortedResiduals = new double[n];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] robustnessWeights = new double[n];
			double[] robustnessWeights = new double[n];

			// Do an initial fit and 'robustnessIters' robustness iterations.
			// This is equivalent to doing 'robustnessIters+1' robustness iterations
			// starting with all robustness weights set to 1.
			Arrays.fill(robustnessWeights, 1);

			for (int iter = 0; iter <= robustnessIters; ++iter)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] bandwidthInterval = {0, bandwidthInPoints - 1};
				int[] bandwidthInterval = new int[] {0, bandwidthInPoints - 1};
				// At each x, compute a local weighted linear regression
				for (int i = 0; i < n; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = xval[i];
					double x = xval[i];

					// Find out the interval of source points on which
					// a regression is to be made.
					if (i > 0)
					{
						updateBandwidthInterval(xval, weights, i, bandwidthInterval);
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ileft = bandwidthInterval[0];
					int ileft = bandwidthInterval[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iright = bandwidthInterval[1];
					int iright = bandwidthInterval[1];

					// Compute the point of the bandwidth interval that is
					// farthest from x
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int edge;
					int edge;
					if (xval[i] - xval[ileft] > xval[iright] - xval[i])
					{
						edge = ileft;
					}
					else
					{
						edge = iright;
					}

					// Compute a least-squares linear fit weighted by
					// the product of robustness weights and the tricube
					// weight function.
					// See http://en.wikipedia.org/wiki/Linear_regression
					// (section "Univariate linear case")
					// and http://en.wikipedia.org/wiki/Weighted_least_squares
					// (section "Weighted least squares")
					double sumWeights = 0;
					double sumX = 0;
					double sumXSquared = 0;
					double sumY = 0;
					double sumXY = 0;
					double denom = FastMath.abs(1.0 / (xval[edge] - x));
					for (int k = ileft; k <= iright; ++k)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xk = xval[k];
						double xk = xval[k];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yk = yval[k];
						double yk = yval[k];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dist = (k < i) ? x - xk : xk - x;
						double dist = (k < i) ? x - xk : xk - x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = tricube(dist * denom) * robustnessWeights[k] * weights[k];
						double w = tricube(dist * denom) * robustnessWeights[k] * weights[k];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xkw = xk * w;
						double xkw = xk * w;
						sumWeights += w;
						sumX += xkw;
						sumXSquared += xk * xkw;
						sumY += yk * w;
						sumXY += yk * xkw;
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double meanX = sumX / sumWeights;
					double meanX = sumX / sumWeights;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double meanY = sumY / sumWeights;
					double meanY = sumY / sumWeights;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double meanXY = sumXY / sumWeights;
					double meanXY = sumXY / sumWeights;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double meanXSquared = sumXSquared / sumWeights;
					double meanXSquared = sumXSquared / sumWeights;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double beta;
					double beta;
					if (FastMath.sqrt(FastMath.abs(meanXSquared - meanX * meanX)) < accuracy)
					{
						beta = 0;
					}
					else
					{
						beta = (meanXY - meanX * meanY) / (meanXSquared - meanX * meanX);
					}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double alpha = meanY - beta * meanX;
					double alpha = meanY - beta * meanX;

					res[i] = beta * x + alpha;
					residuals[i] = FastMath.abs(yval[i] - res[i]);
				}

				// No need to recompute the robustness weights at the last
				// iteration, they won't be needed anymore
				if (iter == robustnessIters)
				{
					break;
				}

				// Recompute the robustness weights.

				// Find the median residual.
				// An arraycopy and a sort are completely tractable here,
				// because the preceding loop is a lot more expensive
				Array.Copy(residuals, 0, sortedResiduals, 0, n);
				Arrays.sort(sortedResiduals);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double medianResidual = sortedResiduals[n / 2];
				double medianResidual = sortedResiduals[n / 2];

				if (FastMath.abs(medianResidual) < accuracy)
				{
					break;
				}

				for (int i = 0; i < n; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double arg = residuals[i] / (6 * medianResidual);
					double arg = residuals[i] / (6 * medianResidual);
					if (arg >= 1)
					{
						robustnessWeights[i] = 0;
					}
					else
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double w = 1 - arg * arg;
						double w = 1 - arg * arg;
						robustnessWeights[i] = w * w;
					}
				}
			}

			return res;
		}

		/// <summary>
		/// Compute a loess fit on the data at the original abscissae.
		/// </summary>
		/// <param name="xval"> the arguments for the interpolation points </param>
		/// <param name="yval"> the values for the interpolation points </param>
		/// <returns> values of the loess fit at corresponding original abscissae </returns>
		/// <exception cref="NonMonotonicSequenceException"> if {@code xval} not sorted in
		/// strictly increasing order. </exception>
		/// <exception cref="DimensionMismatchException"> if {@code xval} and {@code yval} have
		/// different sizes. </exception>
		/// <exception cref="NoDataException"> if {@code xval} or {@code yval} has zero size. </exception>
		/// <exception cref="NotFiniteNumberException"> if any of the arguments and values are
		/// not finite real numbers. </exception>
		/// <exception cref="NumberIsTooSmallException"> if the bandwidth is too small to
		/// accomodate the size of the input data (i.e. the bandwidth must be
		/// larger than 2/n). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public final double[] smooth(final double[] xval, final double[] yval) throws org.apache.commons.math3.exception.NonMonotonicSequenceException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NotFiniteNumberException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public double[] smooth(double[] xval, double[] yval)
		{
			if (xval.Length != yval.Length)
			{
				throw new DimensionMismatchException(xval.Length, yval.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] unitWeights = new double[xval.length];
			double[] unitWeights = new double[xval.Length];
			Arrays.fill(unitWeights, 1.0);

			return smooth(xval, yval, unitWeights);
		}

		/// <summary>
		/// Given an index interval into xval that embraces a certain number of
		/// points closest to {@code xval[i-1]}, update the interval so that it
		/// embraces the same number of points closest to {@code xval[i]},
		/// ignoring zero weights.
		/// </summary>
		/// <param name="xval"> Arguments array. </param>
		/// <param name="weights"> Weights array. </param>
		/// <param name="i"> Index around which the new interval should be computed. </param>
		/// <param name="bandwidthInterval"> a two-element array {left, right} such that:
		/// {@code (left==0 or xval[i] - xval[left-1] > xval[right] - xval[i])}
		/// and
		/// {@code (right==xval.length-1 or xval[right+1] - xval[i] > xval[i] - xval[left])}.
		/// The array will be updated. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void updateBandwidthInterval(final double[] xval, final double[] weights, final int i, final int[] bandwidthInterval)
		private static void updateBandwidthInterval(double[] xval, double[] weights, int i, int[] bandwidthInterval)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int left = bandwidthInterval[0];
			int left = bandwidthInterval[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int right = bandwidthInterval[1];
			int right = bandwidthInterval[1];

			// The right edge should be adjusted if the next point to the right
			// is closer to xval[i] than the leftmost point of the current interval
			int nextRight = nextNonzero(weights, right);
			if (nextRight < xval.Length && xval[nextRight] - xval[i] < xval[i] - xval[left])
			{
				int nextLeft = nextNonzero(weights, bandwidthInterval[0]);
				bandwidthInterval[0] = nextLeft;
				bandwidthInterval[1] = nextRight;
			}
		}

		/// <summary>
		/// Return the smallest index {@code j} such that
		/// {@code j > i && (j == weights.length || weights[j] != 0)}.
		/// </summary>
		/// <param name="weights"> Weights array. </param>
		/// <param name="i"> Index from which to start search. </param>
		/// <returns> the smallest compliant index. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int nextNonzero(final double[] weights, final int i)
		private static int nextNonzero(double[] weights, int i)
		{
			int j = i + 1;
			while (j < weights.Length && weights[j] == 0)
			{
				++j;
			}
			return j;
		}

		/// <summary>
		/// Compute the
		/// <a href="http://en.wikipedia.org/wiki/Local_regression#Weight_function">tricube</a>
		/// weight function
		/// </summary>
		/// <param name="x"> Argument. </param>
		/// <returns> <code>(1 - |x|<sup>3</sup>)<sup>3</sup></code> for |x| &lt; 1, 0 otherwise. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static double tricube(final double x)
		private static double tricube(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double absX = org.apache.commons.math3.util.FastMath.abs(x);
			double absX = FastMath.abs(x);
			if (absX >= 1.0)
			{
				return 0.0;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tmp = 1 - absX * absX * absX;
			double tmp = 1 - absX * absX * absX;
			return tmp * tmp * tmp;
		}

		/// <summary>
		/// Check that all elements of an array are finite real numbers.
		/// </summary>
		/// <param name="values"> Values array. </param>
		/// <exception cref="org.apache.commons.math3.exception.NotFiniteNumberException">
		/// if one of the values is not a finite real number. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static void checkAllFiniteReal(final double[] values)
		private static void checkAllFiniteReal(double[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				MathUtils.checkFinite(values[i]);
			}
		}
	}

}