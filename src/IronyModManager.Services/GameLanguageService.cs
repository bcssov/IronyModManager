// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-25-2024
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="GameLanguageService.cs" company="Mario">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// The game language service.
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IGameLanguageService" />
    /// <param name="storageProvider">The storage provider.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="preferencesService">The preferences service.</param>
    /// <remarks>Initializes a new instance of the <see cref="GameLanguageService" /> class.</remarks>
    public class GameLanguageService(IStorageProvider storageProvider, IMapper mapper, IPreferencesService preferencesService) : BaseService(storageProvider, mapper), IGameLanguageService
    {
        #region Fields

        /// <summary>
        /// All selected
        /// </summary>
        private const string AllSelected = "all";

        /// <summary>
        /// A private string named defaultLanguage.
        /// </summary>
        private const string DefaultLanguage = "l_default";

        /// <summary>
        /// A private static object named objLock.
        /// </summary>
        private static readonly object objLock = new();

        /// <summary>
        /// A private readonly IPreferencesService named preferencesService.
        /// </summary>
        private readonly IPreferencesService preferencesService = preferencesService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Get.
        /// </summary>
        /// <returns>A list of IGameLanguages.<see cref="T:System.Collections.Generic.IEnumerable`1" /></returns>
        public IEnumerable<IGameLanguage> Get()
        {
            var result = new List<IGameLanguage>();
            var selectedLanguages = preferencesService.Get().ConflictSolverLanguages;
            var locales = Parser.Common.Constants.Localization.Locales.Where(p => p != DefaultLanguage);
            foreach (var locale in locales)
            {
                var model = GetModelInstance<IGameLanguage>();
                InitModel(model, locale, selectedLanguages);
                result.Add(model);
            }

            return result;
        }

        /// <summary>
        /// Save.
        /// </summary>
        /// <param name="languages">The languages.</param>
        public void Save(IEnumerable<IGameLanguage> languages)
        {
            lock (objLock)
            {
                var preferences = preferencesService.Get();
                preferences.ConflictSolverLanguages = languages.Where(p => p.IsSelected).Select(p => p.Type).ToList();
                preferencesService.Save(preferences);
            }
        }

        /// <summary>
        /// Init model.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="type">The type.</param>
        /// <param name="selected">A list of strings</param>
        private void InitModel(IGameLanguage language, string type, List<string> selected)
        {
            language.Type = type;
            language.IsSelected = selected.Contains(AllSelected) || selected.Contains(type);
            language.DisplayName = type;
        }

        #endregion Methods
    }
}
