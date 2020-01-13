// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
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
using AutoMapper;
using IronyModManager.DI.Assemblies;
using IronyModManager.DI.Extensions;
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
        /// Finishes this instance.
        /// </summary>
        public static void Finish()
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var pluginsPath = Path.Combine(appPath, DIContainer.PluginPathAndName);

            var appParams = new AssemblyFinderParams()
            {
                EmbededResourceKey = Constants.MainKey,
                Path = appPath,
                AssemblyPatternMatch = nameof(IronyModManager)
            };
            var pluginParams = new AssemblyFinderParams()
            {
                EmbededResourceKey = Constants.PluginKey,
                Path = pluginsPath,
                AssemblyPatternMatch = $"{nameof(IronyModManager)}.{DIContainer.PluginPathAndName}"
            };

            RegisterDIAssemblies(appParams, pluginParams);

            RegisterAutomapperProfiles(appParams, pluginParams);
        }

        /// <summary>
        /// Initializes the specified plugins path and name.
        /// </summary>
        /// <param name="pluginsPathAndName">Name of the plugins path and.</param>
        public static void Init(string pluginsPathAndName)
        {
            var container = new Container();
            DIContainer.Init(container, pluginsPathAndName);

            ConfigureOptions(container);
            ConfigureExtensions(container);
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
        /// Registers the automapper profiles.
        /// </summary>
        /// <param name="assemblyFinderParams">The assembly finder parameters.</param>
        private static void RegisterAutomapperProfiles(params AssemblyFinderParams[] assemblyFinderParams)
        {
            var assemblies = new List<Assembly>() { };
            foreach (var assemblyFinderParam in assemblyFinderParams)
            {
                assemblies.AddRange(AssemblyFinder.FindAndValidateAssemblies(assemblyFinderParam));
            }

            var profiles = assemblies.Select(p => p.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)));
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var assemblyProfiles in profiles)
                {
                    foreach (var assemblyProfile in assemblyProfiles)
                    {
                        cfg.AddProfile(Activator.CreateInstance(assemblyProfile) as Profile);
                    }
                }
            });

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
                assemblies.AddRange(AssemblyFinder.FindAndValidateAssemblies(assemblyFinderParam));
            }

            DIContainer.Container.RegisterPackages(assemblies);
        }

        #endregion Methods
    }
}
