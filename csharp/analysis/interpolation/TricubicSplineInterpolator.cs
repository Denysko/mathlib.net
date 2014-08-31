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
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Generates a tricubic interpolating function.
	/// 
	/// @since 2.2
	/// @version $Id: TricubicSplineInterpolator.java 1455194 2013-03-11 15:45:54Z luc $
	/// </summary>
	public class TricubicSplineInterpolator : TrivariateGridInterpolator
	{
		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TricubicSplineInterpolatingFunction interpolate(final double[] xval, final double[] yval, final double[] zval, final double[][][] fval) throws org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NumberIsTooSmallException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NonMonotonicSequenceException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual TricubicSplineInterpolatingFunction interpolate(double[] xval, double[] yval, double[] zval, double[][][] fval)
		{
			if (xval.Length == 0 || yval.Length == 0 || zval.Length == 0 || fval.Length == 0)
			{
				throw new NoDataException();
			}
			if (xval.Length != fval.Length)
			{
				throw new DimensionMismatchException(xval.Length, fval.Length);
			}

			MathArrays.checkOrder(xval);
			MathArrays.checkOrder(yval);
			MathArrays.checkOrder(zval);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int xLen = xval.length;
			int xLen = xval.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int yLen = yval.length;
			int yLen = yval.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int zLen = zval.length;
			int zLen = zval.Length;

			// Samples, re-ordered as (z, x, y) and (y, z, x) tuplets
			// fvalXY[k][i][j] = f(xval[i], yval[j], zval[k])
			// fvalZX[j][k][i] = f(xval[i], yval[j], zval[k])
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] fvalXY = new double[zLen][xLen][yLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] fvalXY = new double[zLen][xLen][yLen];
			double[][][] fvalXY = RectangularArrays.ReturnRectangularDoubleArray(zLen, xLen, yLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] fvalZX = new double[yLen][zLen][xLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] fvalZX = new double[yLen][zLen][xLen];
			double[][][] fvalZX = RectangularArrays.ReturnRectangularDoubleArray(yLen, zLen, xLen);
			for (int i = 0; i < xLen; i++)
			{
				if (fval[i].Length != yLen)
				{
					throw new DimensionMismatchException(fval[i].Length, yLen);
				}

				for (int j = 0; j < yLen; j++)
				{
					if (fval[i][j].Length != zLen)
					{
						throw new DimensionMismatchException(fval[i][j].Length, zLen);
					}

					for (int k = 0; k < zLen; k++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v = fval[i][j][k];
						double v = fval[i][j][k];
						fvalXY[k][i][j] = v;
						fvalZX[j][k][i] = v;
					}
				}
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolator bsi = new BicubicSplineInterpolator();
			BicubicSplineInterpolator bsi = new BicubicSplineInterpolator();

			// For each line x[i] (0 <= i < xLen), construct a 2D spline in y and z
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolatingFunction[] xSplineYZ = new BicubicSplineInterpolatingFunction[xLen];
			BicubicSplineInterpolatingFunction[] xSplineYZ = new BicubicSplineInterpolatingFunction[xLen];
			for (int i = 0; i < xLen; i++)
			{
				xSplineYZ[i] = bsi.interpolate(yval, zval, fval[i]);
			}

			// For each line y[j] (0 <= j < yLen), construct a 2D spline in z and x
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolatingFunction[] ySplineZX = new BicubicSplineInterpolatingFunction[yLen];
			BicubicSplineInterpolatingFunction[] ySplineZX = new BicubicSplineInterpolatingFunction[yLen];
			for (int j = 0; j < yLen; j++)
			{
				ySplineZX[j] = bsi.interpolate(zval, xval, fvalZX[j]);
			}

			// For each line z[k] (0 <= k < zLen), construct a 2D spline in x and y
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolatingFunction[] zSplineXY = new BicubicSplineInterpolatingFunction[zLen];
			BicubicSplineInterpolatingFunction[] zSplineXY = new BicubicSplineInterpolatingFunction[zLen];
			for (int k = 0; k < zLen; k++)
			{
				zSplineXY[k] = bsi.interpolate(xval, yval, fvalXY[k]);
			}

			// Partial derivatives wrt x and wrt y
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] dFdX = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] dFdX = new double[xLen][yLen][zLen];
			double[][][] dFdX = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] dFdY = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] dFdY = new double[xLen][yLen][zLen];
			double[][][] dFdY = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] d2FdXdY = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] d2FdXdY = new double[xLen][yLen][zLen];
			double[][][] d2FdXdY = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
			for (int k = 0; k < zLen; k++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolatingFunction f = zSplineXY[k];
				BicubicSplineInterpolatingFunction f = zSplineXY[k];
				for (int i = 0; i < xLen; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = xval[i];
					double x = xval[i];
					for (int j = 0; j < yLen; j++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = yval[j];
						double y = yval[j];
						dFdX[i][j][k] = f.partialDerivativeX(x, y);
						dFdY[i][j][k] = f.partialDerivativeY(x, y);
						d2FdXdY[i][j][k] = f.partialDerivativeXY(x, y);
					}
				}
			}

			// Partial derivatives wrt y and wrt z
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] dFdZ = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] dFdZ = new double[xLen][yLen][zLen];
			double[][][] dFdZ = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] d2FdYdZ = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] d2FdYdZ = new double[xLen][yLen][zLen];
			double[][][] d2FdYdZ = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
			for (int i = 0; i < xLen; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolatingFunction f = xSplineYZ[i];
				BicubicSplineInterpolatingFunction f = xSplineYZ[i];
				for (int j = 0; j < yLen; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double y = yval[j];
					double y = yval[j];
					for (int k = 0; k < zLen; k++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = zval[k];
						double z = zval[k];
						dFdZ[i][j][k] = f.partialDerivativeY(y, z);
						d2FdYdZ[i][j][k] = f.partialDerivativeXY(y, z);
					}
				}
			}

			// Partial derivatives wrt x and wrt z
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] d2FdZdX = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] d2FdZdX = new double[xLen][yLen][zLen];
			double[][][] d2FdZdX = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
			for (int j = 0; j < yLen; j++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BicubicSplineInterpolatingFunction f = ySplineZX[j];
				BicubicSplineInterpolatingFunction f = ySplineZX[j];
				for (int k = 0; k < zLen; k++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double z = zval[k];
					double z = zval[k];
					for (int i = 0; i < xLen; i++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = xval[i];
						double x = xval[i];
						d2FdZdX[i][j][k] = f.partialDerivativeXY(z, x);
					}
				}
			}

			// Third partial cross-derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] d3FdXdYdZ = new double[xLen][yLen][zLen];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] d3FdXdYdZ = new double[xLen][yLen][zLen];
			double[][][] d3FdXdYdZ = RectangularArrays.ReturnRectangularDoubleArray(xLen, yLen, zLen);
			for (int i = 0; i < xLen ; i++)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nI = nextIndex(i, xLen);
				int nI = nextIndex(i, xLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pI = previousIndex(i);
				int pI = previousIndex(i);
				for (int j = 0; j < yLen; j++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nJ = nextIndex(j, yLen);
					int nJ = nextIndex(j, yLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pJ = previousIndex(j);
					int pJ = previousIndex(j);
					for (int k = 0; k < zLen; k++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nK = nextIndex(k, zLen);
						int nK = nextIndex(k, zLen);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pK = previousIndex(k);
						int pK = previousIndex(k);

						// XXX Not sure about this formula
						d3FdXdYdZ[i][j][k] = (fval[nI][nJ][nK] - fval[nI][pJ][nK] - fval[pI][nJ][nK] + fval[pI][pJ][nK] - fval[nI][nJ][pK] + fval[nI][pJ][pK] + fval[pI][nJ][pK] - fval[pI][pJ][pK]) / ((xval[nI] - xval[pI]) * (yval[nJ] - yval[pJ]) * (zval[nK] - zval[pK]));
					}
				}
			}

			// Create the interpolating splines
			return new TricubicSplineInterpolatingFunction(xval, yval, zval, fval, dFdX, dFdY, dFdZ, d2FdXdY, d2FdZdX, d2FdYdZ, d3FdXdYdZ);
		}

		/// <summary>
		/// Compute the next index of an array, clipping if necessary.
		/// It is assumed (but not checked) that {@code i} is larger than or equal to 0}.
		/// </summary>
		/// <param name="i"> Index </param>
		/// <param name="max"> Upper limit of the array </param>
		/// <returns> the next index </returns>
		private int nextIndex(int i, int max)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = i + 1;
			int index = i + 1;
			return index < max ? index : index - 1;
		}
		/// <summary>
		/// Compute the previous index of an array, clipping if necessary.
		/// It is assumed (but not checked) that {@code i} is smaller than the size of the array.
		/// </summary>
		/// <param name="i"> Index </param>
		/// <returns> the previous index </returns>
		private int previousIndex(int i)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = i - 1;
			int index = i - 1;
			return index >= 0 ? index : 0;
		}
	}

}