// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
// ***********************************************************************
// <copyright file="JsonExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.DLC;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.Common;
using IronyModManager.IO.Mods.Models.Paradox.v1;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using Newtonsoft.Json;
using Nito.AsyncEx;
using static IronyModManager.IO.Mods.Models.Paradox.Common.Playsets;

namespace IronyModManager.IO.Mods.Exporter
{
    /// <summary>
    /// Class JsonExporter.
    /// Implements the <see cref="IronyModManager.IO.Mods.Exporter.BaseExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Exporter.BaseExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class JsonExporter : BaseExporter
    {
        #region Fields

        /// <summary>
        /// The playset name
        /// </summary>
        private const string PlaysetName = Common.Constants.JsonV2PlaysetName;

        /// <summary>
        /// The write lock
        /// </summary>
        private static readonly AsyncLock writeLock = new();

        #endregion Fields

        #region Methods

        /// <summary>
        /// export DLC as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ExportDLCAsync(DLCParameters parameters)
        {
            using var mutex = await writeLock.LockAsync();

            var result = false;
            if (parameters.DescriptorType == DescriptorType.DescriptorMod)
            {
                var dlcPath = Path.Combine(parameters.RootPath, Constants.DLC_load_path);
                var dlcLoad = await LoadPdxModelAsync<DLCLoad>(dlcPath) ?? new DLCLoad();
                dlcLoad.DisabledDLCs = [.. parameters.DLC.Select(p => p.Path)];
                result = await WritePdxModelAsync(dlcLoad, dlcPath);
            }
            else if (parameters.DescriptorType == DescriptorType.DescriptorMod)
            {
                var contentPath = Path.Combine(parameters.RootPath, Constants.Content_load_path);
                var contentLoad = await LoadContentLoadModelAsync(contentPath);
                contentLoad.DisabledDLC = [.. parameters.DLC.Select(p => new DisabledDLC { ParadoxAppId = p.AppId })];
                result = await WritePdxModelAsync(contentLoad, contentPath);
            }
            else
            {
                var playsetsPath = Path.Combine(parameters.RootPath, Constants.Playsets_path);
                var playsets = await LoadPlaysetsModelAsync(playsetsPath);
                var ironyPlaySet = playsets.PlaysetsCollection.FirstOrDefault(p => p.Name.Equals(PlaysetName, StringComparison.OrdinalIgnoreCase));

                // Irony service layer sends only disabled dlc
                foreach (var dlcObject in parameters.DLC)
                {
                    var dlc = ironyPlaySet!.DLCCollection.FirstOrDefault(p => p.ParadoxAppId == dlcObject.AppId);
                    if (dlc == null)
                    {
                        // Append disabled DLC data
                        dlc = new Playsets.DLC { ParadoxAppId = dlcObject.AppId, IsEnabled = false };
                        ironyPlaySet.DLCCollection.Add(dlc);
                    }
                    else
                    {
                        // Flag as disabled only
                        dlc.IsEnabled = false;
                    }
                }

                result = await WritePdxModelAsync(playsets, playsetsPath);
            }

            // ReSharper disable once DisposeOnUsingVariable
            mutex.Dispose();

            return result;
        }

        /// <summary>
        /// export mods as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ExportModsAsync(ModWriterParameters parameters)
        {
            using var mutex = await writeLock.LockAsync();
            switch (parameters.DescriptorType)
            {
                case DescriptorType.JsonMetadata:
                {
                    var result = await ExportContentLoadModsAsync(parameters);

                    // ReSharper disable once DisposeOnUsingVariable
                    mutex.Dispose();
                    return result;
                }
                case DescriptorType.JsonMetadataV2:
                {
                    var result = await ExportPlaysetsAsync(parameters);

                    // ReSharper disable once DisposeOnUsingVariable
                    mutex.Dispose();
                    return result;
                }
                default:
                {
                    var result = await ExportDLCLoadModsAsync(parameters);

                    // ReSharper disable once DisposeOnUsingVariable
                    mutex.Dispose();
                    return result;
                }
            }
        }

        /// <summary>
        /// get disabled DLC as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IReadOnlyCollection&lt;IDLCObject&gt;.</returns>
        public async Task<IReadOnlyCollection<IDLCObject>> GetDisabledDLCAsync(DLCParameters parameters)
        {
            switch (parameters.DescriptorType)
            {
                case DescriptorType.DescriptorMod:
                {
                    var dlcPath = Path.Combine(parameters.RootPath, Constants.DLC_load_path);
                    var dlcLoad = await LoadPdxModelAsync<DLCLoad>(dlcPath) ?? new DLCLoad();
                    if (dlcLoad.DisabledDLCs?.Count > 0)
                    {
                        var result = dlcLoad.DisabledDLCs.Select(p =>
                        {
                            var model = DIResolver.Get<IDLCObject>();
                            model.Path = p;
                            return model;
                        }).ToList();
                        return result;
                    }

                    break;
                }
                case DescriptorType.JsonMetadata:
                {
                    var contentPath = Path.Combine(parameters.RootPath, Constants.Content_load_path);
                    var contentLoad = await LoadPdxModelAsync<ContentLoad>(contentPath) ?? new ContentLoad();
                    if (contentLoad.DisabledDLC?.Count > 0)
                    {
                        var result = contentLoad.DisabledDLC.Select(p =>
                        {
                            var model = DIResolver.Get<IDLCObject>();
                            model.AppId = p.ParadoxAppId;
                            return model;
                        }).ToList();
                        return result;
                    }

                    break;
                }
                default:
                {
                    var playsetsPath = Path.Combine(parameters.RootPath, Constants.Playsets_path);
                    var playsets = await LoadPlaysetsModelAsync(playsetsPath);
                    var ironyPlaySet = playsets.PlaysetsCollection.FirstOrDefault(p => p.Name.Equals(PlaysetName, StringComparison.OrdinalIgnoreCase));
                    if (ironyPlaySet!.DLCCollection.Count > 0)
                    {
                        var result = ironyPlaySet.DLCCollection.Where(p => !p.IsEnabled).Select(d =>
                        {
                            var model = DIResolver.Get<IDLCObject>();
                            model.AppId = d.ParadoxAppId;
                            return model;
                        }).ToList();
                        return result;
                    }

                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// Deserializes the PDX model asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content">The content.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        private static Task<T> DeserializePdxModelAsync<T>(string content) where T : IPdxFormat
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                var result = JsonConvert.DeserializeObject<T>(content);
                return Task.FromResult(result);
            }

            return Task.FromResult<T>(default);
        }

        /// <summary>
        /// load PDX model as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        private static async Task<T> LoadPdxModelAsync<T>(string path) where T : IPdxFormat
        {
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var result = JsonConvert.DeserializeObject<T>(content);
                    return result;
                }
            }

            return default;
        }

        /// <summary>
        /// Export content load mods as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> ExportContentLoadModsAsync(ModWriterParameters parameters)
        {
            var contentPath = Path.Combine(parameters.RootDirectory, Constants.Content_load_path);
            var contentLoad = await LoadContentLoadModelAsync(contentPath);

            if (!parameters.AppendOnly)
            {
                contentLoad.EnabledMods.Clear();
            }

            parameters.EnabledMods?.ToList().ForEach(p =>
            {
                contentLoad.EnabledMods.Add(new EnabledMod { Path = ResolveContentLoadPath(p.FullPath) });
            });
            parameters.TopPriorityMods?.ToList().ForEach(p =>
            {
                contentLoad.EnabledMods.Add(new EnabledMod { Path = ResolveContentLoadPath(p.FullPath) });
            });
            return await WritePdxModelAsync(contentLoad, contentPath);
        }

        /// <summary>
        /// Export DLC load mods as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> ExportDLCLoadModsAsync(ModWriterParameters parameters)
        {
            var dlcPath = Path.Combine(parameters.RootDirectory, Constants.DLC_load_path);
            var gameDataPath = Path.Combine(parameters.RootDirectory, Constants.Game_data_path);
            var modRegistryPath = Path.Combine(parameters.RootDirectory, Constants.Mod_registry_path);
            var dlcLoad = await LoadPdxModelAsync<DLCLoad>(dlcPath) ?? new DLCLoad();
            var gameData = await LoadPdxModelAsync<GameData>(gameDataPath) ?? new GameData();
            var modRegistry = await LoadPdxModelAsync<ModRegistryCollection>(modRegistryPath) ?? [];

            if (!parameters.AppendOnly)
            {
                gameData.ModsOrder.Clear();
                dlcLoad.EnabledMods.Clear();
            }

            // Remove invalid mods
            var toRemove = new List<string>();
            foreach (var pdxMod in modRegistry)
            {
                if (pdxMod.Value.Status != Constants.Ready_to_play)
                {
                    toRemove.Add(pdxMod.Key);
                }
            }

            foreach (var item in toRemove)
            {
                modRegistry.Remove(item);
            }

            if (parameters.EnabledMods != null)
            {
                foreach (var mod in parameters.EnabledMods)
                {
                    SyncData(dlcLoad, gameData, modRegistry, mod, true);
                }
            }

            if (parameters.OtherMods != null)
            {
                foreach (var mod in parameters.OtherMods)
                {
                    SyncData(dlcLoad, gameData, modRegistry, mod, false);
                }
            }

            if (parameters.TopPriorityMods != null)
            {
                foreach (var mod in parameters.TopPriorityMods)
                {
                    var existingEntry = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                    if (existingEntry != null)
                    {
                        gameData.ModsOrder.Remove(existingEntry.Id);
                    }

                    var existingEnabledMod = dlcLoad.EnabledMods.FirstOrDefault(p => p.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(existingEnabledMod))
                    {
                        dlcLoad.EnabledMods.Remove(existingEnabledMod);
                    }

                    SyncData(dlcLoad, gameData, modRegistry, mod, true);
                }
            }

            var tasks = new[] { WritePdxModelAsync(dlcLoad, dlcPath), WritePdxModelAsync(gameData, gameDataPath), WritePdxModelAsync(modRegistry, modRegistryPath) };
            await Task.WhenAll(tasks);

            return tasks.All(p => p.Result);
        }

        /// <summary>
        /// Export playsets as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> ExportPlaysetsAsync(ModWriterParameters parameters)
        {
            var playsetsPath = Path.Combine(parameters.RootDirectory, Constants.Playsets_path);
            var playsets = await LoadPlaysetsModelAsync(playsetsPath);
            var ironyPlaySet = playsets.PlaysetsCollection.FirstOrDefault(p => p.Name.Equals(PlaysetName, StringComparison.OrdinalIgnoreCase));

            if (!parameters.AppendOnly)
            {
                ironyPlaySet!.OrderedListMods.Clear();
            }

            parameters.EnabledMods?.ToList().ForEach(p =>
            {
                ironyPlaySet!.OrderedListMods.Add(new OrderedListMod { Path = ResolveContentLoadPath(p.FullPath, true, true), IsEnabled = true });
            });
            parameters.TopPriorityMods?.ToList().ForEach(p =>
            {
                ironyPlaySet!.OrderedListMods.Add(new OrderedListMod { Path = ResolveContentLoadPath(p.FullPath, true, true), IsEnabled = true });
            });

            // Set all to false
            foreach (var playset in playsets.PlaysetsCollection)
            {
                playset.IsActive = false;
            }

            // Set Irony only one active
            ironyPlaySet!.IsActive = true;

            return await WritePdxModelAsync(playsets, playsetsPath);
        }

        /// <summary>
        /// Load content load model as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A Task&lt;ContentLoad&gt; representing the asynchronous operation.</returns>
        private async Task<ContentLoad> LoadContentLoadModelAsync(string path)
        {
            ContentLoad contentLoad = new ContentLoadV2();
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path);
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = string.Empty;
                }

                if (content.Contains("enabledUGC", StringComparison.OrdinalIgnoreCase))
                {
                    contentLoad = await DeserializePdxModelAsync<ContentLoadV2>(content);
                }
                else
                {
                    contentLoad = await DeserializePdxModelAsync<ContentLoad>(content);
                }
            }

            return contentLoad;
        }

        /// <summary>
        /// Load playsets model as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A Task&lt;IronyModManager.IO.Mods.Models.Paradox.Common.Playsets&gt; representing the asynchronous operation.</returns>
        private async Task<Playsets> LoadPlaysetsModelAsync(string path)
        {
            static Playset getIronyPlayset()
            {
                return new Playset
                {
                    Name = PlaysetName,
                    DLCCollection = [],
                    IsAutomaticallySorted = false,
                    OrderedListMods = [],
                    IsActive = false
                };
            }

            Playsets playsets = null;
            if (File.Exists(path))
            {
                var content = await File.ReadAllTextAsync(path);
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = string.Empty;
                }

                playsets = await DeserializePdxModelAsync<Playsets>(content) ?? new Playsets
                {
                    FileVersion = "1.0.0",
                    PlaysetsCollection =
                    [
                        getIronyPlayset()
                    ]
                };
                playsets.PlaysetsCollection ??= [];
                if (!playsets.PlaysetsCollection.Any(p => p.Name.Equals(PlaysetName, StringComparison.OrdinalIgnoreCase)))
                {
                    playsets.PlaysetsCollection.Add(getIronyPlayset());
                }

                var ironyPlayset = playsets.PlaysetsCollection.FirstOrDefault(p => p.Name.Equals(PlaysetName, StringComparison.OrdinalIgnoreCase));
                ironyPlayset!.DLCCollection ??= []; // Not null we ensured to add it beforehand
                ironyPlayset.OrderedListMods ??= [];
                ironyPlayset.IsActive = false;
                ironyPlayset.IsAutomaticallySorted = false;
            }
            else
            {
                playsets = new Playsets
                {
                    FileVersion = "1.0.0",
                    PlaysetsCollection =
                    [
                        getIronyPlayset()
                    ]
                };
            }

            return playsets;
        }

        /// <summary>
        /// Resolves the content load path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="replaceForwardSlash">if set to <c>true</c> [replace forward slash].</param>
        /// <param name="appendFinalSlash">if set to <c>true</c> [append final slash].</param>
        /// <returns>System.String.</returns>
        private string ResolveContentLoadPath(string path, bool replaceForwardSlash = false, bool appendFinalSlash = false)
        {
            // Why? Because Paradox went full paradox!
            // If the first mod path in the JSON array has casing Paradox doesn't like, the entire mod list fails...
            // ...even if every single other entry is correct...
            var contentPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? PathOperations.GetActualPathCasing(path) : path;
            var separator = Path.DirectorySeparatorChar;
            if (replaceForwardSlash)
            {
                contentPath = contentPath.Replace("\\", "/");
                separator = '/';
            }

            if (appendFinalSlash)
            {
                contentPath = contentPath.TrimEnd(separator);
                contentPath += separator;
            }

            return contentPath;
        }

        /// <summary>
        /// Synchronizes the data.
        /// </summary>
        /// <param name="dlcLoad">The d lc load.</param>
        /// <param name="gameData">The game data.</param>
        /// <param name="modRegistry">The mod registry.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        private void SyncData(DLCLoad dlcLoad, GameData gameData, ModRegistryCollection modRegistry, IMod mod, bool isEnabled)
        {
            ModRegistry pdxMod;

            // Populate registry
            if (!modRegistry.Values.Any(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase)))
            {
                pdxMod = new ModRegistry { Id = Guid.NewGuid().ToString() };
                modRegistry.Add(pdxMod.Id, pdxMod);
            }
            else
            {
                pdxMod = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
            }

            MapModData(pdxMod, mod);

            // Populate game data
            var entry = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
            gameData.ModsOrder.Add(entry!.Id);

            // Populate dlc
            if (isEnabled)
            {
                dlcLoad.EnabledMods.Add(mod.DescriptorFile);
            }
        }

        /// <summary>
        /// write PDX model as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> WritePdxModelAsync<T>(T model, string path) where T : IPdxFormat
        {
            async Task<bool> writeFile()
            {
                var dirPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath!);
                }

                if (File.Exists(path))
                {
                    _ = new System.IO.FileInfo(path) { IsReadOnly = false };
                }

                await File.WriteAllTextAsync(path!, JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                return true;
            }

            var retry = new RetryStrategy();
            return await retry.RetryActionAsync(writeFile);
        }

        #endregion Methods
    }
}
