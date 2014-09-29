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

namespace mathlib.geometry.euclidean.twod
{


	using MathParseException = mathlib.exception.MathParseException;
	using mathlib.geometry;
	using mathlib.geometry;
	using CompositeFormat = mathlib.util.CompositeFormat;

	/// <summary>
	/// Formats a 2D vector in components list format "{x; y}".
	/// <p>The prefix and suffix "{" and "}" and the separator "; " can be replaced by
	/// any user-defined strings. The number format for components can be configured.</p>
	/// <p>White space is ignored at parse time, even if it is in the prefix, suffix
	/// or separator specifications. So even if the default separator does include a space
	/// character that is used at format time, both input string "{1;1}" and
	/// " { 1 ; 1 } " will be parsed without error and the same vector will be
	/// returned. In the second case, however, the parse position after parsing will be
	/// just after the closing curly brace, i.e. just before the trailing space.</p>
	/// <p><b>Note:</b> using "," as a separator may interfere with the grouping separator
	/// of the default <seealso cref="NumberFormat"/> for the current locale. Thus it is advised
	/// to use a <seealso cref="NumberFormat"/> instance with disabled grouping in such a case.</p>
	/// 
	/// @version $Id: Vector2DFormat.java 1467801 2013-04-14 16:19:33Z tn $
	/// @since 3.0
	/// </summary>
	public class Vector2DFormat : VectorFormat<Euclidean2D>
	{

		/// <summary>
		/// Create an instance with default settings.
		/// <p>The instance uses the default prefix, suffix and separator:
		/// "{", "}", and "; " and the default number format for components.</p>
		/// </summary>
		public Vector2DFormat() : base(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with a custom number format for components. </summary>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2DFormat(final java.text.NumberFormat format)
		public Vector2DFormat(NumberFormat format) : base(DEFAULT_PREFIX, DEFAULT_SUFFIX, DEFAULT_SEPARATOR, format)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix and separator. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="separator"> separator to use instead of the default "; " </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2DFormat(final String prefix, final String suffix, final String separator)
		public Vector2DFormat(string prefix, string suffix, string separator) : base(prefix, suffix, separator, CompositeFormat.DefaultNumberFormat)
		{
		}

		/// <summary>
		/// Create an instance with custom prefix, suffix, separator and format
		/// for components. </summary>
		/// <param name="prefix"> prefix to use instead of the default "{" </param>
		/// <param name="suffix"> suffix to use instead of the default "}" </param>
		/// <param name="separator"> separator to use instead of the default "; " </param>
		/// <param name="format"> the custom format for components. </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Vector2DFormat(final String prefix, final String suffix, final String separator, final java.text.NumberFormat format)
		public Vector2DFormat(string prefix, string suffix, string separator, NumberFormat format) : base(prefix, suffix, separator, format)
		{
		}

		/// <summary>
		/// Returns the default 2D vector format for the current locale. </summary>
		/// <returns> the default 2D vector format. </returns>
		public static Vector2DFormat Instance
		{
			get
			{
				return getInstance(Locale.Default);
			}
		}

		/// <summary>
		/// Returns the default 2D vector format for the given locale. </summary>
		/// <param name="locale"> the specific locale used by the format. </param>
		/// <returns> the 2D vector format specific to the given locale. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static Vector2DFormat getInstance(final java.util.Locale locale)
		public static Vector2DFormat getInstance(Locale locale)
		{
			return new Vector2DFormat(CompositeFormat.getDefaultNumberFormat(locale));
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public StringBuffer format(final mathlib.geometry.Vector<Euclidean2D> vector, final StringBuffer toAppendTo, final java.text.FieldPosition pos)
		public override StringBuilder format(Vector<Euclidean2D> vector, StringBuilder toAppendTo, FieldPosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Vector2D p2 = (Vector2D) vector;
			Vector2D p2 = (Vector2D) vector;
			return format(toAppendTo, pos, p2.X, p2.Y);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Vector2D parse(final String source) throws mathlib.exception.MathParseException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
		public override Vector2D parse(string source)
		{
			ParsePosition parsePosition = new ParsePosition(0);
			Vector2D result = parse(source, parsePosition);
			if (parsePosition.Index == 0)
			{
				throw new MathParseException(source, parsePosition.ErrorIndex, typeof(Vector2D));
			}
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: @Override public Vector2D parse(final String source, final java.text.ParsePosition pos)
		public override Vector2D parse(string source, ParsePosition pos)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] coordinates = parseCoordinates(2, source, pos);
			double[] coordinates = parseCoordinates(2, source, pos);
			if (coordinates == null)
			{
				return null;
			}
			return new Vector2D(coordinates[0], coordinates[1]);
		}

	}

}