// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="ConflictResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;
using IronyModManager.Shared.Models;

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
        /// The allowed languages
        /// </summary>
        private List<string> allowedLanguages;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets all conflicts.
        /// </summary>
        /// <value>All conflicts.</value>
        public IIndexedDefinitions AllConflicts { get; set; }

        /// <summary>
        /// Gets or sets the allowed languages.
        /// </summary>
        /// <value>The allowed languages.</value>
        public IEnumerable<string> AllowedLanguages
        {
            get => allowedLanguages;
            set => allowedLanguages = value == null ? [] : [.. value];
        }

        /// <summary>
        /// Gets or sets the conflicts.
        /// </summary>
        /// <value>The conflicts.</value>
        public IIndexedDefinitions Conflicts { get; set; }

        /// <summary>
        /// Gets or sets the custom conflicts.
        /// </summary>
        /// <value>The custom conflicts.</value>
        public IIndexedDefinitions CustomConflicts { get; set; }

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
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public PatchStateMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the overwritten conflicts.
        /// </summary>
        /// <value>The overwritten conflicts.</value>
        public IIndexedDefinitions OverwrittenConflicts { get; set; }

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

            GC.SuppressFinalize(this);
            disposed = true;
            AllConflicts?.Dispose();
            Conflicts?.Dispose();
            IgnoredConflicts?.Dispose();
            ResolvedConflicts?.Dispose();
            RuleIgnoredConflicts?.Dispose();
            OverwrittenConflicts?.Dispose();
            CustomConflicts?.Dispose();
            AllConflicts = null;
            Conflicts = null;
            IgnoredConflicts = null;
            ResolvedConflicts = null;
            RuleIgnoredConflicts = null;
            OverwrittenConflicts = null;
            CustomConflicts = null;
            AllowedLanguages = null;
        }

        #endregion Methods
    }
}
