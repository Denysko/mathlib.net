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

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;

	/// <summary>
	/// Implementation of
	/// <seealso cref="org.apache.commons.math3.stat.descriptive.MultivariateSummaryStatistics"/> that
	/// is safe to use in a multithreaded environment.  Multiple threads can safely
	/// operate on a single instance without causing runtime exceptions due to race
	/// conditions.  In effect, this implementation makes modification and access
	/// methods atomic operations for a single instance.  That is to say, as one
	/// thread is computing a statistic from the instance, no other thread can modify
	/// the instance nor compute another statistic.
	/// @since 1.2
	/// @version $Id: SynchronizedMultivariateSummaryStatistics.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class SynchronizedMultivariateSummaryStatistics : MultivariateSummaryStatistics
	{

		/// <summary>
		/// Serialization UID </summary>
		private const long serialVersionUID = 7099834153347155363L;

		/// <summary>
		/// Construct a SynchronizedMultivariateSummaryStatistics instance </summary>
		/// <param name="k"> dimension of the data </param>
		/// <param name="isCovarianceBiasCorrected"> if true, the unbiased sample
		/// covariance is computed, otherwise the biased population covariance
		/// is computed </param>
		public SynchronizedMultivariateSummaryStatistics(int k, bool isCovarianceBiasCorrected) : base(k, isCovarianceBiasCorrected)
		{
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void addValue(double[] value) throws org.apache.commons.math3.exception.DimensionMismatchException
		public override void addValue(double[] value)
		{
			lock (this)
			{
			  base.addValue(value);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int Dimension
		{
			get
			{
				lock (this)
				{
					return base.Dimension;
				}
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
		public override double[] Sum
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
		public override double[] SumSq
		{
			get
			{
				lock (this)
				{
					return base.SumSq;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double[] SumLog
		{
			get
			{
				lock (this)
				{
					return base.SumLog;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double[] Mean
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
		public override double[] StandardDeviation
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
		public override RealMatrix Covariance
		{
			get
			{
				lock (this)
				{
					return base.Covariance;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double[] Max
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
		public override double[] Min
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
		public override double[] GeometricMean
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
		public override StorelessUnivariateStatistic[] SumImpl
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
//ORIGINAL LINE: @Override public synchronized void setSumImpl(StorelessUnivariateStatistic[] sumImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic[] SumsqImpl
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
//ORIGINAL LINE: @Override public synchronized void setSumsqImpl(StorelessUnivariateStatistic[] sumsqImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic[] MinImpl
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
//ORIGINAL LINE: @Override public synchronized void setMinImpl(StorelessUnivariateStatistic[] minImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic[] MaxImpl
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
//ORIGINAL LINE: @Override public synchronized void setMaxImpl(StorelessUnivariateStatistic[] maxImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic[] SumLogImpl
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
//ORIGINAL LINE: @Override public synchronized void setSumLogImpl(StorelessUnivariateStatistic[] sumLogImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic[] GeoMeanImpl
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
//ORIGINAL LINE: @Override public synchronized void setGeoMeanImpl(StorelessUnivariateStatistic[] geoMeanImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override StorelessUnivariateStatistic[] MeanImpl
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
//ORIGINAL LINE: @Override public synchronized void setMeanImpl(StorelessUnivariateStatistic[] meanImpl) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MathIllegalStateException
	}

}