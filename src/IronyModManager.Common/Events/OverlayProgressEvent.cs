// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-10-2020
//
// Last Modified By : Mario
// Last Modified On : 07-10-2020
// ***********************************************************************
// <copyright file="OverlayProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class OverlayProgressEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    [ExcludeFromCoverage("Excluding external message bus.")]
    public class OverlayProgressEvent : IMessageBusEvent
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsFireAndForget => true;

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
