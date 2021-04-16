// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-22-2020
//
// Last Modified By : Mario
// Last Modified On : 04-16-2021
// ***********************************************************************
// <copyright file="DIPackage.Configuration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Implementation;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Platform.Configuration;
using IronyModManager.Shared.Cache;
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(Constants.AppSettings, false)
                .Build();

            container.RegisterInstance(configuration);

            container.Collection.Register<ILocalizationResourceProvider>(typeof(LocalizationResourceProvider));
            container.Register<IDefaultLocalizationResourceProvider, LocalizationResourceProvider>();
            container.Register<ICache, Cache>(Lifestyle.Singleton);
            container.Register<IPlatformConfiguration, PlatformConfiguration>(Lifestyle.Singleton);
        }

        #endregion Methods
    }
}
