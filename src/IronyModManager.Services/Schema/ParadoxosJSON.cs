// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-27-2020
//
// Last Modified By : Mario
// Last Modified On : 05-27-2020
// ***********************************************************************
// <copyright file="ParadoxosJSON.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IronyModManager.Services.Schema
{
    /// <summary>
    /// Class ParadoxosJSONExportedList.
    /// </summary>
    internal class ParadoxosJSONExportedList
    {
        #region Properties

        /// <summary>
        /// Gets or sets the exported list.
        /// </summary>
        /// <value>The exported list.</value>
        [JsonProperty("exportedlist")]
        public ParadoxosJSONList ExportedList { get; set; }

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        [JsonProperty("gameID")]
        public int GameID { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ParadoxosJSONList.
    /// </summary>
    internal class ParadoxosJSONList
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [custom order].
        /// </summary>
        /// <value><c>true</c> if [custom order]; otherwise, <c>false</c>.</value>
        [JsonProperty("customOrder")]
        public bool CustomOrder { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty("descr")]
        public string Descr { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [JsonProperty("lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets the launchargs.
        /// </summary>
        /// <value>The launchargs.</value>
        [JsonProperty("launchargs")]
        public string Launchargs { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        [JsonProperty("mod")]
        public IList<ParadoxosJSONMod> Mod { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ParadoxosJSONMod.
    /// </summary>
    internal class ParadoxosJSONMod
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        [JsonProperty("modName")]
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        [JsonProperty("order")]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the remote identifier.
        /// </summary>
        /// <value>The remote identifier.</value>
        [JsonProperty("remoteID")]
        public string RemoteID { get; set; }

        #endregion Properties
    }
}
