// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-14-2020
//
// Last Modified By : Mario
// Last Modified On : 04-01-2020
// ***********************************************************************
// <copyright file="DLCLoad.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models
{
    /// <summary>
    /// Class DLCLoad.
    /// Implements the <see cref="IronyModManager.IO.Models.IPdxFormat" />
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.IPdxFormat" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.IPdxFormat" />
    /// <seealso cref="IronyModManager.IO.Models.IPdxFormat" />
    internal class DLCLoad : IPdxFormat
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCLoad" /> class.
        /// </summary>
        public DLCLoad()
        {
            DisabledDlcs = new List<string>();
            EnabledMods = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the disabled DLCS.
        /// </summary>
        /// <value>The disabled DLCS.</value>
        [JsonProperty("disabled_dlcs")]
        public List<string> DisabledDlcs { get; set; }

        /// <summary>
        /// Gets or sets the enabled mods.
        /// </summary>
        /// <value>The enabled mods.</value>
        [JsonProperty("enabled_mods")]
        public List<string> EnabledMods { get; set; }

        #endregion Properties
    }
}
