// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-24-2020
//
// Last Modified By : Mario
// Last Modified On : 02-08-2020
// ***********************************************************************
// <copyright file="WindowState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class WindowState.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IWindowState" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IWindowState" />
    public class WindowState : BaseModel, IWindowState
    {
        #region Properties

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public virtual int? Height { get; set; }

        /// <summary>
        /// Gets or sets the is maximized.
        /// </summary>
        /// <value>The is maximized.</value>
        public virtual bool? IsMaximized { get; set; }

        /// <summary>
        /// Gets or sets the location x.
        /// </summary>
        /// <value>The location x.</value>
        public virtual int? LocationX { get; set; }

        /// <summary>
        /// Gets or sets the location y.
        /// </summary>
        /// <value>The location y.</value>
        public virtual int? LocationY { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public virtual int? Width { get; set; }

        #endregion Properties
    }
}
