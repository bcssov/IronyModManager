// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-21-2021
//
// Last Modified By : Mario
// Last Modified On : 02-21-2021
// ***********************************************************************
// <copyright file="SuspendHotkeysEvent.cs" company="Mario">
//     Mario
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
    /// Class SuspendHotkeysEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public class SuspendHotkeysEvent : IMessageBusEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspendHotkeysEvent" /> class.
        /// </summary>
        /// <param name="suspendHotkeys">if set to <c>true</c> [suspend hotkeys].</param>
        public SuspendHotkeysEvent(bool suspendHotkeys)
        {
            SuspendHotkeys = suspendHotkeys;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsFireAndForget => true;

        /// <summary>
        /// Gets a value indicating whether [suspend hotkeys].
        /// </summary>
        /// <value><c>true</c> if [suspend hotkeys]; otherwise, <c>false</c>.</value>
        public bool SuspendHotkeys { get; private set; }

        #endregion Properties
    }
}
