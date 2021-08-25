// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-25-2021
//
// Last Modified By : Mario
// Last Modified On : 08-25-2021
// ***********************************************************************
// <copyright file="Mods.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models.Paradox.Json
{
    /// <summary>
    /// Class Mods.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.v2.Mods" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.v2.Mods" />
    internal class Mods : IO.Mods.Models.Paradox.v2.Mods
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Mods"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [JsonProperty("position")]
        public string Position { get; set; }

        #endregion Properties
    }
}
