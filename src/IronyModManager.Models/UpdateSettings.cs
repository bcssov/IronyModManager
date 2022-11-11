// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 11-02-2022
// ***********************************************************************
// <copyright file="UpdateSettings.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class UpdateSettings.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IUpdateSettings" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IUpdateSettings" />
    public class UpdateSettings : BaseModel, IUpdateSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [automatic updates].
        /// </summary>
        /// <value><c>null</c> if [automatic updates] contains no value, <c>true</c> if [automatic updates]; otherwise, <c>false</c>.</value>
        public virtual bool? AutoUpdates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [check for prerelease].
        /// </summary>
        /// <value><c>true</c> if [check for prerelease]; otherwise, <c>false</c>.</value>
        public virtual bool CheckForPrerelease { get; set; }

        /// <summary>
        /// Gets or sets the last skipped version.
        /// </summary>
        /// <value>The last skipped version.</value>
        public virtual string LastSkippedVersion { get; set; }

        #endregion Properties
    }
}
