// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
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
using IronyModManager.IO.Common.FileSystem;
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
    /// <remarks>Initializes a new instance of the <see cref="ModService" /> class.</remarks>
    public class ModService(
        ILanguagesService languageService,
        IParser searchParser,
        ILogger logger,
        ICache cache,
        IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
        IReader reader,
        IModParser modParser,
        IModWriter modWriter,
        IGameService gameService,
        IStorageProvider storageProvider,
        IMapper mapper,
        IFileSystemStateProbe fileSystemStateProbe,
        IGameStateSafetyService gameStateSafetyService,
        Func<IMod> modFactory) : ModBaseService(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper), IModService
    {
        #region Fields

        /// <summary>
        /// The mod read lock
        /// </summary>
        private static readonly AsyncLock modReadLock = new();

        /// <summary>
        /// The language service
        /// </summary>
        private readonly ILanguagesService languageService = languageService;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        /// <summary>
        /// The filesystem state probe.
        /// </summary>
        private readonly IFileSystemStateProbe fileSystemStateProbe = fileSystemStateProbe;

        /// <summary>
        /// The per-game filesystem safety state.
        /// </summary>
        private readonly IGameStateSafetyService gameStateSafetyService = gameStateSafetyService;

        /// <summary>
        /// Creates runtime mod representations.
        /// </summary>
        private readonly Func<IMod> modFactory = modFactory;

        /// <summary>
        /// The search parser
        /// </summary>
        private readonly IParser searchParser = searchParser;

        #endregion Fields

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
        /// <returns>The collection apply result.</returns>
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
            if (mods?.Any(p => p.IsVirtual) == true)
            {
                return Task.FromResult(false);
            }

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
                var filtered = mods.Where(p => !p.IsVirtual && p.IsValid && p.AchievementStatus == AchievementStatus.NotEvaluated);
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
        public virtual async Task<ModApplyResult> ExportModsAsync(IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, IModCollection modCollection)
        {
            var game = GameService.GetSelected();
            var skippedVirtualMods = enabledMods?.Count(p => p.IsVirtual) ?? 0;
            return await gameStateSafetyService.ExecuteMutationAsync(game,
                async () => new ModApplyResult
                {
                    Succeeded = await ExportModsInternalAsync(game, enabledMods, regularMods, modCollection),
                    SkippedVirtualMods = skippedVirtualMods
                }, new ModApplyResult(), "Apply collection load order");
        }

        private async Task<bool> ExportModsInternalAsync(IGame game, IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, IModCollection modCollection)
        {
            if (game == null || enabledMods == null || regularMods == null || modCollection == null)
            {
                return false;
            }

            enabledMods = [.. enabledMods.Where(p => !p.IsVirtual)];
            regularMods = [.. regularMods.Where(p => !p.IsVirtual)];

            var allMods = GetInstalledModsInternal(game, false);
            var mod = GeneratePatchModDescriptor(allMods, game, GenerateCollectionPatchName(modCollection.Name));
            var applyModParams = new ModWriterParameters
            {
                OtherMods = [.. regularMods.Where(p => !enabledMods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile)))],
                EnabledMods = enabledMods,
                RootDirectory = game.UserDirectory,
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            };
            if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters { RootDirectory = mod.FullPath }))
            {
                if (modCollection.PatchModEnabled && enabledMods.Count != 0)
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
                        applyModParams.TopPriorityMods = [mod];
                        Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [GetModsCacheKey(true), GetModsCacheKey(false)] });
                    }
                }
            }
            else
            {
                // Remove left over descriptor
                if (allMods.Any(p => p.Name.Equals(mod.Name)))
                {
                    await DeleteDescriptorsInternalAsync([mod]);
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

            static bool matches(string value, IReadOnlyList<string> pos, IReadOnlyList<string> neg)
            {
                var posOk = pos.Count == 0
                            || (value != null && pos.Any(t => value.Contains(t, StringComparison.OrdinalIgnoreCase)));
                var negOk = neg.Count == 0
                            || value == null
                            || neg.All(t => !value.Contains(t, StringComparison.OrdinalIgnoreCase));
                return posOk && negOk;
            }

            var parameters = CleanSearchResult(searchParser.Parse(languageService.GetSelected().Abrv, text));

            var hasName = parameters.Name.Any();
            var namePos = parameters.Name.Where(t => !t.Negate).Select(t => t.Text).ToList();
            var nameNeg = parameters.Name.Where(t => t.Negate).Select(t => t.Text).ToList();

            var hasId = parameters.RemoteIds.Any();
            var idPos = parameters.RemoteIds.Where(t => !t.Negate).Select(t => t.Text).ToList();
            var idNeg = parameters.RemoteIds.Where(t => t.Negate).Select(t => t.Text).ToList();
            var hasAnyFilter = hasName || hasId;

            var sourceNegateCol = parameters.Source.Any() ? parameters.Source.Where(p => p.Negate).ToList() : [];
            var versionNegateCol = parameters.Version.Any() ? parameters.Version.Where(p => p.Negate).ToList() : [];
            var result = (hasAnyFilter
                    ? collection.Where(p =>
                    {
                        var nameMatch = hasName && matches(p.Name, namePos, nameNeg);
                        var idStr = p.RemoteId?.ToString();
                        var idMatch = hasId && matches(idStr, idPos, idNeg);
                        return nameMatch || idMatch;
                    })
                    : collection)
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
                .ConditionalFilter(parameters.Source.Any(), q => q.Where(p => parameters.Source.Any(s => !s.Negate ? p.Source == SourceTypeToModSource(s.Result) : sourceNegateCol.All(a => p.Source != SourceTypeToModSource(a.Result)))))
                .ConditionalFilter(parameters.Version.Any(), q => q.Where(p => parameters.Version.Any(s => !s.Negate ? IsValidVersion(p.VersionData, s.Version) : !versionNegateCol.Any(a => IsValidVersion(p.VersionData, a.Version)))));
            return [.. result];
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

            static bool matches(string value, IReadOnlyList<string> pos, IReadOnlyList<string> neg)
            {
                var posOk = pos.Count == 0
                            || (value != null && pos.Any(t => value.Contains(t, StringComparison.OrdinalIgnoreCase)));
                var negOk = neg.Count == 0
                            || value == null
                            || neg.All(t => !value.Contains(t, StringComparison.OrdinalIgnoreCase));
                return posOk && negOk;
            }

            var parameters = CleanSearchResult(searchParser.Parse(languageService.GetSelected().Abrv, text));

            var hasName = parameters.Name.Any();
            var namePos = parameters.Name.Where(t => !t.Negate).Select(t => t.Text).ToList();
            var nameNeg = parameters.Name.Where(t => t.Negate).Select(t => t.Text).ToList();

            var hasId = parameters.RemoteIds.Any();
            var idPos = parameters.RemoteIds.Where(t => !t.Negate).Select(t => t.Text).ToList();
            var idNeg = parameters.RemoteIds.Where(t => t.Negate).Select(t => t.Text).ToList();
            var hasAnyFilter = hasName || hasId;

            var sourceNegateCol = parameters.Source.Any() ? parameters.Source.Where(p => p.Negate).ToList() : [];
            var versionNegateCol = parameters.Version.Any() ? parameters.Version.Where(p => p.Negate).ToList() : [];

            var result = !reverse ? collection.Skip(skipIndex.GetValueOrDefault()) : [.. collection.Reverse().Skip(skipIndex.GetValueOrDefault())];
            result = (hasAnyFilter
                    ? result.Where(p =>
                    {
                        var nameMatch = hasName && matches(p.Name, namePos, nameNeg);
                        var idStr = p.RemoteId?.ToString();
                        var idMatch = hasId && matches(idStr, idPos, idNeg);
                        return nameMatch || idMatch;
                    })
                    : result)
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
                .ConditionalFilter(parameters.Source.Any(), q => q.Where(p => parameters.Source.Any(s => !s.Negate ? p.Source == SourceTypeToModSource(s.Result) : sourceNegateCol.All(a => p.Source != SourceTypeToModSource(a.Result)))))
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
            var cacheParameters = new CacheGetParameters { Region = ModsCacheRegion, Prefix = game.Type, Key = GetModsCacheKey(true) };
            var previousMods = Cache.Get<IEnumerable<IMod>>(cacheParameters);
            if (gameStateSafetyService.IsLocked(game))
            {
                return previousMods ?? [];
            }

            IEnumerable<IMod> result;
            try
            {
                result = GetInstalledModsInternal(game, true);
            }
            catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                gameStateSafetyService.Lock(game, GameStateLockReason.DiscoveryUnavailable, "Read available mods", exception);
                result = previousMods ?? [];
            }

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
        public virtual async Task<MemoryStream> GetImageStreamAsync(IMod mod, string path, bool isFromGame = false)
        {
            if (mod?.IsVirtual == true)
            {
                return null;
            }

            var game = GameService.GetSelected();
            if (!isFromGame)
            {
                if (mod != null && !string.IsNullOrWhiteSpace(path))
                {
                    return await Reader.GetImageStreamAsync(mod.FullPath, path);
                }
            }
            else
            {
                return await Reader.GetImageStreamAsync(Path.GetDirectoryName(game.ExecutableLocation), path);
            }

            return null;
        }

        /// <summary>
        /// Gets the installed mods asynchronous.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;IEnumerable&lt;IMod&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<IMod>> GetInstalledModsAsync(IGame game)
        {
            return (await RefreshInstalledModsAsync(game)).Mods;
        }

        /// <inheritdoc />
        public virtual async Task<InstalledModsResult> RefreshInstalledModsAsync(IGame game)
        {
            return await RefreshInstalledModsInternalAsync(game);
        }

        /// <inheritdoc />
        public virtual async Task<InstalledModsResult> RevalidateInstalledModsAsync(IGame game, GameStateLockInfo revalidationLock)
        {
            return await RefreshInstalledModsInternalAsync(game, revalidationLock);
        }

        private async Task<InstalledModsResult> RefreshInstalledModsInternalAsync(IGame game, GameStateLockInfo revalidationLock = null)
        {
            using var mutex = await modReadLock.LockAsync();
            ArgumentNullException.ThrowIfNull(game);

            var regularCacheKey = GetModsCacheKey(true);
            var allCacheKey = GetModsCacheKey(false);
            var previousRegularMods = Cache.Get<IEnumerable<IMod>>(new CacheGetParameters { Region = ModsCacheRegion, Prefix = game.Type, Key = regularCacheKey });
            var previousAllMods = Cache.Get<IEnumerable<IMod>>(new CacheGetParameters { Region = ModsCacheRegion, Prefix = game.Type, Key = allCacheKey });

            var isRevalidation = revalidationLock != null;
            if (isRevalidation
                    ? !gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock)
                    : gameStateSafetyService.IsLocked(game))
            {
                return new InstalledModsResult { IsAuthoritative = false, Mods = previousRegularMods ?? [] };
            }

            var sourceFailure = GetDiscoverySourceFailure(game);
            if (sourceFailure != null)
            {
                var reason = sourceFailure.State == FileSystemPathState.Missing
                    ? GameStateLockReason.ExpectedSourceMissing
                    : GameStateLockReason.DiscoveryUnavailable;
                if (isRevalidation)
                {
                    gameStateSafetyService.LockRevalidationFailure(game, revalidationLock, reason, sourceFailure.Path);
                }
                else
                {
                    gameStateSafetyService.Lock(game, reason, sourceFailure.Path);
                }

                return new InstalledModsResult { IsAuthoritative = false, Mods = previousRegularMods ?? [] };
            }

            try
            {
                Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [regularCacheKey, allCacheKey] });
                var result = GetInstalledModsInternal(game, true).ToList();
                var stillAuthoritative = isRevalidation
                    ? gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock)
                    : !gameStateSafetyService.IsLocked(game);
                if (!stillAuthoritative)
                {
                    RestoreInstalledModsCache(game, regularCacheKey, allCacheKey, previousRegularMods, previousAllMods);
                    return new InstalledModsResult { IsAuthoritative = false, Mods = previousRegularMods ?? [] };
                }

                return new InstalledModsResult { IsAuthoritative = true, Mods = result };
            }
            catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                RestoreInstalledModsCache(game, regularCacheKey, allCacheKey, previousRegularMods, previousAllMods);
                if (isRevalidation)
                {
                    gameStateSafetyService.LockRevalidationFailure(game, revalidationLock,
                        GameStateLockReason.DiscoveryUnavailable, "Installed mod discovery", exception);
                }
                else
                {
                    gameStateSafetyService.Lock(game, GameStateLockReason.DiscoveryUnavailable,
                        "Installed mod discovery", exception);
                }

                return new InstalledModsResult { IsAuthoritative = false, Mods = previousRegularMods ?? [] };
            }
        }

        private void RestoreInstalledModsCache(IGame game, string regularCacheKey, string allCacheKey,
            IEnumerable<IMod> previousRegularMods, IEnumerable<IMod> previousAllMods)
        {
            Cache.Invalidate(new CacheInvalidateParameters
                { Region = ModsCacheRegion, Prefix = game.Type, Keys = [regularCacheKey, allCacheKey] });
            if (previousRegularMods != null)
            {
                Cache.Set(new CacheAddParameters<IEnumerable<IMod>>
                    { Region = ModsCacheRegion, Prefix = game.Type, Key = regularCacheKey, Value = previousRegularMods });
            }

            if (previousAllMods != null)
            {
                Cache.Set(new CacheAddParameters<IEnumerable<IMod>>
                    { Region = ModsCacheRegion, Prefix = game.Type, Key = allCacheKey, Value = previousAllMods });
            }
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<IMod> ResolveCollectionMods(IEnumerable<IMod> installedMods, IModCollection collection, IEnumerable<IMod> previousMods = null)
        {
            var installed = installedMods?.Where(p => !p.IsVirtual).ToList() ?? [];
            var previous = previousMods?.ToList() ?? [];
            var result = new List<IMod>();
            var descriptors = collection?.Mods?.ToList() ?? [];
            var paths = collection?.ModPaths?.ToList() ?? [];
            var names = collection?.ModNames?.ToList() ?? [];
            var ids = collection?.ModIds?.ToList() ?? [];

            for (var index = 0; index < descriptors.Count; index++)
            {
                var descriptor = descriptors[index] ?? string.Empty;
                var path = paths.Count == descriptors.Count ? paths[index] ?? string.Empty : string.Empty;
                var mod = installed.FirstOrDefault(p => string.Equals(p.DescriptorFile, descriptor, StringComparison.OrdinalIgnoreCase));
                mod ??= !string.IsNullOrWhiteSpace(path)
                    ? installed.FirstOrDefault(p => string.Equals(p.FullPath, path, StringComparison.OrdinalIgnoreCase))
                    : null;

                if (mod != null)
                {
                    mod.IsSelected = true;
                    result.Add(mod);
                    continue;
                }

                var oldMod = previous.FirstOrDefault(p => string.Equals(p.DescriptorFile, descriptor, StringComparison.OrdinalIgnoreCase));
                oldMod ??= !string.IsNullOrWhiteSpace(path)
                    ? previous.FirstOrDefault(p => string.Equals(p.FullPath, path, StringComparison.OrdinalIgnoreCase))
                    : null;

                var sourceInfo = ids.Count == descriptors.Count ? ids[index] : null;
                var virtualMod = modFactory();
                virtualMod.AchievementStatus = AchievementStatus.AttemptedEvaluation;
                virtualMod.Dependencies = oldMod?.Dependencies ?? [];
                virtualMod.DescriptorFile = descriptor;
                virtualMod.FileName = oldMod?.FileName ?? string.Empty;
                virtualMod.Files = [];
                virtualMod.FullPath = !string.IsNullOrWhiteSpace(path) ? path : oldMod?.FullPath ?? string.Empty;
                virtualMod.Game = collection.Game;
                virtualMod.IsLocked = true;
                virtualMod.IsSelected = true;
                virtualMod.IsValid = false;
                virtualMod.IsVirtual = true;
                virtualMod.JsonId = oldMod?.JsonId ?? string.Empty;
                virtualMod.Name = names.Count == descriptors.Count ? names[index] ?? string.Empty : oldMod?.Name ?? descriptor;
                virtualMod.Order = index + 1;
                virtualMod.RemoteId = sourceInfo?.SteamId ?? sourceInfo?.ParadoxId ?? oldMod?.RemoteId;
                virtualMod.Source = sourceInfo?.SteamId != null ? ModSource.Steam : sourceInfo?.ParadoxId != null ? ModSource.Paradox : oldMod?.Source ?? ModSource.Local;
                virtualMod.Version = oldMod?.Version ?? string.Empty;
                result.Add(virtualMod);
            }

            return result;
        }

        private FileSystemPathCheckResult GetDiscoverySourceFailure(IGame game)
        {
            var descriptorDirectory = Path.Combine(game.UserDirectory,
                game.ModDescriptorType == ModDescriptorType.DescriptorMod ? Shared.Constants.ModDirectory : Shared.Constants.JsonModDirectory);
            var sources = new List<string> { descriptorDirectory };
            sources.AddRange(game.WorkshopDirectory?.Where(p => !string.IsNullOrWhiteSpace(p)) ?? []);
            if (!string.IsNullOrWhiteSpace(game.CustomModDirectory))
            {
                sources.Add(game.CustomModDirectory);
            }

            foreach (var source in sources.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var result = fileSystemStateProbe.CheckDirectory(source);
                if (result.State != FileSystemPathState.Available)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// install mods as an asynchronous operation.
        /// </summary>
        /// <param name="statusToRetain">The status to retain.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<IReadOnlyCollection<IModInstallationResult>> InstallModsAsync(IEnumerable<IMod> statusToRetain)
        {
            var game = GameService.GetSelected();
            return InstallModsInternalAsync(game, statusToRetain);
        }

        /// <inheritdoc />
        public virtual async Task<bool> InstallModsAsync(IGame game, IEnumerable<IMod> statusToRetain,
            GameStateLockInfo revalidationLock)
        {
            if (!gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock))
            {
                return false;
            }

            await InstallModsInternalAsync(game, statusToRetain, revalidationLock);
            return gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock);
        }

        private async Task<IReadOnlyCollection<IModInstallationResult>> InstallModsInternalAsync(IGame game,
            IEnumerable<IMod> statusToRetain, GameStateLockInfo revalidationLock = null)
        {
            using var mutex = await modReadLock.LockAsync();
            if (game == null)
            {
                return null;
            }

            bool ownsRevalidation() => revalidationLock == null ||
                                        gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock);
            if (!ownsRevalidation())
            {
                return null;
            }

            if (revalidationLock != null)
            {
                var sourceFailure = GetDiscoverySourceFailure(game);
                if (sourceFailure != null)
                {
                    var reason = sourceFailure.State == FileSystemPathState.Missing
                        ? GameStateLockReason.ExpectedSourceMissing
                        : GameStateLockReason.DiscoveryUnavailable;
                    gameStateSafetyService.LockRevalidationFailure(game, revalidationLock, reason, sourceFailure.Path);
                    return null;
                }
            }

            bool canWrite;
            try
            {
                async Task<bool> validateOutput()
                {
                    return ownsRevalidation() &&
                           await ModWriter.CanWriteToModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory }) &&
                           (game.ModDescriptorType is not (ModDescriptorType.JsonMetadata or ModDescriptorType.JsonMetadataV2) ||
                            await ModWriter.CanWriteToModDirectoryAsync(new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory }));
                }

                canWrite = revalidationLock != null
                    ? await validateOutput()
                    : await gameStateSafetyService.ExecuteMutationAsync(game, validateOutput, false, "Validate mod descriptor output");
            }
            catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                LockInstallFailure(game, revalidationLock, GameStateLockReason.WriteAccessFailure,
                    "Validate mod descriptor output", exception);
                return null;
            }

            if (!canWrite)
            {
                if (ownsRevalidation())
                {
                    LockInstallFailure(game, revalidationLock, GameStateLockReason.WriteAccessFailure,
                        revalidationLock != null ? "Validate mod descriptor output" : GetModDirectoryRootPath(game));
                }

                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return null;
            }

            List<IModInstallationResult> filteredDescriptors;
            IEnumerable<IMod> mods;
            try
            {
                if (!ownsRevalidation())
                {
                    return null;
                }

                var args = new ModParserArgs { BaseSteamDirectory = game.BaseSteamGameDirectory, IsProton = !string.IsNullOrWhiteSpace(game.LinuxProtonVersion), SteamAppId = game.SteamAppId };
                mods = GetInstalledModsInternal(game, false);
                var descriptors = new List<IModInstallationResult>();
                var userDirectoryMods = GetAllModDescriptors(Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory), ModSource.Local, game.ModDescriptorType, game.Type, args);
                if (userDirectoryMods?.Count() > 0)
                {
                    descriptors.AddRange(userDirectoryMods);
                }

                if (!string.IsNullOrWhiteSpace(game.CustomModDirectory))
                {
                    var customMods = GetAllModDescriptors(GetModDirectoryRootPath(game), ModSource.Local, game.ModDescriptorType, game.Type, args);
                    if (customMods != null && customMods.Any())
                    {
                        descriptors.AddRange(customMods);
                    }
                }

                var workshopDirectoryMods = game.WorkshopDirectory.SelectMany(p => GetAllModDescriptors(p, ModSource.Steam, game.ModDescriptorType, game.Type, args));
                if (workshopDirectoryMods.Any())
                {
                    descriptors.AddRange(workshopDirectoryMods);
                }

                filteredDescriptors = [];
                var grouped = descriptors.GroupBy(p => p.ParentDirectory);
                foreach (var item in grouped)
                {
                    if (item.Any())
                    {
                        filteredDescriptors.AddRange(item.All(p => p.IsFile) ? item : item.Where(p => !p.IsFile));
                    }
                }
            }
            catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
            {
                LockInstallFailure(game, revalidationLock, GameStateLockReason.DiscoveryUnavailable,
                    "Read mod descriptor sources", exception);
                return null;
            }

            if (!ownsRevalidation())
            {
                return null;
            }

            var diffs = filteredDescriptors.Where(p => p.Mod != null && !mods.Any(m => AreModDefinitionsEquivalent(m, p.Mod))).ToList();
            if (diffs.Count > 0)
            {
                var result = new List<IModInstallationResult>();
                try
                {
                    if (!ownsRevalidation())
                    {
                        return null;
                    }

                    await ModWriter.CreateModDirectoryAsync(
                        new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.ModDirectory });
                    if (!ownsRevalidation())
                    {
                        return null;
                    }

                    if (game.ModDescriptorType is ModDescriptorType.JsonMetadata or ModDescriptorType.JsonMetadataV2)
                    {
                        await ModWriter.CreateModDirectoryAsync(
                            new ModWriterParameters { RootDirectory = game.UserDirectory, Path = Shared.Constants.JsonModDirectory });
                        if (!ownsRevalidation())
                        {
                            return null;
                        }
                    }

                    var tasks = new List<Task<bool>>();
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
                            if (!ownsRevalidation())
                            {
                                return false;
                            }

                            var shouldLock = CheckIfModShouldBeLocked(game, localDiff);
                            if (statusToRetain != null && !shouldLock)
                            {
                                var mod = statusToRetain.FirstOrDefault(p => p.DescriptorFile.Equals(localDiff.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                                if (mod != null)
                                {
                                    shouldLock = mod.IsLocked;
                                }
                            }

                            return await ModWriter.WriteDescriptorAsync(new ModWriterParameters
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
                        var writes = await Task.WhenAll(tasks);
                        if (!ownsRevalidation())
                        {
                            return null;
                        }

                        if (revalidationLock != null && writes.Any(p => !p))
                        {
                            LockInstallFailure(game, revalidationLock, GameStateLockReason.WriteAccessFailure,
                                "Write mod descriptors");
                            return null;
                        }

                        Cache.Invalidate(new CacheInvalidateParameters { Region = ModsCacheRegion, Prefix = game.Type, Keys = [GetModsCacheKey(true), GetModsCacheKey(false)] });
                    }
                }
                catch (Exception exception) when (fileSystemStateProbe.IsFileSystemAccessFailure(exception))
                {
                    LockInstallFailure(game, revalidationLock, GameStateLockReason.WriteAccessFailure,
                        "Write mod descriptors", exception);
                    return null;
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
                return [.. filteredDescriptors.Where(p => p.Invalid)];
            }

            // ReSharper disable once DisposeOnUsingVariable
            mutex.Dispose();
            return null;
        }

        private void LockInstallFailure(IGame game, GameStateLockInfo revalidationLock,
            GameStateLockReason reason, string context, Exception exception = null)
        {
            if (revalidationLock != null)
            {
                gameStateSafetyService.LockRevalidationFailure(game, revalidationLock, reason, context, exception);
            }
            else if (!gameStateSafetyService.IsLocked(game))
            {
                gameStateSafetyService.Lock(game, reason, context, exception);
            }
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
            if (game != null && mods?.Count() > 0 && !mods.Any(p => p.IsVirtual))
            {
                var writableMods = mods.Where(item => !CheckIfModShouldBeLocked(game, item)).ToList();
                var tasks = writableMods.Select(item => ModWriter.SetDescriptorLockAsync(
                    new ModWriterParameters { Mod = item, RootDirectory = game.UserDirectory }, isLocked));
                await Task.WhenAll(tasks);
                foreach (var item in writableMods)
                {
                    item.IsLocked = isLocked;
                }

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
            var realMods = mods?.Where(p => !p.IsVirtual).ToList() ?? [];
            return realMods.Count == 0 ? Task.FromResult(false) : PopulateModFilesInternalAsync(realMods);
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
            if (!result)
            {
                return false;
            }
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
        /// Determines whether two mod definitions have equivalent state.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="otherMod">The other mod.</param>
        /// <returns><c>true</c> when the represented mod definitions are equivalent; otherwise, <c>false</c>.</returns>
        public virtual bool AreModDefinitionsEquivalent(IMod mod, IMod otherMod)
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

            //remoteIds.ForEach(p => names.Remove(p)); Was actually a feature #569
            var result = new ParsedSearchResult
            {
                AchievementCompatible = parserResult.AchievementCompatible,
                Source = [.. parserResult.Source.Where(p => p.Result != SourceType.None)],
                Version = [.. parserResult.Version.Where(p => p.Version != null)],
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
        /// <param name="gameType">Type of the game.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IModInstallationResult&gt;.</returns>
        protected virtual IEnumerable<IModInstallationResult> GetAllModDescriptors(string path, ModSource modSource, ModDescriptorType modDescriptorType, string gameType, ModParserArgs args)
        {
            static bool IsSubPath(string path, string potentialParent)
            {
                return path.StartsWith(potentialParent.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
            }

            // Json metadata doesn't support zips to ignore them
            var files = Directory.Exists(path) && modDescriptorType == ModDescriptorType.DescriptorMod
                ? Directory.EnumerateFiles(path, $"*{Shared.Constants.ZipExtension}").Union(Directory.EnumerateFiles(path, $"*{Shared.Constants.BinExtension}"))
                : [];
            var directories = Directory.Exists(path) ? Directory.EnumerateDirectories(path) : [];
            var mods = new ConcurrentBag<IModInstallationResult>();

            static void setDescriptorPath(IMod mod, string desiredPath, string localPath)
            {
                if (desiredPath.Equals(localPath, StringComparison.OrdinalIgnoreCase) || mod.RemoteId.GetValueOrDefault() > 0)
                {
                    mod.DescriptorFile = desiredPath;
                }
                else
                {
                    mod.Source = ModSource.Local;
                    mod.DescriptorFile = localPath;
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
                    if (modDescriptorType is ModDescriptorType.JsonMetadata or ModDescriptorType.JsonMetadataV2)
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

                    var mod = Mapper.Map<IMod>(ModParser.Parse(fileInfo.Content, MapDescriptorModType(modDescriptorType), args));
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
                    mod.Game = gameType;
                    var cleanedPath = path;
                    if (!isDirectory)
                    {
                        cleanedPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, Path.GetFileNameWithoutExtension(path));
                    }

                    var localPath = modDescriptorType == ModDescriptorType.DescriptorMod
                        ? $"{Shared.Constants.ModDirectory}/{cleanedPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()}{Shared.Constants.ModExtension}"
                        : $"{Shared.Constants.JsonModDirectory}/{cleanedPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()}{Shared.Constants.JsonExtension}";

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
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

            var filtered = mods
                .Where(x => !mods.Any(y => x != y && IsSubPath(x.Path, y.Path)))
                .ToList();

            return [.. filtered];
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
