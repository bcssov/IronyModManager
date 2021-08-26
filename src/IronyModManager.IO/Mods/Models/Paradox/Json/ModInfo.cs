// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-25-2021
//
// Last Modified By : Mario
// Last Modified On : 08-25-2021
// ***********************************************************************
// <copyright file="ModInfo.cs" company="Mario">
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
    /// Class ModInfo.
    /// </summary>
    internal class ModInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        [JsonProperty("game")]
        public string Game { get; set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        [JsonProperty("mods")]
        public List<Mods> Mods { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion Properties
    }
}
