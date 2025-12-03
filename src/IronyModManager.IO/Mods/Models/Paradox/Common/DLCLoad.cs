// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 03-14-2020
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="DLCLoad.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models.Paradox.Common
{
    /// <summary>
    /// Class DLCLoad.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.Common.IPdxFormat" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.Common.IPdxFormat" />
    [ExcludeFromCoverage("Skipping testing IO logic.")]
    internal class DLCLoad : IPdxFormat
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCLoad" /> class.
        /// </summary>
        public DLCLoad()
        {
            DisabledDLCs = [];
            EnabledMods = [];
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the disabled DLCs.
        /// </summary>
        /// <value>The disabled DLCs.</value>
        // ReSharper disable once StringLiteralTypo
        [JsonProperty("disabled_dlcs")]

        // ReSharper disable InconsistentNaming
        public List<string> DisabledDLCs { get; set; }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets or sets the enabled mods.
        /// </summary>
        /// <value>The enabled mods.</value>
        [JsonProperty("enabled_mods")]
        public List<string> EnabledMods { get; set; }

        #endregion Properties
    }
}
