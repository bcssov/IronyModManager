// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-11-2020
// ***********************************************************************
// <copyright file="ConflictResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class ConflictResult.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IConflictResult" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IConflictResult" />
    public class ConflictResult : BaseModel, IConflictResult
    {
        #region Fields

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets all conflicts.
        /// </summary>
        /// <value>All conflicts.</value>
        public IIndexedDefinitions AllConflicts { get; set; }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public IIndexedDefinitions Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the ignored conflicts.
        /// </summary>
        /// <value>The ignored conflicts.</value>
        public IIndexedDefinitions IgnoredConflicts { get; set; }

        /// <summary>
        /// Gets or sets the ignored paths.
        /// </summary>
        /// <value>The ignored paths.</value>
        public string IgnoredPaths { get; set; }

        /// <summary>
        /// Gets or sets the orphan conflicts.
        /// </summary>
        /// <value>The orphan conflicts.</value>
        public IIndexedDefinitions OrphanConflicts { get; set; }

        /// <summary>
        /// Gets or sets the resolved conflicts.
        /// </summary>
        /// <value>The resolved conflicts.</value>
        public IIndexedDefinitions ResolvedConflicts { get; set; }

        /// <summary>
        /// Gets or sets the rule ignored conflicts.
        /// </summary>
        /// <value>The rule ignored conflicts.</value>
        public IIndexedDefinitions RuleIgnoredConflicts { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            AllConflicts?.Dispose();
            Conflicts?.Dispose();
            IgnoredConflicts?.Dispose();
            OrphanConflicts?.Dispose();
            ResolvedConflicts?.Dispose();
            RuleIgnoredConflicts?.Dispose();
            AllConflicts = null;
            Conflicts = null;
            IgnoredConflicts = null;
            ResolvedConflicts = null;
            OrphanConflicts = null;
            RuleIgnoredConflicts = null;
        }

        #endregion Methods
    }
}
