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
namespace org.apache.commons.math3.exception.util
{


	/// <summary>
	/// Interface for localizable strings.
	/// 
	/// @version $Id: Localizable.java 1416643 2012-12-03 19:37:14Z tn $
	/// @since 2.2
	/// </summary>
	public interface Localizable : Serializable
	{
		/// <summary>
		/// Gets the source (non-localized) string.
		/// </summary>
		/// <returns> the source string. </returns>
		string SourceString {get;}

		/// <summary>
		/// Gets the localized string.
		/// </summary>
		/// <param name="locale"> locale into which to get the string. </param>
		/// <returns> the localized string or the source string if no
		/// localized version is available. </returns>
		string getLocalizedString(Locale locale);
	}

}