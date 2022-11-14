// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 11-13-2022
// ***********************************************************************
// <copyright file="ModMergeExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModMergeExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModMergeExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModMergeExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModMergeExporter : IModMergeExporter
    {
        #region Fields

        /// <summary>
        /// The reader
        /// </summary>
        private readonly IReader reader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModMergeExporter" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public ModMergeExporter(IReader reader)
        {
            this.reader = reader;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the files asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentNullException">nameof(parameters), ExportPath.</exception>
        /// <exception cref="ArgumentNullException">nameof(parameters), ExportFile.</exception>
        /// <exception cref="ArgumentNullException">nameof(parameters), RootModPath</exception>
        public Task<bool> ExportFilesAsync(ModMergeFileExporterParameters parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.ExportPath))
            {
                throw new ArgumentNullException(nameof(parameters), "ExportPath.");
            }
            if (string.IsNullOrWhiteSpace(parameters.ExportFile))
            {
                throw new ArgumentNullException(nameof(parameters), "ExportFile.");
            }
            if (string.IsNullOrWhiteSpace(parameters.RootModPath))
            {
                throw new ArgumentNullException(nameof(parameters), "RootModPath");
            }
            var retry = new RetryStrategy();
            return retry.RetryActionAsync(() => CopyStreamAsync(parameters.RootModPath, parameters.ExportFile, parameters.ExportPath));
        }

        /// <summary>
        /// Copies the stream asynchronous.
        /// </summary>
        /// <param name="modPath">The mod path.</param>
        /// <param name="modFile">The mod file.</param>
        /// <param name="exportPath">The export path.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Boolean&gt;.</returns>
        private Task<bool> CopyStreamAsync(string modPath, string modFile, string exportPath)
        {
            static async Task<bool> copyStream(Stream s, FileStream fs)
            {
                await s.CopyToAsync(fs);
                await fs.DisposeAsync();
                await s.DisposeAsync();
                fs.Close();
                s.Close();
                return true;
            }

            var retry = new RetryStrategy();
            var outPath = Path.Combine(exportPath, modFile);
            var stream = reader.GetStream(modPath, modFile);
            // If image and no stream try switching extension
            if (FileSignatureUtility.IsImageFile(modFile) && stream == null)
            {
                var segments = modFile.Split(".", StringSplitOptions.RemoveEmptyEntries);
                var file = string.Join(".", segments.Take(segments.Length - 1));
                foreach (var item in Shared.Constants.ImageExtensions)
                {
                    stream = reader.GetStream(modPath, file + item);
                    if (stream != null)
                    {
                        break;
                    }
                }
            }
            // Potentially malicious if you have a directory named events\test.txt folder
            if (Directory.Exists(outPath) && string.IsNullOrWhiteSpace(Path.GetExtension(outPath)))
            {
                // Some really crap mods seem to have "common" file instead of folder ie
                return Task.FromResult(false);
            }
            var outDir = Path.GetDirectoryName(outPath);
            if (!Directory.Exists(outDir))
            {
                var partialModDirectory = Path.GetDirectoryName(modFile);
                var partialExportPath = Path.Combine(exportPath, partialModDirectory);
                while (!string.IsNullOrWhiteSpace(partialModDirectory))
                {
                    // Some really crap mods seem to have "common" file instead of folder ie
                    if (File.Exists(partialExportPath))
                    {
                        DiskOperations.DeleteFile(partialExportPath);
                    }
                    partialModDirectory = Path.GetDirectoryName(partialModDirectory);
                    partialExportPath = Path.Combine(exportPath, partialModDirectory);
                }
                Directory.CreateDirectory(outDir);
            }
            if (File.Exists(outPath))
            {
                DiskOperations.DeleteFile(outPath);
            }
            var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return retry.RetryActionAsync(() =>
            {
                return copyStream(stream, fs);
            });
        }

        #endregion Methods
    }
}
