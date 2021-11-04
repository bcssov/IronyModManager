// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="IThemeResources.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Platform.Themes
{
    /// <summary>
    /// Interface IThemeResources
    /// </summary>
    public interface IThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the color of the background HTML.
        /// </summary>
        /// <value>The color of the background HTML.</value>
        public string BackgroundHtmlColor { get; }

        /// <summary>
        /// Gets the color of the foreground HTML.
        /// </summary>
        /// <value>The color of the foreground HTML.</value>
        public string ForegroundHtmlColor { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is light theme.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme; otherwise, <c>false</c>.</value>
        bool IsLightTheme { get; }

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        IReadOnlyCollection<string> Styles { get; }

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        string ThemeName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the HTML base CSS.
        /// </summary>
        /// <param name="additionalStyles">The additional styles.</param>
        /// <returns>System.String.</returns>
        string GetHtmlBaseCSS(string additionalStyles);

        /// <summary>
        /// Registers this instance.
        /// </summary>
        void Register();

        #endregion Methods
    }
}
