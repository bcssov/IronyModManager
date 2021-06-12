// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-12-2021
//
// Last Modified By : Mario
// Last Modified On : 06-12-2021
// ***********************************************************************
// <copyright file="SteamLibraryFolderData.cs" company="Mario">
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
    /// Class SteamLibraryFolderData.
    /// </summary>
    internal class SteamLibraryFolderData
    {
        #region Properties

        /// <summary>
        /// Gets or sets the content identifier.
        /// </summary>
        /// <value>The content identifier.</value>
        [JsonProperty("contentstatsid")]
        public string ContentId { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the mounted.
        /// </summary>
        /// <value>The mounted.</value>
        [JsonProperty("mounted")]
        public int Mounted { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        [JsonProperty("path")]
        public string Path { get; set; }

        #endregion Properties
    }
}
