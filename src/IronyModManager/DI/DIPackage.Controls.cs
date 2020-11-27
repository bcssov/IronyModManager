// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 11-27-2020
// ***********************************************************************
// <copyright file="DIPackage.Controls.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
#pragma warning disable CA1822 // Mark members as static

        #region Methods

        private void RegisterControls(Container container)
#pragma warning restore CA1822 // Mark members as static
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
