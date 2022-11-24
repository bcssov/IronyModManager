// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 04-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-24-2022
// ***********************************************************************
// <copyright file="PlatformConfigurationOptions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Platform.Configuration
{
    /// <summary>
    /// Class Fonts.
    /// </summary>
    public class Fonts
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [use inbuilt fonts only].
        /// </summary>
        /// <value><c>true</c> if [use inbuilt fonts only]; otherwise, <c>false</c>.</value>
        public bool UseInbuiltFontsOnly { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class LinuxOptions.
    /// </summary>
    public class LinuxOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the display server.
        /// </summary>
        /// <value>The display server.</value>
        public string DisplayServer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use d bus menu].
        /// </summary>
        /// <value><c>null</c> if [use d bus menu] contains no value, <c>true</c> if [use d bus menu]; otherwise, <c>false</c>.</value>
        public bool? UseDBusMenu { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use deferred rendering].
        /// </summary>
        /// <value><c>null</c> if [use deferred rendering] contains no value, <c>true</c> if [use deferred rendering]; otherwise, <c>false</c>.</value>
        public bool? UseDeferredRendering { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use egl].
        /// </summary>
        /// <value><c>null</c> if [use egl] contains no value, <c>true</c> if [use egl]; otherwise, <c>false</c>.</value>
        public bool? UseEGL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use gpu].
        /// </summary>
        /// <value><c>null</c> if [use gpu] contains no value, <c>true</c> if [use gpu]; otherwise, <c>false</c>.</value>
        public bool? UseGPU { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class PlatformLogging.
    /// </summary>
    public class Logging
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [enable avalonia logger].
        /// </summary>
        /// <value><c>true</c> if [enable avalonia logger]; otherwise, <c>false</c>.</value>
        public bool? EnableAvaloniaLogger { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class PlatformConfigurationOptions.
    /// </summary>
    public class PlatformConfigurationOptions
    {
        #region Properties

        /// <summary>
        /// Gets the fonts.
        /// </summary>
        /// <value>The fonts.</value>
        public Fonts Fonts { get; } = new Fonts();

        /// <summary>
        /// Gets the linux options.
        /// </summary>
        /// <value>The linux options.</value>
        public LinuxOptions LinuxOptions { get; } = new LinuxOptions();

        /// <summary>
        /// Gets the logging.
        /// </summary>
        /// <value>The logging.</value>
        public Logging Logging { get; } = new Logging();

        /// <summary>
        /// Gets the title bar.
        /// </summary>
        /// <value>The title bar.</value>
        public TitleBar TitleBar { get; } = new TitleBar();

        /// <summary>
        /// Gets the tooltips.
        /// </summary>
        /// <value>The tooltips.</value>
        public Tooltips Tooltips { get; } = new Tooltips();

        /// <summary>
        /// Gets the updates.
        /// </summary>
        /// <value>The updates.</value>
        public Updates Updates { get; } = new Updates();

        #endregion Properties
    }

    /// <summary>
    /// Class TitleBar.
    /// </summary>
    public class TitleBar
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TitleBar" /> is native.
        /// </summary>
        /// <value><c>true</c> if native; otherwise, <c>false</c>.</value>
        public bool Native { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class Tooltips.
    /// </summary>
    public class Tooltips
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Tooltips" /> is disable.
        /// </summary>
        /// <value><c>true</c> if disable; otherwise, <c>false</c>.</value>
        public bool Disable { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class Updates.
    /// </summary>
    public class Updates
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Updates" /> is disable.
        /// </summary>
        /// <value><c>true</c> if disable; otherwise, <c>false</c>.</value>
        public bool Disable { get; set; }

        #endregion Properties
    }
}
