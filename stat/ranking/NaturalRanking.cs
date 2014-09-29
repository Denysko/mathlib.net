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

namespace mathlib.stat.ranking
{


	using MathInternalError = mathlib.exception.MathInternalError;
	using NotANumberException = mathlib.exception.NotANumberException;
	using RandomDataGenerator = mathlib.random.RandomDataGenerator;
	using RandomGenerator = mathlib.random.RandomGenerator;
	using FastMath = mathlib.util.FastMath;


	/// <summary>
	/// <p> Ranking based on the natural ordering on doubles.</p>
	/// <p>NaNs are treated according to the configured <seealso cref="NaNStrategy"/> and ties
	/// are handled using the selected <seealso cref="TiesStrategy"/>.
	/// Configuration settings are supplied in optional constructor arguments.
	/// Defaults are <seealso cref="NaNStrategy#FAILED"/> and <seealso cref="TiesStrategy#AVERAGE"/>,
	/// respectively. When using <seealso cref="TiesStrategy#RANDOM"/>, a
	/// <seealso cref="RandomGenerator"/> may be supplied as a constructor argument.</p>
	/// <p>Examples:
	/// <table border="1" cellpadding="3">
	/// <tr><th colspan="3">
	/// Input data: (20, 17, 30, 42.3, 17, 50, Double.NaN, Double.NEGATIVE_INFINITY, 17)
	/// </th></tr>
	/// <tr><th>NaNStrategy</th><th>TiesStrategy</th>
	/// <th><code>rank(data)</code></th>
	/// <tr>
	/// <td>default (NaNs maximal)</td>
	/// <td>default (ties averaged)</td>
	/// <td>(5, 3, 6, 7, 3, 8, 9, 1, 3)</td></tr>
	/// <tr>
	/// <td>default (NaNs maximal)</td>
	/// <td>MINIMUM</td>
	/// <td>(5, 2, 6, 7, 2, 8, 9, 1, 2)</td></tr>
	/// <tr>
	/// <td>MINIMAL</td>
	/// <td>default (ties averaged)</td>
	/// <td>(6, 4, 7, 8, 4, 9, 1.5, 1.5, 4)</td></tr>
	/// <tr>
	/// <td>REMOVED</td>
	/// <td>SEQUENTIAL</td>
	/// <td>(5, 2, 6, 7, 3, 8, 1, 4)</td></tr>
	/// <tr>
	/// <td>MINIMAL</td>
	/// <td>MAXIMUM</td>
	/// <td>(6, 5, 7, 8, 5, 9, 2, 2, 5)</td></tr></table></p>
	/// 
	/// @since 2.0
	/// @version $Id: NaturalRanking.java 1454897 2013-03-10 19:02:54Z luc $
	/// </summary>
	public class NaturalRanking : RankingAlgorithm
	{

		/// <summary>
		/// default NaN strategy </summary>
		public const NaNStrategy DEFAULT_NAN_STRATEGY = NaNStrategy.FAILED;

		/// <summary>
		/// default ties strategy </summary>
		public const TiesStrategy DEFAULT_TIES_STRATEGY = TiesStrategy.AVERAGE;

		/// <summary>
		/// NaN strategy - defaults to NaNs maximal </summary>
		private readonly NaNStrategy nanStrategy;

		/// <summary>
		/// Ties strategy - defaults to ties averaged </summary>
		private readonly TiesStrategy tiesStrategy;

		/// <summary>
		/// Source of random data - used only when ties strategy is RANDOM </summary>
		private readonly RandomDataGenerator randomData;

		/// <summary>
		/// Create a NaturalRanking with default strategies for handling ties and NaNs.
		/// </summary>
		public NaturalRanking() : base()
		{
			tiesStrategy = DEFAULT_TIES_STRATEGY;
			nanStrategy = DEFAULT_NAN_STRATEGY;
			randomData = null;
		}

		/// <summary>
		/// Create a NaturalRanking with the given TiesStrategy.
		/// </summary>
		/// <param name="tiesStrategy"> the TiesStrategy to use </param>
		public NaturalRanking(TiesStrategy tiesStrategy) : base()
		{
			this.tiesStrategy = tiesStrategy;
			nanStrategy = DEFAULT_NAN_STRATEGY;
			randomData = new RandomDataGenerator();
		}

		/// <summary>
		/// Create a NaturalRanking with the given NaNStrategy.
		/// </summary>
		/// <param name="nanStrategy"> the NaNStrategy to use </param>
		public NaturalRanking(NaNStrategy nanStrategy) : base()
		{
			this.nanStrategy = nanStrategy;
			tiesStrategy = DEFAULT_TIES_STRATEGY;
			randomData = null;
		}

		/// <summary>
		/// Create a NaturalRanking with the given NaNStrategy and TiesStrategy.
		/// </summary>
		/// <param name="nanStrategy"> NaNStrategy to use </param>
		/// <param name="tiesStrategy"> TiesStrategy to use </param>
		public NaturalRanking(NaNStrategy nanStrategy, TiesStrategy tiesStrategy) : base()
		{
			this.nanStrategy = nanStrategy;
			this.tiesStrategy = tiesStrategy;
			randomData = new RandomDataGenerator();
		}

		/// <summary>
		/// Create a NaturalRanking with TiesStrategy.RANDOM and the given
		/// RandomGenerator as the source of random data.
		/// </summary>
		/// <param name="randomGenerator"> source of random data </param>
		public NaturalRanking(RandomGenerator randomGenerator) : base()
		{
			this.tiesStrategy = TiesStrategy.RANDOM;
			nanStrategy = DEFAULT_NAN_STRATEGY;
			randomData = new RandomDataGenerator(randomGenerator);
		}


		/// <summary>
		/// Create a NaturalRanking with the given NaNStrategy, TiesStrategy.RANDOM
		/// and the given source of random data.
		/// </summary>
		/// <param name="nanStrategy"> NaNStrategy to use </param>
		/// <param name="randomGenerator"> source of random data </param>
		public NaturalRanking(NaNStrategy nanStrategy, RandomGenerator randomGenerator) : base()
		{
			this.nanStrategy = nanStrategy;
			this.tiesStrategy = TiesStrategy.RANDOM;
			randomData = new RandomDataGenerator(randomGenerator);
		}

		/// <summary>
		/// Return the NaNStrategy
		/// </summary>
		/// <returns> returns the NaNStrategy </returns>
		public virtual NaNStrategy NanStrategy
		{
			get
			{
				return nanStrategy;
			}
		}

		/// <summary>
		/// Return the TiesStrategy
		/// </summary>
		/// <returns> the TiesStrategy </returns>
		public virtual TiesStrategy TiesStrategy
		{
			get
			{
				return tiesStrategy;
			}
		}

		/// <summary>
		/// Rank <code>data</code> using the natural ordering on Doubles, with
		/// NaN values handled according to <code>nanStrategy</code> and ties
		/// resolved using <code>tiesStrategy.</code>
		/// </summary>
		/// <param name="data"> array to be ranked </param>
		/// <returns> array of ranks </returns>
		/// <exception cref="NotANumberException"> if the selected <seealso cref="NaNStrategy"/> is {@code FAILED}
		/// and a <seealso cref="Double#NaN"/> is encountered in the input data </exception>
		public virtual double[] rank(double[] data)
		{

			// Array recording initial positions of data to be ranked
			IntDoublePair[] ranks = new IntDoublePair[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				ranks[i] = new IntDoublePair(data[i], i);
			}

			// Recode, remove or record positions of NaNs
			IList<int?> nanPositions = null;
			switch (nanStrategy)
			{
				case mathlib.stat.ranking.NaNStrategy.MAXIMAL: // Replace NaNs with +INFs
					recodeNaNs(ranks, double.PositiveInfinity);
					break;
				case mathlib.stat.ranking.NaNStrategy.MINIMAL: // Replace NaNs with -INFs
					recodeNaNs(ranks, double.NegativeInfinity);
					break;
				case mathlib.stat.ranking.NaNStrategy.REMOVED: // Drop NaNs from data
					ranks = removeNaNs(ranks);
					break;
				case mathlib.stat.ranking.NaNStrategy.FIXED: // Record positions of NaNs
					nanPositions = getNanPositions(ranks);
					break;
				case mathlib.stat.ranking.NaNStrategy.FAILED:
					nanPositions = getNanPositions(ranks);
					if (nanPositions.Count > 0)
					{
						throw new NotANumberException();
					}
					break;
				default: // this should not happen unless NaNStrategy enum is changed
					throw new MathInternalError();
			}

			// Sort the IntDoublePairs
			Arrays.sort(ranks);

			// Walk the sorted array, filling output array using sorted positions,
			// resolving ties as we go
			double[] @out = new double[ranks.Length];
			int pos = 1; // position in sorted array
			@out[ranks[0].Position] = pos;
			IList<int?> tiesTrace = new List<int?>();
			tiesTrace.Add(ranks[0].Position);
			for (int i = 1; i < ranks.Length; i++)
			{
				if (ranks[i].Value.CompareTo(ranks[i - 1].Value) > 0)
				{
					// tie sequence has ended (or had length 1)
					pos = i + 1;
					if (tiesTrace.Count > 1) // if seq is nontrivial, resolve
					{
						resolveTie(@out, tiesTrace);
					}
					tiesTrace = new List<int?>();
					tiesTrace.Add(ranks[i].Position);
				}
				else
				{
					// tie sequence continues
					tiesTrace.Add(ranks[i].Position);
				}
				@out[ranks[i].Position] = pos;
			}
			if (tiesTrace.Count > 1) // handle tie sequence at end
			{
				resolveTie(@out, tiesTrace);
			}
			if (nanStrategy == NaNStrategy.FIXED)
			{
				restoreNaNs(@out, nanPositions);
			}
			return @out;
		}

		/// <summary>
		/// Returns an array that is a copy of the input array with IntDoublePairs
		/// having NaN values removed.
		/// </summary>
		/// <param name="ranks"> input array </param>
		/// <returns> array with NaN-valued entries removed </returns>
		private IntDoublePair[] removeNaNs(IntDoublePair[] ranks)
		{
			if (!containsNaNs(ranks))
			{
				return ranks;
			}
			IntDoublePair[] outRanks = new IntDoublePair[ranks.Length];
			int j = 0;
			for (int i = 0; i < ranks.Length; i++)
			{
				if (double.IsNaN(ranks[i].Value))
				{
					// drop, but adjust original ranks of later elements
					for (int k = i + 1; k < ranks.Length; k++)
					{
						ranks[k] = new IntDoublePair(ranks[k].Value, ranks[k].Position - 1);
					}
				}
				else
				{
					outRanks[j] = new IntDoublePair(ranks[i].Value, ranks[i].Position);
					j++;
				}
			}
			IntDoublePair[] returnRanks = new IntDoublePair[j];
			Array.Copy(outRanks, 0, returnRanks, 0, j);
			return returnRanks;
		}

		/// <summary>
		/// Recodes NaN values to the given value.
		/// </summary>
		/// <param name="ranks"> array to recode </param>
		/// <param name="value"> the value to replace NaNs with </param>
		private void recodeNaNs(IntDoublePair[] ranks, double value)
		{
			for (int i = 0; i < ranks.Length; i++)
			{
				if (double.IsNaN(ranks[i].Value))
				{
					ranks[i] = new IntDoublePair(value, ranks[i].Position);
				}
			}
		}

		/// <summary>
		/// Checks for presence of NaNs in <code>ranks.</code>
		/// </summary>
		/// <param name="ranks"> array to be searched for NaNs </param>
		/// <returns> true iff ranks contains one or more NaNs </returns>
		private bool containsNaNs(IntDoublePair[] ranks)
		{
			for (int i = 0; i < ranks.Length; i++)
			{
				if (double.IsNaN(ranks[i].Value))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Resolve a sequence of ties, using the configured <seealso cref="TiesStrategy"/>.
		/// The input <code>ranks</code> array is expected to take the same value
		/// for all indices in <code>tiesTrace</code>.  The common value is recoded
		/// according to the tiesStrategy. For example, if ranks = <5,8,2,6,2,7,1,2>,
		/// tiesTrace = <2,4,7> and tiesStrategy is MINIMUM, ranks will be unchanged.
		/// The same array and trace with tiesStrategy AVERAGE will come out
		/// <5,8,3,6,3,7,1,3>.
		/// </summary>
		/// <param name="ranks"> array of ranks </param>
		/// <param name="tiesTrace"> list of indices where <code>ranks</code> is constant
		/// -- that is, for any i and j in TiesTrace, <code> ranks[i] == ranks[j]
		/// </code> </param>
		private void resolveTie(double[] ranks, IList<int?> tiesTrace)
		{

			// constant value of ranks over tiesTrace
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c = ranks[tiesTrace.get(0)];
			double c = ranks[tiesTrace[0]];

			// length of sequence of tied ranks
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = tiesTrace.size();
			int length = tiesTrace.Count;

			switch (tiesStrategy)
			{
				case mathlib.stat.ranking.TiesStrategy.AVERAGE: // Replace ranks with average
					fill(ranks, tiesTrace, (2 * c + length - 1) / 2d);
					break;
				case mathlib.stat.ranking.TiesStrategy.MAXIMUM: // Replace ranks with maximum values
					fill(ranks, tiesTrace, c + length - 1);
					break;
				case mathlib.stat.ranking.TiesStrategy.MINIMUM: // Replace ties with minimum
					fill(ranks, tiesTrace, c);
					break;
				case mathlib.stat.ranking.TiesStrategy.RANDOM: // Fill with random integral values in [c, c + length - 1]
					IEnumerator<int?> iterator = tiesTrace.GetEnumerator();
					long f = FastMath.round(c);
					while (iterator.MoveNext())
					{
						// No advertised exception because args are guaranteed valid
						ranks[iterator.Current] = randomData.nextLong(f, f + length - 1);
					}
					break;
				case mathlib.stat.ranking.TiesStrategy.SEQUENTIAL: // Fill sequentially from c to c + length - 1
					// walk and fill
					iterator = tiesTrace.GetEnumerator();
					f = FastMath.round(c);
					int i = 0;
					while (iterator.MoveNext())
					{
						ranks[iterator.Current] = f + i++;
					}
					break;
				default: // this should not happen unless TiesStrategy enum is changed
					throw new MathInternalError();
			}
		}

		/// <summary>
		/// Sets<code>data[i] = value</code> for each i in <code>tiesTrace.</code>
		/// </summary>
		/// <param name="data"> array to modify </param>
		/// <param name="tiesTrace"> list of index values to set </param>
		/// <param name="value"> value to set </param>
		private void fill(double[] data, IList<int?> tiesTrace, double value)
		{
			IEnumerator<int?> iterator = tiesTrace.GetEnumerator();
			while (iterator.MoveNext())
			{
				data[iterator.Current] = value;
			}
		}

		/// <summary>
		/// Set <code>ranks[i] = Double.NaN</code> for each i in <code>nanPositions.</code>
		/// </summary>
		/// <param name="ranks"> array to modify </param>
		/// <param name="nanPositions"> list of index values to set to <code>Double.NaN</code> </param>
		private void restoreNaNs(double[] ranks, IList<int?> nanPositions)
		{
			if (nanPositions.Count == 0)
			{
				return;
			}
			IEnumerator<int?> iterator = nanPositions.GetEnumerator();
			while (iterator.MoveNext())
			{
				ranks[(int)iterator.Current] = double.NaN;
			}

		}

		/// <summary>
		/// Returns a list of indexes where <code>ranks</code> is <code>NaN.</code>
		/// </summary>
		/// <param name="ranks"> array to search for <code>NaNs</code> </param>
		/// <returns> list of indexes i such that <code>ranks[i] = NaN</code> </returns>
		private IList<int?> getNanPositions(IntDoublePair[] ranks)
		{
			List<int?> @out = new List<int?>();
			for (int i = 0; i < ranks.Length; i++)
			{
				if (double.IsNaN(ranks[i].Value))
				{
					@out.Add(Convert.ToInt32(i));
				}
			}
			return @out;
		}

		/// <summary>
		/// Represents the position of a double value in an ordering.
		/// Comparable interface is implemented so Arrays.sort can be used
		/// to sort an array of IntDoublePairs by value.  Note that the
		/// implicitly defined natural ordering is NOT consistent with equals.
		/// </summary>
		private class IntDoublePair : IComparable<IntDoublePair>
		{

			/// <summary>
			/// Value of the pair </summary>
			internal readonly double value;

			/// <summary>
			/// Original position of the pair </summary>
			internal readonly int position;

			/// <summary>
			/// Construct an IntDoublePair with the given value and position. </summary>
			/// <param name="value"> the value of the pair </param>
			/// <param name="position"> the original position </param>
			public IntDoublePair(double value, int position)
			{
				this.value = value;
				this.position = position;
			}

			/// <summary>
			/// Compare this IntDoublePair to another pair.
			/// Only the <strong>values</strong> are compared.
			/// </summary>
			/// <param name="other"> the other pair to compare this to </param>
			/// <returns> result of <code>Double.compare(value, other.value)</code> </returns>
			public virtual int CompareTo(IntDoublePair other)
			{
				return value.CompareTo(other.value);
			}

			// N.B. equals() and hashCode() are not implemented; see MATH-610 for discussion.

			/// <summary>
			/// Returns the value of the pair. </summary>
			/// <returns> value </returns>
			public virtual double Value
			{
				get
				{
					return value;
				}
			}

			/// <summary>
			/// Returns the original position of the pair. </summary>
			/// <returns> position </returns>
			public virtual int Position
			{
				get
				{
					return position;
				}
			}
		}
	}

}