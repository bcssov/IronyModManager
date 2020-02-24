// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 02-24-2020
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
            var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string rootUserDirectory = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                rootUserDirectory = Path.Combine(userDirectory, PathHelper.MergePaths("Documents", "Paradox Interactive"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                rootUserDirectory = Path.Combine(userDirectory, PathHelper.MergePaths(".local", "share", "Paradox Interactive"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                rootUserDirectory = Path.Combine(userDirectory, PathHelper.MergePaths($"Documents", "Paradox Interactive"));
            }
            return rootUserDirectory;
        }

        #endregion Methods
    }
}
