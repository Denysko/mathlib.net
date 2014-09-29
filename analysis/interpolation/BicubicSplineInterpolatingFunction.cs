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
namespace mathlib.analysis.interpolation
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NoDataException = mathlib.exception.NoDataException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using NonMonotonicSequenceException = mathlib.exception.NonMonotonicSequenceException;
	using MathArrays = mathlib.util.MathArrays;

	/// <summary>
	/// Function that implements the
	/// <a href="http://en.wikipedia.org/wiki/Bicubic_interpolation">
	/// bicubic spline interpolation</a>.
	/// 
	/// @since 2.1
	/// @version $Id: BicubicSplineInterpolatingFunction.java 1512547 2013-08-10 01:13:38Z erans $
	/// </summary>
	public class BicubicSplineInterpolatingFunction : BivariateFunction
	{
		/// <summary>
		/// Number of coefficients. </summary>
		private const int NUM_COEFF = 16;
		/// <summary>
		/// Matrix to compute the spline coefficients from the function values
		/// and function derivatives values
		/// </summary>
		private static readonly double[][] AINV = new double[][] {new double[] {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0}, new double[] {-3,3,0,0,-2,-1,0,0,0,0,0,0,0,0,0,0}, new double[] {2,-2,0,0,1,1,0,0,0,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0}, new double[] {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0}, new double[] {0,0,0,0,0,0,0,0,-3,3,0,0,-2,-1,0,0}, new double[] {0,0,0,0,0,0,0,0,2,-2,0,0,1,1,0,0}, new double[] {-3,0,3,0,0,0,0,0,-2,0,-1,0,0,0,0,0}, new double[] {0,0,0,0,-3,0,3,0,0,0,0,0,-2,0,-1,0}, new double[] {9,-9,-9,9,6,3,-6,-3,6,-6,3,-3,4,2,2,1}, new double[] {-6,6,6,-6,-3,-3,3,3,-4,4,-2,2,-2,-2,-1,-1}, new double[] {2,0,-2,0,0,0,0,0,1,0,1,0,0,0,0,0}, new double[] {0,0,0,0,2,0,-2,0,0,0,0,0,1,0,1,0}, new double[] {-6,6,6,-6,-4,-2,4,2,-3,3,-3,3,-2,-1,-2,-1}, new double[] {4,-4,-4,4,2,2,-2,-2,2,-2,2,-2,1,1,1,1}};

		/// <summary>
		/// Samples x-coordinates </summary>
		private readonly double[] xval;
		/// <summary>
		/// Samples y-coordinates </summary>
		private readonly double[] yval;
		/// <summary>
		/// Set of cubic splines patching the whole data grid </summary>
		private readonly BicubicSplineFunction[][] splines;
		/// <summary>
		/// Partial derivatives
		/// The value of the first index determines the kind of derivatives:
		/// 0 = first partial derivatives wrt x
		/// 1 = first partial derivatives wrt y
		/// 2 = second partial derivatives wrt x
		/// 3 = second partial derivatives wrt y
		/// 4 = cross partial derivatives
		/// </summary>
		private BivariateFunction[][][] partialDerivatives = null;

		/// <param name="x"> Sample values of the x-coordinate, in increasing order. </param>
		/// <param name="y"> Sample values of the y-coordinate, in increasing order. </param>
		/// <param name="f"> Values of the function on every grid point. </param>
		/// <param name="dFdX"> Values of the partial derivative of function with respect
		/// to x on every grid point. </param>
		/// <param name="dFdY"> Values of the partial derivative of function with respect
		/// to y on every grid point. </param>
		/// <param name="d2FdXdY"> Values of the cross partial derivative of function on
		/// every grid point. </param>
		/// <exception cref="DimensionMismatchException"> if the various arrays do not contain
		/// the expected number of elements. </exception>
		/// <exception cref="NonMonotonicSequenceException"> if {@code x} or {@code y} are
		/// not strictly increasing. </exception>
		/// <exception cref="NoDataException"> if any of the arrays has zero length. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BicubicSplineInterpolatingFunction(double[] x, double[] y, double[][] f, double[][] dFdX, double[][] dFdY, double[][] d2FdXdY) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NoDataException, mathlib.exception.NonMonotonicSequenceException
		public BicubicSplineInterpolatingFunction(double[] x, double[] y, double[][] f, double[][] dFdX, double[][] dFdY, double[][] d2FdXdY)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xLen = x.length;
			int xLen = x.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int yLen = y.length;
			int yLen = y.Length;

			if (xLen == 0 || yLen == 0 || f.Length == 0 || f[0].Length == 0)
			{
				throw new NoDataException();
			}
			if (xLen != f.Length)
			{
				throw new DimensionMismatchException(xLen, f.Length);
			}
			if (xLen != dFdX.Length)
			{
				throw new DimensionMismatchException(xLen, dFdX.Length);
			}
			if (xLen != dFdY.Length)
			{
				throw new DimensionMismatchException(xLen, dFdY.Length);
			}
			if (xLen != d2FdXdY.Length)
			{
				throw new DimensionMismatchException(xLen, d2FdXdY.Length);
			}

			MathArrays.checkOrder(x);
			MathArrays.checkOrder(y);

			xval = x.clone();
			yval = y.clone();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastI = xLen - 1;
			int lastI = xLen - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastJ = yLen - 1;
			int lastJ = yLen - 1;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: splines = new BicubicSplineFunction[lastI][lastJ];
			splines = RectangularArrays.ReturnRectangularBicubicSplineFunctionArray(lastI, lastJ);

			for (int i = 0; i < lastI; i++)
			{
				if (f[i].Length != yLen)
				{
					throw new DimensionMismatchException(f[i].Length, yLen);
				}
				if (dFdX[i].Length != yLen)
				{
					throw new DimensionMismatchException(dFdX[i].Length, yLen);
				}
				if (dFdY[i].Length != yLen)
				{
					throw new DimensionMismatchException(dFdY[i].Length, yLen);
				}
				if (d2FdXdY[i].Length != yLen)
				{
					throw new DimensionMismatchException(d2FdXdY[i].Length, yLen);
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ip1 = i + 1;
				int ip1 = i + 1;
				for (int j = 0; j < lastJ; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jp1 = j + 1;
					int jp1 = j + 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] beta = new double[] { f[i][j], f[ip1][j], f[i][jp1], f[ip1][jp1], dFdX[i][j], dFdX[ip1][j], dFdX[i][jp1], dFdX[ip1][jp1], dFdY[i][j], dFdY[ip1][j], dFdY[i][jp1], dFdY[ip1][jp1], d2FdXdY[i][j], d2FdXdY[ip1][j], d2FdXdY[i][jp1], d2FdXdY[ip1][jp1] };
					double[] beta = new double[] {f[i][j], f[ip1][j], f[i][jp1], f[ip1][jp1], dFdX[i][j], dFdX[ip1][j], dFdX[i][jp1], dFdX[ip1][jp1], dFdY[i][j], dFdY[ip1][j], dFdY[i][jp1], dFdY[ip1][jp1], d2FdXdY[i][j], d2FdXdY[ip1][j], d2FdXdY[i][jp1], d2FdXdY[ip1][jp1]};

					splines[i][j] = new BicubicSplineFunction(computeSplineCoefficients(beta));
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double value(double x, double y) throws mathlib.exception.OutOfRangeException
		public virtual double value(double x, double y)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = searchIndex(x, xval);
			int i = searchIndex(x, xval);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int j = searchIndex(y, yval);
			int j = searchIndex(y, yval);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xN = (x - xval[i]) / (xval[i + 1] - xval[i]);
			double xN = (x - xval[i]) / (xval[i + 1] - xval[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yN = (y - yval[j]) / (yval[j + 1] - yval[j]);
			double yN = (y - yval[j]) / (yval[j + 1] - yval[j]);

			return splines[i][j].value(xN, yN);
		}

		/// <summary>
		/// Indicates whether a point is within the interpolation range.
		/// </summary>
		/// <param name="x"> First coordinate. </param>
		/// <param name="y"> Second coordinate. </param>
		/// <returns> {@code true} if (x, y) is a valid point.
		/// @since 3.3 </returns>
		public virtual bool isValidPoint(double x, double y)
		{
			if (x < xval[0] || x > xval[xval.Length - 1] || y < yval[0] || y > yval[yval.Length - 1])
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <param name="x"> x-coordinate. </param>
		/// <param name="y"> y-coordinate. </param>
		/// <returns> the value at point (x, y) of the first partial derivative with
		/// respect to x. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x} (resp. {@code y}) is outside
		/// the range defined by the boundary values of {@code xval} (resp.
		/// {@code yval}). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double partialDerivativeX(double x, double y) throws mathlib.exception.OutOfRangeException
		public virtual double partialDerivativeX(double x, double y)
		{
			return partialDerivative(0, x, y);
		}
		/// <param name="x"> x-coordinate. </param>
		/// <param name="y"> y-coordinate. </param>
		/// <returns> the value at point (x, y) of the first partial derivative with
		/// respect to y. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x} (resp. {@code y}) is outside
		/// the range defined by the boundary values of {@code xval} (resp.
		/// {@code yval}). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double partialDerivativeY(double x, double y) throws mathlib.exception.OutOfRangeException
		public virtual double partialDerivativeY(double x, double y)
		{
			return partialDerivative(1, x, y);
		}
		/// <param name="x"> x-coordinate. </param>
		/// <param name="y"> y-coordinate. </param>
		/// <returns> the value at point (x, y) of the second partial derivative with
		/// respect to x. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x} (resp. {@code y}) is outside
		/// the range defined by the boundary values of {@code xval} (resp.
		/// {@code yval}). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double partialDerivativeXX(double x, double y) throws mathlib.exception.OutOfRangeException
		public virtual double partialDerivativeXX(double x, double y)
		{
			return partialDerivative(2, x, y);
		}
		/// <param name="x"> x-coordinate. </param>
		/// <param name="y"> y-coordinate. </param>
		/// <returns> the value at point (x, y) of the second partial derivative with
		/// respect to y. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x} (resp. {@code y}) is outside
		/// the range defined by the boundary values of {@code xval} (resp.
		/// {@code yval}). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double partialDerivativeYY(double x, double y) throws mathlib.exception.OutOfRangeException
		public virtual double partialDerivativeYY(double x, double y)
		{
			return partialDerivative(3, x, y);
		}
		/// <param name="x"> x-coordinate. </param>
		/// <param name="y"> y-coordinate. </param>
		/// <returns> the value at point (x, y) of the second partial cross-derivative. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x} (resp. {@code y}) is outside
		/// the range defined by the boundary values of {@code xval} (resp.
		/// {@code yval}). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double partialDerivativeXY(double x, double y) throws mathlib.exception.OutOfRangeException
		public virtual double partialDerivativeXY(double x, double y)
		{
			return partialDerivative(4, x, y);
		}

		/// <param name="which"> First index in <seealso cref="#partialDerivatives"/>. </param>
		/// <param name="x"> x-coordinate. </param>
		/// <param name="y"> y-coordinate. </param>
		/// <returns> the value at point (x, y) of the selected partial derivative. </returns>
		/// <exception cref="OutOfRangeException"> if {@code x} (resp. {@code y}) is outside
		/// the range defined by the boundary values of {@code xval} (resp.
		/// {@code yval}). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double partialDerivative(int which, double x, double y) throws mathlib.exception.OutOfRangeException
		private double partialDerivative(int which, double x, double y)
		{
			if (partialDerivatives == null)
			{
				computePartialDerivatives();
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = searchIndex(x, xval);
			int i = searchIndex(x, xval);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int j = searchIndex(y, yval);
			int j = searchIndex(y, yval);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xN = (x - xval[i]) / (xval[i + 1] - xval[i]);
			double xN = (x - xval[i]) / (xval[i + 1] - xval[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yN = (y - yval[j]) / (yval[j + 1] - yval[j]);
			double yN = (y - yval[j]) / (yval[j + 1] - yval[j]);

			return partialDerivatives[which][i][j].value(xN, yN);
		}

		/// <summary>
		/// Compute all partial derivatives.
		/// </summary>
		private void computePartialDerivatives()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastI = xval.length - 1;
			int lastI = xval.Length - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lastJ = yval.length - 1;
			int lastJ = yval.Length - 1;
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: partialDerivatives = new BivariateFunction[5][lastI][lastJ];
			partialDerivatives = RectangularArrays.ReturnRectangularBivariateFunctionArray(5, lastI, lastJ);

			for (int i = 0; i < lastI; i++)
			{
				for (int j = 0; j < lastJ; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineFunction f = splines[i][j];
					BicubicSplineFunction f = splines[i][j];
					partialDerivatives[0][i][j] = f.partialDerivativeX();
					partialDerivatives[1][i][j] = f.partialDerivativeY();
					partialDerivatives[2][i][j] = f.partialDerivativeXX();
					partialDerivatives[3][i][j] = f.partialDerivativeYY();
					partialDerivatives[4][i][j] = f.partialDerivativeXY();
				}
			}
		}

		/// <param name="c"> Coordinate. </param>
		/// <param name="val"> Coordinate samples. </param>
		/// <returns> the index in {@code val} corresponding to the interval
		/// containing {@code c}. </returns>
		/// <exception cref="OutOfRangeException"> if {@code c} is out of the
		/// range defined by the boundary values of {@code val}. </exception>
		private int searchIndex(double c, double[] val)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int r = java.util.Arrays.binarySearch(val, c);
			int r = Arrays.binarySearch(val, c);

			if (r == -1 || r == -val.Length - 1)
			{
				throw new OutOfRangeException(c, val[0], val[val.Length - 1]);
			}

			if (r < 0)
			{
				// "c" in within an interpolation sub-interval: Return the
				// index of the sample at the lower end of the sub-interval.
				return -r - 2;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int last = val.length - 1;
			int last = val.Length - 1;
			if (r == last)
			{
				// "c" is the last sample of the range: Return the index
				// of the sample at the lower end of the last sub-interval.
				return last - 1;
			}

			// "c" is another sample point.
			return r;
		}

		/// <summary>
		/// Compute the spline coefficients from the list of function values and
		/// function partial derivatives values at the four corners of a grid
		/// element. They must be specified in the following order:
		/// <ul>
		///  <li>f(0,0)</li>
		///  <li>f(1,0)</li>
		///  <li>f(0,1)</li>
		///  <li>f(1,1)</li>
		///  <li>f<sub>x</sub>(0,0)</li>
		///  <li>f<sub>x</sub>(1,0)</li>
		///  <li>f<sub>x</sub>(0,1)</li>
		///  <li>f<sub>x</sub>(1,1)</li>
		///  <li>f<sub>y</sub>(0,0)</li>
		///  <li>f<sub>y</sub>(1,0)</li>
		///  <li>f<sub>y</sub>(0,1)</li>
		///  <li>f<sub>y</sub>(1,1)</li>
		///  <li>f<sub>xy</sub>(0,0)</li>
		///  <li>f<sub>xy</sub>(1,0)</li>
		///  <li>f<sub>xy</sub>(0,1)</li>
		///  <li>f<sub>xy</sub>(1,1)</li>
		/// </ul>
		/// where the subscripts indicate the partial derivative with respect to
		/// the corresponding variable(s).
		/// </summary>
		/// <param name="beta"> List of function values and function partial derivatives
		/// values. </param>
		/// <returns> the spline coefficients. </returns>
		private double[] computeSplineCoefficients(double[] beta)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] a = new double[NUM_COEFF];
			double[] a = new double[NUM_COEFF];

			for (int i = 0; i < NUM_COEFF; i++)
			{
				double result = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] row = AINV[i];
				double[] row = AINV[i];
				for (int j = 0; j < NUM_COEFF; j++)
				{
					result += row[j] * beta[j];
				}
				a[i] = result;
			}

			return a;
		}
	}

	/// <summary>
	/// 2D-spline function.
	/// 
	/// @version $Id: BicubicSplineInterpolatingFunction.java 1512547 2013-08-10 01:13:38Z erans $
	/// </summary>
	internal class BicubicSplineFunction : BivariateFunction
	{

		/// <summary>
		/// Number of points. </summary>
		private const short N = 4;

		/// <summary>
		/// Coefficients </summary>
		private readonly double[][] a;

		/// <summary>
		/// First partial derivative along x. </summary>
		private BivariateFunction partialDerivativeX_Renamed;

		/// <summary>
		/// First partial derivative along y. </summary>
		private BivariateFunction partialDerivativeY_Renamed;

		/// <summary>
		/// Second partial derivative along x. </summary>
		private BivariateFunction partialDerivativeXX_Renamed;

		/// <summary>
		/// Second partial derivative along y. </summary>
		private BivariateFunction partialDerivativeYY_Renamed;

		/// <summary>
		/// Second crossed partial derivative. </summary>
		private BivariateFunction partialDerivativeXY_Renamed;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="a"> Spline coefficients </param>
		public BicubicSplineFunction(double[] a)
		{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: this.a = new double[N][N];
			this.a = RectangularArrays.ReturnRectangularDoubleArray(N, N);
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					this.a[i][j] = a[i * N + j];
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual double value(double x, double y)
		{
			if (x < 0 || x > 1)
			{
				throw new OutOfRangeException(x, 0, 1);
			}
			if (y < 0 || y > 1)
			{
				throw new OutOfRangeException(y, 0, 1);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
			double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x3 = x2 * x;
			double x3 = x2 * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = {1, x, x2, x3};
			double[] pX = new double[] {1, x, x2, x3};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
			double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y3 = y2 * y;
			double y3 = y2 * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = {1, y, y2, y3};
			double[] pY = new double[] {1, y, y2, y3};

			return apply(pX, pY, a);
		}

		/// <summary>
		/// Compute the value of the bicubic polynomial.
		/// </summary>
		/// <param name="pX"> Powers of the x-coordinate. </param>
		/// <param name="pY"> Powers of the y-coordinate. </param>
		/// <param name="coeff"> Spline coefficients. </param>
		/// <returns> the interpolated value. </returns>
		private double apply(double[] pX, double[] pY, double[][] coeff)
		{
			double result = 0;
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					result += coeff[i][j] * pX[i] * pY[j];
				}
			}

			return result;
		}

		/// <returns> the partial derivative wrt {@code x}. </returns>
		public virtual BivariateFunction partialDerivativeX()
		{
			if (partialDerivativeX_Renamed == null)
			{
				computePartialDerivatives();
			}

			return partialDerivativeX_Renamed;
		}
		/// <returns> the partial derivative wrt {@code y}. </returns>
		public virtual BivariateFunction partialDerivativeY()
		{
			if (partialDerivativeY_Renamed == null)
			{
				computePartialDerivatives();
			}

			return partialDerivativeY_Renamed;
		}
		/// <returns> the second partial derivative wrt {@code x}. </returns>
		public virtual BivariateFunction partialDerivativeXX()
		{
			if (partialDerivativeXX_Renamed == null)
			{
				computePartialDerivatives();
			}

			return partialDerivativeXX_Renamed;
		}
		/// <returns> the second partial derivative wrt {@code y}. </returns>
		public virtual BivariateFunction partialDerivativeYY()
		{
			if (partialDerivativeYY_Renamed == null)
			{
				computePartialDerivatives();
			}

			return partialDerivativeYY_Renamed;
		}
		/// <returns> the second partial cross-derivative. </returns>
		public virtual BivariateFunction partialDerivativeXY()
		{
			if (partialDerivativeXY_Renamed == null)
			{
				computePartialDerivatives();
			}

			return partialDerivativeXY_Renamed;
		}

		/// <summary>
		/// Compute all partial derivatives functions.
		/// </summary>
		private void computePartialDerivatives()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] aX = new double[N][N];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] aX = new double[N][N];
			double[][] aX = RectangularArrays.ReturnRectangularDoubleArray(N, N);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] aY = new double[N][N];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] aY = new double[N][N];
			double[][] aY = RectangularArrays.ReturnRectangularDoubleArray(N, N);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] aXX = new double[N][N];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] aXX = new double[N][N];
			double[][] aXX = RectangularArrays.ReturnRectangularDoubleArray(N, N);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] aYY = new double[N][N];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] aYY = new double[N][N];
			double[][] aYY = RectangularArrays.ReturnRectangularDoubleArray(N, N);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] aXY = new double[N][N];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] aXY = new double[N][N];
			double[][] aXY = RectangularArrays.ReturnRectangularDoubleArray(N, N);

			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = a[i][j];
					double c = a[i][j];
					aX[i][j] = i * c;
					aY[i][j] = j * c;
					aXX[i][j] = (i - 1) * aX[i][j];
					aYY[i][j] = (j - 1) * aY[i][j];
					aXY[i][j] = j * aX[i][j];
				}
			}

			partialDerivativeX_Renamed = new BivariateFunctionAnonymousInnerClassHelper(this, aX);
			partialDerivativeY_Renamed = new BivariateFunctionAnonymousInnerClassHelper2(this, aY);
			partialDerivativeXX_Renamed = new BivariateFunctionAnonymousInnerClassHelper3(this, aXX);
			partialDerivativeYY_Renamed = new BivariateFunctionAnonymousInnerClassHelper4(this, aYY);
			partialDerivativeXY_Renamed = new BivariateFunctionAnonymousInnerClassHelper5(this, aXY);
		}

		private class BivariateFunctionAnonymousInnerClassHelper : BivariateFunction
		{
			private readonly BicubicSplineFunction outerInstance;

			private double[][] aX;

			public BivariateFunctionAnonymousInnerClassHelper(BicubicSplineFunction outerInstance, double[][] aX)
			{
				this.outerInstance = outerInstance;
				this.aX = aX;
			}

			public virtual double value(double x, double y)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = {0, 1, x, x2};
				double[] pX = new double[] {0, 1, x, x2};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
				double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y3 = y2 * y;
				double y3 = y2 * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = {1, y, y2, y3};
				double[] pY = new double[] {1, y, y2, y3};

				return outerInstance.apply(pX, pY, aX);
			}
		}

		private class BivariateFunctionAnonymousInnerClassHelper2 : BivariateFunction
		{
			private readonly BicubicSplineFunction outerInstance;

			private double[][] aY;

			public BivariateFunctionAnonymousInnerClassHelper2(BicubicSplineFunction outerInstance, double[][] aY)
			{
				this.outerInstance = outerInstance;
				this.aY = aY;
			}

			public virtual double value(double x, double y)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x3 = x2 * x;
				double x3 = x2 * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = {1, x, x2, x3};
				double[] pX = new double[] {1, x, x2, x3};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
				double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = {0, 1, y, y2};
				double[] pY = new double[] {0, 1, y, y2};

				return outerInstance.apply(pX, pY, aY);
			}
		}

		private class BivariateFunctionAnonymousInnerClassHelper3 : BivariateFunction
		{
			private readonly BicubicSplineFunction outerInstance;

			private double[][] aXX;

			public BivariateFunctionAnonymousInnerClassHelper3(BicubicSplineFunction outerInstance, double[][] aXX)
			{
				this.outerInstance = outerInstance;
				this.aXX = aXX;
			}

			public virtual double value(double x, double y)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = {0, 0, 1, x};
				double[] pX = new double[] {0, 0, 1, x};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
				double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y3 = y2 * y;
				double y3 = y2 * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = {1, y, y2, y3};
				double[] pY = new double[] {1, y, y2, y3};

				return outerInstance.apply(pX, pY, aXX);
			}
		}

		private class BivariateFunctionAnonymousInnerClassHelper4 : BivariateFunction
		{
			private readonly BicubicSplineFunction outerInstance;

			private double[][] aYY;

			public BivariateFunctionAnonymousInnerClassHelper4(BicubicSplineFunction outerInstance, double[][] aYY)
			{
				this.outerInstance = outerInstance;
				this.aYY = aYY;
			}

			public virtual double value(double x, double y)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x3 = x2 * x;
				double x3 = x2 * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = {1, x, x2, x3};
				double[] pX = new double[] {1, x, x2, x3};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = {0, 0, 1, y};
				double[] pY = new double[] {0, 0, 1, y};

				return outerInstance.apply(pX, pY, aYY);
			}
		}

		private class BivariateFunctionAnonymousInnerClassHelper5 : BivariateFunction
		{
			private readonly BicubicSplineFunction outerInstance;

			private double[][] aXY;

			public BivariateFunctionAnonymousInnerClassHelper5(BicubicSplineFunction outerInstance, double[][] aXY)
			{
				this.outerInstance = outerInstance;
				this.aXY = aXY;
			}

			public virtual double value(double x, double y)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pX = {0, 1, x, x2};
				double[] pX = new double[] {0, 1, x, x2};

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y2 = y * y;
				double y2 = y * y;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] pY = {0, 1, y, y2};
				double[] pY = new double[] {0, 1, y, y2};

				return outerInstance.apply(pX, pY, aXY);
			}
		}
	}

}