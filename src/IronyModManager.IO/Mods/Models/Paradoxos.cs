// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 05-28-2020
//
// Last Modified By : Mario
// Last Modified On : 05-28-2020
// ***********************************************************************
// <copyright file="Paradoxos.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Public because well... XML serializer.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace IronyModManager.IO.Mods.Models
{
    /// <summary>
    /// Class ParadoxosExportedList.
    /// </summary>
    [Serializable, XmlRoot("exportedlist")]
    public class ParadoxosExportedList
    {
        #region Properties

        /// <summary>
        /// Gets or sets the exported list.
        /// </summary>
        /// <value>The exported list.</value>
        [JsonProperty("exportedlist")]
        [XmlElement(ElementName = "list")]
        public ParadoxosList ExportedList { get; set; }

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        [JsonProperty("gameID")]
        [XmlAttribute(AttributeName = "gameID")]
        public int GameID { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ParadoxosList.
    /// </summary>
    public class ParadoxosList
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [custom order].
        /// </summary>
        /// <value><c>true</c> if [custom order]; otherwise, <c>false</c>.</value>
        [JsonProperty("customOrder")]
        [XmlAttribute(AttributeName = "customOrder")]
        public bool CustomOrder { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty("descr")]
        [XmlElement(ElementName = "descr")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [JsonProperty("lang")]
        [XmlElement(ElementName = "lang")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the launch arguments.
        /// </summary>
        /// <value>The launch arguments.</value>
        [JsonProperty("launchargs")]
        [XmlElement(ElementName = "launchargs")]
        public string LaunchArgs { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        [JsonProperty("mod")]
        [XmlElement(ElementName = "mod")]
        public List<ParadoxosMod> Mod { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ParadoxosMod.
    /// </summary>
    public class ParadoxosMod
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [JsonProperty("fileName")]
        [XmlAttribute(AttributeName = "fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        [JsonProperty("modName")]
        [XmlAttribute(AttributeName = "modName")]
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        [JsonProperty("order")]
        [XmlAttribute(AttributeName = "order")]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the remote identifier.
        /// </summary>
        /// <value>The remote identifier.</value>
        [JsonProperty("remoteID")]
        [XmlAttribute(AttributeName = "remoteID")]
        public string RemoteID { get; set; }

        #endregion Properties
    }
}
