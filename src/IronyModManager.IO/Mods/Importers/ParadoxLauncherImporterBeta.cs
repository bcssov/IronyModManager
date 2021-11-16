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
    public class ParadoxLauncherImporterBeta : ParadoxLauncherImporter
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParadoxLauncherImporterBeta"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ParadoxLauncherImporterBeta(ILogger logger) : base(logger)
        {
        }

        #endregion Constructors

        #region Methods

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
