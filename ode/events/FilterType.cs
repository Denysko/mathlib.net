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

namespace mathlib.ode.events
{

	using MathInternalError = mathlib.exception.MathInternalError;

	/// <summary>
	/// Enumerate for <seealso cref="EventFilter filtering events"/>.
	/// 
	/// @version $Id: FilterType.java 1503290 2013-07-15 15:16:29Z sebb $
	/// @since 3.2
	/// </summary>

	public enum FilterType
	{

		/// <summary>
		/// Constant for triggering only decreasing events.
		/// <p>When this filter is used, the wrapped {@link EventHandler
		/// event handler} {@link EventHandler#eventOccurred(double, double[],
		/// boolean) eventOccurred} method will be called <em>only</em> with
		/// its {@code increasing} argument set to false.</p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		TRIGGER_ONLY_DECREASING_EVENTS
		{

			/// <summary>
			///  {@inheritDoc} </summary>

			/// <summary>
			/// {@inheritDoc}
			/// <p>
			/// states scheduling for computing h(t,y) as an altered version of g(t, y)
			/// <ul>
			/// <li>0 are triggered events for which a zero is produced (here decreasing events)</li>
			/// <li>X are ignored events for which zero is masked (here increasing events)</li>
			/// </ul>
			/// </p>
			/// <pre>
			///  g(t)
			///             ___                     ___                     ___
			///            /   \                   /   \                   /   \
			///           /     \                 /     \                 /     \
			///          /  g>0  \               /  g>0  \               /  g>0  \
			///         /         \             /         \             /         \
			///  ----- X --------- 0 --------- X --------- 0 --------- X --------- 0 ---
			///       /             \         /             \         /             \
			///      /               \ g<0   /               \  g<0  /               \ g<0
			///     /                 \     /                 \     /                 \     /
			/// ___/                   \___/                   \___/                   \___/
			/// </pre>
			/// <pre>
			///  h(t,y)) as an alteration of g(t,y)
			///             ___                                 ___         ___
			///    \       /   \                               /   \       /   \
			///     \     /     \ h=+g                        /     \     /     \
			///      \   /       \      h=min(-s,-g,+g)      /       \   /       \
			///       \_/         \                         /         \_/         \
			///  ------ ---------- 0 ----------_---------- 0 --------------------- 0 ---
			///                     \         / \         /                         \
			///   h=max(+s,-g,+g)    \       /   \       /       h=max(+s,-g,+g)     \
			///                       \     /     \     / h=-g                        \     /
			///                        \___/       \___/                               \___/
			/// </pre>
			/// <p>
			/// As shown by the figure above, several expressions are used to compute h,
			/// depending on the current state:
			/// <ul>
			///   <li>h = max(+s,-g,+g)</li>
			///   <li>h = +g</li>
			///   <li>h = min(-s,-g,+g)</li>
			///   <li>h = -g</li>
			/// </ul>
			/// where s is a tiny positive value: <seealso cref="mathlib.util.Precision#SAFE_MIN"/>.
			/// </p>
			/// </summary>

		},

		/// <summary>
		/// Constant for triggering only increasing events.
		/// <p>When this filter is used, the wrapped {@link EventHandler
		/// event handler} {@link EventHandler#eventOccurred(double, double[],
		/// boolean) eventOccurred} method will be called <em>only</em> with
		/// its {@code increasing} argument set to true.</p>
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
		TRIGGER_ONLY_INCREASING_EVENTS
		{

			/// <summary>
			///  {@inheritDoc} </summary>

			/// <summary>
			/// {@inheritDoc}
			/// <p>
			/// states scheduling for computing h(t,y) as an altered version of g(t, y)
			/// <ul>
			/// <li>0 are triggered events for which a zero is produced (here increasing events)</li>
			/// <li>X are ignored events for which zero is masked (here decreasing events)</li>
			/// </ul>
			/// </p>
			/// <pre>
			///  g(t)
			///             ___                     ___                     ___
			///            /   \                   /   \                   /   \
			///           /     \                 /     \                 /     \
			///          /  g>0  \               /  g>0  \               /  g>0  \
			///         /         \             /         \             /         \
			///  ----- 0 --------- X --------- 0 --------- X --------- 0 --------- X ---
			///       /             \         /             \         /             \
			///      /               \ g<0   /               \  g<0  /               \ g<0
			///     /                 \     /                 \     /                 \     /
			/// ___/                   \___/                   \___/                   \___/
			/// </pre>
			/// <pre>
			///  h(t,y)) as an alteration of g(t,y)
			///                                     ___         ___
			///    \                               /   \       /   \
			///     \ h=-g                        /     \     /     \ h=-g
			///      \      h=min(-s,-g,+g)      /       \   /       \      h=min(-s,-g,+g)
			///       \                         /         \_/         \
			///  ------0 ----------_---------- 0 --------------------- 0 --------- _ ---
			///         \         / \         /                         \         / \
			///          \       /   \       /       h=max(+s,-g,+g)     \       /   \
			///           \     /     \     / h=+g                        \     /     \     /
			///            \___/       \___/                               \___/       \___/
			/// </pre>
			/// <p>
			/// As shown by the figure above, several expressions are used to compute h,
			/// depending on the current state:
			/// <ul>
			///   <li>h = max(+s,-g,+g)</li>
			///   <li>h = +g</li>
			///   <li>h = min(-s,-g,+g)</li>
			///   <li>h = -g</li>
			/// </ul>
			/// where s is a tiny positive value: <seealso cref="mathlib.util.Precision#SAFE_MIN"/>.
			/// </p>
			/// </summary>

		}

		/// <summary>
		/// Get the increasing status of triggered events. </summary>
		/// <returns> true if triggered events are increasing events </returns>
		protected = 

		/// <summary>
		/// Get next function transformer in the specified direction. </summary>
		/// <param name="previous"> transformer active on the previous point with respect
		/// to integration direction (may be null if no previous point is known) </param>
		/// <param name="g"> current value of the g function </param>
		/// <param name="forward"> true if integration goes forward </param>
		/// <returns> next transformer transformer </returns>
//JAVA TO C# CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		protected abstract Transformer selectTransformer(Transformer previous, double g, boolean forward);

	}

}