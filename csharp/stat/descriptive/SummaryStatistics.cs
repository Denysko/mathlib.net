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
namespace org.apache.commons.math3.stat.descriptive
{

	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using GeometricMean = org.apache.commons.math3.stat.descriptive.moment.GeometricMean;
	using Mean = org.apache.commons.math3.stat.descriptive.moment.Mean;
	using SecondMoment = org.apache.commons.math3.stat.descriptive.moment.SecondMoment;
	using Variance = org.apache.commons.math3.stat.descriptive.moment.Variance;
	using Max = org.apache.commons.math3.stat.descriptive.rank.Max;
	using Min = org.apache.commons.math3.stat.descriptive.rank.Min;
	using Sum = org.apache.commons.math3.stat.descriptive.summary.Sum;
	using SumOfLogs = org.apache.commons.math3.stat.descriptive.summary.SumOfLogs;
	using SumOfSquares = org.apache.commons.math3.stat.descriptive.summary.SumOfSquares;
	using MathUtils = org.apache.commons.math3.util.MathUtils;
	using Precision = org.apache.commons.math3.util.Precision;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// <p>
	/// Computes summary statistics for a stream of data values added using the
	/// <seealso cref="#addValue(double) addValue"/> method. The data values are not stored in
	/// memory, so this class can be used to compute statistics for very large data
	/// streams.
	/// </p>
	/// <p>
	/// The <seealso cref="StorelessUnivariateStatistic"/> instances used to maintain summary
	/// state and compute statistics are configurable via setters. For example, the
	/// default implementation for the variance can be overridden by calling
	/// <seealso cref="#setVarianceImpl(StorelessUnivariateStatistic)"/>. Actual parameters to
	/// these methods must implement the <seealso cref="StorelessUnivariateStatistic"/>
	/// interface and configuration must be completed before <code>addValue</code>
	/// is called. No configuration is necessary to use the default, commons-math
	/// provided implementations.
	/// </p>
	/// <p>
	/// Note: This class is not thread-safe. Use
	/// <seealso cref="SynchronizedSummaryStatistics"/> if concurrent access from multiple
	/// threads is required.
	/// </p>
	/// @version $Id: SummaryStatistics.java 1520076 2013-09-04 17:24:02Z psteitz $
	/// </summary>
	[Serializable]
	public class SummaryStatistics : StatisticalSummary
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			geoMean = new GeometricMean(sumLog);
			mean = new Mean(secondMoment);
			variance = new Variance(secondMoment);
			sumImpl = sum;
			sumsqImpl = sumsq;
			minImpl = min;
			maxImpl = max;
			sumLogImpl = sumLog;
			geoMeanImpl = geoMean;
			meanImpl = mean;
			varianceImpl = variance;
		}


		/// <summary>
		/// Serialization UID </summary>
		private const long serialVersionUID = -2021321786743555871L;

		/// <summary>
		/// count of values that have been added </summary>
		private long n = 0;

		/// <summary>
		/// SecondMoment is used to compute the mean and variance </summary>
		private SecondMoment secondMoment = new SecondMoment();

		/// <summary>
		/// sum of values that have been added </summary>
		private Sum sum = new Sum();

		/// <summary>
		/// sum of the square of each value that has been added </summary>
		private SumOfSquares sumsq = new SumOfSquares();

		/// <summary>
		/// min of values that have been added </summary>
		private Min min = new Min();

		/// <summary>
		/// max of values that have been added </summary>
		private Max max = new Max();

		/// <summary>
		/// sumLog of values that have been added </summary>
		private SumOfLogs sumLog = new SumOfLogs();

		/// <summary>
		/// geoMean of values that have been added </summary>
		private GeometricMean geoMean;

		/// <summary>
		/// mean of values that have been added </summary>
		private Mean mean;

		/// <summary>
		/// variance of values that have been added </summary>
		private Variance variance;

		/// <summary>
		/// Sum statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic sumImpl;

		/// <summary>
		/// Sum of squares statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic sumsqImpl;

		/// <summary>
		/// Minimum statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic minImpl;

		/// <summary>
		/// Maximum statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic maxImpl;

		/// <summary>
		/// Sum of log statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic sumLogImpl;

		/// <summary>
		/// Geometric mean statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic geoMeanImpl;

		/// <summary>
		/// Mean statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic meanImpl;

		/// <summary>
		/// Variance statistic implementation - can be reset by setter. </summary>
		private StorelessUnivariateStatistic varianceImpl;

		/// <summary>
		/// Construct a SummaryStatistics instance
		/// </summary>
		public SummaryStatistics()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		/// <summary>
		/// A copy constructor. Creates a deep-copy of the {@code original}.
		/// </summary>
		/// <param name="original"> the {@code SummaryStatistics} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SummaryStatistics(SummaryStatistics original) throws org.apache.commons.math3.exception.NullArgumentException
		public SummaryStatistics(SummaryStatistics original)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			copy(original, this);
		}

		/// <summary>
		/// Return a <seealso cref="StatisticalSummaryValues"/> instance reporting current
		/// statistics. </summary>
		/// <returns> Current values of statistics </returns>
		public virtual StatisticalSummary Summary
		{
			get
			{
				return new StatisticalSummaryValues(Mean, Variance, N, Max, Min, Sum);
			}
		}

		/// <summary>
		/// Add a value to the data </summary>
		/// <param name="value"> the value to add </param>
		public virtual void addValue(double value)
		{
			sumImpl.increment(value);
			sumsqImpl.increment(value);
			minImpl.increment(value);
			maxImpl.increment(value);
			sumLogImpl.increment(value);
			secondMoment.increment(value);
			// If mean, variance or geomean have been overridden,
			// need to increment these
			if (meanImpl != mean)
			{
				meanImpl.increment(value);
			}
			if (varianceImpl != variance)
			{
				varianceImpl.increment(value);
			}
			if (geoMeanImpl != geoMean)
			{
				geoMeanImpl.increment(value);
			}
			n++;
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
		/// Returns the sum of the values that have been added </summary>
		/// <returns> The sum or <code>Double.NaN</code> if no values have been added </returns>
		public virtual double Sum
		{
			get
			{
				return sumImpl.Result;
			}
		}

		/// <summary>
		/// Returns the sum of the squares of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> The sum of squares </returns>
		public virtual double Sumsq
		{
			get
			{
				return sumsqImpl.Result;
			}
		}

		/// <summary>
		/// Returns the mean of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> the mean </returns>
		public virtual double Mean
		{
			get
			{
				return meanImpl.Result;
			}
		}

		/// <summary>
		/// Returns the standard deviation of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> the standard deviation </returns>
		public virtual double StandardDeviation
		{
			get
			{
				double stdDev = double.NaN;
				if (N > 0)
				{
					if (N > 1)
					{
						stdDev = FastMath.sqrt(Variance);
					}
					else
					{
						stdDev = 0.0;
					}
				}
				return stdDev;
			}
		}

		/// <summary>
		/// Returns the (sample) variance of the available values.
		/// 
		/// <p>This method returns the bias-corrected sample variance (using {@code n - 1} in
		/// the denominator).  Use <seealso cref="#getPopulationVariance()"/> for the non-bias-corrected
		/// population variance.</p>
		/// 
		/// <p>Double.NaN is returned if no values have been added.</p>
		/// </summary>
		/// <returns> the variance </returns>
		public virtual double Variance
		{
			get
			{
				return varianceImpl.Result;
			}
		}

		/// <summary>
		/// Returns the <a href="http://en.wikibooks.org/wiki/Statistics/Summary/Variance">
		/// population variance</a> of the values that have been added.
		/// 
		/// <p>Double.NaN is returned if no values have been added.</p>
		/// </summary>
		/// <returns> the population variance </returns>
		public virtual double PopulationVariance
		{
			get
			{
				Variance populationVariance = new Variance(secondMoment);
				populationVariance.BiasCorrected = false;
				return populationVariance.Result;
			}
		}

		/// <summary>
		/// Returns the maximum of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> the maximum </returns>
		public virtual double Max
		{
			get
			{
				return maxImpl.Result;
			}
		}

		/// <summary>
		/// Returns the minimum of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> the minimum </returns>
		public virtual double Min
		{
			get
			{
				return minImpl.Result;
			}
		}

		/// <summary>
		/// Returns the geometric mean of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> the geometric mean </returns>
		public virtual double GeometricMean
		{
			get
			{
				return geoMeanImpl.Result;
			}
		}

		/// <summary>
		/// Returns the sum of the logs of the values that have been added.
		/// <p>
		/// Double.NaN is returned if no values have been added.
		/// </p> </summary>
		/// <returns> the sum of logs
		/// @since 1.2 </returns>
		public virtual double SumOfLogs
		{
			get
			{
				return sumLogImpl.Result;
			}
		}

		/// <summary>
		/// Returns a statistic related to the Second Central Moment.  Specifically,
		/// what is returned is the sum of squared deviations from the sample mean
		/// among the values that have been added.
		/// <p>
		/// Returns <code>Double.NaN</code> if no data values have been added and
		/// returns <code>0</code> if there is just one value in the data set.</p>
		/// <p> </summary>
		/// <returns> second central moment statistic
		/// @since 2.0 </returns>
		public virtual double SecondMoment
		{
			get
			{
				return secondMoment.Result;
			}
		}

		/// <summary>
		/// Generates a text report displaying summary statistics from values that
		/// have been added. </summary>
		/// <returns> String with line feeds displaying statistics
		/// @since 1.2 </returns>
		public override string ToString()
		{
			StringBuilder outBuffer = new StringBuilder();
			string endl = "\n";
			outBuffer.Append("SummaryStatistics:").Append(endl);
			outBuffer.Append("n: ").Append(N).Append(endl);
			outBuffer.Append("min: ").Append(Min).Append(endl);
			outBuffer.Append("max: ").Append(Max).Append(endl);
			outBuffer.Append("mean: ").Append(Mean).Append(endl);
			outBuffer.Append("geometric mean: ").Append(GeometricMean).Append(endl);
			outBuffer.Append("variance: ").Append(Variance).Append(endl);
			outBuffer.Append("sum of squares: ").Append(Sumsq).Append(endl);
			outBuffer.Append("standard deviation: ").Append(StandardDeviation).Append(endl);
			outBuffer.Append("sum of logs: ").Append(SumOfLogs).Append(endl);
			return outBuffer.ToString();
		}

		/// <summary>
		/// Resets all statistics and storage
		/// </summary>
		public virtual void clear()
		{
			this.n = 0;
			minImpl.clear();
			maxImpl.clear();
			sumImpl.clear();
			sumLogImpl.clear();
			sumsqImpl.clear();
			geoMeanImpl.clear();
			secondMoment.clear();
			if (meanImpl != mean)
			{
				meanImpl.clear();
			}
			if (varianceImpl != variance)
			{
				varianceImpl.clear();
			}
		}

		/// <summary>
		/// Returns true iff <code>object</code> is a
		/// <code>SummaryStatistics</code> instance and all statistics have the
		/// same values as this. </summary>
		/// <param name="object"> the object to test equality against. </param>
		/// <returns> true if object equals this </returns>
		public override bool Equals(object @object)
		{
			if (@object == this)
			{
				return true;
			}
			if (@object is SummaryStatistics == false)
			{
				return false;
			}
			SummaryStatistics stat = (SummaryStatistics)@object;
			return Precision.equalsIncludingNaN(stat.GeometricMean, GeometricMean) && Precision.equalsIncludingNaN(stat.Max, Max) && Precision.equalsIncludingNaN(stat.Mean, Mean) && Precision.equalsIncludingNaN(stat.Min, Min) && Precision.equalsIncludingNaN(stat.N, N) && Precision.equalsIncludingNaN(stat.Sum, Sum) && Precision.equalsIncludingNaN(stat.Sumsq, Sumsq) && Precision.equalsIncludingNaN(stat.Variance, Variance);
		}

		/// <summary>
		/// Returns hash code based on values of statistics </summary>
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
			result = result * 31 + MathUtils.hash(Sumsq);
			result = result * 31 + MathUtils.hash(Variance);
			return result;
		}

		// Getters and setters for statistics implementations
		/// <summary>
		/// Returns the currently configured Sum implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the sum
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic SumImpl
		{
			get
			{
				return sumImpl;
			}
			set
			{
				checkEmpty();
				this.sumImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the Sum.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="sumImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the Sum </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n >0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumImpl(StorelessUnivariateStatistic sumImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured sum of squares implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the sum of squares
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic SumsqImpl
		{
			get
			{
				return sumsqImpl;
			}
			set
			{
				checkEmpty();
				this.sumsqImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the sum of squares.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="sumsqImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the sum of squares </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumsqImpl(StorelessUnivariateStatistic sumsqImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured minimum implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the minimum
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic MinImpl
		{
			get
			{
				return minImpl;
			}
			set
			{
				checkEmpty();
				this.minImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the minimum.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="minImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the minimum </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setMinImpl(StorelessUnivariateStatistic minImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured maximum implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the maximum
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic MaxImpl
		{
			get
			{
				return maxImpl;
			}
			set
			{
				checkEmpty();
				this.maxImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the maximum.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="maxImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the maximum </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setMaxImpl(StorelessUnivariateStatistic maxImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured sum of logs implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the log sum
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic SumLogImpl
		{
			get
			{
				return sumLogImpl;
			}
			set
			{
				checkEmpty();
				this.sumLogImpl = value;
				geoMean.SumLogImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the sum of logs.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="sumLogImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the log sum </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSumLogImpl(StorelessUnivariateStatistic sumLogImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured geometric mean implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the geometric mean
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic GeoMeanImpl
		{
			get
			{
				return geoMeanImpl;
			}
			set
			{
				checkEmpty();
				this.geoMeanImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the geometric mean.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="geoMeanImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the geometric mean </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setGeoMeanImpl(StorelessUnivariateStatistic geoMeanImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured mean implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the mean
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic MeanImpl
		{
			get
			{
				return meanImpl;
			}
			set
			{
				checkEmpty();
				this.meanImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the mean.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="meanImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the mean </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setMeanImpl(StorelessUnivariateStatistic meanImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Returns the currently configured variance implementation </summary>
		/// <returns> the StorelessUnivariateStatistic implementing the variance
		/// @since 1.2 </returns>
		public virtual StorelessUnivariateStatistic VarianceImpl
		{
			get
			{
				return varianceImpl;
			}
			set
			{
				checkEmpty();
				this.varianceImpl = value;
			}
		}

		/// <summary>
		/// <p>
		/// Sets the implementation for the variance.
		/// </p>
		/// <p>
		/// This method cannot be activated after data has been added - i.e.,
		/// after <seealso cref="#addValue(double) addValue"/> has been used to add data.
		/// If it is activated after data has been added, an IllegalStateException
		/// will be thrown.
		/// </p> </summary>
		/// <param name="varianceImpl"> the StorelessUnivariateStatistic instance to use for
		///        computing the variance </param>
		/// <exception cref="MathIllegalStateException"> if data has already been added (i.e if n > 0)
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setVarianceImpl(StorelessUnivariateStatistic varianceImpl) throws org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// Throws IllegalStateException if n > 0. </summary>
		/// <exception cref="MathIllegalStateException"> if data has been added </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkEmpty() throws org.apache.commons.math3.exception.MathIllegalStateException
		private void checkEmpty()
		{
			if (n > 0)
			{
				throw new MathIllegalStateException(LocalizedFormats.VALUES_ADDED_BEFORE_CONFIGURING_STATISTIC, n);
			}
		}

		/// <summary>
		/// Returns a copy of this SummaryStatistics instance with the same internal state.
		/// </summary>
		/// <returns> a copy of this </returns>
		public virtual SummaryStatistics copy()
		{
			SummaryStatistics result = new SummaryStatistics();
			// No try-catch or advertised exception because arguments are guaranteed non-null
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> SummaryStatistics to copy </param>
		/// <param name="dest"> SummaryStatistics to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(SummaryStatistics source, SummaryStatistics dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(SummaryStatistics source, SummaryStatistics dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			dest.maxImpl = source.maxImpl.copy();
			dest.minImpl = source.minImpl.copy();
			dest.sumImpl = source.sumImpl.copy();
			dest.sumLogImpl = source.sumLogImpl.copy();
			dest.sumsqImpl = source.sumsqImpl.copy();
			dest.secondMoment = source.secondMoment.copy();
			dest.n = source.n;

			// Keep commons-math supplied statistics with embedded moments in synch
			if (source.VarianceImpl is Variance)
			{
				dest.varianceImpl = new Variance(dest.secondMoment);
			}
			else
			{
				dest.varianceImpl = source.varianceImpl.copy();
			}
			if (source.meanImpl is Mean)
			{
				dest.meanImpl = new Mean(dest.secondMoment);
			}
			else
			{
				dest.meanImpl = source.meanImpl.copy();
			}
			if (source.GeoMeanImpl is GeometricMean)
			{
				dest.geoMeanImpl = new GeometricMean((SumOfLogs) dest.sumLogImpl);
			}
			else
			{
				dest.geoMeanImpl = source.geoMeanImpl.copy();
			}

			// Make sure that if stat == statImpl in source, same
			// holds in dest; otherwise copy stat
			if (source.geoMean == source.geoMeanImpl)
			{
				dest.geoMean = (GeometricMean) dest.geoMeanImpl;
			}
			else
			{
				GeometricMean.copy(source.geoMean, dest.geoMean);
			}
			if (source.max == source.maxImpl)
			{
				dest.max = (Max) dest.maxImpl;
			}
			else
			{
				Max.copy(source.max, dest.max);
			}
			if (source.mean == source.meanImpl)
			{
				dest.mean = (Mean) dest.meanImpl;
			}
			else
			{
				Mean.copy(source.mean, dest.mean);
			}
			if (source.min == source.minImpl)
			{
				dest.min = (Min) dest.minImpl;
			}
			else
			{
				Min.copy(source.min, dest.min);
			}
			if (source.sum == source.sumImpl)
			{
				dest.sum = (Sum) dest.sumImpl;
			}
			else
			{
				Sum.copy(source.sum, dest.sum);
			}
			if (source.variance == source.varianceImpl)
			{
				dest.variance = (Variance) dest.varianceImpl;
			}
			else
			{
				Variance.copy(source.variance, dest.variance);
			}
			if (source.sumLog == source.sumLogImpl)
			{
				dest.sumLog = (SumOfLogs) dest.sumLogImpl;
			}
			else
			{
				SumOfLogs.copy(source.sumLog, dest.sumLog);
			}
			if (source.sumsq == source.sumsqImpl)
			{
				dest.sumsq = (SumOfSquares) dest.sumsqImpl;
			}
			else
			{
				SumOfSquares.copy(source.sumsq, dest.sumsq);
			}
		}
	}

}