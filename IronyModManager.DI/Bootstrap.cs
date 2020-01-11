// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="Bootstrap.cs" company="IronyModManager.DI">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using IronyModManager.DI.Extensions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class Bootstrap.
    /// </summary>
    public static class Bootstrap
    {
        #region Fields

        /// <summary>
        /// The DLL extension
        /// </summary>
        private const string dllExtension = ".dll";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        public static void Finish()
        {
            var files = new DirectoryInfo(DIContainer.PluginsPath).GetFiles().ToList();
            var assemblies = from file in files
                             where file.Extension.Equals(dllExtension, StringComparison.InvariantCultureIgnoreCase)
                             select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));

            DIContainer.Container.RegisterPackages(assemblies);

            DIContainer.Container.Verify();
        }

        /// <summary>
        /// Starts the specified plugins path.
        /// </summary>
        /// <param name="pluginsPath">The plugins path.</param>
        public static void Start(string pluginsPath)
        {
            var container = new Container();

            ConfigureOptions(container);
            ConfigureExtensions(container);

            DIContainer.Init(container, pluginsPath);
        }

        /// <summary>
        /// Configures the extensions.
        /// </summary>
        /// <param name="container">The container.</param>
        private static void ConfigureExtensions(Container container)
        {
            container.AllowResolvingFuncFactories();
            container.AllowResolvingLazyFactories();
            container.AllowResolvingParameterizedFuncFactories();
        }

        /// <summary>
        /// Configures the options.
        /// </summary>
        /// <param name="container">The container.</param>
        private static void ConfigureOptions(Container container)
        {
            container.Options.AllowOverridingRegistrations = true;
        }

        #endregion Methods
    }
}