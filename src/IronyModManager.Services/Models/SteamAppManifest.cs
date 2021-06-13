// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-13-2021
//
// Last Modified By : Mario
// Last Modified On : 06-13-2021
// ***********************************************************************
// <copyright file="SteamAppManifest.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IronyModManager.Services.Models
{
    /// <summary>
    /// Class SteamAppManifest.
    /// </summary>
    internal class SteamAppManifest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the install dir.
        /// </summary>
        /// <value>The install dir.</value>
        [JsonProperty("installdir")]
        public string InstallDir { get; set; }

        #endregion Properties
    }
}
