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
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common.Game;
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
        #region Methods

        /// <summary>
        /// Gets the definitions asynchronous.
        /// </summary>
        /// <param name="storagePath">The storage path.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IDefinition&gt;&gt;.</returns>
        public async Task<IEnumerable<IDefinition>> GetDefinitionsAsync(string storagePath, string path)
        {
            path = SanitizePath(path);
            var fullPath = System.IO.Path.Combine(storagePath, path);
            if (System.IO.File.Exists(fullPath))
            {
                var text = await System.IO.File.ReadAllTextAsync(fullPath);
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
        /// <param name="definitions">The definitions.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        /// <exception cref="ArgumentException">Definitions type differ.</exception>
        public async Task<bool> SaveDefinitionsAsync(string storagePath, IEnumerable<IDefinition> definitions)
        {
            if (definitions.GroupBy(p => p.DiskFileCI).Count() != definitions.Count())
            {
                throw new ArgumentException("Definitions type differ.");
            }
            if (definitions == null || definitions.Count() == 0)
            {
                return false;
            }
            var path = SanitizePath(definitions.FirstOrDefault().ParentDirectory);
            var fullPath = System.IO.Path.Combine(storagePath, path);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            var serialized = JsonDISerializer.Serialize(definitions.ToList());
            await System.IO.File.WriteAllTextAsync(fullPath, serialized);
            return true;
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
