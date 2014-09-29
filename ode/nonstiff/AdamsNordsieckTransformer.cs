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

namespace mathlib.ode.nonstiff
{


	using BigFraction = mathlib.fraction.BigFraction;
	using mathlib.linear;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using mathlib.linear;
	using mathlib.linear;
	using mathlib.linear;
	using mathlib.linear;
	using MatrixUtils = mathlib.linear.MatrixUtils;
	using QRDecomposition = mathlib.linear.QRDecomposition;
	using RealMatrix = mathlib.linear.RealMatrix;

	/// <summary>
	/// Transformer to Nordsieck vectors for Adams integrators.
	/// <p>This class is used by <seealso cref="AdamsBashforthIntegrator Adams-Bashforth"/> and
	/// <seealso cref="AdamsMoultonIntegrator Adams-Moulton"/> integrators to convert between
	/// classical representation with several previous first derivatives and Nordsieck
	/// representation with higher order scaled derivatives.</p>
	/// 
	/// <p>We define scaled derivatives s<sub>i</sub>(n) at step n as:
	/// <pre>
	/// s<sub>1</sub>(n) = h y'<sub>n</sub> for first derivative
	/// s<sub>2</sub>(n) = h<sup>2</sup>/2 y''<sub>n</sub> for second derivative
	/// s<sub>3</sub>(n) = h<sup>3</sup>/6 y'''<sub>n</sub> for third derivative
	/// ...
	/// s<sub>k</sub>(n) = h<sup>k</sup>/k! y<sup>(k)</sup><sub>n</sub> for k<sup>th</sup> derivative
	/// </pre></p>
	/// 
	/// <p>With the previous definition, the classical representation of multistep methods
	/// uses first derivatives only, i.e. it handles y<sub>n</sub>, s<sub>1</sub>(n) and
	/// q<sub>n</sub> where q<sub>n</sub> is defined as:
	/// <pre>
	///   q<sub>n</sub> = [ s<sub>1</sub>(n-1) s<sub>1</sub>(n-2) ... s<sub>1</sub>(n-(k-1)) ]<sup>T</sup>
	/// </pre>
	/// (we omit the k index in the notation for clarity).</p>
	/// 
	/// <p>Another possible representation uses the Nordsieck vector with
	/// higher degrees scaled derivatives all taken at the same step, i.e it handles y<sub>n</sub>,
	/// s<sub>1</sub>(n) and r<sub>n</sub>) where r<sub>n</sub> is defined as:
	/// <pre>
	/// r<sub>n</sub> = [ s<sub>2</sub>(n), s<sub>3</sub>(n) ... s<sub>k</sub>(n) ]<sup>T</sup>
	/// </pre>
	/// (here again we omit the k index in the notation for clarity)
	/// </p>
	/// 
	/// <p>Taylor series formulas show that for any index offset i, s<sub>1</sub>(n-i) can be
	/// computed from s<sub>1</sub>(n), s<sub>2</sub>(n) ... s<sub>k</sub>(n), the formula being exact
	/// for degree k polynomials.
	/// <pre>
	/// s<sub>1</sub>(n-i) = s<sub>1</sub>(n) + &sum;<sub>j&gt;1</sub> j (-i)<sup>j-1</sup> s<sub>j</sub>(n)
	/// </pre>
	/// The previous formula can be used with several values for i to compute the transform between
	/// classical representation and Nordsieck vector at step end. The transform between r<sub>n</sub>
	/// and q<sub>n</sub> resulting from the Taylor series formulas above is:
	/// <pre>
	/// q<sub>n</sub> = s<sub>1</sub>(n) u + P r<sub>n</sub>
	/// </pre>
	/// where u is the [ 1 1 ... 1 ]<sup>T</sup> vector and P is the (k-1)&times;(k-1) matrix built
	/// with the j (-i)<sup>j-1</sup> terms:
	/// <pre>
	///        [  -2   3   -4    5  ... ]
	///        [  -4  12  -32   80  ... ]
	///   P =  [  -6  27 -108  405  ... ]
	///        [  -8  48 -256 1280  ... ]
	///        [          ...           ]
	/// </pre></p>
	/// 
	/// <p>Changing -i into +i in the formula above can be used to compute a similar transform between
	/// classical representation and Nordsieck vector at step start. The resulting matrix is simply
	/// the absolute value of matrix P.</p>
	/// 
	/// <p>For <seealso cref="AdamsBashforthIntegrator Adams-Bashforth"/> method, the Nordsieck vector
	/// at step n+1 is computed from the Nordsieck vector at step n as follows:
	/// <ul>
	///   <li>y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n) + u<sup>T</sup> r<sub>n</sub></li>
	///   <li>s<sub>1</sub>(n+1) = h f(t<sub>n+1</sub>, y<sub>n+1</sub>)</li>
	///   <li>r<sub>n+1</sub> = (s<sub>1</sub>(n) - s<sub>1</sub>(n+1)) P<sup>-1</sup> u + P<sup>-1</sup> A P r<sub>n</sub></li>
	/// </ul>
	/// where A is a rows shifting matrix (the lower left part is an identity matrix):
	/// <pre>
	///        [ 0 0   ...  0 0 | 0 ]
	///        [ ---------------+---]
	///        [ 1 0   ...  0 0 | 0 ]
	///    A = [ 0 1   ...  0 0 | 0 ]
	///        [       ...      | 0 ]
	///        [ 0 0   ...  1 0 | 0 ]
	///        [ 0 0   ...  0 1 | 0 ]
	/// </pre></p>
	/// 
	/// <p>For <seealso cref="AdamsMoultonIntegrator Adams-Moulton"/> method, the predicted Nordsieck vector
	/// at step n+1 is computed from the Nordsieck vector at step n as follows:
	/// <ul>
	///   <li>Y<sub>n+1</sub> = y<sub>n</sub> + s<sub>1</sub>(n) + u<sup>T</sup> r<sub>n</sub></li>
	///   <li>S<sub>1</sub>(n+1) = h f(t<sub>n+1</sub>, Y<sub>n+1</sub>)</li>
	///   <li>R<sub>n+1</sub> = (s<sub>1</sub>(n) - s<sub>1</sub>(n+1)) P<sup>-1</sup> u + P<sup>-1</sup> A P r<sub>n</sub></li>
	/// </ul>
	/// From this predicted vector, the corrected vector is computed as follows:
	/// <ul>
	///   <li>y<sub>n+1</sub> = y<sub>n</sub> + S<sub>1</sub>(n+1) + [ -1 +1 -1 +1 ... &plusmn;1 ] r<sub>n+1</sub></li>
	///   <li>s<sub>1</sub>(n+1) = h f(t<sub>n+1</sub>, y<sub>n+1</sub>)</li>
	///   <li>r<sub>n+1</sub> = R<sub>n+1</sub> + (s<sub>1</sub>(n+1) - S<sub>1</sub>(n+1)) P<sup>-1</sup> u</li>
	/// </ul>
	/// where the upper case Y<sub>n+1</sub>, S<sub>1</sub>(n+1) and R<sub>n+1</sub> represent the
	/// predicted states whereas the lower case y<sub>n+1</sub>, s<sub>n+1</sub> and r<sub>n+1</sub>
	/// represent the corrected states.</p>
	/// 
	/// <p>We observe that both methods use similar update formulas. In both cases a P<sup>-1</sup>u
	/// vector and a P<sup>-1</sup> A P matrix are used that do not depend on the state,
	/// they only depend on k. This class handles these transformations.</p>
	/// 
	/// @version $Id: AdamsNordsieckTransformer.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public class AdamsNordsieckTransformer
	{

		/// <summary>
		/// Cache for already computed coefficients. </summary>
		private static readonly IDictionary<int?, AdamsNordsieckTransformer> CACHE = new Dictionary<int?, AdamsNordsieckTransformer>();

		/// <summary>
		/// Update matrix for the higher order derivatives h<sup>2</sup>/2y'', h<sup>3</sup>/6 y''' ... </summary>
		private readonly Array2DRowRealMatrix update;

		/// <summary>
		/// Update coefficients of the higher order derivatives wrt y'. </summary>
		private readonly double[] c1;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="nSteps"> number of steps of the multistep method
		/// (excluding the one being computed) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private AdamsNordsieckTransformer(final int nSteps)
		private AdamsNordsieckTransformer(int nSteps)
		{

			// compute exact coefficients
			FieldMatrix<BigFraction> bigP = buildP(nSteps);
			FieldDecompositionSolver<BigFraction> pSolver = (new FieldLUDecomposition<BigFraction>(bigP)).Solver;

			BigFraction[] u = new BigFraction[nSteps];
			Arrays.fill(u, BigFraction.ONE);
			BigFraction[] bigC1 = pSolver.solve(new ArrayFieldVector<BigFraction>(u, false)).toArray();

			// update coefficients are computed by combining transform from
			// Nordsieck to multistep, then shifting rows to represent step advance
			// then applying inverse transform
			BigFraction[][] shiftedP = bigP.Data;
			for (int i = shiftedP.Length - 1; i > 0; --i)
			{
				// shift rows
				shiftedP[i] = shiftedP[i - 1];
			}
			shiftedP[0] = new BigFraction[nSteps];
			Arrays.fill(shiftedP[0], BigFraction.ZERO);
			FieldMatrix<BigFraction> bigMSupdate = pSolver.solve(new Array2DRowFieldMatrix<BigFraction>(shiftedP, false));

			// convert coefficients to double
			update = MatrixUtils.bigFractionMatrixToRealMatrix(bigMSupdate);
			c1 = new double[nSteps];
			for (int i = 0; i < nSteps; ++i)
			{
				c1[i] = (double)bigC1[i];
			}

		}

		/// <summary>
		/// Get the Nordsieck transformer for a given number of steps. </summary>
		/// <param name="nSteps"> number of steps of the multistep method
		/// (excluding the one being computed) </param>
		/// <returns> Nordsieck transformer for the specified number of steps </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static AdamsNordsieckTransformer getInstance(final int nSteps)
		public static AdamsNordsieckTransformer getInstance(int nSteps)
		{
			lock (CACHE)
			{
				AdamsNordsieckTransformer t = CACHE[nSteps];
				if (t == null)
				{
					t = new AdamsNordsieckTransformer(nSteps);
					CACHE[nSteps] = t;
				}
				return t;
			}
		}

		/// <summary>
		/// Get the number of steps of the method
		/// (excluding the one being computed). </summary>
		/// <returns> number of steps of the method
		/// (excluding the one being computed) </returns>
		public virtual int NSteps
		{
			get
			{
				return c1.Length;
			}
		}

		/// <summary>
		/// Build the P matrix.
		/// <p>The P matrix general terms are shifted j (-i)<sup>j-1</sup> terms:
		/// <pre>
		///        [  -2   3   -4    5  ... ]
		///        [  -4  12  -32   80  ... ]
		///   P =  [  -6  27 -108  405  ... ]
		///        [  -8  48 -256 1280  ... ]
		///        [          ...           ]
		/// </pre></p> </summary>
		/// <param name="nSteps"> number of steps of the multistep method
		/// (excluding the one being computed) </param>
		/// <returns> P matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private mathlib.linear.FieldMatrix<mathlib.fraction.BigFraction> buildP(final int nSteps)
		private FieldMatrix<BigFraction> buildP(int nSteps)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[][] pData = new mathlib.fraction.BigFraction[nSteps][nSteps];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: BigFraction[][] pData = new BigFraction[nSteps][nSteps];
			BigFraction[][] pData = RectangularArrays.ReturnRectangularBigFractionArray(nSteps, nSteps);

			for (int i = 0; i < pData.Length; ++i)
			{
				// build the P matrix elements from Taylor series formulas
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final mathlib.fraction.BigFraction[] pI = pData[i];
				BigFraction[] pI = pData[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int factor = -(i + 1);
				int factor = -(i + 1);
				int aj = factor;
				for (int j = 0; j < pI.Length; ++j)
				{
					pI[j] = new BigFraction(aj * (j + 2));
					aj *= factor;
				}
			}

			return new Array2DRowFieldMatrix<BigFraction>(pData, false);

		}

		/// <summary>
		/// Initialize the high order scaled derivatives at step start. </summary>
		/// <param name="h"> step size to use for scaling </param>
		/// <param name="t"> first steps times </param>
		/// <param name="y"> first steps states </param>
		/// <param name="yDot"> first steps derivatives </param>
		/// <returns> Nordieck vector at first step (h<sup>2</sup>/2 y''<sub>n</sub>,
		/// h<sup>3</sup>/6 y'''<sub>n</sub> ... h<sup>k</sup>/k! y<sup>(k)</sup><sub>n</sub>) </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.linear.Array2DRowRealMatrix initializeHighOrderDerivatives(final double h, final double[] t, final double[][] y, final double[][] yDot)
		public virtual Array2DRowRealMatrix initializeHighOrderDerivatives(double h, double[] t, double[][] y, double[][] yDot)
		{

			// using Taylor series with di = ti - t0, we get:
			//  y(ti)  - y(t0)  - di y'(t0) =   di^2 / h^2 s2 + ... +   di^k     / h^k sk + O(h^(k+1))
			//  y'(ti) - y'(t0)             = 2 di   / h^2 s2 + ... + k di^(k-1) / h^k sk + O(h^k)
			// we write these relations for i = 1 to i= n-1 as a set of 2(n-1) linear
			// equations depending on the Nordsieck vector [s2 ... sk]
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] a = new double[2 * (y.length - 1)][c1.length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] a = new double[2 * (y.Length - 1)][c1.Length];
			double[][] a = RectangularArrays.ReturnRectangularDoubleArray(2 * (y.Length - 1), c1.Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] b = new double[2 * (y.length - 1)][y[0].length];
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] b = new double[2 * (y.Length - 1)][y[0].Length];
			double[][] b = RectangularArrays.ReturnRectangularDoubleArray(2 * (y.Length - 1), y[0].Length);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] y0 = y[0];
			double[] y0 = y[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDot0 = yDot[0];
			double[] yDot0 = yDot[0];
			for (int i = 1; i < y.Length; ++i)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double di = t[i] - t[0];
				double di = t[i] - t[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ratio = di / h;
				double ratio = di / h;
				double dikM1Ohk = 1 / h;

				// linear coefficients of equations
				// y(ti) - y(t0) - di y'(t0) and y'(ti) - y'(t0)
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] aI = a[2 * i - 2];
				double[] aI = a[2 * i - 2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] aDotI = a[2 * i - 1];
				double[] aDotI = a[2 * i - 1];
				for (int j = 0; j < aI.Length; ++j)
				{
					dikM1Ohk *= ratio;
					aI[j] = di * dikM1Ohk;
					aDotI[j] = (j + 2) * dikM1Ohk;
				}

				// expected value of the previous equations
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yI = y[i];
				double[] yI = y[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] yDotI = yDot[i];
				double[] yDotI = yDot[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bI = b[2 * i - 2];
				double[] bI = b[2 * i - 2];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] bDotI = b[2 * i - 1];
				double[] bDotI = b[2 * i - 1];
				for (int j = 0; j < yI.Length; ++j)
				{
					bI[j] = yI[j] - y0[j] - di * yDot0[j];
					bDotI[j] = yDotI[j] - yDot0[j];
				}

			}

			// solve the rectangular system in the least square sense
			// to get the best estimate of the Nordsieck vector [s2 ... sk]
			QRDecomposition decomposition;
			decomposition = new QRDecomposition(new Array2DRowRealMatrix(a, false));
			RealMatrix x = decomposition.Solver.solve(new Array2DRowRealMatrix(b, false));
			return new Array2DRowRealMatrix(x.Data, false);
		}

		/// <summary>
		/// Update the high order scaled derivatives for Adams integrators (phase 1).
		/// <p>The complete update of high order derivatives has a form similar to:
		/// <pre>
		/// r<sub>n+1</sub> = (s<sub>1</sub>(n) - s<sub>1</sub>(n+1)) P<sup>-1</sup> u + P<sup>-1</sup> A P r<sub>n</sub>
		/// </pre>
		/// this method computes the P<sup>-1</sup> A P r<sub>n</sub> part.</p> </summary>
		/// <param name="highOrder"> high order scaled derivatives
		/// (h<sup>2</sup>/2 y'', ... h<sup>k</sup>/k! y(k)) </param>
		/// <returns> updated high order derivatives </returns>
		/// <seealso cref= #updateHighOrderDerivativesPhase2(double[], double[], Array2DRowRealMatrix) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public mathlib.linear.Array2DRowRealMatrix updateHighOrderDerivativesPhase1(final mathlib.linear.Array2DRowRealMatrix highOrder)
		public virtual Array2DRowRealMatrix updateHighOrderDerivativesPhase1(Array2DRowRealMatrix highOrder)
		{
			return update.multiply(highOrder);
		}

		/// <summary>
		/// Update the high order scaled derivatives Adams integrators (phase 2).
		/// <p>The complete update of high order derivatives has a form similar to:
		/// <pre>
		/// r<sub>n+1</sub> = (s<sub>1</sub>(n) - s<sub>1</sub>(n+1)) P<sup>-1</sup> u + P<sup>-1</sup> A P r<sub>n</sub>
		/// </pre>
		/// this method computes the (s<sub>1</sub>(n) - s<sub>1</sub>(n+1)) P<sup>-1</sup> u part.</p>
		/// <p>Phase 1 of the update must already have been performed.</p> </summary>
		/// <param name="start"> first order scaled derivatives at step start </param>
		/// <param name="end"> first order scaled derivatives at step end </param>
		/// <param name="highOrder"> high order scaled derivatives, will be modified
		/// (h<sup>2</sup>/2 y'', ... h<sup>k</sup>/k! y(k)) </param>
		/// <seealso cref= #updateHighOrderDerivativesPhase1(Array2DRowRealMatrix) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void updateHighOrderDerivativesPhase2(final double[] start, final double[] end, final mathlib.linear.Array2DRowRealMatrix highOrder)
		public virtual void updateHighOrderDerivativesPhase2(double[] start, double[] end, Array2DRowRealMatrix highOrder)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] data = highOrder.getDataRef();
			double[][] data = highOrder.DataRef;
			for (int i = 0; i < data.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dataI = data[i];
				double[] dataI = data[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double c1I = c1[i];
				double c1I = c1[i];
				for (int j = 0; j < dataI.Length; ++j)
				{
					dataI[j] += c1I * (start[j] - end[j]);
				}
			}
		}

	}

}