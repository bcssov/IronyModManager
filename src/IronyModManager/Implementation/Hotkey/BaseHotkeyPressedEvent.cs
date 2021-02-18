// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
// ***********************************************************************
// <copyright file="BaseHotkeyPressedEvent.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class HotkeyPressedEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public abstract class BaseHotkeyPressedEvent : IMessageBusEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHotkeyPressedEvent" /> class.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        public BaseHotkeyPressedEvent(Enums.HotKeys hotKey)
        {
            Hotkey = hotKey;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the hotkey.
        /// </summary>
        /// <value>The hotkey.</value>
        public Enums.HotKeys Hotkey { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsFireAndForget => true;

        #endregion Properties
    }
}
