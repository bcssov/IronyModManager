// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="NotificationPositionType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class NotificationPositionType.
    /// Implements the <see cref="IronyModManager.Storage.Common.INotificationPositionType" />
    /// </summary>
    /// <seealso cref="IronyModManager.Storage.Common.INotificationPositionType" />
    public class NotificationPositionType : INotificationPositionType
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public virtual NotificationPosition Position { get; set; }

        #endregion Properties
    }
}
