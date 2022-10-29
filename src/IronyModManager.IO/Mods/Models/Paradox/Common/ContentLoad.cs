// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 10-29-2022
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
// ***********************************************************************
// <copyright file="ContentLoad.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models.Paradox.Common
{
    /// <summary>
    /// Class ContentLoad.
    /// </summary>
    public class ContentLoad : IPdxFormat
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoad"/> class.
        /// </summary>
        public ContentLoad()
        {
            DisabledDLC = new List<DisabledDLC>();
            EnabledMods = new List<EnabledMod>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the disabled DLC.
        /// </summary>
        /// <value>The disabled DLC.</value>
        [JsonProperty("disabledDLC")]
        public List<DisabledDLC> DisabledDLC { get; set; }

        /// <summary>
        /// Gets or sets the enabled mods.
        /// </summary>
        /// <value>The enabled mods.</value>
        [JsonProperty("enabledMods")]
        public List<EnabledMod> EnabledMods { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class DisabledDLC.
    /// </summary>
    public class DisabledDLC
    {
        #region Properties

        /// <summary>
        /// Gets or sets the paradox application identifier.
        /// </summary>
        /// <value>The paradox application identifier.</value>
        [JsonProperty("paradoxAppId")]
        public string ParadoxAppId { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class EnabledMod.
    /// </summary>
    public class EnabledMod
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [JsonProperty("path")]
        public string Path { get; set; }

        #endregion Properties
    }
}
