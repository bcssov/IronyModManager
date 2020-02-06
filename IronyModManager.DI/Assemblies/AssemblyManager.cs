// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="AssemblyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace IronyModManager.DI.Assemblies
{
    /// <summary>
    /// Class AssemblyManager.
    /// </summary>
    public static class AssemblyManager
    {
        #region Fields

        /// <summary>
        /// The cache
        /// </summary>
        private static readonly ConcurrentDictionary<string, Assembly> assemblyCache = new ConcurrentDictionary<string, Assembly>();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Finds the type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Type.</returns>
        public static Type FindType(string name)
        {
            var type = Type.GetType(name);
            if (type == null)
            {
                foreach (var item in AssemblyLoadContext.Default.Assemblies)
                {
                    type = item.GetType(name);
                    if (type != null)
                    {
                        return type;
                    }
                }
                foreach (var item in AssemblyLoadContext.All.SelectMany(s => s.Assemblies))
                {
                    type = item.GetType(name);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>Assembly.</returns>
        internal static Assembly GetAssembly(AssemblyName assemblyName)
        {
            assemblyCache.TryGetValue(assemblyName.FullName, out var assembly);
            if (assembly == null)
            {
                // Default assemblies take priority
                var assemblies = AssemblyLoadContext.Default.Assemblies.Where(s => s.GetName().Name.Equals(assemblyName.Name));
                if (assemblies.Count() == 0)
                {
                    // Search anywhere now
                    assemblies = AssemblyLoadContext.All.SelectMany(p => p.Assemblies.Where(s => s.GetName().Name.Equals(assemblyName.Name)));
                }
                if (assemblies.Count() > 0)
                {
                    var sorted = new List<AssemblySort>() { new AssemblySort { Version = assemblyName.Version, IsComparerObject = true } };
                    foreach (var item in assemblies)
                    {
                        sorted.Add(new AssemblySort()
                        {
                            Version = item.GetName().Version,
                            Assembly = item
                        });
                    }
                    if (sorted.Any(p => !p.IsComparerObject && p.Version == assemblyName.Version))
                    {
                        assembly = sorted.FirstOrDefault(p => !p.IsComparerObject && p.Version == assemblyName.Version).Assembly;
                    }
                    else
                    {
                        sorted = sorted.OrderBy(p => p.Version).ToList();
                        var sortedIdx = sorted.FindIndex(p => p.IsComparerObject);
                        var prev = sortedIdx - 1;
                        var next = sortedIdx + 1;
                        if (next < sorted.Count)
                        {
                            assembly = sorted[next].Assembly;
                        }
                        else if (prev > -1)
                        {
                            assembly = sorted[prev].Assembly;
                        }
                    }

                    if (assembly != null)
                    {
                        assemblyCache.TryAdd(assemblyName.FullName, assembly);
                    }
                }
            }
            if (assembly != null)
            {
                return assembly;
            }
            return null;
        }

        /// <summary>
        /// Registers the handlers.
        /// </summary>
        internal static void RegisterHandlers()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args == null)
                {
                    return null;
                }
                var assemblyName = new AssemblyName(args.Name);
                var assembly = GetAssembly(assemblyName);
                if (args.RequestingAssembly != null)
                {
                    var ctx = AssemblyLoadContext.GetLoadContext(args.RequestingAssembly);
                    if (ctx != null)
                    {
                        var assemblyPath = Path.Combine(Path.GetDirectoryName(args.RequestingAssembly.Location), $"{assemblyName.Name}{Constants.DllExtension}");
                        if (File.Exists(assemblyPath))
                        {
                            if (assembly == null)
                            {
                                assembly = LoadAssembly(ctx, assemblyPath);
                            }
                            else if (!Path.GetDirectoryName(args.RequestingAssembly.Location).Equals(Path.GetDirectoryName(assembly.Location), StringComparison.OrdinalIgnoreCase))
                            {
                                var loadedAssembly = LoadAssembly(ctx, assemblyPath);
                                if (loadedAssembly != null)
                                {
                                    assembly = loadedAssembly;
                                }
                            }
                        }
                    }
                }
                return assembly;
            };
        }

        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>Assembly.</returns>
        private static Assembly LoadAssembly(AssemblyLoadContext ctx, string assemblyPath)
        {
            try
            {
                if (File.Exists(assemblyPath))
                {
                    using (var stream = File.OpenRead(assemblyPath))
                    {
                        return ctx.LoadFromStream(stream);
                    }
                }
            }
            catch { }

            return null;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class AssemblySort.
        /// </summary>
        private class AssemblySort
        {
            #region Properties

            /// <summary>
            /// Gets or sets the assembly.
            /// </summary>
            /// <value>The assembly.</value>
            public Assembly Assembly { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is comparer object.
            /// </summary>
            /// <value><c>true</c> if this instance is comparer object; otherwise, <c>false</c>.</value>
            public bool IsComparerObject { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            public Version Version { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
