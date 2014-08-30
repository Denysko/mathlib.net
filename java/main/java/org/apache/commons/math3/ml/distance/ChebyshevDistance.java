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
package org.apache.commons.math3.ml.distance;

import org.apache.commons.math3.util.MathArrays;

/**
 * Calculates the L<sub>&infin;</sub> (max of abs) distance between two points.
 *
 * @version $Id: ChebyshevDistance.java 1519184 2013-08-31 15:22:59Z tn $
 * @since 3.2
 */
public class ChebyshevDistance implements DistanceMeasure {

    /** Serializable version identifier. */
    private static final long serialVersionUID = -4694868171115238296L;

    /** {@inheritDoc} */
    public double compute(double[] a, double[] b) {
        return MathArrays.distanceInf(a, b);
    }

}
