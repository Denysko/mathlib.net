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


	using MathInternalError = mathlib.exception.MathInternalError;
	using MathParseException = mathlib.exception.MathParseException;
	using NotPositiveException = mathlib.exception.NotPositiveException;
	using NotStrictlyPositiveException = mathlib.exception.NotStrictlyPositiveException;
	using OutOfRangeException = mathlib.exception.OutOfRangeException;
	using FastMath = mathlib.util.FastMath;

	/// <summary>
	/// Implementation of a Sobol sequence.
	/// <p>
	/// A Sobol sequence is a low-discrepancy sequence with the property that for all values of N,
	/// its subsequence (x1, ... xN) has a low discrepancy. It can be used to generate pseudo-random
	/// points in a space S, which are equi-distributed.
	/// <p>
	/// The implementation already comes with support for up to 1000 dimensions with direction numbers
	/// calculated from <a href="http://web.maths.unsw.edu.au/~fkuo/sobol/">Stephen Joe and Frances Kuo</a>.
	/// <p>
	/// The generator supports two modes:
	/// <ul>
	///   <li>sequential generation of points: <seealso cref="#nextVector()"/></li>
	///   <li>random access to the i-th point in the sequence: <seealso cref="#skipTo(int)"/></li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://en.wikipedia.org/wiki/Sobol_sequence">Sobol sequence (Wikipedia)</a> </seealso>
	/// <seealso cref= <a href="http://web.maths.unsw.edu.au/~fkuo/sobol/">Sobol sequence direction numbers</a>
	/// 
	/// @version $Id: SobolSequenceGenerator.java 1538368 2013-11-03 13:57:37Z erans $
	/// @since 3.3 </seealso>
	public class SobolSequenceGenerator : RandomVectorGenerator
	{

		/// <summary>
		/// The number of bits to use. </summary>
		private const int BITS = 52;

		/// <summary>
		/// The scaling factor. </summary>
		private static readonly double SCALE = FastMath.pow(2, BITS);

		/// <summary>
		/// The maximum supported space dimension. </summary>
		private const int MAX_DIMENSION = 1000;

		/// <summary>
		/// The resource containing the direction numbers. </summary>
		private const string RESOURCE_NAME = "/assets/org/apache/commons/math3/random/new-joe-kuo-6.1000";

		/// <summary>
		/// Character set for file input. </summary>
		private const string FILE_CHARSET = "US-ASCII";

		/// <summary>
		/// Space dimension. </summary>
		private readonly int dimension;

		/// <summary>
		/// The current index in the sequence. </summary>
		private int count = 0;

		/// <summary>
		/// The direction vector for each component. </summary>
		private readonly long[][] direction;

		/// <summary>
		/// The current state. </summary>
		private readonly long[] x;

		/// <summary>
		/// Construct a new Sobol sequence generator for the given space dimension.
		/// </summary>
		/// <param name="dimension"> the space dimension </param>
		/// <exception cref="OutOfRangeException"> if the space dimension is outside the allowed range of [1, 1000] </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SobolSequenceGenerator(final int dimension) throws mathlib.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public SobolSequenceGenerator(int dimension)
		{
			if (dimension < 1 || dimension > MAX_DIMENSION)
			{
				throw new OutOfRangeException(dimension, 1, MAX_DIMENSION);
			}

			// initialize the other dimensions with direction numbers from a resource
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.InputStream is = getClass().getResourceAsStream(RESOURCE_NAME);
			InputStream @is = this.GetType().getResourceAsStream(RESOURCE_NAME);
			if (@is == null)
			{
				throw new MathInternalError();
			}

			this.dimension = dimension;

			// init data structures
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: direction = new long[dimension][BITS + 1];
			direction = RectangularArrays.ReturnRectangularLongArray(dimension, BITS + 1);
			x = new long[dimension];

			try
			{
				initFromStream(@is);
			}
			catch (IOException e)
			{
				// the internal resource file could not be read -> should not happen
				throw new MathInternalError();
			}
			catch (MathParseException e)
			{
				// the internal resource file could not be parsed -> should not happen
				throw new MathInternalError();
			}
			finally
			{
				try
				{
					@is.close();
				} // NOPMD
				catch (IOException e)
				{
					// ignore
				}
			}
		}

		/// <summary>
		/// Construct a new Sobol sequence generator for the given space dimension with
		/// direction vectors loaded from the given stream.
		/// <p>
		/// The expected format is identical to the files available from
		/// <a href="http://web.maths.unsw.edu.au/~fkuo/sobol/">Stephen Joe and Frances Kuo</a>.
		/// The first line will be ignored as it is assumed to contain only the column headers.
		/// The columns are:
		/// <ul>
		///  <li>d: the dimension</li>
		///  <li>s: the degree of the primitive polynomial</li>
		///  <li>a: the number representing the coefficients</li>
		///  <li>m: the list of initial direction numbers</li>
		/// </ul>
		/// Example:
		/// <pre>
		/// d       s       a       m_i
		/// 2       1       0       1
		/// 3       2       1       1 3
		/// </pre>
		/// <p>
		/// The input stream <i>must</i> be an ASCII text containing one valid direction vector per line.
		/// </summary>
		/// <param name="dimension"> the space dimension </param>
		/// <param name="is"> the stream to read the direction vectors from </param>
		/// <exception cref="NotStrictlyPositiveException"> if the space dimension is &lt; 1 </exception>
		/// <exception cref="OutOfRangeException"> if the space dimension is outside the range [1, max], where
		///   max refers to the maximum dimension found in the input stream </exception>
		/// <exception cref="MathParseException"> if the content in the stream could not be parsed successfully </exception>
		/// <exception cref="IOException"> if an error occurs while reading from the input stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SobolSequenceGenerator(final int dimension, final java.io.InputStream is) throws mathlib.exception.NotStrictlyPositiveException, mathlib.exception.MathParseException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public SobolSequenceGenerator(int dimension, InputStream @is)
		{

			if (dimension < 1)
			{
				throw new NotStrictlyPositiveException(dimension);
			}

			this.dimension = dimension;

			// init data structures
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: direction = new long[dimension][BITS + 1];
			direction = RectangularArrays.ReturnRectangularLongArray(dimension, BITS + 1);
			x = new long[dimension];

			// initialize the other dimensions with direction numbers from the stream
			int lastDimension = initFromStream(@is);
			if (lastDimension < dimension)
			{
				throw new OutOfRangeException(dimension, 1, lastDimension);
			}
		}

		/// <summary>
		/// Load the direction vector for each dimension from the given stream.
		/// <p>
		/// The input stream <i>must</i> be an ASCII text containing one
		/// valid direction vector per line.
		/// </summary>
		/// <param name="is"> the input stream to read the direction vector from </param>
		/// <returns> the last dimension that has been read from the input stream </returns>
		/// <exception cref="IOException"> if the stream could not be read </exception>
		/// <exception cref="MathParseException"> if the content could not be parsed successfully </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private int initFromStream(final java.io.InputStream is) throws mathlib.exception.MathParseException, java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private int initFromStream(InputStream @is)
		{

			// special case: dimension 1 -> use unit initialization
			for (int i = 1; i <= BITS; i++)
			{
				direction[0][i] = 1l << (BITS - i);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.nio.charset.Charset charset = java.nio.charset.Charset.forName(FILE_CHARSET);
			Charset charset = Charset.forName(FILE_CHARSET);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.BufferedReader reader = new java.io.BufferedReader(new java.io.InputStreamReader(is, charset));
			BufferedReader reader = new BufferedReader(new InputStreamReader(@is, charset));
			int dim = -1;

			try
			{
				// ignore first line
				reader.readLine();

				int lineNumber = 2;
				int index = 1;
				string line = null;
				while ((line = reader.readLine()) != null)
				{
					StringTokenizer st = new StringTokenizer(line, " ");
					try
					{
						dim = Convert.ToInt32(st.nextToken());
						if (dim >= 2 && dim <= dimension) // we have found the right dimension
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int s = Integer.parseInt(st.nextToken());
							int s = Convert.ToInt32(st.nextToken());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int a = Integer.parseInt(st.nextToken());
							int a = Convert.ToInt32(st.nextToken());
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] m = new int[s + 1];
							int[] m = new int[s + 1];
							for (int i = 1; i <= s; i++)
							{
								m[i] = Convert.ToInt32(st.nextToken());
							}
							initDirectionVector(index++, a, m);
						}

						if (dim > dimension)
						{
							return dim;
						}
					}
					catch (NoSuchElementException e)
					{
						throw new MathParseException(line, lineNumber);
					}
					catch (NumberFormatException e)
					{
						throw new MathParseException(line, lineNumber);
					}
					lineNumber++;
				}
			}
			finally
			{
				reader.close();
			}

			return dim;
		}

		/// <summary>
		/// Calculate the direction numbers from the given polynomial.
		/// </summary>
		/// <param name="d"> the dimension, zero-based </param>
		/// <param name="a"> the coefficients of the primitive polynomial </param>
		/// <param name="m"> the initial direction numbers </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void initDirectionVector(final int d, final int a, final int[] m)
		private void initDirectionVector(int d, int a, int[] m)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int s = m.length - 1;
			int s = m.Length - 1;
			for (int i = 1; i <= s; i++)
			{
				direction[d][i] = ((long) m[i]) << (BITS - i);
			}
			for (int i = s + 1; i <= BITS; i++)
			{
				direction[d][i] = direction[d][i - s] ^ (direction[d][i - s] >> s);
				for (int k = 1; k <= s - 1; k++)
				{
					direction[d][i] ^= ((a >> (s - 1 - k)) & 1) * direction[d][i - k];
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual double[] nextVector()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] v = new double[dimension];
			double[] v = new double[dimension];
			if (count == 0)
			{
				count++;
				return v;
			}

			// find the index c of the rightmost 0
			int c = 1;
			int value = count - 1;
			while ((value & 1) == 1)
			{
				value >>= 1;
				c++;
			}

			for (int i = 0; i < dimension; i++)
			{
				x[i] ^= direction[i][c];
				v[i] = (double) x[i] / SCALE;
			}
			count++;
			return v;
		}

		/// <summary>
		/// Skip to the i-th point in the Sobol sequence.
		/// <p>
		/// This operation can be performed in O(1).
		/// </summary>
		/// <param name="index"> the index in the sequence to skip to </param>
		/// <returns> the i-th point in the Sobol sequence </returns>
		/// <exception cref="NotPositiveException"> if index &lt; 0 </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] skipTo(final int index) throws mathlib.exception.NotPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] skipTo(int index)
		{
			if (index == 0)
			{
				// reset x vector
				Arrays.fill(x, 0);
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int i = index - 1;
				int i = index - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long grayCode = i ^ (i >> 1);
				long grayCode = i ^ (i >> 1); // compute the gray code of i = i XOR floor(i / 2)
				for (int j = 0; j < dimension; j++)
				{
					long result = 0;
					for (int k = 1; k <= BITS; k++)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long shift = grayCode >> (k - 1);
						long shift = grayCode >> (k - 1);
						if (shift == 0)
						{
							// stop, as all remaining bits will be zero
							break;
						}
						// the k-th bit of i
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final long ik = shift & 1;
						long ik = shift & 1;
						result ^= ik * direction[j][k];
					}
					x[j] = result;
				}
			}
			count = index;
			return nextVector();
		}

		/// <summary>
		/// Returns the index i of the next point in the Sobol sequence that will be returned
		/// by calling <seealso cref="#nextVector()"/>.
		/// </summary>
		/// <returns> the index of the next point </returns>
		public virtual int NextIndex
		{
			get
			{
				return count;
			}
		}

	}

}