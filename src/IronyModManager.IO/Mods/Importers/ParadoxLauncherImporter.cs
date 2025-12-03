// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-12-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
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
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using Microsoft.Data.Sqlite;
using RepoDb;

namespace IronyModManager.IO.Mods.Importers
{
    /// <summary>
    /// The paradox launcher importer.
    /// </summary>
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ParadoxLauncherImporter(ILogger logger)
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        /// <summary>
        /// The trace
        /// </summary>
        private readonly SQLTraceLog trace = new(logger);

        #endregion Fields

        #region Enums

        /// <summary>
        /// Enum Version
        /// </summary>
        private enum Version
        {
            /// <summary>
            /// The default
            /// </summary>
            Default,

            /// <summary>
            /// The v4
            /// </summary>
            v4,

            /// <summary>
            /// The v5
            /// </summary>
            v5
        }

        #endregion Enums

        #region Methods

        /// <summary>
        /// database import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotSupportedException">JsonMetadataV2 is not supported.</exception>
        public async Task<ICollectionImportResult> DatabaseImportAsync(ModCollectionExporterParams parameters)
        {
            if (parameters.DescriptorType == DescriptorType.JsonMetadataV2)
            {
                throw new NotSupportedException("JsonMetadataV2 is not supported.");
            }

            if (!File.Exists(GetDbPath(parameters)))
            {
                return null;
            }

            // Caching sucks in this ORM
            DbFieldCache.Flush();
            FieldCache.Flush();
            IdentityCache.Flush();
            PrimaryCache.Flush();
            var version = await GetVersionAsync(parameters);
            return version switch
            {
                Version.v4 => await DatabaseImportV3Async(parameters),
                Version.v5 => await DatabaseImportV4Async(parameters),
                _ => await DatabaseImportV2Async(parameters)
            };
        }

        /// <summary>
        /// json import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotSupportedException">JsonMetadataV2 is not supported.</exception>
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
                        // Validate whether this really is v2 (expecting length larger than 4 as a dumb best guess)
                        if (model.Mods.Any(p => p.Position.Length > 4))
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = model.Name;

                            // Will need to lookup the game and mod ids in the mod service
                            result.Game = model.Game;
                            var mods = model.Mods.Where(p => p.Enabled).OrderBy(p => p.Position);
                            result.ModIds =
                            [
                                .. mods.Select(p =>
                                {
                                    var result = DIResolver.Get<IModCollectionSourceInfo>();
                                    if (long.TryParse(p.PdxId, out var pdxId))
                                    {
                                        result.ParadoxId = pdxId;
                                    }

                                    if (long.TryParse(p.SteamId, out var steamId))
                                    {
                                        result.SteamId = steamId;
                                    }

                                    return result;
                                })
                            ];
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
                        result.ModIds =
                        [
                            .. mods.Select(p =>
                            {
                                var result = DIResolver.Get<IModCollectionSourceInfo>();
                                if (long.TryParse(p.PdxId, out var pdxId))
                                {
                                    result.ParadoxId = pdxId;
                                }

                                if (long.TryParse(p.SteamId, out var steamId))
                                {
                                    result.SteamId = steamId;
                                }

                                return result;
                            })
                        ];
                        return (null, result);
                    }
                }

                return (null, null);
            }

            if (parameters.DescriptorType == DescriptorType.JsonMetadataV2)
            {
                throw new NotSupportedException("JsonMetadataV2 is not supported.");
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

                if (exceptions.Count != 0)
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
        protected virtual async Task<ICollectionImportResult> DatabaseImportV2Async(ModCollectionExporterParams parameters)
        {
            using var con = GetConnection(parameters);
            try
            {
                var activeCollection = (await con.QueryAsync<Models.Paradox.v2.Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<Models.Paradox.v2.PlaysetsMods>(p => p.PlaysetId == activeCollection.Id, trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v2.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => ordered.Any(m => m.ModId.Equals(p.Id))).OrderBy(p => ordered.FindIndex(o => o.ModId == p.Id));
                        if (validMods.Any())
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = activeCollection.Name;
                            if (parameters.DescriptorType == DescriptorType.DescriptorMod)
                            {
                                result.Descriptors = [.. validMods.Select(p => p.GameRegistryId)];
                            }
                            else
                            {
                                result.FullPaths = [.. validMods.Select(p => p.DirPath.StandardizeDirectorySeparator())];
                            }

                            result.ModNames = [.. validMods.Select(p => p.DisplayName)];
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            con.Close();
            return null;
        }

        /// <summary>
        /// Database importv3 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;ICollectionImportResult&gt; representing the asynchronous operation.</returns>
        protected virtual async Task<ICollectionImportResult> DatabaseImportV3Async(ModCollectionExporterParams parameters)
        {
            using var con = GetConnection(parameters);
            try
            {
                var activeCollection = (await con.QueryAsync<Models.Paradox.v4.Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<Models.Paradox.v4.PlaysetsMods>(p => p.PlaysetId == activeCollection.Id, trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v4.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => ordered.Any(m => m.ModId.Equals(p.Id))).OrderBy(p => ordered.FindIndex(o => o.ModId == p.Id));
                        if (validMods.Any())
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = activeCollection.Name;
                            if (parameters.DescriptorType == DescriptorType.DescriptorMod)
                            {
                                result.Descriptors = [.. validMods.Select(p => p.GameRegistryId)];
                            }
                            else
                            {
                                result.FullPaths = [.. validMods.Select(p => p.DirPath.StandardizeDirectorySeparator())];
                            }

                            result.ModNames = [.. validMods.Select(p => p.DisplayName)];
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            con.Close();
            return null;
        }

        /// <summary>
        /// Database importv4 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;ICollectionImportResult&gt; representing the asynchronous operation.</returns>
        protected virtual async Task<ICollectionImportResult> DatabaseImportV4Async(ModCollectionExporterParams parameters)
        {
            using var con = GetConnection(parameters);
            try
            {
                var activeCollection = (await con.QueryAsync<Models.Paradox.v4.Playsets>(p => p.IsActive == true, trace: trace)).FirstOrDefault();
                if (activeCollection != null)
                {
                    var collectionMods = await con.QueryAsync<Models.Paradox.v4.PlaysetsMods>(p => p.PlaysetId == activeCollection.Id, trace: trace);
                    if (collectionMods?.Count() > 0)
                    {
                        var mods = await con.QueryAllAsync<Models.Paradox.v5.Mods>(trace: trace);
                        var ordered = collectionMods.Where(p => p.Enabled).OrderBy(p => p.Position).ToList();
                        var validMods = mods.Where(p => ordered.Any(m => m.ModId.Equals(p.Id))).OrderBy(p => ordered.FindIndex(o => o.ModId == p.Id));
                        if (validMods.Any())
                        {
                            var result = DIResolver.Get<ICollectionImportResult>();
                            result.Name = activeCollection.Name;
                            if (parameters.DescriptorType == DescriptorType.DescriptorMod)
                            {
                                result.Descriptors = [.. validMods.Select(p => p.GameRegistryId)];
                            }
                            else
                            {
                                result.FullPaths = [.. validMods.Select(p => p.DirPath.StandardizeDirectorySeparator())];
                            }

                            result.ModNames = [.. validMods.Select(p => p.DisplayName)];
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            con.Close();
            return null;
        }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetDbPath(ModCollectionExporterParams parameters)
        {
            return Path.Combine(Path.GetDirectoryName(parameters.ModDirectory)!, Constants.Sql_db_path);
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
        /// Is v4 as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        private async Task<Version> GetVersionAsync(ModCollectionExporterParams parameters)
        {
            using var con = GetConnection(parameters);
            try
            {
                var changes = await con.QueryAllAsync<Models.Paradox.v2.KnoxMigrations>();
                if (changes != null)
                {
                    if (changes.Any(c => c.Name.Equals(Constants.SqlV5Idd.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        return Version.v5;
                    }

                    if (changes.Any(c => c.Name.Equals(Constants.SqlV4Id.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        return Version.v4;
                    }
                }
            }
            catch
            {
            }

            con.Close();
            return Version.Default;
        }

        #endregion Methods
    }
}
