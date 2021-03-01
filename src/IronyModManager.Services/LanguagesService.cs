// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2021
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
        /// The character regex
        /// </summary>
        private static readonly ConcurrentDictionary<string, Regex> charRegex = new ConcurrentDictionary<string, Regex>();

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
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public LanguagesService(ILocalizationManager localizationManager, IDefaultLocalizationResourceProvider resourceProvider,
            IPreferencesService preferencesService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.resourceProvider = resourceProvider;
            this.preferencesService = preferencesService;
            this.localizationManager = localizationManager;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Applies the selected.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool ApplySelected()
        {
            var language = GetSelected();
            bool result = false;
            if (language != null)
            {
                CurrentLocale.SetCurrent(language.Abrv);
                result = true;
            }
            else
            {
                CurrentLocale.SetCurrent(Shared.Constants.DefaultAppCulture);
            }
            return result;
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

            return languages.OrderBy(p => p.Abrv);
        }

        /// <summary>
        /// Gets the language by supported name block.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>ILanguage.</returns>
        public virtual ILanguage GetLanguageBySupportedNameBlock(string text)
        {
            var languages = Get().Where(p => !string.IsNullOrWhiteSpace(p.SupportedNameBlock));
            if (languages.Any())
            {
                foreach (var item in languages)
                {
                    if (!charRegex.TryGetValue(item.Abrv, out var regex))
                    {
                        regex = new Regex($"\\p{{{item.SupportedNameBlock}}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        charRegex.TryAdd(item.Abrv, regex);
                    }
                    if (text.Any(c => regex.IsMatch(c.ToString())))
                    {
                        return item;
                    }
                }
            }
            return null;
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
        /// <exception cref="InvalidOperationException">Language not selected.</exception>
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
        /// <exception cref="ArgumentNullException"></exception>
        public virtual bool SetSelected(IEnumerable<ILanguage> languages, ILanguage selectedLanguage)
        {
            if (languages == null || !languages.Any() || selectedLanguage == null)
            {
                throw new ArgumentNullException($"{nameof(languages)} or {nameof(selectedLanguage)}.");
            }

            var currentLanguage = GetSelected();
            if (currentLanguage.Abrv == selectedLanguage.Abrv)
            {
                return false;
            }

            foreach (var item in languages)
            {
                if (item.Abrv != selectedLanguage.Abrv)
                {
                    item.IsSelected = false;
                }
            }

            selectedLanguage.IsSelected = true;
            return Save(selectedLanguage);
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
