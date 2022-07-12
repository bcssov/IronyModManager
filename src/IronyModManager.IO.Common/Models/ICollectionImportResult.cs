// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 08-29-2021
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="ICollectionImportResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.IO.Common.Models
{
    /// <summary>
    /// Interface ICollectionImportResult
    /// </summary>
    public interface ICollectionImportResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the descriptors.
        /// </summary>
        /// <value>The descriptors.</value>
        IEnumerable<string> Descriptors { get; set; }

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        string Game { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name of the merged folder.
        /// </summary>
        /// <value>The name of the merged folder.</value>
        string MergedFolderName { get; set; }

        /// <summary>
        /// Gets or sets the mod ids.
        /// </summary>
        /// <value>The mod ids.</value>
        IEnumerable<IModCollectionSourceInfo> ModIds { get; set; }

        /// <summary>
        /// Gets or sets the mod names.
        /// </summary>
        /// <value>The mod names.</value>
        IEnumerable<string> ModNames { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [patch mod enabled].
        /// </summary>
        /// <value><c>true</c> if [patch mod enabled]; otherwise, <c>false</c>.</value>
        bool PatchModEnabled { get; set; }

        #endregion Properties
    }
}
