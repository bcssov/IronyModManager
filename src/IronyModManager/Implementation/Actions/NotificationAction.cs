// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="NotificationAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Shared;
using MessageBox.Avalonia.Enums;

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
        /// <param name="onClick">The on click.</param>
        public void ShowNotification(string title, string message, NotificationType notificationType, int timeout = 5, Action onClick = null)
        {
            var type = notificationType switch
            {
                NotificationType.Success => Avalonia.Controls.Notifications.NotificationType.Success,
                NotificationType.Warning => Avalonia.Controls.Notifications.NotificationType.Warning,
                NotificationType.Error => Avalonia.Controls.Notifications.NotificationType.Error,
                _ => Avalonia.Controls.Notifications.NotificationType.Information,
            };
            Dispatcher.UIThread.SafeInvoke(() =>
            {
                var model = new Notification(title, message, type, TimeSpan.FromSeconds(timeout), onClick);
                notificationFactory.GetManager().Show(model);
            });
        }

        /// <summary>
        /// confirm action as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="promptType">Type of the prompt.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> ShowPromptAsync(string title, string header, string message, NotificationType notificationType, PromptType promptType = PromptType.YesNo)
        {
            var icon = notificationType switch
            {
                NotificationType.Info => Icon.Info,
                // Perhaps I should just fork this project over...
                NotificationType.Success => Icon.Plus,
                NotificationType.Warning => Icon.Warning,
                NotificationType.Error => Icon.Error,
                _ => Icon.None,
            };
            var mainWindow = Helpers.GetMainWindow();
            var prompt = MessageBoxes.GetPromptWindow(title, header, message, icon, promptType);
            var result = await prompt.ShowDialog(mainWindow);
            return result == ButtonResult.Yes || result == ButtonResult.Ok;
        }

        #endregion Methods
    }
}
