﻿// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 06-18-2025
// ***********************************************************************
// <copyright file="ModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Storage.Common;
using Nito.AsyncEx;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModService" />
    public class ModService : ModBaseService, IModService
    {
        #region Fields

        /// <summary>
        /// The mod read lock
        /// </summary>
        private static readonly AsyncLock modReadLock = new();

        /// <summary>
        /// The language service
        /// </summary>
        private readonly ILanguagesService languageService;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The search parser
        /// </summary>
        private readonly IParser searchParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModService" /> class.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="searchParser">The search parser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModService(ILanguagesService languageService, IParser searchParser, ILogger logger, ICache cache, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IReader reader, IModParser modParser,
            IModWriter modWriter, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.logger = logger;
            this.searchParser = searchParser;
            this.languageService = languageService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Builds the mod URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        public virtual string BuildModUrl(IMod mod)
        {
            if (!mod.RemoteId.HasValue)
            {
                return string.Empty;
            }

            return mod.Source == ModSource.Paradox
                ? string.Format(Constants.Paradox_Url, mod.RemoteId)
                : string.Format(Constants.Steam_Url, mod.RemoteId);
        }

        /// <summary>
        /// Builds the steam URL.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns>System.String.</returns>
        public virtual string BuildSteamUrl(IMod mod)
        {
            if (mod.RemoteId.HasValue && mod.Source != ModSource.Paradox)
            {
                return string.Format(Constants.Steam_protocol_uri, BuildModUrl(mod));
            }

            return string.Empty;
        }

        /// <summary>
        /// Customs the mod directory empty asynchronous.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> CustomModDirectoryEmptyAsync(string gameType)
        {
            var game = GameService.Get().FirstOrDefault(p => p.Type.Equals(gameType));
            if (game == null)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(game.CustomModDirectory))
            {
                return true;
            }

            var path = GetModDirectoryRootPath(game);
            var result = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = path });
            return !result;
        }

        /// <summary>
        /// delete descriptors as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> DeleteDescriptorsAsync(IEnumerable<IMod> mods)
        {
            return DeleteDescriptorsInternalAsync(mods);
        }

        /// <summary>
        /// Evals the achievement compatibility.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns><c>true</c> if achievement compatible, <c>false</c> otherwise.</returns>
        public virtual bool EvalAchievementCompatibility(IEnumerable<IMod> mods)
        {
            if (mods?.Count() > 0)
            {
                var filtered = mods.Where(p => p.IsValid && p.AchievementStatus == AchievementStatus.NotEvaluated);
                if (filtered.Any())
                {
                    var game = GameService.GetSelected();
                    if (game == null)
                    {
                        return false;
                    }

                    foreach (var item in filtered)
                    {
                        if (item.Files.Any())
                        {
                            var isAchievementCompatible = !item.Files.Any(p => game.ChecksumFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase)));
                            item.AchievementStatus = isAchievementCompatible ? AchievementStatus.Compatible : AchievementStatus.NotCompatible;
                        }
                        else
                        {
                            item.AchievementStatus = AchievementStatus.AttemptedEvaluation;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Exports the mods asynchronous.
        /// </summary>
        /// <param name="enabledMods">The mods.</param>
        /// <param name="regularMods">The regular mods.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ExportModsAsync(IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, IModCollection modCollection)
        {
            var game = GameService.GetSelected();
            if (game == null || enabledMods == null || regularMods == null || modCollection == null)
            {
                return false;
            }

            var allMods = GetInstalledModsInternal(game, false);
            var mod = GeneratePatchModDescriptor(allMods, game, GenerateCollectionPatchName(modCollection.Name));
            var applyModParams = new ModWriterParameters
            {
                OtherMods = regularMods.Where(p => !enabledMods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile))).ToList(),
                EnabledMods = enabledMods,
                RootDirectory = game.UserDirectory,
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            };
            if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = mod.FullPath }))
            {
                if (modCollection.PatchModEnabled && enabledMods.Any())
                {
                    if (await ModWriter.WriteDescriptorAsync(new ModWriterParameters
                        {
                            Mod = mod,
                            RootDirectory = game.UserDirectory,
                            Path = mod.DescriptorFile,
                            LockDescriptor = CheckIfModShouldBeLocked(game, mod),
                            DescriptorType = MapDescriptorType(game.ModDescriptorType)
                        }, IsPatchModInternal(mod)))
                    {
                        applyModParams.TopPriorityMods = new List<IMod> { mod };
                        Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
                    }
                }
            }
            else
            {
                // Remove left over descriptor
                if (allMods.Any(p => p.Name.Equals(mod.Name)))
                {
                    await DeleteDescriptorsInternalAsync(new List<IMod> { mod });
                }
            }

            return await ModWriter.ApplyModsAsync(applyModParams);
        }

        /// <summary>
        /// Filters the mods.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="text">The text.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        public virtual IEnumerable<IMod> FilterMods(IEnumerable<IMod> collection, string text)
        {
            if (collection == null)
            {
                return null;
            }

            var parameters = CleanSearchResult(searchParser.Parse(languageService.GetSelected().Abrv, text));
            var nameNegateCol = parameters.Name.Any() ? parameters.Name.Where(p => p.Negate).ToList() : new List<NameFilterResult>();
            var sourceNegateCol = parameters.Source.Any() ? parameters.Source.Where(p => p.Negate).ToList() : new List<SourceTypeResult>();
            var versionNegateCol = parameters.Version.Any() ? parameters.Version.Where(p => p.Negate).ToList() : new List<VersionTypeResult>();
            var result = collection.ConditionalFilter(parameters.Name.Any(),
                    q => q.Where(p => parameters.Name.Any(x => !x.Negate ? p.Name.Contains(x.Text, StringComparison.OrdinalIgnoreCase) : !nameNegateCol.Any(a => p.Name.Contains(a.Text, StringComparison.OrdinalIgnoreCase)))))
                .ConditionalFilter(parameters.RemoteIds.Any(), q => q.Where(p => p.RemoteId.HasValue && parameters.RemoteIds.Any(x => !x.Negate
                    ? p.RemoteId.GetValueOrDefault().ToString().Contains(x.Text, StringComparison.OrdinalIgnoreCase)
                    : !p.RemoteId.GetValueOrDefault().ToString().Contains(x.Text, StringComparison.OrdinalIgnoreCase))))
                .ConditionalFilter(parameters.AchievementCompatible.Result.HasValue, q => q.Where(p =>
                {
                    var result = p.AchievementStatus == (parameters.AchievementCompatible.Result.GetValueOrDefault() ? AchievementStatus.Compatible : AchievementStatus.NotCompatible);
                    return !parameters.AchievementCompatible.Negate ? result : !result;
                }))
                .ConditionalFilter(parameters.IsSelected.Result.HasValue, q =>
                {
                    return parameters.IsSelected.Negate
                        ? q.Where(p => p.IsSelected != parameters.IsSelected.Result.GetValueOrDefault())
                        : q.Where(p => p.IsSelected == parameters.IsSelected.Result.GetValueOrDefault());
                })

                // ReSharper disable once SimplifyLinqExpressionUseAll
                .ConditionalFilter(parameters.Source.Any(), q => q.Where(p => parameters.Source.Any(s => !s.Negate ? p.Source == SourceTypeToModSource(s.Result) : !sourceNegateCol.Any(a => p.Source == SourceTypeToModSource(a.Result)))))
                .ConditionalFilter(parameters.Version.Any(), q => q.Where(p => parameters.Version.Any(s => !s.Negate ? IsValidVersion(p.VersionData, s.Version) : !versionNegateCol.Any(a => IsValidVersion(p.VersionData, a.Version)))));
            return result.ToList();
        }

        /// <summary>
        /// Finds the mod.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="text">The text.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <param name="skipIndex">Index of the skip.</param>
        /// <returns>IMod.</returns>
        public virtual IMod FindMod(IEnumerable<IMod> collection, string text, bool reverse, int? skipIndex = null)
        {
            if (collection == null)
            {
                return null;
            }

            var parameters = CleanSearchResult(searchParser.Parse(languageService.GetSelected().Abrv, text));
            var nameNegateCol = parameters.Name.Any() ? parameters.Name.Where(p => p.Negate).ToList() : new List<NameFilterResult>();
            var sourceNegateCol = parameters.Source.Any() ? parameters.Source.Where(p => p.Negate).ToList() : new List<SourceTypeResult>();
            var versionNegateCol = parameters.Version.Any() ? parameters.Version.Where(p => p.Negate).ToList() : new List<VersionTypeResult>();
            var result = !reverse ? collection.Skip(skipIndex.GetValueOrDefault()) : collection.Reverse().Skip(skipIndex.GetValueOrDefault());
            result = result.ConditionalFilter(parameters.Name.Any(),
                    q => q.Where(p => parameters.Name.Any(x => !x.Negate ? p.Name.Contains(x.Text, StringComparison.OrdinalIgnoreCase) : !nameNegateCol.Any(a => p.Name.Contains(a.Text, StringComparison.OrdinalIgnoreCase)))))
                .ConditionalFilter(parameters.RemoteIds.Any(), q => q.Where(p => p.RemoteId.HasValue && parameters.RemoteIds.Any(x => !x.Negate
                    ? p.RemoteId.GetValueOrDefault().ToString().Contains(x.Text, StringComparison.OrdinalIgnoreCase)
                    : !p.RemoteId.GetValueOrDefault().ToString().Contains(x.Text, StringComparison.OrdinalIgnoreCase))))
                .ConditionalFilter(parameters.AchievementCompatible.Result.HasValue, q => q.Where(p =>
                {
                    var result = p.AchievementStatus == (parameters.AchievementCompatible.Result.GetValueOrDefault() ? AchievementStatus.Compatible : AchievementStatus.NotCompatible);
                    return !parameters.AchievementCompatible.Negate ? result : !result;
                }))
                .ConditionalFilter(parameters.IsSelected.Result.HasValue, q =>
                {
                    return parameters.IsSelected.Negate
                        ? q.Where(p => p.IsSelected != parameters.IsSelected.Result.GetValueOrDefault())
                        : q.Where(p => p.IsSelected == parameters.IsSelected.Result.GetValueOrDefault());
                })

                // ReSharper disable once SimplifyLinqExpressionUseAll
                .ConditionalFilter(parameters.Source.Any(), q => q.Where(p => parameters.Source.Any(s => !s.Negate ? p.Source == SourceTypeToModSource(s.Result) : !sourceNegateCol.Any(a => p.Source == SourceTypeToModSource(a.Result)))))
                .ConditionalFilter(parameters.Version.Any(), q => q.Where(p => parameters.Version.Any(s => !s.Negate ? IsValidVersion(p.VersionData, s.Version) : !versionNegateCol.Any(a => IsValidVersion(p.VersionData, a.Version)))));
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get available mods as an asynchronous operation.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>A Task&lt;IEnumerable`1&gt; representing the asynchronous operation.</returns>
        public virtual async Task<IEnumerable<IMod>> GetAvailableModsAsync(IGame game)
        {
            using var mutex = await modReadLock.LockAsync();
            var result = GetInstalledModsInternal(game, true);

            // ReSharper disable once DisposeOnUsingVariable
            mutex.Dispose();
            return result;
        }

        /// <summary>
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="modName">Name of the mod.</param>
        /// <param name="path">The path.</param>
        /// <param name="isFromGame">if set to <c>true</c> [is from game].</param>
        /// <returns>Task&lt;MemoryStream&gt;.</returns>
        public virtual Task<MemoryStream> GetImageStreamAsync(string modName, string path, bool isFromGame = false)
        {
            var game = GameService.GetSelected();
            if (game == null || string.IsNullOrWhiteSpace(modName))
            {
                return Task.FromResult((MemoryStream)null);
            }

            var mods = GetInstalledModsInternal(game, false);
            return GetImageStreamAsync(mods.FirstOrDefault(p => p.Name.Equals(modName)), path, isFromGame);
        }

        /// <summary>
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="path">The path.</param>
        /// <param name="isFromGame">if set to <c>true</c> [is from game].</param>
        /// <returns>Task&lt;MemoryStream&gt;.</returns>
        public virtual Task<MemoryStream> GetImageStreamAsync(IMod mod, string path, bool isFromGame = false)
        {
            if (!isFromGame)
            {
                if (mod != null && !string.IsNullOrWhiteSpace(path))
                {
                    return Reader.GetImageStreamAsync(mod.FullPath, path);
                }
            }
            else
            {
                return Reader.GetImageStreamAsync(Path.GetDirectoryName(GameService.GetSelected().ExecutableLocation), path);
            }

            return Task.FromResult((MemoryStream)null);
        }

        /// <summary>
        /// Gets the installed mods asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IEnumerable&lt;IMod&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<IMod>> GetInstalledModsAsync(IGame game)
        {
            using var mutex = await modReadLock.LockAsync();
            if (game != null)
            {
                Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
            }

            var result = GetInstalledModsInternal(game, true);

            // ReSharper disable once DisposeOnUsingVariable
            mutex.Dispose();
            return result;
        }

        /// <summary>
        /// install mods as an asynchronous operation.
        /// </summary>
        /// <param name="statusToRetain">The status to retain.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<IReadOnlyCollection<IModInstallationResult>> InstallModsAsync(IEnumerable<IMod> statusToRetain)
        {
            using var mutex = await modReadLock.LockAsync();
            var game = GameService.GetSelected();
            if (game == null || !await ModWriter.CanWriteToModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory }))
            {
                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return null;
            }

            if (game.ModDescriptorType == ModDescriptorType.JsonMetadata && !await ModWriter.CanWriteToModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory }))
            {
                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return null;
            }

            var mods = GetInstalledModsInternal(game, false);
            var descriptors = new List<IModInstallationResult>();
            var userDirectoryMods = GetAllModDescriptors(Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory), ModSource.Local, game.ModDescriptorType);
            if (userDirectoryMods?.Count() > 0)
            {
                descriptors.AddRange(userDirectoryMods);
            }

            if (!string.IsNullOrWhiteSpace(game.CustomModDirectory))
            {
                var customMods = GetAllModDescriptors(GetModDirectoryRootPath(game), ModSource.Local, game.ModDescriptorType);
                if (customMods != null && customMods.Any())
                {
                    descriptors.AddRange(customMods);
                }
            }

            var workshopDirectoryMods = game.WorkshopDirectory.SelectMany(p => GetAllModDescriptors(p, ModSource.Steam, game.ModDescriptorType));
            if (workshopDirectoryMods.Any())
            {
                descriptors.AddRange(workshopDirectoryMods);
            }

            var filteredDescriptors = new List<IModInstallationResult>();
            var grouped = descriptors.GroupBy(p => p.ParentDirectory);
            foreach (var item in grouped)
            {
                if (item.Any())
                {
                    filteredDescriptors.AddRange(item.All(p => p.IsFile) ? item : item.Where(p => !p.IsFile));
                }
            }

            var diffs = filteredDescriptors.Where(p => p.Mod != null && !mods.Any(m => AreModsSame(m, p.Mod))).ToList();
            if (diffs.Count > 0)
            {
                var result = new List<IModInstallationResult>();
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory });
                if (game.ModDescriptorType == ModDescriptorType.JsonMetadata)
                {
                    await ModWriter.CreateModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory });
                }

                var tasks = new List<Task>();
                foreach (var diff in diffs.GroupBy(p => p.Mod.DescriptorFile))
                {
                    var installResult = diff.FirstOrDefault();
                    if (game.WorkshopDirectory.Any() && diff.Any(p => p.Path.StartsWith(game.WorkshopDirectory.FirstOrDefault() ?? string.Empty)))
                    {
                        installResult = diff.FirstOrDefault(p => p.Path.StartsWith(game.WorkshopDirectory.FirstOrDefault() ?? string.Empty));
                    }

                    // ReSharper disable once PossibleNullReferenceException
                    var localDiff = installResult.Mod;
                    if (IsPatchModInternal(localDiff))
                    {
                        continue;
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        var shouldLock = CheckIfModShouldBeLocked(game, localDiff);
                        if (statusToRetain != null && !shouldLock)
                        {
                            var mod = statusToRetain.FirstOrDefault(p => p.DescriptorFile.Equals(localDiff.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                            if (mod != null)
                            {
                                shouldLock = mod.IsLocked;
                            }
                        }

                        await ModWriter.WriteDescriptorAsync(new ModWriterParameters
                        {
                            Mod = localDiff,
                            RootDirectory = game.UserDirectory,
                            Path = localDiff.DescriptorFile,
                            LockDescriptor = shouldLock,
                            DescriptorType = MapDescriptorType(game.ModDescriptorType)
                        }, IsPatchModInternal(localDiff));
                    }));
                    installResult.Installed = true;
                    result.Add(installResult);
                }

                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                    Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
                }

                if (filteredDescriptors.Any(p => p.Invalid))
                {
                    result.AddRange(filteredDescriptors.Where(p => p.Invalid));
                }

                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return result;
            }

            if (filteredDescriptors.Any(p => p.Invalid))
            {
                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return filteredDescriptors.Where(p => p.Invalid).ToList();
            }

            // ReSharper disable once DisposeOnUsingVariable
            mutex.Dispose();
            return null;
        }

        /// <summary>
        /// lock descriptors as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> LockDescriptorsAsync(IEnumerable<IMod> mods, bool isLocked)
        {
            var game = GameService.GetSelected();
            if (game != null && mods?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var item in mods)
                {
                    // Cannot lock\unlock mandatory local zipped mods
                    if (!CheckIfModShouldBeLocked(game, item))
                    {
                        var task = ModWriter.SetDescriptorLockAsync(new ModWriterParameters { Mod = item, RootDirectory = game.UserDirectory }, isLocked);
                        item.IsLocked = isLocked;
                        tasks.Add(task);
                    }
                }

                await Task.WhenAll(tasks);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Mods the directory exists asynchronous.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ModDirectoryExistsAsync(string folder)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }

            var result = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Path.Combine(Shared.Constants.ModDirectory, folder) });
            if (!result && !string.IsNullOrEmpty(game.CustomModDirectory))
            {
                result = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = GetModDirectoryRootPath(game), Path = folder });
            }

            return result;
        }

        /// <summary>
        /// Patches the mod exists asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> PatchModExistsAsync(string collectionName)
        {
            return ModDirectoryExistsAsync(GenerateCollectionPatchName(collectionName));
        }

        /// <summary>
        /// populate mod files as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> PopulateModFilesAsync(IEnumerable<IMod> mods)
        {
            return PopulateModFilesInternalAsync(mods);
        }

        /// <summary>
        /// Purges the mod directory asynchronous.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> PurgeModDirectoryAsync(string folder)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }

            var fullPath = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory, folder);
            var exists = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = fullPath });
            if (!exists)
            {
                fullPath = Path.Combine(GetModDirectoryRootPath(game), folder);
            }

            var result = await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters { RootDirectory = fullPath }, true);
            var mods = GetInstalledModsInternal(game, false);
            if (mods.Any(p => !string.IsNullOrWhiteSpace(p.FullPath) && p.FullPath.Contains(fullPath)))
            {
                var mod = mods.Where(p => p.FullPath.Contains(fullPath));
                if (mod.Any())
                {
                    await DeleteDescriptorsInternalAsync(mod);
                }
            }

            return result;
        }

        /// <summary>
        /// Purges the mod patch asynchronous.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> PurgeModPatchAsync(string collectionName)
        {
            return PurgeModDirectoryAsync(GenerateCollectionPatchName(collectionName));
        }

        /// <summary>
        /// Queries the contains achievements.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns><c>true</c> if query contains achievements <c>false</c> otherwise.</returns>
        public virtual bool QueryContainsAchievements(string query)
        {
            var result = searchParser.Parse(languageService.GetSelected().Abrv, query);
            return result is { AchievementCompatible.Result: not null };
        }

        /// <summary>
        /// Ares the mods same.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="otherMod">The other mod.</param>
        /// <returns><c>true</c> if mods are the same, <c>false</c> otherwise.</returns>
        protected virtual bool AreModsSame(IMod mod, IMod otherMod)
        {
            if (mod == null || otherMod == null)
            {
                return false;
            }

            return mod.DescriptorFile.Equals(otherMod.DescriptorFile, StringComparison.OrdinalIgnoreCase) && mod.Version.Equals(otherMod.Version) &&
                   mod.Name.Equals(otherMod.Name) && mod.Dependencies.ListsSame(otherMod.Dependencies) && mod.RemoteId.GetValueOrDefault().Equals(otherMod.RemoteId.GetValueOrDefault()) &&
                   mod.ReplacePath.ListsSame(otherMod.ReplacePath) && mod.UserDir.ListsSame(otherMod.UserDir) && (mod.JsonId ?? string.Empty).Equals(otherMod.JsonId ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Cleans the search result.
        /// </summary>
        /// <param name="parserResult">The parser result.</param>
        /// <returns>ISearchParserResult.</returns>
        protected virtual ParsedSearchResult CleanSearchResult(ISearchParserResult parserResult)
        {
            var names = parserResult.Name.Where(p => !string.IsNullOrWhiteSpace(p.Text)).ToList();
            var remoteIds = names.Where(p => long.TryParse(p.Text, out _)).ToList();
            remoteIds.ForEach(p => names.Remove(p));
            var result = new ParsedSearchResult
            {
                AchievementCompatible = parserResult.AchievementCompatible,
                Source = parserResult.Source.Where(p => p.Result != SourceType.None).ToList(),
                Version = parserResult.Version.Where(p => p.Version != null).ToList(),
                IsSelected = parserResult.IsSelected,
                Name = names,
                RemoteIds = remoteIds
            };
            return result;
        }

        /// <summary>
        /// Gets all mod descriptors.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="modSource">The mod source.</param>
        /// <param name="modDescriptorType">Type of the mod descriptor.</param>
        /// <returns>IEnumerable&lt;IModInstallationResult&gt;.</returns>
        protected virtual IEnumerable<IModInstallationResult> GetAllModDescriptors(string path, ModSource modSource, ModDescriptorType modDescriptorType)
        {
            // Json metadata doesn't support zips to ignore them
            var files = Directory.Exists(path) && modDescriptorType == ModDescriptorType.DescriptorMod
                ? Directory.EnumerateFiles(path, $"*{Shared.Constants.ZipExtension}").Union(Directory.EnumerateFiles(path, $"*{Shared.Constants.BinExtension}"))
                : [];
            var directories = Directory.Exists(path) ? Directory.EnumerateDirectories(path) : Array.Empty<string>();
            var mods = new ConcurrentBag<IModInstallationResult>();

            static void setDescriptorPath(IMod mod, string desiredPath, string localPath)
            {
                if (desiredPath.Equals(localPath, StringComparison.OrdinalIgnoreCase))
                {
                    mod.DescriptorFile = desiredPath;
                }
                else
                {
                    if (mod.RemoteId.GetValueOrDefault() > 0)
                    {
                        mod.DescriptorFile = desiredPath;
                    }
                    else
                    {
                        mod.Source = ModSource.Local;
                        mod.DescriptorFile = localPath;
                    }
                }
            }

            string readModPrefix(string path)
            {
                if (modDescriptorType == ModDescriptorType.DescriptorMod)
                {
                    var fileInfo = Reader.GetFileInfo(path, Shared.Constants.ModNamePrefixOverride);
                    return fileInfo == null ? string.Empty : fileInfo.Content.FirstOrDefault();
                }

                return string.Empty;
            }

            void parseModFiles(string path, ModSource source, bool isDirectory, string modNamePrefix)
            {
                var result = GetModelInstance<IModInstallationResult>();
                try
                {
                    IFileInfo fileInfo;
                    if (modDescriptorType == ModDescriptorType.JsonMetadata)
                    {
                        fileInfo = Reader.GetFileInfo(path, Shared.Constants.DescriptorJsonMetadata);
                        if (fileInfo == null)
                        {
                            return;
                        }
                    }
                    else
                    {
                        fileInfo = Reader.GetFileInfo(path, Shared.Constants.DescriptorFile);
                        if (fileInfo == null)
                        {
                            fileInfo = Reader.GetFileInfo(path, $"*{Shared.Constants.ModExtension}");
                            if (fileInfo == null)
                            {
                                return;
                            }
                        }
                    }

                    var mod = Mapper.Map<IMod>(ModParser.Parse(fileInfo.Content, MapDescriptorModType(modDescriptorType)));
                    mod.Name = ModWriter.FormatPrefixModName(modNamePrefix, mod.Name);
                    if (!string.IsNullOrWhiteSpace(modNamePrefix) && mod.Dependencies != null && mod.Dependencies.Any())
                    {
                        var dependencies = mod.Dependencies;
                        var newDependencies = new List<string>();
                        dependencies.ToList().ForEach(p => newDependencies.Add(ModWriter.FormatPrefixModName(modNamePrefix, p)));
                        mod.Dependencies = newDependencies;
                    }

                    mod.FileName = path.Replace("\\", "/");
                    mod.FullPath = path.StandardizeDirectorySeparator();
                    mod.IsLocked = fileInfo.IsReadOnly;
                    mod.Source = source;
                    var cleanedPath = path;
                    if (!isDirectory)
                    {
                        cleanedPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, Path.GetFileNameWithoutExtension(path));
                    }

                    var localPath = modDescriptorType == ModDescriptorType.DescriptorMod
                        ? $"{Shared.Constants.ModDirectory}/{cleanedPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()}{Shared.Constants.ModExtension}"
                        : $"{Shared.Constants.JsonModDirectory}/{cleanedPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()}{Shared.Constants.JsonExtension}";

                    switch (mod.Source)
                    {
                        case ModSource.Local:
                            setDescriptorPath(mod, localPath, localPath);
                            break;

                        case ModSource.Steam:
                            if (!isDirectory)
                            {
                                var modParentDirectory = Path.GetDirectoryName(path);
                                mod.RemoteId = GetSteamModId(modParentDirectory);
                            }
                            else
                            {
                                mod.RemoteId = GetSteamModId(path);
                            }

                            var steamPath = modDescriptorType == ModDescriptorType.DescriptorMod
                                ? $"{Shared.Constants.ModDirectory}/{Constants.Steam_mod_id}{mod.RemoteId}{Shared.Constants.ModExtension}"
                                : $"{Shared.Constants.JsonModDirectory}/{Constants.Steam_mod_id}{mod.RemoteId}{Shared.Constants.JsonExtension}";

                            setDescriptorPath(mod, steamPath, localPath);
                            break;

                        case ModSource.Paradox:
                            if (!isDirectory)
                            {
                                var modParentDirectory = Path.GetDirectoryName(path);
                                mod.RemoteId = GetPdxModId(modParentDirectory);
                            }
                            else
                            {
                                mod.RemoteId = GetPdxModId(path);
                            }

                            var pdxPath = modDescriptorType == ModDescriptorType.DescriptorMod
                                ? $"{Shared.Constants.ModDirectory}/{Constants.Paradox_mod_id}{mod.RemoteId}{Shared.Constants.ModExtension}"
                                : $"{Shared.Constants.JsonModDirectory}/{Constants.Paradox_mod_id}{mod.RemoteId}{Shared.Constants.JsonExtension}";

                            setDescriptorPath(mod, pdxPath, localPath);
                            break;
                    }

                    result.Mod = mod;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    result.Invalid = true;
                }

                result.Path = path;
                result.IsFile = File.Exists(path);
                result.ParentDirectory = result.IsFile ? Path.GetDirectoryName(path) : path;

                mods.Add(result);
            }

            if (files.Any())
            {
                files.AsParallel().WithDegreeOfParallelism(MaxModsToProcess).ForAll(file =>
                {
                    parseModFiles(file, modSource, false, string.Empty);
                });
            }

            if (directories.Any())
            {
                directories.AsParallel().WithDegreeOfParallelism(MaxModsToProcess).ForAll(directory =>
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var modSourceOverride = directory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Contains(Constants.Paradox_mod_id, StringComparison.OrdinalIgnoreCase)
                        ? ModSource.Paradox
                        : modSource;

                    var modNamePrefix = readModPrefix(directory);
                    parseModFiles(directory, modSourceOverride, true, modNamePrefix);

                    var zipFiles = Directory.EnumerateFiles(directory, $"*{Shared.Constants.ZipExtension}").Union(Directory.EnumerateFiles(directory, $"*{Shared.Constants.BinExtension}"));
                    if (zipFiles.Any())
                    {
                        foreach (var zip in zipFiles)
                        {
                            parseModFiles(zip, modSourceOverride, false, modNamePrefix);
                        }
                    }

                    var subdirectories = Directory.GetDirectories(directory);
                    if (subdirectories.Length != 0)
                    {
                        foreach (var subdirectory in subdirectories)
                        {
                            // ReSharper disable once PossibleNullReferenceException
                            var subDirectoryModSourceOverride = subdirectory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Contains(Constants.Paradox_mod_id, StringComparison.OrdinalIgnoreCase)
                                ? ModSource.Paradox
                                : modSource;
                            parseModFiles(subdirectory, subDirectoryModSourceOverride, true, modNamePrefix);
                        }
                    }
                });
            }

            return mods.ToList();
        }

        /// <summary>
        /// Gets the steam mod identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int32.</returns>
        protected virtual long GetSteamModId(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
#pragma warning disable CA1806 // Do not ignore method results
            long.TryParse(name.Replace(Constants.Steam_mod_id, string.Empty), out var id);
#pragma warning restore CA1806 // Do not ignore method results
            return id;
        }

        /// <summary>
        /// Determines whether [is valid version] [the specified current version].
        /// </summary>
        /// <param name="currentVersion">The current version.</param>
        /// <param name="requestedVersion">The requested version.</param>
        /// <returns><c>true</c> if [is valid version] [the specified current version]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValidVersion(Shared.Version currentVersion, Shared.Version requestedVersion)
        {
            static bool validateValues(int x, int y)
            {
                return x > -1 && y > -1 && !(x == int.MaxValue || y == int.MaxValue);
            }

            if (validateValues(currentVersion.Major, requestedVersion.Major))
            {
                var result = currentVersion.Major.CompareTo(requestedVersion.Major);
                if (result != 0)
                {
                    return false;
                }
            }

            if (validateValues(currentVersion.Minor, requestedVersion.Minor))
            {
                var result = currentVersion.Minor.CompareTo(requestedVersion.Minor);
                if (result != 0)
                {
                    return false;
                }
            }

            if (validateValues(currentVersion.Build, requestedVersion.Build))
            {
                var result = currentVersion.Build.CompareTo(requestedVersion.Build);
                if (result != 0)
                {
                    return false;
                }
            }

            if (validateValues(currentVersion.Revision, requestedVersion.Revision))
            {
                var result = currentVersion.Revision.CompareTo(requestedVersion.Revision);
                if (result != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sources the type to mod source.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ModSource.</returns>
        protected virtual ModSource SourceTypeToModSource(SourceType type)
        {
            return type switch
            {
                SourceType.Paradox => ModSource.Paradox,
                SourceType.Steam => ModSource.Steam,
                _ => ModSource.Local
            };
        }

        #endregion Methods

        #region Structs

        /// <summary>
        /// Struct ParsedSearchResult
        /// </summary>
        protected struct ParsedSearchResult
        {
            #region Properties

            /// <summary>
            /// Gets or sets the achievement compatible.
            /// </summary>
            /// <value>The achievement compatible.</value>
            public BoolFilterResult AchievementCompatible { get; set; }

            /// <summary>
            /// Gets or sets the is selected.
            /// </summary>
            /// <value>The is selected.</value>
            public BoolFilterResult IsSelected { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public IList<NameFilterResult> Name { get; set; }

            /// <summary>
            /// Gets or sets the remote ids.
            /// </summary>
            /// <value>The remote ids.</value>
            public IList<NameFilterResult> RemoteIds { get; set; }

            /// <summary>
            /// Gets or sets the source.
            /// </summary>
            /// <value>The source.</value>
            public IList<SourceTypeResult> Source { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            public IList<VersionTypeResult> Version { get; set; }

            #endregion Properties
        }

        #endregion Structs
    }
}
