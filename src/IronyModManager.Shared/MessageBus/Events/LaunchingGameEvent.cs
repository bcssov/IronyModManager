// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="LaunchingGameEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.MessageBus.Events
{
    /// <summary>
    /// Class LaunchingGameEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseAwaitableEvent" />
    public class LaunchingGameEvent : BaseAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchingGameEvent" /> class.
        /// </summary>
        /// <param name="gameType">Type of the game.</param>
        public LaunchingGameEvent(string gameType)
        {
            GameType = gameType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the type of the game.
        /// </summary>
        /// <value>The type of the game.</value>
        public string GameType { get; }

        #endregion Properties
    }
}
