// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-11-2020
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
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
    [ExcludeFromCoverage("Skipping testing SQL logic.")]
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

        /// <summary>
        /// The export beta
        /// </summary>
        private readonly bool exportBeta;

        /// <summary>
        /// The export lock
        /// </summary>
        private static readonly Nito.AsyncEx.AsyncLock exportLock = new();

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteExporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exportBeta">if set to <c>true</c> [export beta].</param>
        public SQLiteExporter(ILogger logger, bool exportBeta)
        {
            this.logger = logger;
            trace = new SQLTraceLog(logger);
            this.exportBeta = exportBeta;
        }

        #region Methods

        /// <summary>
        /// export as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ExportAsync(ModWriterParameters parameters)
        {
            var mutex = await exportLock.LockAsync();
            // Caching sucks in this ORM
            TypeMapCache.Flush();
            EnsureDbExists(parameters);
            var isV3 = await IsV3Async(parameters);
            if (isV3)
            {
                var collection = await RecreateCollectionV3Async(parameters);
                await SyncModsV3Async(collection, parameters);
            }
            else
            {
                var collection = await RecreateCollectionV2Async(parameters);
                await SyncModsV2Async(collection, parameters);
            }
            mutex.Dispose();
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
            return Path.Combine(parameters.RootDirectory, !exportBeta ? Constants.Sql_db_path : Constants.Sql_db_beta_path);
        }

        /// <summary>
        /// Is v3 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> IsV3Async(ModWriterParameters parameters)
        {
            try
            {
                using var con = GetConnection(parameters);
                var changes = await con.QueryAllAsync<Models.Paradox.v2.KnoxMigrations>();
                return changes != null && changes.Any(c => c.Name.Equals(Constants.SqlV3Id, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// recreate collection as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Playsets.</returns>
        private async Task<Models.Paradox.v2.Playsets> RecreateCollectionV2Async(ModWriterParameters parameters)
        {
            Models.Paradox.v2.Playsets getDefaultIronyCollection()
            {
                return new Models.Paradox.v2.Playsets()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    LoadOrder = CollectionSortType,
                    Name = GetCollectionName()
                };
            }

            var colName = GetCollectionName();

            // They did do a cascade delete right?
            using var con = GetConnection(parameters);
            using var transaction = con.BeginTransaction();

            var activeCollections = (await con.QueryAsync<Models.Paradox.v2.Playsets>(p => p.IsActive == true, trace: trace)).ToList().Where(p => !p.Name.Equals(colName)).ToList();
            try
            {
                Models.Paradox.v2.Playsets ironyCollection;
                if (!parameters.AppendOnly)
                {
                    await con.DeleteAsync<Models.Paradox.v2.Playsets>(p => p.Name == colName, transaction: transaction, trace: trace);

                    if (activeCollections.Count > 0)
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
                    ironyCollection = (await con.QueryAsync<Models.Paradox.v2.Playsets>(p => p.Name == colName, trace: trace)).FirstOrDefault();

                    if (activeCollections.Count > 0)
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
        /// Recreate collection v3 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;Models.Paradox.v3.Playsets&gt; representing the asynchronous operation.</returns>
        private async Task<Models.Paradox.v3.Playsets> RecreateCollectionV3Async(ModWriterParameters parameters)
        {
            Models.Paradox.v3.Playsets getDefaultIronyCollection()
            {
                return new Models.Paradox.v3.Playsets()
                {
                    Id = Guid.NewGuid().ToString(),
                    IsActive = true,
                    LoadOrder = CollectionSortType,
                    Name = GetCollectionName(),
                    CreatedOn = DateTime.UtcNow
                };
            }

            var colName = GetCollectionName();

            // They did do a cascade delete right?
            using var con = GetConnection(parameters);
            using var transaction = con.BeginTransaction();

            var activeCollections = (await con.QueryAsync<Models.Paradox.v3.Playsets>(p => p.IsActive == true, trace: trace)).ToList().Where(p => !p.Name.Equals(colName)).ToList();
            try
            {
                Models.Paradox.v3.Playsets ironyCollection;
                if (!parameters.AppendOnly)
                {
                    await con.DeleteAsync<Models.Paradox.v3.Playsets>(p => p.Name == colName, transaction: transaction, trace: trace);

                    if (activeCollections.Count > 0)
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
                    ironyCollection = (await con.QueryAsync<Models.Paradox.v3.Playsets>(p => p.Name == colName, trace: trace)).FirstOrDefault();

                    if (activeCollections.Count > 0)
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
        private Models.Paradox.v2.Mods MapPdxModV2(Models.Paradox.v2.Mods pdxMod, IMod mod)
        {
            if (pdxMod == null)
            {
                pdxMod = new Models.Paradox.v2.Mods()
                {
                    Id = Guid.NewGuid().ToString()
                };
            }
            MapModData(pdxMod, mod);
            return pdxMod;
        }

        /// <summary>
        /// Maps the PDX mod v3.
        /// </summary>
        /// <param name="pdxMod">The PDX mod.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>Models.Paradox.v3.Mods.</returns>
        private Models.Paradox.v3.Mods MapPdxModV3(Models.Paradox.v3.Mods pdxMod, IMod mod)
        {
            if (pdxMod == null)
            {
                pdxMod = new Models.Paradox.v3.Mods()
                {
                    Id = Guid.NewGuid().ToString()
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
        private async Task<IEnumerable<Models.Paradox.v2.Mods>> PrepareModsTransactionV2Async(IDbConnection con, IDbTransaction transaction, List<IMod> exportMods, IEnumerable<Models.Paradox.v2.Mods> mods, bool removeInvalid)
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
                        var newPdxMod = MapPdxModV2(null, item);
                        toInsert.Add(newPdxMod);
                        result.Add(newPdxMod);
                    }
                    else
                    {
                        // Pending delete, insert a new entry instead
                        if (toRemove.Contains(pdxMod))
                        {
                            var newPdxMod = MapPdxModV2(null, item);
                            toInsert.Add(newPdxMod);
                            result.Add(newPdxMod);
                        }
                        else
                        {
                            var updatedPdxMod = MapPdxModV2(pdxMod, item);
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
                    var pdxMod = MapPdxModV2(null, mod);
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
        /// Prepare mods transaction v3 as an asynchronous operation.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="exportMods">The export mods.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="removeInvalid">if set to <c>true</c> [remove invalid].</param>
        /// <returns>A Task&lt;IEnumerable`1&gt; representing the asynchronous operation.</returns>
        private async Task<IEnumerable<Models.Paradox.v3.Mods>> PrepareModsTransactionV3Async(IDbConnection con, IDbTransaction transaction, List<IMod> exportMods, IEnumerable<Models.Paradox.v3.Mods> mods, bool removeInvalid)
        {
            var result = new HashSet<Models.Paradox.v3.Mods>();
            if (mods?.Count() > 0)
            {
                var toRemove = new HashSet<Models.Paradox.v3.Mods>();
                var toInsert = new HashSet<Models.Paradox.v3.Mods>();
                var toUpdate = new HashSet<Models.Paradox.v3.Mods>();
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
                        var newPdxMod = MapPdxModV3(null, item);
                        toInsert.Add(newPdxMod);
                        result.Add(newPdxMod);
                    }
                    else
                    {
                        // Pending delete, insert a new entry instead
                        if (toRemove.Contains(pdxMod))
                        {
                            var newPdxMod = MapPdxModV3(null, item);
                            toInsert.Add(newPdxMod);
                            result.Add(newPdxMod);
                        }
                        else
                        {
                            var updatedPdxMod = MapPdxModV3(pdxMod, item);
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
                var toInsert = new HashSet<Models.Paradox.v3.Mods>();
                foreach (var mod in exportMods)
                {
                    var pdxMod = MapPdxModV3(null, mod);
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
        private async Task PreparePlaysetModsTransactionV2Async(IDbConnection con, IDbTransaction transaction, Models.Paradox.v2.Playsets collection, IEnumerable<Models.Paradox.v2.Mods> mods, bool recreateCollection)
        {
            Models.Paradox.v2.PlaysetsMods mapMod(Models.Paradox.v2.Mods mod, int position)
            {
                return new Models.Paradox.v2.PlaysetsMods()
                {
                    Enabled = true,
                    ModId = mod.Id,
                    PlaysetId = collection.Id,
                    Position = formatPosition(position)
                };
            }

            static string formatPosition(int position)
            {
                // Why not simply use numbers? What actually makes sense!!! Therefore generate a 10 digit int with leading zeros... or in other words a proper hex number.
                return position.ToString("x10");
            }

            if (mods?.Count() > 0)
            {
                var collectionMods = await con.QueryAsync<Models.Paradox.v2.PlaysetsMods>(p => p.PlaysetId == collection.Id, trace: trace);
                // Because it's readable for me in hex
                int pos = 4096;
                if (recreateCollection || collectionMods == null || !collectionMods.Any())
                {
                    if (recreateCollection)
                    {
                        await con.DeleteAsync<Models.Paradox.v2.PlaysetsMods>(p => p.PlaysetId == collection.Id, transaction: transaction, trace: trace);
                    }

                    var toInsert = new List<Models.Paradox.v2.PlaysetsMods>();
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
                    var toInsert = new HashSet<Models.Paradox.v2.PlaysetsMods>();
                    var toUpdate = new HashSet<Models.Paradox.v2.PlaysetsMods>();
                    var bottom = new HashSet<Models.Paradox.v2.PlaysetsMods>();
                    foreach (var item in mods)
                    {
                        var existing = collectionMods.FirstOrDefault(p => p.ModId == item.Id);
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
                            await con.UpdateAsync(item, e => e.ModId == item.ModId && e.PlaysetId == item.PlaysetId, transaction: transaction, trace: trace);
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
        /// Prepare playset mods transaction v3 as an asynchronous operation.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="mods">The mods.</param>
        /// <param name="recreateCollection">if set to <c>true</c> [recreate collection].</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task PreparePlaysetModsTransactionV3Async(IDbConnection con, IDbTransaction transaction, Models.Paradox.v3.Playsets collection, IEnumerable<Models.Paradox.v3.Mods> mods, bool recreateCollection)
        {
            Models.Paradox.v3.PlaysetsMods mapMod(Models.Paradox.v3.Mods mod, int position)
            {
                return new Models.Paradox.v3.PlaysetsMods()
                {
                    Enabled = true,
                    ModId = mod.Id,
                    PlaysetId = collection.Id,
                    Position = position
                };
            }

            if (mods?.Count() > 0)
            {
                var collectionMods = await con.QueryAsync<Models.Paradox.v3.PlaysetsMods>(p => p.PlaysetId == collection.Id, trace: trace);
                int pos = 0;
                if (recreateCollection || collectionMods == null || !collectionMods.Any())
                {
                    if (recreateCollection)
                    {
                        await con.DeleteAsync<Models.Paradox.v3.PlaysetsMods>(p => p.PlaysetId == collection.Id, transaction: transaction, trace: trace);
                    }

                    var toInsert = new List<Models.Paradox.v3.PlaysetsMods>();
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
                    var toInsert = new HashSet<Models.Paradox.v3.PlaysetsMods>();
                    var toUpdate = new HashSet<Models.Paradox.v3.PlaysetsMods>();
                    var bottom = new HashSet<Models.Paradox.v3.PlaysetsMods>();
                    foreach (var item in mods)
                    {
                        var existing = collectionMods.FirstOrDefault(p => p.ModId == item.Id);
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
                        item.Position = pos;
                        pos++;
                    }
                    if (toUpdate.Count > 0)
                    {
                        // Composite key, need to handle differently due to limitation in the ORM.
                        foreach (var item in toUpdate)
                        {
                            await con.UpdateAsync(item, e => e.ModId == item.ModId && e.PlaysetId == item.PlaysetId, transaction: transaction, trace: trace);
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
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SyncModsV2Async(Models.Paradox.v2.Playsets collection, ModWriterParameters parameters)
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
                var allMods = await PrepareModsTransactionV2Async(con, transaction, exportMods, mods, !parameters.AppendOnly);
                await PreparePlaysetModsTransactionV2Async(con, transaction, collection, allMods.Where(p => enabledMods.Any(s => s.DescriptorFile.Equals(p.GameRegistryId))), !parameters.AppendOnly);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Synchronize mods v3 as an asynchronous operation.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SyncModsV3Async(Models.Paradox.v3.Playsets collection, ModWriterParameters parameters)
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
            var mods = await con.QueryAllAsync<Models.Paradox.v3.Mods>(trace: trace);
            using var transaction = con.BeginTransaction();
            try
            {
                var allMods = await PrepareModsTransactionV3Async(con, transaction, exportMods, mods, !parameters.AppendOnly);
                await PreparePlaysetModsTransactionV3Async(con, transaction, collection, allMods.Where(p => enabledMods.Any(s => s.DescriptorFile.Equals(p.GameRegistryId))), !parameters.AppendOnly);
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
