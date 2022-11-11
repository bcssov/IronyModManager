// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 07-18-2022
//
// Last Modified By : Mario
// Last Modified On : 11-09-2022
// ***********************************************************************
// <copyright file="EncodingInfo.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class EncodingInfo.
    /// </summary>
    public class EncodingInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has bom.
        /// </summary>
        /// <value><c>true</c> if this instance has bom; otherwise, <c>false</c>.</value>
        public bool HasBOM { get; set; }

        #endregion Properties
    }
}
