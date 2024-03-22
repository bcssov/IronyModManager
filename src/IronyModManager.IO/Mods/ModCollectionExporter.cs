// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-22-2024
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
using IronyModManager.IO.Common.Streams;
using IronyModManager.IO.Mods.Exporter;
using IronyModManager.IO.Mods.Importers;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Configuration;
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
        /// The mod writer
        /// </summary>
        private readonly IModWriter modWriter;

        /// <summary>
        /// The paradox importer
        /// </summary>
        private readonly ParadoxImporter paradoxImporter;

        /// <summary>
        /// The paradox launcher exporter
        /// </summary>
        private readonly ParadoxLauncherExporter paradoxLauncherExporter;

        /// <summary>
        /// The paradox launcher exporter202010
        /// </summary>
        private readonly ParadoxLauncherExporter202110 paradoxLauncherExporter202110;

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
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="mapper">The mapper.</param>
        public ModCollectionExporter(IModWriter modWriter, ILogger logger, IMessageBus messageBus, IMapper mapper)
        {
            paradoxosImporter = new ParadoxosImporter(logger);
            paradoxImporter = new ParadoxImporter(logger);
            paradoxLauncherImporter = new ParadoxLauncherImporter(logger);
            paradoxLauncherExporter = new ParadoxLauncherExporter();
            paradoxLauncherImporterBeta = new ParadoxLauncherImporterBeta(logger);
            paradoxLauncherExporter202110 = new ParadoxLauncherExporter202110();
            this.logger = logger;
            this.messageBus = messageBus;
            this.mapper = mapper;
            this.modWriter = modWriter;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.ArgumentException">Invalid descriptor type</exception>
        public async Task<bool> ExportAsync(ModCollectionExporterParams parameters)
        {
            if (parameters.DescriptorType == DescriptorType.None)
            {
                throw new ArgumentException("Invalid descriptor type");
            }

            double processed = 0;
            double previousProgress = 0;

            void saveProgress(object sender, SaveProgressEventArgs e)
            {
                switch (e.EventType)
                {
                    case ZipProgressEventType.Saving_AfterWriteEntry:
                        processed++;
                        var perc = GetProgressPercentage(e.EntriesTotal, processed, 100);
                        if (perc.IsNotNearlyEqual(previousProgress))
                        {
                            messageBus.Publish(new ModExportProgressEvent(perc));
                            previousProgress = perc;
                        }

                        if (e.CurrentEntry is { InputStream: not null })
                        {
                            e.CurrentEntry.InputStream.Close();
                            e.CurrentEntry.InputStream.Dispose();
                        }

                        break;
                }
            }

            IMod cloneMod(IMod mod)
            {
                var newMod = DIResolver.Get<IMod>();
                newMod.DescriptorFile = mod.DescriptorFile;
                newMod.FileName = mod.FileName;
                newMod.Name = modWriter.FormatPrefixModName(parameters.ModNameOverride, mod.Name);
                newMod.Source = mod.Source;
                newMod.Version = mod.Version;
                newMod.FullPath = mod.FullPath;
                newMod.RemoteId = mod.RemoteId;
                newMod.Picture = mod.Picture;
                newMod.ReplacePath = mod.ReplacePath;
                newMod.Tags = mod.Tags;
                newMod.UserDir = mod.UserDir;
                newMod.Order = mod.Order;
                var dependencies = mod.Dependencies;
                if (dependencies != null && dependencies.Any())
                {
                    var newDependencies = new List<string>();
                    foreach (var item in dependencies)
                    {
                        newDependencies.Add(modWriter.FormatPrefixModName(parameters.ModNameOverride, item));
                    }

                    newMod.Dependencies = newDependencies;
                }

                return newMod;
            }

            var ulimitBypass = DIResolver.Get<IDomainConfiguration>().GetOptions().OSXOptions.UseFileStreams;
            var content = JsonConvert.SerializeObject(parameters.Mod, Newtonsoft.Json.Formatting.None);
            using var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var streams = new List<Stream> { dataStream };
            using var zip = new ZipFile();
            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
            zip.AddEntry(Common.Constants.ExportedModContentId, dataStream);

            if (Directory.Exists(parameters.ModDirectory) && !parameters.ExportModOrderOnly)
            {
                var files = Directory.GetFiles(parameters.ModDirectory, "*.*", SearchOption.AllDirectories);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !ulimitBypass)
                {
                    // Yeah, osx sucks (ulimit bypass)
                    foreach (var item in files)
                    {
                        var fs = new OnDemandFileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                        var ms = new MemoryStream();
                        await fs.CopyToAsync(ms);
                        var file = item.Replace(parameters.ModDirectory, string.Empty).Trim('\\').Trim('/');
                        var entry = zip.AddEntry(file, ms);
                        entry.AlternateEncoding = Encoding.UTF8;
                        entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                        fs.Close();
                        await fs.DisposeAsync();
                        streams.Add(ms);
                    }
                }
                else
                {
                    foreach (var item in files)
                    {
                        var fs = new OnDemandFileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                        var file = item.Replace(parameters.ModDirectory, string.Empty).Trim('\\').Trim('/');
                        var entry = zip.AddEntry(file, fs);
                        entry.AlternateEncoding = Encoding.UTF8;
                        entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                        streams.Add(fs);
                    }
                }
            }

            if ((parameters.ExportMods?.Any()).GetValueOrDefault())
            {
                if (parameters.DescriptorType == DescriptorType.DescriptorMod)
                {
                    var prefixDataStream = new MemoryStream(Encoding.UTF8.GetBytes(parameters.ModNameOverride));
                    streams.Add(prefixDataStream);
                    zip.AddEntry(Path.Combine(Common.Constants.ModExportPath, Shared.Constants.ModNamePrefixOverride), prefixDataStream);
                }

                foreach (var mod in parameters.ExportMods!)
                {
                    if (Directory.Exists(mod.FullPath))
                    {
                        var files = Directory.GetFiles(mod.FullPath, "*.*", SearchOption.AllDirectories);
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !ulimitBypass)
                        {
                            foreach (var item in files)
                            {
                                var file = Path.Combine(Common.Constants.ModExportPath, parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + mod.Name.GenerateValidFileName().GenerateShortFileNameHashId(8),
                                    item.Replace(mod.FullPath, string.Empty).Trim('\\').Trim('/'));
                                if (item.EndsWith(Shared.Constants.DescriptorJsonMetadata) && parameters.DescriptorType == DescriptorType.JsonMetadata)
                                {
                                    var ms = new MemoryStream();
                                    await modWriter.WriteDescriptorToStreamAsync(new ModWriterParameters { Mod = cloneMod(mod), DescriptorType = parameters.DescriptorType }, ms, true);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    var entry = zip.AddEntry(file, ms);
                                    entry.AlternateEncoding = Encoding.UTF8;
                                    entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                                    streams.Add(ms);
                                }
                                else
                                {
                                    var fs = new OnDemandFileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    var ms = new MemoryStream();
                                    await fs.CopyToAsync(ms);
                                    var entry = zip.AddEntry(file, ms);
                                    entry.AlternateEncoding = Encoding.UTF8;
                                    entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                                    fs.Close();
                                    await fs.DisposeAsync();
                                    streams.Add(ms);
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in files)
                            {
                                var file = Path.Combine(Common.Constants.ModExportPath, parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + mod.Name.GenerateValidFileName().GenerateShortFileNameHashId(8),
                                    item.Replace(mod.FullPath, string.Empty).Trim('\\').Trim('/'));
                                if (item.EndsWith(Shared.Constants.DescriptorJsonMetadata) && parameters.DescriptorType == DescriptorType.JsonMetadata)
                                {
                                    var ms = new MemoryStream();
                                    await modWriter.WriteDescriptorToStreamAsync(new ModWriterParameters { Mod = cloneMod(mod), DescriptorType = parameters.DescriptorType }, ms, true);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    var entry = zip.AddEntry(file, ms);
                                    entry.AlternateEncoding = Encoding.UTF8;
                                    entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                                    streams.Add(ms);
                                }
                                else
                                {
                                    var fs = new OnDemandFileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    var entry = zip.AddEntry(file, fs);
                                    entry.AlternateEncoding = Encoding.UTF8;
                                    entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                                    streams.Add(fs);
                                }
                            }
                        }
                    }
                    else if (File.Exists(mod.FullPath) && parameters.DescriptorType == DescriptorType.DescriptorMod) // Zips not allowed in json metadata type games (vicky 3)
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !ulimitBypass)
                        {
                            var fs = new OnDemandFileStream(mod.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            var ms = new MemoryStream();
                            await fs.CopyToAsync(ms);
                            var file = Path.Combine(Common.Constants.ModExportPath,
                                parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + Path.GetFileNameWithoutExtension(mod.FullPath).GenerateShortFileNameHashId(8) + Path.GetExtension(mod.FullPath));
                            var entry = zip.AddEntry(file, ms);
                            entry.AlternateEncoding = Encoding.UTF8;
                            entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                            fs.Close();
                            await fs.DisposeAsync();
                            streams.Add(ms);
                        }
                        else
                        {
                            var fs = new OnDemandFileStream(mod.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            var file = Path.Combine(Common.Constants.ModExportPath,
                                parameters.Mod.Name.GenerateShortFileNameHashId(4).GenerateValidFileName() + "_" + Path.GetFileNameWithoutExtension(mod.FullPath).GenerateShortFileNameHashId(8) + Path.GetExtension(mod.FullPath));
                            var entry = zip.AddEntry(file, fs);
                            entry.AlternateEncoding = Encoding.UTF8;
                            entry.AlternateEncodingUsage = ZipOption.AsNecessary;
                            streams.Add(fs);
                        }
                    }
                }
            }

            zip.SaveProgress += saveProgress;
            zip.Save(parameters.File);
            zip.SaveProgress -= saveProgress;
            if (streams.Count != 0)
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
        /// Exports the paradox launcher json202010 asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportParadoxLauncherJson202110Async(ModCollectionExporterParams parameters)
        {
            return paradoxLauncherExporter202110.ExportAsync(parameters);
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
            var importResult = ImportInternal(parameters, true, out var result);
            if (importResult)
            {
                return Task.FromResult(result);
            }

            return Task.FromResult((ICollectionImportResult)null);
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
        /// <exception cref="System.ArgumentException">Invalid descriptor type.</exception>
        public Task<ICollectionImportResult> ImportParadoxAsync(ModCollectionExporterParams parameters)
        {
            if (parameters.DescriptorType == DescriptorType.None)
            {
                throw new ArgumentException("Invalid descriptor type.");
            }

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
                                var model = JsonDISerializer.Deserialize<IModCollection>(text);
                                mapper.Map(model, importResult);
                                importResult!.ModNames = model.ModNames;
                                importResult.Descriptors = model.Mods;
                                importResult.ModIds = model.ModIds;
                                result = true;
                                break;
                            }
                        }
                        else
                        {
                            if (!importInstance)
                            {
                                var exportFileName = Path.Combine(relativePath.StartsWith(Common.Constants.ModExportPath + Path.DirectorySeparatorChar) ? parameters.ExportModDirectory : parameters.ModDirectory,
                                    relativePath.Replace(Common.Constants.ModExportPath + Path.DirectorySeparatorChar, string.Empty));
                                if (!Directory.Exists(Path.GetDirectoryName(exportFileName)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(exportFileName)!);
                                }

                                reader.WriteEntryToFile(exportFileName, ZipExtractionOpts.GetExtractionOptions());
                            }
                        }

                        processed++;
                        var perc = GetProgressPercentage(total, processed, 100);
                        if (perc.IsNotNearlyEqual(previousProgress))
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
                            var model = JsonDISerializer.Deserialize<IModCollection>(text);
                            mapper.Map(model, importResult);
                            importResult!.ModNames = model.ModNames;
                            importResult.Descriptors = model.Mods;
                            importResult.ModIds = model.ModIds;
                            result = true;
                            break;
                        }
                    }
                    else
                    {
                        if (!importInstance)
                        {
                            var exportFileName = Path.Combine(relativePath.StartsWith(Common.Constants.ModExportPath + Path.DirectorySeparatorChar) ? parameters.ExportModDirectory : parameters.ModDirectory,
                                relativePath.Replace(Common.Constants.ModExportPath + Path.DirectorySeparatorChar, string.Empty));
                            if (!Directory.Exists(Path.GetDirectoryName(exportFileName)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(exportFileName)!);
                            }

                            entry.WriteToFile(exportFileName, ZipExtractionOpts.GetExtractionOptions());
                        }
                    }

                    processed++;
                    var perc = GetProgressPercentage(total, processed, 100);
                    if (perc.IsNotNearlyEqual(previousProgress))
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
