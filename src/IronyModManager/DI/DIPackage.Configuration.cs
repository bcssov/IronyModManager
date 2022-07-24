// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-22-2020
//
// Last Modified By : Mario
// Last Modified On : 07-24-2022
// ***********************************************************************
// <copyright file="DIPackage.Configuration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Config;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Platform.Configuration;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using SimpleInjector;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    public partial class DIPackage
    {
        /// <summary>
        /// Registers the configurations.
        /// </summary>
        /// <param name="container">The container.</param>

        #region Methods

        private void RegisterConfigurations(Container container)
        {
            var configurationLoader = new ConfigurationLoader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.AppSettings), Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.AppSettingsOverride));
            var configuration = new ConfigurationBuilder()
                .AddJsonStream(configurationLoader.GetStream())
                .Build();

            container.RegisterInstance(configuration);

            container.Collection.Register<ILocalizationResourceProvider>(typeof(LocalizationResourceProvider));
            container.Register<IDefaultLocalizationResourceProvider, LocalizationResourceProvider>();
            container.Register<ICache, Cache>(Lifestyle.Singleton);
            container.Register<IPlatformConfiguration, PlatformConfiguration>(Lifestyle.Singleton);
            container.Register<IDomainConfiguration, PlatformConfiguration>(Lifestyle.Singleton);
        }

        #endregion Methods
    }
}
