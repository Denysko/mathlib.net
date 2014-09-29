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

	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Implementation of
	/// <seealso cref="mathlib.stat.descriptive.SummaryStatistics"/> that
	/// is safe to use in a multithreaded environment.  Multiple threads can safely
	/// operate on a single instance without causing runtime exceptions due to race
	/// conditions.  In effect, this implementation makes modification and access
	/// methods atomic operations for a single instance.  That is to say, as one
	/// thread is computing a statistic from the instance, no other thread can modify
	/// the instance nor compute another statistic.
	/// 
	/// @since 1.2
	/// @version $Id: SynchronizedSummaryStatistics.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class SynchronizedSummaryStatistics : SummaryStatistics
	{

		/// <summary>
		/// Serialization UID </summary>
		private const long serialVersionUID = 1909861009042253704L;

		/// <summary>
		/// Construct a SynchronizedSummaryStatistics instance
		/// </summary>
		public SynchronizedSummaryStatistics() : base()
		{
		}

		/// <summary>
		/// A copy constructor. Creates a deep-copy of the {@code original}.
		/// </summary>
		/// <param name="original"> the {@code SynchronizedSummaryStatistics} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SynchronizedSummaryStatistics(SynchronizedSummaryStatistics original) throws mathlib.exception.NullArgumentException
		public SynchronizedSummaryStatistics(SynchronizedSummaryStatistics original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StatisticalSummary Summary
		{
			get
			{
				lock (this)
				{
					return base.Summary;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void addValue(double value)
		{
			lock (this)
			{
				base.addValue(value);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override long N
		{
			get
			{
				lock (this)
				{
					return base.N;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Sum
		{
			get
			{
				lock (this)
				{
					return base.Sum;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Sumsq
		{
			get
			{
				lock (this)
				{
					return base.Sumsq;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Mean
		{
			get
			{
				lock (this)
				{
					return base.Mean;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double StandardDeviation
		{
			get
			{
				lock (this)
				{
					return base.StandardDeviation;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Variance
		{
			get
			{
				lock (this)
				{
					return base.Variance;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double PopulationVariance
		{
			get
			{
				lock (this)
				{
					return base.PopulationVariance;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Max
		{
			get
			{
				lock (this)
				{
					return base.Max;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double Min
		{
			get
			{
				lock (this)
				{
					return base.Min;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double GeometricMean
		{
			get
			{
				lock (this)
				{
					return base.GeometricMean;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override string ToString()
		{
			lock (this)
			{
				return base.ToString();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void clear()
		{
			lock (this)
			{
				base.clear();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override bool Equals(object @object)
		{
			lock (this)
			{
				return base.Equals(@object);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int GetHashCode()
		{
			lock (this)
			{
				return base.GetHashCode();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic SumImpl
		{
			get
			{
				lock (this)
				{
					return base.SumImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.SumImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setSumImpl(StorelessUnivariateStatistic sumImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic SumsqImpl
		{
			get
			{
				lock (this)
				{
					return base.SumsqImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.SumsqImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setSumsqImpl(StorelessUnivariateStatistic sumsqImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic MinImpl
		{
			get
			{
				lock (this)
				{
					return base.MinImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.MinImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setMinImpl(StorelessUnivariateStatistic minImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic MaxImpl
		{
			get
			{
				lock (this)
				{
					return base.MaxImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.MaxImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setMaxImpl(StorelessUnivariateStatistic maxImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic SumLogImpl
		{
			get
			{
				lock (this)
				{
					return base.SumLogImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.SumLogImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setSumLogImpl(StorelessUnivariateStatistic sumLogImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic GeoMeanImpl
		{
			get
			{
				lock (this)
				{
					return base.GeoMeanImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.GeoMeanImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setGeoMeanImpl(StorelessUnivariateStatistic geoMeanImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic MeanImpl
		{
			get
			{
				lock (this)
				{
					return base.MeanImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.MeanImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setMeanImpl(StorelessUnivariateStatistic meanImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic VarianceImpl
		{
			get
			{
				lock (this)
				{
					return base.VarianceImpl;
				}
			}
			set
			{
				lock (this)
				{
					base.VarianceImpl = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setVarianceImpl(StorelessUnivariateStatistic varianceImpl) throws mathlib.exception.MathIllegalStateException

		/// <summary>
		/// Returns a copy of this SynchronizedSummaryStatistics instance with the
		/// same internal state.
		/// </summary>
		/// <returns> a copy of this </returns>
		public override SynchronizedSummaryStatistics copy()
		{
			lock (this)
			{
				SynchronizedSummaryStatistics result = new SynchronizedSummaryStatistics();
				// No try-catch or advertised exception because arguments are guaranteed non-null
				copy(this, result);
				return result;
			}
		}

		/// <summary>
		/// Copies source to dest.
		/// <p>Neither source nor dest can be null.</p>
		/// <p>Acquires synchronization lock on source, then dest before copying.</p>
		/// </summary>
		/// <param name="source"> SynchronizedSummaryStatistics to copy </param>
		/// <param name="dest"> SynchronizedSummaryStatistics to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(SynchronizedSummaryStatistics source, SynchronizedSummaryStatistics dest) throws mathlib.exception.NullArgumentException
		public static void copy(SynchronizedSummaryStatistics source, SynchronizedSummaryStatistics dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			lock (source)
			{
				lock (dest)
				{
					SummaryStatistics.copy(source, dest);
				}
			}
		}

	}

}