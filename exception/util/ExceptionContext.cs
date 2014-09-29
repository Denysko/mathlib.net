using System;
using System.Collections.Generic;
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
using mathlib.exception.util;

namespace mathlib.exception.util
{


    /// <summary>
    /// Class that contains the actual implementation of the functionality mandated
    /// by the <seealso cref="ExceptionContext"/> interface.
    /// All Commons Math exceptions delegate the interface's methods to this class.
    /// 
    /// @since 3.0
    /// @version $Id: ExceptionContext.java 1364388 2012-07-22 18:16:43Z tn $
    /// </summary>
    [Serializable]
    public class ExceptionContext
    {
        /// <summary>
        /// Serializable version Id. </summary>
        private const long serialVersionUID = -6024911025449780478L;
        /// <summary>
        /// The throwable to which this context refers to.
        /// </summary>
        private Exception throwable;
        /// <summary>
        /// Various informations that enrich the informative message.
        /// </summary>
        private IList<Localizable> msgPatterns;
        /// <summary>
        /// Various informations that enrich the informative message.
        /// The arguments will replace the corresponding place-holders in
        /// <seealso cref="#msgPatterns"/>.
        /// </summary>
        private IList<object[]> msgArguments;
        /// <summary>
        /// Arbitrary context information.
        /// </summary>
        private IDictionary<string, object> context;

        /// <summary>
        /// Simple constructor. </summary>
        /// <param name="throwable"> the exception this context refers too </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public ExceptionContext(final Throwable throwable)
        public ExceptionContext(Exception throwable)
        {
            this.throwable = throwable;
            msgPatterns = new List<Localizable>();
            msgArguments = new List<object[]>();
            context = new Dictionary<string, object>();
        }

        /// <summary>
        /// Get a reference to the exception to which the context relates. </summary>
        /// <returns> a reference to the exception to which the context relates </returns>
        public virtual Exception Throwable
        {
            get
            {
                return throwable;
            }
        }

        /// <summary>
        /// Adds a message.
        /// </summary>
        /// <param name="pattern"> Message pattern. </param>
        /// <param name="arguments"> Values for replacing the placeholders in the message
        /// pattern. </param>
        public virtual void addMessage(Localizable pattern, params object[] arguments)
        {
            msgPatterns.Add(pattern);
            msgArguments.Add(ArgUtils.flatten(arguments));
        }

        /// <summary>
        /// Sets the context (key, value) pair.
        /// Keys are assumed to be unique within an instance. If the same key is
        /// assigned a new value, the previous one will be lost.
        /// </summary>
        /// <param name="key"> Context key (not null). </param>
        /// <param name="value"> Context value. </param>
        public virtual void SetValue(string key, object value)
        {
            context[key] = value;
        }

        /// <summary>
        /// Gets the value associated to the given context key.
        /// </summary>
        /// <param name="key"> Context key. </param>
        /// <returns> the context value or {@code null} if the key does not exist. </returns>
        public virtual object GetValue(string key)
        {
            return context[key];
        }

        /// <summary>
        /// Gets all the keys stored in the exception
        /// </summary>
        /// <returns> the set of keys. </returns>
        public virtual Set<string> Keys
        {
            get
            {
                return context.Keys;
            }
        }

        /// <summary>
        /// Gets the default message.
        /// </summary>
        /// <returns> the message. </returns>
        public virtual string Message
        {
            get
            {
                return getMessage(Locale.US);
            }
        }

        /// <summary>
        /// Gets the message in the default locale.
        /// </summary>
        /// <returns> the localized message. </returns>
        public virtual string LocalizedMessage
        {
            get
            {
                return getMessage(Locale.Default);
            }
        }

        /// <summary>
        /// Gets the message in a specified locale.
        /// </summary>
        /// <param name="locale"> Locale in which the message should be translated. </param>
        /// <returns> the localized message. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public String getMessage(final java.util.Locale locale)
        public virtual string GetMessage(Locale locale)
        {
            return buildMessage(locale, ": ");
        }

        /// <summary>
        /// Gets the message in a specified locale.
        /// </summary>
        /// <param name="locale"> Locale in which the message should be translated. </param>
        /// <param name="separator"> Separator inserted between the message parts. </param>
        /// <returns> the localized message. </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
        //ORIGINAL LINE: public String getMessage(final java.util.Locale locale, final String separator)
        public virtual string GetMessage(Locale locale, string separator)
        {
            return buildMessage(locale, separator);
        }

        /// <summary>
        /// Builds a message string.
        /// </summary>
        /// <param name="locale"> Locale in which the message should be translated. </param>
        /// <param name="separator"> Message separator. </param>
        /// <returns> a localized message string. </returns>
        private string BuildMessage(Locale locale, string separator)
        {
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            int count = 0;
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int len = msgPatterns.size();
            int len = msgPatterns.Count;
            for (int i = 0; i < len; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Localizable pat = msgPatterns.get(i);
                Localizable pat = msgPatterns[i];
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object[] args = msgArguments.get(i);
                object[] args = msgArguments[i];
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final java.text.MessageFormat fmt = new java.text.MessageFormat(pat.getLocalizedString(locale), locale);
                MessageFormat fmt = new MessageFormat(pat.getLocalizedString(locale), locale);
                sb.Append(fmt.format(args));
                if (++count < len)
                {
                    // Add a separator if there are other messages.
                    sb.Append(separator);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serialize this object to the given stream.
        /// </summary>
        /// <param name="out"> Stream. </param>
        /// <exception cref="IOException"> This should never happen. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void writeObject(java.io.ObjectOutputStream out) throws java.io.IOException
        private void WriteObject(ObjectOutputStream @out)
        {
            @out.writeObject(throwable);
            serializeMessages(@out);
            serializeContext(@out);
        }
        /// <summary>
        /// Deserialize this object from the given stream.
        /// </summary>
        /// <param name="in"> Stream. </param>
        /// <exception cref="IOException"> This should never happen. </exception>
        /// <exception cref="ClassNotFoundException"> This should never happen. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void readObject(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
        private void ReadObject(ObjectInputStream @in)
        {
            throwable = (Exception)@in.readObject();
            deSerializeMessages(@in);
            deSerializeContext(@in);
        }

        /// <summary>
        /// Serialize  <seealso cref="#msgPatterns"/> and <seealso cref="#msgArguments"/>.
        /// </summary>
        /// <param name="out"> Stream. </param>
        /// <exception cref="IOException"> This should never happen. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void serializeMessages(java.io.ObjectOutputStream out) throws java.io.IOException
        private void SerializeMessages(ObjectOutputStream @out)
        {
            // Step 1.
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int len = msgPatterns.size();
            int len = msgPatterns.Count;
            @out.writeInt(len);
            // Step 2.
            for (int i = 0; i < len; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Localizable pat = msgPatterns.get(i);
                Localizable pat = msgPatterns[i];
                // Step 3.
                @out.writeObject(pat);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object[] args = msgArguments.get(i);
                object[] args = msgArguments[i];
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int aLen = args.length;
                int aLen = args.Length;
                // Step 4.
                @out.writeInt(aLen);
                for (int j = 0; j < aLen; j++)
                {
                    if (args[j] is Serializable)
                    {
                        // Step 5a.
                        @out.writeObject(args[j]);
                    }
                    else
                    {
                        // Step 5b.
                        @out.writeObject(nonSerializableReplacement(args[j]));
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize <seealso cref="#msgPatterns"/> and <seealso cref="#msgArguments"/>.
        /// </summary>
        /// <param name="in"> Stream. </param>
        /// <exception cref="IOException"> This should never happen. </exception>
        /// <exception cref="ClassNotFoundException"> This should never happen. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void deSerializeMessages(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
        private void DeSerializeMessages(ObjectInputStream @in)
        {
            // Step 1.
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int len = in.readInt();
            int len = @in.readInt();
            msgPatterns = new List<Localizable>(len);
            msgArguments = new List<object[]>(len);
            // Step 2.
            for (int i = 0; i < len; i++)
            {
                // Step 3.
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Localizable pat = (Localizable) in.readObject();
                Localizable pat = (Localizable)@in.readObject();
                msgPatterns.Add(pat);
                // Step 4.
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int aLen = in.readInt();
                int aLen = @in.readInt();
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object[] args = new Object[aLen];
                object[] args = new object[aLen];
                for (int j = 0; j < aLen; j++)
                {
                    // Step 5.
                    args[j] = @in.readObject();
                }
                msgArguments.Add(args);
            }
        }

        /// <summary>
        /// Serialize <seealso cref="#context"/>.
        /// </summary>
        /// <param name="out"> Stream. </param>
        /// <exception cref="IOException"> This should never happen. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void serializeContext(java.io.ObjectOutputStream out) throws java.io.IOException
        private void SerializeContext(ObjectOutputStream @out)
        {
            // Step 1.
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int len = context.keySet().size();
            int len = context.Keys.size();
            @out.writeInt(len);
            foreach (string key in context.Keys)
            {
                // Step 2.
                @out.writeObject(key);
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object value = context.get(key);
                object value = context[key];
                if (value is Serializable)
                {
                    // Step 3a.
                    @out.writeObject(value);
                }
                else
                {
                    // Step 3b.
                    @out.writeObject(nonSerializableReplacement(value));
                }
            }
        }

        /// <summary>
        /// Deserialize <seealso cref="#context"/>.
        /// </summary>
        /// <param name="in"> Stream. </param>
        /// <exception cref="IOException"> This should never happen. </exception>
        /// <exception cref="ClassNotFoundException"> This should never happen. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private void deSerializeContext(java.io.ObjectInputStream in) throws java.io.IOException, ClassNotFoundException
        private void DeSerializeContext(ObjectInputStream @in)
        {
            // Step 1.
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int len = in.readInt();
            int len = @in.readInt();
            context = new Dictionary<string, object>();
            for (int i = 0; i < len; i++)
            {
                // Step 2.
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final String key = (String) in.readObject();
                string key = (string)@in.readObject();
                // Step 3.
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Object value = in.readObject();
                object value = @in.readObject();
                context[key] = value;
            }
        }

        /// <summary>
        /// Replaces a non-serializable object with an error message string.
        /// </summary>
        /// <param name="obj"> Object that does not implement the {@code Serializable}
        /// interface. </param>
        /// <returns> a string that mentions which class could not be serialized. </returns>
        private string nonSerializableReplacement(object obj)
        {
            return "[Object could not be serialized: " + obj.GetType().Name + "]";
        }
    }

}