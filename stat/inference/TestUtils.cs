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

	using RealDistribution = mathlib.distribution.RealDistribution;
	using ConvergenceException = mathlib.exception.ConvergenceException;
	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using InsufficientDataException = mathlib.exception.InsufficientDataException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoDataException = mathlib.exception.NoDataException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using ZeroException = mathlib.exception.ZeroException;
	using StatisticalSummary = mathlib.stat.descriptive.StatisticalSummary;

	/// <summary>
	/// A collection of static methods to create inference test instances or to
	/// perform inference tests.
	/// 
	/// @since 1.1
	/// @version $Id: TestUtils.java 1592430 2014-05-04 23:19:43Z psteitz $
	/// </summary>
	public class TestUtils
	{

		/// <summary>
		/// Singleton TTest instance. </summary>
		private static readonly TTest T_TEST = new TTest();

		/// <summary>
		/// Singleton ChiSquareTest instance. </summary>
		private static readonly ChiSquareTest CHI_SQUARE_TEST = new ChiSquareTest();

		/// <summary>
		/// Singleton OneWayAnova instance. </summary>
		private static readonly OneWayAnova ONE_WAY_ANANOVA = new OneWayAnova();

		/// <summary>
		/// Singleton G-Test instance. </summary>
		private static readonly GTest G_TEST = new GTest();

		/// <summary>
		/// Singleton K-S test instance </summary>
		private static readonly KolmogorovSmirnovTest KS_TEST = new KolmogorovSmirnovTest();

		/// <summary>
		/// Prevent instantiation.
		/// </summary>
		private TestUtils() : base()
		{
		}

		// CHECKSTYLE: stop JavadocMethodCheck

		/// <seealso cref= mathlib.stat.inference.TTest#homoscedasticT(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double homoscedasticT(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double homoscedasticT(double[] sample1, double[] sample2)
		{
			return T_TEST.homoscedasticT(sample1, sample2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#homoscedasticT(mathlib.stat.descriptive.StatisticalSummary, mathlib.stat.descriptive.StatisticalSummary) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double homoscedasticT(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double homoscedasticT(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{
			return T_TEST.homoscedasticT(sampleStats1, sampleStats2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#homoscedasticTTest(double[], double[], double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean homoscedasticTTest(final double[] sample1, final double[] sample2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool homoscedasticTTest(double[] sample1, double[] sample2, double alpha)
		{
			return T_TEST.homoscedasticTTest(sample1, sample2, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#homoscedasticTTest(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double homoscedasticTTest(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double homoscedasticTTest(double[] sample1, double[] sample2)
		{
			return T_TEST.homoscedasticTTest(sample1, sample2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#homoscedasticTTest(mathlib.stat.descriptive.StatisticalSummary, mathlib.stat.descriptive.StatisticalSummary) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double homoscedasticTTest(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double homoscedasticTTest(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{
			return T_TEST.homoscedasticTTest(sampleStats1, sampleStats2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#pairedT(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double pairedT(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double pairedT(double[] sample1, double[] sample2)
		{
			return T_TEST.pairedT(sample1, sample2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#pairedTTest(double[], double[], double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean pairedTTest(final double[] sample1, final double[] sample2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool pairedTTest(double[] sample1, double[] sample2, double alpha)
		{
			return T_TEST.pairedTTest(sample1, sample2, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#pairedTTest(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double pairedTTest(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double pairedTTest(double[] sample1, double[] sample2)
		{
			return T_TEST.pairedTTest(sample1, sample2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#t(double, double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double t(final double mu, final double[] observed) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double t(double mu, double[] observed)
		{
			return T_TEST.t(mu, observed);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#t(double, mathlib.stat.descriptive.StatisticalSummary) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double t(final double mu, final mathlib.stat.descriptive.StatisticalSummary sampleStats) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double t(double mu, StatisticalSummary sampleStats)
		{
			return T_TEST.t(mu, sampleStats);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#t(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double t(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double t(double[] sample1, double[] sample2)
		{
			return T_TEST.t(sample1, sample2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#t(mathlib.stat.descriptive.StatisticalSummary, mathlib.stat.descriptive.StatisticalSummary) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double t(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double t(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{
			return T_TEST.t(sampleStats1, sampleStats2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(double, double[], double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean tTest(final double mu, final double[] sample, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool tTest(double mu, double[] sample, double alpha)
		{
			return T_TEST.tTest(mu, sample, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(double, double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double tTest(final double mu, final double[] sample) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double tTest(double mu, double[] sample)
		{
			return T_TEST.tTest(mu, sample);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(double, mathlib.stat.descriptive.StatisticalSummary, double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean tTest(final double mu, final mathlib.stat.descriptive.StatisticalSummary sampleStats, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool tTest(double mu, StatisticalSummary sampleStats, double alpha)
		{
			return T_TEST.tTest(mu, sampleStats, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(double, mathlib.stat.descriptive.StatisticalSummary) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double tTest(final double mu, final mathlib.stat.descriptive.StatisticalSummary sampleStats) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double tTest(double mu, StatisticalSummary sampleStats)
		{
			return T_TEST.tTest(mu, sampleStats);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(double[], double[], double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean tTest(final double[] sample1, final double[] sample2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool tTest(double[] sample1, double[] sample2, double alpha)
		{
			return T_TEST.tTest(sample1, sample2, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(double[], double[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double tTest(final double[] sample1, final double[] sample2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double tTest(double[] sample1, double[] sample2)
		{
			return T_TEST.tTest(sample1, sample2);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(mathlib.stat.descriptive.StatisticalSummary, mathlib.stat.descriptive.StatisticalSummary, double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean tTest(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool tTest(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2, double alpha)
		{
			return T_TEST.tTest(sampleStats1, sampleStats2, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.TTest#tTest(mathlib.stat.descriptive.StatisticalSummary, mathlib.stat.descriptive.StatisticalSummary) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double tTest(final mathlib.stat.descriptive.StatisticalSummary sampleStats1, final mathlib.stat.descriptive.StatisticalSummary sampleStats2) throws mathlib.exception.NullArgumentException, mathlib.exception.NumberIsTooSmallException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double tTest(StatisticalSummary sampleStats1, StatisticalSummary sampleStats2)
		{
			return T_TEST.tTest(sampleStats1, sampleStats2);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquare(double[], long[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double chiSquare(final double[] expected, final long[] observed) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double chiSquare(double[] expected, long[] observed)
		{
			return CHI_SQUARE_TEST.chiSquare(expected, observed);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquare(long[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double chiSquare(final long[][] counts) throws mathlib.exception.NullArgumentException, mathlib.exception.NotPositiveException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double chiSquare(long[][] counts)
		{
			return CHI_SQUARE_TEST.chiSquare(counts);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareTest(double[], long[], double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean chiSquareTest(final double[] expected, final long[] observed, final double alpha) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool chiSquareTest(double[] expected, long[] observed, double alpha)
		{
			return CHI_SQUARE_TEST.chiSquareTest(expected, observed, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareTest(double[], long[]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double chiSquareTest(final double[] expected, final long[] observed) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double chiSquareTest(double[] expected, long[] observed)
		{
			return CHI_SQUARE_TEST.chiSquareTest(expected, observed);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareTest(long[][], double) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean chiSquareTest(final long[][] counts, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool chiSquareTest(long[][] counts, double alpha)
		{
			return CHI_SQUARE_TEST.chiSquareTest(counts, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareTest(long[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double chiSquareTest(final long[][] counts) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double chiSquareTest(long[][] counts)
		{
			return CHI_SQUARE_TEST.chiSquareTest(counts);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareDataSetsComparison(long[], long[])
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double chiSquareDataSetsComparison(final long[] observed1, final long[] observed2) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double chiSquareDataSetsComparison(long[] observed1, long[] observed2)
		{
			return CHI_SQUARE_TEST.chiSquareDataSetsComparison(observed1, observed2);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareTestDataSetsComparison(long[], long[])
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double chiSquareTestDataSetsComparison(final long[] observed1, final long[] observed2) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double chiSquareTestDataSetsComparison(long[] observed1, long[] observed2)
		{
			return CHI_SQUARE_TEST.chiSquareTestDataSetsComparison(observed1, observed2);
		}

		/// <seealso cref= mathlib.stat.inference.ChiSquareTest#chiSquareTestDataSetsComparison(long[], long[], double)
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean chiSquareTestDataSetsComparison(final long[] observed1, final long[] observed2, final double alpha) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool chiSquareTestDataSetsComparison(long[] observed1, long[] observed2, double alpha)
		{
			return CHI_SQUARE_TEST.chiSquareTestDataSetsComparison(observed1, observed2, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.OneWayAnova#anovaFValue(Collection)
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double oneWayAnovaFValue(final java.util.Collection<double[]> categoryData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double oneWayAnovaFValue(ICollection<double[]> categoryData)
		{
			return ONE_WAY_ANANOVA.anovaFValue(categoryData);
		}

		/// <seealso cref= mathlib.stat.inference.OneWayAnova#anovaPValue(Collection)
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double oneWayAnovaPValue(final java.util.Collection<double[]> categoryData) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double oneWayAnovaPValue(ICollection<double[]> categoryData)
		{
			return ONE_WAY_ANANOVA.anovaPValue(categoryData);
		}

		/// <seealso cref= mathlib.stat.inference.OneWayAnova#anovaTest(Collection,double)
		/// 
		/// @since 1.2 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean oneWayAnovaTest(final java.util.Collection<double[]> categoryData, final double alpha) throws mathlib.exception.NullArgumentException, mathlib.exception.DimensionMismatchException, mathlib.exception.OutOfRangeException, mathlib.exception.ConvergenceException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool oneWayAnovaTest(ICollection<double[]> categoryData, double alpha)
		{
			return ONE_WAY_ANANOVA.anovaTest(categoryData, alpha);
		}

		 /// <seealso cref= mathlib.stat.inference.GTest#g(double[], long[])
		 /// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double g(final double[] expected, final long[] observed) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double g(double[] expected, long[] observed)
		{
			return G_TEST.g(expected, observed);
		}

		/// <seealso cref= mathlib.stat.inference.GTest#gTest( double[],  long[] )
		/// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double gTest(final double[] expected, final long[] observed) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double gTest(double[] expected, long[] observed)
		{
			return G_TEST.gTest(expected, observed);
		}

		/// <seealso cref= mathlib.stat.inference.GTest#gTestIntrinsic(double[], long[] )
		/// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double gTestIntrinsic(final double[] expected, final long[] observed) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double gTestIntrinsic(double[] expected, long[] observed)
		{
			return G_TEST.gTestIntrinsic(expected, observed);
		}

		 /// <seealso cref= mathlib.stat.inference.GTest#gTest( double[],long[],double)
		 /// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean gTest(final double[] expected, final long[] observed, final double alpha) throws mathlib.exception.NotPositiveException, mathlib.exception.NotStrictlyPositiveException, mathlib.exception.DimensionMismatchException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool gTest(double[] expected, long[] observed, double alpha)
		{
			return G_TEST.gTest(expected, observed, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.GTest#gDataSetsComparison(long[], long[])
		/// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double gDataSetsComparison(final long[] observed1, final long[] observed2) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double gDataSetsComparison(long[] observed1, long[] observed2)
		{
			return G_TEST.gDataSetsComparison(observed1, observed2);
		}

		/// <seealso cref= mathlib.stat.inference.GTest#rootLogLikelihoodRatio(long, long, long, long)
		/// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double rootLogLikelihoodRatio(final long k11, final long k12, final long k21, final long k22) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double rootLogLikelihoodRatio(long k11, long k12, long k21, long k22)
		{
			return G_TEST.rootLogLikelihoodRatio(k11, k12, k21, k22);
		}


		/// <seealso cref= mathlib.stat.inference.GTest#gTestDataSetsComparison(long[], long[])
		/// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double gTestDataSetsComparison(final long[] observed1, final long[] observed2) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double gTestDataSetsComparison(long[] observed1, long[] observed2)
		{
			return G_TEST.gTestDataSetsComparison(observed1, observed2);
		}

		/// <seealso cref= mathlib.stat.inference.GTest#gTestDataSetsComparison(long[],long[],double)
		/// @since 3.1 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean gTestDataSetsComparison(final long[] observed1, final long[] observed2, final double alpha) throws mathlib.exception.DimensionMismatchException, mathlib.exception.NotPositiveException, mathlib.exception.ZeroException, mathlib.exception.OutOfRangeException, mathlib.exception.MaxCountExceededException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static bool gTestDataSetsComparison(long[] observed1, long[] observed2, double alpha)
		{
			return G_TEST.gTestDataSetsComparison(observed1, observed2, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#kolmogorovSmirnovStatistic(RealDistribution, double[])
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double kolmogorovSmirnovStatistic(mathlib.distribution.RealDistribution dist, double[] data) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static double kolmogorovSmirnovStatistic(RealDistribution dist, double[] data)
		{
			return KS_TEST.kolmogorovSmirnovStatistic(dist, data);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#kolmogorovSmirnovTest(RealDistribution, double[])
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double kolmogorovSmirnovTest(mathlib.distribution.RealDistribution dist, double[] data) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static double kolmogorovSmirnovTest(RealDistribution dist, double[] data)
		{
			return KS_TEST.kolmogorovSmirnovTest(dist, data);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#kolmogorovSmirnovTest(RealDistribution, double[], boolean)
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double kolmogorovSmirnovTest(mathlib.distribution.RealDistribution dist, double[] data, boolean strict) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static double kolmogorovSmirnovTest(RealDistribution dist, double[] data, bool strict)
		{
			return KS_TEST.kolmogorovSmirnovTest(dist, data, strict);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#kolmogorovSmirnovTest(RealDistribution, double[], double)
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static boolean kolmogorovSmirnovTest(mathlib.distribution.RealDistribution dist, double[] data, double alpha) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static bool kolmogorovSmirnovTest(RealDistribution dist, double[] data, double alpha)
		{
			return KS_TEST.kolmogorovSmirnovTest(dist, data, alpha);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#kolmogorovSmirnovStatistic(double[], double[])
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double kolmogorovSmirnovStatistic(double[] x, double[] y) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static double kolmogorovSmirnovStatistic(double[] x, double[] y)
		{
			return KS_TEST.kolmogorovSmirnovStatistic(x, y);
		}

		/// <seealso cref= kolmogorovSmirnovTest(double[], double[])
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double kolmogorovSmirnovTest(double[] x, double[] y) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static double kolmogorovSmirnovTest(double[] x, double[] y)
		{
			return KS_TEST.kolmogorovSmirnovTest(x, y);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#kolmogorovSmirnovTest(double[], double[], boolean)
		/// @since 3.3 </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double kolmogorovSmirnovTest(double[] x, double[] y, boolean strict) throws mathlib.exception.InsufficientDataException, mathlib.exception.NullArgumentException
		public static double kolmogorovSmirnovTest(double[] x, double[] y, bool strict)
		{
			return KS_TEST.kolmogorovSmirnovTest(x, y, strict);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#exactP(double, int, int, boolean)
		/// @since 3.3 </seealso>
		public static double exactP(double d, int m, int n, bool strict)
		{
			return KS_TEST.exactP(d, n, m, strict);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#approximateP(double, int, int)
		/// @since 3.3 </seealso>
		public static double approximateP(double d, int n, int m)
		{
			return KS_TEST.approximateP(d, n, m);
		}

		/// <seealso cref= mathlib.stat.inference.KolmogorovSmirnovTest#monteCarloP(double, int, int, boolean, int)
		/// @since 3.3 </seealso>
		public static double monteCarloP(double d, int n, int m, bool strict, int iterations)
		{
			return KS_TEST.monteCarloP(d, n, m, strict, iterations);
		}


		// CHECKSTYLE: resume JavadocMethodCheck

	}

}