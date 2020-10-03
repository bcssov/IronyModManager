// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-02-2020
//
// Last Modified By : Mario
// Last Modified On : 10-02-2020
// ***********************************************************************
// <copyright file="Extensions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace IronyModManager.Implementation.AssetLoader
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Uses the irony asset loader.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of the t application builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>TAppBuilder.</returns>
        public static TAppBuilder UseIronyAssetLoader<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            builder.AfterSetup(_ =>
            {
                AssetLoader.RegisterResUriParsers();
                AvaloniaLocator.CurrentMutable.Bind<IAssetLoader>().ToConstant(new AssetLoader());
            });
            return builder;
        }

        #endregion Methods
    }
}
