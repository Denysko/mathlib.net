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

namespace org.apache.commons.math3.linear
{


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using NumberIsTooSmallException = org.apache.commons.math3.exception.NumberIsTooSmallException;
	using OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Cache-friendly implementation of RealMatrix using a flat arrays to store
	/// square blocks of the matrix.
	/// <p>
	/// This implementation is specially designed to be cache-friendly. Square blocks are
	/// stored as small arrays and allow efficient traversal of data both in row major direction
	/// and columns major direction, one block at a time. This greatly increases performances
	/// for algorithms that use crossed directions loops like multiplication or transposition.
	/// </p>
	/// <p>
	/// The size of square blocks is a static parameter. It may be tuned according to the cache
	/// size of the target computer processor. As a rule of thumbs, it should be the largest
	/// value that allows three blocks to be simultaneously cached (this is necessary for example
	/// for matrix multiplication). The default value is to use 52x52 blocks which is well suited
	/// for processors with 64k L1 cache (one block holds 2704 values or 21632 bytes). This value
	/// could be lowered to 36x36 for processors with 32k L1 cache.
	/// </p>
	/// <p>
	/// The regular blocks represent <seealso cref="#BLOCK_SIZE"/> x <seealso cref="#BLOCK_SIZE"/> squares. Blocks
	/// at right hand side and bottom side which may be smaller to fit matrix dimensions. The square
	/// blocks are flattened in row major order in single dimension arrays which are therefore
	/// <seealso cref="#BLOCK_SIZE"/><sup>2</sup> elements long for regular blocks. The blocks are themselves
	/// organized in row major order.
	/// </p>
	/// <p>
	/// As an example, for a block size of 52x52, a 100x60 matrix would be stored in 4 blocks.
	/// Block 0 would be a double[2704] array holding the upper left 52x52 square, block 1 would be
	/// a double[416] array holding the upper right 52x8 rectangle, block 2 would be a double[2496]
	/// array holding the lower left 48x52 rectangle and block 3 would be a double[384] array
	/// holding the lower right 48x8 rectangle.
	/// </p>
	/// <p>
	/// The layout complexity overhead versus simple mapping of matrices to java
	/// arrays is negligible for small matrices (about 1%). The gain from cache efficiency leads
	/// to up to 3-fold improvements for matrices of moderate to large size.
	/// </p>
	/// @version $Id: BlockRealMatrix.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public class BlockRealMatrix : AbstractRealMatrix
	{
		/// <summary>
		/// Block size. </summary>
		public const int BLOCK_SIZE = 52;
		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = 4991895511313664478L;
		/// <summary>
		/// Blocks of matrix entries. </summary>
		private readonly double[][] blocks;
		/// <summary>
		/// Number of rows of the matrix. </summary>
		private readonly int rows;
		/// <summary>
		/// Number of columns of the matrix. </summary>
		private readonly int columns;
		/// <summary>
		/// Number of block rows of the matrix. </summary>
		private readonly int blockRows;
		/// <summary>
		/// Number of block columns of the matrix. </summary>
		private readonly int blockColumns;

		/// <summary>
		/// Create a new matrix with the supplied row and column dimensions.
		/// </summary>
		/// <param name="rows">  the number of rows in the new matrix </param>
		/// <param name="columns">  the number of columns in the new matrix </param>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BlockRealMatrix(final int rows, final int columns) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BlockRealMatrix(int rows, int columns) : base(rows, columns)
		{
			this.rows = rows;
			this.columns = columns;

			// number of blocks
			blockRows = (rows + BLOCK_SIZE - 1) / BLOCK_SIZE;
			blockColumns = (columns + BLOCK_SIZE - 1) / BLOCK_SIZE;

			// allocate storage blocks, taking care of smaller ones at right and bottom
			blocks = createBlocksLayout(rows, columns);
		}

		/// <summary>
		/// Create a new dense matrix copying entries from raw layout data.
		/// <p>The input array <em>must</em> already be in raw layout.</p>
		/// <p>Calling this constructor is equivalent to call:
		/// <pre>matrix = new BlockRealMatrix(rawData.length, rawData[0].length,
		///                                   toBlocksLayout(rawData), false);</pre>
		/// </p>
		/// </summary>
		/// <param name="rawData"> data for new matrix, in raw layout </param>
		/// <exception cref="DimensionMismatchException"> if the shape of {@code blockData} is
		/// inconsistent with block layout. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive. </exception>
		/// <seealso cref= #BlockRealMatrix(int, int, double[][], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BlockRealMatrix(final double[][] rawData) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BlockRealMatrix(double[][] rawData) : this(rawData.Length, rawData[0].Length, toBlocksLayout(rawData), false)
		{
		}

		/// <summary>
		/// Create a new dense matrix copying entries from block layout data.
		/// <p>The input array <em>must</em> already be in blocks layout.</p>
		/// </summary>
		/// <param name="rows"> Number of rows in the new matrix. </param>
		/// <param name="columns"> Number of columns in the new matrix. </param>
		/// <param name="blockData"> data for new matrix </param>
		/// <param name="copyArray"> Whether the input array will be copied or referenced. </param>
		/// <exception cref="DimensionMismatchException"> if the shape of {@code blockData} is
		/// inconsistent with block layout. </exception>
		/// <exception cref="NotStrictlyPositiveException"> if row or column dimension is not
		/// positive. </exception>
		/// <seealso cref= #createBlocksLayout(int, int) </seealso>
		/// <seealso cref= #toBlocksLayout(double[][]) </seealso>
		/// <seealso cref= #BlockRealMatrix(double[][]) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BlockRealMatrix(final int rows, final int columns, final double[][] blockData, final boolean copyArray) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public BlockRealMatrix(int rows, int columns, double[][] blockData, bool copyArray) : base(rows, columns)
		{
			this.rows = rows;
			this.columns = columns;

			// number of blocks
			blockRows = (rows + BLOCK_SIZE - 1) / BLOCK_SIZE;
			blockColumns = (columns + BLOCK_SIZE - 1) / BLOCK_SIZE;

			if (copyArray)
			{
				// allocate storage blocks, taking care of smaller ones at right and bottom
				blocks = new double[blockRows * blockColumns][];
			}
			else
			{
				// reference existing array
				blocks = blockData;
			}

			int index = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
				for (int jBlock = 0; jBlock < blockColumns; ++jBlock, ++index)
				{
					if (blockData[index].Length != iHeight * blockWidth(jBlock))
					{
						throw new DimensionMismatchException(blockData[index].Length, iHeight * blockWidth(jBlock));
					}
					if (copyArray)
					{
						blocks[index] = blockData[index].clone();
					}
				}
			}
		}

		/// <summary>
		/// Convert a data array from raw layout to blocks layout.
		/// <p>
		/// Raw layout is the straightforward layout where element at row i and
		/// column j is in array element <code>rawData[i][j]</code>. Blocks layout
		/// is the layout used in <seealso cref="BlockRealMatrix"/> instances, where the matrix
		/// is split in square blocks (except at right and bottom side where blocks may
		/// be rectangular to fit matrix size) and each block is stored in a flattened
		/// one-dimensional array.
		/// </p>
		/// <p>
		/// This method creates an array in blocks layout from an input array in raw layout.
		/// It can be used to provide the array argument of the {@link
		/// #BlockRealMatrix(int, int, double[][], boolean)} constructor.
		/// </p> </summary>
		/// <param name="rawData"> Data array in raw layout. </param>
		/// <returns> a new data array containing the same entries but in blocks layout. </returns>
		/// <exception cref="DimensionMismatchException"> if {@code rawData} is not rectangular. </exception>
		/// <seealso cref= #createBlocksLayout(int, int) </seealso>
		/// <seealso cref= #BlockRealMatrix(int, int, double[][], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double[][] toBlocksLayout(final double[][] rawData) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public static double[][] toBlocksLayout(double[][] rawData)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rows = rawData.length;
			int rows = rawData.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columns = rawData[0].length;
			int columns = rawData[0].Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockRows = (rows + BLOCK_SIZE - 1) / BLOCK_SIZE;
			int blockRows = (rows + BLOCK_SIZE - 1) / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockColumns = (columns + BLOCK_SIZE - 1) / BLOCK_SIZE;
			int blockColumns = (columns + BLOCK_SIZE - 1) / BLOCK_SIZE;

			// safety checks
			for (int i = 0; i < rawData.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = rawData[i].length;
				int length = rawData[i].Length;
				if (length != columns)
				{
					throw new DimensionMismatchException(columns, length);
				}
			}

			// convert array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] blocks = new double[blockRows * blockColumns][];
			double[][] blocks = new double[blockRows * blockColumns][];
			int blockIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = pEnd - pStart;
				int iHeight = pEnd - pStart;
				for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
					int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
					int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = qEnd - qStart;
					int jWidth = qEnd - qStart;

					// allocate new block
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = new double[iHeight * jWidth];
					double[] block = new double[iHeight * jWidth];
					blocks[blockIndex] = block;

					// copy data
					int index = 0;
					for (int p = pStart; p < pEnd; ++p)
					{
						Array.Copy(rawData[p], qStart, block, index, jWidth);
						index += jWidth;
					}
					++blockIndex;
				}
			}

			return blocks;
		}

		/// <summary>
		/// Create a data array in blocks layout.
		/// <p>
		/// This method can be used to create the array argument of the {@link
		/// #BlockRealMatrix(int, int, double[][], boolean)} constructor.
		/// </p> </summary>
		/// <param name="rows"> Number of rows in the new matrix. </param>
		/// <param name="columns"> Number of columns in the new matrix. </param>
		/// <returns> a new data array in blocks layout. </returns>
		/// <seealso cref= #toBlocksLayout(double[][]) </seealso>
		/// <seealso cref= #BlockRealMatrix(int, int, double[][], boolean) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double[][] createBlocksLayout(final int rows, final int columns)
		public static double[][] createBlocksLayout(int rows, int columns)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockRows = (rows + BLOCK_SIZE - 1) / BLOCK_SIZE;
			int blockRows = (rows + BLOCK_SIZE - 1) / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockColumns = (columns + BLOCK_SIZE - 1) / BLOCK_SIZE;
			int blockColumns = (columns + BLOCK_SIZE - 1) / BLOCK_SIZE;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] blocks = new double[blockRows * blockColumns][];
			double[][] blocks = new double[blockRows * blockColumns][];
			int blockIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = pEnd - pStart;
				int iHeight = pEnd - pStart;
				for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
					int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
					int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = qEnd - qStart;
					int jWidth = qEnd - qStart;
					blocks[blockIndex] = new double[iHeight * jWidth];
					++blockIndex;
				}
			}

			return blocks;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix createMatrix(final int rowDimension, final int columnDimension) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix createMatrix(int rowDimension, int columnDimension)
		{
			return new BlockRealMatrix(rowDimension, columnDimension);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override BlockRealMatrix copy()
		{
			// create an empty matrix
			BlockRealMatrix copied = new BlockRealMatrix(rows, columns);

			// copy the blocks
			for (int i = 0; i < blocks.Length; ++i)
			{
				Array.Copy(blocks[i], 0, copied.blocks[i], 0, blocks[i].Length);
			}

			return copied;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix add(final RealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix add(RealMatrix m)
		{
			try
			{
				return add((BlockRealMatrix) m);
			}
			catch (System.InvalidCastException cce)
			{
				// safety check
				MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, columns);
				BlockRealMatrix @out = new BlockRealMatrix(rows, columns);

				// perform addition block-wise, to ensure good cache behavior
				int blockIndex = 0;
				for (int iBlock = 0; iBlock < @out.blockRows; ++iBlock)
				{
					for (int jBlock = 0; jBlock < @out.blockColumns; ++jBlock)
					{

						// perform addition on the current block
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
						double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[blockIndex];
						double[] tBlock = blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
						int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
						int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
						int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
						int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
						int k = 0;
						for (int p = pStart; p < pEnd; ++p)
						{
							for (int q = qStart; q < qEnd; ++q)
							{
								outBlock[k] = tBlock[k] + m.getEntry(p, q);
								++k;
							}
						}
						// go to next block
						++blockIndex;
					}
				}

				return @out;
			}
		}

		/// <summary>
		/// Compute the sum of this matrix and {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to be added. </param>
		/// <returns> {@code this} + m. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the same
		/// size as this matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BlockRealMatrix add(final BlockRealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual BlockRealMatrix add(BlockRealMatrix m)
		{
			// safety check
			MatrixUtils.checkAdditionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, columns);
			BlockRealMatrix @out = new BlockRealMatrix(rows, columns);

			// perform addition block-wise, to ensure good cache behavior
			for (int blockIndex = 0; blockIndex < @out.blocks.Length; ++blockIndex)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
				double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[blockIndex];
				double[] tBlock = blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mBlock = m.blocks[blockIndex];
				double[] mBlock = m.blocks[blockIndex];
				for (int k = 0; k < outBlock.Length; ++k)
				{
					outBlock[k] = tBlock[k] + mBlock[k];
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix subtract(final RealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix subtract(RealMatrix m)
		{
			try
			{
				return subtract((BlockRealMatrix) m);
			}
			catch (System.InvalidCastException cce)
			{
				// safety check
				MatrixUtils.checkSubtractionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, columns);
				BlockRealMatrix @out = new BlockRealMatrix(rows, columns);

				// perform subtraction block-wise, to ensure good cache behavior
				int blockIndex = 0;
				for (int iBlock = 0; iBlock < @out.blockRows; ++iBlock)
				{
					for (int jBlock = 0; jBlock < @out.blockColumns; ++jBlock)
					{

						// perform subtraction on the current block
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
						double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[blockIndex];
						double[] tBlock = blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
						int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
						int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
						int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
						int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
						int k = 0;
						for (int p = pStart; p < pEnd; ++p)
						{
							for (int q = qStart; q < qEnd; ++q)
							{
								outBlock[k] = tBlock[k] - m.getEntry(p, q);
								++k;
							}
						}
						// go to next block
						++blockIndex;
					}
				}

				return @out;
			}
		}

		/// <summary>
		/// Subtract {@code m} from this matrix.
		/// </summary>
		/// <param name="m"> Matrix to be subtracted. </param>
		/// <returns> {@code this} - m. </returns>
		/// <exception cref="MatrixDimensionMismatchException"> if {@code m} is not the
		/// same size as this matrix. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BlockRealMatrix subtract(final BlockRealMatrix m) throws MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual BlockRealMatrix subtract(BlockRealMatrix m)
		{
			// safety check
			MatrixUtils.checkSubtractionCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, columns);
			BlockRealMatrix @out = new BlockRealMatrix(rows, columns);

			// perform subtraction block-wise, to ensure good cache behavior
			for (int blockIndex = 0; blockIndex < @out.blocks.Length; ++blockIndex)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
				double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[blockIndex];
				double[] tBlock = blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mBlock = m.blocks[blockIndex];
				double[] mBlock = m.blocks[blockIndex];
				for (int k = 0; k < outBlock.Length; ++k)
				{
					outBlock[k] = tBlock[k] - mBlock[k];
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix scalarAdd(final double d)
		public override BlockRealMatrix scalarAdd(double d)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, columns);
			BlockRealMatrix @out = new BlockRealMatrix(rows, columns);

			// perform subtraction block-wise, to ensure good cache behavior
			for (int blockIndex = 0; blockIndex < @out.blocks.Length; ++blockIndex)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
				double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[blockIndex];
				double[] tBlock = blocks[blockIndex];
				for (int k = 0; k < outBlock.Length; ++k)
				{
					outBlock[k] = tBlock[k] + d;
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public RealMatrix scalarMultiply(final double d)
		public override RealMatrix scalarMultiply(double d)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, columns);
			BlockRealMatrix @out = new BlockRealMatrix(rows, columns);

			// perform subtraction block-wise, to ensure good cache behavior
			for (int blockIndex = 0; blockIndex < @out.blocks.Length; ++blockIndex)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
				double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[blockIndex];
				double[] tBlock = blocks[blockIndex];
				for (int k = 0; k < outBlock.Length; ++k)
				{
					outBlock[k] = tBlock[k] * d;
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix multiply(final RealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix multiply(RealMatrix m)
		{
			try
			{
				return multiply((BlockRealMatrix) m);
			}
			catch (System.InvalidCastException cce)
			{
				// safety check
				MatrixUtils.checkMultiplicationCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, m.getColumnDimension());
				BlockRealMatrix @out = new BlockRealMatrix(rows, m.ColumnDimension);

				// perform multiplication block-wise, to ensure good cache behavior
				int blockIndex = 0;
				for (int iBlock = 0; iBlock < @out.blockRows; ++iBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
					int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
					int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);

					for (int jBlock = 0; jBlock < @out.blockColumns; ++jBlock)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
						int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, m.getColumnDimension());
						int qEnd = FastMath.min(qStart + BLOCK_SIZE, m.ColumnDimension);

						// select current block
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
						double[] outBlock = @out.blocks[blockIndex];

						// perform multiplication on current block
						for (int kBlock = 0; kBlock < blockColumns; ++kBlock)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kWidth = blockWidth(kBlock);
							int kWidth = blockWidth(kBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[iBlock * blockColumns + kBlock];
							double[] tBlock = blocks[iBlock * blockColumns + kBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rStart = kBlock * BLOCK_SIZE;
							int rStart = kBlock * BLOCK_SIZE;
							int k = 0;
							for (int p = pStart; p < pEnd; ++p)
							{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lStart = (p - pStart) * kWidth;
								int lStart = (p - pStart) * kWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lEnd = lStart + kWidth;
								int lEnd = lStart + kWidth;
								for (int q = qStart; q < qEnd; ++q)
								{
									double sum = 0;
									int r = rStart;
									for (int l = lStart; l < lEnd; ++l)
									{
										sum += tBlock[l] * m.getEntry(r, q);
										++r;
									}
									outBlock[k] += sum;
									++k;
								}
							}
						}
						// go to next block
						++blockIndex;
					}
				}

				return @out;
			}
		}

		/// <summary>
		/// Returns the result of postmultiplying this by {@code m}.
		/// </summary>
		/// <param name="m"> Matrix to postmultiply by. </param>
		/// <returns> {@code this} * m. </returns>
		/// <exception cref="DimensionMismatchException"> if the matrices are not compatible. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public BlockRealMatrix multiply(BlockRealMatrix m) throws org.apache.commons.math3.exception.DimensionMismatchException
		public virtual BlockRealMatrix multiply(BlockRealMatrix m)
		{
			// safety check
			MatrixUtils.checkMultiplicationCompatible(this, m);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, m.columns);
			BlockRealMatrix @out = new BlockRealMatrix(rows, m.columns);

			// perform multiplication block-wise, to ensure good cache behavior
			int blockIndex = 0;
			for (int iBlock = 0; iBlock < @out.blockRows; ++iBlock)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);

				for (int jBlock = 0; jBlock < @out.blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = out.blockWidth(jBlock);
					int jWidth = @out.blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth2 = jWidth + jWidth;
					int jWidth2 = jWidth + jWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth3 = jWidth2 + jWidth;
					int jWidth3 = jWidth2 + jWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth4 = jWidth3 + jWidth;
					int jWidth4 = jWidth3 + jWidth;

					// select current block
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
					double[] outBlock = @out.blocks[blockIndex];

					// perform multiplication on current block
					for (int kBlock = 0; kBlock < blockColumns; ++kBlock)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int kWidth = blockWidth(kBlock);
						int kWidth = blockWidth(kBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[iBlock * blockColumns + kBlock];
						double[] tBlock = blocks[iBlock * blockColumns + kBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] mBlock = m.blocks[kBlock * m.blockColumns + jBlock];
						double[] mBlock = m.blocks[kBlock * m.blockColumns + jBlock];
						int k = 0;
						for (int p = pStart; p < pEnd; ++p)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lStart = (p - pStart) * kWidth;
							int lStart = (p - pStart) * kWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lEnd = lStart + kWidth;
							int lEnd = lStart + kWidth;
							for (int nStart = 0; nStart < jWidth; ++nStart)
							{
								double sum = 0;
								int l = lStart;
								int n = nStart;
								while (l < lEnd - 3)
								{
									sum += tBlock[l] * mBlock[n] + tBlock[l + 1] * mBlock[n + jWidth] + tBlock[l + 2] * mBlock[n + jWidth2] + tBlock[l + 3] * mBlock[n + jWidth3];
									l += 4;
									n += jWidth4;
								}
								while (l < lEnd)
								{
									sum += tBlock[l++] * mBlock[n];
									n += jWidth;
								}
								outBlock[k] += sum;
								++k;
							}
						}
					}
					// go to next block
					++blockIndex;
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double[][] Data
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[][] data = new double[getRowDimension()][getColumnDimension()];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[RowDimension][ColumnDimension];
				double[][] data = RectangularArrays.ReturnRectangularDoubleArray(RowDimension, ColumnDimension);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int lastColumns = columns - (blockColumns - 1) * BLOCK_SIZE;
				int lastColumns = columns - (blockColumns - 1) * BLOCK_SIZE;
    
				for (int iBlock = 0; iBlock < blockRows; ++iBlock)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
					int pStart = iBlock * BLOCK_SIZE;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
					int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
					int regularPos = 0;
					int lastPos = 0;
					for (int p = pStart; p < pEnd; ++p)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] dataP = data[p];
						double[] dataP = data[p];
						int blockIndex = iBlock * blockColumns;
						int dataPos = 0;
						for (int jBlock = 0; jBlock < blockColumns - 1; ++jBlock)
						{
							Array.Copy(blocks[blockIndex++], regularPos, dataP, dataPos, BLOCK_SIZE);
							dataPos += BLOCK_SIZE;
						}
						Array.Copy(blocks[blockIndex], lastPos, dataP, dataPos, lastColumns);
						regularPos += BLOCK_SIZE;
						lastPos += lastColumns;
					}
				}
    
				return data;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double Norm
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] colSums = new double[BLOCK_SIZE];
				double[] colSums = new double[BLOCK_SIZE];
				double maxColSum = 0;
				for (int jBlock = 0; jBlock < blockColumns; jBlock++)
				{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
					int jWidth = blockWidth(jBlock);
					Arrays.fill(colSums, 0, jWidth, 0.0);
					for (int iBlock = 0; iBlock < blockRows; ++iBlock)
					{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
						int iHeight = blockHeight(iBlock);
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
						double[] block = blocks[iBlock * blockColumns + jBlock];
						for (int j = 0; j < jWidth; ++j)
						{
							double sum = 0;
							for (int i = 0; i < iHeight; ++i)
							{
								sum += FastMath.abs(block[i * jWidth + j]);
							}
							colSums[j] += sum;
						}
					}
					for (int j = 0; j < jWidth; ++j)
					{
						maxColSum = FastMath.max(maxColSum, colSums[j]);
					}
				}
				return maxColSum;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override double FrobeniusNorm
		{
			get
			{
				double sum2 = 0;
				for (int blockIndex = 0; blockIndex < blocks.Length; ++blockIndex)
				{
					foreach (double entry in blocks[blockIndex])
					{
						sum2 += entry * entry;
					}
				}
				return FastMath.sqrt(sum2);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix getSubMatrix(final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix getSubMatrix(int startRow, int endRow, int startColumn, int endColumn)
		{
			// safety checks
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);

			// create the output matrix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(endRow - startRow + 1, endColumn - startColumn + 1);
			BlockRealMatrix @out = new BlockRealMatrix(endRow - startRow + 1, endColumn - startColumn + 1);

			// compute blocks shifts
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockStartRow = startRow / BLOCK_SIZE;
			int blockStartRow = startRow / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int rowsShift = startRow % BLOCK_SIZE;
			int rowsShift = startRow % BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockStartColumn = startColumn / BLOCK_SIZE;
			int blockStartColumn = startColumn / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int columnsShift = startColumn % BLOCK_SIZE;
			int columnsShift = startColumn % BLOCK_SIZE;

			// perform extraction block-wise, to ensure good cache behavior
			int pBlock = blockStartRow;
			for (int iBlock = 0; iBlock < @out.blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = out.blockHeight(iBlock);
				int iHeight = @out.blockHeight(iBlock);
				int qBlock = blockStartColumn;
				for (int jBlock = 0; jBlock < @out.blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = out.blockWidth(jBlock);
					int jWidth = @out.blockWidth(jBlock);

					// handle one block of the output matrix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int outIndex = iBlock * out.blockColumns + jBlock;
					int outIndex = iBlock * @out.blockColumns + jBlock;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[outIndex];
					double[] outBlock = @out.blocks[outIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int index = pBlock * blockColumns + qBlock;
					int index = pBlock * blockColumns + qBlock;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int width = blockWidth(qBlock);
					int width = blockWidth(qBlock);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int heightExcess = iHeight + rowsShift - BLOCK_SIZE;
					int heightExcess = iHeight + rowsShift - BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int widthExcess = jWidth + columnsShift - BLOCK_SIZE;
					int widthExcess = jWidth + columnsShift - BLOCK_SIZE;
					if (heightExcess > 0)
					{
						// the submatrix block spans on two blocks rows from the original matrix
						if (widthExcess > 0)
						{
							// the submatrix block spans on two blocks columns from the original matrix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int width2 = blockWidth(qBlock + 1);
							int width2 = blockWidth(qBlock + 1);
							copyBlockPart(blocks[index], width, rowsShift, BLOCK_SIZE, columnsShift, BLOCK_SIZE, outBlock, jWidth, 0, 0);
							copyBlockPart(blocks[index + 1], width2, rowsShift, BLOCK_SIZE, 0, widthExcess, outBlock, jWidth, 0, jWidth - widthExcess);
							copyBlockPart(blocks[index + blockColumns], width, 0, heightExcess, columnsShift, BLOCK_SIZE, outBlock, jWidth, iHeight - heightExcess, 0);
							copyBlockPart(blocks[index + blockColumns + 1], width2, 0, heightExcess, 0, widthExcess, outBlock, jWidth, iHeight - heightExcess, jWidth - widthExcess);
						}
						else
						{
							// the submatrix block spans on one block column from the original matrix
							copyBlockPart(blocks[index], width, rowsShift, BLOCK_SIZE, columnsShift, jWidth + columnsShift, outBlock, jWidth, 0, 0);
							copyBlockPart(blocks[index + blockColumns], width, 0, heightExcess, columnsShift, jWidth + columnsShift, outBlock, jWidth, iHeight - heightExcess, 0);
						}
					}
					else
					{
						// the submatrix block spans on one block row from the original matrix
						if (widthExcess > 0)
						{
							// the submatrix block spans on two blocks columns from the original matrix
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int width2 = blockWidth(qBlock + 1);
							int width2 = blockWidth(qBlock + 1);
							copyBlockPart(blocks[index], width, rowsShift, iHeight + rowsShift, columnsShift, BLOCK_SIZE, outBlock, jWidth, 0, 0);
							copyBlockPart(blocks[index + 1], width2, rowsShift, iHeight + rowsShift, 0, widthExcess, outBlock, jWidth, 0, jWidth - widthExcess);
						}
						else
						{
							// the submatrix block spans on one block column from the original matrix
							copyBlockPart(blocks[index], width, rowsShift, iHeight + rowsShift, columnsShift, jWidth + columnsShift, outBlock, jWidth, 0, 0);
						}
					}
					++qBlock;
				}
				++pBlock;
			}

			return @out;
		}

		/// <summary>
		/// Copy a part of a block into another one
		/// <p>This method can be called only when the specified part fits in both
		/// blocks, no verification is done here.</p> </summary>
		/// <param name="srcBlock"> source block </param>
		/// <param name="srcWidth"> source block width (<seealso cref="#BLOCK_SIZE"/> or smaller) </param>
		/// <param name="srcStartRow"> start row in the source block </param>
		/// <param name="srcEndRow"> end row (exclusive) in the source block </param>
		/// <param name="srcStartColumn"> start column in the source block </param>
		/// <param name="srcEndColumn"> end column (exclusive) in the source block </param>
		/// <param name="dstBlock"> destination block </param>
		/// <param name="dstWidth"> destination block width (<seealso cref="#BLOCK_SIZE"/> or smaller) </param>
		/// <param name="dstStartRow"> start row in the destination block </param>
		/// <param name="dstStartColumn"> start column in the destination block </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void copyBlockPart(final double[] srcBlock, final int srcWidth, final int srcStartRow, final int srcEndRow, final int srcStartColumn, final int srcEndColumn, final double[] dstBlock, final int dstWidth, final int dstStartRow, final int dstStartColumn)
		private void copyBlockPart(double[] srcBlock, int srcWidth, int srcStartRow, int srcEndRow, int srcStartColumn, int srcEndColumn, double[] dstBlock, int dstWidth, int dstStartRow, int dstStartColumn)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int length = srcEndColumn - srcStartColumn;
			int length = srcEndColumn - srcStartColumn;
			int srcPos = srcStartRow * srcWidth + srcStartColumn;
			int dstPos = dstStartRow * dstWidth + dstStartColumn;
			for (int srcRow = srcStartRow; srcRow < srcEndRow; ++srcRow)
			{
				Array.Copy(srcBlock, srcPos, dstBlock, dstPos, length);
				srcPos += srcWidth;
				dstPos += dstWidth;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setSubMatrix(final double[][] subMatrix, final int row, final int column) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setSubMatrix(double[][] subMatrix, int row, int column)
		{
			// safety checks
			MathUtils.checkNotNull(subMatrix);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int refLength = subMatrix[0].length;
			int refLength = subMatrix[0].Length;
			if (refLength == 0)
			{
				throw new NoDataException(LocalizedFormats.AT_LEAST_ONE_COLUMN);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int endRow = row + subMatrix.length - 1;
			int endRow = row + subMatrix.Length - 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int endColumn = column + refLength - 1;
			int endColumn = column + refLength - 1;
			MatrixUtils.checkSubMatrixIndex(this, row, endRow, column, endColumn);
			foreach (double[] subRow in subMatrix)
			{
				if (subRow.length != refLength)
				{
					throw new DimensionMismatchException(refLength, subRow.length);
				}
			}

			// compute blocks bounds
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockStartRow = row / BLOCK_SIZE;
			int blockStartRow = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockEndRow = (endRow + BLOCK_SIZE) / BLOCK_SIZE;
			int blockEndRow = (endRow + BLOCK_SIZE) / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockStartColumn = column / BLOCK_SIZE;
			int blockStartColumn = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int blockEndColumn = (endColumn + BLOCK_SIZE) / BLOCK_SIZE;
			int blockEndColumn = (endColumn + BLOCK_SIZE) / BLOCK_SIZE;

			// perform copy block-wise, to ensure good cache behavior
			for (int iBlock = blockStartRow; iBlock < blockEndRow; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int firstRow = iBlock * BLOCK_SIZE;
				int firstRow = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iStart = org.apache.commons.math3.util.FastMath.max(row, firstRow);
				int iStart = FastMath.max(row, firstRow);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iEnd = org.apache.commons.math3.util.FastMath.min(endRow + 1, firstRow + iHeight);
				int iEnd = FastMath.min(endRow + 1, firstRow + iHeight);

				for (int jBlock = blockStartColumn; jBlock < blockEndColumn; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
					int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int firstColumn = jBlock * BLOCK_SIZE;
					int firstColumn = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jStart = org.apache.commons.math3.util.FastMath.max(column, firstColumn);
					int jStart = FastMath.max(column, firstColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jEnd = org.apache.commons.math3.util.FastMath.min(endColumn + 1, firstColumn + jWidth);
					int jEnd = FastMath.min(endColumn + 1, firstColumn + jWidth);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jLength = jEnd - jStart;
					int jLength = jEnd - jStart;

					// handle one block, row by row
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
					double[] block = blocks[iBlock * blockColumns + jBlock];
					for (int i = iStart; i < iEnd; ++i)
					{
						Array.Copy(subMatrix[i - row], jStart - column, block, (i - firstRow) * jWidth + (jStart - firstColumn), jLength);
					}

				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix getRowMatrix(final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix getRowMatrix(int row)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(1, columns);
			BlockRealMatrix @out = new BlockRealMatrix(1, columns);

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iRow = row - iBlock * BLOCK_SIZE;
			int iRow = row - iBlock * BLOCK_SIZE;
			int outBlockIndex = 0;
			int outIndex = 0;
			double[] outBlock = @out.blocks[outBlockIndex];
			for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
				int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int available = outBlock.length - outIndex;
				int available = outBlock.Length - outIndex;
				if (jWidth > available)
				{
					Array.Copy(block, iRow * jWidth, outBlock, outIndex, available);
					outBlock = @out.blocks[++outBlockIndex];
					Array.Copy(block, iRow * jWidth, outBlock, 0, jWidth - available);
					outIndex = jWidth - available;
				}
				else
				{
					Array.Copy(block, iRow * jWidth, outBlock, outIndex, jWidth);
					outIndex += jWidth;
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setRowMatrix(final int row, final RealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setRowMatrix(int row, RealMatrix matrix)
		{
			try
			{
				setRowMatrix(row, (BlockRealMatrix) matrix);
			}
			catch (System.InvalidCastException cce)
			{
				base.setRowMatrix(row, matrix);
			}
		}

		/// <summary>
		/// Sets the entries in row number <code>row</code>
		/// as a row matrix.  Row indices start at 0.
		/// </summary>
		/// <param name="row"> the row to be set </param>
		/// <param name="matrix"> row matrix (must have one row and the same number of columns
		/// as the instance) </param>
		/// <exception cref="OutOfRangeException"> if the specified row index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the matrix dimensions do
		/// not match one instance row. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setRowMatrix(final int row, final BlockRealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setRowMatrix(int row, BlockRealMatrix matrix)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if ((matrix.RowDimension != 1) || (matrix.ColumnDimension != nCols))
			{
				throw new MatrixDimensionMismatchException(matrix.RowDimension, matrix.ColumnDimension, 1, nCols);
			}

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iRow = row - iBlock * BLOCK_SIZE;
			int iRow = row - iBlock * BLOCK_SIZE;
			int mBlockIndex = 0;
			int mIndex = 0;
			double[] mBlock = matrix.blocks[mBlockIndex];
			for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
				int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int available = mBlock.length - mIndex;
				int available = mBlock.Length - mIndex;
				if (jWidth > available)
				{
					Array.Copy(mBlock, mIndex, block, iRow * jWidth, available);
					mBlock = matrix.blocks[++mBlockIndex];
					Array.Copy(mBlock, 0, block, iRow * jWidth, jWidth - available);
					mIndex = jWidth - available;
				}
				else
				{
					Array.Copy(mBlock, mIndex, block, iRow * jWidth, jWidth);
					mIndex += jWidth;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public BlockRealMatrix getColumnMatrix(final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override BlockRealMatrix getColumnMatrix(int column)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(rows, 1);
			BlockRealMatrix @out = new BlockRealMatrix(rows, 1);

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jColumn = column - jBlock * BLOCK_SIZE;
			int jColumn = column - jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
			int jWidth = blockWidth(jBlock);
			int outBlockIndex = 0;
			int outIndex = 0;
			double[] outBlock = @out.blocks[outBlockIndex];
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				for (int i = 0; i < iHeight; ++i)
				{
					if (outIndex >= outBlock.Length)
					{
						outBlock = @out.blocks[++outBlockIndex];
						outIndex = 0;
					}
					outBlock[outIndex++] = block[i * jWidth + jColumn];
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setColumnMatrix(final int column, final RealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setColumnMatrix(int column, RealMatrix matrix)
		{
			try
			{
				setColumnMatrix(column, (BlockRealMatrix) matrix);
			}
			catch (System.InvalidCastException cce)
			{
				base.setColumnMatrix(column, matrix);
			}
		}

		/// <summary>
		/// Sets the entries in column number <code>column</code>
		/// as a column matrix.  Column indices start at 0.
		/// </summary>
		/// <param name="column"> the column to be set </param>
		/// <param name="matrix"> column matrix (must have one column and the same number of rows
		/// as the instance) </param>
		/// <exception cref="OutOfRangeException"> if the specified column index is invalid. </exception>
		/// <exception cref="MatrixDimensionMismatchException"> if the matrix dimensions do
		/// not match one instance column. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void setColumnMatrix(final int column, final BlockRealMatrix matrix) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		internal virtual void setColumnMatrix(int column, BlockRealMatrix matrix)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
			if ((matrix.RowDimension != nRows) || (matrix.ColumnDimension != 1))
			{
				throw new MatrixDimensionMismatchException(matrix.RowDimension, matrix.ColumnDimension, nRows, 1);
			}

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jColumn = column - jBlock * BLOCK_SIZE;
			int jColumn = column - jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
			int jWidth = blockWidth(jBlock);
			int mBlockIndex = 0;
			int mIndex = 0;
			double[] mBlock = matrix.blocks[mBlockIndex];
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				for (int i = 0; i < iHeight; ++i)
				{
					if (mIndex >= mBlock.Length)
					{
						mBlock = matrix.blocks[++mBlockIndex];
						mIndex = 0;
					}
					block[i * jWidth + jColumn] = mBlock[mIndex++];
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector getRowVector(final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector getRowVector(int row)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outData = new double[columns];
			double[] outData = new double[columns];

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iRow = row - iBlock * BLOCK_SIZE;
			int iRow = row - iBlock * BLOCK_SIZE;
			int outIndex = 0;
			for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
				int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				Array.Copy(block, iRow * jWidth, outData, outIndex, jWidth);
				outIndex += jWidth;
			}

			return new ArrayRealVector(outData, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setRowVector(final int row, final RealVector vector) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setRowVector(int row, RealVector vector)
		{
			try
			{
				setRow(row, ((ArrayRealVector) vector).DataRef);
			}
			catch (System.InvalidCastException cce)
			{
				base.setRowVector(row, vector);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public RealVector getColumnVector(final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override RealVector getColumnVector(int column)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outData = new double[rows];
			double[] outData = new double[rows];

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jColumn = column - jBlock * BLOCK_SIZE;
			int jColumn = column - jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
			int jWidth = blockWidth(jBlock);
			int outIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				for (int i = 0; i < iHeight; ++i)
				{
					outData[outIndex++] = block[i * jWidth + jColumn];
				}
			}

			return new ArrayRealVector(outData, false);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setColumnVector(final int column, final RealVector vector) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setColumnVector(int column, RealVector vector)
		{
			try
			{
				setColumn(column, ((ArrayRealVector) vector).DataRef);
			}
			catch (System.InvalidCastException cce)
			{
				base.setColumnVector(column, vector);
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] getRow(final int row) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] getRow(int row)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[columns];
			double[] @out = new double[columns];

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iRow = row - iBlock * BLOCK_SIZE;
			int iRow = row - iBlock * BLOCK_SIZE;
			int outIndex = 0;
			for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
				int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				Array.Copy(block, iRow * jWidth, @out, outIndex, jWidth);
				outIndex += jWidth;
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setRow(final int row, final double[] array) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setRow(int row, double[] array)
		{
			MatrixUtils.checkRowIndex(this, row);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
			if (array.Length != nCols)
			{
				throw new MatrixDimensionMismatchException(1, array.Length, 1, nCols);
			}

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iRow = row - iBlock * BLOCK_SIZE;
			int iRow = row - iBlock * BLOCK_SIZE;
			int outIndex = 0;
			for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
				int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				Array.Copy(array, outIndex, block, iRow * jWidth, jWidth);
				outIndex += jWidth;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] getColumn(final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] getColumn(int column)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[rows];
			double[] @out = new double[rows];

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jColumn = column - jBlock * BLOCK_SIZE;
			int jColumn = column - jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
			int jWidth = blockWidth(jBlock);
			int outIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				for (int i = 0; i < iHeight; ++i)
				{
					@out[outIndex++] = block[i * jWidth + jColumn];
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setColumn(final int column, final double[] array) throws org.apache.commons.math3.exception.OutOfRangeException, MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setColumn(int column, double[] array)
		{
			MatrixUtils.checkColumnIndex(this, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
			if (array.Length != nRows)
			{
				throw new MatrixDimensionMismatchException(array.Length, 1, nRows, 1);
			}

			// perform copy block-wise, to ensure good cache behavior
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jColumn = column - jBlock * BLOCK_SIZE;
			int jColumn = column - jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
			int jWidth = blockWidth(jBlock);
			int outIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iHeight = blockHeight(iBlock);
				int iHeight = blockHeight(iBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
				double[] block = blocks[iBlock * blockColumns + jBlock];
				for (int i = 0; i < iHeight; ++i)
				{
					block[i * jWidth + jColumn] = array[outIndex++];
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getEntry(final int row, final int column) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double getEntry(int row, int column)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			return blocks[iBlock * blockColumns + jBlock][k];
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setEntry(final int row, final int column, final double value) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void setEntry(int row, int column, double value)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			blocks[iBlock * blockColumns + jBlock][k] = value;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void addToEntry(final int row, final int column, final double increment) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void addToEntry(int row, int column, double increment)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			blocks[iBlock * blockColumns + jBlock][k] += increment;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void multiplyEntry(final int row, final int column, final double factor) throws org.apache.commons.math3.exception.OutOfRangeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override void multiplyEntry(int row, int column, double factor)
		{
			MatrixUtils.checkMatrixIndex(this, row, column);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int iBlock = row / BLOCK_SIZE;
			int iBlock = row / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jBlock = column / BLOCK_SIZE;
			int jBlock = column / BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			int k = (row - iBlock * BLOCK_SIZE) * blockWidth(jBlock) + (column - jBlock * BLOCK_SIZE);
			blocks[iBlock * blockColumns + jBlock][k] *= factor;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override BlockRealMatrix transpose()
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nRows = getRowDimension();
			int nRows = RowDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int nCols = getColumnDimension();
			int nCols = ColumnDimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final BlockRealMatrix out = new BlockRealMatrix(nCols, nRows);
			BlockRealMatrix @out = new BlockRealMatrix(nCols, nRows);

			// perform transpose block-wise, to ensure good cache behavior
			int blockIndex = 0;
			for (int iBlock = 0; iBlock < blockColumns; ++iBlock)
			{
				for (int jBlock = 0; jBlock < blockRows; ++jBlock)
				{
					// transpose current block
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] outBlock = out.blocks[blockIndex];
					double[] outBlock = @out.blocks[blockIndex];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tBlock = blocks[jBlock * blockColumns + iBlock];
					double[] tBlock = blocks[jBlock * blockColumns + iBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
					int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, columns);
					int pEnd = FastMath.min(pStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
					int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, rows);
					int qEnd = FastMath.min(qStart + BLOCK_SIZE, rows);
					int k = 0;
					for (int p = pStart; p < pEnd; ++p)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int lInc = pEnd - pStart;
						int lInc = pEnd - pStart;
						int l = p - pStart;
						for (int q = qStart; q < qEnd; ++q)
						{
							outBlock[k] = tBlock[l];
							++k;
							l += lInc;
						}
					}
					// go to next block
					++blockIndex;
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int RowDimension
		{
			get
			{
				return rows;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int ColumnDimension
		{
			get
			{
				return columns;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] operate(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] operate(double[] v)
		{
			if (v.Length != columns)
			{
				throw new DimensionMismatchException(v.Length, columns);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[rows];
			double[] @out = new double[rows];

			// perform multiplication block-wise, to ensure good cache behavior
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
				for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
					double[] block = blocks[iBlock * blockColumns + jBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
					int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
					int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
					int k = 0;
					for (int p = pStart; p < pEnd; ++p)
					{
						double sum = 0;
						int q = qStart;
						while (q < qEnd - 3)
						{
							sum += block[k] * v[q] + block[k + 1] * v[q + 1] + block[k + 2] * v[q + 2] + block[k + 3] * v[q + 3];
							k += 4;
							q += 4;
						}
						while (q < qEnd)
						{
							sum += block[k++] * v[q++];
						}
						@out[p] += sum;
					}
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double[] preMultiply(final double[] v) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double[] preMultiply(double[] v)
		{
			if (v.Length != rows)
			{
				throw new DimensionMismatchException(v.Length, rows);
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] out = new double[columns];
			double[] @out = new double[columns];

			// perform multiplication block-wise, to ensure good cache behavior
			for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
				int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth2 = jWidth + jWidth;
				int jWidth2 = jWidth + jWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth3 = jWidth2 + jWidth;
				int jWidth3 = jWidth2 + jWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth4 = jWidth3 + jWidth;
				int jWidth4 = jWidth3 + jWidth;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
				int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
				int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
				for (int iBlock = 0; iBlock < blockRows; ++iBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
					double[] block = blocks[iBlock * blockColumns + jBlock];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
					int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
					int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
					for (int q = qStart; q < qEnd; ++q)
					{
						int k = q - qStart;
						double sum = 0;
						int p = pStart;
						while (p < pEnd - 3)
						{
							sum += block[k] * v[p] + block[k + jWidth] * v[p + 1] + block[k + jWidth2] * v[p + 2] + block[k + jWidth3] * v[p + 3];
							k += jWidth4;
							p += 4;
						}
						while (p < pEnd)
						{
							sum += block[k] * v[p++];
							k += jWidth;
						}
						@out[q] += sum;
					}
				}
			}

			return @out;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixChangingVisitor visitor)
		public override double walkInRowOrder(RealMatrixChangingVisitor visitor)
		{
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
				for (int p = pStart; p < pEnd; ++p)
				{
					for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
						int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
						int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
						int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
						double[] block = blocks[iBlock * blockColumns + jBlock];
						int k = (p - pStart) * jWidth;
						for (int q = qStart; q < qEnd; ++q)
						{
							block[k] = visitor.visit(p, q, block[k]);
							++k;
						}
					}
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixPreservingVisitor visitor)
		public override double walkInRowOrder(RealMatrixPreservingVisitor visitor)
		{
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
				for (int p = pStart; p < pEnd; ++p)
				{
					for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
						int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
						int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
						int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
						double[] block = blocks[iBlock * blockColumns + jBlock];
						int k = (p - pStart) * jWidth;
						for (int q = qStart; q < qEnd; ++q)
						{
							visitor.visit(p, q, block[k]);
							++k;
						}
					}
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInRowOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(rows, columns, startRow, endRow, startColumn, endColumn);
			for (int iBlock = startRow / BLOCK_SIZE; iBlock < 1 + endRow / BLOCK_SIZE; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p0 = iBlock * BLOCK_SIZE;
				int p0 = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = org.apache.commons.math3.util.FastMath.max(startRow, p0);
				int pStart = FastMath.max(startRow, p0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				int pEnd = FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				for (int p = pStart; p < pEnd; ++p)
				{
					for (int jBlock = startColumn / BLOCK_SIZE; jBlock < 1 + endColumn / BLOCK_SIZE; ++jBlock)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
						int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int q0 = jBlock * BLOCK_SIZE;
						int q0 = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = org.apache.commons.math3.util.FastMath.max(startColumn, q0);
						int qStart = FastMath.max(startColumn, q0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
						int qEnd = FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
						double[] block = blocks[iBlock * blockColumns + jBlock];
						int k = (p - p0) * jWidth + qStart - q0;
						for (int q = qStart; q < qEnd; ++q)
						{
							block[k] = visitor.visit(p, q, block[k]);
							++k;
						}
					}
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInRowOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInRowOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(rows, columns, startRow, endRow, startColumn, endColumn);
			for (int iBlock = startRow / BLOCK_SIZE; iBlock < 1 + endRow / BLOCK_SIZE; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p0 = iBlock * BLOCK_SIZE;
				int p0 = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = org.apache.commons.math3.util.FastMath.max(startRow, p0);
				int pStart = FastMath.max(startRow, p0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				int pEnd = FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				for (int p = pStart; p < pEnd; ++p)
				{
					for (int jBlock = startColumn / BLOCK_SIZE; jBlock < 1 + endColumn / BLOCK_SIZE; ++jBlock)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
						int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int q0 = jBlock * BLOCK_SIZE;
						int q0 = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = org.apache.commons.math3.util.FastMath.max(startColumn, q0);
						int qStart = FastMath.max(startColumn, q0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
						int qEnd = FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
						double[] block = blocks[iBlock * blockColumns + jBlock];
						int k = (p - p0) * jWidth + qStart - q0;
						for (int q = qStart; q < qEnd; ++q)
						{
							visitor.visit(p, q, block[k]);
							++k;
						}
					}
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealMatrixChangingVisitor visitor)
		public override double walkInOptimizedOrder(RealMatrixChangingVisitor visitor)
		{
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			int blockIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
				for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
					int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
					int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[blockIndex];
					double[] block = blocks[blockIndex];
					int k = 0;
					for (int p = pStart; p < pEnd; ++p)
					{
						for (int q = qStart; q < qEnd; ++q)
						{
							block[k] = visitor.visit(p, q, block[k]);
							++k;
						}
					}
					++blockIndex;
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealMatrixPreservingVisitor visitor)
		public override double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor)
		{
			visitor.start(rows, columns, 0, rows - 1, 0, columns - 1);
			int blockIndex = 0;
			for (int iBlock = 0; iBlock < blockRows; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = iBlock * BLOCK_SIZE;
				int pStart = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min(pStart + BLOCK_SIZE, rows);
				int pEnd = FastMath.min(pStart + BLOCK_SIZE, rows);
				for (int jBlock = 0; jBlock < blockColumns; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = jBlock * BLOCK_SIZE;
					int qStart = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min(qStart + BLOCK_SIZE, columns);
					int qEnd = FastMath.min(qStart + BLOCK_SIZE, columns);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[blockIndex];
					double[] block = blocks[blockIndex];
					int k = 0;
					for (int p = pStart; p < pEnd; ++p)
					{
						for (int q = qStart; q < qEnd; ++q)
						{
							visitor.visit(p, q, block[k]);
							++k;
						}
					}
					++blockIndex;
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealMatrixChangingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInOptimizedOrder(RealMatrixChangingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(rows, columns, startRow, endRow, startColumn, endColumn);
			for (int iBlock = startRow / BLOCK_SIZE; iBlock < 1 + endRow / BLOCK_SIZE; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p0 = iBlock * BLOCK_SIZE;
				int p0 = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = org.apache.commons.math3.util.FastMath.max(startRow, p0);
				int pStart = FastMath.max(startRow, p0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				int pEnd = FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				for (int jBlock = startColumn / BLOCK_SIZE; jBlock < 1 + endColumn / BLOCK_SIZE; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
					int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int q0 = jBlock * BLOCK_SIZE;
					int q0 = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = org.apache.commons.math3.util.FastMath.max(startColumn, q0);
					int qStart = FastMath.max(startColumn, q0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
					int qEnd = FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
					double[] block = blocks[iBlock * blockColumns + jBlock];
					for (int p = pStart; p < pEnd; ++p)
					{
						int k = (p - p0) * jWidth + qStart - q0;
						for (int q = qStart; q < qEnd; ++q)
						{
							block[k] = visitor.visit(p, q, block[k]);
							++k;
						}
					}
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double walkInOptimizedOrder(final RealMatrixPreservingVisitor visitor, final int startRow, final int endRow, final int startColumn, final int endColumn) throws org.apache.commons.math3.exception.OutOfRangeException, org.apache.commons.math3.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override double walkInOptimizedOrder(RealMatrixPreservingVisitor visitor, int startRow, int endRow, int startColumn, int endColumn)
		{
			MatrixUtils.checkSubMatrixIndex(this, startRow, endRow, startColumn, endColumn);
			visitor.start(rows, columns, startRow, endRow, startColumn, endColumn);
			for (int iBlock = startRow / BLOCK_SIZE; iBlock < 1 + endRow / BLOCK_SIZE; ++iBlock)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p0 = iBlock * BLOCK_SIZE;
				int p0 = iBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pStart = org.apache.commons.math3.util.FastMath.max(startRow, p0);
				int pStart = FastMath.max(startRow, p0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int pEnd = org.apache.commons.math3.util.FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				int pEnd = FastMath.min((iBlock + 1) * BLOCK_SIZE, 1 + endRow);
				for (int jBlock = startColumn / BLOCK_SIZE; jBlock < 1 + endColumn / BLOCK_SIZE; ++jBlock)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int jWidth = blockWidth(jBlock);
					int jWidth = blockWidth(jBlock);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int q0 = jBlock * BLOCK_SIZE;
					int q0 = jBlock * BLOCK_SIZE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qStart = org.apache.commons.math3.util.FastMath.max(startColumn, q0);
					int qStart = FastMath.max(startColumn, q0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int qEnd = org.apache.commons.math3.util.FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
					int qEnd = FastMath.min((jBlock + 1) * BLOCK_SIZE, 1 + endColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] block = blocks[iBlock * blockColumns + jBlock];
					double[] block = blocks[iBlock * blockColumns + jBlock];
					for (int p = pStart; p < pEnd; ++p)
					{
						int k = (p - p0) * jWidth + qStart - q0;
						for (int q = qStart; q < qEnd; ++q)
						{
							visitor.visit(p, q, block[k]);
							++k;
						}
					}
				}
			}
			return visitor.end();
		}

		/// <summary>
		/// Get the height of a block. </summary>
		/// <param name="blockRow"> row index (in block sense) of the block </param>
		/// <returns> height (number of rows) of the block </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int blockHeight(final int blockRow)
		private int blockHeight(int blockRow)
		{
			return (blockRow == blockRows - 1) ? rows - blockRow * BLOCK_SIZE : BLOCK_SIZE;
		}

		/// <summary>
		/// Get the width of a block. </summary>
		/// <param name="blockColumn"> column index (in block sense) of the block </param>
		/// <returns> width (number of columns) of the block </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private int blockWidth(final int blockColumn)
		private int blockWidth(int blockColumn)
		{
			return (blockColumn == blockColumns - 1) ? columns - blockColumn * BLOCK_SIZE : BLOCK_SIZE;
		}
	}

}