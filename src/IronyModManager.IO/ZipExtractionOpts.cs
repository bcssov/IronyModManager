// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 05-12-2026
// ***********************************************************************
// <copyright file="ZipExtractionOpts.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;
using SharpCompress.Common;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class ZipExtractionOpts.
    /// </summary>
    internal static class ZipExtractionOpts
    {
        #region Fields

        /// <summary>
        /// The safe path comparison
        /// </summary>
        private static readonly StringComparison safePathComparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        /// <summary>
        /// The extraction options
        /// </summary>
        private static ExtractionOptions extractionOptions;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the extraction options.
        /// </summary>
        /// <returns>ExtractionOptions.</returns>
        public static ExtractionOptions GetExtractionOptions()
        {
            extractionOptions ??= new ExtractionOptions { ExtractFullPath = false, Overwrite = true, PreserveFileTime = true };
            return extractionOptions;
        }

        /// <summary>
        /// Sanitizes the archive path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.InvalidOperationException">Archive entry path is empty.</exception>
        /// <exception cref="System.InvalidOperationException">Rooted archive path is not allowed: {path}</exception>
        /// <exception cref="System.InvalidOperationException">Archive entry contains path traversal: {path}</exception>
        public static string SanitizeArchivePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException("Archive entry path is empty.");
            }

            path = path.StandardizeDirectorySeparator();

            path = path.Trim(Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(path))
            {
                throw new InvalidOperationException($"Rooted archive path is not allowed: {path}");
            }

            var normalized = Path.GetFullPath(Path.Combine(Path.GetTempPath(), path));

            var expectedRoot = Path.GetFullPath(Path.GetTempPath());

            if (!expectedRoot.EndsWith(Path.DirectorySeparatorChar))
            {
                expectedRoot += Path.DirectorySeparatorChar;
            }

            if (!normalized.StartsWith(expectedRoot, safePathComparison))
            {
                throw new InvalidOperationException($"Archive entry contains path traversal: {path}");
            }

            return path;
        }

        #endregion Methods
    }
}
