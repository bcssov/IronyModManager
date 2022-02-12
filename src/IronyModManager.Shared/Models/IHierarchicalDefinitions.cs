// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 03-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-02-2022
// ***********************************************************************
// <copyright file="IHierarchicalDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using CodexMicroORM.Core.Collections;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Interface IHierarchicalDefinitions
    /// Implements the <see cref="CodexMicroORM.Core.Collections.ICEFIndexedListItem" />
    /// Implements the <see cref="IronyModManager.Shared.Models.IQueryableModel" />
    /// </summary>
    /// <seealso cref="CodexMicroORM.Core.Collections.ICEFIndexedListItem" />
    /// <seealso cref="IronyModManager.Shared.Models.IQueryableModel" />
    public interface IHierarchicalDefinitions : ICEFIndexedListItem, IQueryableModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        /// <value>The additional data.</value>
        object AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        ICollection<IHierarchicalDefinitions> Children { get; set; }

        /// <summary>
        /// Gets or sets the file names.
        /// </summary>
        /// <value>The file names.</value>
        IList<string> FileNames { get; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        IList<string> Mods { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the non game definitions.
        /// </summary>
        /// <value>The non game definitions.</value>
        int NonGameDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the type of the reset.
        /// </summary>
        /// <value>The type of the reset.</value>
        ResetType ResetType { get; set; }

        #endregion Properties
    }
}
