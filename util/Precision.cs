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

namespace mathlib.util
{

    using MathArithmeticException = mathlib.exception.MathArithmeticException;
    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

    /// <summary>
    /// Utilities for comparing numbers.
    /// 
    /// @since 3.0
    /// @version $Id: Precision.java 1591835 2014-05-02 09:04:01Z tn $
    /// </summary>
    public class Precision
    {
        /// <summary>
        /// <p>
        /// Largest double-precision floating-point number such that
        /// {@code 1 + EPSILON} is numerically equal to 1. This value is an upper
        /// bound on the relative error due to rounding real numbers to double
        /// precision floating-point numbers.
        /// </p>
        /// <p>
        /// In IEEE 754 arithmetic, this is 2<sup>-53</sup>.
        /// </p>
        /// </summary>
        /// <seealso cref= <a href="http://en.wikipedia.org/wiki/Machine_epsilon">Machine epsilon</a> </seealso>
        public static readonly double EPSILON;

        /// <summary>
        /// Safe minimum, such that {@code 1 / SAFE_MIN} does not overflow.
        /// <br/>
        /// In IEEE 754 arithmetic, this is also the smallest normalized
        /// number 2<sup>-1022</sup>.
        /// </summary>
        public static readonly double SAFE_MIN;

        /// <summary>
        /// Exponent offset in IEEE754 representation. </summary>
        private const long EXPONENT_OFFSET = 1023l;

        /// <summary>
        /// Offset to order signed double numbers lexicographically. </summary>
        private const long SGN_MASK = 0x8000000000000000L;
        /// <summary>
        /// Offset to order signed double numbers lexicographically. </summary>
        private const int SGN_MASK_FLOAT = unchecked((int)0x80000000);
        /// <summary>
        /// Positive zero. </summary>
        private const double POSITIVE_ZERO = 0d;

        static Precision()
        {
            /*
             *  This was previously expressed as = 0x1.0p-53;
             *  However, OpenJDK (Sparc Solaris) cannot handle such small
             *  constants: MATH-721
             */
            EPSILON = double.longBitsToDouble((EXPONENT_OFFSET - 53l) << 52);

            /*
             * This was previously expressed as = 0x1.0p-1022;
             * However, OpenJDK (Sparc Solaris) cannot handle such small
             * constants: MATH-721
             */
            SAFE_MIN = double.longBitsToDouble((EXPONENT_OFFSET - 1022l) << 52);
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Precision()
        {
        }

        /// <summary>
        /// Compares two numbers given some amount of allowed error.
        /// </summary>
        /// <param name="x"> the first number </param>
        /// <param name="y"> the second number </param>
        /// <param name="eps"> the amount of error to allow when checking for equality </param>
        /// <returns> <ul><li>0 if  <seealso cref="#equals(double, double, double) equals(x, y, eps)"/></li>
        ///       <li>&lt; 0 if !<seealso cref="#equals(double, double, double) equals(x, y, eps)"/> &amp;&amp; x &lt; y</li>
        ///       <li>> 0 if !<seealso cref="#equals(double, double, double) equals(x, y, eps)"/> &amp;&amp; x > y</li></ul> </returns>
        public static int compareTo(double x, double y, double eps)
        {
            if (Equals(x, y, eps))
            {
                return 0;
            }
            else if (x < y)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Compares two numbers given some amount of allowed error.
        /// Two float numbers are considered equal if there are {@code (maxUlps - 1)}
        /// (or fewer) floating point numbers between them, i.e. two adjacent floating
        /// point numbers are considered equal.
        /// Adapted from <a
        /// href="http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm">
        /// Bruce Dawson</a>
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="maxUlps"> {@code (maxUlps - 1)} is the number of floating point
        /// values between {@code x} and {@code y}. </param>
        /// <returns> <ul><li>0 if  <seealso cref="#equals(double, double, int) equals(x, y, maxUlps)"/></li>
        ///       <li>&lt; 0 if !<seealso cref="#equals(double, double, int) equals(x, y, maxUlps)"/> &amp;&amp; x &lt; y</li>
        ///       <li>> 0 if !<seealso cref="#equals(double, double, int) equals(x, y, maxUlps)"/> &amp;&amp; x > y</li></ul> </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public static int compareTo(final double x, final double y, final int maxUlps)
        public static int compareTo(double x, double y, int maxUlps)
        {
            if (Equals(x, y, maxUlps))
            {
                return 0;
            }
            else if (x < y)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Returns true iff they are equal as defined by
        /// <seealso cref="#equals(float,float,int) equals(x, y, 1)"/>.
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <returns> {@code true} if the values are equal. </returns>
        public static bool Equals(float x, float y)
        {
            return Equals(x, y, 1);
        }

        /// <summary>
        /// Returns true if both arguments are NaN or neither is NaN and they are
        /// equal as defined by <seealso cref="#equals(float,float) equals(x, y, 1)"/>.
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <returns> {@code true} if the values are equal or both are NaN.
        /// @since 2.2 </returns>
        public static bool equalsIncludingNaN(float x, float y)
        {
            return (float.IsNaN(x) && float.IsNaN(y)) || Equals(x, y, 1);
        }

        /// <summary>
        /// Returns true if both arguments are equal or within the range of allowed
        /// error (inclusive).
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="eps"> the amount of absolute error to allow. </param>
        /// <returns> {@code true} if the values are equal or within range of each other.
        /// @since 2.2 </returns>
        public static bool Equals(float x, float y, float eps)
        {
            return Equals(x, y, 1) || FastMath.abs(y - x) <= eps;
        }

        /// <summary>
        /// Returns true if both arguments are NaN or are equal or within the range
        /// of allowed error (inclusive).
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="eps"> the amount of absolute error to allow. </param>
        /// <returns> {@code true} if the values are equal or within range of each other,
        /// or both are NaN.
        /// @since 2.2 </returns>
        public static bool equalsIncludingNaN(float x, float y, float eps)
        {
            return equalsIncludingNaN(x, y) || (FastMath.abs(y - x) <= eps);
        }

        /// <summary>
        /// Returns true if both arguments are equal or within the range of allowed
        /// error (inclusive).
        /// Two float numbers are considered equal if there are {@code (maxUlps - 1)}
        /// (or fewer) floating point numbers between them, i.e. two adjacent floating
        /// point numbers are considered equal.
        /// Adapted from <a
        /// href="http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm">
        /// Bruce Dawson</a>
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="maxUlps"> {@code (maxUlps - 1)} is the number of floating point
        /// values between {@code x} and {@code y}. </param>
        /// <returns> {@code true} if there are fewer than {@code maxUlps} floating
        /// point values between {@code x} and {@code y}.
        /// @since 2.2 </returns>
        public static bool Equals(float x, float y, int maxUlps)
        {
            int xInt = float.floatToIntBits(x);
            int yInt = float.floatToIntBits(y);

            // Make lexicographically ordered as a two's-complement integer.
            if (xInt < 0)
            {
                xInt = SGN_MASK_FLOAT - xInt;
            }
            if (yInt < 0)
            {
                yInt = SGN_MASK_FLOAT - yInt;
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final boolean isEqual = FastMath.abs(xInt - yInt) <= maxUlps;
            bool isEqual = FastMath.abs(xInt - yInt) <= maxUlps;

            return isEqual && !float.IsNaN(x) && !float.IsNaN(y);
        }

        /// <summary>
        /// Returns true if both arguments are NaN or if they are equal as defined
        /// by <seealso cref="#equals(float,float,int) equals(x, y, maxUlps)"/>.
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="maxUlps"> {@code (maxUlps - 1)} is the number of floating point
        /// values between {@code x} and {@code y}. </param>
        /// <returns> {@code true} if both arguments are NaN or if there are less than
        /// {@code maxUlps} floating point values between {@code x} and {@code y}.
        /// @since 2.2 </returns>
        public static bool equalsIncludingNaN(float x, float y, int maxUlps)
        {
            return (float.IsNaN(x) && float.IsNaN(y)) || Equals(x, y, maxUlps);
        }

        /// <summary>
        /// Returns true iff they are equal as defined by
        /// <seealso cref="#equals(double,double,int) equals(x, y, 1)"/>.
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <returns> {@code true} if the values are equal. </returns>
        public static bool Equals(double x, double y)
        {
            return Equals(x, y, 1);
        }

        /// <summary>
        /// Returns true if both arguments are NaN or neither is NaN and they are
        /// equal as defined by <seealso cref="#equals(double,double) equals(x, y, 1)"/>.
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <returns> {@code true} if the values are equal or both are NaN.
        /// @since 2.2 </returns>
        public static bool equalsIncludingNaN(double x, double y)
        {
            return (double.IsNaN(x) && double.IsNaN(y)) || Equals(x, y, 1);
        }

        /// <summary>
        /// Returns {@code true} if there is no double value strictly between the
        /// arguments or the difference between them is within the range of allowed
        /// error (inclusive).
        /// </summary>
        /// <param name="x"> First value. </param>
        /// <param name="y"> Second value. </param>
        /// <param name="eps"> Amount of allowed absolute error. </param>
        /// <returns> {@code true} if the values are two adjacent floating point
        /// numbers or they are within range of each other. </returns>
        public static bool Equals(double x, double y, double eps)
        {
            return Equals(x, y, 1) || FastMath.abs(y - x) <= eps;
        }

        /// <summary>
        /// Returns {@code true} if there is no double value strictly between the
        /// arguments or the relative difference between them is smaller or equal
        /// to the given tolerance.
        /// </summary>
        /// <param name="x"> First value. </param>
        /// <param name="y"> Second value. </param>
        /// <param name="eps"> Amount of allowed relative error. </param>
        /// <returns> {@code true} if the values are two adjacent floating point
        /// numbers or they are within range of each other.
        /// @since 3.1 </returns>
        public static bool equalsWithRelativeTolerance(double x, double y, double eps)
        {
            if (Equals(x, y, 1))
            {
                return true;
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double absoluteMax = FastMath.max(FastMath.abs(x), FastMath.abs(y));
            double absoluteMax = FastMath.max(FastMath.abs(x), FastMath.abs(y));
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double relativeDifference = FastMath.abs((x - y) / absoluteMax);
            double relativeDifference = FastMath.abs((x - y) / absoluteMax);

            return relativeDifference <= eps;
        }

        /// <summary>
        /// Returns true if both arguments are NaN or are equal or within the range
        /// of allowed error (inclusive).
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="eps"> the amount of absolute error to allow. </param>
        /// <returns> {@code true} if the values are equal or within range of each other,
        /// or both are NaN.
        /// @since 2.2 </returns>
        public static bool equalsIncludingNaN(double x, double y, double eps)
        {
            return equalsIncludingNaN(x, y) || (FastMath.abs(y - x) <= eps);
        }

        /// <summary>
        /// Returns true if both arguments are equal or within the range of allowed
        /// error (inclusive).
        /// Two float numbers are considered equal if there are {@code (maxUlps - 1)}
        /// (or fewer) floating point numbers between them, i.e. two adjacent floating
        /// point numbers are considered equal.
        /// Adapted from <a
        /// href="http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm">
        /// Bruce Dawson</a>
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="maxUlps"> {@code (maxUlps - 1)} is the number of floating point
        /// values between {@code x} and {@code y}. </param>
        /// <returns> {@code true} if there are fewer than {@code maxUlps} floating
        /// point values between {@code x} and {@code y}. </returns>
        public static bool Equals(double x, double y, int maxUlps)
        {
            long xInt = double.doubleToLongBits(x);
            long yInt = double.doubleToLongBits(y);

            // Make lexicographically ordered as a two's-complement integer.
            if (xInt < 0)
            {
                xInt = SGN_MASK - xInt;
            }
            if (yInt < 0)
            {
                yInt = SGN_MASK - yInt;
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final boolean isEqual = FastMath.abs(xInt - yInt) <= maxUlps;
            bool isEqual = FastMath.abs(xInt - yInt) <= maxUlps;

            return isEqual && !double.IsNaN(x) && !double.IsNaN(y);
        }

        /// <summary>
        /// Returns true if both arguments are NaN or if they are equal as defined
        /// by <seealso cref="#equals(double,double,int) equals(x, y, maxUlps)"/>.
        /// </summary>
        /// <param name="x"> first value </param>
        /// <param name="y"> second value </param>
        /// <param name="maxUlps"> {@code (maxUlps - 1)} is the number of floating point
        /// values between {@code x} and {@code y}. </param>
        /// <returns> {@code true} if both arguments are NaN or if there are less than
        /// {@code maxUlps} floating point values between {@code x} and {@code y}.
        /// @since 2.2 </returns>
        public static bool equalsIncludingNaN(double x, double y, int maxUlps)
        {
            return (double.IsNaN(x) && double.IsNaN(y)) || Equals(x, y, maxUlps);
        }

        /// <summary>
        /// Rounds the given value to the specified number of decimal places.
        /// The value is rounded using the <seealso cref="BigDecimal#ROUND_HALF_UP"/> method.
        /// </summary>
        /// <param name="x"> Value to round. </param>
        /// <param name="scale"> Number of digits to the right of the decimal point. </param>
        /// <returns> the rounded value.
        /// @since 1.1 (previously in {@code MathUtils}, moved as of version 3.0) </returns>
        public static double round(double x, int scale)
        {
            return round(x, scale, decimal.ROUND_HALF_UP);
        }

        /// <summary>
        /// Rounds the given value to the specified number of decimal places.
        /// The value is rounded using the given method which is any method defined
        /// in <seealso cref="BigDecimal"/>.
        /// If {@code x} is infinite or {@code NaN}, then the value of {@code x} is
        /// returned unchanged, regardless of the other parameters.
        /// </summary>
        /// <param name="x"> Value to round. </param>
        /// <param name="scale"> Number of digits to the right of the decimal point. </param>
        /// <param name="roundingMethod"> Rounding method as defined in <seealso cref="BigDecimal"/>. </param>
        /// <returns> the rounded value. </returns>
        /// <exception cref="ArithmeticException"> if {@code roundingMethod == ROUND_UNNECESSARY}
        /// and the specified scaling operation would require rounding. </exception>
        /// <exception cref="IllegalArgumentException"> if {@code roundingMethod} does not
        /// represent a valid rounding mode.
        /// @since 1.1 (previously in {@code MathUtils}, moved as of version 3.0) </exception>
        public static double round(double x, int scale, int roundingMethod)
        {
            try
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double rounded = (new java.math.BigDecimal(Double.toString(x))
                double rounded = (new decimal(Convert.ToString(x))
                     .setScale(scale, roundingMethod)).doubleValue();
                // MATH-1089: negative values rounded to zero should result in negative zero
                return rounded == POSITIVE_ZERO ? POSITIVE_ZERO * x : rounded;
            }
            catch (NumberFormatException ex)
            {
                if (double.IsInfinity(x))
                {
                    return x;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        /// <summary>
        /// Rounds the given value to the specified number of decimal places.
        /// The value is rounded using the <seealso cref="BigDecimal#ROUND_HALF_UP"/> method.
        /// </summary>
        /// <param name="x"> Value to round. </param>
        /// <param name="scale"> Number of digits to the right of the decimal point. </param>
        /// <returns> the rounded value.
        /// @since 1.1 (previously in {@code MathUtils}, moved as of version 3.0) </returns>
        public static float round(float x, int scale)
        {
            return round(x, scale, decimal.ROUND_HALF_UP);
        }

        /// <summary>
        /// Rounds the given value to the specified number of decimal places.
        /// The value is rounded using the given method which is any method defined
        /// in <seealso cref="BigDecimal"/>.
        /// </summary>
        /// <param name="x"> Value to round. </param>
        /// <param name="scale"> Number of digits to the right of the decimal point. </param>
        /// <param name="roundingMethod"> Rounding method as defined in <seealso cref="BigDecimal"/>. </param>
        /// <returns> the rounded value.
        /// @since 1.1 (previously in {@code MathUtils}, moved as of version 3.0) </returns>
        /// <exception cref="MathArithmeticException"> if an exact operation is required but result is not exact </exception>
        /// <exception cref="MathIllegalArgumentException"> if {@code roundingMethod} is not a valid rounding method. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static float round(float x, int scale, int roundingMethod) throws org.apache.commons.math3.exception.MathArithmeticException, org.apache.commons.math3.exception.MathIllegalArgumentException
        public static float round(float x, int scale, int roundingMethod)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final float sign = FastMath.copySign(1f, x);
            float sign = FastMath.copySign(1f, x);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final float factor = (float) FastMath.pow(10.0f, scale) * sign;
            float factor = (float)FastMath.pow(10.0f, scale) * sign;
            return (float)roundUnscaled(x * factor, sign, roundingMethod) / factor;
        }

        /// <summary>
        /// Rounds the given non-negative value to the "nearest" integer. Nearest is
        /// determined by the rounding method specified. Rounding methods are defined
        /// in <seealso cref="BigDecimal"/>.
        /// </summary>
        /// <param name="unscaled"> Value to round. </param>
        /// <param name="sign"> Sign of the original, scaled value. </param>
        /// <param name="roundingMethod"> Rounding method, as defined in <seealso cref="BigDecimal"/>. </param>
        /// <returns> the rounded value. </returns>
        /// <exception cref="MathArithmeticException"> if an exact operation is required but result is not exact </exception>
        /// <exception cref="MathIllegalArgumentException"> if {@code roundingMethod} is not a valid rounding method.
        /// @since 1.1 (previously in {@code MathUtils}, moved as of version 3.0) </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static double roundUnscaled(double unscaled, double sign, int roundingMethod) throws org.apache.commons.math3.exception.MathArithmeticException, org.apache.commons.math3.exception.MathIllegalArgumentException
        private static double roundUnscaled(double unscaled, double sign, int roundingMethod)
        {
            switch (roundingMethod)
            {
                case decimal.ROUND_CEILING:
                    if (sign == -1)
                    {
                        unscaled = FastMath.floor(FastMath.nextAfter(unscaled, double.NegativeInfinity));
                    }
                    else
                    {
                        unscaled = FastMath.ceil(FastMath.nextAfter(unscaled, double.PositiveInfinity));
                    }
                    break;
                case decimal.ROUND_DOWN:
                    unscaled = FastMath.floor(FastMath.nextAfter(unscaled, double.NegativeInfinity));
                    break;
                case decimal.ROUND_FLOOR:
                    if (sign == -1)
                    {
                        unscaled = FastMath.ceil(FastMath.nextAfter(unscaled, double.PositiveInfinity));
                    }
                    else
                    {
                        unscaled = FastMath.floor(FastMath.nextAfter(unscaled, double.NegativeInfinity));
                    }
                    break;
                case decimal.ROUND_HALF_DOWN:
                    {
                        unscaled = FastMath.nextAfter(unscaled, double.NegativeInfinity);
                        double fraction = unscaled - FastMath.floor(unscaled);
                        if (fraction > 0.5)
                        {
                            unscaled = FastMath.ceil(unscaled);
                        }
                        else
                        {
                            unscaled = FastMath.floor(unscaled);
                        }
                        break;
                    }
                case decimal.ROUND_HALF_EVEN:
                    {
                        double fraction = unscaled - FastMath.floor(unscaled);
                        if (fraction > 0.5)
                        {
                            unscaled = FastMath.ceil(unscaled);
                        }
                        else if (fraction < 0.5)
                        {
                            unscaled = FastMath.floor(unscaled);
                        }
                        else
                        {
                            // The following equality test is intentional and needed for rounding purposes
                            if (FastMath.floor(unscaled) / 2.0 == FastMath.floor(FastMath.floor(unscaled) / 2.0)) // even
                            {
                                unscaled = FastMath.floor(unscaled);
                            } // odd
                            else
                            {
                                unscaled = FastMath.ceil(unscaled);
                            }
                        }
                        break;
                    }
                case decimal.ROUND_HALF_UP:
                    {
                        unscaled = FastMath.nextAfter(unscaled, double.PositiveInfinity);
                        double fraction = unscaled - FastMath.floor(unscaled);
                        if (fraction >= 0.5)
                        {
                            unscaled = FastMath.ceil(unscaled);
                        }
                        else
                        {
                            unscaled = FastMath.floor(unscaled);
                        }
                        break;
                    }
                case decimal.ROUND_UNNECESSARY:
                    if (unscaled != FastMath.floor(unscaled))
                    {
                        throw new MathArithmeticException();
                    }
                    break;
                case decimal.ROUND_UP:
                    // do not round if the discarded fraction is equal to zero
                    if (unscaled != FastMath.floor(unscaled))
                    {
                        unscaled = FastMath.ceil(FastMath.nextAfter(unscaled, double.PositiveInfinity));
                    }
                    break;
                default:
                    throw new MathIllegalArgumentException(LocalizedFormats.INVALID_ROUNDING_METHOD, roundingMethod, "ROUND_CEILING", decimal.ROUND_CEILING, "ROUND_DOWN", decimal.ROUND_DOWN, "ROUND_FLOOR", decimal.ROUND_FLOOR, "ROUND_HALF_DOWN", decimal.ROUND_HALF_DOWN, "ROUND_HALF_EVEN", decimal.ROUND_HALF_EVEN, "ROUND_HALF_UP", decimal.ROUND_HALF_UP, "ROUND_UNNECESSARY", decimal.ROUND_UNNECESSARY, "ROUND_UP", decimal.ROUND_UP);
            }
            return unscaled;
        }


        /// <summary>
        /// Computes a number {@code delta} close to {@code originalDelta} with
        /// the property that <pre><code>
        ///   x + delta - x
        /// </code></pre>
        /// is exactly machine-representable.
        /// This is useful when computing numerical derivatives, in order to reduce
        /// roundoff errors.
        /// </summary>
        /// <param name="x"> Value. </param>
        /// <param name="originalDelta"> Offset value. </param>
        /// <returns> a number {@code delta} so that {@code x + delta} and {@code x}
        /// differ by a representable floating number. </returns>
        public static double representableDelta(double x, double originalDelta)
        {
            return x + originalDelta - x;
        }
    }

}