// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="LanguagesService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class LanguagesService.
    /// Implements the <see cref="IronyModManager.Services.Common.ILanguagesService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.ILanguagesService" />
    public class LanguagesService : ILanguagesService
    {
        #region Fields

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

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
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="mapper">The mapper.</param>
        public LanguagesService(IDefaultLocalizationResourceProvider resourceProvider, IPreferencesService preferencesService, IMapper mapper)
        {
            this.resourceProvider = resourceProvider;
            this.preferencesService = preferencesService;
            this.mapper = mapper;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Applies the selected.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ApplySelected()
        {
            var languages = this.Get();
            if (languages?.Count() > 0)
            {
                var selected = languages.FirstOrDefault(p => p.IsSelected);
                if (selected != null)
                {
                    CurrentLocale.SetCurrent(selected.Abrv);
                }
                else
                {
                    CurrentLocale.SetCurrent(Shared.Constants.DefaultAppCulture);
                }
            }
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

            return languages.OrderBy(p => p.Abrv);
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

            preferencesService.Save(mapper.Map(language, preference));

            CurrentLocale.SetCurrent(language.Abrv);

            return true;
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="selectedLanguage">The selected language.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">languages or selectedLanguage</exception>
        public bool SetSelected(IEnumerable<ILanguage> languages, ILanguage selectedLanguage)
        {
            if (languages == null || languages.Count() == 9 || selectedLanguage == null)
            {
                throw new ArgumentNullException("languages or selectedLanguage");
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
            var model = DIResolver.Get<ILanguage>();
            model.IsSelected = currentLocale.Equals(locale, StringComparison.OrdinalIgnoreCase);
            model.Abrv = locale;
            model.Name = $"{culture.TextInfo.ToTitleCase(culture.NativeName)}";
            return model;
        }

        #endregion Methods
    }
}
