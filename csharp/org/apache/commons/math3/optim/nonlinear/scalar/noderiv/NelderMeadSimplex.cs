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
namespace org.apache.commons.math3.optim.nonlinear.scalar.noderiv
{

	using MultivariateFunction = org.apache.commons.math3.analysis.MultivariateFunction;

	/// <summary>
	/// This class implements the Nelder-Mead simplex algorithm.
	/// 
	/// @version $Id: NelderMeadSimplex.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.0
	/// </summary>
	public class NelderMeadSimplex : AbstractSimplex
	{
		/// <summary>
		/// Default value for <seealso cref="#rho"/>: {@value}. </summary>
		private const double DEFAULT_RHO = 1;
		/// <summary>
		/// Default value for <seealso cref="#khi"/>: {@value}. </summary>
		private const double DEFAULT_KHI = 2;
		/// <summary>
		/// Default value for <seealso cref="#gamma"/>: {@value}. </summary>
		private const double DEFAULT_GAMMA = 0.5;
		/// <summary>
		/// Default value for <seealso cref="#sigma"/>: {@value}. </summary>
		private const double DEFAULT_SIGMA = 0.5;
		/// <summary>
		/// Reflection coefficient. </summary>
		private readonly double rho;
		/// <summary>
		/// Expansion coefficient. </summary>
		private readonly double khi;
		/// <summary>
		/// Contraction coefficient. </summary>
		private readonly double gamma;
		/// <summary>
		/// Shrinkage coefficient. </summary>
		private readonly double sigma;

		/// <summary>
		/// Build a Nelder-Mead simplex with default coefficients.
		/// The default coefficients are 1.0 for rho, 2.0 for khi and 0.5
		/// for both gamma and sigma.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final int n)
		public NelderMeadSimplex(int n) : this(n, 1d)
		{
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with default coefficients.
		/// The default coefficients are 1.0 for rho, 2.0 for khi and 0.5
		/// for both gamma and sigma.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. </param>
		/// <param name="sideLength"> Length of the sides of the default (hypercube)
		/// simplex. See <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final int n, double sideLength)
		public NelderMeadSimplex(int n, double sideLength) : this(n, sideLength, DEFAULT_RHO, DEFAULT_KHI, DEFAULT_GAMMA, DEFAULT_SIGMA)
		{
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with specified coefficients.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
		/// <param name="sideLength"> Length of the sides of the default (hypercube)
		/// simplex. See <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
		/// <param name="rho"> Reflection coefficient. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
		/// <param name="sigma"> Shrinkage coefficient. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final int n, double sideLength, final double rho, final double khi, final double gamma, final double sigma)
		public NelderMeadSimplex(int n, double sideLength, double rho, double khi, double gamma, double sigma) : base(n, sideLength)
		{

			this.rho = rho;
			this.khi = khi;
			this.gamma = gamma;
			this.sigma = sigma;
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with specified coefficients.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(int)"/>. </param>
		/// <param name="rho"> Reflection coefficient. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
		/// <param name="sigma"> Shrinkage coefficient. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final int n, final double rho, final double khi, final double gamma, final double sigma)
		public NelderMeadSimplex(int n, double rho, double khi, double gamma, double sigma) : this(n, 1d, rho, khi, gamma, sigma)
		{
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with default coefficients.
		/// The default coefficients are 1.0 for rho, 2.0 for khi and 0.5
		/// for both gamma and sigma.
		/// </summary>
		/// <param name="steps"> Steps along the canonical axes representing box edges.
		/// They may be negative but not zero. See </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final double[] steps)
		public NelderMeadSimplex(double[] steps) : this(steps, DEFAULT_RHO, DEFAULT_KHI, DEFAULT_GAMMA, DEFAULT_SIGMA)
		{
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with specified coefficients.
		/// </summary>
		/// <param name="steps"> Steps along the canonical axes representing box edges.
		/// They may be negative but not zero. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(double[])"/>. </param>
		/// <param name="rho"> Reflection coefficient. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
		/// <param name="sigma"> Shrinkage coefficient. </param>
		/// <exception cref="IllegalArgumentException"> if one of the steps is zero. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final double[] steps, final double rho, final double khi, final double gamma, final double sigma)
		public NelderMeadSimplex(double[] steps, double rho, double khi, double gamma, double sigma) : base(steps)
		{

			this.rho = rho;
			this.khi = khi;
			this.gamma = gamma;
			this.sigma = sigma;
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with default coefficients.
		/// The default coefficients are 1.0 for rho, 2.0 for khi and 0.5
		/// for both gamma and sigma.
		/// </summary>
		/// <param name="referenceSimplex"> Reference simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(double[][])"/>. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final double[][] referenceSimplex)
		public NelderMeadSimplex(double[][] referenceSimplex) : this(referenceSimplex, DEFAULT_RHO, DEFAULT_KHI, DEFAULT_GAMMA, DEFAULT_SIGMA)
		{
		}

		/// <summary>
		/// Build a Nelder-Mead simplex with specified coefficients.
		/// </summary>
		/// <param name="referenceSimplex"> Reference simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(double[][])"/>. </param>
		/// <param name="rho"> Reflection coefficient. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
		/// <param name="sigma"> Shrinkage coefficient. </param>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if the reference simplex does not contain at least one point. </exception>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if there is a dimension mismatch in the reference simplex. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public NelderMeadSimplex(final double[][] referenceSimplex, final double rho, final double khi, final double gamma, final double sigma)
		public NelderMeadSimplex(double[][] referenceSimplex, double rho, double khi, double gamma, double sigma) : base(referenceSimplex)
		{

			this.rho = rho;
			this.khi = khi;
			this.gamma = gamma;
			this.sigma = sigma;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void iterate(final org.apache.commons.math3.analysis.MultivariateFunction evaluationFunction, final java.util.Comparator<org.apache.commons.math3.optim.PointValuePair> comparator)
		public override void iterate(MultivariateFunction evaluationFunction, IComparer<PointValuePair> comparator)
		{
			// The simplex has n + 1 points if dimension is n.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = getDimension();
			int n = Dimension;

			// Interesting values.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair best = getPoint(0);
			PointValuePair best = getPoint(0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair secondBest = getPoint(n - 1);
			PointValuePair secondBest = getPoint(n - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair worst = getPoint(n);
			PointValuePair worst = getPoint(n);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xWorst = worst.getPointRef();
			double[] xWorst = worst.PointRef;

			// Compute the centroid of the best vertices (dismissing the worst
			// point at index n).
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] centroid = new double[n];
			double[] centroid = new double[n];
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = getPoint(i).getPointRef();
				double[] x = getPoint(i).PointRef;
				for (int j = 0; j < n; j++)
				{
					centroid[j] += x[j];
				}
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double scaling = 1.0 / n;
			double scaling = 1.0 / n;
			for (int j = 0; j < n; j++)
			{
				centroid[j] *= scaling;
			}

			// compute the reflection point
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xR = new double[n];
			double[] xR = new double[n];
			for (int j = 0; j < n; j++)
			{
				xR[j] = centroid[j] + rho * (centroid[j] - xWorst[j]);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair reflected = new org.apache.commons.math3.optim.PointValuePair(xR, evaluationFunction.value(xR), false);
			PointValuePair reflected = new PointValuePair(xR, evaluationFunction.value(xR), false);

			if (comparator.Compare(best, reflected) <= 0 && comparator.Compare(reflected, secondBest) < 0)
			{
				// Accept the reflected point.
				replaceWorstPoint(reflected, comparator);
			}
			else if (comparator.Compare(reflected, best) < 0)
			{
				// Compute the expansion point.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xE = new double[n];
				double[] xE = new double[n];
				for (int j = 0; j < n; j++)
				{
					xE[j] = centroid[j] + khi * (xR[j] - centroid[j]);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair expanded = new org.apache.commons.math3.optim.PointValuePair(xE, evaluationFunction.value(xE), false);
				PointValuePair expanded = new PointValuePair(xE, evaluationFunction.value(xE), false);

				if (comparator.Compare(expanded, reflected) < 0)
				{
					// Accept the expansion point.
					replaceWorstPoint(expanded, comparator);
				}
				else
				{
					// Accept the reflected point.
					replaceWorstPoint(reflected, comparator);
				}
			}
			else
			{
				if (comparator.Compare(reflected, worst) < 0)
				{
					// Perform an outside contraction.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xC = new double[n];
					double[] xC = new double[n];
					for (int j = 0; j < n; j++)
					{
						xC[j] = centroid[j] + gamma * (xR[j] - centroid[j]);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair outContracted = new org.apache.commons.math3.optim.PointValuePair(xC, evaluationFunction.value(xC), false);
					PointValuePair outContracted = new PointValuePair(xC, evaluationFunction.value(xC), false);
					if (comparator.Compare(outContracted, reflected) <= 0)
					{
						// Accept the contraction point.
						replaceWorstPoint(outContracted, comparator);
						return;
					}
				}
				else
				{
					// Perform an inside contraction.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xC = new double[n];
					double[] xC = new double[n];
					for (int j = 0; j < n; j++)
					{
						xC[j] = centroid[j] - gamma * (centroid[j] - xWorst[j]);
					}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair inContracted = new org.apache.commons.math3.optim.PointValuePair(xC, evaluationFunction.value(xC), false);
					PointValuePair inContracted = new PointValuePair(xC, evaluationFunction.value(xC), false);

					if (comparator.Compare(inContracted, worst) < 0)
					{
						// Accept the contraction point.
						replaceWorstPoint(inContracted, comparator);
						return;
					}
				}

				// Perform a shrink.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xSmallest = getPoint(0).getPointRef();
				double[] xSmallest = getPoint(0).PointRef;
				for (int i = 1; i <= n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x = getPoint(i).getPoint();
					double[] x = getPoint(i).Point;
					for (int j = 0; j < n; j++)
					{
						x[j] = xSmallest[j] + sigma * (x[j] - xSmallest[j]);
					}
					setPoint(i, new PointValuePair(x, double.NaN, false));
				}
				evaluate(evaluationFunction, comparator);
			}
		}
	}

}