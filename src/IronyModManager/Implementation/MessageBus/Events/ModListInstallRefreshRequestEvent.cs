// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-22-2021
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="ModListInstallRefreshRequestEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus.Events
{
    /// <summary>
    /// Class ModListInstallRefreshRequestEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseAwaitableEvent" />
    public class ModListInstallRefreshRequestEvent : BaseAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModListRefreshRequestEvent" /> class.
        /// </summary>
        /// <param name="skipOverlay">if set to <c>true</c> [skip overlay].</param>
        public ModListInstallRefreshRequestEvent(bool skipOverlay)
        {
            SkipOverlay = skipOverlay;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [skip overlay].
        /// </summary>
        /// <value><c>true</c> if [skip overlay]; otherwise, <c>false</c>.</value>
        public bool SkipOverlay { get; set; }

        #endregion Properties
    }
}
