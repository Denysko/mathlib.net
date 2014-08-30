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

	using MathIllegalArgumentException = org.apache.commons.math3.exception.MathIllegalArgumentException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Implementation of
	/// <seealso cref="org.apache.commons.math3.stat.descriptive.DescriptiveStatistics"/> that
	/// is safe to use in a multithreaded environment.  Multiple threads can safely
	/// operate on a single instance without causing runtime exceptions due to race
	/// conditions.  In effect, this implementation makes modification and access
	/// methods atomic operations for a single instance.  That is to say, as one
	/// thread is computing a statistic from the instance, no other thread can modify
	/// the instance nor compute another statistic.
	/// 
	/// @since 1.2
	/// @version $Id: SynchronizedDescriptiveStatistics.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class SynchronizedDescriptiveStatistics : DescriptiveStatistics
	{

		/// <summary>
		/// Serialization UID </summary>
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Construct an instance with infinite window
		/// </summary>
		public SynchronizedDescriptiveStatistics() : this(INFINITE_WINDOW)
		{
			// no try-catch or advertized IAE because arg is valid
		}

		/// <summary>
		/// Construct an instance with finite window </summary>
		/// <param name="window"> the finite window size. </param>
		/// <exception cref="MathIllegalArgumentException"> if window size is less than 1 but
		/// not equal to <seealso cref="#INFINITE_WINDOW"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SynchronizedDescriptiveStatistics(int window) throws org.apache.commons.math3.exception.MathIllegalArgumentException
		public SynchronizedDescriptiveStatistics(int window) : base(window)
		{
		}

		/// <summary>
		/// A copy constructor. Creates a deep-copy of the {@code original}.
		/// </summary>
		/// <param name="original"> the {@code SynchronizedDescriptiveStatistics} instance to copy </param>
		/// <exception cref="NullArgumentException"> if original is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SynchronizedDescriptiveStatistics(SynchronizedDescriptiveStatistics original) throws org.apache.commons.math3.exception.NullArgumentException
		public SynchronizedDescriptiveStatistics(SynchronizedDescriptiveStatistics original)
		{
			copy(original, this);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override void addValue(double v)
		{
			lock (this)
			{
				base.addValue(v);
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override double apply(UnivariateStatistic stat)
		{
			lock (this)
			{
				return base.apply(stat);
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
		public override double getElement(int index)
		{
			lock (this)
			{
				return base.getElement(index);
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
		public override double[] Values
		{
			get
			{
				lock (this)
				{
					return base.Values;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override int WindowSize
		{
			get
			{
				lock (this)
				{
					return base.WindowSize;
				}
			}
			set
			{
				lock (this)
				{
					base.WindowSize = value;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void setWindowSize(int windowSize) throws org.apache.commons.math3.exception.MathIllegalArgumentException

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
		/// Returns a copy of this SynchronizedDescriptiveStatistics instance with the
		/// same internal state.
		/// </summary>
		/// <returns> a copy of this </returns>
		public override SynchronizedDescriptiveStatistics copy()
		{
			lock (this)
			{
				SynchronizedDescriptiveStatistics result = new SynchronizedDescriptiveStatistics();
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
		/// <param name="source"> SynchronizedDescriptiveStatistics to copy </param>
		/// <param name="dest"> SynchronizedDescriptiveStatistics to copy to </param>
		/// <exception cref="NullArgumentException"> if either source or dest is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void copy(SynchronizedDescriptiveStatistics source, SynchronizedDescriptiveStatistics dest) throws org.apache.commons.math3.exception.NullArgumentException
		public static void copy(SynchronizedDescriptiveStatistics source, SynchronizedDescriptiveStatistics dest)
		{
			MathUtils.checkNotNull(source);
			MathUtils.checkNotNull(dest);
			lock (source)
			{
				lock (dest)
				{
					DescriptiveStatistics.copy(source, dest);
				}
			}
		}
	}

}