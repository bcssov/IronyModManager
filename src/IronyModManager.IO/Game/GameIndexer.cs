// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 05-27-2021
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
        /// The version file
        /// </summary>
        private const string VersionFile = "version.txt";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Clears the definition asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual Task<bool> ClearDefinitionAsync(string storagePath, IGame game)
        {
            var fullPath = Path.Combine(storagePath, game.Type);
            if (Directory.Exists(fullPath))
            {
                DiskOperations.DeleteDirectory(fullPath, false);
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
            var fullPath = Path.Combine(storagePath, game.Type, VersionFile);
            if (File.Exists(fullPath))
            {
                var storedVersion = (await File.ReadAllTextAsync(fullPath)).ReplaceNewLine().Trim();
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
            path = SanitizePath(path);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            if (File.Exists(fullPath))
            {
                var text = await File.ReadAllTextAsync(fullPath);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var result = JsonDISerializer.Deserialize<List<IDefinition>>(text);
                    return result;
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
        /// <exception cref="ArgumentException">Definitions type differ.</exception>
        public virtual async Task<bool> SaveDefinitionsAsync(string storagePath, IGame game, IEnumerable<IDefinition> definitions)
        {
            if (definitions.GroupBy(p => p.DiskFileCI).Count() != definitions.Count())
            {
                throw new ArgumentException("Definitions type differ.");
            }
            if (definitions == null || !definitions.Any())
            {
                return false;
            }
            var path = SanitizePath(definitions.FirstOrDefault().ParentDirectory);
            var fullPath = Path.Combine(storagePath, game.Type, path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            var serialized = JsonDISerializer.Serialize(definitions.ToList());
            await File.WriteAllTextAsync(fullPath, serialized);
            return true;
        }

        /// <summary>
        /// write version as an asynchronous operation.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="game">The game.</param>
        /// <param name="version">The version.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public virtual async Task<bool> WriteVersionAsync(string storagePath, IGame game, string version)
        {
            var fullPath = Path.Combine(storagePath, game.Type, VersionFile);
            if (File.Exists(fullPath))
            {
                DiskOperations.DeleteFile(fullPath);
                await File.WriteAllTextAsync(fullPath, version);
            }
            return false;
        }

        /// <summary>
        /// Sanitizes the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        protected virtual string SanitizePath(string path)
        {
            return path.Replace("\\", ".").Replace("/", ".").GenerateValidFileName();
        }

        #endregion Methods
    }
}
