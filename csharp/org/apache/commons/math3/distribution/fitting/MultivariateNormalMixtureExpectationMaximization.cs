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
namespace org.apache.commons.math3.distribution.fitting
{


	using ConvergenceException = org.apache.commons.math3.exception.ConvergenceException;
	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using SingularMatrixException = org.apache.commons.math3.linear.SingularMatrixException;
	using Covariance = org.apache.commons.math3.stat.correlation.Covariance;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;
	using org.apache.commons.math3.util;

	/// <summary>
	/// Expectation-Maximization</a> algorithm for fitting the parameters of
	/// multivariate normal mixture model distributions.
	/// 
	/// This implementation is pure original code based on <a
	/// href="https://www.ee.washington.edu/techsite/papers/documents/UWEETR-2010-0002.pdf">
	/// EM Demystified: An Expectation-Maximization Tutorial</a> by Yihua Chen and Maya R. Gupta,
	/// Department of Electrical Engineering, University of Washington, Seattle, WA 98195.
	/// It was verified using external tools like <a
	/// href="http://cran.r-project.org/web/packages/mixtools/index.html">CRAN Mixtools</a>
	/// (see the JUnit test cases) but it is <strong>not</strong> based on Mixtools code at all.
	/// The discussion of the origin of this class can be seen in the comments of the <a
	/// href="https://issues.apache.org/jira/browse/MATH-817">MATH-817</a> JIRA issue.
	/// @version $Id: MultivariateNormalMixtureExpectationMaximization.java 1547633 2013-12-03 23:03:06Z tn $
	/// @since 3.2
	/// </summary>
	public class MultivariateNormalMixtureExpectationMaximization
	{
		/// <summary>
		/// Default maximum number of iterations allowed per fitting process.
		/// </summary>
		private const int DEFAULT_MAX_ITERATIONS = 1000;
		/// <summary>
		/// Default convergence threshold for fitting.
		/// </summary>
		private const double DEFAULT_THRESHOLD = 1E-5;
		/// <summary>
		/// The data to fit.
		/// </summary>
		private readonly double[][] data;
		/// <summary>
		/// The model fit against the data.
		/// </summary>
		private MixtureMultivariateNormalDistribution fittedModel;
		/// <summary>
		/// The log likelihood of the data given the fitted model.
		/// </summary>
		private double logLikelihood = 0d;

		/// <summary>
		/// Creates an object to fit a multivariate normal mixture model to data.
		/// </summary>
		/// <param name="data"> Data to use in fitting procedure </param>
		/// <exception cref="NotStrictlyPositiveException"> if data has no rows </exception>
		/// <exception cref="DimensionMismatchException"> if rows of data have different numbers
		///             of columns </exception>
		/// <exception cref="NumberIsTooSmallException"> if the number of columns in the data is
		///             less than 2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MultivariateNormalMixtureExpectationMaximization(double[][] data) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooSmallException
		public MultivariateNormalMixtureExpectationMaximization(double[][] data)
		{
			if (data.Length < 1)
			{
				throw new NotStrictlyPositiveException(data.Length);
			}

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: this.data = new double[data.Length][data[0].Length];
			this.data = RectangularArrays.ReturnRectangularDoubleArray(data.Length, data[0].Length);

			for (int i = 0; i < data.Length; i++)
			{
				if (data[i].Length != data[0].Length)
				{
					// Jagged arrays not allowed
					throw new DimensionMismatchException(data[i].Length, data[0].Length);
				}
				if (data[i].Length < 2)
				{
					throw new NumberIsTooSmallException(LocalizedFormats.NUMBER_TOO_SMALL, data[i].Length, 2, true);
				}
				this.data[i] = MathArrays.copyOf(data[i], data[i].Length);
			}
		}

		/// <summary>
		/// Fit a mixture model to the data supplied to the constructor.
		/// 
		/// The quality of the fit depends on the concavity of the data provided to
		/// the constructor and the initial mixture provided to this function. If the
		/// data has many local optima, multiple runs of the fitting function with
		/// different initial mixtures may be required to find the optimal solution.
		/// If a SingularMatrixException is encountered, it is possible that another
		/// initialization would work.
		/// </summary>
		/// <param name="initialMixture"> Model containing initial values of weights and
		///            multivariate normals </param>
		/// <param name="maxIterations"> Maximum iterations allowed for fit </param>
		/// <param name="threshold"> Convergence threshold computed as difference in
		///             logLikelihoods between successive iterations </param>
		/// <exception cref="SingularMatrixException"> if any component's covariance matrix is
		///             singular during fitting </exception>
		/// <exception cref="NotStrictlyPositiveException"> if numComponents is less than one
		///             or threshold is less than Double.MIN_VALUE </exception>
		/// <exception cref="DimensionMismatchException"> if initialMixture mean vector and data
		///             number of columns are not equal </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fit(final org.apache.commons.math3.distribution.MixtureMultivariateNormalDistribution initialMixture, final int maxIterations, final double threshold) throws org.apache.commons.math3.linear.SingularMatrixException, org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void fit(MixtureMultivariateNormalDistribution initialMixture, int maxIterations, double threshold)
		{
			if (maxIterations < 1)
			{
				throw new NotStrictlyPositiveException(maxIterations);
			}

			if (threshold < double.MinValue)
			{
				throw new NotStrictlyPositiveException(threshold);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = data.length;
			int n = data.Length;

			// Number of data columns. Jagged data already rejected in constructor,
			// so we can assume the lengths of each row are equal.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numCols = data[0].length;
			int numCols = data[0].Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = initialMixture.getComponents().size();
			int k = initialMixture.Components.Count;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numMeanColumns = initialMixture.getComponents().get(0).getSecond().getMeans().length;
			int numMeanColumns = initialMixture.Components[0].Second.Means.length;

			if (numMeanColumns != numCols)
			{
				throw new DimensionMismatchException(numMeanColumns, numCols);
			}

			int numIterations = 0;
			double previousLogLikelihood = 0d;

			logLikelihood = double.NegativeInfinity;

			// Initialize model to fit to initial mixture.
			fittedModel = new MixtureMultivariateNormalDistribution(initialMixture.Components);

			while (numIterations++ <= maxIterations && FastMath.abs(previousLogLikelihood - logLikelihood) > threshold)
			{
				previousLogLikelihood = logLikelihood;
				double sumLogLikelihood = 0d;

				// Mixture components
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.util.Pair<Double, org.apache.commons.math3.distribution.MultivariateNormalDistribution>> components = fittedModel.getComponents();
				IList<Pair<double?, MultivariateNormalDistribution>> components = fittedModel.Components;

				// Weight and distribution of each component
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] weights = new double[k];
				double[] weights = new double[k];

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.distribution.MultivariateNormalDistribution[] mvns = new org.apache.commons.math3.distribution.MultivariateNormalDistribution[k];
				MultivariateNormalDistribution[] mvns = new MultivariateNormalDistribution[k];

				for (int j = 0; j < k; j++)
				{
					weights[j] = components[j].First;
					mvns[j] = components[j].Second;
				}

				// E-step: compute the data dependent parameters of the expectation
				// function.
				// The percentage of row's total density between a row and a
				// component
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] gamma = new double[n][k];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] gamma = new double[n][k];
				double[][] gamma = RectangularArrays.ReturnRectangularDoubleArray(n, k);

				// Sum of gamma for each component
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] gammaSums = new double[k];
				double[] gammaSums = new double[k];

				// Sum of gamma times its row for each each component
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] gammaDataProdSums = new double[k][numCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] gammaDataProdSums = new double[k][numCols];
				double[][] gammaDataProdSums = RectangularArrays.ReturnRectangularDoubleArray(k, numCols);

				for (int i = 0; i < n; i++)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rowDensity = fittedModel.density(data[i]);
					double rowDensity = fittedModel.density(data[i]);
					sumLogLikelihood += FastMath.log(rowDensity);

					for (int j = 0; j < k; j++)
					{
						gamma[i][j] = weights[j] * mvns[j].density(data[i]) / rowDensity;
						gammaSums[j] += gamma[i][j];

						for (int col = 0; col < numCols; col++)
						{
							gammaDataProdSums[j][col] += gamma[i][j] * data[i][col];
						}
					}
				}

				logLikelihood = sumLogLikelihood / n;

				// M-step: compute the new parameters based on the expectation
				// function.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] newWeights = new double[k];
				double[] newWeights = new double[k];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] newMeans = new double[k][numCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] newMeans = new double[k][numCols];
				double[][] newMeans = RectangularArrays.ReturnRectangularDoubleArray(k, numCols);

				for (int j = 0; j < k; j++)
				{
					newWeights[j] = gammaSums[j] / n;
					for (int col = 0; col < numCols; col++)
					{
						newMeans[j][col] = gammaDataProdSums[j][col] / gammaSums[j];
					}
				}

				// Compute new covariance matrices
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix[] newCovMats = new org.apache.commons.math3.linear.RealMatrix[k];
				RealMatrix[] newCovMats = new RealMatrix[k];
				for (int j = 0; j < k; j++)
				{
					newCovMats[j] = new Array2DRowRealMatrix(numCols, numCols);
				}
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < k; j++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix vec = new org.apache.commons.math3.linear.Array2DRowRealMatrix(org.apache.commons.math3.util.MathArrays.ebeSubtract(data[i], newMeans[j]));
						RealMatrix vec = new Array2DRowRealMatrix(MathArrays.ebeSubtract(data[i], newMeans[j]));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.linear.RealMatrix dataCov = vec.multiply(vec.transpose()).scalarMultiply(gamma[i][j]);
						RealMatrix dataCov = vec.multiply(vec.transpose()).scalarMultiply(gamma[i][j]);
						newCovMats[j] = newCovMats[j].add(dataCov);
					}
				}

				// Converting to arrays for use by fitted model
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][][] newCovMatArrays = new double[k][numCols][numCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][][] newCovMatArrays = new double[k][numCols][numCols];
				double[][][] newCovMatArrays = RectangularArrays.ReturnRectangularDoubleArray(k, numCols, numCols);
				for (int j = 0; j < k; j++)
				{
					newCovMats[j] = newCovMats[j].scalarMultiply(1d / gammaSums[j]);
					newCovMatArrays[j] = newCovMats[j].Data;
				}

				// Update current model
				fittedModel = new MixtureMultivariateNormalDistribution(newWeights, newMeans, newCovMatArrays);
			}

			if (FastMath.abs(previousLogLikelihood - logLikelihood) > threshold)
			{
				// Did not converge before the maximum number of iterations
				throw new ConvergenceException();
			}
		}

		/// <summary>
		/// Fit a mixture model to the data supplied to the constructor.
		/// 
		/// The quality of the fit depends on the concavity of the data provided to
		/// the constructor and the initial mixture provided to this function. If the
		/// data has many local optima, multiple runs of the fitting function with
		/// different initial mixtures may be required to find the optimal solution.
		/// If a SingularMatrixException is encountered, it is possible that another
		/// initialization would work.
		/// </summary>
		/// <param name="initialMixture"> Model containing initial values of weights and
		///            multivariate normals </param>
		/// <exception cref="SingularMatrixException"> if any component's covariance matrix is
		///             singular during fitting </exception>
		/// <exception cref="NotStrictlyPositiveException"> if numComponents is less than one or
		///             threshold is less than Double.MIN_VALUE </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fit(org.apache.commons.math3.distribution.MixtureMultivariateNormalDistribution initialMixture) throws org.apache.commons.math3.linear.SingularMatrixException, org.apache.commons.math3.exception.NotStrictlyPositiveException
		public virtual void fit(MixtureMultivariateNormalDistribution initialMixture)
		{
			fit(initialMixture, DEFAULT_MAX_ITERATIONS, DEFAULT_THRESHOLD);
		}

		/// <summary>
		/// Helper method to create a multivariate normal mixture model which can be
		/// used to initialize <seealso cref="#fit(MixtureMultivariateNormalDistribution)"/>.
		/// 
		/// This method uses the data supplied to the constructor to try to determine
		/// a good mixture model at which to start the fit, but it is not guaranteed
		/// to supply a model which will find the optimal solution or even converge.
		/// </summary>
		/// <param name="data"> Data to estimate distribution </param>
		/// <param name="numComponents"> Number of components for estimated mixture </param>
		/// <returns> Multivariate normal mixture model estimated from the data </returns>
		/// <exception cref="NumberIsTooLargeException"> if {@code numComponents} is greater
		/// than the number of data rows. </exception>
		/// <exception cref="NumberIsTooSmallException"> if {@code numComponents < 2}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if data has less than 2 rows </exception>
		/// <exception cref="DimensionMismatchException"> if rows of data have different numbers
		///             of columns </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static org.apache.commons.math3.distribution.MixtureMultivariateNormalDistribution estimate(final double[][] data, final int numComponents) throws org.apache.commons.math3.exception.NotStrictlyPositiveException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static MixtureMultivariateNormalDistribution estimate(double[][] data, int numComponents)
		{
			if (data.Length < 2)
			{
				throw new NotStrictlyPositiveException(data.Length);
			}
			if (numComponents < 2)
			{
				throw new NumberIsTooSmallException(numComponents, 2, true);
			}
			if (numComponents > data.Length)
			{
				throw new NumberIsTooLargeException(numComponents, data.Length, true);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numRows = data.length;
			int numRows = data.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numCols = data[0].length;
			int numCols = data[0].Length;

			// sort the data
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DataRow[] sortedData = new DataRow[numRows];
			DataRow[] sortedData = new DataRow[numRows];
			for (int i = 0; i < numRows; i++)
			{
				sortedData[i] = new DataRow(data[i]);
			}
			Arrays.sort(sortedData);

			// uniform weight for each bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double weight = 1d / numComponents;
			double weight = 1d / numComponents;

			// components of mixture model to be created
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.apache.commons.math3.util.Pair<Double, org.apache.commons.math3.distribution.MultivariateNormalDistribution>> components = new java.util.ArrayList<org.apache.commons.math3.util.Pair<Double, org.apache.commons.math3.distribution.MultivariateNormalDistribution>>(numComponents);
			IList<Pair<double?, MultivariateNormalDistribution>> components = new List<Pair<double?, MultivariateNormalDistribution>>(numComponents);

			// create a component based on data in each bin
			for (int binIndex = 0; binIndex < numComponents; binIndex++)
			{
				// minimum index (inclusive) from sorted data for this bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int minIndex = (binIndex * numRows) / numComponents;
				int minIndex = (binIndex * numRows) / numComponents;

				// maximum index (exclusive) from sorted data for this bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxIndex = ((binIndex + 1) * numRows) / numComponents;
				int maxIndex = ((binIndex + 1) * numRows) / numComponents;

				// number of data records that will be in this bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int numBinRows = maxIndex - minIndex;
				int numBinRows = maxIndex - minIndex;

				// data for this bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] binData = new double[numBinRows][numCols];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] binData = new double[numBinRows][numCols];
				double[][] binData = RectangularArrays.ReturnRectangularDoubleArray(numBinRows, numCols);

				// mean of each column for the data in the this bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] columnMeans = new double[numCols];
				double[] columnMeans = new double[numCols];

				// populate bin and create component
				for (int i = minIndex, iBin = 0; i < maxIndex; i++, iBin++)
				{
					for (int j = 0; j < numCols; j++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double val = sortedData[i].getRow()[j];
						double val = sortedData[i].Row[j];
						columnMeans[j] += val;
						binData[iBin][j] = val;
					}
				}

				MathArrays.scaleInPlace(1d / numBinRows, columnMeans);

				// covariance matrix for this bin
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] covMat = new org.apache.commons.math3.stat.correlation.Covariance(binData).getCovarianceMatrix().getData();
				double[][] covMat = (new Covariance(binData)).CovarianceMatrix.Data;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.commons.math3.distribution.MultivariateNormalDistribution mvn = new org.apache.commons.math3.distribution.MultivariateNormalDistribution(columnMeans, covMat);
				MultivariateNormalDistribution mvn = new MultivariateNormalDistribution(columnMeans, covMat);

				components.Add(new Pair<double?, MultivariateNormalDistribution>(weight, mvn));
			}

			return new MixtureMultivariateNormalDistribution(components);
		}

		/// <summary>
		/// Gets the log likelihood of the data under the fitted model.
		/// </summary>
		/// <returns> Log likelihood of data or zero of no data has been fit </returns>
		public virtual double LogLikelihood
		{
			get
			{
				return logLikelihood;
			}
		}

		/// <summary>
		/// Gets the fitted model.
		/// </summary>
		/// <returns> fitted model or {@code null} if no fit has been performed yet. </returns>
		public virtual MixtureMultivariateNormalDistribution FittedModel
		{
			get
			{
				return new MixtureMultivariateNormalDistribution(fittedModel.Components);
			}
		}

		/// <summary>
		/// Class used for sorting user-supplied data.
		/// </summary>
		private class DataRow : IComparable<DataRow>
		{
			/// <summary>
			/// One data row. </summary>
			internal readonly double[] row;
			/// <summary>
			/// Mean of the data row. </summary>
			internal double? mean;

			/// <summary>
			/// Create a data row. </summary>
			/// <param name="data"> Data to use for the row </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: DataRow(final double[] data)
			internal DataRow(double[] data)
			{
				// Store reference.
				row = data;
				// Compute mean.
				mean = 0d;
				for (int i = 0; i < data.Length; i++)
				{
					mean += data[i];
				}
				mean /= data.Length;
			}

			/// <summary>
			/// Compare two data rows. </summary>
			/// <param name="other"> The other row </param>
			/// <returns> int for sorting </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compareTo(final DataRow other)
			public virtual int CompareTo(DataRow other)
			{
				return mean.compareTo(other.mean);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override bool Equals(object other)
			{

				if (this == other)
				{
					return true;
				}

				if (other is DataRow)
				{
					return MathArrays.Equals(row, ((DataRow) other).row);
				}

				return false;

			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public override int GetHashCode()
			{
				return Arrays.GetHashCode(row);
			}
			/// <summary>
			/// Get a data row. </summary>
			/// <returns> data row array </returns>
			public virtual double[] Row
			{
				get
				{
					return row;
				}
			}
		}
	}


}