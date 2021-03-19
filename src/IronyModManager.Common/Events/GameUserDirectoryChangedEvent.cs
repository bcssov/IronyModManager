// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 09-22-2020
//
// Last Modified By : Mario
// Last Modified On : 03-19-2021
// ***********************************************************************
// <copyright file="GameUserDirectoryChangedEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class GameUserDirectoryChangedEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseNonAwaitableEvent" />
    public class GameUserDirectoryChangedEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameUserDirectoryChangedEvent" /> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="customDirectoryChanged">if set to <c>true</c> [custom directory changed].</param>
        public GameUserDirectoryChangedEvent(IGame game, bool customDirectoryChanged)
        {
            Game = game;
            CustomDirectoryChanged = customDirectoryChanged;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [custom directory changed].
        /// </summary>
        /// <value><c>true</c> if [custom directory changed]; otherwise, <c>false</c>.</value>
        public bool CustomDirectoryChanged { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public IGame Game { get; set; }

        #endregion Properties
    }
}
