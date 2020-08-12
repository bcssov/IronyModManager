// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="SQLiteExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.v2;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using Microsoft.Data.Sqlite;
using RepoDb;

namespace IronyModManager.IO.Mods.Exporter
{
    /// <summary>
    /// Class SQLiteExporter.
    /// Implements the <see cref="IronyModManager.IO.Mods.Exporter.BaseExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Exporter.BaseExporter" />
    internal class SQLiteExporter : BaseExporter
    {
        #region Fields

        /// <summary>
        /// The export prefix
        /// </summary>
        private const string ExportName = nameof(IronyModManager);

        /// <summary>
        /// The collection sort type
        /// </summary>
        private const string CollectionSortType = "custom"; // magic string for paradox

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The trace
        /// </summary>
        private readonly SQLTraceLog trace;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteExporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SQLiteExporter(ILogger logger)
        {
            this.logger = logger;
            trace = new SQLTraceLog(logger);
        }

        #region Methods

        /// <summary>
        /// export as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ExportAsync(ModWriterParameters parameters)
        {
            EnsureDbExists(parameters);
            var collection = await RecreateCollectionAsync(parameters);
            await SyncModsAsync(collection, parameters);
            return true;
        }

        /// <summary>
        /// Ensures the database exists.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        private void EnsureDbExists(ModWriterParameters parameters)
        {
            var db = GetDbPath(parameters);
            if (!File.Exists(db))
            {
                File.Copy(Constants.Empty_sql_db_path, db);
            }
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetCollectionName()
        {
            return ExportName;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDbConnection.</returns>
        private IDbConnection GetConnection(ModWriterParameters parameters)
        {
            return new SqliteConnection($"Data Source=\"{GetDbPath(parameters)}\"").EnsureOpen();
        }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        private string GetDbPath(ModWriterParameters parameters)
        {
            return Path.Combine(parameters.RootDirectory, Constants.Sql_db_path);
        }

        /// <summary>
        /// recreate collection as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Playsets.</returns>
        private async Task<Playsets> RecreateCollectionAsync(ModWriterParameters parameters)
        {
            Playsets getDefaultIronyCollection()
            {
                return new Playsets()
                {
                    id = Guid.NewGuid(),
                    IsActive = true,
                    LoadOrder = CollectionSortType,
                    Name = GetCollectionName()
                };
            }

            var colName = GetCollectionName();

            // They did do a cascade delete right?
            using var con = GetConnection(parameters);
            using var transaction = con.BeginTransaction();

            var activeCollections = (await con.QueryAsync<Playsets>(p => p.IsActive == true, trace: trace)).ToList().Where(p => !p.Name.Equals(colName)).ToList();
            try
            {
                Playsets ironyCollection;
                if (!parameters.AppendOnly)
                {
                    await con.DeleteAsync<Playsets>(p => p.Name == colName, transaction: transaction, trace: trace);

                    if (activeCollections.Count() > 0)
                    {
                        foreach (var item in activeCollections)
                        {
                            item.IsActive = false;
                        }
                        await con.UpdateAllAsync(activeCollections, transaction: transaction, trace: trace);
                    }

                    ironyCollection = getDefaultIronyCollection();
                    await con.InsertAsync(ironyCollection, transaction: transaction, trace: trace);
                }
                else
                {
                    ironyCollection = (await con.QueryAsync<Playsets>(p => p.Name == colName, trace: trace)).FirstOrDefault();

                    if (activeCollections.Count() > 0)
                    {
                        foreach (var item in activeCollections)
                        {
                            item.IsActive = false;
                        }
                        await con.UpdateAllAsync(activeCollections, transaction: transaction, trace: trace);
                    }

                    if (ironyCollection == null)
                    {
                        ironyCollection = getDefaultIronyCollection();
                        await con.InsertAsync(ironyCollection, transaction: transaction, trace: trace);
                    }
                    else
                    {
                        ironyCollection.IsActive = true;
                        await con.UpdateAsync(ironyCollection, transaction: transaction, trace: trace);
                    }
                }

                transaction.Commit();

                return ironyCollection;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Maps the PDX mod.
        /// </summary>
        /// <param name="pdxMod">The PDX mod.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>Models.Paradox.v2.Mods.</returns>
        private Models.Paradox.v2.Mods MapPdxMod(Models.Paradox.v2.Mods pdxMod, IMod mod)
        {
            if (pdxMod == null)
            {
                pdxMod = new Models.Paradox.v2.Mods()
                {
                    id = Guid.NewGuid()
                };
            }
            MapModData(pdxMod, mod);
            return pdxMod;
        }

        /// <summary>
        /// prepare mods transaction as an asynchronous operation.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="exportMods">The export mods.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="removeInvalid">if set to <c>true</c> [remove invalid].</param>
        /// <returns>IEnumerable&lt;Models.Paradox.v2.Mods&gt;.</returns>
        private async Task<IEnumerable<Models.Paradox.v2.Mods>> PrepareModsTransactionAsync(IDbConnection con, IDbTransaction transaction, List<IMod> exportMods, IEnumerable<Models.Paradox.v2.Mods> mods, bool removeInvalid)
        {
            var result = new HashSet<Models.Paradox.v2.Mods>();
            if (mods?.Count() > 0)
            {
                var toRemove = new HashSet<Models.Paradox.v2.Mods>();
                var toInsert = new HashSet<Models.Paradox.v2.Mods>();
                var toUpdate = new HashSet<Models.Paradox.v2.Mods>();
                if (removeInvalid)
                {
                    foreach (var item in mods)
                    {
                        if (item.Status != Constants.Ready_to_play)
                        {
                            toRemove.Add(item);
                        }
                    }
                }
                foreach (var item in exportMods)
                {
                    var pdxMod = mods.FirstOrDefault(p => p.GameRegistryId.Equals(item.DescriptorFile, StringComparison.OrdinalIgnoreCase));
                    if (pdxMod == null)
                    {
                        var newPdxMod = MapPdxMod(null, item);
                        toInsert.Add(newPdxMod);
                        result.Add(newPdxMod);
                    }
                    else
                    {
                        // Pending delete, insert a new entry instead
                        if (toRemove.Contains(pdxMod))
                        {
                            var newPdxMod = MapPdxMod(null, item);
                            toInsert.Add(newPdxMod);
                            result.Add(newPdxMod);
                        }
                        else
                        {
                            var updatedPdxMod = MapPdxMod(pdxMod, item);
                            toUpdate.Add(updatedPdxMod);
                            result.Add(updatedPdxMod);
                        }
                    }
                }
                if (toRemove.Count > 0)
                {
                    await con.DeleteAllAsync(toRemove, transaction: transaction, trace: trace);
                }
                if (toUpdate.Count > 0)
                {
                    await con.UpdateAllAsync(toUpdate, transaction: transaction, trace: trace);
                }
                if (toInsert.Count > 0)
                {
                    await con.InsertAllAsync(toInsert, transaction: transaction, trace: trace);
                }
            }
            else
            {
                var toInsert = new HashSet<Models.Paradox.v2.Mods>();
                foreach (var mod in exportMods)
                {
                    var pdxMod = MapPdxMod(null, mod);
                    toInsert.Add(pdxMod);
                    result.Add(pdxMod);
                }
                if (toInsert.Count > 0)
                {
                    await con.InsertAllAsync(toInsert, transaction: transaction, trace: trace);
                }
            }
            return result;
        }

        /// <summary>
        /// Prepares the playset mods transaction asynchronous.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="recreateCollection">The recreate collection.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        private async Task PreparePlaysetModsTransactionAsync(IDbConnection con, IDbTransaction transaction, Playsets collection, IEnumerable<Models.Paradox.v2.Mods> mods, bool recreateCollection)
        {
            PlaysetsMods mapMod(Models.Paradox.v2.Mods mod, int position)
            {
                return new PlaysetsMods()
                {
                    Enabled = true,
                    modId = mod.id.ToString(),
                    playsetId = collection.id.ToString(),
                    Position = formatPosition(position)
                };
            }

            static string formatPosition(int position)
            {
                // Why not simply use numbers? What actually makes sense!!! Therefore generate a 10 digit int with trailing zeroes...
                return position.ToString("0000000000");
            }

            if (mods?.Count() > 0)
            {
                var collectionMods = await con.QueryAsync<PlaysetsMods>(p => p.playsetId == collection.id.ToString(), trace: trace);
                int pos = 1000;
                if (recreateCollection || collectionMods == null || collectionMods.Count() == 0)
                {
                    if (recreateCollection)
                    {
                        await con.DeleteAsync<PlaysetsMods>(p => p.playsetId == collection.id.ToString(), transaction: transaction, trace: trace);
                    }

                    var toInsert = new List<PlaysetsMods>();
                    foreach (var item in mods)
                    {
                        toInsert.Add(mapMod(item, pos));
                        pos++;
                    }
                    if (toInsert.Count > 0)
                    {
                        await con.InsertAllAsync(toInsert, transaction: transaction, trace: trace);
                    }
                }
                else
                {
                    // 33333333ci, 3333333335 - samples autogenerated by pdx launcher, what the actual fuck?
                    var toInsert = new HashSet<PlaysetsMods>();
                    var toUpdate = new HashSet<PlaysetsMods>();
                    var bottom = new HashSet<PlaysetsMods>();
                    foreach (var item in mods)
                    {
                        var existing = collectionMods.FirstOrDefault(p => p.modId == item.id.ToString());
                        if (existing == null)
                        {
                            var newModOrder = mapMod(item, 0);
                            bottom.Add(newModOrder);
                            toInsert.Add(newModOrder);
                        }
                        else
                        {
                            bottom.Add(existing);
                            toUpdate.Add(existing);
                        }
                    }
                    var ordered = collectionMods.Where(p => !bottom.Contains(p)).OrderBy(p => p.Position).ToList();
                    foreach (var item in ordered)
                    {
                        toUpdate.Add(item);
                    }
                    ordered.AddRange(bottom);
                    foreach (var item in ordered)
                    {
                        item.Position = formatPosition(pos);
                        pos++;
                    }
                    if (toUpdate.Count > 0)
                    {
                        // Composite key, need to handle differently due to limitation in the ORM.
                        foreach (var item in toUpdate)
                        {
                            await con.UpdateAsync(item, e => e.modId == item.modId && e.playsetId == item.playsetId, transaction: transaction, trace: trace);
                        }
                    }
                    if (toInsert.Count > 0)
                    {
                        await con.InsertAllAsync(toInsert, transaction: transaction, trace: trace);
                    }
                }
            }
        }

        /// <summary>
        /// synchronize mods as an asynchronous operation.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="parameters">The parameters.</param>
        private async Task SyncModsAsync(Playsets collection, ModWriterParameters parameters)
        {
            var exportMods = new List<IMod>();
            var enabledMods = new List<IMod>();
            if (parameters.EnabledMods != null)
            {
                exportMods.AddRange(parameters.EnabledMods);
                enabledMods.AddRange(parameters.EnabledMods);
            }
            if (parameters.OtherMods != null)
            {
                exportMods.AddRange(parameters.OtherMods);
            }
            if (parameters.TopPriorityMods != null)
            {
                exportMods.AddRange(parameters.TopPriorityMods);
                enabledMods.AddRange(parameters.TopPriorityMods);
            }

            using var con = GetConnection(parameters);
            var mods = await con.QueryAllAsync<Models.Paradox.v2.Mods>(trace: trace);
            using var transaction = con.BeginTransaction();
            try
            {
                var allMods = await PrepareModsTransactionAsync(con, transaction, exportMods, mods, !parameters.AppendOnly);
                await PreparePlaysetModsTransactionAsync(con, transaction, collection, allMods.Where(p => enabledMods.Any(s => s.DescriptorFile.Equals(p.GameRegistryId))), !parameters.AppendOnly);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                transaction.Rollback();
                throw;
            }
        }
    }

    #endregion Methods
}
