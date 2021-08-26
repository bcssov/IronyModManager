// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-12-2020
//
// Last Modified By : Mario
// Last Modified On : 08-26-2021
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
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.Json;
using IronyModManager.IO.Mods.Models.Paradox.v2;
using IronyModManager.Shared;
using Microsoft.Data.Sqlite;
using RepoDb;

namespace IronyModManager.IO.Mods.Importers
{
    /// <summary>
    /// Class ParadoxLauncherImporter.
    /// </summary>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ParadoxLauncherImporter
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
        public async Task<bool> DatabaseImportAsync(ModCollectionExporterParams parameters)
        {
            try
            {
                using var con = GetConnection(parameters);
                var activeCollection = (await con.QueryAsync<Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<PlaysetsMods>(p => p.PlaysetId == activeCollection.Id.ToString(), trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v2.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => ordered.Any(m => m.ModId.Equals(p.Id))).OrderBy(p => ordered.FindIndex(o => o.ModId == p.Id));
                        if (validMods.Any())
                        {
                            parameters.Mod.Name = activeCollection.Name;
                            parameters.Mod.Mods = validMods.Select(p => p.GameRegistryId).ToList();
                            parameters.Mod.ModNames = validMods.Select(p => p.DisplayName).ToList();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// json import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> JsonImportAsync(ModCollectionExporterParams parameters)
        {
            if (File.Exists(parameters.File))
            {
                var content = await File.ReadAllTextAsync(parameters.File);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    ModInfo model = null;
                    try
                    {
                        model = JsonDISerializer.Deserialize<ModInfo>(content);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        return false;
                    }
                    if (!string.IsNullOrWhiteSpace(model.Game) && !string.IsNullOrWhiteSpace(model.Name))
                    {
                        parameters.Mod.Name = model.Name;
                        // Will need to lookup the game and mod ids in the mod service
                        parameters.Mod.Game = model.Game;
                        var mods = model.Mods.Where(p => p.Enabled).OrderBy(p => p.Position);
                        parameters.Mod.ModNames = mods.Select(p => p.DisplayName).ToList();
                        return true;
                    }
                }
            }
            return false;
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
        /// Gets the database path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        private string GetDbPath(ModCollectionExporterParams parameters)
        {
            return Path.Combine(Path.GetDirectoryName(parameters.ModDirectory), Constants.Sql_db_path);
        }

        #endregion Methods
    }
}
