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

namespace org.apache.commons.math3.geometry.euclidean.oned
{


	using MathParseException = org.apache.commons.math3.exception.MathParseException;
	using org.apache.commons.math3.geometry;
	using org.apache.commons.math3.geometry;
	using CompositeFormat = org.apache.commons.math3.util.CompositeFormat;

	/// <summary>
	/// Formats a 1D vector in components list format "{x}".
	/// <p>The prefix and suffix "{" and "}" can be replaced by
	/// any user-defined strings. The number format for components can be configured.</p>
	/// <p>White space is ignored at parse time, even if it is in the prefix, suffix
	/// or separator specifications. So even if the default separator does include a space
	/// character that is used at format time, both input string "{1}" and
	/// " { 1 } " will be parsed without error and the same vector will be
	/// returned. In the second case, however, the parse position after parsing will be
	/// just after the closing curly brace, i.e. just before the trailing space.</p>
	/// <p><b>Note:</b> using "," as a separator may interfere with the grouping separator
	/// of the default <seealso cref="NumberFormat"/> for the current locale. Thus it is advised
	/// to use a <seealso cref="NumberFormat"/> instance with disabled grouping in such a case.</p>
	/// 
	/// @version $Id: Vector1DFormat.java 1467801 2013-04-14 16:19:33Z tn $
	/// @since 3.0
	/// </summary>
	public class Vector1DFormat : VectorFormat<Euclidean1D>
	{

		/// <summary>
		/// Create an instance with default settings.
		/// <p>The instance uses the default prefix, suffix and separator:
		/// "{", "}", and "; " and the default number format for components.</p>
		/// </summary>
		public Vector1DFormat() : base(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with a custom number format for components. </summary>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector1DFormat(final java.text.NumberFormat format)
		public Vector1DFormat(NumberFormat format) : base(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, format)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix and separator. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector1DFormat(final String prefix, final String suffix)
		public Vector1DFormat(string prefix, string suffix) : base(prefix, suffix, DEFAULT_SEPARATOR, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix, separator and format
		/// for components. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector1DFormat(final String prefix, final String suffix, final java.text.NumberFormat format)
		public Vector1DFormat(string prefix, string suffix, NumberFormat format) : base(prefix, suffix, DEFAULT_SEPARATOR, format)
		{
		}

		/// <summary>
		/// Returns the default 1D vector format for the current locale. </summary>
		/// <returns> the default 1D vector format. </returns>
		public static Vector1DFormat Instance
		{
			get
			{
				return getInstance(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the default 1D vector format for the given locale. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the 1D vector format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Vector1DFormat getInstance(final java.util.Locale locale)
		public static Vector1DFormat getInstance(Locale locale)
		{
			return new Vector1DFormat(CompositeFormat.getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final org.apache.commons.math3.geometry.Vector<Euclidean1D> vector, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public override StringBuilder format(Vector<Euclidean1D> vector, StringBuilder toAppendTo, FieldPosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector1D p1 = (Vector1D) vector;
			Vector1D p1 = (Vector1D) vector;
			return format(toAppendTo, pos, p1.X);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Vector1D parse(final String source) throws org.apache.commons.math3.exception.MathParseException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override Vector1D parse(string source)
		{
			ParsePosition parsePosition = new ParsePosition(0);
			Vector1D result = parse(source, parsePosition);
			if (parsePosition.Index == 0)
			{
				throw new MathParseException(source, parsePosition.ErrorIndex, typeof(Vector1D));
			}
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Vector1D parse(final String source, final java.text.ParsePosition pos)
		public override Vector1D parse(string source, ParsePosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] coordinates = parseCoordinates(1, source, pos);
			double[] coordinates = parseCoordinates(1, source, pos);
			if (coordinates == null)
			{
				return null;
			}
			return new Vector1D(coordinates[0]);
		}

	}

}