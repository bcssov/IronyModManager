// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-17-2021
//
// Last Modified By : Mario
// Last Modified On : 03-26-2022
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
        #region Fields

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DriveInfoProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DriveInfoProvider(ILogger logger)
        {
            this.logger = logger;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the free space.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Nullable&lt;System.Int64&gt;.</returns>
        public long? GetFreeSpace(string path)
        {
            try
            {
                // I remember this having problems in earlier versions of .NET or is my memory failing me...
                var info = new DriveInfo(path);
                if (info != null)
                {
                    return info.AvailableFreeSpace;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
