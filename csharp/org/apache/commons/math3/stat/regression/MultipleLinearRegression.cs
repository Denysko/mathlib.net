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
namespace org.apache.commons.math3.stat.regression
{

	/// <summary>
	/// The multiple linear regression can be represented in matrix-notation.
	/// <pre>
	///  y=X*b+u
	/// </pre>
	/// where y is an <code>n-vector</code> <b>regressand</b>, X is a <code>[n,k]</code> matrix whose <code>k</code> columns are called
	/// <b>regressors</b>, b is <code>k-vector</code> of <b>regression parameters</b> and <code>u</code> is an <code>n-vector</code>
	/// of <b>error terms</b> or <b>residuals</b>.
	/// 
	/// The notation is quite standard in literature,
	/// cf eg <a href="http://www.econ.queensu.ca/ETM">Davidson and MacKinnon, Econometrics Theory and Methods, 2004</a>.
	/// @version $Id: MultipleLinearRegression.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	public interface MultipleLinearRegression
	{

		/// <summary>
		/// Estimates the regression parameters b.
		/// </summary>
		/// <returns> The [k,1] array representing b </returns>
		double[] estimateRegressionParameters();

		/// <summary>
		/// Estimates the variance of the regression parameters, ie Var(b).
		/// </summary>
		/// <returns> The [k,k] array representing the variance of b </returns>
		double[][] estimateRegressionParametersVariance();

		/// <summary>
		/// Estimates the residuals, ie u = y - X*b.
		/// </summary>
		/// <returns> The [n,1] array representing the residuals </returns>
		double[] estimateResiduals();

		/// <summary>
		/// Returns the variance of the regressand, ie Var(y).
		/// </summary>
		/// <returns> The double representing the variance of y </returns>
		double estimateRegressandVariance();

		/// <summary>
		/// Returns the standard errors of the regression parameters.
		/// </summary>
		/// <returns> standard errors of estimated regression parameters </returns>
		 double[] estimateRegressionParametersStandardErrors();

	}

}