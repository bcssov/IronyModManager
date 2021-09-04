// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-29-2021
//
// Last Modified By : Mario
// Last Modified On : 08-29-2021
// ***********************************************************************
// <copyright file="CollectionImportResult.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.IO.Common.Models;

namespace IronyModManager.IO.Models
{
    /// <summary>
    /// Class CollectionImportResult.
    /// Implements the <see cref="IronyModManager.IO.Common.Models.ICollectionImportResult" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Models.ICollectionImportResult" />
    public class CollectionImportResult : ICollectionImportResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the descriptors.
        /// </summary>
        /// <value>The descriptors.</value>
        public virtual IEnumerable<string> Descriptors { get; set; }

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        public virtual string Game { get; set; }

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        public virtual string GameId { get; set; }

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
        /// Gets or sets the ids.
        /// </summary>
        /// <value>The ids.</value>
        public virtual IEnumerable<string> ModIds { get; set; }

        /// <summary>
        /// Gets or sets the mod names.
        /// </summary>
        /// <value>The mod names.</value>
        public virtual IEnumerable<string> ModNames { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [patch mod enabled].
        /// </summary>
        /// <value><c>true</c> if [patch mod enabled]; otherwise, <c>false</c>.</value>
        public virtual bool PatchModEnabled { get; set; }

        #endregion Properties
    }
}
