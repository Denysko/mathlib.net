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


	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using GeometricMean = mathlib.stat.descriptive.moment.GeometricMean;
	using Kurtosis = mathlib.stat.descriptive.moment.Kurtosis;
	using Mean = mathlib.stat.descriptive.moment.Mean;
	using Skewness = mathlib.stat.descriptive.moment.Skewness;
	using Variance = mathlib.stat.descriptive.moment.Variance;
	using Max = mathlib.stat.descriptive.rank.Max;
	using Min = mathlib.stat.descriptive.rank.Min;
	using Percentile = mathlib.stat.descriptive.rank.Percentile;
	using Sum = mathlib.stat.descriptive.summary.Sum;
	using SumOfSquares = mathlib.stat.descriptive.summary.SumOfSquares;
	using MathUtils = mathlib.util.MathUtils;
	using ResizableDoubleArray = mathlib.util.ResizableDoubleArray;
	using FastMath = mathlib.util.FastMath;


	/// <summary>
	/// Maintains a dataset of values of a single variable and computes descriptive
	/// statistics based on stored data. The <seealso cref="#getWindowSize() windowSize"/>
	/// property sets a limit on the number of values that can be stored in the
	/// dataset.  The default value, INFINITE_WINDOW, puts no limit on the size of
	/// the dataset.  This value should be used with caution, as the backing store
	/// will grow without bound in this case.  For very large datasets,
	/// <seealso cref="SummaryStatistics"/>, which does not store the dataset, should be used
	/// instead of this class. If <code>windowSize</code> is not INFINITE_WINDOW and
	/// more values are added than can be stored in the dataset, new values are
	/// added in a "rolling" manner, with new values replacing the "oldest" values
	/// in the dataset.
	/// 
	/// <p>Note: this class is not threadsafe.  Use
	/// <seealso cref="SynchronizedDescriptiveStatistics"/> if concurrent access from multiple
	/// threads is required.</p>
	/// 
	/// @version $Id: DescriptiveStatistics.java 1422354 2012-12-15 20:59:01Z psteitz $
	/// </summary>
	[Serializable]
	public class DescriptiveStatistics : StatisticalSummary
	{

		/// <summary>
		/// Represents an infinite window size.  When the <seealso cref="#getWindowSize()"/>
		/// returns this value, there is no limit to the number of data values
		/// that can be stored in the dataset.
		/// </summary>
		public const int INFINITE_WINDOW = -1;

		/// <summary>
		/// Serialization UID </summary>
		private const long serialVersionUID = 4133067267405273064L;

		/// <summary>
		/// Name of the setQuantile method. </summary>
		private const string SET_QUANTILE_METHOD_NAME = "setQuantile";

		/// <summary>
		/// hold the window size * </summary>
		protected internal int windowSize = INFINITE_WINDOW;

		/// <summary>
		///  Stored data values
		/// </summary>
		private ResizableDoubleArray eDA = new ResizableDoubleArray();

		/// <summary>
		/// Mean statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic meanImpl = new Mean();

		/// <summary>
		/// Geometric mean statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic geometricMeanImpl = new GeometricMean();

		/// <summary>
		/// Kurtosis statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic kurtosisImpl = new Kurtosis();

		/// <summary>
		/// Maximum statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic maxImpl = new Max();

		/// <summary>
		/// Minimum statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic minImpl = new Min();

		/// <summary>
		/// Percentile statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic percentileImpl = new Percentile();

		/// <summary>
		/// Skewness statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic skewnessImpl = new Skewness();

		/// <summary>
		/// Variance statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic varianceImpl = new Variance();

		/// <summary>
		/// Sum of squares statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic sumsqImpl = new SumOfSquares();

		/// <summary>
		/// Sum statistic implementation - can be reset by setter. </summary>
		private UnivariateStatistic sumImpl = new Sum();

		/// <summary>
		/// Construct a DescriptiveStatistics instance with an infinite window
		/// </summary>
		public DescriptiveStatistics()
		{
		}

		/// <summary>
		/// Construct a DescriptiveStatistics instance with the specified window
		/// </summary>
		/// <param name="window"> the window size. </param>
		/// <exception cref="MathIllegalArgumentException"> if window size is less than 1 but
		/// not equal to <seealso cref="#INFINITE_WINDOW"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DescriptiveStatistics(int window) throws mathlib.exception.MathIllegalArgumentException
		public DescriptiveStatistics(int window)
		{
			WindowSize = window;
		}

		/// <summary>
		/// Construct a DescriptiveStatistics instance with an infinite window
		/// and the initial data values in double[] initialDoubleArray.
		/// If initialDoubleArray is null, then this constructor corresponds to
		/// DescriptiveStatistics()
		/// </summary>
		/// <param name="initialDoubleArray"> the initial double[]. </param>
		public DescriptiveStatistics(double[] initialDoubleArray)
		{
			if (initialDoubleArray != null)
			{
				eDA = new ResizableDoubleArray(initialDoubleArray);
			}
		}

		/// <summary>
		/// Copy constructor.  Construct a new DescriptiveStatistics instance that
		/// is a copy of original.
		/// </summary>
		/// <param name="original"> DescriptiveStatistics instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DescriptiveStatistics(DescriptiveStatistics original) throws mathlib.exception.NullArgumentException
		public DescriptiveStatistics(DescriptiveStatistics original)
		{
			copy(original, this);
		}

		/// <summary>
		/// Adds the value to the dataset. If the dataset is at the maximum size
		/// (i.e., the number of stored elements equals the currently configured
		/// windowSize), the first (oldest) element in the dataset is discarded
		/// to make room for the new value.
		/// </summary>
		/// <param name="v"> the value to be added </param>
		public virtual void addValue(double v)
		{
			if (windowSize != INFINITE_WINDOW)
			{
				if (N == windowSize)
				{
					eDA.addElementRolling(v);
				}
				else if (N < windowSize)
				{
					eDA.addElement(v);
				}
			}
			else
			{
				eDA.addElement(v);
			}
		}

		/// <summary>
		/// Removes the most recent value from the dataset.
		/// </summary>
		/// <exception cref="MathIllegalStateException"> if there are no elements stored </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void removeMostRecentValue() throws mathlib.exception.MathIllegalStateException
		public virtual void removeMostRecentValue()
		{
			try
			{
				eDA.discardMostRecentElements(1);
			}
			catch (MathIllegalArgumentException ex)
			{
				throw new MathIllegalStateException(LocalizedFormats.NO_DATA);
			}
		}

		/// <summary>
		/// Replaces the most recently stored value with the given value.
		/// There must be at least one element stored to call this method.
		/// </summary>
		/// <param name="v"> the value to replace the most recent stored value </param>
		/// <returns> replaced value </returns>
		/// <exception cref="MathIllegalStateException"> if there are no elements stored </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double replaceMostRecentValue(double v) throws mathlib.exception.MathIllegalStateException
		public virtual double replaceMostRecentValue(double v)
		{
			return eDA.substituteMostRecentElement(v);
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/arithmetic_mean.htm">
		/// arithmetic mean </a> of the available values </summary>
		/// <returns> The mean or Double.NaN if no values have been added. </returns>
		public virtual double Mean
		{
			get
			{
				return apply(meanImpl);
			}
		}

		/// <summary>
		/// Returns the <a href="http://www.xycoon.com/geometric_mean.htm">
		/// geometric mean </a> of the available values </summary>
		/// <returns> The geometricMean, Double.NaN if no values have been added,
		/// or if the product of the available values is less than or equal to 0. </returns>
		public virtual double GeometricMean
		{
			get
			{
				return apply(geometricMeanImpl);
			}
		}

		/// <summary>
		/// Returns the (sample) variance of the available values.
		/// 
		/// <p>This method returns the bias-corrected sample variance (using {@code n - 1} in
		/// the denominator).  Use <seealso cref="#getPopulationVariance()"/> for the non-bias-corrected
		/// population variance.</p>
		/// </summary>
		/// <returns> The variance, Double.NaN if no values have been added
		/// or 0.0 for a single value set. </returns>
		public virtual double Variance
		{
			get
			{
				return apply(varianceImpl);
			}
		}

		/// <summary>
		/// Returns the <a href="http://en.wikibooks.org/wiki/Statistics/Summary/Variance">
		/// population variance</a> of the available values.
		/// </summary>
		/// <returns> The population variance, Double.NaN if no values have been added,
		/// or 0.0 for a single value set. </returns>
		public virtual double PopulationVariance
		{
			get
			{
				return apply(new Variance(false));
			}
		}

		/// <summary>
		/// Returns the standard deviation of the available values. </summary>
		/// <returns> The standard deviation, Double.NaN if no values have been added
		/// or 0.0 for a single value set. </returns>
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
		/// Returns the skewness of the available values. Skewness is a
		/// measure of the asymmetry of a given distribution. </summary>
		/// <returns> The skewness, Double.NaN if no values have been added
		/// or 0.0 for a value set &lt;=2. </returns>
		public virtual double Skewness
		{
			get
			{
				return apply(skewnessImpl);
			}
		}

		/// <summary>
		/// Returns the Kurtosis of the available values. Kurtosis is a
		/// measure of the "peakedness" of a distribution </summary>
		/// <returns> The kurtosis, Double.NaN if no values have been added, or 0.0
		/// for a value set &lt;=3. </returns>
		public virtual double Kurtosis
		{
			get
			{
				return apply(kurtosisImpl);
			}
		}

		/// <summary>
		/// Returns the maximum of the available values </summary>
		/// <returns> The max or Double.NaN if no values have been added. </returns>
		public virtual double Max
		{
			get
			{
				return apply(maxImpl);
			}
		}

		/// <summary>
		/// Returns the minimum of the available values </summary>
		/// <returns> The min or Double.NaN if no values have been added. </returns>
		public virtual double Min
		{
			get
			{
				return apply(minImpl);
			}
		}

		/// <summary>
		/// Returns the number of available values </summary>
		/// <returns> The number of available values </returns>
		public virtual long N
		{
			get
			{
				return eDA.NumElements;
			}
		}

		/// <summary>
		/// Returns the sum of the values that have been added to Univariate. </summary>
		/// <returns> The sum or Double.NaN if no values have been added </returns>
		public virtual double Sum
		{
			get
			{
				return apply(sumImpl);
			}
		}

		/// <summary>
		/// Returns the sum of the squares of the available values. </summary>
		/// <returns> The sum of the squares or Double.NaN if no
		/// values have been added. </returns>
		public virtual double Sumsq
		{
			get
			{
				return apply(sumsqImpl);
			}
		}

		/// <summary>
		/// Resets all statistics and storage
		/// </summary>
		public virtual void clear()
		{
			eDA.clear();
		}


		/// <summary>
		/// Returns the maximum number of values that can be stored in the
		/// dataset, or INFINITE_WINDOW (-1) if there is no limit.
		/// </summary>
		/// <returns> The current window size or -1 if its Infinite. </returns>
		public virtual int WindowSize
		{
			get
			{
				return windowSize;
			}
			set
			{
				if (value < 1 && value != INFINITE_WINDOW)
				{
					throw new MathIllegalArgumentException(LocalizedFormats.NOT_POSITIVE_WINDOW_SIZE, value);
				}
    
				this.windowSize = value;
    
				// We need to check to see if we need to discard elements
				// from the front of the array.  If the value is less than
				// the current number of elements.
				if (value != INFINITE_WINDOW && value < eDA.NumElements)
				{
					eDA.discardFrontElements(eDA.NumElements - value);
				}
			}
		}

		/// <summary>
		/// WindowSize controls the number of values that contribute to the
		/// reported statistics.  For example, if windowSize is set to 3 and the
		/// values {1,2,3,4,5} have been added <strong> in that order</strong> then
		/// the <i>available values</i> are {3,4,5} and all reported statistics will
		/// be based on these values. If {@code windowSize} is decreased as a result
		/// of this call and there are more than the new value of elements in the
		/// current dataset, values from the front of the array are discarded to
		/// reduce the dataset to {@code windowSize} elements.
		/// </summary>
		/// <param name="windowSize"> sets the size of the window. </param>
		/// <exception cref="MathIllegalArgumentException"> if window size is less than 1 but
		/// not equal to <seealso cref="#INFINITE_WINDOW"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setWindowSize(int windowSize) throws mathlib.exception.MathIllegalArgumentException

		/// <summary>
		/// Returns the current set of values in an array of double primitives.
		/// The order of addition is preserved.  The returned array is a fresh
		/// copy of the underlying data -- i.e., it is not a reference to the
		/// stored data.
		/// </summary>
		/// <returns> returns the current set of numbers in the order in which they
		///         were added to this set </returns>
		public virtual double[] Values
		{
			get
			{
				return eDA.Elements;
			}
		}

		/// <summary>
		/// Returns the current set of values in an array of double primitives,
		/// sorted in ascending order.  The returned array is a fresh
		/// copy of the underlying data -- i.e., it is not a reference to the
		/// stored data. </summary>
		/// <returns> returns the current set of
		/// numbers sorted in ascending order </returns>
		public virtual double[] SortedValues
		{
			get
			{
				double[] sort = Values;
				Arrays.sort(sort);
				return sort;
			}
		}

		/// <summary>
		/// Returns the element at the specified index </summary>
		/// <param name="index"> The Index of the element </param>
		/// <returns> return the element at the specified index </returns>
		public virtual double getElement(int index)
		{
			return eDA.getElement(index);
		}

		/// <summary>
		/// Returns an estimate for the pth percentile of the stored values.
		/// <p>
		/// The implementation provided here follows the first estimation procedure presented
		/// <a href="http://www.itl.nist.gov/div898/handbook/prc/section2/prc252.htm">here.</a>
		/// </p><p>
		/// <strong>Preconditions</strong>:<ul>
		/// <li><code>0 &lt; p &le; 100</code> (otherwise an
		/// <code>MathIllegalArgumentException</code> is thrown)</li>
		/// <li>at least one value must be stored (returns <code>Double.NaN
		///     </code> otherwise)</li>
		/// </ul></p>
		/// </summary>
		/// <param name="p"> the requested percentile (scaled from 0 - 100) </param>
		/// <returns> An estimate for the pth percentile of the stored data </returns>
		/// <exception cref="MathIllegalStateException"> if percentile implementation has been
		///  overridden and the supplied implementation does not support setQuantile </exception>
		/// <exception cref="MathIllegalArgumentException"> if p is not a valid quantile </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getPercentile(double p) throws mathlib.exception.MathIllegalStateException, mathlib.exception.MathIllegalArgumentException
		public virtual double getPercentile(double p)
		{
			if (percentileImpl is Percentile)
			{
				((Percentile) percentileImpl).Quantile = p;
			}
			else
			{
				try
				{
					percentileImpl.GetType().GetMethod(SET_QUANTILE_METHOD_NAME, new Type[] {double.TYPE}).invoke(percentileImpl, new object[] {Convert.ToDouble(p)});
				} // Setter guard should prevent
				catch (NoSuchMethodException e1)
				{
					throw new MathIllegalStateException(LocalizedFormats.PERCENTILE_IMPLEMENTATION_UNSUPPORTED_METHOD, percentileImpl.GetType().Name, SET_QUANTILE_METHOD_NAME);
				}
				catch (IllegalAccessException e2)
				{
					throw new MathIllegalStateException(LocalizedFormats.PERCENTILE_IMPLEMENTATION_CANNOT_ACCESS_METHOD, SET_QUANTILE_METHOD_NAME, percentileImpl.GetType().Name);
				}
				catch (InvocationTargetException e3)
				{
					throw new IllegalStateException(e3.InnerException);
				}
			}
			return apply(percentileImpl);
		}

		/// <summary>
		/// Generates a text report displaying univariate statistics from values
		/// that have been added.  Each statistic is displayed on a separate
		/// line.
		/// </summary>
		/// <returns> String with line feeds displaying statistics </returns>
		public override string ToString()
		{
			StringBuilder outBuffer = new StringBuilder();
			string endl = "\n";
			outBuffer.Append("DescriptiveStatistics:").Append(endl);
			outBuffer.Append("n: ").Append(N).Append(endl);
			outBuffer.Append("min: ").Append(Min).Append(endl);
			outBuffer.Append("max: ").Append(Max).Append(endl);
			outBuffer.Append("mean: ").Append(Mean).Append(endl);
			outBuffer.Append("std dev: ").Append(StandardDeviation).Append(endl);
			try
			{
				// No catch for MIAE because actual parameter is valid below
				outBuffer.Append("median: ").Append(getPercentile(50)).Append(endl);
			}
			catch (MathIllegalStateException ex)
			{
				outBuffer.Append("median: unavailable").Append(endl);
			}
			outBuffer.Append("skewness: ").Append(Skewness).Append(endl);
			outBuffer.Append("kurtosis: ").Append(Kurtosis).Append(endl);
			return outBuffer.ToString();
		}

		/// <summary>
		/// Apply the given statistic to the data associated with this set of statistics. </summary>
		/// <param name="stat"> the statistic to apply </param>
		/// <returns> the computed value of the statistic. </returns>
		public virtual double apply(UnivariateStatistic stat)
		{
			// No try-catch or advertised exception here because arguments are guaranteed valid
			return eDA.compute(stat);
		}

		// Implementation getters and setter

		/// <summary>
		/// Returns the currently configured mean implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the mean
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic MeanImpl
		{
			get
			{
				lock (this)
				{
					return meanImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.meanImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured geometric mean implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the geometric mean
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic GeometricMeanImpl
		{
			get
			{
				lock (this)
				{
					return geometricMeanImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.geometricMeanImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured kurtosis implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the kurtosis
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic KurtosisImpl
		{
			get
			{
				lock (this)
				{
					return kurtosisImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.kurtosisImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured maximum implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the maximum
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic MaxImpl
		{
			get
			{
				lock (this)
				{
					return maxImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.maxImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured minimum implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the minimum
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic MinImpl
		{
			get
			{
				lock (this)
				{
					return minImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.minImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured percentile implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the percentile
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic PercentileImpl
		{
			get
			{
				lock (this)
				{
					return percentileImpl;
				}
			}
			set
			{
				lock (this)
				{
					try
					{
						value.GetType().GetMethod(SET_QUANTILE_METHOD_NAME, new Type[] {double.TYPE}).invoke(value, new object[] {Convert.ToDouble(50.0d)});
					}
					catch (NoSuchMethodException e1)
					{
						throw new MathIllegalArgumentException(LocalizedFormats.PERCENTILE_IMPLEMENTATION_UNSUPPORTED_METHOD, value.GetType().Name, SET_QUANTILE_METHOD_NAME);
					}
					catch (IllegalAccessException e2)
					{
						throw new MathIllegalArgumentException(LocalizedFormats.PERCENTILE_IMPLEMENTATION_CANNOT_ACCESS_METHOD, SET_QUANTILE_METHOD_NAME, value.GetType().Name);
					}
					catch (InvocationTargetException e3)
					{
						throw new System.ArgumentException(e3.InnerException);
					}
					this.percentileImpl = value;
				}
			}
		}

		/// <summary>
		/// Sets the implementation to be used by <seealso cref="#getPercentile(double)"/>.
		/// The supplied <code>UnivariateStatistic</code> must provide a
		/// <code>setQuantile(double)</code> method; otherwise
		/// <code>IllegalArgumentException</code> is thrown.
		/// </summary>
		/// <param name="percentileImpl"> the percentileImpl to set </param>
		/// <exception cref="MathIllegalArgumentException"> if the supplied implementation does not
		///  provide a <code>setQuantile</code> method
		/// @since 1.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized void setPercentileImpl(UnivariateStatistic percentileImpl) throws mathlib.exception.MathIllegalArgumentException

		/// <summary>
		/// Returns the currently configured skewness implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the skewness
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic SkewnessImpl
		{
			get
			{
				lock (this)
				{
					return skewnessImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.skewnessImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured variance implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the variance
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic VarianceImpl
		{
			get
			{
				lock (this)
				{
					return varianceImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.varianceImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured sum of squares implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the sum of squares
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic SumsqImpl
		{
			get
			{
				lock (this)
				{
					return sumsqImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.sumsqImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns the currently configured sum implementation.
		/// </summary>
		/// <returns> the UnivariateStatistic implementing the sum
		/// @since 1.2 </returns>
		public virtual UnivariateStatistic SumImpl
		{
			get
			{
				lock (this)
				{
					return sumImpl;
				}
			}
			set
			{
				lock (this)
				{
					this.sumImpl = value;
				}
			}
		}


		/// <summary>
		/// Returns a copy of this DescriptiveStatistics instance with the same internal state.
		/// </summary>
		/// <returns> a copy of this </returns>
		public virtual DescriptiveStatistics copy()
		{
			DescriptiveStatistics result = new DescriptiveStatistics();
			// No try-catch or advertised exception because parms are guaranteed valid
			copy(this, result);
			return result;
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// </summary>
		/// <param name="source"> DescriptiveStatistics to copy </param>
		/// <param name="dest"> DescriptiveStatistics to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(DescriptiveStatistics source, DescriptiveStatistics dest) throws mathlib.exception.NullArgumentException
		public static void copy(DescriptiveStatistics source, DescriptiveStatistics dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			// Copy data and window size
			dest.eDA = source.eDA.copy();
			dest.windowSize = source.windowSize;

			// Copy implementations
			dest.maxImpl = source.maxImpl.copy();
			dest.meanImpl = source.meanImpl.copy();
			dest.minImpl = source.minImpl.copy();
			dest.sumImpl = source.sumImpl.copy();
			dest.varianceImpl = source.varianceImpl.copy();
			dest.sumsqImpl = source.sumsqImpl.copy();
			dest.geometricMeanImpl = source.geometricMeanImpl.copy();
			dest.kurtosisImpl = source.kurtosisImpl;
			dest.skewnessImpl = source.skewnessImpl;
			dest.percentileImpl = source.percentileImpl;
		}
	}

}