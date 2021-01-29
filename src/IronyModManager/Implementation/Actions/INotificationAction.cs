// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2021
// ***********************************************************************
// <copyright file="INotificationAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Interface INotificationAction
    /// </summary>
    public interface INotificationAction
    {
        #region Methods

        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="onClick">The on click.</param>
        void ShowNotification(string title, string message, NotificationType notificationType, int timeout = 5, Action onClick = null);

        /// <summary>
        /// Shows the prompt asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="header">The header.</param>
        /// <param name="message">The message.</param>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="promptType">Type of the prompt.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ShowPromptAsync(string title, string header, string message, NotificationType notificationType, PromptType promptType = PromptType.YesNo);

        #endregion Methods
    }
}
