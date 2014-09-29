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

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using NoBracketingException = mathlib.exception.NoBracketingException;
	using NumberIsTooSmallException = mathlib.exception.NumberIsTooSmallException;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;


	/// <summary>
	/// Base class for <seealso cref="AdamsBashforthIntegrator Adams-Bashforth"/> and
	/// <seealso cref="AdamsMoultonIntegrator Adams-Moulton"/> integrators.
	/// @version $Id: AdamsIntegrator.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public abstract class AdamsIntegrator : MultistepIntegrator
	{

		/// <summary>
		/// Transformer. </summary>
		private readonly AdamsNordsieckTransformer transformer;

		/// <summary>
		/// Build an Adams integrator with the given order and step control parameters. </summary>
		/// <param name="name"> name of the method </param>
		/// <param name="nSteps"> number of steps of the method excluding the one being computed </param>
		/// <param name="order"> order of the method </param>
		/// <param name="minStep"> minimal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="scalAbsoluteTolerance"> allowed absolute error </param>
		/// <param name="scalRelativeTolerance"> allowed relative error </param>
		/// <exception cref="NumberIsTooSmallException"> if order is 1 or less </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdamsIntegrator(final String name, final int nSteps, final int order, final double minStep, final double maxStep, final double scalAbsoluteTolerance, final double scalRelativeTolerance) throws mathlib.exception.NumberIsTooSmallException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AdamsIntegrator(string name, int nSteps, int order, double minStep, double maxStep, double scalAbsoluteTolerance, double scalRelativeTolerance) : base(name, nSteps, order, minStep, maxStep, scalAbsoluteTolerance, scalRelativeTolerance)
		{
			transformer = AdamsNordsieckTransformer.getInstance(nSteps);
		}

		/// <summary>
		/// Build an Adams integrator with the given order and step control parameters. </summary>
		/// <param name="name"> name of the method </param>
		/// <param name="nSteps"> number of steps of the method excluding the one being computed </param>
		/// <param name="order"> order of the method </param>
		/// <param name="minStep"> minimal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="maxStep"> maximal step (sign is irrelevant, regardless of
		/// integration direction, forward or backward), the last step can
		/// be smaller than this </param>
		/// <param name="vecAbsoluteTolerance"> allowed absolute error </param>
		/// <param name="vecRelativeTolerance"> allowed relative error </param>
		/// <exception cref="IllegalArgumentException"> if order is 1 or less </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public AdamsIntegrator(final String name, final int nSteps, final int order, final double minStep, final double maxStep, final double[] vecAbsoluteTolerance, final double[] vecRelativeTolerance) throws IllegalArgumentException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public AdamsIntegrator(string name, int nSteps, int order, double minStep, double maxStep, double[] vecAbsoluteTolerance, double[] vecRelativeTolerance) : base(name, nSteps, order, minStep, maxStep, vecAbsoluteTolerance, vecRelativeTolerance)
		{
			transformer = AdamsNordsieckTransformer.getInstance(nSteps);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public abstract void integrate(final mathlib.ode.ExpandableStatefulODE equations, final double t) throws mathlib.exception.NumberIsTooSmallException, mathlib.exception.DimensionMismatchException, mathlib.exception.MaxCountExceededException, mathlib.exception.NoBracketingException;
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override abstract void integrate(ExpandableStatefulODE equations, double t);

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override protected mathlib.linear.Array2DRowRealMatrix initializeHighOrderDerivatives(final double h, final double[] t, final double[][] y, final double[][] yDot)
		protected internal override Array2DRowRealMatrix initializeHighOrderDerivatives(double h, double[] t, double[][] y, double[][] yDot)
		{
			return transformer.initializeHighOrderDerivatives(h, t, y, yDot);
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
			return transformer.updateHighOrderDerivativesPhase1(highOrder);
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
			transformer.updateHighOrderDerivativesPhase2(start, end, highOrder);
		}

	}

}