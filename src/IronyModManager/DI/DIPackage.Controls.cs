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
            INotificationManager notificationManager = null;
            container.RegisterInitializer(d =>
            {
                if (d.Instance is MainWindow)
                {
                    notificationManager = new NotificationManager(d.Instance as MainWindow)
                    {
                        Position = Avalonia.Controls.Notifications.NotificationPosition.BottomRight
                    };
                }
            }, ctx => ctx.Registration.Lifestyle == Lifestyle.Transient);
            container.Register(() => notificationManager);
        }

        #endregion Methods
    }
}
