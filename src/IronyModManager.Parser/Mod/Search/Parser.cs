// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 07-13-2022
// ***********************************************************************
// <copyright file="Parser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Common.Mod.Search.Converter;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Mod.Search
{
    /// <summary>
    /// Class Parser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.Search.IParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.Search.IParser" />
    public class Parser : IParser
    {
        #region Fields

        /// <summary>
        /// The name property
        /// </summary>
        private const string NameProperty = "name";

        /// <summary>
        /// The or statement separator
        /// </summary>
        private const string OrStatementSeparator = "||";

        /// <summary>
        /// The statement separator
        /// </summary>
        private const string StatementSeparator = "&&";

        /// <summary>
        /// The value separator
        /// </summary>
        private const char ValueSeparator = ':';

        /// <summary>
        /// The search parser properties
        /// </summary>
        private static IEnumerable<PropertyInfo> searchParserProperties;

        /// <summary>
        /// The converters
        /// </summary>
        private readonly IEnumerable<ITypeConverter<object>> converters;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="converters">The converters.</param>
        public Parser(ILogger logger, IEnumerable<ITypeConverter<object>> converters)
        {
            this.logger = logger;
            this.converters = converters;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the specified text.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="text">The text.</param>
        /// <returns>ISearchParserResult.</returns>
        public ISearchParserResult Parse(string locale, string text)
        {
            IList<string> parseOrStatements(string text)
            {
                var list = new List<string>();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (text.Contains(OrStatementSeparator))
                    {
                        var split = text.Split(OrStatementSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
                        split.ForEach(x => list.Add(x.Trim()));
                    }
                    else
                    {
                        list.Add(text);
                    }
                }
                return list;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                var result = DIResolver.Get<ISearchParserResult>();
                result.Name = parseOrStatements((text ?? string.Empty).ToLowerInvariant().Trim());
                return result;
            }
            try
            {
                var tokens = new Dictionary<string, string>();
                foreach (var item in text.ToLowerInvariant().Split(StatementSeparator))
                {
                    string key = string.Empty;
                    string value = string.Empty;
                    if (item.Count(p => p == ValueSeparator) == 1)
                    {
                        var split = item.Split(ValueSeparator);
                        key = split[0].Trim();
                        value = split[1].Trim();
                    }
                    else
                    {
                        key = NameProperty;
                        value = item.Trim();
                    }
                    if (!tokens.ContainsKey(key))
                    {
                        tokens.Add(key, value);
                    }
                }
                var nothingSet = true;
                if (tokens.Any())
                {
                    var result = DIResolver.Get<ISearchParserResult>();
                    foreach (var item in tokens)
                    {
                        string field = item.Key;
                        var converter = converters.FirstOrDefault(x =>
                        {
                            var result = x.CanConvert(locale, item.Key);
                            if (result != null && result.Result)
                            {
                                field = result.MappedStaticField;
                                return true;
                            }
                            return false;
                        });
                        if (converter != null)
                        {
                            var property = GetProperties().FirstOrDefault(p => p.CanRead && ((Common.Mod.Search.DescriptorPropertyAttribute)Attribute.GetCustomAttribute(p, typeof(Common.Mod.Search.DescriptorPropertyAttribute), true)).PropertyName == field);
                            if (property != null && property.CanWrite)
                            {
                                var attribute = property.GetCustomAttribute(typeof(Common.Mod.Search.DescriptorPropertyAttribute), true) as Common.Mod.Search.DescriptorPropertyAttribute;
                                if (attribute.IsList)
                                {
                                    var values = parseOrStatements(item.Value);
                                    var col = property.GetValue(result, null) as IList;
                                    foreach (var val in values)
                                    {
                                        var value = converter.Convert(locale, val);
                                        col.Add(value);
                                    }
                                }
                                else
                                {
                                    var value = converter.Convert(locale, item.Value);
                                    property.SetValue(result, value);
                                }
                                nothingSet = false;
                            }
                        }
                        else
                        {
                            result.Name = parseOrStatements(item.Value);
                        }
                    }
                    if (!nothingSet)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            var emptyResult = DIResolver.Get<ISearchParserResult>();
            emptyResult.Name = parseOrStatements(text.ToLowerInvariant().Trim());
            return emptyResult;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns>IEnumerable&lt;PropertyInfo&gt;.</returns>
        private static IEnumerable<PropertyInfo> GetProperties()
        {
            if (searchParserProperties == null)
            {
                searchParserProperties = typeof(ISearchParserResult).GetProperties().Where(p => Attribute.IsDefined(p, typeof(Common.Mod.Search.DescriptorPropertyAttribute)));
            }
            return searchParserProperties;
        }

        #endregion Methods
    }
}
