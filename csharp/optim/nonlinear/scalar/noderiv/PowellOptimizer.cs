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

	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using MathUnsupportedOperationException = org.apache.commons.math3.exception.MathUnsupportedOperationException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using org.apache.commons.math3.optim;
	using UnivariatePointValuePair = org.apache.commons.math3.optim.univariate.UnivariatePointValuePair;

	/// <summary>
	/// Powell's algorithm.
	/// This code is translated and adapted from the Python version of this
	/// algorithm (as implemented in module {@code optimize.py} v0.5 of
	/// <em>SciPy</em>).
	/// <br/>
	/// The default stopping criterion is based on the differences of the
	/// function value between two successive iterations. It is however possible
	/// to define a custom convergence checker that might terminate the algorithm
	/// earlier.
	/// <br/>
	/// Line search is performed by the <seealso cref="LineSearch"/> class.
	/// <br/>
	/// Constraints are not supported: the call to
	/// <seealso cref="#optimize(OptimizationData[]) optimize"/> will throw
	/// <seealso cref="MathUnsupportedOperationException"/> if bounds are passed to it.
	/// In order to impose simple constraints, the objective function must be
	/// wrapped in an adapter like
	/// {@link org.apache.commons.math3.optim.nonlinear.scalar.MultivariateFunctionMappingAdapter
	/// MultivariateFunctionMappingAdapter} or
	/// {@link org.apache.commons.math3.optim.nonlinear.scalar.MultivariateFunctionPenaltyAdapter
	/// MultivariateFunctionPenaltyAdapter}.
	/// 
	/// @version $Id: PowellOptimizer.java 1579346 2014-03-19 18:43:39Z erans $
	/// @since 2.2
	/// </summary>
	public class PowellOptimizer : MultivariateOptimizer
	{
		/// <summary>
		/// Minimum relative tolerance.
		/// </summary>
		private static readonly double MIN_RELATIVE_TOLERANCE = 2 * FastMath.ulp(1d);
		/// <summary>
		/// Relative threshold.
		/// </summary>
		private readonly double relativeThreshold;
		/// <summary>
		/// Absolute threshold.
		/// </summary>
		private readonly double absoluteThreshold;
		/// <summary>
		/// Line search.
		/// </summary>
		private readonly LineSearch line;

		/// <summary>
		/// This constructor allows to specify a user-defined convergence checker,
		/// in addition to the parameters that control the default convergence
		/// checking procedure.
		/// <br/>
		/// The internal line search tolerances are set to the square-root of their
		/// corresponding value in the multivariate optimizer.
		/// </summary>
		/// <param name="rel"> Relative threshold. </param>
		/// <param name="abs"> Absolute threshold. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code abs <= 0}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code rel < 2 * Math.ulp(1d)}. </exception>
		public PowellOptimizer(double rel, double abs, ConvergenceChecker<PointValuePair> checker) : this(rel, abs, FastMath.sqrt(rel), FastMath.sqrt(abs), checker)
		{
		}

		/// <summary>
		/// This constructor allows to specify a user-defined convergence checker,
		/// in addition to the parameters that control the default convergence
		/// checking procedure and the line search tolerances.
		/// </summary>
		/// <param name="rel"> Relative threshold for this optimizer. </param>
		/// <param name="abs"> Absolute threshold for this optimizer. </param>
		/// <param name="lineRel"> Relative threshold for the internal line search optimizer. </param>
		/// <param name="lineAbs"> Absolute threshold for the internal line search optimizer. </param>
		/// <param name="checker"> Convergence checker. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code abs <= 0}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code rel < 2 * Math.ulp(1d)}. </exception>
		public PowellOptimizer(double rel, double abs, double lineRel, double lineAbs, ConvergenceChecker<PointValuePair> checker) : base(checker)
		{

			if (rel < MIN_RELATIVE_TOLERANCE)
			{
				throw new NumberIsTooSmallException(rel, MIN_RELATIVE_TOLERANCE, true);
			}
			if (abs <= 0)
			{
				throw new NotStrictlyPositiveException(abs);
			}
			relativeThreshold = rel;
			absoluteThreshold = abs;

			// Create the line search optimizer.
			line = new LineSearch(this, lineRel, lineAbs, 1d);
		}

		/// <summary>
		/// The parameters control the default convergence checking procedure.
		/// <br/>
		/// The internal line search tolerances are set to the square-root of their
		/// corresponding value in the multivariate optimizer.
		/// </summary>
		/// <param name="rel"> Relative threshold. </param>
		/// <param name="abs"> Absolute threshold. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code abs <= 0}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code rel < 2 * Math.ulp(1d)}. </exception>
		public PowellOptimizer(double rel, double abs) : this(rel, abs, null)
		{
		}

		/// <summary>
		/// Builds an instance with the default convergence checking procedure.
		/// </summary>
		/// <param name="rel"> Relative threshold. </param>
		/// <param name="abs"> Absolute threshold. </param>
		/// <param name="lineRel"> Relative threshold for the internal line search optimizer. </param>
		/// <param name="lineAbs"> Absolute threshold for the internal line search optimizer. </param>
		/// <exception cref="NotStrictlyPositiveException"> if {@code abs <= 0}. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code rel < 2 * Math.ulp(1d)}. </exception>
		public PowellOptimizer(double rel, double abs, double lineRel, double lineAbs) : this(rel, abs, lineRel, lineAbs, null)
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		protected internal override PointValuePair doOptimize()
		{
			checkParameters();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.nonlinear.scalar.GoalType goal = getGoalType();
			GoalType goal = GoalType;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] guess = getStartPoint();
			double[] guess = StartPoint;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = guess.length;
			int n = guess.Length;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] direc = new double[n][n];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] direc = new double[n][n];
			double[][] direc = RectangularArrays.ReturnRectangularDoubleArray(n, n);
			for (int i = 0; i < n; i++)
			{
				direc[i][i] = 1;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.ConvergenceChecker<org.apache.commons.math3.optim.PointValuePair> checker = getConvergenceChecker();
			ConvergenceChecker<PointValuePair> checker = ConvergenceChecker;

			double[] x = guess;
			double fVal = computeObjectiveValue(x);
			double[] x1 = x.clone();
			while (true)
			{
				incrementIterationCount();

				double fX = fVal;
				double fX2 = 0;
				double delta = 0;
				int bigInd = 0;
				double alphaMin = 0;

				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] d = org.apache.commons.math3.util.MathArrays.copyOf(direc[i]);
					double[] d = MathArrays.copyOf(direc[i]);

					fX2 = fVal;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.univariate.UnivariatePointValuePair optimum = line.search(x, d);
					UnivariatePointValuePair optimum = line.search(x, d);
					fVal = optimum.Value;
					alphaMin = optimum.Point;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] result = newPointAndDirection(x, d, alphaMin);
					double[][] result = newPointAndDirection(x, d, alphaMin);
					x = result[0];

					if ((fX2 - fVal) > delta)
					{
						delta = fX2 - fVal;
						bigInd = i;
					}
				}

				// Default convergence check.
				bool stop = 2 * (fX - fVal) <= (relativeThreshold * (FastMath.abs(fX) + FastMath.abs(fVal)) + absoluteThreshold);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair previous = new org.apache.commons.math3.optim.PointValuePair(x1, fX);
				PointValuePair previous = new PointValuePair(x1, fX);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.PointValuePair current = new org.apache.commons.math3.optim.PointValuePair(x, fVal);
				PointValuePair current = new PointValuePair(x, fVal);
				if (!stop && checker != null) // User-defined stopping criteria.
				{
					stop = checker.converged(Iterations, previous, current);
				}
				if (stop)
				{
					if (goal == GoalType.MINIMIZE)
					{
						return (fVal < fX) ? current : previous;
					}
					else
					{
						return (fVal > fX) ? current : previous;
					}
				}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] d = new double[n];
				double[] d = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] x2 = new double[n];
				double[] x2 = new double[n];
				for (int i = 0; i < n; i++)
				{
					d[i] = x[i] - x1[i];
					x2[i] = 2 * x[i] - x1[i];
				}

				x1 = x.clone();
				fX2 = computeObjectiveValue(x2);

				if (fX > fX2)
				{
					double t = 2 * (fX + fX2 - 2 * fVal);
					double temp = fX - fVal - delta;
					t *= temp * temp;
					temp = fX - fX2;
					t -= delta * temp * temp;

					if (t < 0.0)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.univariate.UnivariatePointValuePair optimum = line.search(x, d);
						UnivariatePointValuePair optimum = line.search(x, d);
						fVal = optimum.Value;
						alphaMin = optimum.Point;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] result = newPointAndDirection(x, d, alphaMin);
						double[][] result = newPointAndDirection(x, d, alphaMin);
						x = result[0];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastInd = n - 1;
						int lastInd = n - 1;
						direc[bigInd] = direc[lastInd];
						direc[lastInd] = result[1];
					}
				}
			}
		}

		/// <summary>
		/// Compute a new point (in the original space) and a new direction
		/// vector, resulting from the line search.
		/// </summary>
		/// <param name="p"> Point used in the line search. </param>
		/// <param name="d"> Direction used in the line search. </param>
		/// <param name="optimum"> Optimum found by the line search. </param>
		/// <returns> a 2-element array containing the new point (at index 0) and
		/// the new direction (at index 1). </returns>
		private double[][] newPointAndDirection(double[] p, double[] d, double optimum)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = p.length;
			int n = p.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] nP = new double[n];
			double[] nP = new double[n];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] nD = new double[n];
			double[] nD = new double[n];
			for (int i = 0; i < n; i++)
			{
				nD[i] = d[i] * optimum;
				nP[i] = p[i] + nD[i];
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] result = new double[2][];
			double[][] result = new double[2][];
			result[0] = nP;
			result[1] = nD;

			return result;
		}

		/// <exception cref="MathUnsupportedOperationException"> if bounds were passed to the
		/// <seealso cref="#optimize(OptimizationData[]) optimize"/> method. </exception>
		private void checkParameters()
		{
			if (LowerBound != null || UpperBound != null)
			{
				throw new MathUnsupportedOperationException(LocalizedFormats.CONSTRAINT);
			}
		}
	}

}