// ***********************************************************************
// Assembly         : IronyModManager.Storage.Common
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="INotificationPositionType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Interface INotificationPositionType
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface INotificationPositionType
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        NotificationPosition Position { get; set; }

        #endregion Properties
    }
}
