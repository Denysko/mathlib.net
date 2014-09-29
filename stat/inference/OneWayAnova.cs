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
namespace mathlib.stat.inference
{


	using FDistribution = mathlib.distribution.FDistribution;
	using ConvergenceException = mathlib.exception.ConvergenceException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using SummaryStatistics = mathlib.stat.descriptive.SummaryStatistics;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// Implements one-way ANOVA (analysis of variance) statistics.
	/// 
	/// <p> Tests for differences between two or more categories of univariate data
	/// (for example, the body mass index of accountants, lawyers, doctors and
	/// computer programmers).  When two categories are given, this is equivalent to
	/// the <seealso cref="mathlib.stat.inference.TTest"/>.
	/// </p><p>
	/// Uses the {@link mathlib.distribution.FDistribution
	/// commons-math F Distribution implementation} to estimate exact p-values.</p>
	/// <p>This implementation is based on a description at
	/// http://faculty.vassar.edu/lowry/ch13pt1.html</p>
	/// <pre>
	/// Abbreviations: bg = between groups,
	///                wg = within groups,
	///                ss = sum squared deviations
	/// </pre>
	/// 
	/// @since 1.2
	/// @version $Id: OneWayAnova.java 1462423 2013-03-29 07:25:18Z luc $
	/// </summary>
	public class OneWayAnova
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public OneWayAnova()
		{
		}

		/// <summary>
		/// Computes the ANOVA F-value for a collection of <code>double[]</code>
		/// arrays.
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>The categoryData <code>Collection</code> must contain
		/// <code>double[]</code> arrays.</li>
		/// <li> There must be at least two <code>double[]</code> arrays in the
		/// <code>categoryData</code> collection and each of these arrays must
		/// contain at least two values.</li></ul></p><p>
		/// This implementation computes the F statistic using the definitional
		/// formula<pre>
		///   F = msbg/mswg</pre>
		/// where<pre>
		///  msbg = between group mean square
		///  mswg = within group mean square</pre>
		/// are as defined <a href="http://faculty.vassar.edu/lowry/ch13pt1.html">
		/// here</a></p>
		/// </summary>
		/// <param name="categoryData"> <code>Collection</code> of <code>double[]</code>
		/// arrays each containing data for one category </param>
		/// <returns> Fvalue </returns>
		/// <exception cref="NullArgumentException"> if <code>categoryData</code> is <code>null</code> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the <code>categoryData</code>
		/// array is less than 2 or a contained <code>double[]</code> array does not have
		/// at least two values </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double anovaFValue(final java.util.Collection<double[]> categoryData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double anovaFValue(ICollection<double[]> categoryData)
		{

			AnovaStats a = anovaStats(categoryData);
			return a.F;

		}

		/// <summary>
		/// Computes the ANOVA P-value for a collection of <code>double[]</code>
		/// arrays.
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>The categoryData <code>Collection</code> must contain
		/// <code>double[]</code> arrays.</li>
		/// <li> There must be at least two <code>double[]</code> arrays in the
		/// <code>categoryData</code> collection and each of these arrays must
		/// contain at least two values.</li></ul></p><p>
		/// This implementation uses the
		/// {@link mathlib.distribution.FDistribution
		/// commons-math F Distribution implementation} to estimate the exact
		/// p-value, using the formula<pre>
		///   p = 1 - cumulativeProbability(F)</pre>
		/// where <code>F</code> is the F value and <code>cumulativeProbability</code>
		/// is the commons-math implementation of the F distribution.</p>
		/// </summary>
		/// <param name="categoryData"> <code>Collection</code> of <code>double[]</code>
		/// arrays each containing data for one category </param>
		/// <returns> Pvalue </returns>
		/// <exception cref="NullArgumentException"> if <code>categoryData</code> is <code>null</code> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the <code>categoryData</code>
		/// array is less than 2 or a contained <code>double[]</code> array does not have
		/// at least two values </exception>
		/// <exception cref="ConvergenceException"> if the p-value can not be computed due to a convergence error </exception>
		/// <exception cref="MaxCountExceededException"> if the maximum number of iterations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double anovaPValue(final java.util.Collection<double[]> categoryData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double anovaPValue(ICollection<double[]> categoryData)
		{

			AnovaStats a = anovaStats(categoryData);
			// No try-catch or advertised exception because args are valid
			FDistribution fdist = new FDistribution(a.dfbg, a.dfwg);
			return 1.0 - fdist.cumulativeProbability(a.F);

		}

		/// <summary>
		/// Computes the ANOVA P-value for a collection of <seealso cref="SummaryStatistics"/>.
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>The categoryData <code>Collection</code> must contain
		/// <seealso cref="SummaryStatistics"/>.</li>
		/// <li> There must be at least two <seealso cref="SummaryStatistics"/> in the
		/// <code>categoryData</code> collection and each of these statistics must
		/// contain at least two values.</li></ul></p><p>
		/// This implementation uses the
		/// {@link mathlib.distribution.FDistribution
		/// commons-math F Distribution implementation} to estimate the exact
		/// p-value, using the formula<pre>
		///   p = 1 - cumulativeProbability(F)</pre>
		/// where <code>F</code> is the F value and <code>cumulativeProbability</code>
		/// is the commons-math implementation of the F distribution.</p>
		/// </summary>
		/// <param name="categoryData"> <code>Collection</code> of <seealso cref="SummaryStatistics"/>
		/// each containing data for one category </param>
		/// <param name="allowOneElementData"> if true, allow computation for one catagory
		/// only or for one data element per category </param>
		/// <returns> Pvalue </returns>
		/// <exception cref="NullArgumentException"> if <code>categoryData</code> is <code>null</code> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the <code>categoryData</code>
		/// array is less than 2 or a contained <seealso cref="SummaryStatistics"/> does not have
		/// at least two values </exception>
		/// <exception cref="ConvergenceException"> if the p-value can not be computed due to a convergence error </exception>
		/// <exception cref="MaxCountExceededException"> if the maximum number of iterations is exceeded
		/// @since 3.2 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double anovaPValue(final java.util.Collection<mathlib.stat.descriptive.SummaryStatistics> categoryData, final boolean allowOneElementData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double anovaPValue(ICollection<SummaryStatistics> categoryData, bool allowOneElementData)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final AnovaStats a = anovaStats(categoryData, allowOneElementData);
			AnovaStats a = anovaStats(categoryData, allowOneElementData);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.FDistribution fdist = new mathlib.distribution.FDistribution(a.dfbg, a.dfwg);
			FDistribution fdist = new FDistribution(a.dfbg, a.dfwg);
			return 1.0 - fdist.cumulativeProbability(a.F);

		}

		/// <summary>
		/// This method calls the method that actually does the calculations (except
		/// P-value).
		/// </summary>
		/// <param name="categoryData">
		///            <code>Collection</code> of <code>double[]</code> arrays each
		///            containing data for one category </param>
		/// <returns> computed AnovaStats </returns>
		/// <exception cref="NullArgumentException">
		///             if <code>categoryData</code> is <code>null</code> </exception>
		/// <exception cref="DimensionMismatchException">
		///             if the length of the <code>categoryData</code> array is less
		///             than 2 or a contained <code>double[]</code> array does not
		///             contain at least two values </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private AnovaStats anovaStats(final java.util.Collection<double[]> categoryData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private AnovaStats anovaStats(ICollection<double[]> categoryData)
		{

			MathUtils.checkNotNull(categoryData);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Collection<mathlib.stat.descriptive.SummaryStatistics> categoryDataSummaryStatistics = new java.util.ArrayList<mathlib.stat.descriptive.SummaryStatistics>(categoryData.size());
			ICollection<SummaryStatistics> categoryDataSummaryStatistics = new List<SummaryStatistics>(categoryData.Count);

			// convert arrays to SummaryStatistics
			foreach (double[] data in categoryData)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.stat.descriptive.SummaryStatistics dataSummaryStatistics = new mathlib.stat.descriptive.SummaryStatistics();
				SummaryStatistics dataSummaryStatistics = new SummaryStatistics();
				categoryDataSummaryStatistics.Add(dataSummaryStatistics);
				foreach (double val in data)
				{
					dataSummaryStatistics.addValue(val);
				}
			}

			return anovaStats(categoryDataSummaryStatistics, false);

		}

		/// <summary>
		/// Performs an ANOVA test, evaluating the null hypothesis that there
		/// is no difference among the means of the data categories.
		/// 
		/// <p><strong>Preconditions</strong>: <ul>
		/// <li>The categoryData <code>Collection</code> must contain
		/// <code>double[]</code> arrays.</li>
		/// <li> There must be at least two <code>double[]</code> arrays in the
		/// <code>categoryData</code> collection and each of these arrays must
		/// contain at least two values.</li>
		/// <li>alpha must be strictly greater than 0 and less than or equal to 0.5.
		/// </li></ul></p><p>
		/// This implementation uses the
		/// {@link mathlib.distribution.FDistribution
		/// commons-math F Distribution implementation} to estimate the exact
		/// p-value, using the formula<pre>
		///   p = 1 - cumulativeProbability(F)</pre>
		/// where <code>F</code> is the F value and <code>cumulativeProbability</code>
		/// is the commons-math implementation of the F distribution.</p>
		/// <p>True is returned iff the estimated p-value is less than alpha.</p>
		/// </summary>
		/// <param name="categoryData"> <code>Collection</code> of <code>double[]</code>
		/// arrays each containing data for one category </param>
		/// <param name="alpha"> significance level of the test </param>
		/// <returns> true if the null hypothesis can be rejected with
		/// confidence 1 - alpha </returns>
		/// <exception cref="NullArgumentException"> if <code>categoryData</code> is <code>null</code> </exception>
		/// <exception cref="DimensionMismatchException"> if the length of the <code>categoryData</code>
		/// array is less than 2 or a contained <code>double[]</code> array does not have
		/// at least two values </exception>
		/// <exception cref="OutOfRangeException"> if <code>alpha</code> is not in the range (0, 0.5] </exception>
		/// <exception cref="ConvergenceException"> if the p-value can not be computed due to a convergence error </exception>
		/// <exception cref="MaxCountExceededException"> if the maximum number of iterations is exceeded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean anovaTest(final java.util.Collection<double[]> categoryData, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.OutOfRangeException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual bool anovaTest(ICollection<double[]> categoryData, double alpha)
		{

			if ((alpha <= 0) || (alpha > 0.5))
			{
				throw new OutOfRangeException(LocalizedFormats.OUT_OF_BOUND_SIGNIFICANCE_LEVEL, alpha, 0, 0.5);
			}
			return anovaPValue(categoryData) < alpha;

		}

		/// <summary>
		/// This method actually does the calculations (except P-value).
		/// </summary>
		/// <param name="categoryData"> <code>Collection</code> of <code>double[]</code>
		/// arrays each containing data for one category </param>
		/// <param name="allowOneElementData"> if true, allow computation for one catagory
		/// only or for one data element per category </param>
		/// <returns> computed AnovaStats </returns>
		/// <exception cref="NullArgumentException"> if <code>categoryData</code> is <code>null</code> </exception>
		/// <exception cref="DimensionMismatchException"> if <code>allowOneElementData</code> is false and the number of
		/// categories is less than 2 or a contained SummaryStatistics does not contain
		/// at least two values </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private AnovaStats anovaStats(final java.util.Collection<mathlib.stat.descriptive.SummaryStatistics> categoryData, final boolean allowOneElementData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private AnovaStats anovaStats(ICollection<SummaryStatistics> categoryData, bool allowOneElementData)
		{

			MathUtils.checkNotNull(categoryData);

			if (!allowOneElementData)
			{
				// check if we have enough categories
				if (categoryData.Count < 2)
				{
					throw new DimensionMismatchException(LocalizedFormats.TWO_OR_MORE_CATEGORIES_REQUIRED, categoryData.Count, 2);
				}

				// check if each category has enough data
				foreach (SummaryStatistics array in categoryData)
				{
					if (array.N <= 1)
					{
						throw new DimensionMismatchException(LocalizedFormats.TWO_OR_MORE_VALUES_IN_CATEGORY_REQUIRED, (int) array.N, 2);
					}
				}
			}

			int dfwg = 0;
			double sswg = 0;
			double totsum = 0;
			double totsumsq = 0;
			int totnum = 0;

			foreach (SummaryStatistics data in categoryData)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sum = data.getSum();
				double sum = data.Sum;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sumsq = data.getSumsq();
				double sumsq = data.Sumsq;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int num = (int) data.getN();
				int num = (int) data.N;
				totnum += num;
				totsum += sum;
				totsumsq += sumsq;

				dfwg += num - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ss = sumsq - ((sum * sum) / num);
				double ss = sumsq - ((sum * sum) / num);
				sswg += ss;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double sst = totsumsq - ((totsum * totsum) / totnum);
			double sst = totsumsq - ((totsum * totsum) / totnum);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ssbg = sst - sswg;
			double ssbg = sst - sswg;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dfbg = categoryData.size() - 1;
			int dfbg = categoryData.Count - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double msbg = ssbg / dfbg;
			double msbg = ssbg / dfbg;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double mswg = sswg / dfwg;
			double mswg = sswg / dfwg;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double F = msbg / mswg;
			double F = msbg / mswg;

			return new AnovaStats(dfbg, dfwg, F);

		}

		/// <summary>
		///    Convenience class to pass dfbg,dfwg,F values around within OneWayAnova.
		///    No get/set methods provided.
		/// </summary>
		private class AnovaStats
		{

			/// <summary>
			/// Degrees of freedom in numerator (between groups). </summary>
			internal readonly int dfbg;

			/// <summary>
			/// Degrees of freedom in denominator (within groups). </summary>
			internal readonly int dfwg;

			/// <summary>
			/// Statistic. </summary>
			internal readonly double F;

			/// <summary>
			/// Constructor </summary>
			/// <param name="dfbg"> degrees of freedom in numerator (between groups) </param>
			/// <param name="dfwg"> degrees of freedom in denominator (within groups) </param>
			/// <param name="F"> statistic </param>
			internal AnovaStats(int dfbg, int dfwg, double F)
			{
				this.dfbg = dfbg;
				this.dfwg = dfwg;
				this.F = F;
			}
		}

	}

}