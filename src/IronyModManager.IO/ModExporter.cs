// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-09-2020
// ***********************************************************************
// <copyright file="ModExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.IO.Common;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class FileWriter.
    /// Implements the <see cref="IronyModManager.IO.Common.IModExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.IModExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModExporter : IModExporter
    {
        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exportPath">The export path.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="modDirectory">The mod directory.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportAsync<T>(string exportPath, T mod, string modDirectory) where T : IModel
        {
            // TODO: Add logic for this, at the moment there is no conflict detector
            if (Directory.Exists(modDirectory))
            {
            }
            var content = JsonConvert.SerializeObject(mod, Formatting.Indented);
            using var zip = ArchiveFactory.Create(SharpCompress.Common.ArchiveType.Zip);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            zip.AddEntry(Common.Constants.ExportedModContentId, stream, false);
            zip.SaveTo(exportPath, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate));
            zip.Dispose();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Imports the asynt.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The file.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ImportAsync<T>(string file, T mod) where T : IModel
        {
            using var fileStream = File.OpenRead(file);
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
                        JsonConvert.PopulateObject(text, mod);
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
