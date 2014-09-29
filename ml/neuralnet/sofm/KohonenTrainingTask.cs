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

namespace mathlib.ml.neuralnet.sofm
{


	/// <summary>
	/// Trainer for Kohonen's Self-Organizing Map.
	/// 
	/// @version $Id: KohonenTrainingTask.java 1566092 2014-02-08 18:48:29Z tn $
	/// @since 3.3
	/// </summary>
	public class KohonenTrainingTask : Runnable
	{
		/// <summary>
		/// SOFM to be trained. </summary>
		private readonly Network net;
		/// <summary>
		/// Training data. </summary>
		private readonly IEnumerator<double[]> featuresIterator;
		/// <summary>
		/// Update procedure. </summary>
		private readonly KohonenUpdateAction updateAction;

		/// <summary>
		/// Creates a (sequential) trainer for the given network.
		/// </summary>
		/// <param name="net"> Network to be trained with the SOFM algorithm. </param>
		/// <param name="featuresIterator"> Training data iterator. </param>
		/// <param name="updateAction"> SOFM update procedure. </param>
		public KohonenTrainingTask(Network net, IEnumerator<double[]> featuresIterator, KohonenUpdateAction updateAction)
		{
			this.net = net;
			this.featuresIterator = featuresIterator;
			this.updateAction = updateAction;
		}

		/// <summary>
		/// {@inheritDoc}
		/// </summary>
		public virtual void run()
		{
			while (featuresIterator.MoveNext())
			{
				updateAction.update(net, featuresIterator.Current);
			}
		}
	}

}