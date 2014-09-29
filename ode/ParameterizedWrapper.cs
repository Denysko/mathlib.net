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
	/// Wrapper class enabling <seealso cref="FirstOrderDifferentialEquations basic simple"/>
	///  ODE instances to be used when processing <seealso cref="JacobianMatrices"/>.
	/// 
	/// @version $Id: ParameterizedWrapper.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	internal class ParameterizedWrapper : ParameterizedODE
	{

		/// <summary>
		/// Basic FODE without parameter. </summary>
		private readonly FirstOrderDifferentialEquations fode;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="ode"> original first order differential equations </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ParameterizedWrapper(final FirstOrderDifferentialEquations ode)
		public ParameterizedWrapper(FirstOrderDifferentialEquations ode)
		{
			this.fode = ode;
		}

		/// <summary>
		/// Get the dimension of the underlying FODE. </summary>
		/// <returns> dimension of the underlying FODE </returns>
		public virtual int Dimension
		{
			get
			{
				return fode.Dimension;
			}
		}

		/// <summary>
		/// Get the current time derivative of the state vector of the underlying FODE. </summary>
		/// <param name="t"> current value of the independent <I>time</I> variable </param>
		/// <param name="y"> array containing the current value of the state vector </param>
		/// <param name="yDot"> placeholder array where to put the time derivative of the state vector </param>
		/// <exception cref="MaxCountExceededException"> if the number of functions evaluations is exceeded </exception>
		/// <exception cref="DimensionMismatchException"> if arrays dimensions do not match equations settings </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeDerivatives(double t, double[] y, double[] yDot) throws mathlib.exception.MaxCountExceededException, mathlib.exception.DimensionMismatchException
		public virtual void computeDerivatives(double t, double[] y, double[] yDot)
		{
			fode.computeDerivatives(t, y, yDot);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ICollection<string> ParametersNames
		{
			get
			{
				return new List<string>();
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool isSupported(string name)
		{
			return false;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getParameter(String name) throws UnknownParameterException
		public virtual double getParameter(string name)
		{
			if (!isSupported(name))
			{
				throw new UnknownParameterException(name);
			}
			return double.NaN;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void setParameter(string name, double value)
		{
		}

	}

}