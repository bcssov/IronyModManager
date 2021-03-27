// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-27-2021
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="IReportExportService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IReportExportService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IReportExportService : IBaseService
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
        /// Gets the collection reports.
        /// </summary>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>IEnumerable&lt;IHashReport&gt;.</returns>
        IEnumerable<IHashReport> GetCollectionReports(IReadOnlyCollection<IHashReport> hashReports);

        /// <summary>
        /// Gets the game reports.
        /// </summary>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>IEnumerable&lt;IHashReport&gt;.</returns>
        IEnumerable<IHashReport> GetGameReports(IReadOnlyCollection<IHashReport> hashReports);

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IHashReport&gt;&gt;.</returns>
        Task<IEnumerable<IHashReport>> ImportAsync(string path);

        #endregion Methods
    }
}
