// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 06-19-2020
// ***********************************************************************
// <copyright file="ModMergeExporterParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModMergeExporterParameters.
    /// </summary>
    [ExcludeFromCoverage("Parameters.")]
    public class ModMergeExporterParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the definitions.
        /// </summary>
        /// <value>The definitions.</value>
        public IEnumerable<IDefinition> Definitions { get; set; }

        /// <summary>
        /// Gets or sets the export path.
        /// </summary>
        /// <value>The export path.</value>
        public string ExportPath { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public string Game { get; set; }

        /// <summary>
        /// Gets or sets the patch definitions.
        /// </summary>
        /// <value>The patch definitions.</value>
        public IEnumerable<IDefinition> PatchDefinitions { get; set; }

        #endregion Properties
    }
}
