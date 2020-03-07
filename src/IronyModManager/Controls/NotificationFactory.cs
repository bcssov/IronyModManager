// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="NotificationFactory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia.Controls.Notifications;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class NotificationFactory.
    /// Implements the <see cref="IronyModManager.Controls.INotificationFactory" />
    /// </summary>
    /// <seealso cref="IronyModManager.Controls.INotificationFactory" />
    [ExcludeFromCoverage("UI Elements should be tested in functional testing.")]
    public class NotificationFactory : INotificationFactory
    {
        #region Fields

        /// <summary>
        /// The manager
        /// </summary>
        private IManagedNotificationManager manager;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <returns>IManagedNotificationManager.</returns>
        public IManagedNotificationManager GetManager()
        {
            return manager;
        }

        /// <summary>
        /// Sets the manager.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public void SetManager(IManagedNotificationManager manager)
        {
            this.manager = manager;
        }

        #endregion Methods
    }
}
