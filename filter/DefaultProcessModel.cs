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
namespace mathlib.filter
{

	using DimensionMismatchException = mathlib.exception.DimensionMismatchException;
	using NoDataException = mathlib.exception.NoDataException;
	using NullArgumentException = mathlib.exception.NullArgumentException;
	using Array2DRowRealMatrix = mathlib.linear.Array2DRowRealMatrix;
	using ArrayRealVector = mathlib.linear.ArrayRealVector;
	using RealMatrix = mathlib.linear.RealMatrix;
	using RealVector = mathlib.linear.RealVector;

	/// <summary>
	/// Default implementation of a <seealso cref="ProcessModel"/> for the use with a <seealso cref="KalmanFilter"/>.
	/// 
	/// @since 3.0
	/// @version $Id: DefaultProcessModel.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class DefaultProcessModel : ProcessModel
	{
		/// <summary>
		/// The state transition matrix, used to advance the internal state estimation each time-step.
		/// </summary>
		private RealMatrix stateTransitionMatrix;

		/// <summary>
		/// The control matrix, used to integrate a control input into the state estimation.
		/// </summary>
		private RealMatrix controlMatrix;

		/// <summary>
		/// The process noise covariance matrix. </summary>
		private RealMatrix processNoiseCovMatrix;

		/// <summary>
		/// The initial state estimation of the observed process. </summary>
		private RealVector initialStateEstimateVector;

		/// <summary>
		/// The initial error covariance matrix of the observed process. </summary>
		private RealMatrix initialErrorCovMatrix;

		/// <summary>
		/// Create a new <seealso cref="ProcessModel"/>, taking double arrays as input parameters.
		/// </summary>
		/// <param name="stateTransition">
		///            the state transition matrix </param>
		/// <param name="control">
		///            the control matrix </param>
		/// <param name="processNoise">
		///            the process noise matrix </param>
		/// <param name="initialStateEstimate">
		///            the initial state estimate vector </param>
		/// <param name="initialErrorCovariance">
		///            the initial error covariance matrix </param>
		/// <exception cref="NullArgumentException">
		///             if any of the input arrays is {@code null} </exception>
		/// <exception cref="NoDataException">
		///             if any row / column dimension of the input matrices is zero </exception>
		/// <exception cref="DimensionMismatchException">
		///             if any of the input matrices is non-rectangular </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DefaultProcessModel(final double[][] stateTransition, final double[][] control, final double[][] processNoise, final double[] initialStateEstimate, final double[][] initialErrorCovariance) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DefaultProcessModel(double[][] stateTransition, double[][] control, double[][] processNoise, double[] initialStateEstimate, double[][] initialErrorCovariance) : this(new Array2DRowRealMatrix(stateTransition), new Array2DRowRealMatrix(control), new Array2DRowRealMatrix(processNoise), new ArrayRealVector(initialStateEstimate), new Array2DRowRealMatrix(initialErrorCovariance))
		{

		}

		/// <summary>
		/// Create a new <seealso cref="ProcessModel"/>, taking double arrays as input parameters.
		/// <p>
		/// The initial state estimate and error covariance are omitted and will be initialized by the
		/// <seealso cref="KalmanFilter"/> to default values.
		/// </summary>
		/// <param name="stateTransition">
		///            the state transition matrix </param>
		/// <param name="control">
		///            the control matrix </param>
		/// <param name="processNoise">
		///            the process noise matrix </param>
		/// <exception cref="NullArgumentException">
		///             if any of the input arrays is {@code null} </exception>
		/// <exception cref="NoDataException">
		///             if any row / column dimension of the input matrices is zero </exception>
		/// <exception cref="DimensionMismatchException">
		///             if any of the input matrices is non-rectangular </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DefaultProcessModel(final double[][] stateTransition, final double[][] control, final double[][] processNoise) throws mathlib.exception.NullArgumentException, mathlib.exception.NoDataException, mathlib.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DefaultProcessModel(double[][] stateTransition, double[][] control, double[][] processNoise) : this(new Array2DRowRealMatrix(stateTransition), new Array2DRowRealMatrix(control), new Array2DRowRealMatrix(processNoise), null, null)
		{

		}

		/// <summary>
		/// Create a new <seealso cref="ProcessModel"/>, taking double arrays as input parameters.
		/// </summary>
		/// <param name="stateTransition">
		///            the state transition matrix </param>
		/// <param name="control">
		///            the control matrix </param>
		/// <param name="processNoise">
		///            the process noise matrix </param>
		/// <param name="initialStateEstimate">
		///            the initial state estimate vector </param>
		/// <param name="initialErrorCovariance">
		///            the initial error covariance matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DefaultProcessModel(final mathlib.linear.RealMatrix stateTransition, final mathlib.linear.RealMatrix control, final mathlib.linear.RealMatrix processNoise, final mathlib.linear.RealVector initialStateEstimate, final mathlib.linear.RealMatrix initialErrorCovariance)
		public DefaultProcessModel(RealMatrix stateTransition, RealMatrix control, RealMatrix processNoise, RealVector initialStateEstimate, RealMatrix initialErrorCovariance)
		{
			this.stateTransitionMatrix = stateTransition;
			this.controlMatrix = control;
			this.processNoiseCovMatrix = processNoise;
			this.initialStateEstimateVector = initialStateEstimate;
			this.initialErrorCovMatrix = initialErrorCovariance;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix StateTransitionMatrix
		{
			get
			{
				return stateTransitionMatrix;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix ControlMatrix
		{
			get
			{
				return controlMatrix;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix ProcessNoise
		{
			get
			{
				return processNoiseCovMatrix;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealVector InitialStateEstimate
		{
			get
			{
				return initialStateEstimateVector;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix InitialErrorCovariance
		{
			get
			{
				return initialErrorCovMatrix;
			}
		}
	}

}