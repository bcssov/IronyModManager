// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-24-2020
//
// Last Modified By : Mario
// Last Modified On : 01-24-2020
// ***********************************************************************
// <copyright file="IWindowState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IWindowState
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public interface IWindowState : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        int? Height { get; set; }

        /// <summary>
        /// Gets or sets the is maximized.
        /// </summary>
        /// <value>The is maximized.</value>
        bool? IsMaximized { get; set; }

        /// <summary>
        /// Gets or sets the location x.
        /// </summary>
        /// <value>The location x.</value>
        int? LocationX { get; set; }

        /// <summary>
        /// Gets or sets the location y.
        /// </summary>
        /// <value>The location y.</value>
        int? LocationY { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        int? Width { get; set; }

        #endregion Properties
    }
}
