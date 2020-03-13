// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 03-10-2020
//
// Last Modified By : Mario
// Last Modified On : 03-10-2020
// ***********************************************************************
// <copyright file="OverlayEventArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class OverlayEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OverlayEventArgs : EventArgs
    {
        #region Properties

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

        #endregion Properties
    }
}
