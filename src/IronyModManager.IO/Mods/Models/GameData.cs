// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-14-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="GameData.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models
{
    /// <summary>
    /// Class GameData.
    /// Implements the <see cref="IronyModManager.IO.Models.IPdxFormat" />
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.IPdxFormat" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.IPdxFormat" />
    /// <seealso cref="IronyModManager.IO.Models.IPdxFormat" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class GameData : IPdxFormat
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameData" /> class.
        /// </summary>
        public GameData()
        {
            ModsOrder = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is EULA accepted.
        /// </summary>
        /// <value><c>true</c> if this instance is EULA accepted; otherwise, <c>false</c>.</value>
        [JsonProperty("isEulaAccepted")]
        public bool IsEulaAccepted { get; set; }

        /// <summary>
        /// Gets or sets the mods order.
        /// </summary>
        /// <value>The mods order.</value>
        [JsonProperty("modsOrder")]
        public List<string> ModsOrder { get; set; }

        #endregion Properties
    }
}
