// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
// ***********************************************************************
// <copyright file="LightTheme.cs" company="Mario">
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
    /// Class LightTheme.
    /// Implements the <see cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    public class LightTheme : BaseThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the color of the background HTML.
        /// </summary>
        /// <value>The color of the background HTML.</value>
        public override string BackgroundHtmlColor => "#FFFFFF";

        /// <summary>
        /// Gets the color of the foreground HTML.
        /// </summary>
        /// <value>The color of the foreground HTML.</value>
        public override string ForegroundHtmlColor => "#FF000000";

        /// <summary>
        /// Gets a value indicating whether this instance is light theme.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme; otherwise, <c>false</c>.</value>
        public override bool IsLightTheme => true;

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        public override IReadOnlyCollection<string> Styles => new List<string>
        {
            "avares://ThemeEditor.Controls.ColorPicker/ColorPicker.axaml",
            "avares://Avalonia.Themes.Default/DefaultTheme.xaml",
            "avares://Avalonia.Themes.Default/Accents/BaseLight.xaml",
            "avares://IronyModManager/Controls/Themes/Light/ThemeOverride.xaml"
        };

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public override string ThemeName => Services.Common.Constants.Themes.Light.Name;

        #endregion Properties
    }
}
