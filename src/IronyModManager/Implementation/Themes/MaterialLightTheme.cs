// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="MaterialLightTheme.cs" company="Mario">
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
    /// Class MaterialLightTheme.
    /// Implements the <see cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.BaseThemeResources" />
    public class MaterialLightTheme : BaseThemeResources
    {
        #region Properties

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        public override IReadOnlyCollection<string> Styles => new List<string>() { "avares://IronyModManager/Controls/Themes/MaterialLight/Theme.axaml", "avares://IronyModManager/Controls/Themes/MaterialLight/ThemeOverride.axaml" };

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public override string ThemeName => Services.Common.Constants.Themes.MaterialLight.Name;

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
            var theme = Theme.Create(BaseThemeMode.Dark.GetBaseTheme(), SwatchHelper.Lookup[MaterialColor.Blue], SwatchHelper.Lookup[MaterialColor.Teal400]);
            helper.SetTheme(theme);
        }

        #endregion Methods
    }
}
