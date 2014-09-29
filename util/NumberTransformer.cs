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

    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;

    /// <summary>
    /// Subclasses implementing this interface can transform Objects to doubles.
    /// @version $Id: NumberTransformer.java 1416643 2012-12-03 19:37:14Z tn $
    /// 
    /// No longer extends Serializable since 2.0
    /// 
    /// </summary>
    public interface NumberTransformer
    {

        /// <summary>
        /// Implementing this interface provides a facility to transform
        /// from Object to Double.
        /// </summary>
        /// <param name="o"> the Object to be transformed. </param>
        /// <returns> the double value of the Object. </returns>
        /// <exception cref="MathIllegalArgumentException"> if the Object can not be transformed into a Double. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: double transform(Object o) throws mathlib.exception.MathIllegalArgumentException;
        double transform(object o);
    }

}