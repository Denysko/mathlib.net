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
namespace org.apache.commons.math3.analysis.differentiation
{


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MathArithmeticException = org.apache.commons.math3.exception.MathArithmeticException;
	using MathInternalError = org.apache.commons.math3.exception.MathInternalError;
	using NotPositiveException = org.apache.commons.math3.exception.NotPositiveException;
	using NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException;
	using CombinatoricsUtils = org.apache.commons.math3.util.CombinatoricsUtils;
	using FastMath = org.apache.commons.math3.util.FastMath;
	using MathArrays = org.apache.commons.math3.util.MathArrays;

	/// <summary>
	/// Class holding "compiled" computation rules for derivative structures.
	/// <p>This class implements the computation rules described in Dan Kalman's paper <a
	/// href="http://www1.american.edu/cas/mathstat/People/kalman/pdffiles/mmgautodiff.pdf">Doubly
	/// Recursive Multivariate Automatic Differentiation</a>, Mathematics Magazine, vol. 75,
	/// no. 3, June 2002. However, in order to avoid performances bottlenecks, the recursive
	/// rules are "compiled" once in an unfold form. This class does this recursion unrolling
	/// and stores the computation rules as simple loops with pre-computed indirection arrays.</p>
	/// <p>
	/// This class maps all derivative computation into single dimension arrays that hold the
	/// value and partial derivatives. The class does not hold these arrays, which remains under
	/// the responsibility of the caller. For each combination of number of free parameters and
	/// derivation order, only one compiler is necessary, and this compiler will be used to
	/// perform computations on all arrays provided to it, which can represent hundreds or
	/// thousands of different parameters kept together with all theur partial derivatives.
	/// </p>
	/// <p>
	/// The arrays on which compilers operate contain only the partial derivatives together
	/// with the 0<sup>th</sup> derivative, i.e. the value. The partial derivatives are stored in
	/// a compiler-specific order, which can be retrieved using methods {@link
	/// #getPartialDerivativeIndex(int...) getPartialDerivativeIndex} and {@link
	/// #getPartialDerivativeOrders(int)}. The value is guaranteed to be stored as the first element
	/// (i.e. the <seealso cref="#getPartialDerivativeIndex(int...) getPartialDerivativeIndex"/> method returns
	/// 0 when called with 0 for all derivation orders and {@link #getPartialDerivativeOrders(int)
	/// getPartialDerivativeOrders} returns an array filled with 0 when called with 0 as the index).
	/// </p>
	/// <p>
	/// Note that the ordering changes with number of parameters and derivation order. For example
	/// given 2 parameters x and y, df/dy is stored at index 2 when derivation order is set to 1 (in
	/// this case the array has three elements: f, df/dx and df/dy). If derivation order is set to
	/// 2, then df/dy will be stored at index 3 (in this case the array has six elements: f, df/dx,
	/// df/dxdx, df/dy, df/dxdy and df/dydy).
	/// </p>
	/// <p>
	/// Given this structure, users can perform some simple operations like adding, subtracting
	/// or multiplying constants and negating the elements by themselves, knowing if they want to
	/// mutate their array or create a new array. These simple operations are not provided by
	/// the compiler. The compiler provides only the more complex operations between several arrays.
	/// </p>
	/// <p>This class is mainly used as the engine for scalar variable <seealso cref="DerivativeStructure"/>.
	/// It can also be used directly to hold several variables in arrays for more complex data
	/// structures. User can for example store a vector of n variables depending on three x, y
	/// and z free parameters in one array as follows:
	/// <pre>
	///   // parameter 0 is x, parameter 1 is y, parameter 2 is z
	///   int parameters = 3;
	///   DSCompiler compiler = DSCompiler.getCompiler(parameters, order);
	///   int size = compiler.getSize();
	/// 
	///   // pack all elements in a single array
	///   double[] array = new double[n * size];
	///   for (int i = 0; i < n; ++i) {
	/// 
	///     // we know value is guaranteed to be the first element
	///     array[i * size] = v[i];
	/// 
	///     // we don't know where first derivatives are stored, so we ask the compiler
	///     array[i * size + compiler.getPartialDerivativeIndex(1, 0, 0) = dvOnDx[i][0];
	///     array[i * size + compiler.getPartialDerivativeIndex(0, 1, 0) = dvOnDy[i][0];
	///     array[i * size + compiler.getPartialDerivativeIndex(0, 0, 1) = dvOnDz[i][0];
	/// 
	///     // we let all higher order derivatives set to 0
	/// 
	///   }
	/// </pre>
	/// Then in another function, user can perform some operations on all elements stored
	/// in the single array, such as a simple product of all variables:
	/// <pre>
	///   // compute the product of all elements
	///   double[] product = new double[size];
	///   prod[0] = 1.0;
	///   for (int i = 0; i < n; ++i) {
	///     double[] tmp = product.clone();
	///     compiler.multiply(tmp, 0, array, i * size, product, 0);
	///   }
	/// 
	///   // value
	///   double p = product[0];
	/// 
	///   // first derivatives
	///   double dPdX = product[compiler.getPartialDerivativeIndex(1, 0, 0)];
	///   double dPdY = product[compiler.getPartialDerivativeIndex(0, 1, 0)];
	///   double dPdZ = product[compiler.getPartialDerivativeIndex(0, 0, 1)];
	/// 
	///   // cross derivatives (assuming order was at least 2)
	///   double dPdXdX = product[compiler.getPartialDerivativeIndex(2, 0, 0)];
	///   double dPdXdY = product[compiler.getPartialDerivativeIndex(1, 1, 0)];
	///   double dPdXdZ = product[compiler.getPartialDerivativeIndex(1, 0, 1)];
	///   double dPdYdY = product[compiler.getPartialDerivativeIndex(0, 2, 0)];
	///   double dPdYdZ = product[compiler.getPartialDerivativeIndex(0, 1, 1)];
	///   double dPdZdZ = product[compiler.getPartialDerivativeIndex(0, 0, 2)];
	/// </p> </summary>
	/// <seealso cref= DerivativeStructure
	/// @version $Id: DSCompiler.java 1517788 2013-08-27 11:15:18Z luc $
	/// @since 3.1 </seealso>
	public class DSCompiler
	{

		/// <summary>
		/// Array of all compilers created so far. </summary>
		private static AtomicReference<DSCompiler[][]> compilers = new AtomicReference<DSCompiler[][]>(null);

		/// <summary>
		/// Number of free parameters. </summary>
		private readonly int parameters;

		/// <summary>
		/// Derivation order. </summary>
		private readonly int order;

		/// <summary>
		/// Number of partial derivatives (including the single 0 order derivative element). </summary>
		private readonly int[][] sizes;

		/// <summary>
		/// Indirection array for partial derivatives. </summary>
		private readonly int[][] derivativesIndirection;

		/// <summary>
		/// Indirection array of the lower derivative elements. </summary>
		private readonly int[] lowerIndirection;

		/// <summary>
		/// Indirection arrays for multiplication. </summary>
		private readonly int[][][] multIndirection;

		/// <summary>
		/// Indirection arrays for function composition. </summary>
		private readonly int[][][] compIndirection;

		/// <summary>
		/// Private constructor, reserved for the factory method <seealso cref="#getCompiler(int, int)"/>. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="valueCompiler"> compiler for the value part </param>
		/// <param name="derivativeCompiler"> compiler for the derivative part </param>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private DSCompiler(final int parameters, final int order, final DSCompiler valueCompiler, final DSCompiler derivativeCompiler) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private DSCompiler(int parameters, int order, DSCompiler valueCompiler, DSCompiler derivativeCompiler)
		{

			this.parameters = parameters;
			this.order = order;
			this.sizes = compileSizes(parameters, order, valueCompiler);
			this.derivativesIndirection = compileDerivativesIndirection(parameters, order, valueCompiler, derivativeCompiler);
			this.lowerIndirection = compileLowerIndirection(parameters, order, valueCompiler, derivativeCompiler);
			this.multIndirection = compileMultiplicationIndirection(parameters, order, valueCompiler, derivativeCompiler, lowerIndirection);
			this.compIndirection = compileCompositionIndirection(parameters, order, valueCompiler, derivativeCompiler, sizes, derivativesIndirection);

		}

		/// <summary>
		/// Get the compiler for number of free parameters and order. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <returns> cached rules set </returns>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static DSCompiler getCompiler(int parameters, int order) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		public static DSCompiler getCompiler(int parameters, int order)
		{

			// get the cached compilers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DSCompiler[][] cache = compilers.get();
			DSCompiler[][] cache = compilers.get();
			if (cache != null && cache.Length > parameters && cache[parameters].Length > order && cache[parameters][order] != null)
			{
				// the compiler has already been created
				return cache[parameters][order];
			}

			// we need to create more compilers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxParameters = org.apache.commons.math3.util.FastMath.max(parameters, cache == null ? 0 : cache.length);
			int maxParameters = FastMath.max(parameters, cache == null ? 0 : cache.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxOrder = org.apache.commons.math3.util.FastMath.max(order, cache == null ? 0 : cache[0].length);
			int maxOrder = FastMath.max(order, cache == null ? 0 : cache[0].Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DSCompiler[][] newCache = new DSCompiler[maxParameters + 1][maxOrder + 1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: DSCompiler[][] newCache = new DSCompiler[maxParameters + 1][maxOrder + 1];
			DSCompiler[][] newCache = RectangularArrays.ReturnRectangularDSCompilerArray(maxParameters + 1, maxOrder + 1);

			if (cache != null)
			{
				// preserve the already created compilers
				for (int i = 0; i < cache.Length; ++i)
				{
					Array.Copy(cache[i], 0, newCache[i], 0, cache[i].Length);
				}
			}

			// create the array in increasing diagonal order
			for (int diag = 0; diag <= parameters + order; ++diag)
			{
				for (int o = FastMath.max(0, diag - parameters); o <= FastMath.min(order, diag); ++o)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int p = diag - o;
					int p = diag - o;
					if (newCache[p][o] == null)
					{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DSCompiler valueCompiler = (p == 0) ? null : newCache[p - 1][o];
						DSCompiler valueCompiler = (p == 0) ? null : newCache[p - 1][o];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final DSCompiler derivativeCompiler = (o == 0) ? null : newCache[p][o - 1];
						DSCompiler derivativeCompiler = (o == 0) ? null : newCache[p][o - 1];
						newCache[p][o] = new DSCompiler(p, o, valueCompiler, derivativeCompiler);
					}
				}
			}

			// atomically reset the cached compilers array
			compilers.compareAndSet(cache, newCache);

			return newCache[parameters][order];

		}

		/// <summary>
		/// Compile the sizes array. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="valueCompiler"> compiler for the value part </param>
		/// <returns> sizes array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int[][] compileSizes(final int parameters, final int order, final DSCompiler valueCompiler)
		private static int[][] compileSizes(int parameters, int order, DSCompiler valueCompiler)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] sizes = new int[parameters + 1][order + 1];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] sizes = new int[parameters + 1][order + 1];
			int[][] sizes = RectangularArrays.ReturnRectangularIntArray(parameters + 1, order + 1);
			if (parameters == 0)
			{
				Arrays.fill(sizes[0], 1);
			}
			else
			{
				Array.Copy(valueCompiler.sizes, 0, sizes, 0, parameters);
				sizes[parameters][0] = 1;
				for (int i = 0; i < order; ++i)
				{
					sizes[parameters][i + 1] = sizes[parameters][i] + sizes[parameters - 1][i + 1];
				}
			}

			return sizes;

		}

		/// <summary>
		/// Compile the derivatives indirection array. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="valueCompiler"> compiler for the value part </param>
		/// <param name="derivativeCompiler"> compiler for the derivative part </param>
		/// <returns> derivatives indirection array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int[][] compileDerivativesIndirection(final int parameters, final int order, final DSCompiler valueCompiler, final DSCompiler derivativeCompiler)
		private static int[][] compileDerivativesIndirection(int parameters, int order, DSCompiler valueCompiler, DSCompiler derivativeCompiler)
		{

			if (parameters == 0 || order == 0)
			{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: return new int[1][parameters];
				return RectangularArrays.ReturnRectangularIntArray(1, parameters);
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vSize = valueCompiler.derivativesIndirection.length;
			int vSize = valueCompiler.derivativesIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dSize = derivativeCompiler.derivativesIndirection.length;
			int dSize = derivativeCompiler.derivativesIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] derivativesIndirection = new int[vSize + dSize][parameters];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: int[][] derivativesIndirection = new int[vSize + dSize][parameters];
			int[][] derivativesIndirection = RectangularArrays.ReturnRectangularIntArray(vSize + dSize, parameters);

			// set up the indices for the value part
			for (int i = 0; i < vSize; ++i)
			{
				// copy the first indices, the last one remaining set to 0
				Array.Copy(valueCompiler.derivativesIndirection[i], 0, derivativesIndirection[i], 0, parameters - 1);
			}

			// set up the indices for the derivative part
			for (int i = 0; i < dSize; ++i)
			{

				// copy the indices
				Array.Copy(derivativeCompiler.derivativesIndirection[i], 0, derivativesIndirection[vSize + i], 0, parameters);

				// increment the derivation order for the last parameter
				derivativesIndirection[vSize + i][parameters - 1]++;

			}

			return derivativesIndirection;

		}

		/// <summary>
		/// Compile the lower derivatives indirection array.
		/// <p>
		/// This indirection array contains the indices of all elements
		/// except derivatives for last derivation order.
		/// </p> </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="valueCompiler"> compiler for the value part </param>
		/// <param name="derivativeCompiler"> compiler for the derivative part </param>
		/// <returns> lower derivatives indirection array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int[] compileLowerIndirection(final int parameters, final int order, final DSCompiler valueCompiler, final DSCompiler derivativeCompiler)
		private static int[] compileLowerIndirection(int parameters, int order, DSCompiler valueCompiler, DSCompiler derivativeCompiler)
		{

			if (parameters == 0 || order <= 1)
			{
				return new int[] {0};
			}

			// this is an implementation of definition 6 in Dan Kalman's paper.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vSize = valueCompiler.lowerIndirection.length;
			int vSize = valueCompiler.lowerIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dSize = derivativeCompiler.lowerIndirection.length;
			int dSize = derivativeCompiler.lowerIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] lowerIndirection = new int[vSize + dSize];
			int[] lowerIndirection = new int[vSize + dSize];
			Array.Copy(valueCompiler.lowerIndirection, 0, lowerIndirection, 0, vSize);
			for (int i = 0; i < dSize; ++i)
			{
				lowerIndirection[vSize + i] = valueCompiler.Size + derivativeCompiler.lowerIndirection[i];
			}

			return lowerIndirection;

		}

		/// <summary>
		/// Compile the multiplication indirection array.
		/// <p>
		/// This indirection array contains the indices of all pairs of elements
		/// involved when computing a multiplication. This allows a straightforward
		/// loop-based multiplication (see <seealso cref="#multiply(double[], int, double[], int, double[], int)"/>).
		/// </p> </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="valueCompiler"> compiler for the value part </param>
		/// <param name="derivativeCompiler"> compiler for the derivative part </param>
		/// <param name="lowerIndirection"> lower derivatives indirection array </param>
		/// <returns> multiplication indirection array </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static int[][][] compileMultiplicationIndirection(final int parameters, final int order, final DSCompiler valueCompiler, final DSCompiler derivativeCompiler, final int[] lowerIndirection)
		private static int[][][] compileMultiplicationIndirection(int parameters, int order, DSCompiler valueCompiler, DSCompiler derivativeCompiler, int[] lowerIndirection)
		{

			if ((parameters == 0) || (order == 0))
			{
				return new int[][][] {new int[][] {new int[] {1, 0, 0}}};
			}

			// this is an implementation of definition 3 in Dan Kalman's paper.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vSize = valueCompiler.multIndirection.length;
			int vSize = valueCompiler.multIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dSize = derivativeCompiler.multIndirection.length;
			int dSize = derivativeCompiler.multIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][][] multIndirection = new int[vSize + dSize][][];
			int[][][] multIndirection = new int[vSize + dSize][][];

			Array.Copy(valueCompiler.multIndirection, 0, multIndirection, 0, vSize);

			for (int i = 0; i < dSize; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] dRow = derivativeCompiler.multIndirection[i];
				int[][] dRow = derivativeCompiler.multIndirection[i];
				IList<int[]> row = new List<int[]>(dRow.Length * 2);
				for (int j = 0; j < dRow.Length; ++j)
				{
					row.Add(new int[] {dRow[j][0], lowerIndirection[dRow[j][1]], vSize + dRow[j][2]});
					row.Add(new int[] {dRow[j][0], vSize + dRow[j][1], lowerIndirection[dRow[j][2]]});
				}

				// combine terms with similar derivation orders
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<int[]> combined = new java.util.ArrayList<int[]>(row.size());
				IList<int[]> combined = new List<int[]>(row.Count);
				for (int j = 0; j < row.Count; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] termJ = row.get(j);
					int[] termJ = row[j];
					if (termJ[0] > 0)
					{
						for (int k = j + 1; k < row.Count; ++k)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] termK = row.get(k);
							int[] termK = row[k];
							if (termJ[1] == termK[1] && termJ[2] == termK[2])
							{
								// combine termJ and termK
								termJ[0] += termK[0];
								// make sure we will skip termK later on in the outer loop
								termK[0] = 0;
							}
						}
						combined.Add(termJ);
					}
				}

				multIndirection[vSize + i] = combined.ToArray();

			}

			return multIndirection;

		}

		/// <summary>
		/// Compile the function composition indirection array.
		/// <p>
		/// This indirection array contains the indices of all sets of elements
		/// involved when computing a composition. This allows a straightforward
		/// loop-based composition (see <seealso cref="#compose(double[], int, double[], double[], int)"/>).
		/// </p> </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="valueCompiler"> compiler for the value part </param>
		/// <param name="derivativeCompiler"> compiler for the derivative part </param>
		/// <param name="sizes"> sizes array </param>
		/// <param name="derivativesIndirection"> derivatives indirection array </param>
		/// <returns> multiplication indirection array </returns>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int[][][] compileCompositionIndirection(final int parameters, final int order, final DSCompiler valueCompiler, final DSCompiler derivativeCompiler, final int[][] sizes, final int[][] derivativesIndirection) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static int[][][] compileCompositionIndirection(int parameters, int order, DSCompiler valueCompiler, DSCompiler derivativeCompiler, int[][] sizes, int[][] derivativesIndirection)
		{

			if ((parameters == 0) || (order == 0))
			{
				return new int[][][] {new int[][] {new int[] {1, 0}}};
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int vSize = valueCompiler.compIndirection.length;
			int vSize = valueCompiler.compIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dSize = derivativeCompiler.compIndirection.length;
			int dSize = derivativeCompiler.compIndirection.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][][] compIndirection = new int[vSize + dSize][][];
			int[][][] compIndirection = new int[vSize + dSize][][];

			// the composition rules from the value part can be reused as is
			Array.Copy(valueCompiler.compIndirection, 0, compIndirection, 0, vSize);

			// the composition rules for the derivative part are deduced by
			// differentiation the rules from the underlying compiler once
			// with respect to the parameter this compiler handles and the
			// underlying one did not handle
			for (int i = 0; i < dSize; ++i)
			{
				IList<int[]> row = new List<int[]>();
				foreach (int[] term in derivativeCompiler.compIndirection[i])
				{

					// handle term p * f_k(g(x)) * g_l1(x) * g_l2(x) * ... * g_lp(x)

					// derive the first factor in the term: f_k with respect to new parameter
					int[] derivedTermF = new int[term.Length + 1];
					derivedTermF[0] = term[0]; // p
					derivedTermF[1] = term[1] + 1; // f_(k+1)
					int[] orders = new int[parameters];
					orders[parameters - 1] = 1;
					derivedTermF[term.Length] = getPartialDerivativeIndex(parameters, order, sizes, orders); // g_1
					for (int j = 2; j < term.Length; ++j)
					{
						// convert the indices as the mapping for the current order
						// is different from the mapping with one less order
						derivedTermF[j] = convertIndex(term[j], parameters, derivativeCompiler.derivativesIndirection, parameters, order, sizes);
					}
					Arrays.sort(derivedTermF, 2, derivedTermF.Length);
					row.Add(derivedTermF);

					// derive the various g_l
					for (int l = 2; l < term.Length; ++l)
					{
						int[] derivedTermG = new int[term.Length];
						derivedTermG[0] = term[0];
						derivedTermG[1] = term[1];
						for (int j = 2; j < term.Length; ++j)
						{
							// convert the indices as the mapping for the current order
							// is different from the mapping with one less order
							derivedTermG[j] = convertIndex(term[j], parameters, derivativeCompiler.derivativesIndirection, parameters, order, sizes);
							if (j == l)
							{
								// derive this term
								Array.Copy(derivativesIndirection[derivedTermG[j]], 0, orders, 0, parameters);
								orders[parameters - 1]++;
								derivedTermG[j] = getPartialDerivativeIndex(parameters, order, sizes, orders);
							}
						}
						Arrays.sort(derivedTermG, 2, derivedTermG.Length);
						row.Add(derivedTermG);
					}

				}

				// combine terms with similar derivation orders
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<int[]> combined = new java.util.ArrayList<int[]>(row.size());
				IList<int[]> combined = new List<int[]>(row.Count);
				for (int j = 0; j < row.Count; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] termJ = row.get(j);
					int[] termJ = row[j];
					if (termJ[0] > 0)
					{
						for (int k = j + 1; k < row.Count; ++k)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] termK = row.get(k);
							int[] termK = row[k];
							bool equals = termJ.Length == termK.Length;
							for (int l = 1; equals && l < termJ.Length; ++l)
							{
								equals &= termJ[l] == termK[l];
							}
							if (equals)
							{
								// combine termJ and termK
								termJ[0] += termK[0];
								// make sure we will skip termK later on in the outer loop
								termK[0] = 0;
							}
						}
						combined.Add(termJ);
					}
				}

				compIndirection[vSize + i] = combined.ToArray();

			}

			return compIndirection;

		}

		/// <summary>
		/// Get the index of a partial derivative in the array.
		/// <p>
		/// If all orders are set to 0, then the 0<sup>th</sup> order derivative
		/// is returned, which is the value of the function.
		/// </p>
		/// <p>The indices of derivatives are between 0 and <seealso cref="#getSize() getSize()"/> - 1.
		/// Their specific order is fixed for a given compiler, but otherwise not
		/// publicly specified. There are however some simple cases which have guaranteed
		/// indices:
		/// </p>
		/// <ul>
		///   <li>the index of 0<sup>th</sup> order derivative is always 0</li>
		///   <li>if there is only 1 <seealso cref="#getFreeParameters() free parameter"/>, then the
		///   derivatives are sorted in increasing derivation order (i.e. f at index 0, df/dp
		///   at index 1, d<sup>2</sup>f/dp<sup>2</sup> at index 2 ...
		///   d<sup>k</sup>f/dp<sup>k</sup> at index k),</li>
		///   <li>if the <seealso cref="#getOrder() derivation order"/> is 1, then the derivatives
		///   are sorted in increasing free parameter order (i.e. f at index 0, df/dx<sub>1</sub>
		///   at index 1, df/dx<sub>2</sub> at index 2 ... df/dx<sub>k</sub> at index k),</li>
		///   <li>all other cases are not publicly specified</li>
		/// </ul>
		/// <p>
		/// This method is the inverse of method <seealso cref="#getPartialDerivativeOrders(int)"/>
		/// </p> </summary>
		/// <param name="orders"> derivation orders with respect to each parameter </param>
		/// <returns> index of the partial derivative </returns>
		/// <exception cref="DimensionMismatchException"> if the numbers of parameters does not
		/// match the instance </exception>
		/// <exception cref="NumberIsTooLargeException"> if sum of derivation orders is larger
		/// than the instance limits </exception>
		/// <seealso cref= #getPartialDerivativeOrders(int) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getPartialDerivativeIndex(final int... orders) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual int getPartialDerivativeIndex(params int[] orders)
		{

			// safety check
			if (orders.Length != FreeParameters)
			{
				throw new DimensionMismatchException(orders.Length, FreeParameters);
			}

			return getPartialDerivativeIndex(parameters, order, sizes, orders);

		}

		/// <summary>
		/// Get the index of a partial derivative in an array. </summary>
		/// <param name="parameters"> number of free parameters </param>
		/// <param name="order"> derivation order </param>
		/// <param name="sizes"> sizes array </param>
		/// <param name="orders"> derivation orders with respect to each parameter
		/// (the lenght of this array must match the number of parameters) </param>
		/// <returns> index of the partial derivative </returns>
		/// <exception cref="NumberIsTooLargeException"> if sum of derivation orders is larger
		/// than the instance limits </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int getPartialDerivativeIndex(final int parameters, final int order, final int[][] sizes, final int... orders) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static int getPartialDerivativeIndex(int parameters, int order, int[][] sizes, params int[] orders)
		{

			// the value is obtained by diving into the recursive Dan Kalman's structure
			// this is theorem 2 of his paper, with recursion replaced by iteration
			int index = 0;
			int m = order;
			int ordersSum = 0;
			for (int i = parameters - 1; i >= 0; --i)
			{

				// derivative order for current free parameter
				int derivativeOrder = orders[i];

				// safety check
				ordersSum += derivativeOrder;
				if (ordersSum > order)
				{
					throw new NumberIsTooLargeException(ordersSum, order, true);
				}

				while (derivativeOrder-- > 0)
				{
					// as long as we differentiate according to current free parameter,
					// we have to skip the value part and dive into the derivative part
					// so we add the size of the value part to the base index
					index += sizes[i][m--];
				}

			}

			return index;

		}

		/// <summary>
		/// Convert an index from one (parameters, order) structure to another. </summary>
		/// <param name="index"> index of a partial derivative in source derivative structure </param>
		/// <param name="srcP"> number of free parameters in source derivative structure </param>
		/// <param name="srcDerivativesIndirection"> derivatives indirection array for the source
		/// derivative structure </param>
		/// <param name="destP"> number of free parameters in destination derivative structure </param>
		/// <param name="destO"> derivation order in destination derivative structure </param>
		/// <param name="destSizes"> sizes array for the destination derivative structure </param>
		/// <returns> index of the partial derivative with the <em>same</em> characteristics
		/// in destination derivative structure </returns>
		/// <exception cref="NumberIsTooLargeException"> if order is too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static int convertIndex(final int index, final int srcP, final int[][] srcDerivativesIndirection, final int destP, final int destO, final int[][] destSizes) throws org.apache.commons.math3.exception.NumberIsTooLargeException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private static int convertIndex(int index, int srcP, int[][] srcDerivativesIndirection, int destP, int destO, int[][] destSizes)
		{
			int[] orders = new int[destP];
			Array.Copy(srcDerivativesIndirection[index], 0, orders, 0, FastMath.min(srcP, destP));
			return getPartialDerivativeIndex(destP, destO, destSizes, orders);
		}

		/// <summary>
		/// Get the derivation orders for a specific index in the array.
		/// <p>
		/// This method is the inverse of <seealso cref="#getPartialDerivativeIndex(int...)"/>.
		/// </p> </summary>
		/// <param name="index"> of the partial derivative </param>
		/// <returns> orders derivation orders with respect to each parameter </returns>
		/// <seealso cref= #getPartialDerivativeIndex(int...) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int[] getPartialDerivativeOrders(final int index)
		public virtual int[] getPartialDerivativeOrders(int index)
		{
			return derivativesIndirection[index];
		}

		/// <summary>
		/// Get the number of free parameters. </summary>
		/// <returns> number of free parameters </returns>
		public virtual int FreeParameters
		{
			get
			{
				return parameters;
			}
		}

		/// <summary>
		/// Get the derivation order. </summary>
		/// <returns> derivation order </returns>
		public virtual int Order
		{
			get
			{
				return order;
			}
		}

		/// <summary>
		/// Get the array size required for holding partial derivatives data.
		/// <p>
		/// This number includes the single 0 order derivative element, which is
		/// guaranteed to be stored in the first element of the array.
		/// </p> </summary>
		/// <returns> array size required for holding partial derivatives data </returns>
		public virtual int Size
		{
			get
			{
				return sizes[parameters][order];
			}
		}

		/// <summary>
		/// Compute linear combination.
		/// The derivative structure built will be a1 * ds1 + a2 * ds2 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="c1"> first base (unscaled) component </param>
		/// <param name="offset1"> offset of first operand in its array </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="c2"> second base (unscaled) component </param>
		/// <param name="offset2"> offset of second operand in its array </param>
		/// <param name="result"> array where result must be stored (it may be
		/// one of the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void linearCombination(final double a1, final double[] c1, final int offset1, final double a2, final double[] c2, final int offset2, final double[] result, final int resultOffset)
		public virtual void linearCombination(double a1, double[] c1, int offset1, double a2, double[] c2, int offset2, double[] result, int resultOffset)
		{
			for (int i = 0; i < Size; ++i)
			{
				result[resultOffset + i] = MathArrays.linearCombination(a1, c1[offset1 + i], a2, c2[offset2 + i]);
			}
		}

		/// <summary>
		/// Compute linear combination.
		/// The derivative structure built will be a1 * ds1 + a2 * ds2 + a3 * ds3 + a4 * ds4 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="c1"> first base (unscaled) component </param>
		/// <param name="offset1"> offset of first operand in its array </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="c2"> second base (unscaled) component </param>
		/// <param name="offset2"> offset of second operand in its array </param>
		/// <param name="a3"> third scale factor </param>
		/// <param name="c3"> third base (unscaled) component </param>
		/// <param name="offset3"> offset of third operand in its array </param>
		/// <param name="result"> array where result must be stored (it may be
		/// one of the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void linearCombination(final double a1, final double[] c1, final int offset1, final double a2, final double[] c2, final int offset2, final double a3, final double[] c3, final int offset3, final double[] result, final int resultOffset)
		public virtual void linearCombination(double a1, double[] c1, int offset1, double a2, double[] c2, int offset2, double a3, double[] c3, int offset3, double[] result, int resultOffset)
		{
			for (int i = 0; i < Size; ++i)
			{
				result[resultOffset + i] = MathArrays.linearCombination(a1, c1[offset1 + i], a2, c2[offset2 + i], a3, c3[offset3 + i]);
			}
		}

		/// <summary>
		/// Compute linear combination.
		/// The derivative structure built will be a1 * ds1 + a2 * ds2 + a3 * ds3 + a4 * ds4 </summary>
		/// <param name="a1"> first scale factor </param>
		/// <param name="c1"> first base (unscaled) component </param>
		/// <param name="offset1"> offset of first operand in its array </param>
		/// <param name="a2"> second scale factor </param>
		/// <param name="c2"> second base (unscaled) component </param>
		/// <param name="offset2"> offset of second operand in its array </param>
		/// <param name="a3"> third scale factor </param>
		/// <param name="c3"> third base (unscaled) component </param>
		/// <param name="offset3"> offset of third operand in its array </param>
		/// <param name="a4"> fourth scale factor </param>
		/// <param name="c4"> fourth base (unscaled) component </param>
		/// <param name="offset4"> offset of fourth operand in its array </param>
		/// <param name="result"> array where result must be stored (it may be
		/// one of the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void linearCombination(final double a1, final double[] c1, final int offset1, final double a2, final double[] c2, final int offset2, final double a3, final double[] c3, final int offset3, final double a4, final double[] c4, final int offset4, final double[] result, final int resultOffset)
		public virtual void linearCombination(double a1, double[] c1, int offset1, double a2, double[] c2, int offset2, double a3, double[] c3, int offset3, double a4, double[] c4, int offset4, double[] result, int resultOffset)
		{
			for (int i = 0; i < Size; ++i)
			{
				result[resultOffset + i] = MathArrays.linearCombination(a1, c1[offset1 + i], a2, c2[offset2 + i], a3, c3[offset3 + i], a4, c4[offset4 + i]);
			}
		}

		/// <summary>
		/// Perform addition of two derivative structures. </summary>
		/// <param name="lhs"> array holding left hand side of addition </param>
		/// <param name="lhsOffset"> offset of the left hand side in its array </param>
		/// <param name="rhs"> array right hand side of addition </param>
		/// <param name="rhsOffset"> offset of the right hand side in its array </param>
		/// <param name="result"> array where result must be stored (it may be
		/// one of the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void add(final double[] lhs, final int lhsOffset, final double[] rhs, final int rhsOffset, final double[] result, final int resultOffset)
		public virtual void add(double[] lhs, int lhsOffset, double[] rhs, int rhsOffset, double[] result, int resultOffset)
		{
			for (int i = 0; i < Size; ++i)
			{
				result[resultOffset + i] = lhs[lhsOffset + i] + rhs[rhsOffset + i];
			}
		}
		/// <summary>
		/// Perform subtraction of two derivative structures. </summary>
		/// <param name="lhs"> array holding left hand side of subtraction </param>
		/// <param name="lhsOffset"> offset of the left hand side in its array </param>
		/// <param name="rhs"> array right hand side of subtraction </param>
		/// <param name="rhsOffset"> offset of the right hand side in its array </param>
		/// <param name="result"> array where result must be stored (it may be
		/// one of the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void subtract(final double[] lhs, final int lhsOffset, final double[] rhs, final int rhsOffset, final double[] result, final int resultOffset)
		public virtual void subtract(double[] lhs, int lhsOffset, double[] rhs, int rhsOffset, double[] result, int resultOffset)
		{
			for (int i = 0; i < Size; ++i)
			{
				result[resultOffset + i] = lhs[lhsOffset + i] - rhs[rhsOffset + i];
			}
		}

		/// <summary>
		/// Perform multiplication of two derivative structures. </summary>
		/// <param name="lhs"> array holding left hand side of multiplication </param>
		/// <param name="lhsOffset"> offset of the left hand side in its array </param>
		/// <param name="rhs"> array right hand side of multiplication </param>
		/// <param name="rhsOffset"> offset of the right hand side in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// multiplication the result array <em>cannot</em> be one of
		/// the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void multiply(final double[] lhs, final int lhsOffset, final double[] rhs, final int rhsOffset, final double[] result, final int resultOffset)
		public virtual void multiply(double[] lhs, int lhsOffset, double[] rhs, int rhsOffset, double[] result, int resultOffset)
		{
			for (int i = 0; i < multIndirection.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] mappingI = multIndirection[i];
				int[][] mappingI = multIndirection[i];
				double r = 0;
				for (int j = 0; j < mappingI.Length; ++j)
				{
					r += mappingI[j][0] * lhs[lhsOffset + mappingI[j][1]] * rhs[rhsOffset + mappingI[j][2]];
				}
				result[resultOffset + i] = r;
			}
		}

		/// <summary>
		/// Perform division of two derivative structures. </summary>
		/// <param name="lhs"> array holding left hand side of division </param>
		/// <param name="lhsOffset"> offset of the left hand side in its array </param>
		/// <param name="rhs"> array right hand side of division </param>
		/// <param name="rhsOffset"> offset of the right hand side in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// division the result array <em>cannot</em> be one of
		/// the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void divide(final double[] lhs, final int lhsOffset, final double[] rhs, final int rhsOffset, final double[] result, final int resultOffset)
		public virtual void divide(double[] lhs, int lhsOffset, double[] rhs, int rhsOffset, double[] result, int resultOffset)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] reciprocal = new double[getSize()];
			double[] reciprocal = new double[Size];
			pow(rhs, lhsOffset, -1, reciprocal, 0);
			multiply(lhs, lhsOffset, reciprocal, 0, result, resultOffset);
		}

		/// <summary>
		/// Perform remainder of two derivative structures. </summary>
		/// <param name="lhs"> array holding left hand side of remainder </param>
		/// <param name="lhsOffset"> offset of the left hand side in its array </param>
		/// <param name="rhs"> array right hand side of remainder </param>
		/// <param name="rhsOffset"> offset of the right hand side in its array </param>
		/// <param name="result"> array where result must be stored (it may be
		/// one of the input arrays) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void remainder(final double[] lhs, final int lhsOffset, final double[] rhs, final int rhsOffset, final double[] result, final int resultOffset)
		public virtual void remainder(double[] lhs, int lhsOffset, double[] rhs, int rhsOffset, double[] result, int resultOffset)
		{

			// compute k such that lhs % rhs = lhs - k rhs
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rem = org.apache.commons.math3.util.FastMath.IEEEremainder(lhs[lhsOffset], rhs[rhsOffset]);
			double rem = FastMath.IEEEremainder(lhs[lhsOffset], rhs[rhsOffset]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double k = org.apache.commons.math3.util.FastMath.rint((lhs[lhsOffset] - rem) / rhs[rhsOffset]);
			double k = FastMath.rint((lhs[lhsOffset] - rem) / rhs[rhsOffset]);

			// set up value
			result[resultOffset] = rem;

			// set up partial derivatives
			for (int i = 1; i < Size; ++i)
			{
				result[resultOffset + i] = lhs[lhsOffset + i] - k * rhs[rhsOffset + i];
			}

		}

		/// <summary>
		/// Compute power of a double to a derivative structure. </summary>
		/// <param name="a"> number to exponentiate </param>
		/// <param name="operand"> array holding the power </param>
		/// <param name="operandOffset"> offset of the power in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// power the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array
		/// @since 3.3 </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void pow(final double a, final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void pow(double a, double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			// [a^x, ln(a) a^x, ln(a)^2 a^x,, ln(a)^3 a^x, ... ]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] function = new double[1 + order];
			double[] function = new double[1 + order];
			if (a == 0)
			{
				if (operand[operandOffset] == 0)
				{
					function[0] = 1;
					double infinity = double.PositiveInfinity;
					for (int i = 1; i < function.Length; ++i)
					{
						infinity = -infinity;
						function[i] = infinity;
					}
				}
				else if (operand[operandOffset] < 0)
				{
					Arrays.fill(function, double.NaN);
				}
			}
			else
			{
				function[0] = FastMath.pow(a, operand[operandOffset]);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double lnA = org.apache.commons.math3.util.FastMath.log(a);
				double lnA = FastMath.log(a);
				for (int i = 1; i < function.Length; ++i)
				{
					function[i] = lnA * function[i - 1];
				}
			}


			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute power of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="p"> power to apply </param>
		/// <param name="result"> array where result must be stored (for
		/// power the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void pow(final double[] operand, final int operandOffset, final double p, final double[] result, final int resultOffset)
		public virtual void pow(double[] operand, int operandOffset, double p, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			// [x^p, px^(p-1), p(p-1)x^(p-2), ... ]
			double[] function = new double[1 + order];
			double xk = FastMath.pow(operand[operandOffset], p - order);
			for (int i = order; i > 0; --i)
			{
				function[i] = xk;
				xk *= operand[operandOffset];
			}
			function[0] = xk;
			double coefficient = p;
			for (int i = 1; i <= order; ++i)
			{
				function[i] *= coefficient;
				coefficient *= p - i;
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute integer power of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="n"> power to apply </param>
		/// <param name="result"> array where result must be stored (for
		/// power the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void pow(final double[] operand, final int operandOffset, final int n, final double[] result, final int resultOffset)
		public virtual void pow(double[] operand, int operandOffset, int n, double[] result, int resultOffset)
		{

			if (n == 0)
			{
				// special case, x^0 = 1 for all x
				result[resultOffset] = 1.0;
				Arrays.fill(result, resultOffset + 1, resultOffset + Size, 0);
				return;
			}

			// create the power function value and derivatives
			// [x^n, nx^(n-1), n(n-1)x^(n-2), ... ]
			double[] function = new double[1 + order];

			if (n > 0)
			{
				// strictly positive power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int maxOrder = org.apache.commons.math3.util.FastMath.min(order, n);
				int maxOrder = FastMath.min(order, n);
				double xk = FastMath.pow(operand[operandOffset], n - maxOrder);
				for (int i = maxOrder; i > 0; --i)
				{
					function[i] = xk;
					xk *= operand[operandOffset];
				}
				function[0] = xk;
			}
			else
			{
				// strictly negative power
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double inv = 1.0 / operand[operandOffset];
				double inv = 1.0 / operand[operandOffset];
				double xk = FastMath.pow(inv, -n);
				for (int i = 0; i <= order; ++i)
				{
					function[i] = xk;
					xk *= inv;
				}
			}

			double coefficient = n;
			for (int i = 1; i <= order; ++i)
			{
				function[i] *= coefficient;
				coefficient *= n - i;
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute power of a derivative structure. </summary>
		/// <param name="x"> array holding the base </param>
		/// <param name="xOffset"> offset of the base in its array </param>
		/// <param name="y"> array holding the exponent </param>
		/// <param name="yOffset"> offset of the exponent in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// power the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void pow(final double[] x, final int xOffset, final double[] y, final int yOffset, final double[] result, final int resultOffset)
		public virtual void pow(double[] x, int xOffset, double[] y, int yOffset, double[] result, int resultOffset)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] logX = new double[getSize()];
			double[] logX = new double[Size];
			log(x, xOffset, logX, 0);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yLogX = new double[getSize()];
			double[] yLogX = new double[Size];
			multiply(logX, 0, y, yOffset, yLogX, 0);
			exp(yLogX, 0, result, resultOffset);
		}

		/// <summary>
		/// Compute n<sup>th</sup> root of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="n"> order of the root </param>
		/// <param name="result"> array where result must be stored (for
		/// n<sup>th</sup> root the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void rootN(final double[] operand, final int operandOffset, final int n, final double[] result, final int resultOffset)
		public virtual void rootN(double[] operand, int operandOffset, int n, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			// [x^(1/n), (1/n)x^((1/n)-1), (1-n)/n^2x^((1/n)-2), ... ]
			double[] function = new double[1 + order];
			double xk;
			if (n == 2)
			{
				function[0] = FastMath.sqrt(operand[operandOffset]);
				xk = 0.5 / function[0];
			}
			else if (n == 3)
			{
				function[0] = FastMath.cbrt(operand[operandOffset]);
				xk = 1.0 / (3.0 * function[0] * function[0]);
			}
			else
			{
				function[0] = FastMath.pow(operand[operandOffset], 1.0 / n);
				xk = 1.0 / (n * FastMath.pow(function[0], n - 1));
			}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double nReciprocal = 1.0 / n;
			double nReciprocal = 1.0 / n;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double xReciprocal = 1.0 / operand[operandOffset];
			double xReciprocal = 1.0 / operand[operandOffset];
			for (int i = 1; i <= order; ++i)
			{
				function[i] = xk;
				xk *= xReciprocal * (nReciprocal - i);
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute exponential of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// exponential the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void exp(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void exp(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			Arrays.fill(function, FastMath.exp(operand[operandOffset]));

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute exp(x) - 1 of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// exponential the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void expm1(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void expm1(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.expm1(operand[operandOffset]);
			Arrays.fill(function, 1, 1 + order, FastMath.exp(operand[operandOffset]));

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute natural logarithm of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// logarithm the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void log(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void log(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.log(operand[operandOffset]);
			if (order > 0)
			{
				double inv = 1.0 / operand[operandOffset];
				double xk = inv;
				for (int i = 1; i <= order; ++i)
				{
					function[i] = xk;
					xk *= -i * inv;
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Computes shifted logarithm of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// shifted logarithm the result array <em>cannot</em> be the input array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void log1p(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void log1p(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.log1p(operand[operandOffset]);
			if (order > 0)
			{
				double inv = 1.0 / (1.0 + operand[operandOffset]);
				double xk = inv;
				for (int i = 1; i <= order; ++i)
				{
					function[i] = xk;
					xk *= -i * inv;
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Computes base 10 logarithm of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// base 10 logarithm the result array <em>cannot</em> be the input array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void log10(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void log10(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.log10(operand[operandOffset]);
			if (order > 0)
			{
				double inv = 1.0 / operand[operandOffset];
				double xk = inv / FastMath.log(10.0);
				for (int i = 1; i <= order; ++i)
				{
					function[i] = xk;
					xk *= -i * inv;
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute cosine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// cosine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void cos(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void cos(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.cos(operand[operandOffset]);
			if (order > 0)
			{
				function[1] = -FastMath.sin(operand[operandOffset]);
				for (int i = 2; i <= order; ++i)
				{
					function[i] = -function[i - 2];
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute sine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// sine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void sin(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void sin(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.sin(operand[operandOffset]);
			if (order > 0)
			{
				function[1] = FastMath.cos(operand[operandOffset]);
				for (int i = 2; i <= order; ++i)
				{
					function[i] = -function[i - 2];
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute tangent of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// tangent the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void tan(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void tan(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] function = new double[1 + order];
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = org.apache.commons.math3.util.FastMath.tan(operand[operandOffset]);
			double t = FastMath.tan(operand[operandOffset]);
			function[0] = t;

			if (order > 0)
			{

				// the nth order derivative of tan has the form:
				// dn(tan(x)/dxn = P_n(tan(x))
				// where P_n(t) is a degree n+1 polynomial with same parity as n+1
				// P_0(t) = t, P_1(t) = 1 + t^2, P_2(t) = 2 t (1 + t^2) ...
				// the general recurrence relation for P_n is:
				// P_n(x) = (1+t^2) P_(n-1)'(t)
				// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[order + 2];
				double[] p = new double[order + 2];
				p[1] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t2 = t * t;
				double t2 = t * t;
				for (int n = 1; n <= order; ++n)
				{

					// update and evaluate polynomial P_n(t)
					double v = 0;
					p[n + 1] = n * p[n];
					for (int k = n + 1; k >= 0; k -= 2)
					{
						v = v * t2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (k - 1) * p[k - 1] + (k - 3) * p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = p[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= t;
					}

					function[n] = v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute arc cosine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// arc cosine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void acos(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void acos(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = operand[operandOffset];
			double x = operand[operandOffset];
			function[0] = FastMath.acos(x);
			if (order > 0)
			{
				// the nth order derivative of acos has the form:
				// dn(acos(x)/dxn = P_n(x) / [1 - x^2]^((2n-1)/2)
				// where P_n(x) is a degree n-1 polynomial with same parity as n-1
				// P_1(x) = -1, P_2(x) = -x, P_3(x) = -2x^2 - 1 ...
				// the general recurrence relation for P_n is:
				// P_n(x) = (1-x^2) P_(n-1)'(x) + (2n-3) x P_(n-1)(x)
				// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[order];
				double[] p = new double[order];
				p[0] = -1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1.0 / (1 - x2);
				double f = 1.0 / (1 - x2);
				double coeff = FastMath.sqrt(f);
				function[1] = coeff * p[0];
				for (int n = 2; n <= order; ++n)
				{

					// update and evaluate polynomial P_n(x)
					double v = 0;
					p[n - 1] = (n - 1) * p[n - 2];
					for (int k = n - 1; k >= 0; k -= 2)
					{
						v = v * x2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (k - 1) * p[k - 1] + (2 * n - k) * p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = p[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= x;
					}

					coeff *= f;
					function[n] = coeff * v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute arc sine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// arc sine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void asin(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void asin(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = operand[operandOffset];
			double x = operand[operandOffset];
			function[0] = FastMath.asin(x);
			if (order > 0)
			{
				// the nth order derivative of asin has the form:
				// dn(asin(x)/dxn = P_n(x) / [1 - x^2]^((2n-1)/2)
				// where P_n(x) is a degree n-1 polynomial with same parity as n-1
				// P_1(x) = 1, P_2(x) = x, P_3(x) = 2x^2 + 1 ...
				// the general recurrence relation for P_n is:
				// P_n(x) = (1-x^2) P_(n-1)'(x) + (2n-3) x P_(n-1)(x)
				// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[order];
				double[] p = new double[order];
				p[0] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1.0 / (1 - x2);
				double f = 1.0 / (1 - x2);
				double coeff = FastMath.sqrt(f);
				function[1] = coeff * p[0];
				for (int n = 2; n <= order; ++n)
				{

					// update and evaluate polynomial P_n(x)
					double v = 0;
					p[n - 1] = (n - 1) * p[n - 2];
					for (int k = n - 1; k >= 0; k -= 2)
					{
						v = v * x2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (k - 1) * p[k - 1] + (2 * n - k) * p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = p[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= x;
					}

					coeff *= f;
					function[n] = coeff * v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute arc tangent of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// arc tangent the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void atan(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void atan(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = operand[operandOffset];
			double x = operand[operandOffset];
			function[0] = FastMath.atan(x);
			if (order > 0)
			{
				// the nth order derivative of atan has the form:
				// dn(atan(x)/dxn = Q_n(x) / (1 + x^2)^n
				// where Q_n(x) is a degree n-1 polynomial with same parity as n-1
				// Q_1(x) = 1, Q_2(x) = -2x, Q_3(x) = 6x^2 - 2 ...
				// the general recurrence relation for Q_n is:
				// Q_n(x) = (1+x^2) Q_(n-1)'(x) - 2(n-1) x Q_(n-1)(x)
				// as per polynomial parity, we can store coefficients of both Q_(n-1) and Q_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] q = new double[order];
				double[] q = new double[order];
				q[0] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1.0 / (1 + x2);
				double f = 1.0 / (1 + x2);
				double coeff = f;
				function[1] = coeff * q[0];
				for (int n = 2; n <= order; ++n)
				{

					// update and evaluate polynomial Q_n(x)
					double v = 0;
					q[n - 1] = -n * q[n - 2];
					for (int k = n - 1; k >= 0; k -= 2)
					{
						v = v * x2 + q[k];
						if (k > 2)
						{
							q[k - 2] = (k - 1) * q[k - 1] + (k - 1 - 2 * n) * q[k - 3];
						}
						else if (k == 2)
						{
							q[0] = q[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= x;
					}

					coeff *= f;
					function[n] = coeff * v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute two arguments arc tangent of a derivative structure. </summary>
		/// <param name="y"> array holding the first operand </param>
		/// <param name="yOffset"> offset of the first operand in its array </param>
		/// <param name="x"> array holding the second operand </param>
		/// <param name="xOffset"> offset of the second operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// two arguments arc tangent the result array <em>cannot</em>
		/// be the input array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void atan2(final double[] y, final int yOffset, final double[] x, final int xOffset, final double[] result, final int resultOffset)
		public virtual void atan2(double[] y, int yOffset, double[] x, int xOffset, double[] result, int resultOffset)
		{

			// compute r = sqrt(x^2+y^2)
			double[] tmp1 = new double[Size];
			multiply(x, xOffset, x, xOffset, tmp1, 0); // x^2
			double[] tmp2 = new double[Size];
			multiply(y, yOffset, y, yOffset, tmp2, 0); // y^2
			add(tmp1, 0, tmp2, 0, tmp2, 0); // x^2 + y^2
			rootN(tmp2, 0, 2, tmp1, 0); // r = sqrt(x^2 + y^2)

			if (x[xOffset] >= 0)
			{

				// compute atan2(y, x) = 2 atan(y / (r + x))
				add(tmp1, 0, x, xOffset, tmp2, 0); // r + x
				divide(y, yOffset, tmp2, 0, tmp1, 0); // y /(r + x)
				atan(tmp1, 0, tmp2, 0); // atan(y / (r + x))
				for (int i = 0; i < tmp2.Length; ++i)
				{
					result[resultOffset + i] = 2 * tmp2[i]; // 2 * atan(y / (r + x))
				}

			}
			else
			{

				// compute atan2(y, x) = +/- pi - 2 atan(y / (r - x))
				subtract(tmp1, 0, x, xOffset, tmp2, 0); // r - x
				divide(y, yOffset, tmp2, 0, tmp1, 0); // y /(r - x)
				atan(tmp1, 0, tmp2, 0); // atan(y / (r - x))
				result[resultOffset] = ((tmp2[0] <= 0) ? - FastMath.PI : FastMath.PI) - 2 * tmp2[0]; // +/-pi - 2 * atan(y / (r - x))
				for (int i = 1; i < tmp2.Length; ++i)
				{
					result[resultOffset + i] = -2 * tmp2[i]; // +/-pi - 2 * atan(y / (r - x))
				}

			}

			// fix value to take special cases (+0/+0, +0/-0, -0/+0, -0/-0, +/-infinity) correctly
			result[resultOffset] = FastMath.atan2(y[yOffset], x[xOffset]);

		}

		/// <summary>
		/// Compute hyperbolic cosine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// hyperbolic cosine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void cosh(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void cosh(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.cosh(operand[operandOffset]);
			if (order > 0)
			{
				function[1] = FastMath.sinh(operand[operandOffset]);
				for (int i = 2; i <= order; ++i)
				{
					function[i] = function[i - 2];
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute hyperbolic sine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// hyperbolic sine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void sinh(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void sinh(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
			function[0] = FastMath.sinh(operand[operandOffset]);
			if (order > 0)
			{
				function[1] = FastMath.cosh(operand[operandOffset]);
				for (int i = 2; i <= order; ++i)
				{
					function[i] = function[i - 2];
				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute hyperbolic tangent of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// hyperbolic tangent the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void tanh(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void tanh(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] function = new double[1 + order];
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = org.apache.commons.math3.util.FastMath.tanh(operand[operandOffset]);
			double t = FastMath.tanh(operand[operandOffset]);
			function[0] = t;

			if (order > 0)
			{

				// the nth order derivative of tanh has the form:
				// dn(tanh(x)/dxn = P_n(tanh(x))
				// where P_n(t) is a degree n+1 polynomial with same parity as n+1
				// P_0(t) = t, P_1(t) = 1 - t^2, P_2(t) = -2 t (1 - t^2) ...
				// the general recurrence relation for P_n is:
				// P_n(x) = (1-t^2) P_(n-1)'(t)
				// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[order + 2];
				double[] p = new double[order + 2];
				p[1] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t2 = t * t;
				double t2 = t * t;
				for (int n = 1; n <= order; ++n)
				{

					// update and evaluate polynomial P_n(t)
					double v = 0;
					p[n + 1] = -n * p[n];
					for (int k = n + 1; k >= 0; k -= 2)
					{
						v = v * t2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (k - 1) * p[k - 1] - (k - 3) * p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = p[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= t;
					}

					function[n] = v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute inverse hyperbolic cosine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// inverse hyperbolic cosine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void acosh(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void acosh(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = operand[operandOffset];
			double x = operand[operandOffset];
			function[0] = FastMath.acosh(x);
			if (order > 0)
			{
				// the nth order derivative of acosh has the form:
				// dn(acosh(x)/dxn = P_n(x) / [x^2 - 1]^((2n-1)/2)
				// where P_n(x) is a degree n-1 polynomial with same parity as n-1
				// P_1(x) = 1, P_2(x) = -x, P_3(x) = 2x^2 + 1 ...
				// the general recurrence relation for P_n is:
				// P_n(x) = (x^2-1) P_(n-1)'(x) - (2n-3) x P_(n-1)(x)
				// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[order];
				double[] p = new double[order];
				p[0] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1.0 / (x2 - 1);
				double f = 1.0 / (x2 - 1);
				double coeff = FastMath.sqrt(f);
				function[1] = coeff * p[0];
				for (int n = 2; n <= order; ++n)
				{

					// update and evaluate polynomial P_n(x)
					double v = 0;
					p[n - 1] = (1 - n) * p[n - 2];
					for (int k = n - 1; k >= 0; k -= 2)
					{
						v = v * x2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (1 - k) * p[k - 1] + (k - 2 * n) * p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = -p[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= x;
					}

					coeff *= f;
					function[n] = coeff * v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute inverse hyperbolic sine of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// inverse hyperbolic sine the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void asinh(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void asinh(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = operand[operandOffset];
			double x = operand[operandOffset];
			function[0] = FastMath.asinh(x);
			if (order > 0)
			{
				// the nth order derivative of asinh has the form:
				// dn(asinh(x)/dxn = P_n(x) / [x^2 + 1]^((2n-1)/2)
				// where P_n(x) is a degree n-1 polynomial with same parity as n-1
				// P_1(x) = 1, P_2(x) = -x, P_3(x) = 2x^2 - 1 ...
				// the general recurrence relation for P_n is:
				// P_n(x) = (x^2+1) P_(n-1)'(x) - (2n-3) x P_(n-1)(x)
				// as per polynomial parity, we can store coefficients of both P_(n-1) and P_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] p = new double[order];
				double[] p = new double[order];
				p[0] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1.0 / (1 + x2);
				double f = 1.0 / (1 + x2);
				double coeff = FastMath.sqrt(f);
				function[1] = coeff * p[0];
				for (int n = 2; n <= order; ++n)
				{

					// update and evaluate polynomial P_n(x)
					double v = 0;
					p[n - 1] = (1 - n) * p[n - 2];
					for (int k = n - 1; k >= 0; k -= 2)
					{
						v = v * x2 + p[k];
						if (k > 2)
						{
							p[k - 2] = (k - 1) * p[k - 1] + (k - 2 * n) * p[k - 3];
						}
						else if (k == 2)
						{
							p[0] = p[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= x;
					}

					coeff *= f;
					function[n] = coeff * v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute inverse hyperbolic tangent of a derivative structure. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="result"> array where result must be stored (for
		/// inverse hyperbolic tangent the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void atanh(final double[] operand, final int operandOffset, final double[] result, final int resultOffset)
		public virtual void atanh(double[] operand, int operandOffset, double[] result, int resultOffset)
		{

			// create the function value and derivatives
			double[] function = new double[1 + order];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x = operand[operandOffset];
			double x = operand[operandOffset];
			function[0] = FastMath.atanh(x);
			if (order > 0)
			{
				// the nth order derivative of atanh has the form:
				// dn(atanh(x)/dxn = Q_n(x) / (1 - x^2)^n
				// where Q_n(x) is a degree n-1 polynomial with same parity as n-1
				// Q_1(x) = 1, Q_2(x) = 2x, Q_3(x) = 6x^2 + 2 ...
				// the general recurrence relation for Q_n is:
				// Q_n(x) = (1-x^2) Q_(n-1)'(x) + 2(n-1) x Q_(n-1)(x)
				// as per polynomial parity, we can store coefficients of both Q_(n-1) and Q_n in the same array
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] q = new double[order];
				double[] q = new double[order];
				q[0] = 1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double x2 = x * x;
				double x2 = x * x;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double f = 1.0 / (1 - x2);
				double f = 1.0 / (1 - x2);
				double coeff = f;
				function[1] = coeff * q[0];
				for (int n = 2; n <= order; ++n)
				{

					// update and evaluate polynomial Q_n(x)
					double v = 0;
					q[n - 1] = n * q[n - 2];
					for (int k = n - 1; k >= 0; k -= 2)
					{
						v = v * x2 + q[k];
						if (k > 2)
						{
							q[k - 2] = (k - 1) * q[k - 1] + (2 * n - k + 1) * q[k - 3];
						}
						else if (k == 2)
						{
							q[0] = q[1];
						}
					}
					if ((n & 0x1) == 0)
					{
						v *= x;
					}

					coeff *= f;
					function[n] = coeff * v;

				}
			}

			// apply function composition
			compose(operand, operandOffset, function, result, resultOffset);

		}

		/// <summary>
		/// Compute composition of a derivative structure by a function. </summary>
		/// <param name="operand"> array holding the operand </param>
		/// <param name="operandOffset"> offset of the operand in its array </param>
		/// <param name="f"> array of value and derivatives of the function at
		/// the current point (i.e. at {@code operand[operandOffset]}). </param>
		/// <param name="result"> array where result must be stored (for
		/// composition the result array <em>cannot</em> be the input
		/// array) </param>
		/// <param name="resultOffset"> offset of the result in its array </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void compose(final double[] operand, final int operandOffset, final double[] f, final double[] result, final int resultOffset)
		public virtual void compose(double[] operand, int operandOffset, double[] f, double[] result, int resultOffset)
		{
			for (int i = 0; i < compIndirection.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[][] mappingI = compIndirection[i];
				int[][] mappingI = compIndirection[i];
				double r = 0;
				for (int j = 0; j < mappingI.Length; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] mappingIJ = mappingI[j];
					int[] mappingIJ = mappingI[j];
					double product = mappingIJ[0] * f[mappingIJ[1]];
					for (int k = 2; k < mappingIJ.Length; ++k)
					{
						product *= operand[operandOffset + mappingIJ[k]];
					}
					r += product;
				}
				result[resultOffset + i] = r;
			}
		}

		/// <summary>
		/// Evaluate Taylor expansion of a derivative structure. </summary>
		/// <param name="ds"> array holding the derivative structure </param>
		/// <param name="dsOffset"> offset of the derivative structure in its array </param>
		/// <param name="delta"> parameters offsets (&Delta;x, &Delta;y, ...) </param>
		/// <returns> value of the Taylor expansion at x + &Delta;x, y + &Delta;y, ... </returns>
		/// <exception cref="MathArithmeticException"> if factorials becomes too large </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double taylor(final double[] ds, final int dsOffset, final double... delta) throws org.apache.commons.math3.exception.MathArithmeticException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double taylor(double[] ds, int dsOffset, params double[] delta)
		{
			double value = 0;
			for (int i = Size - 1; i >= 0; --i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] orders = getPartialDerivativeOrders(i);
				int[] orders = getPartialDerivativeOrders(i);
				double term = ds[dsOffset + i];
				for (int k = 0; k < orders.Length; ++k)
				{
					if (orders[k] > 0)
					{
						try
						{
							term *= FastMath.pow(delta[k], orders[k]) / CombinatoricsUtils.factorial(orders[k]);
						}
						catch (NotPositiveException e)
						{
							// this cannot happen
							throw new MathInternalError(e);
						}
					}
				}
				value += term;
			}
			return value;
		}

		/// <summary>
		/// Check rules set compatibility. </summary>
		/// <param name="compiler"> other compiler to check against instance </param>
		/// <exception cref="DimensionMismatchException"> if number of free parameters or orders are inconsistent </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void checkCompatibility(final DSCompiler compiler) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void checkCompatibility(DSCompiler compiler)
		{
			if (parameters != compiler.parameters)
			{
				throw new DimensionMismatchException(parameters, compiler.parameters);
			}
			if (order != compiler.order)
			{
				throw new DimensionMismatchException(order, compiler.order);
			}
		}

	}

}