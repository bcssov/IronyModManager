// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="IModWriter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IModWriter
    /// </summary>
    public interface IModWriter
    {
        #region Methods

        /// <summary>
        /// Applies the mods asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ApplyModsAsync(ModWriterParameters parameters);

        /// <summary>
        /// Creates the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CreateModDirectoryAsync(ModWriterParameters parameters);

        /// <summary>
        /// Deletes the descriptor asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DeleteDescriptorAsync(ModWriterParameters parameters);

        /// <summary>
        /// Descriptors the exists asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> DescriptorExistsAsync(ModWriterParameters parameters);

        /// <summary>
        /// Mods the directory exists.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool ModDirectoryExists(ModWriterParameters parameters);

        /// <summary>
        /// Mods the directory exists asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ModDirectoryExistsAsync(ModWriterParameters parameters);

        /// <summary>
        /// Purges the mod directory asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="deleteAll">if set to <c>true</c> [delete all].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> PurgeModDirectoryAsync(ModWriterParameters parameters, bool deleteAll = false);

        /// <summary>
        /// Sets the descriptor lock asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> SetDescriptorLockAsync(ModWriterParameters parameters, bool isLocked);

        /// <summary>
        /// Writes the descriptor asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="writeDescriptorInModDirectory">if set to <c>true</c> [write descriptor in mod directory].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> WriteDescriptorAsync(ModWriterParameters parameters, bool writeDescriptorInModDirectory);

        /// <summary>
        /// Writes the descriptor to stream asynchronous.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> WriteDescriptorToStreamAsync(ModWriterParameters parameters, Stream stream);

        #endregion Methods
    }
}
