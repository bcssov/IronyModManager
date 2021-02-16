// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2021
// ***********************************************************************
// <copyright file="ModCollection.cs" company="Mario">
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
    /// Class ModCollection.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IModCollection" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IModCollection" />
    public class ModCollection : BaseModel, IModCollection
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollection" /> class.
        /// </summary>
        public ModCollection()
        {
            Mods = new List<string>();
            ModNames = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public virtual string Game { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name of the merged folder.
        /// </summary>
        /// <value>The name of the merged folder.</value>
        public virtual string MergedFolderName { get; set; }

        /// <summary>
        /// Gets or sets the mod names.
        /// </summary>
        /// <value>The mod names.</value>
        public virtual IEnumerable<string> ModNames { get; set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public virtual IEnumerable<string> Mods { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [patch mod enabled].
        /// </summary>
        /// <value><c>true</c> if [patch mod enabled]; otherwise, <c>false</c>.</value>
        public bool PatchModEnabled { get; set; } = true;

        #endregion Properties
    }
}
