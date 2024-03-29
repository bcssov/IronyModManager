﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="ThemeManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Platform.Themes;
using IronyModManager.Services.Common;

namespace IronyModManager.Implementation.Themes
{
    /// <summary>
    /// Class ThemeManager.
    /// Implements the <see cref="IronyModManager.Platform.Themes.IThemeManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.IThemeManager" />
    public class ThemeManager : IThemeManager
    {
        #region Fields

        /// <summary>
        /// The theme resources
        /// </summary>
        private readonly IEnumerable<IThemeResources> themeResources;

        /// <summary>
        /// The theme service
        /// </summary>
        private readonly IThemeService themeService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeManager" /> class.
        /// </summary>
        /// <param name="themeService">The theme service.</param>
        /// <param name="themeResources">The theme resources.</param>
        public ThemeManager(IThemeService themeService, IEnumerable<IThemeResources> themeResources)
        {
            this.themeService = themeService;
            this.themeResources = themeResources;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Applies the theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        public void ApplyTheme(string theme)
        {
            ValidateThemes();
            var resource = themeResources.FirstOrDefault(p => p.ThemeName.Equals(theme));
            resource.Register();
        }

        /// <summary>
        /// Gets the HTML base CSS.
        /// </summary>
        /// <param name="additionalStyles">The additional styles.</param>
        /// <returns>System.String.</returns>
        public string GetHtmlBaseCSS(string additionalStyles)
        {
            ValidateThemes();
            var theme = themeService.GetSelected();
            var resource = themeResources.FirstOrDefault(p => p.ThemeName.Equals(theme.Type));
            return resource.GetHtmlBaseCSS(additionalStyles);
        }

        /// <summary>
        /// Determines whether [is light theme] [the specified theme].
        /// </summary>
        /// <param name="theme">The theme.</param>
        /// <returns><c>true</c> if [is light theme] [the specified theme]; otherwise, <c>false</c>.</returns>
        public bool IsLightTheme(string theme)
        {
            var resource = themeResources.FirstOrDefault(p => p.ThemeName.Equals(theme));
            return resource.IsLightTheme;
        }

        /// <summary>
        /// Validates the themes.
        /// </summary>
        /// <exception cref="ArgumentException">Some themes are missing theme resource registrations or vice versa.</exception>
        private void ValidateThemes()
        {
            var themes = themeService.Get();
            if (themeResources.GroupBy(p => p.ThemeName).Count() != themeResources.Count() || themeResources.Count() != themes.Count() || themeResources.Any(p => !themes.Any(t => t.Type.Equals(p.ThemeName))))
            {
                throw new ArgumentException("Some themes are missing theme resource registrations or vice versa.");
            }
        }

        #endregion Methods
    }
}
