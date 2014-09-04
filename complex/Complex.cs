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

namespace mathlib.complex
{


    using mathlib;
    using NotPositiveException = mathlib.exception.NotPositiveException;
    using NullArgumentException = mathlib.exception.NullArgumentException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using FastMath = mathlib.util.FastMath;
    using MathUtils = mathlib.util.MathUtils;
    using Precision = mathlib.util.Precision;

    /// <summary>
    /// Representation of a Complex number, i.e. a number which has both a
    /// real and imaginary part.
    /// <br/>
    /// Implementations of arithmetic operations handle {@code NaN} and
    /// infinite values according to the rules for <seealso cref="java.lang.Double"/>, i.e.
    /// <seealso cref="#equals"/> is an equivalence relation for all instances that have
    /// a {@code NaN} in either real or imaginary part, e.g. the following are
    /// considered equal:
    /// <ul>
    ///  <li>{@code 1 + NaNi}</li>
    ///  <li>{@code NaN + i}</li>
    ///  <li>{@code NaN + NaNi}</li>
    /// </ul>
    /// Note that this is in contradiction with the IEEE-754 standard for floating
    /// point numbers (according to which the test {@code x == x} must fail if
    /// {@code x} is {@code NaN}). The method
    /// {@link org.apache.commons.math3.util.Precision#equals(double,double,int)
    /// equals for primitive double} in <seealso cref="org.apache.commons.math3.util.Precision"/>
    /// conforms with IEEE-754 while this class conforms with the standard behavior
    /// for Java object types.
    /// <br/>
    /// Implements Serializable since 2.0
    /// 
    /// @version $Id: Complex.java 1591835 2014-05-02 09:04:01Z tn $
    /// </summary>
    [Serializable]
    public class Complex : FieldElement<Complex>
    {
        /// <summary>
        /// The square root of -1. A number representing "0.0 + 1.0i" </summary>
        public static readonly Complex I = new Complex(0.0, 1.0);
        // CHECKSTYLE: stop ConstantName
        /// <summary>
        /// A complex number representing "NaN + NaNi" </summary>
        public static readonly Complex NaN_Renamed = new Complex(double.NaN, double.NaN);
        // CHECKSTYLE: resume ConstantName
        /// <summary>
        /// A complex number representing "+INF + INFi" </summary>
        public static readonly Complex INF = new Complex(double.PositiveInfinity, double.PositiveInfinity);
        /// <summary>
        /// A complex number representing "1.0 + 0.0i" </summary>
        public static readonly Complex ONE = new Complex(1.0, 0.0);
        /// <summary>
        /// A complex number representing "0.0 + 0.0i" </summary>
        public static readonly Complex ZERO = new Complex(0.0, 0.0);

        /// <summary>
        /// Serializable version identifier </summary>
        private const long SerialVersionUid = -6195664516687396620L;

        /// <summary>
        /// The imaginary part. </summary>
        private readonly double imaginary;
        /// <summary>
        /// The real part. </summary>
        private readonly double real;
        /// <summary>
        /// Record whether this complex number is equal to NaN. </summary>
        [NonSerialized]
        private readonly bool isNaN;
        /// <summary>
        /// Record whether this complex number is infinite. </summary>
        [NonSerialized]
        private readonly bool isInfinite;

        /// <summary>
        /// Create a complex number given only the real part.
        /// </summary>
        /// <param name="real"> Real part. </param>
        public Complex(double real)
            : this(real, 0.0)
        {
        }

        /// <summary>
        /// Create a complex number given the real and imaginary parts.
        /// </summary>
        /// <param name="real"> Real part. </param>
        /// <param name="imaginary"> Imaginary part. </param>
        public Complex(double real, double imaginary)
        {
            this.real = real;
            this.imaginary = imaginary;

            isNaN = double.IsNaN(real) || double.IsNaN(imaginary);
            isInfinite = !isNaN && (double.IsInfinity(real) || double.IsInfinity(imaginary));
        }

        /// <summary>
        /// Return the absolute value of this complex number.
        /// Returns {@code NaN} if either real or imaginary part is {@code NaN}
        /// and {@code Double.POSITIVE_INFINITY} if neither part is {@code NaN},
        /// but at least one part is infinite.
        /// </summary>
        /// <returns> the absolute value. </returns>
        public virtual double Abs()
        {
            if (isNaN)
            {
                return double.NaN;
            }
            if (Infinite)
            {
                return double.PositiveInfinity;
            }
            if (FastMath.abs(real) < FastMath.abs(imaginary))
            {
                if (imaginary == 0.0)
                {
                    return FastMath.abs(real);
                }
                double q = real / imaginary;
                return FastMath.abs(imaginary) * FastMath.sqrt(1 + q * q);
            }
            else
            {
                if (real == 0.0)
                {
                    return FastMath.abs(imaginary);
                }
                double q = imaginary / real;
                return FastMath.abs(real) * FastMath.sqrt(1 + q * q);
            }
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is
        /// {@code (this + addend)}.
        /// Uses the definitional formula
        /// <pre>
        ///  <code>
        ///   (a + bi) + (c + di) = (a+c) + (b+d)i
        ///  </code>
        /// </pre>
        /// <br/>
        /// If either {@code this} or {@code addend} has a {@code NaN} value in
        /// either part, <seealso cref="#NaN"/> is returned; otherwise {@code Infinite}
        /// and {@code NaN} values are returned in the parts of the result
        /// according to the rules for <seealso cref="java.lang.Double"/> arithmetic.
        /// </summary>
        /// <param name="addend"> Value to be added to this {@code Complex}. </param>
        /// <returns> {@code this + addend}. </returns>
        /// <exception cref="NullArgumentException"> if {@code addend} is {@code null}. </exception>
        public virtual Complex Add(Complex addend)
        {
            MathUtils.checkNotNull(addend);
            if (isNaN || addend.isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(real + addend.Real, imaginary + addend.Imaginary);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is {@code (this + addend)},
        /// with {@code addend} interpreted as a real number.
        /// </summary>
        /// <param name="addend"> Value to be added to this {@code Complex}. </param>
        /// <returns> {@code this + addend}. </returns>
        /// <seealso cref= #add(Complex) </seealso>
        public virtual Complex Add(double addend)
        {
            if (isNaN || double.IsNaN(addend))
            {
                return NaN_Renamed;
            }

            return CreateComplex(real + addend, imaginary);
        }

        /// <summary>
        /// Return the conjugate of this complex number.
        /// The conjugate of {@code a + bi} is {@code a - bi}.
        /// <br/>
        /// <seealso cref="#NaN"/> is returned if either the real or imaginary
        /// part of this Complex number equals {@code Double.NaN}.
        /// <br/>
        /// If the imaginary part is infinite, and the real part is not
        /// {@code NaN}, the returned value has infinite imaginary part
        /// of the opposite sign, e.g. the conjugate of
        /// {@code 1 + POSITIVE_INFINITY i} is {@code 1 - NEGATIVE_INFINITY i}.
        /// </summary>
        /// <returns> the conjugate of this Complex object. </returns>
        public virtual Complex Conjugate()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(real, -imaginary);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is
        /// {@code (this / divisor)}.
        /// Implements the definitional formula
        /// <pre>
        ///  <code>
        ///    a + bi          ac + bd + (bc - ad)i
        ///    ----------- = -------------------------
        ///    c + di         c<sup>2</sup> + d<sup>2</sup>
        ///  </code>
        /// </pre>
        /// but uses
        /// <a href="http://doi.acm.org/10.1145/1039813.1039814">
        /// prescaling of operands</a> to limit the effects of overflows and
        /// underflows in the computation.
        /// <br/>
        /// {@code Infinite} and {@code NaN} values are handled according to the
        /// following rules, applied in the order presented:
        /// <ul>
        ///  <li>If either {@code this} or {@code divisor} has a {@code NaN} value
        ///   in either part, <seealso cref="#NaN"/> is returned.
        ///  </li>
        ///  <li>If {@code divisor} equals <seealso cref="#ZERO"/>, <seealso cref="#NaN"/> is returned.
        ///  </li>
        ///  <li>If {@code this} and {@code divisor} are both infinite,
        ///   <seealso cref="#NaN"/> is returned.
        ///  </li>
        ///  <li>If {@code this} is finite (i.e., has no {@code Infinite} or
        ///   {@code NaN} parts) and {@code divisor} is infinite (one or both parts
        ///   infinite), <seealso cref="#ZERO"/> is returned.
        ///  </li>
        ///  <li>If {@code this} is infinite and {@code divisor} is finite,
        ///   {@code NaN} values are returned in the parts of the result if the
        ///   <seealso cref="java.lang.Double"/> rules applied to the definitional formula
        ///   force {@code NaN} results.
        ///  </li>
        /// </ul>
        /// </summary>
        /// <param name="divisor"> Value by which this {@code Complex} is to be divided. </param>
        /// <returns> {@code this / divisor}. </returns>
        /// <exception cref="NullArgumentException"> if {@code divisor} is {@code null}. </exception>
        public virtual Complex Divide(Complex divisor)
        {
            MathUtils.checkNotNull(divisor);
            if (isNaN || divisor.isNaN)
            {
                return NaN_Renamed;
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double c = divisor.getReal();
            double c = divisor.Real;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double d = divisor.getImaginary();
            double d = divisor.Imaginary;
            if (c == 0.0 && d == 0.0)
            {
                return NaN_Renamed;
            }

            if (divisor.Infinite && !Infinite)
            {
                return ZERO;
            }

            if (FastMath.abs(c) < FastMath.abs(d))
            {
                double q = c / d;
                double denominator = c * q + d;
                return CreateComplex((real * q + imaginary) / denominator, (imaginary * q - real) / denominator);
            }
            else
            {
                double q = d / c;
                double denominator = d * q + c;
                return CreateComplex((imaginary * q + real) / denominator, (imaginary - real * q) / denominator);
            }
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is {@code (this / divisor)},
        /// with {@code divisor} interpreted as a real number.
        /// </summary>
        /// <param name="divisor"> Value by which this {@code Complex} is to be divided. </param>
        /// <returns> {@code this / divisor}. </returns>
        /// <seealso cref= #divide(Complex) </seealso>
        public virtual Complex Divide(double divisor)
        {
            if (isNaN || double.IsNaN(divisor))
            {
                return NaN_Renamed;
            }
            if (divisor == 0d)
            {
                return NaN_Renamed;
            }
            if (double.IsInfinity(divisor))
            {
                return !Infinite ? ZERO : NaN_Renamed;
            }
            return CreateComplex(real / divisor, imaginary / divisor);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual Complex Reciprocal()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            if (real == 0.0 && imaginary == 0.0)
            {
                return INF;
            }

            if (isInfinite)
            {
                return ZERO;
            }

            if (FastMath.abs(real) < FastMath.abs(imaginary))
            {
                double q = real / imaginary;
                double scale = 1.0 / (real * q + imaginary);
                return CreateComplex(scale * q, -scale);
            }
            else
            {
                double q = imaginary / real;
                double scale = 1.0 / (imaginary * q + real);
                return CreateComplex(scale, -scale * q);
            }
        }

        /// <summary>
        /// Test for equality with another object.
        /// If both the real and imaginary parts of two complex numbers
        /// are exactly the same, and neither is {@code Double.NaN}, the two
        /// Complex objects are considered to be equal.
        /// The behavior is the same as for JDK's {@link Double#equals(Object)
        /// Double}:
        /// <ul>
        ///  <li>All {@code NaN} values are considered to be equal,
        ///   i.e, if either (or both) real and imaginary parts of the complex
        ///   number are equal to {@code Double.NaN}, the complex number is equal
        ///   to {@code NaN}.
        ///  </li>
        ///  <li>
        ///   Instances constructed with different representations of zero (i.e.
        ///   either "0" or "-0") are <em>not</em> considered to be equal.
        ///  </li>
        /// </ul>
        /// </summary>
        /// <param name="other"> Object to test for equality with this instance. </param>
        /// <returns> {@code true} if the objects are equal, {@code false} if object
        /// is {@code null}, not an instance of {@code Complex}, or not equal to
        /// this instance. </returns>
        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            if (other is Complex)
            {
                Complex c = (Complex)other;
                if (c.isNaN)
                {
                    return isNaN;
                }
                else
                {
                    return MathUtils.Equals(real, c.real) && MathUtils.Equals(imaginary, c.imaginary);
                }
            }
            return false;
        }

        /// <summary>
        /// Test for the floating-point equality between Complex objects.
        /// It returns {@code true} if both arguments are equal or within the
        /// range of allowed error (inclusive).
        /// </summary>
        /// <param name="x"> First value (cannot be {@code null}). </param>
        /// <param name="y"> Second value (cannot be {@code null}). </param>
        /// <param name="maxUlps"> {@code (maxUlps - 1)} is the number of floating point
        /// values between the real (resp. imaginary) parts of {@code x} and
        /// {@code y}. </param>
        /// <returns> {@code true} if there are fewer than {@code maxUlps} floating
        /// point values between the real (resp. imaginary) parts of {@code x}
        /// and {@code y}.
        /// </returns>
        /// <seealso cref= Precision#equals(double,double,int)
        /// @since 3.3 </seealso>
        public static bool Equals(Complex x, Complex y, int maxUlps)
        {
            return Precision.Equals(x.real, y.real, maxUlps) && Precision.Equals(x.imaginary, y.imaginary, maxUlps);
        }

        /// <summary>
        /// Returns {@code true} iff the values are equal as defined by
        /// <seealso cref="#equals(Complex,Complex,int) equals(x, y, 1)"/>.
        /// </summary>
        /// <param name="x"> First value (cannot be {@code null}). </param>
        /// <param name="y"> Second value (cannot be {@code null}). </param>
        /// <returns> {@code true} if the values are equal.
        /// 
        /// @since 3.3 </returns>
        public static bool Equals(Complex x, Complex y)
        {
            return Equals(x, y, 1);
        }

        /// <summary>
        /// Returns {@code true} if, both for the real part and for the imaginary
        /// part, there is no double value strictly between the arguments or the
        /// difference between them is within the range of allowed error
        /// (inclusive).
        /// </summary>
        /// <param name="x"> First value (cannot be {@code null}). </param>
        /// <param name="y"> Second value (cannot be {@code null}). </param>
        /// <param name="eps"> Amount of allowed absolute error. </param>
        /// <returns> {@code true} if the values are two adjacent floating point
        /// numbers or they are within range of each other.
        /// </returns>
        /// <seealso cref= Precision#equals(double,double,double)
        /// @since 3.3 </seealso>
        public static bool Equals(Complex x, Complex y, double eps)
        {
            return Precision.Equals(x.real, y.real, eps) && Precision.Equals(x.imaginary, y.imaginary, eps);
        }

        /// <summary>
        /// Returns {@code true} if, both for the real part and for the imaginary
        /// part, there is no double value strictly between the arguments or the
        /// relative difference between them is smaller or equal to the given
        /// tolerance.
        /// </summary>
        /// <param name="x"> First value (cannot be {@code null}). </param>
        /// <param name="y"> Second value (cannot be {@code null}). </param>
        /// <param name="eps"> Amount of allowed relative error. </param>
        /// <returns> {@code true} if the values are two adjacent floating point
        /// numbers or they are within range of each other.
        /// </returns>
        /// <seealso cref= Precision#equalsWithRelativeTolerance(double,double,double)
        /// @since 3.3 </seealso>
        public static bool EqualsWithRelativeTolerance(Complex x, Complex y, double eps)
        {
            return Precision.equalsWithRelativeTolerance(x.real, y.real, eps) && Precision.equalsWithRelativeTolerance(x.imaginary, y.imaginary, eps);
        }

        /// <summary>
        /// Get a hashCode for the complex number.
        /// Any {@code Double.NaN} value in real or imaginary part produces
        /// the same hash code {@code 7}.
        /// </summary>
        /// <returns> a hash code value for this object. </returns>
        public override int GetHashCode()
        {
            if (isNaN)
            {
                return 7;
            }
            return 37 * (17 * MathUtils.hash(imaginary) + MathUtils.hash(real));
        }

        /// <summary>
        /// Access the imaginary part.
        /// </summary>
        /// <returns> the imaginary part. </returns>
        public virtual double Imaginary
        {
            get
            {
                return imaginary;
            }
        }

        /// <summary>
        /// Access the real part.
        /// </summary>
        /// <returns> the real part. </returns>
        public virtual double Real
        {
            get
            {
                return real;
            }
        }

        /// <summary>
        /// Checks whether either or both parts of this complex number is
        /// {@code NaN}.
        /// </summary>
        /// <returns> true if either or both parts of this complex number is
        /// {@code NaN}; false otherwise. </returns>
        public virtual bool NaN
        {
            get
            {
                return isNaN;
            }
        }

        /// <summary>
        /// Checks whether either the real or imaginary part of this complex number
        /// takes an infinite value (either {@code Double.POSITIVE_INFINITY} or
        /// {@code Double.NEGATIVE_INFINITY}) and neither part
        /// is {@code NaN}.
        /// </summary>
        /// <returns> true if one or both parts of this complex number are infinite
        /// and neither part is {@code NaN}. </returns>
        public virtual bool Infinite
        {
            get
            {
                return isInfinite;
            }
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is {@code this * factor}.
        /// Implements preliminary checks for {@code NaN} and infinity followed by
        /// the definitional formula:
        /// <pre>
        ///  <code>
        ///   (a + bi)(c + di) = (ac - bd) + (ad + bc)i
        ///  </code>
        /// </pre>
        /// Returns <seealso cref="#NaN"/> if either {@code this} or {@code factor} has one or
        /// more {@code NaN} parts.
        /// <br/>
        /// Returns <seealso cref="#INF"/> if neither {@code this} nor {@code factor} has one
        /// or more {@code NaN} parts and if either {@code this} or {@code factor}
        /// has one or more infinite parts (same result is returned regardless of
        /// the sign of the components).
        /// <br/>
        /// Returns finite values in components of the result per the definitional
        /// formula in all remaining cases.
        /// </summary>
        /// <param name="factor"> value to be multiplied by this {@code Complex}. </param>
        /// <returns> {@code this * factor}. </returns>
        /// <exception cref="NullArgumentException"> if {@code factor} is {@code null}. </exception>
        public virtual Complex Multiply(Complex factor)
        {
            MathUtils.checkNotNull(factor);
            if (isNaN || factor.isNaN)
            {
                return NaN_Renamed;
            }
            if (double.IsInfinity(real) || double.IsInfinity(imaginary) || double.IsInfinity(factor.real) || double.IsInfinity(factor.imaginary))
            {
                // we don't use isInfinite() to avoid testing for NaN again
                return INF;
            }
            return CreateComplex(real * factor.real - imaginary * factor.imaginary, real * factor.imaginary + imaginary * factor.real);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is {@code this * factor}, with {@code factor}
        /// interpreted as a integer number.
        /// </summary>
        /// <param name="factor"> value to be multiplied by this {@code Complex}. </param>
        /// <returns> {@code this * factor}. </returns>
        /// <seealso cref= #multiply(Complex) </seealso>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public Complex multiply(final int factor)
        public virtual Complex Multiply(int factor)
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }
            if (double.IsInfinity(real) || double.IsInfinity(imaginary))
            {
                return INF;
            }
            return CreateComplex(real * factor, imaginary * factor);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is {@code this * factor}, with {@code factor}
        /// interpreted as a real number.
        /// </summary>
        /// <param name="factor"> value to be multiplied by this {@code Complex}. </param>
        /// <returns> {@code this * factor}. </returns>
        /// <seealso cref= #multiply(Complex) </seealso>
        public virtual Complex Multiply(double factor)
        {
            if (isNaN || double.IsNaN(factor))
            {
                return NaN_Renamed;
            }
            if (double.IsInfinity(real) || double.IsInfinity(imaginary) || double.IsInfinity(factor))
            {
                // we don't use isInfinite() to avoid testing for NaN again
                return INF;
            }
            return CreateComplex(real * factor, imaginary * factor);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is {@code (-this)}.
        /// Returns {@code NaN} if either real or imaginary
        /// part of this Complex number equals {@code Double.NaN}.
        /// </summary>
        /// <returns> {@code -this}. </returns>
        public virtual Complex Negate()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(-real, -imaginary);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is
        /// {@code (this - subtrahend)}.
        /// Uses the definitional formula
        /// <pre>
        ///  <code>
        ///   (a + bi) - (c + di) = (a-c) + (b-d)i
        ///  </code>
        /// </pre>
        /// If either {@code this} or {@code subtrahend} has a {@code NaN]} value in either part,
        /// <seealso cref="#NaN"/> is returned; otherwise infinite and {@code NaN} values are
        /// returned in the parts of the result according to the rules for
        /// <seealso cref="java.lang.Double"/> arithmetic.
        /// </summary>
        /// <param name="subtrahend"> value to be subtracted from this {@code Complex}. </param>
        /// <returns> {@code this - subtrahend}. </returns>
        /// <exception cref="NullArgumentException"> if {@code subtrahend} is {@code null}. </exception>
        public virtual Complex Subtract(Complex subtrahend)
        {
            MathUtils.checkNotNull(subtrahend);
            if (isNaN || subtrahend.isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(real - subtrahend.Real, imaginary - subtrahend.Imaginary);
        }

        /// <summary>
        /// Returns a {@code Complex} whose value is
        /// {@code (this - subtrahend)}.
        /// </summary>
        /// <param name="subtrahend"> value to be subtracted from this {@code Complex}. </param>
        /// <returns> {@code this - subtrahend}. </returns>
        /// <seealso cref= #subtract(Complex) </seealso>
        public virtual Complex Subtract(double subtrahend)
        {
            if (isNaN || double.IsNaN(subtrahend))
            {
                return NaN_Renamed;
            }
            return CreateComplex(real - subtrahend, imaginary);
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/InverseCosine.html" TARGET="_top">
        /// inverse cosine</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   acos(z) = -i (log(z + i (sqrt(1 - z<sup>2</sup>))))
        ///  </code>
        /// </pre>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN} or infinite.
        /// </summary>
        /// <returns> the inverse cosine of this complex number.
        /// @since 1.2 </returns>
        public virtual Complex Acos()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return this.Add(this.Sqrt1z().Multiply(I)).Log().Multiply(I.Negate());
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/InverseSine.html" TARGET="_top">
        /// inverse sine</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   asin(z) = -i (log(sqrt(1 - z<sup>2</sup>) + iz))
        ///  </code>
        /// </pre>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN} or infinite.
        /// </summary>
        /// <returns> the inverse sine of this complex number.
        /// @since 1.2 </returns>
        public virtual Complex Asin()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return Sqrt1z().Add(this.Multiply(I)).Log().Multiply(I.Negate());
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/InverseTangent.html" TARGET="_top">
        /// inverse tangent</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   atan(z) = (i/2) log((i + z)/(i - z))
        ///  </code>
        /// </pre>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN} or infinite.
        /// </summary>
        /// <returns> the inverse tangent of this complex number
        /// @since 1.2 </returns>
        public virtual Complex Atan()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return this.Add(I).Divide(I.Subtract(this)).Log().Multiply(I.Divide(CreateComplex(2.0, 0.0)));
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/Cosine.html" TARGET="_top">
        /// cosine</a>
        /// of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   cos(a + bi) = cos(a)cosh(b) - sin(a)sinh(b)i
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#sin"/>, <seealso cref="FastMath#cos"/>,
        /// <seealso cref="FastMath#cosh"/> and <seealso cref="FastMath#sinh"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   cos(1 &plusmn; INFINITY i) = 1 &#x2213; INFINITY i
        ///   cos(&plusmn;INFINITY + i) = NaN + NaN i
        ///   cos(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the cosine of this complex number.
        /// @since 1.2 </returns>
        public virtual Complex Cos()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(FastMath.Cos(real) * FastMath.Cosh(imaginary), -FastMath.Sin(real) * FastMath.Sinh(imaginary));
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/HyperbolicCosine.html" TARGET="_top">
        /// hyperbolic cosine</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   cosh(a + bi) = cosh(a)cos(b) + sinh(a)sin(b)i}
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#sin"/>, <seealso cref="FastMath#cos"/>,
        /// <seealso cref="FastMath#cosh"/> and <seealso cref="FastMath#sinh"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   cosh(1 &plusmn; INFINITY i) = NaN + NaN i
        ///   cosh(&plusmn;INFINITY + i) = INFINITY &plusmn; INFINITY i
        ///   cosh(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the hyperbolic cosine of this complex number.
        /// @since 1.2 </returns>
        public virtual Complex Cosh()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(FastMath.cosh(real) * FastMath.cos(imaginary), FastMath.sinh(real) * FastMath.sin(imaginary));
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/ExponentialFunction.html" TARGET="_top">
        /// exponential function</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   exp(a + bi) = exp(a)cos(b) + exp(a)sin(b)i
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#exp"/>, <seealso cref="FastMath#cos"/>, and
        /// <seealso cref="FastMath#sin"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   exp(1 &plusmn; INFINITY i) = NaN + NaN i
        ///   exp(INFINITY + i) = INFINITY + INFINITY i
        ///   exp(-INFINITY + i) = 0 + 0i
        ///   exp(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> <code><i>e</i><sup>this</sup></code>.
        /// @since 1.2 </returns>
        public virtual Complex Exp()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            double expReal = FastMath.exp(real);
            return CreateComplex(expReal * FastMath.Cos(imaginary), expReal * FastMath.Sin(imaginary));
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/NaturalLogarithm.html" TARGET="_top">
        /// natural logarithm</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   log(a + bi) = ln(|a + bi|) + arg(a + bi)i
        ///  </code>
        /// </pre>
        /// where ln on the right hand side is <seealso cref="FastMath#log"/>,
        /// {@code |a + bi|} is the modulus, <seealso cref="Complex#abs"/>,  and
        /// {@code arg(a + bi) = }<seealso cref="FastMath#atan2"/>(b, a).
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite (or critical) values in real or imaginary parts of the input may
        /// result in infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   log(1 &plusmn; INFINITY i) = INFINITY &plusmn; (&pi;/2)i
        ///   log(INFINITY + i) = INFINITY + 0i
        ///   log(-INFINITY + i) = INFINITY + &pi;i
        ///   log(INFINITY &plusmn; INFINITY i) = INFINITY &plusmn; (&pi;/4)i
        ///   log(-INFINITY &plusmn; INFINITY i) = INFINITY &plusmn; (3&pi;/4)i
        ///   log(0 + 0i) = -INFINITY + 0i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the value <code>ln &nbsp; this</code>, the natural logarithm
        /// of {@code this}.
        /// @since 1.2 </returns>
        public virtual Complex Log()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(FastMath.Log(Abs()), FastMath.Atan2(imaginary, real));
        }

        /// <summary>
        /// Returns of value of this complex number raised to the power of {@code x}.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   y<sup>x</sup> = exp(x&middot;log(y))
        ///  </code>
        /// </pre>
        /// where {@code exp} and {@code log} are <seealso cref="#exp"/> and
        /// <seealso cref="#log"/>, respectively.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN} or infinite, or if {@code y}
        /// equals <seealso cref="Complex#ZERO"/>.
        /// </summary>
        /// <param name="x"> exponent to which this {@code Complex} is to be raised. </param>
        /// <returns> <code> this<sup>{@code x}</sup></code>. </returns>
        /// <exception cref="NullArgumentException"> if x is {@code null}.
        /// @since 1.2 </exception>
        public virtual Complex Pow(Complex x)
        {
            MathUtils.checkNotNull(x);
            return this.Log().Multiply(x).Exp();
        }

        /// <summary>
        /// Returns of value of this complex number raised to the power of {@code x}.
        /// </summary>
        /// <param name="x"> exponent to which this {@code Complex} is to be raised. </param>
        /// <returns> <code>this<sup>x</sup></code>. </returns>
        /// <seealso cref= #pow(Complex) </seealso>
        public virtual Complex Pow(double x)
        {
            return this.Log().Multiply(x).Exp();
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/Sine.html" TARGET="_top">
        /// sine</a>
        /// of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   sin(a + bi) = sin(a)cosh(b) - cos(a)sinh(b)i
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#sin"/>, <seealso cref="FastMath#cos"/>,
        /// <seealso cref="FastMath#cosh"/> and <seealso cref="FastMath#sinh"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or {@code NaN} values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   sin(1 &plusmn; INFINITY i) = 1 &plusmn; INFINITY i
        ///   sin(&plusmn;INFINITY + i) = NaN + NaN i
        ///   sin(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the sine of this complex number.
        /// @since 1.2 </returns>
        public virtual Complex Sin()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(FastMath.sin(real) * FastMath.cosh(imaginary), FastMath.cos(real) * FastMath.sinh(imaginary));
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/HyperbolicSine.html" TARGET="_top">
        /// hyperbolic sine</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   sinh(a + bi) = sinh(a)cos(b)) + cosh(a)sin(b)i
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#sin"/>, <seealso cref="FastMath#cos"/>,
        /// <seealso cref="FastMath#cosh"/> and <seealso cref="FastMath#sinh"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   sinh(1 &plusmn; INFINITY i) = NaN + NaN i
        ///   sinh(&plusmn;INFINITY + i) = &plusmn; INFINITY + INFINITY i
        ///   sinh(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the hyperbolic sine of {@code this}.
        /// @since 1.2 </returns>
        public virtual Complex Sinh()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            return CreateComplex(FastMath.sinh(real) * FastMath.cos(imaginary), FastMath.cosh(real) * FastMath.sin(imaginary));
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/SquareRoot.html" TARGET="_top">
        /// square root</a> of this complex number.
        /// Implements the following algorithm to compute {@code sqrt(a + bi)}:
        /// <ol><li>Let {@code t = sqrt((|a| + |a + bi|) / 2)}</li>
        /// <li><pre>if {@code  a &#8805; 0} return {@code t + (b/2t)i}
        ///  else return {@code |b|/2t + sign(b)t i }</pre></li>
        /// </ol>
        /// where <ul>
        /// <li>{@code |a| = }<seealso cref="FastMath#abs"/>(a)</li>
        /// <li>{@code |a + bi| = }<seealso cref="Complex#abs"/>(a + bi)</li>
        /// <li>{@code sign(b) =  }<seealso cref="FastMath#copySign(double,double) copySign(1d, b)"/>
        /// </ul>
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   sqrt(1 &plusmn; INFINITY i) = INFINITY + NaN i
        ///   sqrt(INFINITY + i) = INFINITY + 0i
        ///   sqrt(-INFINITY + i) = 0 + INFINITY i
        ///   sqrt(INFINITY &plusmn; INFINITY i) = INFINITY + NaN i
        ///   sqrt(-INFINITY &plusmn; INFINITY i) = NaN &plusmn; INFINITY i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the square root of {@code this}.
        /// @since 1.2 </returns>
        public virtual Complex Sqrt()
        {
            if (isNaN)
            {
                return NaN_Renamed;
            }

            if (real == 0.0 && imaginary == 0.0)
            {
                return CreateComplex(0.0, 0.0);
            }

            double t = FastMath.sqrt((FastMath.abs(real) + abs()) / 2.0);
            if (real >= 0.0)
            {
                return CreateComplex(t, imaginary / (2.0 * t));
            }
            else
            {
                return CreateComplex(FastMath.abs(imaginary) / (2.0 * t), FastMath.copySign(1d, imaginary) * t);
            }
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/SquareRoot.html" TARGET="_top">
        /// square root</a> of <code>1 - this<sup>2</sup></code> for this complex
        /// number.
        /// Computes the result directly as
        /// {@code sqrt(ONE.subtract(z.multiply(z)))}.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// </summary>
        /// <returns> the square root of <code>1 - this<sup>2</sup></code>.
        /// @since 1.2 </returns>
        public virtual Complex Sqrt1z()
        {
            return CreateComplex(1.0, 0.0).Subtract(this.Multiply(this)).sqrt();
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/Tangent.html" TARGET="_top">
        /// tangent</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   tan(a + bi) = sin(2a)/(cos(2a)+cosh(2b)) + [sinh(2b)/(cos(2a)+cosh(2b))]i
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#sin"/>, <seealso cref="FastMath#cos"/>, <seealso cref="FastMath#cosh"/> and
        /// <seealso cref="FastMath#sinh"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite (or critical) values in real or imaginary parts of the input may
        /// result in infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   tan(a &plusmn; INFINITY i) = 0 &plusmn; i
        ///   tan(&plusmn;INFINITY + bi) = NaN + NaN i
        ///   tan(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///   tan(&plusmn;&pi;/2 + 0 i) = &plusmn;INFINITY + NaN i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the tangent of {@code this}.
        /// @since 1.2 </returns>
        public virtual Complex Tan()
        {
            if (isNaN || double.IsInfinity(real))
            {
                return NaN_Renamed;
            }
            if (imaginary > 20.0)
            {
                return CreateComplex(0.0, 1.0);
            }
            if (imaginary < -20.0)
            {
                return CreateComplex(0.0, -1.0);
            }

            double real2 = 2.0 * real;
            double imaginary2 = 2.0 * imaginary;
            double d = FastMath.cos(real2) + FastMath.cosh(imaginary2);

            return CreateComplex(FastMath.sin(real2) / d, FastMath.sinh(imaginary2) / d);
        }

        /// <summary>
        /// Compute the
        /// <a href="http://mathworld.wolfram.com/HyperbolicTangent.html" TARGET="_top">
        /// hyperbolic tangent</a> of this complex number.
        /// Implements the formula:
        /// <pre>
        ///  <code>
        ///   tan(a + bi) = sinh(2a)/(cosh(2a)+cos(2b)) + [sin(2b)/(cosh(2a)+cos(2b))]i
        ///  </code>
        /// </pre>
        /// where the (real) functions on the right-hand side are
        /// <seealso cref="FastMath#sin"/>, <seealso cref="FastMath#cos"/>, <seealso cref="FastMath#cosh"/> and
        /// <seealso cref="FastMath#sinh"/>.
        /// <br/>
        /// Returns <seealso cref="Complex#NaN"/> if either real or imaginary part of the
        /// input argument is {@code NaN}.
        /// <br/>
        /// Infinite values in real or imaginary parts of the input may result in
        /// infinite or NaN values returned in parts of the result.
        /// <pre>
        ///  Examples:
        ///  <code>
        ///   tanh(a &plusmn; INFINITY i) = NaN + NaN i
        ///   tanh(&plusmn;INFINITY + bi) = &plusmn;1 + 0 i
        ///   tanh(&plusmn;INFINITY &plusmn; INFINITY i) = NaN + NaN i
        ///   tanh(0 + (&pi;/2)i) = NaN + INFINITY i
        ///  </code>
        /// </pre>
        /// </summary>
        /// <returns> the hyperbolic tangent of {@code this}.
        /// @since 1.2 </returns>
        public virtual Complex Tanh()
        {
            if (isNaN || double.IsInfinity(imaginary))
            {
                return NaN_Renamed;
            }
            if (real > 20.0)
            {
                return CreateComplex(1.0, 0.0);
            }
            if (real < -20.0)
            {
                return CreateComplex(-1.0, 0.0);
            }
            double real2 = 2.0 * real;
            double imaginary2 = 2.0 * imaginary;
            double d = FastMath.cosh(real2) + FastMath.cos(imaginary2);

            return CreateComplex(FastMath.sinh(real2) / d, FastMath.sin(imaginary2) / d);
        }



        /// <summary>
        /// Compute the argument of this complex number.
        /// The argument is the angle phi between the positive real axis and
        /// the point representing this number in the complex plane.
        /// The value returned is between -PI (not inclusive)
        /// and PI (inclusive), with negative values returned for numbers with
        /// negative imaginary parts.
        /// <br/>
        /// If either real or imaginary part (or both) is NaN, NaN is returned.
        /// Infinite parts are handled as {@code Math.atan2} handles them,
        /// essentially treating finite parts as zero in the presence of an
        /// infinite coordinate and returning a multiple of pi/4 depending on
        /// the signs of the infinite parts.
        /// See the javadoc for {@code Math.atan2} for full details.
        /// </summary>
        /// <returns> the argument of {@code this}. </returns>
        public virtual double Argument
        {
            get
            {
                return FastMath.atan2(Imaginary, Real);
            }
        }

        /// <summary>
        /// Computes the n-th roots of this complex number.
        /// The nth roots are defined by the formula:
        /// <pre>
        ///  <code>
        ///   z<sub>k</sub> = abs<sup>1/n</sup> (cos(phi + 2&pi;k/n) + i (sin(phi + 2&pi;k/n))
        ///  </code>
        /// </pre>
        /// for <i>{@code k=0, 1, ..., n-1}</i>, where {@code abs} and {@code phi}
        /// are respectively the <seealso cref="#abs() modulus"/> and
        /// <seealso cref="#getArgument() argument"/> of this complex number.
        /// <br/>
        /// If one or both parts of this complex number is NaN, a list with just
        /// one element, <seealso cref="#NaN"/> is returned.
        /// if neither part is NaN, but at least one part is infinite, the result
        /// is a one-element list containing <seealso cref="#INF"/>.
        /// </summary>
        /// <param name="n"> Degree of root. </param>
        /// <returns> a List<Complex> of all {@code n}-th roots of {@code this}. </returns>
        /// <exception cref="NotPositiveException"> if {@code n <= 0}.
        /// @since 2.0 </exception>
        public virtual IList<Complex> nthRoot(int n)
        {

            if (n <= 0)
            {
                throw new NotPositiveException(LocalizedFormats.CANNOT_COMPUTE_NTH_ROOT_FOR_NEGATIVE_N, n);
            }

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final java.util.List<Complex> result = new java.util.ArrayList<Complex>();
            IList<Complex> result = new List<Complex>();

            if (isNaN)
            {
                result.Add(NaN_Renamed);
                return result;
            }
            if (Infinite)
            {
                result.Add(INF);
                return result;
            }

            // nth root of abs -- faster / more accurate to use a solver here?
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double nthRootOfAbs = org.apache.commons.math3.util.FastMath.pow(abs(), 1.0 / n);
            double nthRootOfAbs = FastMath.pow(abs(), 1.0 / n);

            // Compute nth roots of complex number with k = 0, 1, ... n-1
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double nthPhi = getArgument() / n;
            double nthPhi = Argument / n;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final double slice = 2 * org.apache.commons.math3.util.FastMath.PI / n;
            double slice = 2 * FastMath.PI / n;
            double innerPart = nthPhi;
            for (int k = 0; k < n; k++)
            {
                // inner part
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double realPart = nthRootOfAbs * org.apache.commons.math3.util.FastMath.cos(innerPart);
                double realPart = nthRootOfAbs * FastMath.cos(innerPart);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final double imaginaryPart = nthRootOfAbs * org.apache.commons.math3.util.FastMath.sin(innerPart);
                double imaginaryPart = nthRootOfAbs * FastMath.sin(innerPart);
                result.Add(CreateComplex(realPart, imaginaryPart));
                innerPart += slice;
            }

            return result;
        }

        /// <summary>
        /// Create a complex number given the real and imaginary parts.
        /// </summary>
        /// <param name="realPart"> Real part. </param>
        /// <param name="imaginaryPart"> Imaginary part. </param>
        /// <returns> a new complex number instance.
        /// @since 1.2 </returns>
        /// <seealso cref= #valueOf(double, double) </seealso>
        protected internal virtual Complex CreateComplex(double realPart, double imaginaryPart)
        {
            return new Complex(realPart, imaginaryPart);
        }

        /// <summary>
        /// Create a complex number given the real and imaginary parts.
        /// </summary>
        /// <param name="realPart"> Real part. </param>
        /// <param name="imaginaryPart"> Imaginary part. </param>
        /// <returns> a Complex instance. </returns>
        public static Complex ValueOf(double realPart, double imaginaryPart)
        {
            if (double.IsNaN(realPart) || double.IsNaN(imaginaryPart))
            {
                return NaN_Renamed;
            }
            return new Complex(realPart, imaginaryPart);
        }

        /// <summary>
        /// Create a complex number given only the real part.
        /// </summary>
        /// <param name="realPart"> Real part. </param>
        /// <returns> a Complex instance. </returns>
        public static Complex ValueOf(double realPart)
        {
            if (double.IsNaN(realPart))
            {
                return NaN_Renamed;
            }
            return new Complex(realPart);
        }

        /// <summary>
        /// Resolve the transient fields in a deserialized Complex Object.
        /// Subclasses will need to override <seealso cref="#createComplex"/> to
        /// deserialize properly.
        /// </summary>
        /// <returns> A Complex instance with all fields resolved.
        /// @since 2.0 </returns>
        protected internal object ReadResolve()
        {
            return CreateComplex(real, imaginary);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public virtual ComplexField Field
        {
            get
            {
                return ComplexField.Instance;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public override string ToString()
        {
            return "(" + real + ", " + imaginary + ")";
        }

    }

}