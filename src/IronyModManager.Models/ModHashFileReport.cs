// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 10-30-2020
// ***********************************************************************
// <copyright file="ModHashFileReport.cs" company="Mario">
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
    /// Class ModHashFileReport.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IModHashFileReport" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IModHashFileReport" />
    public class ModHashFileReport : BaseModel, IModHashFileReport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The hash.</value>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the second hash.
        /// </summary>
        /// <value>The second hash.</value>
        public string SecondHash { get; set; }

        #endregion Properties
    }
}
