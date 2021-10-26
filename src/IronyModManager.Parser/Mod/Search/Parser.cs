// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="Parser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
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
        /// The statement separator
        /// </summary>
        private const string StatementSeparator = "&&";

        /// <summary>
        /// The value separator
        /// </summary>
        private const string ValueSeparator = ":";

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
            if (string.IsNullOrWhiteSpace(text))
            {
                var result = DIResolver.Get<ISearchParserResult>();
                result.Name = (text ?? string.Empty).ToLowerInvariant();
                return result;
            }
            try
            {
                var nameAdded = false;
                var tokens = text.ToLowerInvariant().Split(StatementSeparator).ToDictionary(k =>
                {
                    if (k.Split(ValueSeparator).Length > 1)
                    {
                        return k.Split(ValueSeparator)[0].Trim();
                    }
                    else
                    {
                        if (nameAdded)
                        {
                            return k.Trim();
                        }
                        nameAdded = true;
                        return NameProperty;
                    }
                }, v => v.Split(ValueSeparator).Length > 1 ? v.Split(ValueSeparator)[1].Trim() : v.Trim());
                if (tokens.Any())
                {
                    var result = DIResolver.Get<ISearchParserResult>();
                    foreach (var item in tokens)
                    {
                        object value;
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
                            value = converter.Convert(locale, item.Value);
                            var property = GetProperties().FirstOrDefault(p => p.CanRead && ((Common.Mod.Search.DescriptorPropertyAttribute)Attribute.GetCustomAttribute(p, typeof(Common.Mod.Search.DescriptorPropertyAttribute), true)).PropertyName == field);
                            if (property != null && property.CanWrite)
                            {
                                property.SetValue(result, value);
                            }
                        }
                        else
                        {
                            result.Name = item.Value;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(result.Name))
                    {
                        result.Name = string.Empty;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            var emptyResult = DIResolver.Get<ISearchParserResult>();
            emptyResult.Name = (text ?? string.Empty).ToLowerInvariant();
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
