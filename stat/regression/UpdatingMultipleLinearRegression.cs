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
namespace mathlib.stat.regression
{

	using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
	using NoDataException = mathlib.exception.NoDataException;

	/// <summary>
	/// An interface for regression models allowing for dynamic updating of the data.
	/// That is, the entire data set need not be loaded into memory. As observations
	/// become available, they can be added to the regression  model and an updated
	/// estimate regression statistics can be calculated.
	/// 
	/// @version $Id: UpdatingMultipleLinearRegression.java 1392342 2012-10-01 14:08:52Z psteitz $
	/// @since 3.0
	/// </summary>
	public interface UpdatingMultipleLinearRegression
	{

		/// <summary>
		/// Returns true if a constant has been included false otherwise.
		/// </summary>
		/// <returns> true if constant exists, false otherwise </returns>
		bool hasIntercept();

		/// <summary>
		/// Returns the number of observations added to the regression model.
		/// </summary>
		/// <returns> Number of observations </returns>
		long N {get;}

		/// <summary>
		/// Adds one observation to the regression model.
		/// </summary>
		/// <param name="x"> the independent variables which form the design matrix </param>
		/// <param name="y"> the dependent or response variable </param>
		/// <exception cref="ModelSpecificationException"> if the length of {@code x} does not equal
		/// the number of independent variables in the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addObservation(double[] x, double y) throws ModelSpecificationException;
		void addObservation(double[] x, double y);

		/// <summary>
		/// Adds a series of observations to the regression model. The lengths of
		/// x and y must be the same and x must be rectangular.
		/// </summary>
		/// <param name="x"> a series of observations on the independent variables </param>
		/// <param name="y"> a series of observations on the dependent variable
		/// The length of x and y must be the same </param>
		/// <exception cref="ModelSpecificationException"> if {@code x} is not rectangular, does not match
		/// the length of {@code y} or does not contain sufficient data to estimate the model </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void addObservations(double[][] x, double[] y) throws ModelSpecificationException;
		void addObservations(double[][] x, double[] y);

		/// <summary>
		/// Clears internal buffers and resets the regression model. This means all
		/// data and derived values are initialized
		/// </summary>
		void clear();


		/// <summary>
		/// Performs a regression on data present in buffers and outputs a RegressionResults object </summary>
		/// <returns> RegressionResults acts as a container of regression output </returns>
		/// <exception cref="ModelSpecificationException"> if the model is not correctly specified </exception>
		/// <exception cref="NoDataException"> if there is not sufficient data in the model to
		/// estimate the regression parameters </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RegressionResults regress() throws ModelSpecificationException, mathlib.exception.NoDataException;
		RegressionResults regress();

		/// <summary>
		/// Performs a regression on data present in buffers including only regressors
		/// indexed in variablesToInclude and outputs a RegressionResults object </summary>
		/// <param name="variablesToInclude"> an array of indices of regressors to include </param>
		/// <returns> RegressionResults acts as a container of regression output </returns>
		/// <exception cref="ModelSpecificationException"> if the model is not correctly specified </exception>
		/// <exception cref="MathIllegalArgumentException"> if the variablesToInclude array is null or zero length </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: RegressionResults regress(int[] variablesToInclude) throws ModelSpecificationException, mathlib.exception.MathIllegalArgumentException;
		RegressionResults regress(int[] variablesToInclude);
	}

}