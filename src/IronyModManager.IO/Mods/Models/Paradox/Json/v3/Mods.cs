// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 11-16-2021
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
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

namespace IronyModManager.IO.Mods.Models.Paradox.Json.v3
{
    /// <summary>
    /// Class Mods.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.v2.Mods" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.v2.Mods" />
    internal class Mods
    {
        #region Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Mods" /> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the PDX identifier.
        /// </summary>
        /// <value>The PDX identifier.</value>
        [JsonProperty("pdxId")]
        public string PdxId { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        [JsonProperty("position")]
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the steam identifier.
        /// </summary>
        /// <value>The steam identifier.</value>
        [JsonProperty("steamId")]
        public string SteamId { get; set; }

        #endregion Properties
    }
}
