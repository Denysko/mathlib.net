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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NonMonotonicSequenceException = org.apache.commons.math3.exception.NonMonotonicSequenceException;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using Precision = org.apache.commons.math3.util.Precision;
	using GaussNewtonOptimizer = org.apache.commons.math3.optim.nonlinear.vector.jacobian.GaussNewtonOptimizer;
	using PolynomialFitter = org.apache.commons.math3.fitting.PolynomialFitter;
	using PolynomialFunction = org.apache.commons.math3.analysis.polynomials.PolynomialFunction;
	using SimpleVectorValueChecker = org.apache.commons.math3.optim.SimpleVectorValueChecker;

	/// <summary>
	/// Generates a bicubic interpolation function.
	/// Prior to generating the interpolating function, the input is smoothed using
	/// polynomial fitting.
	/// 
	/// @version $Id: SmoothingPolynomialBicubicSplineInterpolator.java 1455194 2013-03-11 15:45:54Z luc $
	/// @since 2.2
	/// </summary>
	public class SmoothingPolynomialBicubicSplineInterpolator : BicubicSplineInterpolator
	{
		/// <summary>
		/// Fitter for x. </summary>
		private readonly PolynomialFitter xFitter;
		/// <summary>
		/// Degree of the fitting polynomial. </summary>
		private readonly int xDegree;
		/// <summary>
		/// Fitter for y. </summary>
		private readonly PolynomialFitter yFitter;
		/// <summary>
		/// Degree of the fitting polynomial. </summary>
		private readonly int yDegree;

		/// <summary>
		/// Default constructor. The degree of the fitting polynomials is set to 3.
		/// </summary>
		public SmoothingPolynomialBicubicSplineInterpolator() : this(3)
		{
		}

		/// <param name="degree"> Degree of the polynomial fitting functions. </param>
		/// <exception cref="NotPositiveException"> if degree is not positive </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SmoothingPolynomialBicubicSplineInterpolator(int degree) throws org.apache.commons.math3.exception.NotPositiveException
		public SmoothingPolynomialBicubicSplineInterpolator(int degree) : this(degree, degree)
		{
		}

		/// <param name="xDegree"> Degree of the polynomial fitting functions along the
		/// x-dimension. </param>
		/// <param name="yDegree"> Degree of the polynomial fitting functions along the
		/// y-dimension. </param>
		/// <exception cref="NotPositiveException"> if degrees are not positive </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SmoothingPolynomialBicubicSplineInterpolator(int xDegree, int yDegree) throws org.apache.commons.math3.exception.NotPositiveException
		public SmoothingPolynomialBicubicSplineInterpolator(int xDegree, int yDegree)
		{
			if (xDegree < 0)
			{
				throw new NotPositiveException(xDegree);
			}
			if (yDegree < 0)
			{
				throw new NotPositiveException(yDegree);
			}
			this.xDegree = xDegree;
			this.yDegree = yDegree;

			const double safeFactor = 1e2;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.optim.SimpleVectorValueChecker checker = new org.apache.commons.math3.optim.SimpleVectorValueChecker(safeFactor * org.apache.commons.math3.util.Precision.EPSILON, safeFactor * org.apache.commons.math3.util.Precision.SAFE_MIN);
			SimpleVectorValueChecker checker = new SimpleVectorValueChecker(safeFactor * Precision.EPSILON, safeFactor * Precision.SAFE_MIN);
			xFitter = new PolynomialFitter(new GaussNewtonOptimizer(false, checker));
			yFitter = new PolynomialFitter(new GaussNewtonOptimizer(false, checker));
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BicubicSplineInterpolatingFunction interpolate(final double[] xval, final double[] yval, final double[][] fval) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NonMonotonicSequenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BicubicSplineInterpolatingFunction interpolate(double[] xval, double[] yval, double[][] fval)
		{
			if (xval.Length == 0 || yval.Length == 0 || fval.Length == 0)
			{
				throw new NoDataException();
			}
			if (xval.Length != fval.Length)
			{
				throw new DimensionMismatchException(xval.Length, fval.Length);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xLen = xval.length;
			int xLen = xval.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int yLen = yval.length;
			int yLen = yval.Length;

			for (int i = 0; i < xLen; i++)
			{
				if (fval[i].Length != yLen)
				{
					throw new DimensionMismatchException(fval[i].Length, yLen);
				}
			}

			MathArrays.checkOrder(xval);
			MathArrays.checkOrder(yval);

			// For each line y[j] (0 <= j < yLen), construct a polynomial, with
			// respect to variable x, fitting array fval[][j]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.polynomials.PolynomialFunction[] yPolyX = new org.apache.commons.math3.analysis.polynomials.PolynomialFunction[yLen];
			PolynomialFunction[] yPolyX = new PolynomialFunction[yLen];
			for (int j = 0; j < yLen; j++)
			{
				xFitter.clearObservations();
				for (int i = 0; i < xLen; i++)
				{
					xFitter.addObservedPoint(1, xval[i], fval[i][j]);
				}

				// Initial guess for the fit is zero for each coefficients (of which
				// there are "xDegree" + 1).
				yPolyX[j] = new PolynomialFunction(xFitter.fit(new double[xDegree + 1]));
			}

			// For every knot (xval[i], yval[j]) of the grid, calculate corrected
			// values fval_1
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] fval_1 = new double[xLen][yLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] fval_1 = new double[xLen][yLen];
			double[][] fval_1 = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen);
			for (int j = 0; j < yLen; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.polynomials.PolynomialFunction f = yPolyX[j];
				PolynomialFunction f = yPolyX[j];
				for (int i = 0; i < xLen; i++)
				{
					fval_1[i][j] = f.value(xval[i]);
				}
			}

			// For each line x[i] (0 <= i < xLen), construct a polynomial, with
			// respect to variable y, fitting array fval_1[i][]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.polynomials.PolynomialFunction[] xPolyY = new org.apache.commons.math3.analysis.polynomials.PolynomialFunction[xLen];
			PolynomialFunction[] xPolyY = new PolynomialFunction[xLen];
			for (int i = 0; i < xLen; i++)
			{
				yFitter.clearObservations();
				for (int j = 0; j < yLen; j++)
				{
					yFitter.addObservedPoint(1, yval[j], fval_1[i][j]);
				}

				// Initial guess for the fit is zero for each coefficients (of which
				// there are "yDegree" + 1).
				xPolyY[i] = new PolynomialFunction(yFitter.fit(new double[yDegree + 1]));
			}

			// For every knot (xval[i], yval[j]) of the grid, calculate corrected
			// values fval_2
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] fval_2 = new double[xLen][yLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] fval_2 = new double[xLen][yLen];
			double[][] fval_2 = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen);
			for (int i = 0; i < xLen; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.analysis.polynomials.PolynomialFunction f = xPolyY[i];
				PolynomialFunction f = xPolyY[i];
				for (int j = 0; j < yLen; j++)
				{
					fval_2[i][j] = f.value(yval[j]);
				}
			}

			return base.interpolate(xval, yval, fval_2);
		}
	}

}