// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="IModReportExporter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Interface IModReportExporter
    /// </summary>
    public interface IModReportExporter
    {
        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="modHashes">The mod hashes.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ExportAsync(IEnumerable<IHashReport> modHashes, string path);

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IHashReport&gt;&gt;.</returns>
        Task<IEnumerable<IHashReport>> ImportAsync(string path);

        #endregion Methods
    }
}
