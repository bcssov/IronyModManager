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
using IronyModManager.Services.Common.MessageBus;
using IronyModManager.Shared.MessageBus;
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
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        /// <summary>
        /// The report exporter
        /// </summary>
        private readonly IReportExporter reportExporter;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportExportService" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        /// <param name="reportExporter">The report exporter.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public ReportExportService(IMessageBus messageBus, IReportExporter reportExporter,
            IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.reportExporter = reportExporter;
            this.messageBus = messageBus;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Compares the reports.
        /// </summary>
        /// <param name="firstReport">The first report.</param>
        /// <param name="secondReport">The second report.</param>
        /// <returns>IReadOnlyCollection&lt;IHashReport&gt;.</returns>
        public virtual IReadOnlyCollection<IHashReport> CompareReports(IReadOnlyCollection<IHashReport> firstReport, IReadOnlyCollection<IHashReport> secondReport)
        {
            firstReport ??= new List<IHashReport>();
            secondReport ??= new List<IHashReport>();

            var total = firstReport.Sum(p => p.Reports.Count) + secondReport.Sum(p => p.Reports.Count);
            var progress = 0;
            double lastPercentage = 0;

            void compareReports(List<IHashReport> reports, IEnumerable<IHashReport> firstReports, IEnumerable<IHashReport> secondReports)
            {
                foreach (var first in firstReports)
                {
                    if (!secondReports.Any(p => p.Name.Equals(first.Name)))
                    {
                        progress += first.Reports.Count;
                        var percentage = GetProgressPercentage(total, progress);
                        if (percentage != lastPercentage)
                        {
                            messageBus.Publish(new ModReportExportEvent(2, percentage));
                        }
                        lastPercentage = percentage;
                        if (!reports.Any(p => p.Name.Equals(first.Name)))
                        {
                            reports.Add(first);
                        }
                        continue;
                    }
                    foreach (var item in first.Reports)
                    {
                        var secondReport = secondReports.FirstOrDefault(p => p.Name.Equals(first.Name));
                        if (!secondReport.Reports.Any(p => p.File.Equals(item.File) && p.Hash.Equals(item.Hash)))
                        {
                            var report = reports.FirstOrDefault(p => p.Name.Equals(first.Name));
                            if (report == null)
                            {
                                report = GetModelInstance<IHashReport>();
                                report.Name = first.Name;
                                reports.Add(report);
                            }
                            if (report.Reports == null)
                            {
                                report.Reports = new List<IHashFileReport>();
                            }
                            if (!report.Reports.Any(p => p.File.Equals(item.File)))
                            {
                                var hashReport = GetModelInstance<IHashFileReport>();
                                hashReport.File = item.File;
                                hashReport.Hash = item.Hash;
                                var secondHash = secondReport.Reports.FirstOrDefault(p => p.File.Equals(item.File));
                                if (secondHash != null)
                                {
                                    hashReport.SecondHash = secondHash.Hash;
                                }
                                report.Reports.Add(hashReport);
                            }
                        }
                        progress++;
                        var percentage = GetProgressPercentage(total, progress);
                        if (percentage != lastPercentage)
                        {
                            messageBus.Publish(new ModReportExportEvent(2, percentage));
                        }
                        lastPercentage = percentage;
                    }
                }
            }
            var reports = new List<IHashReport>();
            compareReports(reports, firstReport, secondReport);
            compareReports(reports, secondReport, firstReport);
            return reports;
        }

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

        /// <summary>
        /// Gets the progress percentage.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <param name="processed">The processed.</param>
        /// <param name="maxPerc">The maximum perc.</param>
        /// <returns>System.Double.</returns>
        protected virtual double GetProgressPercentage(double total, double processed, double maxPerc = 100)
        {
            var perc = Math.Round(processed / total * 100, 2);
            if (perc < 0)
            {
                perc = 0;
            }
            else if (perc > maxPerc)
            {
                perc = maxPerc;
            }
            return perc;
        }

        #endregion Methods
    }
}
