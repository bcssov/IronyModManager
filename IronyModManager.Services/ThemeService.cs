// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="ThemeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ThemeService.
    /// Implements the <see cref="IronyModManager.Services.IThemeService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.IThemeService" />
    public class ThemeService : IThemeService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeService" /> class.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="mapper">The mapper.</param>
        public ThemeService(IPreferencesService preferencesService, IMapper mapper)
        {
            PreferencesService = preferencesService;
            Mapper = mapper;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the mapper.
        /// </summary>
        /// <value>The mapper.</value>
        protected IMapper Mapper { get; private set; }

        /// <summary>
        /// Gets the preferences service.
        /// </summary>
        /// <value>The preferences service.</value>
        protected IPreferencesService PreferencesService { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ITheme&gt;.</returns>
        public IEnumerable<ITheme> Get()
        {
            var preferences = PreferencesService.Get();

            var lightTheme = InitModel(Models.Common.Enums.Theme.Light, preferences.Theme);
            var darkTheme = InitModel(Models.Common.Enums.Theme.Dark, preferences.Theme);
            var themes = new List<ITheme>() { lightTheme, darkTheme };

            return themes;
        }

        /// <summary>
        /// Saves the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <exception cref="InvalidOperationException">Theme not selected.</exception>
        public void Save(ITheme theme)
        {
            if (!theme.IsSelected)
            {
                throw new InvalidOperationException("Theme not selected.");
            }
            var preference = Mapper.Map<IPreferences>(theme);

            PreferencesService.Save(preference);
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="selectedType">Type of the selected.</param>
        /// <returns>ITheme.</returns>
        protected virtual ITheme InitModel(Models.Common.Enums.Theme type, Models.Common.Enums.Theme selectedType)
        {
            var theme = DIResolver.Get<ITheme>();
            theme.Type = type;
            theme.IsSelected = type == selectedType;
            return theme;
        }

        #endregion Methods
    }
}
