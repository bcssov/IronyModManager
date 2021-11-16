// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
// ***********************************************************************
// <copyright file="ParadoxLauncherImporterBeta.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Common.Mods;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods.Importers
{
    /// <summary>
    /// Class ParadoxLauncherImporterBeta.
    /// Implements the <see cref="IronyModManager.IO.Mods.Importers.ParadoxLauncherImporter" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Importers.ParadoxLauncherImporter" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class ParadoxLauncherImporterBeta : ParadoxLauncherImporter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxLauncherImporterBeta" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ParadoxLauncherImporterBeta(ILogger logger) : base(logger)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// json import as an asynchronous operation.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public override Task<ICollectionImportResult> JsonImportAsync(ModCollectionExporterParams parameters)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        protected override string GetDbPath(ModCollectionExporterParams parameters)
        {
            return Path.Combine(Path.GetDirectoryName(parameters.ModDirectory), Constants.Sql_db_beta_path);
        }

        #endregion Methods
    }
}
