// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="AssemblyFinder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using IronyModManager.DI.Readers;
using IronyModManager.Shared;
using McMaster.NETCore.Plugins;

namespace IronyModManager.DI.Assemblies
{
    /// <summary>
    /// Class AssemblyFinder.
    /// </summary>
    internal static class AssemblyFinder
    {
        #region Methods

        /// <summary>
        /// Finds the specified finder parameters.
        /// </summary>
        /// <param name="finderParams">The finder parameters.</param>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        public static IEnumerable<Assembly> Find(AssemblyFinderParams finderParams)
        {
            var assemblies = FindAndLoadAssemblies(finderParams);

            ValidateAssemblies(assemblies, finderParams);

            var infos = new List<ModuleInfo>();
            foreach (var item in assemblies)
            {
                infos.Add(GetModuleInfo(item));
            }

            var sorted = SortDependencies(infos);

            return sorted.Select(p => p.Assembly);
        }

        /// <summary>
        /// Finds the and load assemblies.
        /// </summary>
        /// <param name="finderParams">The finder parameters.</param>
        /// <returns>List&lt;Assembly&gt;.</returns>
        private static List<Assembly> FindAndLoadAssemblies(AssemblyFinderParams finderParams)
        {
            var files = new DirectoryInfo(finderParams.Path).GetFiles($"*{Constants.DllExtension}", finderParams.SearchOption)
                .Where(p => p.Name.Contains(finderParams.AssemblyPatternMatch, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.Name).ToList();

            var assemblies = new List<Assembly>();

            foreach (var item in files)
            {
                if (!AssemblyLoadContext.Default.Assemblies.Any(p => p.GetName().Name.Equals(Path.GetFileNameWithoutExtension(item.Name), StringComparison.OrdinalIgnoreCase)))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(assemblyFile: item.FullName,
                        sharedTypes: finderParams.SharedTypes.ToArray(),
                        configure =>
                        {
                            configure.PreferSharedTypes = true;
                        });
                    var assembly = loader.LoadDefaultAssembly();
                    assemblies.Add(assembly);
                }
                else
                {
                    var assembly = AssemblyLoadContext.Default.Assemblies.FirstOrDefault(p => p.GetName().Name.Equals(Path.GetFileNameWithoutExtension(item.Name), StringComparison.OrdinalIgnoreCase));
                    assemblies.Add(assembly);
                }
            }

            return assemblies;
        }

        /// <summary>
        /// Gets the module information.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>IronyModManager.DI.Assemblies.AssemblyFinder.ModuleInfo.</returns>
        private static ModuleInfo GetModuleInfo(Assembly assembly)
        {
            var addin = assembly.GetTypes().First(p => typeof(IAddin).IsAssignableFrom(p) && !p.IsAbstract);
            var instance = Activator.CreateInstance(addin) as IAddin;
            return new ModuleInfo()
            {
                Assembly = assembly,
                Dependencies = instance.Dependencies ?? new List<string>(),
                Name = instance.Name
            };
        }

        /// <summary>
        /// Sorts the dependencies.
        /// </summary>
        /// <param name="infos">The infos.</param>
        /// <returns>List&lt;ModuleInfo&gt;.</returns>
        private static List<ModuleInfo> SortDependencies(List<ModuleInfo> infos)
        {
            var sorted = new List<ModuleInfo>();
            var processed = new HashSet<ModuleInfo>();

            void process(ModuleInfo item)
            {
                if (!processed.Contains(item))
                {
                    processed.Add(item);

                    foreach (var dependency in item.Dependencies)
                    {
                        var dependentItem = infos.FirstOrDefault(p => p.Name.Equals(dependency));
                        if (dependentItem != null)
                        {
                            process(dependentItem);
                        }
                    }
                    sorted.Add(item);
                }
            }

            foreach (var item in infos)
            {
                process(item);
            }

            return sorted;
        }

        /// <summary>
        /// Validates the assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="finderParams">The finder parameters.</param>
        /// <exception cref="InvalidOperationException">Assembly {assembly.FullName} does not belong to {finderParams.Path}, application execution has been stopped due to security reasons.</exception>
        private static void ValidateAssemblies(IEnumerable<Assembly> assemblies, AssemblyFinderParams finderParams)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var thisAssemblyKey = ResourceReader.GetEmbeddedResource(finderParams.EmbededResourceKey, thisAssembly);
            var thisStrongName = ResourceReader.GetStrongName(thisAssemblyKey, thisAssembly);

            foreach (var assembly in assemblies)
            {
                var key = ResourceReader.GetPublicKey(assembly);
                var strongName = ResourceReader.GetStrongName(key, assembly);
                if (!strongName.PublicKey.Equals(thisStrongName.PublicKey))
                {
                    throw new InvalidOperationException($"Assembly {assembly.FullName} does not belong to {finderParams.Path}, application execution has been stopped due to security reasons.");
                }
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ModuleInfo.
        /// </summary>
        private class ModuleInfo
        {
            #region Properties

            /// <summary>
            /// Gets or sets the assembly.
            /// </summary>
            /// <value>The assembly.</value>
            public Assembly Assembly { get; set; }

            /// <summary>
            /// Gets or sets the dependencies.
            /// </summary>
            /// <value>The dependencies.</value>
            public IEnumerable<string> Dependencies { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
