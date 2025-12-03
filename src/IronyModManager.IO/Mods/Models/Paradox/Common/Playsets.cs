// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 12-02-2025
//
// Last Modified By : Mario
// Last Modified On : 12-02-2025
// ***********************************************************************
// <copyright file="Playsets.cs" company="Mario">
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
    /// Class Playsets.
    /// Implements the <see cref="IronyModManager.IO.Mods.Models.Paradox.Common.IPdxFormat" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Mods.Models.Paradox.Common.IPdxFormat" />
    internal class Playsets : IPdxFormat
    {
        #region Properties

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        /// <value>The file version.</value>
        [JsonProperty("file_version")]
        public string FileVersion { get; set; }

        /// <summary>
        /// Gets or sets the playsets collection.
        /// </summary>
        /// <value>The playsets collection.</value>
        [JsonProperty("playsets")]
        public List<Playset> PlaysetsCollection { get; set; }

        #endregion Properties

        #region Classes

        /// <summary>
        /// Class DLC.
        /// </summary>
        public class DLC
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this instance is enabled.
            /// </summary>
            /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
            [JsonProperty("isEnabled")]
            public bool IsEnabled { get; set; }

            /// <summary>
            /// Gets or sets the paradox application identifier.
            /// </summary>
            /// <value>The paradox application identifier.</value>
            [JsonProperty("paradoxAppId")]
            public string ParadoxAppId { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class OrderedListMod.
        /// </summary>
        public class OrderedListMod
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this instance is enabled.
            /// </summary>
            /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
            [JsonProperty("isEnabled")]
            public bool IsEnabled { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            [JsonProperty("path")]
            public string Path { get; set; }

            #endregion Properties
        }

        /// <summary>
        /// Class Playset.
        /// </summary>
        public class Playset
        {
            #region Properties

            /// <summary>
            /// Gets or sets the DLC.
            /// </summary>
            /// <value>The DLC.</value>
            [JsonProperty("DLC")]
            public List<DLC> DLCCollection { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is active.
            /// </summary>
            /// <value><c>null</c> if [is active] contains no value, <c>true</c> if [is active]; otherwise, <c>false</c>.</value>
            [JsonProperty("isActive")]
            public bool? IsActive { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is automatically sorted.
            /// </summary>
            /// <value><c>true</c> if this instance is automatically sorted; otherwise, <c>false</c>.</value>
            [JsonProperty("isAutomaticallySorted")]
            public bool IsAutomaticallySorted { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the ordered list mods.
            /// </summary>
            /// <value>The ordered list mods.</value>
            [JsonProperty("orderedListMods")]
            public List<OrderedListMod> OrderedListMods { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
