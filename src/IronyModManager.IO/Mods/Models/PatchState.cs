// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="PatchState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;

namespace IronyModManager.IO.Mods.Models
{
    /// <summary>
    /// Class PatchState.
    /// Implements the <see cref="IronyModManager.IO.Common.Mods.Models.IPatchState" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Mods.Models.IPatchState" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class PatchState : IPatchState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conflict history.
        /// </summary>
        /// <value>The conflict history.</value>
        public IEnumerable<IDefinition> ConflictHistory { get; set; }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public IEnumerable<IDefinition> Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        public IEnumerable<IDefinition> OrphanConflicts { get; set; }

        /// <summary>
        /// Gets or sets the resolved conflicts.
        /// </summary>
        /// <value>The resolved conflicts.</value>
        public IEnumerable<IDefinition> ResolvedConflicts { get; set; }

        #endregion Properties
    }
}
