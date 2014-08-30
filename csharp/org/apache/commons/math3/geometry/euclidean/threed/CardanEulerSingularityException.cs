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

namespace org.apache.commons.math3.geometry.euclidean.threed
{

	using MathIllegalStateException = org.apache.commons.math3.exception.MathIllegalStateException;
	using LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats;

	/// <summary>
	/// This class represents exceptions thrown while extractiong Cardan
	/// or Euler angles from a rotation.
	/// 
	/// @version $Id: CardanEulerSingularityException.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2
	/// </summary>
	public class CardanEulerSingularityException : MathIllegalStateException
	{

		/// <summary>
		/// Serializable version identifier </summary>
		private const long serialVersionUID = -1360952845582206770L;

		/// <summary>
		/// Simple constructor.
		/// build an exception with a default message. </summary>
		/// <param name="isCardan"> if true, the rotation is related to Cardan angles,
		/// if false it is related to EulerAngles </param>
		public CardanEulerSingularityException(bool isCardan) : base(isCardan ? LocalizedFormats.CARDAN_ANGLES_SINGULARITY : LocalizedFormats.EULER_ANGLES_SINGULARITY)
		{
		}

	}

}