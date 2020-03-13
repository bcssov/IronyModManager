// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="INotificationFactory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Controls.Notifications;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Interface INotificationManager
    /// Implements the <see cref="Avalonia.Controls.Notifications.IManagedNotificationManager" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Notifications.IManagedNotificationManager" />
    public interface INotificationFactory
    {
        #region Methods

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <returns>IManagedNotificationManager.</returns>
        public IManagedNotificationManager GetManager();

        #endregion Methods
    }
}
