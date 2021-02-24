// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-15-2021
// ***********************************************************************
// <copyright file="DLC.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class DLC.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IDLC" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IDLC" />
    public class DLC : BaseModel, IDLC
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public virtual bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public virtual string Path { get; set; }

        #endregion Properties
    }
}
