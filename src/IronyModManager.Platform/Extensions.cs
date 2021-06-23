// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 06-23-2021
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using IronyModManager.Platform.Assets;
using IronyModManager.Platform.Clipboard;
using IronyModManager.Platform.Fonts;
using IronyModManager.Shared;

namespace IronyModManager.Platform
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    [ExcludeFromCoverage("External component.")]
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Uses the irony platform detect.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>TAppBuilder.</returns>
        public static TAppBuilder UseIronyPlatformDetect<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.UsePlatformDetect();
            builder.AfterSetup(s =>
            {
                // Use already registered manager as a proxy -- doing it like this because the implementation is hidden away as internal
                var fontManager = AvaloniaLocator.Current.GetService<IFontManagerImpl>();
                AvaloniaLocator.CurrentMutable.Bind<IFontManagerImpl>().ToConstant(new FontManager(fontManager));
                // Too many variations to take into consideration so escape the hacky way of getting clipboard implementation
                var clipboard = AvaloniaLocator.Current.GetService<IClipboard>();
                AvaloniaLocator.CurrentMutable.Bind<IClipboard>().ToConstant(new IronyClipboard(clipboard));
                // Asset loader
                AssetLoader.RegisterResUriParsers();
                AvaloniaLocator.CurrentMutable.Bind<IAssetLoader>().ToConstant(new AssetLoader());
                // Input manager
                var inputManager = AvaloniaLocator.Current.GetService<IInputManager>();
                AvaloniaLocator.CurrentMutable.Bind<IInputManager>().ToConstant(new InputManager.InputManager(inputManager));
            });
            return builder;
        }

        #endregion Methods
    }
}
