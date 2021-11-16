// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-12-2020
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
// ***********************************************************************
// <copyright file="ParadoxLauncherImporter.cs" company="Mario">
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
using IronyModManager.DI;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Shared;
using Microsoft.Data.Sqlite;
using RepoDb;

namespace IronyModManager.IO.Mods.Importers
{
    /// <summary>
    /// Class ParadoxLauncherImporter.
    /// </summary>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ParadoxLauncherImporter
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The trace
        /// </summary>
        private readonly SQLTraceLog trace;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxLauncherImporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ParadoxLauncherImporter(ILogger logger)
        {
            this.logger = logger;
            trace = new SQLTraceLog(logger);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// database import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<ICollectionImportResult> DatabaseImportAsync(ModCollectionExporterParams parameters)
        {
            if (await IsV3Async(parameters))
            {
                return await DatabaseImportv3Async(parameters);
            }
            return await DatabaseImportv2Async(parameters);
        }

        /// <summary>
        /// json import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.AggregateException"></exception>
        public virtual async Task<ICollectionImportResult> JsonImportAsync(ModCollectionExporterParams parameters)
        {
            async Task<(Exception, ICollectionImportResult)> parseV2()
            {
                var content = await File.ReadAllTextAsync(parameters.File);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    Models.Paradox.Json.v2.ModInfo model = null;
                    try
                    {
                        model = JsonDISerializer.Deserialize<Models.Paradox.Json.v2.ModInfo>(content);
                    }
                    catch (Exception ex)
                    {
                        return (ex, null);
                    }
                    if (!string.IsNullOrWhiteSpace(model.Game) && !string.IsNullOrWhiteSpace(model.Name))
                    {
                        // Validate whether this really is v2 (execting length larger than 4 as a dumb best guess)
                        if (model.Mods.Any(p => p.Position.Length >= 4))
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = model.Name;
                            // Will need to lookup the game and mod ids in the mod service
                            result.Game = model.Game;
                            var mods = model.Mods.Where(p => p.Enabled).OrderBy(p => p.Position);
                            result.ModIds = mods.Select(p => !string.IsNullOrWhiteSpace(p.PdxId) ? p.PdxId : p.SteamId).ToList();
                            return (null, result);
                        }
                    }
                }
                return (null, null);
            }
            async Task<(Exception, ICollectionImportResult)> parseV3()
            {
                var content = await File.ReadAllTextAsync(parameters.File);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    Models.Paradox.Json.v3.ModInfo model = null;
                    try
                    {
                        model = JsonDISerializer.Deserialize<Models.Paradox.Json.v3.ModInfo>(content);
                    }
                    catch (Exception ex)
                    {
                        return (ex, null);
                    }
                    if (!string.IsNullOrWhiteSpace(model.Game) && !string.IsNullOrWhiteSpace(model.Name))
                    {
                        var result = DIResolver.Get<ICollectionImportResult>();
                        result.Name = model.Name;
                        // Will need to lookup the game and mod ids in the mod service
                        result.Game = model.Game;
                        var mods = model.Mods.Where(p => p.Enabled).OrderBy(p => p.Position);
                        result.ModIds = mods.Select(p => !string.IsNullOrWhiteSpace(p.PdxId) ? p.PdxId : p.SteamId).ToList();
                        return (null, result);
                    }
                }
                return (null, null);
            }

            if (File.Exists(parameters.File))
            {
                var exceptions = new List<Exception>();
                var result = await parseV2();
                if (result.Item2 != null)
                {
                    return result.Item2;
                }
                if (result.Item1 != null)
                {
                    exceptions.Add(result.Item1);
                }
                result = await parseV3();
                if (result.Item2 != null)
                {
                    return result.Item2;
                }
                if (result.Item1 != null)
                {
                    exceptions.Add(result.Item1);
                }
                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }
            }
            return null;
        }

        /// <summary>
        /// Database importv2 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;ICollectionImportResult&gt; representing the asynchronous operation.</returns>
        protected virtual async Task<ICollectionImportResult> DatabaseImportv2Async(ModCollectionExporterParams parameters)
        {
            try
            {
                using var con = GetConnection(parameters);
                var activeCollection = (await con.QueryAsync<Models.Paradox.v2.Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<Models.Paradox.v2.PlaysetsMods>(p => p.PlaysetId == activeCollection.Id.ToString(), trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v2.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => ordered.Any(m => m.ModId.Equals(p.Id))).OrderBy(p => ordered.FindIndex(o => o.ModId == p.Id));
                        if (validMods.Any())
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = activeCollection.Name;
                            result.Descriptors = validMods.Select(p => p.GameRegistryId).ToList();
                            result.ModNames = validMods.Select(p => p.DisplayName).ToList();
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Database importv3 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;ICollectionImportResult&gt; representing the asynchronous operation.</returns>
        protected virtual async Task<ICollectionImportResult> DatabaseImportv3Async(ModCollectionExporterParams parameters)
        {
            try
            {
                using var con = GetConnection(parameters);
                var activeCollection = (await con.QueryAsync<Models.Paradox.v3.Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<Models.Paradox.v3.PlaysetsMods>(p => p.PlaysetId == activeCollection.Id.ToString(), trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v3.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => ordered.Any(m => m.ModId.Equals(p.Id))).OrderBy(p => ordered.FindIndex(o => o.ModId == p.Id));
                        if (validMods.Any())
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = activeCollection.Name;
                            result.Descriptors = validMods.Select(p => p.GameRegistryId).ToList();
                            result.ModNames = validMods.Select(p => p.DisplayName).ToList();
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetDbPath(ModCollectionExporterParams parameters)
        {
            return Path.Combine(Path.GetDirectoryName(parameters.ModDirectory), Constants.Sql_db_path);
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IDbConnection.</returns>
        private IDbConnection GetConnection(ModCollectionExporterParams parameters)
        {
            return new SqliteConnection($"Data Source=\"{GetDbPath(parameters)}\"").EnsureOpen();
        }

        /// <summary>
        /// Is v3 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<bool> IsV3Async(ModCollectionExporterParams parameters)
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

        #endregion Methods
    }
}
