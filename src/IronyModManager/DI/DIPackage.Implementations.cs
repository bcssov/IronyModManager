// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="DIPackage.Implementations.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Common;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Log;
using IronyModManager.Shared;
using Container = SimpleInjector.Container;

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
        /// Registers the implementations.
        /// </summary>
        /// <param name="container">The container.</param>
        private void RegisterImplementations(Container container)
        {
            container.Register<IViewResolver, ViewResolver>();
            container.Register<ILogger, Logger>();
            container.Collection.Register<ILocalizationResourceProvider>(typeof(LocalizationResourceProvider));
            container.Register<IDefaultLocalizationResourceProvider, LocalizationResourceProvider>();
            container.Register<IAppAction, AppAction>();
            container.Register<INotificationAction, NotificationAction>();
            container.Register<IFileDialogAction, FileDialogAction>();
            container.Register<WritingStateOperationHandler>(SimpleInjector.Lifestyle.Singleton);
        }

        #endregion Methods
    }
}
