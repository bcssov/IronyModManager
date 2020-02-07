// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-07-2020
// ***********************************************************************
// <copyright file="Bootstrap.cs" company="IronyModManager.DI">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IronyModManager.DI.Assemblies;
using IronyModManager.DI.Extensions;
using IronyModManager.DI.Mappers;
using IronyModManager.DI.PostStartup;
using SimpleInjector;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class Bootstrap.
    /// </summary>
    public static class Bootstrap
    {
        #region Methods

        /// <summary>
        /// Posts the startup.
        /// </summary>
        public static void PostStartup()
        {
            var assemblyFinderParams = new AssemblyFinderParams[] { GetAppParams(), GetPluginParams() };
            var assemblies = new List<Assembly>() { };
            foreach (var assemblyFinderParam in assemblyFinderParams)
            {
                assemblies.AddRange(AssemblyFinder.Find(assemblyFinderParam));
            }
            PostStartupFinder.FindAndExecute(assemblies);
        }

        /// <summary>
        /// Initializes the specified plugins path and name.
        /// </summary>
        /// <param name="opts">The opts.</param>
        public static void Setup(DIOptions opts)
        {
            DIContainer.Init(opts);

            ConfigureOptions(opts.Container);
            ConfigureExtensions(opts.Container);

            RegisterAssemblies();
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

        /// <summary>
        /// Gets the application parameters.
        /// </summary>
        /// <returns>AssemblyFinderParams.</returns>
        private static AssemblyFinderParams GetAppParams()
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var appParams = new AssemblyFinderParams()
            {
                EmbededResourceKey = Constants.MainKey,
                Path = appPath,
                AssemblyPatternMatch = nameof(IronyModManager),
                SearchOption = SearchOption.TopDirectoryOnly,
                SharedTypes = DIContainer.ModuleTypes
            };
            return appParams;
        }

        /// <summary>
        /// Gets the plugin parameters.
        /// </summary>
        /// <returns>AssemblyFinderParams.</returns>
        private static AssemblyFinderParams GetPluginParams()
        {
            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DIContainer.PluginPathAndName);
            var pluginParams = new AssemblyFinderParams()
            {
                EmbededResourceKey = Constants.PluginKey,
                Path = pluginsPath,
                AssemblyPatternMatch = nameof(IronyModManager),
                SearchOption = SearchOption.AllDirectories,
                SharedTypes = DIContainer.PluginTypes
            };
            return pluginParams;
        }

        /// <summary>
        /// Registers the assemblies.
        /// </summary>
        private static void RegisterAssemblies()
        {
            var appParams = GetAppParams();
            var pluginParams = GetPluginParams();

            AssemblyManager.RegisterHandlers();

            RegisterDIAssemblies(appParams, pluginParams);
            RegisterAutomapperProfiles(appParams, pluginParams);
        }

        /// <summary>
        /// Registers the automapper profiles.
        /// </summary>
        /// <param name="assemblyFinderParams">The assembly finder parameters.</param>
        private static void RegisterAutomapperProfiles(params AssemblyFinderParams[] assemblyFinderParams)
        {
            var assemblies = new List<Assembly>() { };
            foreach (var assemblyFinderParam in assemblyFinderParams)
            {
                assemblies.AddRange(AssemblyFinder.Find(assemblyFinderParam));
            }

            var config = MapperFinder.Find(assemblies);
            var container = DIContainer.Container;

            container.RegisterInstance(config);
            container.Register(() => config.CreateMapper(container.GetInstance));
        }

        /// <summary>
        /// Registers the di assemblies.
        /// </summary>
        /// <param name="assemblyFinderParams">The assembly finder parameters.</param>
        private static void RegisterDIAssemblies(params AssemblyFinderParams[] assemblyFinderParams)
        {
            var assemblies = new List<Assembly>() { };
            foreach (var assemblyFinderParam in assemblyFinderParams)
            {
                assemblies.AddRange(AssemblyFinder.Find(assemblyFinderParam));
            }

            DIContainer.Container.RegisterPackages(assemblies);
        }

        #endregion Methods
    }
}
