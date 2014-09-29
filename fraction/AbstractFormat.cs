using System;
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

namespace mathlib.fraction
{


	using NullArgumentException = mathlib.exception.NullArgumentException;
	using LocalizedFormats = mathlib.exception.util.LocalizedFormats;

	/// <summary>
	/// Common part shared by both <seealso cref="FractionFormat"/> and <seealso cref="BigFractionFormat"/>.
	/// @version $Id: AbstractFormat.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.0
	/// </summary>
	[Serializable]
	public abstract class AbstractFormat : NumberFormat
	{

		/// <summary>
		/// Serializable version identifier. </summary>
		private const long serialVersionUID = -6981118387974191891L;

		/// <summary>
		/// The format used for the denominator. </summary>
		private NumberFormat denominatorFormat;

		/// <summary>
		/// The format used for the numerator. </summary>
		private NumberFormat numeratorFormat;

		/// <summary>
		/// Create an improper formatting instance with the default number format
		/// for the numerator and denominator.
		/// </summary>
		protected internal AbstractFormat() : this(DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an improper formatting instance with a custom number format for
		/// both the numerator and denominator. </summary>
		/// <param name="format"> the custom format for both the numerator and denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractFormat(final java.text.NumberFormat format)
		protected internal AbstractFormat(NumberFormat format) : this(format, (NumberFormat) format.clone())
		{
		}

		/// <summary>
		/// Create an improper formatting instance with a custom number format for
		/// the numerator and a custom number format for the denominator. </summary>
		/// <param name="numeratorFormat"> the custom format for the numerator. </param>
		/// <param name="denominatorFormat"> the custom format for the denominator. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected AbstractFormat(final java.text.NumberFormat numeratorFormat, final java.text.NumberFormat denominatorFormat)
		protected internal AbstractFormat(NumberFormat numeratorFormat, NumberFormat denominatorFormat)
		{
			this.numeratorFormat = numeratorFormat;
			this.denominatorFormat = denominatorFormat;
		}

		/// <summary>
		/// Create a default number format.  The default number format is based on
		/// <seealso cref="NumberFormat#getNumberInstance(java.util.Locale)"/>. The only
		/// customization is the maximum number of BigFraction digits, which is set to 0. </summary>
		/// <returns> the default number format. </returns>
		protected internal static NumberFormat DefaultNumberFormat
		{
			get
			{
				return getDefaultNumberFormat(Locale.Default);
			}
		}

		/// <summary>
		/// Create a default number format.  The default number format is based on
		/// <seealso cref="NumberFormat#getNumberInstance(java.util.Locale)"/>. The only
		/// customization is the maximum number of BigFraction digits, which is set to 0. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the default number format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static java.text.NumberFormat getDefaultNumberFormat(final java.util.Locale locale)
		protected internal static NumberFormat getDefaultNumberFormat(Locale locale)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat nf = java.text.NumberFormat.getNumberInstance(locale);
			NumberFormat nf = NumberFormat.getNumberInstance(locale);
			nf.MaximumFractionDigits = 0;
			nf.ParseIntegerOnly = true;
			return nf;
		}

		/// <summary>
		/// Access the denominator format. </summary>
		/// <returns> the denominator format. </returns>
		public virtual NumberFormat DenominatorFormat
		{
			get
			{
				return denominatorFormat;
			}
			set
			{
				if (value == null)
				{
					throw new NullArgumentException(LocalizedFormats.DENOMINATOR_FORMAT);
				}
				this.denominatorFormat = value;
			}
		}

		/// <summary>
		/// Access the numerator format. </summary>
		/// <returns> the numerator format. </returns>
		public virtual NumberFormat NumeratorFormat
		{
			get
			{
				return numeratorFormat;
			}
			set
			{
				if (value == null)
				{
					throw new NullArgumentException(LocalizedFormats.NUMERATOR_FORMAT);
				}
				this.numeratorFormat = value;
			}
		}

		/// <summary>
		/// Modify the denominator format. </summary>
		/// <param name="format"> the new denominator format value. </param>
		/// <exception cref="NullArgumentException"> if {@code format} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setDenominatorFormat(final java.text.NumberFormat format)

		/// <summary>
		/// Modify the numerator format. </summary>
		/// <param name="format"> the new numerator format value. </param>
		/// <exception cref="NullArgumentException"> if {@code format} is {@code null}. </exception>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void setNumeratorFormat(final java.text.NumberFormat format)

		/// <summary>
		/// Parses <code>source</code> until a non-whitespace character is found. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter.  On output, <code>pos</code>
		///        holds the index of the next non-whitespace character. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static void parseAndIgnoreWhitespace(final String source, final java.text.ParsePosition pos)
		protected internal static void parseAndIgnoreWhitespace(string source, ParsePosition pos)
		{
			parseNextCharacter(source, pos);
			pos.Index = pos.Index - 1;
		}

		/// <summary>
		/// Parses <code>source</code> until a non-whitespace character is found. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the first non-whitespace character. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected static char parseNextCharacter(final String source, final java.text.ParsePosition pos)
		protected internal static char parseNextCharacter(string source, ParsePosition pos)
		{
			 int index = pos.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = source.length();
			 int n = source.Length;
			 char ret = (char)0;

			 if (index < n)
			 {
				 char c;
				 do
				 {
					 c = source[index++];
				 } while (char.IsWhiteSpace(c) && index < n);
				 pos.Index = index;

				 if (index < n)
				 {
					 ret = c;
				 }
			 }

			 return ret;
		}

		/// <summary>
		/// Formats a double value as a fraction and appends the result to a StringBuffer.
		/// </summary>
		/// <param name="value"> the double value to format </param>
		/// <param name="buffer"> StringBuffer to append to </param>
		/// <param name="position"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> a reference to the appended buffer </returns>
		/// <seealso cref= #format(Object, StringBuffer, FieldPosition) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final double value, final StringBuffer buffer, final java.text.FieldPosition position)
		public override StringBuilder format(double value, StringBuilder buffer, FieldPosition position)
		{
			return format(Convert.ToDouble(value), buffer, position);
		}


		/// <summary>
		/// Formats a long value as a fraction and appends the result to a StringBuffer.
		/// </summary>
		/// <param name="value"> the long value to format </param>
		/// <param name="buffer"> StringBuffer to append to </param>
		/// <param name="position"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> a reference to the appended buffer </returns>
		/// <seealso cref= #format(Object, StringBuffer, FieldPosition) </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final long value, final StringBuffer buffer, final java.text.FieldPosition position)
		public override StringBuilder format(long value, StringBuilder buffer, FieldPosition position)
		{
			return format(Convert.ToInt64(value), buffer, position);
		}

	}

}