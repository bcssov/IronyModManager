// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-06-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="IPatchState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.IO.Common.Mods.Models
{
    /// <summary>
    /// Interface IPatchState
    /// </summary>
    public interface IPatchState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conflict history.
        /// </summary>
        /// <value>The conflict history.</value>
        IEnumerable<IDefinition> ConflictHistory { get; set; }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        IEnumerable<IDefinition> Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        IEnumerable<IDefinition> OrphanConflicts { get; set; }

        /// <summary>
        /// Gets or sets the resolved conflicts.
        /// </summary>
        /// <value>The resolved conflicts.</value>
        IEnumerable<IDefinition> ResolvedConflicts { get; set; }

        #endregion Properties
    }
}
