// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 09-16-2020
// ***********************************************************************
// <copyright file="StaticResources.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
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
        /// The window icon
        /// </summary>
        private static WindowIcon windowIcon;

        #endregion Fields

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
        public static string GetUpdaterPath()
        {
            string companyPart = string.Empty;
            string appNamePart = string.Empty;

            var entryAssembly = Assembly.GetEntryAssembly();
            var companyAttribute = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
            if (!string.IsNullOrEmpty(companyAttribute.Company))
                companyPart = $"{companyAttribute.Company}\\";
            var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute.Title))
                appNamePart = $"{titleAttribute.Title}\\";

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{companyPart}{appNamePart}-Updater");
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
