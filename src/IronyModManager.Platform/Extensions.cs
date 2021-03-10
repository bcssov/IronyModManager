// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-10-2021
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
using Avalonia.Platform;
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

            // We don't have the ability to load every assembly right now, so we are
            // stuck with manual configuration  here
            // Helpers are extracted to separate methods to take the advantage of the fact
            // that CLR doesn't try to load dependencies before referencing method is jitted
            // Additionally, by having a hard reference to each assembly,
            // we verify that the assemblies are in the final .deps.json file
            //  so .NET Core knows where to load the assemblies from,.
            if (os == OperatingSystemType.WinNT)
            {
                LoadWin32(builder);
                LoadSkia(builder);
            }
            else if (os == OperatingSystemType.OSX)
            {
                LoadAvaloniaNative(builder);
                LoadSkia(builder);
            }
            else
            {
                LoadX11(builder);
                LoadSkia(builder);
            }
            return builder;
        }

        /// <summary>
        /// Loads the avalonia native.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        private static void LoadAvaloniaNative<TAppBuilder>(TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
             => builder.UseAvaloniaNative();

        /// <summary>
        /// Loads the direct2 d1.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
#pragma warning disable IDE0051 // Remove unused private members

        private static void LoadDirect2D1<TAppBuilder>(TAppBuilder builder)
#pragma warning restore IDE0051 // Remove unused private members
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
             => builder.UseWin32();

        /// <summary>
        /// Loads the skia.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        private static void LoadSkia<TAppBuilder>(TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
             => builder.UseSkia();

        /// <summary>
        /// Loads the win32.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        private static void LoadWin32<TAppBuilder>(TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
             => builder.UseWin32();

        /// <summary>
        /// Loads the X11.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        private static void LoadX11<TAppBuilder>(TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
             => builder.UseX11();

        #endregion Methods
    }
}
