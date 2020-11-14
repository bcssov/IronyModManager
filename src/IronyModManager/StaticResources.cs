// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 11-14-2020
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
        /// The application icon
        /// </summary>
        private static Bitmap iconBitmap;

        /// <summary>
        /// The updater path
        /// </summary>
        private static string[] updaterPath = null;

        /// <summary>
        /// The window icon
        /// </summary>
        private static WindowIcon windowIcon;

        #endregion Fields

        #region Properties

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
        /// Gets the path.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string[] GetUpdaterPath()
        {
            static string generatePath(bool useProperSeparator)
            {
                string companyPart = string.Empty;
                string appNamePart = string.Empty;

                var entryAssembly = Assembly.GetEntryAssembly();
                var companyAttribute = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
                if (!string.IsNullOrEmpty(companyAttribute.Company))
                {
                    if (useProperSeparator)
                    {
                        companyPart = $"{companyAttribute.Company}\\";
                    }
                    else
                    {
                        companyPart = $"{companyAttribute.Company}{Path.DirectorySeparatorChar}";
                    }
                }
                var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
                if (!string.IsNullOrEmpty(titleAttribute.Title))
                {
                    if (useProperSeparator)
                    {
                        appNamePart = $"{titleAttribute.Title}-Updater\\";
                    }
                    else
                    {
                        appNamePart = $"{titleAttribute.Title}-Updater{Path.DirectorySeparatorChar}";
                    }
                }
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{companyPart}{appNamePart}");
            }

            if (updaterPath == null)
            {
                var col = new List<string>() { generatePath(true), generatePath(false) };
                updaterPath = col.Distinct().ToArray();
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
