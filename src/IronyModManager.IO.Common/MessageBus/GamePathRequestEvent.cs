// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 05-18-2026
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="GamePathRequestEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class GameInfoRequestEvent.
    /// Implements the <see cref="BaseAwaitableEvent" />
    /// </summary>
    /// <seealso cref="BaseAwaitableEvent" />
    public class GameInfoRequestEvent(string gameId) : BaseAwaitableEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base steam dir.
        /// </summary>
        /// <value>The base steam dir.</value>
        public string BaseSteamDir { get; set; }

        /// <summary>
        /// Gets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        public string GameId { get; } = gameId;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is proton.
        /// </summary>
        /// <value><c>true</c> if this instance is proton; otherwise, <c>false</c>.</value>
        public bool IsProton { get; set; }

        /// <summary>
        /// Gets or sets the steam application identifier.
        /// </summary>
        /// <value>The steam application identifier.</value>
        public long SteamAppId { get; set; }

        #endregion Properties
    }
}
