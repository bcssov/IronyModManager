// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="IDLCObject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared.Models
{
    /// <summary>
    /// Interface IDLCObject
    /// </summary>
    public interface IDLCObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        string AppId { get; set; }

        /// <summary>
        /// Determines if DLC is visible.
        /// </summary>
        /// <value>Is Visible flag.</value>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        string Path { get; set; }

        #endregion Properties
    }
}
