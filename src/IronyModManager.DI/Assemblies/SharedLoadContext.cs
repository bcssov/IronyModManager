// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 02-25-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2020
// ***********************************************************************
// <copyright file="SharedLoadContext.cs" company="Mario">
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

namespace IronyModManager.DI.Assemblies
{
    /// <summary>
    /// Class SharedContextLoader.
    /// Implements the <see cref="System.Runtime.Loader.AssemblyLoadContext" />
    /// </summary>
    /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
    internal class SharedLoadContext : AssemblyLoadContext
    {
        #region Fields

        /// <summary>
        /// The assembly
        /// </summary>
        private readonly string assembly;

        /// <summary>
        /// The dependency resolver
        /// </summary>
        private readonly AssemblyDependencyResolver dependencyResolver;

        /// <summary>
        /// The shared assemblies
        /// </summary>
        private readonly HashSet<string> sharedAssemblies;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedLoadContext" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public SharedLoadContext(string assembly) : base(Path.GetFileNameWithoutExtension(assembly))
        {
            sharedAssemblies = new HashSet<string>(StringComparer.Ordinal);
            dependencyResolver = new AssemblyDependencyResolver(assembly);
            this.assembly = assembly;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds the shared assembly.
        /// </summary>
        /// <param name="assemblyNames">The assembly names.</param>
        public void AddSharedAssembly(AssemblyName[] assemblyNames)
        {
            if (assemblyNames?.Count() > 0)
            {
                foreach (var item in assemblyNames)
                {
                    AddSharedAssembly(item);
                }
            }
        }

        /// <summary>
        /// Adds the shared assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        public void AddSharedAssembly(AssemblyName assemblyName)
        {
            if (assemblyName?.Name != null && !sharedAssemblies.Contains(assemblyName.Name))
            {
                try
                {
                    var assembly = AssemblyManager.GetAssembly(assemblyName);
                    if (assembly == null)
                    {
                        assembly = Default.LoadFromAssemblyName(assemblyName);
                    }
                    if (assembly != null)
                    {
                        sharedAssemblies.Add(assemblyName.Name);
                        foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                        {
                            AddSharedAssembly(referencedAssembly);
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Loads the main assembly.
        /// </summary>
        /// <returns>Assembly.</returns>
        public Assembly LoadMainAssembly()
        {
            return LoadFromAssemblyPath(assembly);
        }

        /// <summary>
        /// When overridden in a derived class, allows an assembly to be resolved and loaded based on its <see cref="T:System.Reflection.AssemblyName" />.
        /// </summary>
        /// <param name="assemblyName">The object that describes the assembly to be loaded.</param>
        /// <returns>The loaded assembly, or <see langword="null" />.</returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName?.Name != null)
            {
                try
                {
                    var assembly = Default.LoadFromAssemblyName(assemblyName);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }
                catch
                {
                }
                var path = dependencyResolver.ResolveAssemblyToPath(assemblyName);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return LoadFromAssemblyPath(path);
                }
            }
            return base.Load(assemblyName);
        }

        /// <summary>
        /// Allows derived class to load an unmanaged library by name.
        /// </summary>
        /// <param name="unmanagedDllName">Name of the unmanaged library. Typically this is the filename without its path or extensions.</param>
        /// <returns>A handle to the loaded library, or <see cref="F:System.IntPtr.Zero" />.</returns>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                return LoadUnmanagedDllFromPath(path);
            }
            return base.LoadUnmanagedDll(unmanagedDllName);
        }

        #endregion Methods
    }
}
