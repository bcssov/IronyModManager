// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 12-02-2022
// ***********************************************************************
// <copyright file="ModCollectionService.cs" company="Mario">
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
using IronyModManager.IO.Common.Models;
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

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ModCollectionService.
    /// Implements the <see cref="IronyModManager.Services.ModBaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IModCollectionService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.ModBaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IModCollectionService" />
    public class ModCollectionService : ModBaseService, IModCollectionService
    {
        #region Fields

        /// <summary>
        /// The database lock
        /// </summary>
        private static readonly object serviceLock = new { };

        /// <summary>
        /// The mod report exporter
        /// </summary>
        private readonly IReportExportService exportService;

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The mod collection exporter
        /// </summary>
        private readonly IModCollectionExporter modCollectionExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollectionService" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="exportService">The export service.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="modCollectionExporter">The mod collection exporter.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModCollectionService(IMessageBus messageBus, IReportExportService exportService, ICache cache,
            IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService, IModCollectionExporter modCollectionExporter,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.messageBus = messageBus;
            this.exportService = exportService;
            this.modCollectionExporter = modCollectionExporter;
        }

        #endregion Constructors

        #region Enums

        /// <summary>
        /// Enum ImportType
        /// </summary>
        protected enum ImportType
        {
            /// <summary>
            /// The paradox
            /// </summary>
            Paradox,

            /// <summary>
            /// The paradox launcher
            /// </summary>
            ParadoxLauncher,

            /// <summary>
            /// The paradoxos
            /// </summary>
            Paradoxos,

            /// <summary>
            /// The paradox launcher json
            /// </summary>
            ParadoxLauncherJson,

            /// <summary>
            /// The paradox launcher beta
            /// </summary>
            ParadoxLauncherBeta
        }

        #endregion Enums

        #region Methods

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>IModCollection.</returns>
        public virtual IModCollection Create()
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = GetModelInstance<IModCollection>();
            instance.Game = game.Type;
            return instance;
        }

        /// <summary>
        /// Deletes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Delete(string name)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections.Count > 0)
                {
                    var existing = collections.FirstOrDefault(p => p.Game.Equals(game.Type) && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                    {
                        collections.Remove(existing);
                        return StorageProvider.SetModCollections(collections);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Existses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Exists(string name)
        {
            var all = GetAll();
            return all.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <param name="exportOrderOnly">if set to <c>true</c> [export order only].</param>
        /// <param name="exportMods">if set to <c>true</c> [export mods].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExportAsync(string file, IModCollection modCollection, bool exportOrderOnly = false, bool exportMods = false)
        {
            var game = GameService.GetSelected();
            if (game == null || modCollection == null)
            {
                return Task.FromResult(false);
            }
            var collection = Mapper.Map<IModCollection>(modCollection);
            if (string.IsNullOrWhiteSpace(collection.MergedFolderName) && exportMods)
            {
                collection.MergedFolderName = collection.Name.GenerateValidFileName();
            }
            var path = GetPatchModDirectory(game, modCollection);
            var modNameOverride = $"({collection.Name}) ";
            var parameters = new ModCollectionExporterParams()
            {
                File = file,
                Mod = collection,
                ModDirectory = path,
                ExportModOrderOnly = exportOrderOnly,
                ModNameOverride = modNameOverride,
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            };
            if (exportMods)
            {
                parameters.ExportMods = GetCollectionMods(collectionName: modCollection.Name);
                var prefixModNames = new List<string>();
                collection.ModNames.ToList().ForEach(p => prefixModNames.Add(ModWriter.FormatPrefixModName(modNameOverride, p)));
                collection.ModNames = prefixModNames;
            }
            // Privacy
            collection.ModPaths = null;
            return modCollectionExporter.ExportAsync(parameters);
        }

        /// <summary>
        /// export hash report as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> ExportHashReportAsync(IEnumerable<IMod> mods, string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && mods?.Count() > 0)
            {
                var modExport = mods.ToList();
                var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
                var patchModName = GenerateCollectionPatchName(collection.Name);
                var allMods = GetInstalledModsInternal(GameService.GetSelected(), false);
                var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchModName));
                if (patchMod == null)
                {
                    var game = GameService.GetSelected();
                    if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
                    {
                        RootDirectory = GetPatchModDirectory(game, patchModName)
                    }))
                    {
                        patchMod = GeneratePatchModDescriptor(allMods, game, patchModName);
                    }
                }
                if (patchMod != null && collection.PatchModEnabled)
                {
                    modExport.Add(patchMod);
                }
                await PopulateModFilesInternalAsync(modExport);
                var reports = await ParseReportAsync(modExport);
                return await exportService.ExportAsync(reports, path);
            }
            return false;
        }

        /// <summary>
        /// Exports the paradox launcher202110 json asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExportParadoxLauncher202110JsonAsync(string file, IModCollection modCollection)
        {
            var game = GameService.GetSelected();
            if (game == null || modCollection == null)
            {
                return Task.FromResult(false);
            }
            var collection = Mapper.Map<IModCollection>(modCollection);
            var parameters = new ModCollectionExporterParams()
            {
                File = file,
                Mod = collection,
                ExportMods = GetCollectionMods(collectionName: modCollection.Name),
                Game = game,
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            };
            return modCollectionExporter.ExportParadoxLauncherJson202110Async(parameters);
        }

        /// <summary>
        /// Exports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="modCollection">The mod collection.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExportParadoxLauncherJsonAsync(string file, IModCollection modCollection)
        {
            var game = GameService.GetSelected();
            if (game == null || modCollection == null)
            {
                return Task.FromResult(false);
            }
            var collection = Mapper.Map<IModCollection>(modCollection);
            var parameters = new ModCollectionExporterParams()
            {
                File = file,
                Mod = collection,
                ExportMods = GetCollectionMods(collectionName: modCollection.Name),
                Game = game,
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            };
            return modCollectionExporter.ExportParadoxLauncherJsonAsync(parameters);
        }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IModCollection.</returns>
        public virtual IModCollection Get(string name)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var collections = StorageProvider.GetModCollections();
            if (collections?.Count() > 0)
            {
                var collection = collections.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Game.Equals(game.Type));
                return collection;
            }
            return null;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;IModCollection&gt;.</returns>
        public virtual IEnumerable<IModCollection> GetAll()
        {
            return GetAllModCollectionsInternal();
        }

        /// <summary>
        /// get imported collection details as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual async Task<IModCollection> GetImportedCollectionDetailsAsync(string file)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = GetModelInstance<IModCollection>();
            var result = await modCollectionExporter.ImportAsync(new ModCollectionExporterParams()
            {
                File = file,
                Mod = instance,
                DescriptorType = MapDescriptorType(game.ModDescriptorType)
            });
            if (result != null)
            {
                MapImportResult(instance, result, false);
                return instance;
            }
            return null;
        }

        /// <summary>
        /// import as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public async Task<IModCollection> ImportAsync(string file)
        {
            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            var instance = await GetImportedCollectionDetailsAsync(file);
            if (instance != null)
            {
                // Incase selected game != imported collection game
                if (!game.Type.Equals(instance.Game))
                {
                    game = GameService.Get().FirstOrDefault(p => p.Type.Equals(instance.Game));
                }
                var path = GetPatchModDirectory(game, instance);
                var exportPath = GetPatchModDirectory(game, !string.IsNullOrWhiteSpace(instance.MergedFolderName) ? instance.MergedFolderName : instance.Name);
                var result = await modCollectionExporter.ImportModDirectoryAsync(new ModCollectionExporterParams()
                {
                    File = file,
                    ModDirectory = path,
                    Mod = instance,
                    ExportModDirectory = exportPath,
                    DescriptorType = MapDescriptorType(game.ModDescriptorType)
                });
                if (result)
                {
                    return instance;
                }
            }
            return null;
        }

        /// <summary>
        /// Imports the hash report asynchronous.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>Task&lt;IEnumerable&lt;IModHashReport&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<IHashReport>> ImportHashReportAsync(IEnumerable<IMod> mods, IReadOnlyCollection<IHashReport> hashReports)
        {
            var importedReports = exportService.GetCollectionReports(hashReports);
            if (importedReports == null || !importedReports.Any())
            {
                return null;
            }
            var modExport = mods.ToList();
            var collection = GetAllModCollectionsInternal().FirstOrDefault(p => p.IsSelected);
            var patchModName = GenerateCollectionPatchName(collection.Name);
            var allMods = GetInstalledModsInternal(GameService.GetSelected(), false);
            var patchMod = allMods.FirstOrDefault(p => p.Name.Equals(patchModName));
            if (patchMod == null)
            {
                var game = GameService.GetSelected();
                if (await ModWriter.ModDirectoryExistsAsync(new ModWriterParameters()
                {
                    RootDirectory = GetPatchModDirectory(game, patchModName)
                }))
                {
                    patchMod = GeneratePatchModDescriptor(allMods, game, patchModName);
                }
            }
            if (patchMod != null)
            {
                modExport.Add(patchMod);
            }
            await PopulateModFilesInternalAsync(modExport);
            var currentReports = await ParseReportAsync(modExport);
            return exportService.CompareReports(currentReports.ToList(), importedReports.ToList());
        }

        /// <summary>
        /// import paradox as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual Task<IModCollection> ImportParadoxAsync()
        {
            return ImportModsAsync(ImportType.Paradox);
        }

        /// <summary>
        /// Imports the paradox launcher asynchronous.
        /// </summary>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual Task<IModCollection> ImportParadoxLauncherAsync()
        {
            return ImportModsAsync(ImportType.ParadoxLauncher);
        }

        /// <summary>
        /// Imports the paradox launcher beta asynchronous.
        /// </summary>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual Task<IModCollection> ImportParadoxLauncherBetaAsync()
        {
            return ImportModsAsync(ImportType.ParadoxLauncherBeta);
        }

        /// <summary>
        /// Imports the paradox launcher json asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual Task<IModCollection> ImportParadoxLauncherJsonAsync(string file)
        {
            return ImportModsAsync(ImportType.ParadoxLauncherJson, file);
        }

        /// <summary>
        /// import paradoxos as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;IModCollection&gt;.</returns>
        public virtual Task<IModCollection> ImportParadoxosAsync(string file)
        {
            return ImportModsAsync(ImportType.Paradoxos, file);
        }

        /// <summary>
        /// Saves the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">collection</exception>
        public virtual bool Save(IModCollection collection)
        {
            if (collection == null || string.IsNullOrWhiteSpace(collection.Game))
            {
                throw new ArgumentNullException(nameof(collection));
            }
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections.Count > 0)
                {
                    var existing = collections.FirstOrDefault(p => p.Name.Equals(collection.Name, StringComparison.OrdinalIgnoreCase) && p.Game.Equals(collection.Game));
                    if (existing != null)
                    {
                        collections.Remove(existing);
                    }
                    if (collection.IsSelected)
                    {
                        foreach (var item in collections.Where(p => p.Game.Equals(collection.Game) && p.IsSelected))
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                collections.Add(collection);
                return StorageProvider.SetModCollections(collections);
            }
        }

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Double.</returns>
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

        /// <summary>
        /// import mods as an asynchronous operation.
        /// </summary>
        /// <param name="importType">Type of the import.</param>
        /// <param name="file">The file.</param>
        /// <returns>IModCollection.</returns>
        protected virtual async Task<IModCollection> ImportModsAsync(ImportType importType, string file = Shared.Constants.EmptyParam)
        {
            async Task<IModCollection> performImport(IGame game)
            {
                var instance = Create();
                var parameters = new ModCollectionExporterParams()
                {
                    ModDirectory = Path.Combine(game.UserDirectory, Shared.Constants.ModDirectory),
                    File = file,
                    Mod = instance,
                    DescriptorType = MapDescriptorType(game.ModDescriptorType)
                };
                ICollectionImportResult result = null;
                switch (importType)
                {
                    case ImportType.Paradox:
                        result = await modCollectionExporter.ImportParadoxAsync(parameters);
                        break;

                    case ImportType.ParadoxLauncher:
                        result = await modCollectionExporter.ImportParadoxLauncherAsync(parameters);
                        break;

                    case ImportType.Paradoxos:
                        result = await modCollectionExporter.ImportParadoxosAsync(parameters);
                        break;

                    case ImportType.ParadoxLauncherBeta:
                        result = await modCollectionExporter.ImportParadoxLauncherBetaAsync(parameters);
                        break;

                    case ImportType.ParadoxLauncherJson:
                        result = await modCollectionExporter.ImportParadoxLauncherJsonAsync(parameters);
                        break;

                    default:
                        break;
                }
                if (result != null)
                {
                    // Order of operations is very important here
                    MapImportResult(instance, result, true);
                    return instance;
                }
                return null;
            }

            var game = GameService.GetSelected();
            if (game == null)
            {
                return null;
            }
            return await performImport(game);
        }

        /// <summary>
        /// Maps the import result.
        /// </summary>
        /// <param name="modCollection">The mod collection.</param>
        /// <param name="importResult">The import result.</param>
        /// <param name="importByOtherModId">if set to <c>true</c> [import by other mod identifier].</param>
        protected virtual void MapImportResult(IModCollection modCollection, ICollectionImportResult importResult, bool importByOtherModId)
        {
            if (!string.IsNullOrWhiteSpace(importResult.Game))
            {
                var collectionGame = GameService.Get().FirstOrDefault(p => p.Type.Equals(importResult.Game));
                collectionGame ??= GameService.Get().FirstOrDefault(p => p.ParadoxGameId.Equals(importResult.Game));
                if (collectionGame != null)
                {
                    modCollection.Game = collectionGame.Type;
                }
            }
            modCollection.IsSelected = importResult.IsSelected;
            modCollection.MergedFolderName = importResult.MergedFolderName;
            modCollection.ModNames = importResult.ModNames;
            modCollection.Mods = importResult.Descriptors;
            modCollection.Name = importResult.Name;
            modCollection.PatchModEnabled = importResult.PatchModEnabled;
            modCollection.ModIds = importResult.ModIds;
            if (importByOtherModId)
            {
                if (importResult.ModIds != null && importResult.ModIds.Any())
                {
                    var mods = GetInstalledModsInternal(modCollection.Game, false);
                    if (mods.Any())
                    {
                        var sort = importResult.ModIds.ToList();
                        var collectionMods = mods.Where(p => importResult.ModIds.Any(x => (x.ParadoxId.HasValue && x.ParadoxId.GetValueOrDefault() == p.RemoteId.GetValueOrDefault()) || (x.SteamId.HasValue && x.SteamId.GetValueOrDefault() == p.RemoteId.GetValueOrDefault()))).
                            OrderBy(p => sort.IndexOf(sort.FirstOrDefault(x => (x.ParadoxId.HasValue && x.ParadoxId.GetValueOrDefault() == p.RemoteId.GetValueOrDefault()) || (x.SteamId.HasValue && x.SteamId.GetValueOrDefault() == p.RemoteId.GetValueOrDefault())))).ToList();
                        collectionMods = collectionMods.GroupBy(p => p.FullPath).Select(p => p.FirstOrDefault()).ToList();
                        modCollection.Mods = collectionMods.Select(p => p.DescriptorFile).ToList();
                        modCollection.ModNames = collectionMods.Select(p => p.Name).ToList();
                        modCollection.ModPaths = collectionMods.Select(p => p.FullPath).ToList();
                    }
                }
                else if (importResult.FullPaths != null && importResult.FullPaths.Any())
                {
                    var mods = GetInstalledModsInternal(modCollection.Game, false);
                    if (mods.Any())
                    {
                        var sort = importResult.FullPaths.ToList();
                        var collectionMods = mods.Where(p => importResult.FullPaths.Any(x => x.StandardizeDirectorySeparator().Equals(p.FullPath.StandardizeDirectorySeparator(), StringComparison.OrdinalIgnoreCase))).
                            OrderBy(p => sort.IndexOf(sort.FirstOrDefault(x => x.StandardizeDirectorySeparator().Equals(p.FullPath.StandardizeDirectorySeparator(), StringComparison.OrdinalIgnoreCase)))).ToList();
                        collectionMods = collectionMods.GroupBy(p => p.FullPath).Select(p => p.FirstOrDefault()).ToList();
                        modCollection.Mods = collectionMods.Select(p => p.DescriptorFile).ToList();
                        modCollection.ModNames = collectionMods.Select(p => p.Name).ToList();
                        modCollection.ModPaths = collectionMods.Select(p => p.FullPath).ToList();
                    }
                }
            }
            else
            {
                var mods = GetInstalledModsInternal(modCollection.Game, false);
                if (mods.Any() && importResult.Descriptors != null && importResult.Descriptors.Any())
                {
                    var sort = importResult.Descriptors.ToList();
                    var collectionMods = mods.Where(p => importResult.Descriptors.Any(x => x.Equals(p.DescriptorFile, StringComparison.OrdinalIgnoreCase))).OrderBy(p => sort.IndexOf(sort.FirstOrDefault(x => x.Equals(p.DescriptorFile, StringComparison.OrdinalIgnoreCase)))).ToList();
                    modCollection.ModPaths = collectionMods.Select(p => p.FullPath).ToList();
                    if (modCollection.ModPaths.Count() != modCollection.Mods.Count() && importResult.ModNames != null && importResult.ModNames.Any())
                    {
                        sort = importResult.ModNames.ToList();
                        collectionMods = mods.Where(p => importResult.ModNames.Any(x => x.Equals(p.Name))).OrderBy(p => sort.IndexOf(sort.FirstOrDefault(x => x.Equals(p.Name)))).ToList();
                        collectionMods = collectionMods.GroupBy(p => p.FullPath).Select(p => p.FirstOrDefault()).ToList();
                        modCollection.ModPaths = collectionMods.Select(p => p.FullPath).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// parse report as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>IEnumerable&lt;IModHashReport&gt;.</returns>
        protected virtual async Task<IEnumerable<IHashReport>> ParseReportAsync(IEnumerable<IMod> mods)
        {
            var game = GameService.GetSelected();
            var reports = new List<IHashReport>();
            var total = mods.SelectMany(p => p.Files).Count(p => game.GameFolders.Any(a => p.StartsWith(a)));
            var progress = 0;
            double lastPercentage = 0;
            foreach (var mod in mods)
            {
                var report = GetModelInstance<IHashReport>();
                report.Name = mod.Name;
                report.ReportType = HashReportType.Collection;
                var hashReports = new List<IHashFileReport>();
                foreach (var item in mod.Files.Where(p => game.GameFolders.Any(a => p.StartsWith(a))))
                {
                    var info = Reader.GetFileInfo(mod.FullPath, item);
                    if (info != null)
                    {
                        var fileReport = GetModelInstance<IHashFileReport>();
                        fileReport.File = item;
                        fileReport.Hash = info.ContentSHA;
                        hashReports.Add(fileReport);
                    }
                    progress++;
                    var percentage = GetProgressPercentage(total, progress);
                    if (percentage != lastPercentage)
                    {
                        await messageBus.PublishAsync(new ModReportExportEvent(1, percentage));
                    }
                    lastPercentage = percentage;
                }
                report.Reports = hashReports;
                reports.Add(report);
            }
            return reports;
        }

        #endregion Methods
    }
}
