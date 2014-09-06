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
namespace mathlib.util
{


	/// <summary>
	/// Base class for formatters of composite objects (complex numbers, vectors ...).
	/// 
	/// @version $Id: CompositeFormat.java 1462503 2013-03-29 15:48:27Z luc $
	/// </summary>
	public class CompositeFormat
	{

		/// <summary>
		/// Class contains only static methods.
		/// </summary>
		private CompositeFormat()
		{
		}

		/// <summary>
		/// Create a default number format.  The default number format is based on
		/// <seealso cref="NumberFormat#getInstance()"/> with the only customizing that the
		/// maximum number of fraction digits is set to 10. </summary>
		/// <returns> the default number format. </returns>
		public static NumberFormat DefaultNumberFormat
		{
			get
			{
				return getDefaultNumberFormat(Locale.Default);
			}
		}

		/// <summary>
		/// Create a default number format.  The default number format is based on
		/// <seealso cref="NumberFormat#getInstance(java.util.Locale)"/> with the only
		/// customizing that the maximum number of fraction digits is set to 10. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the default number format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static java.text.NumberFormat getDefaultNumberFormat(final java.util.Locale locale)
		public static NumberFormat getDefaultNumberFormat(Locale locale)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.text.NumberFormat nf = java.text.NumberFormat.getInstance(locale);
			NumberFormat nf = NumberFormat.getInstance(locale);
			nf.MaximumFractionDigits = 10;
			return nf;
		}

		/// <summary>
		/// Parses <code>source</code> until a non-whitespace character is found.
		/// </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter.  On output, <code>pos</code>
		///        holds the index of the next non-whitespace character. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static void parseAndIgnoreWhitespace(final String source, final java.text.ParsePosition pos)
		public static void parseAndIgnoreWhitespace(string source, ParsePosition pos)
		{
			parseNextCharacter(source, pos);
			pos.Index = pos.Index - 1;
		}

		/// <summary>
		/// Parses <code>source</code> until a non-whitespace character is found.
		/// </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the first non-whitespace character. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static char parseNextCharacter(final String source, final java.text.ParsePosition pos)
		public static char parseNextCharacter(string source, ParsePosition pos)
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
		/// Parses <code>source</code> for special double values.  These values
		/// include Double.NaN, Double.POSITIVE_INFINITY, Double.NEGATIVE_INFINITY.
		/// </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="value"> the special value to parse. </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the special number. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static Number parseNumber(final String source, final double value, final java.text.ParsePosition pos)
		private static Number parseNumber(string source, double value, ParsePosition pos)
		{
			Number ret = null;

			StringBuilder sb = new StringBuilder();
			sb.Append('(');
			sb.Append(value);
			sb.Append(')');

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int n = sb.length();
			int n = sb.Length;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startIndex = pos.getIndex();
			int startIndex = pos.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int endIndex = startIndex + n;
			int endIndex = startIndex + n;
			if (endIndex < source.Length && source.Substring(startIndex, endIndex - startIndex).CompareTo(sb.ToString()) == 0)
			{
				ret = Convert.ToDouble(value);
				pos.Index = endIndex;
			}

			return ret;
		}

		/// <summary>
		/// Parses <code>source</code> for a number.  This method can parse normal,
		/// numeric values as well as special values.  These special values include
		/// Double.NaN, Double.POSITIVE_INFINITY, Double.NEGATIVE_INFINITY.
		/// </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="format"> the number format used to parse normal, numeric values. </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> the parsed number. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Number parseNumber(final String source, final java.text.NumberFormat format, final java.text.ParsePosition pos)
		public static Number parseNumber(string source, NumberFormat format, ParsePosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startIndex = pos.getIndex();
			int startIndex = pos.Index;
			Number number = format.parse(source, pos);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int endIndex = pos.getIndex();
			int endIndex = pos.Index;

			// check for error parsing number
			if (startIndex == endIndex)
			{
				// try parsing special numbers
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] special = { Double.NaN, Double.POSITIVE_INFINITY, Double.NEGATIVE_INFINITY };
				double[] special = new double[] {double.NaN, double.PositiveInfinity, double.NegativeInfinity};
				for (int i = 0; i < special.Length; ++i)
				{
					number = parseNumber(source, special[i], pos);
					if (number != null)
					{
						break;
					}
				}
			}

			return number;
		}

		/// <summary>
		/// Parse <code>source</code> for an expected fixed string. </summary>
		/// <param name="source"> the string to parse </param>
		/// <param name="expected"> expected string </param>
		/// <param name="pos"> input/output parsing parameter. </param>
		/// <returns> true if the expected string was there </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static boolean parseFixedstring(final String source, final String expected, final java.text.ParsePosition pos)
		public static bool parseFixedstring(string source, string expected, ParsePosition pos)
		{

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int startIndex = pos.getIndex();
			int startIndex = pos.Index;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int endIndex = startIndex + expected.length();
			int endIndex = startIndex + expected.Length;
			if ((startIndex >= source.Length) || (endIndex > source.Length) || (source.Substring(startIndex, endIndex - startIndex).CompareTo(expected) != 0))
			{
				// set index back to start, error index should be the start index
				pos.Index = startIndex;
				pos.ErrorIndex = startIndex;
				return false;
			}

			// the string was here
			pos.Index = endIndex;
			return true;
		}

		/// <summary>
		/// Formats a double value to produce a string.  In general, the value is
		/// formatted using the formatting rules of <code>format</code>.  There are
		/// three exceptions to this:
		/// <ol>
		/// <li>NaN is formatted as '(NaN)'</li>
		/// <li>Positive infinity is formatted as '(Infinity)'</li>
		/// <li>Negative infinity is formatted as '(-Infinity)'</li>
		/// </ol>
		/// </summary>
		/// <param name="value"> the double to format. </param>
		/// <param name="format"> the format used. </param>
		/// <param name="toAppendTo"> where the text is to be appended </param>
		/// <param name="pos"> On input: an alignment field, if desired. On output: the
		///            offsets of the alignment field </param>
		/// <returns> the value passed in as toAppendTo. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static StringBuffer formatDouble(final double value, final java.text.NumberFormat format, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public static StringBuilder formatDouble(double value, NumberFormat format, StringBuilder toAppendTo, FieldPosition pos)
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
			{
				toAppendTo.Append('(');
				toAppendTo.Append(value);
				toAppendTo.Append(')');
			}
			else
			{
				format.format(value, toAppendTo, pos);
			}
			return toAppendTo;
		}
	}

}