// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="Stubs.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using Avalonia;
using Avalonia.Platform;
using IronyModManager.Shared;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class PlatformSettingsStub.
    /// Implements the <see cref="Avalonia.Platform.IPlatformSettings" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IPlatformSettings" />
    [ExcludeFromCoverage("External component.")]
    internal class PlatformSettingsStub : IPlatformSettings
    {
        #region Properties

        /// <summary>
        /// Gets the size of the double click.
        /// </summary>
        /// <value>The size of the double click.</value>
        public Size DoubleClickSize { get; } = new Size(2, 2);

        /// <summary>
        /// Gets the double click time.
        /// </summary>
        /// <value>The double click time.</value>
        public TimeSpan DoubleClickTime { get; } = TimeSpan.FromMilliseconds(500);

        #endregion Properties
    }
}
