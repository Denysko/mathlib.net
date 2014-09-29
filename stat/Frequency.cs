using System;
using System.Collections.Generic;
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
namespace mathlib.stat
{


	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Maintains a frequency distribution.
	/// <p>
	/// Accepts int, long, char or Comparable values.  New values added must be
	/// comparable to those that have been added, otherwise the add method will
	/// throw an IllegalArgumentException.</p>
	/// <p>
	/// Integer values (int, long, Integer, Long) are not distinguished by type --
	/// i.e. <code>addValue(Long.valueOf(2)), addValue(2), addValue(2l)</code> all have
	/// the same effect (similarly for arguments to <code>getCount,</code> etc.).</p>
	/// <p>NOTE: byte and short values will be implicitly converted to int values
	/// by the compiler, thus there are no explicit overloaded methods for these
	/// primitive types.</p>
	/// <p>
	/// char values are converted by <code>addValue</code> to Character instances.
	/// As such, these values are not comparable to integral values, so attempts
	/// to combine integral types with chars in a frequency distribution will fail.
	/// </p>
	/// <p>
	/// Float is not coerced to Double.
	/// Since they are not Comparable with each other the user must do any necessary coercion.
	/// Float.NaN and Double.NaN are not treated specially; they may occur in input and will
	/// occur in output if appropriate.
	/// </b>
	/// <p>
	/// The values are ordered using the default (natural order), unless a
	/// <code>Comparator</code> is supplied in the constructor.</p>
	/// 
	/// @version $Id: Frequency.java 1519820 2013-09-03 19:58:03Z tn $
	/// </summary>
	[Serializable]
	public class Frequency
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -3845586908418844111L;

		/// <summary>
		/// underlying collection </summary>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private final java.util.TreeMap<Comparable<?>, Long> freqTable;
		private readonly SortedDictionary<IComparable<?>, long?> freqTable;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Frequency()
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: freqTable = new java.util.TreeMap<Comparable<?>, Long>();
			freqTable = new SortedDictionary<IComparable<?>, long?>();
		}

		/// <summary>
		/// Constructor allowing values Comparator to be specified.
		/// </summary>
		/// <param name="comparator"> Comparator used to order values </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public Frequency(java.util.Comparator<?> comparator)
		public Frequency<T1>(IComparer<T1> comparator) // TODO is the cast OK?
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: freqTable = new java.util.TreeMap<Comparable<?>, Long>((java.util.Comparator<? base Comparable<?>>) comparator);
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			freqTable = new SortedDictionary<IComparable<?>, long?>((IComparer<?>) comparator);
		}

		/// <summary>
		/// Return a string representation of this frequency distribution.
		/// </summary>
		/// <returns> a string representation. </returns>
		public override string ToString()
		{
			NumberFormat nf = NumberFormat.PercentInstance;
			StringBuilder outBuffer = new StringBuilder();
			outBuffer.Append("Value \t Freq. \t Pct. \t Cum Pct. \n");
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<Comparable<?>> iter = freqTable.keySet().iterator();
			IEnumerator<IComparable<?>> iter = freqTable.Keys.GetEnumerator();
			while (iter.MoveNext())
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Comparable<?> value = iter.Current;
				IComparable<?> value = iter.Current;
				outBuffer.Append(value);
				outBuffer.Append('\t');
				outBuffer.Append(getCount(value));
				outBuffer.Append('\t');
				outBuffer.Append(nf.format(getPct(value)));
				outBuffer.Append('\t');
				outBuffer.Append(nf.format(getCumPct(value)));
				outBuffer.Append('\n');
			}
			return outBuffer.ToString();
		}

		/// <summary>
		/// Adds 1 to the frequency count for v.
		/// <p>
		/// If other objects have already been added to this Frequency, v must
		/// be comparable to those that have already been added.
		/// </p>
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <exception cref="MathIllegalArgumentException"> if <code>v</code> is not comparable with previous entries </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addValue(Comparable<?> v) throws mathlib.exception.MathIllegalArgumentException
		public virtual void addValue<T1>(IComparable<T1> v)
		{
			incrementValue(v, 1);
		}

		/// <summary>
		/// Adds 1 to the frequency count for v.
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <exception cref="MathIllegalArgumentException"> if the table contains entries not
		/// comparable to Long </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addValue(int v) throws mathlib.exception.MathIllegalArgumentException
		public virtual void addValue(int v)
		{
			addValue(Convert.ToInt64(v));
		}

		/// <summary>
		/// Adds 1 to the frequency count for v.
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <exception cref="MathIllegalArgumentException"> if the table contains entries not
		/// comparable to Long </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addValue(long v) throws mathlib.exception.MathIllegalArgumentException
		public virtual void addValue(long v)
		{
			addValue(Convert.ToInt64(v));
		}

		/// <summary>
		/// Adds 1 to the frequency count for v.
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <exception cref="MathIllegalArgumentException"> if the table contains entries not
		/// comparable to Char </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void addValue(char v) throws mathlib.exception.MathIllegalArgumentException
		public virtual void addValue(char v)
		{
			addValue(Convert.ToChar(v));
		}

		/// <summary>
		/// Increments the frequency count for v.
		/// <p>
		/// If other objects have already been added to this Frequency, v must
		/// be comparable to those that have already been added.
		/// </p>
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <param name="increment"> the amount by which the value should be incremented </param>
		/// <exception cref="MathIllegalArgumentException"> if <code>v</code> is not comparable with previous entries
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementValue(Comparable<?> v, long increment) throws mathlib.exception.MathIllegalArgumentException
		public virtual void incrementValue<T1>(IComparable<T1> v, long increment)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Comparable<?> obj = v;
			IComparable<?> obj = v;
			if (v is int?)
			{
				obj = Convert.ToInt64((long)((int?) v));
			}
			try
			{
				long? count = freqTable[obj];
				if (count == null)
				{
					freqTable[obj] = Convert.ToInt64(increment);
				}
				else
				{
					freqTable[obj] = Convert.ToInt64((long)count + increment);
				}
			}
			catch (System.InvalidCastException ex)
			{
				//TreeMap will throw ClassCastException if v is not comparable
				throw new MathIllegalArgumentException(LocalizedFormats.INSTANCES_NOT_COMPARABLE_TO_EXISTING_VALUES, v.GetType().Name);
			}
		}

		/// <summary>
		/// Increments the frequency count for v.
		/// <p>
		/// If other objects have already been added to this Frequency, v must
		/// be comparable to those that have already been added.
		/// </p>
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <param name="increment"> the amount by which the value should be incremented </param>
		/// <exception cref="MathIllegalArgumentException"> if the table contains entries not
		/// comparable to Long
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementValue(int v, long increment) throws mathlib.exception.MathIllegalArgumentException
		public virtual void incrementValue(int v, long increment)
		{
			incrementValue(Convert.ToInt64(v), increment);
		}

		/// <summary>
		/// Increments the frequency count for v.
		/// <p>
		/// If other objects have already been added to this Frequency, v must
		/// be comparable to those that have already been added.
		/// </p>
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <param name="increment"> the amount by which the value should be incremented </param>
		/// <exception cref="MathIllegalArgumentException"> if the table contains entries not
		/// comparable to Long
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementValue(long v, long increment) throws mathlib.exception.MathIllegalArgumentException
		public virtual void incrementValue(long v, long increment)
		{
			incrementValue(Convert.ToInt64(v), increment);
		}

		/// <summary>
		/// Increments the frequency count for v.
		/// <p>
		/// If other objects have already been added to this Frequency, v must
		/// be comparable to those that have already been added.
		/// </p>
		/// </summary>
		/// <param name="v"> the value to add. </param>
		/// <param name="increment"> the amount by which the value should be incremented </param>
		/// <exception cref="MathIllegalArgumentException"> if the table contains entries not
		/// comparable to Char
		/// @since 3.3 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void incrementValue(char v, long increment) throws mathlib.exception.MathIllegalArgumentException
		public virtual void incrementValue(char v, long increment)
		{
			incrementValue(Convert.ToChar(v), increment);
		}

		/// <summary>
		/// Clears the frequency table </summary>
		public virtual void clear()
		{
			freqTable.Clear();
		}

		/// <summary>
		/// Returns an Iterator over the set of values that have been added.
		/// <p>
		/// If added values are integral (i.e., integers, longs, Integers, or Longs),
		/// they are converted to Longs when they are added, so the objects returned
		/// by the Iterator will in this case be Longs.</p>
		/// </summary>
		/// <returns> values Iterator </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Iterator<Comparable<?>> valuesIterator()
		public virtual IEnumerator<IComparable<?>> valuesIterator()
		{
			return freqTable.Keys.GetEnumerator();
		}

		/// <summary>
		/// Return an Iterator over the set of keys and values that have been added.
		/// Using the entry set to iterate is more efficient in the case where you
		/// need to access respective counts as well as values, since it doesn't
		/// require a "get" for every key...the value is provided in the Map.Entry.
		/// <p>
		/// If added values are integral (i.e., integers, longs, Integers, or Longs),
		/// they are converted to Longs when they are added, so the values of the
		/// map entries returned by the Iterator will in this case be Longs.</p>
		/// </summary>
		/// <returns> entry set Iterator
		/// @since 3.1 </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.Iterator<java.util.Map.Entry<Comparable<?>, Long>> entrySetIterator()
		public virtual IEnumerator<KeyValuePair<IComparable<?>, long?>> entrySetIterator()
		{
			return freqTable.GetEnumerator();
		}

		//-------------------------------------------------------------------------

		/// <summary>
		/// Returns the sum of all frequencies.
		/// </summary>
		/// <returns> the total frequency count. </returns>
		public virtual long SumFreq
		{
			get
			{
				long result = 0;
				IEnumerator<long?> iterator = freqTable.Values.GetEnumerator();
				while (iterator.MoveNext())
				{
					result += (long)iterator.Current;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns the number of values equal to v.
		/// Returns 0 if the value is not comparable.
		/// </summary>
		/// <param name="v"> the value to lookup. </param>
		/// <returns> the frequency of v. </returns>
		public virtual long getCount<T1>(IComparable<T1> v)
		{
			if (v is int?)
			{
				return getCount((long)((int?) v));
			}
			long result = 0;
			try
			{
				long? count = freqTable[v];
				if (count != null)
				{
					result = (long)count;
				}
			} // NOPMD
			catch (System.InvalidCastException ex)
			{
				// ignore and return 0 -- ClassCastException will be thrown if value is not comparable
			}
			return result;
		}

		/// <summary>
		/// Returns the number of values equal to v.
		/// </summary>
		/// <param name="v"> the value to lookup. </param>
		/// <returns> the frequency of v. </returns>
		public virtual long getCount(int v)
		{
			return getCount(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the number of values equal to v.
		/// </summary>
		/// <param name="v"> the value to lookup. </param>
		/// <returns> the frequency of v. </returns>
		public virtual long getCount(long v)
		{
			return getCount(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the number of values equal to v.
		/// </summary>
		/// <param name="v"> the value to lookup. </param>
		/// <returns> the frequency of v. </returns>
		public virtual long getCount(char v)
		{
			return getCount(Convert.ToChar(v));
		}

		/// <summary>
		/// Returns the number of values in the frequency table.
		/// </summary>
		/// <returns> the number of unique values that have been added to the frequency table. </returns>
		/// <seealso cref= #valuesIterator() </seealso>
		public virtual int UniqueCount
		{
			get
			{
				return freqTable.Keys.size();
			}
		}

		/// <summary>
		/// Returns the percentage of values that are equal to v
		/// (as a proportion between 0 and 1).
		/// <p>
		/// Returns <code>Double.NaN</code> if no values have been added.
		/// Returns 0 if at least one value has been added, but v is not comparable
		/// to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values equal to v </returns>
		public virtual double getPct<T1>(IComparable<T1> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long sumFreq = getSumFreq();
			long sumFreq = SumFreq;
			if (sumFreq == 0)
			{
				return double.NaN;
			}
			return (double) getCount(v) / (double) sumFreq;
		}

		/// <summary>
		/// Returns the percentage of values that are equal to v
		/// (as a proportion between 0 and 1).
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values equal to v </returns>
		public virtual double getPct(int v)
		{
			return getPct(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the percentage of values that are equal to v
		/// (as a proportion between 0 and 1).
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values equal to v </returns>
		public virtual double getPct(long v)
		{
			return getPct(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the percentage of values that are equal to v
		/// (as a proportion between 0 and 1).
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values equal to v </returns>
		public virtual double getPct(char v)
		{
			return getPct(Convert.ToChar(v));
		}

		//-----------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the cumulative frequency of values less than or equal to v.
		/// <p>
		/// Returns 0 if v is not comparable to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup. </param>
		/// <returns> the proportion of values equal to v </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public long getCumFreq(Comparable<?> v)
		public virtual long getCumFreq<T1>(IComparable<T1> v)
		{
			if (SumFreq == 0)
			{
				return 0;
			}
			if (v is int?)
			{
				return getCumFreq((long)((int?) v));
			}
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Comparator<Comparable<?>> c = (java.util.Comparator<Comparable<?>>) freqTable.comparator();
			IComparer<IComparable<?>> c = (IComparer<IComparable<?>>) freqTable.comparator();
			if (c == null)
			{
				c = new NaturalComparator();
			}
			long result = 0;

			try
			{
				long? value = freqTable[v];
				if (value != null)
				{
					result = (long)value;
				}
			}
			catch (System.InvalidCastException ex)
			{
				return result; // v is not comparable
			}

			if (c.Compare(v, freqTable.firstKey()) < 0)
			{
				return 0; // v is comparable, but less than first value
			}

			if (c.Compare(v, freqTable.lastKey()) >= 0)
			{
				return SumFreq; // v is comparable, but greater than the last value
			}

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: java.util.Iterator<Comparable<?>> values = valuesIterator();
			IEnumerator<IComparable<?>> values = valuesIterator();
			while (values.MoveNext())
			{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: Comparable<?> nextValue = values.Current;
				IComparable<?> nextValue = values.Current;
				if (c.Compare(v, nextValue) > 0)
				{
					result += getCount(nextValue);
				}
				else
				{
					return result;
				}
			}
			return result;
		}

		 /// <summary>
		 /// Returns the cumulative frequency of values less than or equal to v.
		 /// <p>
		 /// Returns 0 if v is not comparable to the values set.</p>
		 /// </summary>
		 /// <param name="v"> the value to lookup </param>
		 /// <returns> the proportion of values equal to v </returns>
		public virtual long getCumFreq(int v)
		{
			return getCumFreq(Convert.ToInt64(v));
		}

		 /// <summary>
		 /// Returns the cumulative frequency of values less than or equal to v.
		 /// <p>
		 /// Returns 0 if v is not comparable to the values set.</p>
		 /// </summary>
		 /// <param name="v"> the value to lookup </param>
		 /// <returns> the proportion of values equal to v </returns>
		public virtual long getCumFreq(long v)
		{
			return getCumFreq(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the cumulative frequency of values less than or equal to v.
		/// <p>
		/// Returns 0 if v is not comparable to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values equal to v </returns>
		public virtual long getCumFreq(char v)
		{
			return getCumFreq(Convert.ToChar(v));
		}

		//----------------------------------------------------------------------------------------------

		/// <summary>
		/// Returns the cumulative percentage of values less than or equal to v
		/// (as a proportion between 0 and 1).
		/// <p>
		/// Returns <code>Double.NaN</code> if no values have been added.
		/// Returns 0 if at least one value has been added, but v is not comparable
		/// to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values less than or equal to v </returns>
		public virtual double getCumPct<T1>(IComparable<T1> v)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long sumFreq = getSumFreq();
			long sumFreq = SumFreq;
			if (sumFreq == 0)
			{
				return double.NaN;
			}
			return (double) getCumFreq(v) / (double) sumFreq;
		}

		/// <summary>
		/// Returns the cumulative percentage of values less than or equal to v
		/// (as a proportion between 0 and 1).
		/// <p>
		/// Returns 0 if v is not comparable to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values less than or equal to v </returns>
		public virtual double getCumPct(int v)
		{
			return getCumPct(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the cumulative percentage of values less than or equal to v
		/// (as a proportion between 0 and 1).
		/// <p>
		/// Returns 0 if v is not comparable to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values less than or equal to v </returns>
		public virtual double getCumPct(long v)
		{
			return getCumPct(Convert.ToInt64(v));
		}

		/// <summary>
		/// Returns the cumulative percentage of values less than or equal to v
		/// (as a proportion between 0 and 1).
		/// <p>
		/// Returns 0 if v is not comparable to the values set.</p>
		/// </summary>
		/// <param name="v"> the value to lookup </param>
		/// <returns> the proportion of values less than or equal to v </returns>
		public virtual double getCumPct(char v)
		{
			return getCumPct(Convert.ToChar(v));
		}

		/// <summary>
		/// Returns the mode value(s) in comparator order.
		/// </summary>
		/// <returns> a list containing the value(s) which appear most often.
		/// @since 3.3 </returns>
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public java.util.List<Comparable<?>> getMode()
		public virtual IList<IComparable<?>> Mode
		{
			get
			{
				long mostPopular = 0; // frequencies are always positive
    
				// Get the max count first, so we avoid having to recreate the List each time
				foreach (long? l in freqTable.Values)
				{
					long frequency = (long)l;
					if (frequency > mostPopular)
					{
						mostPopular = frequency;
					}
				}
    
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: java.util.List<Comparable<?>> modeList = new java.util.ArrayList<Comparable<?>>();
				IList<IComparable<?>> modeList = new List<IComparable<?>>();
	//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
	//ORIGINAL LINE: for (java.util.Map.Entry<Comparable<?>, Long> ent : freqTable.entrySet())
				foreach (KeyValuePair<IComparable<?>, long?> ent in freqTable)
				{
					long frequency = (long)ent.Value;
					if (frequency == mostPopular)
					{
					   modeList.Add(ent.Key);
					}
				}
				return modeList;
			}
		}

		//----------------------------------------------------------------------------------------------

		/// <summary>
		/// Merge another Frequency object's counts into this instance.
		/// This Frequency's counts will be incremented (or set when not already set)
		/// by the counts represented by other.
		/// </summary>
		/// <param name="other"> the other <seealso cref="Frequency"/> object to be merged </param>
		/// <exception cref="NullArgumentException"> if {@code other} is null
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void merge(final Frequency other) throws mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void merge(Frequency other)
		{
			MathUtils.checkNotNull(other, LocalizedFormats.NULL_NOT_ALLOWED);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<java.util.Map.Entry<Comparable<?>, Long>> iter = other.entrySetIterator();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			IEnumerator<KeyValuePair<IComparable<?>, long?>> iter = other.entrySetIterator();
			while (iter.MoveNext())
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Map.Entry<Comparable<?>, Long> entry = iter.Current;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				KeyValuePair<IComparable<?>, long?> entry = iter.Current;
				incrementValue(entry.Key, (long)entry.Value);
			}
		}

		/// <summary>
		/// Merge a <seealso cref="Collection"/> of <seealso cref="Frequency"/> objects into this instance.
		/// This Frequency's counts will be incremented (or set when not already set)
		/// by the counts represented by each of the others.
		/// </summary>
		/// <param name="others"> the other <seealso cref="Frequency"/> objects to be merged </param>
		/// <exception cref="NullArgumentException"> if the collection is null
		/// @since 3.1 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void merge(final java.util.Collection<Frequency> others) throws mathlib.exception.NullArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void merge(ICollection<Frequency> others)
		{
			MathUtils.checkNotNull(others, LocalizedFormats.NULL_NOT_ALLOWED);

			foreach (Frequency freq in others)
			{
				merge(freq);
			}
		}

		//----------------------------------------------------------------------------------------------

		/// <summary>
		/// A Comparator that compares comparable objects using the
		/// natural order.  Copied from Commons Collections ComparableComparator.
		/// </summary>
		[Serializable]
		private class NaturalComparator<T> : IComparer<IComparable<T>> where T : Comparable<T>
		{

			/// <summary>
			/// Serializable version identifier </summary>
			internal const long serialVersionUID = -3852193713161395148L;

			/// <summary>
			/// Compare the two <seealso cref="Comparable Comparable"/> arguments.
			/// This method is equivalent to:
			/// <pre>((<seealso cref="Comparable Comparable"/>)o1).<seealso cref="Comparable#compareTo compareTo"/>(o2)</pre>
			/// </summary>
			/// <param name="o1"> the first object </param>
			/// <param name="o2"> the second object </param>
			/// <returns>  result of comparison </returns>
			/// <exception cref="NullPointerException"> when <i>o1</i> is <code>null</code>,
			///         or when <code>((Comparable)o1).compareTo(o2)</code> does </exception>
			/// <exception cref="ClassCastException"> when <i>o1</i> is not a <seealso cref="Comparable Comparable"/>,
			///         or when <code>((Comparable)o1).compareTo(o2)</code> does </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public int compare(Comparable<T> o1, Comparable<T> o2)
			public virtual int Compare(IComparable<T> o1, IComparable<T> o2) // cast to (T) may throw ClassCastException, see Javadoc
			{
				return o1.CompareTo((T) o2);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			const int prime = 31;
			int result = 1;
			result = prime * result + ((freqTable == null) ? 0 : freqTable.GetHashCode());
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is Frequency))
			{
				return false;
			}
			Frequency other = (Frequency) obj;
			if (freqTable == null)
			{
				if (other.freqTable != null)
				{
					return false;
				}
			}
			else if (!freqTable.Equals(other.freqTable))
			{
				return false;
			}
			return true;
		}

	}

}