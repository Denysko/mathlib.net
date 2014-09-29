using System;
using System.Collections.Generic;

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
namespace mathlib.fitting
{


	using HarmonicOscillator = mathlib.analysis.function.HarmonicOscillator;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using ZeroException = mathlib.exception.ZeroException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using LeastSquaresBuilder = mathlib.fitting.leastsquares.LeastSquaresBuilder;
	using LeastSquaresProblem = mathlib.fitting.leastsquares.LeastSquaresProblem;
	using DiagonalMatrix = mathlib.linear.DiagonalMatrix;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Fits points to a {@link
	/// mathlib.analysis.function.HarmonicOscillator.Parametric harmonic oscillator}
	/// function.
	/// <br/>
	/// The <seealso cref="#withStartPoint(double[]) initial guess values"/> must be passed
	/// in the following order:
	/// <ul>
	///  <li>Amplitude</li>
	///  <li>Angular frequency</li>
	///  <li>phase</li>
	/// </ul>
	/// The optimal values will be returned in the same order.
	/// 
	/// @version $Id: HarmonicCurveFitter.java 1571640 2014-02-25 10:27:21Z erans $
	/// @since 3.3
	/// </summary>
	public class HarmonicCurveFitter : AbstractCurveFitter
	{
		/// <summary>
		/// Parametric function to be fitted. </summary>
		private static readonly HarmonicOscillator.Parametric FUNCTION = new HarmonicOscillator.Parametric();
		/// <summary>
		/// Initial guess. </summary>
		private readonly double[] initialGuess;
		/// <summary>
		/// Maximum number of iterations of the optimization algorithm. </summary>
		private readonly int maxIter;

		/// <summary>
		/// Contructor used by the factory methods.
		/// </summary>
		/// <param name="initialGuess"> Initial guess. If set to {@code null}, the initial guess
		/// will be estimated using the <seealso cref="ParameterGuesser"/>. </param>
		/// <param name="maxIter"> Maximum number of iterations of the optimization algorithm. </param>
		private HarmonicCurveFitter(double[] initialGuess, int maxIter)
		{
			this.initialGuess = initialGuess;
			this.maxIter = maxIter;
		}

		/// <summary>
		/// Creates a default curve fitter.
		/// The initial guess for the parameters will be <seealso cref="ParameterGuesser"/>
		/// computed automatically, and the maximum number of iterations of the
		/// optimization algorithm is set to <seealso cref="Integer#MAX_VALUE"/>.
		/// </summary>
		/// <returns> a curve fitter.
		/// </returns>
		/// <seealso cref= #withStartPoint(double[]) </seealso>
		/// <seealso cref= #withMaxIterations(int) </seealso>
		public static HarmonicCurveFitter create()
		{
			return new HarmonicCurveFitter(null, int.MaxValue);
		}

		/// <summary>
		/// Configure the start point (initial guess). </summary>
		/// <param name="newStart"> new start point (initial guess) </param>
		/// <returns> a new instance. </returns>
		public virtual HarmonicCurveFitter withStartPoint(double[] newStart)
		{
			return new HarmonicCurveFitter(newStart.clone(), maxIter);
		}

		/// <summary>
		/// Configure the maximum number of iterations. </summary>
		/// <param name="newMaxIter"> maximum number of iterations </param>
		/// <returns> a new instance. </returns>
		public virtual HarmonicCurveFitter withMaxIterations(int newMaxIter)
		{
			return new HarmonicCurveFitter(initialGuess, newMaxIter);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override LeastSquaresProblem getProblem(ICollection<WeightedObservedPoint> observations)
		{
			// Prepare least-squares problem.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = observations.size();
			int len = observations.Count;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] target = new double[len];
			double[] target = new double[len];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] weights = new double[len];
			double[] weights = new double[len];

			int i = 0;
			foreach (WeightedObservedPoint obs in observations)
			{
				target[i] = obs.Y;
				weights[i] = obs.Weight;
				++i;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AbstractCurveFitter.TheoreticalValuesFunction model = new AbstractCurveFitter.TheoreticalValuesFunction(FUNCTION, observations);
			AbstractCurveFitter.TheoreticalValuesFunction model = new AbstractCurveFitter.TheoreticalValuesFunction(FUNCTION, observations);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] startPoint = initialGuess != null ? initialGuess : new ParameterGuesser(observations).guess();
			double[] startPoint = initialGuess != null ? initialGuess : (new ParameterGuesser(observations)).guess();
				// Compute estimation.

			// Return a new optimizer set up to fit a Gaussian curve to the
			// observed points.
			return (new LeastSquaresBuilder()).maxEvaluations(int.MaxValue).maxIterations(maxIter).start(startPoint).target(target).weight(new DiagonalMatrix(weights)).model(model.ModelFunction, model.ModelFunctionJacobian).build();

		}

		/// <summary>
		/// This class guesses harmonic coefficients from a sample.
		/// <p>The algorithm used to guess the coefficients is as follows:</p>
		/// 
		/// <p>We know \( f(t) \) at some sampling points \( t_i \) and want
		/// to find \( a \), \( \omega \) and \( \phi \) such that
		/// \( f(t) = a \cos (\omega t + \phi) \).
		/// </p>
		/// 
		/// <p>From the analytical expression, we can compute two primitives :
		/// \[
		///     If2(t) = \int f^2 dt  = a^2 (t + S(t)) / 2
		/// \]
		/// \[
		///     If'2(t) = \int f'^2 dt = a^2 \omega^2 (t - S(t)) / 2
		/// \]
		/// where \(S(t) = \frac{\sin(2 (\omega t + \phi))}{2\omega}\)
		/// </p>
		/// 
		/// <p>We can remove \(S\) between these expressions :
		/// \[
		///     If'2(t) = a^2 \omega^2 t - \omega^2 If2(t)
		/// \]
		/// </p>
		/// 
		/// <p>The preceding expression shows that \(If'2 (t)\) is a linear
		/// combination of both \(t\) and \(If2(t)\):
		/// \[
		///   If'2(t) = A t + B If2(t)
		/// \]
		/// </p>
		/// 
		/// <p>From the primitive, we can deduce the same form for definite
		/// integrals between \(t_1\) and \(t_i\) for each \(t_i\) :
		/// \[
		///   If2(t_i) - If2(t_1) = A (t_i - t_1) + B (If2 (t_i) - If2(t_1))
		/// \]
		/// </p>
		/// 
		/// <p>We can find the coefficients \(A\) and \(B\) that best fit the sample
		/// to this linear expression by computing the definite integrals for
		/// each sample points.
		/// </p>
		/// 
		/// <p>For a bilinear expression \(z(x_i, y_i) = A x_i + B y_i\), the
		/// coefficients \(A\) and \(B\) that minimize a least-squares criterion
		/// \(\sum (z_i - z(x_i, y_i))^2\) are given by these expressions:</p>
		/// \[
		///   A = \frac{\sum y_i y_i \sum x_i z_i - \sum x_i y_i \sum y_i z_i}
		///            {\sum x_i x_i \sum y_i y_i - \sum x_i y_i \sum x_i y_i}
		/// \]
		/// \[
		///   B = \frac{\sum x_i x_i \sum y_i z_i - \sum x_i y_i \sum x_i z_i}
		///            {\sum x_i x_i \sum y_i y_i - \sum x_i y_i \sum x_i y_i}
		/// 
		/// \]
		/// 
		/// <p>In fact, we can assume that both \(a\) and \(\omega\) are positive and
		/// compute them directly, knowing that \(A = a^2 \omega^2\) and that
		/// \(B = -\omega^2\). The complete algorithm is therefore:</p>
		/// 
		/// For each \(t_i\) from \(t_1\) to \(t_{n-1}\), compute:
		/// \[ f(t_i) \]
		/// \[ f'(t_i) = \frac{f (t_{i+1}) - f(t_{i-1})}{t_{i+1} - t_{i-1}} \]
		/// \[ x_i = t_i  - t_1 \]
		/// \[ y_i = \int_{t_1}^{t_i} f^2(t) dt \]
		/// \[ z_i = \int_{t_1}^{t_i} f'^2(t) dt \]
		/// and update the sums:
		/// \[ \sum x_i x_i, \sum y_i y_i, \sum x_i y_i, \sum x_i z_i, \sum y_i z_i \]
		/// 
		/// Then:
		/// \[
		///  a = \sqrt{\frac{\sum y_i y_i  \sum x_i z_i - \sum x_i y_i \sum y_i z_i }
		///                 {\sum x_i y_i  \sum x_i z_i - \sum x_i x_i \sum y_i z_i }}
		/// \]
		/// \[
		///  \omega = \sqrt{\frac{\sum x_i y_i \sum x_i z_i - \sum x_i x_i \sum y_i z_i}
		///                      {\sum x_i x_i \sum y_i y_i - \sum x_i y_i \sum x_i y_i}}
		/// \]
		/// 
		/// <p>Once we know \(\omega\) we can compute:
		/// \[
		///    fc = \omega f(t) \cos(\omega t) - f'(t) \sin(\omega t)
		/// \]
		/// \[
		///    fs = \omega f(t) \sin(\omega t) + f'(t) \cos(\omega t)
		/// \]
		/// </p>
		/// 
		/// <p>It appears that \(fc = a \omega \cos(\phi)\) and
		/// \(fs = -a \omega \sin(\phi)\), so we can use these
		/// expressions to compute \(\phi\). The best estimate over the sample is
		/// given by averaging these expressions.
		/// </p>
		/// 
		/// <p>Since integrals and means are involved in the preceding
		/// estimations, these operations run in \(O(n)\) time, where \(n\) is the
		/// number of measurements.</p>
		/// </summary>
		public class ParameterGuesser
		{
			/// <summary>
			/// Amplitude. </summary>
			internal readonly double a;
			/// <summary>
			/// Angular frequency. </summary>
			internal readonly double omega;
			/// <summary>
			/// Phase. </summary>
			internal readonly double phi;

			/// <summary>
			/// Simple constructor.
			/// </summary>
			/// <param name="observations"> Sampled observations. </param>
			/// <exception cref="NumberIsTooSmallException"> if the sample is too short. </exception>
			/// <exception cref="ZeroException"> if the abscissa range is zero. </exception>
			/// <exception cref="MathIllegalStateException"> when the guessing procedure cannot
			/// produce sensible results. </exception>
			public ParameterGuesser(ICollection<WeightedObservedPoint> observations)
			{
				if (observations.Count < 4)
				{
					throw new NumberIsTooSmallException(LocalizedFormats.INSUFFICIENT_OBSERVED_POINTS_IN_SAMPLE, observations.Count, 4, true);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint[] sorted = sortObservations(observations).toArray(new WeightedObservedPoint[0]);
				WeightedObservedPoint[] sorted = sortObservations(observations).ToArray();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aOmega[] = guessAOmega(sorted);
				double[] aOmega = guessAOmega(sorted);
				a = aOmega[0];
				omega = aOmega[1];

				phi = guessPhi(sorted);
			}

			/// <summary>
			/// Gets an estimation of the parameters.
			/// </summary>
			/// <returns> the guessed parameters, in the following order:
			/// <ul>
			///  <li>Amplitude</li>
			///  <li>Angular frequency</li>
			///  <li>Phase</li>
			/// </ul> </returns>
			public virtual double[] guess()
			{
				return new double[] {a, omega, phi};
			}

			/// <summary>
			/// Sort the observations with respect to the abscissa.
			/// </summary>
			/// <param name="unsorted"> Input observations. </param>
			/// <returns> the input observations, sorted. </returns>
			internal virtual IList<WeightedObservedPoint> sortObservations(ICollection<WeightedObservedPoint> unsorted)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<WeightedObservedPoint> observations = new java.util.ArrayList<WeightedObservedPoint>(unsorted);
				IList<WeightedObservedPoint> observations = new List<WeightedObservedPoint>(unsorted);

				// Since the samples are almost always already sorted, this
				// method is implemented as an insertion sort that reorders the
				// elements in place. Insertion sort is very efficient in this case.
				WeightedObservedPoint curr = observations[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int len = observations.size();
				int len = observations.Count;
				for (int j = 1; j < len; j++)
				{
					WeightedObservedPoint prec = curr;
					curr = observations[j];
					if (curr.X < prec.X)
					{
						// the current element should be inserted closer to the beginning
						int i = j - 1;
						WeightedObservedPoint mI = observations[i];
						while ((i >= 0) && (curr.X < mI.X))
						{
							observations[i + 1] = mI;
							if (i-- != 0)
							{
								mI = observations[i];
							}
						}
						observations[i + 1] = curr;
						curr = observations[j];
					}
				}

				return observations;
			}

			/// <summary>
			/// Estimate a first guess of the amplitude and angular frequency.
			/// </summary>
			/// <param name="observations"> Observations, sorted w.r.t. abscissa. </param>
			/// <exception cref="ZeroException"> if the abscissa range is zero. </exception>
			/// <exception cref="MathIllegalStateException"> when the guessing procedure cannot
			/// produce sensible results. </exception>
			/// <returns> the guessed amplitude (at index 0) and circular frequency
			/// (at index 1). </returns>
			internal virtual double[] guessAOmega(WeightedObservedPoint[] observations)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] aOmega = new double[2];
				double[] aOmega = new double[2];

				// initialize the sums for the linear model between the two integrals
				double sx2 = 0;
				double sy2 = 0;
				double sxy = 0;
				double sxz = 0;
				double syz = 0;

				double currentX = observations[0].X;
				double currentY = observations[0].Y;
				double f2Integral = 0;
				double fPrime2Integral = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double startX = currentX;
				double startX = currentX;
				for (int i = 1; i < observations.Length; ++i)
				{
					// one step forward
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousX = currentX;
					double previousX = currentX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousY = currentY;
					double previousY = currentY;
					currentX = observations[i].X;
					currentY = observations[i].Y;

					// update the integrals of f<sup>2</sup> and f'<sup>2</sup>
					// considering a linear model for f (and therefore constant f')
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dx = currentX - previousX;
					double dx = currentX - previousX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dy = currentY - previousY;
					double dy = currentY - previousY;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f2StepIntegral = dx * (previousY * previousY + previousY * currentY + currentY * currentY) / 3;
					double f2StepIntegral = dx * (previousY * previousY + previousY * currentY + currentY * currentY) / 3;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fPrime2StepIntegral = dy * dy / dx;
					double fPrime2StepIntegral = dy * dy / dx;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = currentX - startX;
					double x = currentX - startX;
					f2Integral += f2StepIntegral;
					fPrime2Integral += fPrime2StepIntegral;

					sx2 += x * x;
					sy2 += f2Integral * f2Integral;
					sxy += x * f2Integral;
					sxz += x * fPrime2Integral;
					syz += f2Integral * fPrime2Integral;
				}

				// compute the amplitude and pulsation coefficients
				double c1 = sy2 * sxz - sxy * syz;
				double c2 = sxy * sxz - sx2 * syz;
				double c3 = sx2 * sy2 - sxy * sxy;
				if ((c1 / c2 < 0) || (c2 / c3 < 0))
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int last = observations.length - 1;
					int last = observations.Length - 1;
					// Range of the observations, assuming that the
					// observations are sorted.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xRange = observations[last].getX() - observations[0].getX();
					double xRange = observations[last].X - observations[0].X;
					if (xRange == 0)
					{
						throw new ZeroException();
					}
					aOmega[1] = 2 * Math.PI / xRange;

					double yMin = double.PositiveInfinity;
					double yMax = double.NegativeInfinity;
					for (int i = 1; i < observations.Length; ++i)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = observations[i].getY();
						double y = observations[i].Y;
						if (y < yMin)
						{
							yMin = y;
						}
						if (y > yMax)
						{
							yMax = y;
						}
					}
					aOmega[0] = 0.5 * (yMax - yMin);
				}
				else
				{
					if (c2 == 0)
					{
						// In some ill-conditioned cases (cf. MATH-844), the guesser
						// procedure cannot produce sensible results.
						throw new MathIllegalStateException(LocalizedFormats.ZERO_DENOMINATOR);
					}

					aOmega[0] = FastMath.sqrt(c1 / c2);
					aOmega[1] = FastMath.sqrt(c2 / c3);
				}

				return aOmega;
			}

			/// <summary>
			/// Estimate a first guess of the phase.
			/// </summary>
			/// <param name="observations"> Observations, sorted w.r.t. abscissa. </param>
			/// <returns> the guessed phase. </returns>
			internal virtual double guessPhi(WeightedObservedPoint[] observations)
			{
				// initialize the means
				double fcMean = 0;
				double fsMean = 0;

				double currentX = observations[0].X;
				double currentY = observations[0].Y;
				for (int i = 1; i < observations.Length; ++i)
				{
					// one step forward
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousX = currentX;
					double previousX = currentX;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double previousY = currentY;
					double previousY = currentY;
					currentX = observations[i].X;
					currentY = observations[i].Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double currentYPrime = (currentY - previousY) / (currentX - previousX);
					double currentYPrime = (currentY - previousY) / (currentX - previousX);

					double omegaX = omega * currentX;
					double cosine = FastMath.cos(omegaX);
					double sine = FastMath.sin(omegaX);
					fcMean += omega * currentY * cosine - currentYPrime * sine;
					fsMean += omega * currentY * sine + currentYPrime * cosine;
				}

				return FastMath.atan2(-fsMean, fcMean);
			}
		}
	}

}