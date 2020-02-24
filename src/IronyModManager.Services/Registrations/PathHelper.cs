// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 02-24-2020
// ***********************************************************************
// <copyright file="PathHelper.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using IronyModManager.Shared;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class PathHelper.
    /// </summary>
    [ExcludeFromCoverage("Helper setup static class.")]
    public static class PathHelper
    {
        #region Methods

        /// <summary>
        /// Merges the paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>System.String.</returns>
        public static string MergePaths(params string[] paths)
        {
            return string.Join(Path.DirectorySeparatorChar, paths);
        }

        #endregion Methods
    }
}
