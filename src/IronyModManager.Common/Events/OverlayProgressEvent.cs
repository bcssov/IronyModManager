// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="OverlayProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus.Events;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class OverlayProgressEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    [ExcludeFromCoverage("Excluding external message bus.")]
    public class OverlayProgressEvent : BaseNonAwaitableEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message progress.
        /// </summary>
        /// <value>The message progress.</value>
        public string MessageProgress { get; set; }

        #endregion Properties
    }
}
