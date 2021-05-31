// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 05-31-2021
// ***********************************************************************
// <copyright file="GameIndexer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Game;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

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
        /// The extension
        /// </summary>
        private const string Extension = ".irony";

        /// <summary>
        /// The version file
        /// </summary>
        private const string GameVersionFile = "game-version.txt";

        /// <summary>
        /// The storage sub folder
        /// </summary>
        private const string StorageSubFolder = "IndexCache";

        #endregion Fields

        #region Methods

        /// <summary>
        /// cached definitions same as an asynchronous operation.
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
        /// definition exists as an asynchronous operation.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> DefinitionExistsAsync(string storagePath, IGame game, string version)
        {
            storagePath = ResolveStoragePath(storagePath);
            var fullPath = Path.Combine(storagePath, game.Type, GameVersionFile);
            if (File.Exists(fullPath))
            {
                var storedVersion = (await File.ReadAllTextAsync(fullPath) ?? string.Empty).ReplaceNewLine().Trim();
                return storedVersion.Equals(version ?? string.Empty);
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
            storagePath = ResolveStoragePath(storagePath);
            path = SanitizePath(path);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            if (File.Exists(fullPath))
            {
                var bytes = await File.ReadAllBytesAsync(fullPath);
                if (bytes.Any())
                {
                    using var source = new MemoryStream(bytes);
                    using var destination = new MemoryStream();
                    using var compress = new DeflateStream(source, CompressionMode.Decompress);
                    await compress.CopyToAsync(destination);
                    var text = Encoding.UTF8.GetString(destination.ToArray());
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        var result = JsonDISerializer.Deserialize<List<IDefinition>>(text);
                        return result;
                    }
                }
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
        /// <exception cref="ArgumentException">Definitions types differ.</exception>
        public virtual async Task<bool> SaveDefinitionsAsync(string storagePath, IGame game, IEnumerable<IDefinition> definitions)
        {
            storagePath = ResolveStoragePath(storagePath);
            if (definitions.GroupBy(p => p.ParentDirectory).Count() > 1)
            {
                throw new ArgumentException("Definitions types differ.");
            }
            if (definitions == null || !definitions.Any())
            {
                return false;
            }
            var path = SanitizePath(definitions.FirstOrDefault().ParentDirectory);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            if (File.Exists(fullPath))
            {
                DiskOperations.DeleteFile(fullPath);
            }
            var json = JsonDISerializer.Serialize(definitions.ToList());
            var bytes = Encoding.UTF8.GetBytes(json);
            using var source = new MemoryStream(bytes);
            using var destination = new MemoryStream();
            using var compress = new DeflateStream(destination, CompressionLevel.Fastest, true);
            await source.CopyToAsync(compress);
            await compress.FlushAsync();
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            await File.WriteAllBytesAsync(fullPath, destination.ToArray());
            return true;
        }

        /// <summary>
        /// write version as an asynchronous operation.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="gameVersion">The game version.</param>
        /// <param name="cacheVersion">The cache version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> WriteVersionAsync(string storagePath, IGame game, string gameVersion, int cacheVersion)
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
            await File.WriteAllTextAsync(gameVersionFullPath, gameVersion);
            await File.WriteAllTextAsync(cacheVersionFullPath, cacheVersion.ToString());
            return false;
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
