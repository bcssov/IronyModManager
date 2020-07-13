// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-22-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2020
// ***********************************************************************
// <copyright file="DIPackage.Configuration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Implementation;
using IronyModManager.Localization.ResourceProviders;
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
        #region Methods

        /// <summary>
        /// Registers the configurations.
        /// </summary>
        /// <param name="container">The container.</param>
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
        }

        #endregion Methods
    }
}
