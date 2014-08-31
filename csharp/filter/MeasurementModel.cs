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

	/// <summary>
	/// Defines the measurement model for the use with a <seealso cref="KalmanFilter"/>.
	/// 
	/// @since 3.0
	/// @version $Id: MeasurementModel.java 1416643 2012-12-03 19:37:14Z tn $
	/// </summary>
	public interface MeasurementModel
	{
		/// <summary>
		/// Returns the measurement matrix.
		/// </summary>
		/// <returns> the measurement matrix </returns>
		RealMatrix MeasurementMatrix {get;}

		/// <summary>
		/// Returns the measurement noise matrix. This method is called by the <seealso cref="KalmanFilter"/> every
		/// correction step, so implementations of this interface may return a modified measurement noise
		/// depending on the current iteration step.
		/// </summary>
		/// <returns> the measurement noise matrix </returns>
		/// <seealso cref= KalmanFilter#correct(double[]) </seealso>
		/// <seealso cref= KalmanFilter#correct(org.apache.commons.math3.linear.RealVector) </seealso>
		RealMatrix MeasurementNoise {get;}
	}

}