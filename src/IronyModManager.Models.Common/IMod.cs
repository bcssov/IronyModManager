// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="IMod.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using IronyModManager.Localization;
using IronyModManager.Shared.Models;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IMod
    /// Implements the <see cref="IronyModManager.Shared.Models.IModObject" />
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IModObject" />
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    public interface IMod : IModObject, ILocalizableModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the achievement status.
        /// </summary>
        /// <value>The achievement status.</value>
        AchievementStatus AchievementStatus { get; set; }

        /// <summary>
        /// Gets or sets the descriptor file.
        /// </summary>
        /// <value>The descriptor file.</value>
        string DescriptorFile { get; set; }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>The files.</value>
        IEnumerable<string> Files { get; set; }

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        /// <value>The full path.</value>
        string FullPath { get; set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        string Game { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        int Order { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        ModSource Source { get; set; }

        /// <summary>
        /// Gets or sets the version data.
        /// </summary>
        /// <value>The version data.</value>
        Version VersionData { get; }

        #endregion Properties
    }
}
