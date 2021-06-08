// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-29-2021
//
// Last Modified By : Mario
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="IModInstallationResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IModInstallationResult
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IModInstallationResult : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IModInstallationResult" /> is installed.
        /// </summary>
        /// <value><c>true</c> if installed; otherwise, <c>false</c>.</value>
        bool Installed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IModInstallationResult" /> is invalid.
        /// </summary>
        /// <value><c>true</c> if invalid; otherwise, <c>false</c>.</value>
        bool Invalid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is file.
        /// </summary>
        /// <value><c>true</c> if this instance is file; otherwise, <c>false</c>.</value>
        bool IsFile { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        IMod Mod { get; set; }

        /// <summary>
        /// Gets or sets the parent directory.
        /// </summary>
        /// <value>The parent directory.</value>
        string ParentDirectory { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        string Path { get; set; }

        #endregion Properties
    }
}
