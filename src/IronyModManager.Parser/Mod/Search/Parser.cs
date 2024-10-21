// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 10-26-2021
//
// Last Modified By : Mario
// Last Modified On : 10-18-2024
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
using IronyModManager.Shared.Cache;
using DescriptorPropertyAttribute = IronyModManager.Parser.Common.Mod.Search.DescriptorPropertyAttribute;

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
        /// The cache prefix
        /// </summary>
        private const string CachePrefix = "SearchParserEntry-";

        /// <summary>
        /// The cache region
        /// </summary>
        private const string CacheRegion = "SearchParserRegion";

        /// <summary>
        /// The maximum records to cache
        /// </summary>
        private const int MaxRecordsToCache = 1000;

        /// <summary>
        /// The name property
        /// </summary>
        private const string NameProperty = "name";

        /// <summary>
        /// The search parser properties
        /// </summary>
        private static IEnumerable<PropertyInfo> searchParserProperties;

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache;

        /// <summary>
        /// The converters
        /// </summary>
        private readonly IEnumerable<ITypeConverter<object>> converters;

        /// <summary>
        /// The localization registry
        /// </summary>
        private readonly ILocalizationRegistry localizationRegistry;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="converters">The converters.</param>
        /// <param name="localizationRegistry">The localization registry.</param>
        public Parser(ICache cache, ILogger logger, IEnumerable<ITypeConverter<object>> converters, ILocalizationRegistry localizationRegistry)
        {
            this.logger = logger;
            this.converters = converters;
            this.localizationRegistry = localizationRegistry;
            this.cache = cache;
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
            var cacheKey = GetCacheKey(text);
            var cacheEntry = cache.Get<CacheEntry>(new CacheGetParameters { Region = CacheRegion, Key = cacheKey });
            if (cacheEntry != null)
            {
                return cacheEntry.SearchResult;
            }

            var orStatementSeparator = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterOperators.OrStatementSeparator);
            var valueSeparator = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterOperators.ValueSeparator);
            var statementSeparator = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterOperators.StatementSeparator);
            var negateOperator = localizationRegistry.GetTranslation(locale, LocalizationResources.FilterOperators.Negate);

            IList<NameFilterResult> parseOrStatements(string text)
            {
                var list = new List<NameFilterResult>();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (text.Contains(orStatementSeparator))
                    {
                        var split = text.Split(orStatementSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
                        split.ForEach(x =>
                        {
                            var negate = x.Trim().StartsWith(negateOperator);
                            list.Add(new NameFilterResult(x.Trim().TrimStart(negateOperator).Trim()) { Negate = negate });
                        });
                    }
                    else
                    {
                        var negate = text.Trim().StartsWith(negateOperator);
                        list.Add(new NameFilterResult(text.Trim().TrimStart(negateOperator).Trim()) { Negate = negate });
                    }
                }

                return list;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                var result = DIResolver.Get<ISearchParserResult>();
                result.Name = parseOrStatements((text ?? string.Empty).ToLowerInvariant().Trim());
                cache.Set(new CacheAddParameters<CacheEntry> { MaxItems = MaxRecordsToCache, Region = CacheRegion, Key = cacheKey, Value = new CacheEntry(result) });
                return result;
            }

            try
            {
                var tokens = new Dictionary<string, string>();
                foreach (var item in text.ToLowerInvariant().Split(statementSeparator))
                {
                    var key = string.Empty;
                    var value = string.Empty;
                    var split = item.Split(valueSeparator);
                    if (split.Length == 2)
                    {
                        key = split[0].Trim();
                        value = split[1].Trim();
                    }
                    else
                    {
                        key = NameProperty;
                        value = item.Trim();
                    }

                    tokens.TryAdd(key, value);
                }

                var nothingSet = true;
                if (tokens.Count != 0)
                {
                    var result = DIResolver.Get<ISearchParserResult>();
                    foreach (var item in tokens)
                    {
                        var field = item.Key;
                        var converter = converters.FirstOrDefault(x =>
                        {
                            var result = x.CanConvert(locale, item.Key);
                            if (result is { Result: true })
                            {
                                field = result.MappedStaticField;
                                return true;
                            }

                            return false;
                        });
                        if (converter != null)
                        {
                            var property = GetProperties().FirstOrDefault(p => p.CanRead && ((DescriptorPropertyAttribute)Attribute.GetCustomAttribute(p, typeof(DescriptorPropertyAttribute), true))?.PropertyName == field);
                            if (property != null && property.CanWrite)
                            {
                                if (property.GetCustomAttribute(typeof(DescriptorPropertyAttribute), true) is DescriptorPropertyAttribute { IsList: true })
                                {
                                    var values = parseOrStatements(item.Value);
                                    var col = property.GetValue(result, null) as IList;
                                    foreach (var val in values)
                                    {
                                        var value = converter.Convert(locale, val.Text);
                                        ((BaseFilterResult)value).Negate = val.Negate;
                                        col?.Add(value);
                                    }
                                }
                                else
                                {
                                    var negate = item.Value.Trim().StartsWith(negateOperator);
                                    var parsedVal = item.Value.Trim().TrimStart(negateOperator).Trim();
                                    var value = converter.Convert(locale, parsedVal);
                                    ((BaseFilterResult)value).Negate = negate;
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
                        cache.Set(new CacheAddParameters<CacheEntry> { MaxItems = MaxRecordsToCache, Region = CacheRegion, Key = cacheKey, Value = new CacheEntry(result) });
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
            cache.Set(new CacheAddParameters<CacheEntry> { MaxItems = MaxRecordsToCache, Region = CacheRegion, Key = cacheKey, Value = new CacheEntry(emptyResult) });
            return emptyResult;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns>IEnumerable&lt;PropertyInfo&gt;.</returns>
        private static IEnumerable<PropertyInfo> GetProperties()
        {
            searchParserProperties ??= typeof(ISearchParserResult).GetProperties().Where(p => Attribute.IsDefined(p, typeof(DescriptorPropertyAttribute)));
            return searchParserProperties;
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>System.String.</returns>
        private string GetCacheKey(string text)
        {
            text ??= string.Empty;
            return $"{CachePrefix}{text.Replace(" ", string.Empty)}";
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class CacheEntry.
        /// </summary>
        private class CacheEntry
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheEntry" /> class.
            /// </summary>
            /// <param name="searchResult">The search result.</param>
            public CacheEntry(ISearchParserResult searchResult)
            {
                SearchResult = searchResult;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets or sets the search result.
            /// </summary>
            /// <value>The search result.</value>
            public ISearchParserResult SearchResult { get; }

            #endregion Properties
        }

        #endregion Classes
    }
}
