// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 04-04-2020
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
        /// The notification factory
        /// </summary>
        private readonly Controls.INotificationFactory notificationFactory;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationAction" /> class.
        /// </summary>
        /// <param name="notificationFactory">The notification factory.</param>
        public NotificationAction(Controls.INotificationFactory notificationFactory)
        {
            this.notificationFactory = notificationFactory;
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
            var type = notificationType switch
            {
                NotificationType.Success => Avalonia.Controls.Notifications.NotificationType.Success,
                NotificationType.Warning => Avalonia.Controls.Notifications.NotificationType.Warning,
                NotificationType.Error => Avalonia.Controls.Notifications.NotificationType.Error,
                _ => Avalonia.Controls.Notifications.NotificationType.Information,
            };
            var model = new Notification(title, message, type, TimeSpan.FromSeconds(timeout));
            notificationFactory.GetManager().Show(model);
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
            var icon = notificationType switch
            {
                NotificationType.Info => MessageBox.Avalonia.Enums.Icon.Info,
                // Perhaps I should just fork this project over...
                NotificationType.Success => MessageBox.Avalonia.Enums.Icon.Plus,
                NotificationType.Warning => MessageBox.Avalonia.Enums.Icon.Warning,
                NotificationType.Error => MessageBox.Avalonia.Enums.Icon.Error,
                _ => MessageBox.Avalonia.Enums.Icon.None,
            };
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
