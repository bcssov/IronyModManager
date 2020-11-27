// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-26-2020
//
// Last Modified By : Mario
// Last Modified On : 11-27-2020
// ***********************************************************************
// <copyright file="IModMergeCompressExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IModMergeCompressExporter
    /// </summary>
    public interface IModMergeCompressExporter
    {
        #region Events

        /// <summary>
        /// Occurs when [processed file].
        /// </summary>
        public event EventHandler ProcessedFile;

        #endregion Events

        #region Methods

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        void AddFile(ModMergeCompressExporterParameters parameters);

        /// <summary>
        /// Finalizes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="exportPath">The export path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Finalize(long id, string exportPath);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>System.Int64.</returns>
        long Start();

        #endregion Methods
    }
}
