// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="NavigationEventArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class NavigationEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [ExcludeFromCoverage("Excluding external message bus.")]
    public class NavigationEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public IConflictResult Results { get; set; }

        /// <summary>
        /// Gets or sets the selected collection.
        /// </summary>
        /// <value>The selected collection.</value>
        public IModCollection SelectedCollection { get; set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public IList<string> SelectedMods { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public NavigationState State { get; set; }

        #endregion Properties
    }
}
