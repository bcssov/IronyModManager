// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-03-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="ResourceReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class ResourceReader.
    /// </summary>
    [ExcludeFromCoverage("Excluding resource reader due to static nature of the code.")]
    public static class ResourceReader
    {
        #region Fields

        /// <summary>
        /// The cache
        /// </summary>
        private static ConcurrentDictionary<string, byte[]> cache = new ConcurrentDictionary<string, byte[]>();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the embedded resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetEmbeddedResource(string resourceName)
        {
            return GetEmbeddedResource(resourceName, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Gets the embedded resource.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] GetEmbeddedResource(string resourceName, Assembly assembly)
        {
            if (cache.TryGetValue(ConstructCacheKey(resourceName, assembly), out var value))
            {
                return value;
            }
            var resource = GetEmbeddedResourceInternal(resourceName, assembly);
            cache.TryAdd(ConstructCacheKey(resourceName, assembly), resource);
            return resource;
        }

        /// <summary>
        /// Constructs the cache key.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.String.</returns>
        private static string ConstructCacheKey(string resourceName, Assembly assembly)
        {
            return $"{resourceName}-{assembly.GetName().Name}";
        }

        /// <summary>
        /// Gets the embedded resource internal.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="ArgumentNullException">resourceName</exception>
        /// <exception cref="ArgumentNullException">assembly</exception>
        /// <exception cref="ArgumentNullException">resourceName</exception>
        /// <exception cref="ArgumentNullException">resourceName</exception>
        /// <exception cref="ArgumentNullException">assembly</exception>
        /// <exception cref="ArgumentNullException">resourceName</exception>
        /// <exception cref="ArgumentNullException">resourceName</exception>
        private static byte[] GetEmbeddedResourceInternal(string resourceName, Assembly assembly)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            var name = $"{ assembly.GetName().Name}.{resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".")}";
            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion Methods
    }
}
