// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="DarkTheme.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Platform.Themes;

namespace IronyModManager.Implementation.Themes
{
    /// <summary>
    /// Class DarkTheme.
    /// Implements the <see cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    public class DarkTheme : BaseThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the color of the background HTML.
        /// </summary>
        /// <value>The color of the background HTML.</value>
        public override string BackgroundHtmlColor => "#FFDEDEDE";

        /// <summary>
        /// Gets the color of the foreground HTML.
        /// </summary>
        /// <value>The color of the foreground HTML.</value>
        public override string ForegroundHtmlColor => "#D2D2D2";

        /// <summary>
        /// Gets a value indicating whether this instance is light theme.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme; otherwise, <c>false</c>.</value>
        public override bool IsLightTheme => false;

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        public override IReadOnlyCollection<string> Styles => new List<string>() { "avares://Avalonia.Themes.Default/DefaultTheme.xaml", "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml", "avares://IronyModManager/Controls/Themes/Dark/ThemeOverride.xaml" };

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public override string ThemeName => Services.Common.Constants.Themes.Dark.Name;

        #endregion Properties
    }
}
