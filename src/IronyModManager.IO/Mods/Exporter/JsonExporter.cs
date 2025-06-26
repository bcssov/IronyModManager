// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 06-26-2025
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
                dlcLoad.DisabledDlcs = parameters.DLC.Select(p => p.Path).ToList();
                result = await WritePdxModelAsync(dlcLoad, dlcPath);
            }
            else
            {
                var contentPath = Path.Combine(parameters.RootPath, Constants.Content_load_path);
                var contentLoad = await LoadContentLoadModelAsync(contentPath);
                contentLoad.DisabledDLC = parameters.DLC.Select(p => new DisabledDLC { ParadoxAppId = p.AppId }).ToList();
                result = await WritePdxModelAsync(contentLoad, contentPath);
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
            if (parameters.DescriptorType == DescriptorType.JsonMetadata)
            {
                var result = await ExportContentLoadModsAsync(parameters);

                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return result;
            }
            else
            {
                var result = await ExportDLCLoadModsAsync(parameters);

                // ReSharper disable once DisposeOnUsingVariable
                mutex.Dispose();
                return result;
            }
        }

        /// <summary>
        /// get disabled DLC as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IReadOnlyCollection&lt;IDLCObject&gt;.</returns>
        public async Task<IReadOnlyCollection<IDLCObject>> GetDisabledDLCAsync(DLCParameters parameters)
        {
            if (parameters.DescriptorType == DescriptorType.DescriptorMod)
            {
                var dlcPath = Path.Combine(parameters.RootPath, Constants.DLC_load_path);
                var dlcLoad = await LoadPdxModelAsync<DLCLoad>(dlcPath) ?? new DLCLoad();
                if (dlcLoad.DisabledDlcs?.Count > 0)
                {
                    var result = dlcLoad.DisabledDlcs.Select(p =>
                    {
                        var model = DIResolver.Get<IDLCObject>();
                        model.Path = p;
                        return model;
                    }).ToList();
                    return result;
                }
            }
            else
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
            var modRegistry = await LoadPdxModelAsync<ModRegistryCollection>(modRegistryPath) ?? new ModRegistryCollection();

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
        /// Resolves the content load path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private string ResolveContentLoadPath(string path)
        {
            // Why? Because Paradox went full paradox!
            // If the first mod path in the JSON array has casing Paradox doesn't like, the entire mod list fails...
            // ...even if every single other entry is correct...
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? PathOperations.GetActualPathCasing(path) : path;
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
