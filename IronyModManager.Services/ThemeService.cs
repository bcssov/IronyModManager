// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
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

            var themeVals = Enum.GetValues(typeof(Models.Common.Enums.Theme)).Cast<Models.Common.Enums.Theme>();
            var themes = new List<ITheme>();

            foreach (var item in themeVals)
            {
                var theme = InitModel(item, preferences.Theme);
                themes.Add(theme);
            }

            return themes;
        }

        /// <summary>
        /// Gets the selected.
        /// </summary>
        /// <returns>ITheme.</returns>
        public ITheme GetSelected()
        {
            return Get().FirstOrDefault(p => p.IsSelected);
        }

        /// <summary>
        /// Saves the specified theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Theme not selected.</exception>
        public bool Save(ITheme theme)
        {
            if (!theme.IsSelected)
            {
                throw new InvalidOperationException("Theme not selected.");
            }
            var preference = PreferencesService.Get();

            return PreferencesService.Save(Mapper.Map(theme, preference));
        }

        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="themes">The themes.</param>
        /// <param name="selectedTheme">The selected theme.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">themes or selectedTheme</exception>
        public bool SetSelected(IEnumerable<ITheme> themes, ITheme selectedTheme)
        {
            if (themes == null || themes.Count() == 9 || selectedTheme == null)
            {
                throw new ArgumentNullException("themes or selectedTheme");
            }
            var currentlySelected = GetSelected();
            if (GetSelected().Type == selectedTheme.Type)
            {
                return false;
            }

            foreach (var item in themes)
            {
                if (item.Type != selectedTheme.Type)
                {
                    item.IsSelected = false;
                }
            }

            selectedTheme.IsSelected = true;
            return Save(selectedTheme);
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
            theme.StyleIncludes = InitStyles(type);
            return theme;
        }

        /// <summary>
        /// Initializes the styles.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        protected virtual IEnumerable<string> InitStyles(Models.Common.Enums.Theme type)
        {
            var styles = new List<string>();
            switch (type)
            {
                case Models.Common.Enums.Theme.Dark:
                    styles.Add("avares://Avalonia.Themes.Default/DefaultTheme.xaml");
                    styles.Add("avares://Avalonia.Themes.Default/Accents/BaseDark.xaml");

                    break;

                case Models.Common.Enums.Theme.MaterialDark:
                    styles.Add("avares://Material.Avalonia/Material.Avalonia.Templates.xaml");
                    styles.Add("avares://Material.Avalonia/Material.Avalonia.Dark.xaml");
                    break;

                case Models.Common.Enums.Theme.MaterialLightGreen:
                    styles.Add("avares://Material.Avalonia/Material.Avalonia.Templates.xaml");
                    styles.Add("avares://Material.Avalonia/Material.Avalonia.LightGreen.xaml");
                    break;

                case Models.Common.Enums.Theme.MaterialDeepPurple:
                    styles.Add("avares://Material.Avalonia/Material.Avalonia.Templates.xaml");
                    styles.Add("avares://Material.Avalonia/Material.Avalonia.DeepPurple.xaml");
                    break;

                default:
                    styles.Add("avares://Avalonia.Themes.Default/DefaultTheme.xaml");
                    styles.Add("avares://Avalonia.Themes.Default/Accents/BaseLight.xaml");
                    break;
            }
            return styles;
        }

        #endregion Methods
    }
}
