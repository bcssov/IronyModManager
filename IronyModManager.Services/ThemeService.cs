// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
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
using IronyModManager.Models;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ThemeService.
    /// </summary>
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

            var lightTheme = InitTheme(Models.Enums.Theme.Light, preferences.Theme);
            var darkTheme = InitTheme(Models.Enums.Theme.Dark, preferences.Theme);
            var themes = new List<ITheme>() { lightTheme, darkTheme };

            return themes;
        }

        /// <summary>
        /// Saves the specified themes.
        /// </summary>
        /// <param name="themes">The themes.</param>
        /// <exception cref="InvalidOperationException">No selected themes</exception>
        /// <exception cref="InvalidOperationException">Too many selected themes</exception>
        /// <exception cref="InvalidOperationException">No selected themes</exception>
        public void Save(IEnumerable<ITheme> themes)
        {
            if (!themes.Any(p => p.IsSelected))
            {
                throw new InvalidOperationException("No selected themes");
            }
            if (themes.Count(p => p.IsSelected) > 1)
            {
                throw new InvalidOperationException("Too many selected themes");
            }
            var selected = themes.FirstOrDefault(p => p.IsSelected);
            var preference = Mapper.Map<IPreferences>(selected);

            PreferencesService.Save(preference);
        }

        /// <summary>
        /// Initializes the theme.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="selectedType">Type of the selected.</param>
        /// <returns>ITheme.</returns>
        private ITheme InitTheme(Models.Enums.Theme type, Models.Enums.Theme selectedType)
        {
            var theme = DIResolver.Get<ITheme>();
            theme.Type = type;
            theme.Name = type == Models.Enums.Theme.Light ? "Light" : "Dark";
            theme.IsSelected = type == selectedType;
            return theme;
        }

        #endregion Methods
    }
}
