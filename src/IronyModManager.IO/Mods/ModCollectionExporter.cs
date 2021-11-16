// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Ionic.Zip;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.MessageBus;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Exporter;
using IronyModManager.IO.Mods.Importers;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;
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
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The paradox importer
        /// </summary>
        private readonly ParadoxImporter paradoxImporter;

        /// <summary>
        /// The paradox launcher exporter
        /// </summary>
        private readonly ParadoxLauncherExporter paradoxLauncherExporter;

        /// <summary>
        /// The paradox launcher importer
        /// </summary>
        private readonly ParadoxLauncherImporter paradoxLauncherImporter;

        /// <summary>
        /// The paradox launcher importer beta
        /// </summary>
        private readonly ParadoxLauncherImporterBeta paradoxLauncherImporterBeta;

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
        /// <param name="messageBus">The message bus.</param>
        /// <param name="mapper">The mapper.</param>
        public ModCollectionExporter(ILogger logger, IMessageBus messageBus, IMapper mapper)
        {
            paradoxosImporter = new ParadoxosImporter(logger);
            paradoxImporter = new ParadoxImporter(logger);
            paradoxLauncherImporter = new ParadoxLauncherImporter(logger);
            paradoxLauncherExporter = new ParadoxLauncherExporter();
            paradoxLauncherImporterBeta = new ParadoxLauncherImporterBeta(logger);
            this.logger = logger;
            this.messageBus = messageBus;
            this.mapper = mapper;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ExportAsync(ModCollectionExporterParams parameters)
        {
            double processed = 0;
            double previousProgress = 0;
            void saveProgress(object sender, SaveProgressEventArgs e)
            {
                switch (e.EventType)
                {
                    case ZipProgressEventType.Saving_AfterWriteEntry:
                        processed++;
                        var perc = GetProgressPercentage(e.EntriesTotal, processed, 100);
                        if (perc != previousProgress)
                        {
                            messageBus.Publish(new ModExportProgressEvent(perc));
                            previousProgress = perc;
                        }
                        break;

                    default:
                        break;
                }
            }

            var content = JsonConvert.SerializeObject(parameters.Mod, Formatting.None);
            using var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var streams = new List<Stream>() { dataStream };
            using var zip = new ZipFile();
            zip.AddEntry(Common.Constants.ExportedModContentId, dataStream);

            if (Directory.Exists(parameters.ModDirectory) && !parameters.ExportModOrderOnly)
            {
                var files = Directory.GetFiles(parameters.ModDirectory, "*.*", SearchOption.AllDirectories);
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    foreach (var item in files)
                    {
                        var fs = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                        var file = item.Replace(parameters.ModDirectory, string.Empty).Trim('\\').Trim('/');
                        zip.AddEntry(file, fs);
                        streams.Add(fs);
                    }
                }
                else
                {
                    // Yeah, osx sucks (ulimit bypass)
                    foreach (var item in files)
                    {
                        var fs = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                        var ms = new MemoryStream();
                        await fs.CopyToAsync(ms);
                        var file = item.Replace(parameters.ModDirectory, string.Empty).Trim('\\').Trim('/');
                        zip.AddEntry(file, ms);
                        fs.Close();
                        await fs.DisposeAsync();
                        streams.Add(ms);
                    }
                }
            }
            if ((parameters.ExportMods?.Any()).GetValueOrDefault())
            {
                foreach (var mod in parameters.ExportMods)
                {
                    if (Directory.Exists(mod.FullPath))
                    {
                        var files = Directory.GetFiles(mod.FullPath, "*.*", SearchOption.AllDirectories);
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            foreach (var item in files)
                            {
                                var fs = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                                var file = Path.Combine(Common.Constants.ModExportPath, parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + mod.Name.GenerateValidFileName() + "_" + item.Replace(Path.GetDirectoryName(mod.FullPath), string.Empty).Trim('\\').Trim('/'));
                                zip.AddEntry(file, fs);
                                streams.Add(fs);
                            }
                        }
                        else
                        {
                            foreach (var item in files)
                            {
                                var fs = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                                var ms = new MemoryStream();
                                await fs.CopyToAsync(ms);
                                var file = Path.Combine(Common.Constants.ModExportPath, parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + mod.Name.GenerateValidFileName() + "_" + item.Replace(Path.GetDirectoryName(mod.FullPath), string.Empty).Trim('\\').Trim('/'));
                                zip.AddEntry(file, ms);
                                fs.Close();
                                await fs.DisposeAsync();
                                streams.Add(ms);
                            }
                        }
                    }
                    else if (File.Exists(mod.FullPath))
                    {
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            var fs = new FileStream(mod.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            var file = Path.Combine(Common.Constants.ModExportPath, parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + mod.Name.GenerateValidFileName() + "_" + Path.GetFileName(mod.FullPath));
                            zip.AddEntry(file, fs);
                            streams.Add(fs);
                        }
                        else
                        {
                            var fs = new FileStream(mod.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            var ms = new MemoryStream();
                            await fs.CopyToAsync(ms);
                            var file = Path.Combine(Common.Constants.ModExportPath, parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + mod.Name.GenerateValidFileName() + "_" + Path.GetFileName(mod.FullPath));
                            zip.AddEntry(file, ms);
                            fs.Close();
                            await fs.DisposeAsync();
                            streams.Add(ms);
                        }
                    }
                }
            }

            zip.SaveProgress += saveProgress;
            zip.Save(parameters.File);
            zip.SaveProgress -= saveProgress;
            zip.Dispose();
            if (streams.Any())
            {
                var task = streams.Select(async p =>
                {
                    p.Close();
                    await p.DisposeAsync();
                });
                await Task.WhenAll(task);
            }
            return true;
        }

        /// <summary>
        /// Exports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportParadoxLauncherJsonAsync(ModCollectionExporterParams parameters)
        {
            return paradoxLauncherExporter.ExportAsync(parameters);
        }

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<ICollectionImportResult> ImportAsync(ModCollectionExporterParams parameters)
        {
            ImportInternal(parameters, true, out var result);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Imports the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ImportModDirectoryAsync(ModCollectionExporterParams parameters)
        {
            var result = ImportInternal(parameters, false, out _);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Imports the paradox asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<ICollectionImportResult> ImportParadoxAsync(ModCollectionExporterParams parameters)
        {
            return paradoxImporter.ImportAsync(parameters);
        }

        /// <summary>
        /// Imports the paradox launcher asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<ICollectionImportResult> ImportParadoxLauncherAsync(ModCollectionExporterParams parameters)
        {
            return paradoxLauncherImporter.DatabaseImportAsync(parameters);
        }

        /// <summary>
        /// Imports the paradox launcher beta asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;ICollectionImportResult&gt;.</returns>
        public Task<ICollectionImportResult> ImportParadoxLauncherBetaAsync(ModCollectionExporterParams parameters)
        {
            return paradoxLauncherImporterBeta.DatabaseImportAsync(parameters);
        }

        /// <summary>
        /// Imports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<ICollectionImportResult> ImportParadoxLauncherJsonAsync(ModCollectionExporterParams parameters)
        {
            return paradoxLauncherImporter.JsonImportAsync(parameters);
        }

        /// <summary>
        /// import paradoxos as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<ICollectionImportResult> ImportParadoxosAsync(ModCollectionExporterParams parameters)
        {
            return paradoxosImporter.ImportAsync(parameters);
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Double.</returns>
        protected virtual double GetProgressPercentage(double total, double processed, double maxPerc = 100)
        {
            var perc = Math.Round(processed / total * 100, 2);
            if (perc < 0)
            {
                perc = 0;
            }
            else if (perc > maxPerc)
            {
                perc = maxPerc;
            }
            return perc;
        }

        /// <summary>
        /// Imports the internal.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="importInstance">if set to <c>true</c> [import instance].</param>
        /// <param name="collectionImportResult">The collection import result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ImportInternal(ModCollectionExporterParams parameters, bool importInstance, out ICollectionImportResult collectionImportResult)
        {
            ICollectionImportResult importResult = null;
            if (importInstance)
            {
                importResult = DIResolver.Get<ICollectionImportResult>();
            }
            if (!importInstance)
            {
                if (Directory.Exists(parameters.ModDirectory))
                {
                    DiskOperations.DeleteDirectory(parameters.ModDirectory, true);
                }
            }

            var result = false;

            int getTotalFileCount()
            {
                var count = 0;
                using var fileStream = File.OpenRead(parameters.File);
                using var reader = ReaderFactory.Open(fileStream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        count++;
                    }
                }
                fileStream.Close();
                fileStream.Dispose();
                return count;
            }

            void parseUsingReaderFactory()
            {
                double total = getTotalFileCount();
                using var fileStream = File.OpenRead(parameters.File);
                using var reader = ReaderFactory.Open(fileStream);
                double processed = 0;
                double previousProgress = 0;
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        var relativePath = reader.Entry.Key.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
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
                                streamReader.Dispose();
                                var model = JsonDISerializer.Deserialize<IModCollection>(text);
                                mapper.Map(model, importResult);
                                importResult.ModNames = model.ModNames;
                                importResult.Descriptors = model.Mods;
                                result = true;
                                break;
                            }
                        }
                        else
                        {
                            if (!importInstance)
                            {
                                var exportFileName = Path.Combine(relativePath.StartsWith(Common.Constants.ModExportPath + Path.DirectorySeparatorChar) ? parameters.ExportModDirectory : parameters.ModDirectory, relativePath.Replace(Common.Constants.ModExportPath + Path.DirectorySeparatorChar, string.Empty));
                                if (!Directory.Exists(Path.GetDirectoryName(exportFileName)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(exportFileName));
                                }
                                reader.WriteEntryToFile(exportFileName, ZipExtractionOpts.GetExtractionOptions());
                            }
                        }
                        processed++;
                        var perc = GetProgressPercentage(total, processed, 100);
                        if (perc != previousProgress)
                        {
                            messageBus.Publish(new ModExportProgressEvent(perc));
                            previousProgress = perc;
                        }
                    }
                }
            }

            void parseUsingArchiveFactory()
            {
                using var fileStream = File.OpenRead(parameters.File);
                using var reader = ArchiveFactory.Open(fileStream);
                var entries = reader.Entries.Where(entry => !entry.IsDirectory);
                double total = !importInstance ? entries.Count() : 1;
                double processed = 0;
                double previousProgress = 0;
                foreach (var entry in entries)
                {
                    var relativePath = entry.Key.StandardizeDirectorySeparator().Trim(Path.DirectorySeparatorChar);
                    if (entry.Key.Equals(Common.Constants.ExportedModContentId, StringComparison.OrdinalIgnoreCase))
                    {
                        if (importInstance)
                        {
                            using var entryStream = entry.OpenEntryStream();
                            using var memoryStream = new MemoryStream();
                            entryStream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            using var streamReader = new StreamReader(memoryStream, true);
                            var text = streamReader.ReadToEnd();
                            streamReader.Close();
                            streamReader.Dispose();
                            var model = JsonDISerializer.Deserialize<IModCollection>(text);
                            mapper.Map(model, importResult);
                            importResult.ModNames = model.ModNames;
                            importResult.Descriptors = model.Mods;
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        if (!importInstance)
                        {
                            var exportFileName = Path.Combine(relativePath.StartsWith(Common.Constants.ModExportPath + Path.DirectorySeparatorChar) ? parameters.ExportModDirectory : parameters.ModDirectory, relativePath.Replace(Common.Constants.ModExportPath + Path.DirectorySeparatorChar, string.Empty));
                            if (!Directory.Exists(Path.GetDirectoryName(exportFileName)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(exportFileName));
                            }
                            entry.WriteToFile(exportFileName, ZipExtractionOpts.GetExtractionOptions());
                        }
                    }
                    processed++;
                    var perc = GetProgressPercentage(total, processed, 100);
                    if (perc != previousProgress)
                    {
                        messageBus.Publish(new ModExportProgressEvent(perc));
                        previousProgress = perc;
                    }
                }
            }
            try
            {
                parseUsingArchiveFactory();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
                parseUsingReaderFactory();
            }
            collectionImportResult = importResult;
            return !importInstance || result;
        }

        #endregion Methods
    }
}
