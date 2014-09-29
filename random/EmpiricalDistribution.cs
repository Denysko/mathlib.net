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

namespace mathlib.random
{


	using AbstractRealDistribution = mathlib.distribution.AbstractRealDistribution;
	using NormalDistribution = mathlib.distribution.NormalDistribution;
	using RealDistribution = mathlib.distribution.RealDistribution;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using MathInternalError = mathlib.exception.MathInternalError;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using ZeroException = mathlib.exception.ZeroException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
	using StatisticalSummary = mathlib.stat.descriptive.StatisticalSummary;
	using SummaryStatistics = mathlib.stat.descriptive.SummaryStatistics;
	using FastMath = mathlib.util.FastMath;
	using MathUtils = mathlib.util.MathUtils;

	/// <summary>
	/// <p>Represents an <a href="http://http://en.wikipedia.org/wiki/Empirical_distribution_function">
	/// empirical probability distribution</a> -- a probability distribution derived
	/// from observed data without making any assumptions about the functional form
	/// of the population distribution that the data come from.</p>
	/// 
	/// <p>An <code>EmpiricalDistribution</code> maintains data structures, called
	/// <i>distribution digests</i>, that describe empirical distributions and
	/// support the following operations: <ul>
	/// <li>loading the distribution from a file of observed data values</li>
	/// <li>dividing the input data into "bin ranges" and reporting bin frequency
	///     counts (data for histogram)</li>
	/// <li>reporting univariate statistics describing the full set of data values
	///     as well as the observations within each bin</li>
	/// <li>generating random values from the distribution</li>
	/// </ul>
	/// Applications can use <code>EmpiricalDistribution</code> to build grouped
	/// frequency histograms representing the input data or to generate random values
	/// "like" those in the input file -- i.e., the values generated will follow the
	/// distribution of the values in the file.</p>
	/// 
	/// <p>The implementation uses what amounts to the
	/// <a href="http://nedwww.ipac.caltech.edu/level5/March02/Silverman/Silver2_6.html">
	/// Variable Kernel Method</a> with Gaussian smoothing:<p>
	/// <strong>Digesting the input file</strong>
	/// <ol><li>Pass the file once to compute min and max.</li>
	/// <li>Divide the range from min-max into <code>binCount</code> "bins."</li>
	/// <li>Pass the data file again, computing bin counts and univariate
	///     statistics (mean, std dev.) for each of the bins </li>
	/// <li>Divide the interval (0,1) into subintervals associated with the bins,
	///     with the length of a bin's subinterval proportional to its count.</li></ol>
	/// <strong>Generating random values from the distribution</strong><ol>
	/// <li>Generate a uniformly distributed value in (0,1) </li>
	/// <li>Select the subinterval to which the value belongs.
	/// <li>Generate a random Gaussian value with mean = mean of the associated
	///     bin and std dev = std dev of associated bin.</li></ol></p>
	/// 
	/// <p>EmpiricalDistribution implements the <seealso cref="RealDistribution"/> interface
	/// as follows.  Given x within the range of values in the dataset, let B
	/// be the bin containing x and let K be the within-bin kernel for B.  Let P(B-)
	/// be the sum of the probabilities of the bins below B and let K(B) be the
	/// mass of B under K (i.e., the integral of the kernel density over B).  Then
	/// set P(X < x) = P(B-) + P(B) * K(x) / K(B) where K(x) is the kernel distribution
	/// evaluated at x. This results in a cdf that matches the grouped frequency
	/// distribution at the bin endpoints and interpolates within bins using
	/// within-bin kernels.</p>
	/// 
	/// <strong>USAGE NOTES:</strong><ul>
	/// <li>The <code>binCount</code> is set by default to 1000.  A good rule of thumb
	///    is to set the bin count to approximately the length of the input file divided
	///    by 10. </li>
	/// <li>The input file <i>must</i> be a plain text file containing one valid numeric
	///    entry per line.</li>
	/// </ul></p>
	/// 
	/// @version $Id: EmpiricalDistribution.java 1587494 2014-04-15 10:02:54Z erans $
	/// </summary>
	public class EmpiricalDistribution : AbstractRealDistribution
	{

		/// <summary>
		/// Default bin count </summary>
		public const int DEFAULT_BIN_COUNT = 1000;

		/// <summary>
		/// Character set for file input </summary>
		private const string FILE_CHARSET = "US-ASCII";

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 5729073523949762654L;

		/// <summary>
		/// RandomDataGenerator instance to use in repeated calls to getNext() </summary>
		protected internal readonly new RandomDataGenerator randomData;

		/// <summary>
		/// List of SummaryStatistics objects characterizing the bins </summary>
		private readonly IList<SummaryStatistics> binStats;

		/// <summary>
		/// Sample statistics </summary>
		private SummaryStatistics sampleStats = null;

		/// <summary>
		/// Max loaded value </summary>
		private double max = double.NegativeInfinity;

		/// <summary>
		/// Min loaded value </summary>
		private double min = double.PositiveInfinity;

		/// <summary>
		/// Grid size </summary>
		private double delta = 0d;

		/// <summary>
		/// number of bins </summary>
		private readonly int binCount;

		/// <summary>
		/// is the distribution loaded? </summary>
		private bool loaded = false;

		/// <summary>
		/// upper bounds of subintervals in (0,1) "belonging" to the bins </summary>
		private double[] upperBounds = null;

		/// <summary>
		/// Creates a new EmpiricalDistribution with the default bin count.
		/// </summary>
		public EmpiricalDistribution() : this(DEFAULT_BIN_COUNT)
		{
		}

		/// <summary>
		/// Creates a new EmpiricalDistribution with the specified bin count.
		/// </summary>
		/// <param name="binCount"> number of bins </param>
		public EmpiricalDistribution(int binCount) : this(binCount, new RandomDataGenerator())
		{
		}

		/// <summary>
		/// Creates a new EmpiricalDistribution with the specified bin count using the
		/// provided <seealso cref="RandomGenerator"/> as the source of random data.
		/// </summary>
		/// <param name="binCount"> number of bins </param>
		/// <param name="generator"> random data generator (may be null, resulting in default JDK generator)
		/// @since 3.0 </param>
		public EmpiricalDistribution(int binCount, RandomGenerator generator) : this(binCount, new RandomDataGenerator(generator))
		{
		}

		/// <summary>
		/// Creates a new EmpiricalDistribution with default bin count using the
		/// provided <seealso cref="RandomGenerator"/> as the source of random data.
		/// </summary>
		/// <param name="generator"> random data generator (may be null, resulting in default JDK generator)
		/// @since 3.0 </param>
		public EmpiricalDistribution(RandomGenerator generator) : this(DEFAULT_BIN_COUNT, generator)
		{
		}

		/// <summary>
		/// Creates a new EmpiricalDistribution with the specified bin count using the
		/// provided <seealso cref="RandomDataImpl"/> instance as the source of random data.
		/// </summary>
		/// <param name="binCount"> number of bins </param>
		/// <param name="randomData"> random data generator (may be null, resulting in default JDK generator)
		/// @since 3.0 </param>
		/// @deprecated As of 3.1. Please use <seealso cref="#EmpiricalDistribution(int,RandomGenerator)"/> instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#EmpiricalDistribution(int,RandomGenerator)"/> instead.")]
		public EmpiricalDistribution(int binCount, RandomDataImpl randomData) : this(binCount, randomData.Delegate)
		{
		}

		/// <summary>
		/// Creates a new EmpiricalDistribution with default bin count using the
		/// provided <seealso cref="RandomDataImpl"/> as the source of random data.
		/// </summary>
		/// <param name="randomData"> random data generator (may be null, resulting in default JDK generator)
		/// @since 3.0 </param>
		/// @deprecated As of 3.1. Please use <seealso cref="#EmpiricalDistribution(RandomGenerator)"/> instead. 
		[Obsolete("As of 3.1. Please use <seealso cref="#EmpiricalDistribution(RandomGenerator)"/> instead.")]
		public EmpiricalDistribution(RandomDataImpl randomData) : this(DEFAULT_BIN_COUNT, randomData)
		{
		}

		/// <summary>
		/// Private constructor to allow lazy initialisation of the RNG contained
		/// in the <seealso cref="#randomData"/> instance variable.
		/// </summary>
		/// <param name="binCount"> number of bins </param>
		/// <param name="randomData"> Random data generator. </param>
		private EmpiricalDistribution(int binCount, RandomDataGenerator randomData) : base(null)
		{
			this.binCount = binCount;
			this.randomData = randomData;
			binStats = new List<SummaryStatistics>();
		}

		/// <summary>
		/// Computes the empirical distribution from the provided
		/// array of numbers.
		/// </summary>
		/// <param name="in"> the input data array </param>
		/// <exception cref="NullArgumentException"> if in is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(double[] in) throws mathlib.exception.NullArgumentException
		public virtual void load(double[] @in)
		{
			DataAdapter da = new ArrayDataAdapter(this, @in);
			try
			{
				da.computeStats();
				// new adapter for the second pass
				fillBinStats(new ArrayDataAdapter(this, @in));
			}
			catch (IOException ex)
			{
				// Can't happen
				throw new MathInternalError();
			}
			loaded = true;

		}

		/// <summary>
		/// Computes the empirical distribution using data read from a URL.
		/// 
		/// <p>The input file <i>must</i> be an ASCII text file containing one
		/// valid numeric entry per line.</p>
		/// </summary>
		/// <param name="url"> url of the input file
		/// </param>
		/// <exception cref="IOException"> if an IO error occurs </exception>
		/// <exception cref="NullArgumentException"> if url is null </exception>
		/// <exception cref="ZeroException"> if URL contains no data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.net.URL url) throws java.io.IOException, mathlib.exception.NullArgumentException, mathlib.exception.ZeroException
		public virtual void load(URL url)
		{
			MathUtils.checkNotNull(url);
			Charset charset = Charset.forName(FILE_CHARSET);
			BufferedReader @in = new BufferedReader(new InputStreamReader(url.openStream(), charset));
			try
			{
				DataAdapter da = new StreamDataAdapter(this, @in);
				da.computeStats();
				if (sampleStats.N == 0)
				{
					throw new ZeroException(LocalizedFormats.URL_CONTAINS_NO_DATA, url);
				}
				// new adapter for the second pass
				@in = new BufferedReader(new InputStreamReader(url.openStream(), charset));
				fillBinStats(new StreamDataAdapter(this, @in));
				loaded = true;
			}
			finally
			{
			   try
			   {
				   @in.close();
			   } //NOPMD
			   catch (IOException ex)
			   {
				   // ignore
			   }
			}
		}

		/// <summary>
		/// Computes the empirical distribution from the input file.
		/// 
		/// <p>The input file <i>must</i> be an ASCII text file containing one
		/// valid numeric entry per line.</p>
		/// </summary>
		/// <param name="file"> the input file </param>
		/// <exception cref="IOException"> if an IO error occurs </exception>
		/// <exception cref="NullArgumentException"> if file is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void load(java.io.File file) throws java.io.IOException, mathlib.exception.NullArgumentException
		public virtual void load(File file)
		{
			MathUtils.checkNotNull(file);
			Charset charset = Charset.forName(FILE_CHARSET);
			InputStream @is = new FileInputStream(file);
			BufferedReader @in = new BufferedReader(new InputStreamReader(@is, charset));
			try
			{
				DataAdapter da = new StreamDataAdapter(this, @in);
				da.computeStats();
				// new adapter for second pass
				@is = new FileInputStream(file);
				@in = new BufferedReader(new InputStreamReader(@is, charset));
				fillBinStats(new StreamDataAdapter(this, @in));
				loaded = true;
			}
			finally
			{
				try
				{
					@in.close();
				} //NOPMD
				catch (IOException ex)
				{
					// ignore
				}
			}
		}

		/// <summary>
		/// Provides methods for computing <code>sampleStats</code> and
		/// <code>beanStats</code> abstracting the source of data.
		/// </summary>
		private abstract class DataAdapter
		{
			private readonly EmpiricalDistribution outerInstance;

			public DataAdapter(EmpiricalDistribution outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			/// <summary>
			/// Compute bin stats.
			/// </summary>
			/// <exception cref="IOException">  if an error occurs computing bin stats </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void computeBinStats() throws java.io.IOException;
			public abstract void computeBinStats();

			/// <summary>
			/// Compute sample statistics.
			/// </summary>
			/// <exception cref="IOException"> if an error occurs computing sample stats </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public abstract void computeStats() throws java.io.IOException;
			public abstract void computeStats();

		}

		/// <summary>
		/// <code>DataAdapter</code> for data provided through some input stream
		/// </summary>
		private class StreamDataAdapter : DataAdapter
		{
			private readonly EmpiricalDistribution outerInstance;


			/// <summary>
			/// Input stream providing access to the data </summary>
			internal BufferedReader inputStream;

			/// <summary>
			/// Create a StreamDataAdapter from a BufferedReader
			/// </summary>
			/// <param name="in"> BufferedReader input stream </param>
			public StreamDataAdapter(EmpiricalDistribution outerInstance, BufferedReader @in) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				inputStream = @in;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void computeBinStats() throws java.io.IOException
			public override void computeBinStats()
			{
				string str = null;
				double val = 0.0d;
				while ((str = inputStream.readLine()) != null)
				{
					val = Convert.ToDouble(str);
					SummaryStatistics stats = outerInstance.binStats[outerInstance.findBin(val)];
					stats.addValue(val);
				}

				inputStream.close();
				inputStream = null;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void computeStats() throws java.io.IOException
			public override void computeStats()
			{
				string str = null;
				double val = 0.0;
				outerInstance.sampleStats = new SummaryStatistics();
				while ((str = inputStream.readLine()) != null)
				{
					val = Convert.ToDouble(str);
					outerInstance.sampleStats.addValue(val);
				}
				inputStream.close();
				inputStream = null;
			}
		}

		/// <summary>
		/// <code>DataAdapter</code> for data provided as array of doubles.
		/// </summary>
		private class ArrayDataAdapter : DataAdapter
		{
			private readonly EmpiricalDistribution outerInstance;


			/// <summary>
			/// Array of input  data values </summary>
			internal double[] inputArray;

			/// <summary>
			/// Construct an ArrayDataAdapter from a double[] array
			/// </summary>
			/// <param name="in"> double[] array holding the data </param>
			/// <exception cref="NullArgumentException"> if in is null </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ArrayDataAdapter(double[] in) throws mathlib.exception.NullArgumentException
			public ArrayDataAdapter(EmpiricalDistribution outerInstance, double[] @in) : base(outerInstance)
			{
				this.outerInstance = outerInstance;
				MathUtils.checkNotNull(@in);
				inputArray = @in;
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void computeStats() throws java.io.IOException
			public override void computeStats()
			{
				outerInstance.sampleStats = new SummaryStatistics();
				for (int i = 0; i < inputArray.Length; i++)
				{
					outerInstance.sampleStats.addValue(inputArray[i]);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void computeBinStats() throws java.io.IOException
			public override void computeBinStats()
			{
				for (int i = 0; i < inputArray.Length; i++)
				{
					SummaryStatistics stats = outerInstance.binStats[outerInstance.findBin(inputArray[i])];
					stats.addValue(inputArray[i]);
				}
			}
		}

		/// <summary>
		/// Fills binStats array (second pass through data file).
		/// </summary>
		/// <param name="da"> object providing access to the data </param>
		/// <exception cref="IOException">  if an IO error occurs </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void fillBinStats(final DataAdapter da) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void fillBinStats(DataAdapter da)
		{
			// Set up grid
			min = sampleStats.Min;
			max = sampleStats.Max;
			delta = (max - min) / ((double) binCount);

			// Initialize binStats ArrayList
			if (binStats.Count > 0)
			{
				binStats.Clear();
			}
			for (int i = 0; i < binCount; i++)
			{
				SummaryStatistics stats = new SummaryStatistics();
				binStats.Insert(i,stats);
			}

			// Filling data in binStats Array
			da.computeBinStats();

			// Assign upperBounds based on bin counts
			upperBounds = new double[binCount];
			upperBounds[0] = ((double) binStats[0].N) / (double) sampleStats.N;
			for (int i = 1; i < binCount - 1; i++)
			{
				upperBounds[i] = upperBounds[i - 1] + ((double) binStats[i].N) / (double) sampleStats.N;
			}
			upperBounds[binCount - 1] = 1.0d;
		}

		/// <summary>
		/// Returns the index of the bin to which the given value belongs
		/// </summary>
		/// <param name="value">  the value whose bin we are trying to find </param>
		/// <returns> the index of the bin containing the value </returns>
		private int findBin(double value)
		{
			return FastMath.min(FastMath.max((int) FastMath.ceil((value - min) / delta) - 1, 0), binCount - 1);
		}

		/// <summary>
		/// Generates a random value from this distribution.
		/// <strong>Preconditions:</strong><ul>
		/// <li>the distribution must be loaded before invoking this method</li></ul> </summary>
		/// <returns> the random value. </returns>
		/// <exception cref="MathIllegalStateException"> if the distribution has not been loaded </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getNextValue() throws mathlib.exception.MathIllegalStateException
		public virtual double NextValue
		{
			get
			{
    
				if (!loaded)
				{
					throw new MathIllegalStateException(LocalizedFormats.DISTRIBUTION_NOT_LOADED);
				}
    
				// Start with a uniformly distributed random number in (0,1)
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double x = randomData.nextUniform(0,1);
				double x = randomData.nextUniform(0,1);
    
				// Use this to select the bin and generate a Gaussian within the bin
				for (int i = 0; i < binCount; i++)
				{
				   if (x <= upperBounds[i])
				   {
					   SummaryStatistics stats = binStats[i];
					   if (stats.N > 0)
					   {
						   if (stats.StandardDeviation > 0) // more than one obs
						   {
							   return getKernel(stats).sample();
						   }
						   else
						   {
							   return stats.Mean; // only one obs in bin
						   }
					   }
				   }
				}
				throw new MathIllegalStateException(LocalizedFormats.NO_BIN_SELECTED);
			}
		}

		/// <summary>
		/// Returns a <seealso cref="StatisticalSummary"/> describing this distribution.
		/// <strong>Preconditions:</strong><ul>
		/// <li>the distribution must be loaded before invoking this method</li></ul>
		/// </summary>
		/// <returns> the sample statistics </returns>
		/// <exception cref="IllegalStateException"> if the distribution has not been loaded </exception>
		public virtual StatisticalSummary SampleStats
		{
			get
			{
				return sampleStats;
			}
		}

		/// <summary>
		/// Returns the number of bins.
		/// </summary>
		/// <returns> the number of bins. </returns>
		public virtual int BinCount
		{
			get
			{
				return binCount;
			}
		}

		/// <summary>
		/// Returns a List of <seealso cref="SummaryStatistics"/> instances containing
		/// statistics describing the values in each of the bins.  The list is
		/// indexed on the bin number.
		/// </summary>
		/// <returns> List of bin statistics. </returns>
		public virtual IList<SummaryStatistics> BinStats
		{
			get
			{
				return binStats;
			}
		}

		/// <summary>
		/// <p>Returns a fresh copy of the array of upper bounds for the bins.
		/// Bins are: <br/>
		/// [min,upperBounds[0]],(upperBounds[0],upperBounds[1]],...,
		///  (upperBounds[binCount-2], upperBounds[binCount-1] = max].</p>
		/// 
		/// <p>Note: In versions 1.0-2.0 of commons-math, this method
		/// incorrectly returned the array of probability generator upper
		/// bounds now returned by <seealso cref="#getGeneratorUpperBounds()"/>.</p>
		/// </summary>
		/// <returns> array of bin upper bounds
		/// @since 2.1 </returns>
		public virtual double[] UpperBounds
		{
			get
			{
				double[] binUpperBounds = new double[binCount];
				for (int i = 0; i < binCount - 1; i++)
				{
					binUpperBounds[i] = min + delta * (i + 1);
				}
				binUpperBounds[binCount - 1] = max;
				return binUpperBounds;
			}
		}

		/// <summary>
		/// <p>Returns a fresh copy of the array of upper bounds of the subintervals
		/// of [0,1] used in generating data from the empirical distribution.
		/// Subintervals correspond to bins with lengths proportional to bin counts.</p>
		/// 
		/// <strong>Preconditions:</strong><ul>
		/// <li>the distribution must be loaded before invoking this method</li></ul>
		/// 
		/// <p>In versions 1.0-2.0 of commons-math, this array was (incorrectly) returned
		/// by <seealso cref="#getUpperBounds()"/>.</p>
		/// 
		/// @since 2.1 </summary>
		/// <returns> array of upper bounds of subintervals used in data generation </returns>
		/// <exception cref="NullPointerException"> unless a {@code load} method has been
		/// called beforehand. </exception>
		public virtual double[] GeneratorUpperBounds
		{
			get
			{
				int len = upperBounds.Length;
				double[] @out = new double[len];
				Array.Copy(upperBounds, 0, @out, 0, len);
				return @out;
			}
		}

		/// <summary>
		/// Property indicating whether or not the distribution has been loaded.
		/// </summary>
		/// <returns> true if the distribution has been loaded </returns>
		public virtual bool Loaded
		{
			get
			{
				return loaded;
			}
		}

		/// <summary>
		/// Reseeds the random number generator used by <seealso cref="#getNextValue()"/>.
		/// </summary>
		/// <param name="seed"> random generator seed
		/// @since 3.0 </param>
		public virtual void reSeed(long seed)
		{
			randomData.reSeed(seed);
		}

		// Distribution methods ---------------------------

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override double probability(double x)
		{
			return 0;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>Returns the kernel density normalized so that its integral over each bin
		/// equals the bin mass.</p>
		/// 
		/// <p>Algorithm description: <ol>
		/// <li>Find the bin B that x belongs to.</li>
		/// <li>Compute K(B) = the mass of B with respect to the within-bin kernel (i.e., the
		/// integral of the kernel density over B).</li>
		/// <li>Return k(x) * P(B) / K(B), where k is the within-bin kernel density
		/// and P(B) is the mass of B.</li></ol></p>
		/// @since 3.1
		/// </summary>
		public override double density(double x)
		{
			if (x < min || x > max)
			{
				return 0d;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int binIndex = findBin(x);
			int binIndex = findBin(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.RealDistribution kernel = getKernel(binStats.get(binIndex));
			RealDistribution kernel = getKernel(binStats[binIndex]);
			return kernel.density(x) * pB(binIndex) / kB(binIndex);
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>Algorithm description:<ol>
		/// <li>Find the bin B that x belongs to.</li>
		/// <li>Compute P(B) = the mass of B and P(B-) = the combined mass of the bins below B.</li>
		/// <li>Compute K(B) = the probability mass of B with respect to the within-bin kernel
		/// and K(B-) = the kernel distribution evaluated at the lower endpoint of B</li>
		/// <li>Return P(B-) + P(B) * [K(x) - K(B-)] / K(B) where
		/// K(x) is the within-bin kernel distribution function evaluated at x.</li></ol></p>
		/// 
		/// @since 3.1
		/// </summary>
		public override double cumulativeProbability(double x)
		{
			if (x < min)
			{
				return 0d;
			}
			else if (x >= max)
			{
				return 1d;
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int binIndex = findBin(x);
			int binIndex = findBin(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pBminus = pBminus(binIndex);
			double pBminus = pBminus(binIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pB = pB(binIndex);
			double pB = pB(binIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] binBounds = getUpperBounds();
			double[] binBounds = UpperBounds;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double kB = kB(binIndex);
			double kB = kB(binIndex);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lower = binIndex == 0 ? min : binBounds[binIndex - 1];
			double lower = binIndex == 0 ? min : binBounds[binIndex - 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.RealDistribution kernel = k(x);
			RealDistribution kernel = k(x);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double withinBinCum = (kernel.cumulativeProbability(x) - kernel.cumulativeProbability(lower)) / kB;
			double withinBinCum = (kernel.cumulativeProbability(x) - kernel.cumulativeProbability(lower)) / kB;
			return pBminus + pB * withinBinCum;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p>Algorithm description:<ol>
		/// <li>Find the smallest i such that the sum of the masses of the bins
		///  through i is at least p.</li>
		/// <li>
		///   Let K be the within-bin kernel distribution for bin i.</br>
		///   Let K(B) be the mass of B under K. <br/>
		///   Let K(B-) be K evaluated at the lower endpoint of B (the combined
		///   mass of the bins below B under K).<br/>
		///   Let P(B) be the probability of bin i.<br/>
		///   Let P(B-) be the sum of the bin masses below bin i. <br/>
		///   Let pCrit = p - P(B-)<br/>
		/// <li>Return the inverse of K evaluated at <br/>
		///    K(B-) + pCrit * K(B) / P(B) </li>
		///  </ol></p>
		/// 
		/// @since 3.1
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double inverseCumulativeProbability(double p)
		{
			if (p < 0.0 || p > 1.0)
			{
				throw new OutOfRangeException(p, 0, 1);
			}

			if (p == 0.0)
			{
				return SupportLowerBound;
			}

			if (p == 1.0)
			{
				return SupportUpperBound;
			}

			int i = 0;
			while (cumBinP(i) < p)
			{
				i++;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.RealDistribution kernel = getKernel(binStats.get(i));
			RealDistribution kernel = getKernel(binStats[i]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double kB = kB(i);
			double kB = kB(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] binBounds = getUpperBounds();
			double[] binBounds = UpperBounds;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lower = i == 0 ? min : binBounds[i - 1];
			double lower = i == 0 ? min : binBounds[i - 1];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double kBminus = kernel.cumulativeProbability(lower);
			double kBminus = kernel.cumulativeProbability(lower);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pB = pB(i);
			double pB = pB(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pBminus = pBminus(i);
			double pBminus = pBminus(i);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double pCrit = p - pBminus;
			double pCrit = p - pBminus;
			if (pCrit <= 0)
			{
				return lower;
			}
			return kernel.inverseCumulativeProbability(kBminus + pCrit * kB / pB);
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override double NumericalMean
		{
			get
			{
			   return sampleStats.Mean;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override double NumericalVariance
		{
			get
			{
				return sampleStats.Variance;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override double SupportLowerBound
		{
			get
			{
			   return min;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override double SupportUpperBound
		{
			get
			{
				return max;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override bool SupportLowerBoundInclusive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override bool SupportUpperBoundInclusive
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override bool SupportConnected
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override double sample()
		{
			return NextValue;
		}

		/// <summary>
		/// {@inheritDoc}
		/// @since 3.1
		/// </summary>
		public override void reseedRandomGenerator(long seed)
		{
			randomData.reSeed(seed);
		}

		/// <summary>
		/// The probability of bin i.
		/// </summary>
		/// <param name="i"> the index of the bin </param>
		/// <returns> the probability that selection begins in bin i </returns>
		private double pB(int i)
		{
			return i == 0 ? upperBounds[0] : upperBounds[i] - upperBounds[i - 1];
		}

		/// <summary>
		/// The combined probability of the bins up to but not including bin i.
		/// </summary>
		/// <param name="i"> the index of the bin </param>
		/// <returns> the probability that selection begins in a bin below bin i. </returns>
		private double pBminus(int i)
		{
			return i == 0 ? 0 : upperBounds[i - 1];
		}

		/// <summary>
		/// Mass of bin i under the within-bin kernel of the bin.
		/// </summary>
		/// <param name="i"> index of the bin </param>
		/// <returns> the difference in the within-bin kernel cdf between the
		/// upper and lower endpoints of bin i </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("deprecation") private double kB(int i)
		private double kB(int i)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] binBounds = getUpperBounds();
			double[] binBounds = UpperBounds;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.distribution.RealDistribution kernel = getKernel(binStats.get(i));
			RealDistribution kernel = getKernel(binStats[i]);
			return i == 0 ? kernel.cumulativeProbability(min, binBounds[0]) : kernel.cumulativeProbability(binBounds[i - 1], binBounds[i]);
		}

		/// <summary>
		/// The within-bin kernel of the bin that x belongs to.
		/// </summary>
		/// <param name="x"> the value to locate within a bin </param>
		/// <returns> the within-bin kernel of the bin containing x </returns>
		private RealDistribution k(double x)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int binIndex = findBin(x);
			int binIndex = findBin(x);
			return getKernel(binStats[binIndex]);
		}

		/// <summary>
		/// The combined probability of the bins up to and including binIndex.
		/// </summary>
		/// <param name="binIndex"> maximum bin index </param>
		/// <returns> sum of the probabilities of bins through binIndex </returns>
		private double cumBinP(int binIndex)
		{
			return upperBounds[binIndex];
		}

		/// <summary>
		/// The within-bin smoothing kernel.
		/// </summary>
		/// <param name="bStats"> summary statistics for the bin </param>
		/// <returns> within-bin kernel parameterized by bStats </returns>
		protected internal virtual RealDistribution getKernel(SummaryStatistics bStats)
		{
			// Default to Gaussian
			return new NormalDistribution(randomData.RandomGenerator, bStats.Mean, bStats.StandardDeviation, NormalDistribution.DEFAULT_INVERSE_ABSOLUTE_ACCURACY);
		}
	}

}