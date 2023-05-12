// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 05-12-2023
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
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using IronyModManager.DI;
using IronyModManager.Platform.Assets;
using IronyModManager.Platform.Clipboard;
using IronyModManager.Platform.Configuration;
using IronyModManager.Platform.Drives;
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
            var os = builder.RuntimePlatform.GetRuntimeInfo().OperatingSystem;
            var options = DIResolver.Get<IPlatformConfiguration>().GetOptions().LinuxOptions;
            if (LinuxDisplayServer.IsAuto(options.DisplayServer))
            {
                // Use the same logic from the fork, if env variable is set then use wayland display otherwise fallback
                if (Environment.GetEnvironmentVariable("WAYLAND_DISPLAY") is not null)
                {
                    LoadWayland(builder);
                    builder.UseSkia();
                }
                else
                {
                    builder.UsePlatformDetect();
                }
            }
            else if (LinuxDisplayServer.IsX11(options.DisplayServer))
            {
                // Standard check if x11 it will fallback to x11 itself
                builder.UsePlatformDetect();
            }
            else if (LinuxDisplayServer.IsWayland(options.DisplayServer))
            {
                // User says I want wayland give them wayland
                if (os == OperatingSystemType.Linux)
                {
                    LoadWayland(builder);
                    builder.UseSkia();
                }
                else
                {
                    builder.UsePlatformDetect();
                }
            }
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
                // Windows only
                if (os == OperatingSystemType.WinNT)
                {
                    AvaloniaLocator.CurrentMutable.Bind<IMountedVolumeInfoProvider>().ToConstant(new WindowsMountedVolumeInfoProvider());
                }
            });
            return builder;
        }

        /// <summary>
        /// Loads the wayland.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        private static void LoadWayland<TAppBuilder>(TAppBuilder builder) where TAppBuilder : AppBuilderBase<TAppBuilder>, new() => builder.UseWayland();

        #endregion Methods
    }
}
