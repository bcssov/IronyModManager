// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-27-2020
//
// Last Modified By : Mario
// Last Modified On : 05-27-2020
// ***********************************************************************
// <copyright file="ParadoxosXML.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IronyModManager.Services.Schema
{
    /// <summary>
    /// Class ParadoxosXMLExportedlist.
    /// </summary>
    [XmlRoot(ElementName = "exportedlist")]
    internal class ParadoxosXMLExportedlist
    {
        #region Properties

        /// <summary>
        /// Gets or sets the game identifier.
        /// </summary>
        /// <value>The game identifier.</value>
        [XmlAttribute(AttributeName = "gameID")]
        public string GameID { get; set; }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        [XmlElement(ElementName = "list")]
        public ParadoxosXMLList List { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ParadoxosXMLList.
    /// </summary>
    [XmlRoot(ElementName = "list")]
    internal class ParadoxosXMLList
    {
        #region Properties

        /// <summary>
        /// Gets or sets the custom order.
        /// </summary>
        /// <value>The custom order.</value>
        [XmlAttribute(AttributeName = "customOrder")]
        public string CustomOrder { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(ElementName = "descr")]
        public string Descr { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [XmlElement(ElementName = "lang")]
        public string Lang { get; set; }

        /// <summary>
        /// Gets or sets the launchargs.
        /// </summary>
        /// <value>The launchargs.</value>
        [XmlElement(ElementName = "launchargs")]
        public string Launchargs { get; set; }

        /// <summary>
        /// Gets or sets the mod.
        /// </summary>
        /// <value>The mod.</value>
        [XmlElement(ElementName = "mod")]
        public List<ParadoxosXMLMod> Mod { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Class ParadoxosXMLMod.
    /// </summary>
    [XmlRoot(ElementName = "mod")]
    internal class ParadoxosXMLMod
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlAttribute(AttributeName = "fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        [XmlAttribute(AttributeName = "modName")]
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        [XmlAttribute(AttributeName = "order")]
        public string Order { get; set; }

        /// <summary>
        /// Gets or sets the remote identifier.
        /// </summary>
        /// <value>The remote identifier.</value>
        [XmlAttribute(AttributeName = "remoteID")]
        public string RemoteID { get; set; }

        #endregion Properties
    }
}
