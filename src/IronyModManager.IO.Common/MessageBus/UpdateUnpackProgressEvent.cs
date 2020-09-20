// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 09-20-2020
// ***********************************************************************
// <copyright file="UpdateUnpackProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class UpdateUnpackProgressEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public class UpdateUnpackProgressEvent : IMessageBusEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUnpackProgressEvent"/> class.
        /// </summary>
        /// <param name="progress">The progress.</param>
        public UpdateUnpackProgressEvent(int progress)
        {
            Progress = progress;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        public bool IsFireAndForget => false;

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public int Progress { get; set; }

        #endregion Properties
    }
}
