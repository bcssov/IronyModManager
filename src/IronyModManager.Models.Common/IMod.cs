// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 06-16-2020
// ***********************************************************************
// <copyright file="IMod.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Localization;
using IronyModManager.Parser.Common.Mod;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IMod
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModObject" />
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModObject" />
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

        #endregion Properties
    }
}
