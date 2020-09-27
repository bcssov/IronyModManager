// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 09-25-2020
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
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.Common;
using IronyModManager.IO.Mods.Models.Paradox.v1;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using Newtonsoft.Json;

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
        #region Methods

        /// <summary>
        /// export as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ExportAsync(ModWriterParameters parameters)
        {
            var dlcPath = Path.Combine(parameters.RootDirectory, Constants.DLC_load_path);
            var gameDataPath = Path.Combine(parameters.RootDirectory, Constants.Game_data_path);
            var modRegistryPath = Path.Combine(parameters.RootDirectory, Constants.Mod_registry_path);
            var dLCLoad = await LoadPdxModelAsync<DLCLoad>(dlcPath) ?? new DLCLoad();
            var gameData = await LoadPdxModelAsync<GameData>(gameDataPath) ?? new GameData();
            var modRegistry = await LoadPdxModelAsync<ModRegistryCollection>(modRegistryPath) ?? new ModRegistryCollection();

            if (!parameters.AppendOnly)
            {
                gameData.ModsOrder.Clear();
                dLCLoad.EnabledMods.Clear();
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
                    SyncData(dLCLoad, gameData, modRegistry, mod, true);
                }
            }

            if (parameters.OtherMods != null)
            {
                foreach (var mod in parameters.OtherMods)
                {
                    SyncData(dLCLoad, gameData, modRegistry, mod, false);
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
                    var existingEnabledMod = dLCLoad.EnabledMods.FirstOrDefault(p => p.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(existingEnabledMod))
                    {
                        dLCLoad.EnabledMods.Remove(existingEnabledMod);
                    }
                    SyncData(dLCLoad, gameData, modRegistry, mod, true);
                }
            }

            var tasks = new Task<bool>[]
            {
                WritePdxModelAsync(dLCLoad, dlcPath),
                WritePdxModelAsync(gameData, gameDataPath),
                WritePdxModelAsync(modRegistry, modRegistryPath),
            };
            await Task.WhenAll(tasks);

            return tasks.All(p => p.Result);
        }

        /// <summary>
        /// load PDX model as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        private async Task<T> LoadPdxModelAsync<T>(string path) where T : IPdxFormat
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
        /// Synchronizes the data.
        /// </summary>
        /// <param name="dLCLoad">The d lc load.</param>
        /// <param name="gameData">The game data.</param>
        /// <param name="modRegistry">The mod registry.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        private void SyncData(DLCLoad dLCLoad, GameData gameData, ModRegistryCollection modRegistry, IMod mod, bool isEnabled)
        {
            ModRegistry pdxMod;
            // Populate registry
            if (!modRegistry.Values.Any(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase)))
            {
                pdxMod = new ModRegistry()
                {
                    Id = Guid.NewGuid().ToString()
                };
                modRegistry.Add(pdxMod.Id, pdxMod);
            }
            else
            {
                pdxMod = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
            }
            MapModData(pdxMod, mod);

            // Populate game data
            var entry = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
            gameData.ModsOrder.Add(entry.Id);

            // Populate dlc
            if (isEnabled)
            {
                dLCLoad.EnabledMods.Add(mod.DescriptorFile);
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
                    Directory.CreateDirectory(dirPath);
                }

                if (File.Exists(path))
                {
                    _ = new System.IO.FileInfo(path)
                    {
                        IsReadOnly = false
                    };
                }
                await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));
                return true;
            }

            var retry = new RetryStrategy();
            return await retry.RetryActionAsync(writeFile);
        }

        #endregion Methods
    }
}
