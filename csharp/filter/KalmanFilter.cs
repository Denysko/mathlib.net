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
namespace org.apache.commons.math3.filter
{

	using DimensionMismatchException = org.apache.commons.math3.exception.DimensionMismatchException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using ArrayRealVector = org.apache.commons.math3.linear.ArrayRealVector;
	using MatrixDimensionMismatchException = org.apache.commons.math3.linear.MatrixDimensionMismatchException;
	using MatrixUtils = org.apache.commons.math3.linear.MatrixUtils;
	using NonSquareMatrixException = org.apache.commons.math3.linear.NonSquareMatrixException;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;
	using SingularMatrixException = org.apache.commons.math3.linear.SingularMatrixException;
	using MathUtils = org.apache.commons.math3.util.MathUtils;

	/// <summary>
	/// Implementation of a Kalman filter to estimate the state <i>x<sub>k</sub></i>
	/// of a discrete-time controlled process that is governed by the linear
	/// stochastic difference equation:
	/// 
	/// <pre>
	/// <i>x<sub>k</sub></i> = <b>A</b><i>x<sub>k-1</sub></i> + <b>B</b><i>u<sub>k-1</sub></i> + <i>w<sub>k-1</sub></i>
	/// </pre>
	/// 
	/// with a measurement <i>x<sub>k</sub></i> that is
	/// 
	/// <pre>
	/// <i>z<sub>k</sub></i> = <b>H</b><i>x<sub>k</sub></i> + <i>v<sub>k</sub></i>.
	/// </pre>
	/// 
	/// <p>
	/// The random variables <i>w<sub>k</sub></i> and <i>v<sub>k</sub></i> represent
	/// the process and measurement noise and are assumed to be independent of each
	/// other and distributed with normal probability (white noise).
	/// <p>
	/// The Kalman filter cycle involves the following steps:
	/// <ol>
	/// <li>predict: project the current state estimate ahead in time</li>
	/// <li>correct: adjust the projected estimate by an actual measurement</li>
	/// </ol>
	/// <p>
	/// The Kalman filter is initialized with a <seealso cref="ProcessModel"/> and a
	/// <seealso cref="MeasurementModel"/>, which contain the corresponding transformation and
	/// noise covariance matrices. The parameter names used in the respective models
	/// correspond to the following names commonly used in the mathematical
	/// literature:
	/// <ul>
	/// <li>A - state transition matrix</li>
	/// <li>B - control input matrix</li>
	/// <li>H - measurement matrix</li>
	/// <li>Q - process noise covariance matrix</li>
	/// <li>R - measurement noise covariance matrix</li>
	/// <li>P - error covariance matrix</li>
	/// </ul>
	/// </summary>
	/// <seealso cref= <a href="http://www.cs.unc.edu/~welch/kalman/">Kalman filter
	///      resources</a> </seealso>
	/// <seealso cref= <a href="http://www.cs.unc.edu/~welch/media/pdf/kalman_intro.pdf">An
	///      introduction to the Kalman filter by Greg Welch and Gary Bishop</a> </seealso>
	/// <seealso cref= <a href="http://academic.csuohio.edu/simond/courses/eec644/kalman.pdf">
	///      Kalman filter example by Dan Simon</a> </seealso>
	/// <seealso cref= ProcessModel </seealso>
	/// <seealso cref= MeasurementModel
	/// @since 3.0
	/// @version $Id: KalmanFilter.java 1539676 2013-11-07 15:15:18Z tn $ </seealso>
	public class KalmanFilter
	{
		/// <summary>
		/// The process model used by this filter instance. </summary>
		private readonly ProcessModel processModel;
		/// <summary>
		/// The measurement model used by this filter instance. </summary>
		private readonly MeasurementModel measurementModel;
		/// <summary>
		/// The transition matrix, equivalent to A. </summary>
		private RealMatrix transitionMatrix;
		/// <summary>
		/// The transposed transition matrix. </summary>
		private RealMatrix transitionMatrixT;
		/// <summary>
		/// The control matrix, equivalent to B. </summary>
		private RealMatrix controlMatrix;
		/// <summary>
		/// The measurement matrix, equivalent to H. </summary>
		private RealMatrix measurementMatrix;
		/// <summary>
		/// The transposed measurement matrix. </summary>
		private RealMatrix measurementMatrixT;
		/// <summary>
		/// The internal state estimation vector, equivalent to x hat. </summary>
		private RealVector stateEstimation;
		/// <summary>
		/// The error covariance matrix, equivalent to P. </summary>
		private RealMatrix errorCovariance;

		/// <summary>
		/// Creates a new Kalman filter with the given process and measurement models.
		/// </summary>
		/// <param name="process">
		///            the model defining the underlying process dynamics </param>
		/// <param name="measurement">
		///            the model defining the given measurement characteristics </param>
		/// <exception cref="NullArgumentException">
		///             if any of the given inputs is null (except for the control matrix) </exception>
		/// <exception cref="NonSquareMatrixException">
		///             if the transition matrix is non square </exception>
		/// <exception cref="DimensionMismatchException">
		///             if the column dimension of the transition matrix does not match the dimension of the
		///             initial state estimation vector </exception>
		/// <exception cref="MatrixDimensionMismatchException">
		///             if the matrix dimensions do not fit together </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KalmanFilter(final ProcessModel process, final MeasurementModel measurement) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.linear.NonSquareMatrixException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.linear.MatrixDimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public KalmanFilter(ProcessModel process, MeasurementModel measurement)
		{

			MathUtils.checkNotNull(process);
			MathUtils.checkNotNull(measurement);

			this.processModel = process;
			this.measurementModel = measurement;

			transitionMatrix = processModel.StateTransitionMatrix;
			MathUtils.checkNotNull(transitionMatrix);
			transitionMatrixT = transitionMatrix.transpose();

			// create an empty matrix if no control matrix was given
			if (processModel.ControlMatrix == null)
			{
				controlMatrix = new Array2DRowRealMatrix();
			}
			else
			{
				controlMatrix = processModel.ControlMatrix;
			}

			measurementMatrix = measurementModel.MeasurementMatrix;
			MathUtils.checkNotNull(measurementMatrix);
			measurementMatrixT = measurementMatrix.transpose();

			// check that the process and measurement noise matrices are not null
			// they will be directly accessed from the model as they may change
			// over time
			RealMatrix processNoise = processModel.ProcessNoise;
			MathUtils.checkNotNull(processNoise);
			RealMatrix measNoise = measurementModel.MeasurementNoise;
			MathUtils.checkNotNull(measNoise);

			// set the initial state estimate to a zero vector if it is not
			// available from the process model
			if (processModel.InitialStateEstimate == null)
			{
				stateEstimation = new ArrayRealVector(transitionMatrix.ColumnDimension);
			}
			else
			{
				stateEstimation = processModel.InitialStateEstimate;
			}

			if (transitionMatrix.ColumnDimension != stateEstimation.Dimension)
			{
				throw new DimensionMismatchException(transitionMatrix.ColumnDimension, stateEstimation.Dimension);
			}

			// initialize the error covariance to the process noise if it is not
			// available from the process model
			if (processModel.InitialErrorCovariance == null)
			{
				errorCovariance = processNoise.copy();
			}
			else
			{
				errorCovariance = processModel.InitialErrorCovariance;
			}

			// sanity checks, the control matrix B may be null

			// A must be a square matrix
			if (!transitionMatrix.Square)
			{
				throw new NonSquareMatrixException(transitionMatrix.RowDimension, transitionMatrix.ColumnDimension);
			}

			// row dimension of B must be equal to A
			// if no control matrix is available, the row and column dimension will be 0
			if (controlMatrix != null && controlMatrix.RowDimension > 0 && controlMatrix.ColumnDimension > 0 && controlMatrix.RowDimension != transitionMatrix.RowDimension)
			{
				throw new MatrixDimensionMismatchException(controlMatrix.RowDimension, controlMatrix.ColumnDimension, transitionMatrix.RowDimension, controlMatrix.ColumnDimension);
			}

			// Q must be equal to A
			MatrixUtils.checkAdditionCompatible(transitionMatrix, processNoise);

			// column dimension of H must be equal to row dimension of A
			if (measurementMatrix.ColumnDimension != transitionMatrix.RowDimension)
			{
				throw new MatrixDimensionMismatchException(measurementMatrix.RowDimension, measurementMatrix.ColumnDimension, measurementMatrix.RowDimension, transitionMatrix.RowDimension);
			}

			// row dimension of R must be equal to row dimension of H
			if (measNoise.RowDimension != measurementMatrix.RowDimension)
			{
				throw new MatrixDimensionMismatchException(measNoise.RowDimension, measNoise.ColumnDimension, measurementMatrix.RowDimension, measNoise.ColumnDimension);
			}
		}

		/// <summary>
		/// Returns the dimension of the state estimation vector.
		/// </summary>
		/// <returns> the state dimension </returns>
		public virtual int StateDimension
		{
			get
			{
				return stateEstimation.Dimension;
			}
		}

		/// <summary>
		/// Returns the dimension of the measurement vector.
		/// </summary>
		/// <returns> the measurement vector dimension </returns>
		public virtual int MeasurementDimension
		{
			get
			{
				return measurementMatrix.RowDimension;
			}
		}

		/// <summary>
		/// Returns the current state estimation vector.
		/// </summary>
		/// <returns> the state estimation vector </returns>
		public virtual double[] StateEstimation
		{
			get
			{
				return stateEstimation.toArray();
			}
		}

		/// <summary>
		/// Returns a copy of the current state estimation vector.
		/// </summary>
		/// <returns> the state estimation vector </returns>
		public virtual RealVector StateEstimationVector
		{
			get
			{
				return stateEstimation.copy();
			}
		}

		/// <summary>
		/// Returns the current error covariance matrix.
		/// </summary>
		/// <returns> the error covariance matrix </returns>
		public virtual double[][] ErrorCovariance
		{
			get
			{
				return errorCovariance.Data;
			}
		}

		/// <summary>
		/// Returns a copy of the current error covariance matrix.
		/// </summary>
		/// <returns> the error covariance matrix </returns>
		public virtual RealMatrix ErrorCovarianceMatrix
		{
			get
			{
				return errorCovariance.copy();
			}
		}

		/// <summary>
		/// Predict the internal state estimation one time step ahead.
		/// </summary>
		public virtual void predict()
		{
			predict((RealVector) null);
		}

		/// <summary>
		/// Predict the internal state estimation one time step ahead.
		/// </summary>
		/// <param name="u">
		///            the control vector </param>
		/// <exception cref="DimensionMismatchException">
		///             if the dimension of the control vector does not fit </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void predict(final double[] u) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void predict(double[] u)
		{
			predict(new ArrayRealVector(u, false));
		}

		/// <summary>
		/// Predict the internal state estimation one time step ahead.
		/// </summary>
		/// <param name="u">
		///            the control vector </param>
		/// <exception cref="DimensionMismatchException">
		///             if the dimension of the control vector does not match </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void predict(final org.apache.commons.math3.linear.RealVector u) throws org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void predict(RealVector u)
		{
			// sanity checks
			if (u != null && u.Dimension != controlMatrix.ColumnDimension)
			{
				throw new DimensionMismatchException(u.Dimension, controlMatrix.ColumnDimension);
			}

			// project the state estimation ahead (a priori state)
			// xHat(k)- = A * xHat(k-1) + B * u(k-1)
			stateEstimation = transitionMatrix.operate(stateEstimation);

			// add control input if it is available
			if (u != null)
			{
				stateEstimation = stateEstimation.add(controlMatrix.operate(u));
			}

			// project the error covariance ahead
			// P(k)- = A * P(k-1) * A' + Q
			errorCovariance = transitionMatrix.multiply(errorCovariance).multiply(transitionMatrixT).add(processModel.ProcessNoise);
		}

		/// <summary>
		/// Correct the current state estimate with an actual measurement.
		/// </summary>
		/// <param name="z">
		///            the measurement vector </param>
		/// <exception cref="NullArgumentException">
		///             if the measurement vector is {@code null} </exception>
		/// <exception cref="DimensionMismatchException">
		///             if the dimension of the measurement vector does not fit </exception>
		/// <exception cref="SingularMatrixException">
		///             if the covariance matrix could not be inverted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void correct(final double[] z) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.linear.SingularMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void correct(double[] z)
		{
			correct(new ArrayRealVector(z, false));
		}

		/// <summary>
		/// Correct the current state estimate with an actual measurement.
		/// </summary>
		/// <param name="z">
		///            the measurement vector </param>
		/// <exception cref="NullArgumentException">
		///             if the measurement vector is {@code null} </exception>
		/// <exception cref="DimensionMismatchException">
		///             if the dimension of the measurement vector does not fit </exception>
		/// <exception cref="SingularMatrixException">
		///             if the covariance matrix could not be inverted </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void correct(final org.apache.commons.math3.linear.RealVector z) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.DimensionMismatchException, org.apache.commons.math3.linear.SingularMatrixException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public virtual void correct(RealVector z)
		{

			// sanity checks
			MathUtils.checkNotNull(z);
			if (z.Dimension != measurementMatrix.RowDimension)
			{
				throw new DimensionMismatchException(z.Dimension, measurementMatrix.RowDimension);
			}

			// S = H * P(k) * H' + R
			RealMatrix s = measurementMatrix.multiply(errorCovariance).multiply(measurementMatrixT).add(measurementModel.MeasurementNoise);

			// invert S
			RealMatrix invertedS = MatrixUtils.inverse(s);

			// Inn = z(k) - H * xHat(k)-
			RealVector innovation = z.subtract(measurementMatrix.operate(stateEstimation));

			// calculate gain matrix
			// K(k) = P(k)- * H' * (H * P(k)- * H' + R)^-1
			// K(k) = P(k)- * H' * S^-1
			RealMatrix kalmanGain = errorCovariance.multiply(measurementMatrixT).multiply(invertedS);

			// update estimate with measurement z(k)
			// xHat(k) = xHat(k)- + K * Inn
			stateEstimation = stateEstimation.add(kalmanGain.operate(innovation));

			// update covariance of prediction error
			// P(k) = (I - K * H) * P(k)-
			RealMatrix identity = MatrixUtils.createRealIdentityMatrix(kalmanGain.RowDimension);
			errorCovariance = identity.subtract(kalmanGain.multiply(measurementMatrix)).multiply(errorCovariance);
		}
	}

}