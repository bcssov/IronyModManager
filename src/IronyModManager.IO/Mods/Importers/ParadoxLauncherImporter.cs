// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-12-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="ParadoxLauncherImporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Mods.Models.Paradox.v2;
using IronyModManager.Shared;
using Microsoft.Data.Sqlite;
using RepoDb;

namespace IronyModManager.IO.Mods.Importers
{
    /// <summary>
    /// Class ParadoxLauncherImporter.
    /// </summary>
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
        /// import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public async Task<bool> ImportAsync(ModCollectionExporterParams parameters)
        {
            try
            {
                using var con = GetConnection(parameters);
                var activeCollection = (await con.QueryAsync<Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<PlaysetsMods>(p => p.playsetId == activeCollection.id.ToString(), trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v2.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled.GetValueOrDefault()).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => collectionMods.Any(m => m.modId.Equals(p.id.ToString()))).OrderBy(p => ordered.FindIndex(o => o.modId == p.id.ToString()));
                        if (validMods.Count() > 0)
                        {
                            parameters.Mod.Name = activeCollection.Name;
                            parameters.Mod.Mods = validMods.Select(p => p.GameRegistryId).ToList();
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
