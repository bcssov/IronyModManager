// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-07-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
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

namespace IronyModManager.Services
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
            storage.RegisterTheme("Light", new List<string> { "avares://Avalonia.Themes.Default/DefaultTheme.xaml", "avares://Avalonia.Themes.Default/Accents/BaseLight.xaml" }, true);
            storage.RegisterTheme("Dark", new List<string> { "avares://Avalonia.Themes.Default/DefaultTheme.xaml", "avares://Avalonia.Themes.Default/Accents/BaseDark.xaml" });
            storage.RegisterTheme("MaterialDark", new List<string> { "avares://Material.Avalonia/Material.Avalonia.Templates.xaml", "avares://Material.Avalonia/Material.Avalonia.Dark.xaml" });
            storage.RegisterTheme("MaterialLightGreen", new List<string> { "avares://Material.Avalonia/Material.Avalonia.Templates.xaml", "avares://Material.Avalonia/Material.Avalonia.LightGreen.xaml" });
            storage.RegisterTheme("MaterialDeepPurple", new List<string> { "avares://Material.Avalonia/Material.Avalonia.Templates.xaml", "avares://Material.Avalonia/Material.Avalonia.DeepPurple.xaml" });
        }

        #endregion Methods
    }
}
