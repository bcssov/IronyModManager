// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
// ***********************************************************************
// <copyright file="MaterialDarkTheme.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Platform.Themes;
using Material.Colors;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace IronyModManager.Implementation.Themes
{
    /// <summary>
    /// Class MaterialDarkTheme.
    /// Implements the <see cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    public class MaterialDarkTheme : BaseThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the color of the background HTML.
        /// </summary>
        /// <value>The color of the background HTML.</value>
        public override string BackgroundHtmlColor => "#FF303030";

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
        public override IReadOnlyCollection<string> Styles => new List<string>
        {
            "avares://ThemeEditor.Controls.ColorPicker/ColorPicker.axaml",
            "avares://IronyModManager/Controls/Themes/MaterialDark/Theme.axaml",
            "avares://Material.Icons.Avalonia/App.xaml",
            "avares://IronyModManager/Controls/Themes/MaterialDark/ThemeOverride.axaml"
        };

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public override string ThemeName => Services.Common.Constants.Themes.MaterialDark.Name;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers this instance.
        /// </summary>
        public override void Register()
        {
            base.Register();
            RegisterResources();
        }

        /// <summary>
        /// Registers the resources.
        /// </summary>
        protected virtual void RegisterResources()
        {
            var helper = new PaletteHelper();
            var theme = Theme.Create(BaseThemeMode.Dark.GetBaseTheme(), SwatchHelper.Lookup[MaterialColor.Blue800], SwatchHelper.Lookup[MaterialColor.LightBlue800]);
            helper.SetTheme(theme);
        }

        #endregion Methods
    }
}
