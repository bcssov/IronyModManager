// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="INotificationPositionSettingsService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface INotificationPositionSettingsService
    /// </summary>
    public interface INotificationPositionSettingsService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;INotificationPosition&gt;.</returns>
        IEnumerable<INotificationPosition> Get();

        /// <summary>
        /// Saves the specified notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(INotificationPosition notification);

        #endregion Methods
    }
}
