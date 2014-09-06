using System.Text;

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


    using MathIllegalArgumentException = mathlib.exception.MathIllegalArgumentException;
    using MathParseException = mathlib.exception.MathParseException;
    using NoDataException = mathlib.exception.NoDataException;
    using NullArgumentException = mathlib.exception.NullArgumentException;
    using LocalizedFormats = mathlib.exception.util.LocalizedFormats;
    using CompositeFormat = mathlib.util.CompositeFormat;

    /// <summary>
    /// Formats a Complex number in cartesian format "Re(c) + Im(c)i".  'i' can
    /// be replaced with 'j' (or anything else), and the number format for both real
    /// and imaginary parts can be configured.
    /// 
    /// @version $Id: ComplexFormat.java 1416643 2012-12-03 19:37:14Z tn $
    /// </summary>
    public class ComplexFormat
    {

        /// <summary>
        /// The default imaginary character. </summary>
        private const string DEFAULT_IMAGINARY_CHARACTER = "i";
        /// <summary>
        /// The notation used to signify the imaginary part of the complex number. </summary>
        private readonly string imaginaryCharacter;
        /// <summary>
        /// The format used for the imaginary part. </summary>
        private readonly NumberFormat imaginaryFormat;
        /// <summary>
        /// The format used for the real part. </summary>
        private readonly NumberFormat realFormat;

        /// <summary>
        /// Create an instance with the default imaginary character, 'i', and the
        /// default number format for both real and imaginary parts.
        /// </summary>
        public ComplexFormat()
        {
            this.imaginaryCharacter = DEFAULT_IMAGINARY_CHARACTER;
            this.imaginaryFormat = CompositeFormat.DefaultNumberFormat;
            this.realFormat = imaginaryFormat;
        }

        /// <summary>
        /// Create an instance with a custom number format for both real and
        /// imaginary parts. </summary>
        /// <param name="format"> the custom format for both real and imaginary parts. </param>
        /// <exception cref="NullArgumentException"> if {@code realFormat} is {@code null}. </exception>
        public ComplexFormat(NumberFormat format)
        {
            if (format == null)
            {
                throw new NullArgumentException(LocalizedFormats.IMAGINARY_FORMAT);
            }
            this.imaginaryCharacter = DEFAULT_IMAGINARY_CHARACTER;
            this.imaginaryFormat = format;
            this.realFormat = format;
        }

        /// <summary>
        /// Create an instance with a custom number format for the real part and a
        /// custom number format for the imaginary part. </summary>
        /// <param name="realFormat"> the custom format for the real part. </param>
        /// <param name="imaginaryFormat"> the custom format for the imaginary part. </param>
        /// <exception cref="NullArgumentException"> if {@code imaginaryFormat} is {@code null}. </exception>
        /// <exception cref="NullArgumentException"> if {@code realFormat} is {@code null}. </exception>
        public ComplexFormat(NumberFormat realFormat, NumberFormat imaginaryFormat)
        {
            if (imaginaryFormat == null)
            {
                throw new NullArgumentException(LocalizedFormats.IMAGINARY_FORMAT);
            }
            if (realFormat == null)
            {
                throw new NullArgumentException(LocalizedFormats.REAL_FORMAT);
            }

            this.imaginaryCharacter = DEFAULT_IMAGINARY_CHARACTER;
            this.imaginaryFormat = imaginaryFormat;
            this.realFormat = realFormat;
        }

        /// <summary>
        /// Create an instance with a custom imaginary character, and the default
        /// number format for both real and imaginary parts. </summary>
        /// <param name="imaginaryCharacter"> The custom imaginary character. </param>
        /// <exception cref="NullArgumentException"> if {@code imaginaryCharacter} is
        /// {@code null}. </exception>
        /// <exception cref="NoDataException"> if {@code imaginaryCharacter} is an
        /// empty string. </exception>
        public ComplexFormat(string imaginaryCharacter)
            : this(imaginaryCharacter, CompositeFormat.DefaultNumberFormat)
        {
        }

        /// <summary>
        /// Create an instance with a custom imaginary character, and a custom number
        /// format for both real and imaginary parts. </summary>
        /// <param name="imaginaryCharacter"> The custom imaginary character. </param>
        /// <param name="format"> the custom format for both real and imaginary parts. </param>
        /// <exception cref="NullArgumentException"> if {@code imaginaryCharacter} is
        /// {@code null}. </exception>
        /// <exception cref="NoDataException"> if {@code imaginaryCharacter} is an
        /// empty string. </exception>
        /// <exception cref="NullArgumentException"> if {@code format} is {@code null}. </exception>
        public ComplexFormat(string imaginaryCharacter, NumberFormat format)
            : this(imaginaryCharacter, format, format)
        {
        }

        /// <summary>
        /// Create an instance with a custom imaginary character, a custom number
        /// format for the real part, and a custom number format for the imaginary
        /// part.
        /// </summary>
        /// <param name="imaginaryCharacter"> The custom imaginary character. </param>
        /// <param name="realFormat"> the custom format for the real part. </param>
        /// <param name="imaginaryFormat"> the custom format for the imaginary part. </param>
        /// <exception cref="NullArgumentException"> if {@code imaginaryCharacter} is
        /// {@code null}. </exception>
        /// <exception cref="NoDataException"> if {@code imaginaryCharacter} is an
        /// empty string. </exception>
        /// <exception cref="NullArgumentException"> if {@code imaginaryFormat} is {@code null}. </exception>
        /// <exception cref="NullArgumentException"> if {@code realFormat} is {@code null}. </exception>
        public ComplexFormat(string imaginaryCharacter, NumberFormat realFormat, NumberFormat imaginaryFormat)
        {
            if (imaginaryCharacter == null)
            {
                throw new NullArgumentException();
            }
            if (imaginaryCharacter.Length == 0)
            {
                throw new NoDataException();
            }
            if (imaginaryFormat == null)
            {
                throw new NullArgumentException(LocalizedFormats.IMAGINARY_FORMAT);
            }
            if (realFormat == null)
            {
                throw new NullArgumentException(LocalizedFormats.REAL_FORMAT);
            }

            this.imaginaryCharacter = imaginaryCharacter;
            this.imaginaryFormat = imaginaryFormat;
            this.realFormat = realFormat;
        }

        /// <summary>
        /// Get the set of locales for which complex formats are available.
        /// <p>This is the same set as the <seealso cref="NumberFormat"/> set.</p> </summary>
        /// <returns> available complex format locales. </returns>
        public static Locale[] AvailableLocales
        {
            get
            {
                return NumberFormat.AvailableLocales;
            }
        }

        /// <summary>
        /// This method calls <seealso cref="#format(Object,StringBuffer,FieldPosition)"/>.
        /// </summary>
        /// <param name="c"> Complex object to format. </param>
        /// <returns> A formatted number in the form "Re(c) + Im(c)i". </returns>
        public virtual string Format(Complex c)
        {
            return format(c, new StringBuilder(), new FieldPosition(0)).ToString();
        }

        /// <summary>
        /// This method calls <seealso cref="#format(Object,StringBuffer,FieldPosition)"/>.
        /// </summary>
        /// <param name="c"> Double object to format. </param>
        /// <returns> A formatted number. </returns>
        public virtual string Format(double? c)
        {
            return format(new Complex(c, 0), new StringBuilder(), new FieldPosition(0)).ToString();
        }

        /// <summary>
        /// Formats a <seealso cref="Complex"/> object to produce a string.
        /// </summary>
        /// <param name="complex"> the object to format. </param>
        /// <param name="toAppendTo"> where the text is to be appended </param>
        /// <param name="pos"> On input: an alignment field, if desired. On output: the
        ///            offsets of the alignment field </param>
        /// <returns> the value passed in as toAppendTo. </returns>
        public virtual StringBuilder Format(Complex complex, StringBuilder toAppendTo, FieldPosition pos)
        {
            pos.BeginIndex = 0;
            pos.EndIndex = 0;

            // format real
            double re = complex.Real;
            CompositeFormat.formatDouble(re, RealFormat, toAppendTo, pos);

            // format sign and imaginary
            double im = complex.Imaginary;
            StringBuilder imAppendTo;
            if (im < 0.0)
            {
                toAppendTo.Append(" - ");
                imAppendTo = formatImaginary(-im, new StringBuilder(), pos);
                toAppendTo.Append(imAppendTo);
                toAppendTo.Append(ImaginaryCharacter);
            }
            else if (im > 0.0 || double.IsNaN(im))
            {
                toAppendTo.Append(" + ");
                imAppendTo = formatImaginary(im, new StringBuilder(), pos);
                toAppendTo.Append(imAppendTo);
                toAppendTo.Append(ImaginaryCharacter);
            }

            return toAppendTo;
        }

        /// <summary>
        /// Format the absolute value of the imaginary part.
        /// </summary>
        /// <param name="absIm"> Absolute value of the imaginary part of a complex number. </param>
        /// <param name="toAppendTo"> where the text is to be appended. </param>
        /// <param name="pos"> On input: an alignment field, if desired. On output: the
        /// offsets of the alignment field. </param>
        /// <returns> the value passed in as toAppendTo. </returns>
        private StringBuilder FormatImaginary(double absIm, StringBuilder toAppendTo, FieldPosition pos)
        {
            pos.BeginIndex = 0;
            pos.EndIndex = 0;

            CompositeFormat.formatDouble(absIm, ImaginaryFormat, toAppendTo, pos);
            if (toAppendTo.ToString().Equals("1"))
            {
                // Remove the character "1" if it is the only one.
                toAppendTo.Length = 0;
            }

            return toAppendTo;
        }

        /// <summary>
        /// Formats a object to produce a string.  {@code obj} must be either a
        /// <seealso cref="Complex"/> object or a <seealso cref="Number"/> object.  Any other type of
        /// object will result in an <seealso cref="IllegalArgumentException"/> being thrown.
        /// </summary>
        /// <param name="obj"> the object to format. </param>
        /// <param name="toAppendTo"> where the text is to be appended </param>
        /// <param name="pos"> On input: an alignment field, if desired. On output: the
        ///            offsets of the alignment field </param>
        /// <returns> the value passed in as toAppendTo. </returns>
        /// <seealso cref= java.text.Format#format(java.lang.Object, java.lang.StringBuffer, java.text.FieldPosition) </seealso>
        /// <exception cref="MathIllegalArgumentException"> is {@code obj} is not a valid type. </exception>
        public virtual StringBuilder Format(object obj, StringBuilder toAppendTo, FieldPosition pos)
        {

            StringBuilder ret = null;

            if (obj is Complex)
            {
                ret = format((Complex)obj, toAppendTo, pos);
            }
            else if (obj is Number)
            {
                ret = format(new Complex((double)((Number)obj), 0.0), toAppendTo, pos);
            }
            else
            {
                throw new MathIllegalArgumentException(LocalizedFormats.CANNOT_FORMAT_INSTANCE_AS_COMPLEX, obj.GetType().Name);
            }

            return ret;
        }

        /// <summary>
        /// Access the imaginaryCharacter. </summary>
        /// <returns> the imaginaryCharacter. </returns>
        public virtual string ImaginaryCharacter
        {
            get
            {
                return imaginaryCharacter;
            }
        }

        /// <summary>
        /// Access the imaginaryFormat. </summary>
        /// <returns> the imaginaryFormat. </returns>
        public virtual NumberFormat ImaginaryFormat
        {
            get
            {
                return imaginaryFormat;
            }
        }

        /// <summary>
        /// Returns the default complex format for the current locale. </summary>
        /// <returns> the default complex format. </returns>
        public static ComplexFormat Instance
        {
            get
            {
                return getInstance(Locale.Default);
            }
        }

        /// <summary>
        /// Returns the default complex format for the given locale. </summary>
        /// <param name="locale"> the specific locale used by the format. </param>
        /// <returns> the complex format specific to the given locale. </returns>
        public static ComplexFormat getInstance(Locale locale)
        {
            NumberFormat f = CompositeFormat.getDefaultNumberFormat(locale);
            return new ComplexFormat(f);
        }

        /// <summary>
        /// Returns the default complex format for the given locale. </summary>
        /// <param name="locale"> the specific locale used by the format. </param>
        /// <param name="imaginaryCharacter"> Imaginary character. </param>
        /// <returns> the complex format specific to the given locale. </returns>
        /// <exception cref="NullArgumentException"> if {@code imaginaryCharacter} is
        /// {@code null}. </exception>
        /// <exception cref="NoDataException"> if {@code imaginaryCharacter} is an
        /// empty string. </exception>
        public static ComplexFormat GetInstance(string imaginaryCharacter, Locale locale)
        {
            NumberFormat f = CompositeFormat.getDefaultNumberFormat(locale);
            return new ComplexFormat(imaginaryCharacter, f);
        }

        /// <summary>
        /// Access the realFormat. </summary>
        /// <returns> the realFormat. </returns>
        public virtual NumberFormat RealFormat
        {
            get
            {
                return realFormat;
            }
        }

        /// <summary>
        /// Parses a string to produce a <seealso cref="Complex"/> object.
        /// </summary>
        /// <param name="source"> the string to parse. </param>
        /// <returns> the parsed <seealso cref="Complex"/> object. </returns>
        /// <exception cref="MathParseException"> if the beginning of the specified string
        /// cannot be parsed. </exception>
        public virtual Complex Parse(string source)
        {
            ParsePosition parsePosition = new ParsePosition(0);
            Complex result = parse(source, parsePosition);
            if (parsePosition.Index == 0)
            {
                throw new MathParseException(source, parsePosition.ErrorIndex, typeof(Complex));
            }
            return result;
        }

        /// <summary>
        /// Parses a string to produce a <seealso cref="Complex"/> object.
        /// </summary>
        /// <param name="source"> the string to parse </param>
        /// <param name="pos"> input/ouput parsing parameter. </param>
        /// <returns> the parsed <seealso cref="Complex"/> object. </returns>
        public virtual Complex Parse(string source, ParsePosition pos)
        {
            int initialIndex = pos.Index;

            // parse whitespace
            CompositeFormat.parseAndIgnoreWhitespace(source, pos);

            // parse real
            Number re = CompositeFormat.parseNumber(source, RealFormat, pos);
            if (re == null)
            {
                // invalid real number
                // set index back to initial, error index should already be set
                pos.Index = initialIndex;
                return null;
            }

            // parse sign
            int startIndex = pos.Index;
            char c = CompositeFormat.parseNextCharacter(source, pos);
            int sign = 0;
            switch (c)
            {
                case 0:
                    // no sign
                    // return real only complex number
                    return new Complex((double)re, 0.0);
                case '-':
                    sign = -1;
                    break;
                case '+':
                    sign = 1;
                    break;
                default:
                    // invalid sign
                    // set index back to initial, error index should be the last
                    // character examined.
                    pos.Index = initialIndex;
                    pos.ErrorIndex = startIndex;
                    return null;
            }

            // parse whitespace
            CompositeFormat.parseAndIgnoreWhitespace(source, pos);

            // parse imaginary
            Number im = CompositeFormat.parseNumber(source, RealFormat, pos);
            if (im == null)
            {
                // invalid imaginary number
                // set index back to initial, error index should already be set
                pos.Index = initialIndex;
                return null;
            }

            // parse imaginary character
            if (!CompositeFormat.parseFixedstring(source, ImaginaryCharacter, pos))
            {
                return null;
            }

            return new Complex((double)re, (double)im * sign);

        }
    }

}