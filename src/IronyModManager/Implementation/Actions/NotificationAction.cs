// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="NotificationAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using IronyModManager.Shared;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Class NotificationAction.
    /// Implements the <see cref="IronyModManager.Implementation.Actions.INotificationAction" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Actions.INotificationAction" />
    [ExcludeFromCoverage("UI Actions are tested via functional testing.")]
    public class NotificationAction : INotificationAction
    {
        #region Fields

        /// <summary>
        /// The notification manager
        /// </summary>
        private readonly Controls.INotificationManager notificationManager;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationAction" /> class.
        /// </summary>
        /// <param name="notificationManager">The notification manager.</param>
        public NotificationAction(Controls.INotificationManager notificationManager)
        {
            this.notificationManager = notificationManager;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="timeout">The timeout.</param>
        public void ShowNotification(string title, string message, NotificationType notificationType, int timeout = 5)
        {
            Avalonia.Controls.Notifications.NotificationType type;
            switch (notificationType)
            {
                case NotificationType.Success:
                    type = Avalonia.Controls.Notifications.NotificationType.Success;
                    break;

                case NotificationType.Warning:
                    type = Avalonia.Controls.Notifications.NotificationType.Warning;
                    break;

                case NotificationType.Error:
                    type = Avalonia.Controls.Notifications.NotificationType.Error;
                    break;

                default:
                    type = Avalonia.Controls.Notifications.NotificationType.Information;
                    break;
            }
            var model = new Notification(title, message, type, TimeSpan.FromSeconds(timeout));
            notificationManager.Show(model);
        }

        /// <summary>
        /// confirm action as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="notificationType">Type of the notification.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ShowPromptAsync(string title, string header, string message, NotificationType notificationType)
        {
            MessageBox.Avalonia.Enums.Icon icon;
            switch (notificationType)
            {
                case NotificationType.Info:
                    icon = MessageBox.Avalonia.Enums.Icon.Info;
                    break;

                case NotificationType.Success:
                    icon = MessageBox.Avalonia.Enums.Icon.Success;
                    break;

                case NotificationType.Warning:
                    icon = MessageBox.Avalonia.Enums.Icon.Warning;
                    break;

                case NotificationType.Error:
                    icon = MessageBox.Avalonia.Enums.Icon.Error;
                    break;

                default:
                    icon = MessageBox.Avalonia.Enums.Icon.None;
                    break;
            }
            var mainWindow = GetMainWindow();
            var prompt = MessageBoxes.GetYesNoWindow(title, header, message, icon);
            var result = await prompt.ShowDialog(mainWindow);
            return result == MessageBox.Avalonia.Enums.ButtonResult.Yes;
        }

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <returns>Window.</returns>
        private Window GetMainWindow()
        {
            return ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
        }

        #endregion Methods
    }
}
