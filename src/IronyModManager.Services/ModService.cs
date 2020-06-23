// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2020
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
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Cache;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModService" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModService(IReader reader,
            IModParser modParser, IModWriter modWriter,
            IGameService gameService,
            IStorageProvider storageProvider, IMapper mapper) : base(reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
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
                    var isAchievementCompatible = !item.Files.Any(p => game.ChecksumFolders.Any(s => p.StartsWith(s, StringComparison.OrdinalIgnoreCase)));
                    item.AchievementStatus = isAchievementCompatible ? AchievementStatus.Compatible : AchievementStatus.NotCompatible;
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
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ExportModsAsync(IReadOnlyCollection<IMod> enabledMods, IReadOnlyCollection<IMod> regularMods, string collectionName)
        {
            var game = GameService.GetSelected();
            if (game == null || enabledMods == null || regularMods == null)
            {
                return false;
            }
            var allMods = GetInstalledModsInternal(game, false);
            var mod = GeneratePatchModDescriptor(allMods, game, GenerateCollectionPatchName(collectionName));
            var applyModParams = new ModWriterParameters()
            {
                OtherMods = regularMods.Where(p => !enabledMods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile))).ToList(),
                EnabledMods = enabledMods,
                RootDirectory = game.UserDirectory
            };
            if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
            {
                RootDirectory = game.UserDirectory,
                Path = mod.FileName
            }))
            {
                if (await ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                {
                    Mod = mod,
                    RootDirectory = game.UserDirectory,
                    Path = mod.DescriptorFile
                }, IsPatchModInternal(mod)))
                {
                    applyModParams.TopPriorityMods = new List<IMod>() { mod };
                    ModsCache.InvalidateCache(game);
                }
            }
            return await ModWriter.ApplyModsAsync(applyModParams);
        }

        /// <summary>
        /// Gets the installed mods.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        public virtual IEnumerable<IMod> GetInstalledMods(IGame game)
        {
            ModsCache.InvalidateCache(game);
            return GetInstalledModsInternal(game, true);
        }

        /// <summary>
        /// install mods as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> InstallModsAsync()
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            var mods = GetInstalledModsInternal(game, false);
            var descriptors = new List<IMod>();
            var userDirectoryMods = GetAllModDescriptors(Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory), ModSource.Local);
            if (userDirectoryMods?.Count() > 0)
            {
                descriptors.AddRange(userDirectoryMods);
            }
            var workshopDirectoryMods = GetAllModDescriptors(game.WorkshopDirectory, ModSource.Steam);
            if (workshopDirectoryMods?.Count() > 0)
            {
                descriptors.AddRange(workshopDirectoryMods);
            }
            var diffs = descriptors.Where(p => !mods.Any(m => m.DescriptorFile.Equals(p.DescriptorFile, StringComparison.OrdinalIgnoreCase))).ToList();
            if (diffs.Count > 0)
            {
                await ModWriter.CreateModDirectoryAsync(new ModWriterParameters()
                {
                    RootDirectory = game.UserDirectory,
                    Path = Shared.Constants.ModDirectory
                });
                var tasks = new List<Task>();
                foreach (var diff in diffs)
                {
                    if (IsPatchModInternal(diff))
                    {
                        continue;
                    }
                    tasks.Add(ModWriter.WriteDescriptorAsync(new ModWriterParameters()
                    {
                        Mod = diff,
                        RootDirectory = game.UserDirectory,
                        Path = diff.DescriptorFile
                    }, IsPatchModInternal(diff)));
                }
                await Task.WhenAll(tasks);
                ModsCache.InvalidateCache(game);
                return true;
            }

            return false;
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
                    var task = ModWriter.SetDescriptorLockAsync(new ModWriterParameters()
                    {
                        Mod = item,
                        RootDirectory = game.UserDirectory
                    }, isLocked);
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                return true;
            }
            return false;
        }

        /// <summary>
        /// populate mod files as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> PopulateModFilesAsync(IEnumerable<IMod> mods)
        {
            if (mods?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var mod in mods)
                {
                    if (mod.IsValid)
                    {
                        var task = Task.Run(() =>
                        {
                            var localMod = mod;
                            var files = Reader.GetFiles(mod.FullPath);
                            localMod.Files = files ?? new List<string>();
                        });
                        tasks.Add(task);
                    }
                    else
                    {
                        mod.Files = new List<string>();
                    }
                }
                await Task.WhenAll(tasks);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all mod descriptors.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="modSource">The mod source.</param>
        /// <returns>IEnumerable&lt;IMod&gt;.</returns>
        protected virtual IEnumerable<IMod> GetAllModDescriptors(string path, ModSource modSource)
        {
            var files = Directory.Exists(path) ? Directory.EnumerateFiles(path, $"*{Shared.Constants.ZipExtension}") : new string[] { };
            var directories = Directory.Exists(path) ? Directory.EnumerateDirectories(path) : new string[] { };
            var mods = new List<IMod>();

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
                mods.Add(mod);
            }
            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    parseModFiles(file, modSource, false);
                }
            }
            if (directories.Count() > 0)
            {
                foreach (var directory in directories)
                {
                    var modSourceOverride = directory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).
                            LastOrDefault().Contains(Constants.Paradox_mod_id, StringComparison.OrdinalIgnoreCase) ? ModSource.Paradox : modSource;
                    var zipFiles = Directory.EnumerateFiles(directory, $"*{Shared.Constants.ZipExtension}");
                    if (zipFiles.Count() > 0)
                    {
                        foreach (var zip in zipFiles)
                        {
                            parseModFiles(zip, modSourceOverride, false);
                        }
                    }
                    else
                    {
                        parseModFiles(directory, modSourceOverride, true);
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
        protected virtual int GetSteamModId(string path, bool isDirectory = false)
        {
            var name = !isDirectory ? Path.GetFileNameWithoutExtension(path) : path;
            int.TryParse(name.Replace(Constants.Steam_mod_id, string.Empty), out var id);
            return id;
        }

        #endregion Methods
    }
}
