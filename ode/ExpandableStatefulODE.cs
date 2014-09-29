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
	using MaxCountExceededException = mathlib.exception.MaxCountExceededException;


	/// <summary>
	/// This class represents a combined set of first order differential equations,
	/// with at least a primary set of equations expandable by some sets of secondary
	/// equations.
	/// <p>
	/// One typical use case is the computation of the Jacobian matrix for some ODE.
	/// In this case, the primary set of equations corresponds to the raw ODE, and we
	/// add to this set another bunch of secondary equations which represent the Jacobian
	/// matrix of the primary set.
	/// </p>
	/// <p>
	/// We want the integrator to use <em>only</em> the primary set to estimate the
	/// errors and hence the step sizes. It should <em>not</em> use the secondary
	/// equations in this computation. The <seealso cref="AbstractIntegrator integrator"/> will
	/// be able to know where the primary set ends and so where the secondary sets begin.
	/// </p>
	/// </summary>
	/// <seealso cref= FirstOrderDifferentialEquations </seealso>
	/// <seealso cref= JacobianMatrices
	/// 
	/// @version $Id: ExpandableStatefulODE.java 1463680 2013-04-02 19:02:55Z luc $
	/// @since 3.0 </seealso>

	public class ExpandableStatefulODE
	{

		/// <summary>
		/// Primary differential equation. </summary>
		private readonly FirstOrderDifferentialEquations primary;

		/// <summary>
		/// Mapper for primary equation. </summary>
		private readonly EquationsMapper primaryMapper;

		/// <summary>
		/// Time. </summary>
		private double time;

		/// <summary>
		/// State. </summary>
		private readonly double[] primaryState;

		/// <summary>
		/// State derivative. </summary>
		private readonly double[] primaryStateDot;

		/// <summary>
		/// Components of the expandable ODE. </summary>
		private IList<SecondaryComponent> components;

		/// <summary>
		/// Build an expandable set from its primary ODE set. </summary>
		/// <param name="primary"> the primary set of differential equations to be integrated. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ExpandableStatefulODE(final FirstOrderDifferentialEquations primary)
		public ExpandableStatefulODE(FirstOrderDifferentialEquations primary)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = primary.getDimension();
			int n = primary.Dimension;
			this.primary = primary;
			this.primaryMapper = new EquationsMapper(0, n);
			this.time = double.NaN;
			this.primaryState = new double[n];
			this.primaryStateDot = new double[n];
			this.components = new List<ExpandableStatefulODE.SecondaryComponent>();
		}

		/// <summary>
		/// Get the primary set of differential equations. </summary>
		/// <returns> primary set of differential equations </returns>
		public virtual FirstOrderDifferentialEquations Primary
		{
			get
			{
				return primary;
			}
		}

		/// <summary>
		/// Return the dimension of the complete set of equations.
		/// <p>
		/// The complete set of equations correspond to the primary set plus all secondary sets.
		/// </p> </summary>
		/// <returns> dimension of the complete set of equations </returns>
		public virtual int TotalDimension
		{
			get
			{
				if (components.Count == 0)
				{
					// there are no secondary equations, the complete set is limited to the primary set
					return primaryMapper.Dimension;
				}
				else
				{
					// there are secondary equations, the complete set ends after the last set
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final EquationsMapper lastMapper = components.get(components.size() - 1).mapper;
					EquationsMapper lastMapper = components[components.Count - 1].mapper;
					return lastMapper.FirstIndex + lastMapper.Dimension;
				}
			}
		}

		/// <summary>
		/// Get the current time derivative of the complete state vector. </summary>
		/// <param name="t"> current value of the independent <I>time</I> variable </param>
		/// <param name="y"> array containing the current value of the complete state vector </param>
		/// <param name="yDot"> placeholder array where to put the time derivative of the complete state vector </param>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDerivatives(final double t, final double[] y, final double[] yDot) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void computeDerivatives(double t, double[] y, double[] yDot)
		{

			// compute derivatives of the primary equations
			primaryMapper.extractEquationData(y, primaryState);
			primary.computeDerivatives(t, primaryState, primaryStateDot);

			// Add contribution for secondary equations
			foreach (SecondaryComponent component in components)
			{
				component.mapper.extractEquationData(y, component.state);
				component.equation.computeDerivatives(t, primaryState, primaryStateDot, component.state, component.stateDot);
				component.mapper.insertEquationData(component.stateDot, yDot);
			}

			primaryMapper.insertEquationData(primaryStateDot, yDot);

		}

		/// <summary>
		/// Add a set of secondary equations to be integrated along with the primary set. </summary>
		/// <param name="secondary"> secondary equations set </param>
		/// <returns> index of the secondary equation in the expanded state </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public int addSecondaryEquations(final SecondaryEquations secondary)
		public virtual int addSecondaryEquations(SecondaryEquations secondary)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int firstIndex;
			int firstIndex;
			if (components.Count == 0)
			{
				// lazy creation of the components list
				components = new List<ExpandableStatefulODE.SecondaryComponent>();
				firstIndex = primary.Dimension;
			}
			else
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final SecondaryComponent last = components.get(components.size() - 1);
				SecondaryComponent last = components[components.Count - 1];
				firstIndex = last.mapper.FirstIndex + last.mapper.Dimension;
			}

			components.Add(new SecondaryComponent(secondary, firstIndex));

			return components.Count - 1;

		}

		/// <summary>
		/// Get an equations mapper for the primary equations set. </summary>
		/// <returns> mapper for the primary set </returns>
		/// <seealso cref= #getSecondaryMappers() </seealso>
		public virtual EquationsMapper PrimaryMapper
		{
			get
			{
				return primaryMapper;
			}
		}

		/// <summary>
		/// Get the equations mappers for the secondary equations sets. </summary>
		/// <returns> equations mappers for the secondary equations sets </returns>
		/// <seealso cref= #getPrimaryMapper() </seealso>
		public virtual EquationsMapper[] SecondaryMappers
		{
			get
			{
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final EquationsMapper[] mappers = new EquationsMapper[components.size()];
				EquationsMapper[] mappers = new EquationsMapper[components.Count];
				for (int i = 0; i < mappers.Length; ++i)
				{
					mappers[i] = components[i].mapper;
				}
				return mappers;
			}
		}

		/// <summary>
		/// Set current time. </summary>
		/// <param name="time"> current time </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setTime(final double time)
		public virtual double Time
		{
			set
			{
				this.time = value;
			}
			get
			{
				return time;
			}
		}


		/// <summary>
		/// Set primary part of the current state. </summary>
		/// <param name="primaryState"> primary part of the current state </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of the array does not
		/// match the primary set </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setPrimaryState(final double[] primaryState) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] PrimaryState
		{
			set
			{
    
				// safety checks
				if (value.Length != this.primaryState.Length)
				{
					throw new DimensionMismatchException(value.Length, this.primaryState.Length);
				}
    
				// set the data
				Array.Copy(value, 0, this.primaryState, 0, value.Length);
    
			}
			get
			{
				return primaryState.clone();
			}
		}


		/// <summary>
		/// Get primary part of the current state derivative. </summary>
		/// <returns> primary part of the current state derivative </returns>
		public virtual double[] PrimaryStateDot
		{
			get
			{
				return primaryStateDot.clone();
			}
		}

		/// <summary>
		/// Set secondary part of the current state. </summary>
		/// <param name="index"> index of the part to set as returned by {@link
		/// #addSecondaryEquations(SecondaryEquations)} </param>
		/// <param name="secondaryState"> secondary part of the current state </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of the partial state does not
		/// match the selected equations set dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setSecondaryState(final int index, final double[] secondaryState) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void setSecondaryState(int index, double[] secondaryState)
		{

			// get either the secondary state
			double[] localArray = components[index].state;

			// safety checks
			if (secondaryState.Length != localArray.Length)
			{
				throw new DimensionMismatchException(secondaryState.Length, localArray.Length);
			}

			// set the data
			Array.Copy(secondaryState, 0, localArray, 0, secondaryState.Length);

		}

		/// <summary>
		/// Get secondary part of the current state. </summary>
		/// <param name="index"> index of the part to set as returned by {@link
		/// #addSecondaryEquations(SecondaryEquations)} </param>
		/// <returns> secondary part of the current state </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] getSecondaryState(final int index)
		public virtual double[] getSecondaryState(int index)
		{
			return components[index].state.clone();
		}

		/// <summary>
		/// Get secondary part of the current state derivative. </summary>
		/// <param name="index"> index of the part to set as returned by {@link
		/// #addSecondaryEquations(SecondaryEquations)} </param>
		/// <returns> secondary part of the current state derivative </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double[] getSecondaryStateDot(final int index)
		public virtual double[] getSecondaryStateDot(int index)
		{
			return components[index].stateDot.clone();
		}

		/// <summary>
		/// Set the complete current state. </summary>
		/// <param name="completeState"> complete current state to copy data from </param>
		/// <exception cref="DimensionMismatchException"> if the dimension of the complete state does not
		/// match the complete equations sets dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setCompleteState(final double[] completeState) throws mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual double[] CompleteState
		{
			set
			{
    
				// safety checks
				if (value.Length != TotalDimension)
				{
					throw new DimensionMismatchException(value.Length, TotalDimension);
				}
    
				// set the data
				primaryMapper.extractEquationData(value, primaryState);
				foreach (SecondaryComponent component in components)
				{
					component.mapper.extractEquationData(value, component.state);
				}
    
			}
			get
			{
    
				// allocate complete array
				double[] completeState = new double[TotalDimension];
    
				// set the data
				primaryMapper.insertEquationData(primaryState, completeState);
				foreach (SecondaryComponent component in components)
				{
					component.mapper.insertEquationData(component.state, completeState);
				}
    
				return completeState;
    
			}
		}

		/// <summary>
		/// Get the complete current state. </summary>
		/// <returns> complete current state </returns>
		/// <exception cref="DimensionMismatchException"> if the dimension of the complete state does not
		/// match the complete equations sets dimension </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double[] getCompleteState() throws mathlib.exception.DimensionMismatchException

		/// <summary>
		/// Components of the compound stateful ODE. </summary>
		private class SecondaryComponent
		{

			/// <summary>
			/// Secondary differential equation. </summary>
			internal readonly SecondaryEquations equation;

			/// <summary>
			/// Mapper between local and complete arrays. </summary>
			internal readonly EquationsMapper mapper;

			/// <summary>
			/// State. </summary>
			internal readonly double[] state;

			/// <summary>
			/// State derivative. </summary>
			internal readonly double[] stateDot;

			/// <summary>
			/// Simple constructor. </summary>
			/// <param name="equation"> secondary differential equation </param>
			/// <param name="firstIndex"> index to use for the first element in the complete arrays </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public SecondaryComponent(final SecondaryEquations equation, final int firstIndex)
			public SecondaryComponent(SecondaryEquations equation, int firstIndex)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = equation.getDimension();
				int n = equation.Dimension;
				this.equation = equation;
				mapper = new EquationsMapper(firstIndex, n);
				state = new double[n];
				stateDot = new double[n];
			}

		}

	}

}