
// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 11-26-2023
// ***********************************************************************
// <copyright file="GameIndexer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ionic.Zip;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Game;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Configuration;
using IronyModManager.Shared.Models;
using LiteDB;

namespace IronyModManager.IO.Game
{

    /// <summary>
    /// Class GameIndexer.
    /// Implements the <see cref="IronyModManager.IO.Common.Game.IGameIndexer" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Game.IGameIndexer" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class GameIndexer : IGameIndexer
    {
        #region Fields

        /// <summary>
        /// The cache version file
        /// </summary>
        private const string CacheVersionFile = "cache-version.txt";

        /// <summary>
        /// The compressed mode
        /// </summary>
        private const string CompressedMode = "zip";

        /// <summary>
        /// The database extension
        /// </summary>
        private const string DBExtension = ".db";

        /// <summary>
        /// The extension
        /// </summary>
        private const string Extension = ".irony";

        /// <summary>
        /// The version file
        /// </summary>
        private const string GameVersionFile = "game-version.txt";

        /// <summary>
        /// The mode extension
        /// </summary>
        private const string ModeExtension = ".mode";
        /// <summary>
        /// The storage sub folder
        /// </summary>
        private const string StorageSubFolder = "IndexCache";

        /// <summary>
        /// The table name
        /// </summary>
        private const string TableName = "definitions";

        /// <summary>
        /// The uncompressed mode
        /// </summary>
        private const string UncompressedMode = "flat";
        /// <summary>
        /// The domain configuration
        /// </summary>
        private readonly IDomainConfiguration domainConfiguration;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameIndexer" /> class.
        /// </summary>
        /// <param name="domainConfiguration">The domain configuration.</param>
        public GameIndexer(IDomainConfiguration domainConfiguration)
        {
            this.domainConfiguration = domainConfiguration;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Checks whether the cached definitions version signatures are the same.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> CachedDefinitionsSameAsync(string storagePath, IGame game, int version)
        {
            storagePath = ResolveStoragePath(storagePath);
            var fullPath = Path.Combine(storagePath, game.Type, CacheVersionFile);
            if (File.Exists(fullPath))
            {
                var text = (await File.ReadAllTextAsync(fullPath) ?? string.Empty).ReplaceNewLine().Trim();
                if (int.TryParse(text, out var storedVersion))
                {
                    return storedVersion.Equals(version);
                }
            }
            return false;
        }

        /// <summary>
        /// Clears the definition asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ClearDefinitionAsync(string storagePath, IGame game)
        {
            storagePath = ResolveStoragePath(storagePath);
            var fullPath = Path.Combine(storagePath, game.Type);
            if (Directory.Exists(fullPath))
            {
                DiskOperations.DeleteDirectory(fullPath, true);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Checks whether the folder is cached.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> FolderCachedAsync(string storagePath, IGame game, string path)
        {
            storagePath = ResolveStoragePath(storagePath);
            path = SanitizePath(path);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            return Task.FromResult(File.Exists(fullPath));
        }

        /// <summary>
        /// Check whether the cached game info is the same.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="versions">The versions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> GameVersionsSameAsync(string storagePath, IGame game, IEnumerable<string> versions)
        {
            storagePath = ResolveStoragePath(storagePath);
            var fullPath = Path.Combine(storagePath, game.Type, GameVersionFile);
            if (File.Exists(fullPath))
            {
                var storedVersions = (await File.ReadAllTextAsync(fullPath) ?? string.Empty).SplitOnNewLine().ToList();
                if (storedVersions.Count == versions.Count())
                {
                    return storedVersions.SequenceEqual(versions);
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the definitions asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        public virtual async Task<IEnumerable<IDefinition>> GetDefinitionsAsync(string storagePath, IGame game, string path)
        {
            static bool isCompressed(string path)
            {
                var modePath = path + ModeExtension;
                if (!File.Exists(modePath))
                {
                    // New so if no file means compressed
                    return true;
                }
                var mode = File.ReadAllText(modePath);
                return !string.IsNullOrWhiteSpace(mode) && mode.Equals(CompressedMode, StringComparison.OrdinalIgnoreCase);
            }

            storagePath = ResolveStoragePath(storagePath);
            path = SanitizePath(path);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            if (File.Exists(fullPath))
            {
                Stream dbStream;
                ZipFile zip = null;

                // Test whether compressed
                if (isCompressed(fullPath))
                {
                    zip = ZipFile.Read(fullPath);
                    var entry = zip.Entries.FirstOrDefault(p => !p.IsDirectory);
                    var ms = new MemoryStream();
                    entry.Extract(ms);
                    dbStream = ms;
                }
                else
                {
                    dbStream = File.OpenRead(fullPath);
                }
                var db = GetDatabase(dbStream);
                var col = db.GetCollection<IDefinition>(TableName);
                var result = col.FindAll().ToList() as IEnumerable<IDefinition>;
                db.Dispose();
                zip?.Dispose();
                dbStream.Close();
                await dbStream.DisposeAsync();
                return result;
            }
            return null;
        }

        /// <summary>
        /// Saves the definitions asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="System.ArgumentException">Definitions types differ.</exception>
        public virtual Task<bool> SaveDefinitionsAsync(string storagePath, IGame game, IEnumerable<IDefinition> definitions)
        {
            storagePath = ResolveStoragePath(storagePath);
            if (definitions.GroupBy(p => p.ParentDirectory).Count() > 1)
            {
                throw new ArgumentException("Definitions types differ.");
            }
            if (definitions == null || !definitions.Any())
            {
                return Task.FromResult(false);
            }
            var path = SanitizePath(definitions.FirstOrDefault().ParentDirectory);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            var dbPath = fullPath + DBExtension;
            var modePath = fullPath + ModeExtension;
            if (File.Exists(fullPath))
            {
                DiskOperations.DeleteFile(fullPath);
            }
            if (File.Exists(dbPath))
            {
                DiskOperations.DeleteFile(dbPath);
            }
            if (File.Exists(modePath))
            {
                DiskOperations.DeleteFile(modePath);
            }
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            var db = GetDatabase(dbPath);
            var col = db.GetCollection<IDefinition>(TableName);
            var inserted = col.InsertBulk(definitions);
            db.Dispose();

            var opts = domainConfiguration.GetOptions();
            if (opts.ConflictSolver.CompressIndexedDefinitions)
            {
                // Now compress the table to reduce space
                var zip = new ZipFile();
                zip.AddFile(dbPath, Shared.Constants.EmptyParam);
                zip.Save(fullPath);
                zip.Dispose();
                if (File.Exists(dbPath))
                {
                    DiskOperations.DeleteFile(dbPath);
                }
                File.WriteAllText(modePath, CompressedMode);
            }
            else
            {
                File.Move(dbPath, fullPath, true);
                File.WriteAllText(modePath, UncompressedMode);
            }
            return Task.FromResult(inserted > 0);
        }

        /// <summary>
        /// write version as an asynchronous operation.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="gameVersion">The game version.</param>
        /// <param name="cacheVersion">The cache version.</param>
        /// <returns>System.Threading.Tasks.Task&lt;bool&gt;.</returns>
        public virtual async Task<bool> WriteVersionAsync(string storagePath, IGame game, IEnumerable<string> gameVersion, int cacheVersion)
        {
            storagePath = ResolveStoragePath(storagePath);
            var gameVersionFullPath = Path.Combine(storagePath, game.Type, GameVersionFile);
            var cacheVersionFullPath = Path.Combine(storagePath, game.Type, CacheVersionFile);
            if (!Directory.Exists(Path.GetDirectoryName(gameVersionFullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(gameVersionFullPath));
            }
            if (File.Exists(gameVersionFullPath))
            {
                DiskOperations.DeleteFile(gameVersionFullPath);
            }
            if (File.Exists(cacheVersionFullPath))
            {
                DiskOperations.DeleteFile(cacheVersionFullPath);
            }
            await File.WriteAllTextAsync(gameVersionFullPath, string.Join(Environment.NewLine, gameVersion));
            await File.WriteAllTextAsync(cacheVersionFullPath, cacheVersion.ToString());
            return false;
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>LiteDatabase.</returns>
        protected virtual LiteDatabase GetDatabase(string path)
        {
            return new LiteDatabase(path, new DefinitionBsonMapper());
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>LiteDatabase.</returns>
        protected virtual LiteDatabase GetDatabase(Stream stream)
        {
            return new LiteDatabase(stream, new DefinitionBsonMapper());
        }

        /// <summary>
        /// Resolves the storage path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        protected virtual string ResolveStoragePath(string path)
        {
            return Path.Combine(path, StorageSubFolder);
        }

        /// <summary>
        /// Sanitizes the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        protected virtual string SanitizePath(string path)
        {
            return Path.Combine(path.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0], path.StandardizeDirectorySeparator().Replace(Path.DirectorySeparatorChar, '.') + Extension);
        }

        #endregion Methods
    }
}
