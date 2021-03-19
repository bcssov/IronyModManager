// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2021
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
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;
using Nito.AsyncEx;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModMergeService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModMergeService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModMergeService" />
    public class ModMergeService : ModBaseService, IModMergeService
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
        private readonly IDriveInfoProvider driveInfoProvider;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The mod merge compress exporter
        /// </summary>
        private readonly IModMergeCompressExporter modMergeCompressExporter;

        /// <summary>
        /// The mod merge exporter
        /// </summary>
        private readonly IModMergeExporter modMergeExporter;

        /// <summary>
        /// The mod patch exporter
        /// </summary>
        private readonly IModPatchExporter modPatchExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModMergeService" /> class.
        /// </summary>
        /// <param name="driveInfoProvider">The drive information provider.</param>
        /// <param name="modMergeCompressExporter">The mod merge compress exporter.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="modMergeExporter">The mod merge exporter.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModMergeService(IDriveInfoProvider driveInfoProvider, IModMergeCompressExporter modMergeCompressExporter,
            ICache cache, IMessageBus messageBus, IModPatchExporter modPatchExporter,
            IModMergeExporter modMergeExporter, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.driveInfoProvider = driveInfoProvider;
            this.modMergeCompressExporter = modMergeCompressExporter;
            this.messageBus = messageBus;
            this.modMergeExporter = modMergeExporter;
            this.modPatchExporter = modPatchExporter;
        }

        #endregion Constructors

        #region Methods

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
            await PopulateModFilesInternalAsync(collectionMods);
            var totalFiles = collectionMods.Where(p => p.Files != null).SelectMany(p => p.Files).Count();
            double lastPercentage = 0;
            var processed = 0;
            long requiredSize = 0;
            foreach (var collectionMod in collectionMods.Where(p => p.Files != null))
            {
                foreach (var file in collectionMod.Files)
                {
                    requiredSize += Reader.GetFileSize(collectionMod.FullPath, file);
                    processed++;
                    var percentage = GetProgressPercentage(totalFiles, processed, 100);
                    if (lastPercentage != percentage)
                    {
                        await messageBus.PublishAsync(new ModMergeFreeSpaceCheckEvent(percentage));
                    }
                    lastPercentage = percentage;
                }
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
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                Path = modDirPath
            }, true);
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Shared.Constants.ModDirectory
            });
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = modDirPath
            });

            var mod = DIResolver.Get<IMod>();
            mod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{mergeCollectionPath}{Shared.Constants.ModExtension}";
            mod.FileName = modDirPath.Replace("\\", "/");
            mod.Name = collectionName;
            mod.Source = ModSource.Local;
            mod.Version = allMods.OrderByDescending(p => p.VersionData).FirstOrDefault() != null ? allMods.OrderByDescending(p => p.VersionData).FirstOrDefault().Version : string.Empty;
            mod.FullPath = modDirPath;
            await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
            {
                Mod = mod,
                RootDirectory = game.UserDirectory,
                Path = mod.DescriptorFile,
                LockDescriptor = CheckIfModShouldBeLocked(game, mod)
            }, true);
            Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));

            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchName = GenerateCollectionPatchName(collection.Name);
            var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));
            if (patchMod != null)
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
                    await modMergeExporter.ExportFilesAsync(new ModMergeFileExporterParameters()
                    {
                        RootModPath = collectionMod.FullPath,
                        ExportFile = file,
                        ExportPath = mod.FullPath
                    });
                    var percentage = GetProgressPercentage(totalFiles, processed, 100);
                    if (lastPercentage != percentage)
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

            IMod cloneMod(IMod mod, string fileName, int order)
            {
                var modDirPath = GetPatchModDirectory(game, fileName);
                var newMod = DIResolver.Get<IMod>();
                newMod.DescriptorFile = $"{Shared.Constants.ModDirectory}/{Path.GetFileNameWithoutExtension(fileName)}{Shared.Constants.ModExtension}";
                newMod.FileName = modDirPath.Replace("\\", "/");
                newMod.Name = !string.IsNullOrWhiteSpace(copiedNamePrefix) ? $"{copiedNamePrefix} {mod.Name}" : mod.Name;
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
                        newDependencies.Add($"{copiedNamePrefix} {item}");
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
            await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                Path = modDirPath,
            }, true);
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Shared.Constants.ModDirectory
            });
            await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
            {
                Path = modDirPath
            });

            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchName = GenerateCollectionPatchName(collection.Name);
            var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchName));

            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(1, 0));
            await PopulateModFilesInternalAsync(collectionMods);
            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(1, 100));

            var newPatchName = GenerateCollectionPatchName(collectionName);
            var renamePairs = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(patchName, newPatchName) };

            var totalFiles = (collectionMods.Where(p => p.Files != null).SelectMany(p => p.Files.Where(f => game.GameFolders.Any(s => f.StartsWith(s, StringComparison.OrdinalIgnoreCase)))).Count() * 2) + collectionMods.Count;
            double lastPercentage = 0;
            var processed = 0;
            async void modMergeCompressExporter_ProcessedFile(object sender, EventArgs e)
            {
                using var mutex = await zipLock.LockAsync();
                processed++;
                var percentage = GetProgressPercentage(totalFiles, processed, 99.98);
                if (lastPercentage != percentage)
                {
                    await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, percentage));
                }
                lastPercentage = percentage;
                mutex.Dispose();
            }
            modMergeCompressExporter.ProcessedFile += modMergeCompressExporter_ProcessedFile;

            var exportedMods = new List<IMod>();
            using var semaphore = new SemaphoreSlim(MaxZipsToMerge);
            var zipTasks = collectionMods.Where(p => p.Files != null).Select(async collectionMod =>
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
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            {
                                var streamCopy = new MemoryStream();
                                if (stream.CanSeek)
                                {
                                    stream.Seek(0, SeekOrigin.Begin);
                                }
                                await stream.CopyToAsync(streamCopy);
                                stream.Close();
                                await stream.DisposeAsync();
                                modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters()
                                {
                                    FileName = file,
                                    QueueId = queueId,
                                    Stream = streamCopy
                                });
                                streams.Add(streamCopy);
                            }
                            else
                            {
                                modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters()
                                {
                                    FileName = file,
                                    QueueId = queueId,
                                    Stream = stream
                                });
                                streams.Add(stream);
                            }
                        }
                        using var innerProgressLock = await zipLock.LockAsync();
                        processed++;
                        var innerPercentage = GetProgressPercentage(totalFiles, processed, 99.98);
                        if (lastPercentage != innerPercentage)
                        {
                            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, innerPercentage));
                        }
                        lastPercentage = innerPercentage;
                        innerProgressLock.Dispose();
                    }

                    var newMod = cloneMod(collectionMod,
                        Path.Combine(mergeCollectionPath, !string.IsNullOrWhiteSpace(copiedNamePrefix) ?
                        $"{copiedNamePrefix} {collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName() :
                        $"{collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName()),
                        collectionMods.IndexOf(collectionMod));
                    var ms = new MemoryStream();
                    await ModWriter.WriteDescriptorToStreamAsync(new ModWriterParameters()
                    {
                        Mod = newMod
                    }, ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    modMergeCompressExporter.AddFile(new ModMergeCompressExporterParameters()
                    {
                        FileName = Shared.Constants.DescriptorFile,
                        QueueId = queueId,
                        Stream = ms
                    });
                    streams.Add(ms);

                    using var outerProgressLock = await zipLock.LockAsync();
                    var outerPercentage = GetProgressPercentage(totalFiles, processed, 99.98);
                    if (lastPercentage != outerPercentage)
                    {
                        await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, outerPercentage));
                    }
                    lastPercentage = outerPercentage;
                    outerProgressLock.Dispose();

                    modMergeCompressExporter.Finalize(queueId, Path.Combine(modDirRootPath, mergeCollectionPath, !string.IsNullOrWhiteSpace(copiedNamePrefix) ?
                        $"{copiedNamePrefix} {collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName() : $"{collectionMod.Name.GenerateValidFileName()}{Shared.Constants.ZipExtension}".GenerateValidFileName()));
                    renamePairs.Add(new KeyValuePair<string, string>(collectionMod.Name, newMod.Name));
                    renamePairs.Add(new KeyValuePair<string, string>(collectionMod.DescriptorFile, newMod.DescriptorFile));
                    using var exportModLock = await zipLock.LockAsync();
                    exportedMods.Add(newMod);
                    exportModLock.Dispose();

                    var streamTasks = streams.Select(async p =>
                    {
                        p.Close();
                        await p.DisposeAsync();
                    });
                    await Task.WhenAll(streamTasks);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(zipTasks);

            modMergeCompressExporter.ProcessedFile -= modMergeCompressExporter_ProcessedFile;

            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, 99.99));
            await modPatchExporter.CopyPatchModAsync(new ModPatchExporterParameters()
            {
                RootPath = modDirRootPath,
                ModPath = EvaluatePatchNamePath(game, patchName, modDirRootPath),
                PatchPath = EvaluatePatchNamePath(game, newPatchName, modDirRootPath),
                RenamePairs = renamePairs
            });
            await messageBus.PublishAsync(new ModCompressMergeProgressEvent(2, 100));

            Cache.Invalidate(ModsCachePrefix, ConstructModsCacheKey(game, true), ConstructModsCacheKey(game, false));
            var ordered = exportedMods.OrderBy(p => p.Order).ToList();
            return ordered;
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
