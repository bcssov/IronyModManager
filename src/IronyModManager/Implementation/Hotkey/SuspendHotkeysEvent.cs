// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-21-2021
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="SuspendHotkeysEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus.Events;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class SuspendHotkeysEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    public class SuspendHotkeysEvent : BaseNonAwaitableEvent
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
        /// Gets a value indicating whether [suspend hotkeys].
        /// </summary>
        /// <value><c>true</c> if [suspend hotkeys]; otherwise, <c>false</c>.</value>
        public bool SuspendHotkeys { get; private set; }

        #endregion Properties
    }
}
