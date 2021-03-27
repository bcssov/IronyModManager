// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-27-2021
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="ReportExportService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IronyModManager.IO.Common;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class ReportExportService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.IReportExportService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.IReportExportService" />
    public class ReportExportService : BaseService, IReportExportService
    {
        #region Fields

        /// <summary>
        /// The report exporter
        /// </summary>
        private readonly IReportExporter reportExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportExportService" /> class.
        /// </summary>
        /// <param name="reportExporter">The report exporter.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ReportExportService(IReportExporter reportExporter, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.reportExporter = reportExporter;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Exports the asynchronous.
        /// </summary>
        /// <param name="modHashes">The mod hashes.</param>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public Task<bool> ExportAsync(IEnumerable<IHashReport> modHashes, string path)
        {
            return reportExporter.ExportAsync(modHashes, path);
        }

        /// <summary>
        /// Gets the collection reports.
        /// </summary>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>IEnumerable&lt;IHashReport&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<IHashReport> GetCollectionReports(IReadOnlyCollection<IHashReport> hashReports)
        {
            if (hashReports != null)
            {
                return hashReports.Where(p => p.ReportType == HashReportType.Collection).ToList();
            }
            return null;
        }

        /// <summary>
        /// Gets the game reports.
        /// </summary>
        /// <param name="hashReports">The hash reports.</param>
        /// <returns>IEnumerable&lt;IHashReport&gt;.</returns>
        public IEnumerable<IHashReport> GetGameReports(IReadOnlyCollection<IHashReport> hashReports)
        {
            if (hashReports != null)
            {
                return hashReports.Where(p => p.ReportType == HashReportType.Game).ToList();
            }
            return null;
        }

        /// <summary>
        /// Imports the asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IHashReport&gt;&gt;.</returns>
        public Task<IEnumerable<IHashReport>> ImportAsync(string path)
        {
            return reportExporter.ImportAsync(path);
        }

        #endregion Methods
    }
}
