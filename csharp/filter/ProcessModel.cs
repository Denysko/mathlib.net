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

	using RealMatrix = org.apache.commons.math3.linear.RealMatrix;
	using RealVector = org.apache.commons.math3.linear.RealVector;

	/// <summary>
	/// Defines the process dynamics model for the use with a <seealso cref="KalmanFilter"/>.
	/// 
	/// @since 3.0
	/// @version $Id: ProcessModel.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface ProcessModel
	{
		/// <summary>
		/// Returns the state transition matrix.
		/// </summary>
		/// <returns> the state transition matrix </returns>
		RealMatrix StateTransitionMatrix {get;}

		/// <summary>
		/// Returns the control matrix.
		/// </summary>
		/// <returns> the control matrix </returns>
		RealMatrix ControlMatrix {get;}

		/// <summary>
		/// Returns the process noise matrix. This method is called by the <seealso cref="KalmanFilter"/> every
		/// prediction step, so implementations of this interface may return a modified process noise
		/// depending on the current iteration step.
		/// </summary>
		/// <returns> the process noise matrix </returns>
		/// <seealso cref= KalmanFilter#predict() </seealso>
		/// <seealso cref= KalmanFilter#predict(double[]) </seealso>
		/// <seealso cref= KalmanFilter#predict(RealVector) </seealso>
		RealMatrix ProcessNoise {get;}

		/// <summary>
		/// Returns the initial state estimation vector.
		/// <p>
		/// <b>Note:</b> if the return value is zero, the Kalman filter will initialize the
		/// state estimation with a zero vector.
		/// </summary>
		/// <returns> the initial state estimation vector </returns>
		RealVector InitialStateEstimate {get;}

		/// <summary>
		/// Returns the initial error covariance matrix.
		/// <p>
		/// <b>Note:</b> if the return value is zero, the Kalman filter will initialize the
		/// error covariance with the process noise matrix.
		/// </summary>
		/// <returns> the initial error covariance matrix </returns>
		RealMatrix InitialErrorCovariance {get;}
	}

}