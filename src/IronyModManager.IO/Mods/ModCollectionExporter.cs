// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 04-04-2020
// ***********************************************************************
// <copyright file="ModCollectionExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Shared;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModCollectionExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModCollectionExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModCollectionExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModCollectionExporter : IModCollectionExporter
    {
        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportAsync(ModCollectionExporterParams parameters)
        {
            // TODO: Add logic for this, at the moment there is no conflict detector
            if (Directory.Exists(parameters.ModDirectory))
            {
            }
            var content = JsonConvert.SerializeObject(parameters.Mod, Formatting.Indented);
            using var zip = ArchiveFactory.Create(SharpCompress.Common.ArchiveType.Zip);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            zip.AddEntry(Common.Constants.ExportedModContentId, stream, false);
            zip.SaveTo(parameters.File, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate));
            zip.Dispose();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ImportAsync(ModCollectionExporterParams parameters)
        {
            using var fileStream = File.OpenRead(parameters.File);
            using var reader = ReaderFactory.Open(fileStream);
            var result = false;
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var relativePath = reader.Entry.Key.Trim("\\/".ToCharArray()).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    using var entryStream = reader.OpenEntryStream();
                    using var memoryStream = new MemoryStream();
                    entryStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    if (reader.Entry.Key.Equals(Common.Constants.ExportedModContentId, StringComparison.OrdinalIgnoreCase))
                    {
                        using var streamReader = new StreamReader(memoryStream, true);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        JsonConvert.PopulateObject(text, parameters.Mod);
                        result = true;
                    }
                    else
                    {
                        // TODO: Add logic for mod directory import, there is no conflict detector at the moment
                    }
                }
            }
            return Task.FromResult(result);
        }

        #endregion Methods
    }
}
