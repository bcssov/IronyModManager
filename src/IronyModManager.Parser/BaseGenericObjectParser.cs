﻿// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Nick Butcher
// Last Modified On : 12-12-2021
// ***********************************************************************
// <copyright file="BaseGenericObjectParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class BaseGenericObjectParser.
    /// </summary>
    public abstract class BaseGenericObjectParser
    {
        #region Fields

        /// <summary>
        /// The converters
        /// </summary>
        private static readonly ConcurrentDictionary<Type, TypeConverter> converters = new ConcurrentDictionary<Type, TypeConverter>();

        /// <summary>
        /// The text parser
        /// </summary>
        private readonly ICodeParser codeParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public BaseGenericObjectParser(ICodeParser codeParser)
        {
            this.codeParser = codeParser;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        protected static T Convert<T>(string value)
        {
            value = value.Trim().Trim('"').Replace("\\\"", "\"");
            var converter = GetConverter<T>();
            return converter.IsValid(value) ? (T)converter.ConvertFromString(value) : default;
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TypeConverter.</returns>
        protected static TypeConverter GetConverter<T>()
        {
            TypeConverter converter;
            if (converters.ContainsKey(typeof(T)))
            {
                converter = converters[typeof(T)];
            }
            else
            {
                converter = TypeDescriptor.GetConverter(typeof(T));
                converters.TryAdd(typeof(T), converter);
            }
            return converter;
        }

        /// <summary>
        /// Gets the keyed values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        protected static IEnumerable<T> GetKeyedValues<T>(IEnumerable<IScriptElement> elements, params string[] keys)
        {
            // One thing consistent about Paradox is that they're inconsistent
            var type = typeof(List<>).MakeGenericType(typeof(T));
            var result = (IList)Activator.CreateInstance(type);

            foreach (var key in keys)
            {
                var values = elements.Where(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (values.Any())
                {
                    foreach (var value in values)
                    {
                        if (!string.IsNullOrWhiteSpace(value.Value))
                        {
                            result.Add(Convert<T>(value.Value));
                        }
                    }
                }
                if (result.Count > 0)
                {
                    break;
                }
            }

            if (result.Count > 0)
            {
                return (IEnumerable<T>)result;
            }
            return null;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected static T GetValue<T>(IEnumerable<IScriptElement> elements, params string[] keys)
        {
            foreach (var key in keys)
            {
                var value = elements.FirstOrDefault(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (value != null && !string.IsNullOrWhiteSpace(value.Value))
                {
                    return Convert<T>(value.Value);
                }
            }
            return default;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected static IEnumerable<T> GetValues<T>(IEnumerable<IScriptElement> elements, params string[] keys)
        {
            var type = typeof(List<>).MakeGenericType(typeof(T));
            var result = (IList)Activator.CreateInstance(type);
            foreach (var key in keys)
            {
                var value = elements.FirstOrDefault(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (value?.Values?.Count() > 0)
                {
                    foreach (var item in value.Values)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Key))
                        {
                            result.Add(Convert<T>(item.Key));
                        }
                    }
                }
                if (result.Count > 0)
                {
                    break;
                }
            }
            if (result.Count > 0)
            {
                return (IEnumerable<T>)result;
            }
            return null;
        }

        /// <summary>
        /// Parses the code.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IParseResponse.</returns>
        protected virtual IParseResponse ParseCode(IEnumerable<string> lines)
        {
            var data = codeParser.ParseScriptWithoutValidation(lines);
            if (data.Error == null && data.Values?.Count() > 0)
            {
                return data;
            }
            return null;
        }

        #endregion Methods
    }
}
