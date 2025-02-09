// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2025
// ***********************************************************************
// <copyright file="LanguagesService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using IronyModManager.Localization;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class LanguagesService.
    /// Implements the <see cref="IronyModManager.Services.Common.ILanguagesService" />
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.ILanguagesService" />
    public class LanguagesService : BaseService, ILanguagesService
    {
        #region Fields

        /// <summary>
        /// The font region
        /// </summary>
        private const string FontRegion = "Font";

        /// <summary>
        /// The maximum cached font records
        /// </summary>
        private const int MaxCachedFontRecords = 5000;

        /// <summary>
        /// The character regex
        /// </summary>
        private static readonly ConcurrentDictionary<string, Regex> charRegex = new();

        /// <summary>
        /// The cache
        /// </summary>
        private readonly ICache cache;

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The preferences service
        /// </summary>
        private readonly IPreferencesService preferencesService;

        /// <summary>
        /// The resource provider
        /// </summary>
        private readonly IDefaultLocalizationResourceProvider resourceProvider;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguagesService" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public LanguagesService(ICache cache, ILocalizationManager localizationManager, IDefaultLocalizationResourceProvider resourceProvider,
            IPreferencesService preferencesService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.resourceProvider = resourceProvider;
            this.preferencesService = preferencesService;
            this.localizationManager = localizationManager;
            this.cache = cache;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Applies the selected.
        /// </summary>
        /// <returns><c>true</c> if applied, <c>false</c> otherwise.</returns>
        public virtual bool ApplySelected()
        {
            var language = GetSelected();
            CurrentLocale.SetCurrent(language.Abrv);
            return true;
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ILanguage&gt;.</returns>
        public virtual IEnumerable<ILanguage> Get()
        {
            var preferences = preferencesService.Get();
            var locales = resourceProvider.GetAvailableLocales();
            var currentLocale = !string.IsNullOrWhiteSpace(preferences.Locale) ? preferences.Locale : CurrentLocale.CultureName;
            var languages = new List<ILanguage>();

            foreach (var locale in locales)
            {
                languages.Add(InitModel(currentLocale, locale));
            }

            EnsureSelected(languages);

            return languages.OrderBy(p => p.Abrv);
        }

        /// <summary>
        /// Gets the language by supported name block.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>ILanguage.</returns>
        public virtual ILanguage GetLanguageBySupportedNameBlock(string text)
        {
            var all = Get();
            var selectedLanguage = all.FirstOrDefault(p => p.IsSelected);
            var languages = all.Where(p => !string.IsNullOrWhiteSpace(p.SupportedNameBlock) && p != selectedLanguage);
            var cached = cache.Get<string>(new CacheGetParameters { Region = ConstructFontRegionCacheKey(selectedLanguage), Key = text });
            if (!string.IsNullOrWhiteSpace(cached))
            {
                return all.FirstOrDefault(p => p.Abrv.Equals(cached));
            }
            else if (languages.Any())
            {
                foreach (var item in languages)
                {
                    cached = cache.Get<string>(new CacheGetParameters { Region = ConstructFontRegionCacheKey(item), Key = text });
                    if (string.IsNullOrWhiteSpace(cached))
                    {
                        if (!charRegex.TryGetValue(item.Abrv, out var regex))
                        {
                            regex = new Regex($"\\p{{{item.SupportedNameBlock}}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            charRegex.TryAdd(item.Abrv, regex);
                        }

                        if (text.Any(c => regex.IsMatch(c.ToString())))
                        {
                            cache.Set(new CacheAddParameters<string> { Region = ConstructFontRegionCacheKey(item), Key = text, Value = item.Abrv, MaxItems = MaxCachedFontRecords });
                            return item;
                        }
                    }
                    else
                    {
                        return all.FirstOrDefault(p => p.Abrv.Equals(cached));
                    }
                }
            }

            cache.Set(new CacheAddParameters<string> { Region = ConstructFontRegionCacheKey(selectedLanguage), Key = text, Value = selectedLanguage?.Abrv, MaxItems = MaxCachedFontRecords });
            return selectedLanguage;
        }

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>ILanguage.</returns>
        public virtual ILanguage GetSelected()
        {
            return Get().FirstOrDefault(p => p.IsSelected);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.InvalidOperationException">Language not selected.</exception>
        public virtual bool Save(ILanguage language)
        {
            if (!language.IsSelected)
            {
                throw new InvalidOperationException("Language not selected.");
            }

            var preference = preferencesService.Get();

            var result = preferencesService.Save(Mapper.Map(language, preference));

            CurrentLocale.SetCurrent(language.Abrv);

            return result;
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="selectedLanguage">The selected language.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public virtual bool SetSelected(IEnumerable<ILanguage> languages, ILanguage selectedLanguage)
        {
            if (languages == null || !languages.Any() || selectedLanguage == null)
            {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException($"{nameof(languages)} or {nameof(selectedLanguage)}.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }

            var currentLanguage = GetSelected();
            if (currentLanguage.Abrv == selectedLanguage.Abrv)
            {
                return false;
            }

            foreach (var item in languages)
            {
                item.IsSelected = item.Abrv == selectedLanguage.Abrv;
            }

            selectedLanguage.IsSelected = true;
            return Save(selectedLanguage);
        }

        /// <summary>
        /// Constructs the font cache key.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>string.</returns>
        protected virtual string ConstructFontRegionCacheKey(ILanguage language)
        {
            return $"{FontRegion}-{language.Abrv}";
        }

        /// <summary>
        /// Ensures the selected.
        /// </summary>
        /// <param name="languages">The languages.</param>
        protected virtual void EnsureSelected(IEnumerable<ILanguage> languages)
        {
            if (!languages.Any(p => p.IsSelected))
            {
                languages.FirstOrDefault(p => p.Abrv.Equals(Shared.Constants.DefaultAppCulture))!.IsSelected = true;
            }
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="currentLocale">The current locale.</param>
        /// <param name="locale">The locale.</param>
        /// <returns>ILanguage.</returns>
        protected virtual ILanguage InitModel(string currentLocale, string locale)
        {
            var culture = new CultureInfo(locale);
            var model = GetModelInstance<ILanguage>();
            model.IsSelected = currentLocale.Equals(locale, StringComparison.OrdinalIgnoreCase);
            model.Abrv = locale;
            model.Name = $"{culture.TextInfo.ToTitleCase(culture.NativeName)}";
            model.Font = localizationManager.GetResource(locale, LocalizationResources.App.FontFamily);
            model.SupportedNameBlock = localizationManager.GetResource(locale, LocalizationResources.App.SupportedNameBlock);
            return model;
        }

        #endregion Methods
    }
}
