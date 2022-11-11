// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="DLCParameters.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.Models;

namespace IronyModManager.IO.Common.DLC
{
    /// <summary>
    /// Class DLCParameters.
    /// </summary>
    public class DLCParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type of the descriptor.
        /// </summary>
        /// <value>The type of the descriptor.</value>
        public DescriptorType DescriptorType { get; set; }

        /// <summary>
        /// Gets or sets the DLC.
        /// </summary>
        /// <value>The DLC.</value>
        public IEnumerable<IDLCObject> DLC { get; set; }

        /// <summary>
        /// Gets or sets the root path.
        /// </summary>
        /// <value>The root path.</value>
        public string RootPath { get; set; }

        #endregion Properties
    }
}
