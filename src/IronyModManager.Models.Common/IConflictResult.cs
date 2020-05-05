// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-05-2020
// ***********************************************************************
// <copyright file="IConflictResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IConflictResult
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IConflictResult : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets all conflicts.
        /// </summary>
        /// <value>All conflicts.</value>
        IIndexedDefinitions AllConflicts { get; set; }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        IIndexedDefinitions Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the ignored conflicts.
        /// </summary>
        /// <value>The ignored conflicts.</value>
        IIndexedDefinitions IgnoredConflicts { get; set; }

        /// <summary>
        /// Gets or sets the ignored paths.
        /// </summary>
        /// <value>The ignored paths.</value>
        string IgnoredPaths { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        IIndexedDefinitions OrphanConflicts { get; set; }

        /// <summary>
        /// Gets or sets the resolved conflicts.
        /// </summary>
        /// <value>The resolved conflicts.</value>
        IIndexedDefinitions ResolvedConflicts { get; set; }

        /// <summary>
        /// Gets or sets the rule ignored conflicts.
        /// </summary>
        /// <value>The rule ignored conflicts.</value>
        IIndexedDefinitions RuleIgnoredConflicts { get; set; }

        #endregion Properties
    }
}
