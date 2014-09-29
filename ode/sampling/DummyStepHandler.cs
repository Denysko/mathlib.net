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

namespace mathlib.ode.sampling
{

	/// <summary>
	/// This class is a step handler that does nothing.
	/// 
	/// <p>This class is provided as a convenience for users who are only
	/// interested in the final state of an integration and not in the
	/// intermediate steps. Its handleStep method does nothing.</p>
	/// 
	/// <p>Since this class has no internal state, it is implemented using
	/// the Singleton design pattern. This means that only one instance is
	/// ever created, which can be retrieved using the getInstance
	/// method. This explains why there is no public constructor.</p>
	/// </summary>
	/// <seealso cref= StepHandler
	/// @version $Id: DummyStepHandler.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 1.2 </seealso>

	public class DummyStepHandler : StepHandler
	{

		/// <summary>
		/// Private constructor.
		/// The constructor is private to prevent users from creating
		/// instances (Singleton design-pattern).
		/// </summary>
		private DummyStepHandler()
		{
		}

		/// <summary>
		/// Get the only instance. </summary>
		/// <returns> the only instance </returns>
		public static DummyStepHandler Instance
		{
			get
			{
				return LazyHolder.INSTANCE;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public virtual void init(double t0, double[] y0, double t)
		{
		}

		/// <summary>
		/// Handle the last accepted step.
		/// This method does nothing in this class. </summary>
		/// <param name="interpolator"> interpolator for the last accepted step. For
		/// efficiency purposes, the various integrators reuse the same
		/// object on each call, so if the instance wants to keep it across
		/// all calls (for example to provide at the end of the integration a
		/// continuous model valid throughout the integration range), it
		/// should build a local copy using the clone method and store this
		/// copy. </param>
		/// <param name="isLast"> true if the step is the last one </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void handleStep(final StepInterpolator interpolator, final boolean isLast)
		public virtual void handleStep(StepInterpolator interpolator, bool isLast)
		{
		}

		// CHECKSTYLE: stop HideUtilityClassConstructor
		/// <summary>
		/// Holder for the instance.
		/// <p>We use here the Initialization On Demand Holder Idiom.</p>
		/// </summary>
		private class LazyHolder
		{
			/// <summary>
			/// Cached field instance. </summary>
			internal static readonly DummyStepHandler INSTANCE = new DummyStepHandler();
		}
		// CHECKSTYLE: resume HideUtilityClassConstructor

		/// <summary>
		/// Handle deserialization of the singleton. </summary>
		/// <returns> the singleton instance </returns>
		private object readResolve()
		{
			// return the singleton instance
			return LazyHolder.INSTANCE;
		}

	}

}