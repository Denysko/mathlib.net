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
namespace mathlib.ode
{


	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// This class defines a set of <seealso cref="SecondaryEquations secondary equations"/> to
	/// compute the Jacobian matrices with respect to the initial state vector and, if
	/// any, to some parameters of the primary ODE set.
	/// <p>
	/// It is intended to be packed into an <seealso cref="ExpandableStatefulODE"/>
	/// in conjunction with a primary set of ODE, which may be:
	/// <ul>
	/// <li>a <seealso cref="FirstOrderDifferentialEquations"/></li>
	/// <li>a <seealso cref="MainStateJacobianProvider"/></li>
	/// </ul>
	/// In order to compute Jacobian matrices with respect to some parameters of the
	/// primary ODE set, the following parameter Jacobian providers may be set:
	/// <ul>
	/// <li>a <seealso cref="ParameterJacobianProvider"/></li>
	/// <li>a <seealso cref="ParameterizedODE"/></li>
	/// </ul>
	/// </p>
	/// </summary>
	/// <seealso cref= ExpandableStatefulODE </seealso>
	/// <seealso cref= FirstOrderDifferentialEquations </seealso>
	/// <seealso cref= MainStateJacobianProvider </seealso>
	/// <seealso cref= ParameterJacobianProvider </seealso>
	/// <seealso cref= ParameterizedODE
	/// 
	/// @version $Id: JacobianMatrices.java 1538354 2013-11-03 12:48:40Z tn $
	/// @since 3.0 </seealso>
	public class JacobianMatrices
	{

		/// <summary>
		/// Expandable first order differential equation. </summary>
		private ExpandableStatefulODE efode;

		/// <summary>
		/// Index of the instance in the expandable set. </summary>
		private int index;

		/// <summary>
		/// FODE with exact primary Jacobian computation skill. </summary>
		private MainStateJacobianProvider jode;

		/// <summary>
		/// FODE without exact parameter Jacobian computation skill. </summary>
		private ParameterizedODE pode;

		/// <summary>
		/// Main state vector dimension. </summary>
		private int stateDim;

		/// <summary>
		/// Selected parameters for parameter Jacobian computation. </summary>
		private ParameterConfiguration[] selectedParameters;

		/// <summary>
		/// FODE with exact parameter Jacobian computation skill. </summary>
		private IList<ParameterJacobianProvider> jacobianProviders;

		/// <summary>
		/// Parameters dimension. </summary>
		private int paramDim;

		/// <summary>
		/// Boolean for selected parameters consistency. </summary>
		private bool dirtyParameter;

		/// <summary>
		/// State and parameters Jacobian matrices in a row. </summary>
		private double[] matricesData;

		/// <summary>
		/// Simple constructor for a secondary equations set computing Jacobian matrices.
		/// <p>
		/// Parameters must belong to the supported ones given by {@link
		/// Parameterizable#getParametersNames()}, so the primary set of differential
		/// equations must be <seealso cref="Parameterizable"/>.
		/// </p>
		/// <p>Note that each selection clears the previous selected parameters.</p>
		/// </summary>
		/// <param name="fode"> the primary first order differential equations set to extend </param>
		/// <param name="hY"> step used for finite difference computation with respect to state vector </param>
		/// <param name="parameters"> parameters to consider for Jacobian matrices processing
		/// (may be null if parameters Jacobians is not desired) </param>
		/// <exception cref="DimensionMismatchException"> if there is a dimension mismatch between
		/// the steps array {@code hY} and the equation dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JacobianMatrices(final FirstOrderDifferentialEquations fode, final double[] hY, final String... parameters) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public JacobianMatrices(FirstOrderDifferentialEquations fode, double[] hY, params string[] parameters) : this(new MainStateJacobianWrapper(fode, hY), parameters)
		{
		}

		/// <summary>
		/// Simple constructor for a secondary equations set computing Jacobian matrices.
		/// <p>
		/// Parameters must belong to the supported ones given by {@link
		/// Parameterizable#getParametersNames()}, so the primary set of differential
		/// equations must be <seealso cref="Parameterizable"/>.
		/// </p>
		/// <p>Note that each selection clears the previous selected parameters.</p>
		/// </summary>
		/// <param name="jode"> the primary first order differential equations set to extend </param>
		/// <param name="parameters"> parameters to consider for Jacobian matrices processing
		/// (may be null if parameters Jacobians is not desired) </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public JacobianMatrices(final MainStateJacobianProvider jode, final String... parameters)
		public JacobianMatrices(MainStateJacobianProvider jode, params string[] parameters)
		{

			this.efode = null;
			this.index = -1;

			this.jode = jode;
			this.pode = null;

			this.stateDim = jode.Dimension;

			if (parameters == null)
			{
				selectedParameters = null;
				paramDim = 0;
			}
			else
			{
				this.selectedParameters = new ParameterConfiguration[parameters.Length];
				for (int i = 0; i < parameters.Length; ++i)
				{
					selectedParameters[i] = new ParameterConfiguration(parameters[i], double.NaN);
				}
				paramDim = parameters.Length;
			}
			this.dirtyParameter = false;

			this.jacobianProviders = new List<ParameterJacobianProvider>();

			// set the default initial state Jacobian to the identity
			// and the default initial parameters Jacobian to the null matrix
			matricesData = new double[(stateDim + paramDim) * stateDim];
			for (int i = 0; i < stateDim; ++i)
			{
				matricesData[i * (stateDim + 1)] = 1.0;
			}

		}

		/// <summary>
		/// Register the variational equations for the Jacobians matrices to the expandable set. </summary>
		/// <param name="expandable"> expandable set into which variational equations should be registered </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of the partial state does not
		/// match the selected equations set dimension </exception>
		/// <exception cref="MismatchedEquations"> if the primary set of the expandable set does
		/// not match the one used to build the instance </exception>
		/// <seealso cref= ExpandableStatefulODE#addSecondaryEquations(SecondaryEquations) </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void registerVariationalEquations(final ExpandableStatefulODE expandable) throws mathlib.exception.DimensionMismatchException, MismatchedEquations
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void registerVariationalEquations(ExpandableStatefulODE expandable)
		{

			// safety checks
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FirstOrderDifferentialEquations ode = (jode instanceof MainStateJacobianWrapper) ? ((MainStateJacobianWrapper) jode).ode : jode;
			FirstOrderDifferentialEquations ode = (jode is MainStateJacobianWrapper) ? ((MainStateJacobianWrapper) jode).ode : jode;
			if (expandable.Primary != ode)
			{
				throw new MismatchedEquations();
			}

			efode = expandable;
			index = efode.addSecondaryEquations(new JacobiansSecondaryEquations(this));
			efode.setSecondaryState(index, matricesData);

		}

		/// <summary>
		/// Add a parameter Jacobian provider. </summary>
		/// <param name="provider"> the parameter Jacobian provider to compute exactly the parameter Jacobian matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addParameterJacobianProvider(final ParameterJacobianProvider provider)
		public virtual void addParameterJacobianProvider(ParameterJacobianProvider provider)
		{
			jacobianProviders.Add(provider);
		}

		/// <summary>
		/// Set a parameter Jacobian provider. </summary>
		/// <param name="parameterizedOde"> the parameterized ODE to compute the parameter Jacobian matrix using finite differences </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setParameterizedODE(final ParameterizedODE parameterizedOde)
		public virtual ParameterizedODE ParameterizedODE
		{
			set
			{
				this.pode = value;
				dirtyParameter = true;
			}
		}

		/// <summary>
		/// Set the step associated to a parameter in order to compute by finite
		///  difference the Jacobian matrix.
		/// <p>
		/// Needed if and only if the primary ODE set is a <seealso cref="ParameterizedODE"/>.
		/// </p>
		/// <p>
		/// Given a non zero parameter value pval for the parameter, a reasonable value
		/// for such a step is {@code pval * FastMath.sqrt(Precision.EPSILON)}.
		/// </p>
		/// <p>
		/// A zero value for such a step doesn't enable to compute the parameter Jacobian matrix.
		/// </p> </summary>
		/// <param name="parameter"> parameter to consider for Jacobian processing </param>
		/// <param name="hP"> step for Jacobian finite difference computation w.r.t. the specified parameter </param>
		/// <seealso cref= ParameterizedODE </seealso>
		/// <exception cref="UnknownParameterException"> if the parameter is not supported </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setParameterStep(final String parameter, final double hP) throws UnknownParameterException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setParameterStep(string parameter, double hP)
		{

			foreach (ParameterConfiguration param in selectedParameters)
			{
				if (parameter.Equals(param.ParameterName))
				{
					param.HP = hP;
					dirtyParameter = true;
					return;
				}
			}

			throw new UnknownParameterException(parameter);

		}

		/// <summary>
		/// Set the initial value of the Jacobian matrix with respect to state.
		/// <p>
		/// If this method is not called, the initial value of the Jacobian
		/// matrix with respect to state is set to identity.
		/// </p> </summary>
		/// <param name="dYdY0"> initial Jacobian matrix w.r.t. state </param>
		/// <exception cref="DimensionMismatchException"> if matrix dimensions are incorrect </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInitialMainStateJacobian(final double[][] dYdY0) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[][] InitialMainStateJacobian
		{
			set
			{
    
				// Check dimensions
				checkDimension(stateDim, value);
				checkDimension(stateDim, value[0]);
    
				// store the matrix in row major order as a single dimension array
				int i = 0;
				foreach (double[] row in value)
				{
					Array.Copy(row, 0, matricesData, i, stateDim);
					i += stateDim;
				}
    
				if (efode != null)
				{
					efode.setSecondaryState(index, matricesData);
				}
    
			}
		}

		/// <summary>
		/// Set the initial value of a column of the Jacobian matrix with respect to one parameter.
		/// <p>
		/// If this method is not called for some parameter, the initial value of
		/// the column of the Jacobian matrix with respect to this parameter is set to zero.
		/// </p> </summary>
		/// <param name="pName"> parameter name </param>
		/// <param name="dYdP"> initial Jacobian column vector with respect to the parameter </param>
		/// <exception cref="UnknownParameterException"> if a parameter is not supported </exception>
		/// <exception cref="DimensionMismatchException"> if the column vector does not match state dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setInitialParameterJacobian(final String pName, final double[] dYdP) throws UnknownParameterException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setInitialParameterJacobian(string pName, double[] dYdP)
		{

			// Check dimensions
			checkDimension(stateDim, dYdP);

			// store the column in a global single dimension array
			int i = stateDim * stateDim;
			foreach (ParameterConfiguration param in selectedParameters)
			{
				if (pName.Equals(param.ParameterName))
				{
					Array.Copy(dYdP, 0, matricesData, i, stateDim);
					if (efode != null)
					{
						efode.setSecondaryState(index, matricesData);
					}
					return;
				}
				i += stateDim;
			}

			throw new UnknownParameterException(pName);

		}

		/// <summary>
		/// Get the current value of the Jacobian matrix with respect to state. </summary>
		/// <param name="dYdY0"> current Jacobian matrix with respect to state. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void getCurrentMainSetJacobian(final double[][] dYdY0)
		public virtual void getCurrentMainSetJacobian(double[][] dYdY0)
		{

			// get current state for this set of equations from the expandable fode
			double[] p = efode.getSecondaryState(index);

			int j = 0;
			for (int i = 0; i < stateDim; i++)
			{
				Array.Copy(p, j, dYdY0[i], 0, stateDim);
				j += stateDim;
			}

		}

		/// <summary>
		/// Get the current value of the Jacobian matrix with respect to one parameter. </summary>
		/// <param name="pName"> name of the parameter for the computed Jacobian matrix </param>
		/// <param name="dYdP"> current Jacobian matrix with respect to the named parameter </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void getCurrentParameterJacobian(String pName, final double[] dYdP)
		public virtual void getCurrentParameterJacobian(string pName, double[] dYdP)
		{

			// get current state for this set of equations from the expandable fode
			double[] p = efode.getSecondaryState(index);

			int i = stateDim * stateDim;
			foreach (ParameterConfiguration param in selectedParameters)
			{
				if (param.ParameterName.Equals(pName))
				{
					Array.Copy(p, i, dYdP, 0, stateDim);
					return;
				}
				i += stateDim;
			}

		}

		/// <summary>
		/// Check array dimensions. </summary>
		/// <param name="expected"> expected dimension </param>
		/// <param name="array"> (may be null if expected is 0) </param>
		/// <exception cref="DimensionMismatchException"> if the array dimension does not match the expected one </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void checkDimension(final int expected, final Object array) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		private void checkDimension(int expected, object array)
		{
			int arrayDimension = (array == null) ? 0 : Array.getLength(array);
			if (arrayDimension != expected)
			{
				throw new DimensionMismatchException(arrayDimension, expected);
			}
		}

		/// <summary>
		/// Local implementation of secondary equations.
		/// <p>
		/// This class is an inner class to ensure proper scheduling of calls
		/// by forcing the use of <seealso cref="JacobianMatrices#registerVariationalEquations(ExpandableStatefulODE)"/>.
		/// </p>
		/// </summary>
		private class JacobiansSecondaryEquations : SecondaryEquations
		{
			private readonly JacobianMatrices outerInstance;

			public JacobiansSecondaryEquations(JacobianMatrices outerInstance)
			{
				this.outerInstance = outerInstance;
			}


			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int Dimension
			{
				get
				{
					return outerInstance.stateDim * (outerInstance.stateDim + outerInstance.paramDim);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDerivatives(final double t, final double[] y, final double[] yDot, final double[] z, final double[] zDot) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public virtual void computeDerivatives(double t, double[] y, double[] yDot, double[] z, double[] zDot)
			{

				// Lazy initialization
				if (outerInstance.dirtyParameter && (outerInstance.paramDim != 0))
				{
					outerInstance.jacobianProviders.Add(new ParameterJacobianWrapper(outerInstance.jode, outerInstance.pode, outerInstance.selectedParameters));
					outerInstance.dirtyParameter = false;
				}

				// variational equations:
				// from d[dy/dt]/dy0 and d[dy/dt]/dp to d[dy/dy0]/dt and d[dy/dp]/dt

				// compute Jacobian matrix with respect to primary state
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] dFdY = new double[outerInstance.stateDim][outerInstance.stateDim];
				double[][] dFdY = RectangularArrays.ReturnRectangularDoubleArray(outerInstance.stateDim, outerInstance.stateDim);
				outerInstance.jode.computeMainStateJacobian(t, y, yDot, dFdY);

				// Dispatch Jacobian matrix in the compound secondary state vector
				for (int i = 0; i < outerInstance.stateDim; ++i)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dFdYi = dFdY[i];
					double[] dFdYi = dFdY[i];
					for (int j = 0; j < outerInstance.stateDim; ++j)
					{
						double s = 0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startIndex = j;
						int startIndex = j;
						int zIndex = startIndex;
						for (int l = 0; l < outerInstance.stateDim; ++l)
						{
							s += dFdYi[l] * z[zIndex];
							zIndex += outerInstance.stateDim;
						}
						zDot[startIndex + i * outerInstance.stateDim] = s;
					}
				}

				if (outerInstance.paramDim != 0)
				{
					// compute Jacobian matrices with respect to parameters
					double[] dFdP = new double[outerInstance.stateDim];
					int startIndex = outerInstance.stateDim * outerInstance.stateDim;
					foreach (ParameterConfiguration param in outerInstance.selectedParameters)
					{
						bool found = false;
						for (int k = 0 ; (!found) && (k < outerInstance.jacobianProviders.Count); ++k)
						{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ParameterJacobianProvider provider = jacobianProviders.get(k);
							ParameterJacobianProvider provider = outerInstance.jacobianProviders[k];
							if (provider.isSupported(param.ParameterName))
							{
								provider.computeParameterJacobian(t, y, yDot, param.ParameterName, dFdP);
								for (int i = 0; i < outerInstance.stateDim; ++i)
								{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] dFdYi = dFdY[i];
									double[] dFdYi = dFdY[i];
									int zIndex = startIndex;
									double s = dFdP[i];
									for (int l = 0; l < outerInstance.stateDim; ++l)
									{
										s += dFdYi[l] * z[zIndex];
										zIndex++;
									}
									zDot[startIndex + i] = s;
								}
								found = true;
							}
						}
						if (!found)
						{
							Arrays.fill(zDot, startIndex, startIndex + outerInstance.stateDim, 0.0);
						}
						startIndex += outerInstance.stateDim;
					}
				}

			}
		}

		/// <summary>
		/// Wrapper class to compute jacobian matrices by finite differences for ODE
		///  which do not compute them by themselves.
		/// </summary>
		private class MainStateJacobianWrapper : MainStateJacobianProvider
		{

			/// <summary>
			/// Raw ODE without jacobians computation skill to be wrapped into a MainStateJacobianProvider. </summary>
			internal readonly FirstOrderDifferentialEquations ode;

			/// <summary>
			/// Steps for finite difference computation of the jacobian df/dy w.r.t. state. </summary>
			internal readonly double[] hY;

			/// <summary>
			/// Wrap a <seealso cref="FirstOrderDifferentialEquations"/> into a <seealso cref="MainStateJacobianProvider"/>. </summary>
			/// <param name="ode"> original ODE problem, without jacobians computation skill </param>
			/// <param name="hY"> step sizes to compute the jacobian df/dy </param>
			/// <exception cref="DimensionMismatchException"> if there is a dimension mismatch between
			/// the steps array {@code hY} and the equation dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public MainStateJacobianWrapper(final FirstOrderDifferentialEquations ode, final double[] hY) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
			public MainStateJacobianWrapper(FirstOrderDifferentialEquations ode, double[] hY)
			{
				this.ode = ode;
				this.hY = hY.clone();
				if (hY.Length != ode.Dimension)
				{
					throw new DimensionMismatchException(ode.Dimension, hY.Length);
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
			public virtual int Dimension
			{
				get
				{
					return ode.Dimension;
				}
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDerivatives(double t, double[] y, double[] yDot) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException
			public virtual void computeDerivatives(double t, double[] y, double[] yDot)
			{
				ode.computeDerivatives(t, y, yDot);
			}

			/// <summary>
			/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeMainStateJacobian(double t, double[] y, double[] yDot, double[][] dFdY) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException
			public virtual void computeMainStateJacobian(double t, double[] y, double[] yDot, double[][] dFdY)
			{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = ode.getDimension();
				int n = ode.Dimension;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmpDot = new double[n];
				double[] tmpDot = new double[n];

				for (int j = 0; j < n; ++j)
				{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double savedYj = y[j];
					double savedYj = y[j];
					y[j] += hY[j];
					ode.computeDerivatives(t, y, tmpDot);
					for (int i = 0; i < n; ++i)
					{
						dFdY[i][j] = (tmpDot[i] - yDot[i]) / hY[j];
					}
					y[j] = savedYj;
				}
			}

		}

		/// <summary>
		/// Special exception for equations mismatch.
		/// @since 3.1
		/// </summary>
		public class MismatchedEquations : MathIllegalArgumentException
		{

			/// <summary>
			/// Serializable UID. </summary>
			internal const long serialVersionUID = 20120902L;

			/// <summary>
			/// Simple constructor. </summary>
			public MismatchedEquations() : base(LocalizedFormats.UNMATCHED_ODE_IN_EXPANDED_SET)
			{
			}

		}

	}


}