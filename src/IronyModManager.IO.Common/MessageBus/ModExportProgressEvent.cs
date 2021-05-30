// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 05-30-2021
//
// Last Modified By : Mario
// Last Modified On : 05-30-2021
// ***********************************************************************
// <copyright file="ModExportProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class ModExportProgressEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.BaseNonAwaitableEvent" />
    public class ModExportProgressEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModExportProgressEvent" /> class.
        /// </summary>
        /// <param name="progress">The progress.</param>
        public ModExportProgressEvent(double progress)
        {
            Progress = progress;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public double Progress { get; set; }

        #endregion Properties
    }
}
