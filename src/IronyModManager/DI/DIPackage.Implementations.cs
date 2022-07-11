// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="DIPackage.Implementations.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Common;
using IronyModManager.DI.Extensions;
using IronyModManager.Fonts;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.AvaloniaEdit;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Implementation.Themes;
using IronyModManager.Implementation.Updater;
using IronyModManager.Log;
using IronyModManager.Platform.Fonts;
using IronyModManager.Platform.Themes;
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
        /// <summary>
        /// Registers the implementations.
        /// </summary>
        /// <param name="container">The container.</param>

        #region Methods

        private void RegisterImplementations(Container container)
        {
            container.Register<IViewResolver, ViewResolver>();
            container.Register<ILogger, Logger>();
            container.Register<IAppAction, AppAction>();
            container.Register<INotificationAction, NotificationAction>();
            container.Register<IFileDialogAction, FileDialogAction>();
            container.Register<IShutDownState, ShutdownState>(SimpleInjector.Lifestyle.Singleton);
            container.Register<IUpdater, Updater>(SimpleInjector.Lifestyle.Singleton);
            container.RemoveMixedLifetimeWarning<IAppAction>();
            container.Collection.Register(typeof(IFontFamily), new List<Type>()
            {
                typeof(NotoSansFontFamily), typeof(NotoSansSCFontFamily)
            }, SimpleInjector.Lifestyle.Singleton);
            container.Register<IFontFamilyManager, FontFamilyManager>(SimpleInjector.Lifestyle.Singleton);
            container.Register<IIDGenerator, IDGenerator>(SimpleInjector.Lifestyle.Singleton);
            container.Register<IHotkeyManager, HotkeyManager>();
            container.Register<IThemeManager, ThemeManager>();
            container.Collection.Register(typeof(IThemeResources), typeof(DIPackage).Assembly);
            container.Register<IResourceLoader, ResourceLoader>();
            container.Register<IScrollState, ScrollState>(SimpleInjector.Lifestyle.Singleton);
        }

        #endregion Methods
    }
}
