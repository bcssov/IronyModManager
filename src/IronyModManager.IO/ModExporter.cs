// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2020
// ***********************************************************************
// <copyright file="ModExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.IO.Common;
using IronyModManager.IO.Models;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class FileWriter.
    /// Implements the <see cref="IronyModManager.IO.Common.IModExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.IModExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModExporter : IModExporter
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
        /// apply collection as an asynchronous operation.
        /// </summary>
        /// <param name="collectionMods">The collection mods.</param>
        /// <param name="rootDirectory">The root directory.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ApplyCollectionAsync(IReadOnlyCollection<IMod> collectionMods, string rootDirectory)
        {
            var dlcPath = Path.Combine(rootDirectory, DLC_load_path);
            var gameDataPath = Path.Combine(rootDirectory, Game_data_path);
            var modRegistryPath = Path.Combine(rootDirectory, Mod_registry_path);
            var dLCLoad = await LoadPdxModelAsync<DLCLoad>(dlcPath) ?? new DLCLoad();
            var gameData = await LoadPdxModelAsync<GameData>(gameDataPath) ?? new GameData();
            var modRegistry = await LoadPdxModelAsync<ModRegistryCollection>(modRegistryPath) ?? new ModRegistryCollection();

            gameData.ModsOrder.Clear();
            dLCLoad.EnabledMods.Clear();
            foreach (var mod in collectionMods)
            {
                // Populate registry
                if (!modRegistry.Values.Any(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase)))
                {
                    var pdxMod = new ModRegistry()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DisplayName = mod.Name,
                        Tags = mod.Tags.ToList(),
                        RequiredVersion = mod.Version,
                        GameRegistryId = mod.DescriptorFile,
                        Status = Ready_to_play,
                        Source = MapPdxType(mod.Source)
                    };
                    MapPdxPath(pdxMod, mod);
                    MapPdxId(pdxMod, mod);
                    modRegistry.Add(pdxMod.Id, pdxMod);
                }
                // Populate game data
                var entry = modRegistry.Values.FirstOrDefault(p => p.GameRegistryId.Equals(mod.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                gameData.ModsOrder.Add(entry.Id);

                // Populate dlc
                dLCLoad.EnabledMods.Add(mod.DescriptorFile);
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
        /// Exports the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exportPath">The export path.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="modDirectory">The mod directory.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportAsync<T>(string exportPath, T mod, string modDirectory) where T : IModCollection
        {
            // TODO: Add logic for this, at the moment there is no conflict detector
            if (Directory.Exists(modDirectory))
            {
            }
            var content = JsonConvert.SerializeObject(mod, Formatting.Indented);
            using var zip = ArchiveFactory.Create(SharpCompress.Common.ArchiveType.Zip);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            zip.AddEntry(Common.Constants.ExportedModContentId, stream, false);
            zip.SaveTo(exportPath, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate));
            zip.Dispose();
            return Task.FromResult(true);
        }

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">The file.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ImportAsync<T>(string file, T mod) where T : IModCollection
        {
            using var fileStream = File.OpenRead(file);
            using var reader = ReaderFactory.Open(fileStream);
            var result = false;
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var relativePath = reader.Entry.Key.Trim("\\/".ToCharArray()).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                    using var entryStream = reader.OpenEntryStream();
                    using var memoryStream = new MemoryStream();
                    entryStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    if (reader.Entry.Key.Equals(Common.Constants.ExportedModContentId, StringComparison.OrdinalIgnoreCase))
                    {
                        using var streamReader = new StreamReader(memoryStream, true);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        JsonConvert.PopulateObject(text, mod);
                        result = true;
                    }
                    else
                    {
                        // TODO: Add logic for mod directory import, there is no conflict detector at the moment
                    }
                }
            }
            return Task.FromResult(result);
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
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            return true;
        }

        #endregion Methods
    }
}
