// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-12-2021
// ***********************************************************************
// <copyright file="ThemeRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class ThemeRegistration.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    [ExcludeFromCoverage("Setup module.")]
    public class ThemeRegistration : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            var storage = DIResolver.Get<IStorageProvider>();
            storage.RegisterTheme(Constants.Themes.Light.Name, new List<string> { Constants.Themes.Light.MainResource, Constants.Themes.Light.AccentResource, Constants.Themes.Light.OverrideTheme }, true);
            storage.RegisterTheme(Constants.Themes.Dark.Name, new List<string> { Constants.Themes.Dark.MainResource, Constants.Themes.Dark.AccentResource, Constants.Themes.Dark.OverrideTheme });
            storage.RegisterTheme(Constants.Themes.FluentLight.Name, new List<string> { Constants.Themes.FluentLight.MainResource, Constants.Themes.FluentLight.Compact, Constants.Themes.FluentLight.OverrideTheme });
            storage.RegisterTheme(Constants.Themes.FluentDark.Name, new List<string> { Constants.Themes.FluentDark.MainResource, Constants.Themes.FluentDark.Compact, Constants.Themes.FluentDark.OverrideTheme });
        }

        #endregion Methods
    }
}
