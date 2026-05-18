// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 05-18-2026
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="ModParserArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common.Mod
{
    /// <summary>
    /// Class ModParserArgs.
    /// </summary>
    public class ModParserArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base steam directory.
        /// </summary>
        /// <value>The base steam directory.</value>
        public string BaseSteamDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is proton.
        /// </summary>
        /// <value><c>true</c> if this instance is proton; otherwise, <c>false</c>.</value>
        public bool IsProton { get; set; }

        /// <summary>
        /// Gets or sets the steam application identifier.
        /// </summary>
        /// <value>The steam application identifier.</value>
        public long SteamAppId { get; set; }

        #endregion Properties
    }
}
