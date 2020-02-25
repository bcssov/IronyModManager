// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-07-2020
//
// Last Modified By : Mario
// Last Modified On : 02-24-2020
// ***********************************************************************
// <copyright file="ThemeRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
            storage.RegisterTheme(Constants.Themes.Light.Name, new List<string> { Constants.Themes.Light.MainResource, Constants.Themes.Light.AccentResource }, true);
            storage.RegisterTheme(Constants.Themes.Dark.Name, new List<string> { Constants.Themes.Dark.MainResource, Constants.Themes.Dark.AccentResource });
            storage.RegisterTheme(Constants.Themes.MaterialDark.Name, new List<string> { Constants.Themes.MaterialDark.MainResource, Constants.Themes.MaterialDark.AccentResource });
            storage.RegisterTheme(Constants.Themes.MaterialLightGreen.Name, new List<string> { Constants.Themes.MaterialLightGreen.MainResource, Constants.Themes.MaterialLightGreen.AccentResource });
            storage.RegisterTheme(Constants.Themes.MaterialDeepPurple.Name, new List<string> { Constants.Themes.MaterialDeepPurple.MainResource, Constants.Themes.MaterialDeepPurple.AccentResource });
        }

        #endregion Methods
    }
}
