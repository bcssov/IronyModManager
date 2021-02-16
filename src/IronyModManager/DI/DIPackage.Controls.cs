// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2021
// ***********************************************************************
// <copyright file="DIPackage.Controls.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Controls.Notifications;
using IronyModManager.Controls;
using IronyModManager.Views;
using SimpleInjector;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIPackage.
    /// Implements the <see cref="SimpleInjector.Packaging.IPackage" />
    /// </summary>
    /// <seealso cref="SimpleInjector.Packaging.IPackage" />
    public partial class DIPackage
    {
        /// <summary>
        /// Registers the controls.
        /// </summary>
        /// <param name="container">The container.</param>

        #region Methods

        private void RegisterControls(Container container)
        {
            var notificationFactory = new NotificationFactory();
            container.RegisterInitializer(d =>
            {
                var notificationManager = new WindowNotificationManager(d.Instance as MainWindow)
                {
                    Position = NotificationPosition.BottomRight,
                    MaxItems = 3
                };
                notificationFactory.SetManager(notificationManager);
            }, ctx => ctx.Registration.ImplementationType == typeof(MainWindow));
            container.RegisterInstance<INotificationFactory>(notificationFactory);
        }

        #endregion Methods
    }
}
