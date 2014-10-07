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

    using FastMath = mathlib.util.FastMath;
    using Precision = mathlib.util.Precision;


    /// <summary>
    /// Transformer for <seealso cref="EventHandler#g(double, double[]) g functions"/>. 
    /// </summary>
    /// <seealso cref= EventFilter </seealso>
    /// <seealso cref= FilterType </seealso>
    public enum TransformerEnum
    {

        /// <summary>
        /// Transformer computing transformed = 0.
        /// <p>
        /// This transformer is used when we initialize the filter, until we get at
        /// least one non-zero value to select the proper transformer.
        /// </p>
        /// </summary>
        UNINITIALIZED,

        /// <summary>
        /// Transformer computing transformed = g.
        /// <p>
        /// When this transformer is applied, the roots of the original function
        /// are preserved, with the same {@code increasing/decreasing} status.
        /// </p>
        /// </summary>
        PLUS,

        /// <summary>
        /// Transformer computing transformed = -g.
        /// <p>
        /// When this transformer is applied, the roots of the original function
        /// are preserved, with reversed {@code increasing/decreasing} status.
        /// </p>
        /// </summary>
        MINUS,

        /// <summary>
        /// Transformer computing transformed = min(-<seealso cref="Precision#SAFE_MIN"/>, -g, +g).
        /// <p>
        /// When this transformer is applied, the transformed function is
        /// guaranteed to be always strictly negative (i.e. there are no roots).
        /// </p>
        /// </summary>
        MIN,

        /// <summary>
        /// Transformer computing transformed = max(+<seealso cref="Precision#SAFE_MIN"/>, -g, +g).
        /// <p>
        /// When this transformer is applied, the transformed function is
        /// guaranteed to be always strictly positive (i.e. there are no roots).
        /// </p>
        /// </summary>
        MAX
    }

    public static class Transformer
    {
        /// <summary>
        /// Transform value of function g. </summary>
        /// <param name="g"> raw value of function g </param>
        /// <returns> transformed value of function g </returns>
        public static double transformed(TransformerEnum instance, double g)
        {
            if (instance == TransformerEnum.UNINITIALIZED)
            {
                return 0;
            }
            if (instance == TransformerEnum.PLUS)
            {
                return g;
            }
            if (instance == TransformerEnum.MINUS)
            {
                return -g;
            }
            if (instance == TransformerEnum.MIN)
            {
                return FastMath.min(-Precision.SAFE_MIN, FastMath.min(-g, +g));
            }
            if (instance == TransformerEnum.MAX)
            {
                return FastMath.max(+Precision.SAFE_MIN, FastMath.max(-g, +g));
            }
            //Тут взагалі краще генерувати помилку
            return 0;
        }

    }

}
