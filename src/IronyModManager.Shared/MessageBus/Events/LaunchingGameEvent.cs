// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
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
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public class LaunchingGameEvent : IMessageBusEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LaunchingGameEvent"/> class.
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

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsFireAndForget => false;

        #endregion Properties
    }
}
