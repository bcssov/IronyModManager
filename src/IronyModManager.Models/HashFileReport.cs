// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="HashFileReport.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class HashFileReport.
    /// Implements the <see cref="IronyModManager.Models.Common.BaseModel" />
    /// Implements the <see cref="IronyModManager.Models.Common.IHashFileReport" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.BaseModel" />
    /// <seealso cref="IronyModManager.Models.Common.IHashFileReport" />
    public class HashFileReport : BaseModel, IHashFileReport
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public virtual string File { get; set; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>The hash.</value>
        public virtual string Hash { get; set; }

        /// <summary>
        /// Gets or sets the second hash.
        /// </summary>
        /// <value>The second hash.</value>
        public virtual string SecondHash { get; set; }

        #endregion Properties
    }
}
