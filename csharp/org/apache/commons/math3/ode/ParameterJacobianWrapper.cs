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
namespace org.apache.commons.math3.ode
{


	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using MaxCountExceededException = org.apache.commons.math3.exception.MaxCountExceededException;

	/// <summary>
	/// Wrapper class to compute Jacobian matrices by finite differences for ODE
	///  which do not compute them by themselves.
	/// 
	/// @version $Id: ParameterJacobianWrapper.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	internal class ParameterJacobianWrapper : ParameterJacobianProvider
	{

		/// <summary>
		/// Main ODE set. </summary>
		private readonly FirstOrderDifferentialEquations fode;

		/// <summary>
		/// Raw ODE without Jacobian computation skill to be wrapped into a ParameterJacobianProvider. </summary>
		private readonly ParameterizedODE pode;

		/// <summary>
		/// Steps for finite difference computation of the Jacobian df/dp w.r.t. parameters. </summary>
		private readonly IDictionary<string, double?> hParam;

		/// <summary>
		/// Wrap a <seealso cref="ParameterizedODE"/> into a <seealso cref="ParameterJacobianProvider"/>. </summary>
		/// <param name="fode"> main first order differential equations set </param>
		/// <param name="pode"> secondary problem, without parameter Jacobian computation skill </param>
		/// <param name="paramsAndSteps"> parameters and steps to compute the Jacobians df/dp </param>
		/// <seealso cref= JacobianMatrices#setParameterStep(String, double) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ParameterJacobianWrapper(final FirstOrderDifferentialEquations fode, final ParameterizedODE pode, final ParameterConfiguration[] paramsAndSteps)
		public ParameterJacobianWrapper(FirstOrderDifferentialEquations fode, ParameterizedODE pode, ParameterConfiguration[] paramsAndSteps)
		{
			this.fode = fode;
			this.pode = pode;
			this.hParam = new Dictionary<string, double?>();

			// set up parameters for jacobian computation
			foreach (ParameterConfiguration param in paramsAndSteps)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = param.getParameterName();
				string name = param.ParameterName;
				if (pode.isSupported(name))
				{
					hParam[name] = param.HP;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual ICollection<string> ParametersNames
		{
			get
			{
				return pode.ParametersNames;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual bool isSupported(string name)
		{
			return pode.isSupported(name);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void computeParameterJacobian(double t, double[] y, double[] yDot, String paramName, double[] dFdP) throws org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.exception.MaxCountExceededException
		public virtual void computeParameterJacobian(double t, double[] y, double[] yDot, string paramName, double[] dFdP)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = fode.getDimension();
			int n = fode.Dimension;
			if (pode.isSupported(paramName))
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] tmpDot = new double[n];
				double[] tmpDot = new double[n];

				// compute the jacobian df/dp w.r.t. parameter
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double p = pode.getParameter(paramName);
				double p = pode.getParameter(paramName);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double hP = hParam.get(paramName);
				double hP = hParam[paramName];
				pode.setParameter(paramName, p + hP);
				fode.computeDerivatives(t, y, tmpDot);
				for (int i = 0; i < n; ++i)
				{
					dFdP[i] = (tmpDot[i] - yDot[i]) / hP;
				}
				pode.setParameter(paramName, p);
			}
			else
			{
				Arrays.fill(dFdP, 0, n, 0.0);
			}

		}

	}

}