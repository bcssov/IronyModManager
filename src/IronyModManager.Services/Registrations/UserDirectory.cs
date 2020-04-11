// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-11-2020
// ***********************************************************************
// <copyright file="UserDirectory.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using IronyModManager.Shared;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class UserDirectory.
    /// </summary>
    [ExcludeFromCoverage("Helper setup static class.")]
    public static class UserDirectory
    {
        #region Methods

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetDirectory()
        {
            string rootUserDirectory = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                rootUserDirectory = Path.Combine(userDirectory, PathHelper.MergePaths("Documents", "Paradox Interactive"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                rootUserDirectory = Path.Combine(userDirectory, PathHelper.MergePaths("Paradox Interactive"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                rootUserDirectory = Path.Combine(userDirectory, PathHelper.MergePaths("Paradox Interactive"));
            }
            return rootUserDirectory;
        }

        #endregion Methods
    }
}
