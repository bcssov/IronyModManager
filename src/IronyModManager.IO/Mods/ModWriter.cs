// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-09-2020
// ***********************************************************************
// <copyright file="ModWriter.cs" company="Mario">
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
using IronyModManager.IO.Mods.Models;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModWriter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModWriter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModWriter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModWriter : IModWriter
    {
        #region Fields

        /// <summary>
        /// The DLC load path
        /// </summary>
        private const string DLC_load_path = "dlc_load.json";

        /// <summary>
        /// The game data path
        /// </summary>
        private const string Game_data_path = "game_data.json";

        /// <summary>
        /// The mod registry path
        /// </summary>
        private const string Mod_registry_path = "mods_registry.json";

        /// <summary>
        /// The ready to play
        /// </summary>
        private const string Ready_to_play = "ready_to_play";

        #endregion Fields

        #region Methods

        /// <summary>
        /// apply mods as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ApplyModsAsync(ModWriterParameters parameters)
        {
            var dlcPath = Path.Combine(parameters.RootDirectory, DLC_load_path);
            var gameDataPath = Path.Combine(parameters.RootDirectory, Game_data_path);
            var modRegistryPath = Path.Combine(parameters.RootDirectory, Mod_registry_path);
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
                if (pdxMod.Value.Status != Ready_to_play)
                {
                    toRemove.Add(pdxMod.Key);
                }
            }
            foreach (var item in toRemove)
            {
                modRegistry.Remove(item);
            }

            if (parameters.Mods != null)
            {
                foreach (var mod in parameters.Mods)
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
                    pdxMod.DisplayName = mod.Name;
                    pdxMod.Tags = mod.Tags.ToList();
                    pdxMod.RequiredVersion = mod.Version;
                    pdxMod.GameRegistryId = mod.DescriptorFile;
                    pdxMod.Status = Ready_to_play;
                    pdxMod.Source = MapPdxType(mod.Source);
                    MapPdxPath(pdxMod, mod);
                    MapPdxId(pdxMod, mod);

                    // Populate game data
                    var entry = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                    gameData.ModsOrder.Add(entry.Id);

                    // Populate dlc
                    dLCLoad.EnabledMods.Add(mod.DescriptorFile);
                }
            }

            if (parameters.HiddenMods != null)
            {
                foreach (var hiddenMod in parameters.HiddenMods)
                {
                    if (!dLCLoad.EnabledMods.Any(p => p.Equals(hiddenMod.DescriptorFile, StringComparison.OrdinalIgnoreCase)))
                    {
                        dLCLoad.EnabledMods.Add(hiddenMod.DescriptorFile);
                    }
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
        /// Creates the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> CreateModDirectoryAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Descriptors the exists asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> DescriptorExistsAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Path);
            if (File.Exists(fullPath))
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Purges the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> PurgeModDirectoryAsync(ModWriterParameters parameters)
        {
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Path);
            if (Directory.Exists(fullPath))
            {
                var files = Directory.EnumerateFiles(fullPath, "*", SearchOption.TopDirectoryOnly);
                foreach (var item in files)
                {
                    File.Delete(item);
                }
                return Task.FromResult(true);
            }
            else if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Writes the descriptor asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> WriteDescriptorAsync(ModWriterParameters parameters)
        {
            // If needed I've got a much more complex serializer, it is written for Kerbal Space Program but the structure seems to be the same though this is much more simpler
            var fullPath = Path.Combine(parameters.RootDirectory, parameters.Path);
            using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            using var sw = new StreamWriter(fs);
            var props = parameters.Mod.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(DescriptorPropertyAttribute)));
            foreach (var prop in props)
            {
                var attr = Attribute.GetCustomAttribute(prop, typeof(DescriptorPropertyAttribute), true) as DescriptorPropertyAttribute;
                var val = prop.GetValue(parameters.Mod, null);
                if (val is IEnumerable<string> col)
                {
                    if (col.Count() > 0)
                    {
                        await sw.WriteLineAsync($"{attr.PropertyName}={{");
                        foreach (var item in col)
                        {
                            await sw.WriteLineAsync($"\t\"{item}\"");
                        }
                        await sw.WriteLineAsync("}");
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(val != null ? val.ToString() : string.Empty))
                    {
                        await sw.WriteLineAsync($"{attr.PropertyName}=\"{val}\"");
                    }
                }
            }
            await sw.FlushAsync();
            return true;
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
                var result = JsonConvert.DeserializeObject<T>(content);
                return result;
            }
            return default;
        }

        /// <summary>
        /// Maps the PDX identifier.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="mod">The mod.</param>
        private void MapPdxId(ModRegistry registry, IMod mod)
        {
            if (mod.RemoteId.HasValue)
            {
                switch (mod.Source)
                {
                    case ModSource.Paradox:
                        registry.PdxId = mod.RemoteId.ToString();
                        break;

                    default:
                        // Assume steam
                        registry.SteamId = mod.RemoteId.ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// Maps the PDX path.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="mod">The mod.</param>
        private void MapPdxPath(ModRegistry registry, IMod mod)
        {
            if (mod.FileName.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase))
            {
                registry.ArchivePath = mod.FileName;
            }
            else
            {
                registry.DirPath = mod.FileName;
            }
        }

        /// <summary>
        /// Maps the type of the PDX.
        /// </summary>
        /// <param name="modSource">The mod source.</param>
        /// <returns>System.String.</returns>
        private string MapPdxType(ModSource modSource)
        {
            var pdxSource = modSource switch
            {
                ModSource.Paradox => "pdx",
                ModSource.Steam => "steam",
                _ => "local",
            };
            return pdxSource;
        }

        /// <summary>
        /// write PDX model as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private async Task<bool> WritePdxModelAsync<T>(T model, string path) where T : IPdxFormat
        {
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            return true;
        }

        #endregion Methods
    }
}
