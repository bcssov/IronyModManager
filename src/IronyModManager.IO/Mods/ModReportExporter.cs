// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="ModReportExporter.cs" company="Mario">
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
using IronyModManager.IO.Common.Mods;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods
{
    /// <summary>
    /// Class ModReportExporter.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.IModReportExporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.IModReportExporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class ModReportExporter : IModReportExporter
    {
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModReportExporter" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ModReportExporter(ILogger logger)
        {
            this.logger = logger;
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
            var retryStrategy = new RetryStrategy();
            if (modHashes?.Count() > 0)
            {
                var json = JsonDISerializer.Serialize(modHashes);
                return retryStrategy.RetryActionAsync(async () =>
                {
                    await File.WriteAllTextAsync(path, json);
                    return true;
                });
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// import as an asynchronous operation.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Task&lt;IEnumerable&lt;IModHashFileReport&gt;&gt;.</returns>
        public async Task<IEnumerable<IHashReport>> ImportAsync(string path)
        {
            if (File.Exists(path))
            {
                var json = await File.ReadAllTextAsync(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        var result = JsonDISerializer.Deserialize<IEnumerable<IHashReport>>(json);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
            return null;
        }

        #endregion Methods
    }
}
