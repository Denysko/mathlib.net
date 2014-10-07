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
    /// <seealso cref="StepNormalizer Step normalizer"/> bounds settings. They influence
    /// whether the underlying fixed step size step handler is called for the first
    /// and last points. Note that if the last point coincides with a normalized
    /// point, then the underlying fixed step size step handler is always called,
    /// regardless of these settings. </summary>
    /// <seealso cref= StepNormalizer </seealso>
    /// <seealso cref= StepNormalizerMode
    /// @version $Id: StepNormalizerBounds.java 1416643 2012-12-03 19:37:14Z tn $
    /// @since 3.0 </seealso>

    public static class StepNormalizerBounds
    {

        private bool first;
        private bool last;

        /// <summary>
        /// Simple constructor. 
        ///</summary>
        /// <param name="first"> Whether the first point should be passed to the
        /// underlying fixed step size step handler. </param>
        /// <param name="last"> Whether the last point should be passed to the
        /// underlying fixed step size step handler. </param>
        private StepNormalizerBounds(bool first, bool last)
        {
            /// <summary>
            /// Whether the first point should be passed to the underlying fixed
            /// step size step handler.
            /// </summary>	
            this.first = first;

            /// <summary>
            /// Whether the last point should be passed to the underlying fixed
            /// step size step handler.
            /// </summary>		
            this.last = last;
        }
        public StepNormalizerBounds(StepNormalizerBoundsEnum instance)
        {
            if (instance == StepNormalizerBoundsEnum.NEITHER)
            {
                this.first = false;
                this.last = false;
            }
            if (instance == StepNormalizerBoundsEnum.FIRST)
            {
                this.first = true;
                this.last = false;
            }
            if (instance == StepNormalizerBoundsEnum.LAST)
            {
                this.first = false;
                this.last = true;
            }
            if (instance == StepNormalizerBoundsEnum.BOTH)
            {
                this.first = true;
                this.last = true;
            }
        }

        ///<summary>
        /// Returns a value indicating whether the first point should be passed
        /// to the underlying fixed step size step handler.
        /// @return value indicating whether the first point should be passed
        /// to the underlying fixed step size step handler.
        ///</summary>	
        public bool firstIncluded()
        {
            return first;
        }

        ///<summary>
        /// Returns a value indicating whether the last point should be passed
        /// to the underlying fixed step size step handler.
        /// @return value indicating whether the last point should be passed
        /// to the underlying fixed step size step handler.
        ///</summary>	
        public bool lastIncluded()
        {
            return last;
        }
    }
}