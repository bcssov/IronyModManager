// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-17-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="UpdateSettings.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Implementation.Updater
{
    /// <summary>
    /// Class UpdateSettings.
    /// </summary>
    public class UpdateSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is installer.
        /// </summary>
        /// <value><c>true</c> if this instance is installer; otherwise, <c>false</c>.</value>
        public bool IsInstaller { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UpdateSettings" /> is updated.
        /// </summary>
        /// <value><c>true</c> if updated; otherwise, <c>false</c>.</value>
        public bool Updated { get; set; }

        #endregion Properties
    }
}
