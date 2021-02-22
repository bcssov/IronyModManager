// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-02-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="ModPatchExporterParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.Common.Mods
{
    /// <summary>
    /// Class ModPatchExporterParameters.
    /// </summary>
    [ExcludeFromCoverage("Parameters.")]
    public class ModPatchExporterParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public IEnumerable<IDefinition> Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the custom conflicts.
        /// </summary>
        /// <value>The custom conflicts.</value>
        public IEnumerable<IDefinition> CustomConflicts { get; set; }

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
        /// Gets or sets the ignore conflict paths.
        /// </summary>
        /// <value>The ignore conflict paths.</value>
        public string IgnoreConflictPaths { get; set; }

        /// <summary>
        /// Gets or sets the ignored conflicts.
        /// </summary>
        /// <value>The ignored conflicts.</value>
        public IEnumerable<IDefinition> IgnoredConflicts { get; set; }

        /// <summary>
        /// Gets or sets the load order.
        /// </summary>
        /// <value>The load order.</value>
        public IEnumerable<string> LoadOrder { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public PatchStateMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the mod path.
        /// </summary>
        /// <value>The mod path.</value>
        public string ModPath { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        public IEnumerable<IDefinition> OrphanConflicts { get; set; }

        /// <summary>
        /// Gets or sets the overwritten conflicts.
        /// </summary>
        /// <value>The overwritten conflicts.</value>
        public IEnumerable<IDefinition> OverwrittenConflicts { get; set; }

        /// <summary>
        /// Gets or sets the name of the patch.
        /// </summary>
        /// <value>The name of the patch.</value>
        public string PatchName { get; set; }

        /// <summary>
        /// Gets or sets the rename pairs.
        /// </summary>
        /// <value>The rename pairs.</value>
        public List<KeyValuePair<string, string>> RenamePairs { get; set; }

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
