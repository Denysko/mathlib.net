using System;
using System.Text;

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
namespace mathlib.stat.descriptive
{


	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using RealMatrix = mathlib.linear.RealMatrix;
	using GeometricMean = mathlib.stat.descriptive.moment.GeometricMean;
	using Mean = mathlib.stat.descriptive.moment.Mean;
	using VectorialCovariance = mathlib.stat.descriptive.moment.VectorialCovariance;
	using Max = mathlib.stat.descriptive.rank.Max;
	using Min = mathlib.stat.descriptive.rank.Min;
	using Sum = mathlib.stat.descriptive.summary.Sum;
	using SumOfLogs = mathlib.stat.descriptive.summary.SumOfLogs;
	using SumOfSquares = mathlib.stat.descriptive.summary.SumOfSquares;
	using MathUtils = mathlib.util.MathUtils;
	using MathArrays = mathlib.util.MathArrays;
	using Precision = mathlib.util.Precision;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// <p>Computes summary statistics for a stream of n-tuples added using the
	/// <seealso cref="#addValue(double[]) addValue"/> method. The data values are not stored
	/// in memory, so this class can be used to compute statistics for very large
	/// n-tuple streams.</p>
	/// 
	/// <p>The <seealso cref="StorelessUnivariateStatistic"/> instances used to maintain
	/// summary state and compute statistics are configurable via setters.
	/// For example, the default implementation for the mean can be overridden by
	/// calling <seealso cref="#setMeanImpl(StorelessUnivariateStatistic[])"/>. Actual
	/// parameters to these methods must implement the
	/// <seealso cref="StorelessUnivariateStatistic"/> interface and configuration must be
	/// completed before <code>addValue</code> is called. No configuration is
	/// necessary to use the default, commons-math provided implementations.</p>
	/// 
	/// <p>To compute statistics for a stream of n-tuples, construct a
	/// MultivariateStatistics instance with dimension n and then use
	/// <seealso cref="#addValue(double[])"/> to add n-tuples. The <code>getXxx</code>
	/// methods where Xxx is a statistic return an array of <code>double</code>
	/// values, where for <code>i = 0,...,n-1</code> the i<sup>th</sup> array element is the
	/// value of the given statistic for data range consisting of the i<sup>th</sup> element of
	/// each of the input n-tuples.  For example, if <code>addValue</code> is called
	/// with actual parameters {0, 1, 2}, then {3, 4, 5} and finally {6, 7, 8},
	/// <code>getSum</code> will return a three-element array with values
	/// {0+3+6, 1+4+7, 2+5+8}</p>
	/// 
	/// <p>Note: This class is not thread-safe. Use
	/// <seealso cref="SynchronizedMultivariateSummaryStatistics"/> if concurrent access from multiple
	/// threads is required.</p>
	/// 
	/// @since 1.2
	/// @version $Id: MultivariateSummaryStatistics.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	[Serializable]
	public class MultivariateSummaryStatistics : StatisticalMultivariateSummary
	{

		/// <summary>
		/// Serialization UID </summary>
		private const long serialVersionUID = 2271900808994826718L;

		/// <summary>
		/// Dimension of the data. </summary>
		private int k;

		/// <summary>
		/// Count of values that have been added </summary>
		private long n = 0;

		/// <summary>
		/// Sum statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] sumImpl;

		/// <summary>
		/// Sum of squares statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] sumSqImpl;

		/// <summary>
		/// Minimum statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] minImpl;

		/// <summary>
		/// Maximum statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] maxImpl;

		/// <summary>
		/// Sum of log statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] sumLogImpl;

		/// <summary>
		/// Geometric mean statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] geoMeanImpl;

		/// <summary>
		/// Mean statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic[] meanImpl;

		/// <summary>
		/// Covariance statistic implementation - cannot be reset. </summary>
		private VectorialCovariance covarianceImpl;

		/// <summary>
		/// Construct a MultivariateSummaryStatistics instance </summary>
		/// <param name="k"> dimension of the data </param>
		/// <param name="isCovarianceBiasCorrected"> if true, the unbiased sample
		/// covariance is computed, otherwise the biased population covariance
		/// is computed </param>
		public MultivariateSummaryStatistics(int k, bool isCovarianceBiasCorrected)
		{
			this.k = k;

			sumImpl = new StorelessUnivariateStatistic[k];
			sumSqImpl = new StorelessUnivariateStatistic[k];
			minImpl = new StorelessUnivariateStatistic[k];
			maxImpl = new StorelessUnivariateStatistic[k];
			sumLogImpl = new StorelessUnivariateStatistic[k];
			geoMeanImpl = new StorelessUnivariateStatistic[k];
			meanImpl = new StorelessUnivariateStatistic[k];

			for (int i = 0; i < k; ++i)
			{
				sumImpl[i] = new Sum();
				sumSqImpl[i] = new SumOfSquares();
				minImpl[i] = new Min();
				maxImpl[i] = new Max();
				sumLogImpl[i] = new SumOfLogs();
				geoMeanImpl[i] = new GeometricMean();
				meanImpl[i] = new Mean();
			}

			covarianceImpl = new VectorialCovariance(k, isCovarianceBiasCorrected);

		}

		/// <summary>
		/// Add an n-tuple to the data
		/// </summary>
		/// <param name="value">  the n-tuple to add </param>
		/// <exception cref="DimensionMismatchException"> if the length of the array
		/// does not match the one used at construction </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addValue(double[] value) throws mathlib.exception.DimensionMismatchException
		public virtual void addValue(double[] value)
		{
			checkDimension(value.Length);
			for (int i = 0; i < k; ++i)
			{
				double v = value[i];
				sumImpl[i].increment(v);
				sumSqImpl[i].increment(v);
				minImpl[i].increment(v);
				maxImpl[i].increment(v);
				sumLogImpl[i].increment(v);
				geoMeanImpl[i].increment(v);
				meanImpl[i].increment(v);
			}
			covarianceImpl.increment(value);
			n++;
		}

		/// <summary>
		/// Returns the dimension of the data </summary>
		/// <returns> The dimension of the data </returns>
		public virtual int Dimension
		{
			get
			{
				return k;
			}
		}

		/// <summary>
		/// Returns the number of available values </summary>
		/// <returns> The number of available values </returns>
		public virtual long N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// Returns an array of the results of a statistic. </summary>
		/// <param name="stats"> univariate statistic array </param>
		/// <returns> results array </returns>
		private double[] getResults(StorelessUnivariateStatistic[] stats)
		{
			double[] results = new double[stats.Length];
			for (int i = 0; i < results.Length; ++i)
			{
				results[i] = stats[i].Result;
			}
			return results;
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the sum of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component sums </returns>
		public virtual double[] Sum
		{
			get
			{
				return getResults(sumImpl);
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the sum of squares of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component sums of squares </returns>
		public virtual double[] SumSq
		{
			get
			{
				return getResults(sumSqImpl);
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the sum of logs of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component log sums </returns>
		public virtual double[] SumLog
		{
			get
			{
				return getResults(sumLogImpl);
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the mean of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component means </returns>
		public virtual double[] Mean
		{
			get
			{
				return getResults(meanImpl);
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the standard deviation of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component standard deviations </returns>
		public virtual double[] StandardDeviation
		{
			get
			{
				double[] stdDev = new double[k];
				if (N < 1)
				{
					Arrays.fill(stdDev, double.NaN);
				}
				else if (N < 2)
				{
					Arrays.fill(stdDev, 0.0);
				}
				else
				{
					RealMatrix matrix = covarianceImpl.Result;
					for (int i = 0; i < k; ++i)
					{
						stdDev[i] = FastMath.sqrt(matrix.getEntry(i, i));
					}
				}
				return stdDev;
			}
		}

		/// <summary>
		/// Returns the covariance matrix of the values that have been added.
		/// </summary>
		/// <returns> the covariance matrix </returns>
		public virtual RealMatrix Covariance
		{
			get
			{
				return covarianceImpl.Result;
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the maximum of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component maxima </returns>
		public virtual double[] Max
		{
			get
			{
				return getResults(maxImpl);
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the minimum of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component minima </returns>
		public virtual double[] Min
		{
			get
			{
				return getResults(minImpl);
			}
		}

		/// <summary>
		/// Returns an array whose i<sup>th</sup> entry is the geometric mean of the
		/// i<sup>th</sup> entries of the arrays that have been added using
		/// <seealso cref="#addValue(double[])"/>
		/// </summary>
		/// <returns> the array of component geometric means </returns>
		public virtual double[] GeometricMean
		{
			get
			{
				return getResults(geoMeanImpl);
			}
		}

		/// <summary>
		/// Generates a text report displaying
		/// summary statistics from values that
		/// have been added. </summary>
		/// <returns> String with line feeds displaying statistics </returns>
		public override string ToString()
		{
			const string separator = ", ";
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String suffix = System.getProperty("line.separator");
			string suffix = System.getProperty("line.separator");
			StringBuilder outBuffer = new StringBuilder();
			outBuffer.Append("MultivariateSummaryStatistics:" + suffix);
			outBuffer.Append("n: " + N + suffix);
			append(outBuffer, Min, "min: ", separator, suffix);
			append(outBuffer, Max, "max: ", separator, suffix);
			append(outBuffer, Mean, "mean: ", separator, suffix);
			append(outBuffer, GeometricMean, "geometric mean: ", separator, suffix);
			append(outBuffer, SumSq, "sum of squares: ", separator, suffix);
			append(outBuffer, SumLog, "sum of logarithms: ", separator, suffix);
			append(outBuffer, StandardDeviation, "standard deviation: ", separator, suffix);
			outBuffer.Append("covariance: " + Covariance.ToString() + suffix);
			return outBuffer.ToString();
		}

		/// <summary>
		/// Append a text representation of an array to a buffer. </summary>
		/// <param name="buffer"> buffer to fill </param>
		/// <param name="data"> data array </param>
		/// <param name="prefix"> text prefix </param>
		/// <param name="separator"> elements separator </param>
		/// <param name="suffix"> text suffix </param>
		private void append(StringBuilder buffer, double[] data, string prefix, string separator, string suffix)
		{
			buffer.Append(prefix);
			for (int i = 0; i < data.Length; ++i)
			{
				if (i > 0)
				{
					buffer.Append(separator);
				}
				buffer.Append(data[i]);
			}
			buffer.Append(suffix);
		}

		/// <summary>
		/// Resets all statistics and storage
		/// </summary>
		public virtual void clear()
		{
			this.n = 0;
			for (int i = 0; i < k; ++i)
			{
				minImpl[i].clear();
				maxImpl[i].clear();
				sumImpl[i].clear();
				sumLogImpl[i].clear();
				sumSqImpl[i].clear();
				geoMeanImpl[i].clear();
				meanImpl[i].clear();
			}
			covarianceImpl.clear();
		}

		/// <summary>
		/// Returns true iff <code>object</code> is a <code>MultivariateSummaryStatistics</code>
		/// instance and all statistics have the same values as this. </summary>
		/// <param name="object"> the object to test equality against. </param>
		/// <returns> true if object equals this </returns>
		public override bool Equals(object @object)
		{
			if (@object == this)
			{
				return true;
			}
			if (@object is MultivariateSummaryStatistics == false)
			{
				return false;
			}
			MultivariateSummaryStatistics stat = (MultivariateSummaryStatistics) @object;
			return MathArrays.equalsIncludingNaN(stat.GeometricMean, GeometricMean) && MathArrays.equalsIncludingNaN(stat.Max, Max) && MathArrays.equalsIncludingNaN(stat.Mean, Mean) && MathArrays.equalsIncludingNaN(stat.Min, Min) && Precision.equalsIncludingNaN(stat.N, N) && MathArrays.equalsIncludingNaN(stat.Sum, Sum) && MathArrays.equalsIncludingNaN(stat.SumSq, SumSq) && MathArrays.equalsIncludingNaN(stat.SumLog, SumLog) && stat.Covariance.Equals(Covariance);
		}

		/// <summary>
		/// Returns hash code based on values of statistics
		/// </summary>
		/// <returns> hash code </returns>
		public override int GetHashCode()
		{
			int result = 31 + MathUtils.hash(GeometricMean);
			result = result * 31 + MathUtils.hash(GeometricMean);
			result = result * 31 + MathUtils.hash(Max);
			result = result * 31 + MathUtils.hash(Mean);
			result = result * 31 + MathUtils.hash(Min);
			result = result * 31 + MathUtils.hash(N);
			result = result * 31 + MathUtils.hash(Sum);
			result = result * 31 + MathUtils.hash(SumSq);
			result = result * 31 + MathUtils.hash(SumLog);
			result = result * 31 + Covariance.GetHashCode();
			return result;
		}

		// Getters and setters for statistics implementations
		/// <summary>
		/// Sets statistics implementations. </summary>
		/// <param name="newImpl"> new implementations for statistics </param>
		/// <param name="oldImpl"> old implementations for statistics </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		/// (i.e. if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void setImpl(StorelessUnivariateStatistic[] newImpl, StorelessUnivariateStatistic[] oldImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException
		private void setImpl(StorelessUnivariateStatistic[] newImpl, StorelessUnivariateStatistic[] oldImpl)
		{
			checkEmpty();
			checkDimension(newImpl.Length);
			Array.Copy(newImpl, 0, oldImpl, 0, newImpl.Length);
		}

		/// <summary>
		/// Returns the currently configured Sum implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the sum </returns>
		public virtual StorelessUnivariateStatistic[] SumImpl
		{
			get
			{
				return sumImpl.clone();
			}
			set
			{
				setImpl(value, this.sumImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the Sum.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="sumImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the Sum </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumImpl(StorelessUnivariateStatistic[] sumImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Returns the currently configured sum of squares implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the sum of squares </returns>
		public virtual StorelessUnivariateStatistic[] SumsqImpl
		{
			get
			{
				return sumSqImpl.clone();
			}
			set
			{
				setImpl(value, this.sumSqImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the sum of squares.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="sumsqImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the sum of squares </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumsqImpl(StorelessUnivariateStatistic[] sumsqImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Returns the currently configured minimum implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the minimum </returns>
		public virtual StorelessUnivariateStatistic[] MinImpl
		{
			get
			{
				return minImpl.clone();
			}
			set
			{
				setImpl(value, this.minImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the minimum.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="minImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the minimum </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setMinImpl(StorelessUnivariateStatistic[] minImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Returns the currently configured maximum implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the maximum </returns>
		public virtual StorelessUnivariateStatistic[] MaxImpl
		{
			get
			{
				return maxImpl.clone();
			}
			set
			{
				setImpl(value, this.maxImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the maximum.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="maxImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the maximum </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setMaxImpl(StorelessUnivariateStatistic[] maxImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Returns the currently configured sum of logs implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the log sum </returns>
		public virtual StorelessUnivariateStatistic[] SumLogImpl
		{
			get
			{
				return sumLogImpl.clone();
			}
			set
			{
				setImpl(value, this.sumLogImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the sum of logs.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="sumLogImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the log sum </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumLogImpl(StorelessUnivariateStatistic[] sumLogImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Returns the currently configured geometric mean implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the geometric mean </returns>
		public virtual StorelessUnivariateStatistic[] GeoMeanImpl
		{
			get
			{
				return geoMeanImpl.clone();
			}
			set
			{
				setImpl(value, this.geoMeanImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the geometric mean.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="geoMeanImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the geometric mean </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setGeoMeanImpl(StorelessUnivariateStatistic[] geoMeanImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Returns the currently configured mean implementation
		/// </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the mean </returns>
		public virtual StorelessUnivariateStatistic[] MeanImpl
		{
			get
			{
				return meanImpl.clone();
			}
			set
			{
				setImpl(value, this.meanImpl);
			}
		}

		/// <summary>
		/// <p>Sets the implementation for the mean.</p>
		/// <p>This method must be activated before any data has been added - i.e.,
		/// before <seealso cref="#addValue(double[]) addValue"/> has been used to add data;
		/// otherwise an IllegalStateException will be thrown.</p>
		/// </summary>
		/// <param name="meanImpl"> the StorelessUnivariateStatistic instance to use
		/// for computing the mean </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension
		/// does not match the one used at construction </exception>
		/// <exception cref="MathIllegalStateException"> if data has already been added
		///  (i.e if n > 0) </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setMeanImpl(StorelessUnivariateStatistic[] meanImpl) throws mathlib.exception.MathIllegalStateException, mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Throws MathIllegalStateException if the statistic is not empty. </summary>
		/// <exception cref="MathIllegalStateException"> if n > 0. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkEmpty() throws mathlib.exception.MathIllegalStateException
		private void checkEmpty()
		{
			if (n > 0)
			{
				throw new MathIllegalStateException(LocalizedFormats.VALUES_ADDED_BEFORE_CONFIGURING_STATISTIC, n);
			}
		}

		/// <summary>
		/// Throws DimensionMismatchException if dimension != k. </summary>
		/// <param name="dimension"> dimension to check </param>
		/// <exception cref="DimensionMismatchException"> if dimension != k </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkDimension(int dimension) throws mathlib.exception.DimensionMismatchException
		private void checkDimension(int dimension)
		{
			if (dimension != k)
			{
				throw new DimensionMismatchException(dimension, k);
			}
		}
	}

}