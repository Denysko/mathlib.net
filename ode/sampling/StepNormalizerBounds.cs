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

namespace org.apache.commons.math3.ode.sampling
{

	/// <summary>
	/// <seealso cref="StepNormalizer Step normalizer"/> bounds settings. They influence
	/// whether the underlying fixed step size step handler is called for the first
	/// and last points. Note that if the last point coincides with a normalized
	/// point, then the underlying fixed step size step handler is always called,
	/// regardless of these settings. </summary>
	/// <seealso cref= StepNormalizer </seealso>
	/// <seealso cref= StepNormalizerMode
	/// @version $Id: StepNormalizerBounds.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 3.0 </seealso>
	public enum StepNormalizerBounds
	{
		/// <summary>
		/// Do not include the first and last points. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		NEITHER(false, false),

		/// <summary>
		/// Include the first point, but not the last point. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		FIRST(true, false),

		/// <summary>
		/// Include the last point, but not the first point. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		LAST(false, true),

		/// <summary>
		/// Include both the first and last points. </summary>
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		BOTH(true, true);

		/// <summary>
		/// Whether the first point should be passed to the underlying fixed
		/// step size step handler.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//		private final boolean first;

		/// <summary>
		/// Whether the last point should be passed to the underlying fixed
		/// step size step handler.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain fields in .NET:
//		private final boolean last;

		/// <summary>
		/// Simple constructor. </summary>
		/// <param name="first"> Whether the first point should be passed to the
		/// underlying fixed step size step handler. </param>
		/// <param name="last"> Whether the last point should be passed to the
		/// underlying fixed step size step handler. </param>
//JAVA TO C# CONVERTER TODO TASK: Enums cannot contain methods in .NET:
//		private StepNormalizerBounds(final boolean first, final boolean last)
	//	{
	//		this.first = first;
	//		this.last = last;
	//	}

		/// <summary>
		/// Returns a value indicating whether the first point should be passed
		/// to the underlying fixed step size step handler. </summary>
		/// <returns> value indicating whether the first point should be passed
		/// to the underlying fixed step size step handler. </returns>

		/// <summary>
		/// Returns a value indicating whether the last point should be passed
		/// to the underlying fixed step size step handler. </summary>
		/// <returns> value indicating whether the last point should be passed
		/// to the underlying fixed step size step handler. </returns>
	}
	public static partial class EnumExtensionMethods
	{
		public static bool firstIncluded(this StepNormalizerBounds instance)
		{
			return first;
		}
		public static bool lastIncluded(this StepNormalizerBounds instance)
		{
			return last;
		}
	}

}