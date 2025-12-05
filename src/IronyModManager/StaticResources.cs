// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-05-2025
// ***********************************************************************
// <copyright file="StaticResources.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using IronyModManager.IO.Common;
using IronyModManager.Shared;

namespace IronyModManager
{
    /// <summary>
    /// Class StaticResources.
    /// </summary>
    [ExcludeFromCoverage("Static resource will not be tested.")]
    public static class StaticResources
    {
        #region Fields

        /// <summary>
        /// The command line options
        /// </summary>
        private static CommandLineArgs commandLineOptions;

        /// <summary>
        /// The application icon
        /// </summary>
        private static Bitmap iconBitmap;

        /// <summary>
        /// The last known location
        /// </summary>
        private static string lastKnownLocation = string.Empty;

        /// <summary>
        /// The log location
        /// </summary>
        private static string logLocation = string.Empty;

        /// <summary>
        /// The updater path
        /// </summary>
        private static string updaterPath = string.Empty;

        /// <summary>
        /// The window icon
        /// </summary>
        private static WindowIcon windowIcon;

        #endregion Fields

        #region Delegates

        /// <summary>
        /// Delegate CommandLineArgsChangedDelegate
        /// </summary>
        public delegate void CommandLineArgsChangedDelegate();

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [command line arguments changed].
        /// </summary>
        public static event CommandLineArgsChangedDelegate CommandLineArgsChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the allow command line change.
        /// </summary>
        /// <value><c>true</c> if allow command line change; otherwise, <c>false</c>.</value>
        public static bool AllowCommandLineChange { get; set; } = true;

        /// <summary>
        /// Gets or sets the command line options.
        /// </summary>
        /// <value>The command line options.</value>
        public static CommandLineArgs CommandLineOptions
        {
            get
            {
                commandLineOptions ??= new CommandLineArgs();
                return commandLineOptions;
            }
            set
            {
                commandLineOptions = value;
                CommandLineArgsChanged?.Invoke();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is verifying container.
        /// </summary>
        /// <value><c>true</c> if this instance is verifying container; otherwise, <c>false</c>.</value>
        public static bool IsVerifyingContainer { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the application icon.
        /// </summary>
        /// <returns>WindowIcon.</returns>
        public static WindowIcon GetAppIcon()
        {
            if (windowIcon == null)
            {
                using var ms = GetAppIconStream();
                windowIcon = new WindowIcon(ms);
            }

            return windowIcon;
        }

        /// <summary>
        /// Gets the application icon bitmap.
        /// </summary>
        /// <returns>Bitmap.</returns>
        public static Bitmap GetAppIconBitmap()
        {
            if (iconBitmap == null)
            {
                using var ms = GetAppIconStream();
                iconBitmap = new Bitmap(ms);
            }

            return iconBitmap;
        }

        /// <summary>
        /// Gets the last known location information.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetLastKnownLocationInfo()
        {
            if (!string.IsNullOrWhiteSpace(lastKnownLocation))
            {
                return lastKnownLocation;
            }

            var segment = string.Empty;

            var entryAssembly = Assembly.GetEntryAssembly();
            var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly!, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute!.Title))
            {
                segment = $"{titleAttribute.Title}-Location{Path.DirectorySeparatorChar}";
            }

            var storagePath = DiskOperations.ResolveStoragePath();

            lastKnownLocation = Path.Combine(storagePath, segment).StandardizeDirectorySeparator();

            return lastKnownLocation;
        }

        /// <summary>
        /// Gets the log location.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetLogLocation()
        {
            if (!string.IsNullOrWhiteSpace(logLocation))
            {
                return logLocation;
            }

            var segment = string.Empty;

            var entryAssembly = Assembly.GetEntryAssembly();
            var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly!, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute!.Title))
            {
                segment = $"{titleAttribute.Title}-Logs{Path.DirectorySeparatorChar}";
            }

            var storagePath = DiskOperations.ResolveStoragePath();

            logLocation = Path.Combine(storagePath, segment).StandardizeDirectorySeparator();

            return logLocation;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetUpdaterPath()
        {
            static string generatePath()
            {
                var segment = string.Empty;

                var entryAssembly = Assembly.GetEntryAssembly();
                var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly!, typeof(AssemblyTitleAttribute));
                if (!string.IsNullOrEmpty(titleAttribute!.Title))
                {
                    segment = $"{titleAttribute.Title}-Updater\\";
                }

                var storagePath = DiskOperations.ResolveStoragePath();

                return Path.Combine(storagePath, segment).StandardizeDirectorySeparator();
            }

            if (string.IsNullOrWhiteSpace(updaterPath))
            {
                updaterPath = generatePath();
            }

            return updaterPath;
        }

        /// <summary>
        /// Gets the application icon stream.
        /// </summary>
        /// <returns>MemoryStream.</returns>
        private static MemoryStream GetAppIconStream()
        {
            var ms = new MemoryStream(ResourceReader.GetEmbeddedResource(Constants.Resources.LogoIco));
            return ms;
        }

        #endregion Methods
    }
}
