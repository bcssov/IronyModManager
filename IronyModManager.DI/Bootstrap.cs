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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using IronyModManager.DI.Extensions;
using SimpleInjector;

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
        private const string DllExtension = ".dll";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        public static void Finish()
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var pluginsPath = Path.Combine(appPath, DIContainer.PluginsPath);

            RegisterDIAssemblies(appPath, pluginsPath);

            RegisterAutomapperProfiles(appPath, pluginsPath);

            DIContainer.Container.Verify();
        }

        /// <summary>
        /// Initializes the specified plugins path.
        /// </summary>
        /// <param name="pluginsPath">The plugins path.</param>
        public static void Init(string pluginsPath)
        {
            var container = new Container();
            DIContainer.Init(container, pluginsPath);

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
        /// Gets the valid assemblies.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        private static IEnumerable<Assembly> GetValidAssemblies(string path)
        {
            var files = new DirectoryInfo(path).GetFiles().Where(p => p.Name.Contains(nameof(IronyModManager), StringComparison.InvariantCultureIgnoreCase) &&
                                                                 p.Extension.Equals(DllExtension, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Name).ToList();

            var assemblies = from file in files
                             select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));
            return assemblies;
        }

        /// <summary>
        /// Registers the automapper profiles.
        /// </summary>
        /// <param name="paths">The paths.</param>
        private static void RegisterAutomapperProfiles(params string[] paths)
        {
            var assemblies = new List<Assembly>() { };
            foreach (var path in paths)
            {
                assemblies.AddRange(GetValidAssemblies(path));
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
        /// <param name="paths">The paths.</param>
        private static void RegisterDIAssemblies(params string[] paths)
        {
            var assemblies = new List<Assembly>();
            foreach (var path in paths)
            {
                assemblies.AddRange(GetValidAssemblies(path));
            }

            DIContainer.Container.RegisterPackages(assemblies);
        }

        #endregion Methods
    }
}
