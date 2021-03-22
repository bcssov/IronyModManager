// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-17-2021
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="DriveInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.IO.Common;
using IronyModManager.Shared;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class DriveInfoProvider.
    /// Implements the <see cref="IronyModManager.IO.Common.IDriveInfoProvider" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.IDriveInfoProvider" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    public class DriveInfoProvider : IDriveInfoProvider
    {
        #region Methods

        /// <summary>
        /// Gets the free space.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        public long? GetFreeSpace(string path)
        {
            var root = Path.GetPathRoot(path);
            var drive = DriveInfo.GetDrives().FirstOrDefault(p => p.Name.Equals(root, StringComparison.OrdinalIgnoreCase));
            if (drive != null)
            {
                return drive.AvailableFreeSpace;
            }
            return null;
        }

        /// <summary>
        /// Determines whether [has free space] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="requiredSize">Size of the required.</param>
        /// <returns><c>true</c> if [has free space] [the specified path]; otherwise, <c>false</c>.</returns>
        public bool HasFreeSpace(string path, long requiredSize)
        {
            var space = GetFreeSpace(path);
            return space.HasValue && space.GetValueOrDefault() > requiredSize;
        }

        #endregion Methods
    }
}
