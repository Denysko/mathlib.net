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
namespace org.apache.commons.math3.optim.nonlinear.scalar
{

	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using RandomVectorGenerator = org.apache.commons.math3.random.RandomVectorGenerator;
	using org.apache.commons.math3.optim;

	/// <summary>
	/// Multi-start optimizer.
	/// 
	/// This class wraps an optimizer in order to use it several times in
	/// turn with different starting points (trying to avoid being trapped
	/// in a local extremum when looking for a global one).
	/// 
	/// @version $Id: MultiStartMultivariateOptimizer.java 1435539 2013-01-19 13:27:24Z tn $
	/// @since 3.0
	/// </summary>
	public class MultiStartMultivariateOptimizer : BaseMultiStartMultivariateOptimizer<PointValuePair>
	{
		/// <summary>
		/// Underlying optimizer. </summary>
		private readonly MultivariateOptimizer optimizer;
		/// <summary>
		/// Found optima. </summary>
		private readonly IList<PointValuePair> optima = new List<PointValuePair>();

		/// <summary>
		/// Create a multi-start optimizer from a single-start optimizer.
		/// </summary>
		/// <param name="optimizer"> Single-start optimizer to wrap. </param>
		/// <param name="starts"> Number of starts to perform.
		/// If {@code starts == 1}, the result will be same as if {@code optimizer}
		/// is called directly. </param>
		/// <param name="generator"> Random vector generator to use for restarts. </param>
		/// <exception cref="NullArgumentException"> if {@code optimizer} or {@code generator}
		/// is {@code null}. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if {@code starts < 1}. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MultiStartMultivariateOptimizer(final MultivariateOptimizer optimizer, final int starts, final org.apache.commons.math3.random.RandomVectorGenerator generator) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public MultiStartMultivariateOptimizer(MultivariateOptimizer optimizer, int starts, RandomVectorGenerator generator) : base(optimizer, starts, generator)
		{
			this.optimizer = optimizer;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public override PointValuePair[] Optima
		{
			get
			{
				optima.Sort(PairComparator);
				return optima.ToArray();
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		protected internal override void store(PointValuePair optimum)
		{
			optima.Add(optimum);
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		protected internal override void clear()
		{
			optima.Clear();
		}

		/// <returns> a comparator for sorting the optima. </returns>
		private IComparer<PointValuePair> PairComparator
		{
			get
			{
				return new ComparatorAnonymousInnerClassHelper(this);
			}
		}

		private class ComparatorAnonymousInnerClassHelper : IComparer<PointValuePair>
		{
			private readonly MultiStartMultivariateOptimizer outerInstance;

			public ComparatorAnonymousInnerClassHelper(MultiStartMultivariateOptimizer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int compare(final org.apache.commons.math3.optim.PointValuePair o1, final org.apache.commons.math3.optim.PointValuePair o2)
			public virtual int Compare(PointValuePair o1, PointValuePair o2)
			{
				if (o1 == null)
				{
					return (o2 == null) ? 0 : 1;
				}
				else if (o2 == null)
				{
					return -1;
				}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v1 = o1.getValue();
				double v1 = o1.Value;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double v2 = o2.getValue();
				double v2 = o2.Value;
				return (outerInstance.optimizer.GoalType == GoalType.MINIMIZE) ? v1.CompareTo(v2) : v2.CompareTo(v1);
			}
		}
	}

}