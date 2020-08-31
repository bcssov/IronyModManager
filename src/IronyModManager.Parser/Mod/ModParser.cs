// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="ModParser.cs" company="Mario">
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
using IronyModManager.DI;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModParser" />
    public class ModParser : IModParser
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
        public ModParser(ICodeParser codeParser)
        {
            this.codeParser = codeParser;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the specified lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IModObject.</returns>
        public IModObject Parse(IEnumerable<string> lines)
        {
            var obj = DIResolver.Get<IModObject>();
            var data = codeParser.ParseScriptWithoutValidation(lines);
            if (data.Values?.Count() > 0)
            {
                obj.ReplacePath = GetValue<string>(data.Values, "replace_path");
                obj.UserDir = GetValue<string>(data.Values, "user_dir");
                obj.FileName = GetValue<string>(data.Values, "path", "archive");
                obj.Picture = GetValue<string>(data.Values, "picture");
                obj.Name = GetValue<string>(data.Values, "name");
                obj.Version = GetValue<string>(data.Values, "supported_version", "version");
                obj.Tags = GetValues<string>(data.Values, "tags");
                obj.RemoteId = GetValue<long?>(data.Values, "remote_file_id");
                obj.Dependencies = GetValues<string>(data.Values, "dependencies");
            }
            return obj;
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        private T Convert<T>(string value)
        {
            value = value.Replace("\"", string.Empty);
            var converter = GetConverter<T>();
            return converter.IsValid(value) ? (T)converter.ConvertFromString(value) : default;
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>TypeConverter.</returns>
        private TypeConverter GetConverter<T>()
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
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        private T GetValue<T>(IEnumerable<IScriptElement> elements, params string[] keys)
        {
            foreach (var key in keys)
            {
                var value = elements.FirstOrDefault(p => p.Key == key);
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
        private IEnumerable<T> GetValues<T>(IEnumerable<IScriptElement> elements, params string[] keys)
        {
            var type = typeof(List<>).MakeGenericType(typeof(T));
            var result = (IList)Activator.CreateInstance(type);
            foreach (var key in keys)
            {
                var value = elements.FirstOrDefault(p => p.Key == key);
                if (value != null && value.Values.Count() > 0)
                {
                    foreach (var item in value.Values)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Key))
                        {
                            result.Add(Convert<T>(item.Key));
                        }
                    }
                }
            }
            if (result.Count > 0)
            {
                return (IEnumerable<T>)result;
            }
            return null;
        }

        #endregion Methods
    }
}
