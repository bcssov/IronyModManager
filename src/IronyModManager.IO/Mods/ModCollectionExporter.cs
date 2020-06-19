// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
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
using SharpCompress.Common;
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
        #region Fields

        /// <summary>
        /// The extraction options
        /// </summary>
        private static ExtractionOptions extractionOptions;

        /// <summary>
        /// The paradoxos importer
        /// </summary>
        private readonly ParadoxosImporter paradoxosImporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollectionExporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ModCollectionExporter(ILogger logger)
        {
            paradoxosImporter = new ParadoxosImporter(logger);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportAsync(ModCollectionExporterParams parameters)
        {
            var content = JsonConvert.SerializeObject(parameters.Mod, Formatting.None);
            using var zip = ArchiveFactory.Create(ArchiveType.Zip);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            zip.AddEntry(Common.Constants.ExportedModContentId, stream, false);
            if (Directory.Exists(parameters.ModDirectory))
            {
                zip.AddAllFromDirectory(parameters.ModDirectory);
            }
            zip.SaveTo(parameters.File, new SharpCompress.Writers.WriterOptions(CompressionType.Deflate));
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
            return Task.FromResult(ImportInternal(parameters, true));
        }

        /// <summary>
        /// Imports the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ImportModDirectoryAsync(ModCollectionExporterParams parameters)
        {
            return Task.FromResult(ImportInternal(parameters, false));
        }

        /// <summary>
        /// import paradoxos as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ImportParadoxosAsync(ModCollectionExporterParams parameters)
        {
            return paradoxosImporter.ImportAsync(parameters);
        }

        /// <summary>
        /// Gets the extraction options.
        /// </summary>
        /// <returns>ExtractionOptions.</returns>
        private ExtractionOptions GetExtractionOptions()
        {
            if (extractionOptions == null)
            {
                extractionOptions = new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = true,
                    PreserveFileTime = true
                };
            }
            return extractionOptions;
        }

        /// <summary>
        /// Imports the internal.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="importInstance">if set to <c>true</c> [import instance].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ImportInternal(ModCollectionExporterParams parameters, bool importInstance)
        {
            if (!importInstance)
            {
                if (Directory.Exists(parameters.ModDirectory))
                {
                    Directory.Delete(parameters.ModDirectory, true);
                }
            }
            using var fileStream = File.OpenRead(parameters.File);
            using var reader = ReaderFactory.Open(fileStream);
            var result = false;
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var relativePath = reader.Entry.Key.Trim("\\/".ToCharArray()).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    if (reader.Entry.Key.Equals(Common.Constants.ExportedModContentId, StringComparison.OrdinalIgnoreCase))
                    {
                        if (importInstance)
                        {
                            using var entryStream = reader.OpenEntryStream();
                            using var memoryStream = new MemoryStream();
                            entryStream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            using var streamReader = new StreamReader(memoryStream, true);
                            var text = streamReader.ReadToEnd();
                            streamReader.Close();
                            JsonConvert.PopulateObject(text, parameters.Mod);
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        reader.WriteEntryToDirectory(parameters.ModDirectory, GetExtractionOptions());
                    }
                }
            }
            return !importInstance || result;
        }

        #endregion Methods
    }
}
