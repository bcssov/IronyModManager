// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 08-12-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2020
// ***********************************************************************
// <copyright file="LauncherSettings.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IronyModManager.Services.Registrations.Models
{
    /// <summary>
    /// Class LauncherSettings.
    /// </summary>
    internal class LauncherSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets the browser DLC URL.
        /// </summary>
        /// <value>The browser DLC URL.</value>
        [JsonProperty("browserDlcUrl")]
        public string BrowserDlcUrl { get; set; }

        /// <summary>
        /// Gets or sets the dist platform.
        /// </summary>
        /// <value>The dist platform.</value>
        [JsonProperty("distPlatform")]
        public string DistPlatform { get; set; }

        /// <summary>
        /// Gets or sets the executable arguments.
        /// </summary>
        /// <value>The executable arguments.</value>
        [JsonProperty("exeArgs")]
        public IList<string> ExeArgs { get; set; }

        /// <summary>
        /// Gets or sets the executable path.
        /// </summary>
        /// <value>The executable path.</value>
        [JsonProperty("exePath")]
        public string ExePath { get; set; }

        /// <summary>
        /// Gets or sets the game data path.
        /// </summary>
        /// <value>The game data path.</value>
        [JsonProperty("gameDataPath")]
        public string GameDataPath { get; set; }

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        [JsonProperty("gameId")]
        public string GameId { get; set; }

        /// <summary>
        /// Gets or sets the ingame settings layout path.
        /// </summary>
        /// <value>The ingame settings layout path.</value>
        [JsonProperty("ingameSettingsLayoutPath")]
        public string IngameSettingsLayoutPath { get; set; }

        /// <summary>
        /// Gets or sets the raw version.
        /// </summary>
        /// <value>The raw version.</value>
        [JsonProperty("rawVersion")]
        public string RawVersion { get; set; }

        /// <summary>
        /// Gets or sets the theme file.
        /// </summary>
        /// <value>The theme file.</value>
        [JsonProperty("themeFile")]
        public string ThemeFile { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [JsonProperty("version")]
        public string Version { get; set; }

        #endregion Properties
    }
}
