// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 09-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="UpdateUnpackProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared.MessageBus.Events;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class UpdateUnpackProgressEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    public class UpdateUnpackProgressEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUnpackProgressEvent" /> class.
        /// </summary>
        /// <param name="progress">The progress.</param>
        public UpdateUnpackProgressEvent(int progress)
        {
            Progress = progress;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public int Progress { get; set; }

        #endregion Properties
    }
}
