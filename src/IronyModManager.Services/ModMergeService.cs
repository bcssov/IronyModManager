// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 03-20-2024
// ***********************************************************************
// <copyright file="ModMergeService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Configuration;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;
using Nito.AsyncEx;
using ModSource = IronyModManager.Models.Common.ModSource;

namespace IronyModManager.Services
{
    /// <summary>
    /// The mod merge service.
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModMergeService" />
    public class ModMergeService(
        IPreferencesService preferencesService,
        IDriveInfoProvider driveInfoProvider,
        IModMergeCompressExporter modMergeCompressExporter,
        ICache cache,
        IMessageBus messageBus,
        IModPatchExporter modPatchExporter,
        IModMergeExporter modMergeExporter,
        IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
        IReader reader,
        IModWriter modWriter,
        IModParser modParser,
        IGameService gameService,
        IStorageProvider storageProvider,
        IMapper mapper) : ModBaseService(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper), IModMergeService
    {
        #region Fields

        /// <summary>
        /// The maximum zips to merge
        /// </summary>
        private const int MaxZipsToMerge = 4;

        /// <summary>
        /// The zip lock
        /// </summary>
        private static readonly AsyncLock zipLock = new();

        /// <summary>
        /// The drive information provider
        /// </summary>
        private readonly IDriveInfoProvider driveInfoProvider = driveInfoProvider;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus = messageBus;

        /// <summary>
        /// The mod merge compress exporter
        /// </summary>
        private readonly IModMergeCompressExporter modMergeCompressExporter = modMergeCompressExporter;

        /// <summary>
        /// The mod merge exporter
        /// </summary>
        private readonly IModMergeExporter modMergeExporter = modMergeExporter;

        /// <summary>
        /// The mod patch exporter
        /// </summary>
        private readonly IModPatchExporter modPatchExporter = modPatchExporter;

        /// <summary>
        /// A private readonly IPreferencesService named preferencesService.
        /// </summary>
        private readonly IPreferencesService preferencesService = preferencesService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Allow mod merge as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public virtual ValueTask<bool> AllowModMergeAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return ValueTask.FromResult(false);
            }

            var allMods = GetInstalledModsInternal(game, false).ToList();
            var collectionMods = GetCollectionMods(allMods).ToList();
            if (collectionMods.Count == 0)
            {
                return ValueTask.FromResult(false);
            }

            var mergeCollectionPath = collectionName.GenerateValidFileName();
            var modDirPath = GetPatchModDirectory(game, mergeCollectionPath);
            return ValueTask.FromResult(!collectionMods.Any(p => p.ParentDirectory.Equals(modDirPath, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Gets a merge collection mod name template.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetMergeCollectionModNameTemplate()
        {
            var result = preferencesService.Get().MergedCollectionNameTemplate;
            if (!string.IsNullOrWhiteSpace(result) && result.Contains(Common.Constants.MergeModTemplateFormat) && result.Contains(Common.Constants.MergeTemplateFormat))
            {
                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a merge collection name template.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetMergeCollectionNameTemplate()
        {
            var result = preferencesService.Get().MergeCollectionModNameTemplate;
            if (!string.IsNullOrWhiteSpace(result) && result.Contains(Common.Constants.MergeModTemplateFormat) && result.Contains(Common.Constants.MergeTemplateFormat))
            {
                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// has enough free space as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>System.Threading.Tasks.Task&lt;bool&gt;.</returns>
        public virtual async Task<bool> HasEnoughFreeSpaceAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return false;
            }

            var allMods = GetInstalledModsInternal(game, false).ToList();
            var collectionMods = GetCollectionMods(allMods).ToList();
            if (collectionMods.Count == 0)
            {
                return false;
            }

            var totalFiles = collectionMods.Count;
            double lastPercentage = 0;
            var processed = 0;
            long requiredSize = 0;
            foreach (var collectionMod in collectionMods)
            {
                requiredSize += Reader.GetTotalSize(collectionMod.FullPath);
                processed++;
                var percentage = GetProgressPercentage(totalFiles, processed, 100);
                if (lastPercentage.IsNotNearlyEqual(percentage))
                {
                    await messageBus.PublishAsync(new ModMergeFreeSpaceCheckEvent(percentage));
                }

                lastPercentage = percentage;
            }

            return driveInfoProvider.HasFreeSpace(GetPatchModDirectory(game, collectionName), requiredSize);
        }

        /// <summary>
        /// Merges the collection by files asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;IMod&gt;.</returns>
        public virtual async Task<IMod> MergeCollectionByFilesAsync(string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return null;
            }

            var allMods = GetInstalledModsInternal(game, false).ToList();
            var collectionMods = GetCollectionMods(allMods).ToList();
            if (collectionMods.Count == 0)
            {
                return null;
            }

            var mergeCollectionPath = collectionName.GenerateValidFileName();
            var modDirPath = GetPatchModDirectory(game, mergeCollectionPath);
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { Path = modDirPath }, true);
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory });
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = modDirPath });
            if (game.ModDescriptorType == ModDescriptorType.JsonMetadata)
            {
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory });
            }

            var mod = DIResolver.Get<IMod>();
            if (game.ModDescriptorType == ModDescriptorType.DescriptorMod)
            {
                mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{mergeCollectionPath}{Shared.Constants.ModExtension}";
            }
            else
            {
                mod.DescriptorFile = $"{Shared.Constants.JsonModDirectory}/{mergeCollectionPath}{Shared.Constants.JsonExtension}";
            }

            mod.FileName = modDirPath.Replace("\\", "/");
            mod.Name = collectionName;
            mod.Source = ModSource.Local;
            mod.Version = allMods.OrderByDescending(p => p.VersionData)!.FirstOrDefault() != null ? allMods.OrderByDescending(p => p.VersionData)!.FirstOrDefault()!.Version : string.Empty;
            mod.FullPath = modDirPath;
            if (collectionMods.Any(p => p.UserDir != null && p.UserDir.Any()))
            {
                mod.UserDir = collectionMods.Where(p => p.UserDir != null && p.UserDir.Any()).SelectMany(p => p.UserDir).ToList();
            }

            if (collectionMods.Any(p => p.ReplacePath != null && p.ReplacePath.Any()))
            {
                mod.ReplacePath = collectionMods.Where(p => p.ReplacePath != null && p.ReplacePath.Any()).SelectMany(p => p.ReplacePath).ToList();
            }

            if (collectionMods.Any(p => p.AdditionalData != null))
            {
                mod.AdditionalData = collectionMods.Where(p => p.AdditionalData != null).SelectMany(p => p.AdditionalData).ToLookup(p => p.Key, p => p.Value).ToDictionary(p => p.Key, p => p.First());
            }

            await ModWriter.WriteDescriptorAsync(new ModWriterParameters
            {
                Mod = mod,
                RootDirectory = game.UserDirectory,
                Path = mod.DescriptorFile,
                LockDescriptor = CheckIfModShouldBeLocked(game, mod),
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            }, true);
            Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [GetModsCacheKey(true), GetModsCacheKey(false)] });

            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchName = GenerateCollectionPatchName(collection!.Name);
            var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
            if (patchMod == null)
            {
                if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = GetPatchModDirectory(game, patchName) }))
                {
                    patchMod = GeneratePatchModDescriptor(allMods, game, patchName);
                }
            }

            if (patchMod != null && collection.PatchModEnabled)
            {
                collectionMods.Add(patchMod);
            }

            await messageBus.PublishAsync(new ModFileMergeProgressEvent(1, 0));
            await PopulateModFilesInternalAsync(collectionMods);
            await messageBus.PublishAsync(new ModFileMergeProgressEvent(1, 100));

            var totalFiles = collectionMods.Where(p => p.Files != null).SelectMany(p => p.Files.Where(f => game.GameFolders.Any(s => f.StartsWith(s, StringComparison.OrdinalIgnoreCase)))).Count();
            double lastPercentage = 0;
            var processed = 0;
            foreach (var collectionMod in collectionMods.Where(p => p.Files != null))
            {
                foreach (var file in collectionMod.Files.Where(p => game.GameFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase))))
                {
                    processed++;
                    await modMergeExporter.ExportFilesAsync(new ModMergeFileExporterParameters { RootModPath = collectionMod.FullPath, ExportFile = file, ExportPath = mod.FullPath });
                    var percentage = GetProgressPercentage(totalFiles, processed, 100);
                    if (lastPercentage.IsNotNearlyEqual(percentage))
                    {
                        await messageBus.PublishAsync(new ModFileMergeProgressEvent(2, percentage));
                    }

                    lastPercentage = percentage;
                }
            }

            return mod;
        }

        /// <summary>
        /// merge compress collection as an asynchronous operation.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="copiedNamePrefix">The copied name prefix.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        public virtual async Task<IEnumerable<IMod>> MergeCompressCollectionAsync(string collectionName, string copiedNamePrefix)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(collectionName))
            {
                return null;
            }

            var modTemplate = GetMergeCollectionModNameTemplate();

            IMod cloneMod(IMod mod, string fileName, int order)
            {
                var modDirPath = GetPatchModDirectory(game, fileName);
                var newMod = DIResolver.Get<IMod>();
                newMod.DescriptorFile = game.ModDescriptorType == ModDescriptorType.DescriptorMod
                    ? $"{Shared.Constants.ModDirectory}/{Path.GetFileNameWithoutExtension(fileName)}{Shared.Constants.ModExtension}"
                    : $"{Shared.Constants.JsonModDirectory}/{Path.GetFileNameWithoutExtension(fileName)}{Shared.Constants.JsonExtension}";

                newMod.FileName = modDirPath.Replace("\\", "/");
                if (!string.IsNullOrWhiteSpace(modTemplate))
                {
                    newMod.Name = IronyFormatter.Format(modTemplate, new { mod.Name, Merged = copiedNamePrefix });
                }
                else
                {
                    newMod.Name = !string.IsNullOrWhiteSpace(copiedNamePrefix) ? $"{copiedNamePrefix} {mod.Name}" : mod.Name;
                }

                newMod.Source = ModSource.Local;
                newMod.Version = mod.Version;
                newMod.FullPath = modDirPath;
                newMod.RemoteId = mod.RemoteId;
                newMod.Picture = mod.Picture;
                newMod.ReplacePath = mod.ReplacePath;
                newMod.Tags = mod.Tags;
                newMod.UserDir = mod.UserDir;
                newMod.Order = order;
                var dependencies = mod.Dependencies;
                if (dependencies != null && dependencies.Any())
                {
                    var newDependencies = new List<string>();
                    foreach (var item in dependencies)
                    {
                        newDependencies.Add(!string.IsNullOrWhiteSpace(modTemplate) ? IronyFormatter.Format(modTemplate, new { Name = item, Merged = copiedNamePrefix }) : $"{copiedNamePrefix} {item}");
                    }

                    newMod.Dependencies = newDependencies;
                }

                return newMod;
            }

            var allMods = GetInstalledModsInternal(game, false).ToList();
            var collectionMods = GetCollectionMods(allMods).ToList();
            if (collectionMods.Count == 0)
            {
                return null;
            }

            var mergeCollectionPath = collectionName.GenerateValidFileName();
            var modDirPath = GetPatchModDirectory(game, mergeCollectionPath);
            var modDirRootPath = GetModDirectoryRootPath(game);
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { Path = modDirPath }, true);
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory });
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { Path = modDirPath });
            if (game.ModDescriptorType == ModDescriptorType.JsonMetadata)
            {
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory });
            }

            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchName = GenerateCollectionPatchName(collection!.Name);

            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(1, 0));
            await PopulateModFilesInternalAsync(collectionMods);
            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(1, 100));

            var newPatchName = GenerateCollectionPatchName(collectionName);
            var renamePairs = new List<KeyValuePair<string, string>> { new(patchName, newPatchName) };

            var totalFiles = (collectionMods.Where(p => p.Files != null).SelectMany(p => p.Files.Where(f => game.GameFolders.Any(s => f.StartsWith(s, StringComparison.OrdinalIgnoreCase)))).Count() * 2) + collectionMods.Count;
            double lastPercentage = 0;
            var processed = 0;

            async void ModMergeCompressExporterProcessedFile(object sender, EventArgs e)
            {
                using var mutex = await zipLock.LockAsync();
                processed++;
                var percentage = GetProgressPercentage(totalFiles, processed, 99.98);
                if (lastPercentage.IsNotNearlyEqual(percentage))
                {
                    await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, percentage));
                }

                lastPercentage = percentage;
                mutex.Dispose();
            }

            modMergeCompressExporter.ProcessedFile += ModMergeCompressExporterProcessedFile;

            var ulimitBypass = DIResolver.Get<IDomainConfiguration>().GetOptions().OSXOptions.UseFileStreams;
            var exportedMods = new List<IMod>();
            using var semaphore = new SemaphoreSlim(MaxZipsToMerge);
            var zipTasks = collectionMods.AsParallel().Where(p => p.Files != null).Select(async collectionMod =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var queueId = modMergeCompressExporter.Start();
                    var streams = new List<Stream>();
                    foreach (var file in collectionMod.Files.Where(p => game.GameFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase))))
                    {
                        // OSX seems to have a very low ulimit (according to #183) -- so don't punish other OS users by placing stuff in memory
                        var stream = Reader.GetStream(collectionMod.FullPath, file);
                        if (stream != null)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !ulimitBypass)
                            {
                                var streamCopy = new MemoryStream();
                                if (stream.CanSeek)
                                {
                                    stream.Seek(0, SeekOrigin.Begin);
                                }

                                await stream.CopyToAsync(streamCopy);
                                stream.Close();
                                await stream.DisposeAsync();
                                modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters { FileName = file, QueueId = queueId, Stream = streamCopy });
                                streams.Add(streamCopy);
                            }
                            else
                            {
                                modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters { FileName = file, QueueId = queueId, Stream = stream });
                                streams.Add(stream);
                            }
                        }

                        using var innerProgressLock = await zipLock.LockAsync();
                        processed++;
                        var innerPercentage = GetProgressPercentage(totalFiles, processed, 99.98);
                        if (lastPercentage.IsNotNearlyEqual(innerPercentage))
                        {
                            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, innerPercentage));
                        }

                        lastPercentage = innerPercentage;
                        innerProgressLock.Dispose();
                    }

                    string path;
                    if (!string.IsNullOrWhiteSpace(modTemplate))
                    {
                        path = $"{IronyFormatter.Format(modTemplate, new { Name = collectionMod.Name.GenerateValidFileName(), Merged = copiedNamePrefix })}{Shared.Constants.ZipExtension}".GenerateValidFileName();
                    }
                    else
                    {
                        path = !string.IsNullOrWhiteSpace(copiedNamePrefix)
                            ? $"{copiedNamePrefix} {collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName()
                            : $"{collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName();
                    }

                    var newMod = cloneMod(collectionMod,
                        Path.Combine(mergeCollectionPath, path),
                        collectionMods.IndexOf(collectionMod));
                    var ms = new MemoryStream();
                    await ModWriter.WriteDescriptorToStreamAsync(new ModWriterParameters { Mod = newMod, DescriptorType = MapDescriptorType(game.ModDescriptorType) }, ms, true);
                    ms.Seek(0, SeekOrigin.Begin);
                    if (game.ModDescriptorType != ModDescriptorType.DescriptorMod)
                    {
                        modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters { FileName = Shared.Constants.DescriptorJsonMetadata, QueueId = queueId, Stream = ms });
                    }
                    else
                    {
                        modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters { FileName = Shared.Constants.DescriptorFile, QueueId = queueId, Stream = ms });
                    }

                    streams.Add(ms);

                    using var outerProgressLock = await zipLock.LockAsync();
                    var outerPercentage = GetProgressPercentage(totalFiles, processed, 99.98);
                    if (lastPercentage.IsNotNearlyEqual(outerPercentage))
                    {
                        await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, outerPercentage));
                    }

                    lastPercentage = outerPercentage;
                    outerProgressLock.Dispose();

                    modMergeCompressExporter.Finalize(queueId,
                        Path.Combine(modDirRootPath, mergeCollectionPath, path));
                    renamePairs.Add(new KeyValuePair<string, string>(collectionMod.Name, newMod.Name));
                    renamePairs.Add(new KeyValuePair<string, string>(collectionMod.DescriptorFile, newMod.DescriptorFile));
                    using var exportModLock = await zipLock.LockAsync();
                    exportedMods.Add(newMod);
                    var streamTasks = streams.Select(async p =>
                    {
                        p.Close();
                        await p.DisposeAsync();
                    });
                    await Task.WhenAll(streamTasks);
                    exportModLock.Dispose();
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(zipTasks);

            modMergeCompressExporter.ProcessedFile -= ModMergeCompressExporterProcessedFile;

            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, 99.99));
            await modPatchExporter.CopyPatchModAsync(new ModPatchExporterParameters
            {
                RootPath = modDirRootPath, ModPath = EvaluatePatchNamePath(game, patchName, modDirRootPath), PatchPath = EvaluatePatchNamePath(game, newPatchName, modDirRootPath), RenamePairs = renamePairs
            });
            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, 100));

            Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [GetModsCacheKey(true), GetModsCacheKey(false)] });
            var ordered = exportedMods.OrderBy(p => p.Order).ToList();
            return ordered;
        }

        /// <summary>
        /// Saves a merge collection mod name teplate.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>A string.</returns>
        public bool SaveMergeCollectionModNameTeplate(string template)
        {
            if ((!string.IsNullOrWhiteSpace(template) && template.Contains(Common.Constants.MergeModTemplateFormat) && template.Contains(Common.Constants.MergeTemplateFormat)) || string.IsNullOrWhiteSpace(template))
            {
                var preferences = preferencesService.Get();
                preferences.MergeCollectionModNameTemplate = template;
                return preferencesService.Save(preferences);
            }

            return false;
        }

        /// <summary>
        /// Saves a merged collection name template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>A string.</returns>
        public bool SaveMergedCollectionNameTemplate(string template)
        {
            if ((!string.IsNullOrWhiteSpace(template) && template.Contains(Common.Constants.MergeModTemplateFormat) && template.Contains(Common.Constants.MergeTemplateFormat)) || string.IsNullOrWhiteSpace(template))
            {
                var preferences = preferencesService.Get();
                preferences.MergedCollectionNameTemplate = template;
                return preferencesService.Save(preferences);
            }

            return false;
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Int32.</returns>
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

        #endregion Methods
    }
}
