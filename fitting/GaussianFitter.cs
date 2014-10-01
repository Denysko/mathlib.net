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

	using Gaussian = mathlib.analysis.function.Gaussian;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using ZeroException = mathlib.exception.ZeroException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MultivariateVectorOptimizer = mathlib.optim.nonlinear.vector.MultivariateVectorOptimizer;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Fits points to a {@link
	/// mathlib.analysis.function.Gaussian.Parametric Gaussian} function.
	/// <p>
	/// Usage example:
	/// <pre>
	///   GaussianFitter fitter = new GaussianFitter(
	///     new LevenbergMarquardtOptimizer());
	///   fitter.addObservedPoint(4.0254623,  531026.0);
	///   fitter.addObservedPoint(4.03128248, 984167.0);
	///   fitter.addObservedPoint(4.03839603, 1887233.0);
	///   fitter.addObservedPoint(4.04421621, 2687152.0);
	///   fitter.addObservedPoint(4.05132976, 3461228.0);
	///   fitter.addObservedPoint(4.05326982, 3580526.0);
	///   fitter.addObservedPoint(4.05779662, 3439750.0);
	///   fitter.addObservedPoint(4.0636168,  2877648.0);
	///   fitter.addObservedPoint(4.06943698, 2175960.0);
	///   fitter.addObservedPoint(4.07525716, 1447024.0);
	///   fitter.addObservedPoint(4.08237071, 717104.0);
	///   fitter.addObservedPoint(4.08366408, 620014.0);
	///   double[] parameters = fitter.fit();
	/// </pre>
	/// 
	/// @since 2.2
	/// @version $Id: GaussianFitter.java 1416643 2012-12-03 19:37:14Z tn $ </summary>
	/// @deprecated As of 3.3. Please use <seealso cref="GaussianCurveFitter"/> and
	/// <seealso cref="WeightedObservedPoints"/> instead. 
	[Obsolete]//("As of 3.3. Please use <seealso cref="GaussianCurveFitter"/> and")]
	public class GaussianFitter : CurveFitter<Gaussian.Parametric>
	{
		/// <summary>
		/// Constructs an instance using the specified optimizer.
		/// </summary>
		/// <param name="optimizer"> Optimizer to use for the fitting. </param>
		public GaussianFitter(MultivariateVectorOptimizer optimizer) : base(optimizer)
		{
		}

		/// <summary>
		/// Fits a Gaussian function to the observed points.
		/// </summary>
		/// <param name="initialGuess"> First guess values in the following order:
		/// <ul>
		///  <li>Norm</li>
		///  <li>Mean</li>
		///  <li>Sigma</li>
		/// </ul> </param>
		/// <returns> the parameters of the Gaussian function that best fits the
		/// observed points (in the same order as above).
		/// @since 3.0 </returns>
		public virtual double[] fit(double[] initialGuess)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.analysis.function.Gaussian.Parametric f = new mathlib.analysis.function.Gaussian.Parametric()
			Gaussian.Parametric f = new ParametricAnonymousInnerClassHelper(this);

			return fit(f, initialGuess);
		}

		private class ParametricAnonymousInnerClassHelper : Gaussian.Parametric
		{
			private readonly GaussianFitter outerInstance;

			public ParametricAnonymousInnerClassHelper(GaussianFitter outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public override double value(double x, params double[] p)
			{
				double v = double.PositiveInfinity;
				try
				{
					v = base.value(x, p);
				} // NOPMD
				catch (NotStrictlyPositiveException e)
				{
					// Do nothing.
				}
				return v;
			}

			public override double[] gradient(double x, params double[] p)
			{
				double[] v = new double[] {double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity};
				try
				{
					v = base.gradient(x, p);
				} // NOPMD
				catch (NotStrictlyPositiveException e)
				{
					// Do nothing.
				}
				return v;
			}
		}

		/// <summary>
		/// Fits a Gaussian function to the observed points.
		/// </summary>
		/// <returns> the parameters of the Gaussian function that best fits the
		/// observed points (in the same order as above). </returns>
		public virtual double[] fit()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] guess = (new ParameterGuesser(getObservations())).guess();
			double[] guess = (new ParameterGuesser(Observations)).guess();
			return fit(guess);
		}

		/// <summary>
		/// Guesses the parameters {@code norm}, {@code mean}, and {@code sigma}
		/// of a <seealso cref="mathlib.analysis.function.Gaussian.Parametric"/>
		/// based on the specified observed points.
		/// </summary>
		public class ParameterGuesser
		{
			/// <summary>
			/// Normalization factor. </summary>
			internal readonly double norm;
			/// <summary>
			/// Mean. </summary>
			internal readonly double mean;
			/// <summary>
			/// Standard deviation. </summary>
			internal readonly double sigma;

			/// <summary>
			/// Constructs instance with the specified observed points.
			/// </summary>
			/// <param name="observations"> Observed points from which to guess the
			/// parameters of the Gaussian. </param>
			/// <exception cref="NullArgumentException"> if {@code observations} is
			/// {@code null}. </exception>
			/// <exception cref="NumberIsTooSmallException"> if there are less than 3
			/// observations. </exception>
			public ParameterGuesser(WeightedObservedPoint[] observations)
			{
				if (observations == null)
				{
					throw new NullArgumentException(LocalizedFormats.INPUT_ARRAY);
				}
				if (observations.Length < 3)
				{
					throw new NumberIsTooSmallException(observations.Length, 3, true);
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint[] sorted = sortObservations(observations);
				WeightedObservedPoint[] sorted = sortObservations(observations);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] params = basicGuess(sorted);
				double[] @params = basicGuess(sorted);

				norm = @params[0];
				mean = @params[1];
				sigma = @params[2];
			}

			/// <summary>
			/// Gets an estimation of the parameters.
			/// </summary>
			/// <returns> the guessed parameters, in the following order:
			/// <ul>
			///  <li>Normalization factor</li>
			///  <li>Mean</li>
			///  <li>Standard deviation</li>
			/// </ul> </returns>
			public virtual double[] guess()
			{
				return new double[] {norm, mean, sigma};
			}

			/// <summary>
			/// Sort the observations.
			/// </summary>
			/// <param name="unsorted"> Input observations. </param>
			/// <returns> the input observations, sorted. </returns>
			internal virtual WeightedObservedPoint[] sortObservations(WeightedObservedPoint[] unsorted)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint[] observations = unsorted.clone();
				WeightedObservedPoint[] observations = unsorted.clone();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Comparator<WeightedObservedPoint> cmp = new java.util.Comparator<WeightedObservedPoint>()
				IComparer<WeightedObservedPoint> cmp = new ComparatorAnonymousInnerClassHelper(this);

				Arrays.sort(observations, cmp);
				return observations;
			}

			private class ComparatorAnonymousInnerClassHelper : IComparer<WeightedObservedPoint>
			{
				private readonly ParameterGuesser outerInstance;

				public ComparatorAnonymousInnerClassHelper(ParameterGuesser outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public virtual int Compare(WeightedObservedPoint p1, WeightedObservedPoint p2)
				{
					if (p1 == null && p2 == null)
					{
						return 0;
					}
					if (p1 == null)
					{
						return -1;
					}
					if (p2 == null)
					{
						return 1;
					}
					if (p1.X < p2.X)
					{
						return -1;
					}
					if (p1.X > p2.X)
					{
						return 1;
					}
					if (p1.Y < p2.Y)
					{
						return -1;
					}
					if (p1.Y > p2.Y)
					{
						return 1;
					}
					if (p1.Weight < p2.Weight)
					{
						return -1;
					}
					if (p1.Weight > p2.Weight)
					{
						return 1;
					}
					return 0;
				}
			}

			/// <summary>
			/// Guesses the parameters based on the specified observed points.
			/// </summary>
			/// <param name="points"> Observed points, sorted. </param>
			/// <returns> the guessed parameters (normalization factor, mean and
			/// sigma). </returns>
			internal virtual double[] basicGuess(WeightedObservedPoint[] points)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxYIdx = findMaxY(points);
				int maxYIdx = findMaxY(points);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double n = points[maxYIdx].getY();
				double n = points[maxYIdx].Y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double m = points[maxYIdx].getX();
				double m = points[maxYIdx].X;

				double fwhmApprox;
				try
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double halfY = n + ((m - n) / 2);
					double halfY = n + ((m - n) / 2);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fwhmX1 = interpolateXAtY(points, maxYIdx, -1, halfY);
					double fwhmX1 = interpolateXAtY(points, maxYIdx, -1, halfY);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fwhmX2 = interpolateXAtY(points, maxYIdx, 1, halfY);
					double fwhmX2 = interpolateXAtY(points, maxYIdx, 1, halfY);
					fwhmApprox = fwhmX2 - fwhmX1;
				}
				catch (OutOfRangeException e)
				{
					// TODO: Exceptions should not be used for flow control.
					fwhmApprox = points[points.Length - 1].X - points[0].X;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double s = fwhmApprox / (2 * mathlib.util.FastMath.sqrt(2 * mathlib.util.FastMath.log(2)));
				double s = fwhmApprox / (2 * FastMath.sqrt(2 * FastMath.log(2)));

				return new double[] {n, m, s};
			}

			/// <summary>
			/// Finds index of point in specified points with the largest Y.
			/// </summary>
			/// <param name="points"> Points to search. </param>
			/// <returns> the index in specified points array. </returns>
			internal virtual int findMaxY(WeightedObservedPoint[] points)
			{
				int maxYIdx = 0;
				for (int i = 1; i < points.Length; i++)
				{
					if (points[i].Y > points[maxYIdx].Y)
					{
						maxYIdx = i;
					}
				}
				return maxYIdx;
			}

			/// <summary>
			/// Interpolates using the specified points to determine X at the
			/// specified Y.
			/// </summary>
			/// <param name="points"> Points to use for interpolation. </param>
			/// <param name="startIdx"> Index within points from which to start the search for
			/// interpolation bounds points. </param>
			/// <param name="idxStep"> Index step for searching interpolation bounds points. </param>
			/// <param name="y"> Y value for which X should be determined. </param>
			/// <returns> the value of X for the specified Y. </returns>
			/// <exception cref="ZeroException"> if {@code idxStep} is 0. </exception>
			/// <exception cref="OutOfRangeException"> if specified {@code y} is not within the
			/// range of the specified {@code points}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double interpolateXAtY(WeightedObservedPoint[] points, int startIdx, int idxStep, double y) throws mathlib.exception.OutOfRangeException
			internal virtual double interpolateXAtY(WeightedObservedPoint[] points, int startIdx, int idxStep, double y)
			{
				if (idxStep == 0)
				{
					throw new ZeroException();
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint[] twoPoints = getInterpolationPointsForY(points, startIdx, idxStep, y);
				WeightedObservedPoint[] twoPoints = getInterpolationPointsForY(points, startIdx, idxStep, y);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint p1 = twoPoints[0];
				WeightedObservedPoint p1 = twoPoints[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint p2 = twoPoints[1];
				WeightedObservedPoint p2 = twoPoints[1];
				if (p1.Y == y)
				{
					return p1.X;
				}
				if (p2.Y == y)
				{
					return p2.X;
				}
				return p1.X + (((y - p1.Y) * (p2.X - p1.X)) / (p2.Y - p1.Y));
			}

			/// <summary>
			/// Gets the two bounding interpolation points from the specified points
			/// suitable for determining X at the specified Y.
			/// </summary>
			/// <param name="points"> Points to use for interpolation. </param>
			/// <param name="startIdx"> Index within points from which to start search for
			/// interpolation bounds points. </param>
			/// <param name="idxStep"> Index step for search for interpolation bounds points. </param>
			/// <param name="y"> Y value for which X should be determined. </param>
			/// <returns> the array containing two points suitable for determining X at
			/// the specified Y. </returns>
			/// <exception cref="ZeroException"> if {@code idxStep} is 0. </exception>
			/// <exception cref="OutOfRangeException"> if specified {@code y} is not within the
			/// range of the specified {@code points}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private WeightedObservedPoint[] getInterpolationPointsForY(WeightedObservedPoint[] points, int startIdx, int idxStep, double y) throws mathlib.exception.OutOfRangeException
			internal virtual WeightedObservedPoint[] getInterpolationPointsForY(WeightedObservedPoint[] points, int startIdx, int idxStep, double y)
			{
				if (idxStep == 0)
				{
					throw new ZeroException();
				}
				for (int i = startIdx; idxStep < 0 ? i + idxStep >= 0 : i + idxStep < points.Length; i += idxStep)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint p1 = points[i];
					WeightedObservedPoint p1 = points[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final WeightedObservedPoint p2 = points[i + idxStep];
					WeightedObservedPoint p2 = points[i + idxStep];
					if (isBetween(y, p1.Y, p2.Y))
					{
						if (idxStep < 0)
						{
							return new WeightedObservedPoint[] {p2, p1};
						}
						else
						{
							return new WeightedObservedPoint[] {p1, p2};
						}
					}
				}

				// Boundaries are replaced by dummy values because the raised
				// exception is caught and the message never displayed.
				// TODO: Exceptions should not be used for flow control.
				throw new OutOfRangeException(y, double.NegativeInfinity, double.PositiveInfinity);
			}

			/// <summary>
			/// Determines whether a value is between two other values.
			/// </summary>
			/// <param name="value"> Value to test whether it is between {@code boundary1}
			/// and {@code boundary2}. </param>
			/// <param name="boundary1"> One end of the range. </param>
			/// <param name="boundary2"> Other end of the range. </param>
			/// <returns> {@code true} if {@code value} is between {@code boundary1} and
			/// {@code boundary2} (inclusive), {@code false} otherwise. </returns>
			internal virtual bool isBetween(double value, double boundary1, double boundary2)
			{
				return (value >= boundary1 && value <= boundary2) || (value >= boundary2 && value <= boundary1);
			}
		}
	}

}