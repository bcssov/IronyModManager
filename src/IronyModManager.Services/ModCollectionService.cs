// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
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
using IronyModManager.DI;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Services.Common.MessageBus;
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
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The mod collection exporter
        /// </summary>
        private readonly IModCollectionExporter modCollectionExporter;

        /// <summary>
        /// The mod report exporter
        /// </summary>
        private readonly IModReportExporter modReportExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollectionService" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="modReportExporter">The mod report exporter.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="modCollectionExporter">The mod collection exporter.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ModCollectionService(IMessageBus messageBus, IModReportExporter modReportExporter, ICache cache, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders, IReader reader, IModWriter modWriter,
            IModParser modParser, IGameService gameService, IModCollectionExporter modCollectionExporter,
            IStorageProvider storageProvider, IMapper mapper) : base(cache, definitionInfoProviders, reader, modWriter, modParser, gameService, storageProvider, mapper)
        {
            this.messageBus = messageBus;
            this.modReportExporter = modReportExporter;
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
            Paradoxos
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
                if (collections.Count() > 0)
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
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ExportAsync(string file, IModCollection modCollection, bool exportOrderOnly = false)
        {
            var game = GameService.GetSelected();
            if (game == null || modCollection == null)
            {
                return Task.FromResult(false);
            }
            var path = GetModDirectory(game, modCollection);
            return modCollectionExporter.ExportAsync(new ModCollectionExporterParams()
            {
                File = file,
                Mod = modCollection,
                ModDirectory = path,
                ExportModOrderOnly = exportOrderOnly
            });
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
                var reports = await ParseReportAsync(mods);
                return await modReportExporter.ExportAsync(reports, path);
            }
            return false;
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
                Mod = instance
            });
            if (result)
            {
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
                var path = GetModDirectory(game, instance);
                if (await modCollectionExporter.ImportModDirectoryAsync(new ModCollectionExporterParams()
                {
                    File = file,
                    ModDirectory = path,
                    Mod = instance
                }))
                {
                    return instance;
                }
            }
            return null;
        }

        /// <summary>
        /// Imports the hash report asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IModHashReport&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<IModHashReport>> ImportHashReportAsync(IEnumerable<IMod> mods, string path)
        {
            var currentReports = await ParseReportAsync(mods);
            var importedReports = await modReportExporter.ImportAsync(path);
            if (importedReports != null)
            {
                static void compareReports(List<IModHashReport> reports, IEnumerable<IModHashReport> firstReports, IEnumerable<IModHashReport> secondReports)
                {
                    foreach (var first in firstReports)
                    {
                        if (!secondReports.Any(p => p.Name.Equals(first.Name)))
                        {
                            if (!reports.Any(p => p.Name.Equals(first.Name)))
                            {
                                reports.Add(first);
                            }
                            continue;
                        }
                        foreach (var item in first.Reports)
                        {
                            var secondReport = secondReports.FirstOrDefault(p => p.Name.Equals(first.Name));
                            if (!secondReport.Reports.Any(p => p.File.Equals(item.File) && p.Hash.Equals(item.Hash)))
                            {
                                var report = reports.FirstOrDefault(p => p.Name.Equals(first.Name));
                                if (report == null)
                                {
                                    report = DIResolver.Get<IModHashReport>();
                                    report.Name = first.Name;
                                    reports.Add(report);
                                }
                                if (report.Reports == null)
                                {
                                    report.Reports = new List<IModHashFileReport>();
                                }
                                if (!report.Reports.Any(p => p.File.Equals(item.File)))
                                {
                                    var hashReport = DIResolver.Get<IModHashFileReport>();
                                    hashReport.File = item.File;
                                    report.Reports.Add(hashReport);
                                }
                            }
                        }
                    }
                }
                var reports = new List<IModHashReport>();
                compareReports(reports, currentReports, importedReports);
                compareReports(reports, importedReports, currentReports);
                return reports;
            }
            return null;
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
        /// <exception cref="ArgumentNullException">collection</exception>
        public virtual bool Save(IModCollection collection)
        {
            if (collection == null || string.IsNullOrWhiteSpace(collection.Game))
            {
                throw new ArgumentNullException("collection");
            }
            var game = GameService.GetSelected();
            if (game == null)
            {
                return false;
            }
            lock (serviceLock)
            {
                var collections = StorageProvider.GetModCollections().ToList();
                if (collections.Count() > 0)
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
        /// <returns>System.Int32.</returns>
        protected virtual int GetProgressPercentage(double total, double processed, int maxPerc = 100)
        {
            var perc = Convert.ToInt32(processed / total * 100);
            if (perc < 1)
            {
                perc = 1;
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
                    Mod = instance
                };
                bool result = false;
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

                    default:
                        break;
                }
                if (result)
                {
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
        /// parse report as an asynchronous operation.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <returns>IEnumerable&lt;IModHashReport&gt;.</returns>
        protected virtual async Task<IEnumerable<IModHashReport>> ParseReportAsync(IEnumerable<IMod> mods)
        {
            var game = GameService.GetSelected();
            var reports = new List<IModHashReport>();
            var total = mods.SelectMany(p => p.Files).Count(p => game.GameFolders.Any(a => p.StartsWith(a)));
            var progress = 0;
            var lastPercentage = 0;
            foreach (var mod in mods)
            {
                var report = GetModelInstance<IModHashReport>();
                report.Name = mod.Name;
                var hashReports = new List<IModHashFileReport>();
                foreach (var item in mod.Files.Where(p => game.GameFolders.Any(a => p.StartsWith(a))))
                {
                    var info = Reader.GetFileInfo(mod.FullPath, item);
                    if (info != null)
                    {
                        var fileReport = GetModelInstance<IModHashFileReport>();
                        fileReport.File = item;
                        fileReport.Hash = info.ContentSHA;
                        hashReports.Add(fileReport);
                    }
                    progress++;
                    var percentage = GetProgressPercentage(total, progress);
                    if (percentage != lastPercentage)
                    {
                        await messageBus.PublishAsync(new ModReportExportEvent(percentage));
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
