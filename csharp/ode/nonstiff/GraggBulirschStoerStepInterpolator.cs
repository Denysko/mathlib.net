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

namespace org.apache.commons.math3.ode.nonstiff
{


	using AbstractStepInterpolator = org.apache.commons.math3.ode.sampling.AbstractStepInterpolator;
	using StepInterpolator = org.apache.commons.math3.ode.sampling.StepInterpolator;
	using FastMath = org.apache.commons.math3.util.FastMath;

	/// <summary>
	/// This class implements an interpolator for the Gragg-Bulirsch-Stoer
	/// integrator.
	/// 
	/// <p>This interpolator compute dense output inside the last step
	/// produced by a Gragg-Bulirsch-Stoer integrator.</p>
	/// 
	/// <p>
	/// This implementation is basically a reimplementation in Java of the
	/// <a
	/// href="http://www.unige.ch/math/folks/hairer/prog/nonstiff/odex.f">odex</a>
	/// fortran code by E. Hairer and G. Wanner. The redistribution policy
	/// for this code is available <a
	/// href="http://www.unige.ch/~hairer/prog/licence.txt">here</a>, for
	/// convenience, it is reproduced below.</p>
	/// </p>
	/// 
	/// <table border="0" width="80%" cellpadding="10" align="center" bgcolor="#E0E0E0">
	/// <tr><td>Copyright (c) 2004, Ernst Hairer</td></tr>
	/// 
	/// <tr><td>Redistribution and use in source and binary forms, with or
	/// without modification, are permitted provided that the following
	/// conditions are met:
	/// <ul>
	///  <li>Redistributions of source code must retain the above copyright
	///      notice, this list of conditions and the following disclaimer.</li>
	///  <li>Redistributions in binary form must reproduce the above copyright
	///      notice, this list of conditions and the following disclaimer in the
	///      documentation and/or other materials provided with the distribution.</li>
	/// </ul></td></tr>
	/// 
	/// <tr><td><strong>THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
	/// CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,
	/// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
	/// FOR A  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR
	/// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
	/// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
	/// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
	/// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
	/// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
	/// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
	/// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.</strong></td></tr>
	/// </table>
	/// </summary>
	/// <seealso cref= GraggBulirschStoerIntegrator
	/// @version $Id: GraggBulirschStoerStepInterpolator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	internal class GraggBulirschStoerStepInterpolator : AbstractStepInterpolator
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = 20110928L;

		/// <summary>
		/// Slope at the beginning of the step. </summary>
		private double[] y0Dot;

		/// <summary>
		/// State at the end of the step. </summary>
		private double[] y1;

		/// <summary>
		/// Slope at the end of the step. </summary>
		private double[] y1Dot;

		/// <summary>
		/// Derivatives at the middle of the step.
		/// element 0 is state at midpoint, element 1 is first derivative ...
		/// </summary>
		private double[][] yMidDots;

		/// <summary>
		/// Interpolation polynomials. </summary>
		private double[][] polynomials;

		/// <summary>
		/// Error coefficients for the interpolation. </summary>
		private double[] errfac;

		/// <summary>
		/// Degree of the interpolation polynomials. </summary>
		private int currentDegree;

	  /// <summary>
	  /// Simple constructor.
	  /// This constructor should not be used directly, it is only intended
	  /// for the serialization process.
	  /// </summary>
	  public GraggBulirschStoerStepInterpolator()
	  {
		y0Dot = null;
		y1 = null;
		y1Dot = null;
		yMidDots = null;
		resetTables(-1);
	  }

	  /// <summary>
	  /// Simple constructor. </summary>
	  /// <param name="y"> reference to the integrator array holding the current state </param>
	  /// <param name="y0Dot"> reference to the integrator array holding the slope
	  /// at the beginning of the step </param>
	  /// <param name="y1"> reference to the integrator array holding the state at
	  /// the end of the step </param>
	  /// <param name="y1Dot"> reference to the integrator array holding the slope
	  /// at the end of the step </param>
	  /// <param name="yMidDots"> reference to the integrator array holding the
	  /// derivatives at the middle point of the step </param>
	  /// <param name="forward"> integration direction indicator </param>
	  /// <param name="primaryMapper"> equations mapper for the primary equations set </param>
	  /// <param name="secondaryMappers"> equations mappers for the secondary equations sets </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GraggBulirschStoerStepInterpolator(final double[] y, final double[] y0Dot, final double[] y1, final double[] y1Dot, final double[][] yMidDots, final boolean forward, final org.apache.commons.math3.ode.EquationsMapper primaryMapper, final org.apache.commons.math3.ode.EquationsMapper[] secondaryMappers)
	  public GraggBulirschStoerStepInterpolator(double[] y, double[] y0Dot, double[] y1, double[] y1Dot, double[][] yMidDots, bool forward, EquationsMapper primaryMapper, EquationsMapper[] secondaryMappers) : base(y, forward, primaryMapper, secondaryMappers)
	  {

		this.y0Dot = y0Dot;
		this.y1 = y1;
		this.y1Dot = y1Dot;
		this.yMidDots = yMidDots;

		resetTables(yMidDots.Length + 4);

	  }

	  /// <summary>
	  /// Copy constructor. </summary>
	  /// <param name="interpolator"> interpolator to copy from. The copy is a deep
	  /// copy: its arrays are separated from the original arrays of the
	  /// instance </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public GraggBulirschStoerStepInterpolator(final GraggBulirschStoerStepInterpolator interpolator)
	  public GraggBulirschStoerStepInterpolator(GraggBulirschStoerStepInterpolator interpolator) : base(interpolator)
	  {


//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = currentState.length;
		int dimension = currentState.Length;

		// the interpolator has been finalized,
		// the following arrays are not needed anymore
		y0Dot = null;
		y1 = null;
		y1Dot = null;
		yMidDots = null;

		// copy the interpolation polynomials (up to the current degree only)
		if (interpolator.polynomials == null)
		{
		  polynomials = null;
		  currentDegree = -1;
		}
		else
		{
		  resetTables(interpolator.currentDegree);
		  for (int i = 0; i < polynomials.Length; ++i)
		  {
			polynomials[i] = new double[dimension];
			Array.Copy(interpolator.polynomials[i], 0, polynomials[i], 0, dimension);
		  }
		  currentDegree = interpolator.currentDegree;
		}

	  }

	  /// <summary>
	  /// Reallocate the internal tables.
	  /// Reallocate the internal tables in order to be able to handle
	  /// interpolation polynomials up to the given degree </summary>
	  /// <param name="maxDegree"> maximal degree to handle </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private void resetTables(final int maxDegree)
	  private void resetTables(int maxDegree)
	  {

		if (maxDegree < 0)
		{
		  polynomials = null;
		  errfac = null;
		  currentDegree = -1;
		}
		else
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[][] newPols = new double[maxDegree + 1][];
		  double[][] newPols = new double[maxDegree + 1][];
		  if (polynomials != null)
		  {
			Array.Copy(polynomials, 0, newPols, 0, polynomials.Length);
			for (int i = polynomials.Length; i < newPols.Length; ++i)
			{
			  newPols[i] = new double[currentState.Length];
			}
		  }
		  else
		  {
			for (int i = 0; i < newPols.Length; ++i)
			{
			  newPols[i] = new double[currentState.Length];
			}
		  }
		  polynomials = newPols;

		  // initialize the error factors array for interpolation
		  if (maxDegree <= 4)
		  {
			errfac = null;
		  }
		  else
		  {
			errfac = new double[maxDegree - 4];
			for (int i = 0; i < errfac.Length; ++i)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int ip5 = i + 5;
			  int ip5 = i + 5;
			  errfac[i] = 1.0 / (ip5 * ip5);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double e = 0.5 * org.apache.commons.math3.util.FastMath.sqrt(((double)(i + 1)) / ip5);
			  double e = 0.5 * FastMath.sqrt(((double)(i + 1)) / ip5);
			  for (int j = 0; j <= i; ++j)
			  {
				errfac[i] *= e / (j + 1);
			  }
			}
		  }

		  currentDegree = 0;

		}

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
	  protected internal override StepInterpolator doCopy()
	  {
		return new GraggBulirschStoerStepInterpolator(this);
	  }


	  /// <summary>
	  /// Compute the interpolation coefficients for dense output. </summary>
	  /// <param name="mu"> degree of the interpolation polynomial </param>
	  /// <param name="h"> current step </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void computeCoefficients(final int mu, final double h)
	  public virtual void computeCoefficients(int mu, double h)
	  {

		if ((polynomials == null) || (polynomials.Length <= (mu + 4)))
		{
		  resetTables(mu + 4);
		}

		currentDegree = mu + 4;

		for (int i = 0; i < currentState.Length; ++i)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yp0 = h * y0Dot[i];
		  double yp0 = h * y0Dot[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double yp1 = h * y1Dot[i];
		  double yp1 = h * y1Dot[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ydiff = y1[i] - currentState[i];
		  double ydiff = y1[i] - currentState[i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double aspl = ydiff - yp1;
		  double aspl = ydiff - yp1;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double bspl = yp0 - ydiff;
		  double bspl = yp0 - ydiff;

		  polynomials[0][i] = currentState[i];
		  polynomials[1][i] = ydiff;
		  polynomials[2][i] = aspl;
		  polynomials[3][i] = bspl;

		  if (mu < 0)
		  {
			return;
		  }

		  // compute the remaining coefficients
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ph0 = 0.5 * (currentState[i] + y1[i]) + 0.125 * (aspl + bspl);
		  double ph0 = 0.5 * (currentState[i] + y1[i]) + 0.125 * (aspl + bspl);
		  polynomials[4][i] = 16 * (yMidDots[0][i] - ph0);

		  if (mu > 0)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ph1 = ydiff + 0.25 * (aspl - bspl);
			double ph1 = ydiff + 0.25 * (aspl - bspl);
			polynomials[5][i] = 16 * (yMidDots[1][i] - ph1);

			if (mu > 1)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ph2 = yp1 - yp0;
			  double ph2 = yp1 - yp0;
			  polynomials[6][i] = 16 * (yMidDots[2][i] - ph2 + polynomials[4][i]);

			  if (mu > 2)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double ph3 = 6 * (bspl - aspl);
				double ph3 = 6 * (bspl - aspl);
				polynomials[7][i] = 16 * (yMidDots[3][i] - ph3 + 3 * polynomials[5][i]);

				for (int j = 4; j <= mu; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fac1 = 0.5 * j * (j - 1);
				  double fac1 = 0.5 * j * (j - 1);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double fac2 = 2 * fac1 * (j - 2) * (j - 3);
				  double fac2 = 2 * fac1 * (j - 2) * (j - 3);
				  polynomials[j + 4][i] = 16 * (yMidDots[j][i] + fac1 * polynomials[j + 2][i] - fac2 * polynomials[j][i]);
				}

			  }
			}
		  }
		}

	  }

	  /// <summary>
	  /// Estimate interpolation error. </summary>
	  /// <param name="scale"> scaling array </param>
	  /// <returns> estimate of the interpolation error </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double estimateError(final double[] scale)
	  public virtual double estimateError(double[] scale)
	  {
		double error = 0;
		if (currentDegree >= 5)
		{
		  for (int i = 0; i < scale.Length; ++i)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double e = polynomials[currentDegree][i] / scale[i];
			double e = polynomials[currentDegree][i] / scale[i];
			error += e * e;
		  }
		  error = FastMath.sqrt(error / scale.Length) * errfac[currentDegree - 5];
		}
		return error;
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected void computeInterpolatedStateAndDerivatives(final double theta, final double oneMinusThetaH)
	  protected internal override void computeInterpolatedStateAndDerivatives(double theta, double oneMinusThetaH)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = currentState.length;
		int dimension = currentState.Length;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double oneMinusTheta = 1.0 - theta;
		double oneMinusTheta = 1.0 - theta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double theta05 = theta - 0.5;
		double theta05 = theta - 0.5;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double tOmT = theta * oneMinusTheta;
		double tOmT = theta * oneMinusTheta;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t4 = tOmT * tOmT;
		double t4 = tOmT * tOmT;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t4Dot = 2 * tOmT * (1 - 2 * theta);
		double t4Dot = 2 * tOmT * (1 - 2 * theta);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dot1 = 1.0 / h;
		double dot1 = 1.0 / h;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dot2 = theta * (2 - 3 * theta) / h;
		double dot2 = theta * (2 - 3 * theta) / h;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double dot3 = ((3 * theta - 4) * theta + 1) / h;
		double dot3 = ((3 * theta - 4) * theta + 1) / h;

		for (int i = 0; i < dimension; ++i)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p0 = polynomials[0][i];
			double p0 = polynomials[0][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p1 = polynomials[1][i];
			double p1 = polynomials[1][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p2 = polynomials[2][i];
			double p2 = polynomials[2][i];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p3 = polynomials[3][i];
			double p3 = polynomials[3][i];
			interpolatedState[i] = p0 + theta * (p1 + oneMinusTheta * (p2 * theta + p3 * oneMinusTheta));
			interpolatedDerivatives[i] = dot1 * p1 + dot2 * p2 + dot3 * p3;

			if (currentDegree > 3)
			{
				double cDot = 0;
				double c = polynomials[currentDegree][i];
				for (int j = currentDegree - 1; j > 3; --j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double d = 1.0 / (j - 3);
					double d = 1.0 / (j - 3);
					cDot = d * (theta05 * cDot + c);
					c = polynomials[j][i] + c * d * theta05;
				}
				interpolatedState[i] += t4 * c;
				interpolatedDerivatives[i] += (t4 * cDot + t4Dot * c) / h;
			}

		}

		if (h == 0)
		{
			// in this degenerated case, the previous computation leads to NaN for derivatives
			// we fix this by using the derivatives at midpoint
			Array.Copy(yMidDots[1], 0, interpolatedDerivatives, 0, dimension);
		}

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void writeExternal(final java.io.ObjectOutput out) throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public override void writeExternal(ObjectOutput @out)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = (currentState == null) ? -1 : currentState.length;
		int dimension = (currentState == null) ? - 1 : currentState.Length;

		// save the state of the base class
		writeBaseExternal(@out);

		// save the local attributes (but not the temporary vectors)
		@out.writeInt(currentDegree);
		for (int k = 0; k <= currentDegree; ++k)
		{
		  for (int l = 0; l < dimension; ++l)
		  {
			@out.writeDouble(polynomials[k][l]);
		  }
		}

	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void readExternal(final java.io.ObjectInput in) throws java.io.IOException, ClassNotFoundException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
	  public override void readExternal(ObjectInput @in)
	  {

		// read the base class
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double t = readBaseExternal(in);
		double t = readBaseExternal(@in);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int dimension = (currentState == null) ? -1 : currentState.length;
		int dimension = (currentState == null) ? - 1 : currentState.Length;

		// read the local attributes
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int degree = in.readInt();
		int degree = @in.readInt();
		resetTables(degree);
		currentDegree = degree;

		for (int k = 0; k <= currentDegree; ++k)
		{
		  for (int l = 0; l < dimension; ++l)
		  {
			polynomials[k][l] = @in.readDouble();
		  }
		}

		// we can now set the interpolated time and state
		InterpolatedTime = t;

	  }

	}

}