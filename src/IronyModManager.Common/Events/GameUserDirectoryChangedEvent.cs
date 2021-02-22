// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 09-22-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="GameUserDirectoryChangedEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Shared.MessageBus.Events;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class GameUserDirectoryChangedEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    public class GameUserDirectoryChangedEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameUserDirectoryChangedEvent" /> class.
        /// </summary>
        /// <param name="game">The game.</param>
        public GameUserDirectoryChangedEvent(IGame game)
        {
            Game = game;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public IGame Game { get; set; }

        #endregion Properties
    }
}
