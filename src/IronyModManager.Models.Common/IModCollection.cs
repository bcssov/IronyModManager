// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2021
// ***********************************************************************
// <copyright file="IModCollection.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IModCollection Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IModCollection : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
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
        /// Gets or sets the mod names.
        /// </summary>
        /// <value>The mod names.</value>
        IEnumerable<string> ModNames { get; set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        IEnumerable<string> Mods { get; set; }

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
