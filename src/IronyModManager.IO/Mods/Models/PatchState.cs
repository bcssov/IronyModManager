// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 04-01-2020
// ***********************************************************************
// <copyright file="PatchState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.IO.Mods.Models
{
    /// <summary>
    /// Class PatchState.
    /// </summary>
    internal class PatchState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public List<IDefinition> Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        public List<IDefinition> OrphanConflicts { get; set; }

        #endregion Properties
    }
}
