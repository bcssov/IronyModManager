// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="DIPackage.Controls.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
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
        #region Methods

        /// <summary>
        /// Registers the controls.
        /// </summary>
        /// <param name="container">The container.</param>
        private void RegisterControls(Container container)
        {
            var notificationFactory = new NotificationFactory();
            container.RegisterInitializer(d =>
            {
                var notificationManager = new NotificationManager(d.Instance as MainWindow)
                {
                    Position = Avalonia.Controls.Notifications.NotificationPosition.BottomRight
                };
                notificationFactory.SetManager(notificationManager);
            }, ctx => ctx.Registration.ImplementationType == typeof(MainWindow));
            container.RegisterInstance<INotificationFactory>(notificationFactory);
        }

        #endregion Methods
    }
}
