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
namespace mathlib.util
{

    /// <summary>
    /// The root class from which all events occurring while running an
    /// <seealso cref="IterationManager"/> should be derived.
    /// 
    /// @version $Id: IterationEvent.java 1416643 2012-12-03 19:37:14Z tn $
    /// </summary>
    public class IterationEvent : EventObject
    {
        private const long serialVersionUID = 20120128L;

        /// <summary>
        /// The number of iterations performed so far. </summary>
        private readonly int iterations;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="source"> the iterative algorithm on which the event initially
        /// occurred </param>
        /// <param name="iterations"> the number of iterations performed at the time
        /// {@code this} event is created </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public IterationEvent(final Object source, final int iterations)
        public IterationEvent(object source, int iterations)
            : base(source)
        {
            this.iterations = iterations;
        }

        /// <summary>
        /// Returns the number of iterations performed at the time {@code this} event
        /// is created.
        /// </summary>
        /// <returns> the number of iterations performed </returns>
        public virtual int Iterations
        {
            get
            {
                return iterations;
            }
        }
    }

}