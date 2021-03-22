// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 03-17-2021
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="IDriveInfoProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Interface IDriveInfoProvider
    /// </summary>
    public interface IDriveInfoProvider
    {
        #region Methods

        /// <summary>
        /// Gets the free space.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int64.</returns>
        long? GetFreeSpace(string path);

        /// <summary>
        /// Determines whether [has free space] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="requiredSize">Size of the required.</param>
        /// <returns><c>true</c> if [has free space] [the specified path]; otherwise, <c>false</c>.</returns>
        bool HasFreeSpace(string path, long requiredSize);

        #endregion Methods
    }
}
