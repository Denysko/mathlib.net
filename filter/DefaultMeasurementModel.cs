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
	using NoDataException = org.apache.commons.math3.exception.NoDataException;
	using NullArgumentException = org.apache.commons.math3.exception.NullArgumentException;
	using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;

	/// <summary>
	/// Default implementation of a <seealso cref="MeasurementModel"/> for the use with a <seealso cref="KalmanFilter"/>.
	/// 
	/// @since 3.0
	/// @version $Id: DefaultMeasurementModel.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public class DefaultMeasurementModel : MeasurementModel
	{

		/// <summary>
		/// The measurement matrix, used to associate the measurement vector to the
		/// internal state estimation vector.
		/// </summary>
		private RealMatrix measurementMatrix;

		/// <summary>
		/// The measurement noise covariance matrix.
		/// </summary>
		private RealMatrix measurementNoise;

		/// <summary>
		/// Create a new <seealso cref="MeasurementModel"/>, taking double arrays as input parameters for the
		/// respective measurement matrix and noise.
		/// </summary>
		/// <param name="measMatrix">
		///            the measurement matrix </param>
		/// <param name="measNoise">
		///            the measurement noise matrix </param>
		/// <exception cref="NullArgumentException">
		///             if any of the input matrices is {@code null} </exception>
		/// <exception cref="NoDataException">
		///             if any row / column dimension of the input matrices is zero </exception>
		/// <exception cref="DimensionMismatchException">
		///             if any of the input matrices is non-rectangular </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public DefaultMeasurementModel(final double[][] measMatrix, final double[][] measNoise) throws org.apache.commons.math3.exception.NullArgumentException, org.apache.commons.math3.exception.NoDataException, org.apache.commons.math3.exception.DimensionMismatchException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public DefaultMeasurementModel(double[][] measMatrix, double[][] measNoise) : this(new Array2DRowRealMatrix(measMatrix), new Array2DRowRealMatrix(measNoise))
		{
		}

		/// <summary>
		/// Create a new <seealso cref="MeasurementModel"/>, taking <seealso cref="RealMatrix"/> objects
		/// as input parameters for the respective measurement matrix and noise.
		/// </summary>
		/// <param name="measMatrix"> the measurement matrix </param>
		/// <param name="measNoise"> the measurement noise matrix </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DefaultMeasurementModel(final org.apache.commons.math3.linear.RealMatrix measMatrix, final org.apache.commons.math3.linear.RealMatrix measNoise)
		public DefaultMeasurementModel(RealMatrix measMatrix, RealMatrix measNoise)
		{
			this.measurementMatrix = measMatrix;
			this.measurementNoise = measNoise;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix MeasurementMatrix
		{
			get
			{
				return measurementMatrix;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual RealMatrix MeasurementNoise
		{
			get
			{
				return measurementNoise;
			}
		}
	}

}