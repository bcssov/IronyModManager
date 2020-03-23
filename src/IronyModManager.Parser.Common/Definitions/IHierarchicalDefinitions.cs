// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 03-23-2020
//
// Last Modified By : Mario
// Last Modified On : 03-23-2020
// ***********************************************************************
// <copyright file="IHierarchicalDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Definitions
{
    /// <summary>
    /// Interface IHierarchicalDefinitions
    /// </summary>
    public interface IHierarchicalDefinitions
    {
        #region Properties

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IList<IHierarchicalDefinitions> Children { get; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        #endregion Properties
    }
}
