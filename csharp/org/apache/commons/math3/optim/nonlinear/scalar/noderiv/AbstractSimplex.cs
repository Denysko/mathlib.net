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

namespace org.apache.commons.math3.optim.nonlinear.scalar.noderiv
{


	using MultivariateFunction = org.apache.commons.math3.analysis.MultivariateFunction;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using ZeroException = org.apache.commons.math3.exception.ZeroException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// This class implements the simplex concept.
	/// It is intended to be used in conjunction with <seealso cref="SimplexOptimizer"/>.
	/// <br/>
	/// The initial configuration of the simplex is set by the constructors
	/// <seealso cref="#AbstractSimplex(double[])"/> or <seealso cref="#AbstractSimplex(double[][])"/>.
	/// The other <seealso cref="#AbstractSimplex(int) constructor"/> will set all steps
	/// to 1, thus building a default configuration from a unit hypercube.
	/// <br/>
	/// Users <em>must</em> call the <seealso cref="#build(double[]) build"/> method in order
	/// to create the data structure that will be acted on by the other methods of
	/// this class.
	/// </summary>
	/// <seealso cref= SimplexOptimizer
	/// @version $Id: AbstractSimplex.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.0 </seealso>
	public abstract class AbstractSimplex : OptimizationData
	{
		/// <summary>
		/// Simplex. </summary>
		private PointValuePair[] simplex;
		/// <summary>
		/// Start simplex configuration. </summary>
		private double[][] startConfiguration;
		/// <summary>
		/// Simplex dimension (must be equal to {@code simplex.length - 1}). </summary>
		private readonly int dimension;

		/// <summary>
		/// Build a unit hypercube simplex.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. </param>
		protected internal AbstractSimplex(int n) : this(n, 1d)
		{
		}

		/// <summary>
		/// Build a hypercube simplex with the given side length.
		/// </summary>
		/// <param name="n"> Dimension of the simplex. </param>
		/// <param name="sideLength"> Length of the sides of the hypercube. </param>
		protected internal AbstractSimplex(int n, double sideLength) : this(createHypercubeSteps(n, sideLength))
		{
		}

		/// <summary>
		/// The start configuration for simplex is built from a box parallel to
		/// the canonical axes of the space. The simplex is the subset of vertices
		/// of a box parallel to the canonical axes. It is built as the path followed
		/// while traveling from one vertex of the box to the diagonally opposite
		/// vertex moving only along the box edges. The first vertex of the box will
		/// be located at the start point of the optimization.
		/// As an example, in dimension 3 a simplex has 4 vertices. Setting the
		/// steps to (1, 10, 2) and the start point to (1, 1, 1) would imply the
		/// start simplex would be: { (1, 1, 1), (2, 1, 1), (2, 11, 1), (2, 11, 3) }.
		/// The first vertex would be set to the start point at (1, 1, 1) and the
		/// last vertex would be set to the diagonally opposite vertex at (2, 11, 3).
		/// </summary>
		/// <param name="steps"> Steps along the canonical axes representing box edges. They
		/// may be negative but not zero. </param>
		/// <exception cref="NullArgumentException"> if {@code steps} is {@code null}. </exception>
		/// <exception cref="ZeroException"> if one of the steps is zero. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractSimplex(final double[] steps)
		protected internal AbstractSimplex(double[] steps)
		{
			if (steps == null)
			{
				throw new NullArgumentException();
			}
			if (steps.Length == 0)
			{
				throw new ZeroException();
			}
			dimension = steps.Length;

			// Only the relative position of the n final vertices with respect
			// to the first one are stored.
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: startConfiguration = new double[dimension][dimension];
			startConfiguration = RectangularArrays.ReturnRectangularDoubleArray(dimension, dimension);
			for (int i = 0; i < dimension; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vertexI = startConfiguration[i];
				double[] vertexI = startConfiguration[i];
				for (int j = 0; j < i + 1; j++)
				{
					if (steps[j] == 0)
					{
						throw new ZeroException(LocalizedFormats.EQUAL_VERTICES_IN_SIMPLEX);
					}
					Array.Copy(steps, 0, vertexI, 0, j + 1);
				}
			}
		}

		/// <summary>
		/// The real initial simplex will be set up by moving the reference
		/// simplex such that its first point is located at the start point of the
		/// optimization.
		/// </summary>
		/// <param name="referenceSimplex"> Reference simplex. </param>
		/// <exception cref="NotStrictlyPositiveException"> if the reference simplex does not
		/// contain at least one point. </exception>
		/// <exception cref="DimensionMismatchException"> if there is a dimension mismatch
		/// in the reference simplex. </exception>
		/// <exception cref="IllegalArgumentException"> if one of its vertices is duplicated. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractSimplex(final double[][] referenceSimplex)
		protected internal AbstractSimplex(double[][] referenceSimplex)
		{
			if (referenceSimplex.Length <= 0)
			{
				throw new NotStrictlyPositiveException(LocalizedFormats.SIMPLEX_NEED_ONE_POINT, referenceSimplex.Length);
			}
			dimension = referenceSimplex.Length - 1;

			// Only the relative position of the n final vertices with respect
			// to the first one are stored.
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: startConfiguration = new double[dimension][dimension];
			startConfiguration = RectangularArrays.ReturnRectangularDoubleArray(dimension, dimension);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ref0 = referenceSimplex[0];
			double[] ref0 = referenceSimplex[0];

			// Loop over vertices.
			for (int i = 0; i < referenceSimplex.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] refI = referenceSimplex[i];
				double[] refI = referenceSimplex[i];

				// Safety checks.
				if (refI.Length != dimension)
				{
					throw new DimensionMismatchException(refI.Length, dimension);
				}
				for (int j = 0; j < i; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] refJ = referenceSimplex[j];
					double[] refJ = referenceSimplex[j];
					bool allEquals = true;
					for (int k = 0; k < dimension; k++)
					{
						if (refI[k] != refJ[k])
						{
							allEquals = false;
							break;
						}
					}
					if (allEquals)
					{
						throw new MathIllegalArgumentException(LocalizedFormats.EQUAL_VERTICES_IN_SIMPLEX, i, j);
					}
				}

				// Store vertex i position relative to vertex 0 position.
				if (i > 0)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] confI = startConfiguration[i - 1];
					double[] confI = startConfiguration[i - 1];
					for (int k = 0; k < dimension; k++)
					{
						confI[k] = refI[k] - ref0[k];
					}
				}
			}
		}

		/// <summary>
		/// Get simplex dimension.
		/// </summary>
		/// <returns> the dimension of the simplex. </returns>
		public virtual int Dimension
		{
			get
			{
				return dimension;
			}
		}

		/// <summary>
		/// Get simplex size.
		/// After calling the <seealso cref="#build(double[]) build"/> method, this method will
		/// will be equivalent to {@code getDimension() + 1}.
		/// </summary>
		/// <returns> the size of the simplex. </returns>
		public virtual int Size
		{
			get
			{
				return simplex.Length;
			}
		}

		/// <summary>
		/// Compute the next simplex of the algorithm.
		/// </summary>
		/// <param name="evaluationFunction"> Evaluation function. </param>
		/// <param name="comparator"> Comparator to use to sort simplex vertices from best
		/// to worst. </param>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the algorithm fails to converge. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public abstract void iterate(final org.apache.commons.math3.analysis.MultivariateFunction evaluationFunction, final java.util.Comparator<org.apache.commons.math3.optim.PointValuePair> comparator);
		public abstract void iterate(MultivariateFunction evaluationFunction, IComparer<PointValuePair> comparator);

		/// <summary>
		/// Build an initial simplex.
		/// </summary>
		/// <param name="startPoint"> First point of the simplex. </param>
		/// <exception cref="DimensionMismatchException"> if the start point does not match
		/// simplex dimension. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void build(final double[] startPoint)
		public virtual void build(double[] startPoint)
		{
			if (dimension != startPoint.Length)
			{
				throw new DimensionMismatchException(dimension, startPoint.Length);
			}

			// Set first vertex.
			simplex = new PointValuePair[dimension + 1];
			simplex[0] = new PointValuePair(startPoint, double.NaN);

			// Set remaining vertices.
			for (int i = 0; i < dimension; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] confI = startConfiguration[i];
				double[] confI = startConfiguration[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] vertexI = new double[dimension];
				double[] vertexI = new double[dimension];
				for (int k = 0; k < dimension; k++)
				{
					vertexI[k] = startPoint[k] + confI[k];
				}
				simplex[i + 1] = new PointValuePair(vertexI, double.NaN);
			}
		}

		/// <summary>
		/// Evaluate all the non-evaluated points of the simplex.
		/// </summary>
		/// <param name="evaluationFunction"> Evaluation function. </param>
		/// <param name="comparator"> Comparator to use to sort simplex vertices from best to worst. </param>
		/// <exception cref="org.apache.commons.math3.exception.TooManyEvaluationsException">
		/// if the maximal number of evaluations is exceeded. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void evaluate(final org.apache.commons.math3.analysis.MultivariateFunction evaluationFunction, final java.util.Comparator<org.apache.commons.math3.optim.PointValuePair> comparator)
		public virtual void evaluate(MultivariateFunction evaluationFunction, IComparer<PointValuePair> comparator)
		{
			// Evaluate the objective function at all non-evaluated simplex points.
			for (int i = 0; i < simplex.Length; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair vertex = simplex[i];
				PointValuePair vertex = simplex[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] point = vertex.getPointRef();
				double[] point = vertex.PointRef;
				if (double.IsNaN(vertex.Value))
				{
					simplex[i] = new PointValuePair(point, evaluationFunction.value(point), false);
				}
			}

			// Sort the simplex from best to worst.
			Arrays.sort(simplex, comparator);
		}

		/// <summary>
		/// Replace the worst point of the simplex by a new point.
		/// </summary>
		/// <param name="pointValuePair"> Point to insert. </param>
		/// <param name="comparator"> Comparator to use for sorting the simplex vertices
		/// from best to worst. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void replaceWorstPoint(org.apache.commons.math3.optim.PointValuePair pointValuePair, final java.util.Comparator<org.apache.commons.math3.optim.PointValuePair> comparator)
		protected internal virtual void replaceWorstPoint(PointValuePair pointValuePair, IComparer<PointValuePair> comparator)
		{
			for (int i = 0; i < dimension; i++)
			{
				if (comparator.Compare(simplex[i], pointValuePair) > 0)
				{
					PointValuePair tmp = simplex[i];
					simplex[i] = pointValuePair;
					pointValuePair = tmp;
				}
			}
			simplex[dimension] = pointValuePair;
		}

		/// <summary>
		/// Get the points of the simplex.
		/// </summary>
		/// <returns> all the simplex points. </returns>
		public virtual PointValuePair[] Points
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair[] copy = new org.apache.commons.math3.optim.PointValuePair[simplex.length];
				PointValuePair[] copy = new PointValuePair[simplex.Length];
				Array.Copy(simplex, 0, copy, 0, simplex.Length);
				return copy;
			}
			set
			{
				if (value.Length != simplex.Length)
				{
					throw new DimensionMismatchException(value.Length, simplex.Length);
				}
				simplex = value;
			}
		}

		/// <summary>
		/// Get the simplex point stored at the requested {@code index}.
		/// </summary>
		/// <param name="index"> Location. </param>
		/// <returns> the point at location {@code index}. </returns>
		public virtual PointValuePair getPoint(int index)
		{
			if (index < 0 || index >= simplex.Length)
			{
				throw new OutOfRangeException(index, 0, simplex.Length - 1);
			}
			return simplex[index];
		}

		/// <summary>
		/// Store a new point at location {@code index}.
		/// Note that no deep-copy of {@code point} is performed.
		/// </summary>
		/// <param name="index"> Location. </param>
		/// <param name="point"> New value. </param>
		protected internal virtual void setPoint(int index, PointValuePair point)
		{
			if (index < 0 || index >= simplex.Length)
			{
				throw new OutOfRangeException(index, 0, simplex.Length - 1);
			}
			simplex[index] = point;
		}


		/// <summary>
		/// Create steps for a unit hypercube.
		/// </summary>
		/// <param name="n"> Dimension of the hypercube. </param>
		/// <param name="sideLength"> Length of the sides of the hypercube. </param>
		/// <returns> the steps. </returns>
		private static double[] createHypercubeSteps(int n, double sideLength)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] steps = new double[n];
			double[] steps = new double[n];
			for (int i = 0; i < n; i++)
			{
				steps[i] = sideLength;
			}
			return steps;
		}
	}

}