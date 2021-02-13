// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="DLCObject.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.DLC
{
    /// <summary>
    /// Class DLCObject.
    /// Implements the <see cref="IronyModManager.Shared.Models.IDLCObject" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IDLCObject" />
    public class DLCObject : IDLCObject
    {
        #region Properties

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
