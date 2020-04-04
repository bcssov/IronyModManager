// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 04-04-2020
// ***********************************************************************
// <copyright file="ModPatchExporterParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModPatchExporterParameters.
    /// </summary>
    public class ModPatchExporterParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public IEnumerable<IDefinition> Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>The definition.</value>
        public IEnumerable<IDefinition> Definitions { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public string Game { get; set; }

        /// <summary>
        /// Gets or sets the mod root path.
        /// </summary>
        /// <value>The mod root path.</value>
        public string ModRootPath { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        public IEnumerable<IDefinition> OrphanConflicts { get; set; }

        /// <summary>
        /// Gets or sets the name of the patch.
        /// </summary>
        /// <value>The name of the patch.</value>
        public string PatchName { get; set; }

        /// <summary>
        /// Gets or sets the resolved conflicts.
        /// </summary>
        /// <value>The resolved conflicts.</value>
        public IEnumerable<IDefinition> ResolvedConflicts { get; set; }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        /// <value>The root path.</value>
        public string RootPath { get; set; }

        #endregion Properties
    }
}
