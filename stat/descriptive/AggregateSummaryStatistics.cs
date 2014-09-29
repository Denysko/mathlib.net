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

namespace mathlib.stat.descriptive
{


	using NullArgumentException = mathlib.exception.NullArgumentException;

	/// <summary>
	/// <p>
	/// An aggregator for {@code SummaryStatistics} from several data sets or
	/// data set partitions.  In its simplest usage mode, the client creates an
	/// instance via the zero-argument constructor, then uses
	/// <seealso cref="#createContributingStatistics()"/> to obtain a {@code SummaryStatistics}
	/// for each individual data set / partition.  The per-set statistics objects
	/// are used as normal, and at any time the aggregate statistics for all the
	/// contributors can be obtained from this object.
	/// </p><p>
	/// Clients with specialized requirements can use alternative constructors to
	/// control the statistics implementations and initial values used by the
	/// contributing and the internal aggregate {@code SummaryStatistics} objects.
	/// </p><p>
	/// A static <seealso cref="#aggregate(Collection)"/> method is also included that computes
	/// aggregate statistics directly from a Collection of SummaryStatistics instances.
	/// </p><p>
	/// When <seealso cref="#createContributingStatistics()"/> is used to create SummaryStatistics
	/// instances to be aggregated concurrently, the created instances'
	/// <seealso cref="SummaryStatistics#addValue(double)"/> methods must synchronize on the aggregating
	/// instance maintained by this class.  In multithreaded environments, if the functionality
	/// provided by <seealso cref="#aggregate(Collection)"/> is adequate, that method should be used
	/// to avoid unnecessary computation and synchronization delays.</p>
	/// 
	/// @since 2.0
	/// @version $Id: AggregateSummaryStatistics.java 1416643 2012-12-03 19:37:14Z tn $
	/// 
	/// </summary>
	[Serializable]
	public class AggregateSummaryStatistics : StatisticalSummary
	{


		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -8207112444016386906L;

		/// <summary>
		/// A SummaryStatistics serving as a prototype for creating SummaryStatistics
		/// contributing to this aggregate
		/// </summary>
		private readonly SummaryStatistics statisticsPrototype;

		/// <summary>
		/// The SummaryStatistics in which aggregate statistics are accumulated.
		/// </summary>
		private readonly SummaryStatistics statistics;

		/// <summary>
		/// Initializes a new AggregateSummaryStatistics with default statistics
		/// implementations.
		/// 
		/// </summary>
		public AggregateSummaryStatistics() : this(new SummaryStatistics())
		{
			// No try-catch or throws NAE because arg is guaranteed non-null
		}

		/// <summary>
		/// Initializes a new AggregateSummaryStatistics with the specified statistics
		/// object as a prototype for contributing statistics and for the internal
		/// aggregate statistics.  This provides for customized statistics implementations
		/// to be used by contributing and aggregate statistics.
		/// </summary>
		/// <param name="prototypeStatistics"> a {@code SummaryStatistics} serving as a
		///      prototype both for the internal aggregate statistics and for
		///      contributing statistics obtained via the
		///      {@code createContributingStatistics()} method.  Being a prototype
		///      means that other objects are initialized by copying this object's state.
		///      If {@code null}, a new, default statistics object is used.  Any statistic
		///      values in the prototype are propagated to contributing statistics
		///      objects and (once) into these aggregate statistics. </param>
		/// <exception cref="NullArgumentException"> if prototypeStatistics is null </exception>
		/// <seealso cref= #createContributingStatistics() </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AggregateSummaryStatistics(SummaryStatistics prototypeStatistics) throws mathlib.exception.NullArgumentException
		public AggregateSummaryStatistics(SummaryStatistics prototypeStatistics) : this(prototypeStatistics, prototypeStatistics == null ? null : new SummaryStatistics(prototypeStatistics))
		{
		}

		/// <summary>
		/// Initializes a new AggregateSummaryStatistics with the specified statistics
		/// object as a prototype for contributing statistics and for the internal
		/// aggregate statistics.  This provides for different statistics implementations
		/// to be used by contributing and aggregate statistics and for an initial
		/// state to be supplied for the aggregate statistics.
		/// </summary>
		/// <param name="prototypeStatistics"> a {@code SummaryStatistics} serving as a
		///      prototype both for the internal aggregate statistics and for
		///      contributing statistics obtained via the
		///      {@code createContributingStatistics()} method.  Being a prototype
		///      means that other objects are initialized by copying this object's state.
		///      If {@code null}, a new, default statistics object is used.  Any statistic
		///      values in the prototype are propagated to contributing statistics
		///      objects, but not into these aggregate statistics. </param>
		/// <param name="initialStatistics"> a {@code SummaryStatistics} to serve as the
		///      internal aggregate statistics object.  If {@code null}, a new, default
		///      statistics object is used. </param>
		/// <seealso cref= #createContributingStatistics() </seealso>
		public AggregateSummaryStatistics(SummaryStatistics prototypeStatistics, SummaryStatistics initialStatistics)
		{
			this.statisticsPrototype = (prototypeStatistics == null) ? new SummaryStatistics() : prototypeStatistics;
			this.statistics = (initialStatistics == null) ? new SummaryStatistics() : initialStatistics;
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns the maximum over all the aggregated
		/// data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getMax() </seealso>
		public virtual double Max
		{
			get
			{
				lock (statistics)
				{
					return statistics.Max;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns the mean of all the aggregated data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getMean() </seealso>
		public virtual double Mean
		{
			get
			{
				lock (statistics)
				{
					return statistics.Mean;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns the minimum over all the aggregated
		/// data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getMin() </seealso>
		public virtual double Min
		{
			get
			{
				lock (statistics)
				{
					return statistics.Min;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns a count of all the aggregated data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getN() </seealso>
		public virtual long N
		{
			get
			{
				lock (statistics)
				{
					return statistics.N;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns the standard deviation of all the
		/// aggregated data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getStandardDeviation() </seealso>
		public virtual double StandardDeviation
		{
			get
			{
				lock (statistics)
				{
					return statistics.StandardDeviation;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns a sum of all the aggregated data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getSum() </seealso>
		public virtual double Sum
		{
			get
			{
				lock (statistics)
				{
					return statistics.Sum;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}.  This version returns the variance of all the aggregated
		/// data.
		/// </summary>
		/// <seealso cref= StatisticalSummary#getVariance() </seealso>
		public virtual double Variance
		{
			get
			{
				lock (statistics)
				{
					return statistics.Variance;
				}
			}
		}

		/// <summary>
		/// Returns the sum of the logs of all the aggregated data.
		/// </summary>
		/// <returns> the sum of logs </returns>
		/// <seealso cref= SummaryStatistics#getSumOfLogs() </seealso>
		public virtual double SumOfLogs
		{
			get
			{
				lock (statistics)
				{
					return statistics.SumOfLogs;
				}
			}
		}

		/// <summary>
		/// Returns the geometric mean of all the aggregated data.
		/// </summary>
		/// <returns> the geometric mean </returns>
		/// <seealso cref= SummaryStatistics#getGeometricMean() </seealso>
		public virtual double GeometricMean
		{
			get
			{
				lock (statistics)
				{
					return statistics.GeometricMean;
				}
			}
		}

		/// <summary>
		/// Returns the sum of the squares of all the aggregated data.
		/// </summary>
		/// <returns> The sum of squares </returns>
		/// <seealso cref= SummaryStatistics#getSumsq() </seealso>
		public virtual double Sumsq
		{
			get
			{
				lock (statistics)
				{
					return statistics.Sumsq;
				}
			}
		}

		/// <summary>
		/// Returns a statistic related to the Second Central Moment.  Specifically,
		/// what is returned is the sum of squared deviations from the sample mean
		/// among the all of the aggregated data.
		/// </summary>
		/// <returns> second central moment statistic </returns>
		/// <seealso cref= SummaryStatistics#getSecondMoment() </seealso>
		public virtual double SecondMoment
		{
			get
			{
				lock (statistics)
				{
					return statistics.SecondMoment;
				}
			}
		}

		/// <summary>
		/// Return a <seealso cref="StatisticalSummaryValues"/> instance reporting current
		/// aggregate statistics.
		/// </summary>
		/// <returns> Current values of aggregate statistics </returns>
		public virtual StatisticalSummary Summary
		{
			get
			{
				lock (statistics)
				{
					return new StatisticalSummaryValues(Mean, Variance, N, Max, Min, Sum);
				}
			}
		}

		/// <summary>
		/// Creates and returns a {@code SummaryStatistics} whose data will be
		/// aggregated with those of this {@code AggregateSummaryStatistics}.
		/// </summary>
		/// <returns> a {@code SummaryStatistics} whose data will be aggregated with
		///      those of this {@code AggregateSummaryStatistics}.  The initial state
		///      is a copy of the configured prototype statistics. </returns>
		public virtual SummaryStatistics createContributingStatistics()
		{
			SummaryStatistics contributingStatistics = new AggregatingSummaryStatistics(statistics);

			// No try - catch or advertising NAE because neither argument will ever be null
			SummaryStatistics.copy(statisticsPrototype, contributingStatistics);

			return contributingStatistics;
		}

		/// <summary>
		/// Computes aggregate summary statistics. This method can be used to combine statistics
		/// computed over partitions or subsamples - i.e., the StatisticalSummaryValues returned
		/// should contain the same values that would have been obtained by computing a single
		/// StatisticalSummary over the combined dataset.
		/// <p>
		/// Returns null if the collection is empty or null.
		/// </p>
		/// </summary>
		/// <param name="statistics"> collection of SummaryStatistics to aggregate </param>
		/// <returns> summary statistics for the combined dataset </returns>
		public static StatisticalSummaryValues aggregate(ICollection<SummaryStatistics> statistics)
		{
			if (statistics == null)
			{
				return null;
			}
			IEnumerator<SummaryStatistics> iterator = statistics.GetEnumerator();
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			if (!iterator.hasNext())
			{
				return null;
			}
//JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			SummaryStatistics current = iterator.next();
			long n = current.N;
			double min = current.Min;
			double sum = current.Sum;
			double max = current.Max;
			double m2 = current.SecondMoment;
			double mean = current.Mean;
			while (iterator.MoveNext())
			{
				current = iterator.Current;
				if (current.Min < min || double.IsNaN(min))
				{
					min = current.Min;
				}
				if (current.Max > max || double.IsNaN(max))
				{
					max = current.Max;
				}
				sum += current.Sum;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oldN = n;
				double oldN = n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double curN = current.getN();
				double curN = current.N;
				n += (long)curN;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double meanDiff = current.getMean() - mean;
				double meanDiff = current.Mean - mean;
				mean = sum / n;
				m2 = m2 + current.SecondMoment + meanDiff * meanDiff * oldN * curN / n;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double variance;
			double variance;
			if (n == 0)
			{
				variance = double.NaN;
			}
			else if (n == 1)
			{
				variance = 0d;
			}
			else
			{
				variance = m2 / (n - 1);
			}
			return new StatisticalSummaryValues(mean, variance, n, max, min, sum);
		}

		/// <summary>
		/// A SummaryStatistics that also forwards all values added to it to a second
		/// {@code SummaryStatistics} for aggregation.
		/// 
		/// @since 2.0
		/// </summary>
		private class AggregatingSummaryStatistics : SummaryStatistics
		{

			/// <summary>
			/// The serialization version of this class
			/// </summary>
			internal const long serialVersionUID = 1L;

			/// <summary>
			/// An additional SummaryStatistics into which values added to these
			/// statistics (and possibly others) are aggregated
			/// </summary>
			internal readonly SummaryStatistics aggregateStatistics;

			/// <summary>
			/// Initializes a new AggregatingSummaryStatistics with the specified
			/// aggregate statistics object
			/// </summary>
			/// <param name="aggregateStatistics"> a {@code SummaryStatistics} into which
			///      values added to this statistics object should be aggregated </param>
			public AggregatingSummaryStatistics(SummaryStatistics aggregateStatistics)
			{
				this.aggregateStatistics = aggregateStatistics;
			}

			/// <summary>
			/// {@inheritDoc}.  This version adds the provided value to the configured
			/// aggregate after adding it to these statistics.
			/// </summary>
			/// <seealso cref= SummaryStatistics#addValue(double) </seealso>
			public override void addValue(double value)
			{
				base.addValue(value);
				lock (aggregateStatistics)
				{
					aggregateStatistics.addValue(value);
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
				if (@object is AggregatingSummaryStatistics == false)
				{
					return false;
				}
				AggregatingSummaryStatistics stat = (AggregatingSummaryStatistics)@object;
				return base.Equals(stat) && aggregateStatistics.Equals(stat.aggregateStatistics);
			}

			/// <summary>
			/// Returns hash code based on values of statistics </summary>
			/// <returns> hash code </returns>
			public override int GetHashCode()
			{
				return 123 + base.GetHashCode() + aggregateStatistics.GetHashCode();
			}
		}
	}

}