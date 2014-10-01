using System;

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

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MathIllegalStateException = mathlib.exception.MathIllegalStateException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using ZeroException = mathlib.exception.ZeroException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Generates values for use in simulation applications.
	/// <p>
	/// How values are generated is determined by the <code>mode</code>
	/// property.</p>
	/// <p>
	/// Supported <code>mode</code> values are: <ul>
	/// <li> DIGEST_MODE -- uses an empirical distribution </li>
	/// <li> REPLAY_MODE -- replays data from <code>valuesFileURL</code></li>
	/// <li> UNIFORM_MODE -- generates uniformly distributed random values with
	///                      mean = <code>mu</code> </li>
	/// <li> EXPONENTIAL_MODE -- generates exponentially distributed random values
	///                         with mean = <code>mu</code></li>
	/// <li> GAUSSIAN_MODE -- generates Gaussian distributed random values with
	///                       mean = <code>mu</code> and
	///                       standard deviation = <code>sigma</code></li>
	/// <li> CONSTANT_MODE -- returns <code>mu</code> every time.</li></ul></p>
	/// 
	/// @version $Id: ValueServer.java 1587494 2014-04-15 10:02:54Z erans $
	/// 
	/// </summary>
	public class ValueServer
	{

		/// <summary>
		/// Use empirical distribution. </summary>
		public const int DIGEST_MODE = 0;

		/// <summary>
		/// Replay data from valuesFilePath. </summary>
		public const int REPLAY_MODE = 1;

		/// <summary>
		/// Uniform random deviates with mean = &mu;. </summary>
		public const int UNIFORM_MODE = 2;

		/// <summary>
		/// Exponential random deviates with mean = &mu;. </summary>
		public const int EXPONENTIAL_MODE = 3;

		/// <summary>
		/// Gaussian random deviates with mean = &mu;, std dev = &sigma;. </summary>
		public const int GAUSSIAN_MODE = 4;

		/// <summary>
		/// Always return mu </summary>
		public const int CONSTANT_MODE = 5;

		/// <summary>
		/// mode determines how values are generated. </summary>
		private int mode = 5;

		/// <summary>
		/// URI to raw data values. </summary>
		private URL valuesFileURL = null;

		/// <summary>
		/// Mean for use with non-data-driven modes. </summary>
		private double mu = 0.0;

		/// <summary>
		/// Standard deviation for use with GAUSSIAN_MODE. </summary>
		private double sigma = 0.0;

		/// <summary>
		/// Empirical probability distribution for use with DIGEST_MODE. </summary>
		private EmpiricalDistribution empiricalDistribution = null;

		/// <summary>
		/// File pointer for REPLAY_MODE. </summary>
		private BufferedReader filePointer = null;

		/// <summary>
		/// RandomDataImpl to use for random data generation. </summary>
		private readonly RandomDataImpl randomData;

		// Data generation modes ======================================

		/// <summary>
		/// Creates new ValueServer </summary>
		public ValueServer()
		{
			randomData = new RandomDataImpl();
		}

		/// <summary>
		/// Construct a ValueServer instance using a RandomDataImpl as its source
		/// of random data.
		/// </summary>
		/// <param name="randomData"> the RandomDataImpl instance used to source random data
		/// @since 3.0 </param>
		/// @deprecated use <seealso cref="#ValueServer(RandomGenerator)"/> 
		[Obsolete]//("use <seealso cref="#ValueServer(RandomGenerator)"/>")]
		public ValueServer(RandomDataImpl randomData)
		{
			this.randomData = randomData;
		}

		/// <summary>
		/// Construct a ValueServer instance using a RandomGenerator as its source
		/// of random data.
		/// 
		/// @since 3.1 </summary>
		/// <param name="generator"> source of random data </param>
		public ValueServer(RandomGenerator generator)
		{
			this.randomData = new RandomDataImpl(generator);
		}

		/// <summary>
		/// Returns the next generated value, generated according
		/// to the mode value (see MODE constants).
		/// </summary>
		/// <returns> generated value </returns>
		/// <exception cref="IOException"> in REPLAY_MODE if a file I/O error occurs </exception>
		/// <exception cref="MathIllegalStateException"> if mode is not recognized </exception>
		/// <exception cref="MathIllegalArgumentException"> if the underlying random generator thwrows one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getNext() throws java.io.IOException, mathlib.exception.MathIllegalStateException, mathlib.exception.MathIllegalArgumentException
		public virtual double Next
		{
			get
			{
				switch (mode)
				{
					case DIGEST_MODE:
						return NextDigest;
					case REPLAY_MODE:
						return NextReplay;
					case UNIFORM_MODE:
						return NextUniform;
					case EXPONENTIAL_MODE:
						return NextExponential;
					case GAUSSIAN_MODE:
						return NextGaussian;
					case CONSTANT_MODE:
						return mu;
					default:
						throw new MathIllegalStateException(LocalizedFormats.UNKNOWN_MODE, mode, "DIGEST_MODE", DIGEST_MODE, "REPLAY_MODE", REPLAY_MODE, "UNIFORM_MODE", UNIFORM_MODE, "EXPONENTIAL_MODE", EXPONENTIAL_MODE, "GAUSSIAN_MODE", GAUSSIAN_MODE, "CONSTANT_MODE", CONSTANT_MODE);
				}
			}
		}

		/// <summary>
		/// Fills the input array with values generated using getNext() repeatedly.
		/// </summary>
		/// <param name="values"> array to be filled </param>
		/// <exception cref="IOException"> in REPLAY_MODE if a file I/O error occurs </exception>
		/// <exception cref="MathIllegalStateException"> if mode is not recognized </exception>
		/// <exception cref="MathIllegalArgumentException"> if the underlying random generator thwrows one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void fill(double[] values) throws java.io.IOException, mathlib.exception.MathIllegalStateException, mathlib.exception.MathIllegalArgumentException
		public virtual void fill(double[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Next;
			}
		}

		/// <summary>
		/// Returns an array of length <code>length</code> with values generated
		/// using getNext() repeatedly.
		/// </summary>
		/// <param name="length"> length of output array </param>
		/// <returns> array of generated values </returns>
		/// <exception cref="IOException"> in REPLAY_MODE if a file I/O error occurs </exception>
		/// <exception cref="MathIllegalStateException"> if mode is not recognized </exception>
		/// <exception cref="MathIllegalArgumentException"> if the underlying random generator thwrows one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] fill(int length) throws java.io.IOException, mathlib.exception.MathIllegalStateException, mathlib.exception.MathIllegalArgumentException
		public virtual double[] fill(int length)
		{
			double[] @out = new double[length];
			for (int i = 0; i < length; i++)
			{
				@out[i] = Next;
			}
			return @out;
		}

		/// <summary>
		/// Computes the empirical distribution using values from the file
		/// in <code>valuesFileURL</code>, using the default number of bins.
		/// <p>
		/// <code>valuesFileURL</code> must exist and be
		/// readable by *this at runtime.</p>
		/// <p>
		/// This method must be called before using <code>getNext()</code>
		/// with <code>mode = DIGEST_MODE</code></p>
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs reading the input file </exception>
		/// <exception cref="NullArgumentException"> if the {@code valuesFileURL} has not been set </exception>
		/// <exception cref="ZeroException"> if URL contains no data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDistribution() throws java.io.IOException, mathlib.exception.ZeroException, mathlib.exception.NullArgumentException
		public virtual void computeDistribution()
		{
			computeDistribution(EmpiricalDistribution.DEFAULT_BIN_COUNT);
		}

		/// <summary>
		/// Computes the empirical distribution using values from the file
		/// in <code>valuesFileURL</code> and <code>binCount</code> bins.
		/// <p>
		/// <code>valuesFileURL</code> must exist and be readable by this process
		/// at runtime.</p>
		/// <p>
		/// This method must be called before using <code>getNext()</code>
		/// with <code>mode = DIGEST_MODE</code></p>
		/// </summary>
		/// <param name="binCount"> the number of bins used in computing the empirical
		/// distribution </param>
		/// <exception cref="NullArgumentException"> if the {@code valuesFileURL} has not been set </exception>
		/// <exception cref="IOException"> if an error occurs reading the input file </exception>
		/// <exception cref="ZeroException"> if URL contains no data </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDistribution(int binCount) throws mathlib.exception.NullArgumentException, java.io.IOException, mathlib.exception.ZeroException
		public virtual void computeDistribution(int binCount)
		{
			empiricalDistribution = new EmpiricalDistribution(binCount, randomData);
			empiricalDistribution.load(valuesFileURL);
			mu = empiricalDistribution.SampleStats.Mean;
			sigma = empiricalDistribution.SampleStats.StandardDeviation;
		}

		/// <summary>
		/// Returns the data generation mode. See <seealso cref="ValueServer the class javadoc"/>
		/// for description of the valid values of this property.
		/// </summary>
		/// <returns> Value of property mode. </returns>
		public virtual int Mode
		{
			get
			{
				return mode;
			}
			set
			{
				this.mode = value;
			}
		}


		/// <summary>
		/// Returns the URL for the file used to build the empirical distribution
		/// when using <seealso cref="#DIGEST_MODE"/>.
		/// </summary>
		/// <returns> Values file URL. </returns>
		public virtual URL ValuesFileURL
		{
			get
			{
				return valuesFileURL;
			}
			set
			{
				this.valuesFileURL = new URL(value);
			}
		}

		/// <summary>
		/// Sets the <seealso cref="#getValuesFileURL() values file URL"/> using a string
		/// URL representation.
		/// </summary>
		/// <param name="url"> String representation for new valuesFileURL. </param>
		/// <exception cref="MalformedURLException"> if url is not well formed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setValuesFileURL(String url) throws java.net.MalformedURLException

		/// <summary>
		/// Sets the the <seealso cref="#getValuesFileURL() values file URL"/>.
		/// 
		/// <p>The values file <i>must</i> be an ASCII text file containing one
		/// valid numeric entry per line.</p>
		/// </summary>
		/// <param name="url"> URL of the values file. </param>
		public virtual URL ValuesFileURL
		{
			set
			{
				this.valuesFileURL = value;
			}
		}

		/// <summary>
		/// Returns the <seealso cref="EmpiricalDistribution"/> used when operating in {@value #DIGEST_MODE}.
		/// </summary>
		/// <returns> EmpircalDistribution built by <seealso cref="#computeDistribution()"/> </returns>
		public virtual EmpiricalDistribution EmpiricalDistribution
		{
			get
			{
				return empiricalDistribution;
			}
		}

		/// <summary>
		/// Resets REPLAY_MODE file pointer to the beginning of the <code>valuesFileURL</code>.
		/// </summary>
		/// <exception cref="IOException"> if an error occurs opening the file </exception>
		/// <exception cref="NullPointerException"> if the {@code valuesFileURL} has not been set. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void resetReplayFile() throws java.io.IOException
		public virtual void resetReplayFile()
		{
			if (filePointer != null)
			{
				try
				{
					filePointer.close();
					filePointer = null;
				} //NOPMD
				catch (IOException ex)
				{
					// ignore
				}
			}
			filePointer = new BufferedReader(new InputStreamReader(valuesFileURL.openStream(), "UTF-8"));
		}

		/// <summary>
		/// Closes {@code valuesFileURL} after use in REPLAY_MODE.
		/// </summary>
		/// <exception cref="IOException"> if an error occurs closing the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void closeReplayFile() throws java.io.IOException
		public virtual void closeReplayFile()
		{
			if (filePointer != null)
			{
				filePointer.close();
				filePointer = null;
			}
		}

		/// <summary>
		/// Returns the mean used when operating in <seealso cref="#GAUSSIAN_MODE"/>, <seealso cref="#EXPONENTIAL_MODE"/>
		/// or <seealso cref="#UNIFORM_MODE"/>.  When operating in <seealso cref="#CONSTANT_MODE"/>, this is the constant
		/// value always returned.  Calling <seealso cref="#computeDistribution()"/> sets this value to the
		/// overall mean of the values in the <seealso cref="#getValuesFileURL() values file"/>.
		/// </summary>
		/// <returns> Mean used in data generation. </returns>
		public virtual double Mu
		{
			get
			{
				return mu;
			}
			set
			{
				this.mu = value;
			}
		}


		/// <summary>
		/// Returns the standard deviation used when operating in <seealso cref="#GAUSSIAN_MODE"/>.
		/// Calling <seealso cref="#computeDistribution()"/> sets this value to the overall standard
		/// deviation of the values in the <seealso cref="#getValuesFileURL() values file"/>.  This
		/// property has no effect when the data generation mode is not
		/// <seealso cref="#GAUSSIAN_MODE"/>.
		/// </summary>
		/// <returns> Standard deviation used when operating in <seealso cref="#GAUSSIAN_MODE"/>. </returns>
		public virtual double Sigma
		{
			get
			{
				return sigma;
			}
			set
			{
				this.sigma = value;
			}
		}


		/// <summary>
		/// Reseeds the random data generator.
		/// </summary>
		/// <param name="seed"> Value with which to reseed the <seealso cref="RandomDataImpl"/>
		/// used to generate random data. </param>
		public virtual void reSeed(long seed)
		{
			randomData.reSeed(seed);
		}

		//------------- private methods ---------------------------------

		/// <summary>
		/// Gets a random value in DIGEST_MODE.
		/// <p>
		/// <strong>Preconditions</strong>: <ul>
		/// <li>Before this method is called, <code>computeDistribution()</code>
		/// must have completed successfully; otherwise an
		/// <code>IllegalStateException</code> will be thrown</li></ul></p>
		/// </summary>
		/// <returns> next random value from the empirical distribution digest </returns>
		/// <exception cref="MathIllegalStateException"> if digest has not been initialized </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getNextDigest() throws mathlib.exception.MathIllegalStateException
		private double NextDigest
		{
			get
			{
				if ((empiricalDistribution == null) || (empiricalDistribution.BinStats.Count == 0))
				{
					throw new MathIllegalStateException(LocalizedFormats.DIGEST_NOT_INITIALIZED);
				}
				return empiricalDistribution.NextValue;
			}
		}

		/// <summary>
		/// Gets next sequential value from the <code>valuesFileURL</code>.
		/// <p>
		/// Throws an IOException if the read fails.</p>
		/// <p>
		/// This method will open the <code>valuesFileURL</code> if there is no
		/// replay file open.</p>
		/// <p>
		/// The <code>valuesFileURL</code> will be closed and reopened to wrap around
		/// from EOF to BOF if EOF is encountered. EOFException (which is a kind of
		/// IOException) may still be thrown if the <code>valuesFileURL</code> is
		/// empty.</p>
		/// </summary>
		/// <returns> next value from the replay file </returns>
		/// <exception cref="IOException"> if there is a problem reading from the file </exception>
		/// <exception cref="MathIllegalStateException"> if URL contains no data </exception>
		/// <exception cref="NumberFormatException"> if an invalid numeric string is
		///   encountered in the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getNextReplay() throws java.io.IOException, mathlib.exception.MathIllegalStateException
		private double NextReplay
		{
			get
			{
				string str = null;
				if (filePointer == null)
				{
					resetReplayFile();
				}
				if ((str = filePointer.readLine()) == null)
				{
					// we have probably reached end of file, wrap around from EOF to BOF
					closeReplayFile();
					resetReplayFile();
					if ((str = filePointer.readLine()) == null)
					{
						throw new MathIllegalStateException(LocalizedFormats.URL_CONTAINS_NO_DATA, valuesFileURL);
					}
				}
				return Convert.ToDouble(str);
			}
		}

		/// <summary>
		/// Gets a uniformly distributed random value with mean = mu.
		/// </summary>
		/// <returns> random uniform value </returns>
		/// <exception cref="MathIllegalArgumentException"> if the underlying random generator thwrows one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getNextUniform() throws mathlib.exception.MathIllegalArgumentException
		private double NextUniform
		{
			get
			{
				return randomData.nextUniform(0, 2 * mu);
			}
		}

		/// <summary>
		/// Gets an exponentially distributed random value with mean = mu.
		/// </summary>
		/// <returns> random exponential value </returns>
		/// <exception cref="MathIllegalArgumentException"> if the underlying random generator thwrows one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getNextExponential() throws mathlib.exception.MathIllegalArgumentException
		private double NextExponential
		{
			get
			{
				return randomData.nextExponential(mu);
			}
		}

		/// <summary>
		/// Gets a Gaussian distributed random value with mean = mu
		/// and standard deviation = sigma.
		/// </summary>
		/// <returns> random Gaussian value </returns>
		/// <exception cref="MathIllegalArgumentException"> if the underlying random generator thwrows one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private double getNextGaussian() throws mathlib.exception.MathIllegalArgumentException
		private double NextGaussian
		{
			get
			{
				return randomData.nextGaussian(mu, sigma);
			}
		}

	}

}