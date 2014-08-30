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
namespace org.apache.commons.math3.ode
{

	/// <summary>
	/// Simple container pairing a parameter name with a step in order to compute
	///  the associated Jacobian matrix by finite difference.
	/// 
	/// @version $Id: ParameterConfiguration.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0
	/// </summary>
	[Serializable]
	internal class ParameterConfiguration
	{

		/// <summary>
		/// Serializable UID. </summary>
		private const long serialVersionUID = 2247518849090889379L;

		/// <summary>
		/// Parameter name. </summary>
		private string parameterName;

		/// <summary>
		/// Parameter step for finite difference computation. </summary>
		private double hP;

		/// <summary>
		/// Parameter name and step pair constructor. </summary>
		/// <param name="parameterName"> parameter name </param>
		/// <param name="hP"> parameter step </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ParameterConfiguration(final String parameterName, final double hP)
		public ParameterConfiguration(string parameterName, double hP)
		{
			this.parameterName = parameterName;
			this.hP = hP;
		}

		/// <summary>
		/// Get parameter name. </summary>
		/// <returns> parameterName parameter name </returns>
		public virtual string ParameterName
		{
			get
			{
				return parameterName;
			}
		}

		/// <summary>
		/// Get parameter step. </summary>
		/// <returns> hP parameter step </returns>
		public virtual double HP
		{
			get
			{
				return hP;
			}
			set
			{
				this.hP = value;
			}
		}

		/// <summary>
		/// Set parameter step. </summary>
		/// <param name="hParam"> parameter step </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setHP(final double hParam)

	}

}