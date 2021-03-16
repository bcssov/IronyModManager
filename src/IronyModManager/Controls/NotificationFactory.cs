// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="NotificationFactory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Notifications;
using IronyModManager.DI;
using IronyModManager.Services.Common;
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

        /// <summary>
        /// The position settings service
        /// </summary>
        private INotificationPositionSettingsService positionSettingsService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <returns>IManagedNotificationManager.</returns>
        public IManagedNotificationManager GetManager()
        {
            if (manager is WindowNotificationManager windowNotificationManager)
            {
                if (positionSettingsService == null)
                {
                    positionSettingsService = DIResolver.Get<INotificationPositionSettingsService>();
                }
                var selected = positionSettingsService.Get().FirstOrDefault(p => p.IsSelected);
                if (selected != null)
                {
                    switch (selected.Type)
                    {
                        case Models.Common.NotificationPosition.BottomRight:
                            windowNotificationManager.Position = NotificationPosition.BottomRight;
                            break;

                        case Models.Common.NotificationPosition.BottomLeft:
                            windowNotificationManager.Position = NotificationPosition.BottomLeft;
                            break;

                        case Models.Common.NotificationPosition.TopRight:
                            windowNotificationManager.Position = NotificationPosition.TopRight;
                            break;

                        case Models.Common.NotificationPosition.TopLeft:
                            windowNotificationManager.Position = NotificationPosition.TopLeft;
                            break;

                        default:
                            break;
                    }
                }
            }
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
