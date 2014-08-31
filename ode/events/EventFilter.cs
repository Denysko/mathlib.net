using System;

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

namespace org.apache.commons.math3.ode.events
{

	/// <summary>
	/// Wrapper used to detect only increasing or decreasing events.
	/// 
	/// <p>General <seealso cref="EventHandler events"/> are defined implicitely
	/// by a <seealso cref="EventHandler#g(double, double[]) g function"/> crossing
	/// zero. This function needs to be continuous in the event neighborhood,
	/// and its sign must remain consistent between events. This implies that
	/// during an ODE integration, events triggered are alternately events
	/// for which the function increases from negative to positive values,
	/// and events for which the function decreases from positive to
	/// negative values.
	/// </p>
	/// 
	/// <p>Sometimes, users are only interested in one type of event (say
	/// increasing events for example) and not in the other type. In these
	/// cases, looking precisely for all events location and triggering
	/// events that will later be ignored is a waste of computing time.</p>
	/// 
	/// <p>Users can wrap a regular <seealso cref="EventHandler event handler"/> in
	/// an instance of this class and provide this wrapping instance to
	/// the <seealso cref="org.apache.commons.math3.ode.FirstOrderIntegrator ODE solver"/>
	/// in order to avoid wasting time looking for uninteresting events.
	/// The wrapper will intercept the calls to the {@link
	/// EventHandler#g(double, double[]) g function} and to the {@link
	/// EventHandler#eventOccurred(double, double[], boolean)
	/// eventOccurred} method in order to ignore uninteresting events. The
	/// wrapped regular <seealso cref="EventHandler event handler"/> will the see only
	/// the interesting events, i.e. either only {@code increasing} events or
	/// {@code decreasing} events. the number of calls to the {@link
	/// EventHandler#g(double, double[]) g function} will also be reduced.</p>
	/// 
	/// @version $Id: EventFilter.java 1458491 2013-03-19 20:13:11Z luc $
	/// @since 3.2
	/// </summary>

	public class EventFilter : EventHandler
	{

		/// <summary>
		/// Number of past transformers updates stored. </summary>
		private const int HISTORY_SIZE = 100;

		/// <summary>
		/// Wrapped event handler. </summary>
		private readonly EventHandler rawHandler;

		/// <summary>
		/// Filter to use. </summary>
		private readonly FilterType filter;

		/// <summary>
		/// Transformers of the g function. </summary>
		private readonly Transformer[] transformers;

		/// <summary>
		/// Update time of the transformers. </summary>
		private readonly double[] updates;

		/// <summary>
		/// Indicator for forward integration. </summary>
		private bool forward;

		/// <summary>
		/// Extreme time encountered so far. </summary>
		private double extremeT;

		/// <summary>
		/// Wrap an <seealso cref="EventHandler event handler"/>. </summary>
		/// <param name="rawHandler"> event handler to wrap </param>
		/// <param name="filter"> filter to use </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public EventFilter(final EventHandler rawHandler, final FilterType filter)
		public EventFilter(EventHandler rawHandler, FilterType filter)
		{
			this.rawHandler = rawHandler;
			this.filter = filter;
			this.transformers = new Transformer[HISTORY_SIZE];
			this.updates = new double[HISTORY_SIZE];
		}

		/// <summary>
		///  {@inheritDoc} </summary>
		public virtual void init(double t0, double[] y0, double t)
		{

			// delegate to raw handler
			rawHandler.init(t0, y0, t);

			// initialize events triggering logic
			forward = t >= t0;
			extremeT = forward ? double.NegativeInfinity : double.PositiveInfinity;
			Arrays.fill(transformers, Transformer.UNINITIALIZED);
			Arrays.fill(updates, extremeT);

		}

		/// <summary>
		///  {@inheritDoc} </summary>
		public virtual double g(double t, double[] y)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double rawG = rawHandler.g(t, y);
			double rawG = rawHandler.g(t, y);

			// search which transformer should be applied to g
			if (forward)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int last = transformers.length - 1;
				int last = transformers.Length - 1;
				if (extremeT < t)
				{
					// we are at the forward end of the history

					// check if a new rough root has been crossed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Transformer previous = transformers[last];
					Transformer previous = transformers[last];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Transformer next = filter.selectTransformer(previous, rawG, forward);
					Transformer next = filter.selectTransformer(previous, rawG, forward);
					if (next != previous)
					{
						// there is a root somewhere between extremeT end t
						// the new transformer, which is valid on both sides of the root,
						// so it is valid for t (this is how we have just computed it above),
						// but it was already valid before, so we store the switch at extremeT
						// for safety, to ensure the previous transformer is not applied too
						// close of the root
						Array.Copy(updates, 1, updates, 0, last);
						Array.Copy(transformers, 1, transformers, 0, last);
						updates[last] = extremeT;
						transformers[last] = next;
					}

					extremeT = t;

					// apply the transform
					return next.transformed(rawG);

				}
				else
				{
					// we are in the middle of the history

					// select the transformer
					for (int i = last; i > 0; --i)
					{
						if (updates[i] <= t)
						{
							// apply the transform
							return transformers[i].transformed(rawG);
						}
					}

					return transformers[0].transformed(rawG);

				}
			}
			else
			{
				if (t < extremeT)
				{
					// we are at the backward end of the history

					// check if a new rough root has been crossed
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Transformer previous = transformers[0];
					Transformer previous = transformers[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Transformer next = filter.selectTransformer(previous, rawG, forward);
					Transformer next = filter.selectTransformer(previous, rawG, forward);
					if (next != previous)
					{
						// there is a root somewhere between extremeT end t
						// the new transformer, which is valid on both sides of the root,
						// so it is valid for t (this is how we have just computed it above),
						// but it was already valid before, so we store the switch at extremeT
						// for safety, to ensure the previous transformer is not applied too
						// close of the root
						Array.Copy(updates, 0, updates, 1, updates.Length - 1);
						Array.Copy(transformers, 0, transformers, 1, transformers.Length - 1);
						updates[0] = extremeT;
						transformers[0] = next;
					}

					extremeT = t;

					// apply the transform
					return next.transformed(rawG);

				}
				else
				{
					// we are in the middle of the history

					// select the transformer
					for (int i = 0; i < updates.Length - 1; ++i)
					{
						if (t <= updates[i])
						{
							// apply the transform
							return transformers[i].transformed(rawG);
						}
					}

					return transformers[updates.Length - 1].transformed(rawG);

				}
			}

		}

		/// <summary>
		///  {@inheritDoc} </summary>
		public virtual EventHandler_Action eventOccurred(double t, double[] y, bool increasing)
		{
			// delegate to raw handler, fixing increasing status on the fly
			return rawHandler.eventOccurred(t, y, filter.TriggeredIncreasing);
		}

		/// <summary>
		///  {@inheritDoc} </summary>
		public virtual void resetState(double t, double[] y)
		{
			// delegate to raw handler
			rawHandler.resetState(t, y);
		}

	}

}