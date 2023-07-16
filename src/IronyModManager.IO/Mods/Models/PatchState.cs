// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-31-2020
//
// Last Modified By : Mario
// Last Modified On : 11-01-2021
// ***********************************************************************
// <copyright file="PatchState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

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
        #region Fields

        /// <summary>
        /// The conflict history
        /// </summary>
        private IEnumerable<IDefinition> conflictHistory;

        /// <summary>
        /// The indexed conflict history
        /// </summary>
        private IDictionary<string, IEnumerable<IDefinition>> indexedConflictHistory;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the conflict history.
        /// </summary>
        /// <value>The conflict history.</value>
        public IEnumerable<IDefinition> ConflictHistory
        {
            get
            {
                return conflictHistory;
            }
            set
            {
                conflictHistory = value;
                indexedConflictHistory = null;
                InitConflictHistoryIndex();
            }
        }

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
        /// Gets or sets a value indicating whether [game files included].
        /// </summary>
        /// <value><c>true</c> if [game files included]; otherwise, <c>false</c>.</value>
        public bool HasGameDefinitions { get; set; }

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
        /// Gets the indexed conflict history.
        /// </summary>
        /// <value>The indexed conflict history.</value>
        public IDictionary<string, IEnumerable<IDefinition>> IndexedConflictHistory
        {
            get
            {
                if (indexedConflictHistory == null)
                {
                    InitConflictHistoryIndex();
                }
                return indexedConflictHistory;
            }
        }

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
        /// Gets or sets the overwritten conflicts.
        /// </summary>
        /// <value>The overwritten conflicts.</value>
        public IEnumerable<IDefinition> OverwrittenConflicts { get; set; }

        /// <summary>
        /// Gets or sets the resolved conflicts.
        /// </summary>
        /// <value>The resolved conflicts.</value>
        public IEnumerable<IDefinition> ResolvedConflicts { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the index of the conflict history.
        /// </summary>
        private void InitConflictHistoryIndex()
        {
            indexedConflictHistory ??= new Dictionary<string, IEnumerable<IDefinition>>();
            if (conflictHistory != null && conflictHistory.Any())
            {
                conflictHistory.ToList().ForEach(p =>
                {
                    if (indexedConflictHistory.ContainsKey(p.TypeAndId))
                    {
                        var col = indexedConflictHistory[p.TypeAndId].ToList();
                        col.Add(p);
                        indexedConflictHistory[p.Type] = col;
                    }
                    else
                    {
                        indexedConflictHistory.Add(p.TypeAndId, new List<IDefinition>() { p });
                    }
                });
            }
        }

        #endregion Methods
    }
}
