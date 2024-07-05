// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 09-12-2021
//
// Last Modified By : Mario
// Last Modified On : 07-05-2024
// ***********************************************************************
// <copyright file="PathOperations.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class PathOperations.
    /// </summary>
    public static class PathOperations
    {
        #region Methods

        /// <summary>
        /// Resolves the relative path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public static string ResolveRelativePath(string basePath, string path)
        {
            var result = ResolveRelativePath(basePath, path, false);
            if (!File.Exists(result) && !Directory.Exists(result) && PathContainsRelativeSegments(path))
            {
                // Fallback
                result = ResolveRelativePath(basePath, path, true);
            }

            return result;
        }

        /// <summary>
        /// Pathes the contains relative segments.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool PathContainsRelativeSegments(string path)
        {
            return path.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar).Any(p => string.IsNullOrWhiteSpace(p.Replace(".", string.Empty)));
        }

        /// <summary>
        /// Resolves the relative path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="path">The path.</param>
        /// <param name="checkSubFolder">if set to <c>true</c> [check sub folder].</param>
        /// <returns>System.String.</returns>
        private static string ResolveRelativePath(string basePath, string path, bool checkSubFolder)
        {
            basePath ??= string.Empty;
            basePath = basePath.StandardizeDirectorySeparator();
            path = path.StandardizeDirectorySeparator();
            if (checkSubFolder)
            {
                basePath = Path.Combine(basePath, "dummy");
            }

            string result;
            if (string.IsNullOrWhiteSpace(basePath) || !Path.IsPathFullyQualified(basePath))
            {
                result = Path.Combine(basePath!, path);
            }
            else
            {
                result = Path.GetFullPath(path, basePath);
            }

            return result;
        }

        #endregion Methods
    }
}
