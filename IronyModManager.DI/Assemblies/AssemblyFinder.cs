// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
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
using IronyModManager.DI.Readers;

namespace IronyModManager.DI.Assemblies
{
    /// <summary>
    /// Class AssemblyFinder.
    /// </summary>
    internal static class AssemblyFinder
    {
        #region Methods

        /// <summary>
        /// Finds and vlidates assemblies.
        /// </summary>
        /// <param name="finderParams">The finder parameters.</param>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        public static IEnumerable<Assembly> FindAndValidateAssemblies(AssemblyFinderParams finderParams)
        {
            var files = new DirectoryInfo(finderParams.Path).GetFiles().Where(p => p.Name.Contains(finderParams.AssemblyPatternMatch, StringComparison.InvariantCultureIgnoreCase) &&
                                                                 p.Extension.Equals(Constants.DllExtension, StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => p.Name).ToList();

            files = PrioritizeFiles(finderParams, files);

            var assemblies = from file in files
                             select Assembly.Load(AssemblyName.GetAssemblyName(file.FullName));

            ValidateAssemblies(assemblies, finderParams);

            return assemblies;
        }

        /// <summary>
        /// Prioritizes the files.
        /// </summary>
        /// <param name="finderParams">The finder parameters.</param>
        /// <param name="files">The files.</param>
        /// <returns>List&lt;FileInfo&gt;.</returns>
        private static List<FileInfo> PrioritizeFiles(AssemblyFinderParams finderParams, List<FileInfo> files)
        {
            if (finderParams.PriorityAssemblies?.Count() > 0)
            {
                var orderedFiles = new List<FileInfo>();

                foreach (var assembly in finderParams.PriorityAssemblies)
                {
                    foreach (var item in files)
                    {
                        if (Path.GetFileNameWithoutExtension(item.Name) == assembly)
                        {
                            orderedFiles.Add(item);
                        }
                    }
                }
                orderedFiles.AddRange(files.Where(p => !orderedFiles.Any(s => s == p)));

                return orderedFiles;
            }
            return files;
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
    }
}
