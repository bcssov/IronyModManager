// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 10-26-2021
// ***********************************************************************
// <copyright file="ModService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Localization;
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
        public ModService(IParser searchParser, ILogger logger, ICache cache, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders,
            IReader reader, IModParser modParser,
            IModWriter modWriter, IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.logger = logger;
            this.searchParser = searchParser;
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
            if (mod.Source == ModSource.Paradox)
            {
                return string.Format(Constants.Paradox_Url, mod.RemoteId);
            }
            else
            {
                return string.Format(Constants.Steam_Url, mod.RemoteId);
            }
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
            var result = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
            {
                RootDirectory = path
            });
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
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool EvalAchievementCompatibility(IEnumerable<IMod> mods)
        {
            var game = GameService.GetSelected();
            if (game != null && mods?.Count() > 0)
            {
                foreach (var item in mods.Where(p => p.IsValid))
                {
                    if (item.Files.Any())
                    {
                        var isAchievementCompatible = !item.Files.Any(p => game.ChecksumFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase)));
                        item.AchievementStatus = isAchievementCompatible ? AchievementStatus.Compatible : AchievementStatus.NotCompatible;
                    }
                    else
                    {
                        item.AchievementStatus = AchievementStatus.NotEvaluated;
                    }
                }
                return true;
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
            var applyModParams = new ModWriterParameters()
            {
                OtherMods = regularMods.Where(p => !enabledMods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile))).ToList(),
                EnabledMods = enabledMods,
                RootDirectory = game.UserDirectory
            };
            if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
            {
                RootDirectory = mod.FullPath
            }))
            {
                if (modCollection.PatchModEnabled && enabledMods.Any())
                {
                    if (await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = mod,
                        RootDirectory = game.UserDirectory,
                        Path = mod.DescriptorFile,
                        LockDescriptor = CheckIfModShouldBeLocked(game, mod)
                    }, IsPatchModInternal(mod)))
                    {
                        applyModParams.TopPriorityMods = new List<IMod>() { mod };
                        Cache.Invalidate(new CacheInvalidateParameters() { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
                    }
                }
            }
            else
            {
                // Remove left over descriptor
                if (allMods.Any(p => p.Name.Equals(mod.Name)))
                {
                    await DeleteDescriptorsInternalAsync(new List<IMod>() { mod });
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
                return collection;
            }
            var parameters = searchParser.Parse(CurrentLocale.CultureName, text);
            var result = collection.Where(p => p.Name.Contains(parameters.Name, StringComparison.InvariantCultureIgnoreCase) ||
                    (p.RemoteId.HasValue && p.RemoteId.GetValueOrDefault().ToString().Contains(parameters.Name)))
                    .ConditionalFilter(parameters.AchievementCompatible.Result.HasValue, x => x.Where(p => p.AchievementStatus == (parameters.AchievementCompatible.Result.GetValueOrDefault() ? AchievementStatus.Compatible : AchievementStatus.NotCompatible)))
                    .ConditionalFilter(parameters.IsSelected.Result.HasValue, x => x.Where(p => p.IsSelected == parameters.IsSelected.Result.GetValueOrDefault()))
                    .ConditionalFilter(parameters.Source.Result != SourceType.None, x => x.Where(p => p.Source == SourceTypeToModSource(parameters.Source.Result)))
                    .ConditionalFilter(parameters.Version != null, x => x.Where(p => p.VersionData > parameters.Version));
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
            var parameters = searchParser.Parse(CurrentLocale.CultureName, text);
            var result = !reverse ? collection.Skip(skipIndex.GetValueOrDefault()) : collection.Reverse().Skip(skipIndex.GetValueOrDefault());
            result = result.Where(p => p.Name.Contains(parameters.Name, StringComparison.InvariantCultureIgnoreCase) ||
                    (p.RemoteId.HasValue && p.RemoteId.GetValueOrDefault().ToString().Contains(parameters.Name)))
                    .ConditionalFilter(parameters.AchievementCompatible.Result.HasValue, x => x.Where(p => p.AchievementStatus == (parameters.AchievementCompatible.Result.GetValueOrDefault() ? AchievementStatus.Compatible : AchievementStatus.NotCompatible)))
                    .ConditionalFilter(parameters.IsSelected.Result.HasValue, x => x.Where(p => p.IsSelected == parameters.IsSelected.Result.GetValueOrDefault()))
                    .ConditionalFilter(parameters.Source.Result != SourceType.None, x => x.Where(p => p.Source == SourceTypeToModSource(parameters.Source.Result)))
                    .ConditionalFilter(parameters.Version != null, x => x.Where(p => p.VersionData > parameters.Version));
            return result.FirstOrDefault();
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
                Cache.Invalidate(new CacheInvalidateParameters() { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
            }
            var result = GetInstalledModsInternal(game, true);
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
            if (game == null)
            {
                mutex.Dispose();
                return null;
            }
            var mods = GetInstalledModsInternal(game, false);
            var descriptors = new List<IModInstallationResult>();
            var userDirectoryMods = GetAllModDescriptors(Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory), ModSource.Local);
            if (userDirectoryMods?.Count() > 0)
            {
                descriptors.AddRange(userDirectoryMods);
            }
            if (!string.IsNullOrWhiteSpace(game.CustomModDirectory))
            {
                var customMods = GetAllModDescriptors(GetModDirectoryRootPath(game), ModSource.Local);
                if (customMods != null && customMods.Any())
                {
                    descriptors.AddRange(customMods);
                }
            }
            var workshopDirectoryMods = game.WorkshopDirectory.SelectMany(p => GetAllModDescriptors(p, ModSource.Steam));
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
                    if (item.All(p => p.IsFile))
                    {
                        filteredDescriptors.AddRange(item);
                    }
                    else
                    {
                        filteredDescriptors.AddRange(item.Where(p => !p.IsFile));
                    }
                }
            }
            var diffs = filteredDescriptors.Where(p => p.Mod != null && !mods.Any(m => AreModsSame(m, p.Mod))).ToList();
            if (diffs.Count > 0)
            {
                var result = new List<IModInstallationResult>();
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Shared.Constants.ModDirectory
                });
                var tasks = new List<Task>();
                foreach (var diff in diffs.GroupBy(p => p.Mod.DescriptorFile))
                {
                    IModInstallationResult installResult = diff.FirstOrDefault();
                    if (game.WorkshopDirectory.Any() && diff.Any(p => p.Path.StartsWith(game.WorkshopDirectory.FirstOrDefault())))
                    {
                        installResult = diff.FirstOrDefault(p => p.Path.StartsWith(game.WorkshopDirectory.FirstOrDefault()));                        
                    }
                    var localDiff = installResult.Mod;
                    if (IsPatchModInternal(localDiff))
                    {
                        continue;
                    }
                    tasks.Add(Task.Run(async () =>
                    {
                        bool shouldLock = CheckIfModShouldBeLocked(game, localDiff);
                        if (statusToRetain != null && !shouldLock)
                        {
                            var mod = statusToRetain.FirstOrDefault(p => p.DescriptorFile.Equals(localDiff.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                            if (mod != null)
                            {
                                shouldLock = mod.IsLocked;
                            }
                        }
                        await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                        {
                            Mod = localDiff,
                            RootDirectory = game.UserDirectory,
                            Path = localDiff.DescriptorFile,
                            LockDescriptor = shouldLock
                        }, IsPatchModInternal(localDiff));
                    }));
                    installResult.Installed = true;
                    result.Add(installResult);
                }
                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                    Cache.Invalidate(new CacheInvalidateParameters() { Region = ModsCacheRegion, Prefix = game.Type, Keys = new List<string> { GetModsCacheKey(true), GetModsCacheKey(false) } });
                }
                if (filteredDescriptors.Any(p => p.Invalid))
                {
                    result.AddRange(filteredDescriptors.Where(p => p.Invalid));
                }
                mutex.Dispose();
                return result;
            }
            if (filteredDescriptors.Any(p => p.Invalid))
            {
                mutex.Dispose();
                return filteredDescriptors.Where(p => p.Invalid).ToList();
            }
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
                        var task = ModWriter.SetDescriptorLockAsync(new ModWriterParameters()
                        {
                            Mod = item,
                            RootDirectory = game.UserDirectory
                        }, isLocked);
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
            var result = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = Path.Combine(Shared.Constants.ModDirectory, folder)
            });
            if (!result && !string.IsNullOrEmpty(game.CustomModDirectory))
            {
                result = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
                {
                    RootDirectory = GetModDirectoryRootPath(game),
                    Path = folder
                });
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
            var exists = await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
            {
                RootDirectory = fullPath
            });
            if (!exists)
            {
                fullPath = Path.Combine(GetModDirectoryRootPath(game), folder);
            }
            var result = await ModWriter.PurgeModDirectoryAsync(new ModWriterParameters()
            {
                RootDirectory = fullPath
            }, true);
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
        /// Ares the mods same.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <param name="otherMod">The other mod.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool AreModsSame(IMod mod, IMod otherMod)
        {
            if (mod == null || otherMod == null)
            {
                return false;
            }
            return mod.DescriptorFile.Equals(otherMod.DescriptorFile, StringComparison.OrdinalIgnoreCase) && mod.Version.Equals(otherMod.Version) &&
                mod.Name.Equals(otherMod.Name) && mod.Dependencies.ListsSame(otherMod.Dependencies) && mod.RemoteId.GetValueOrDefault().Equals(otherMod.RemoteId.GetValueOrDefault()) &&
                mod.ReplacePath.ListsSame(otherMod.ReplacePath) && mod.UserDir.ListsSame(otherMod.UserDir);
        }

        /// <summary>
        /// Gets all mod descriptors.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="modSource">The mod source.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        protected virtual IEnumerable<IModInstallationResult> GetAllModDescriptors(string path, ModSource modSource)
        {
            var files = Directory.Exists(path) ? Directory.EnumerateFiles(path, $"*{Shared.Constants.ZipExtension}").Union(Directory.EnumerateFiles(path, $"*{Shared.Constants.BinExtension}")) : Array.Empty<string>();
            var directories = Directory.Exists(path) ? Directory.EnumerateDirectories(path) : Array.Empty<string>();
            var mods = new List<IModInstallationResult>();

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

            void parseModFiles(string path, ModSource source, bool isDirectory)
            {
                var result = GetModelInstance<IModInstallationResult>();
                try
                {
                    var fileInfo = Reader.GetFileInfo(path, Shared.Constants.DescriptorFile);
                    if (fileInfo == null)
                    {
                        fileInfo = Reader.GetFileInfo(path, $"*{Shared.Constants.ModExtension}");
                        if (fileInfo == null)
                        {
                            return;
                        }
                    }
                    var mod = Mapper.Map<IMod>(ModParser.Parse(fileInfo.Content));
                    mod.FileName = path.Replace("\\", "/");
                    mod.FullPath = path.StandardizeDirectorySeparator();
                    mod.IsLocked = fileInfo.IsReadOnly;
                    mod.Source = source;
                    var cleanedPath = path;
                    if (!isDirectory)
                    {
                        cleanedPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                    }

                    var localPath = $"{Shared.Constants.ModDirectory}/{cleanedPath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()}{Shared.Constants.ModExtension}";
                    switch (mod.Source)
                    {
                        case ModSource.Local:
                            setDescriptorPath(mod, localPath, localPath);
                            break;

                        case ModSource.Steam:
                            if (mod.RemoteId.GetValueOrDefault() == 0)
                            {
                                if (!isDirectory)
                                {
                                    var modParentDirectory = Path.GetDirectoryName(path);
                                    mod.RemoteId = GetSteamModId(modParentDirectory, isDirectory);
                                }
                                else
                                {
                                    mod.RemoteId = GetSteamModId(path, isDirectory);
                                }
                            }
                            setDescriptorPath(mod, $"{Shared.Constants.ModDirectory}/{Constants.Steam_mod_id}{mod.RemoteId}{Shared.Constants.ModExtension}", localPath);
                            break;

                        case ModSource.Paradox:
                            if (!isDirectory)
                            {
                                var modParentDirectory = Path.GetDirectoryName(path);
                                mod.RemoteId = GetPdxModId(modParentDirectory, isDirectory);
                            }
                            else
                            {
                                mod.RemoteId = GetPdxModId(path, isDirectory);
                            }
                            setDescriptorPath(mod, $"{Shared.Constants.ModDirectory}/{Constants.Paradox_mod_id}{mod.RemoteId}{Shared.Constants.ModExtension}", localPath);
                            break;

                        default:
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
                if (result.IsFile)
                {
                    result.ParentDirectory = Path.GetDirectoryName(path);
                }
                else
                {
                    result.ParentDirectory = path;
                }
                mods.Add(result);
            }
            if (files.Any())
            {
                foreach (var file in files)
                {
                    parseModFiles(file, modSource, false);
                }
            }
            if (directories.Any())
            {
                foreach (var directory in directories)
                {
                    var modSourceOverride = directory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).
                            LastOrDefault().Contains(Constants.Paradox_mod_id, StringComparison.OrdinalIgnoreCase) ? ModSource.Paradox : modSource;

                    parseModFiles(directory, modSourceOverride, true);

                    var zipFiles = Directory.EnumerateFiles(directory, $"*{Shared.Constants.ZipExtension}").Union(Directory.EnumerateFiles(directory, $"*{Shared.Constants.BinExtension}"));
                    if (zipFiles.Any())
                    {
                        foreach (var zip in zipFiles)
                        {
                            parseModFiles(zip, modSourceOverride, false);
                        }
                    }

                    var subdirectories = Directory.GetDirectories(directory);
                    if (subdirectories.Any())
                    {
                        foreach (var subdirectory in subdirectories)
                        {
                            var subDirectoryModSourceOverride = subdirectory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).
                                LastOrDefault().Contains(Constants.Paradox_mod_id, StringComparison.OrdinalIgnoreCase) ? ModSource.Paradox : modSource;
                            parseModFiles(subdirectory, subDirectoryModSourceOverride, true);
                        }
                    }
                }
            }
            return mods;
        }

        /// <summary>
        /// Gets the steam mod identifier.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
        /// <returns>System.Int32.</returns>
        protected virtual long GetSteamModId(string path, bool isDirectory = false)
        {
            var name = !isDirectory ? Path.GetFileNameWithoutExtension(path) : path;
#pragma warning disable CA1806 // Do not ignore method results
            long.TryParse(name.Replace(Constants.Steam_mod_id, string.Empty), out var id);
#pragma warning restore CA1806 // Do not ignore method results
            return id;
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
                _ => ModSource.Local,
            };
        }

        #endregion Methods
    }
}
