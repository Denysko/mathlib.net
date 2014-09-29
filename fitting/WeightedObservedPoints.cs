using System;
using System.Collections.Generic;

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
namespace mathlib.fitting
{


	/// <summary>
	/// Simple container for weighted observed points used
	/// in <seealso cref="AbstractCurveFitter curve fitting"/> algorithms.
	/// 
	/// @version $Id: WeightedObservedPoints.java 1516875 2013-08-23 15:01:33Z erans $
	/// @since 3.3
	/// </summary>
	[Serializable]
	public class WeightedObservedPoints
	{
		/// <summary>
		/// Serializable version id. </summary>
		private const long serialVersionUID = 20130813L;

		/// <summary>
		/// Observed points. </summary>
		private readonly IList<WeightedObservedPoint> observations = new List<WeightedObservedPoint>();

		/// <summary>
		/// Adds a point to the sample.
		/// Calling this method is equivalent to calling
		/// {@code add(1.0, x, y)}.
		/// </summary>
		/// <param name="x"> Abscissa of the point. </param>
		/// <param name="y"> Observed value  at {@code x}. After fitting we should
		/// have {@code f(x)} as close as possible to this value.
		/// </param>
		/// <seealso cref= #add(double, double, double) </seealso>
		/// <seealso cref= #add(WeightedObservedPoint) </seealso>
		/// <seealso cref= #toList() </seealso>
		public virtual void add(double x, double y)
		{
			add(1d, x, y);
		}

		/// <summary>
		/// Adds a point to the sample.
		/// </summary>
		/// <param name="weight"> Weight of the observed point. </param>
		/// <param name="x"> Abscissa of the point. </param>
		/// <param name="y"> Observed value  at {@code x}. After fitting we should
		/// have {@code f(x)} as close as possible to this value.
		/// </param>
		/// <seealso cref= #add(double, double) </seealso>
		/// <seealso cref= #add(WeightedObservedPoint) </seealso>
		/// <seealso cref= #toList() </seealso>
		public virtual void add(double weight, double x, double y)
		{
			observations.Add(new WeightedObservedPoint(weight, x, y));
		}

		/// <summary>
		/// Adds a point to the sample.
		/// </summary>
		/// <param name="observed"> Observed point to add.
		/// </param>
		/// <seealso cref= #add(double, double) </seealso>
		/// <seealso cref= #add(double, double, double) </seealso>
		/// <seealso cref= #toList() </seealso>
		public virtual void add(WeightedObservedPoint observed)
		{
			observations.Add(observed);
		}

		/// <summary>
		/// Gets a <em>snapshot</em> of the observed points.
		/// The list of stored points is copied in order to ensure that
		/// modification of the returned instance does not affect this
		/// container.
		/// Conversely, further modification of this container (through
		/// the {@code add} or {@code clear} methods) will not affect the
		/// returned list.
		/// </summary>
		/// <returns> the observed points, in the order they were added to this
		/// container.
		/// </returns>
		/// <seealso cref= #add(double, double) </seealso>
		/// <seealso cref= #add(double, double, double) </seealso>
		/// <seealso cref= #add(WeightedObservedPoint) </seealso>
		public virtual IList<WeightedObservedPoint> toList()
		{
			// The copy is necessary to ensure thread-safety because of the
			// "clear" method (which otherwise would be able to empty the
			// list of points while it is being used by another thread).
			return new List<WeightedObservedPoint>(observations);
		}

		/// <summary>
		/// Removes all observations from this container.
		/// </summary>
		public virtual void clear()
		{
			observations.Clear();
		}
	}

}