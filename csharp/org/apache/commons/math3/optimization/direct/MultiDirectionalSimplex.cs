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

namespace org.apache.commons.math3.optimization.direct
{

	using MultivariateFunction = org.apache.commons.math3.analysis.MultivariateFunction;

	/// <summary>
	/// This class implements the multi-directional direct search method.
	/// 
	/// @version $Id: MultiDirectionalSimplex.java 1422230 2012-12-15 12:11:13Z erans $ </summary>
	/// @deprecated As of 3.1 (to be removed in 4.0).
	/// @since 3.0 
	[Obsolete("As of 3.1 (to be removed in 4.0).")]
	public class MultiDirectionalSimplex : AbstractSimplex
	{
		/// <summary>
		/// Default value for <seealso cref="#khi"/>: {@value}. </summary>
		private const double DEFAULT_KHI = 2;
		/// <summary>
		/// Default value for <seealso cref="#gamma"/>: {@value}. </summary>
		private const double DEFAULT_GAMMA = 0.5;
		/// <summary>
		/// Expansion coefficient. </summary>
		private readonly double khi;
		/// <summary>
		/// Contraction coefficient. </summary>
		private readonly double gamma;

		/// <summary>
		/// Build a multi-directional simplex with default coefficients.
		/// The default values are 2.0 for khi and 0.5 for gamma.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final int n)
		public MultiDirectionalSimplex(int n) : this(n, 1d)
		{
		}

		/// <summary>
		/// Build a multi-directional simplex with default coefficients.
		/// The default values are 2.0 for khi and 0.5 for gamma.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. </param>
		/// <param name="sideLength"> Length of the sides of the default (hypercube)
		/// simplex. See <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final int n, double sideLength)
		public MultiDirectionalSimplex(int n, double sideLength) : this(n, sideLength, DEFAULT_KHI, DEFAULT_GAMMA)
		{
		}

		/// <summary>
		/// Build a multi-directional simplex with specified coefficients.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final int n, final double khi, final double gamma)
		public MultiDirectionalSimplex(int n, double khi, double gamma) : this(n, 1d, khi, gamma)
		{
		}

		/// <summary>
		/// Build a multi-directional simplex with specified coefficients.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
		/// <param name="sideLength"> Length of the sides of the default (hypercube)
		/// simplex. See <seealso cref="AbstractSimplex#AbstractSimplex(int,double)"/>. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final int n, double sideLength, final double khi, final double gamma)
		public MultiDirectionalSimplex(int n, double sideLength, double khi, double gamma) : base(n, sideLength)
		{

			this.khi = khi;
			this.gamma = gamma;
		}

		/// <summary>
		/// Build a multi-directional simplex with default coefficients.
		/// The default values are 2.0 for khi and 0.5 for gamma.
		/// </summary>
		/// <param name="steps"> Steps along the canonical axes representing box edges.
		/// They may be negative but not zero. See </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final double[] steps)
		public MultiDirectionalSimplex(double[] steps) : this(steps, DEFAULT_KHI, DEFAULT_GAMMA)
		{
		}

		/// <summary>
		/// Build a multi-directional simplex with specified coefficients.
		/// </summary>
		/// <param name="steps"> Steps along the canonical axes representing box edges.
		/// They may be negative but not zero. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(double[])"/>. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final double[] steps, final double khi, final double gamma)
		public MultiDirectionalSimplex(double[] steps, double khi, double gamma) : base(steps)
		{

			this.khi = khi;
			this.gamma = gamma;
		}

		/// <summary>
		/// Build a multi-directional simplex with default coefficients.
		/// The default values are 2.0 for khi and 0.5 for gamma.
		/// </summary>
		/// <param name="referenceSimplex"> Reference simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(double[][])"/>. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final double[][] referenceSimplex)
		public MultiDirectionalSimplex(double[][] referenceSimplex) : this(referenceSimplex, DEFAULT_KHI, DEFAULT_GAMMA)
		{
		}

		/// <summary>
		/// Build a multi-directional simplex with specified coefficients.
		/// </summary>
		/// <param name="referenceSimplex"> Reference simplex. See
		/// <seealso cref="AbstractSimplex#AbstractSimplex(double[][])"/>. </param>
		/// <param name="khi"> Expansion coefficient. </param>
		/// <param name="gamma"> Contraction coefficient. </param>
		/// <exception cref="org.apache.commons.math3.exception.NotStrictlyPositiveException">
		/// if the reference simplex does not contain at least one point. </exception>
		/// <exception cref="org.apache.commons.math3.exception.DimensionMismatchException">
		/// if there is a dimension mismatch in the reference simplex. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public MultiDirectionalSimplex(final double[][] referenceSimplex, final double khi, final double gamma)
		public MultiDirectionalSimplex(double[][] referenceSimplex, double khi, double gamma) : base(referenceSimplex)
		{

			this.khi = khi;
			this.gamma = gamma;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public void iterate(final org.apache.commons.math3.analysis.MultivariateFunction evaluationFunction, final java.util.Comparator<org.apache.commons.math3.optimization.PointValuePair> comparator)
		public override void iterate(MultivariateFunction evaluationFunction, IComparer<PointValuePair> comparator)
		{
			// Save the original simplex.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optimization.PointValuePair[] original = getPoints();
			PointValuePair[] original = Points;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optimization.PointValuePair best = original[0];
			PointValuePair best = original[0];

			// Perform a reflection step.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optimization.PointValuePair reflected = evaluateNewSimplex(evaluationFunction, original, 1, comparator);
			PointValuePair reflected = evaluateNewSimplex(evaluationFunction, original, 1, comparator);
			if (comparator.Compare(reflected, best) < 0)
			{
				// Compute the expanded simplex.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optimization.PointValuePair[] reflectedSimplex = getPoints();
				PointValuePair[] reflectedSimplex = Points;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optimization.PointValuePair expanded = evaluateNewSimplex(evaluationFunction, original, khi, comparator);
				PointValuePair expanded = evaluateNewSimplex(evaluationFunction, original, khi, comparator);
				if (comparator.Compare(reflected, expanded) <= 0)
				{
					// Keep the reflected simplex.
					Points = reflectedSimplex;
				}
				// Keep the expanded simplex.
				return;
			}

			// Compute the contracted simplex.
			evaluateNewSimplex(evaluationFunction, original, gamma, comparator);

		}

		/// <summary>
		/// Compute and evaluate a new simplex.
		/// </summary>
		/// <param name="evaluationFunction"> Evaluation function. </param>
		/// <param name="original"> Original simplex (to be preserved). </param>
		/// <param name="coeff"> Linear coefficient. </param>
		/// <param name="comparator"> Comparator to use to sort simplex vertices from best
		/// to poorest. </param>
		/// <returns> the best point in the transformed simplex. </returns>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private org.apache.commons.math3.optimization.PointValuePair evaluateNewSimplex(final org.apache.commons.math3.analysis.MultivariateFunction evaluationFunction, final org.apache.commons.math3.optimization.PointValuePair[] original, final double coeff, final java.util.Comparator<org.apache.commons.math3.optimization.PointValuePair> comparator)
		private PointValuePair evaluateNewSimplex(MultivariateFunction evaluationFunction, PointValuePair[] original, double coeff, IComparer<PointValuePair> comparator)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xSmallest = original[0].getPointRef();
			double[] xSmallest = original[0].PointRef;
			// Perform a linear transformation on all the simplex points,
			// except the first one.
			setPoint(0, original[0]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dim = getDimension();
			int dim = Dimension;
			for (int i = 1; i < Size; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xOriginal = original[i].getPointRef();
				double[] xOriginal = original[i].PointRef;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] xTransformed = new double[dim];
				double[] xTransformed = new double[dim];
				for (int j = 0; j < dim; j++)
				{
					xTransformed[j] = xSmallest[j] + coeff * (xSmallest[j] - xOriginal[j]);
				}
				setPoint(i, new PointValuePair(xTransformed, double.NaN, false));
			}

			// Evaluate the simplex.
			evaluate(evaluationFunction, comparator);

			return getPoint(0);
		}
	}

}